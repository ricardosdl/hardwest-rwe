using System;
using UnityEngine;

namespace BehaviorPattern;

[Serializable]
public class Operation<OP_TYPE> : IOperation where OP_TYPE : IConvertible
{
	[SerializeField]
	public OP_TYPE m_Operator = default(OP_TYPE);

	public Type OpType => m_Operator.GetType();

	public int OpIndex
	{
		get
		{
			return m_Operator.ToInt32(null);
		}
		set
		{
			m_Operator = (OP_TYPE)(object)value;
		}
	}

	public virtual bool Evaluate<T>(params T[] args)
	{
		return false;
	}
}
