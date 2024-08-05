public abstract class BTTask : BTNode, IBTLatent
{
	public virtual bool IsLatentTask => true;

	protected override void NotifyExecution(BehaviorTree agent, EBTResult result)
	{
		base.NotifyExecution(agent, result);
		if (result == EBTResult.Execution)
		{
			agent.RegisterLatent(this);
		}
	}

	public virtual void TickLatent(BehaviorTree agent)
	{
	}

	public void FinishLatent(BehaviorTree agent, EBTResult result)
	{
		agent.UnRegigterLatent(this);
		ExecutionFinished(agent, result);
	}

	public override void OnNodeAborted(BehaviorTree agent)
	{
		if (IsLatentTask)
		{
			agent.UnRegigterLatent(this);
		}
	}
}
