using UnityEngine;
using UnityEngine.UI;

public class CFGMissionEnd : CFGPanel
{
	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_YButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_LoadButtonPad;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	public Text m_Title;

	public Text m_Text;

	public CFGButtonExtension m_ButtonOk;

	public CFGButtonExtension m_Quit;

	public CFGButtonExtension m_LoadSave;

	public CFGImageExtension m_Icon;

	public Image m_Background;

	private bool m_AlreadyConfirmed;

	protected override void Awake()
	{
		base.Awake();
		SetLocalisationOnAwake();
	}

	protected override void Start()
	{
		base.Start();
		SetButtonStates();
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: true);
	}

	private void InitCallbacks()
	{
		if ((bool)m_ButtonOk)
		{
			m_ButtonOk.m_ButtonClickedCallback = OnMenuButtonsClick;
		}
		if ((bool)m_LoadSave)
		{
			m_LoadSave.m_ButtonClickedCallback = OnMenuButtonsClick;
		}
		if ((bool)m_Quit)
		{
			m_Quit.m_ButtonClickedCallback = QuitToMenu;
		}
	}

	public void SetLocalisationOnAwake()
	{
		if ((bool)m_Quit && (bool)m_Quit.m_Label)
		{
			m_Quit.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("quit_mission_end");
			m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_missionfailed_quit");
		}
		if ((bool)m_LoadSave && (bool)m_LoadSave.m_Label)
		{
			m_LoadSave.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_restart");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_missionfailed_restart");
		}
		if ((bool)m_LoadButtonPad)
		{
			m_LoadButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_missionfailed_quickload");
		}
	}

	public override void SetLocalisation()
	{
	}

	public void Init_MissionComplete()
	{
		InitCallbacks();
		m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_missioncomplete");
		m_Text.text = string.Empty;
		m_Icon.IconNumber = 0;
		m_ButtonOk.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_continue");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_continue").ToUpper();
	}

	public void Init_MissionFail(string reason_id, params string[] args)
	{
		InitCallbacks();
		m_Title.text = "<color=#EA4242>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_missionfail") + "</color>";
		m_Text.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(reason_id, args);
		m_Icon.IconNumber = 1;
		if (CFGGame.Permadeath)
		{
			m_ButtonOk.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_exit");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_exit").ToUpper();
			m_ButtonOk.m_ButtonClickedCallback = QuitToMenu;
		}
		else
		{
			m_YButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_restart").ToUpper();
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("loadsave_mission_end").ToUpper();
		}
	}

	public void Init_ScenarioComplete()
	{
		InitCallbacks();
		m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_scenariocomplete");
		m_Text.text = string.Empty;
		m_Icon.IconNumber = 0;
		m_ButtonOk.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_exit");
		m_YButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_exit").ToUpper();
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("missionend_exit").ToUpper();
	}

	private void SetButtonStates()
	{
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if (cFGSessionSingle == null)
		{
			return;
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		bool permadeath = CFGGame.Permadeath;
		bool success = cFGSessionSingle.GetMissionStats().success;
		m_AButtonPad.gameObject.SetActive(flag && (success || permadeath) && !m_AlreadyConfirmed);
		m_YButtonPad.gameObject.SetActive(flag && !success && !permadeath && !m_AlreadyConfirmed);
		m_BButtonPad.gameObject.SetActive(flag && !success && !permadeath && !m_AlreadyConfirmed);
		RectTransform component = m_YButtonPad.gameObject.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.25f, component.anchorMin.y);
		component.anchorMax = new Vector2(0.35f, component.anchorMax.y);
		RectTransform component2 = m_BButtonPad.gameObject.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(0.55f, component2.anchorMin.y);
		component2.anchorMax = new Vector2(0.65f, component2.anchorMax.y);
		if (CFGOptions.Gameplay.QuickSaveEnabled && (bool)m_LoadButtonPad)
		{
			m_LoadButtonPad.gameObject.SetActive(CFG_SG_Manager.QuickSaveExist() && flag && !success && !permadeath && !m_AlreadyConfirmed);
			if (m_LoadButtonPad.gameObject.activeSelf)
			{
				component.anchorMin = new Vector2(0.1f, component.anchorMin.y);
				component.anchorMax = new Vector2(0.2f, component.anchorMax.y);
				component2.anchorMin = new Vector2(0.7f, component2.anchorMin.y);
				component2.anchorMax = new Vector2(0.8f, component2.anchorMax.y);
			}
		}
		m_Quit.gameObject.SetActive(!flag && !success && !permadeath && !m_AlreadyConfirmed);
		m_LoadSave.gameObject.SetActive(!flag && !success && !permadeath && !m_AlreadyConfirmed);
		m_ButtonOk.gameObject.SetActive(!flag && (success || permadeath) && !m_AlreadyConfirmed);
	}

	public override void Update()
	{
		base.Update();
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			SetButtonStates();
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		bool flag = false;
		if (sessionSingle != null)
		{
			flag = sessionSingle.GetMissionStats().success;
		}
		if (Input.GetKeyUp(KeyCode.Return))
		{
			if (!flag && CFGGame.Permadeath)
			{
				InvokeQuit();
			}
			else
			{
				ConfirmMissionEnd();
			}
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			if (flag || CFGGame.Permadeath)
			{
				m_AButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			else
			{
				m_YButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (!flag && CFGGame.Permadeath)
			{
				Invoke("InvokeQuit", 0.3f);
			}
			else
			{
				Invoke("ConfirmMissionEnd", 0.3f);
			}
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f && m_BButtonPad.gameObject.activeSelf)
		{
			m_BButtonPad.SimulateClickGraphicAndSoundOnly();
			Invoke("InvokeQuit", 0.3f);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f && m_LoadButtonPad.gameObject.activeSelf)
		{
			m_LoadButtonPad.SimulateClickGraphicAndSoundOnly();
			if (CFG_SG_Manager.QuickSaveExist())
			{
				Invoke("InvokeQuickLoad", 0.3f);
			}
		}
	}

	private void InvokeQuit()
	{
		QuitToMenu(0);
	}

	private void InvokeQuickLoad()
	{
		if (!m_AlreadyConfirmed)
		{
			m_AlreadyConfirmed = true;
			CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: false);
			CFGSingleton<CFGWindowMgr>.Instance.UnloadMissionEnd();
			CFG_SG_Manager.LoadQuickSave();
		}
	}

	public void OnMenuButtonsClick(int id)
	{
		ConfirmMissionEnd();
	}

	private void ConfirmMissionEnd()
	{
		if (m_AlreadyConfirmed)
		{
			return;
		}
		Debug.Log("mission end confirmed");
		m_AlreadyConfirmed = true;
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: true);
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(item == m_Background.transform);
		}
		CFGFadeToColor componentInChildren = Camera.main.GetComponentInChildren<CFGFadeToColor>();
		if (componentInChildren != null)
		{
			componentInChildren.SetFade(CFGFadeToColor.FadeType.fadeOut, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_FadingColor, 1f, OnFadeOutEnd_ContinueRestart);
		}
		else
		{
			OnFadeOutEnd_ContinueRestart();
		}
	}

	private void QuitToMenu(int i)
	{
		if (m_AlreadyConfirmed)
		{
			return;
		}
		Debug.Log("mission end confirmed");
		m_AlreadyConfirmed = true;
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.StopCurrentDialog(UseCallback: true);
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(item == m_Background.transform);
		}
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

	private void OnFadeOutEnd_ContinueRestart()
	{
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: false);
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMissionEnd();
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if (!(cFGSessionSingle != null))
		{
			return;
		}
		if (cFGSessionSingle.GetMissionStats().scenario_end)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.UnloadingMission);
			}
			CFGSingleton<CFGGame>.Instance.GoToMainMenus();
		}
		else if (cFGSessionSingle.GetMissionStats().success)
		{
			CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.UnloadingMission);
			cFGSessionSingle.ResetMissionStats();
			cFGSessionSingle.GoToStrategic();
		}
		else if (!CFGGame.Permadeath)
		{
			cFGSessionSingle.RestartTactical();
		}
	}

	private void OnFadeOutEnd_Quit()
	{
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_MissionEnd, bEnable: false);
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMissionEnd();
		CFGSingleton<CFGGame>.Instance.GoToMainMenus();
	}
}
