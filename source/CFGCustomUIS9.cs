using System.Collections.Generic;
using UnityEngine;

public class CFGCustomUIS9 : CFGPanel
{
	[SerializeField]
	private CFGDLCPanelElement[] m_Elements;

	private bool m_IsInitialized;

	protected override void Start()
	{
		base.Start();
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(0f, 0f);
		UpdatePanel();
		if (m_Elements == null)
		{
			return;
		}
		for (int i = 0; i < m_Elements.Length; i++)
		{
			if (!(m_Elements[i] == null))
			{
				RectTransform component2 = m_Elements[i].GetComponent<RectTransform>();
				if (!(component2 == null))
				{
					m_Elements[i].StartPosition = component2.transform.position;
				}
			}
		}
		m_IsInitialized = true;
	}

	public void UpdatePanel()
	{
		if (m_Elements == null || m_Elements.Length == 0)
		{
			return;
		}
		List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
		for (int i = 0; i < m_Elements.Length; i++)
		{
			if (!(m_Elements[i] == null))
			{
				if (teamCharactersList.Count <= i)
				{
					m_Elements[i].gameObject.SetActive(value: false);
				}
				else
				{
					m_Elements[i].gameObject.SetActive(value: true);
					CFGCharacterData characterData = teamCharactersList[i];
					m_Elements[i].UpdateData(characterData);
				}
				if (m_IsInitialized)
				{
					Vector3 vector = new Vector3(0f, (0f - m_Elements[1].Height) * (float)(m_Elements.Length - teamCharactersList.Count), 0f);
					Vector3 position = m_Elements[i].StartPosition + vector;
					m_Elements[i].GetComponent<RectTransform>().transform.position = position;
				}
			}
		}
	}

	public void SetReward(int reward)
	{
	}

	public void SetRewardDesc(int deaths, int damage)
	{
	}

	public void SetIcons(int ic_gun, int ic_elixir)
	{
	}

	public override void SetLocalisation()
	{
	}
}
