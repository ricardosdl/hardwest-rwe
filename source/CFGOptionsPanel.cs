using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CFGOptionsPanel : CFGPanel
{
	private enum EOptionsSubMenu
	{
		None,
		Keybindings,
		ControllerLayout
	}

	public CFGButtonExtension m_LPad;

	public CFGButtonExtension m_LPad2;

	public CFGButtonExtension m_APad;

	public CFGButtonExtension m_BPad;

	public CFGButtonExtension m_XPad;

	public List<CFGButtonExtension> m_MainButtons = new List<CFGButtonExtension>();

	public CFGButtonExtension m_Accept;

	public CFGButtonExtension m_Cancel;

	public CFGButtonExtension m_Default;

	public CFGButtonExtension m_Apply;

	public Text m_Title;

	public GameObject m_Gamepad;

	public Text m_AbilitySwitch;

	public Text m_CharacterChange;

	public Text m_WeaponSwitch;

	public Text m_CancelTxt;

	public Text m_EndTurn;

	public Text m_UseAccept;

	public Text m_MoveCamera;

	public Text m_CameraFocus;

	public Text m_PauseMenu;

	public Text m_CharacterScreen;

	public Text m_RotateCamera;

	public Text m_SwitchFloor;

	public Text m_MoveCursor;

	public GameObject m_ElementPrefab;

	public GameObject m_OptionsParent;

	public Scrollbar m_Scrollbar;

	public List<CFGOptElement> m_OptElements = new List<CFGOptElement>();

	private int m_CurrentOptionsSelected = -1;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private EInputMode m_LastReadInput = EInputMode.KeyboardAndMouse;

	private CFGJoyMenuController m_JMC = new CFGJoyMenuController();

	private EOptionsSubMenu m_CurrentSubMenu;

	private ELanguage m_InitLanguage;

	private EDifficulty m_Difficulty = EDifficulty.Normal;

	private List<Resolution> m_Resolutions = new List<Resolution>();

	private int m_ShadowsQuality = 1;

	private List<CFGActionItem> m_BindActions = new List<CFGActionItem>();

	private int m_BindingKey = -1;

	private bool m_BindingAlt;

	private CFGVolumeTester m_VolumeTest;

	private int m_Controller_ElementSelected = -1;

	private float m_LastJoyReadTime;

	private float m_LastJoyReadTimeLR;

	public void OnResolutionChanged(Vector2 new_resolution, Vector2 old_resolution)
	{
		float num = new_resolution.x / old_resolution.x;
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			int num2 = 1;
			num2 = ((!text.resizeTextForBestFit) ? text.fontSize : text.resizeTextMaxSize);
			if (Screen.height > 900 || Screen.width > 1600)
			{
				text.resizeTextForBestFit = false;
			}
			CFGTextExtension cFGTextExtension = text as CFGTextExtension;
			if (cFGTextExtension == null)
			{
				text.fontSize = Mathf.FloorToInt(Mathf.Min((float)num2 * num, (float)num2 * num));
			}
			else
			{
				cFGTextExtension.OnResolutionChange();
			}
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		SetVideoOptions();
	}

	public void ShowGamepad()
	{
		foreach (CFGOptElement optElement in m_OptElements)
		{
			UnityEngine.Object.Destroy(optElement.gameObject);
		}
		m_OptElements.Clear();
		m_Gamepad.SetActive(value: true);
	}

	public void HideGamepad()
	{
		m_Gamepad.SetActive(value: false);
	}

	public override void SetLocalisation()
	{
		CFGTextManager instance = CFGSingletonResourcePrefab<CFGTextManager>.Instance;
		m_Title.text = instance.GetLocalizedText("options_title");
		m_Cancel.m_Label.text = instance.GetLocalizedText("options_cancel");
		m_Accept.m_Label.text = instance.GetLocalizedText("options_accept");
		m_MainButtons[0].m_Label.text = instance.GetLocalizedText((m_CurrentSubMenu != 0) ? "options_back" : "options_button_gameplay");
		m_MainButtons[1].m_Label.text = instance.GetLocalizedText("options_button_interface");
		m_MainButtons[2].m_Label.text = instance.GetLocalizedText("options_button_video");
		m_MainButtons[3].m_Label.text = instance.GetLocalizedText("options_button_audio");
		m_MainButtons[4].m_Label.text = instance.GetLocalizedText("options_button_controls");
		m_LPad.m_Label.text = instance.GetLocalizedText("pad_options_chooseoption");
		m_LPad2.m_Label.text = instance.GetLocalizedText("pad_options_changecategory");
		m_APad.m_Label.text = instance.GetLocalizedText("pad_options_adjust");
		m_BPad.m_Label.text = instance.GetLocalizedText("pad_options_close");
		m_XPad.m_Label.text = instance.GetLocalizedText("pad_options_apply");
		m_AbilitySwitch.text = instance.GetLocalizedText("options_padlayout_switchab");
		m_CharacterChange.text = instance.GetLocalizedText("options_padlayout_switchchar");
		m_WeaponSwitch.text = instance.GetLocalizedText("options_padlayout_switchweapon");
		m_CancelTxt.text = instance.GetLocalizedText("options_padlayout_cancel");
		m_EndTurn.text = instance.GetLocalizedText("options_padlayout_skipturn");
		m_UseAccept.text = instance.GetLocalizedText("options_padlayout_moveuse");
		m_MoveCamera.text = instance.GetLocalizedText("options_padlayout_camera");
		m_CameraFocus.text = instance.GetLocalizedText("options_padlayout_focus");
		m_PauseMenu.text = instance.GetLocalizedText("options_padlayout_pause");
		m_CharacterScreen.text = instance.GetLocalizedText("options_padlayout_charscreen");
		m_RotateCamera.text = instance.GetLocalizedText("options_padlayout_rotatecam");
		m_SwitchFloor.text = instance.GetLocalizedText("options_padlayout_switchfloor");
		m_MoveCursor.text = instance.GetLocalizedText("options_padlayout_cursor");
	}

	protected override void Start()
	{
		base.Start();
		m_Apply.gameObject.SetActive(value: false);
		m_Default.gameObject.SetActive(value: false);
		m_Cancel.m_ButtonClickedCallback = OnCancelClick;
		m_Accept.m_ButtonClickedCallback = OnAcceptClick;
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		m_InitLanguage = CFGOptions.Gameplay.Language;
		m_Difficulty = CFGGame.Difficulty;
		m_ShadowsQuality = QualitySettings.GetQualityLevel();
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			if (Screen.resolutions[i].width >= 1280)
			{
				m_Resolutions.Add(Screen.resolutions[i]);
			}
		}
		m_BindActions.Clear();
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in CFGInput.m_AllActions)
		{
			if (!allAction.Value.HasFlag(CFGActionItem.EFlag.Unbindable))
			{
				m_BindActions.Add(allAction.Value);
			}
		}
		GotoSubMenu(EOptionsSubMenu.None);
		OnMainOptionClick(0);
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_JMC.AddList(m_MainButtons, Controller_OnMoveList, null);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_Down_Digital = EJoyButton.RightBumper;
			cFGJoyMenuButtonList.m_UP_Digital = EJoyButton.LeftBumper;
			cFGJoyMenuButtonList.m_UP_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_Down_Analog = EJoyButton.Unknown;
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_Accept.gameObject.transform.parent.gameObject.SetActive(!flag);
			m_APad.gameObject.SetActive(flag);
			m_BPad.gameObject.SetActive(flag);
			m_XPad.gameObject.SetActive(flag);
			m_LPad.gameObject.SetActive(flag);
			m_LPad2.gameObject.SetActive(flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		m_JMC.SetListChangeButtons(EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.Unknown);
		CFGSingleton<CFGWindowMgr>.Instance.m_OnResolutionChangeCallback = OnResolutionChanged;
	}

	private void Switch_ToGamepad()
	{
		if (m_OptElements == null || m_OptElements.Count == 0)
		{
			return;
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			for (int i = 0; i < m_OptElements.Count; i++)
			{
				if (m_OptElements[i].GetControlType() == 3)
				{
					m_OptElements[i].SetType(6);
				}
			}
			if (m_Controller_ElementSelected >= 0)
			{
				float height = m_OptElements[0].transform.parent.GetComponent<RectTransform>().rect.height;
				float num = (float)m_Controller_ElementSelected * m_OptElements[0].GetComponent<RectTransform>().rect.height;
				num /= height;
				m_Scrollbar.value = 0f;
				m_Scrollbar.value = 1f - num;
			}
		}
		else
		{
			if (CFGInput.LastReadInputDevice != EInputMode.KeyboardAndMouse)
			{
				return;
			}
			for (int j = 0; j < m_OptElements.Count; j++)
			{
				if (m_OptElements[j].GetControlType() == 6)
				{
					m_OptElements[j].SetType(3);
				}
			}
		}
	}

	public override void Update()
	{
		base.Update();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_Accept.gameObject.transform.parent.gameObject.SetActive(!flag);
			m_APad.gameObject.SetActive(flag);
			m_BPad.gameObject.SetActive(flag);
			m_XPad.gameObject.SetActive(flag);
			m_LPad.gameObject.SetActive(flag);
			m_LPad2.gameObject.SetActive(flag);
			if (!flag && m_Controller_ElementSelected >= 0)
			{
				m_OptElements[m_Controller_ElementSelected].OnPointerExit(null);
			}
			if (flag)
			{
				foreach (CFGOptElement optElement in m_OptElements)
				{
					optElement.OnPointerExit(null);
				}
				if (m_CurrentOptionsSelected != 2)
				{
					Controller_SelectElement(1);
				}
				else
				{
					Controller_SelectElement(0);
				}
			}
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (m_BindingKey != -1)
		{
			bool flag2 = false;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				flag2 = true;
			}
			else
			{
				KeyCode[] validUserKeys = CFGInput.ValidUserKeys;
				foreach (KeyCode key in validUserKeys)
				{
					if (Input.GetKeyDown(key))
					{
						CFGInput.BindKey(m_BindActions[m_BindingKey].ActionCommand, !m_BindingAlt, (ECFGKeyCode)key, bAlt: false, bCtrl: false);
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				List<EActionCommand> lastUnbindList = CFGInput.LastUnbindList;
				for (int j = 0; j < m_OptElements.Count; j++)
				{
					if (j == m_BindingKey)
					{
						m_OptElements[j].Controls = m_BindActions[m_BindingKey].ToTextLong_First();
						m_OptElements[j].ControlsAlt = m_BindActions[m_BindingKey].ToTextLong_Second();
						if (m_BindingAlt)
						{
							m_OptElements[j].ControlsAltSelected = false;
							m_OptElements[j].m_Bind.enabled = true;
						}
						else
						{
							m_OptElements[j].ControlsSelected = false;
							m_OptElements[j].m_AltBind.enabled = true;
						}
					}
					else
					{
						m_OptElements[j].ToggleEnabled(enabled: true);
					}
					if (lastUnbindList.Contains(m_BindActions[j].ActionCommand))
					{
						m_OptElements[j].Controls = m_BindActions[j].ToTextLong_First();
						m_OptElements[j].ControlsAlt = m_BindActions[j].ToTextLong_Second();
					}
				}
				m_Cancel.enabled = true;
				m_Accept.enabled = true;
				m_MainButtons[0].enabled = true;
				m_BindingKey = -1;
			}
		}
		m_JMC.UpdateInput();
		if (Time.time > m_LastJoyReadTime)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0.6f)
			{
				Controller_SelectNext(-1);
				m_LastJoyReadTime = Time.time + 0.2f;
				m_LPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0.6f)
			{
				Controller_SelectNext(1);
				m_LPad.SimulateClickGraphicAndSoundOnly();
				m_LastJoyReadTime = Time.time + 0.2f;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Left) > 0.6f)
			{
				if (Controller_ActivateItem(1))
				{
					m_LastJoyReadTime = Time.time + 0.02f;
				}
				else if (Controller_ActivateItem(3))
				{
					m_LastJoyReadTime = Time.time + 0.2f;
				}
				m_LPad2.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Right) > 0.6f)
			{
				if (Controller_ActivateItem(2))
				{
					m_LastJoyReadTime = Time.time + 0.02f;
				}
				else if (Controller_ActivateItem(4))
				{
					m_LastJoyReadTime = Time.time + 0.2f;
				}
				m_LPad2.SimulateClickGraphicAndSoundOnly();
			}
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
		{
			CFGAudioManager.Instance.PlaySound2D(m_MainButtons[0].m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
		{
			CFGAudioManager.Instance.PlaySound2D(m_MainButtons[0].m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Bottom) > 0f)
		{
			m_LPad.SimulateClickGraphicAndSoundOnly();
			Controller_SelectNext(1);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Top) > 0f)
		{
			m_LPad.SimulateClickGraphicAndSoundOnly();
			Controller_SelectNext(-1);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			Controller_ActivateItem(0);
			m_APad.SimulateClickGraphicAndSoundOnly();
			CFGJoyManager.ClearJoyActions(0.1f);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
		{
			if (m_CurrentSubMenu != 0)
			{
				OnBackOptionClick(0);
			}
			else
			{
				m_Cancel.SimulateClick();
			}
			m_BPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f && m_CurrentSubMenu == EOptionsSubMenu.None)
		{
			m_Accept.SimulateClick();
			m_XPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left, bContinous: true) > 0f)
		{
			Controller_ActivateItem(1);
			m_LPad2.SimulateClickGraphicAndSoundOnly();
			CFGJoyManager.ClearJoyActions(0.2f);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right, bContinous: true) > 0f)
		{
			Controller_ActivateItem(2);
			m_LPad2.SimulateClickGraphicAndSoundOnly();
			CFGJoyManager.ClearJoyActions(0.2f);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left) > 0f)
		{
			Controller_ActivateItem(3);
			m_LPad2.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right) > 0f)
		{
			Controller_ActivateItem(4);
			m_LPad2.SimulateClickGraphicAndSoundOnly();
		}
		Switch_ToGamepad();
	}

	private void GotoSubMenu(EOptionsSubMenu sub_menu)
	{
		m_CurrentSubMenu = sub_menu;
		SetLocalisation();
		for (int i = 0; i < m_MainButtons.Count; i++)
		{
			m_MainButtons[i].IsSelected = false;
		}
		switch (m_CurrentSubMenu)
		{
		case EOptionsSubMenu.None:
		{
			for (int j = 0; j < m_MainButtons.Count; j++)
			{
				m_MainButtons[j].gameObject.SetActive(value: true);
				m_MainButtons[j].m_ButtonClickedCallback = OnMainOptionClick;
			}
			break;
		}
		case EOptionsSubMenu.Keybindings:
			m_MainButtons[0].gameObject.SetActive(value: true);
			m_MainButtons[0].m_ButtonClickedCallback = OnBackOptionClick;
			m_MainButtons[1].gameObject.SetActive(value: false);
			m_MainButtons[2].gameObject.SetActive(value: false);
			m_MainButtons[3].gameObject.SetActive(value: false);
			m_MainButtons[4].gameObject.SetActive(value: false);
			SetKeybindingsOptions();
			break;
		case EOptionsSubMenu.ControllerLayout:
			m_MainButtons[0].gameObject.SetActive(value: true);
			m_MainButtons[0].m_ButtonClickedCallback = OnBackOptionClick;
			m_MainButtons[1].gameObject.SetActive(value: false);
			m_MainButtons[2].gameObject.SetActive(value: false);
			m_MainButtons[3].gameObject.SetActive(value: false);
			m_MainButtons[4].gameObject.SetActive(value: false);
			ShowGamepad();
			break;
		}
	}

	private void OnMainOptionClick(int nr)
	{
		if (m_CurrentOptionsSelected != -1)
		{
			m_MainButtons[m_CurrentOptionsSelected].IsSelected = false;
		}
		m_CurrentOptionsSelected = nr;
		m_MainButtons[m_CurrentOptionsSelected].IsSelected = true;
		if (m_VolumeTest != null)
		{
			UnityEngine.Object.Destroy(m_VolumeTest);
			m_VolumeTest = null;
		}
		switch (nr)
		{
		case 0:
			SetGameplayOptions();
			break;
		case 1:
			SetInterfaceOptions();
			break;
		case 2:
			SetVideoOptions();
			break;
		case 3:
			SetAudioOptions();
			break;
		case 4:
			SetControlsOptions();
			break;
		}
	}

	private void OnBackOptionClick(int nr)
	{
		GotoSubMenu(EOptionsSubMenu.None);
		HideGamepad();
		OnMainOptionClick(4);
	}

	private void SetGameplayOptions()
	{
		bool flag = CFGSingleton<CFGGame>.Instance.IsInGame() || CFGSingleton<CFGGame>.Instance.IsInStrategic();
		SpawnOptionsElements((!flag) ? 6 : 7);
		m_OptElements[0].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_language");
		foreach (int value in Enum.GetValues(typeof(ELanguage)))
		{
			if (value != 6 && value != 5)
			{
				m_OptElements[0].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_language_" + ((ELanguage)value).GetLanguageCode()));
			}
		}
		string[] directories = Directory.GetDirectories(CFGData.GetDataPathFor("Texts"));
		foreach (string path in directories)
		{
			m_OptElements[0].m_DropDownElements.Add(new DirectoryInfo(path).Name);
			m_OptElements[0].m_DropDownElementColors.Add(m_OptElements[0].m_DropDownElements.Count - 1, Color.green);
		}
		if (CFGOptions.Gameplay.Language == ELanguage.Custom)
		{
			if (!string.IsNullOrEmpty(CFGOptions.Gameplay.CustomLanguage) && !m_OptElements[0].m_DropDownElements.Contains(CFGOptions.Gameplay.CustomLanguage))
			{
				m_OptElements[0].m_DropDownElements.Add(CFGOptions.Gameplay.CustomLanguage);
				m_OptElements[0].m_DropDownElementColors.Add(m_OptElements[0].m_DropDownElements.Count - 1, Color.green);
			}
			m_OptElements[0].m_CurrentSelectedDropdownElement = Mathf.Max(0, m_OptElements[0].m_DropDownElements.IndexOf(CFGOptions.Gameplay.CustomLanguage));
		}
		else
		{
			m_OptElements[0].m_CurrentSelectedDropdownElement = (int)CFGOptions.Gameplay.Language;
		}
		m_OptElements[0].m_SelectDropdownCallback = OnGameplayDropdownList;
		m_OptElements[0].SetType(3);
		m_OptElements[0].ToggleEnabled(enabled: true);
		m_OptElements[1].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_subtitles");
		m_OptElements[1].CheckBox = CFGOptions.Gameplay.DisplaySubtitles;
		m_OptElements[1].m_ToggleCheckBoxCallback = OnGameplayCheckBox;
		m_OptElements[1].SetType(0);
		m_OptElements[1].ToggleEnabled(enabled: true);
		m_OptElements[2].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_always_run");
		m_OptElements[2].CheckBox = CFGOptions.Gameplay.AlwaysRunOnTactical;
		m_OptElements[2].m_ToggleCheckBoxCallback = OnGameplayCheckBox;
		m_OptElements[2].SetType(0);
		m_OptElements[2].ToggleEnabled(enabled: true);
		m_OptElements[3].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_barter_autoadjust");
		m_OptElements[3].CheckBox = CFGOptions.Gameplay.BarterAutoAdjust;
		m_OptElements[3].m_ToggleCheckBoxCallback = OnGameplayCheckBox;
		m_OptElements[3].SetType(0);
		m_OptElements[3].ToggleEnabled(enabled: true);
		m_OptElements[4].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_pawn_superspeed");
		m_OptElements[4].CheckBox = CFGOptions.Gameplay.PawnSuperSpeed;
		m_OptElements[4].m_ToggleCheckBoxCallback = OnGameplayCheckBox;
		m_OptElements[4].SetType(0);
		m_OptElements[4].ToggleEnabled(enabled: true);
		int num = 5;
		m_OptElements[5].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_quicksave_experimental");
		m_OptElements[5].m_NormalTextColor = Color.red;
		m_OptElements[5].CheckBox = CFGOptions.Gameplay.QuickSaveEnabled;
		m_OptElements[5].m_ToggleCheckBoxCallback = OnGameplayCheckBox;
		m_OptElements[5].SetType(0);
		m_OptElements[5].ToggleEnabled(enabled: true);
		num++;
		if (flag)
		{
			m_OptElements[num].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_difficulty");
			m_OptElements[num].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_difficulty_easy"));
			m_OptElements[num].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_difficulty_medium"));
			m_OptElements[num].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_difficulty_hard"));
			m_OptElements[num].m_CurrentSelectedDropdownElement = (int)m_Difficulty;
			m_OptElements[num].m_SelectDropdownCallback = OnGameplayDropdownList;
			m_OptElements[num].SetType(3);
			m_OptElements[num].ToggleEnabled(enabled: true);
		}
		m_Controller_ElementSelected = -1;
		Controller_SelectNext(1);
	}

	private void OnGameplayDropdownList(int val, int position_in_list)
	{
		Debug.Log("On gameplay option change " + val + " " + position_in_list);
		switch (position_in_list)
		{
		case 0:
		{
			ELanguage eLanguage = ELanguage.Custom;
			if (Enum.IsDefined(typeof(ELanguage), val))
			{
				eLanguage = (ELanguage)val;
				if (eLanguage == ELanguage.None)
				{
					eLanguage = ELanguage.Custom;
				}
			}
			if (eLanguage != ELanguage.Custom)
			{
				CFGOptions.Gameplay.SelectLanguage(eLanguage, bForce: true);
			}
			else
			{
				string newCustomLanguage = m_OptElements[0].m_DropDownElements[val];
				CFGOptions.Gameplay.SelectCustomLanguage(newCustomLanguage, bForce: true);
			}
			SetLocalisation();
			OnMainOptionClick(0);
			break;
		}
		case 5:
			m_Difficulty = (EDifficulty)val;
			break;
		case 6:
			m_Difficulty = (EDifficulty)val;
			break;
		}
	}

	private void OnGameplayCheckBox(bool enabled, int position_in_list)
	{
		Debug.Log("On game checkbox clicked " + enabled + " " + position_in_list);
		switch (position_in_list)
		{
		case 1:
			CFGOptions.Gameplay.DisplaySubtitles = enabled;
			break;
		case 2:
			CFGOptions.Gameplay.AlwaysRunOnTactical = enabled;
			break;
		case 3:
			CFGOptions.Gameplay.BarterAutoAdjust = enabled;
			break;
		case 4:
			CFGOptions.Gameplay.PawnSuperSpeed = enabled;
			break;
		case 5:
			CFGOptions.Gameplay.QuickSaveEnabled = enabled;
			break;
		}
	}

	private void ApplyGameplayOptions()
	{
		CFGGame.Difficulty = m_Difficulty;
	}

	private void SetInterfaceOptions()
	{
		SpawnOptionsElements(7);
		m_OptElements[0].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_coneofview");
		m_OptElements[0].CheckBox = CFGOptions.Gameplay.ShowConeOfView;
		m_OptElements[0].m_ToggleCheckBoxCallback = OnInterfaceCheckBox;
		m_OptElements[0].SetType(0);
		m_OptElements[0].ToggleEnabled(enabled: true);
		m_OptElements[1].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_reactionshot");
		m_OptElements[1].CheckBox = CFGOptions.Gameplay.ShowReactionRange;
		m_OptElements[1].m_ToggleCheckBoxCallback = OnInterfaceCheckBox;
		m_OptElements[1].SetType(0);
		m_OptElements[1].ToggleEnabled(enabled: true);
		m_OptElements[2].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_covers");
		m_OptElements[2].CheckBox = CFGOptions.Gameplay.ShowCoverIcons;
		m_OptElements[2].m_ToggleCheckBoxCallback = OnInterfaceCheckBox;
		m_OptElements[2].SetType(0);
		m_OptElements[2].ToggleEnabled(enabled: true);
		m_OptElements[3].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_floatingtext");
		m_OptElements[3].CheckBox = CFGOptions.Gameplay.ShowFloatingTexts;
		m_OptElements[3].m_ToggleCheckBoxCallback = OnInterfaceCheckBox;
		m_OptElements[3].SetType(0);
		m_OptElements[3].ToggleEnabled(enabled: true);
		m_OptElements[4].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_range");
		m_OptElements[4].CheckBox = CFGOptions.Gameplay.ShowWeaponRange;
		m_OptElements[4].m_ToggleCheckBoxCallback = OnInterfaceCheckBox;
		m_OptElements[4].SetType(0);
		m_OptElements[4].ToggleEnabled(enabled: true);
		m_OptElements[5].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_usableglow");
		m_OptElements[5].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_usableglow_0"));
		m_OptElements[5].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_usableglow_1"));
		m_OptElements[5].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_usableglow_2"));
		m_OptElements[5].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_usableglow_3"));
		m_OptElements[5].m_CurrentSelectedDropdownElement = CFGOptions.Gameplay.InteractiveObjectsGlow;
		m_OptElements[5].m_SelectDropdownCallback = OnInterfaceDropdownList;
		m_OptElements[5].SetType(3);
		m_OptElements[5].ToggleEnabled(enabled: true);
		m_OptElements[6].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_explorationtextspeed");
		m_OptElements[6].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_explorationtextspeed_2"));
		m_OptElements[6].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_explorationtextspeed_1"));
		m_OptElements[6].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_helpers_explorationtextspeed_0"));
		m_OptElements[6].m_CurrentSelectedDropdownElement = CFGOptions.Gameplay.ExplorationTextSpeed;
		m_OptElements[6].m_SelectDropdownCallback = OnInterfaceDropdownList;
		m_OptElements[6].SetType(3);
		m_OptElements[6].ToggleEnabled(enabled: true);
		m_Controller_ElementSelected = -1;
		Controller_SelectNext(1);
	}

	private void OnInterfaceCheckBox(bool enabled, int position_in_list)
	{
		Debug.Log("On interface checkbox clicked " + enabled + " " + position_in_list);
		switch (position_in_list)
		{
		case 0:
			CFGOptions.Gameplay.ShowConeOfView = enabled;
			break;
		case 1:
			CFGOptions.Gameplay.ShowReactionRange = enabled;
			break;
		case 2:
			CFGOptions.Gameplay.ShowCoverIcons = enabled;
			break;
		case 3:
			CFGOptions.Gameplay.ShowFloatingTexts = enabled;
			break;
		case 4:
			CFGOptions.Gameplay.ShowWeaponRange = enabled;
			break;
		}
	}

	private void OnInterfaceDropdownList(int val, int position_in_list)
	{
		Debug.Log("On interface option change " + val + " " + position_in_list);
		switch (position_in_list)
		{
		case 5:
			CFGOptions.Gameplay.InteractiveObjectsGlow = val;
			break;
		case 6:
			CFGOptions.Gameplay.ExplorationTextSpeed = val;
			break;
		}
	}

	private void SetVideoOptions()
	{
		SpawnOptionsElements(13);
		int num = -1;
		int num2 = 0;
		List<string> list = new List<string>();
		for (int i = 0; i < m_Resolutions.Count; i++)
		{
			string text = m_Resolutions[i].width + "x" + m_Resolutions[i].height + "x" + m_Resolutions[i].refreshRate;
			if (m_Resolutions[i].width > 1920)
			{
				text = "<color=#EA4242>" + text + "</color>";
			}
			list.Add(text);
			if (m_Resolutions[i].width == Screen.width && m_Resolutions[i].height == Screen.height)
			{
				if (m_Resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
				{
					num = i;
				}
				else
				{
					num2 = i;
				}
			}
		}
		if (num == -1)
		{
			num = num2;
		}
		string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_video_low");
		string localizedText2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_video_medium");
		string localizedText3 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_video_high");
		string localizedText4 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_video_veryhigh");
		m_OptElements[0].TitleGray = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_video_title");
		m_OptElements[0].SetType(4);
		m_OptElements[0].ToggleEnabled(enabled: true);
		m_OptElements[1].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_resolutions");
		m_OptElements[1].m_DropDownElements = list;
		m_OptElements[1].m_CurrentSelectedDropdownElement = num;
		m_OptElements[1].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[1].SetType(3);
		m_OptElements[1].ToggleEnabled(enabled: true);
		m_OptElements[2].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_fullscreen");
		m_OptElements[2].CheckBox = Screen.fullScreen;
		m_OptElements[2].m_ToggleCheckBoxCallback = OnVideoCheckBox;
		m_OptElements[2].SetType(0);
		m_OptElements[2].ToggleEnabled(enabled: true);
		m_OptElements[3].m_ApllyVideo.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_resolution_apply");
		m_OptElements[3].ApplyVideoBtn = OnApplyDisplaySettings;
		m_OptElements[3].SetType(5);
		m_OptElements[3].ToggleEnabled(enabled: true);
		m_OptElements[4].SetType(7);
		m_OptElements[5].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_vsync");
		m_OptElements[5].CheckBox = CFGOptions.Graphics.VSync;
		m_OptElements[5].m_ToggleCheckBoxCallback = OnVideoCheckBox;
		m_OptElements[5].SetType(0);
		m_OptElements[5].ToggleEnabled(enabled: true);
		m_OptElements[6].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_textures");
		m_OptElements[6].m_DropDownElements.Add(localizedText3);
		m_OptElements[6].m_DropDownElements.Add(localizedText2);
		m_OptElements[6].m_DropDownElements.Add(localizedText);
		m_OptElements[6].m_CurrentSelectedDropdownElement = CFGOptions.Graphics.TexturesQuality;
		m_OptElements[6].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[6].SetType(3);
		m_OptElements[6].ToggleEnabled(enabled: true);
		m_OptElements[7].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_aniso");
		m_OptElements[7].CheckBox = CFGOptions.Graphics.Aniso;
		m_OptElements[7].m_ToggleCheckBoxCallback = OnVideoCheckBox;
		m_OptElements[7].SetType(0);
		m_OptElements[7].ToggleEnabled(enabled: true);
		m_OptElements[8].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_shadows_quality");
		m_OptElements[8].m_DropDownElements.Add(localizedText4);
		m_OptElements[8].m_DropDownElements.Add(localizedText3);
		m_OptElements[8].m_DropDownElements.Add(localizedText2);
		m_OptElements[8].m_DropDownElements.Add(localizedText);
		m_OptElements[8].m_CurrentSelectedDropdownElement = m_ShadowsQuality;
		m_OptElements[8].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[8].SetType(3);
		m_OptElements[8].ToggleEnabled(enabled: true);
		m_OptElements[9].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_shaders_quality");
		m_OptElements[9].m_DropDownElements.Add(localizedText3);
		m_OptElements[9].m_DropDownElements.Add(localizedText2);
		m_OptElements[9].m_DropDownElements.Add(localizedText);
		m_OptElements[9].m_CurrentSelectedDropdownElement = CFGOptions.Graphics.ShadersQuality;
		m_OptElements[9].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[9].SetType(3);
		m_OptElements[9].ToggleEnabled(enabled: true);
		m_OptElements[10].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_post_process_quality");
		m_OptElements[10].m_DropDownElements.Add(localizedText3);
		m_OptElements[10].m_DropDownElements.Add(localizedText2);
		m_OptElements[10].m_DropDownElements.Add(localizedText);
		m_OptElements[10].m_CurrentSelectedDropdownElement = CFGOptions.Graphics.PostProcessing;
		m_OptElements[10].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[10].SetType(3);
		m_OptElements[10].ToggleEnabled(enabled: true);
		m_OptElements[11].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_waterreflections");
		m_OptElements[11].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_waterreflections_high"));
		m_OptElements[11].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_waterreflections_medium"));
		m_OptElements[11].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_waterreflections_low"));
		m_OptElements[11].m_DropDownElements.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_waterreflections_off"));
		m_OptElements[11].m_CurrentSelectedDropdownElement = CFGOptions.Graphics.WaterReflections;
		m_OptElements[11].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[11].SetType(3);
		m_OptElements[11].ToggleEnabled(enabled: true);
		m_OptElements[12].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_world_details");
		m_OptElements[12].m_DropDownElements.Add(localizedText3);
		m_OptElements[12].m_DropDownElements.Add(localizedText2);
		m_OptElements[12].m_DropDownElements.Add(localizedText);
		m_OptElements[12].m_CurrentSelectedDropdownElement = CFGOptions.Graphics.WorldDetails;
		m_OptElements[12].m_SelectDropdownCallback = OnVideoDropdownList;
		m_OptElements[12].SetType(3);
		m_OptElements[12].ToggleEnabled(enabled: true);
		m_Controller_ElementSelected = -1;
		Controller_SelectElement(1);
	}

	private void OnVideoDropdownList(int val, int position_in_list)
	{
		Debug.Log("On video option change " + val + " " + position_in_list);
		switch (position_in_list)
		{
		case 6:
			CFGOptions.Graphics.TexturesQuality = val;
			break;
		case 8:
			m_ShadowsQuality = val;
			break;
		case 9:
			CFGOptions.Graphics.ShadersQuality = val;
			break;
		case 10:
			CFGOptions.Graphics.PostProcessing = val;
			break;
		case 11:
			CFGOptions.Graphics.WaterReflections = val;
			break;
		case 12:
			CFGOptions.Graphics.WorldDetails = val;
			break;
		case 7:
			break;
		}
	}

	private void OnVideoCheckBox(bool enabled, int position_in_list)
	{
		Debug.Log("On video checkbox clicked " + enabled + " " + position_in_list);
		switch (position_in_list)
		{
		case 5:
			CFGOptions.Graphics.VSync = enabled;
			break;
		case 7:
			CFGOptions.Graphics.Aniso = enabled;
			break;
		case 6:
			break;
		}
	}

	private void OnApplyDisplaySettings(int a)
	{
		int currentSelectedDropdownElement = m_OptElements[1].m_CurrentSelectedDropdownElement;
		if (currentSelectedDropdownElement >= 0 && currentSelectedDropdownElement < m_Resolutions.Count)
		{
			int width = m_Resolutions[currentSelectedDropdownElement].width;
			int height = m_Resolutions[currentSelectedDropdownElement].height;
			int refreshRate = m_Resolutions[currentSelectedDropdownElement].refreshRate;
			bool checkBox = m_OptElements[2].CheckBox;
			Debug.Log("Applying video: " + width + "x" + height + "x" + refreshRate + ((!checkBox) ? " windowed" : " fullscreen"));
			Screen.SetResolution(width, height, checkBox, refreshRate);
		}
	}

	private void ApplyVideoOptions()
	{
		Debug.Log("Apply Video options");
		if (m_CurrentSubMenu == EOptionsSubMenu.None && m_CurrentOptionsSelected == 2)
		{
			OnApplyDisplaySettings(0);
		}
		QualitySettings.SetQualityLevel(m_ShadowsQuality, applyExpensiveChanges: true);
		CFGOptions.Graphics.ApplyToUnity();
	}

	private void SetAudioOptions()
	{
		SpawnOptionsElements(7);
		m_VolumeTest = base.gameObject.AddComponent<CFGVolumeTester>();
		m_OptElements[0].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_master_volume");
		m_OptElements[0].SliderMin = 0.0001f;
		m_OptElements[0].SliderMax = 1f;
		m_OptElements[0].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.MasterVolume);
		m_OptElements[0].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[0].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[0].SetType(1);
		m_OptElements[0].ToggleEnabled(enabled: true);
		m_OptElements[1].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_music_volume");
		m_OptElements[1].SliderMin = 0.0001f;
		m_OptElements[1].SliderMax = 1f;
		m_OptElements[1].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.MusicVolume);
		m_OptElements[1].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[1].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[1].SetType(1);
		m_OptElements[1].ToggleEnabled(enabled: true);
		m_OptElements[2].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_speech_volume");
		m_OptElements[2].SliderMin = 0.0001f;
		m_OptElements[2].SliderMax = 1f;
		m_OptElements[2].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.DialogsVolume);
		m_OptElements[2].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[2].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[2].SetType(1);
		m_OptElements[2].ToggleEnabled(enabled: true);
		m_OptElements[3].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_enviro_volume");
		m_OptElements[3].SliderMin = 0.0001f;
		m_OptElements[3].SliderMax = 1f;
		m_OptElements[3].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.EnviroVolume);
		m_OptElements[3].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[4].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[3].SetType(1);
		m_OptElements[3].ToggleEnabled(enabled: true);
		m_OptElements[4].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_sfx_volume");
		m_OptElements[4].SliderMin = 0.0001f;
		m_OptElements[4].SliderMax = 1f;
		m_OptElements[4].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.SFXVolume);
		m_OptElements[4].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[4].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[4].SetType(1);
		m_OptElements[4].ToggleEnabled(enabled: true);
		m_OptElements[5].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_interface_volume");
		m_OptElements[5].SliderMin = 0.0001f;
		m_OptElements[5].SliderMax = 1f;
		m_OptElements[5].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.InterfaceVolume);
		m_OptElements[5].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[5].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[5].SetType(1);
		m_OptElements[5].ToggleEnabled(enabled: true);
		m_OptElements[6].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_cinematic_volume");
		m_OptElements[6].SliderMin = 0.0001f;
		m_OptElements[6].SliderMax = 1f;
		m_OptElements[6].Slider = CFGAudioManager.DbToLinear(CFGAudioManager.Instance.CinematicVolume);
		m_OptElements[6].m_SliderChangedCallback = OnAudioSliderChange;
		m_OptElements[6].m_SliderDownCallback = OnAudioSliderMouseDown;
		m_OptElements[6].SetType(1);
		m_OptElements[6].ToggleEnabled(enabled: true);
		m_Controller_ElementSelected = -1;
		Controller_SelectNext(1);
	}

	private void OnAudioSliderChange(float val, int position_in_list)
	{
		float num = CFGAudioManager.LinearToDb(val);
		if (!(m_VolumeTest == null))
		{
			switch (position_in_list)
			{
			case 0:
				CFGAudioManager.Instance.MasterVolume = num;
				break;
			case 1:
				CFGAudioManager.Instance.MusicVolume = num;
				break;
			case 2:
				CFGAudioManager.Instance.DialogsVolume = num;
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_SpeechTest, CFGAudioManager.Instance.m_MixDialogs);
				break;
			case 3:
				CFGAudioManager.Instance.EnviroVolume = num;
				break;
			case 4:
				CFGAudioManager.Instance.SFXVolume = num;
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_SFXTest, CFGAudioManager.Instance.m_MixSFX);
				break;
			case 5:
				CFGAudioManager.Instance.InterfaceVolume = num;
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_InterfaceTest, CFGAudioManager.Instance.m_MixInterface);
				break;
			case 6:
				CFGAudioManager.Instance.CinematicVolume = num;
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CinematicTest, CFGAudioManager.Instance.m_MixCinematic);
				break;
			}
		}
	}

	private void OnAudioSliderMouseDown(int position_in_list)
	{
		if (!(m_VolumeTest == null))
		{
			switch (position_in_list)
			{
			case 0:
				break;
			case 1:
				break;
			case 2:
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_SpeechTest, CFGAudioManager.Instance.m_MixDialogs);
				break;
			case 3:
				break;
			case 4:
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_SFXTest, CFGAudioManager.Instance.m_MixSFX);
				break;
			case 5:
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_InterfaceTest, CFGAudioManager.Instance.m_MixInterface);
				break;
			case 6:
				m_VolumeTest.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CinematicTest, CFGAudioManager.Instance.m_MixCinematic);
				break;
			}
		}
	}

	private void SaveAudioOptions()
	{
		CFGOptions.Audio.Vol_Master = CFGAudioManager.Instance.MasterVolume;
		CFGOptions.Audio.Vol_Music = CFGAudioManager.Instance.MusicVolume;
		CFGOptions.Audio.Vol_Voice = CFGAudioManager.Instance.DialogsVolume;
		CFGOptions.Audio.Vol_Enviro = CFGAudioManager.Instance.EnviroVolume;
		CFGOptions.Audio.Vol_SFX = CFGAudioManager.Instance.SFXVolume;
		CFGOptions.Audio.Vol_UI = CFGAudioManager.Instance.InterfaceVolume;
		CFGOptions.Audio.Vol_Cinematic = CFGAudioManager.Instance.CinematicVolume;
	}

	private void SetControlsOptions()
	{
		SpawnOptionsElements(5);
		m_OptElements[0].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_panningspeed");
		m_OptElements[0].SliderMin = 0.1f;
		m_OptElements[0].SliderMax = 2f;
		m_OptElements[0].Slider = CFGOptions.Gameplay.CameraPanningSpeed;
		m_OptElements[0].m_SliderChangedCallback = OnControlsSliderChange;
		m_OptElements[0].SetType(1);
		m_OptElements[0].ToggleEnabled(enabled: true);
		m_OptElements[1].m_ApllyVideo.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_keybindings");
		m_OptElements[1].ApplyVideoBtn = OnKeybindings;
		m_OptElements[1].SetType(5);
		m_OptElements[1].ToggleEnabled(enabled: true);
		m_OptElements[2].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_disablecontroller");
		m_OptElements[2].CheckBox = CFGOptions.Gameplay.DisableController;
		m_OptElements[2].m_ToggleCheckBoxCallback = OnControlsCheckBox;
		m_OptElements[2].SetType(0);
		m_OptElements[2].ToggleEnabled(enabled: true);
		m_OptElements[3].Title = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_pad_deadzone");
		m_OptElements[3].SliderMin = 0.1f;
		m_OptElements[3].SliderMax = 0.5f;
		m_OptElements[3].Slider = CFGOptions.Input.GamepadDZ;
		m_OptElements[3].m_SliderChangedCallback = OnControlsSliderChange;
		m_OptElements[3].SetType(1);
		m_OptElements[3].ToggleEnabled(enabled: true);
		m_OptElements[4].m_ApllyVideo.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_padmapping");
		m_OptElements[4].ApplyVideoBtn = OnControllerLayout;
		m_OptElements[4].SetType(5);
		m_OptElements[4].ToggleEnabled(enabled: true);
		m_Controller_ElementSelected = -1;
		Controller_SelectNext(1);
	}

	private void OnControlsSliderChange(float val, int position_in_list)
	{
		Debug.Log("On controls slider change" + val + " " + position_in_list);
		switch (position_in_list)
		{
		case 0:
			CFGOptions.Gameplay.CameraPanningSpeed = val;
			break;
		case 3:
			CFGOptions.Input.GamepadDZ = val;
			break;
		case 1:
		case 2:
			break;
		}
	}

	private void OnControlsCheckBox(bool enabled, int position_in_list)
	{
		Debug.Log("On controls checkbox clicked " + enabled + " " + position_in_list);
		if (position_in_list == 2)
		{
			CFGOptions.Gameplay.DisableController = enabled;
			CFGInput.ExclusiveInputDevice = (enabled ? EInputMode.KeyboardAndMouse : EInputMode.Auto);
		}
	}

	private void OnKeybindings(int a)
	{
		Debug.Log("Keybindings");
		GotoSubMenu(EOptionsSubMenu.Keybindings);
	}

	private void OnControllerLayout(int a)
	{
		Debug.Log("ControllerLayout");
		GotoSubMenu(EOptionsSubMenu.ControllerLayout);
	}

	private void SetKeybindingsOptions()
	{
		CFGTextManager instance = CFGSingletonResourcePrefab<CFGTextManager>.Instance;
		SpawnOptionsElements(m_BindActions.Count);
		for (int i = 0; i < m_BindActions.Count; i++)
		{
			m_OptElements[i].Title = instance.GetLocalizedText("action_" + m_BindActions[i].ActionCommand.ToString().ToLower());
			m_OptElements[i].BindClicked = OnControlsBind;
			m_OptElements[i].AltBindClicked = OnControlsAltBind;
			m_OptElements[i].Controls = m_BindActions[i].ToTextLong_First();
			m_OptElements[i].ControlsAlt = m_BindActions[i].ToTextLong_Second();
			m_OptElements[i].SetType(2);
			m_OptElements[i].ToggleEnabled(enabled: true);
		}
	}

	private void OnControlsBind(int position_in_list)
	{
		if (m_BindingKey != -1)
		{
			return;
		}
		Debug.Log("On controls bind " + position_in_list);
		m_BindingKey = position_in_list;
		m_BindingAlt = false;
		for (int i = 0; i < m_OptElements.Count; i++)
		{
			if (i == m_BindingKey)
			{
				m_OptElements[i].Controls = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_presstobind");
				m_OptElements[i].ControlsSelected = true;
				m_OptElements[i].m_AltBind.enabled = false;
			}
			else
			{
				m_OptElements[i].ToggleEnabled(enabled: false);
			}
		}
		m_Cancel.enabled = false;
		m_Accept.enabled = false;
		m_MainButtons[0].enabled = false;
	}

	private void OnControlsAltBind(int position_in_list)
	{
		if (m_BindingKey != -1)
		{
			return;
		}
		Debug.Log("On controls alt bind " + position_in_list);
		m_BindingKey = position_in_list;
		m_BindingAlt = true;
		for (int i = 0; i < m_OptElements.Count; i++)
		{
			if (i == m_BindingKey)
			{
				m_OptElements[i].ControlsAlt = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("options_presstobind");
				m_OptElements[i].ControlsAltSelected = true;
				m_OptElements[i].m_Bind.enabled = false;
			}
			else
			{
				m_OptElements[i].ToggleEnabled(enabled: false);
			}
		}
		m_Cancel.enabled = false;
		m_Accept.enabled = false;
		m_MainButtons[0].enabled = false;
	}

	private void OnAcceptClick(int a)
	{
		Debug.Log("Accept");
		ApplyGameplayOptions();
		ApplyVideoOptions();
		SaveAudioOptions();
		CFGOptions.Save();
		CFGInput.SaveINI();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadOptions(OnMenuUnloaded_MainMenu);
	}

	private void OnCancelClick(int a)
	{
		Debug.Log("Cancel");
		if (CFGOptions.Gameplay.Language != m_InitLanguage)
		{
			CFGOptions.Gameplay.SelectLanguage(m_InitLanguage, bForce: true);
		}
		CFGOptions.Reset(null);
		CFGOptions.Load();
		CFGOptions.Audio.ApplyToUnity();
		CFGInput.LoadINI();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadOptions(OnMenuUnloaded_MainMenu);
	}

	private void OnMenuUnloaded_MainMenu()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadMainMenus();
	}

	private void SpawnOptionsElements(int number)
	{
		float num = (m_ElementPrefab.GetComponent<RectTransform>().anchorMax.y - m_ElementPrefab.GetComponent<RectTransform>().anchorMin.y) * base.transform.parent.GetComponent<RectTransform>().rect.height;
		RectTransform component = m_OptionsParent.GetComponent<RectTransform>();
		float num2 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		float num3 = 0.11f / num2;
		if (Screen.width < 1600)
		{
			num3 = 0.08f / num2;
		}
		else if (Screen.width > 1920)
		{
			num3 = 0.15f / num2;
		}
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, (!(1f >= num3 * (float)number)) ? (num3 * (float)number) : 1f);
		component.offsetMax = new Vector2(1f, 1f);
		component.offsetMin = new Vector2(0f, 0f);
		foreach (CFGOptElement optElement in m_OptElements)
		{
			UnityEngine.Object.Destroy(optElement.gameObject);
		}
		m_OptElements.Clear();
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_ElementPrefab);
			gameObject.name = "SPAWN item";
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.SetParent(m_OptionsParent.transform, worldPositionStays: false);
			component2.transform.localPosition = new Vector3(0f, 0f, 0f);
			float num4 = 1f - (float)i * (num / m_OptionsParent.GetComponent<RectTransform>().rect.height);
			float y = num4 - num / m_OptionsParent.GetComponent<RectTransform>().rect.height;
			component2.offsetMax = new Vector2(0f, 1f);
			component2.offsetMin = new Vector2(0f, 1f);
			component2.anchorMax = new Vector2(1f, num4);
			component2.anchorMin = new Vector2(0f, y);
			m_OptElements.Add(gameObject.GetComponent<CFGOptElement>());
			gameObject.GetComponent<CFGOptElement>().m_Data = i;
			Image[] componentsInChildren = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image in componentsInChildren)
			{
				if (image.name == "TloPod")
				{
					image.enabled = i % 2 == 1;
				}
			}
		}
		if (m_Scrollbar != null)
		{
			m_Scrollbar.value = 0f;
			m_Scrollbar.value = 1f;
		}
	}

	private void Controller_SelectNext(int Add, int First = -1)
	{
		if (m_OptElements == null || m_OptElements.Count == 0)
		{
			return;
		}
		if (First < 0)
		{
			First = m_Controller_ElementSelected;
		}
		if (m_Controller_ElementSelected < 0)
		{
			Controller_SelectElement(0);
			return;
		}
		int num = First + Add;
		if (num < 0)
		{
			num = m_OptElements.Count - 1;
		}
		if (num >= m_OptElements.Count)
		{
			num = 0;
		}
		int controlType = m_OptElements[num].GetControlType();
		if (!m_OptElements[num].IsActive() || controlType == 4 || controlType == 7)
		{
			Controller_SelectNext(Add, num);
		}
		else
		{
			Controller_SelectElement(num);
		}
	}

	private void Controller_SelectElement(int NewElement)
	{
		if (m_OptElements == null || m_OptElements.Count == 0 || CFGInput.LastReadInputDevice != EInputMode.Gamepad)
		{
			return;
		}
		if (m_Controller_ElementSelected > -1 && m_Controller_ElementSelected < m_OptElements.Count)
		{
			if (m_OptElements[m_Controller_ElementSelected].GetControlType() == 5)
			{
				m_OptElements[m_Controller_ElementSelected].m_ApllyVideo.OnPointerExit(null);
			}
			else
			{
				m_OptElements[m_Controller_ElementSelected].m_Text.color = m_OptElements[m_Controller_ElementSelected].m_NormalTextColor;
			}
		}
		if (m_Controller_ElementSelected >= 0)
		{
			m_OptElements[m_Controller_ElementSelected].OnPointerEnter(null);
		}
		if (m_OptElements[NewElement].GetControlType() == 5)
		{
			m_OptElements[NewElement].OnPointerEnter(null);
			if (m_Controller_ElementSelected >= 0)
			{
				m_OptElements[m_Controller_ElementSelected].OnPointerExit(null);
			}
		}
		else
		{
			if (m_Controller_ElementSelected >= 0)
			{
				m_OptElements[m_Controller_ElementSelected].OnPointerExit(null);
			}
			m_OptElements[NewElement].OnPointerEnter(null);
			if (m_Controller_ElementSelected >= 0 && m_OptElements[m_Controller_ElementSelected].GetControlType() == 5)
			{
				m_OptElements[NewElement].m_ApllyVideo.OnPointerExit(null);
			}
		}
		m_Controller_ElementSelected = NewElement;
	}

	private void Controller_OnMoveList(int item)
	{
		OnMainOptionClick(item);
	}

	private bool Controller_ActivateItem(int action)
	{
		Debug.Log("Activating item: " + m_Controller_ElementSelected);
		if (m_OptElements == null || m_OptElements.Count == 0)
		{
			return false;
		}
		if (m_Controller_ElementSelected < 0 || m_Controller_ElementSelected >= m_OptElements.Count)
		{
			return false;
		}
		CFGOptElement cFGOptElement = m_OptElements[m_Controller_ElementSelected];
		switch (cFGOptElement.GetControlType())
		{
		case 0:
			if (action == 0)
			{
				cFGOptElement.m_CheckBox.SimulateClick();
			}
			return true;
		case 1:
		{
			float num = (cFGOptElement.SliderMax - cFGOptElement.SliderMin) * 0.05f;
			if (action == 0 && m_VolumeTest != null)
			{
				OnAudioSliderMouseDown(m_Controller_ElementSelected);
			}
			switch (action)
			{
			case 1:
				num = 0f - num;
				break;
			default:
				return false;
			case 2:
				break;
			}
			cFGOptElement.m_Slider.value = cFGOptElement.m_Slider.value + num;
			return true;
		}
		case 6:
			switch (action)
			{
			case 3:
				cFGOptElement.OnChooseOptionLeft(0);
				return true;
			case 4:
				cFGOptElement.OnChooseOptionRight(0);
				return true;
			}
			break;
		case 5:
			if (action == 0)
			{
				cFGOptElement.m_ApllyVideo.SimulateClick();
				return true;
			}
			break;
		}
		return false;
	}
}
