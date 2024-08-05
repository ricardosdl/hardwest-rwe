using System.Collections.Generic;
using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Node("Log")]
public class BTTask_Log : BTTask
{
	public BBString m_Text;

	[Body("Enable")]
	public bool m_Enable = true;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		while (true)
		{
			if (m_Enable)
			{
				Debug.Log(m_Text.GetValue(agent));
			}
			yield return TaskResult.Success;
		}
	}
}
