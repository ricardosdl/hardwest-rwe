using System.Collections.Generic;
using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("Run Behavior")]
public class BTTask_RunBehavior : BTTask
{
	[SerializeField]
	[Body("Behavior")]
	public BehaviorAsset m_Behavior;

	public override BehaviorExec InstanceMemory(BehaviorComponent agent, ref BTInstance mem)
	{
		BehaviorExec result = base.InstanceMemory(agent, ref mem);
		if (m_Behavior.IObject != null)
		{
			BehaviorInstance instance = mem;
			m_Behavior.IObject.GetInstance(agent, ref instance);
			mem = instance as BTInstance;
		}
		return result;
	}

	public override void Abort(BehaviorComponent agent)
	{
		AbortBehavior(agent);
		base.Abort(agent);
	}

	protected void AbortBehavior(BehaviorComponent agent)
	{
		BehaviorExec behaviorExec = null;
		if (m_Behavior.IObject != null)
		{
			behaviorExec = m_Behavior.IObject.GetRoot(agent);
			if (behaviorExec != null)
			{
				agent.RemoveTask(behaviorExec);
			}
		}
	}

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		BehaviorExec root = null;
		if (m_Behavior.IObject != null)
		{
			root = m_Behavior.IObject.StartRoot(agent);
		}
		while (true)
		{
			if (root != null)
			{
				agent.AddTask(root);
				while (true)
				{
					if (root != null)
					{
						yield return TaskResult.Running;
						if (!task.Continue)
						{
							break;
						}
						if (root.Current == TaskResult.Running)
						{
							continue;
						}
					}
					AbortBehavior(agent);
					yield return root.Current;
					break;
				}
			}
			else
			{
				yield return TaskResult.Fail;
			}
		}
	}
}
