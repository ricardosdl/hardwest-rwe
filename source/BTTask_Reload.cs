public class BTTask_Reload : BTTask
{
	protected override EBTResult OnExecute(BehaviorTree agent)
	{
		return EBTResult.Fail;
	}

	public override void TickLatent(BehaviorTree agent)
	{
		FinishLatent(agent, EBTResult.Fail);
	}
}
