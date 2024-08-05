using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("AND")]
public class BTOp_AND : BTOpLogic
{
	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		if (m_Operands.Count > 0)
		{
			return true;
		}
		return false;
	}
}
