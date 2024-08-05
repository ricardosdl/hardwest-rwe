#define USE_ERROR_REPORTING
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGScenarioDifficulyPanel : CFGPanel
{
	public delegate void OnStartClickedDelegate();

	[SerializeField]
	private bool m_GamePlus;

	[SerializeField]
	private CFGButtonExtension m_AButtonPadP;

	[SerializeField]
	private CFGButtonExtension m_XButtonPadP;

	[SerializeField]
	private CFGButtonExtension m_YButtonPadP;

	[SerializeField]
	private CFGButtonExtension m_BButtonPadP;

	[SerializeField]
	private CFGButtonExtension m_RSButtonPadP;

	[SerializeField]
	private CFGButtonExtension m_LPadP;

	[SerializeField]
	private GameObject m_ButtonsMKP;

	[SerializeField]
	private GameObject m_Popup;

	[SerializeField]
	private Animator m_PopupAnimator;

	[SerializeField]
	private CFGButtonExtension m_EasyBtn;

	[SerializeField]
	private CFGButtonExtension m_MediumBtn;

	[SerializeField]
	private CFGButtonExtension m_HardBtn;

	[SerializeField]
	private CFGButtonExtension m_IronmanBtn;

	[SerializeField]
	private CFGButtonExtension m_InjuriesBtn;

	[SerializeField]
	private CFGButtonExtension m_CancelBtn;

	[SerializeField]
	private CFGButtonExtension m_ProceedBtn;

	[SerializeField]
	private CFGButtonExtension m_GamePlusBtn;

	[SerializeField]
	private Text m_PopupDesc;

	[SerializeField]
	private Text m_PopupTitle;

	[SerializeField]
	private Text m_SelectMod;

	[SerializeField]
	private Text m_Inj;

	[SerializeField]
	private Text m_Iron;

	[SerializeField]
	private List<CFGButtonExtension> m_DifficultyList = new List<CFGButtonExtension>();

	private float m_LastTimeChange = -1f;

	private EDifficulty m_NewDifficulty = EDifficulty.Normal;

	public ECustomGameplayOption m_CustomOptions;

	private int m_DifficultySelected = 1;

	private bool m_PermaDeath;

	private bool m_Injuries;

	private float m_NextLARead;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private OnStartClickedDelegate m_OnStartClickCallback;

	private string m_CampaignID;

	private string m_ScenarioID;

	public bool Injuries => m_Injuries;

	public void SetOnStartClickCallback(OnStartClickedDelegate _Callback, string CampaignID, string ScenarioID)
	{
		m_OnStartClickCallback = _Callback;
		m_CampaignID = CampaignID;
		m_ScenarioID = ScenarioID;
	}

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
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
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
		m_YButtonPadP.gameObject.SetActive(flag);
		m_AButtonPadP.gameObject.SetActive(flag);
		m_XButtonPadP.gameObject.SetActive(flag);
		m_BButtonPadP.gameObject.SetActive(flag);
		m_RSButtonPadP.gameObject.SetActive(flag && m_GamePlus);
		m_LPadP.gameObject.SetActive(flag);
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
		if (CFGSingleton<CFGWindowMgr>.Instance.m_GamePlus != null)
		{
			return;
		}
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
			m_AButtonPadP.gameObject.SetActive(flag);
			m_XButtonPadP.gameObject.SetActive(flag);
			m_YButtonPadP.gameObject.SetActive(flag);
			m_BButtonPadP.gameObject.SetActive(flag);
			m_RSButtonPadP.gameObject.SetActive(flag && m_GamePlus);
			m_LPadP.gameObject.SetActive(flag);
			m_ButtonsMKP.gameObject.SetActive(!flag);
			m_GamePlusBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGInput.IsActivated(EActionCommand.Confirm) && m_ProceedBtn.gameObject.activeSelf && m_Popup.activeSelf)
		{
			m_ProceedBtn.m_ButtonClickedCallback(0);
			m_AButtonPadP.SimulateClickGraphicAndSoundOnly();
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RA_Button) > 0.5f && m_GamePlus)
		{
			m_GamePlusBtn.m_ButtonClickedCallback(0);
			m_RSButtonPadP.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGInput.IsActivated(EActionCommand.Exit))
		{
			if (m_Popup.activeSelf)
			{
				m_CancelBtn.m_ButtonClickedCallback(0);
				m_BButtonPadP.SimulateClickGraphicAndSoundOnly();
			}
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f && m_ProceedBtn.gameObject.activeSelf && m_Popup.activeSelf)
		{
			m_ProceedBtn.m_ButtonClickedCallback(0);
			m_AButtonPadP.SimulateClickGraphicAndSoundOnly();
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f && m_CancelBtn.gameObject.activeSelf && m_Popup.activeSelf)
		{
			m_CancelBtn.m_ButtonClickedCallback(0);
			m_BButtonPadP.SimulateClickGraphicAndSoundOnly();
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f && m_Popup.activeSelf)
		{
			m_InjuriesBtn.m_ButtonClickedCallback(0);
			m_XButtonPadP.SimulateClickGraphicAndSoundOnly();
			m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_ironman");
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f && m_Popup.activeSelf)
		{
			m_IronmanBtn.m_ButtonClickedCallback(0);
			m_YButtonPadP.SimulateClickGraphicAndSoundOnly();
			m_PopupDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_injuries");
		}
		else
		{
			if (!m_Popup.activeSelf)
			{
				return;
			}
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
			}
			else
			{
				if (!flag2)
				{
					return;
				}
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
			}
		}
	}

	public override void SetLocalisation()
	{
		m_PopupDesc.text = string.Empty;
		m_PopupTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_title");
		m_SelectMod.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_select_mod");
		m_Inj.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_injuries");
		m_Iron.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_ironman");
		m_ProceedBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_proceed");
		m_CancelBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_cancel");
		m_GamePlusBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more");
		m_GamePlusBtn.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_popup_desc_more");
		m_AButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_confirm");
		m_BButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_close");
		m_RSButtonPadP.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more").ToUpper();
	}

	private void OnButtonPressedCancel(int a)
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadScenarioDifficulty();
	}

	private void OnGamePlusBtnPressed(int a)
	{
		CFGSingleton<CFGWindowMgr>.Instance.LoadGamePlusPanel();
		CFGGamePlusPanel gamePlus = CFGSingleton<CFGWindowMgr>.Instance.m_GamePlus;
		if (!(gamePlus == null))
		{
			gamePlus.m_TempOption = m_CustomOptions;
		}
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
		if (m_OnStartClickCallback != null)
		{
			m_OnStartClickCallback();
		}
		if (StartGame())
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus(OnMenuUnloaded_LoadLevel);
		}
		CFGSingleton<CFGWindowMgr>.Instance.UnloadScenarioDifficulty();
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

	private bool StartGame()
	{
		if (string.IsNullOrEmpty(m_CampaignID) || string.IsNullOrEmpty(m_ScenarioID))
		{
			return false;
		}
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.CreateSessionSingle();
		if (cFGSessionSingle == null)
		{
			Debug.LogErrorFormat("Failed to create session");
			return false;
		}
		if (!cFGSessionSingle.SelectCampaign(m_CampaignID))
		{
			CFGError.ReportError("Failed to start campaign : " + m_CampaignID, CFGError.ErrorCode.Fail);
			return false;
		}
		if (!cFGSessionSingle.StartScenario(m_ScenarioID))
		{
			CFGError.ReportError("Failed to start scenario has no id!", CFGError.ErrorCode.Fail);
			return false;
		}
		cFGSessionSingle.ResetMissionStats();
		CFGSessionSingle.SetScenarioProgress(m_CampaignID, m_ScenarioID, 0);
		CFGGame.Permadeath = m_PermaDeath;
		CFGGame.InjuriesEnabled = m_Injuries;
		CFGGame.NewGamePlusFlags = 0;
		cFGSessionSingle.OnNewGame(m_NewDifficulty);
		cFGSessionSingle.ReadScenarioVariableDefinitions();
		return true;
	}

	private void OnMenuUnloaded_LoadLevel()
	{
		CFGSingleton<CFGGame>.Instance.SessionSingle.LoadLevel();
	}
}
