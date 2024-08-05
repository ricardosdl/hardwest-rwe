using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGObjectivesPanel : CFGPanel
{
	public List<Text> m_ObjsTexts = new List<Text>();

	public List<CFGImageExtension> m_ObjsIcons = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_ObjsIconsAnimations = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_ObjsIconsAnimationsCompleted = new List<CFGImageExtension>();

	public Color m_ActiveObj = default(Color);

	public Color m_CompletedObj = default(Color);

	public GameObject m_Frame;

	private Vector3 m_YPos = default(Vector3);

	private List<Vector3> m_YLinePos = new List<Vector3>();

	public float m_MultilineBias = 15f;

	public float m_PerLineOffset2 = 37f;

	public void SetObjectivesNumber(int number)
	{
		bool activeSelf = m_Frame.activeSelf;
		for (int i = 0; i < m_ObjsTexts.Count; i++)
		{
			m_ObjsTexts[i].transform.parent.gameObject.SetActive(value: false);
		}
		for (int j = 0; j < number; j++)
		{
			m_ObjsTexts[j].transform.parent.gameObject.SetActive(value: true);
		}
		if ((bool)m_Frame)
		{
			m_Frame.SetActive(number > 0);
		}
		if ((number <= 0 || activeSelf) && number == 0 && !activeSelf)
		{
		}
	}

	public void SetObjState(int number, int state, int type, bool is_dirty, EObjectiveImportance importance)
	{
		if (importance == EObjectiveImportance.Main)
		{
			m_ObjsIcons[number].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ObjMainIcons;
		}
		else
		{
			m_ObjsIcons[number].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ObjSecondIcons;
		}
		if (number < m_ObjsIcons.Count)
		{
			m_ObjsIcons[number].IconNumber = type;
			m_ObjsIcons[number].gameObject.SetActive(state != 2);
		}
		if (number < m_ObjsIconsAnimations.Count)
		{
			m_ObjsIconsAnimations[number].IconNumber = type;
			m_ObjsIconsAnimations[number].gameObject.SetActive(state != 2);
		}
		if (number < m_ObjsIconsAnimations.Count && is_dirty)
		{
			Animator component = m_ObjsIconsAnimations[number].gameObject.GetComponent<Animator>();
			if (component.enabled)
			{
				component.SetTrigger("Update");
			}
		}
		if (state != 2)
		{
			Animator component2 = m_ObjsIconsAnimationsCompleted[number].gameObject.GetComponent<Animator>();
			if (!component2.IsInTransition(0) && component2.GetCurrentAnimatorStateInfo(0).IsTag("pauza"))
			{
				component2.SetTrigger("Reset");
			}
		}
		if (state == 2)
		{
			if (number < m_ObjsTexts.Count)
			{
				m_ObjsTexts[number].color = m_CompletedObj;
			}
			if (number < m_ObjsIconsAnimationsCompleted.Count && is_dirty)
			{
				Animator component3 = m_ObjsIconsAnimationsCompleted[number].gameObject.GetComponent<Animator>();
				if (component3.enabled && m_ObjsIconsAnimationsCompleted[number].gameObject.activeSelf)
				{
					component3.SetTrigger("Start");
				}
			}
		}
		else if (number < m_ObjsTexts.Count)
		{
			m_ObjsTexts[number].color = m_ActiveObj;
		}
	}

	protected override void Start()
	{
		base.Start();
		CheckField(m_Frame, "Frame");
		foreach (Text objsText in m_ObjsTexts)
		{
			objsText.color = m_ActiveObj;
			m_YLinePos.Add(objsText.transform.parent.gameObject.GetComponent<RectTransform>().localPosition);
		}
		foreach (CFGImageExtension objsIcon in m_ObjsIcons)
		{
			objsIcon.IconNumber = 0;
		}
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		float y = 145f * num;
		if (num > 1f)
		{
			y = 170f;
		}
		GetComponent<RectTransform>().transform.Translate(0f, y, 0f);
		m_YPos = GetComponent<RectTransform>().localPosition;
		if ((bool)m_Frame)
		{
			m_Frame.SetActive(m_ObjsTexts.Count > 0 && m_ObjsTexts[0] != null && m_ObjsTexts[0].text != string.Empty);
		}
	}

	public override void Update()
	{
		base.Update();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		float num4 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		for (int i = 0; i < m_ObjsTexts.Count; i++)
		{
			RectTransform component = m_ObjsTexts[i].transform.parent.gameObject.GetComponent<RectTransform>();
			if (component.localPosition != m_YLinePos[i])
			{
				component.localPosition = m_YLinePos[i];
			}
			if (m_ObjsTexts[i].transform.parent.gameObject.activeSelf)
			{
				num3++;
				int lineCount = m_ObjsTexts[i].cachedTextGenerator.lineCount;
				num += lineCount;
				if (lineCount > 1)
				{
					num2 += lineCount - 1;
				}
				if (num2 > 0)
				{
					component.Translate(0f, (float)num2 * m_MultilineBias * num4, 0f);
				}
			}
		}
		RectTransform component2 = GetComponent<RectTransform>();
		if (component2.localPosition != m_YPos)
		{
			component2.localPosition = m_YPos;
		}
		float num5 = (float)num2 * m_MultilineBias * num4;
		if (num3 > 1)
		{
			num5 += (float)(num3 - 1) * m_PerLineOffset2 * num4;
		}
		component2.Translate(0f, 0f - num5, 0f);
	}
}
