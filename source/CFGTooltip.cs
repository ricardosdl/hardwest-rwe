using UnityEngine;
using UnityEngine.UI;

public class CFGTooltip : CFGPanel
{
	public Text m_TooltipText;

	public Image m_Bg;

	public float m_Height = 1f;

	private Vector2 m_AnchorMax = Vector2.zero;

	public string TooltipText
	{
		set
		{
			m_AnchorMax = GetComponent<RectTransform>().anchorMax;
			m_TooltipText.text = value;
			TextGenerator cachedTextGenerator = m_TooltipText.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_TooltipText.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(value, generationSettings);
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchorMax = m_AnchorMax;
		}
	}

	protected override void Start()
	{
		base.Start();
		m_AnchorMax = GetComponent<RectTransform>().anchorMax;
	}

	public override void Update()
	{
		RectTransform component = GetComponent<RectTransform>();
		Vector2 anchorMax = m_AnchorMax;
		if (m_TooltipText.cachedTextGenerator.lineCount > 1)
		{
			anchorMax.y = m_AnchorMax.y * (float)m_TooltipText.cachedTextGenerator.lineCount * 0.35f;
		}
		else if (m_TooltipText.cachedTextGenerator.lineCount == 1)
		{
			anchorMax.y = m_AnchorMax.y * (float)m_TooltipText.cachedTextGenerator.lineCount * 0.65f;
		}
		component.anchorMax = anchorMax;
		m_Height = component.rect.height;
		Vector3 mousePosition = Input.mousePosition;
		if (Input.mousePosition.x > (float)(Screen.width - 140 * Screen.width / 1600))
		{
			mousePosition += new Vector3(-140 * Screen.width / 1600, 0f, 0f);
		}
		if (Input.mousePosition.y < (float)(20 * Screen.height / 1600))
		{
			mousePosition += new Vector3(0f, 20 * Screen.height / 900, 0f);
		}
		if (Input.mousePosition.y + m_Height > (float)Screen.height)
		{
			mousePosition += new Vector3(0f, (0f - m_Height) * 1.2f, 0f);
		}
		base.transform.position = mousePosition;
		CanvasGroup component2 = base.gameObject.GetComponent<CanvasGroup>();
		if ((bool)component2 && component2.alpha == 0f)
		{
			component2.alpha = 1f;
		}
	}
}
