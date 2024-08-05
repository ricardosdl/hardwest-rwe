using System.Collections.Generic;
using UnityEngine;

public class CFGAchievementManager : CFGSingleton<CFGAchievementManager>
{
	public Dictionary<EAchievement, CFGAchievement> m_Achievements = new Dictionary<EAchievement, CFGAchievement>();

	private bool m_NeedLoad;

	public void LoadFromSteam()
	{
		m_NeedLoad = true;
	}

	public void UnlockAchievement(EAchievement achievement_id)
	{
		Debug.Log("Unlock achievment: " + achievement_id);
		CFGAchievement achievement = GetAchievement(achievement_id);
		if (achievement != null)
		{
			if (!achievement.m_Achieved)
			{
				achievement.m_Achieved = true;
				achievement.m_IsDirty = true;
			}
			else
			{
				Debug.LogWarning("WARNING! CFGAchievementManager.UnlockAchievement() - achievement " + achievement_id.ToString() + " already achieved.");
			}
		}
	}

	public void DEV_ResetAll()
	{
		Debug.Log("DEV reseting all achievements!");
		foreach (KeyValuePair<EAchievement, CFGAchievement> achievement in m_Achievements)
		{
			achievement.Value.m_Achieved = false;
			achievement.Value.m_IsDirty = true;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 1; i <= 35; i++)
		{
			m_Achievements.Add((EAchievement)i, new CFGAchievement());
		}
	}

	private void FixedUpdate()
	{
		if (CFGSingleton<CFGSteam>.Instance.Initialized)
		{
			UpdateSteamSave();
			UpdateSteamLoad();
		}
	}

	private CFGAchievement GetAchievement(EAchievement achievement_id)
	{
		CFGAchievement value = null;
		m_Achievements.TryGetValue(achievement_id, out value);
		return value;
	}

	private void UpdateSteamSave()
	{
		bool flag = false;
		foreach (KeyValuePair<EAchievement, CFGAchievement> achievement in m_Achievements)
		{
			if (achievement.Value.m_IsDirty)
			{
				string ach_id = achievement.Key.ToString();
				if (achievement.Value.m_Achieved)
				{
					API_SetAchievement(ach_id);
				}
				else
				{
					API_ClearAchievement(ach_id);
				}
				achievement.Value.m_IsDirty = false;
				flag = true;
			}
		}
		if (flag)
		{
			API_StoreStats();
		}
	}

	private void UpdateSteamLoad()
	{
		if (!m_NeedLoad)
		{
			return;
		}
		foreach (KeyValuePair<EAchievement, CFGAchievement> achievement in m_Achievements)
		{
			string ach_id = achievement.Key.ToString();
			API_GetAchievementData(ach_id, out var unlocked, out var text, out var desc);
			achievement.Value.m_Achieved = unlocked;
			achievement.Value.m_Name = text;
			achievement.Value.m_Description = desc;
		}
		m_NeedLoad = false;
	}

	private void API_SetAchievement(string ach_id)
	{
		if (CFGSingleton<CFGSteam>.Instance.Initialized)
		{
			CFGSingleton<CFGSteam>.Instance.SetAchievement(ach_id);
		}
	}

	private void API_ClearAchievement(string ach_id)
	{
		if (CFGSingleton<CFGSteam>.Instance.Initialized)
		{
			CFGSingleton<CFGSteam>.Instance.ClearAchievement(ach_id);
		}
	}

	private void API_GetAchievementData(string ach_id, out bool unlocked, out string name, out string desc)
	{
		if (CFGSingleton<CFGSteam>.Instance.Initialized)
		{
			unlocked = CFGSingleton<CFGSteam>.Instance.IsAchievementUnlocked(ach_id);
			name = CFGSingleton<CFGSteam>.Instance.GetAchievementName(ach_id);
			desc = CFGSingleton<CFGSteam>.Instance.GetAchievementDesc(ach_id);
		}
		else
		{
			unlocked = false;
			name = string.Empty;
			desc = string.Empty;
		}
	}

	private void API_StoreStats()
	{
		if (CFGSingleton<CFGSteam>.Instance.Initialized)
		{
			CFGSingleton<CFGSteam>.Instance.StoreStats();
		}
	}
}
