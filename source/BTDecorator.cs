using UnityEngine;

public abstract class BTDecorator : BTObject
{
	[HideInInspector]
	public BTNode m_PrentNode;

	public EExecOrder m_ExecutionOrder;

	public virtual bool NotifyExecution => false;

	public virtual bool NotifyAbortion => false;

	public virtual EBTResult OnExecutionStart(BehaviorTree agent)
	{
		return EBTResult.Success;
	}

	public virtual EBTResult OnExecutionEnd(BehaviorTree agent, EBTResult result)
	{
		return result;
	}

	public virtual void OnNodeExecuted(BehaviorTree agent, EBTResult result)
	{
	}

	public virtual void OnNodeAborted(BehaviorTree agent)
	{
	}
}
