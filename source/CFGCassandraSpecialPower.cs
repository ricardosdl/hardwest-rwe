using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGCassandraSpecialPower : CFGPanel
{
	public CFGImageExtension m_CassandraFace;

	public List<Image> m_PowerBar = new List<Image>();

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

	public override void SetLocalisation()
	{
		base.gameObject.GetComponentInChildren<Text>().text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cassandra_power");
	}

	public void SetProgress(int progress)
	{
		int num = 0;
		num = progress switch
		{
			0 => 6, 
			1 => 4, 
			2 => 2, 
			_ => 0, 
		};
		for (int i = 0; i < m_PowerBar.Count; i++)
		{
			m_PowerBar[i].enabled = i < num;
		}
		m_CassandraFace.IconNumber = ((num == 6) ? 1 : 0);
		foreach (CFGImageExtension item in m_PowerBar)
		{
			item.IconNumber = ((num != 6) ? 1 : 0);
		}
	}
}
