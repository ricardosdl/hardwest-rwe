using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGCustomUIS5 : CFGPanel
{
	public List<Image> m_ChemistryLvl = new List<Image>();

	public List<Image> m_EngineeringLvl = new List<Image>();

	public List<Image> m_GunsmithLvl = new List<Image>();

	public Image m_Marker;

	public Image m_MarkerParent;

	public GameObject m_Madness;

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
	}

	public void SetScienceLevels(int chem, int eng, int gun)
	{
		for (int i = 0; i < m_ChemistryLvl.Count; i++)
		{
			m_ChemistryLvl[i].gameObject.SetActive(i < chem);
		}
		for (int j = 0; j < m_EngineeringLvl.Count; j++)
		{
			m_EngineeringLvl[j].gameObject.SetActive(j < eng);
		}
		for (int k = 0; k < m_GunsmithLvl.Count; k++)
		{
			m_GunsmithLvl[k].gameObject.SetActive(k < gun);
		}
	}

	public void SetMadnessLevel(int madness)
	{
		RectTransform rectTransform = m_MarkerParent.rectTransform;
		float width = rectTransform.rect.width;
		m_Marker.transform.position = new Vector3(m_MarkerParent.transform.position.x + rectTransform.rect.width * (float)madness / 20f, m_MarkerParent.transform.position.y, m_MarkerParent.transform.position.z);
	}

	public void SetMadnessVisible(bool visible)
	{
		m_Madness.gameObject.SetActive(visible);
		m_MarkerParent.gameObject.SetActive(visible);
	}
}
