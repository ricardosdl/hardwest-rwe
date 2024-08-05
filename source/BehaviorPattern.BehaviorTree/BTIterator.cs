using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Iterator")]
public class BTIterator : BTDecorator
{
	[Body("Return")]
	public TaskInstantResult m_OverrideResult;

	public BBList<int> m_List;

	public BBVariable m_Output;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		while (true)
		{
			BehaviorExec child = m_Child.GetTask(agent);
			while (true)
			{
				if (child.Update())
				{
					TaskResult result = child.Current;
					if (result == TaskResult.Running)
					{
						yield return TaskResult.Running;
						if (!task.Continue)
						{
							break;
						}
						continue;
					}
				}
				yield return (TaskResult)m_OverrideResult;
				break;
			}
		}
	}
}
