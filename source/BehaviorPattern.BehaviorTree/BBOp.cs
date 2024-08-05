using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("BB Op")]
public abstract class BBOp<OP> : BTOpBase where OP : IOperation, new()
{
	[Body("")]
	public OP m_Op = new OP();

	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return true;
	}
}
