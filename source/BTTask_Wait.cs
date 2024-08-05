public class BTTask_Wait : BTTask
{
	public float m_WaitTime;

	protected override EBTResult OnExecute(BehaviorTree agent)
	{
		return EBTResult.Execution;
	}

	public override void TickLatent(BehaviorTree agent)
	{
	}
}
