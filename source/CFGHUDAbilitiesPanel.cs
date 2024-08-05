using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGHUDAbilitiesPanel : CFGPanel
{
	public CFGButtonExtension m_LTButtonPad;

	public CFGButtonExtension m_RTButtonPad;

	public List<CFGButtonExtension> m_SkillButtons = new List<CFGButtonExtension>();

	public List<CFGImageExtension> m_SkillNormalIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillAnimIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillHoverIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillHighlightIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillClickedIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillDisabledIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillDisabledFakeIcon = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillDisabledFakeBg = new List<CFGImageExtension>();

	public List<Image> m_SkillCooldown = new List<Image>();

	public List<Text> m_SkillCooldowns = new List<Text>();

	public List<Image> m_SkillFrames = new List<Image>();

	public List<Image> m_SkillEnds = new List<Image>();

	public Image m_EnemyTurnPanel;

	public GameObject m_ActionsPanel;

	public Text m_EnemyTurnPanelText;

	private Vector3 m_YPosSkills = default(Vector3);

	private Vector3 m_YPosBtn = default(Vector3);

	public List<CFGSkillButtonsData> m_SkillButtonsData = new List<CFGSkillButtonsData>();

	private int m_OldSkillsButtonCount;

	protected override void Start()
	{
		base.Start();
		foreach (CFGButtonExtension skillButton in m_SkillButtons)
		{
			skillButton.m_ButtonClickedCallback = OnAbilityButton;
		}
		m_YPosSkills = m_ActionsPanel.transform.position;
		m_YPosBtn = m_RTButtonPad.transform.position;
		foreach (CFGImageExtension item in m_SkillNormalIcon)
		{
			item.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesIcons;
		}
		foreach (CFGImageExtension item2 in m_SkillHoverIcon)
		{
			item2.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesHoverIcons;
		}
		foreach (CFGImageExtension item3 in m_SkillHighlightIcon)
		{
			item3.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesHoverIcons;
		}
		foreach (CFGImageExtension item4 in m_SkillClickedIcon)
		{
			item4.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesClickedIcons;
		}
		foreach (CFGImageExtension item5 in m_SkillDisabledIcon)
		{
			item5.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesDisabledIcons;
		}
		foreach (CFGImageExtension item6 in m_SkillDisabledFakeIcon)
		{
			item6.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesDisabledIcons;
		}
		foreach (CFGImageExtension item7 in m_SkillAnimIcon)
		{
			item7.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesDisabledIcons;
		}
		base.transform.position = new Vector3(Screen.width / 2, base.transform.position.y);
	}

	public void SetEnemyTurnPanelVisibility(bool visible)
	{
		m_EnemyTurnPanel.gameObject.SetActive(visible);
	}

	public void ToggleSkillUseMeState(int skill_data, bool enable)
	{
		for (int i = 0; i < m_SkillButtons.Count; i++)
		{
			if (m_SkillButtons[i].m_Data == skill_data)
			{
				m_SkillButtons[i].IsInUseMeState = enable;
			}
		}
	}

	public void SetSkillsButtons()
	{
		for (int i = 0; i < m_SkillButtons.Count; i++)
		{
			m_SkillButtons[i].gameObject.SetActive(m_SkillButtonsData.Count > i);
			if (m_SkillButtonsData.Count > i)
			{
				if (m_SkillButtonsData[i].icon_list == 0)
				{
					m_SkillNormalIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesIcons;
					m_SkillHoverIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesHoverIcons;
					m_SkillHighlightIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesHoverIcons;
					m_SkillClickedIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesClickedIcons;
					m_SkillDisabledIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesDisabledIcons;
					m_SkillDisabledFakeIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesDisabledIcons;
					m_SkillAnimIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesHoverIcons;
				}
				else
				{
					m_SkillNormalIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableIcons;
					m_SkillHoverIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableHoverIcons;
					m_SkillHighlightIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableHoverIcons;
					m_SkillClickedIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableClickedIcons;
					m_SkillDisabledIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableDisabledIcons;
					m_SkillDisabledFakeIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableDisabledIcons;
					m_SkillAnimIcon[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableHoverIcons;
				}
			}
		}
		for (int j = 0; j < m_SkillFrames.Count; j++)
		{
			m_SkillFrames[j].gameObject.SetActive(m_SkillButtonsData.Count > j + 1);
		}
		for (int k = 0; k < m_SkillEnds.Count; k++)
		{
			m_SkillEnds[k].gameObject.SetActive(m_SkillButtonsData.Count - 1 == k);
		}
		for (int l = 0; l < m_SkillButtonsData.Count && l < m_SkillButtons.Count; l++)
		{
			m_SkillButtons[l].IsSelected = m_SkillButtonsData[l].selected;
			if (m_SkillButtonsData[l].enabled && m_SkillButtons[l].m_VisualsDisabled)
			{
				m_SkillButtons[l].EnableVisuals();
			}
			else if (!m_SkillButtonsData[l].enabled && !m_SkillButtons[l].m_VisualsDisabled)
			{
				m_SkillButtons[l].DisableVisuals();
			}
			m_SkillButtons[l].IsInUseMeState = m_SkillButtonsData[l].useme_state;
			m_SkillButtons[l].m_TooltipText = m_SkillButtonsData[l].tooltip_id;
			m_SkillButtons[l].m_Data = m_SkillButtonsData[l].data;
			if (m_SkillButtons[l].m_Data == 25)
			{
				if (m_SkillButtons[l].m_InactiveButtonOverCallback == null)
				{
					m_SkillButtons[l].m_InactiveButtonOverCallback = CFGSelectionManager.Instance.RenderFinderHelpers;
					m_SkillButtons[l].m_InactiveButtonOutCallback = CFGSelectionManager.Instance.DestroyFinderHelpers;
				}
			}
			else if (m_SkillButtons[l].m_Data == 35)
			{
				if (m_SkillButtons[l].m_InactiveButtonOverCallback == null)
				{
					m_SkillButtons[l].m_InactiveButtonOverCallback = CFGSelectionManager.Instance.RenderCannibalHelpers;
					m_SkillButtons[l].m_InactiveButtonOutCallback = CFGSelectionManager.Instance.DestroyCannibalHelpers;
				}
			}
			else if (m_SkillButtons[l].m_InactiveButtonOverCallback != null || m_SkillButtons[l].m_InactiveButtonOutCallback != null)
			{
				CFGButtonExtension cFGButtonExtension = m_SkillButtons[l];
				cFGButtonExtension.m_InactiveButtonOverCallback = (CFGButtonExtension.OnButtonOverDelegate)Delegate.Remove(cFGButtonExtension.m_InactiveButtonOverCallback, new CFGButtonExtension.OnButtonOverDelegate(CFGSelectionManager.Instance.RenderFinderHelpers));
				CFGButtonExtension cFGButtonExtension2 = m_SkillButtons[l];
				cFGButtonExtension2.m_InactiveButtonOverCallback = (CFGButtonExtension.OnButtonOverDelegate)Delegate.Remove(cFGButtonExtension2.m_InactiveButtonOverCallback, new CFGButtonExtension.OnButtonOverDelegate(CFGSelectionManager.Instance.RenderCannibalHelpers));
				CFGButtonExtension cFGButtonExtension3 = m_SkillButtons[l];
				cFGButtonExtension3.m_InactiveButtonOutCallback = (CFGButtonExtension.OnButtonOutDelegate)Delegate.Remove(cFGButtonExtension3.m_InactiveButtonOutCallback, new CFGButtonExtension.OnButtonOutDelegate(CFGSelectionManager.Instance.DestroyFinderHelpers));
				CFGButtonExtension cFGButtonExtension4 = m_SkillButtons[l];
				cFGButtonExtension4.m_InactiveButtonOutCallback = (CFGButtonExtension.OnButtonOutDelegate)Delegate.Remove(cFGButtonExtension4.m_InactiveButtonOutCallback, new CFGButtonExtension.OnButtonOutDelegate(CFGSelectionManager.Instance.DestroyCannibalHelpers));
			}
			CFGImageExtension[] componentsInChildren = m_SkillButtons[l].gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
			{
				if (cFGImageExtension.m_SpriteList.Count > 1)
				{
					cFGImageExtension.IconNumber = m_SkillButtonsData[l].icon;
				}
			}
			m_SkillButtons[l].m_Label.text = ((m_SkillButtonsData[l].uses_count > 0) ? m_SkillButtonsData[l].uses_count.ToString() : string.Empty);
			m_SkillCooldowns[l].text = ((m_SkillButtonsData[l].cooldown > 0) ? ("<color=#EA4242>" + m_SkillButtonsData[l].cooldown + "</color>") : string.Empty);
			m_SkillCooldown[l].gameObject.SetActive(m_SkillButtonsData[l].cooldown > 0);
			m_SkillDisabledFakeBg[l].gameObject.SetActive(m_SkillButtonsData[l].cooldown > 0);
			m_SkillDisabledFakeIcon[l].gameObject.SetActive(m_SkillButtonsData[l].cooldown > 0);
		}
		float width = m_SkillButtons[0].gameObject.GetComponent<RectTransform>().rect.width;
		m_ActionsPanel.transform.position = new Vector3(m_YPosSkills.x + (float)(10 - m_SkillButtonsData.Count) * 0.42f * width, m_ActionsPanel.transform.position.y, 0f);
		m_RTButtonPad.transform.position = new Vector3(m_YPosBtn.x - (float)(10 - m_SkillButtonsData.Count) * 0.42f * width, m_RTButtonPad.transform.position.y, m_RTButtonPad.transform.position.z);
		if (m_OldSkillsButtonCount != m_SkillButtonsData.Count && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_ShouldUpdateRaycasts = true;
			m_OldSkillsButtonCount = m_SkillButtonsData.Count;
		}
	}

	public void ClearCannibalAndFinderEvent()
	{
		if (m_SkillButtons == null || m_SkillButtons.Count == 0)
		{
			return;
		}
		foreach (CFGButtonExtension skillButton in m_SkillButtons)
		{
			if (skillButton.m_Data == 25 || skillButton.m_Data == 35)
			{
				skillButton.m_InactiveButtonOverCallback = null;
				skillButton.m_InactiveButtonOutCallback = null;
			}
		}
	}

	public void OnAbilityButton(int id)
	{
		CFGSelectionManager.Instance.OnAbilityClick(id);
	}

	public override void SetLocalisation()
	{
		m_EnemyTurnPanelText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_enemyturn");
	}
}
