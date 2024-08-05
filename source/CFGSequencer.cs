using System.Collections;
using UnityEngine;

public class CFGSequencer : CFGSequencerBase
{
	protected string m_TacticalToLoad;

	protected bool m_bCreateAutoSave;

	public void StartLoadingTactical()
	{
		CFG_SG_Manager.CreateRestorePoint_Strategic();
		StartCoroutine(LoadTacticalMap_PL(m_TacticalToLoad));
		m_TacticalToLoad = null;
	}

	protected IEnumerator LoadTacticalMap_PL(string TacticalID)
	{
		Debug.LogWarning("LoadTacticalMap: " + TacticalID);
		CFGFadeToColor camera_fade = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (camera_fade != null)
		{
			camera_fade.SetFade(CFGFadeToColor.FadeType.fadeOut, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, 1f, null);
			while (camera_fade.m_IsFading)
			{
				yield return null;
			}
		}
		CFGSession session = CFGSingleton<CFGGame>.Instance.GetSession();
		session.PrepareLevel_Tactical(TacticalID);
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnStrategicEnd();
		session.LoadLevel();
	}

	protected override void Update()
	{
		CFGFlowSequence mainSequence = MainSequence;
		if (CFGTimer.IsPaused_Gameplay || !(mainSequence != null) || !mainSequence.m_Active)
		{
			return;
		}
		mainSequence.UpdateFlow(Time.deltaTime);
		if (m_TacticalToLoad != null)
		{
			if (CFGCharacterList.GetTeamCharactersList().Count > 0)
			{
				CFGSingleton<CFGWindowMgr>.Instance.LoadCharacterScreen(combat_loadout: true, this);
			}
			else
			{
				StartLoadingTactical();
			}
			mainSequence.m_Active = false;
		}
		else if (m_bCreateAutoSave)
		{
			CFGSingleton<CFGGame>.Instance.CreateSaveGame(CFG_SG_SaveGame.eSG_Source.AutoSave);
			m_bCreateAutoSave = false;
		}
	}

	public bool OnPostLoad()
	{
		return MainSequence.OnPostLoad();
	}
}
