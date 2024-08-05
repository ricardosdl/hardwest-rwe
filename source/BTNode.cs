using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode : BTObject
{
	[HideInInspector]
	public BTControl m_Parent;

	public string m_NodeName = string.Empty;

	public List<BTDecorator> m_Decorators = new List<BTDecorator>();

	protected virtual bool WantsPostExecution => false;

	public override void PostLoad(BTTemplate template)
	{
		base.PostLoad(template);
		for (int i = 0; i < m_Decorators.Count; i++)
		{
			m_Decorators[i].PostLoad(template);
		}
	}

	public EBTResult Execute(BehaviorTree agent, out EBTResult decorationResult)
	{
		decorationResult = DecorateExecutionStart(agent);
		EBTResult eBTResult = decorationResult;
		if (eBTResult == EBTResult.Success)
		{
			eBTResult = OnExecute(agent);
			NotifyExecution(agent, eBTResult);
			if (eBTResult == EBTResult.Execution)
			{
				return eBTResult;
			}
		}
		decorationResult = DecorateExecutionEnd(agent, eBTResult);
		if (WantsPostExecution)
		{
			return OnPostExecute(agent, eBTResult, decorationResult);
		}
		return decorationResult;
	}

	protected virtual EBTResult OnExecute(BehaviorTree agent)
	{
		return EBTResult.Success;
	}

	protected virtual EBTResult OnPostExecute(BehaviorTree agent, EBTResult inResult, EBTResult decorationResult)
	{
		return decorationResult;
	}

	protected EBTResult DecorateExecutionStart(BehaviorTree agent)
	{
		EBTResult eBTResult = EBTResult.Success;
		int num = 0;
		if (m_Decorators.Count > 0)
		{
			while (eBTResult == EBTResult.Success && num < m_Decorators.Count)
			{
				eBTResult = m_Decorators[num++].OnExecutionStart(agent);
			}
		}
		return eBTResult;
	}

	protected EBTResult DecorateExecutionEnd(BehaviorTree agent, EBTResult withResult)
	{
		EBTResult eBTResult = withResult;
		int i = 0;
		if (m_Decorators.Count > 0)
		{
			if (eBTResult != EBTResult.Abort)
			{
				while (eBTResult != EBTResult.Execution && i < m_Decorators.Count)
				{
					eBTResult = m_Decorators[i++].OnExecutionEnd(agent, eBTResult);
				}
			}
			else
			{
				for (; i < m_Decorators.Count; i++)
				{
					if (m_Decorators[i].NotifyAbortion)
					{
						m_Decorators[i].OnNodeAborted(agent);
					}
				}
			}
		}
		return eBTResult;
	}

	protected virtual void NotifyExecution(BehaviorTree agent, EBTResult result)
	{
		for (int i = 0; i < m_Decorators.Count; i++)
		{
			if (m_Decorators[i].NotifyExecution)
			{
				m_Decorators[i].OnNodeExecuted(agent, result);
			}
		}
	}

	protected void ExecutionFinished(BehaviorTree agent, EBTResult result)
	{
		EBTResult eBTResult = DecorateExecutionEnd(agent, result);
		if (WantsPostExecution)
		{
			result = OnPostExecute(agent, result, eBTResult);
		}
		else if (eBTResult == EBTResult.Abort || eBTResult == EBTResult.Error)
		{
			result = eBTResult;
		}
		if (m_Parent != null)
		{
			m_Parent.ChildExecutionFinished(agent, this, result);
		}
		else
		{
			agent.ExecutionFinished(result);
		}
	}

	public void Abort(BehaviorTree agent, EBTResult result = EBTResult.Abort)
	{
		OnNodeAborted(agent);
		ExecutionFinished(agent, result);
	}

	public virtual void OnNodeAborted(BehaviorTree agent)
	{
	}

	public virtual void GetNodes(ref List<BTNode> nodes)
	{
		nodes.Add(this);
	}
}
