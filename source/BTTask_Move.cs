public class BTTask_Move : BTTask
{
	protected override EBTResult OnExecute(BehaviorTree agent)
	{
		CFGCharacter component = agent.GetComponent<CFGCharacter>();
		if (component != null)
		{
			return EBTResult.Execution;
		}
		return EBTResult.Fail;
	}

	public override void TickLatent(BehaviorTree agent)
	{
		CFGCharacter component = agent.GetComponent<CFGCharacter>();
		if (component == null || component.ShouldFinishMove())
		{
			FinishLatent(agent, EBTResult.Success);
		}
	}
}
