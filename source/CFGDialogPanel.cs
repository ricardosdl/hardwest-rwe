using UnityEngine;
using UnityEngine.UI;

public class CFGDialogPanel : CFGPanel
{
	public CFGImageExtension m_CharacterIcon;

	public Text m_TitleText;

	public Text m_DialogText;

	public Image m_Bg;

	public CFGImageExtension m_BottomLine;

	public Image m_Mask;

	public CFGImageExtension m_TopLine;

	public CFGImageExtension m_Frame;

	private int m_LastId = -1;

	private Vector3 m_BottomLinePosition = default(Vector3);

	protected override void Start()
	{
		base.Start();
		CheckField(m_CharacterIcon, "CharacterIcon");
		CheckField(m_TitleText, "TitleText");
		CheckField(m_DialogText, "DialogText");
		CheckField(m_TopLine, "TopLine");
		CheckField(m_Frame, "Frame");
		CheckField(m_Bg, "Bg");
		CheckField(m_BottomLine, "BottomLine");
		CheckField(m_Mask, "Mask");
		GetComponent<CanvasGroup>().alpha = 0f;
		if (m_CharacterIcon != null)
		{
			m_CharacterIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		}
	}

	public void SetCurrentDialog(string title, string text, int char_id)
	{
		GetComponent<CanvasGroup>().alpha = 0f;
		if ((bool)m_TitleText)
		{
			m_TitleText.text = title;
		}
		if ((bool)m_DialogText)
		{
			m_DialogText.text = text;
		}
		if (m_LastId != char_id)
		{
			if ((bool)m_BottomLine)
			{
				m_BottomLine.IconNumber = 1;
			}
			if ((bool)m_TopLine)
			{
				m_TopLine.IconNumber = 1;
			}
			if ((bool)m_Frame)
			{
				m_Frame.IconNumber = 1;
			}
			if ((bool)m_CharacterIcon)
			{
				m_CharacterIcon.IconNumber = char_id;
			}
			m_LastId = char_id;
		}
		else
		{
			if ((bool)m_BottomLine)
			{
				m_BottomLine.IconNumber = 0;
			}
			if ((bool)m_TopLine)
			{
				m_TopLine.IconNumber = 0;
			}
			if ((bool)m_Frame)
			{
				m_Frame.IconNumber = 0;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		int num = 0;
		if (m_DialogText != null)
		{
			num = m_DialogText.cachedTextGenerator.lineCount;
		}
		m_Bg.transform.SetParent(m_Mask.transform.parent);
		m_Bg.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Bg.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Mask.rectTransform.anchorMin = new Vector2(0f, 0.78f - (float)num * 0.06f);
		m_Mask.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_Bg.transform.SetParent(m_Mask.transform);
		m_BottomLine.transform.position = new Vector3(m_BottomLine.transform.position.x, m_Mask.transform.position.y, m_BottomLine.transform.position.z);
		GetComponent<CanvasGroup>().alpha = 1f;
	}
}
