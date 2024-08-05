using System.Collections.Generic;
using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("Wait")]
public class BTTask_Wait : BTTask
{
	[BBDefaultValue(5f)]
	public BBFloat m_WaitTime;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		while (true)
		{
			float startTime = Time.time;
			float waitTime = m_WaitTime.GetValue(agent);
			do
			{
				if (Time.time - startTime < waitTime)
				{
					yield return TaskResult.Running;
					continue;
				}
				yield return TaskResult.Success;
				break;
			}
			while (task.Continue);
		}
	}
}
