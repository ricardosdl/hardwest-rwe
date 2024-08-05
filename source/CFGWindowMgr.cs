using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGWindowMgr : CFGSingleton<CFGWindowMgr>
{
	public delegate void OnInitFinishDelegate();

	public delegate void OnUILoadedDelegate();

	public delegate void OnResolutionChangeDelegate(Vector2 new_resolution, Vector2 old_resolution);

	public EventSystem m_EventSystem;

	public GameObject m_UICanvas;

	public Camera m_SubtitleCamera;

	public CFGCharacterScreenNew m_CharacterScreen;

	public CFGMainMenu m_MainMenu;

	public CFGScenarioMenuNew m_ScenarioMenu;

	public CFGScenarioDifficulyPanel m_ScenarioDifficulty;

	public CFGGamePlusPanel m_GamePlus;

	public CFGLoadingScreen m_LoadingScreen;

	public CFGInGameMenu m_InGameMenu;

	public CFGMissionEnd m_MissionEnd;

	public CFGBarterScreenNew m_BarterScreen;

	public CFGStrategicExplorator m_StrategicExplorator;

	public CFGGamepadStrategicPanel m_GamepadStrategicPanel;

	public CFGStrategicExploratorButtons m_StrategicExploratorButtons;

	public CFGObjectivesPanel m_ObjectivePanel;

	public CFGDialogPanel m_DialogPanel;

	public CFGHUD m_HUD;

	public CFGHUDEnemyPanel m_HUDEnemyPanel;

	public CFGHUDAbilitiesPanel m_HUDAbilities;

	public CFGTermsOfShootings m_TermsOfShootings;

	public CFGSubtitlesMovie m_Subtitles;

	public CFGAbilityInfoPanel m_AbilityInfo;

	public CFGAbilityInfoPanel m_AbilityInfo2;

	public CFGSetupStagePanel m_SetupStagePanel;

	public CFGActionButtonsPanel m_ActionButtonsPanel;

	public CFGCardsPanel m_CardsPanel;

	public CFGTacticalCharacterDetailsPanel m_TacticalCharacterDetails;

	public CFGCassandraSpecialPower m_CassandraSP;

	public CFGCustomUIS5 m_CustomS5;

	public CFGCustomUIS1 m_CustomS1;

	public CFGCustomUIS2 m_CustomS2;

	public CFGCustomUIS4 m_CustomS4;

	public CFGCustomUIS6 m_CustomS6;

	public CFGCustomUIS7 m_CustomS7;

	public CFGCustomUIS9 m_CustomS9;

	public CFGOptionsPanel m_Options;

	public CFGImageExtension m_CustomS3;

	public CFGCreditsPanel m_CreditsPanel;

	public List<GameObject> m_StrategicEventsPopups = new List<GameObject>();

	public bool m_EventPopupsNeedRefresh = true;

	public float m_PosEventPopup = 40950f / (float)Screen.height;

	public List<EventPopupInfo> m_PopupInfo = new List<EventPopupInfo>();

	public CFGTutorialPopup m_TutorialPopup;

	public CFGTutorialPanel m_TutorialPanel;

	public GameObject m_TutorialMarker;

	public GameObject m_TimeMarker;

	public CFGTutorialMiniPopup m_TutorialMiniPopup;

	public CFGHUDSave m_HudSave;

	public OnResolutionChangeDelegate m_OnResolutionChangeCallback;

	private Dictionary<CFGSplash, Vector3> m_Splashes = new Dictionary<CFGSplash, Vector3>();

	private Vector2 m_LastResolution;

	private bool m_LastFullScreen;

	private bool m_bInitialized;

	private float m_LastPopupTime = -1f;

	private bool m_FullStackOfPopupsTime;

	public bool m_ShouldUpdateRaycasts;

	private GameObject m_FadeCanvas;

	[HideInInspector]
	public static List<string> m_CardsForPopup = new List<string>();

	private float m_LastCardPopupTime = -1f;

	[HideInInspector]
	public static List<string> m_ItemsForPopup = new List<string>();

	public static int m_LastItem = 0;

	private float m_LastItemsPopupTime = -1f;

	private Dictionary<EWindowID, CFGWindow> m_Windows = new Dictionary<EWindowID, CFGWindow>();

	private bool m_IsCursorOverUI;

	public override void Init()
	{
		if (m_bInitialized)
		{
			return;
		}
		base.Init();
		RegisterWindow(base.gameObject.AddComponent<CFGDevInGameWindow>());
		RegisterWindow(base.gameObject.AddComponent<CFGError>());
		RegisterWindow(base.gameObject.AddComponent<CFGDevMenuWindow>());
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_UICanvasPrefab && m_UICanvas == null)
		{
			m_UICanvas = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_UICanvasPrefab);
			Transform transform = m_UICanvas.transform.FindChild("fade");
			if (transform != null)
			{
				m_FadeCanvas = transform.gameObject;
			}
			Object.DontDestroyOnLoad(m_UICanvas.transform);
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_EventSystemPrefab && m_EventSystem == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_EventSystemPrefab);
			m_EventSystem = gameObject.GetComponent<EventSystem>();
			Object.DontDestroyOnLoad(gameObject.transform);
		}
		m_bInitialized = true;
	}

	public void LoadTutorialMarker(ETutorialMarkerPlaces place)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialMarker && (bool)m_UICanvas && m_TutorialMarker == null)
		{
			GameObject tutorialMarker = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialMarker);
			m_TutorialMarker = tutorialMarker;
			m_TutorialMarker.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
		int num = -1;
		if (m_HUDAbilities != null)
		{
			for (int i = 0; i < m_HUDAbilities.m_SkillButtons.Count; i++)
			{
				CFGButtonExtension cFGButtonExtension = m_HUDAbilities.m_SkillButtons[i];
				if (cFGButtonExtension.m_Data == 1 && place == ETutorialMarkerPlaces.ShootButton)
				{
					num = i;
				}
				else if (cFGButtonExtension.m_Data == 4 && place == ETutorialMarkerPlaces.ReloadButton)
				{
					num = i;
				}
				else if (cFGButtonExtension.m_Data == 16 && place == ETutorialMarkerPlaces.FanningButton)
				{
					num = i;
				}
			}
		}
		if (!(m_TutorialMarker != null))
		{
			return;
		}
		switch (place)
		{
		case ETutorialMarkerPlaces.ReloadButton:
		case ETutorialMarkerPlaces.ShootButton:
		case ETutorialMarkerPlaces.FanningButton:
			if (num >= 0)
			{
				m_TutorialMarker.gameObject.transform.position = m_HUDAbilities.m_SkillButtons[num].gameObject.transform.position;
			}
			break;
		case ETutorialMarkerPlaces.ConfirmAbilityButton:
		{
			Vector3 vector = Vector3.zero;
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad && m_TermsOfShootings != null)
			{
				vector = m_TermsOfShootings.m_AButtonPad.transform.position;
			}
			else if (CFGInput.LastReadInputDevice == EInputMode.KeyboardAndMouse && m_TermsOfShootings != null)
			{
				vector = m_TermsOfShootings.m_ConfirmButton.transform.position;
			}
			if (vector != Vector3.zero)
			{
				m_TutorialMarker.gameObject.transform.position = vector;
			}
			break;
		}
		}
	}

	public void UnloadTutorialMarker()
	{
		if (m_TutorialMarker != null)
		{
			Object.Destroy(m_TutorialMarker.gameObject);
		}
	}

	public void LoadTimeMarker(int icon_number)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TimeMarker && (bool)m_UICanvas && m_TimeMarker == null)
		{
			GameObject timeMarker = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TimeMarker);
			m_TimeMarker = timeMarker;
			m_TimeMarker.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
		}
		if (m_TimeMarker != null)
		{
			CFGImageExtension componentInChildren = m_TimeMarker.GetComponentInChildren<CFGImageExtension>();
			CFGButtonExtension componentInChildren2 = m_TimeMarker.GetComponentInChildren<CFGButtonExtension>();
			if (componentInChildren != null)
			{
				componentInChildren.IconNumber = icon_number;
			}
			if (componentInChildren2 != null)
			{
				componentInChildren2.m_TooltipText = icon_number switch
				{
					0 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_day"), 
					1 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_night"), 
					_ => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_nightmare"), 
				};
			}
		}
	}

	public void UnloadTimeMarker()
	{
		if (m_TimeMarker != null)
		{
			Object.Destroy(m_TimeMarker.gameObject);
		}
	}

	public void LoadMainMenus()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_MainMenuPrefab && (bool)m_UICanvas && m_MainMenu == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_MainMenuPrefab);
			m_MainMenu = gameObject.GetComponent<CFGMainMenu>();
			m_MainMenu.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			m_PopupInfo.Clear();
		}
	}

	public void UnloadMainMenus(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_MainMenu != null)
		{
			m_MainMenu.Unload(callback);
			m_MainMenu = null;
		}
	}

	public void LoadScenarioMenu()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ScenarioMenuPrefab != null && (bool)m_UICanvas && m_ScenarioMenu == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ScenarioMenuPrefab);
			m_ScenarioMenu = gameObject.GetComponent<CFGScenarioMenuNew>();
			m_ScenarioMenu.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			m_ScenarioMenu.SetCampaignID(1);
		}
	}

	public void UnloadScenarioMenu(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_ScenarioMenu != null)
		{
			m_ScenarioMenu.Unload(callback);
			m_ScenarioMenu = null;
		}
	}

	public void LoadScenarioDifficulty()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ScenarioDifficultyPrefab != null && (bool)m_UICanvas && m_ScenarioDifficulty == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ScenarioDifficultyPrefab);
			m_ScenarioDifficulty = gameObject.GetComponent<CFGScenarioDifficulyPanel>();
			m_ScenarioDifficulty.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadScenarioDifficulty(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_ScenarioDifficulty != null)
		{
			Object.Destroy(m_ScenarioDifficulty.gameObject);
		}
	}

	public void LoadGamePlusPanel()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_GamePlusPrefab != null && (bool)m_UICanvas && m_GamePlus == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_GamePlusPrefab);
			m_GamePlus = gameObject.GetComponent<CFGGamePlusPanel>();
			m_GamePlus.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadGamePlusPanel(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_GamePlus != null)
		{
			Object.Destroy(m_GamePlus.gameObject);
		}
	}

	public void LoadObjectivePanel()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ObjectivePanelPrefab && (bool)m_UICanvas && m_ObjectivePanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ObjectivePanelPrefab);
			m_ObjectivePanel = gameObject.GetComponent<CFGObjectivesPanel>();
			m_ObjectivePanel.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
			CFGSingletonResourcePrefab<CFGObjectiveSystem>.Instance.OnObjectivesChanged();
		}
	}

	public void UnloadObjectivePanel()
	{
		if (m_ObjectivePanel != null)
		{
			m_ObjectivePanel.Unload();
			m_ObjectivePanel = null;
		}
	}

	public void LoadCreditsPanel()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CreditsPanelPrefab && (bool)m_UICanvas && m_CreditsPanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CreditsPanelPrefab);
			m_CreditsPanel = gameObject.GetComponent<CFGCreditsPanel>();
			m_CreditsPanel.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadCreditsPanel(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_CreditsPanel != null)
		{
			m_CreditsPanel.Unload(callback);
			m_CreditsPanel = null;
		}
	}

	public void LoadTacticalCharacterDetails()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TacticalDetailsPanelPrefab && (bool)m_UICanvas && m_TacticalCharacterDetails == null && m_InGameMenu == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TacticalDetailsPanelPrefab);
			m_TacticalCharacterDetails = gameObject.GetComponent<CFGTacticalCharacterDetailsPanel>();
			m_TacticalCharacterDetails.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadTacticalCharacterDetails()
	{
		if (m_TacticalCharacterDetails != null)
		{
			m_TacticalCharacterDetails.Unload();
			m_TacticalCharacterDetails = null;
		}
	}

	public void LoadCardsPanel(bool combat_loadout, CFGSequencer Sequencer)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsPanelPrefab && (bool)m_UICanvas && m_CardsPanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsPanelPrefab);
			m_CardsPanel = gameObject.GetComponent<CFGCardsPanel>();
			m_CardsPanel.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			m_CardsPanel.m_CombatLoadout = combat_loadout;
			m_CardsPanel.m_Sequencer = Sequencer;
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadCardsPanell()
	{
		if (m_CardsPanel != null)
		{
			m_CardsPanel.Unload();
			m_CardsPanel = null;
		}
	}

	public CFGTooltip LoadTooltip()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TooltipPrefab && (bool)m_UICanvas)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TooltipPrefab);
			CFGTooltip component = gameObject.GetComponent<CFGTooltip>();
			gameObject.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
			gameObject.transform.position = Input.mousePosition;
			return component;
		}
		return null;
	}

	public void LoadDialogPanel()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_DialogPanelPrefab && (bool)m_UICanvas && m_DialogPanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_DialogPanelPrefab);
			m_DialogPanel = gameObject.GetComponent<CFGDialogPanel>();
			m_DialogPanel.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_StrategicExplorator != null)
			{
				m_StrategicExplorator.transform.SetParent(null, worldPositionStays: false);
				m_StrategicExplorator.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadDialogPanel()
	{
		if (m_DialogPanel != null)
		{
			Object.Destroy(m_DialogPanel.gameObject);
		}
	}

	public void LoadLoadingScreen()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_LoadingScreenPrefab && (bool)m_UICanvas && m_LoadingScreen == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_LoadingScreenPrefab);
			m_LoadingScreen = gameObject.GetComponent<CFGLoadingScreen>();
			m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadLoadingScreen(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_LoadingScreen != null)
		{
			m_LoadingScreen.Unload(callback);
			m_LoadingScreen = null;
		}
	}

	public void LoadCharacterScreen(bool combat_loadout, CFGSequencer Sequencer)
	{
		if (CFGCharacterList.GetTeamCharactersList().Count > 0 && (bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterScreenPrefab && (bool)m_UICanvas && m_CharacterScreen == null && m_CardsPanel == null && m_InGameMenu == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterScreenPrefab);
			m_CharacterScreen = gameObject.GetComponent<CFGCharacterScreenNew>();
			m_CharacterScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			m_CharacterScreen.m_CombatLoadout = combat_loadout;
			m_CharacterScreen.m_Sequencer = Sequencer;
		}
	}

	public void UnloadCharacterScreen()
	{
		if (m_CharacterScreen != null)
		{
			m_CharacterScreen.Unload();
			m_CharacterScreen = null;
		}
	}

	public void LoadCassandraSP()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CassandraSP && (bool)m_UICanvas && m_CassandraSP == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CassandraSP);
			m_CassandraSP = gameObject.GetComponent<CFGCassandraSpecialPower>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadSP()
	{
		if (m_CassandraSP != null)
		{
			Object.Destroy(m_CassandraSP.gameObject);
		}
	}

	public void LoadCustomS5()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS5 && (bool)m_UICanvas && m_CustomS5 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS5);
			m_CustomS5 = gameObject.GetComponent<CFGCustomUIS5>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS5()
	{
		if (m_CustomS5 != null)
		{
			Object.Destroy(m_CustomS5.gameObject);
		}
	}

	public void LoadCustomS3()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS3 && (bool)m_UICanvas && m_CustomS3 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS3);
			float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			}
			m_CustomS3 = gameObject.GetComponentInChildren<CFGImageExtension>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			gameObject.transform.position = new Vector3(0f, 0f);
		}
	}

	public void UnloadCustomS3()
	{
		if (m_CustomS3 != null)
		{
			Object.Destroy(m_CustomS3.transform.parent.gameObject);
		}
	}

	public void LoadCustomS4()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS4 && (bool)m_UICanvas && m_CustomS4 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS4);
			m_CustomS4 = gameObject.GetComponentInChildren<CFGCustomUIS4>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS4()
	{
		if (m_CustomS4 != null)
		{
			Object.Destroy(m_CustomS4.gameObject);
		}
	}

	public void LoadCustomS1()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS1 && (bool)m_UICanvas && m_CustomS1 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS1);
			m_CustomS1 = gameObject.GetComponent<CFGCustomUIS1>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS1()
	{
		if (m_CustomS1 != null)
		{
			Object.Destroy(m_CustomS1.gameObject);
		}
	}

	public void LoadCustomS2()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS2 && (bool)m_UICanvas && m_CustomS2 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS2);
			m_CustomS2 = gameObject.GetComponent<CFGCustomUIS2>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS2()
	{
		if (m_CustomS2 != null)
		{
			Object.Destroy(m_CustomS2.gameObject);
		}
	}

	public void LoadCustomS6()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS6 && (bool)m_UICanvas && m_CustomS6 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS6);
			m_CustomS6 = gameObject.GetComponent<CFGCustomUIS6>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS6()
	{
		if (m_CustomS6 != null)
		{
			Object.Destroy(m_CustomS6.gameObject);
		}
	}

	public void LoadCustomS7()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS7 && (bool)m_UICanvas && m_CustomS7 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS7);
			m_CustomS7 = gameObject.GetComponent<CFGCustomUIS7>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadCustomS7()
	{
		if (m_CustomS7 != null)
		{
			Object.Destroy(m_CustomS7.gameObject);
		}
	}

	public void LoadCustomS9()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS9 && (bool)m_UICanvas && m_CustomS9 == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CustomS9);
			m_CustomS9 = gameObject.GetComponent<CFGCustomUIS9>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UpdateCustomS9()
	{
		if (!(m_CustomS9 == null))
		{
			m_CustomS9.UpdatePanel();
		}
	}

	public void UnloadCustomS9()
	{
		if (m_CustomS9 != null)
		{
			Object.Destroy(m_CustomS9.gameObject);
		}
	}

	public void LoadOptions()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_Options && (bool)m_UICanvas && m_Options == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_Options);
			m_Options = gameObject.GetComponent<CFGOptionsPanel>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadOptions(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		m_OnResolutionChangeCallback = null;
		if (m_Options != null)
		{
			m_Options.Unload(callback);
			m_Options = null;
		}
	}

	public void LoadTutorialMini()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialMiniPopup && (bool)m_UICanvas && m_TutorialMiniPopup == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialMiniPopup);
			m_TutorialMiniPopup = gameObject.GetComponent<CFGTutorialMiniPopup>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadTutorialMini()
	{
		if (m_TutorialMiniPopup != null)
		{
			m_TutorialMiniPopup.Unload();
			m_TutorialMiniPopup = null;
		}
	}

	public void LoadHudSave()
	{
		UnloadMainMenus();
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDSave && (bool)m_UICanvas && m_HudSave == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDSave);
			m_HudSave = gameObject.GetComponent<CFGHUDSave>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadHudSave()
	{
		if (m_HudSave != null)
		{
			m_HudSave.Unload();
			m_HudSave = null;
		}
	}

	public void LoadTutorialPanel(string text, bool is_with_button, out bool is_ready)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialPanel && (bool)m_UICanvas && m_TutorialPanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialPanel);
			m_TutorialPanel = gameObject.GetComponent<CFGTutorialPanel>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
		if (m_TutorialPanel != null)
		{
			m_TutorialPanel.m_Text.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text);
			m_TutorialPanel.m_IsWithButton = is_with_button;
			m_TutorialPanel.m_CG.alpha = 0f;
		}
		is_ready = false;
	}

	public void UnloadTutorialPanel()
	{
		if (m_TutorialPanel != null)
		{
			m_TutorialPanel.Unload();
			m_TutorialPanel = null;
		}
	}

	public bool IsAnyTutorialActive()
	{
		if (m_TutorialPanel != null && m_TutorialPanel.m_ButtonOk != null && m_TutorialPanel.m_ButtonOk.isActiveAndEnabled)
		{
			return true;
		}
		if (m_TutorialPopup != null)
		{
			return true;
		}
		if (m_TutorialMiniPopup != null)
		{
			return true;
		}
		return false;
	}

	public void LoadTutorialPopup(string title, string text, int image_nr, out bool ready)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialPopup && (bool)m_UICanvas && m_TutorialPopup == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TutorialPopup);
			m_TutorialPopup = gameObject.GetComponent<CFGTutorialPopup>();
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_TutorialPopup != null)
			{
				m_TutorialPopup.m_TitleText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(title);
				m_TutorialPopup.m_DescriptionText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text);
				m_TutorialPopup.m_Image.IconNumber = image_nr;
			}
		}
		ready = false;
	}

	public void UnloadTutorialPopup()
	{
		if (m_TutorialPopup != null)
		{
			m_TutorialPopup.Unload();
			m_TutorialPopup = null;
		}
	}

	public void LoadStrategicEventPopup(string text, int type, int ico)
	{
		m_PopupInfo.Add(new EventPopupInfo(text, type, ico));
	}

	public void StrategicEventPopup(string text, int type, int ico)
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicEventPopup && (bool)m_UICanvas)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicEventPopup);
			CFGEventStrategicPopup componentInChildren = gameObject.GetComponentInChildren<CFGEventStrategicPopup>();
			int num = m_StrategicEventsPopups.Count;
			List<int> list = new List<int>();
			list.Add(0);
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);
			list.Add(6);
			for (int i = 0; i < m_StrategicEventsPopups.Count; i++)
			{
				CFGEventStrategicPopup componentInChildren2 = m_StrategicEventsPopups[i].GetComponentInChildren<CFGEventStrategicPopup>();
				list.Remove(componentInChildren2.m_Number);
			}
			if (list.Count > 0)
			{
				num = list[0];
			}
			componentInChildren.m_Number = num;
			float y = 45.5f * (float)Screen.height / 900f + 90f * (float)Screen.height / 900f * (float)num;
			m_StrategicEventsPopups.Add(gameObject);
			gameObject.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, y, gameObject.transform.position.z);
			componentInChildren.m_EventText.text = text;
			CFGImageExtension[] componentsInChildren = gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
			{
				cFGImageExtension.IconNumber = ico;
			}
			componentInChildren.m_BuffImg.IconNumber = ico;
			componentInChildren.m_CharacterImg.IconNumber = ico;
			componentInChildren.m_IconExploratorImg.IconNumber = ico;
			componentInChildren.m_ItemImg.IconNumber = ico;
			componentInChildren.m_IconCashPlace.IconNumber = ico;
			componentInChildren.m_CardsIcon.IconNumber = ico;
			componentInChildren.m_BuffImg.gameObject.SetActive(type == 0);
			componentInChildren.m_CharacterImg.gameObject.transform.parent.gameObject.SetActive(type == 1);
			componentInChildren.m_IconExploratorImg.gameObject.SetActive(type == 2);
			componentInChildren.m_ItemImg.gameObject.SetActive(type == 3);
			componentInChildren.m_IconCashPlace.gameObject.SetActive(type == 4);
			componentInChildren.m_CardsIcon.gameObject.SetActive(type == 6);
			m_EventPopupsNeedRefresh = true;
		}
	}

	public void UnloadStrategicEventPopup(CFGEventStrategicPopup event_popup)
	{
		if (m_StrategicEventsPopups != null)
		{
			m_StrategicEventsPopups.Remove(event_popup.gameObject.transform.parent.gameObject);
		}
		if (event_popup != null)
		{
			Object.Destroy(event_popup.gameObject.transform.parent.gameObject);
		}
	}

	public void LoadStrategicExplorator(OnUILoadedDelegate callback)
	{
		UnloadMainMenus();
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorButtonsPrefab && (bool)m_UICanvas && m_StrategicExploratorButtons == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorButtonsPrefab);
			m_StrategicExploratorButtons = gameObject.GetComponent<CFGStrategicExploratorButtons>();
			m_StrategicExploratorButtons.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorPrefab && (bool)m_UICanvas && m_StrategicExplorator == null)
		{
			GameObject gameObject2 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorPrefab);
			m_StrategicExplorator = gameObject2.GetComponent<CFGStrategicExplorator>();
			m_StrategicExplorator.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
			callback?.Invoke();
			LoadObjectivePanel();
			CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_08")
			{
				LoadCassandraSP();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_05")
			{
				LoadCustomS5();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_01")
			{
				LoadCustomS1();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_02")
			{
				LoadCustomS2();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_03")
			{
				LoadCustomS3();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_04")
			{
				LoadCustomS4();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_06")
			{
				LoadCustomS6();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_07")
			{
				LoadCustomS7();
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && (bool)sessionSingle && sessionSingle.ScenarioName == "scen_09")
			{
				LoadCustomS9();
			}
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_GamepadStrategicPanel && (bool)m_UICanvas && m_GamepadStrategicPanel == null)
		{
			GameObject gameObject3 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_GamepadStrategicPanel);
			m_GamepadStrategicPanel = gameObject3.GetComponent<CFGGamepadStrategicPanel>();
			m_GamepadStrategicPanel.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
		LoadHudSave();
	}

	public void UnloadStrategicExplorator()
	{
		UnloadObjectivePanel();
		UnloadDialogPanel();
		UnloadCharacterScreen();
		UnloadBarterScreen();
		UnloadCardsPanell();
		UnloadSP();
		UnloadCustomS5();
		UnloadCustomS1();
		UnloadCustomS2();
		UnloadCustomS3();
		UnloadCustomS4();
		UnloadTutorialPopup();
		UnloadTutorialPanel();
		UnloadTutorialMarker();
		UnloadTimeMarker();
		UnloadCustomS6();
		UnloadCustomS7();
		UnloadCustomS9();
		UnloadOptions();
		UnloadHudSave();
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			m_PopupInfo.Clear();
		}
		List<GameObject> list = new List<GameObject>(m_StrategicEventsPopups);
		foreach (GameObject item in list)
		{
			UnloadStrategicEventPopup(item.GetComponentInChildren<CFGEventStrategicPopup>());
		}
		if (m_StrategicExplorator != null)
		{
			Object.Destroy(m_StrategicExplorator.gameObject);
		}
		if (m_StrategicExploratorButtons != null)
		{
			Object.Destroy(m_StrategicExploratorButtons.gameObject);
		}
		if (m_GamepadStrategicPanel != null)
		{
			Object.Destroy(m_GamepadStrategicPanel.gameObject);
		}
	}

	public void LoadBarterScreen()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BarterScreenPrefab && (bool)m_UICanvas && m_BarterScreen == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BarterScreenPrefab);
			m_BarterScreen = gameObject.GetComponent<CFGBarterScreenNew>();
			m_BarterScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_StrategicExplorator != null)
			{
				m_StrategicExplorator.HideExplorationWindow();
			}
		}
	}

	public void UnloadBarterScreen()
	{
		if (m_BarterScreen != null)
		{
			m_BarterScreen.Unload();
			m_BarterScreen = null;
		}
	}

	public void LoadTermsOfShooting()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TermsOfShootingPrefab && (bool)m_UICanvas && m_TermsOfShootings == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_TermsOfShootingPrefab);
			m_TermsOfShootings = gameObject.GetComponent<CFGTermsOfShootings>();
			m_TermsOfShootings.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadTermsOfShooting()
	{
		if (m_HUD != null)
		{
			Object.Destroy(m_TermsOfShootings.gameObject);
		}
	}

	public void LoadSetupStagePanel()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_SetupStagePanelPrefab && (bool)m_UICanvas && m_SetupStagePanel == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_SetupStagePanelPrefab);
			m_SetupStagePanel = gameObject.GetComponent<CFGSetupStagePanel>();
			m_SetupStagePanel.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadSetupStagePanel()
	{
		if (m_SetupStagePanel != null)
		{
			Object.Destroy(m_SetupStagePanel.gameObject);
		}
	}

	public void LoadHUD()
	{
		UnloadMainMenus();
		LoadObjectivePanel();
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDPrefab && (bool)m_UICanvas && m_HUD == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDPrefab);
			m_HUD = gameObject.GetComponent<CFGHUD>();
			m_HUD.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDEnemyPanelPrefab && (bool)m_UICanvas && m_HUDEnemyPanel == null)
		{
			GameObject gameObject2 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDEnemyPanelPrefab);
			m_HUDEnemyPanel = gameObject2.GetComponent<CFGHUDEnemyPanel>();
			m_HUDEnemyPanel.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDAbilitiesPrefab && (bool)m_UICanvas && m_HUDAbilities == null)
		{
			GameObject gameObject3 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDAbilitiesPrefab);
			m_HUDAbilities = gameObject3.GetComponent<CFGHUDAbilitiesPanel>();
			m_HUDAbilities.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
		LoadHudSave();
		LoadTermsOfShooting();
		LoadSetupStagePanel();
		LoadTimeMarker(0);
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDActions && (bool)m_UICanvas && m_ActionButtonsPanel == null)
		{
			GameObject gameObject4 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_HUDActions);
			m_ActionButtonsPanel = gameObject4.GetComponent<CFGActionButtonsPanel>();
			m_ActionButtonsPanel.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorButtonsPrefab && (bool)m_UICanvas && m_StrategicExploratorButtons == null)
		{
			GameObject gameObject5 = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorButtonsPrefab);
			m_StrategicExploratorButtons = gameObject5.GetComponent<CFGStrategicExploratorButtons>();
			m_StrategicExploratorButtons.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_LoadingScreen != null)
			{
				m_LoadingScreen.transform.SetParent(null, worldPositionStays: false);
				m_LoadingScreen.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			}
		}
	}

	public void UnloadHUD()
	{
		UnloadObjectivePanel();
		UnloadDialogPanel();
		UnloadTermsOfShooting();
		UnloadSetupStagePanel();
		UnloadTutorialPopup();
		UnloadTutorialPanel();
		UnloadTutorialMarker();
		UnloadTimeMarker();
		UnloadTutorialMini();
		UnloadOptions();
		UnloadHudSave();
		DestroyAllSplashes();
		if (m_HUD != null)
		{
			Object.Destroy(m_HUD.gameObject);
		}
		if (m_HUDEnemyPanel != null)
		{
			Object.Destroy(m_HUDEnemyPanel.gameObject);
		}
		if (m_HUDAbilities != null)
		{
			Object.Destroy(m_HUDAbilities.gameObject);
		}
		if (m_StrategicExploratorButtons != null)
		{
			Object.Destroy(m_StrategicExploratorButtons.gameObject);
		}
		if (m_ActionButtonsPanel != null)
		{
			Object.Destroy(m_ActionButtonsPanel.gameObject);
		}
	}

	public void ShowHUD()
	{
		if (m_HUD != null)
		{
			m_HUD.SetPanelVisible(visible: true);
		}
	}

	public void HideHUD()
	{
		if (m_HUD != null)
		{
			m_HUD.SetPanelVisible(visible: false);
		}
	}

	public void ShowUI()
	{
		if (m_UICanvas != null)
		{
			m_UICanvas.SetActive(value: true);
		}
	}

	public void HideUI()
	{
		if (m_UICanvas != null)
		{
			m_UICanvas.SetActive(value: false);
		}
	}

	public bool IsUIVisible()
	{
		return m_UICanvas != null && m_UICanvas.activeSelf;
	}

	public void LoadInGameMenu()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_InGameMenuPrefab && (bool)m_UICanvas && m_InGameMenu == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_InGameMenuPrefab);
			m_InGameMenu = gameObject.GetComponent<CFGInGameMenu>();
			m_InGameMenu.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
			if (m_SetupStagePanel != null)
			{
				m_SetupStagePanel.transform.SetParent(null);
				m_SetupStagePanel.transform.SetParent(m_FadeCanvas.transform);
			}
		}
	}

	public void UnloadInGameMenu(CFGPanel.OnPanelUnloadedDelegate callback = null)
	{
		if (m_InGameMenu != null)
		{
			m_InGameMenu.Unload(callback);
			m_InGameMenu = null;
		}
	}

	public void LoadMissionEnd()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_MissionEndPrefab && (bool)m_UICanvas && m_MissionEnd == null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_MissionEndPrefab);
			m_MissionEnd = gameObject.GetComponent<CFGMissionEnd>();
			m_MissionEnd.transform.SetParent(m_UICanvas.transform, worldPositionStays: false);
		}
	}

	public void UnloadMissionEnd()
	{
		if (m_MissionEnd != null)
		{
			m_MissionEnd.Unload();
			m_MissionEnd = null;
		}
	}

	public void LoadSubtitles()
	{
		if (!CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_SubtitlesPrefab || !(m_Subtitles == null))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_SubtitlesPrefab);
		m_Subtitles = gameObject.GetComponent<CFGSubtitlesMovie>();
		Canvas component = gameObject.GetComponent<Canvas>();
		if (m_SubtitleCamera == null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SubtitleCamera != null)
		{
			m_SubtitleCamera = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SubtitleCamera);
			if (component != null)
			{
				component.worldCamera = m_SubtitleCamera;
			}
			CFGSubtitleDisplayer component2 = m_SubtitleCamera.GetComponent<CFGSubtitleDisplayer>();
			if ((bool)component2)
			{
				component2.StartDisplaying();
			}
		}
	}

	public void UnloadSubtitles()
	{
		if (m_SubtitleCamera != null)
		{
			CFGSubtitleDisplayer component = m_SubtitleCamera.GetComponent<CFGSubtitleDisplayer>();
			if ((bool)component)
			{
				component.StopDisplaying();
			}
			Object.Destroy(m_SubtitleCamera.gameObject);
			m_SubtitleCamera = null;
		}
		if (m_Subtitles != null)
		{
			Object.Destroy(m_Subtitles.gameObject);
		}
	}

	public GameObject SpawnCharacterFlag()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterFlagPrefab != null && m_UICanvas != null)
		{
			GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterFlagPrefab);
			gameObject.transform.SetParent(m_FadeCanvas.transform);
			return gameObject;
		}
		return null;
	}

	public void ReparentMainPanels()
	{
		if (m_HUD != null)
		{
			m_HUD.transform.SetParent(null);
			m_HUD.transform.SetParent(m_FadeCanvas.transform);
		}
		if (m_ObjectivePanel != null)
		{
			m_ObjectivePanel.transform.SetParent(null);
			m_ObjectivePanel.transform.SetParent(m_UICanvas.transform);
		}
		if (m_DialogPanel != null)
		{
			m_DialogPanel.transform.SetParent(null);
			m_DialogPanel.transform.SetParent(m_UICanvas.transform);
		}
		if (m_TermsOfShootings != null)
		{
			m_TermsOfShootings.transform.SetParent(null);
			m_TermsOfShootings.transform.SetParent(m_UICanvas.transform);
		}
		if (m_SetupStagePanel != null)
		{
			m_SetupStagePanel.transform.SetParent(null);
			m_SetupStagePanel.transform.SetParent(m_FadeCanvas.transform);
		}
		if (m_LoadingScreen != null)
		{
			m_LoadingScreen.transform.SetParent(null);
			m_LoadingScreen.transform.SetParent(m_UICanvas.transform);
		}
	}

	public CFGSplash SpawnSplash(Vector3 position, string val, bool plus, int type, int icon, CFGCharacter character)
	{
		if (m_UICanvas == null || !CFGOptions.Gameplay.ShowFloatingTexts)
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_SplashPrefab);
		gameObject.transform.SetParent(m_FadeCanvas.transform, worldPositionStays: false);
		CFGSplash component = gameObject.GetComponent<CFGSplash>();
		m_Splashes.Add(component, position);
		SetSplashPosition(position, component);
		gameObject.SetActive(value: false);
		int num = 0;
		foreach (CFGSplash key in m_Splashes.Keys)
		{
			if (key.m_Char == character && key.m_SplashMain.color.a > 0.72f)
			{
				num++;
			}
		}
		if (type == 0 && icon == 0)
		{
			component.m_Text.color = Color.white;
			component.m_TextCenter.color = Color.white;
		}
		component.m_BuffRecivedImage.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
		component.m_Text.text = val;
		component.m_TextCenter.text = val;
		component.m_BuffRecivedText.text = val;
		if (icon > 0)
		{
			component.m_BuffRecivedImage.IconNumber = icon;
		}
		if (type == 0)
		{
			if (icon == 0)
			{
				component.m_Splash.IconNumber = ((!plus) ? 1 : 0);
			}
			if (icon == 2)
			{
				component.m_Splash.IconNumber = ((!plus) ? 3 : 2);
			}
		}
		if (type == 0 && icon == 0)
		{
			component.m_SplashMain.IconNumber = Random.Range(0, 3);
		}
		else
		{
			component.m_SplashMain.IconNumber = 3;
		}
		component.m_Text.gameObject.SetActive(value: false);
		component.m_TextCenter.gameObject.SetActive(type < 1);
		component.m_BuffRecivedText.gameObject.SetActive(type >= 1);
		component.m_BuffRecivedImage.gameObject.SetActive(type >= 1 && icon > 0);
		component.m_Splash.gameObject.SetActive(type < 1);
		component.m_SplashMain.gameObject.SetActive(type < 1);
		component.m_Char = character;
		component.m_Number = num;
		if (type == 0 && icon == 0 && plus)
		{
			component.m_Reversed = true;
			gameObject.transform.position += new Vector3(0f, 10f, 0f);
		}
		return component;
	}

	private void DestroyAllSplashes()
	{
		foreach (CFGSplash key in m_Splashes.Keys)
		{
			Object.Destroy(key.gameObject);
		}
		m_Splashes.Clear();
	}

	public void SetSplashPosition(Vector3 position, CFGSplash splash)
	{
		position = Camera.main.WorldToScreenPoint(position);
		position.x += 10f;
		RectTransform component = m_UICanvas.GetComponent<RectTransform>();
		float width = component.rect.width;
		float height = component.rect.height;
		Image component2 = splash.gameObject.GetComponent<Image>();
		float num = component2.sprite.rect.width * width / 1920f;
		float num2 = component2.sprite.rect.height * height / 1080f;
		float num3 = position.x / width;
		float num4 = position.y / height;
		RectTransform rectTransform = component2.rectTransform;
		rectTransform.offsetMax = new Vector2(1f, 1f);
		rectTransform.offsetMin = new Vector2(0f, 0f);
		splash.gameObject.SetActive(!(position.x > width) && !(position.y > height) && !(position.x < 0f) && !(position.y < 0f));
		if (position.x > width || position.y > height || position.x < 0f || position.y < 0f)
		{
			rectTransform.anchorMin = new Vector2(0f, 0f);
			rectTransform.anchorMax = new Vector2(0f, 0f);
		}
		else
		{
			rectTransform.anchorMin = new Vector2(num3 - num / width / 2f, num4);
			rectTransform.anchorMax = new Vector2(num3 + num / width / 2f, num4 + num2 / height);
		}
	}

	public void SetHUDAlpha(float alpha)
	{
		if (m_FadeCanvas != null)
		{
			CanvasGroup component = m_FadeCanvas.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = alpha;
			}
		}
	}

	public CFGWindow GetWindow(EWindowID id)
	{
		m_Windows.TryGetValue(id, out var value);
		return value;
	}

	public int GetWindowCount()
	{
		return m_Windows.Count;
	}

	public bool ActivateWindow(EWindowID id)
	{
		CFGWindow window = GetWindow(id);
		if (window == null)
		{
			Debug.LogWarning("Trying to activate window that is not registered in WindowMgr (EWindowID == " + id.ToString() + ").");
			return false;
		}
		window.Activate();
		return true;
	}

	public bool DeactivateWindow(EWindowID id)
	{
		CFGWindow window = GetWindow(id);
		if (window == null)
		{
			Debug.LogWarning("Trying to deactivate window that is not registered in WindowMgr (EWindowID == " + id.ToString() + ").");
			return false;
		}
		window.Deactivate();
		return true;
	}

	public void DeactivateAllWindows()
	{
		foreach (CFGWindow value in m_Windows.Values)
		{
			value.Deactivate();
		}
	}

	public bool IsCursorOverUI()
	{
		return m_IsCursorOverUI;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		m_LastResolution = new Vector2(Screen.width, Screen.height);
		m_LastFullScreen = Screen.fullScreen;
	}

	private void Update()
	{
		m_IsCursorOverUI = false;
		if ((bool)m_EventSystem)
		{
			m_IsCursorOverUI = m_EventSystem.IsPointerOverGameObject();
		}
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		if (m_LastResolution != vector || m_LastFullScreen != Screen.fullScreen)
		{
			if (m_OnResolutionChangeCallback != null)
			{
				m_OnResolutionChangeCallback(vector, m_LastResolution);
			}
			m_LastResolution = vector;
			m_LastFullScreen = Screen.fullScreen;
		}
		List<CFGSplash> list = new List<CFGSplash>();
		foreach (CFGSplash key in m_Splashes.Keys)
		{
			if (key.m_Number > 0)
			{
				continue;
			}
			key.gameObject.SetActive(value: true);
			key.m_BuffRecivedImage.color = new Color(key.m_BuffRecivedImage.color.r, key.m_BuffRecivedImage.color.g, key.m_BuffRecivedImage.color.b, key.m_BuffRecivedImage.color.a - key.m_FloatingSpeed);
			key.m_Splash.color = new Color(key.m_Splash.color.r, key.m_Splash.color.g, key.m_Splash.color.b, key.m_Splash.color.a - key.m_FloatingSpeed);
			key.m_SplashMain.color = new Color(key.m_SplashMain.color.r, key.m_SplashMain.color.g, key.m_SplashMain.color.b, key.m_SplashMain.color.a - key.m_FloatingSpeed);
			key.m_BuffRecivedText.color = new Color(key.m_BuffRecivedText.color.r, key.m_BuffRecivedText.color.g, key.m_BuffRecivedText.color.b, key.m_BuffRecivedText.color.a - key.m_FloatingSpeed);
			key.m_Text.color = new Color(key.m_Text.color.r, key.m_Text.color.g, key.m_Text.color.b, key.m_Text.color.a - key.m_FloatingSpeed);
			key.m_TextCenter.color = new Color(key.m_TextCenter.color.r, key.m_TextCenter.color.g, key.m_TextCenter.color.b, key.m_TextCenter.color.a - key.m_FloatingSpeed);
			if (key.m_SplashMain.color.a <= 0f)
			{
				list.Add(key);
			}
			else
			{
				SetSplashPosition(m_Splashes[key], key);
			}
			if (!key.m_Reversed)
			{
				key.transform.Translate(0f, (1f - key.m_SplashMain.color.a) * 100f, 0f);
			}
			else
			{
				key.transform.Translate(0f, key.m_SplashMain.color.a * 100f, 0f);
			}
			if (!(key.m_SplashMain.color.a < 0.71f) || !(key.m_SplashMain.color.a >= 0.7f))
			{
				continue;
			}
			foreach (CFGSplash key2 in m_Splashes.Keys)
			{
				if (key.m_Char == key2.m_Char && key2.m_Number != 0)
				{
					key2.m_Number--;
				}
			}
		}
		foreach (CFGSplash item in list)
		{
			m_Splashes.Remove(item);
			Object.Destroy(item.gameObject);
		}
		if (m_PopupInfo.Count > 0 && CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			if (m_StrategicEventsPopups.Count < 7)
			{
				float num = ((!m_FullStackOfPopupsTime) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_NotificationDelay : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StackDelay);
				if (m_LastPopupTime + num < Time.time)
				{
					StrategicEventPopup(m_PopupInfo[0].text, m_PopupInfo[0].type, m_PopupInfo[0].ico);
					m_PopupInfo.RemoveAt(0);
					m_LastPopupTime = Time.time;
					m_FullStackOfPopupsTime = false;
				}
			}
			else
			{
				m_FullStackOfPopupsTime = true;
			}
		}
		if (m_ShouldUpdateRaycasts)
		{
			List<RaycastResult> list2 = new List<RaycastResult>();
			List<CFGButtonExtension> list3 = new List<CFGButtonExtension>();
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.RaycastAll(pointerEventData, list2);
			}
			foreach (RaycastResult item2 in list2)
			{
				if ((bool)item2.gameObject && (bool)item2.gameObject.GetComponent<CFGButtonExtension>())
				{
					list3.Add(item2.gameObject.GetComponent<CFGButtonExtension>());
				}
			}
			foreach (Selectable allSelectable in Selectable.allSelectables)
			{
				CFGButtonExtension cFGButtonExtension = allSelectable as CFGButtonExtension;
				if ((bool)cFGButtonExtension && cFGButtonExtension.m_IsCursorHoveredOverGameObject)
				{
					cFGButtonExtension.OnPointerExit(null);
				}
			}
			if (list3.Count > 0)
			{
				list3[0].OnPointerEnter(pointerEventData);
			}
			m_ShouldUpdateRaycasts = false;
		}
		if (m_TimeMarker != null)
		{
			CFGImageExtension componentInChildren = m_TimeMarker.GetComponentInChildren<CFGImageExtension>();
			if (componentInChildren != null)
			{
				int iconNumber = componentInChildren.IconNumber;
				if (CFGGame.Nightmare)
				{
					componentInChildren.IconNumber = 2;
				}
				else if (CFGSingleton<CFGGame>.IsInstanceInitialized() && CFGSingleton<CFGGame>.Instance.IsDarkness)
				{
					componentInChildren.IconNumber = 1;
				}
				else
				{
					componentInChildren.IconNumber = 0;
				}
				CFGButtonExtension componentInChildren2 = m_TimeMarker.GetComponentInChildren<CFGButtonExtension>();
				if (componentInChildren2 != null && iconNumber != componentInChildren.IconNumber)
				{
					componentInChildren2.m_TooltipText = ((componentInChildren.IconNumber == 0) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_day") : ((componentInChildren.IconNumber != 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_nightmare") : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_lightpanel_night")));
				}
			}
		}
		if (m_LastCardPopupTime + 2f < Time.time)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && m_CardsForPopup.Count > 0)
			{
				string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_card_received") + m_CardsForPopup[0];
				for (int i = 1; i < m_CardsForPopup.Count; i++)
				{
					text = text + ", " + m_CardsForPopup[i];
				}
				CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 6, 0);
			}
			m_CardsForPopup.Clear();
			m_LastCardPopupTime = Time.time;
		}
		if (!(m_LastItemsPopupTime + 2f < Time.time))
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && m_ItemsForPopup.Count > 0)
		{
			string text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_item_received") + m_ItemsForPopup[0];
			for (int j = 1; j < m_ItemsForPopup.Count && j < 4; j++)
			{
				text2 = text2 + ", " + m_ItemsForPopup[j];
			}
			if (m_ItemsForPopup.Count > 3)
			{
				text2 += "...";
			}
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text2, 3, m_LastItem);
		}
		m_ItemsForPopup.Clear();
		m_LastItemsPopupTime = Time.time;
	}

	private void OnDestroy()
	{
		if (m_EventSystem != null)
		{
			Object.Destroy(m_EventSystem.gameObject);
		}
		if (m_UICanvas != null)
		{
			Object.Destroy(m_UICanvas.gameObject);
		}
	}

	private void RegisterWindow(CFGWindow window)
	{
		m_Windows[window.GetWindowID()] = window;
	}

	public void OnInputModeChange(EInputMode NewInputMode)
	{
		Debug.Log("New input mode: " + NewInputMode);
	}
}
