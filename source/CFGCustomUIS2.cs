using System.Collections.Generic;
using UnityEngine;

public class CFGCustomUIS2 : CFGPanel
{
	public Animator m_Animator;

	public List<GameObject> m_ElementsList = new List<GameObject>();

	protected override void Start()
	{
		base.Start();
		base.transform.position = new Vector3(0f, 0f);
	}
}
