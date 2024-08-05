using System;
using UnityEngine;

public abstract class BTObject : ScriptableObject
{
	[NonSerialized]
	[HideInInspector]
	public BTTemplate m_Owner;

	[HideInInspector]
	[SerializeField]
	protected int m_DataOffset;

	public virtual void PostLoad(BTTemplate template)
	{
		m_Owner = template;
	}

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}
}
