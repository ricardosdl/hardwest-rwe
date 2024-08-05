using UnityEngine;

public class CFGHUDSave : CFGPanel
{
	public GameObject m_Save;

	protected override void Start()
	{
		base.Start();
		CFG_SG_Manager.m_SaveEnd = SaveEndAnim;
	}

	public void SaveEndAnim()
	{
		if (m_Save != null)
		{
			m_Save.SetActive(value: true);
		}
	}
}
