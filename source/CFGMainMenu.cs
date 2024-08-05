using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGMainMenu : CFGPanel
{
	private const int BUTTON_CONTINUE = 0;

	private const int BUTTON_CAMPAIGN = 1;

	private const int BUTTON_DLC1 = 2;

	private const int BUTTON_OPTIONS = 3;

	private const int BUTTON_CREDITS = 4;

	private const int BUTTON_EXIT = 5;

	public Image m_LogoNormal;

	public Image m_LogoDLC;

	public List<CFGButtonExtension> m_MainButtons = new List<CFGButtonExtension>();

	public List<Image> m_EndFrames = new List<Image>();

	public Text m_BuildNr;

	public int m_VisibleButtonsCount = 6;

	private CFGDef_Campaign m_SelectedCampaign;

	private CFGDef_Scenario m_SelectedScenario;

	private string m_SelectedScenarioLastSave;

	private string m_LastSaveGame;

	public GameObject m_PadBtns;

	public CFGButtonExtension m_APad;

	public CFGButtonExtension m_LPad;

	private CFGJoyMenuController m_Controller = new CFGJoyMenuController();

	private float m_NextReadAnalog;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	protected override void Start()
	{
		base.Start();
		m_LastSaveGame = CFG_SG_Manager.GetLastSaveName(bCheckVersions: true);
		CheckField(m_BuildNr, "BuildNr");
		foreach (CFGButtonExtension mainButton in m_MainButtons)
		{
			CheckField(mainButton, "Main button");
		}
		foreach (Image endFrame in m_EndFrames)
		{
			CheckField(endFrame, "End frames");
		}
		if (m_BuildNr != null)
		{
			m_BuildNr.text = "v1.5";
		}
		SetButtons();
		Debug.Log("Main menu loaded");
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_Controller.AddList(m_MainButtons, null, OnControllerButton);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_bCheckVisibleOnly = true;
		}
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_PadBtns.SetActive(active);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		CFGVariableContainer.Instance.ClearScope("campaign");
		CFGVariableContainer.Instance.ClearScope("scenario");
		UpdateLogo();
	}

	private void UpdateLogo()
	{
		if (!(m_LogoNormal == null) && !(m_LogoDLC == null))
		{
			m_LogoNormal.gameObject.SetActive(value: false);
			m_LogoDLC.gameObject.SetActive(value: false);
			if (CFGApplication.IsDLCInstalled(EDLC.DLC1))
			{
				m_LogoDLC.gameObject.SetActive(value: true);
			}
			else
			{
				m_LogoNormal.gameObject.SetActive(value: true);
			}
		}
	}

	private void OnControllerButton(int Item, bool Secondary)
	{
		if (!Secondary)
		{
			m_MainButtons[Item].SimulateClick(bDelayed: true);
		}
	}

	public override void Update()
	{
		base.Update();
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_PadBtns.SetActive(active);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (!(CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioDifficulty != null))
		{
			if (!CFGButtonExtension.IsWaitingForClick)
			{
				m_Controller.UpdateInput();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				m_APad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0.4f && Time.time > m_NextReadAnalog)
			{
				m_LPad.SimulateClickGraphicAndSoundOnly();
				m_NextReadAnalog = Time.time + 0.4f;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0.4f && Time.time > m_NextReadAnalog)
			{
				m_LPad.SimulateClickGraphicAndSoundOnly();
				m_NextReadAnalog = Time.time + 0.4f;
			}
		}
	}

	private bool InitScenario(string CampaingID, string ScenarioID)
	{
		m_SelectedCampaign = null;
		m_SelectedScenario = null;
		m_SelectedScenarioLastSave = null;
		m_SelectedCampaign = CFGStaticDataContainer.GetCampaign(CampaingID);
		if (m_SelectedCampaign == null)
		{
			Debug.LogError("Failed to find campaign [" + CampaingID + "]");
			return false;
		}
		m_SelectedScenario = m_SelectedCampaign.GetScenarioByScenario(ScenarioID);
		if (m_SelectedScenario == null)
		{
			m_SelectedCampaign = null;
			Debug.LogErrorFormat("Failed to find scenario [" + ScenarioID + "] within campaign: [" + CampaingID + "]");
			return false;
		}
		m_SelectedScenarioLastSave = CFG_SG_Manager.GetMostRecentSave(CampaingID, ScenarioID, bCheckVersions: true);
		return true;
	}

	public void SetScenarioButtons(string CampaingID, string ScenarioID)
	{
		if (!InitScenario(CampaingID, ScenarioID))
		{
			SetButtons();
			return;
		}
		m_MainButtons[0].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_dlc1_continue");
		if (!string.IsNullOrEmpty(m_SelectedScenarioLastSave))
		{
			m_MainButtons[1].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_dlc1_restart");
			m_MainButtons[0].m_ButtonClickedCallback = On_DLCButton_Continue;
			m_MainButtons[1].m_ButtonClickedCallback = On_DLCButton_StartNewGame;
		}
		else
		{
			m_MainButtons[1].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_dlc1_newgame");
			m_MainButtons[0].m_ButtonClickedCallback = null;
			m_MainButtons[1].m_ButtonClickedCallback = On_DLCButton_StartNewGame;
			m_MainButtons[0].enabled = false;
			m_MainButtons[0].DisableVisuals();
		}
		m_MainButtons[1].gameObject.SetActive(value: true);
		m_MainButtons[2].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_dlc1_goback");
		m_MainButtons[2].m_ButtonClickedCallback = On_DLCButton_ReturnToMainMenu;
		m_MainButtons[2].gameObject.SetActive(value: true);
		m_MainButtons[1].EnableVisuals();
		m_MainButtons[2].EnableVisuals();
		for (int i = 3; i < m_MainButtons.Count; i++)
		{
			if (m_MainButtons[i] != null)
			{
				m_MainButtons[i].m_ButtonClickedCallback = null;
				m_MainButtons[i].gameObject.SetActive(value: false);
			}
			if (m_EndFrames[i] != null)
			{
				m_EndFrames[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void On_DLCButton_Continue(int id)
	{
		if (string.IsNullOrEmpty(m_SelectedScenarioLastSave))
		{
			Debug.LogWarningFormat("Need save name!");
		}
		else
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus(OnMenuUnloaded_ContinueScenario);
		}
	}

	private void OnMenuUnloaded_ContinueScenario()
	{
		if (m_SelectedCampaign == null || m_SelectedScenario == null)
		{
			Debug.LogWarningFormat("need data to continue!");
		}
		else
		{
			CFG_SG_Manager.ContinueScenario(m_SelectedCampaign.CampaignID, m_SelectedScenario.ScenarioID);
		}
	}

	private void On_DLCButton_StartNewGame(int id)
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadScenarioDifficulty();
		if (CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioDifficulty != null)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioDifficulty.SetOnStartClickCallback(null, m_SelectedCampaign.CampaignID, m_SelectedScenario.ScenarioID);
		}
	}

	private void On_DLCButton_ReturnToMainMenu(int id)
	{
		CFGInput.ClearActions(0.5f);
		CFGJoyManager.ClearJoyActions(0.5f);
		SetButtons();
	}

	public void SetButtons()
	{
		m_MainButtons[0].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_continue");
		m_MainButtons[1].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_campain1");
		m_MainButtons[2].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_dlc");
		m_MainButtons[3].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_options");
		m_MainButtons[4].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_credits");
		m_MainButtons[5].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("mainmenu_quit");
		m_MainButtons[5].m_Data = 5;
		m_APad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_chooseoption");
		m_LPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_movecursor");
		for (int i = 0; i < m_MainButtons.Count; i++)
		{
			if (!(m_MainButtons[i] == null))
			{
				m_MainButtons[i].m_ButtonClickedCallback = OnMenuButtons;
				m_MainButtons[i].gameObject.SetActive(value: true);
			}
		}
		if (m_LastSaveGame == null)
		{
			m_MainButtons[0].enabled = false;
			m_MainButtons[0].DisableVisuals();
		}
		if (!CFGApplication.IsDLCInstalled(EDLC.DLC1))
		{
			m_MainButtons[2].enabled = false;
			m_MainButtons[2].DisableVisuals();
		}
		for (int j = 0; j < m_EndFrames.Count; j++)
		{
			if (m_EndFrames[j] != null)
			{
				m_EndFrames[j].gameObject.SetActive(j == m_VisibleButtonsCount - 1);
			}
		}
	}

	public void OnMenuButtons(int button_id)
	{
		switch (button_id)
		{
		case 0:
			if (m_LastSaveGame != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus();
				CFG_SG_Manager.LoadSaveGameFile(m_LastSaveGame);
			}
			break;
		case 2:
			SetScenarioButtons("camp_02", "scen_09");
			break;
		case 1:
			OnButtonPressedCampain();
			break;
		case 3:
			OnButtonPressedOptions();
			break;
		case 4:
			OnButtonPressedCredits();
			break;
		case 5:
			OnButtonPressedExit();
			break;
		}
	}

	private void OnButtonPressedCampain()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus(OnMenuUnloaded_Scenario);
	}

	private void OnMenuUnloaded_Scenario()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadScenarioMenu();
	}

	private void OnButtonPressedOptions()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus(OnMenuUnloaded_Options);
	}

	private void OnMenuUnloaded_Options()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadOptions();
	}

	private void OnButtonPressedCredits()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus(OnMenuUnloaded_Credits);
	}

	private void OnMenuUnloaded_Credits()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadCreditsPanel();
	}

	private void OnButtonPressedExit()
	{
		Debug.Log("EXIT");
		CFGWinAPI.OnDestroy();
		Application.Quit();
	}

	private void PressBuildInit()
	{
		if (m_MainButtons == null)
		{
			return;
		}
		for (int i = 0; i < m_MainButtons.Count; i++)
		{
			switch (i)
			{
			case 0:
				m_MainButtons[i].enabled = true;
				m_MainButtons[i].EnableVisuals();
				m_MainButtons[i].m_Label.text = "Play Demo";
				m_MainButtons[i].m_ButtonClickedCallback = OnPressBuildButtonClick;
				break;
			default:
				m_MainButtons[i].enabled = false;
				m_MainButtons[i].DisableVisuals();
				break;
			case 3:
			case 5:
				break;
			}
		}
	}

	private void OnPressBuildButtonClick(int data)
	{
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.CreateSessionSingle();
		cFGSessionSingle.SelectCampaign(1);
		cFGSessionSingle.StartScenario("scen_01");
		cFGSessionSingle.PrepareLevel_Tactical("1_5_OilRig");
		cFGSessionSingle.ResetMissionStats();
		cFGSessionSingle.OnNewGame(EDifficulty.Normal);
		cFGSessionSingle.ReadCampaignVariableDefinitions();
		cFGSessionSingle.ReadScenarioVariableDefinitions();
		CFGSingleton<CFGGame>.Instance.GoToStoryMovie();
	}
}
