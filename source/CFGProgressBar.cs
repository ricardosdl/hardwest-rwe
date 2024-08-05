using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGProgressBar : UIBehaviour
{
	public Image m_Bg;

	public Image m_ProgressBar;

	protected override void Start()
	{
		base.Start();
		if (m_ProgressBar != null && m_Bg != null)
		{
			m_ProgressBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
			m_ProgressBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Bg.rectTransform.rect.height);
		}
	}

	public void SetProgress(int percent)
	{
		m_ProgressBar.rectTransform.anchorMin = m_Bg.rectTransform.anchorMin;
		m_ProgressBar.rectTransform.anchorMax = m_Bg.rectTransform.anchorMax - new Vector2(1 - percent / 100, 0f);
		m_ProgressBar.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_ProgressBar.rectTransform.offsetMin = new Vector2(0f, 0f);
	}
}
