using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGTextExtension : Text
{
	public delegate void OnAnimEndDelegate();

	public delegate void OnAnimBeginDelegate();

	public static List<CFGTextExtension> m_Texts = new List<CFGTextExtension>();

	private int m_NormalFontSize = 1;

	private List<string> m_WordAfterWordText = new List<string>();

	private List<float> m_WordAfterWordAlpha = new List<float>();

	public float m_AnimSpeed = 0.05f;

	private int m_AnimPos;

	public Color m_AnimColor = default(Color);

	public float m_LastTimeWord = -1f;

	public bool m_StaticFontSize;

	public OnAnimEndDelegate m_AnimEndCallback;

	public OnAnimBeginDelegate m_AnimBeginCallback;

	protected override void Start()
	{
		base.Start();
		m_NormalFontSize = base.fontSize;
		OnResolutionChange();
		m_Texts.Add(this);
		CFGButtonExtension component = base.gameObject.GetComponent<CFGButtonExtension>();
		if ((bool)component)
		{
			component.m_ButtonClickedCallback = OnTextClick;
		}
		m_AnimSpeed = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_StrategicExploratorAnimSpeed;
		if (CFGOptions.Gameplay.ExplorationTextSpeed == 1)
		{
			m_AnimSpeed *= 0.25f;
		}
	}

	public void ResetClickable()
	{
		CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
		if ((bool)component)
		{
			component.blocksRaycasts = CFGOptions.Gameplay.ExplorationTextSpeed != 2;
		}
	}

	protected void Update()
	{
		if (m_WordAfterWordText.Count <= 0 || !(m_LastTimeWord + m_AnimSpeed < Time.time))
		{
			return;
		}
		string text = string.Empty;
		for (int i = 0; i < m_WordAfterWordAlpha.Count; i++)
		{
			if (m_WordAfterWordAlpha[i] > 0f)
			{
				List<float> wordAfterWordAlpha;
				List<float> list = (wordAfterWordAlpha = m_WordAfterWordAlpha);
				int index;
				int index2 = (index = i);
				float num = wordAfterWordAlpha[index];
				list[index2] = num + 0.04f;
			}
		}
		for (int j = 0; j < m_WordAfterWordText.Count; j++)
		{
			if (j == m_AnimPos)
			{
				List<float> wordAfterWordAlpha2;
				List<float> list2 = (wordAfterWordAlpha2 = m_WordAfterWordAlpha);
				int index;
				int index3 = (index = j);
				float num = wordAfterWordAlpha2[index];
				list2[index3] = num + 0.04f;
			}
			Color color = new Color(m_AnimColor.r, m_AnimColor.g, m_AnimColor.b, m_WordAfterWordAlpha[j]);
			string text2 = text;
			text = text2 + "<color=" + ColorToRGBA(color) + ">" + m_WordAfterWordText[j] + "</color>";
			text += " ";
		}
		m_AnimPos++;
		this.text = text;
		if (m_WordAfterWordAlpha[m_WordAfterWordAlpha.Count - 1] >= 1f)
		{
			m_WordAfterWordText.Clear();
			m_WordAfterWordAlpha.Clear();
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				component.blocksRaycasts = false;
			}
			if (m_AnimEndCallback != null)
			{
				m_AnimEndCallback();
			}
		}
		m_LastTimeWord = Time.time;
	}

	public void OnTextClick(int a)
	{
		if (m_WordAfterWordText.Count > 0)
		{
			string text = m_WordAfterWordText[0];
			for (int i = 1; i < m_WordAfterWordText.Count; i++)
			{
				text = text + " " + m_WordAfterWordText[i];
			}
			this.text = text;
			m_WordAfterWordText.Clear();
			m_WordAfterWordAlpha.Clear();
			CanvasGroup component = base.gameObject.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				component.blocksRaycasts = false;
			}
			if (m_AnimEndCallback != null)
			{
				m_AnimEndCallback();
			}
		}
	}

	public string ColorToRGBA(Color color)
	{
		return string.Format("#{0}{1}{2}{3}", ((int)(color.r * 255f)).ToString("X2"), ((int)(color.g * 255f)).ToString("X2"), ((int)(color.b * 255f)).ToString("X2"), ((int)(color.a * 255f)).ToString("X2"));
	}

	protected override void OnDestroy()
	{
		m_Texts.Remove(this);
		base.OnDestroy();
	}

	public void OnResolutionChange()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance && (bool)instance.m_UICanvas)
		{
			RectTransform component = instance.m_UICanvas.GetComponent<RectTransform>();
			int num = 1;
			num = ((!base.resizeTextForBestFit) ? m_NormalFontSize : base.resizeTextMaxSize);
			if (Screen.height > 900 && Screen.width > 1600)
			{
				base.resizeTextForBestFit = false;
			}
			base.fontSize = Mathf.FloorToInt(Mathf.Min((float)num * component.rect.width / 1600f, (float)num * component.rect.height / 900f));
		}
	}

	public void WordAfterWordText(string text_to)
	{
		if (CFGOptions.Gameplay.ExplorationTextSpeed == 2)
		{
			if (m_AnimBeginCallback != null)
			{
				m_AnimBeginCallback();
			}
			if (m_AnimEndCallback != null)
			{
				m_AnimEndCallback();
			}
			text = text_to;
			return;
		}
		if (m_AnimBeginCallback != null)
		{
			m_AnimBeginCallback();
		}
		text = string.Empty;
		m_WordAfterWordText.Clear();
		m_WordAfterWordAlpha.Clear();
		m_AnimPos = 0;
		string[] array = text_to.Split(' ');
		foreach (string item in array)
		{
			m_WordAfterWordText.Add(item);
			m_WordAfterWordAlpha.Add(0f);
		}
	}
}
