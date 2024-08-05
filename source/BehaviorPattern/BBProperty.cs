using System;
using UnityEngine;

namespace BehaviorPattern;

public abstract class BBProperty : ScriptableObject
{
	[HideInInspector]
	public Blackboard m_Owner;

	public string Name
	{
		get
		{
			return base.name;
		}
		set
		{
			base.name = value;
		}
	}

	public abstract Type Type { get; }

	public virtual object RawValue => null;

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}

	public PropertyScope GetScope()
	{
		return PropertyScope.Personal;
	}

	public virtual void SetFromVariable(BehaviorComponent agent, BBVariable bbVar)
	{
	}
}
public abstract class BBProperty<T> : BBProperty
{
	[SerializeField]
	private T m_Value;

	public T Value
	{
		get
		{
			return m_Value;
		}
		set
		{
			if ((value != null || m_Value != null) && ((value != null && !value.Equals(m_Value)) || (m_Value != null && !m_Value.Equals(value))))
			{
				m_Value = value;
			}
		}
	}

	public override object RawValue => Value;

	public override Type Type => typeof(T);

	public override void SetFromVariable(BehaviorComponent agent, BBVariable bbVar)
	{
		BBVariable<T> bBVariable = bbVar as BBVariable<T>;
		if (bBVariable != null)
		{
			Value = bBVariable.GetValue(agent);
			Debug.Log("Setting value " + Value);
		}
	}

	public static implicit operator T(BBProperty<T> property)
	{
		return property.Value;
	}
}
