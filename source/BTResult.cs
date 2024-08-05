public class BTResult : BTDecorator
{
	public EBTResult m_ReturnResult;

	public override EBTResult OnExecutionStart(BehaviorTree agent)
	{
		if (m_ExecutionOrder != EExecOrder.OnFinished)
		{
			return m_ReturnResult;
		}
		return EBTResult.Success;
	}

	public override EBTResult OnExecutionEnd(BehaviorTree agent, EBTResult result)
	{
		if (m_ExecutionOrder != 0)
		{
			return m_ReturnResult;
		}
		return result;
	}
}
