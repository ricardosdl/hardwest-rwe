namespace BehaviorPattern.BehaviorTree;

public abstract class BBOpBinary<VAR, T, OP> : BBOp<OP> where VAR : BBVariable<T> where OP : IOperation, new()
{
	public VAR m_OperandA;

	public VAR m_OperandB;

	public override bool Evaluate(BehaviorComponent agent)
	{
		return RawEvaluate(agent);
	}

	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return m_Op.Evaluate<T>(m_OperandA.GetValue(agent), m_OperandB.GetValue(agent));
	}
}
