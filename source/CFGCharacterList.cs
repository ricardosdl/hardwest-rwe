using System.Collections.Generic;
using UnityEngine;

public static class CFGCharacterList
{
	public const int TEAM_CHARACTERS = 4;

	private static List<CFGCharacterData> m_Characters = new List<CFGCharacterData>();

	private static List<CFGCharacterData> m_TeamCharacters = new List<CFGCharacterData>();

	private static int m_CurrentTeam = 0;

	private static bool m_TeamInitialized = false;

	public static int CurrentTeamID => m_CurrentTeam;

	public static void SetActiveTeam(int TeamID)
	{
		if (TeamID < 0 || TeamID > 1)
		{
			Debug.LogError("Team ID must be 0 or 1");
			return;
		}
		m_CurrentTeam = TeamID;
		Debug.Log("Activating team " + m_CurrentTeam);
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			m_TeamCharacters[i].PositionInTeam = i;
		}
		BuildCurrentTeam();
	}

	public static void OnDifficultyLevelChanged(EDifficulty OldDiffLevel, EDifficulty NewLevel)
	{
		foreach (CFGCharacterData character in m_Characters)
		{
			character?.OnDifficultyChanged(NewLevel);
		}
	}

	public static CFGCharacterData GetCharacterData(string CharacterID)
	{
		int characterIndex = GetCharacterIndex(CharacterID);
		if (characterIndex == -1)
		{
			return null;
		}
		return m_Characters[characterIndex];
	}

	private static void ClearCharacters()
	{
		Debug.Log("Clear chars");
		m_Characters.Clear();
		m_TeamCharacters.Clear();
	}

	public static void OnCampaignStart()
	{
		ClearCharacters();
	}

	public static void OnCampaignEnd()
	{
		ClearCharacters();
	}

	public static void OnScenarioStart()
	{
		ClearCharacters();
	}

	public static void OnScenarioEnd()
	{
		ClearCharacters();
	}

	public static void OnStrategicStart()
	{
	}

	public static void OnStrategicEnter()
	{
	}

	public static void OnStrategicExit()
	{
	}

	public static void OnStrategicFinish()
	{
	}

	public static void OnTacticalStart()
	{
		m_TeamInitialized = false;
		foreach (CFGCharacterData character in m_Characters)
		{
			if (character.GunpointState != 0)
			{
				character.CurrentModel.HACKPlayGunpointAnimation();
			}
			if (!CFG_SG_Manager.IsLoading || CFG_SG_Manager.SGSource != CFG_SG_SaveGame.eSG_Source.QuickSave)
			{
				character.SetState(ECharacterStateFlag.ExistedAtTacticalStart, Value: true);
				character.Hp = character.MaxHp;
			}
		}
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			if (m_TeamCharacters[i] != null && m_TeamCharacters[i].IsAlive)
			{
				m_TeamInitialized = true;
				break;
			}
		}
	}

	public static void OnTacticalRestart()
	{
		int num = 0;
		while (num < m_Characters.Count)
		{
			if (m_Characters[num] == null || m_Characters[num].IsStateSet(ECharacterStateFlag.TempTactical) || !m_Characters[num].IsStateSet(ECharacterStateFlag.ExistedAtTacticalStart))
			{
				RemoveFromTeam(m_Characters[num].Definition.NameID, bRemoveEquipment: true);
				m_Characters.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public static void OnTacticalEnd()
	{
		if (m_Characters == null || m_Characters.Count == 0)
		{
			return;
		}
		int num = 0;
		while (num < m_Characters.Count)
		{
			if (m_Characters[num] == null || m_Characters[num].IsStateSet(ECharacterStateFlag.TempTactical))
			{
				m_Characters.RemoveAt(num);
				continue;
			}
			if (m_Characters[num].IsAlive)
			{
				m_Characters[num].OnEndTactical();
				m_Characters[num].Hp = m_Characters[num].MaxHp;
				m_Characters[num].SetLuck(m_Characters[num].MaxLuck, bAllowSplash: false);
				if (m_Characters[num].UnlockedCardSlots < 5)
				{
					m_Characters[num].UnlockedCardSlots++;
				}
			}
			if (string.Compare(m_Characters[num].Weapon1, "revolver_coltarmyrusty", ignoreCase: true) == 0 && string.IsNullOrEmpty(m_Characters[num].Weapon2))
			{
				m_Characters[num].EquipItem(EItemSlot.Weapon1, null);
			}
			else if (string.Compare(m_Characters[num].Weapon2, "revolver_coltarmyrusty", ignoreCase: true) == 0 && string.IsNullOrEmpty(m_Characters[num].Weapon1))
			{
				m_Characters[num].EquipItem(EItemSlot.Weapon2, null);
			}
			num++;
		}
	}

	public static void RemoveTempChars()
	{
		if (m_Characters == null || m_Characters.Count == 0)
		{
			return;
		}
		int num = 0;
		while (num < m_Characters.Count)
		{
			if (m_Characters[num] == null || m_Characters[num].IsStateSet(ECharacterStateFlag.TempTactical))
			{
				m_Characters.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public static void MoveCharToTeamTop(string CharacterID)
	{
		int positionInTeam = GetPositionInTeam(CharacterID);
		if (positionInTeam >= 1)
		{
			CFGCharacterData cFGCharacterData = m_TeamCharacters[positionInTeam];
			for (int i = 0; i < positionInTeam; i++)
			{
				m_TeamCharacters[i].PositionInTeam++;
			}
			m_TeamCharacters.RemoveAt(positionInTeam);
			m_TeamCharacters.Insert(0, cFGCharacterData);
			cFGCharacterData.PositionInTeam = 0;
		}
	}

	public static bool RemoveFromTeam(string szName, bool bRemoveEquipment, bool bUsePopup = true)
	{
		int positionInTeam = GetPositionInTeam(szName);
		return RemoveFromTeam(positionInTeam, bRemoveEquipment, bUsePopup);
	}

	public static bool RemoveFromTeam(int Position, bool bRemoveEquipment, bool bUsePopup = true)
	{
		if (m_TeamCharacters == null || Position < 0 || Position >= m_TeamCharacters.Count || m_TeamCharacters[Position] == null)
		{
			return false;
		}
		if (bRemoveEquipment)
		{
			m_TeamCharacters[Position].RemoveEquipment();
		}
		else
		{
			for (int i = 0; i < 5; i++)
			{
				CFGDef_Card card = m_TeamCharacters[Position].GetCard(i);
				if (card != null)
				{
					CFGInventory.MarkCardAsNotCollected(card.CardID);
				}
			}
		}
		if (bUsePopup && CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
		{
			string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_character_fired") + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_TeamCharacters[Position].Definition.NameID);
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 1, m_TeamCharacters[Position].ImageIDX);
		}
		m_TeamCharacters[Position].PositionInTeam = -1;
		m_TeamCharacters.RemoveAt(Position);
		if (m_TeamCharacters.Count > 0)
		{
			for (int j = 0; j < m_TeamCharacters.Count; j++)
			{
				m_TeamCharacters[j].PositionInTeam = j;
			}
		}
		return true;
	}

	public static bool CheckIfTeamLostAllCharacters()
	{
		if (!m_TeamInitialized)
		{
			return false;
		}
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			if (m_TeamCharacters[i] != null && !m_TeamCharacters[i].IsDead)
			{
				return false;
			}
		}
		return true;
	}

	public static List<string> GetTeamMembersAsIDs()
	{
		List<string> list = new List<string>();
		if (m_TeamCharacters != null)
		{
			for (int i = 0; i < m_TeamCharacters.Count; i++)
			{
				if (m_TeamCharacters[i] != null && m_TeamCharacters[i].Definition != null)
				{
					list.Add(m_TeamCharacters[i].Definition.NameID);
				}
			}
		}
		return list;
	}

	public static int CharacterOrderInTeam(string CharID)
	{
		if (m_TeamCharacters == null)
		{
			return -1;
		}
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			if (m_TeamCharacters[i] != null && m_TeamCharacters[i].Definition != null && string.Compare(CharID, m_TeamCharacters[i].Definition.NameID, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool AssignToTeamAtPosition(CFGCharacterData Char, bool bHidePopup = false)
	{
		if (Char == null || Char.Definition == null)
		{
			return false;
		}
		string nameID = Char.Definition.NameID;
		if (string.IsNullOrEmpty(nameID))
		{
			Debug.LogError("Failed to assing to team: Empty name");
			return false;
		}
		int num = CharacterOrderInTeam(nameID);
		if (num != -1)
		{
			return true;
		}
		if (Char.IsStateSet(ECharacterStateFlag.TempTactical))
		{
			Debug.LogWarning("Character " + nameID + " is temporary and cannot be assigned to the team");
			return false;
		}
		if (Char.IsDead)
		{
			Debug.LogWarning("Character " + nameID + " is dead and in this state has problems with limbs");
			return false;
		}
		Char.PositionInTeam = m_TeamCharacters.Count;
		Char.TeamID = m_CurrentTeam;
		m_TeamCharacters.Add(Char);
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && !bHidePopup)
		{
			string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_character_hired") + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(Char.Definition.NameID);
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 1, Char.ImageIDX);
		}
		Char.AddCardsFromAbilities();
		m_TeamInitialized = true;
		return true;
	}

	public static bool AssignToTeamAtPosition(string CharID, bool bHidePopup = false)
	{
		int characterIndex = GetCharacterIndex(CharID);
		if (characterIndex == -1)
		{
			Debug.LogWarning("Could not find character " + CharID + " on the available character list");
			return false;
		}
		return AssignToTeamAtPosition(m_Characters[characterIndex], bHidePopup);
	}

	public static int GetPositionInTeam(string CharID)
	{
		if (m_TeamCharacters == null)
		{
			return -1;
		}
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			if (m_TeamCharacters[i] != null && m_TeamCharacters[i].Definition != null && string.Compare(m_TeamCharacters[i].Definition.NameID, CharID, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool AssignToTeam(string CharID, bool bHidePopup = false)
	{
		if (GetPositionInTeam(CharID) > -1)
		{
			return true;
		}
		if (m_TeamCharacters.Count < 4)
		{
			return AssignToTeamAtPosition(CharID, bHidePopup);
		}
		Debug.LogWarning("Cannot assign " + CharID + " to team: NO FREE SLOTS [MAX IS " + 4 + "]");
		return false;
	}

	public static bool AssignToTeamDebug(CFGCharacterData Char)
	{
		if (Char == null || Char.Definition == null)
		{
			return false;
		}
		if (Char.PositionInTeam > -1)
		{
			return true;
		}
		if (Char.IsStateSet(ECharacterStateFlag.TempTactical))
		{
			Char.SetState(ECharacterStateFlag.TempTactical, Value: false);
		}
		return AssignToTeamAtPosition(Char);
	}

	public static bool AssignToTeam(CFGCharacterData Char)
	{
		if (Char == null || Char.Definition == null)
		{
			return false;
		}
		if (Char.PositionInTeam > -1)
		{
			return true;
		}
		return AssignToTeamAtPosition(Char);
	}

	public static void ApplyBuff(CFGCharacterData chd, string BuffName, EBuffSource Source)
	{
		chd?.AddBuff(BuffName, Source);
	}

	public static void ApplyBuff(string CharacterID, string BuffName, EBuffSource Source)
	{
		CFGCharacterData characterData = GetCharacterData(CharacterID);
		ApplyBuff(characterData, BuffName, Source);
	}

	public static void RemoveBuff(CFGCharacterData chd, string BuffName)
	{
		chd?.RemBuff(BuffName);
	}

	public static void RemoveBuff(string CharacterID, string BuffName)
	{
		CFGCharacterData characterData = GetCharacterData(CharacterID);
		RemoveBuff(characterData, BuffName);
	}

	public static int GetCharacterCount()
	{
		return m_Characters.Count;
	}

	public static CFGCharacterData GetCharacterData(int Index)
	{
		if (Index < 0 || Index >= m_Characters.Count)
		{
			return null;
		}
		return m_Characters[Index];
	}

	public static int GetCharacterIndex(string CharID, int FirstID = 0)
	{
		if (FirstID >= m_Characters.Count)
		{
			return -1;
		}
		if (string.IsNullOrEmpty(CharID))
		{
			return -1;
		}
		for (int i = FirstID; i < m_Characters.Count; i++)
		{
			if (m_Characters[i] != null && m_Characters[i].Definition != null && string.Compare(CharID, m_Characters[i].Definition.NameID, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static void UnRegisterCharacter(CFGCharacterData chardata)
	{
		m_Characters.Remove(chardata);
	}

	public static CFGCharacterData GetTeamCharacter(int Pos)
	{
		if (m_TeamCharacters == null || Pos < 0 || Pos >= m_TeamCharacters.Count)
		{
			return null;
		}
		return m_TeamCharacters[Pos];
	}

	public static CFGCharacterData GetCurrentSelectedTeamCharacter()
	{
		foreach (CFGCharacterData teamCharacter in m_TeamCharacters)
		{
			if (teamCharacter != null && teamCharacter.CurrentModel != null && teamCharacter.CurrentModel.IsSelected)
			{
				return teamCharacter;
			}
		}
		return null;
	}

	public static List<CFGCharacterData> GetTeamCharactersList()
	{
		List<CFGCharacterData> list = new List<CFGCharacterData>();
		foreach (CFGCharacterData teamCharacter in m_TeamCharacters)
		{
			if (teamCharacter != null)
			{
				list.Add(teamCharacter);
			}
		}
		return list;
	}

	public static List<CFGCharacterData> GetTeamCharactersListTactical()
	{
		List<CFGCharacterData> list = new List<CFGCharacterData>();
		foreach (CFGCharacterData teamCharacter in m_TeamCharacters)
		{
			if (teamCharacter != null && teamCharacter.CurrentModel != null)
			{
				list.Add(teamCharacter);
			}
		}
		return list;
	}

	public static int GetTeamCharacterCount()
	{
		if (m_TeamCharacters == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			if (m_TeamCharacters[i] != null)
			{
				num++;
			}
		}
		return num;
	}

	public static CFGCharacterData RegisterNewCharacter(string CharName, bool CanBeHired, bool TempTactical, CFGCharacter CurrentModel, bool bAutoAssignToTeam = true)
	{
		if (CanBeHired)
		{
			int characterIndex = GetCharacterIndex(CharName);
			if (characterIndex != -1)
			{
				Debug.LogWarning("Character " + CharName + " is already registerd. Hireable characters cannot be registered twice!");
				return m_Characters[characterIndex];
			}
		}
		CFGCharacterData cFGCharacterData = new CFGCharacterData();
		if (cFGCharacterData == null)
		{
			return null;
		}
		cFGCharacterData.SetState(ECharacterStateFlag.IsDead, Value: false);
		cFGCharacterData.PositionInTeam = -1;
		cFGCharacterData.SetState(ECharacterStateFlag.TempTactical, TempTactical);
		cFGCharacterData.SetState(ECharacterStateFlag.CanBeHired, CanBeHired);
		cFGCharacterData.AssignDefinition(CharName);
		cFGCharacterData.AssignModel(CurrentModel);
		cFGCharacterData.AssignDefItems();
		cFGCharacterData.Hp = cFGCharacterData.MaxHp;
		if (!CanBeHired)
		{
			cFGCharacterData.SetLuck(Random.Range(0, cFGCharacterData.MaxLuck), bAllowSplash: false);
		}
		m_Characters.Add(cFGCharacterData);
		if (CanBeHired && bAutoAssignToTeam)
		{
			AssignToTeam(CharName);
			if (CurrentModel != null && CFGSelectionManager.Instance.SelectedCharacter == null)
			{
				CFGSelectionManager.Instance.SelectCharacter(CurrentModel, focus: true);
			}
		}
		cFGCharacterData.ForbiddenBuffs_Recalculate();
		if (cFGCharacterData.Definition != null)
		{
			if (!string.IsNullOrEmpty(cFGCharacterData.Definition.Buff1))
			{
				cFGCharacterData.AddBuff(cFGCharacterData.Definition.Buff1, EBuffSource.Definition);
			}
			if (!string.IsNullOrEmpty(cFGCharacterData.Definition.Buff2))
			{
				cFGCharacterData.AddBuff(cFGCharacterData.Definition.Buff2, EBuffSource.Definition);
			}
			if (!string.IsNullOrEmpty(cFGCharacterData.Definition.Buff3))
			{
				cFGCharacterData.AddBuff(cFGCharacterData.Definition.Buff3, EBuffSource.Definition);
			}
		}
		cFGCharacterData.AddLDAbilities();
		return cFGCharacterData;
	}

	public static CFGCharacter SpawnCharacter(string character_id, CFGOwner owner, Vector3 pos, Quaternion rot, bool bSingleInstance, int uuid = 0, CFG_SG_Node CharNode = null)
	{
		if (string.IsNullOrEmpty(character_id))
		{
			return null;
		}
		bool flag = false;
		if ((bool)owner && owner.IsPlayer)
		{
			flag = true;
		}
		CFGCharacterData cFGCharacterData = null;
		if (flag || bSingleInstance)
		{
			cFGCharacterData = GetCharacterData(character_id);
		}
		if (cFGCharacterData == null)
		{
			cFGCharacterData = RegisterNewCharacter(character_id, flag, !flag, null);
		}
		if (cFGCharacterData == null)
		{
			return null;
		}
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && !CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoReactionShot))
		{
			cFGCharacterData.SetState(ECharacterStateFlag.CanDoReactionShoot, Value: true);
		}
		if (CharNode != null)
		{
			cFGCharacterData.OnDeserialize(CharNode);
		}
		CFGCharacter characterPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetCharacterPrefab(character_id);
		if (characterPrefab == null)
		{
			Debug.LogWarning("Failed to find character prefab for " + character_id);
			return null;
		}
		CFGCharacter cFGCharacter = CFGObjectManager.Instantiate(characterPrefab, pos, rot) as CFGCharacter;
		if (cFGCharacter == null)
		{
			Debug.LogWarning("Failed to instantiate character   " + character_id);
			return null;
		}
		cFGCharacter.AssignUUID(uuid);
		cFGCharacter.NameId = character_id;
		cFGCharacter.SetOwner(owner);
		if ((bool)owner && owner.IsPlayer && cFGCharacterData.PositionInTeam >= 0 && CharNode == null && cFGCharacterData.Definition != null)
		{
			CFGAchievmentTracker.OnCharacterSpawn(cFGCharacterData.Definition.Achiev23Name);
		}
		if (CharNode == null)
		{
			cFGCharacterData.EquipDefaultWeapon();
		}
		cFGCharacter.AssignToCharacterData(cFGCharacterData, IsReInint: false);
		if (CharNode != null)
		{
			cFGCharacter.EquipWeapon(bCheckInGame: false);
		}
		cFGCharacterData.ForbiddenBuffs_Recalculate();
		if (CharNode == null)
		{
			cFGCharacterData.Hp = cFGCharacterData.MaxHp;
		}
		List<CFGCharacterData> teamCharactersListTactical = GetTeamCharactersListTactical();
		int id = 0;
		for (int i = 0; i < teamCharactersListTactical.Count; i++)
		{
			if (teamCharactersListTactical[i].CurrentModel == CFGSelectionManager.Instance.SelectedCharacter)
			{
				id = i;
				break;
			}
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUD)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_HUD.SelectCharacter(id);
		}
		return cFGCharacter;
	}

	public static void RecalculateDecay()
	{
		if (m_TeamCharacters != null && m_TeamCharacters.Count >= 1)
		{
			for (int i = 0; i < m_TeamCharacters.Count; i++)
			{
				m_TeamCharacters[i].RecalculateDecayBuff();
			}
		}
	}

	public static void OnSerialize(CFG_SG_Node node)
	{
		node.Attrib_Set("ID", m_CurrentTeam);
		for (int i = 0; i < m_TeamCharacters.Count; i++)
		{
			m_TeamCharacters[i].PositionInTeam = i;
		}
		for (int j = 0; j < m_Characters.Count; j++)
		{
			if (m_Characters[j] != null)
			{
				m_Characters[j].OnSerialize(node);
			}
		}
	}

	public static bool OnDeserialize_UpdatePositionsOnly(CFG_SG_Node node)
	{
		for (int i = 0; i < node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = node.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Char", ignoreCase: true) != 0)
			{
				continue;
			}
			CFG_SG_Node cFG_SG_Node = subNode.FindSubNode("Model");
			if (cFG_SG_Node == null)
			{
				continue;
			}
			int num = cFG_SG_Node.Attrib_Get("UUID", 0, bReport: false);
			if (num != 0)
			{
				Vector3 position = cFG_SG_Node.Attrib_Get("Pos", Vector3.zero);
				Quaternion rotation = cFG_SG_Node.Attrib_Get("Rot", Quaternion.identity);
				CFGCharacter cFGCharacter = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(num, ESerializableType.Character);
				if (cFGCharacter == null)
				{
					Debug.LogWarning("Failed to find character: " + num);
					continue;
				}
				cFGCharacter.transform.position = position;
				cFGCharacter.transform.rotation = rotation;
			}
		}
		return true;
	}

	public static bool OnDeserialize_ListOnly(CFG_SG_Node node)
	{
		ClearCharacters();
		int activeTeam = node.Attrib_Get("ID", 0, bReport: false);
		SetActiveTeam(activeTeam);
		for (int i = 0; i < node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = node.GetSubNode(i);
			if (subNode != null && string.Compare(subNode.Name, "Char", ignoreCase: true) == 0)
			{
				string charName = subNode.Attrib_Get<string>("Name", null);
				CFGCharacterData cFGCharacterData = RegisterNewCharacter(charName, CanBeHired: false, TempTactical: false, null, bAutoAssignToTeam: false);
				if (cFGCharacterData == null)
				{
					return false;
				}
				cFGCharacterData.OnDeserialize(subNode);
			}
		}
		BuildCurrentTeam();
		return true;
	}

	private static bool DeserializeModel(CFG_SG_Node Parent, string CharID, ref HashSet<CFGCharacter> chartodelete)
	{
		CFG_SG_Node cFG_SG_Node = Parent.FindSubNode("Model");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		int num = cFG_SG_Node.Attrib_Get("UUID", 0, bReport: false);
		if (num != 0)
		{
			int uuid = cFG_SG_Node.Attrib_Get("Owner", 0);
			CFGOwner ownerByUUID = CFGOwner.GetOwnerByUUID(uuid);
			Vector3 vector = cFG_SG_Node.Attrib_Get("Pos", Vector3.zero);
			Quaternion quaternion = cFG_SG_Node.Attrib_Get("Rot", Quaternion.identity);
			CFGCharacter cFGCharacter = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(num, ESerializableType.Character);
			if (cFGCharacter != null)
			{
				CFGCharacterData cFGCharacterData = RegisterNewCharacter(CharID, CanBeHired: false, TempTactical: false, null);
				if (cFGCharacterData == null)
				{
					return false;
				}
				cFGCharacterData.OnDeserialize(Parent);
				chartodelete.Remove(cFGCharacter);
				cFGCharacter.SetOwner(ownerByUUID);
				cFGCharacter.transform.position = vector;
				cFGCharacter.transform.rotation = quaternion;
				cFGCharacter.NameId = CharID;
				cFGCharacter.AssignToCharacterData(cFGCharacterData, IsReInint: false);
			}
			else
			{
				bool bSingleInstance = false;
				if ((bool)ownerByUUID && ownerByUUID.IsPlayer)
				{
					bSingleInstance = true;
				}
				cFGCharacter = SpawnCharacter(CharID, ownerByUUID, vector, quaternion, bSingleInstance, num, Parent);
			}
			if ((bool)cFGCharacter)
			{
				cFGCharacter.OnDeserialize(cFG_SG_Node);
			}
		}
		return true;
	}

	public static bool OnDeserialize(CFG_SG_Node node)
	{
		ClearCharacters();
		int activeTeam = node.Attrib_Get("ID", 0, bReport: false);
		SetActiveTeam(activeTeam);
		HashSet<CFGCharacter> chartodelete = new HashSet<CFGCharacter>();
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			chartodelete.Add(character);
		}
		for (int i = 0; i < node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = node.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Char", ignoreCase: true) != 0)
			{
				continue;
			}
			string text = subNode.Attrib_Get<string>("Name", null);
			if (!DeserializeModel(subNode, text, ref chartodelete))
			{
				CFGCharacterData cFGCharacterData = RegisterNewCharacter(text, CanBeHired: false, TempTactical: false, null);
				if (cFGCharacterData == null)
				{
					return false;
				}
				cFGCharacterData.OnDeserialize(subNode);
			}
		}
		if (chartodelete.Count > 0)
		{
			foreach (CFGCharacter item in chartodelete)
			{
				if ((bool)item)
				{
					Object.Destroy(item.gameObject);
				}
			}
		}
		foreach (CFGCharacterData character2 in m_Characters)
		{
			character2.Deserialize_UpdateNemezis();
		}
		BuildCurrentTeam();
		return true;
	}

	private static void BuildCurrentTeam()
	{
		m_TeamCharacters.Clear();
		for (int i = 0; i < m_Characters.Count; i++)
		{
			if (m_Characters[i] != null && m_Characters[i].TeamID == m_CurrentTeam && m_Characters[i].PositionInTeam >= 0)
			{
				AssignToTeamAtPosition(m_Characters[i].Definition.NameID, bHidePopup: true);
			}
		}
		if (m_TeamCharacters != null && m_TeamCharacters.Count > 0)
		{
			m_TeamCharacters.Sort(CompareTeamChars);
		}
	}

	private static int CompareTeamChars(CFGCharacterData ch1, CFGCharacterData ch2)
	{
		if (ch1.PositionInTeam < 0)
		{
			return -1;
		}
		if (ch2.PositionInTeam < 0)
		{
			return -1;
		}
		if (ch1.PositionInTeam < ch2.PositionInTeam)
		{
			return -1;
		}
		if (ch1.PositionInTeam > ch2.PositionInTeam)
		{
			return 1;
		}
		return 0;
	}
}
