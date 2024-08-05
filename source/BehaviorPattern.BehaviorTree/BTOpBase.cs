using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("Op")]
public abstract class BTOpBase : BTObject
{
	public bool m_Result;

	public virtual bool Evaluate(BehaviorComponent agent)
	{
		return RawEvaluate(agent);
	}

	protected virtual bool RawEvaluate(BehaviorComponent agent)
	{
		return false;
	}
}
