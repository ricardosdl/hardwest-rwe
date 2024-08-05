#define USE_ERROR_REPORTING
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGScenarioMenuNew : CFGPanel
{
	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_YButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_LPad;

	public CFGButtonExtension m_AButtonPadP;

	public CFGButtonExtension m_XButtonPadP;

	public CFGButtonExtension m_YButtonPadP;

	public CFGButtonExtension m_BButtonPadP;

	public CFGButtonExtension m_RSButtonPadP;

	public CFGButtonExtension m_LPadP;

	public GameObject m_ButtonsMK;

	public GameObject m_ButtonsMKP;

	public Text m_ScenarioTitle;

	public Text m_ScenarioName;

	public Text m_ScenarioDescription;

	public Text m_ScenarioCompleteRatioName;

	public Text m_ScenarioCompleteRatioVal;

	public CFGImageExtension m_LevelImage;

	public Text m_EasyDiffTxt;

	public Text m_MediumDiffTxt;

	public Text m_HardDiffTxt;

	public Text m_IronmanTxt;

	public Text m_InjuriesTxt;

	public CFGImageExtension m_EasyDiffImg;

	public CFGImageExtension m_MediumDiffImg;

	public CFGImageExtension m_HardDiffImg;

	public CFGImageExtension m_IronmanImg;

	public CFGImageExtension m_InjuriesImg;

	public Text m_UnlockableText1;

	public Text m_UnlockableText2;

	public Text m_UnlockableText3;

	public CFGImageExtension m_UnlockableImg1;

	public CFGImageExtension m_UnlockableImg2;

	public CFGImageExtension m_UnlockableImg3;

	public CFGButtonExtension m_ContinueButton;

	public CFGButtonExtension m_RestartButton;

	public CFGButtonExtension m_CancelButton;

	public List<RectTransform> m_PlacesForScenarios = new List<RectTransform>();

	public List<CFGScenarioElement> m_ScenarioElements = new List<CFGScenarioElement>();

	public GameObject m_ScenarioElement;

	public List<CFGImageExtension> m_Connectors = new List<CFGImageExtension>();

	public ECustomGameplayOption m_CustomOptions;

	public GameObject m_Popup;

	private Animator m_PopupAnimator;

	public CFGButtonExtension m_EasyBtn;

	public CFGButtonExtension m_MediumBtn;

	public CFGButtonExtension m_HardBtn;

	public CFGButtonExtension m_IronmanBtn;

	public CFGButtonExtension m_InjuriesBtn;

	public CFGButtonExtension m_CancelBtn;

	public CFGButtonExtension m_ProceedBtn;

	public CFGButtonExtension m_GamePlusBtn;

	public Text m_PopupDesc;

	public Text m_PopupTitle;

	public Text m_SelectMod;

	public Text m_Inj;

	public Text m_Iron;

	public List<CFGButtonExtension> m_DifficultyList = new List<CFGButtonExtension>();

	private int m_CurrentSelectedScenario = -1;

	protected CFGDef_Campaign m_CampaignDef;

	private float m_LastTimeChange = -1f;

	private EDifficulty m_NewDifficulty = EDifficulty.Normal;

	private int m_DifficultySelected = 1;

	private bool m_PermaDeath;

	private bool m_Injuries;

	private float m_NextLARead;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private string m_MostRecentSave;

	public bool Injuries => m_Injuries;

	protected override void Start()
	{
		base.Start();
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		SpawnElements();
		m_ScenarioElements[6].Selected = true;
		m_CurrentSelectedScenario = 6;
		m_ScenarioTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[6].ScenarioNameID);
		m_ScenarioName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[6].ScenarioNameID);
		m_LevelImage.IconNumber = m_ScenarioElements[6].m_LevelImageB.IconNumber - 1;
		m_ScenarioDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[6].ScenarioNameID + "_desc");
		m_ScenarioCompleteRatioVal.text = m_ScenarioElements[6].m_PercentB.text;
		m_MostRecentSave = CFG_SG_Manager.GetMostRecentSave(m_CampaignDef.CampaignID, m_ScenarioElements[6].ScenarioNameID, bCheckVersions: true);
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_MostRecentSave == null)
		{
			m_ContinueButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_play");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_play").ToUpper();
			m_ContinueButton.m_ButtonClickedCallback = OnButtonPressedPlay;
			m_RestartButton.gameObject.SetActive(value: false);
			m_YButtonPad.gameObject.SetActive(value: false);
		}
		else
		{
			m_ContinueButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue").ToUpper();
			m_ContinueButton.m_ButtonClickedCallback = OnButtonPressedContinue;
			m_RestartButton.gameObject.SetActive(value: true);
			m_YButtonPad.gameObject.SetActive(flag);
		}
		CFGDef_Scenario scenario = m_CampaignDef.GetScenario(6);
		CFGAchievmentTracker.GetScenarioCompletion("camp_01", scenario.Index, out var Completed_Easy, out var Completed_Med, out var Completed_Hard, out var Completed_Ironman, out var Completed_Injuries);
		m_EasyDiffImg.IconNumber = (Completed_Easy ? 1 : 0);
		m_MediumDiffImg.IconNumber = (Completed_Med ? 1 : 0);
		m_HardDiffImg.IconNumber = (Completed_Hard ? 1 : 0);
		m_IronmanImg.IconNumber = (Completed_Ironman ? 1 : 0);
		m_InjuriesImg.IconNumber = (Completed_Injuries ? 1 : 0);
		CFGVar variable = CFGVariableContainer.Instance.GetVariable("trinket_s7_01", "campaign");
		CFGVar variable2 = CFGVariableContainer.Instance.GetVariable("trinket_s7_02", "campaign");
		CFGVar variable3 = CFGVariableContainer.Instance.GetVariable("trinket_s7_03", "campaign");
		if (variable != null && variable2 != null && variable3 != null)
		{
			m_UnlockableImg1.IconNumber = ((variable.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableImg2.IconNumber = ((variable2.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableImg3.IconNumber = ((variable3.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableText1.text = ((!(variable.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable.ValueString + "_name"));
			m_UnlockableText2.text = ((!(variable2.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable2.ValueString + "_name"));
			m_UnlockableText3.text = ((!(variable3.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable3.ValueString + "_name"));
		}
		m_CancelButton.m_ButtonClickedCallback = OnButtonPressedExit;
		m_RestartButton.m_ButtonClickedCallback = OnButtonPressedPlay;
		m_CancelBtn.m_ButtonClickedCallback = OnButtonPressedCancel;
		m_ProceedBtn.m_ButtonClickedCallback = OnButtonPressedProceed;
		m_GamePlusBtn.m_ButtonClickedCallback = OnGamePlusBtnPressed;
		m_DifficultyList.Add(m_EasyBtn);
		m_DifficultyList.Add(m_MediumBtn);
		m_DifficultyList.Add(m_HardBtn);
		m_EasyBtn.m_ButtonClickedCallback = OnButtonDifficultyModeClick;
		m_MediumBtn.m_ButtonClickedCallback = OnButtonDifficultyModeClick;
		m_HardBtn.m_ButtonClickedCallback = OnButtonDifficultyModeClick;
		m_DifficultyList[m_DifficultySelected].IsSelected = true;
		m_InjuriesBtn.IsSelected = m_Injuries;
		m_IronmanBtn.IsSelected = m_PermaDeath;
		m_InjuriesBtn.m_ButtonClickedCallback = OnInjuriesClick;
		m_IronmanBtn.m_ButtonClickedCallback = OnPermaDeathClick;
		m_EasyBtn.m_ButtonOverCallback = OnEasyHover;
		m_MediumBtn.m_ButtonOverCallback = OnMediumHover;
		m_HardBtn.m_ButtonOverCallback = OnHardHover;
		m_InjuriesBtn.m_ButtonOverCallback = OnInjuriesHover;
		m_IronmanBtn.m_ButtonOverCallback = OnIronmanHover;
		m_PopupAnimator = m_Popup.GetComponent<Animator>();
		m_AButtonPad.gameObject.SetActive(flag);
		m_YButtonPadP.gameObject.SetActive(flag);
		m_BButtonPad.gameObject.SetActive(flag);
		m_LPad.gameObject.SetActive(flag);
		m_LPad.gameObject.SetActive(flag);
		m_AButtonPadP.gameObject.SetActive(flag);
		m_XButtonPadP.gameObject.SetActive(flag);
		m_BButtonPadP.gameObject.SetActive(flag);
		m_RSButtonPadP.gameObject.SetActive(flag);
		m_LPadP.gameObject.SetActive(flag);
		m_ButtonsMK.gameObject.SetActive(!flag);
		m_ButtonsMKP.gameObject.SetActive(!flag);
		m_GamePlusBtn.gameObject.SetActive(!flag);
		if (m_DifficultySelected == 0)
		{
			OnEasyHover(0);
		}
		else if (m_DifficultySelected == 1)
		{
			OnMediumHover(1);
		}
		else
		{
			OnHardHover(2);
		}
	}

	public override void Update()
	{
		base.Update();
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.U))
		{
			UnlockAllScenarios();
		}
		CFGCheats.Check_Cheats(this);
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
			m_AButtonPad.gameObject.SetActive(flag);
			m_YButtonPad.gameObject.SetActive(flag && m_MostRecentSave != null);
			m_BButtonPad.gameObject.SetActive(flag);
			m_LPad.gameObject.SetActive(flag);
			m_LPad.gameObject.SetActive(flag);
			m_AButtonPadP.gameObject.SetActive(flag);
			m_XButtonPadP.gameObject.SetActive(flag);
			m_YButtonPadP.gameObject.SetActive(flag);
			m_BButtonPadP.gameObject.SetActive(flag);
			m_RSButtonPadP.gameObject.SetActive(flag);
			m_LPadP.gameObject.SetActive(flag);
			m_ButtonsMK.gameObject.SetActive(!flag);
			m_ButtonsMKP.gameObject.SetActive(!flag);
			m_GamePlusBtn.gameObject.SetActive(!flag);
			if (m_MostRecentSave != null)
			{
				m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue").ToUpper();
			}
			else
			{
				m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_play").ToUpper();
			}
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGSingleton<CFGWindowMgr>.Instance.m_GamePlus != null)
		{
			return;
		}
		if (CFGInput.IsActivated(EActionCommand.Confirm))
		{
			if (m_ContinueButton.gameObject.activeSelf && !m_Popup.activeSelf)
			{
				m_ContinueButton.m_ButtonClickedCallback(0);
				m_AButtonPad.SimulateClickGraphicAndSoundOnly();
				return;
			}
			if (m_ProceedBtn.gameObject.activeSelf && m_Popup.activeSelf)
			{
				m_ProceedBtn.m_ButtonClickedCallback(0);
				m_AButtonPadP.SimulateClickGraphicAndSoundOnly();
				return;
			}
		}
		if (CFGInput.IsActivated(EActionCommand.Exit))
		{
			if (!m_Popup.activeSelf)
			{
				m_CancelButton.m_ButtonClickedCallback(0);
				m_BButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			else
			{
				m_CancelBtn.m_ButtonClickedCallback(0);
				m_BButtonPadP.SimulateClickGraphicAndSoundOnly();
			}
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f && m_ContinueButton.gameObject.activeSelf && !m_Popup.activeSelf)
		{
			m_ContinueButton.m_ButtonClickedCallback(0);
			m_AButtonPad.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f && m_RestartButton.gameObject.activeSelf && !m_Popup.activeSelf)
		{
			m_RestartButton.m_ButtonClickedCallback(0);
			m_YButtonPad.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f && m_CancelButton.gameObject.activeSelf && !m_Popup.activeSelf)
		{
			m_CancelButton.m_ButtonClickedCallback(0);
			m_BButtonPad.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f && m_ProceedBtn.gameObject.activeSelf && m_Popup.activeSelf)
		{
			m_ProceedBtn.m_ButtonClickedCallback(0);
			m_AButtonPadP.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f && m_CancelBtn.gameObject.activeSelf && m_Popup.activeSelf)
		{
			m_CancelBtn.m_ButtonClickedCallback(0);
			m_BButtonPadP.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f && m_Popup.activeSelf)
		{
			m_InjuriesBtn.m_ButtonClickedCallback(0);
			m_XButtonPadP.SimulateClickGraphicAndSoundOnly();
			m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_ironman");
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RA_Button) > 0.5f && m_Popup.activeSelf)
		{
			m_GamePlusBtn.m_ButtonClickedCallback(0);
			m_RSButtonPadP.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f && m_Popup.activeSelf)
		{
			m_IronmanBtn.m_ButtonClickedCallback(0);
			m_YButtonPadP.SimulateClickGraphicAndSoundOnly();
			m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_injuries");
			return;
		}
		if (m_Popup.activeSelf)
		{
			bool flag2 = CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left) > 0f;
			bool flag3 = CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right) > 0f;
			if (Time.time > m_NextLARead)
			{
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Right) > 0.5f)
				{
					m_NextLARead = Time.time + 0.3f;
					flag3 = true;
				}
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Left) > 0.5f)
				{
					m_NextLARead = Time.time + 0.3f;
					flag2 = true;
				}
			}
			if (flag3)
			{
				if (m_DifficultySelected > 1)
				{
					m_DifficultyList[0].m_ButtonClickedCallback(0);
				}
				else
				{
					m_DifficultyList[m_DifficultySelected + 1].m_ButtonClickedCallback(m_DifficultySelected + 1);
				}
				string text = string.Empty;
				if (m_EasyBtn.IsSelected)
				{
					text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_easy");
				}
				else if (m_MediumBtn.IsSelected)
				{
					text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_medium");
				}
				else if (m_HardBtn.IsSelected)
				{
					text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_hard");
				}
				m_PopupDesc.text = text;
				if (m_LastInput == EInputMode.Gamepad)
				{
					if (m_DifficultySelected == 0)
					{
						CFGAudioManager.Instance.PlaySound2D(m_EasyBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
					else if (m_DifficultySelected == 1)
					{
						CFGAudioManager.Instance.PlaySound2D(m_MediumBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
					else if (m_DifficultySelected == 2)
					{
						CFGAudioManager.Instance.PlaySound2D(m_HardBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
				}
				m_LPadP.SimulateClickGraphicAndSoundOnly();
				return;
			}
			if (flag2)
			{
				if (m_DifficultySelected < 1)
				{
					m_DifficultyList[2].m_ButtonClickedCallback(2);
				}
				else
				{
					m_DifficultyList[m_DifficultySelected - 1].m_ButtonClickedCallback(m_DifficultySelected - 1);
				}
				string text2 = string.Empty;
				if (m_EasyBtn.IsSelected)
				{
					text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_easy");
				}
				else if (m_MediumBtn.IsSelected)
				{
					text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_medium");
				}
				else if (m_HardBtn.IsSelected)
				{
					text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_hard");
				}
				m_PopupDesc.text = text2;
				if (m_LastInput == EInputMode.Gamepad)
				{
					if (m_DifficultySelected == 0)
					{
						CFGAudioManager.Instance.PlaySound2D(m_EasyBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
					else if (m_DifficultySelected == 1)
					{
						CFGAudioManager.Instance.PlaySound2D(m_MediumBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
					else if (m_DifficultySelected == 2)
					{
						CFGAudioManager.Instance.PlaySound2D(m_HardBtn.m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
					}
				}
				m_LPadP.SimulateClickGraphicAndSoundOnly();
				return;
			}
		}
		bool flag4 = false;
		if (m_Popup.activeSelf)
		{
			return;
		}
		float num = CFGJoyManager.ReadAsButton(EJoyButton.LA_Top);
		float num2 = CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom);
		float num3 = CFGJoyManager.ReadAsButton(EJoyButton.LA_Right);
		float num4 = CFGJoyManager.ReadAsButton(EJoyButton.LA_Left);
		float num5 = num - num2;
		float num6 = num3 - num4;
		float num7 = Mathf.Abs(num5);
		float num8 = Mathf.Abs(num6);
		if (num7 < 0.5f)
		{
			num5 = 0f;
		}
		if (num8 < 0.5f)
		{
			num6 = 0f;
		}
		if (num5 == 0f && num6 == 0f)
		{
			return;
		}
		int num9 = -1;
		if (!flag4)
		{
			switch (m_CurrentSelectedScenario)
			{
			default:
				num9 = -1;
				break;
			case 0:
				if (num5 < 0f)
				{
					num9 = 1;
				}
				else if (num6 > 0f)
				{
					num9 = 2;
				}
				else if (num6 < 0f)
				{
					num9 = 6;
				}
				break;
			case 1:
				if (num5 > 0f)
				{
					num9 = 0;
				}
				else if (num6 > 0f)
				{
					num9 = 7;
				}
				else if (num6 < 0f)
				{
					num9 = 4;
				}
				break;
			case 2:
				if (num5 < 0f)
				{
					num9 = 7;
				}
				else if (num6 > 0f)
				{
					num9 = 3;
				}
				else if (num6 < 0f)
				{
					num9 = 0;
				}
				break;
			case 3:
				if (num5 < 0f)
				{
					num9 = 7;
				}
				else if (num6 < 0f)
				{
					num9 = 2;
				}
				break;
			case 4:
				if (num5 > 0f)
				{
					num9 = 6;
				}
				else if (num6 > 0f)
				{
					num9 = 1;
				}
				break;
			case 5:
				if (num5 < 0f)
				{
					num9 = 4;
				}
				else if (num6 > 0f)
				{
					num9 = 6;
				}
				break;
			case 6:
				if (num5 < 0f)
				{
					num9 = 4;
				}
				else if (num6 > 0f)
				{
					num9 = 0;
				}
				else if (num6 < 0f)
				{
					num9 = 5;
				}
				break;
			case 7:
				if (num5 > 0f)
				{
					num9 = 2;
				}
				else if (num6 < 0f)
				{
					num9 = 1;
				}
				break;
			}
		}
		else
		{
			switch (m_CurrentSelectedScenario)
			{
			default:
				num9 = 0;
				break;
			case 0:
				if (num6 > 0f || num6 < 0f)
				{
					num9 = 6;
				}
				else if (num5 < 0f)
				{
					num9 = 7;
				}
				break;
			case 6:
				if (num6 > 0f || num6 < 0f)
				{
					num9 = 0;
				}
				else if (num5 < 0f)
				{
					num9 = 7;
				}
				break;
			case 7:
				if (num5 < 0f)
				{
					num9 = 0;
				}
				else if (num5 > 0f)
				{
					num9 = 0;
				}
				break;
			}
		}
		if (num9 > -1 && m_LastTimeChange + 0.5f < Time.time)
		{
			if (m_ScenarioElements[num9].Unlocked)
			{
				OnScenarioButtonClick(num9);
				m_LPad.SimulateClickGraphicAndSoundOnly();
			}
			m_LastTimeChange = Time.time;
		}
	}

	public override void SetLocalisation()
	{
		m_EasyDiffTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_easy");
		m_MediumDiffTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_medium");
		m_HardDiffTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_hard");
		m_IronmanTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_ironman");
		m_InjuriesTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_injuries");
		m_ScenarioCompleteRatioName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_completed_ratio");
		m_RestartButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_restart");
		m_CancelButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_cancel");
		m_ContinueButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue");
		m_PopupDesc.text = string.Empty;
		m_PopupTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_title");
		m_SelectMod.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_select_mod");
		m_Inj.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_injuries");
		m_Iron.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_ironman");
		m_ProceedBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_proceed");
		m_CancelBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_cancel");
		m_GamePlusBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more");
		m_GamePlusBtn.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_more");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue").ToUpper();
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_cancel").ToUpper();
		m_YButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_restart").ToUpper();
		m_LPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_select");
		m_AButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_confirm");
		m_BButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_close");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_proceed").ToUpper();
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_cancel").ToUpper();
		m_RSButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more").ToUpper();
	}

	public void OnEasyHover(int a)
	{
		m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_easy");
	}

	public void OnMediumHover(int a)
	{
		m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_medium");
	}

	public void OnHardHover(int a)
	{
		m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_hard");
	}

	public void OnIronmanHover(int a)
	{
		m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_ironman");
	}

	public void OnInjuriesHover(int a)
	{
		m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_injuries");
	}

	public void SetCampaignID(int id)
	{
		m_CampaignDef = null;
		m_CampaignDef = CFGStaticDataContainer.GetCampaign(id);
		if (m_CampaignDef == null)
		{
			CFGError.ReportError("Failed to find campaign #" + id, CFGError.ErrorCode.Fail);
			return;
		}
		CFGSessionSingle.ReadCampaignVariables("camp_0" + id);
		CFGVariableContainer.Instance.LoadValuesGlobal("camp_0" + id, bCampaign: true, bProfile: false);
	}

	public void SpawnElements()
	{
		bool flag = false;
		for (int i = 0; i < m_PlacesForScenarios.Count; i++)
		{
			bool flag2 = false;
			CFGScenarioElement cFGScenarioElement = null;
			if (m_CampaignDef != null)
			{
				CFGDef_Scenario scenario = m_CampaignDef.GetScenario(i);
				if (scenario != null)
				{
					GameObject gameObject = Object.Instantiate(m_ScenarioElement);
					float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
					RectTransform component = gameObject.GetComponent<RectTransform>();
					if ((bool)component)
					{
						component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
						component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
					}
					gameObject.transform.position = m_PlacesForScenarios[i].transform.position;
					gameObject.transform.SetParent(m_PlacesForScenarios[i].transform);
					cFGScenarioElement = gameObject.GetComponent<CFGScenarioElement>();
					cFGScenarioElement.m_Star2B.gameObject.SetActive(value: false);
					cFGScenarioElement.m_Star2S.gameObject.SetActive(value: false);
					cFGScenarioElement.m_Star3B.gameObject.SetActive(value: true);
					cFGScenarioElement.m_Star3S.gameObject.SetActive(value: true);
					switch (i)
					{
					case 5:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[0]);
						cFGScenarioElement.m_Connectors.Add(m_Connectors[1]);
						break;
					case 6:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[2]);
						cFGScenarioElement.m_Connectors.Add(m_Connectors[3]);
						break;
					case 0:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[4]);
						break;
					case 2:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[5]);
						break;
					case 4:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[6]);
						break;
					case 1:
						cFGScenarioElement.m_Connectors.Add(m_Connectors[7]);
						break;
					}
					CFGAchievmentTracker.GetScenarioCompletion("camp_01", scenario.Index, out var Completed_Easy, out var Completed_Med, out var Completed_Hard, out var Completed_Ironman, out var Completed_Injuries);
					cFGScenarioElement.m_EasyModeS.IconNumber = (Completed_Easy ? 1 : 0);
					cFGScenarioElement.m_MediumModeS.IconNumber = (Completed_Med ? 1 : 0);
					cFGScenarioElement.m_HardModeS.IconNumber = (Completed_Hard ? 1 : 0);
					cFGScenarioElement.m_IronmanS.IconNumber = (Completed_Ironman ? 1 : 0);
					cFGScenarioElement.m_InjuriesS.IconNumber = (Completed_Injuries ? 1 : 0);
					cFGScenarioElement.m_EasyModeB.IconNumber = (Completed_Easy ? 1 : 0);
					cFGScenarioElement.m_MediumModeB.IconNumber = (Completed_Med ? 1 : 0);
					cFGScenarioElement.m_HardModeB.IconNumber = (Completed_Hard ? 1 : 0);
					cFGScenarioElement.m_IronmanB.IconNumber = (Completed_Ironman ? 1 : 0);
					cFGScenarioElement.m_InjuriesB.IconNumber = (Completed_Injuries ? 1 : 0);
					CFGVar variable = CFGVariableContainer.Instance.GetVariable("trinket_s" + (i + 1) + "_01", "campaign");
					CFGVar variable2 = CFGVariableContainer.Instance.GetVariable("trinket_s" + (i + 1) + "_02", "campaign");
					CFGVar variable3 = CFGVariableContainer.Instance.GetVariable("trinket_s" + (i + 1) + "_03", "campaign");
					if (variable != null && variable2 != null && variable3 != null)
					{
						cFGScenarioElement.m_UnlockableImg1aB.IconNumber = ((variable.ValueString != string.Empty) ? 1 : 0);
						cFGScenarioElement.m_UnlockableImg2aB.IconNumber = ((variable2.ValueString != string.Empty) ? 1 : 0);
						cFGScenarioElement.m_UnlockableImg3aB.IconNumber = ((variable3.ValueString != string.Empty) ? 1 : 0);
						cFGScenarioElement.m_UnlockableImg1aS.IconNumber = ((variable.ValueString != string.Empty) ? 1 : 0);
						cFGScenarioElement.m_UnlockableImg2aS.IconNumber = ((variable2.ValueString != string.Empty) ? 1 : 0);
						cFGScenarioElement.m_UnlockableImg3aS.IconNumber = ((variable3.ValueString != string.Empty) ? 1 : 0);
					}
					cFGScenarioElement.Completed = true;
					cFGScenarioElement.Unlocked = !flag || i == 0 || i == 6 || i == 7;
					cFGScenarioElement.ScenarioId = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(scenario.ScenarioID);
					cFGScenarioElement.ScenarioNameID = scenario.ScenarioID;
					cFGScenarioElement.CompletedPercent = 0;
					cFGScenarioElement.m_LevelImageB.IconNumber = scenario.ImageID;
					cFGScenarioElement.m_LevelImageS.IconNumber = scenario.ImageID;
					cFGScenarioElement.Selected = false;
					cFGScenarioElement.m_Button.m_Data = i;
					cFGScenarioElement.m_Button.m_ButtonClickedCallback = OnScenarioButtonClick;
					m_ScenarioElements.Add(cFGScenarioElement);
					CFGVar variable4 = CFGVariableContainer.Instance.GetVariable(CFGSessionSingle.GenerateVariable(m_CampaignDef.CampaignID, scenario.ScenarioID, "progress"), "profile");
					if (variable4 != null)
					{
						cFGScenarioElement.CompletedPercent = (int)variable4.Value;
					}
					variable4 = CFGVariableContainer.Instance.GetVariable(CFGSessionSingle.GenerateVariable(m_CampaignDef.CampaignID, scenario.ScenarioID, "completed"), "profile");
					if (variable4 != null)
					{
						cFGScenarioElement.Completed = (bool)variable4.Value;
					}
					if (scenario.PrerequisiteScenarioID != null && scenario.PrerequisiteScenarioID.Count > 0 && !flag)
					{
						foreach (string item in scenario.PrerequisiteScenarioID)
						{
							variable4 = CFGVariableContainer.Instance.GetVariable(CFGSessionSingle.GenerateVariable(m_CampaignDef.CampaignID, item, "completed"), "profile");
							if (variable4 == null)
							{
								cFGScenarioElement.Unlocked = false;
								break;
							}
							if (!(bool)variable4.Value)
							{
								cFGScenarioElement.Unlocked = false;
								break;
							}
						}
					}
					flag2 = true;
				}
			}
			if (!flag2 && cFGScenarioElement != null && !flag)
			{
				cFGScenarioElement.Completed = true;
				cFGScenarioElement.Unlocked = true;
				cFGScenarioElement.Selected = false;
				cFGScenarioElement.ScenarioId = "(CODE) Scenario #" + (i + 1);
			}
		}
	}

	public void OnScenarioButtonClick(int nr)
	{
		m_ScenarioElements[m_CurrentSelectedScenario].Selected = false;
		m_ScenarioElements[nr].Selected = true;
		m_CurrentSelectedScenario = nr;
		m_ScenarioTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[nr].ScenarioNameID);
		m_ScenarioName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[nr].ScenarioNameID);
		m_LevelImage.IconNumber = m_ScenarioElements[nr].m_LevelImageB.IconNumber - 1;
		if (m_LevelImage.IconNumber == -1)
		{
			m_LevelImage.IconNumber = 7;
		}
		m_ScenarioDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ScenarioElements[nr].ScenarioNameID + "_desc");
		m_ScenarioCompleteRatioVal.text = m_ScenarioElements[m_CurrentSelectedScenario].m_PercentB.text;
		bool active = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_MostRecentSave = CFG_SG_Manager.GetMostRecentSave(m_CampaignDef.CampaignID, m_ScenarioElements[nr].ScenarioNameID, bCheckVersions: true);
		if (m_MostRecentSave == null)
		{
			m_ContinueButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_play");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_play").ToUpper();
			m_ContinueButton.m_ButtonClickedCallback = OnButtonPressedPlay;
			m_RestartButton.gameObject.SetActive(value: false);
			m_YButtonPad.gameObject.SetActive(value: false);
		}
		else
		{
			m_ContinueButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue");
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_continue").ToUpper();
			m_ContinueButton.m_ButtonClickedCallback = OnButtonPressedContinue;
			m_RestartButton.gameObject.SetActive(value: true);
			m_YButtonPad.gameObject.SetActive(active);
		}
		CFGDef_Scenario scenario = m_CampaignDef.GetScenario(nr);
		CFGAchievmentTracker.GetScenarioCompletion("camp_01", scenario.Index, out var Completed_Easy, out var Completed_Med, out var Completed_Hard, out var Completed_Ironman, out var Completed_Injuries);
		m_EasyDiffImg.IconNumber = (Completed_Easy ? 1 : 0);
		m_MediumDiffImg.IconNumber = (Completed_Med ? 1 : 0);
		m_HardDiffImg.IconNumber = (Completed_Hard ? 1 : 0);
		m_IronmanImg.IconNumber = (Completed_Ironman ? 1 : 0);
		m_InjuriesImg.IconNumber = (Completed_Injuries ? 1 : 0);
		CFGVar variable = CFGVariableContainer.Instance.GetVariable("trinket_s" + (nr + 1) + "_01", "campaign");
		CFGVar variable2 = CFGVariableContainer.Instance.GetVariable("trinket_s" + (nr + 1) + "_02", "campaign");
		CFGVar variable3 = CFGVariableContainer.Instance.GetVariable("trinket_s" + (nr + 1) + "_03", "campaign");
		if (variable != null && variable2 != null && variable3 != null)
		{
			m_UnlockableImg1.IconNumber = ((variable.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableImg2.IconNumber = ((variable2.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableImg3.IconNumber = ((variable3.ValueString != string.Empty) ? 1 : 0);
			m_UnlockableText1.text = ((!(variable.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable.ValueString + "_name"));
			m_UnlockableText2.text = ((!(variable2.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable2.ValueString + "_name"));
			m_UnlockableText3.text = ((!(variable3.ValueString != string.Empty)) ? string.Empty : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(variable3.ValueString + "_name"));
		}
	}

	private void OnButtonPressedExit(int a)
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadScenarioMenu(OnMenuUnloaded_MainMenu);
	}

	private void OnMenuUnloaded_MainMenu()
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadMainMenus();
	}

	private void OnButtonPressedContinue(int a)
	{
		Debug.Log("OnButtonPressedContinue: " + a);
		if (m_CampaignDef != null && m_CurrentSelectedScenario < m_ScenarioElements.Count && m_CurrentSelectedScenario >= 0)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadScenarioMenu(OnMenuUnloaded_ContinueScenario);
		}
	}

	private void OnMenuUnloaded_ContinueScenario()
	{
		CFG_SG_Manager.ContinueScenario(m_CampaignDef.CampaignID, m_ScenarioElements[m_CurrentSelectedScenario].ScenarioNameID);
	}

	private void OnButtonPressedPlay(int a)
	{
		m_Popup.gameObject.SetActive(value: true);
		if (m_PopupAnimator != null)
		{
			m_PopupAnimator.SetTrigger("Otwarcie");
		}
		if (m_DifficultySelected == 0)
		{
			OnEasyHover(0);
		}
		else if (m_DifficultySelected == 1)
		{
			OnMediumHover(1);
		}
		else
		{
			OnHardHover(2);
		}
	}

	private void OnButtonPressedCancel(int a)
	{
		if (m_PopupAnimator != null)
		{
			m_PopupAnimator.SetTrigger("Zamkniecie");
		}
		Invoke("PopupCloseDelayed", 1f);
	}

	private void PopupCloseDelayed()
	{
		if (m_Popup != null)
		{
			m_Popup.gameObject.SetActive(value: false);
		}
	}

	private void OnButtonPressedProceed(int a)
	{
		if (m_PopupAnimator != null)
		{
			m_PopupAnimator.SetTrigger("Zamkniecie");
		}
		Invoke("PopupCloseDelayed", 1f);
		if (m_CurrentSelectedScenario >= 0 && m_CampaignDef != null && m_CampaignDef.ScenarioList != null && m_CampaignDef.ScenarioList.Count > m_CurrentSelectedScenario)
		{
			CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.CreateSessionSingle();
			if (!cFGSessionSingle.SelectCampaign(m_CampaignDef.CampaignID))
			{
				CFGError.ReportError("Failed to start campaign : " + m_CampaignDef.CampaignID, CFGError.ErrorCode.Fail);
				return;
			}
			if (m_ScenarioElements[m_CurrentSelectedScenario].ScenarioNameID == null)
			{
				CFGError.ReportError("Selected scenario has no id!", CFGError.ErrorCode.Fail);
				return;
			}
			if (!cFGSessionSingle.StartScenario(m_ScenarioElements[m_CurrentSelectedScenario].ScenarioNameID))
			{
				CFGError.ReportError("Failed to start scenario has no id!", CFGError.ErrorCode.Fail);
				return;
			}
			cFGSessionSingle.ResetMissionStats();
			CFGSessionSingle.SetScenarioProgress(m_CampaignDef.CampaignID, m_ScenarioElements[m_CurrentSelectedScenario].ScenarioNameID, 0);
			CFGGame.Permadeath = m_PermaDeath;
			CFGGame.InjuriesEnabled = m_Injuries;
			CFGGame.NewGamePlusFlags = (int)m_CustomOptions;
			cFGSessionSingle.OnNewGame(m_NewDifficulty);
			cFGSessionSingle.ReadScenarioVariableDefinitions();
			CFGSingleton<CFGWindowMgr>.Instance.UnloadScenarioMenu(OnMenuUnloaded_LoadLevel);
		}
	}

	private void OnMenuUnloaded_LoadLevel()
	{
		CFGSingleton<CFGGame>.Instance.GoToStoryMovie();
	}

	private void OnButtonDifficultyModeClick(int nr)
	{
		m_DifficultyList[m_DifficultySelected].IsSelected = false;
		m_DifficultyList[nr].IsSelected = true;
		m_DifficultySelected = nr;
		m_NewDifficulty = nr switch
		{
			0 => EDifficulty.Easy, 
			1 => EDifficulty.Normal, 
			_ => EDifficulty.Hard, 
		};
	}

	private void OnPermaDeathClick(int nr)
	{
		m_PermaDeath = !m_PermaDeath;
		m_IronmanBtn.IsSelected = m_PermaDeath;
	}

	private void OnInjuriesClick(int nr)
	{
		m_Injuries = !m_Injuries;
		m_InjuriesBtn.IsSelected = m_Injuries;
	}

	private void OnGamePlusBtnPressed(int nr)
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadGamePlusPanel();
		CFGGamePlusPanel gamePlus = CFGSingleton<CFGWindowMgr>.Instance.m_GamePlus;
		if (!(gamePlus == null))
		{
			gamePlus.m_TempOption = m_CustomOptions;
		}
	}

	public void UnlockAllScenarios()
	{
		Debug.Log("Cheat: Unlocking all scenario buttons");
		for (int i = 0; i < m_ScenarioElements.Count; i++)
		{
			if (m_ScenarioElements[i] != null)
			{
				m_ScenarioElements[i].Unlocked = true;
			}
		}
	}
}
