using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGStrategicExploratorButtons : CFGPanel
{
	public CFGButtonExtension m_CharacterButton;

	public CFGButtonExtension m_OptionsButton;

	public Text m_PlayerGold;

	public Text m_PlayerOtherGoods;

	public CFGImageExtension m_PlayerOtherGoodsIcon;

	public GameObject m_PlacePad;

	private Vector3 m_PositionNormal1 = Vector3.zero;

	private Vector3 m_PositionNormal2 = Vector3.zero;

	private EInputMode m_LastInput = EInputMode.KeyboardAndMouse;

	protected override void Start()
	{
		base.Start();
		m_CharacterButton.m_ButtonClickedCallback = OnCharacterButtonClick;
		m_OptionsButton.m_ButtonClickedCallback = OnButtonPressedMenu;
		m_CharacterButton.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_characterpanel", FormatShortcut(EActionCommand.ToggleCharacterInfo));
		m_OptionsButton.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_mainmenu", FormatShortcut(EActionCommand.Exit));
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance && instance.m_HUD != null)
		{
			m_PlayerGold.transform.parent.gameObject.SetActive(value: false);
		}
		UpdateCustoms();
		m_PositionNormal1 = m_PlayerGold.transform.parent.position;
		m_PositionNormal2 = m_PlayerOtherGoods.transform.parent.position;
	}

	public static string FormatShortcut(EActionCommand command)
	{
		string keyCombo = CFGInput.GetKeyCombo(command);
		if (keyCombo != string.Empty)
		{
			return "[<color=#FFBE35>" + keyCombo + "</color>]";
		}
		return string.Empty;
	}

	public void UpdateCustoms()
	{
		if ((bool)m_PlayerOtherGoodsIcon)
		{
			m_PlayerOtherGoodsIcon.transform.parent.gameObject.SetActive(value: false);
		}
		if (CFGSingleton<CFGWindowMgr>.Instance.m_CassandraSP != null)
		{
			CFGVar variable = CFGVariableContainer.Instance.GetVariable("s8_powerCooldown", "scenario");
			if (variable != null)
			{
				int progress = (int)variable.Value;
				CFGSingleton<CFGWindowMgr>.Instance.m_CassandraSP.SetProgress(progress);
			}
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS5 != null)
		{
			CFGVar variable2 = CFGVariableContainer.Instance.GetVariable("engineering_level", "scenario");
			CFGVar variable3 = CFGVariableContainer.Instance.GetVariable("gunsmith_level", "scenario");
			CFGVar variable4 = CFGVariableContainer.Instance.GetVariable("chemistry_level", "scenario");
			if (variable2 != null && variable3 != null && variable4 != null)
			{
				int chem = (int)variable4.Value;
				int gun = (int)variable3.Value;
				int eng = (int)variable2.Value;
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS5.SetScienceLevels(chem, eng, gun);
			}
			CFGVar variable5 = CFGVariableContainer.Instance.GetVariable("crat_cp", "scenario");
			CFGVar variable6 = CFGVariableContainer.Instance.GetVariable("area_madness", "scenario");
			if (variable5 != null && variable6 != null)
			{
				bool madnessVisible = (bool)variable5.Value;
				int madnessLevel = (int)variable6.Value;
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS5.SetMadnessVisible(madnessVisible);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS5.SetMadnessLevel(madnessLevel);
			}
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS1 != null)
		{
			CFGVar variable7 = CFGVariableContainer.Instance.GetVariable("ui_reward", "scenario");
			CFGVar variable8 = CFGVariableContainer.Instance.GetVariable("deaths", "scenario");
			CFGVar variable9 = CFGVariableContainer.Instance.GetVariable("ui_damage", "scenario");
			CFGVar variable10 = CFGVariableContainer.Instance.GetVariable("ui_gunshop_closed", "scenario");
			CFGVar variable11 = CFGVariableContainer.Instance.GetVariable("ui_gunshop_prices", "scenario");
			CFGVar variable12 = CFGVariableContainer.Instance.GetVariable("ui_elixir_closed", "scenario");
			CFGVar variable13 = CFGVariableContainer.Instance.GetVariable("ui_elixir_prices", "scenario");
			int ic_gun = -1;
			int ic_elixir = -1;
			if ((bool)variable10.Value)
			{
				ic_gun = 1;
			}
			else if ((bool)variable11.Value)
			{
				ic_gun = 0;
			}
			if ((bool)variable12.Value)
			{
				ic_elixir = 1;
			}
			else if ((bool)variable13.Value)
			{
				ic_elixir = 0;
			}
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS1.SetReward((int)variable7.Value);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS1.SetRewardDesc((int)variable8.Value, (int)variable9.Value);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS1.SetIcons(ic_gun, ic_elixir);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS2 != null)
		{
			CFGVar variable14 = CFGVariableContainer.Instance.GetVariable("cipher_progress", "scenario");
			if (variable14 != null)
			{
				for (int i = 0; i < CFGSingleton<CFGWindowMgr>.Instance.m_CustomS2.m_ElementsList.Count; i++)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_CustomS2.m_ElementsList[i].SetActive(i < (int)variable14.Value);
				}
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS2.m_Animator.enabled = (int)variable14.Value == 12;
			}
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS6 != null)
		{
			CFGVar variable15 = CFGVariableContainer.Instance.GetVariable("manpower", "scenario");
			CFGVar variable16 = CFGVariableContainer.Instance.GetVariable("provisions", "scenario");
			CFGVar variable17 = CFGVariableContainer.Instance.GetVariable("sick", "scenario");
			CFGVar variable18 = CFGVariableContainer.Instance.GetVariable("dead", "scenario");
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS6.m_Peons.text = string.Empty + (int)variable15.Value;
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS6.m_Provisions.text = string.Empty + (int)variable16.Value;
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS6.m_Sick.text = string.Empty + (int)variable17.Value;
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS6.m_Dead.text = string.Empty + (int)variable18.Value;
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS3 != null)
		{
			CFGVar variable19 = CFGVariableContainer.Instance.GetVariable("s3_day_time", "scenario");
			if (variable19 != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS3.IconNumber = (int)variable19.Value;
			}
			m_PlayerOtherGoodsIcon.transform.parent.gameObject.SetActive(value: true);
			m_PlayerOtherGoodsIcon.IconNumber = 0;
			m_PlayerOtherGoods.text = string.Empty + CFGInventory.Item_GetCount("food_portion");
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4 != null)
		{
			CFGVar variable20 = CFGVariableContainer.Instance.GetVariable("s4_ui_comp_01_spec", "scenario");
			CFGVar variable21 = CFGVariableContainer.Instance.GetVariable("s4_ui_comp_02_spec", "scenario");
			CFGVar variable22 = CFGVariableContainer.Instance.GetVariable("s4_ui_comp_03_spec", "scenario");
			List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_NoIcon1.SetActive(teamCharactersList.Count < 2);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_NoIcon2.SetActive(teamCharactersList.Count < 3);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_NoIcon3.SetActive(teamCharactersList.Count < 4);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Frame1.SetActive(teamCharactersList.Count > 1);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Frame2.SetActive(teamCharactersList.Count > 2);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description1.gameObject.SetActive(teamCharactersList.Count > 1);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description2.gameObject.SetActive(teamCharactersList.Count > 2);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description3.gameObject.SetActive(teamCharactersList.Count > 3);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name1.gameObject.SetActive(teamCharactersList.Count > 1);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name2.gameObject.SetActive(teamCharactersList.Count > 2);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name3.gameObject.SetActive(teamCharactersList.Count > 3);
			if (teamCharactersList.Count > 1)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name1.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharactersList[1].Definition.NameID);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description1.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText((string)variable20.Value);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Icon1.IconNumber = teamCharactersList[1].ImageIDX;
			}
			if (teamCharactersList.Count > 2)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharactersList[2].Definition.NameID);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText((string)variable21.Value);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Icon2.IconNumber = teamCharactersList[2].ImageIDX;
			}
			if (teamCharactersList.Count > 3)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Name3.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharactersList[3].Definition.NameID);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Description3.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText((string)variable22.Value);
				CFGSingleton<CFGWindowMgr>.Instance.m_CustomS4.m_Icon3.IconNumber = teamCharactersList[3].ImageIDX;
			}
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7 != null)
		{
			CFGVar variable23 = CFGVariableContainer.Instance.GetVariable("s7_placer_tech", "scenario");
			CFGVar variable24 = CFGVariableContainer.Instance.GetVariable("s7_deeper_tech", "scenario");
			CFGVar variable25 = CFGVariableContainer.Instance.GetVariable("s7_hardrock_tech", "scenario");
			CFGVar variable26 = CFGVariableContainer.Instance.GetVariable("s7_tech_modifier", "scenario");
			CFGVar variable27 = CFGVariableContainer.Instance.GetVariable("s7_prospect_bool_mercury", "scenario");
			CFGVar variable28 = CFGVariableContainer.Instance.GetVariable("s7_prospect_bool_drilling", "scenario");
			CFGVar variable29 = CFGVariableContainer.Instance.GetVariable("s7_prospect_bool_stampmill", "scenario");
			CFGVar variable30 = CFGVariableContainer.Instance.GetVariable("s7_prospect_bool_jet", "scenario");
			CFGVar variable31 = CFGVariableContainer.Instance.GetVariable("s7_objective_uses_limit", "scenario");
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_Drilling.IconNumber = (((bool)variable28.Value) ? 1 : 0);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_Mercury.IconNumber = (((bool)variable27.Value) ? 1 : 0);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_Stampmill.IconNumber = (((bool)variable29.Value) ? 1 : 0);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_Jet.IconNumber = (((bool)variable30.Value) ? 1 : 0);
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_SpherePlacerTxt.text = "x" + (1f + (float)(int)variable23.Value * (float)variable26.Value).ToString("0.00");
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_SphereDeeperTxt.text = "x" + (1f + (float)(int)variable24.Value * (float)variable26.Value).ToString("0.00");
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_SphereHardrockTxt.text = "x" + (1f + (float)(int)variable25.Value * (float)variable26.Value).ToString("0.00");
			CFGSingleton<CFGWindowMgr>.Instance.m_CustomS7.m_UsesTxt.text = variable31.ValueString;
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CustomS9 != null)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UpdateCustomS9();
			if (m_PlayerOtherGoodsIcon != null && m_PlayerOtherGoods != null)
			{
				m_PlayerOtherGoodsIcon.transform.parent.gameObject.SetActive(value: true);
				m_PlayerOtherGoodsIcon.IconNumber = 2;
				int num = CFGInventory.Item_GetCount("ether_dlc1");
				m_PlayerOtherGoods.text = num.ToString();
			}
		}
	}

	public override void Update()
	{
		base.Update();
		m_PlayerGold.text = CFGInventory.Cash_Get().ToString();
		UpdateCustoms();
		m_CharacterButton.gameObject.SetActive(CFGInput.LastReadInputDevice != EInputMode.Gamepad && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && !CFGSingleton<CFGWindowMgr>.Instance.m_HUD);
		m_OptionsButton.gameObject.SetActive(CFGInput.LastReadInputDevice != EInputMode.Gamepad);
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator != null && CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator.IsExplorationWindowVisible())
		{
			m_CharacterButton.enabled = false;
			m_OptionsButton.enabled = false;
		}
		else
		{
			if (!m_CharacterButton.enabled)
			{
				m_CharacterButton.enabled = true;
				m_OptionsButton.enabled = true;
			}
			if (CFGInput.IsActivated(EActionCommand.ToggleCharacterInfo))
			{
				OnCharacterButtonClick(0);
			}
		}
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
			{
				m_PlayerGold.transform.parent.position = new Vector3(m_PlacePad.transform.position.x, m_PlayerGold.transform.position.y, m_PlayerGold.transform.position.z);
				m_PlayerOtherGoods.transform.parent.position = new Vector3(m_PlacePad.transform.position.x, m_PlayerOtherGoods.transform.position.y, m_PlayerOtherGoods.transform.position.z);
			}
			else
			{
				m_PlayerGold.transform.parent.position = m_PositionNormal1;
				m_PlayerOtherGoods.transform.parent.position = m_PositionNormal2;
			}
			m_LastInput = CFGInput.LastReadInputDevice;
		}
	}

	public void OnCharacterButtonClick(int a)
	{
		Debug.Log("character button click");
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance && instance.m_HUD == null)
		{
			instance.LoadCharacterScreen(combat_loadout: false, null);
		}
		else if ((bool)CFGSelectionManager.Instance && (bool)CFGSelectionManager.Instance.SelectedCharacter)
		{
			instance.LoadTacticalCharacterDetails();
		}
	}

	public void OnButtonPressedMenu(int a)
	{
		Debug.Log("Menu clicked");
		CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
		if ((bool)cFGSessionSingle && !cFGSessionSingle.IsLoadingLevel())
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadInGameMenu();
		}
	}
}
