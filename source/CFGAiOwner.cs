using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGAiOwner : CFGOwner
{
	private class AiTeamData
	{
		public CFGAiPreset ai_preset;

		public Transform objective;

		public CFGCharacter priority_target;

		public bool roaming;

		public CFG_SG_Node OnSerialize(CFG_SG_Node ParentNode)
		{
			CFG_SG_Node cFG_SG_Node = ParentNode.AddSubNode("AiTeamData");
			if (cFG_SG_Node == null)
			{
				return null;
			}
			cFG_SG_Node.Attrib_Set("Roaming", roaming);
			if (objective != null)
			{
				CFGSerializableObject[] components = objective.gameObject.GetComponents<CFGSerializableObject>();
				if (components.Length > 0)
				{
					cFG_SG_Node.Attrib_Set("Pos", components[0].UniqueID);
				}
				else
				{
					Debug.LogError("Failed to serialize objective: " + objective);
				}
			}
			if (priority_target != null)
			{
				cFG_SG_Node.Attrib_Set("TargetUUID", priority_target.UniqueID);
			}
			if (ai_preset != null)
			{
				ai_preset.OnSerialize(cFG_SG_Node);
			}
			return cFG_SG_Node;
		}

		public void OnDeserialize(CFG_SG_Node node)
		{
			roaming = node.Attrib_Get("Roaming", DefVal: false);
			int num = node.Attrib_Get("Pos", 0);
			if (num != 0)
			{
				CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(num, ESerializableType.NotSerializable);
				if ((bool)cFGSerializableObject)
				{
					objective = cFGSerializableObject.gameObject.transform;
				}
				else
				{
					Debug.LogError("Failed to find objective: " + num);
				}
			}
			int num2 = node.Attrib_Get("TargetUUID", 0);
			if (num2 != 0)
			{
				priority_target = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(num2, ESerializableType.NotSerializable);
				if (priority_target == null)
				{
					Debug.LogError("Failed to find priority target: " + num2);
				}
			}
			CFG_SG_Node cFG_SG_Node = node.FindSubNode("AiPreset");
			if (cFG_SG_Node != null)
			{
				ai_preset = ScriptableObject.CreateInstance<CFGAiPreset>();
				ai_preset.OnDeserialize(cFG_SG_Node);
			}
		}
	}

	public delegate void OnCombatStartDelegate();

	[CFGFlowCode(Category = "Owner", Title = "On Combat Start")]
	public OnCombatStartDelegate m_OnCombatStartCallback;

	[NonSerialized]
	public bool m_AiDebugging;

	[NonSerialized]
	public bool m_DebugHoldAiUpdate = true;

	[NonSerialized]
	public bool m_DebugOnlySimulate;

	[NonSerialized]
	public string m_DebugSimulationResult = string.Empty;

	[NonSerialized]
	public bool m_DebugShowCellsEval;

	[NonSerialized]
	public Transform m_LastRoamingTarget;

	[NonSerialized]
	public bool m_LastRoamingSet;

	private bool m_UpdateAi = true;

	private CFGCharacter m_CurrentCharacter;

	private Dictionary<int, AiTeamData> m_AiTeamsData = new Dictionary<int, AiTeamData>();

	public override bool IsAi => true;

	[CFGFlowCode]
	public bool UpdateAi
	{
		get
		{
			return m_UpdateAi;
		}
		set
		{
			m_UpdateAi = value;
		}
	}

	[CFGFlowCode]
	public ESetupStage SetupStage
	{
		get
		{
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				return ESetupStage.Unexpecting;
			}
			return ESetupStage.Combat;
		}
		set
		{
			if (value == ESetupStage.Combat)
			{
				if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
				{
					return;
				}
				CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = false;
				for (int i = 0; i < CFGCharacterList.GetCharacterCount(); i++)
				{
					CFGCharacterData characterData = CFGCharacterList.GetCharacterData(i);
					if (characterData != null && !CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoReactionShot))
					{
						characterData.SetState(ECharacterStateFlag.CanDoReactionShoot, Value: true);
					}
				}
			}
			else
			{
				CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = true;
			}
		}
	}

	public CFGCharacter CurrentCharacter => m_CurrentCharacter;

	public override bool NeedsSaving => true;

	public static bool IsCharacterNearObjective(CFGCharacter character, Transform objective, int radius, bool need_los)
	{
		CFGCell currentCell = character.CurrentCell;
		CFGCell characterCell = CFGCellMap.GetCharacterCell(objective.position);
		if (currentCell == null || characterCell == null)
		{
			Debug.LogError("ERROR! CFGAiOwner.IsCharacterNearObjective() - character or objective is not on grid!");
			return false;
		}
		if (CFGCellMap.Distance(currentCell, characterCell) > radius)
		{
			return false;
		}
		if (need_los)
		{
			return CFGCellMap.GetLineOfSightAutoSideSteps(null, null, currentCell, characterCell, character.BuffedSight) == ELOXHitType.None;
		}
		return true;
	}

	public void AiTeamSetPreset(int ai_team, CFGAiPreset ai_preset)
	{
		if (!m_AiTeamsData.ContainsKey(ai_team))
		{
			m_AiTeamsData.Add(ai_team, new AiTeamData());
		}
		m_AiTeamsData[ai_team].ai_preset = ai_preset;
	}

	public CFGAiPreset GetAiTeamPreset(int ai_team)
	{
		if (m_AiTeamsData.TryGetValue(ai_team, out var value))
		{
			return value.ai_preset;
		}
		return null;
	}

	public void AiTeamSetObjective(int ai_team, Transform transform)
	{
		if (!m_AiTeamsData.ContainsKey(ai_team))
		{
			m_AiTeamsData.Add(ai_team, new AiTeamData());
		}
		m_AiTeamsData[ai_team].objective = transform;
		m_AiTeamsData[ai_team].roaming = false;
	}

	public Transform GetAiTeamObjective(int ai_team)
	{
		if (m_AiTeamsData.TryGetValue(ai_team, out var value))
		{
			return value.objective;
		}
		return null;
	}

	public void AiTeamActivateRoaming(int ai_team)
	{
		if (!m_AiTeamsData.ContainsKey(ai_team))
		{
			m_AiTeamsData.Add(ai_team, new AiTeamData());
		}
		m_AiTeamsData[ai_team].objective = null;
		m_AiTeamsData[ai_team].roaming = true;
	}

	public bool AiTeamIsRoaming(int ai_team)
	{
		if (m_AiTeamsData.TryGetValue(ai_team, out var value))
		{
			return value.roaming;
		}
		return false;
	}

	public void AiTeamSetPriorityTarget(int ai_team, CFGCharacter target)
	{
		if (!m_AiTeamsData.ContainsKey(ai_team))
		{
			m_AiTeamsData.Add(ai_team, new AiTeamData());
		}
		m_AiTeamsData[ai_team].priority_target = target;
	}

	public CFGCharacter GetAiTeamPriorityTarget(int ai_team)
	{
		if (m_AiTeamsData.TryGetValue(ai_team, out var value))
		{
			return value.priority_target;
		}
		return null;
	}

	public override void OnTacticalStart()
	{
		UpdateStagePanel();
	}

	public void UpdateStagePanel()
	{
		CFGSetupStagePanel setupStagePanel = CFGSingleton<CFGWindowMgr>.Instance.m_SetupStagePanel;
		if (!(setupStagePanel == null))
		{
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				setupStagePanel.ShowStagePanel(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_unexp_head"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_unexp_body"), 0);
			}
			else
			{
				setupStagePanel.ShowStagePanel(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_alarm_head"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_alarm_body"), 1);
			}
		}
	}

	public override void EndTurn(CFGOwner owner)
	{
		base.EndTurn(owner);
		if (owner == this && !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			CFGSetupStagePanel setupStagePanel = CFGSingleton<CFGWindowMgr>.Instance.m_SetupStagePanel;
			if (setupStagePanel != null)
			{
				setupStagePanel.HideStagePanel();
			}
		}
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();
		if (!(CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner == this))
		{
			return;
		}
		if (UpdateAi)
		{
			if (m_CurrentCharacter == null || m_CurrentCharacter.IsDead || (m_CurrentCharacter.ActionPoints == 0 && m_CurrentCharacter.CurrentAction == ETurnAction.None))
			{
				m_CurrentCharacter = FindFirstCharacterWithAP();
			}
			if (m_CurrentCharacter != null)
			{
				if (m_CurrentCharacter.CurrentAction != ETurnAction.None || !(m_CurrentCharacter.CharacterAnimator != null) || !m_CurrentCharacter.CharacterAnimator.IsFacingRightDirection() || (m_AiDebugging && m_DebugHoldAiUpdate))
				{
					return;
				}
				UpdateCharacterAi(m_CurrentCharacter);
				if (m_AiDebugging)
				{
					m_DebugHoldAiUpdate = true;
					if (!m_DebugOnlySimulate)
					{
						m_DebugSimulationResult = string.Empty;
						m_DebugShowCellsEval = false;
					}
				}
				return;
			}
		}
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.EndTurn(bUpdateTurnCounter: true);
	}

	protected override void Start()
	{
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RegisterAiOwner(this);
	}

	private static Color CalcColor(int value, int min_value, int max_value)
	{
		float num = (float)(value - min_value) / (float)(max_value - min_value);
		if (num < 0.5f)
		{
			return new Color(1f, num * 2f, 0f);
		}
		return new Color(1f - (num - 0.5f) * 2f, 1f, 0f);
	}

	protected void OnGUI()
	{
		if (!m_AiDebugging || !m_DebugShowCellsEval || m_CurrentCharacter == null)
		{
			return;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component == null)
		{
			return;
		}
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(m_CurrentCharacter.CurrentCell, m_CurrentCharacter.HalfMovement);
		int num = int.MaxValue;
		int num2 = 0;
		foreach (CFGCell item in hashSet)
		{
			num = Mathf.Min(num, item.m_AiValue);
			num2 = Mathf.Max(num2, item.m_AiValue);
		}
		float num3 = 50f;
		float num4 = 25f;
		foreach (CFGCell item2 in hashSet)
		{
			if ((item2 == m_CurrentCharacter.CurrentCell || item2.CanStandOnThisTile(can_stand_now: true)) && item2.StairsType != CFGCell.EStairsType.Slope && item2.Floor == (int)component.CurrentFloorLevel)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(item2.WorldPosition);
				vector.y = (float)Screen.height - vector.y;
				vector.x -= num3 * 0.5f;
				vector.y -= num4 * 0.5f;
				Rect position = new Rect(vector.x, vector.y, num3, num4);
				GUI.color = CalcColor(item2.m_AiValue, num, num2);
				if (item2.m_AiValue == num2)
				{
					GUI.Box(position, item2.m_AiValue.ToString());
				}
				else
				{
					GUI.Label(position, item2.m_AiValue.ToString());
				}
			}
		}
		GUI.color = Color.white;
	}

	public void OnCombatStart()
	{
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner.Characters)
		{
			if (character.IsAlive && character.GunpointState != EGunpointState.Target && !character.Imprisoned)
			{
				if (character.CurrentAction == ETurnAction.None)
				{
					OnCharacterActionFinished(character, ETurnAction.None);
				}
				else
				{
					character.m_OnCharacterActionFinishedCallback = (CFGCharacter.OnCharacterActionFinishedDelegate)Delegate.Combine(character.m_OnCharacterActionFinishedCallback, new CFGCharacter.OnCharacterActionFinishedDelegate(OnCharacterActionFinished));
				}
			}
		}
		foreach (CFGCharacter character2 in base.Characters)
		{
			if ((bool)character2 && character2.IsAlive)
			{
				character2.ActionPoints = character2.MaxActionPoints;
			}
		}
		if (m_OnCombatStartCallback != null)
		{
			m_OnCombatStartCallback();
			m_OnCombatStartCallback = null;
		}
		foreach (CFGCharacter character3 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character3 == null)
			{
				continue;
			}
			character3.CanDoReactionShot = false;
			if (!character3.IsDead && character3.CharacterData != null)
			{
				if (character3.GunpointState != 0)
				{
					character3.GunpointState = EGunpointState.None;
				}
				character3.CharacterData.SubduedCount = -1;
				character3.CharacterData.SuspicionLevel = -1;
				character3.CharacterData.AIState = EAIState.InCombat;
			}
		}
	}

	public void OnCharacterActionFinished(CFGCharacter character, ETurnAction action)
	{
		character.ProcessFacingToCover();
		if (character.CharacterAnimator != null && character.CurrentWeapon != null && character.CurrentWeapon.Visualisation == null)
		{
			if (character.CurrentWeapon.TwoHanded)
			{
				character.CharacterAnimator.PlayWeaponChange2(null, character.OnAnimWeaponEquipped);
			}
			else
			{
				character.CharacterAnimator.PlayWeaponChange1(null, character.OnAnimWeaponEquipped);
			}
		}
		character.m_OnCharacterActionFinishedCallback = (CFGCharacter.OnCharacterActionFinishedDelegate)Delegate.Remove(character.m_OnCharacterActionFinishedCallback, new CFGCharacter.OnCharacterActionFinishedDelegate(OnCharacterActionFinished));
	}

	private CFGCharacter FindFirstCharacterWithAP()
	{
		foreach (CFGCharacter character in base.Characters)
		{
			if ((bool)character && character.IsAlive && character.ActionPoints > 0)
			{
				return character;
			}
		}
		return null;
	}

	private CFGCharacter FindNearestEnemy(CFGCharacter character)
	{
		CFGCharacter result = null;
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner != null)
		{
			int num = int.MaxValue;
			foreach (CFGCharacter character2 in playerOwner.Characters)
			{
				if (!character2.Imprisoned)
				{
					int num2 = CFGCellMap.Distance(character.CurrentCell, character2.CurrentCell);
					if (num2 < num)
					{
						result = character2;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	private void UpdateCharacterAi(CFGCharacter character)
	{
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner == null)
		{
			return;
		}
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			CharacterMakeAction(character, "In setup stage", ETurnAction.End);
			return;
		}
		if (character.AIState == EAIState.Subdued)
		{
			CharacterMakeAction(character, "Im subdued! Not going to move an inch!", ETurnAction.End);
			return;
		}
		if (character.AiTeam == -1)
		{
			Debug.LogWarning("WARNING! CFGAiOwner.UpdateCharacterAi() - character " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(character.NameId) + " has no AI team assigned. AI will do nothing.");
		}
		CFGAiPreset cFGAiPreset = null;
		cFGAiPreset = ((character.CharacterData == null || !character.CharacterData.HasBuff("intimidated")) ? GetAiTeamPreset(character.AiTeam) : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_IntimidateAiPreset);
		if (cFGAiPreset == null)
		{
			CharacterMakeAction(character, "No AI preset assigned", ETurnAction.End);
			return;
		}
		bool flag = character.VisibleEnemies.Count > 0;
		Transform transform;
		if (!flag && AiTeamIsRoaming(character.AiTeam))
		{
			if (character.ActionPoints != 2)
			{
				CharacterMakeAction(character, "Wlaczony roaming, zostal mi 1 AP, nie widze wrogow.", ETurnAction.End);
				return;
			}
			if (UnityEngine.Random.Range(0, 100) < 70)
			{
				CFGCharacter cFGCharacter = FindNearestEnemy(character);
				m_LastRoamingTarget = ((!cFGCharacter) ? null : cFGCharacter.transform);
			}
			else
			{
				m_LastRoamingTarget = null;
			}
			transform = m_LastRoamingTarget;
		}
		else
		{
			transform = GetAiTeamObjective(character.AiTeam);
			m_LastRoamingTarget = null;
		}
		if (transform != null && !IsCharacterNearObjective(character, transform, cFGAiPreset.m_ObjectiveRadius, cFGAiPreset.m_ObjectiveInLoSRequired) && ((character.ActionPoints == 2 && (!flag || cFGAiPreset.m_ApReservedForMove == CFGAiPreset.EAPReservedForMove.One)) || cFGAiPreset.m_ApReservedForMove == CFGAiPreset.EAPReservedForMove.Both))
		{
			CFGCell cFGCell = FindBestCellForMoveToObj(character, transform);
			if (cFGCell != null && CharacterMakeAction(character, "Mam teraz isc do objective'a (nie widze wrogow lub mam zarezerwowany AP na ruch do objective'a).", ETurnAction.Move, cFGCell))
			{
				return;
			}
		}
		if (!flag)
		{
			CFGCell cFGCell2 = FindBestCellForCombat(character);
			if (cFGCell2 == null || character.CurrentCell == cFGCell2 || !CharacterMakeAction(character, "Nie mam objective'a (albo mam ale nie chce/nie moge do niego isc), nie widze wrogow, nie jestem w optymalnej pozycji.", ETurnAction.Move, cFGCell2))
			{
				if (character.CanMakeAction(ETurnAction.Reload) == EActionResult.Success && ShouldReload(character))
				{
					CharacterMakeAction(character, "Nie mam objective'a (albo mam ale nie chce/nie moge do niego isc), nie widze wrogow, jestem w optymalnej pozycji, mam niepelny magazynek.", ETurnAction.Reload);
				}
				else
				{
					CharacterMakeAction(character, "Nie mam objective'a (albo mam ale nie chce/nie moge do niego isc), nie widze wrogow, jestem w optymalnej pozycji, mam pelny magazynek.", ETurnAction.End);
				}
			}
			return;
		}
		if (character.ActionPoints == 2)
		{
			CFGCell cFGCell3 = FindBestCellForCombat(character);
			if ((cFGCell3 != null && character.CurrentCell != cFGCell3 && CharacterMakeAction(character, "Mam 2 AP, nie jestem w optymalnej pozycji.", ETurnAction.Move, cFGCell3)) || (character.CanMakeAction(ETurnAction.Reload) == EActionResult.Success && ShouldReload(character) && CharacterMakeAction(character, "Mam 2 AP, jestem w optymalnej pozycji, mam niepelny magazynek.", ETurnAction.Reload)))
			{
				return;
			}
		}
		else if (character.CanMakeAction(ETurnAction.Shoot) == EActionResult.Success)
		{
			CFGCharacter cFGCharacter2 = FindBestTargetToShoot(character);
			if (cFGCharacter2 != null && CharacterMakeAction(character, "Zostal mi 1 AP, mam dobry cel do strzalu.", ETurnAction.Shoot, cFGCharacter2))
			{
				return;
			}
		}
		if (character.CanMakeAction(ETurnAction.Reload) == EActionResult.Success && ShouldReload(character) && CharacterMakeAction(character, "Nie mam nic sensownego do zrobienia, mam niepelny magazynek.", ETurnAction.Reload))
		{
			return;
		}
		if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.EnemyAbilities))
		{
			switch (UnityEngine.Random.Range(0, 10))
			{
			case 0:
				if (!character.HaveAbility(ETurnAction.Courage))
				{
					character.CharacterData.AddAbility(ETurnAction.Courage, EAbilitySource.FlowCode, 0);
				}
				if (character.CanMakeAction(ETurnAction.Courage) == EActionResult.Success && CharacterMakeAction(character, "Mam 2 AP, jestem w optymalnej pozycji, chce zrobic cos zajebistego.", ETurnAction.Courage, null, null, null))
				{
					return;
				}
				break;
			case 1:
				if (!character.HaveAbility(ETurnAction.Dodge))
				{
					character.CharacterData.AddAbility(ETurnAction.Dodge, EAbilitySource.FlowCode, 0);
				}
				if (character.CanMakeAction(ETurnAction.Dodge) == EActionResult.Success && CharacterMakeAction(character, "Mam 2 AP, jestem w optymalnej pozycji, chce zrobic cos zajebistego.", ETurnAction.Dodge, null, null, null))
				{
					return;
				}
				break;
			case 2:
				if (!character.HaveAbility(ETurnAction.Prayer))
				{
					character.CharacterData.AddAbility(ETurnAction.Prayer, EAbilitySource.FlowCode, 0);
				}
				if (character.CanMakeAction(ETurnAction.Prayer) == EActionResult.Success && CharacterMakeAction(character, "Mam 2 AP, jestem w optymalnej pozycji, chce zrobic cos zajebistego.", ETurnAction.Prayer, null, null, null))
				{
					return;
				}
				break;
			}
		}
		if (character.ActionPoints == 2 && character.CanMakeAction(ETurnAction.Shoot) == EActionResult.Success)
		{
			CFGCharacter cFGCharacter3 = FindBestTargetToShoot(character);
			if (cFGCharacter3 != null && CharacterMakeAction(character, "Mam 2 AP, nie mam za bardzo co robic, mam dobry cel do strzalu.", ETurnAction.Shoot, cFGCharacter3))
			{
				return;
			}
		}
		CharacterMakeAction(character, "Nie mam kompletnie nic sensownego do zrobienia.", ETurnAction.End);
	}

	private bool ShouldReload(CFGCharacter character)
	{
		return character.CurrentWeapon != null && character.CurrentWeapon.AmmoCapacity - character.CurrentWeapon.CurrentAmmo >= character.CurrentWeapon.AmmoPerReload;
	}

	private CFGCell FindBestCellForCombat(CFGCharacter character)
	{
		HashSet<CFGCell> cells = CFGCellDistanceFinder.FindCellsInDistance(character.CurrentCell, character.HalfMovement);
		return CFGAiCellsEvaluator.EvaluateCellsForCombat(cells, character);
	}

	private CFGCell FindBestCellForMoveToObj(CFGCharacter character, Transform objective)
	{
		return CalcBestCellToMove(character, objective);
	}

	private CFGCharacter FindBestTargetToShoot(CFGCharacter character)
	{
		CFGCharacter aiTeamPriorityTarget = GetAiTeamPriorityTarget(character.AiTeam);
		if (aiTeamPriorityTarget != null && character.VisibleEnemies.Contains(aiTeamPriorityTarget))
		{
			int chanceToHit = character.GetChanceToHit(aiTeamPriorityTarget, null, null, null, ETurnAction.Shoot);
			if (chanceToHit > 0)
			{
				return aiTeamPriorityTarget;
			}
		}
		CFGCharacter result = null;
		CFGAiPreset aiTeamPreset = GetAiTeamPreset(character.AiTeam);
		if (aiTeamPreset == null)
		{
			return null;
		}
		switch (aiTeamPreset.m_TargetingMode)
		{
		case CFGAiPreset.ETargetingMode.Highest_CTH:
		{
			int num6 = 0;
			foreach (CFGCharacter visibleEnemy in character.VisibleEnemies)
			{
				int chanceToHit3 = character.GetChanceToHit(visibleEnemy, null, null, null, ETurnAction.Shoot);
				if (chanceToHit3 > num6)
				{
					num6 = chanceToHit3;
					result = visibleEnemy;
				}
			}
			break;
		}
		case CFGAiPreset.ETargetingMode.Highest_Dmg:
		{
			int num4 = 0;
			foreach (CFGCharacter visibleEnemy2 in character.VisibleEnemies)
			{
				if (character.GetChanceToHit(visibleEnemy2, null, null, null, ETurnAction.Shoot) != 0)
				{
					int num5 = character.CalcDamage(visibleEnemy2, ETurnAction.Shoot);
					if (num5 > num4)
					{
						num4 = num5;
						result = visibleEnemy2;
					}
				}
			}
			break;
		}
		case CFGAiPreset.ETargetingMode.Optimum:
		{
			int num = 0;
			foreach (CFGCharacter visibleEnemy3 in character.VisibleEnemies)
			{
				int chanceToHit2 = character.GetChanceToHit(visibleEnemy3, null, null, null, ETurnAction.Shoot);
				int num2 = character.CalcDamage(visibleEnemy3, ETurnAction.Shoot);
				int num3 = chanceToHit2 * num2;
				if (num3 > num)
				{
					num = num3;
					result = visibleEnemy3;
				}
			}
			break;
		}
		}
		return result;
	}

	private bool CharacterMakeAction(CFGCharacter character, string debug_reason, ETurnAction action, params object[] args)
	{
		if (m_DebugOnlySimulate)
		{
			m_DebugSimulationResult = action.ToString();
			for (int i = 0; i < args.Length; i++)
			{
				m_DebugSimulationResult = m_DebugSimulationResult + "\n" + args[i];
			}
			m_DebugSimulationResult = m_DebugSimulationResult + "\n\nReason: " + debug_reason;
			return true;
		}
		return character.MakeAction(action, args) == EActionResult.Success;
	}

	private CFGCell CalcBestCellToMove(CFGCharacter character, Transform ai_objective)
	{
		NavigationComponent component = character.GetComponent<NavigationComponent>();
		if (component == null)
		{
			return null;
		}
		CFGCell characterCell = CFGCellMap.GetCharacterCell(ai_objective.position);
		NavGoal_At navGoal_At = new NavGoal_At(characterCell);
		LinkedList<CFGCell> path = new LinkedList<CFGCell>();
		if (component.GeneratePath(character.CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out path))
		{
			HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(character.CurrentCell, character.HalfMovement);
			foreach (CFGCell item in hashSet)
			{
				item.m_AiValue = 0;
			}
			CFGCell cFGCell = null;
			int num = 0;
			float num2 = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo.m_Range + 1f;
			CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
			for (LinkedListNode<CFGCell> linkedListNode = path.First; linkedListNode != path.Last; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.HaveFloor)
				{
					if (!hashSet.Contains(linkedListNode.Value))
					{
						break;
					}
					if (linkedListNode.Value.StairsType != CFGCell.EStairsType.Slope)
					{
						bool flag = false;
						foreach (CFGCharacter character2 in playerOwner.Characters)
						{
							if ((float)CFGCellMap.Distance(linkedListNode.Value, character2.CurrentCell) <= num2)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							cFGCell = linkedListNode.Value;
							cFGCell.m_AiValue = num;
							num += 10;
						}
					}
				}
			}
			if (cFGCell != character.CurrentCell)
			{
				return cFGCell;
			}
		}
		return null;
	}

	public void RaiseAlarm()
	{
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			return;
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_allalarmed");
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = false;
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			character.CanDoReactionShot = false;
		}
	}

	public override bool OnSerialize(CFG_SG_Node ParentNode)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(ParentNode);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.AddSubNode("AiTeamsData");
		if (cFG_SG_Node2 != null)
		{
			foreach (KeyValuePair<int, AiTeamData> aiTeamsDatum in m_AiTeamsData)
			{
				aiTeamsDatum.Value.OnSerialize(cFG_SG_Node2)?.Attrib_Set("ID", aiTeamsDatum.Key);
			}
		}
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		CFG_SG_Node cFG_SG_Node = Node.FindSubNode("AiTeamsData");
		if (cFG_SG_Node != null)
		{
			for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
			{
				CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
				if (subNode != null)
				{
					int num = subNode.Attrib_Get("ID", int.MinValue);
					if (num != int.MinValue)
					{
						AiTeamData aiTeamData = new AiTeamData();
						aiTeamData.OnDeserialize(subNode);
						m_AiTeamsData.Add(num, aiTeamData);
					}
					else
					{
						Debug.LogError("Failed to find get AI preset key");
					}
				}
			}
		}
		return true;
	}
}
