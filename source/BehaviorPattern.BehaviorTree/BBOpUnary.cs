namespace BehaviorPattern.BehaviorTree;

public abstract class BBOpUnary<T, OP> : BBOp<OP> where OP : IOperation, new()
{
	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return false;
	}
}
