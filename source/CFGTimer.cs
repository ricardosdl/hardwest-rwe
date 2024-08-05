using System;
using UnityEngine;

public static class CFGTimer
{
	private static OnGameplay_Paused m_OnGameplay_Paused;

	private static OnGameplay_Unpaused m_OnGameplay_Unpaused;

	private static float m_MissionTime;

	private static int m_PauseRC_Gameplay;

	private static float m_Delta_Gameplay;

	public static float MissionTime
	{
		get
		{
			return m_MissionTime;
		}
		set
		{
			m_MissionTime = value;
		}
	}

	public static float DeltaTime_Gameplay => m_Delta_Gameplay;

	public static bool IsPaused_Gameplay => m_PauseRC_Gameplay > 0;

	public static void RegisterCallback_OnGameplayPaused(OnGameplay_Paused callback)
	{
		m_OnGameplay_Paused = (OnGameplay_Paused)Delegate.Combine(m_OnGameplay_Paused, callback);
	}

	public static void UnRegisterCallback_OnGameplayPaused(OnGameplay_Paused callback)
	{
		m_OnGameplay_Paused = (OnGameplay_Paused)Delegate.Remove(m_OnGameplay_Paused, callback);
	}

	public static void RegisterCallback_OnGameplayUnPaused(OnGameplay_Unpaused callback)
	{
		m_OnGameplay_Unpaused = (OnGameplay_Unpaused)Delegate.Combine(m_OnGameplay_Unpaused, callback);
	}

	public static void UnRegisterCallback_OnGameplayUnPaused(OnGameplay_Unpaused callback)
	{
		m_OnGameplay_Unpaused = (OnGameplay_Unpaused)Delegate.Remove(m_OnGameplay_Unpaused, callback);
	}

	public static void RegisterCallbacks(OnGameplay_Paused cb_OnPaused, OnGameplay_Unpaused cb_OnUnpaused)
	{
		m_OnGameplay_Paused = (OnGameplay_Paused)Delegate.Combine(m_OnGameplay_Paused, cb_OnPaused);
		m_OnGameplay_Unpaused = (OnGameplay_Unpaused)Delegate.Combine(m_OnGameplay_Unpaused, cb_OnUnpaused);
	}

	public static void UnRegisterCallbacks(OnGameplay_Paused cb_OnPaused, OnGameplay_Unpaused cb_OnUnpaused)
	{
		m_OnGameplay_Paused = (OnGameplay_Paused)Delegate.Remove(m_OnGameplay_Paused, cb_OnPaused);
		m_OnGameplay_Unpaused = (OnGameplay_Unpaused)Delegate.Remove(m_OnGameplay_Unpaused, cb_OnUnpaused);
	}

	public static void SetPaused_Gameplay(bool bPauseGameplay)
	{
		bool isPaused_Gameplay = IsPaused_Gameplay;
		if (bPauseGameplay)
		{
			m_PauseRC_Gameplay++;
		}
		else
		{
			m_PauseRC_Gameplay--;
		}
		if (m_PauseRC_Gameplay < 0)
		{
			Debug.LogError("Pause/Unpause - gameplay pause reference counter is below zero!");
			m_PauseRC_Gameplay = 0;
		}
		bool isPaused_Gameplay2 = IsPaused_Gameplay;
		if (isPaused_Gameplay2 != isPaused_Gameplay)
		{
			NotifyPausableGameObjects(isPaused_Gameplay2);
		}
	}

	public static void OnMissionStart()
	{
		m_MissionTime = 0f;
		m_PauseRC_Gameplay = 0;
		m_Delta_Gameplay = 0f;
	}

	public static void Update()
	{
		m_Delta_Gameplay = 0f;
		if (!IsPaused_Gameplay)
		{
			m_Delta_Gameplay = Time.deltaTime;
			m_MissionTime += m_Delta_Gameplay;
		}
	}

	private static void NotifyPausableGameObjects(bool bNewPauseState)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		CFGCamera component2 = Camera.main.GetComponent<CFGCamera>();
		if (bNewPauseState)
		{
			Debug.Log("Gameplay: PAUSED (" + m_PauseRC_Gameplay + ")");
			if (m_OnGameplay_Paused != null)
			{
				m_OnGameplay_Paused();
			}
			foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				character.OnGameplayPause();
			}
			if ((bool)component)
			{
				component.OnGameplayPause();
			}
			if ((bool)component2)
			{
				component2.OnGameplayPause();
			}
			CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PauseDialog(bPause: true);
			return;
		}
		Debug.Log("Gameplay: UnPAUSED (" + m_PauseRC_Gameplay + ")");
		if (m_OnGameplay_Unpaused != null)
		{
			m_OnGameplay_Unpaused();
		}
		foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			character2.OnGameplayUnPause();
		}
		if ((bool)component)
		{
			component.OnGameplayUnPause();
		}
		if ((bool)component2)
		{
			component2.OnGameplayUnPause();
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PauseDialog(bPause: false);
	}
}
