using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("Logic Op")]
public abstract class BTOpLogic : BTOpBase
{
	public List<BTOpRef> m_Operands = new List<BTOpRef>();

	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return true;
	}

	public virtual void ChildChanged(BehaviorComponent agent, bool value)
	{
	}
}
