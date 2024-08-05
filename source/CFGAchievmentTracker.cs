using System.Collections.Generic;
using UnityEngine;

public static class CFGAchievmentTracker
{
	private static CFGAchievementData m_AchievementData = null;

	private static int m_TotalAPUsed = 0;

	private static int m_TotalShots = 0;

	private static int m_RustyShots = 0;

	private static int m_CTH100Shots = 0;

	private static int m_NoLOSKills = 0;

	private static HashSet<string> m_UsedCharacters = new HashSet<string>();

	private static HashSet<string> m_UsedItems = new HashSet<string>();

	public static bool EqualizationUsedInFirstTurn { get; set; }

	public static int LastAPCost { get; set; }

	public static int TotalShots => m_TotalShots;

	public static int CTH100Shots => m_CTH100Shots;

	public static int BLINDShots => m_NoLOSKills;

	public static int RustyShots => m_RustyShots;

	public static int UsedCharCnt
	{
		get
		{
			if (m_UsedCharacters == null)
			{
				return 0;
			}
			return m_UsedCharacters.Count;
		}
	}

	public static int UsedItemCnt
	{
		get
		{
			if (m_UsedItems == null)
			{
				return 0;
			}
			return m_UsedItems.Count;
		}
	}

	public static void AddAPUsed(int AP)
	{
		m_TotalAPUsed += AP;
	}

	public static void OnShotFired(CFGCharacter Shooter, CFGCharacter Target, int CTH, bool bKilled)
	{
		if (Shooter == null || Target == null || Shooter.Owner == null || !Shooter.Owner.IsPlayer)
		{
			return;
		}
		m_TotalShots++;
		CFGGun currentWeapon = Shooter.CurrentWeapon;
		if (currentWeapon != null && currentWeapon.m_Definition != null && string.Compare("revolver_coltarmyrusty", currentWeapon.m_Definition.ItemID, ignoreCase: true) == 0)
		{
			m_RustyShots++;
		}
		if (CTH == 100)
		{
			m_CTH100Shots++;
		}
		if (CFGCellMap.GetLineOfSightAutoSideSteps(Shooter, Target) != 0 && bKilled)
		{
			int num = 10;
			if (m_AchievementData != null)
			{
				num = m_AchievementData.m_A24_BlindKillsRequired;
			}
			m_NoLOSKills++;
			if (m_NoLOSKills >= num)
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_24);
			}
			CFGVar variable = CFGVariableContainer.Instance.GetVariable("bsc", "profile");
			if (variable != null)
			{
				variable.Value = m_NoLOSKills;
				CFGVariableContainer.Instance.SaveValuesGlobal(null);
			}
		}
		if (bKilled && Shooter.IsInDemonForm && Target.CharacterData != null && string.Compare(Target.CharacterData.Definition.PrefabSubPath, "demon", ignoreCase: true) == 0)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_28);
		}
		if (bKilled && m_TotalAPUsed - LastAPCost == 0)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_30);
		}
		if (bKilled && m_AchievementData != null && (bool)Shooter.CurrentWeapon && Shooter.CurrentWeapon.m_Definition != null && (bool)Target && Target.CharacterData != null && Target.CharacterData.IsStateSet(ECharacterStateFlag.MarkedForAchiev_06) && string.Compare(m_AchievementData.m_A06_WeaponID, Shooter.CurrentWeapon.m_Definition.ItemID, ignoreCase: true) == 0)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_06);
		}
		LastAPCost = 0;
	}

	public static void OnMissionStart()
	{
		LastAPCost = 0;
		EqualizationUsedInFirstTurn = false;
		m_TotalShots = 0;
		m_RustyShots = 0;
		m_CTH100Shots = 0;
		m_TotalAPUsed = 0;
	}

	public static void OnTacticalWin()
	{
		if (EqualizationUsedInFirstTurn)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_19);
		}
		if (m_TotalShots > 0)
		{
			if (m_RustyShots == m_TotalShots)
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_21);
			}
			if (m_CTH100Shots == m_TotalShots)
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_22);
			}
		}
		else
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_25);
		}
	}

	public static void GetScenarioCompletion(string CampNameID, int ScenarioID, out bool Completed_Easy, out bool Completed_Med, out bool Completed_Hard, out bool Completed_Ironman, out bool Completed_Injuries)
	{
		Completed_Easy = false;
		Completed_Med = false;
		Completed_Hard = false;
		Completed_Ironman = false;
		Completed_Injuries = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		string text = CampNameID + "_";
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(text + "easy", "profile");
		CFGVar variable2 = CFGVariableContainer.Instance.GetVariable(text + "med", "profile");
		CFGVar variable3 = CFGVariableContainer.Instance.GetVariable(text + "hard", "profile");
		CFGVar variable4 = CFGVariableContainer.Instance.GetVariable(text + "injury", "profile");
		CFGVar variable5 = CFGVariableContainer.Instance.GetVariable(text + "iron", "profile");
		int num6 = 1 << ScenarioID;
		if (variable3 != null)
		{
			num = (int)variable3.Value;
		}
		if (variable4 != null)
		{
			num2 = (int)variable4.Value;
		}
		if (variable5 != null)
		{
			num3 = (int)variable5.Value;
		}
		if (variable != null)
		{
			num4 = (int)variable.Value;
		}
		if (variable2 != null)
		{
			num5 = (int)variable2.Value;
		}
		if ((num4 & num6) != 0)
		{
			Completed_Easy = true;
		}
		if ((num5 & num6) != 0)
		{
			Completed_Med = true;
		}
		if ((num & num6) != 0)
		{
			Completed_Hard = true;
		}
		if ((num3 & num6) != 0)
		{
			Completed_Ironman = true;
		}
		if ((num2 & num6) != 0)
		{
			Completed_Injuries = true;
		}
	}

	public static void OnScenarioWin(string campid, int scenarioid)
	{
		switch (scenarioid)
		{
		case 8:
			break;
		default:
			Debug.LogError("Invalid scenario ID: " + scenarioid);
			break;
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		{
			bool[] array = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				string valname = "camp_01_scen_0" + (i + 1) + "_completed";
				array[i] = GetProfileValBool(valname);
			}
			array[scenarioid] = true;
			bool flag = true;
			for (int j = 0; j < 8; j++)
			{
				flag &= array[j];
			}
			if (flag)
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_16);
			}
			if (!CFGGame.DifficultyChanged)
			{
				string text = campid + "_";
				int num = 255;
				int num2 = 1 << scenarioid;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				CFGVar variable = CFGVariableContainer.Instance.GetVariable(text + "easy", "profile");
				CFGVar variable2 = CFGVariableContainer.Instance.GetVariable(text + "med", "profile");
				CFGVar variable3 = CFGVariableContainer.Instance.GetVariable(text + "hard", "profile");
				CFGVar variable4 = CFGVariableContainer.Instance.GetVariable(text + "injury", "profile");
				CFGVar variable5 = CFGVariableContainer.Instance.GetVariable(text + "iron", "profile");
				if (variable3 != null)
				{
					num3 = (int)variable3.Value;
				}
				if (variable4 != null)
				{
					num4 = (int)variable4.Value;
				}
				if (variable5 != null)
				{
					num5 = (int)variable5.Value;
				}
				if (variable != null)
				{
					num6 = (int)variable.Value;
				}
				if (variable2 != null)
				{
					num7 = (int)variable2.Value;
				}
				if (CFGGame.Difficulty == EDifficulty.Hard)
				{
					num3 |= num2;
					num7 |= num2;
					num6 |= num2;
				}
				if (CFGGame.Difficulty == EDifficulty.Normal)
				{
					num7 |= num2;
					num6 |= num2;
				}
				if (CFGGame.Difficulty == EDifficulty.Easy)
				{
					num6 |= num2;
				}
				if (CFGGame.InjuriesEnabled)
				{
					num4 |= num2;
				}
				if (CFGGame.Permadeath)
				{
					num5 |= num2;
				}
				num3 &= num;
				num7 &= num;
				num6 &= num;
				num4 &= num;
				num5 &= num;
				if (CFGGame.Difficulty == EDifficulty.Hard && CFGGame.InjuriesEnabled && CFGGame.Permadeath)
				{
					CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_17);
				}
				if (num3 == num && num4 == num && num5 == num)
				{
					CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_18);
				}
				if (variable3 != null)
				{
					variable3.Value = num3;
				}
				if (variable4 != null)
				{
					variable4.Value = num4;
				}
				if (variable5 != null)
				{
					variable5.Value = num5;
				}
				if (variable != null)
				{
					variable.Value = num6;
				}
				if (variable2 != null)
				{
					variable2.Value = num7;
				}
				CFGVariableContainer.Instance.SaveValuesGlobal(null);
			}
			break;
		}
		}
	}

	private static bool GetProfileValBool(string valname)
	{
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(valname, "profile");
		if (variable == null)
		{
			return false;
		}
		return (bool)variable.Value;
	}

	public static void OnTrinketUnlock()
	{
		int num = 8;
		int num2 = 3;
		for (int i = 1; i <= num; i++)
		{
			for (int j = 1; j <= num2; j++)
			{
				string text = "trinket_s" + i + "_0" + j;
				CFGVar variable = CFGVariableContainer.Instance.GetVariable(text, "campaign");
				if (variable == null)
				{
					Debug.LogWarning("Failed to find fate tradem item def: " + text + ". Achievment 20 is not possible!");
					continue;
				}
				string value = variable.Value as string;
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
			}
		}
		CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_20);
	}

	public static void InitData()
	{
		m_AchievementData = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AchievementData;
		if (m_AchievementData == null)
		{
			Debug.LogError("Failed to access achievement data in CFGGameplaySettings!");
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable("camp_01_uitems", "profile");
		if (variable == null)
		{
			Debug.LogError("Cannot find used items list for achiev!");
		}
		else
		{
			StringToHashSet((string)variable.Value, ref m_UsedItems);
		}
		CFGVar variable2 = CFGVariableContainer.Instance.GetVariable("camp_01_uchars", "profile");
		if (variable2 == null)
		{
			Debug.LogError("Cannot find used items list for achiev!");
		}
		else
		{
			StringToHashSet((string)variable2.Value, ref m_UsedCharacters);
		}
		m_NoLOSKills = 0;
		CFGVar variable3 = CFGVariableContainer.Instance.GetVariable("bsc", "profile");
		if (variable3 == null)
		{
			Debug.LogWarning("Cannot find blind shot count!");
		}
		else
		{
			m_NoLOSKills = (int)variable3.Value;
		}
	}

	private static void StringToHashSet(string val, ref HashSet<string> hset)
	{
		if (hset == null)
		{
			hset = new HashSet<string>();
			if (hset == null)
			{
				return;
			}
		}
		hset.Clear();
		if (val == null)
		{
			return;
		}
		string[] array = val.Split(';');
		if (array == null || array.Length == 0)
		{
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim();
			if (!string.IsNullOrEmpty(text2) && !hset.Contains(text2))
			{
				hset.Add(text2);
			}
		}
	}

	private static string HashSetToString(ref HashSet<string> hset)
	{
		string text = string.Empty;
		if (hset == null || hset.Count == 0)
		{
			return text;
		}
		int num = 0;
		foreach (string item in hset)
		{
			text += item;
			if (num < hset.Count - 1)
			{
				text += ";";
			}
			num++;
		}
		return text;
	}

	public static void OnItemUse(string ItemID)
	{
		if (string.IsNullOrEmpty(ItemID))
		{
			return;
		}
		string item = ItemID.ToLower();
		if (m_UsedItems == null)
		{
			m_UsedItems = new HashSet<string>();
			if (m_UsedItems == null)
			{
				return;
			}
		}
		if (!m_UsedItems.Contains(item))
		{
			m_UsedItems.Add(item);
		}
		int num = 10;
		if (m_AchievementData != null)
		{
			num = m_AchievementData.m_A26_NumberOfDifferentItems;
		}
		if (m_UsedItems.Count >= num)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_26);
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable("camp_01_uitems", "profile");
		if (variable == null)
		{
			Debug.LogError("Cannot store used items!");
			return;
		}
		variable.Value = HashSetToString(ref m_UsedItems);
		CFGVariableContainer.Instance.SaveValuesGlobal(null);
	}

	public static void OnCharacterSpawn(string CharID)
	{
		if (string.IsNullOrEmpty(CharID))
		{
			return;
		}
		if (m_UsedCharacters == null)
		{
			m_UsedCharacters = new HashSet<string>();
			if (m_UsedCharacters == null)
			{
				return;
			}
		}
		string item = CharID.ToLower();
		if (!m_UsedCharacters.Contains(item))
		{
			m_UsedCharacters.Add(item);
		}
		int num = 10;
		if (m_AchievementData != null)
		{
			num = m_AchievementData.m_A23_NumberOfDifferentChars;
		}
		if (m_UsedCharacters.Count >= num)
		{
			CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_23);
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable("camp_01_uchars", "profile");
		if (variable == null)
		{
			Debug.LogError("Cannot store used chars!");
			return;
		}
		variable.Value = HashSetToString(ref m_UsedCharacters);
		CFGVariableContainer.Instance.SaveValuesGlobal(null);
	}

	public static void OnSerialize(CFG_SG_Node nd)
	{
		if (nd != null)
		{
			nd.Attrib_Set("AEU", EqualizationUsedInFirstTurn);
			nd.Attrib_Set("ATAP", m_TotalAPUsed);
			nd.Attrib_Set("ATS", m_TotalShots);
			nd.Attrib_Set("ARS", m_RustyShots);
			nd.Attrib_Set("ASH", m_CTH100Shots);
		}
	}

	public static void OnDeserialize(CFG_SG_Node nd)
	{
		if (nd != null)
		{
			EqualizationUsedInFirstTurn = nd.Attrib_Get("AEU", EqualizationUsedInFirstTurn);
			m_TotalAPUsed = nd.Attrib_Get("ATAP", m_TotalAPUsed);
			m_TotalShots = nd.Attrib_Get("ATS", m_TotalShots);
			m_RustyShots = nd.Attrib_Get("ARS", m_RustyShots);
			m_CTH100Shots = nd.Attrib_Get("ASH", m_CTH100Shots);
		}
	}
}
