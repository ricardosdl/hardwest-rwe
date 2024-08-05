using System;
using BehaviorPattern;
using BehaviorPattern.BehaviorTree;

[Serializable]
public class BTOpRef
{
	public BTOpBase m_Op;

	public int m_ResultIdx = -1;

	public BTOpRef(BTOpBase op, int resultIdx)
	{
		m_Op = op;
		m_ResultIdx = 0;
	}

	public bool Evaluate(BehaviorComponent agent)
	{
		bool result = false;
		if (m_Op != null)
		{
			result = m_Op.Evaluate(agent);
		}
		return result;
	}
}
