using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Condition")]
public class BTConditionOP : BTConditionBase
{
	public BTOpRef m_Op;

	public override bool Evaluate(BehaviorComponent agent)
	{
		if (m_Op != null)
		{
			return m_Op.Evaluate(agent);
		}
		return false;
	}

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		while (true)
		{
			bool val = Evaluate(agent);
			if (m_Mode == ConditionMode.Instant)
			{
				yield return val ? TaskResult.Success : TaskResult.Fail;
				continue;
			}
			while (true)
			{
				if (val)
				{
					yield return TaskResult.Running;
					if (!task.Continue)
					{
						break;
					}
					val = Evaluate(agent);
					continue;
				}
				yield return TaskResult.Fail;
				break;
			}
		}
	}
}
