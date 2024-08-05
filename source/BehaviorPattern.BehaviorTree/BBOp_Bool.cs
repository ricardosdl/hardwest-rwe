using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("BB Op")]
[Node("BB Bool")]
public class BBOp_Bool : BTOpBase
{
	public BBBool m_Value;

	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return false;
	}
}
