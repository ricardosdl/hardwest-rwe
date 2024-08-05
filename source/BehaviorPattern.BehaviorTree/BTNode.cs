using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Node")]
public abstract class BTNode : BTObject
{
	public BTNode m_Parent;

	public virtual void GetNodes(ref List<BTNode> nodes)
	{
		nodes.Add(this);
	}

	public virtual BehaviorExec GetTask(BehaviorComponent agent)
	{
		return agent.Behavior.GetMemory(m_MemoryOffset);
	}

	public BehaviorExec StartTask(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		OnBecomeRelevant(agent);
		return task;
	}

	public void EndTask(BehaviorComponent agent)
	{
		OnCeaseRelevant(agent);
	}

	public virtual void Abort(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		OnCeaseRelevant(agent);
		task.Abort();
	}

	public virtual void OnBecomeRelevant(BehaviorComponent agent)
	{
	}

	public virtual void OnCeaseRelevant(BehaviorComponent agent)
	{
	}
}
