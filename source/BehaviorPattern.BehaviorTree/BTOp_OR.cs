using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("OR")]
public class BTOp_OR : BTOpLogic
{
	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		if (m_Operands.Count > 0)
		{
			return false;
		}
		return false;
	}
}
