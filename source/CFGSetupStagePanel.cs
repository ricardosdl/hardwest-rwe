using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGSetupStagePanel : CFGPanel
{
	public GameObject m_SetupStagePanel;

	public GameObject m_AlertsPanel;

	public List<CFGTextExtension> m_Alerts = new List<CFGTextExtension>();

	public CFGTextExtension m_SetupStageTitle;

	public CFGTextExtension m_SetupStageText;

	public CFGImageExtension m_SetupStageIcon;

	public Animator m_SetupStageAnimation;

	public Text m_SetupStageTextAnimation;

	public Image m_MaskSetupStagePanel;

	public Image m_BgSetupStagePanel;

	public List<CFGImageExtension> m_SetupStageFrame = new List<CFGImageExtension>();

	private Vector3 m_YPosAlerts = default(Vector3);

	private int m_CurrentSetupStageState = -1;

	private string m_CurrentSetupStageAnimText = string.Empty;

	private CFGImageExtension m_AnimatedImg;

	private Vector3 m_BottomLinePosition = default(Vector3);

	private Vector3 m_SSPosition = default(Vector3);

	private bool m_AnimatePos;

	private float m_LastTimeAnim = -1f;

	protected override void Start()
	{
		base.Start();
		m_YPosAlerts = m_AlertsPanel.transform.position;
		HideStagePanel();
		m_AnimatedImg = m_SetupStageAnimation.GetComponent<CFGImageExtension>();
		m_AnimatedImg.m_OnAnimateCallback = SetTextSetupStageAnim;
		m_SSPosition = m_SetupStageAnimation.gameObject.transform.position;
	}

	public void SetTextSetupStageAnim(string text, int state)
	{
		if (m_SetupStageAnimation.GetCurrentAnimatorStateInfo(0).IsName("Stop") || m_SetupStageAnimation.GetCurrentAnimatorStateInfo(0).IsName("Wyjazd"))
		{
			m_AnimatePos = true;
			return;
		}
		string empty = string.Empty;
		empty = ((state != 0) ? ("<color=#EA4242>" + m_CurrentSetupStageAnimText + "</color>") : m_CurrentSetupStageAnimText);
		m_SetupStageTextAnimation.text = empty;
		int length = text.Length;
		int length2 = m_CurrentSetupStageAnimText.Length;
		if (length2 < length)
		{
			m_CurrentSetupStageAnimText += text[length2];
		}
		else
		{
			m_SetupStageAnimation.SetTrigger("TextReady");
		}
	}

	public void ShowStagePanel(string title, string text, int state)
	{
		m_AlertsPanel.transform.position = m_YPosAlerts;
		string text2 = string.Empty;
		List<string> list = new List<string>();
		foreach (CFGCharacterData teamCharacters in CFGCharacterList.GetTeamCharactersList())
		{
			if (teamCharacters.Buffs.ContainsKey("critical_buff") && teamCharacters.Definition != null)
			{
				list.Add(teamCharacters.Definition.NameID);
			}
		}
		if (list.Count == 1)
		{
			text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[0]) + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_01");
		}
		else if (list.Count == 2)
		{
			text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[0]) + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_02") + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[1]) + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_01");
		}
		else if (list.Count > 0)
		{
			for (int i = 0; i < list.Count - 1; i++)
			{
				text2 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[i]) + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_03");
			}
			text2 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_02") + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[list.Count - 1]) + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_setup_critical_info_01");
		}
		string empty = string.Empty;
		empty = ((state != 0) ? ("<color=#EA4242>" + title + "</color>") : title);
		m_SetupStageTitle.text = empty;
		m_SetupStageText.text = text + "\n" + text2;
		m_SetupStageIcon.IconNumber = state;
		foreach (CFGImageExtension item in m_SetupStageFrame)
		{
			item.IconNumber = state;
		}
		m_SetupStagePanel.SetActive(value: true);
		if (m_CurrentSetupStageState != state)
		{
			m_CurrentSetupStageAnimText = string.Empty;
			m_SetupStageTextAnimation.text = string.Empty;
			m_AnimatedImg.m_DataString = title;
			m_AnimatedImg.m_Data = state;
			if (!m_SetupStageAnimation.IsInTransition(0) && m_SetupStageAnimation.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				m_SetupStageAnimation.SetTrigger("Update");
				m_SetupStageAnimation.gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
				m_SetupStageAnimation.SetTrigger("IsOnCenter");
			}
			m_CurrentSetupStageState = state;
		}
		float preferredHeight = m_SetupStageText.preferredHeight;
		float height = m_SetupStageText.rectTransform.rect.height;
		m_BgSetupStagePanel.transform.SetParent(m_MaskSetupStagePanel.transform.parent);
		m_BgSetupStagePanel.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_BgSetupStagePanel.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_BgSetupStagePanel.rectTransform.anchorMin = new Vector2(0f, 0f);
		m_BgSetupStagePanel.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_MaskSetupStagePanel.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_MaskSetupStagePanel.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_MaskSetupStagePanel.rectTransform.anchorMin = new Vector2(0f, 1f - preferredHeight / height - 0.2f);
		m_MaskSetupStagePanel.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_BgSetupStagePanel.transform.SetParent(m_MaskSetupStagePanel.transform);
		m_SetupStageFrame[1].transform.position = new Vector3(m_BottomLinePosition.x, m_MaskSetupStagePanel.transform.position.y, m_BottomLinePosition.y);
	}

	public void HideStagePanel()
	{
		m_AlertsPanel.transform.position = new Vector3(m_AlertsPanel.transform.position.x, m_SetupStagePanel.transform.position.y, 0f);
		m_SetupStagePanel.SetActive(value: false);
	}

	public void ShowAlert(string alert_text)
	{
		Image image = null;
		for (int num = m_Alerts.Count - 1; num >= 1; num--)
		{
			m_Alerts[num].text = m_Alerts[num - 1].text;
			m_Alerts[num].color = m_Alerts[num - 1].color;
			image = m_Alerts[num].gameObject.GetComponentInChildren<Image>();
			if (image != null)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, m_Alerts[num].color.a);
			}
		}
		m_Alerts[0].text = alert_text;
		m_Alerts[0].color = new Color(m_Alerts[0].color.r, m_Alerts[0].color.g, m_Alerts[0].color.b, 1f);
		image = m_Alerts[0].gameObject.GetComponentInChildren<Image>();
		if (image != null)
		{
			image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
		}
	}

	public void SetAlertsPanelVisibility(bool visible)
	{
		m_AlertsPanel.SetActive(visible);
	}

	public override void Update()
	{
		base.Update();
		foreach (CFGTextExtension alert in m_Alerts)
		{
			Image componentInChildren = alert.gameObject.GetComponentInChildren<Image>();
			if (componentInChildren != null && (alert.color.a > 0f || componentInChildren.color.a > 0f))
			{
				alert.color = new Color(alert.color.r, alert.color.g, alert.color.b, alert.color.a - Time.deltaTime * 0.1f);
				componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, componentInChildren.color.a - Time.deltaTime * 0.1f);
			}
		}
		if (m_AnimatePos && m_LastTimeAnim + 0.01f < Time.time)
		{
			m_LastTimeAnim = Time.time;
			if (m_SetupStageAnimation.gameObject.transform.position == m_SSPosition)
			{
				m_AnimatePos = false;
			}
			else if (m_SetupStageAnimation.gameObject.transform.position.x - m_SSPosition.x < 0.01f || m_SSPosition.y - m_SetupStageAnimation.gameObject.transform.position.y < 0.01f)
			{
				m_SetupStageAnimation.gameObject.transform.position = m_SSPosition;
			}
			else
			{
				m_SetupStageAnimation.gameObject.transform.position -= (m_SetupStageAnimation.gameObject.transform.position - m_SSPosition) / 10f;
			}
		}
	}
}
