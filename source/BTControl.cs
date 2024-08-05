using System.Collections.Generic;
using UnityEngine;

public abstract class BTControl : BTNode, IBTLatent
{
	[HideInInspector]
	public List<BTNode> m_Children = new List<BTNode>();

	protected override bool WantsPostExecution => true;

	public virtual void ChildExecutionFinished(BehaviorTree agent, BTNode child, EBTResult result)
	{
	}

	public override void PostLoad(BTTemplate template)
	{
		base.PostLoad(template);
	}

	protected override EBTResult OnPostExecute(BehaviorTree agent, EBTResult inResult, EBTResult decorationResult)
	{
		if (decorationResult == EBTResult.Execution)
		{
			agent.RegisterLatent(this);
		}
		return decorationResult;
	}

	public virtual void TickLatent(BehaviorTree agent)
	{
		agent.UnRegigterLatent(this);
		EBTResult eBTResult = OnExecute(agent);
		if (eBTResult != EBTResult.Execution)
		{
			ExecutionFinished(agent, eBTResult);
		}
	}

	public override void GetNodes(ref List<BTNode> nodes)
	{
		base.GetNodes(ref nodes);
		foreach (BTNode child in m_Children)
		{
			child.GetNodes(ref nodes);
		}
	}
}
