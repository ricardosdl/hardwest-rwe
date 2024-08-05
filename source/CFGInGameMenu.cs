using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGInGameMenu : CFGPanel
{
	public Text m_Title;

	public List<CFGButtonExtension> m_Buttons = new List<CFGButtonExtension>();

	private CFGJoyMenuController m_Controller = new CFGJoyMenuController();

	public GameObject m_PadBtns;

	public CFGButtonExtension m_APad;

	public CFGButtonExtension m_LPad;

	private float m_NextReadAnalog;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	protected override void Start()
	{
		base.Start();
		Debug.Log("In-Game Menu loaded");
		m_Buttons[0].m_ButtonClickedCallback = OnMenuButtons;
		m_Buttons[1].m_ButtonClickedCallback = OnMenuButtons;
		m_Buttons[2].m_ButtonClickedCallback = OnMenuButtons;
		if (!CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			m_Buttons[1].gameObject.SetActive(value: false);
		}
		if (CFGGame.Permadeath)
		{
			m_Buttons[1].gameObject.SetActive(value: false);
		}
		SetPause(bPause: true);
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_Controller.AddList(m_Buttons, null, OnControllerButton);
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
	}

	public override void Update()
	{
		base.Update();
		if (Input.GetKeyUp(KeyCode.Escape) || CFGJoyManager.IsActivated(EJoyAction.ExitB))
		{
			if (m_Buttons[0] != null)
			{
				m_Buttons[0].SimulateClick(bDelayed: true);
			}
			return;
		}
		m_Controller.UpdateInput();
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_PadBtns.SetActive(active);
			m_LastInput = CFGInput.LastReadInputDevice;
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

	private void OnControllerButton(int Btn, bool bSecond)
	{
		if (Btn < m_Buttons.Count && !(m_Buttons[Btn] == null) && !bSecond)
		{
			m_Buttons[Btn].SimulateClick(bDelayed: true);
		}
	}

	public override void SetLocalisation()
	{
		m_Buttons[0].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ingamemenu_resume");
		m_Buttons[1].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ingamemenu_restart");
		m_Buttons[2].m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ingamemenu_quit");
		m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_pause");
		m_APad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_chooseoption");
		m_LPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_strategicpause_movecursor");
	}

	public void OnMenuButtons(int button_id)
	{
		if (!IsPanelWaitingForDestroy())
		{
			switch (button_id)
			{
			case 0:
				OnButtonPressedResume();
				break;
			case 1:
				OnButtonPressedRestart();
				break;
			case 2:
				OnButtonPressedQuit();
				break;
			}
		}
	}

	private void OnButtonPressedResume()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu();
		SetPause(bPause: false);
	}

	private void OnButtonPressedRestart()
	{
		Debug.Log("Restarting tactical");
		CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu(OnMenuUnloaded_Restart);
	}

	private void OnButtonPressedQuit()
	{
		Debug.Log("Leaving game, returning to Main Menu");
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			CFGSingleton<CFGGame>.Instance.CreateSaveGame(CFG_SG_SaveGame.eSG_Source.AutoSave);
		}
		CFGAudioManager.Instance.StopBackgroundMusic();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadObjectivePanel();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu(OnMenuUnloaded_Quit);
	}

	private void OnMenuUnloaded_Restart()
	{
		SetPause(bPause: false);
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if (cFGSessionSingle != null)
		{
			cFGSessionSingle.RestartTactical();
		}
	}

	private void OnMenuUnloaded_Quit()
	{
		SetPause(bPause: false);
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.UnloadingMission);
		}
		CFGSingleton<CFGGame>.Instance.GoToMainMenus();
	}

	private void SetPause(bool bPause)
	{
		CFGTimer.SetPaused_Gameplay(bPause);
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_InGameMenu, bPause);
	}
}
