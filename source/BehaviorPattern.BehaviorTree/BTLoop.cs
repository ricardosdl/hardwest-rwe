using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Loop")]
public class BTLoop : BTDecorator
{
	[Body("Iterations")]
	public int m_Iterations = -1;

	[Body("Repeat on")]
	public LoopMode m_RepeatMode;

	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		while (true)
		{
			int loopCount = m_Iterations;
			do
			{
				if (m_Iterations == -1 || loopCount > 0)
				{
					TaskResult result = TaskResult.Fail;
					BehaviorExec child = m_Child.GetTask(agent);
					while (child.Update())
					{
						result = child.Current;
						if (result != TaskResult.Running)
						{
							break;
						}
						yield return TaskResult.Running;
						if (!task.Continue)
						{
							goto end_IL_0162;
						}
					}
					if ((m_RepeatMode == LoopMode.Fail && result != 0) || (m_RepeatMode == LoopMode.Success && result != TaskResult.Success))
					{
						yield return TaskResult.Fail;
						break;
					}
					loopCount--;
					yield return TaskResult.Running;
					continue;
				}
				yield return TaskResult.Success;
				break;
				continue;
				end_IL_0162:
				break;
			}
			while (task.Continue);
		}
	}
}
