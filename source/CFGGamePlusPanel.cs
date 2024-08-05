using System.Collections.Generic;
using UnityEngine;

public class CFGGamePlusPanel : CFGPanel
{
	public ECustomGameplayOption m_TempOption;

	[SerializeField]
	private CFGButtonExtension m_FullDeckBtn;

	[SerializeField]
	private CFGButtonExtension m_NoReactionShotBtn;

	[SerializeField]
	private CFGButtonExtension m_NoCoverReductionBtn;

	[SerializeField]
	private CFGButtonExtension m_EnemyAbilitiesBtn;

	[SerializeField]
	private CFGButtonExtension m_FastScarsBtn;

	[SerializeField]
	private CFGButtonExtension m_KillsReplenishLuckBtn;

	[SerializeField]
	private CFGButtonExtension m_LuckRegenerationBtn;

	[SerializeField]
	private CFGButtonExtension m_MoreWoundsBtn;

	[SerializeField]
	private CFGButtonExtension m_UnknownEnemyHealthBtn;

	[SerializeField]
	private CFGButtonExtension m_NonchangableCardsBtn;

	[SerializeField]
	private CFGButtonExtension m_ProceedBtn;

	[SerializeField]
	private CFGButtonExtension m_ProceedBtnPad;

	[SerializeField]
	private CFGButtonExtension m_CancelBtn;

	[SerializeField]
	private CFGButtonExtension m_CancelBtnPad;

	[SerializeField]
	private CFGButtonExtension m_SelectBtnPad;

	[SerializeField]
	private CFGTextExtension m_Header;

	[SerializeField]
	private CFGTextExtension m_Description;

	private EInputMode m_LastInput;

	private List<CFGButtonExtension> m_CheckBoxes;

	private int m_SelectedId = -1;

	private bool m_WoundsActive;

	private float m_TimeDiff = 0.15f;

	private float m_LastIputTime;

	protected override void Start()
	{
		base.Start();
		if (IsInitialized())
		{
			CFGScenarioDifficulyPanel scenarioDifficulty = CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioDifficulty;
			CFGScenarioMenuNew scenarioMenu = CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioMenu;
			if (scenarioDifficulty != null)
			{
				m_WoundsActive = scenarioDifficulty.Injuries;
			}
			else if (scenarioMenu != null)
			{
				m_WoundsActive = scenarioMenu.Injuries;
			}
			m_MoreWoundsBtn.enabled = m_WoundsActive;
			m_FullDeckBtn.m_ButtonClickedCallback = OnFullDeckBtnClick;
			m_FullDeckBtn.m_ButtonOverCallback = OnFullDeckBtnOvert;
			m_FullDeckBtn.m_ButtonOutCallback = OnButtonOut;
			m_FullDeckBtn.IsSelected = (m_TempOption & ECustomGameplayOption.FullDeck) == ECustomGameplayOption.FullDeck;
			m_NoReactionShotBtn.m_ButtonClickedCallback = OnNoReactionShotBtnClick;
			m_NoReactionShotBtn.m_ButtonOverCallback = OnNoReactionShotBtnOver;
			m_NoReactionShotBtn.m_ButtonOutCallback = OnButtonOut;
			m_NoReactionShotBtn.IsSelected = (m_TempOption & ECustomGameplayOption.NoReactionShot) == ECustomGameplayOption.NoReactionShot;
			m_NoCoverReductionBtn.m_ButtonClickedCallback = OnNoCoverReductionBtnClick;
			m_NoCoverReductionBtn.m_ButtonOverCallback = OnNoCoverReductionBtnOver;
			m_NoCoverReductionBtn.m_ButtonOutCallback = OnButtonOut;
			m_NoCoverReductionBtn.IsSelected = (m_TempOption & ECustomGameplayOption.NoCoverReduction) == ECustomGameplayOption.NoCoverReduction;
			m_EnemyAbilitiesBtn.m_ButtonClickedCallback = OnEnemyAbilitiesBtnClick;
			m_EnemyAbilitiesBtn.m_ButtonOverCallback = OnEnemyAbilitiesBtnOver;
			m_EnemyAbilitiesBtn.m_ButtonOutCallback = OnButtonOut;
			m_EnemyAbilitiesBtn.IsSelected = (m_TempOption & ECustomGameplayOption.EnemyAbilities) == ECustomGameplayOption.EnemyAbilities;
			m_FastScarsBtn.m_ButtonClickedCallback = OnFastScarsBtnClick;
			m_FastScarsBtn.m_ButtonOverCallback = OnFastScarsBtnOver;
			m_FastScarsBtn.m_ButtonOutCallback = OnButtonOut;
			m_FastScarsBtn.IsSelected = (m_TempOption & ECustomGameplayOption.FastScars) == ECustomGameplayOption.FastScars;
			m_KillsReplenishLuckBtn.m_ButtonClickedCallback = OnKillsReplenishLuckBtnClick;
			m_KillsReplenishLuckBtn.m_ButtonOverCallback = OnKillsReplenishLuckBtnOver;
			m_KillsReplenishLuckBtn.m_ButtonOutCallback = OnButtonOut;
			m_KillsReplenishLuckBtn.IsSelected = (m_TempOption & ECustomGameplayOption.KillsReplenishLuck) == ECustomGameplayOption.KillsReplenishLuck;
			m_LuckRegenerationBtn.m_ButtonClickedCallback = OnLuckRegenerationBtnClick;
			m_LuckRegenerationBtn.m_ButtonOverCallback = OnLuckRegenerationBtnOver;
			m_LuckRegenerationBtn.m_ButtonOutCallback = OnButtonOut;
			m_LuckRegenerationBtn.IsSelected = (m_TempOption & ECustomGameplayOption.LuckRegeneration) == ECustomGameplayOption.LuckRegeneration;
			m_MoreWoundsBtn.m_ButtonClickedCallback = OnMoreWoundsBtnClick;
			m_MoreWoundsBtn.m_ButtonOverCallback = OnMoreWoundsBtnOver;
			m_MoreWoundsBtn.m_ButtonOutCallback = OnButtonOut;
			m_MoreWoundsBtn.IsSelected = (m_TempOption & ECustomGameplayOption.MoreWounds) == ECustomGameplayOption.MoreWounds && m_WoundsActive;
			m_UnknownEnemyHealthBtn.m_ButtonClickedCallback = OnUnknownEnemyHealthBtnClick;
			m_UnknownEnemyHealthBtn.m_ButtonOverCallback = OnUnknownEnemyHealthBtnOver;
			m_UnknownEnemyHealthBtn.m_ButtonOutCallback = OnButtonOut;
			m_UnknownEnemyHealthBtn.IsSelected = (m_TempOption & ECustomGameplayOption.UnknownEnemyHealth) == ECustomGameplayOption.UnknownEnemyHealth;
			m_NonchangableCardsBtn.m_ButtonClickedCallback = OnNonchangableCardsBtnClick;
			m_NonchangableCardsBtn.m_ButtonOverCallback = OnNonchangableCardsBtnOver;
			m_NonchangableCardsBtn.m_ButtonOutCallback = OnButtonOut;
			m_NonchangableCardsBtn.IsSelected = (m_TempOption & ECustomGameplayOption.NonchangableCards) == ECustomGameplayOption.NonchangableCards;
			m_ProceedBtn.m_ButtonClickedCallback = OnProceedBtnClick;
			m_CancelBtn.m_ButtonClickedCallback = OnCancelBtnClick;
			m_CheckBoxes = new List<CFGButtonExtension>();
			m_CheckBoxes.Add(m_FullDeckBtn);
			m_CheckBoxes.Add(m_NoReactionShotBtn);
			m_CheckBoxes.Add(m_EnemyAbilitiesBtn);
			m_CheckBoxes.Add(m_NoCoverReductionBtn);
			m_CheckBoxes.Add(m_FastScarsBtn);
			m_CheckBoxes.Add(m_KillsReplenishLuckBtn);
			m_CheckBoxes.Add(m_LuckRegenerationBtn);
			m_CheckBoxes.Add(m_UnknownEnemyHealthBtn);
			m_CheckBoxes.Add(m_MoreWoundsBtn);
			m_CheckBoxes.Add(m_NonchangableCardsBtn);
			SelectCheckBox(0);
			UpdateButtonsVisibility();
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateButtonsVisibility();
		if (CFGTimer.MissionTime > m_LastIputTime + m_TimeDiff)
		{
			int num = m_SelectedId;
			m_LastIputTime = CFGTimer.MissionTime;
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0.5f)
			{
				num--;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0.5f)
			{
				num++;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Left) > 0.5f)
			{
				num -= 5;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Right) > 0.5f)
			{
				num += 5;
			}
			if (num > 9)
			{
				num -= 10;
			}
			else if (num < 0)
			{
				num += 10;
			}
			SelectCheckBox(num);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
		{
			m_CheckBoxes[m_SelectedId].m_ButtonClickedCallback(0);
			m_SelectBtnPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			m_ProceedBtn.m_ButtonClickedCallback(0);
			m_ProceedBtnPad.SimulateClickGraphicAndSoundOnly();
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
		{
			m_CancelBtn.m_ButtonClickedCallback(0);
			m_CancelBtnPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	public override void SetLocalisation()
	{
		base.SetLocalisation();
		if (IsInitialized())
		{
			m_FullDeckBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_fulldeck");
			m_NoReactionShotBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_noreaction");
			m_NoCoverReductionBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_noreduction");
			m_EnemyAbilitiesBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_aiabilities");
			m_FastScarsBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_fastscars");
			m_KillsReplenishLuckBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_killsluck");
			m_LuckRegenerationBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_luckregen");
			m_MoreWoundsBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_morewounds");
			m_UnknownEnemyHealthBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_unknownhp");
			m_NonchangableCardsBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_cardlock");
			m_ProceedBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more_confirm");
			m_CancelBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more_cancel");
			m_ProceedBtnPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more_confirm");
			m_CancelBtnPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more_cancel");
			m_SelectBtnPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gameplaymods_pad_selectoption");
			m_Header.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_more");
			m_Description.text = string.Empty;
		}
	}

	private void UpdateButtonsVisibility()
	{
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_LastInput = CFGInput.LastReadInputDevice;
			bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
			m_ProceedBtnPad.gameObject.SetActive(flag);
			m_CancelBtnPad.gameObject.SetActive(flag);
			m_SelectBtnPad.gameObject.SetActive(flag);
			m_ProceedBtn.gameObject.SetActive(!flag);
			m_CancelBtn.gameObject.SetActive(!flag);
			if (!flag)
			{
				HideHighlight();
			}
		}
	}

	private bool IsInitialized()
	{
		if (m_FullDeckBtn == null || m_FullDeckBtn.m_Label == null || m_NoReactionShotBtn == null || m_NoReactionShotBtn.m_Label == null || m_NoCoverReductionBtn == null || m_NoCoverReductionBtn.m_Label == null || m_EnemyAbilitiesBtn == null || m_EnemyAbilitiesBtn.m_Label == null || m_FastScarsBtn == null || m_FastScarsBtn.m_Label == null || m_KillsReplenishLuckBtn == null || m_KillsReplenishLuckBtn.m_Label == null || m_LuckRegenerationBtn == null || m_LuckRegenerationBtn.m_Label == null || m_MoreWoundsBtn == null || m_MoreWoundsBtn.m_Label == null || m_UnknownEnemyHealthBtn == null || m_UnknownEnemyHealthBtn.m_Label == null || m_NonchangableCardsBtn == null || m_NonchangableCardsBtn.m_Label == null || m_ProceedBtn == null || m_ProceedBtn.m_Label == null || m_CancelBtn == null || m_CancelBtn.m_Label == null || m_ProceedBtnPad == null || m_ProceedBtnPad.m_Label == null || m_CancelBtnPad == null || m_CancelBtnPad.m_Label == null || m_SelectBtnPad == null || m_SelectBtnPad.m_Label == null || m_Header == null || m_Description == null)
		{
			return false;
		}
		return true;
	}

	private void OnButtonOut(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = string.Empty;
		}
	}

	private void OnFullDeckBtnClick(int button_data)
	{
		if (!(m_FullDeckBtn == null))
		{
			m_FullDeckBtn.IsSelected = !m_FullDeckBtn.IsSelected;
		}
	}

	private void OnFullDeckBtnOvert(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_fulldeck_desc");
		}
	}

	private void OnNoReactionShotBtnClick(int button_data)
	{
		if (!(m_NoReactionShotBtn == null))
		{
			m_NoReactionShotBtn.IsSelected = !m_NoReactionShotBtn.IsSelected;
		}
	}

	private void OnNoReactionShotBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_noreaction_desc");
		}
	}

	private void OnNoCoverReductionBtnClick(int button_data)
	{
		if (!(m_NoCoverReductionBtn == null))
		{
			m_NoCoverReductionBtn.IsSelected = !m_NoCoverReductionBtn.IsSelected;
		}
	}

	private void OnNoCoverReductionBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_noreduction_desc");
		}
	}

	private void OnEnemyAbilitiesBtnClick(int button_data)
	{
		if (!(m_EnemyAbilitiesBtn == null))
		{
			m_EnemyAbilitiesBtn.IsSelected = !m_EnemyAbilitiesBtn.IsSelected;
		}
	}

	private void OnEnemyAbilitiesBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_aiabilities_desc");
		}
	}

	private void OnFastScarsBtnClick(int button_data)
	{
		if (!(m_FastScarsBtn == null))
		{
			m_FastScarsBtn.IsSelected = !m_FastScarsBtn.IsSelected;
		}
	}

	private void OnFastScarsBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_fastscars_desc");
		}
	}

	private void OnKillsReplenishLuckBtnClick(int button_data)
	{
		if (!(m_KillsReplenishLuckBtn == null))
		{
			m_KillsReplenishLuckBtn.IsSelected = !m_KillsReplenishLuckBtn.IsSelected;
		}
	}

	private void OnKillsReplenishLuckBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_killsluck_desc");
		}
	}

	private void OnLuckRegenerationBtnClick(int button_data)
	{
		if (!(m_LuckRegenerationBtn == null))
		{
			m_LuckRegenerationBtn.IsSelected = !m_LuckRegenerationBtn.IsSelected;
		}
	}

	private void OnLuckRegenerationBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_luckregen_desc");
		}
	}

	private void OnMoreWoundsBtnClick(int button_data)
	{
		if (!(m_MoreWoundsBtn == null) && m_WoundsActive)
		{
			m_MoreWoundsBtn.IsSelected = !m_MoreWoundsBtn.IsSelected;
		}
	}

	private void OnMoreWoundsBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_morewounds_desc");
		}
	}

	private void OnUnknownEnemyHealthBtnClick(int button_data)
	{
		if (!(m_UnknownEnemyHealthBtn == null))
		{
			m_UnknownEnemyHealthBtn.IsSelected = !m_UnknownEnemyHealthBtn.IsSelected;
		}
	}

	private void OnUnknownEnemyHealthBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_unknownhp_desc");
		}
	}

	private void OnNonchangableCardsBtnClick(int button_data)
	{
		if (!(m_NonchangableCardsBtn == null))
		{
			m_NonchangableCardsBtn.IsSelected = !m_NonchangableCardsBtn.IsSelected;
		}
	}

	private void OnNonchangableCardsBtnOver(int button_data)
	{
		if (!(m_Description == null))
		{
			m_Description.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("scenario_menu_cardlock_desc");
		}
	}

	private void OnProceedBtnClick(int button_data)
	{
		UpdateTempOption(m_FullDeckBtn, ECustomGameplayOption.FullDeck);
		UpdateTempOption(m_NoReactionShotBtn, ECustomGameplayOption.NoReactionShot);
		UpdateTempOption(m_NoCoverReductionBtn, ECustomGameplayOption.NoCoverReduction);
		UpdateTempOption(m_EnemyAbilitiesBtn, ECustomGameplayOption.EnemyAbilities);
		UpdateTempOption(m_FastScarsBtn, ECustomGameplayOption.FastScars);
		UpdateTempOption(m_KillsReplenishLuckBtn, ECustomGameplayOption.KillsReplenishLuck);
		UpdateTempOption(m_LuckRegenerationBtn, ECustomGameplayOption.LuckRegeneration);
		UpdateTempOption(m_MoreWoundsBtn, ECustomGameplayOption.MoreWounds);
		UpdateTempOption(m_UnknownEnemyHealthBtn, ECustomGameplayOption.UnknownEnemyHealth);
		UpdateTempOption(m_NonchangableCardsBtn, ECustomGameplayOption.NonchangableCards);
		CFGScenarioDifficulyPanel scenarioDifficulty = CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioDifficulty;
		CFGScenarioMenuNew scenarioMenu = CFGSingleton<CFGWindowMgr>.Instance.m_ScenarioMenu;
		if (scenarioDifficulty != null)
		{
			scenarioDifficulty.m_CustomOptions = m_TempOption;
		}
		else if (scenarioMenu != null)
		{
			scenarioMenu.m_CustomOptions = m_TempOption;
		}
		CFGSingleton<CFGWindowMgr>.Instance.UnloadGamePlusPanel();
	}

	private void OnCancelBtnClick(int button_data)
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadGamePlusPanel();
	}

	private void UpdateTempOption(CFGButtonExtension button, ECustomGameplayOption option)
	{
		if (!(button == null))
		{
			if (button.IsSelected)
			{
				m_TempOption |= option;
			}
			else
			{
				m_TempOption &= ~option;
			}
		}
	}

	private void SelectCheckBox(int id)
	{
		if (m_CheckBoxes == null || m_CheckBoxes.Count <= id || m_SelectedId == id)
		{
			return;
		}
		m_SelectedId = id;
		for (int i = 0; i < m_CheckBoxes.Count; i++)
		{
			int childCount = m_CheckBoxes[i].transform.parent.parent.childCount;
			if (childCount != 0)
			{
				Transform child = m_CheckBoxes[i].transform.parent.parent.GetChild(childCount - 1);
				bool flag = i == m_SelectedId;
				child.gameObject.SetActive(flag);
				if (flag)
				{
					m_CheckBoxes[i].m_ButtonOverCallback(0);
				}
			}
		}
	}

	private void HideHighlight()
	{
		if (m_CheckBoxes == null)
		{
			return;
		}
		OnButtonOut(0);
		for (int i = 0; i < m_CheckBoxes.Count; i++)
		{
			int childCount = m_CheckBoxes[i].transform.parent.parent.childCount;
			if (childCount != 0)
			{
				Transform child = m_CheckBoxes[i].transform.parent.parent.GetChild(childCount - 1);
				child.gameObject.SetActive(value: false);
			}
		}
	}
}
