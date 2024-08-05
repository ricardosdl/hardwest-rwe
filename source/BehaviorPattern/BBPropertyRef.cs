using System;
using UnityEngine;

namespace BehaviorPattern;

[Serializable]
public class BBPropertyRef
{
	[SerializeField]
	public string m_KeyName;

	[SerializeField]
	public PropertyScope m_Scope;

	public BBPropertyRef()
	{
		m_KeyName = null;
		m_Scope = PropertyScope.None;
	}

	public BBPropertyRef(BBProperty property)
	{
		m_KeyName = property.Name;
		m_Scope = property.GetScope();
	}

	public BBProperty GetProperty(BehaviorComponent agent)
	{
		if (m_Scope != 0 && m_KeyName != null)
		{
			BBProperty bBProperty = null;
			PropertyScope scope = m_Scope;
			if (scope != 0 && scope == PropertyScope.Personal)
			{
				bBProperty = agent.BB.GetProperty(m_KeyName);
			}
			if (bBProperty != null)
			{
				return bBProperty;
			}
			Debug.LogWarning(string.Concat("BB Property ", m_KeyName, " [", m_Scope, "] not found !"));
		}
		return null;
	}

	public BBProperty<T> GetProperty<T>(BehaviorComponent agent)
	{
		if (m_Scope != 0 && m_KeyName != null)
		{
			BBProperty<T> bBProperty = null;
			PropertyScope scope = m_Scope;
			if (scope != 0 && scope == PropertyScope.Personal)
			{
				bBProperty = agent.BB.GetProperty<T>(m_KeyName);
			}
			if (bBProperty != null)
			{
				return bBProperty;
			}
			Debug.LogWarning(string.Concat("BB Property ", m_KeyName, " [", m_Scope, "] not found !"));
		}
		return null;
	}
}
