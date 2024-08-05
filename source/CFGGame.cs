using UnityEngine;

public class CFGGame : CFGSingleton<CFGGame>
{
	public delegate void LevelLoaded(int id);

	public const int LEVEL_INTRO = 0;

	public const int LEVEL_LOADER = 1;

	public const int LEVEL_MAIN_MENU1 = 2;

	public const int LEVEL_MOVIE = 3;

	public const int LEVEL_MAIN_MENU2 = 4;

	public const int LEVEL_MAIN_MENU3 = 5;

	private static bool s_bNightmare = false;

	private static bool s_bUpdateCharShadows = false;

	private static EDifficulty s_Difficulty = EDifficulty.Normal;

	private static bool s_Injuries = true;

	private static bool s_Permadeath = false;

	private static bool s_DifficultyChanged = false;

	private static EDifficulty s_LowestDifficulty = s_Difficulty;

	private static EDLC s_CurrentDLC = EDLC.None;

	private static int s_CustomGameplayOptions = 0;

	private static string s_MainMenuScene = null;

	private EGameState m_GameState;

	private CFGSession m_Session;

	private CFGLevelSettings m_LevelSettings;

	public int m_FastLoadLevelId = -1;

	public int m_FastLoadStrategicId = -1;

	public bool m_FastLoading;

	public static bool s_DontInitApp = false;

	public static bool s_LoadCreditsFirst = false;

	public static int NewGamePlusFlags
	{
		get
		{
			return s_CustomGameplayOptions;
		}
		set
		{
			s_CustomGameplayOptions = value;
		}
	}

	public static EDLC DlcType => s_CurrentDLC;

	public static bool InjuriesEnabled
	{
		get
		{
			return s_Injuries;
		}
		set
		{
			s_Injuries = value;
		}
	}

	public static bool Permadeath
	{
		get
		{
			return s_Permadeath;
		}
		set
		{
			s_Permadeath = value;
		}
	}

	public static bool DifficultyChanged => s_DifficultyChanged;

	public static EDifficulty ScenarioLowestDifficulty => s_LowestDifficulty;

	public static EDifficulty Difficulty
	{
		get
		{
			return s_Difficulty;
		}
		set
		{
			if (s_Difficulty != value)
			{
				EDifficulty eDifficulty = s_Difficulty;
				s_Difficulty = value;
				CFGCharacterList.OnDifficultyLevelChanged(eDifficulty, s_Difficulty);
				s_DifficultyChanged = true;
				if (eDifficulty > value)
				{
					s_LowestDifficulty = value;
				}
			}
		}
	}

	public bool IsDarkness
	{
		get
		{
			if (s_bNightmare)
			{
				return true;
			}
			if (m_LevelSettings == null)
			{
				return false;
			}
			return m_LevelSettings.m_Darkness;
		}
	}

	public static bool Nightmare
	{
		get
		{
			return s_bNightmare;
		}
		set
		{
			s_bNightmare = value;
		}
	}

	public CFGLevelSettings LevelSettings
	{
		get
		{
			return m_LevelSettings;
		}
		set
		{
			m_LevelSettings = value;
		}
	}

	public bool CanSaveGame
	{
		get
		{
			if (m_Session == null)
			{
				return false;
			}
			if (!IsInGame() && !IsInStrategic())
			{
				return false;
			}
			if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
			{
				return false;
			}
			if (CFGSelectionManager.Instance.IsLocked && (CFGSelectionManager.Instance.LockStatus != ELockReason.Wnd_InGameMenu || CFGInput.LastReadInputDevice != EInputMode.Gamepad))
			{
				return false;
			}
			if (CFGTimer.IsPaused_Gameplay)
			{
				return false;
			}
			if (CFGSelectionManager.Instance.AllowedMode != 0)
			{
				return false;
			}
			if (Permadeath)
			{
				return false;
			}
			return true;
		}
	}

	public bool CanQuickLoad
	{
		get
		{
			if (!CFG_SG_Manager.QuickSaveExist())
			{
				return false;
			}
			if (m_Session == null)
			{
				return false;
			}
			if (!IsInGame() && !IsInStrategic())
			{
				return false;
			}
			if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
			{
				return false;
			}
			if (CFGSelectionManager.Instance.IsLocked && (CFGSelectionManager.Instance.LockStatus != ELockReason.Wnd_MissionEnd || CFGSingleton<CFGWindowMgr>.Instance.m_MissionEnd.m_Icon.IconNumber != 1) && (CFGSelectionManager.Instance.LockStatus != ELockReason.Wnd_InGameMenu || CFGInput.LastReadInputDevice != EInputMode.Gamepad))
			{
				return false;
			}
			if (CFGTimer.IsPaused_Gameplay)
			{
				return false;
			}
			if (CFGSelectionManager.Instance.AllowedMode != 0)
			{
				return false;
			}
			if (Permadeath)
			{
				return false;
			}
			return true;
		}
	}

	public CFGSessionSingle SessionSingle => m_Session as CFGSessionSingle;

	public static bool IsCustomGameplayOptionActive(ECustomGameplayOption Option)
	{
		if (((uint)s_CustomGameplayOptions & (uint)Option) == (uint)Option)
		{
			return true;
		}
		return false;
	}

	public static void SetCustomGameplayOption(ECustomGameplayOption Option, bool bActive)
	{
		if (bActive)
		{
			s_CustomGameplayOptions |= (int)Option;
		}
		else
		{
			s_CustomGameplayOptions &= (int)(~Option);
		}
	}

	public static void SetCurrentDLC(EDLC NewDLC)
	{
		s_CurrentDLC = NewDLC;
		CFGStaticDataContainer.InitDLCData(NewDLC);
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.LoadSpeakers();
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.LoadLanguageFiles();
		CFGSingletonResourcePrefab<CFGTextManager>.Instance.LoadLanguageFiles();
		CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.Init();
	}

	public static void OnScenarioStart(EDifficulty ScenarioDiff)
	{
		s_Difficulty = ScenarioDiff;
		s_LowestDifficulty = s_Difficulty;
		s_DifficultyChanged = false;
	}

	public static void OnDeserializeMain(bool bInjuries, bool bPermadeath, bool bDiffChanged, EDifficulty Current, EDifficulty MinDiff, int NewGamePlusFlags)
	{
		s_Difficulty = Current;
		s_DifficultyChanged = bDiffChanged;
		s_Injuries = bInjuries;
		s_Permadeath = bPermadeath;
		s_LowestDifficulty = MinDiff;
		s_CustomGameplayOptions = NewGamePlusFlags;
	}

	public override void Init()
	{
		base.Init();
		Debug.Log("Initializing Game");
		m_GameState = EGameState.Initializing;
		DetectLevelType();
	}

	public bool CanSaveWhileInGameMenu()
	{
		if (m_Session == null)
		{
			return false;
		}
		if (!IsInGame() && !IsInStrategic())
		{
			return false;
		}
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			return false;
		}
		if (CFGSelectionManager.Instance.IsLocked && CFGSelectionManager.Instance.LockStatus != ELockReason.Wnd_InGameMenu)
		{
			return false;
		}
		if (CFGSelectionManager.Instance.AllowedMode != 0)
		{
			return false;
		}
		if (Permadeath)
		{
			return false;
		}
		return true;
	}

	public bool CanLoadWhileInGameMenu()
	{
		if (m_Session == null)
		{
			return false;
		}
		if (!IsInGame() && !IsInStrategic())
		{
			return false;
		}
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			return false;
		}
		if (CFGSelectionManager.Instance.IsLocked && CFGSelectionManager.Instance.LockStatus != ELockReason.Wnd_InGameMenu)
		{
			return false;
		}
		if (CFGSelectionManager.Instance.AllowedMode != 0)
		{
			return false;
		}
		if (Permadeath)
		{
			return false;
		}
		return true;
	}

	public CFGSessionSingle CreateSessionSingle()
	{
		if (m_Session != null)
		{
			DestroySession();
		}
		Debug.Log("Creating session (singleplayer)");
		return (CFGSessionSingle)(m_Session = base.gameObject.AddComponent<CFGSessionSingle>());
	}

	public void DestroySession()
	{
		if (m_Session != null)
		{
			Debug.Log("Destroying session");
			Object.Destroy(m_Session);
			m_Session = null;
		}
	}

	public CFGSession GetSession()
	{
		return m_Session;
	}

	public bool IsInMainMenu()
	{
		return m_GameState == EGameState.MainMenu;
	}

	public bool IsInGame()
	{
		return m_GameState == EGameState.InGame;
	}

	public bool IsInStrategic()
	{
		return m_GameState == EGameState.Strategic;
	}

	public void SetGameState(EGameState game_state)
	{
		m_GameState = game_state;
		if (m_GameState == EGameState.MainMenu)
		{
			m_LevelSettings = null;
		}
	}

	public EGameState GetGameState()
	{
		return m_GameState;
	}

	public void GoToMainMenus()
	{
		if (m_GameState == EGameState.InGame || m_GameState == EGameState.UnloadingMission)
		{
			DestroyHUD();
			if (m_Session != null)
			{
				m_Session.OnMissionUnloadingStart();
			}
		}
		else if (m_GameState == EGameState.Strategic)
		{
			DestroyHUDStrategic();
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: false);
		DestroySession();
		CFGCharacterList.OnCampaignStart();
		int seed = (int)(Mathf.Abs(Time.realtimeSinceStartup) * 2687f * Random.Range(1f, 177f)) & (268435455 + Random.Range(13, 11795));
		Random.seed = seed;
		switch (Random.Range(0, 99) % 3)
		{
		default:
			s_MainMenuScene = "MainMenu";
			break;
		case 1:
			s_MainMenuScene = "MainMenu_2";
			break;
		case 2:
			s_MainMenuScene = "MainMenu_3";
			break;
		}
		Debug.Log("Loading MainMenu Scene: " + s_MainMenuScene);
		Application.LoadLevel(s_MainMenuScene);
	}

	public void GoToOutroMovie()
	{
		if (m_GameState == EGameState.InGame || m_GameState == EGameState.UnloadingMission)
		{
			DestroyHUD();
			if (m_Session != null)
			{
				m_Session.OnMissionUnloadingStart();
			}
		}
		else if (m_GameState == EGameState.Strategic)
		{
			DestroyHUDStrategic();
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: false);
		CFGAudioManager.Instance.StopBackgroundMusic();
		CFGCharacterList.OnCampaignStart();
		Application.LoadLevel("Movie");
	}

	public void GoToStoryMovie()
	{
		CFGAudioManager.Instance.StopBackgroundMusic();
		Application.LoadLevel("Movie");
	}

	public void MissionComplete()
	{
		if (m_Session != null)
		{
			m_Session.MissionComplete();
		}
	}

	public void MissionFail(string reason_id, params string[] args)
	{
		if (m_Session != null)
		{
			m_Session.MissionFail(reason_id, args);
		}
	}

	public void ScenarioComplete()
	{
		if (m_Session != null)
		{
			m_Session.ScenarioComplete();
		}
	}

	public void CampaignComplete(int video_idx)
	{
		if (m_Session != null)
		{
			m_Session.CampaignComplete(video_idx);
		}
	}

	public static void EnableUpdate(bool Vampire, bool CharShadows)
	{
		if (CharShadows)
		{
			s_bUpdateCharShadows = true;
		}
	}

	public static void UpdateShadowStuff()
	{
		if (!s_bUpdateCharShadows)
		{
			return;
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (!(character == null) && character.IsAlive)
			{
				character.RecalculateSelfShadow();
			}
		}
		s_bUpdateCharShadows = false;
	}

	public static void SetNightmareMode(bool enabled, bool onLevelStart)
	{
		Nightmare = enabled;
		CFGSpawnPostProcessCamera component = Camera.main.GetComponent<CFGSpawnPostProcessCamera>();
		if (component != null)
		{
			component.SetupNightmareMode(enabled, onLevelStart);
		}
		CFGCellMap.ToggleAdditionalArt(!enabled);
		CFGLevelSettings levelSettings = CFGSingleton<CFGGame>.Instance.LevelSettings;
		if (levelSettings != null)
		{
			if (levelSettings.m_NormalEnvSoundsRoot != null)
			{
				levelSettings.m_NormalEnvSoundsRoot.SetActive(!enabled);
			}
			if (levelSettings.m_NightmareEnvSoundsRoot != null)
			{
				levelSettings.m_NightmareEnvSoundsRoot.SetActive(enabled);
			}
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			character.UpdateVampire();
		}
		UpdateShadowStuff();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (!s_DontInitApp)
		{
			CFGApplication.Init();
		}
	}

	private void DetectLevelType()
	{
		CFGStaticDataContainer.Init();
		CFGEconomy.Reset();
		if (string.Compare(Application.loadedLevelName, "loader", ignoreCase: true) == 0)
		{
			GoToMainMenus();
			return;
		}
		CFGSessionSingle cFGSessionSingle = CreateSessionSingle();
		string CampaignName = null;
		string ScenarioName = null;
		string TacticalID = null;
		CFGStaticDataContainer.GetCampaignAndScenarioFromSceneName(Application.loadedLevelName, out CampaignName, out ScenarioName, out TacticalID);
		if (CampaignName == null || ScenarioName == null)
		{
			CampaignName = "DevTestCampaign";
			ScenarioName = "DevTestScenario";
			CFGDef_Tactical tacticalByID = CFGStaticDataContainer.GetTacticalByID("DevTestTactical");
			if (tacticalByID != null)
			{
				tacticalByID.SceneName = Application.loadedLevelName;
			}
			TacticalID = "DevTestTactical";
		}
		cFGSessionSingle.SelectCampaign(CampaignName);
		switch (cFGSessionSingle.GetLevelType())
		{
		case CFG_SG_SaveGame.eSG_Mode.Unknown:
			m_GameState = EGameState.Error;
			break;
		case CFG_SG_SaveGame.eSG_Mode.Tactical:
			CFGSingleton<CFGWindowMgr>.Instance.Init();
			CFGSingleton<CFGWindowMgr>.Instance.LoadHUD();
			cFGSessionSingle.StartScenario(ScenarioName, TacticalID, IsTactical: true);
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnTacticalStart();
			CFGSelectionManager.Instance.OnTacticalStart();
			SetGameState(EGameState.InGame);
			if (LevelSettings != null)
			{
				SetNightmareMode(!LevelSettings.StartInNormalMode, onLevelStart: true);
			}
			else
			{
				SetNightmareMode(enabled: false, onLevelStart: true);
			}
			break;
		case CFG_SG_SaveGame.eSG_Mode.Strategic:
			CFGSingleton<CFGWindowMgr>.Instance.Init();
			CreateHUDStrategic();
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OnStrategicStart();
			CFGSelectionManager.Instance.OnStrategicStart();
			CFGCharacterList.OnStrategicStart();
			SetGameState(EGameState.Strategic);
			cFGSessionSingle.PrepareLevel_Strategic(ScenarioName);
			break;
		}
		CFGVariableContainer.Instance.Init();
		cFGSessionSingle.ReadCampaignVariableDefinitions();
		cFGSessionSingle.ReadScenarioVariableDefinitions();
		CFGEconomy.UpdateFateTrader();
		m_FastLoading = true;
	}

	private void Update()
	{
		CFGInput.Update();
		CFGJoyManager.OnUpdate();
		CFGTimer.Update();
		UpdateShadowStuff();
		if (m_FastLoading)
		{
			Debug.Log("Fast loading");
			m_FastLoading = false;
			if ((bool)m_Session)
			{
				m_Session.OnMissionLoaded();
				(m_Session as CFGSessionSingle).StartLevel();
			}
			CFGFlowSequence mainSequence = CFGSequencerBase.GetMainSequence();
			if (mainSequence != null)
			{
				mainSequence.Activated();
			}
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.SetupCurrentOwner(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner);
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.StartTurn();
		}
		if (CFGOptions.Gameplay.QuickSaveEnabled)
		{
			if (CFGInput.IsActivated(EActionCommand.EXP_QuickSave))
			{
				Debug.Log("QuickSave!");
				if (!CanSaveGame)
				{
					Debug.LogWarning("Cannot save game right now. Please try later");
				}
				else
				{
					CreateSaveGame(CFG_SG_SaveGame.eSG_Source.QuickSave);
				}
			}
			if (CFGInput.IsActivated(EActionCommand.EXP_QuickLoad))
			{
				Debug.Log("QuickLoad!");
				if (!CanQuickLoad)
				{
					Debug.LogWarning("Cannot load game right now. Please try later");
				}
				else
				{
					CFG_SG_Manager.LoadQuickSave();
				}
			}
			if ((bool)CFGSelectionManager.Instance && CFGSelectionManager.Instance.IsLocked && CFGSelectionManager.Instance.LockStatus == ELockReason.Wnd_InGameMenu)
			{
				if (CFGJoyManager.IsActivated(EJoyAction.EXP_QuickSave))
				{
					Debug.Log("QuickSave!");
					if (CanSaveWhileInGameMenu())
					{
						CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu();
						CFGTimer.SetPaused_Gameplay(bPauseGameplay: false);
						CFGSelectionManager.GlobalLock(ELockReason.Wnd_InGameMenu, bEnable: false);
						Input.ResetInputAxes();
						CFGJoyManager.ClearJoyActions();
						CreateSaveGame(CFG_SG_SaveGame.eSG_Source.QuickSave);
					}
					else
					{
						Debug.LogWarning("Cannot save game right now. Please try later");
					}
				}
				if (CFGJoyManager.IsActivated(EJoyAction.EXP_QuickLoad))
				{
					Debug.Log("QuickLoad!");
					if (CanLoadWhileInGameMenu())
					{
						CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu();
						CFGTimer.SetPaused_Gameplay(bPauseGameplay: false);
						CFGSelectionManager.GlobalLock(ELockReason.Wnd_InGameMenu, bEnable: false);
						Input.ResetInputAxes();
						CFGJoyManager.ClearJoyActions();
						CFG_SG_Manager.LoadQuickSave();
					}
					else
					{
						Debug.LogWarning("Cannot load game right now. Please try later");
					}
				}
			}
		}
		if (!CFGInput.IsActivated(EActionCommand.SYS_ToggleHUD) || (!IsInGame() && !IsInStrategic()))
		{
			return;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			if (instance.IsUIVisible())
			{
				instance.HideUI();
			}
			else
			{
				instance.ShowUI();
			}
		}
	}

	public void CreateSaveGame(CFG_SG_SaveGame.eSG_Source _Src)
	{
		if (!(SessionSingle == null))
		{
			SessionSingle.CreateSaveGame((!IsInGame()) ? CFG_SG_SaveGame.eSG_Mode.Strategic : CFG_SG_SaveGame.eSG_Mode.Tactical, _Src);
		}
	}

	public void OnEscapeKey()
	{
		if (IsInGame() || IsInStrategic())
		{
			CFGSessionSingle cFGSessionSingle = m_Session as CFGSessionSingle;
			if ((bool)cFGSessionSingle && !cFGSessionSingle.IsLoadingLevel() && CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons != null && ((CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen != null && !CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen.m_CombatLoadout) || CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen == null) && CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons.m_OptionsButton != null && CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons.m_OptionsButton.enabled && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.LoadInGameMenu();
			}
		}
	}

	public bool StartTrade(string ShopID)
	{
		if (!CFGEconomy.SelectShop(ShopID))
		{
			return false;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			instance.LoadBarterScreen();
		}
		return true;
	}

	private void OnLevelWasLoaded(int level)
	{
		CFGVariableContainer.Instance.UnloadLocals();
		switch (level)
		{
		case 0:
		case 1:
		case 3:
			CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: false);
			break;
		case 2:
		case 4:
		case 5:
			CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: false);
			if (s_LoadCreditsFirst)
			{
				CFGSingleton<CFGWindowMgr>.Instance.LoadCreditsPanel();
				s_LoadCreditsFirst = false;
			}
			else
			{
				CreateMainMenus();
			}
			OnMainMenuCreated();
			break;
		default:
			if (m_Session != null)
			{
				m_Session.OnMissionLoaded();
			}
			break;
		}
	}

	private void CreateMainMenus()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadMainMenus();
	}

	private void DestroyMainMenus()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus();
	}

	public void CreateHUDStrategic()
	{
		Debug.Log("CreateIntermission");
		CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicExplorator(OnHUDStrategicCreated);
	}

	private void DestroyHUDStrategic()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadStrategicExplorator();
	}

	public void DestroyHUD()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
	}

	private void OnMainMenuCreated()
	{
		if (m_GameState == EGameState.Strategic)
		{
			DestroyHUDStrategic();
		}
		m_GameState = EGameState.MainMenu;
		if (m_FastLoadLevelId != -1)
		{
			Debug.Log("Fast load level id: " + m_FastLoadLevelId);
			CFGSessionSingle cFGSessionSingle = CreateSessionSingle();
			cFGSessionSingle.SelectCampaign(0);
			cFGSessionSingle.PrepareLevel_AutoTactical(CFGSingletonResourcePrefab<CFGGameSettings>.Instance.m_TacticalMaps[m_FastLoadLevelId]);
			cFGSessionSingle.LoadLevel();
		}
		else
		{
			AudioClip menuMusic = CFGSingletonResourcePrefab<CFGGameSettings>.Instance.m_MenuMusic;
			if (menuMusic != null)
			{
				CFGAudioManager.Instance.PlayBackgroundMusic(menuMusic);
			}
		}
	}

	private void OnHUDStrategicCreated()
	{
		Debug.Log("OnIntermissionCreated");
		if (m_GameState == EGameState.MainMenu)
		{
			DestroyMainMenus();
		}
		m_GameState = EGameState.Strategic;
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		Debug.Log("Application's focus has changed; Have focus : " + focusStatus);
		CFGInput.OnApplicationChangeFocus(focusStatus);
	}
}
