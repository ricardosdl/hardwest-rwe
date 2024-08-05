using System.Text;
using Steamworks;
using UnityEngine;

public class CFGSteam : CFGSingleton<CFGSteam>
{
	public delegate void OnSteamOverlayDelegate(bool activated);

	private const uint STEAM_APP_ID = 307670u;

	private const int STEAM_STATS_VERSION = 0;

	private bool m_Initialized;

	private CGameID m_GameID;

	private CSteamID m_UserID;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	private Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	private Callback<UserStatsReceived_t> m_UserStatsReceived;

	private Callback<UserStatsStored_t> m_UserStatsStored;

	private Callback<UserAchievementStored_t> m_UserAchievementStored;

	public OnSteamOverlayDelegate m_OnSteamOverlayCallback;

	public bool Initialized => m_Initialized;

	public CSteamID SteamUserID => m_UserID;

	public void ActivateOverlayWebPage(string url)
	{
		if (m_Initialized)
		{
			SteamFriends.ActivateGameOverlayToWebPage(url);
		}
	}

	public void SetAchievement(string ach_id)
	{
		SteamUserStats.SetAchievement(ach_id);
	}

	public void ClearAchievement(string ach_id)
	{
		SteamUserStats.ClearAchievement(ach_id);
	}

	public bool IsAchievementUnlocked(string ach_id)
	{
		bool pbAchieved = false;
		SteamUserStats.GetAchievement(ach_id, out pbAchieved);
		return pbAchieved;
	}

	public string GetAchievementName(string ach_id)
	{
		return SteamUserStats.GetAchievementDisplayAttribute(ach_id, "name");
	}

	public string GetAchievementDesc(string ach_id)
	{
		return SteamUserStats.GetAchievementDisplayAttribute(ach_id, "desc");
	}

	public void StoreStats()
	{
		if (m_Initialized && !SteamUserStats.StoreStats())
		{
			Debug.LogError("Steam error with store stats!");
		}
	}

	public void ResetAllStats()
	{
		Debug.Log("Steam: resetting all stats.");
		if (m_Initialized)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		m_Initialized = SteamAPI.Init();
		if (!m_Initialized)
		{
			Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed. Most probably Steam Client is not running.");
			return;
		}
		m_GameID = new CGameID(SteamUtils.GetAppID());
		m_UserID = SteamUser.GetSteamID();
		if (m_SteamAPIWarningMessageHook == null)
		{
			m_SteamAPIWarningMessageHook = SteamAPIDebugTextHook;
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
		m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnOverlayActivated);
		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
		SteamUserStats.RequestCurrentStats();
	}

	private void OnDestroy()
	{
		if (m_Initialized)
		{
			SteamAPI.Shutdown();
		}
	}

	private void Update()
	{
		if (m_Initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	private static void SteamAPIDebugTextHook(int severity, StringBuilder info)
	{
		if (severity == 1)
		{
			Debug.LogWarning("Steam: " + info.ToString());
		}
		else
		{
			Debug.Log("Steam: " + info.ToString());
		}
	}

	private void OnOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			Debug.Log("Steam Overlay has been activated");
			if (m_OnSteamOverlayCallback != null)
			{
				m_OnSteamOverlayCallback(activated: true);
			}
		}
		else
		{
			Debug.Log("Steam Overlay has been closed");
			if (m_OnSteamOverlayCallback != null)
			{
				m_OnSteamOverlayCallback(activated: false);
			}
		}
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (m_Initialized && (ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				Debug.Log("Received stats and achievements from Steam.");
				CFGSingleton<CFGAchievementManager>.Instance.LoadFromSteam();
			}
			else
			{
				Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID)
		{
			if (pCallback.m_eResult == EResult.k_EResultOK)
			{
				Debug.Log("Steam stats stored.");
			}
			else if (pCallback.m_eResult == EResult.k_EResultInvalidParam)
			{
				Debug.Log("StoreStats - some failed to validate");
				UserStatsReceived_t pCallback2 = default(UserStatsReceived_t);
				pCallback2.m_eResult = EResult.k_EResultOK;
				pCallback2.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(pCallback2);
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)m_GameID == pCallback.m_nGameID && pCallback.m_nMaxProgress == 0)
		{
			Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
		}
	}
}
