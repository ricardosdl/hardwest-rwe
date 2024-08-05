using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGHUD : CFGPanel
{
	public CFGButtonExtension m_XButtonPad;

	public CFGButtonExtension m_YButtonPad;

	public CFGButtonExtension m_LBButtonPad;

	public CFGButtonExtension m_RBButtonPad;

	public CFGButtonExtension m_VerticalPad;

	public CFGButtonExtension m_HorizontalPad;

	public List<CFGButtonExtension> m_GunButtons = new List<CFGButtonExtension>();

	public CFGTextExtension m_NameChar;

	public CFGTextExtension m_GunName;

	public CFGTextExtension m_Ammo;

	public CFGTextExtension m_Hp;

	public CFGTextExtension m_Luck;

	public CFGImageExtension m_GunLeft;

	public CFGImageExtension m_GunRight;

	public CFGImageExtension m_Avatar;

	public List<CFGImageExtension> m_CharButtons = new List<CFGImageExtension>();

	public List<CFGButtonExtension> m_CharExtButtons = new List<CFGButtonExtension>();

	public List<GameObject> m_CharAbilityIcons = new List<GameObject>();

	public List<CFGImageExtension> m_CharAbilityIconsNormal = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CharAbilityIconsHover = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CharAbilityIconsHighlight = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CharAbilityIconsClick = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_ApPointList = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_ApPointListBG = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_Imprisoned = new List<CFGImageExtension>();

	public CFGImageExtension m_ImprisonedBig;

	public CFGButtonExtension m_CharBig;

	public CFGMaskedProgressBar m_HpBar;

	public CFGMaskedProgressBar m_LuckBar;

	public List<Image> m_CharFrames = new List<Image>();

	public CFGButtonExtension m_LuckBtn;

	public CFGButtonExtension m_HPBtn;

	public Animator m_FlashCharacterBig = new Animator();

	public List<Animator> m_FlashCharacterSmalls = new List<Animator>();

	public Animator m_FlashCharacterHP = new Animator();

	public Animator m_FlashCharacterLuck = new Animator();

	public GameObject m_FlashingCharacterHP;

	public List<Image> m_HeatMarkers = new List<Image>();

	public CFGWeaponRange m_WeaponMark;

	public Text m_WeaponRangeText;

	public GameObject m_SelectionPanel;

	public GameObject m_GunPanel;

	public GameObject m_BtnPanel;

	public GameObject m_BtnPadPanel;

	public Text m_HeatText;

	private bool m_Initalised;

	protected override void Start()
	{
		base.Start();
		foreach (CFGButtonExtension gunButton in m_GunButtons)
		{
			gunButton.m_ButtonClickedCallback = OnChangeGunButton;
		}
		foreach (CFGButtonExtension charExtButton in m_CharExtButtons)
		{
			charExtButton.m_ButtonClickedCallback = OnCharacterButtonClick;
			charExtButton.IsSelected = false;
		}
		m_CharBig.m_ButtonClickedCallback = OnCharacterBigButtonClick;
		foreach (CFGImageExtension charButton in m_CharButtons)
		{
			charButton.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		}
		m_Avatar.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_GunLeft.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		m_GunRight.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		m_WeaponMark.SetPanelVisible(CFGOptions.Gameplay.ShowWeaponRange);
		OnCharacterButtonClick(0);
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

	public void FlashCharacterBig(bool _yellowFlash = true)
	{
		int iconNumber = ((!_yellowFlash) ? 1 : 0);
		CFGImageExtension component = m_FlashCharacterBig.GetComponent<CFGImageExtension>();
		if (!(component == null))
		{
			component.IconNumber = iconNumber;
			m_FlashCharacterBig.SetTrigger("UseAbility");
		}
	}

	public void FlashCharacterSmallCurrent(bool _yellowFlash = true)
	{
		for (int i = 0; i < m_CharExtButtons.Count; i++)
		{
			if (m_CharExtButtons[i].IsSelected)
			{
				int iconNumber = ((!_yellowFlash) ? 1 : 0);
				CFGImageExtension component = m_FlashCharacterSmalls[i].GetComponent<CFGImageExtension>();
				if (component == null)
				{
					break;
				}
				component.IconNumber = iconNumber;
				m_FlashCharacterSmalls[i].SetTrigger("UseAbility");
			}
		}
	}

	public void FlashCharacterSmall(int number, bool _yellowFlash = true)
	{
		int iconNumber = ((!_yellowFlash) ? 1 : 0);
		CFGImageExtension component = m_FlashCharacterSmalls[number].GetComponent<CFGImageExtension>();
		if (!(component == null))
		{
			component.IconNumber = iconNumber;
			m_FlashCharacterSmalls[number].SetTrigger("UseAbility");
		}
	}

	public void FlashHP()
	{
		m_FlashCharacterHP.SetTrigger("UseAbility");
	}

	public void FlashingHP(bool enabled)
	{
		m_FlashingCharacterHP.SetActive(enabled);
	}

	public void FlashLuck()
	{
		m_FlashCharacterLuck.SetTrigger("UseAbility");
	}

	public override void SetLocalisation()
	{
		foreach (CFGButtonExtension gunButton in m_GunButtons)
		{
			gunButton.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_switchweapons", FormatShortcut(EActionCommand.WeaponChange));
		}
		if (m_XButtonPad != null)
		{
			m_XButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_tacticalscene_endturn");
		}
		if (m_VerticalPad != null)
		{
			m_VerticalPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_tacticalscene_changefloor");
		}
		if (m_HorizontalPad != null)
		{
			m_HorizontalPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_tacticalscene_rotatecamera");
		}
		m_WeaponRangeText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_range");
		m_HeatText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_heat");
		m_HeatMarkers[0].transform.parent.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_heat");
		m_LuckBtn.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_luck");
		m_HPBtn.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_hp");
		m_CharBig.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_characterpanel", FormatShortcut(EActionCommand.ToggleCharacterInfo));
	}

	protected new void Update()
	{
	}

	public void OnChangeGunButton(int a)
	{
		CFGSelectionManager.Instance.OnChangeWeaponClick();
	}

	public void OnCharacterButtonClick(int id)
	{
		CFGSelectionManager.Instance.OnCharacterButtonClick(id);
	}

	public void OnCharacterBigButtonClick(int a)
	{
		CFGSelectionManager.Instance.OnCharacterBigButtonClick();
		Debug.Log("character button click");
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)CFGSelectionManager.Instance && !CFGSelectionManager.Instance.IsLocked && (bool)CFGSelectionManager.Instance.SelectedCharacter)
		{
			instance.LoadTacticalCharacterDetails();
		}
	}

	public void SelectCharacter(int id)
	{
		foreach (CFGButtonExtension charExtButton in m_CharExtButtons)
		{
			charExtButton.IsSelected = false;
		}
		m_CharExtButtons[id].IsSelected = true;
		foreach (CFGImageExtension apPoint in m_ApPointList)
		{
			apPoint.IconNumber = 0;
		}
		m_ApPointList[id * 2].IconNumber = 1;
		m_ApPointList[id * 2 + 1].IconNumber = 1;
		m_NameChar.color = new Color(m_NameChar.color.r, m_NameChar.color.g, m_NameChar.color.b, 1f);
	}

	public void SetSelectionPanelVisibility(bool visible)
	{
		m_SelectionPanel.SetActive(visible);
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_ActionsPanel.SetActive(visible);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel.m_StartPad.gameObject.SetActive(CFGInput.LastReadInputDevice == EInputMode.Gamepad && visible);
			CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel.m_BackPad.gameObject.SetActive(CFGInput.LastReadInputDevice == EInputMode.Gamepad && visible);
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_LBButtonPad != null)
		{
			m_LBButtonPad.gameObject.SetActive(flag && visible && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings && !CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.gameObject.activeSelf && CFGCharacterList.GetTeamCharactersListTactical().Count > 1);
		}
		if (m_RBButtonPad != null)
		{
			m_RBButtonPad.gameObject.SetActive(flag && visible && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings && !CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.gameObject.activeSelf && CFGCharacterList.GetTeamCharactersListTactical().Count > 1);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities)
		{
			if (CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad.gameObject.SetActive(flag && visible && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_SkillButtonsData.Count > 0 && CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings != null && CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.gameObject.activeSelf);
			}
			if (CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad.gameObject.SetActive(flag && visible && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_SkillButtonsData.Count > 0);
			}
		}
		if (m_YButtonPad != null)
		{
			m_YButtonPad.gameObject.SetActive(flag && visible);
		}
	}

	public void SetMainBtnPanelVisibility(bool visible)
	{
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_BtnPanel.SetActive(!flag && visible);
		m_BtnPadPanel.SetActive(flag && visible);
	}

	public void SetGunPanelVisibility(bool visible)
	{
		m_GunPanel.SetActive(visible);
	}

	public void SetCharactersVisible(int number)
	{
		for (int i = 0; i < m_CharButtons.Count; i++)
		{
			m_CharButtons[i].transform.parent.gameObject.SetActive(i < number);
		}
		for (int j = 0; j < m_CharFrames.Count; j++)
		{
			m_CharFrames[j].gameObject.SetActive(number > j + 1);
		}
	}

	public void SetCharacterAbilityIcons(int number, int icon)
	{
		if (icon == -1)
		{
			m_CharAbilityIcons[number].SetActive(value: false);
			return;
		}
		m_CharAbilityIcons[number].SetActive(value: true);
		m_CharAbilityIconsNormal[number].IconNumber = icon;
		m_CharAbilityIconsHover[number].IconNumber = icon;
		m_CharAbilityIconsHighlight[number].IconNumber = icon;
		m_CharAbilityIconsClick[number].IconNumber = icon;
	}
}
