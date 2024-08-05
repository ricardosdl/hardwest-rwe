using System.Collections.Generic;
using System.Linq;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Parallel")]
public class BTParallel : BTComposite
{
	[Body("Break on")]
	public PrallelBreakMode m_Mode;

	public override void AbortChildren(BehaviorComponent agent)
	{
		base.AbortChildren(agent);
		foreach (BTNode child in m_Children)
		{
			agent.RemoveTask(child.GetTask(agent));
		}
	}

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		TaskResult[] subTasks = new TaskResult[m_Children.Count];
		while (true)
		{
			for (int j = m_Children.Count - 1; j >= 0; j--)
			{
				BehaviorExec child = m_Children[j].StartTask(agent);
				subTasks[j] = TaskResult.Running;
				agent.AddTask(child);
			}
			TaskResult result = TaskResult.Running;
			while (true)
			{
				if (result == TaskResult.Running)
				{
					yield return TaskResult.Running;
					if (!task.Continue)
					{
						break;
					}
					for (int i = 0; i < m_Children.Count; i++)
					{
						subTasks[i] = m_Children[i].GetTask(agent).Current;
					}
					result = GetChildrenResult(ref subTasks);
					continue;
				}
				AbortChildren(agent);
				yield return result;
				break;
			}
		}
	}

	protected TaskResult GetChildrenResult(ref TaskResult[] tasks)
	{
		int num = tasks.Count();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < num; i++)
		{
			flag = flag || tasks[i] == TaskResult.Success;
			flag2 = flag2 || tasks[i] == TaskResult.Fail;
			flag3 = flag3 || tasks[i] == TaskResult.Running;
		}
		if (m_Mode == PrallelBreakMode.FirstSuccess)
		{
			if (flag)
			{
				return TaskResult.Success;
			}
			if (!flag3)
			{
				return TaskResult.Fail;
			}
		}
		else if (m_Mode == PrallelBreakMode.FirstFail)
		{
			if (flag2)
			{
				return TaskResult.Fail;
			}
			if (!flag3)
			{
				return TaskResult.Success;
			}
		}
		return TaskResult.Running;
	}
}
