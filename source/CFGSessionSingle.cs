#define USE_ERROR_REPORTING
using System.Collections;
using UnityEngine;

public class CFGSessionSingle : CFGSession
{
	private SMissionStats m_MissionStats = default(SMissionStats);

	private float m_LoadingProgress;

	private bool m_QuickLoading;

	private bool m_IsLoadingLevel;

	private bool m_bCreateAutoSave;

	private bool m_HUD_Restored;

	private bool m_HUD_CanRestore;

	private bool m_bTacticalRestarted;

	private CFGFlowSequence mainSequence;

	private bool m_FastLoad;

	public bool WasTacticalRestarted => m_bTacticalRestarted;

	public string CampaignName => (m_Current_Campaign != null) ? m_Current_Campaign.CampaignID : string.Empty;

	public string SceneName => (m_Current_Scene != null) ? m_Current_Scene : string.Empty;

	public string ScenarioName => (m_Current_Scenario != null) ? m_Current_Scenario.ScenarioID : string.Empty;

	public float GetLoadingProgress()
	{
		return m_LoadingProgress;
	}

	public void ResetMissionStats()
	{
		m_MissionStats.finished = false;
		m_MissionStats.success = false;
		m_MissionStats.scenario_end = false;
		m_MissionStats.reason_id = string.Empty;
		m_MissionStats.play_count = 0;
		m_MissionStats.video_idx = -1;
	}

	public void OnNewGame(EDifficulty NewDifficulty)
	{
		Debug.Log("On new game : " + CFG_SG_Manager.LoadType);
		CFGGame.OnScenarioStart(NewDifficulty);
		CFGEconomy.Reset();
		CFGInventory.Reset();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnNewCampaign();
		CFGCharacterList.OnCampaignStart();
		if (CFG_SG_Manager.LoadType != CFG_SG_Manager.eLoadType.ResetTactical)
		{
			CFG_SG_Manager.ResetStrategicState();
		}
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.OnNewMission();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnNewCampaign();
	}

	private bool SelectCampaign(CFGDef_Campaign newc)
	{
		m_Current_Campaign = null;
		m_Current_Scenario = null;
		m_Current_Tactical = null;
		m_Current_Scene = null;
		if (newc == null)
		{
			CFGError.ReportError("Failed to find campaign definition: " + CampaignName, CFGError.ErrorCode.Fail);
			return false;
		}
		Debug.Log("Starting Campaign: " + newc.CampaignID);
		CFGGame.SetCurrentDLC(newc.Dlc);
		m_Current_Campaign = newc;
		return true;
	}

	public bool SelectCampaign(int CampaingID)
	{
		return SelectCampaign(CFGStaticDataContainer.GetCampaign(CampaingID));
	}

	public bool SelectCampaign(EDLC DlcVersion)
	{
		CFGDef_Campaign dLCCampaign = CFGStaticDataContainer.GetDLCCampaign(DlcVersion);
		if (dLCCampaign == null)
		{
			Debug.LogError("Failed to find campaign for DLC: " + DlcVersion);
			return false;
		}
		return SelectCampaign(dLCCampaign);
	}

	public bool SelectCampaign(string CampaignName)
	{
		return SelectCampaign(CFGStaticDataContainer.GetCampaign(CampaignName));
	}

	public bool StartScenario(string ScenarioID, string SceneNameOrTacticalID = null, bool IsTactical = false)
	{
		if (m_Current_Campaign == null)
		{
			CFGError.ReportError("Cannot start scenario -> campaign has not been selected!", CFGError.ErrorCode.NotInitialized);
			return false;
		}
		m_Current_Scenario = m_Current_Campaign.GetScenario(ScenarioID);
		if (m_Current_Scenario == null)
		{
			CFGError.ReportError("Failed to find scenario " + ScenarioID + " within current campaing: " + m_Current_Campaign.CampaignID, CFGError.ErrorCode.Fail);
			return false;
		}
		Debug.Log("Starting scenario " + m_Current_Scenario.ScenarioID + " Tactical: [" + SceneNameOrTacticalID + "]");
		if (SceneNameOrTacticalID == null)
		{
			if (string.IsNullOrEmpty(m_Current_Scenario.StartTacticalID))
			{
				PrepareLevel_Strategic(ScenarioID);
			}
			else
			{
				PrepareLevel_Tactical(m_Current_Scenario.StartTacticalID);
			}
		}
		else if (IsTactical)
		{
			PrepareLevel_Tactical(SceneNameOrTacticalID);
		}
		else
		{
			PrepareLevel_Strategic(ScenarioID);
		}
		if (m_Current_Scene == null)
		{
			CFGError.ReportError("Failed to start scene/tacticalid: " + SceneNameOrTacticalID, CFGError.ErrorCode.Fail);
			return false;
		}
		return true;
	}

	public void PreQuickLoad()
	{
		CFGFlowSequence cFGFlowSequence = CFGSequencerBase.GetMainSequence();
		if ((bool)cFGFlowSequence)
		{
			cFGFlowSequence.m_Active = false;
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: true);
		CFGAudioManager.Instance.StopBackgroundMusic();
		CFGInventory.Reset();
		CFGEconomy.Reset();
		CFGCharacterList.OnCampaignStart();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalEnd();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.RemoveNewObjectives();
		CFGScopedVariables scoped = CFGVariableContainer.Instance.GetScoped("scenario");
		if (scoped != null)
		{
			scoped.ResetVariables();
		}
		else
		{
			Debug.LogError("No scope found");
		}
		CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
	}

	public void RestartTactical()
	{
		if (m_Current_Tactical == null)
		{
			Debug.LogWarning("RestartTactical can only be restarted while on tactical");
			return;
		}
		CFGFlowSequence cFGFlowSequence = CFGSequencerBase.GetMainSequence();
		if ((bool)cFGFlowSequence)
		{
			cFGFlowSequence.m_Active = false;
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: true);
		if (CFG_SG_Manager.HasStrategicRestorePoint)
		{
			CFG_SG_Manager.RestartTactical();
			return;
		}
		PrepareLevel_Tactical(m_Current_Tactical.TacticalID);
		CFGAudioManager.Instance.StopBackgroundMusic();
		CFGInventory.Reset();
		CFGEconomy.Reset();
		CFGCharacterList.OnCampaignStart();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalEnd();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.RemoveNewObjectives();
		CFGScopedVariables scoped = CFGVariableContainer.Instance.GetScoped("scenario");
		if (scoped != null)
		{
			scoped.ResetVariables();
		}
		else
		{
			Debug.LogError("No scope found");
		}
		CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
		LoadLevel();
	}

	private void RestartCurrentInvalidTactical()
	{
		if (m_Current_Tactical == null || !CFG_SG_Manager.IsLoading || CFG_SG_Manager.CurrentSaveGame == null)
		{
			Debug.LogWarning("RestartTactical can only be restarted while on tactical");
			return;
		}
		CFGScopedVariables scoped = CFGVariableContainer.Instance.GetScoped("scenario");
		if (scoped != null)
		{
			scoped.ResetVariables();
		}
		else
		{
			Debug.LogError("No scope found");
		}
		PrepareLevel_Tactical(m_Current_Tactical.TacticalID);
		CFGAudioManager.Instance.StopBackgroundMusic();
		CFGInventory.Reset();
		CFGEconomy.Reset();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Clear();
		CFGCharacterList.OnCampaignStart();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalEnd();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.RemoveNewObjectives();
		CFG_SG_Manager.Sync_StrategiOnlyForInvalidTactRestart();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
		LoadLevel();
	}

	public bool LoadLevelFromSaveGame(string CampaignName, string ScenarioName, string SceneName, string TacticalID, bool IsTactical, bool bIsReturnToStrategic)
	{
		if (string.Compare(CampaignName, "DevTestCampaign", ignoreCase: true) == 0 && string.Compare(ScenarioName, "DevTestScenario", ignoreCase: true) == 0 && string.Compare(ScenarioName, "DevTestTactical", ignoreCase: true) == 0)
		{
			CFGDef_Tactical tacticalByID = CFGStaticDataContainer.GetTacticalByID("DevTestTactical");
			if (tacticalByID != null)
			{
				tacticalByID.SceneName = SceneName;
			}
		}
		if (!SelectCampaign(CampaignName))
		{
			return false;
		}
		if (IsTactical)
		{
			if (!StartScenario(ScenarioName, TacticalID, IsTactical))
			{
				return false;
			}
		}
		else if (!StartScenario(ScenarioName, SceneName, IsTactical))
		{
			return false;
		}
		if (!bIsReturnToStrategic)
		{
			OnNewGame(CFGGame.Difficulty);
			ReadCampaignVariableDefinitions();
			ReadScenarioVariableDefinitions();
			CFGVariableContainer.Instance.LoadValuesGlobal(CampaignName, bCampaign: true, bProfile: false);
		}
		m_QuickLoading = false;
		return LoadLevel();
	}

	public override bool LoadLevel()
	{
		if (m_Current_Scene == null)
		{
			CFGError.ReportError("Scene name is not set. Cannot load level.", CFGError.ErrorCode.NotInitialized);
			return false;
		}
		Debug.Log("Loading level... " + m_Current_Scene);
		CFGSerializableObject.ResetFreeID();
		ResetMissionStats();
		CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.LoadingMission);
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		instance.UnloadMainMenus();
		instance.UnloadCharacterScreen();
		instance.UnloadBarterScreen();
		instance.UnloadStrategicExplorator();
		instance.UnloadHUD();
		instance.UnloadInGameMenu();
		if (CFGSingleton<CFGGame>.Instance.m_FastLoadLevelId == -1)
		{
			instance.LoadLoadingScreen();
		}
		StartCoroutine(LoadLevelAsync(m_Current_Scene));
		return true;
	}

	public override void OnMissionLoaded()
	{
		Debug.Log("OnMissionLoaded");
		if (GetLevelType() == CFG_SG_SaveGame.eSG_Mode.Strategic)
		{
			CFGSingleton<CFGGame>.Instance.CreateHUDStrategic();
		}
		base.OnMissionLoaded();
	}

	private void Update()
	{
		if (mainSequence != null && !m_IsLoadingLevel)
		{
			mainSequence.UpdateFlow(Time.deltaTime);
		}
	}

	public override void OnMissionUnloadingStart()
	{
		base.OnMissionUnloadingStart();
		CFGSelectionManager.Instance.OnTacticalEnd();
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalEnd();
		CFGCharacterList.OnTacticalEnd();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnTacticalEnd();
		CFGSingleton<CFGGame>.Instance.DestroyHUD();
	}

	public override void MissionComplete()
	{
		if (m_MissionStats.finished)
		{
			Debug.Log("Mission already finished!");
			return;
		}
		Debug.Log("Mission Complete");
		m_MissionStats.success = true;
		m_MissionStats.play_count++;
		m_MissionStats.finished = true;
		m_MissionStats.scenario_end = false;
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.UpdateAi = false;
		}
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnMissionComplete();
		CFGAchievmentTracker.OnTacticalWin();
		m_Current_Tactical = null;
		StopMissionFlow();
		CFGAudioManager.Instance.OnMissionEnd();
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			if (instance.m_StrategicExploratorButtons != null && instance.m_StrategicExploratorButtons.m_OptionsButton != null)
			{
				instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = false;
			}
			instance.LoadMissionEnd();
			instance.m_MissionEnd.Init_MissionComplete();
			instance.UnloadHUD();
		}
	}

	public override void MissionFail(string reason_id, params string[] args)
	{
		if (m_MissionStats.finished)
		{
			Debug.Log("Mission already finished!");
			return;
		}
		if (reason_id == null)
		{
			reason_id = "Podepnij mnie, bom nullem jest";
		}
		Debug.Log("Mission Fail (reason_id - " + reason_id + ")");
		StopMissionFlow();
		m_MissionStats.success = false;
		m_MissionStats.reason_id = reason_id;
		m_MissionStats.play_count++;
		m_MissionStats.finished = true;
		m_MissionStats.scenario_end = false;
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.UpdateAi = false;
		}
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnMissionFail();
		CFGAudioManager.Instance.OnMissionEnd();
		if (CFGGame.Permadeath && m_Current_Campaign != null && m_Current_Scenario != null)
		{
			string variableName = GenerateVariable(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID, "progress");
			CFGVar variable = CFGVariableContainer.Instance.GetVariable(variableName, "profile");
			if (variable != null)
			{
				variable.Value = 0;
				CFGVariableContainer.Instance.SaveValuesGlobal(null);
			}
			else
			{
				Debug.LogWarning("Failed to find current scenario progress");
			}
			CFG_SG_Manager.RemoveScenarioProgress(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID);
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			if (instance.m_StrategicExploratorButtons != null && instance.m_StrategicExploratorButtons.m_OptionsButton != null)
			{
				instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = false;
			}
			instance.LoadMissionEnd();
			instance.m_MissionEnd.Init_MissionFail(reason_id, args);
			instance.UnloadHUD();
		}
	}

	public override void ScenarioComplete()
	{
		if (m_MissionStats.finished)
		{
			Debug.Log("Mission already finished!");
			return;
		}
		Debug.Log("Scenario Complete");
		m_MissionStats.success = true;
		m_MissionStats.play_count++;
		m_MissionStats.finished = true;
		m_MissionStats.scenario_end = true;
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.UpdateAi = false;
		}
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnMissionComplete();
		if (m_Current_Tactical != null)
		{
			CFGAchievmentTracker.OnTacticalWin();
		}
		if (m_Current_Campaign != null && m_Current_Scenario != null)
		{
			CFGAchievmentTracker.OnScenarioWin(m_Current_Campaign.CampaignID, m_Current_Scenario.Index);
			CFG_SG_Manager.RemoveScenarioProgress(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID);
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(GenerateVariable(CampaignName, ScenarioName, "completed"), "profile");
		if (variable != null)
		{
			variable.Value = true;
		}
		SetScenarioProgress(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID, 100);
		CFGVariableContainer.Instance.SaveValuesGlobal(null);
		m_Current_Tactical = null;
		m_Current_Scenario = null;
		StopMissionFlow();
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			if (instance.m_StrategicExploratorButtons != null && instance.m_StrategicExploratorButtons.m_OptionsButton != null)
			{
				instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = false;
			}
			instance.LoadMissionEnd();
			instance.m_MissionEnd.Init_ScenarioComplete();
			instance.UnloadHUD();
		}
	}

	public override void CampaignComplete(int video_idx)
	{
		if (m_MissionStats.finished)
		{
			Debug.Log("Mission already finished!");
			return;
		}
		Debug.Log("Campaign Complete");
		m_MissionStats.success = true;
		m_MissionStats.play_count++;
		m_MissionStats.finished = true;
		m_MissionStats.scenario_end = true;
		m_MissionStats.video_idx = video_idx;
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner != null)
		{
			aiOwner.UpdateAi = false;
		}
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnMissionComplete();
		if (m_Current_Tactical != null)
		{
			CFGAchievmentTracker.OnTacticalWin();
		}
		if (m_Current_Campaign != null && m_Current_Scenario != null)
		{
			CFGAchievmentTracker.OnScenarioWin(m_Current_Campaign.CampaignID, m_Current_Scenario.Index);
			CFG_SG_Manager.RemoveScenarioProgress(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID);
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(GenerateVariable(CampaignName, ScenarioName, "completed"), "profile");
		if (variable != null)
		{
			variable.Value = true;
		}
		SetScenarioProgress(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID, 100);
		CFGVariableContainer.Instance.SaveValuesGlobal(null);
		m_Current_Tactical = null;
		m_Current_Scenario = null;
		StopMissionFlow();
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			if (instance.m_StrategicExploratorButtons != null && instance.m_StrategicExploratorButtons.m_OptionsButton != null)
			{
				instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = false;
			}
			instance.UnloadHUD();
		}
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: true);
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: true);
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeOut, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, 1f, OnFadeOutEnd_Quit);
		}
		else
		{
			OnFadeOutEnd_Quit();
		}
	}

	private void OnFadeOutEnd_Quit()
	{
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: false);
		CFGSingleton<CFGGame>.Instance.GoToOutroMovie();
	}

	public SMissionStats GetMissionStats()
	{
		return m_MissionStats;
	}

	public bool IsLoadingLevel()
	{
		return m_IsLoadingLevel;
	}

	public bool IsFastLoad()
	{
		return m_FastLoad;
	}

	private IEnumerator LoadLevelAsync(string level_name)
	{
		Debug.Log("Loading level " + level_name);
		m_LoadingProgress = 0f;
		m_IsLoadingLevel = true;
		m_FastLoad = false;
		m_bTacticalRestarted = false;
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Clear();
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.OnNewMission();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnLevelEnd();
		bool bCallStart = false;
		if (CFGSingleton<CFGGame>.Instance.m_FastLoadLevelId != -1 || m_QuickLoading)
		{
			m_FastLoad = true;
			bCallStart = true;
			CFGSingleton<CFGGame>.Instance.m_FastLoadLevelId = -1;
			CFGSingleton<CFGGame>.Instance.m_FastLoading = false;
		}
		m_QuickLoading = false;
		if (CFG_SG_Manager.IsLoadingSaveGame && !CFG_SG_Manager.IsLoadingStrategic)
		{
			m_bCreateAutoSave = false;
		}
		else
		{
			m_bCreateAutoSave = true;
		}
		AsyncOperation async = Application.LoadLevelAsync(level_name);
		while (!async.isDone)
		{
			m_LoadingProgress = async.progress;
			yield return null;
		}
		CFGAchievmentTracker.OnMissionStart();
		Debug.Log("Loading complete");
		if (CFG_SG_Manager.IsLoading)
		{
			if (CFG_SG_Manager.LoadType == CFG_SG_Manager.eLoadType.ResetTactical)
			{
				m_bTacticalRestarted = true;
			}
			if (CFG_SG_Manager.CurrentSaveGame != null && CFGSingleton<CFGGame>.Instance.LevelSettings != null)
			{
				if (CFGSingleton<CFGGame>.Instance.LevelSettings.m_LevelType == CFG_SG_Manager.CurrentSaveGame.SaveGameMode && CFG_SG_Manager.LoadType != CFG_SG_Manager.eLoadType.ResetTactical && CFG_SG_Manager.CurrentSaveGame.SceneVersion == CFGSingleton<CFGGame>.Instance.LevelSettings.UID_Version)
				{
				}
			}
			else
			{
				Debug.LogWarning("Failed to check version");
			}
			CFG_SG_Manager.SynchronizeScene();
		}
		if (m_Current_Tactical != null)
		{
			CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.InGame);
			CFGWindowMgr window_mgr = CFGSingleton<CFGWindowMgr>.Instance;
			window_mgr.LoadHUD();
		}
		else
		{
			CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.Strategic);
		}
		m_LoadingProgress = 1.2f;
		CFGCamera camera = Camera.main.GetComponent<CFGCamera>();
		if ((bool)camera)
		{
			camera.SetEnabled(enabled: false);
		}
		m_HUD_Restored = false;
		m_HUD_CanRestore = false;
		if (CFG_SG_Manager.IsLoading)
		{
			CFG_SG_Manager.DoPostSync();
			switch (CFG_SG_Manager.LoadType)
			{
			case CFG_SG_Manager.eLoadType.NoLoad:
			case CFG_SG_Manager.eLoadType.ResetTactical:
				if (mainSequence != null)
				{
					mainSequence.Activated();
				}
				break;
			}
		}
		else if (mainSequence != null)
		{
			mainSequence.Activated();
		}
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnLevelStart();
		if (bCallStart)
		{
			m_HUD_CanRestore = true;
			StartLevel();
		}
	}

	private bool ShouldDoFadeIn()
	{
		if (!CFG_SG_Manager.IsLoading)
		{
			return false;
		}
		if (CFG_SG_Manager.LoadType == CFG_SG_Manager.eLoadType.ReturnToStrategic)
		{
			if (m_Current_Scenario.ScenarioID == "scen_07")
			{
				return true;
			}
			return false;
		}
		if (CFG_SG_Manager.LoadType != CFG_SG_Manager.eLoadType.ResetTactical && (CFG_SG_Manager.SGSource != CFG_SG_SaveGame.eSG_Source.AutoSaveOnLoad || CFG_SG_Manager.SGMode == CFG_SG_SaveGame.eSG_Mode.Strategic))
		{
			return true;
		}
		return false;
	}

	public void StartLevel()
	{
		Debug.Log("CFGSessionSingle.StartLevel()");
		CFGTimer.OnMissionStart();
		if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.FullDeck))
		{
			CFGInventory.CollectAllCards();
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		instance.UnloadLoadingScreen();
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.SetEnabled(enabled: true);
			CFGFadeToColor componentInChildren = component.GetComponentInChildren<CFGFadeToColor>();
			if (componentInChildren != null)
			{
				if ((CFGSingleton<CFGGame>.Instance.LevelSettings != null && CFGSingleton<CFGGame>.Instance.LevelSettings.FadeInOnStart) || ShouldDoFadeIn())
				{
					CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
					if ((bool)component2)
					{
						component2.SetLock(ELockReason.FadeOut, bLock: true);
					}
					component.EnableCameraControlLock(ELockReason.FadeOut, Enable: true);
					componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeIn, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, 1f, OnFadeInEnd);
				}
				else
				{
					component.EnableCameraControlLock(ELockReason.FadeOut, Enable: false);
					componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeIn, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, 0f, null);
				}
			}
			else
			{
				Debug.Log("No CFGFadeToColor component in the camera!");
			}
		}
		m_IsLoadingLevel = false;
		m_HUD_CanRestore = true;
		RestoreHUD();
		if (m_Current_Tactical != null)
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalStart();
			CFGCharacterList.OnTacticalStart();
			CFGSelectionManager.Instance.OnTacticalStart();
		}
		else
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnStrategicStart();
			CFGCharacterList.OnStrategicStart();
			CFGSelectionManager.Instance.OnStrategicStart();
		}
		if (CFG_SG_Manager.IsLoading)
		{
			CFG_SG_Manager.FinalizeLoading();
		}
		CFGEconomy.UpdateFateTrader();
		CFGFlowSequence cFGFlowSequence = CFGSequencerBase.GetMainSequence();
		if (cFGFlowSequence != null)
		{
			if (cFGFlowSequence.m_ActivateCount == 0)
			{
				cFGFlowSequence.Activated();
			}
			else if (!cFGFlowSequence.m_Active)
			{
				cFGFlowSequence.m_Active = true;
			}
		}
		if (m_bCreateAutoSave)
		{
			Debug.Log("AutoSave on level start " + m_Current_Scene);
			CFGSingleton<CFGGame>.Instance.CreateSaveGame(CFG_SG_SaveGame.eSG_Source.AutoSaveOnLoad);
			m_bCreateAutoSave = false;
		}
	}

	private void OnFadeInEnd()
	{
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = false;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.EnableCameraControlLock(ELockReason.FadeOut, Enable: false);
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.SetLock(ELockReason.FadeOut, bLock: false);
		}
	}

	public void RestoreHUD()
	{
		if (!m_HUD_Restored && m_HUD_CanRestore)
		{
			CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.FinalizeObjectiveLoad();
			m_HUD_Restored = true;
			CFGSingleton<CFGWindowMgr>.Instance.ShowUI();
		}
	}

	private void StopMissionFlow()
	{
		CFGSequencer cFGSequencer = Object.FindObjectOfType<CFGSequencer>();
		if ((bool)cFGSequencer && (bool)cFGSequencer.MainSequence)
		{
			cFGSequencer.MainSequence.StopExecution();
		}
	}

	public CFG_SG_SaveGame.eSG_Mode GetLevelType()
	{
		if (Object.FindObjectsOfType(typeof(CFGLevelSettings)) is CFGLevelSettings[] array && array.Length > 0 && array[0].m_LevelType != 0)
		{
			Debug.Log("Level type (from levelsettings): " + array[0].m_LevelType);
			return array[0].m_LevelType;
		}
		string loadedLevelName = Application.loadedLevelName;
		string[] strategicMaps = CFGSingletonResourcePrefab<CFGGameSettings>.Instance.m_StrategicMaps;
		foreach (string strA in strategicMaps)
		{
			if (string.Compare(strA, loadedLevelName, ignoreCase: true) == 0)
			{
				return CFG_SG_SaveGame.eSG_Mode.Strategic;
			}
		}
		string[] tacticalMaps = CFGSingletonResourcePrefab<CFGGameSettings>.Instance.m_TacticalMaps;
		foreach (string strA2 in tacticalMaps)
		{
			if (string.Compare(strA2, loadedLevelName, ignoreCase: true) == 0)
			{
				return CFG_SG_SaveGame.eSG_Mode.Tactical;
			}
		}
		Debug.LogWarning("Unknown level type!");
		return CFG_SG_SaveGame.eSG_Mode.Strategic;
	}

	public override bool CreateSaveGame(CFG_SG_SaveGame.eSG_Mode _Mode, CFG_SG_SaveGame.eSG_Source _Source)
	{
		if (m_Current_Campaign == null || m_Current_Scenario == null)
		{
			return false;
		}
		string text = CFG_SG_Manager.GenerateSaveGameName(m_Current_Campaign.CampaignID, m_Current_Scenario.ScenarioID, _Source);
		if (text == null)
		{
			return false;
		}
		return CFG_SG_Manager.CreateSaveGame(CFG_SG_SaveGame.eSG_Type.SinglePlayer, _Mode, _Source, text);
	}

	public static string GenerateVariable(string CampaignID, string ScenarioID, string PostID)
	{
		if (CampaignID == null || ScenarioID == null || PostID == null)
		{
			return null;
		}
		return CampaignID + "_" + ScenarioID + "_" + PostID;
	}

	public static void SetScenarioProgress(string CampaignID, string ScenarioID, int Prog)
	{
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(GenerateVariable(CampaignID, ScenarioID, "progress"), "profile");
		if (variable != null)
		{
			variable.Value = Prog;
			Debug.Log("Progress changed to " + Prog);
			CFGVariableContainer.Instance.SaveValuesGlobal(null);
		}
		else
		{
			Debug.LogError("Failed to find scenario progress variable Camp = " + CampaignID + " Scenario = " + ScenarioID);
		}
	}

	public void ReadCampaignVariableDefinitions()
	{
		if (m_Current_Campaign == null)
		{
			Debug.LogWarning("No campaign defined. cannot load definitions.");
		}
		else
		{
			ReadCampaignVariables(m_Current_Campaign.CampaignID);
		}
	}

	public static void ReadCampaignVariables(string CampID)
	{
		Debug.Log("Reading campaing variable definitions");
		CFGVariableContainer.Instance.ClearScope("campaign");
		CFGVariableContainer.Instance.ReadVariableDefinitions(CFGData.GetDataPathFor("Variables/" + $"vars_{CampID}.tsv"));
	}

	public void ReadScenarioVariableDefinitions()
	{
		Debug.Log("Reading scenario variable definitions");
		if (m_Current_Scenario == null)
		{
			Debug.LogWarning("No scenario defined. cannot load.");
			return;
		}
		CFGVariableContainer.Instance.ClearScope("scenario");
		CFGVariableContainer.Instance.ReadVariableDefinitions(CFGData.GetDataPathFor("Variables/" + $"vars_{m_Current_Scenario.ScenarioID}.tsv"));
	}
}
