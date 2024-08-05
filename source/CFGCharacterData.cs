using System.Collections.Generic;
using UnityEngine;

public class CFGCharacterData
{
	public const int MAX_CARDS = 5;

	private CFGDef_Character m_Definition;

	private ECharacterStateFlag m_State;

	private int m_InTeamPos = -1;

	private int m_TeamID;

	private int m_Hp = 10;

	private int m_Luck = 100;

	private int m_DifficultyHPBonus;

	private CFGGun m_Gun1 = new CFGGun();

	private CFGGun m_Gun2 = new CFGGun();

	private string m_Talisman = string.Empty;

	private string m_Item1 = string.Empty;

	private string m_Item2 = string.Empty;

	private EAIState m_AIState;

	private int m_SuspicionLevel = -1;

	private int m_SubduedCount = -1;

	private CFGDef_Card[] m_Cards = new CFGDef_Card[5];

	private int m_CardSlotsUnlocked = 3;

	private CFGCharacter m_CurrentModel;

	private CFGCharacterStats m_BaseStats = new CFGCharacterStats();

	private CFGCharacterStats m_BuffedStats = new CFGCharacterStats();

	private Dictionary<string, CFGBuff> m_Buffs = new Dictionary<string, CFGBuff>();

	private Dictionary<ETurnAction, CAbilityInfo> m_Abilities = new Dictionary<ETurnAction, CAbilityInfo>();

	private int m_ActionPoints;

	private int m_ArteryDistance;

	private CFGCharacter m_Nemesis;

	private float m_DeathTime = -1f;

	private int m_AiTeam = -1;

	private int m_DecayLevel = -1;

	private int m_DecayFromCards;

	private int m_TotalHeat;

	private HashSet<string> m_ForbiddenBuffs;

	private int m_NemesisID;

	private EGunpointState m_GunpointState;

	private ECardHandBonus m_CardHandBonus;

	public HashSet<string> ForbiddenBuffs => m_ForbiddenBuffs;

	public CFGDef_Character Definition => m_Definition;

	public Dictionary<string, CFGBuff> Buffs => m_Buffs;

	public ECardHandBonus CardHandBonus => m_CardHandBonus;

	public int Aim => m_BaseStats.m_Aim;

	public int BuffedAim => m_BuffedStats.m_Aim;

	public int Defence => m_BaseStats.m_Defense;

	public int BuffedDefense => m_BuffedStats.m_Defense;

	public int BuffedDamage => m_BuffedStats.m_Damage;

	public int Movement => m_BaseStats.m_Movement;

	public int BuffedMovement => Mathf.Max(5, m_BuffedStats.m_Movement);

	public int Sight => m_BaseStats.m_Sight;

	public int BuffedSight => m_BuffedStats.m_Sight;

	public int BuffedMaxHP => m_BuffedStats.m_MaxHealth;

	public int BuffedMaxLuck => m_BuffedStats.m_MaxLuck;

	public int BuffedLuckReplenishPerHit => m_BuffedStats.m_LuckReplenishHit;

	public int Luck => m_Luck;

	public int SuspicionLimit => (m_Definition == null) ? 5 : m_Definition.SuspicionLimit;

	public int SubduedLimit => (m_Definition == null) ? 5 : m_Definition.SubduedLimit;

	public int Heat => (m_Definition != null) ? m_Definition.Heat : 0;

	public int ActionPoints
	{
		get
		{
			return m_ActionPoints;
		}
		set
		{
			m_ActionPoints = Mathf.Clamp(value, 0, MaxAP);
		}
	}

	public int MaxAP => (m_Definition == null) ? 2 : m_Definition.MaxAP;

	public float DeathTime
	{
		get
		{
			return m_DeathTime;
		}
		set
		{
			m_DeathTime = value;
		}
	}

	public int PositionInTeam
	{
		get
		{
			return m_InTeamPos;
		}
		set
		{
			m_InTeamPos = value;
		}
	}

	public int TeamID
	{
		get
		{
			return m_TeamID;
		}
		set
		{
			m_TeamID = value;
		}
	}

	public int ArteryDistance
	{
		get
		{
			return m_ArteryDistance;
		}
		set
		{
			m_ArteryDistance = value;
		}
	}

	public int SuspicionLevel
	{
		get
		{
			return m_SuspicionLevel;
		}
		set
		{
			m_SuspicionLevel = value;
		}
	}

	public int SubduedCount
	{
		get
		{
			return m_SubduedCount;
		}
		set
		{
			m_SubduedCount = value;
		}
	}

	public int AITeam
	{
		get
		{
			return m_AiTeam;
		}
		set
		{
			m_AiTeam = value;
		}
	}

	public EAIState AIState
	{
		get
		{
			return m_AIState;
		}
		set
		{
			m_AIState = value;
		}
	}

	public EGunpointState GunpointState
	{
		get
		{
			return m_GunpointState;
		}
		set
		{
			m_GunpointState = value;
		}
	}

	public int DecayLevel => m_DecayLevel;

	public int TotalDecayLevel => m_DecayLevel + m_DecayFromCards;

	public CFGGun Gun1
	{
		get
		{
			if (m_Gun1 == null || m_Gun1.m_Definition == null)
			{
				return null;
			}
			return m_Gun1;
		}
	}

	public CFGGun Gun2
	{
		get
		{
			if (m_Gun2 == null || m_Gun2.m_Definition == null)
			{
				return null;
			}
			return m_Gun2;
		}
	}

	public bool Gun1IsTemporary
	{
		get
		{
			if ((bool)m_Gun1)
			{
				return m_Gun1.m_bTemporary;
			}
			return false;
		}
	}

	public bool Gun2IsTemporary
	{
		get
		{
			if ((bool)m_Gun2)
			{
				return m_Gun2.m_bTemporary;
			}
			return false;
		}
	}

	public CFGCharacter Nemesis
	{
		get
		{
			return m_Nemesis;
		}
		set
		{
			m_Nemesis = value;
		}
	}

	public Dictionary<ETurnAction, CAbilityInfo> Abilities => m_Abilities;

	public int HP
	{
		get
		{
			return m_Hp;
		}
		set
		{
			m_Hp = Mathf.Clamp(value, 0, MaxHp);
		}
	}

	public string Weapon1
	{
		get
		{
			if (m_Gun1 == null || m_Gun1.m_Definition == null)
			{
				return null;
			}
			return m_Gun1.m_Definition.ItemID;
		}
	}

	public string Weapon2
	{
		get
		{
			if (m_Gun2 == null || m_Gun2.m_Definition == null)
			{
				return null;
			}
			return m_Gun2.m_Definition.ItemID;
		}
	}

	public string Item1 => m_Item1;

	public string Item2 => m_Item2;

	public string Talisman => m_Talisman;

	public int TotalHeat => m_TotalHeat;

	public int ImageIDX
	{
		get
		{
			if (m_Definition == null)
			{
				return 0;
			}
			if (!IsStateSet(ECharacterStateFlag.InDemonForm))
			{
				return m_Definition.ImageID;
			}
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_DemonImageID >= 0)
			{
				return CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_DemonImageID;
			}
			return 0;
		}
	}

	public int UnlockedCardSlots
	{
		get
		{
			return m_CardSlotsUnlocked;
		}
		set
		{
			m_CardSlotsUnlocked = Mathf.Clamp(value, 0, 5);
		}
	}

	public CFGCharacter CurrentModel
	{
		get
		{
			return m_CurrentModel;
		}
		set
		{
			m_CurrentModel = value;
		}
	}

	public int MaxLuck => m_BuffedStats.m_MaxLuck;

	public int Hp
	{
		get
		{
			return m_Hp;
		}
		set
		{
			m_Hp = Mathf.Clamp(value, 0, MaxHp);
		}
	}

	public int MaxHp => m_BuffedStats.m_MaxHealth;

	public bool Invulnerable
	{
		get
		{
			return IsStateSet(ECharacterStateFlag.Invulnerable);
		}
		set
		{
			SetState(ECharacterStateFlag.Invulnerable, value);
		}
	}

	public bool IsDead
	{
		get
		{
			return IsStateSet(ECharacterStateFlag.IsDead);
		}
		set
		{
			SetState(ECharacterStateFlag.IsDead, value);
		}
	}

	public bool IsAlive => !IsStateSet(ECharacterStateFlag.IsDead);

	public CFGCharacterStats BaseStats => m_BaseStats;

	public CFGDef_Card GetCard(int i)
	{
		if (m_Cards == null || i < 0 || i >= m_Cards.Length)
		{
			return null;
		}
		return m_Cards[i];
	}

	public void OnDifficultyChanged(EDifficulty NewLevel)
	{
		int difficultyHPBonus = m_DifficultyHPBonus;
		if (m_InTeamPos > -1)
		{
			m_DifficultyHPBonus = 0;
		}
		else
		{
			switch (NewLevel)
			{
			case EDifficulty.Easy:
				m_DifficultyHPBonus = m_Definition.MaxHPEasy - m_Definition.MaxHealth;
				break;
			case EDifficulty.Normal:
				m_DifficultyHPBonus = 0;
				break;
			case EDifficulty.Hard:
				m_DifficultyHPBonus = m_Definition.MaxHPHard - m_Definition.MaxHealth;
				break;
			}
		}
		if (m_DifficultyHPBonus != difficultyHPBonus)
		{
			EvaluateBuffs();
			if (m_DifficultyHPBonus > difficultyHPBonus)
			{
				Hp += m_DifficultyHPBonus;
			}
			else
			{
				Hp = Hp;
			}
		}
	}

	public void SetLuck(int NewValue, bool bAllowSplash)
	{
		if (!IsDead)
		{
			int num = Mathf.Clamp(NewValue, 0, MaxLuck);
			if (CFGCheats.InfiniteLuck)
			{
				num = 100;
			}
			if (bAllowSplash && m_CurrentModel != null && Hp > 0)
			{
				m_CurrentModel.SpawnLuckSplash(num - m_Luck);
			}
			m_Luck = num;
		}
	}

	public bool IsStateSet(ECharacterStateFlag State)
	{
		if (State == ECharacterStateFlag.None)
		{
			return m_State == ECharacterStateFlag.None;
		}
		return (m_State & State) == State;
	}

	public void SetState(ECharacterStateFlag State, bool Value)
	{
		if (State != 0)
		{
			if (Value)
			{
				m_State |= State;
			}
			else
			{
				m_State &= ~State;
			}
		}
	}

	public void OnNewOwner()
	{
		UpdateCards();
	}

	public void AssignModel(CFGCharacter charm)
	{
		m_CurrentModel = charm;
		RemoveBuffs(EBuffSource.Weapon);
		if (!(charm == null))
		{
		}
	}

	public void AssignDefinition(string DefName)
	{
		m_Definition = CFGStaticDataContainer.GetCharacterDefinition(DefName);
		if (m_Definition == null)
		{
			Debug.LogWarning("Failed to find character definition: " + DefName);
			return;
		}
		OnNewOwner();
		OnDifficultyChanged(CFGGame.Difficulty);
		SetState(ECharacterStateFlag.ImmuneToGunpoint, m_Definition.GunpointImmunity);
	}

	public void RestoreBaseStats()
	{
		m_BaseStats.Clear();
		m_BaseStats.m_Aim = m_Definition.Aim;
		m_BaseStats.m_MaxHealth = m_Definition.MaxHealth;
		m_BaseStats.m_Defense = m_Definition.Defense;
		m_BaseStats.m_Movement = m_Definition.Movement;
		m_BaseStats.m_Sight = m_Definition.Sight;
		m_BaseStats.m_MaxLuck = m_Definition.MaxLuck;
		m_BaseStats.m_LuckReplenishTurn = m_Definition.LuckR_Turn;
		m_BaseStats.m_LuckReplenishHit = m_Definition.LuckR_Hit;
	}

	public string GetScarsAndInjuries()
	{
		string text = string.Empty;
		int num = 0;
		if (m_Buffs != null && m_Buffs.Count > 0)
		{
			foreach (CFGBuff value in m_Buffs.Values)
			{
				if (value == null || value.m_Def == null)
				{
					continue;
				}
				switch (value.m_Def.BuffType)
				{
				case EBuffType.Scar:
					if (num > 0)
					{
						text += ",";
					}
					text += value.m_Def.BuffID;
					num++;
					break;
				case EBuffType.Wound:
					if (num > 0)
					{
						text += ",";
					}
					text += value.m_Def.SpecialEffect;
					num++;
					break;
				}
			}
		}
		return text;
	}

	public void ApplyScars(string ScarList)
	{
		string[] array = ScarList.Split(',');
		if (array == null || array.Length == 0)
		{
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			string buff_id = text.Trim();
			if (!HasBuff(buff_id))
			{
				AddBuff(buff_id, EBuffSource.Injury);
			}
		}
	}

	public bool HasHealableBuffs()
	{
		if (m_Buffs == null || m_Buffs.Count == 0)
		{
			return false;
		}
		foreach (CFGBuff value in m_Buffs.Values)
		{
			if (value == null || value.m_Def == null || !value.m_Def.Healable)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public void HealBuffs()
	{
		if (m_Buffs == null || m_Buffs.Count == 0)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		if (hashSet == null)
		{
			return;
		}
		foreach (CFGBuff value in m_Buffs.Values)
		{
			if (value != null && value.m_Def != null && value.m_Def.Healable && value.m_Def.Healable)
			{
				hashSet.Add(value.m_Def.BuffID);
			}
		}
		if (hashSet.Count == 0)
		{
			return;
		}
		foreach (string item in hashSet)
		{
			m_Buffs.Remove(item);
		}
		EvaluateBuffs();
	}

	public bool HasBuff(string buff_id)
	{
		return m_Buffs.ContainsKey(buff_id);
	}

	private void EvaluateBuffs(bool OnDeserialization = false)
	{
		int maxLuck = m_BuffedStats.m_MaxLuck;
		SetState(ECharacterStateFlag.Injured, Value: false);
		m_BuffedStats.Clear();
		m_BuffedStats.m_MaxHealth += m_DifficultyHPBonus;
		m_BuffedStats.AddStats(m_BaseStats);
		bool value = false;
		foreach (CFGBuff value2 in m_Buffs.Values)
		{
			m_BuffedStats.AddStats(value2.m_Stats);
			if (string.Compare("critical_buff", value2.m_Def.BuffID, ignoreCase: true) == 0)
			{
				value = true;
			}
			if (value2 != null && value2.m_Def != null && value2.m_Def.BuffType == EBuffType.Wound)
			{
				SetState(ECharacterStateFlag.Injured, Value: true);
			}
		}
		SetState(ECharacterStateFlag.IsCritical, value);
		for (int i = 0; i < 5; i++)
		{
			if (m_Cards[i] != null && m_Cards[i].CharStats != null)
			{
				m_BuffedStats.AddStats(m_Cards[i].CharStats);
			}
		}
		m_BuffedStats.AddStatsFromItem(Weapon1);
		m_BuffedStats.AddStatsFromItem(Weapon2);
		m_BuffedStats.AddStatsFromItem(m_Item1);
		m_BuffedStats.AddStatsFromItem(m_Item2);
		m_BuffedStats.AddStatsFromItem(m_Talisman);
		if (!OnDeserialization && m_BuffedStats.m_MaxLuck > maxLuck)
		{
			SetLuck(Luck + (m_BuffedStats.m_MaxLuck - maxLuck), bAllowSplash: false);
		}
		m_BuffedStats.m_MaxHealth = Mathf.Clamp(m_BuffedStats.m_MaxHealth, CFGCharacterStats.MAX_HELATH_MIN, CFGCharacterStats.MAX_HELATH_MAX);
		m_BuffedStats.m_Aim = Mathf.Clamp(m_BuffedStats.m_Aim, CFGCharacterStats.AIM_MIN, CFGCharacterStats.AIM_MAX);
		m_BuffedStats.m_Defense = Mathf.Clamp(m_BuffedStats.m_Defense, CFGCharacterStats.DEFENCE_MIN, CFGCharacterStats.DEFENCE_MAX);
		m_BuffedStats.m_Sight = Mathf.Clamp(m_BuffedStats.m_Sight, CFGCharacterStats.SIGHT_MIN, CFGCharacterStats.SIGHT_MAX);
		Hp = Mathf.Max(Hp, 1);
		SetLuck(Luck, bAllowSplash: false);
	}

	public List<CFGBuff> GenerateBuffList()
	{
		List<CFGBuff> list = new List<CFGBuff>();
		foreach (KeyValuePair<string, CFGBuff> buff in m_Buffs)
		{
			list.Add(buff.Value);
		}
		return list;
	}

	public void UpdateBuffs()
	{
		if (m_Buffs == null || m_Buffs.Count == 0)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		SetState(ECharacterStateFlag.Injured, Value: false);
		foreach (KeyValuePair<string, CFGBuff> buff in m_Buffs)
		{
			if (buff.Value == null || buff.Value.m_Def == null)
			{
				continue;
			}
			CheckForAchiev19(buff.Value.m_Def);
			switch (buff.Value.AutoEndType)
			{
			case EBuffAutoEndType.Never:
			case EBuffAutoEndType.EndOfTactical:
			case EBuffAutoEndType.AfterDuration:
				if (!buff.Value.m_Def.InstantApply && (buff.Value.m_Duration & 1) == 0 && m_CurrentModel != null && buff.Value.m_Def.HPChange != 0)
				{
					m_CurrentModel.SetHP(m_Hp + buff.Value.m_Def.HPChange, m_CurrentModel, bSilent: false);
				}
				buff.Value.m_Duration++;
				if (buff.Value.AutoEndType == EBuffAutoEndType.AfterDuration && buff.Value.m_Duration >= buff.Value.TotalDuration)
				{
					if (m_CurrentModel != null)
					{
						m_CurrentModel.OnEndBuff(buff.Value.m_Def, bRemoved: false);
					}
					hashSet.Add(buff.Key);
					if (buff.Value.m_Def.BuffType == EBuffType.Wound)
					{
						SetState(ECharacterStateFlag.Injured, Value: true);
					}
				}
				else if (buff.Value.m_Def.InstantApply && (buff.Value.m_Duration & 1) == 0 && m_CurrentModel != null && buff.Value.m_Def.HPChange != 0)
				{
					m_CurrentModel.SetHP(m_Hp + buff.Value.m_Def.HPChange, m_CurrentModel, bSilent: false);
				}
				break;
			}
		}
		if (hashSet.Count <= 0)
		{
			return;
		}
		foreach (string item in hashSet)
		{
			if (!(item == "jinxed"))
			{
				m_Buffs.Remove(item);
			}
		}
		EvaluateBuffs();
	}

	public void SetWeaponBuff(CFGGun Weapon)
	{
		RemoveBuffs(EBuffSource.Weapon);
		if (Weapon != null && Weapon.m_Definition != null)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(Weapon.m_Definition.ItemID);
			if (itemDefinition == null)
			{
				Debug.LogWarning("Failed to find weapon's item definition: " + Weapon.m_Definition.ItemID);
			}
			if (!string.IsNullOrEmpty(itemDefinition.Perm_Buff))
			{
				AddBuff(itemDefinition.Perm_Buff, EBuffSource.Weapon);
			}
		}
	}

	private void CheckForAchiev19(CFGDef_Buff buff)
	{
		if (buff != null && buff.BuffType == EBuffType.ItemBuff && (buff.HPChange >= 1 || buff.Mod_MaxHP >= 1))
		{
			CFGAchievmentTracker.EqualizationUsedInFirstTurn = false;
		}
	}

	public void AddBuff(CFGDef_Buff buff, EBuffSource Source)
	{
		if (buff == null)
		{
			return;
		}
		if (ForbiddenBuffs_IsInTheList(buff.BuffID))
		{
			if (m_CurrentModel != null)
			{
				Vector3 position = m_CurrentModel.Position;
				position.y += 1.9f;
				string val = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("protected_from") + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(buff.BuffID);
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, val, plus: false, 1, -1, m_CurrentModel);
			}
			return;
		}
		CheckForAchiev19(buff);
		if (string.Compare(buff.BuffID, "critical_buff", ignoreCase: true) == 0)
		{
			SetState(ECharacterStateFlag.IsCritical, Value: true);
		}
		CFGBuff value = null;
		if (m_Buffs.TryGetValue(buff.BuffID, out value))
		{
			value.m_Duration = 0;
			return;
		}
		m_Buffs.Add(buff.BuffID, new CFGBuff(buff, Source));
		if (m_CurrentModel != null)
		{
			m_CurrentModel.OnStartBuff(buff);
		}
		EvaluateBuffs();
		if (buff.Mod_MaxHP > 0)
		{
			Hp += buff.Mod_MaxHP;
		}
		if (buff.InstantApply)
		{
			if (buff.LuckChange != 0)
			{
				SetLuck(Luck + buff.LuckChange, bAllowSplash: true);
			}
			if (buff.HPChange != 0)
			{
				if (m_CurrentModel != null)
				{
					if (buff.HPChange > 0)
					{
						m_CurrentModel.Heal(buff.HPChange, bSilent: false);
					}
					else
					{
						m_CurrentModel.TakeDamage(-buff.HPChange, null, bSilent: false);
					}
				}
				else
				{
					Hp += buff.HPChange;
				}
			}
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && m_Definition != null && buff.BuffID != "critical_buff" && buff.BuffType != EBuffType.HandBuff && PositionInTeam >= 0)
		{
			string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_character_buff", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Definition.NameID), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(buff.BuffID));
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText, 0, buff.Icon);
		}
	}

	public void AddBuff(string buff_id, EBuffSource Source)
	{
		AddBuff(CFGStaticDataContainer.GetBuff(buff_id), Source);
	}

	public void AddItemBuff(string ItemID, EBuffSource source)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition != null && !string.IsNullOrEmpty(itemDefinition.Perm_Buff))
		{
			AddBuff(itemDefinition.Perm_Buff, source);
		}
	}

	public void RemBuff(string id)
	{
		if (m_Buffs.ContainsKey(id))
		{
			m_Buffs.Remove(id);
			CFGDef_Buff buff = CFGStaticDataContainer.GetBuff(id);
			UpdateHPOnBuffRemove(buff);
			if (m_CurrentModel != null)
			{
				m_CurrentModel.OnEndBuff(buff, bRemoved: true);
			}
			EvaluateBuffs();
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && m_Definition != null && buff.BuffID != "critical_buff" && buff.BuffType != EBuffType.HandBuff)
			{
				string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_character_buff_lost", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Definition.NameID), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(buff.BuffID));
				CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText, 0, buff.Icon);
			}
		}
	}

	private void UpdateHPOnBuffRemove(CFGDef_Buff buff)
	{
		if (buff == null)
		{
			return;
		}
		if (buff.Mod_MaxHP != 0)
		{
			if (buff.Mod_MaxHP > 0)
			{
				Hp = Hp;
			}
			else
			{
				Hp += buff.Mod_MaxHP;
			}
		}
		if (buff.Mod_MaxLuck > 0)
		{
			SetLuck(Luck - buff.Mod_MaxLuck, bAllowSplash: false);
		}
	}

	public void RemoveBuffs(EBuffSource Source)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, CFGBuff> buff in m_Buffs)
		{
			if (buff.Value.m_Source == Source)
			{
				hashSet.Add(buff.Key);
				if (m_CurrentModel != null)
				{
					m_CurrentModel.OnEndBuff(buff.Value.m_Def, bRemoved: true);
				}
				UpdateHPOnBuffRemove(buff.Value.m_Def);
			}
		}
		if (hashSet.Count <= 0)
		{
			return;
		}
		foreach (string item in hashSet)
		{
			m_Buffs.Remove(item);
		}
		EvaluateBuffs();
	}

	public bool CanApplyBuff(string BuffID)
	{
		if (!CanApplyBufWithItem(Weapon1, BuffID))
		{
			return false;
		}
		if (!CanApplyBufWithItem(Weapon2, BuffID))
		{
			return false;
		}
		if (!CanApplyBufWithItem(Talisman, BuffID))
		{
			return false;
		}
		if (!CanApplyBufWithItem(Item1, BuffID))
		{
			return false;
		}
		if (!CanApplyBufWithItem(Item2, BuffID))
		{
			return false;
		}
		return true;
	}

	public static bool CanApplyBufWithItem(string ItemID, string BuffID)
	{
		if (ItemID == null)
		{
			return true;
		}
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition == null)
		{
			return true;
		}
		if (itemDefinition.ProtectsAgainstBuff(BuffID))
		{
			return false;
		}
		return true;
	}

	public void ForbiddenBuffs_Recalculate()
	{
		m_ForbiddenBuffs = new HashSet<string>();
		ForbiddenBuffs_HandleItem(Weapon1);
		ForbiddenBuffs_HandleItem(Weapon2);
		ForbiddenBuffs_HandleItem(m_Item1);
		ForbiddenBuffs_HandleItem(m_Item2);
		ForbiddenBuffs_HandleItem(m_Talisman);
		EvaluateBuffs();
	}

	private void ForbiddenBuffs_HandleItem(string ItemID)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition == null || itemDefinition.m_ForbiddenBuffs == null || itemDefinition.m_ForbiddenBuffs.Count == 0)
		{
			return;
		}
		foreach (string forbiddenBuff in itemDefinition.m_ForbiddenBuffs)
		{
			string item = forbiddenBuff.ToLower();
			if (!m_ForbiddenBuffs.Contains(item))
			{
				m_ForbiddenBuffs.Add(item);
			}
		}
	}

	public bool ForbiddenBuffs_IsInTheList(string BuffID)
	{
		if (m_ForbiddenBuffs == null || m_ForbiddenBuffs.Count == 0 || BuffID == null)
		{
			return false;
		}
		string item = BuffID.ToLower();
		return m_ForbiddenBuffs.Contains(item);
	}

	private void OnItemRemoved(EItemSlot slot)
	{
		string text = null;
		switch (slot)
		{
		case EItemSlot.Item1:
			text = Item1;
			break;
		case EItemSlot.Item2:
			text = Item2;
			break;
		case EItemSlot.Weapon1:
			text = Weapon1;
			break;
		case EItemSlot.Weapon2:
			text = Weapon2;
			break;
		case EItemSlot.Talisman:
			text = Talisman;
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(text);
			if (itemDefinition != null && itemDefinition.Mod_MaxLuck > 0)
			{
				SetLuck(Luck - itemDefinition.Mod_MaxLuck, bAllowSplash: false);
			}
		}
	}

	public void EquipItem(EItemSlot slot, string ItemID, bool bTemporary = false)
	{
		if (string.IsNullOrEmpty(ItemID))
		{
			ItemID = string.Empty;
		}
		OnItemRemoved(slot);
		switch (slot)
		{
		case EItemSlot.Item1:
		{
			RemoveBuffs(EBuffSource.Item1);
			m_Item1 = ItemID;
			AddItemBuff(ItemID, EBuffSource.Item1);
			CFGDef_UsableItem usableItemDef2 = CFGStaticDataContainer.GetUsableItemDef(m_Item1);
			if (usableItemDef2 == null)
			{
				RemoveAbility(ETurnAction.Use_Item1, EAbilitySource.Item);
			}
			else
			{
				AddAbility(ETurnAction.Use_Item1, EAbilitySource.Item, 0, usableItemDef2.UseLimit);
			}
			break;
		}
		case EItemSlot.Item2:
		{
			RemoveBuffs(EBuffSource.Item2);
			m_Item2 = ItemID;
			AddItemBuff(ItemID, EBuffSource.Item2);
			CFGDef_UsableItem usableItemDef = CFGStaticDataContainer.GetUsableItemDef(m_Item2);
			if (usableItemDef == null)
			{
				RemoveAbility(ETurnAction.Use_Item2, EAbilitySource.Item);
			}
			else
			{
				AddAbility(ETurnAction.Use_Item2, EAbilitySource.Item, 0, usableItemDef.UseLimit);
			}
			break;
		}
		case EItemSlot.Talisman:
		{
			RemoveBuffs(EBuffSource.Talisman);
			m_Talisman = ItemID;
			AddItemBuff(ItemID, EBuffSource.Talisman);
			CFGDef_UsableItem usableItemDef3 = CFGStaticDataContainer.GetUsableItemDef(m_Talisman);
			if (usableItemDef3 == null)
			{
				RemoveAbility(ETurnAction.Use_Talisman, EAbilitySource.Item);
			}
			else
			{
				AddAbility(ETurnAction.Use_Talisman, EAbilitySource.Item, 0, usableItemDef3.UseLimit);
			}
			break;
		}
		case EItemSlot.Weapon1:
			RemoveBuffs(EBuffSource.Weapon);
			if ((bool)m_Gun1)
			{
				m_Gun1.SetDefinition(ItemID);
				m_Gun1.m_bTemporary = bTemporary;
			}
			break;
		case EItemSlot.Weapon2:
			RemoveBuffs(EBuffSource.Weapon);
			if ((bool)m_Gun2)
			{
				m_Gun2.SetDefinition(ItemID);
				m_Gun2.m_bTemporary = bTemporary;
			}
			break;
		}
		EvaluateBuffs();
		RecalculateTotalHeat();
		ForbiddenBuffs_Recalculate();
	}

	public void AssignDefItems()
	{
		if (m_Definition != null)
		{
			m_BaseStats.m_Aim = m_Definition.Aim;
			m_BaseStats.m_MaxHealth = m_Definition.MaxHealth;
			m_BaseStats.m_Defense = m_Definition.Defense;
			m_BaseStats.m_Movement = m_Definition.Movement;
			m_BaseStats.m_Sight = m_Definition.Sight;
			m_BaseStats.m_MaxLuck = m_Definition.MaxLuck;
			m_BaseStats.m_LuckReplenishTurn = m_Definition.LuckR_Turn;
			m_BaseStats.m_LuckReplenishHit = m_Definition.LuckR_Hit;
			RemoveBuffs(EBuffSource.Weapon);
			EquipItem(EItemSlot.Item1, m_Definition.Item1);
			EquipItem(EItemSlot.Item2, m_Definition.Item2);
			EquipItem(EItemSlot.Talisman, m_Definition.Talisman);
			EquipItem(EItemSlot.Weapon1, m_Definition.Weapon1);
			EquipItem(EItemSlot.Weapon2, m_Definition.Weapon2);
		}
	}

	public void OnStartTurn()
	{
		int num = m_BuffedStats.m_LuckReplenishTurn;
		if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.LuckRegeneration))
		{
			num += CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_mode_constant_luck_regen_amount;
		}
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.Turn == 0)
		{
			SetLuck(Luck + num, bAllowSplash: false);
		}
		else
		{
			SetLuck(Luck + num, bAllowSplash: true);
		}
	}

	public void OnEndTurn()
	{
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_Abilities)
		{
			if (ability.Value != null && ability.Value.Ability != null)
			{
				ability.Value.Ability.OnEndTurn();
			}
		}
	}

	public void OnEndTactical()
	{
		HashSet<string> hashSet = new HashSet<string>();
		HashSet<CFGDef_Buff> hashSet2 = new HashSet<CFGDef_Buff>();
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_Abilities)
		{
			if (ability.Value != null && ability.Value.Ability != null)
			{
				ability.Value.Ability.OnEndTactical();
			}
		}
		foreach (KeyValuePair<string, CFGBuff> buff in m_Buffs)
		{
			switch (buff.Value.AutoEndType)
			{
			case EBuffAutoEndType.EndOfTactical:
			case EBuffAutoEndType.AfterDuration:
				hashSet.Add(buff.Key);
				break;
			case EBuffAutoEndType.AfterTacticals:
			{
				bool flag = false;
				if (buff.Value.m_Def != null && buff.Value.m_Def.BuffType == EBuffType.Wound)
				{
					flag = true;
				}
				buff.Value.m_Duration += 2;
				if (flag && CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.FastScars))
				{
					buff.Value.m_Duration = buff.Value.TotalDuration + 666;
				}
				if (buff.Value.m_Duration < buff.Value.TotalDuration)
				{
					break;
				}
				hashSet.Add(buff.Key);
				if (flag)
				{
					if (string.IsNullOrEmpty(buff.Value.m_Def.SpecialEffect))
					{
						Debug.LogWarning("Wound buff has no scar buff: " + buff.Value.m_Def.BuffID);
					}
					else
					{
						hashSet2.Add(buff.Value.m_Def);
					}
				}
				break;
			}
			}
		}
		if (hashSet.Count > 0)
		{
			foreach (string item in hashSet)
			{
				if (string.Compare(item, "demonpower", ignoreCase: true) == 0)
				{
					SetState(ECharacterStateFlag.InDemonForm, Value: false);
				}
				m_Buffs.Remove(item);
			}
		}
		if (hashSet2 != null && hashSet2.Count > 0 && PositionInTeam >= 0)
		{
			foreach (CFGDef_Buff item2 in hashSet2)
			{
				AddBuff(item2.SpecialEffect, EBuffSource.Injury);
				if (CFGSingleton<CFGWindowMgr>.Instance != null && m_Definition != null)
				{
					string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_character_injuryhealed", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Definition.NameID), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item2.BuffID), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(item2.SpecialEffect));
					CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(localizedText, 0, item2?.Icon ?? 0);
				}
			}
		}
		if ((bool)Gun1)
		{
			Gun1.RemoveVisualisation();
			if (Gun1.m_bTemporary)
			{
				Gun1.SetDefinition(null);
			}
			else
			{
				Gun1.CurrentAmmo = Gun1.AmmoCapacity;
			}
		}
		if ((bool)Gun2)
		{
			Gun2.RemoveVisualisation();
			if (Gun2.m_bTemporary)
			{
				Gun2.SetDefinition(null);
			}
			else
			{
				Gun2.CurrentAmmo = Gun2.AmmoCapacity;
			}
		}
		if (hashSet.Count > 0 || (hashSet2 != null && hashSet2.Count > 0))
		{
			EvaluateBuffs();
		}
	}

	public void EquipDefaultWeapon()
	{
		if (IsStateSet(ECharacterStateFlag.Imprisoned))
		{
			return;
		}
		string value = null;
		string value2 = null;
		if ((bool)m_Gun1 && m_Gun1.m_Definition != null)
		{
			value = m_Gun1.m_Definition.ItemID;
		}
		if ((bool)m_Gun2 && m_Gun2.m_Definition != null)
		{
			value2 = m_Gun2.m_Definition.ItemID;
		}
		if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value2))
		{
			EquipItem(EItemSlot.Weapon1, "revolver_coltarmyrusty");
			if ((bool)Gun1)
			{
				Gun1.m_bTemporary = true;
			}
			value = "revolver_coltarmyrusty";
		}
		if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value2))
		{
		}
	}

	public void SwapWeapons()
	{
		CFGGun gun = m_Gun1;
		m_Gun1 = m_Gun2;
		m_Gun2 = gun;
	}

	public void SetCard(int SlotID, CFGDef_Card Card)
	{
		if (SlotID >= 0 && m_Cards != null && SlotID < m_Cards.Length)
		{
			if (Card == null)
			{
				RemoveCard(SlotID);
				return;
			}
			m_Cards[SlotID] = Card;
			OnCardAdded(Card);
		}
	}

	private void OnCardAdded(CFGDef_Card card)
	{
		if (card != null)
		{
			if (card.AbilityID != ETurnAction.None)
			{
				AddAbility(card.AbilityID, EAbilitySource.Card, -1);
			}
			UpdateCards();
			if (card.MaxHealth > 0)
			{
				Hp += card.MaxHealth;
			}
		}
	}

	public void RemoveCard(int SlotID)
	{
		if (SlotID >= 0 && m_Cards != null && SlotID < m_Cards.Length && m_Cards[SlotID] != null)
		{
			CFGDef_Card card = m_Cards[SlotID];
			m_Cards[SlotID] = null;
			OnCardRemoved(card);
		}
	}

	private void OnCardRemoved(CFGDef_Card card)
	{
		if (card != null)
		{
			if (card.AbilityID != ETurnAction.None)
			{
				RemoveAbility(card.AbilityID, EAbilitySource.Card);
			}
			if (card.MaxLuck > 0)
			{
				SetLuck(Luck - card.MaxLuck, bAllowSplash: false);
			}
			UpdateCards();
		}
	}

	private void UpdateCards(bool OnDeserialization = false)
	{
		EvaluateBuffs(OnDeserialization);
		string strA = null;
		foreach (KeyValuePair<string, CFGBuff> buff in m_Buffs)
		{
			if (buff.Value == null || buff.Value.m_Source != EBuffSource.CardHandBonus)
			{
				continue;
			}
			strA = buff.Key;
			break;
		}
		m_CardHandBonus = CalcHandBonus(ref m_Cards);
		if (m_CardHandBonus == ECardHandBonus.FiveOfAKind)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_29);
		}
		if (CFGGame.DlcType == EDLC.DLC1)
		{
			switch (m_CardHandBonus)
			{
			case ECardHandBonus.DLC_Leve1:
				m_DecayFromCards = 1;
				break;
			case ECardHandBonus.DLC_Leve2:
				m_DecayFromCards = 2;
				break;
			case ECardHandBonus.DLC_Leve3:
				m_DecayFromCards = 3;
				break;
			case ECardHandBonus.DLC_Leve4:
				m_DecayFromCards = 4;
				break;
			case ECardHandBonus.DLC_Leve5:
				m_DecayFromCards = 5;
				break;
			case ECardHandBonus.DLC_Cursed:
			case ECardHandBonus.DLC_Ubermensh:
			case ECardHandBonus.DLC_UniversalSoldier:
			case ECardHandBonus.DLC_Monster:
			case ECardHandBonus.DLC_Zombie:
				m_DecayFromCards = 5;
				break;
			}
		}
		string handBonusBuffName = GetHandBonusBuffName(m_CardHandBonus);
		bool flag = false;
		if (handBonusBuffName != null)
		{
			if (string.Compare(strA, handBonusBuffName, ignoreCase: true) != 0)
			{
				flag = true;
			}
		}
		else
		{
			RemoveBuffs(EBuffSource.CardHandBonus);
		}
		if (flag)
		{
			RemoveBuffs(EBuffSource.CardHandBonus);
			AddBuff(handBonusBuffName, EBuffSource.CardHandBonus);
		}
		EvaluateBuffs(OnDeserialization);
	}

	public static string GetHandBonusBuffName(ECardHandBonus hand_bonus)
	{
		return hand_bonus switch
		{
			ECardHandBonus.HighCard => "hand_highcard", 
			ECardHandBonus.Pair => "hand_pair", 
			ECardHandBonus.TwoPairs => "hand_twopairs", 
			ECardHandBonus.Straight => "hand_straight", 
			ECardHandBonus.ThreeOfAKind => "hand_threeofakind", 
			ECardHandBonus.FullHouse => "hand_fullhouse", 
			ECardHandBonus.Flush => "hand_flush", 
			ECardHandBonus.FourOfAKind => "hand_fourofakind", 
			ECardHandBonus.StraightFlush => "hand_straightflush", 
			ECardHandBonus.RoyalFlush => "hand_royalflush", 
			ECardHandBonus.FiveOfAKind => "hand_fiveofakind", 
			ECardHandBonus.DLC_Leve1 => "dlcbuff_hand_1", 
			ECardHandBonus.DLC_Leve2 => "dlcbuff_hand_2", 
			ECardHandBonus.DLC_Leve3 => "dlcbuff_hand_3", 
			ECardHandBonus.DLC_Leve4 => "dlcbuff_hand_4", 
			ECardHandBonus.DLC_Leve5 => "dlcbuff_hand_5", 
			ECardHandBonus.DLC_Cursed => "dlcbuff_hand_cursed", 
			ECardHandBonus.DLC_Ubermensh => "dlcbuff_hand_uber", 
			ECardHandBonus.DLC_UniversalSoldier => "dlcbuff_hand_universal", 
			ECardHandBonus.DLC_Monster => "dlcbuff_hand_monster", 
			ECardHandBonus.DLC_Zombie => "dlcbuff_hand_zombie", 
			_ => null, 
		};
	}

	public static CFGDef_Buff GetBuffHandBonus(ECardHandBonus hand_bonus)
	{
		string handBonusBuffName = GetHandBonusBuffName(hand_bonus);
		if (handBonusBuffName == null)
		{
			return null;
		}
		return CFGStaticDataContainer.GetBuff(handBonusBuffName);
	}

	public static bool HasCard(string CardID, ref CFGDef_Card[] CardArr)
	{
		if (CardArr == null)
		{
			return false;
		}
		for (int i = 0; i < CardArr.Length; i++)
		{
			if (CardArr[i] != null && string.Compare(CardArr[i].CardID, CardID, ignoreCase: true) == 0)
			{
				return true;
			}
		}
		return false;
	}

	private static ECardHandBonus DLC1_CalcHandBonus(ref CFGDef_Card[] CardArr)
	{
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			if (CardArr[i] != null)
			{
				num++;
			}
		}
		if (num < 2)
		{
			return ECardHandBonus.None;
		}
		if (HasCard("cannibal_stomach_dlc1", ref CardArr) && HasCard("brain_dlc1", ref CardArr) && HasCard("teeth_dlc1", ref CardArr) && HasCard("skin_dlc1", ref CardArr) && HasCard("blood_dlc1", ref CardArr))
		{
			return ECardHandBonus.DLC_Cursed;
		}
		if (HasCard("bulls_heart_dlc1", ref CardArr) && HasCard("heart_dlc1", ref CardArr) && HasCard("left_lung_dlc1", ref CardArr) && HasCard("mesmer_eye_dlc1", ref CardArr) && HasCard("eye_dlc1", ref CardArr))
		{
			return ECardHandBonus.DLC_Ubermensh;
		}
		if (HasCard("ear_dlc1", ref CardArr) && HasCard("arm_dlc1", ref CardArr) && HasCard("chest_dlc1", ref CardArr) && HasCard("stomach_dlc1", ref CardArr) && HasCard("kidney_dlc1", ref CardArr))
		{
			return ECardHandBonus.DLC_UniversalSoldier;
		}
		if (HasCard("tail_dlc1", ref CardArr) && HasCard("unidentified_dlc1", ref CardArr) && HasCard("third_eye_dlc1", ref CardArr) && HasCard("sting_dlc1", ref CardArr) && HasCard("spine_dlc1", ref CardArr))
		{
			return ECardHandBonus.DLC_Monster;
		}
		if (HasCard("liver_dlc1", ref CardArr) && HasCard("hand_dlc1", ref CardArr) && HasCard("right_lung_dlc1", ref CardArr) && HasCard("tongue_dlc1", ref CardArr) && HasCard("lymph_dlc1", ref CardArr))
		{
			return ECardHandBonus.DLC_Zombie;
		}
		byte[] SumTable = new byte[26];
		for (int j = 0; j < 5; j++)
		{
			if (CardArr[j] != null)
			{
				CardArr[j].UpdateHand_DLC1(ref SumTable);
			}
		}
		int num2 = 0;
		for (int k = 0; k < 26; k++)
		{
			if (SumTable[k] > num2)
			{
				num2 = SumTable[k];
			}
		}
		if (num2 < 2)
		{
			return ECardHandBonus.DLC_Leve1;
		}
		if (num2 > 4)
		{
			return ECardHandBonus.DLC_Leve5;
		}
		return num2 switch
		{
			2 => ECardHandBonus.DLC_Leve2, 
			3 => ECardHandBonus.DLC_Leve3, 
			_ => ECardHandBonus.DLC_Leve4, 
		};
	}

	public static ECardHandBonus CalcHandBonus(ref CFGDef_Card[] CardArr)
	{
		if (CFGGame.DlcType == EDLC.DLC1)
		{
			return DLC1_CalcHandBonus(ref CardArr);
		}
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			if (CardArr[i] != null)
			{
				num++;
			}
		}
		if (num < 2)
		{
			return ECardHandBonus.None;
		}
		int cardCount = GetCardCount(ECardRank.Rank_9, ref CardArr);
		int cardCount2 = GetCardCount(ECardRank.Rank_10, ref CardArr);
		int cardCount3 = GetCardCount(ECardRank.Rank_J, ref CardArr);
		int cardCount4 = GetCardCount(ECardRank.Rank_Q, ref CardArr);
		int cardCount5 = GetCardCount(ECardRank.Rank_K, ref CardArr);
		int cardCount6 = GetCardCount(ECardRank.Rank_A, ref CardArr);
		int cardCount7 = GetCardCount(ECardRank.Rank_Joker, ref CardArr);
		int countByColor = GetCountByColor(ECardColor.Clubs, ref CardArr);
		int countByColor2 = GetCountByColor(ECardColor.Diamonds, ref CardArr);
		int countByColor3 = GetCountByColor(ECardColor.Hearts, ref CardArr);
		int countByColor4 = GetCountByColor(ECardColor.Spades, ref CardArr);
		if (cardCount6 == 4 || cardCount5 == 4 || cardCount4 == 4 || cardCount3 == 4 || cardCount2 == 4 || cardCount == 4)
		{
			if (cardCount7 == 1)
			{
				return ECardHandBonus.FiveOfAKind;
			}
			return ECardHandBonus.FourOfAKind;
		}
		if (countByColor == 5 || countByColor2 == 5 || countByColor3 == 5 || countByColor4 == 5)
		{
			if (cardCount == 1 && cardCount6 == 1)
			{
				return ECardHandBonus.Flush;
			}
			if (cardCount == 1)
			{
				return ECardHandBonus.StraightFlush;
			}
			return ECardHandBonus.RoyalFlush;
		}
		if (cardCount7 == 1 && (countByColor == 4 || countByColor2 == 4 || countByColor3 == 4 || countByColor4 == 4))
		{
			if (cardCount == 1 && cardCount6 == 1)
			{
				return ECardHandBonus.Flush;
			}
			if (cardCount == 1)
			{
				return ECardHandBonus.StraightFlush;
			}
			return ECardHandBonus.RoyalFlush;
		}
		bool flag = cardCount == 3 || cardCount2 == 3 || cardCount3 == 3 || cardCount5 == 3 || cardCount6 == 3 || cardCount4 == 3;
		if (cardCount7 == 2)
		{
			if (countByColor == 3 || countByColor2 == 3 || countByColor3 == 3 || countByColor4 == 3)
			{
				if (cardCount == 1 && cardCount6 == 1)
				{
					return ECardHandBonus.Flush;
				}
				if (cardCount == 1)
				{
					return ECardHandBonus.StraightFlush;
				}
				return ECardHandBonus.RoyalFlush;
			}
			if (cardCount2 < 2 && cardCount3 < 2 && cardCount4 < 2 && cardCount5 < 2 && cardCount6 < 2 && cardCount < 2)
			{
				int num2 = cardCount2 + cardCount3 + cardCount + cardCount4 + cardCount5 + cardCount6;
				if (num2 == 3)
				{
					if (cardCount == 1 && cardCount6 == 0)
					{
						return ECardHandBonus.Straight;
					}
					if (cardCount == 0 && cardCount6 == 1)
					{
						return ECardHandBonus.Straight;
					}
				}
			}
		}
		int num3 = 0;
		if (cardCount == 2)
		{
			num3++;
		}
		if (cardCount2 == 2)
		{
			num3++;
		}
		if (cardCount3 == 2)
		{
			num3++;
		}
		if (cardCount4 == 2)
		{
			num3++;
		}
		if (cardCount5 == 2)
		{
			num3++;
		}
		if (cardCount6 == 2)
		{
			num3++;
		}
		switch (cardCount7)
		{
		case 0:
			if (cardCount == 1 && cardCount2 == 1 && cardCount3 == 1 && cardCount4 == 1 && cardCount5 == 1)
			{
				return ECardHandBonus.Straight;
			}
			if (cardCount2 == 1 && cardCount3 == 1 && cardCount4 == 1 && cardCount5 == 1 && cardCount6 == 1)
			{
				return ECardHandBonus.Straight;
			}
			if (flag)
			{
				if (num3 == 1)
				{
					return ECardHandBonus.FullHouse;
				}
				return ECardHandBonus.ThreeOfAKind;
			}
			switch (num3)
			{
			case 2:
				return ECardHandBonus.TwoPairs;
			case 1:
				return ECardHandBonus.Pair;
			default:
				if (num < 3)
				{
					return ECardHandBonus.None;
				}
				return ECardHandBonus.HighCard;
			}
		case 1:
			if (flag)
			{
				return ECardHandBonus.FourOfAKind;
			}
			switch (num3)
			{
			case 2:
				return ECardHandBonus.FullHouse;
			case 1:
				return ECardHandBonus.ThreeOfAKind;
			default:
				if (cardCount2 < 2 && cardCount3 < 2 && cardCount4 < 2 && cardCount5 < 2 && cardCount6 < 2 && cardCount < 2)
				{
					int num4 = cardCount2 + cardCount3 + cardCount + cardCount4 + cardCount5 + cardCount6;
					if (num4 == 4)
					{
						if (cardCount == 1 && cardCount6 == 0)
						{
							return ECardHandBonus.Straight;
						}
						if (cardCount == 0 && cardCount6 == 1)
						{
							return ECardHandBonus.Straight;
						}
					}
				}
				return ECardHandBonus.Pair;
			}
		default:
			if (flag)
			{
				return ECardHandBonus.FiveOfAKind;
			}
			if (num3 == 1)
			{
				return ECardHandBonus.FourOfAKind;
			}
			if (num > 2)
			{
				return ECardHandBonus.ThreeOfAKind;
			}
			return ECardHandBonus.Pair;
		}
	}

	private static int GetCardCount(ECardRank rank, ref CFGDef_Card[] CardArr)
	{
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			if (CardArr[i] != null && CardArr[i].CardRank == rank)
			{
				num++;
			}
		}
		return num;
	}

	private static int GetCountByColor(ECardColor ccolor, ref CFGDef_Card[] CardArr)
	{
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			if (CardArr[i] != null && CardArr[i].CardColor == ccolor)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasCard(CFGDef_Card card)
	{
		if (card == null)
		{
			return false;
		}
		for (int i = 0; i < 5; i++)
		{
			if (m_Cards[i] == card)
			{
				return true;
			}
		}
		return false;
	}

	public int GetFirstFreeCardSlot()
	{
		for (int i = 0; i < 5; i++)
		{
			if (m_Cards[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public void UpdateAbilities()
	{
		HashSet<ETurnAction> hashSet = null;
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_Abilities)
		{
			bool flag = false;
			if (ability.Value == null)
			{
				flag = true;
			}
			else if (ability.Value.Ability != null)
			{
				flag = ability.Value.Ability.ShouldRemove();
			}
			if (flag)
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<ETurnAction>();
				}
				if (hashSet == null)
				{
					return;
				}
				hashSet.Add(ability.Key);
			}
		}
		if (hashSet == null)
		{
			return;
		}
		foreach (ETurnAction item in hashSet)
		{
			switch (item)
			{
			case ETurnAction.Use_Item1:
				EquipItem(EItemSlot.Item1, null);
				break;
			case ETurnAction.Use_Item2:
				EquipItem(EItemSlot.Item2, null);
				break;
			case ETurnAction.Use_Talisman:
				EquipItem(EItemSlot.Talisman, null);
				break;
			default:
				m_Abilities.Remove(item);
				break;
			}
		}
	}

	public bool HasAbility(ETurnAction Ability)
	{
		if (m_Abilities == null)
		{
			return false;
		}
		if (m_Abilities.ContainsKey(Ability))
		{
			return true;
		}
		return false;
	}

	public void AddCardsFromAbilities()
	{
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_Abilities)
		{
			AddCardFromAbility(ability.Key);
		}
	}

	private bool AddCardFromAbility(ETurnAction action)
	{
		CFGDef_Card cardWithAbility = CFGStaticDataContainer.GetCardWithAbility(action);
		if (cardWithAbility == null)
		{
			return false;
		}
		if (HasCard(cardWithAbility))
		{
			return false;
		}
		int firstFreeCardSlot = GetFirstFreeCardSlot();
		if (firstFreeCardSlot < 0)
		{
			Debug.LogWarning("Cannot add card for LD ability:  " + cardWithAbility.CardID + " reason: No free card slots!");
			return false;
		}
		if (CFGInventory.IsCardInUse(cardWithAbility.CardID))
		{
			Debug.LogWarning("Card for LD ability:  " + cardWithAbility.CardID + " is already in use by other character!");
		}
		else
		{
			if (m_Definition != null)
			{
				Debug.Log(string.Concat("Adding ability's card: ", cardWithAbility.CardID, " (", action, ") to ", m_Definition.NameID, " slot = ", firstFreeCardSlot));
			}
			m_Cards[firstFreeCardSlot] = cardWithAbility;
			CFGInventory.CollectCard(cardWithAbility.CardID, ECardStatus.InUse, bForceSilent: true);
		}
		return true;
	}

	public void AddAbilityWithCard(ETurnAction action, EAbilitySource Source = EAbilitySource.CharDefinition)
	{
		if (action != ETurnAction.None)
		{
			bool flag = HasAbility(action);
			EAbilitySource aSource = EAbilitySource.CharDefinition;
			if (m_InTeamPos >= 0 && AddCardFromAbility(action))
			{
				aSource = EAbilitySource.Card;
			}
			if (!flag)
			{
				AddAbility(action, aSource, 0);
			}
		}
	}

	public void AddLDAbilities()
	{
		if (m_Definition != null)
		{
			AddAbilityWithCard(m_Definition.Ability1);
			AddAbilityWithCard(m_Definition.Ability2);
			AddAbilityWithCard(m_Definition.Ability3);
			AddAbilityWithCard(m_Definition.Ability4);
			AddAbilityWithCard(m_Definition.Ability5);
			EvaluateBuffs();
		}
	}

	public void RemoveAbility(ETurnAction AType, EAbilitySource ASource)
	{
		CAbilityInfo value = null;
		if (!m_Abilities.TryGetValue(AType, out value))
		{
			return;
		}
		if (ASource == EAbilitySource.Any)
		{
			m_Abilities.Remove(AType);
		}
		else if ((value.Source & ASource) == ASource)
		{
			value.Source &= ~ASource;
			if (value.Source == EAbilitySource.Unknown)
			{
				m_Abilities.Remove(AType);
			}
		}
	}

	public void AddAbility(ETurnAction AType, EAbilitySource ASource, int cooldown, int limit = int.MinValue)
	{
		if (AType == ETurnAction.None || ASource == EAbilitySource.Unknown)
		{
			return;
		}
		switch (AType)
		{
		case ETurnAction.Move:
		case ETurnAction.Shoot:
		case ETurnAction.End:
		case ETurnAction.Use:
		case ETurnAction.Reload:
		case ETurnAction.ChangeWeapon:
		case ETurnAction.OpenDoor:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			Debug.LogWarning("Cannot add ability: " + AType);
			return;
		}
		CAbilityInfo value = null;
		if (!m_Abilities.TryGetValue(AType, out value))
		{
			value = new CAbilityInfo();
			if (value != null)
			{
				value.Source = ASource;
				m_Abilities.Add(AType, value);
			}
			switch (AType)
			{
			case ETurnAction.Use_Item1:
				OnAddAbility(AType, cooldown, limit, m_Item1);
				break;
			case ETurnAction.Use_Item2:
				OnAddAbility(AType, cooldown, limit, m_Item2);
				break;
			case ETurnAction.Use_Talisman:
				OnAddAbility(AType, cooldown, limit, m_Talisman);
				break;
			default:
				OnAddAbility(AType, cooldown, limit, string.Empty);
				break;
			}
		}
		else
		{
			value.Source |= ASource;
		}
	}

	private void OnAddAbility(ETurnAction Ability, int cooldown = 0, int limit = int.MinValue, string name = "")
	{
		CAbilityInfo value = null;
		if (!m_Abilities.TryGetValue(Ability, out value) || value.Ability != null)
		{
			return;
		}
		if (Ability.IsStandard())
		{
			CFGAbility cFGAbility = new CFGAbility_Simple(Ability, limit, cooldown);
			if (cFGAbility == null)
			{
				return;
			}
			cFGAbility.Init(m_CurrentModel);
			value.Ability = cFGAbility;
			if (m_CurrentModel != null)
			{
				switch (Ability)
				{
				case ETurnAction.Jinx:
					m_CurrentModel.ApplyJinx();
					break;
				case ETurnAction.Intimidate:
					m_CurrentModel.UpdateIntimidate();
					break;
				}
			}
			return;
		}
		switch (Ability)
		{
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
		{
			if (string.IsNullOrEmpty(name))
			{
				break;
			}
			CFGDef_UsableItem usableItemDef = CFGStaticDataContainer.GetUsableItemDef(name);
			if (usableItemDef == null)
			{
				Debug.LogWarning("Failed to find usable item definition: " + name);
				break;
			}
			CFGAbility_Item cFGAbility_Item = new CFGAbility_Item(usableItemDef, limit);
			if (cFGAbility_Item != null)
			{
				cFGAbility_Item.Init(m_CurrentModel);
				value.Ability = cFGAbility_Item;
			}
			break;
		}
		}
	}

	private void RecalculateTotalHeat()
	{
		if (m_Definition == null)
		{
			m_TotalHeat = 0;
			return;
		}
		m_TotalHeat = m_Definition.Heat;
		int a = 0;
		int b = 0;
		if ((bool)Gun1 && Gun1.m_Definition != null)
		{
			a = Gun1.m_Definition.Heat;
		}
		if ((bool)Gun2 && Gun2.m_Definition != null)
		{
			b = Gun2.m_Definition.Heat;
		}
		m_TotalHeat += Mathf.Max(a, b);
		m_TotalHeat += ItemHeatMod(m_Item1);
		m_TotalHeat += ItemHeatMod(m_Item2);
		m_TotalHeat += ItemHeatMod(m_Talisman);
	}

	private int ItemHeatMod(string ItemID)
	{
		if (string.IsNullOrEmpty(ItemID))
		{
			return 0;
		}
		return CFGStaticDataContainer.GetItemDefinition(ItemID)?.Heat ?? 0;
	}

	public void AddInjury(bool bCombatInjury)
	{
		if (bCombatInjury && !CFGGame.InjuriesEnabled)
		{
			return;
		}
		List<string> list = new List<string>();
		Dictionary<string, CFGDef_Buff> buffDefs = CFGStaticDataContainer.GetBuffDefs();
		if (list == null || buffDefs == null)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGDef_Buff> item in buffDefs)
		{
			if (item.Value != null && item.Value.BuffType == EBuffType.Wound && !string.IsNullOrEmpty(item.Value.SpecialEffect) && !HasBuff(item.Value.SpecialEffect) && !HasBuff(item.Key))
			{
				list.Add(item.Key);
			}
		}
		if (list.Count != 0)
		{
			AddBuff(list[Random.Range(0, list.Count)], EBuffSource.FromShot);
			SetState(ECharacterStateFlag.Injured, Value: true);
		}
	}

	public void RemoveEquipment()
	{
		if (!string.IsNullOrEmpty(m_Item1))
		{
			CFGInventory.AddItem(m_Item1, 1, SetAsNew: true);
		}
		if (!string.IsNullOrEmpty(m_Item2))
		{
			CFGInventory.AddItem(m_Item2, 1, SetAsNew: true);
		}
		if (!string.IsNullOrEmpty(m_Talisman))
		{
			CFGInventory.AddItem(m_Talisman, 1, SetAsNew: true);
		}
		if (!string.IsNullOrEmpty(Weapon1) && string.Compare("revolver_coltarmyrusty", Weapon1, ignoreCase: true) != 0)
		{
			CFGInventory.AddItem(Weapon1, 1, SetAsNew: true);
		}
		if (!string.IsNullOrEmpty(Weapon2) && string.Compare("revolver_coltarmyrusty", Weapon2, ignoreCase: true) != 0)
		{
			CFGInventory.AddItem(Weapon2, 1, SetAsNew: true);
		}
		EquipItem(EItemSlot.Weapon1, string.Empty);
		EquipItem(EItemSlot.Weapon2, string.Empty);
		EquipItem(EItemSlot.Item1, string.Empty);
		EquipItem(EItemSlot.Item2, string.Empty);
		EquipItem(EItemSlot.Talisman, string.Empty);
		for (int i = 0; i < 5; i++)
		{
			CFGInventory.MoveCardFromCharacter(m_Definition.NameID, i);
		}
	}

	public void ReplaceDefinition(string NewCharacterID)
	{
		CFGDef_Character characterDefinition = CFGStaticDataContainer.GetCharacterDefinition(NewCharacterID);
		if (characterDefinition == null)
		{
			Debug.LogError("Failed to find character definition " + NewCharacterID);
			return;
		}
		OnNewOwner();
		OnDifficultyChanged(CFGGame.Difficulty);
		SetState(ECharacterStateFlag.ImmuneToGunpoint, m_Definition.GunpointImmunity);
		EvaluateBuffs();
	}

	public void SetDecayLevel(int NewLevel)
	{
		if (IsStateSet(ECharacterStateFlag.ImmuneToDecay))
		{
			m_DecayLevel = -1;
			RemoveBuffs(EBuffSource.Decay);
		}
		else
		{
			m_DecayLevel = NewLevel;
			RecalculateDecayBuff();
		}
	}

	public void RecalculateDecayBuff()
	{
		int totalDecayLevel = TotalDecayLevel;
		if (totalDecayLevel < 0)
		{
			RemoveBuffs(EBuffSource.Decay);
			return;
		}
		CFGDef_Buff bestDecayBuff = CFGStaticDataContainer.GetBestDecayBuff(totalDecayLevel);
		if (bestDecayBuff != null && !HasBuff(bestDecayBuff.BuffID))
		{
			RemoveBuffs(EBuffSource.Decay);
			AddBuff(bestDecayBuff, EBuffSource.Decay);
		}
	}

	public void OnSerialize(CFG_SG_Node ParentNode)
	{
		if (m_Definition == null)
		{
			return;
		}
		CFG_SG_Node cFG_SG_Node = ParentNode.AddSubNode("Char");
		if (cFG_SG_Node == null)
		{
			return;
		}
		Serialize(cFG_SG_Node, IsWriting: true);
		if ((bool)m_Nemesis)
		{
			cFG_SG_Node.Attrib_Set("Nemesis", m_Nemesis.UniqueID);
		}
		if ((bool)m_CurrentModel)
		{
			CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.AddSubNode("Model");
			if (cFG_SG_Node2 == null)
			{
				return;
			}
			cFG_SG_Node2.Attrib_Set("UUID", m_CurrentModel.UniqueID);
			int value = 0;
			if ((bool)m_CurrentModel.Owner)
			{
				value = m_CurrentModel.Owner.UniqueID;
			}
			cFG_SG_Node2.Attrib_Set("Owner", value);
			cFG_SG_Node2.Attrib_Set("Pos", m_CurrentModel.transform.position);
			cFG_SG_Node2.Attrib_Set("Rot", m_CurrentModel.transform.rotation);
		}
		CFG_SG_Node cFG_SG_Node3 = null;
		cFG_SG_Node3 = cFG_SG_Node.AddSubNode("Abilities");
		cFG_SG_Node3.Attrib_Set("Count", m_Abilities.Count);
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_Abilities)
		{
			CFG_SG_Node cFG_SG_Node4 = cFG_SG_Node3.AddSubNode("Ability");
			if (cFG_SG_Node4 == null)
			{
				return;
			}
			int source = (int)ability.Value.Source;
			cFG_SG_Node4.Attrib_Set("Name", ability.Key);
			cFG_SG_Node4.Attrib_Set("Source", source);
			if (ability.Value.Ability != null)
			{
				cFG_SG_Node4.Attrib_Set("Cooldown", ability.Value.Ability.CooldownLeft);
				cFG_SG_Node4.Attrib_Set("Used", ability.Value.Ability.UsesPerTacticalLeft);
			}
		}
		m_BaseStats.OnSerialize(cFG_SG_Node, IsWriting: true);
		cFG_SG_Node3 = cFG_SG_Node.AddSubNode("Buffs");
		foreach (CFGBuff value2 in m_Buffs.Values)
		{
			value2.OnSerialize(cFG_SG_Node3);
		}
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			if (m_Cards[i] != null)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		cFG_SG_Node3 = cFG_SG_Node.AddSubNode("Cards");
		if (cFG_SG_Node3 == null)
		{
			return;
		}
		for (int j = 0; j < 5; j++)
		{
			if (m_Cards[j] != null)
			{
				string attName = "ID" + j;
				cFG_SG_Node3.Attrib_Set(attName, m_Cards[j].CardID);
			}
		}
	}

	private bool Serialize(CFG_SG_Node nd, bool IsWriting)
	{
		nd.Serialize(IsWriting, "Name", ref m_Definition.NameID);
		nd.Serialize(IsWriting, "TeamPos", ref m_InTeamPos);
		nd.Serialize(IsWriting, "TeamID", ref m_TeamID);
		nd.Serialize(IsWriting, "HP", ref m_Hp);
		nd.Serialize(IsWriting, "Luck", ref m_Luck);
		nd.Serialize(IsWriting, "Slots", ref m_CardSlotsUnlocked);
		int Value = (int)m_State;
		nd.Serialize(IsWriting, "State", ref Value);
		if (!IsWriting)
		{
			m_State = (ECharacterStateFlag)Value;
		}
		nd.Serialize(IsWriting, "AIState", ref m_AIState);
		nd.Serialize(IsWriting, "GunpointState", ref m_GunpointState);
		nd.Serialize(IsWriting, "SuspiciousLevel", ref m_SuspicionLevel);
		nd.Serialize(IsWriting, "SubduedCnt", ref m_SubduedCount);
		nd.Serialize(IsWriting, "APLeft", ref m_ActionPoints);
		nd.Serialize(IsWriting, "ArteryDist", ref m_ArteryDistance);
		nd.Serialize(IsWriting, "Death", ref m_DeathTime);
		nd.Serialize(IsWriting, "AITeam", ref m_AiTeam);
		if (!IsWriting)
		{
			m_DecayLevel = -1;
		}
		nd.Serialize(IsWriting, "Decay", ref m_DecayLevel);
		CFG_SG_Node cFG_SG_Node = null;
		cFG_SG_Node = ((!IsWriting) ? nd.FindSubNode("Items") : nd.AddSubNode("Items"));
		if (cFG_SG_Node != null)
		{
			cFG_SG_Node.Serialize(IsWriting, "Talisman", ref m_Talisman);
			cFG_SG_Node.Serialize(IsWriting, "Item1", ref m_Item1);
			cFG_SG_Node.Serialize(IsWriting, "Item2", ref m_Item2);
		}
		if (m_Gun1 != null)
		{
			m_Gun1.OnSerialize(nd, "Weapon1", IsWriting);
		}
		if (m_Gun2 != null)
		{
			m_Gun2.OnSerialize(nd, "Weapon2", IsWriting);
		}
		return true;
	}

	public void OnDeserialize(CFG_SG_Node node)
	{
		m_Buffs.Clear();
		m_Abilities.Clear();
		m_BaseStats.OnSerialize(node, IsWriting: false);
		Serialize(node, IsWriting: false);
		m_NemesisID = node.Attrib_Get("Nemesis", 0, bReport: false);
		CFG_SG_Node cFG_SG_Node = null;
		cFG_SG_Node = node.FindSubNode("Abilities");
		if (cFG_SG_Node != null)
		{
			for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
			{
				CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
				if (subNode != null && string.Compare(subNode.Name, "Ability", ignoreCase: true) == 0)
				{
					ETurnAction aType = subNode.Attrib_Get("Name", ETurnAction.None);
					int aSource = subNode.Attrib_Get("Source", 0);
					int cooldown = subNode.Attrib_Get("Cooldown", 0, bReport: false);
					int limit = subNode.Attrib_Get("Used", 0, bReport: false);
					AddAbility(aType, (EAbilitySource)aSource, cooldown, limit);
				}
			}
		}
		cFG_SG_Node = node.FindSubNode("Buffs");
		if (cFG_SG_Node != null)
		{
			for (int j = 0; j < cFG_SG_Node.SubNodeCount; j++)
			{
				CFG_SG_Node subNode2 = cFG_SG_Node.GetSubNode(j);
				if (subNode2 != null && string.Compare(subNode2.Name, "Buff", ignoreCase: true) == 0)
				{
					CFGBuff cFGBuff = new CFGBuff(subNode2);
					if (cFGBuff != null && cFGBuff.m_Def != null && !m_Buffs.ContainsKey(cFGBuff.m_Def.BuffID))
					{
						m_Buffs.Add(cFGBuff.m_Def.BuffID, cFGBuff);
					}
				}
			}
		}
		if (m_Cards == null)
		{
			m_Cards = new CFGDef_Card[5];
		}
		if (m_Cards == null)
		{
			return;
		}
		for (int k = 0; k < 5; k++)
		{
			m_Cards[k] = null;
		}
		cFG_SG_Node = node.FindSubNode("Cards");
		if (cFG_SG_Node != null)
		{
			for (int l = 0; l < 5; l++)
			{
				string attName = "ID" + l;
				string text = cFG_SG_Node.Attrib_Get<string>(attName, null, bReport: false);
				if (!string.IsNullOrEmpty(text))
				{
					CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(text);
					if (cardDefinition == null)
					{
						Debug.LogWarning("Failed to find card " + text);
					}
					else
					{
						m_Cards[l] = cardDefinition;
					}
				}
			}
		}
		UpdateCards(OnDeserialization: true);
	}

	public bool Deserialize_UpdateNemezis()
	{
		if (m_NemesisID == 0)
		{
			return false;
		}
		m_Nemesis = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(m_NemesisID, ESerializableType.NotSerializable);
		if (m_Nemesis == null)
		{
			Debug.LogError("Failed to find nemesis character: " + m_NemesisID);
		}
		m_NemesisID = 0;
		return m_Nemesis != null;
	}
}
