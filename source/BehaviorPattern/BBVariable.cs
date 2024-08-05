using System;
using UnityEngine;

namespace BehaviorPattern;

public abstract class BBVariable : ScriptableObject
{
	[SerializeField]
	public BBPropertyRef m_Ref;

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}
}
public abstract class BBVariable<T> : BBVariable
{
	[SerializeField]
	public T Value;

	public Type Type => typeof(T);

	public T GetValue(BehaviorComponent agent)
	{
		BBProperty<T> bBProperty = null;
		if (m_Ref != null && m_Ref.m_Scope != 0 && m_Ref.m_KeyName != null)
		{
			PropertyScope scope = m_Ref.m_Scope;
			if (scope != 0 && scope == PropertyScope.Personal)
			{
				bBProperty = agent.BB.GetProperty<T>(m_Ref.m_KeyName);
			}
			if (bBProperty != null)
			{
				return bBProperty.Value;
			}
			Debug.LogWarning("BB Property " + m_Ref.m_KeyName + " not found !");
		}
		return Value;
	}
}
