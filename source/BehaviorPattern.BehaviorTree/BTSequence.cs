using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Sequence")]
public class BTSequence : BTComposite
{
	public override IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		BehaviorExec task = GetTask(agent);
		while (true)
		{
			int i = 0;
			while (true)
			{
				TaskResult result;
				if (i < m_Children.Count)
				{
					BehaviorExec child = m_Children[i].StartTask(agent);
					while (child.Update())
					{
						result = child.Current;
						if (result == TaskResult.Running)
						{
							yield return TaskResult.Running;
							if (!task.Continue)
							{
								goto end_IL_0146;
							}
							continue;
						}
						goto IL_00c6;
					}
					goto IL_0138;
				}
				yield return TaskResult.Success;
				break;
				IL_0138:
				i++;
				continue;
				IL_00c6:
				EndTask(agent);
				if (result == TaskResult.Success)
				{
					yield return TaskResult.Running;
					if (!task.Continue)
					{
						break;
					}
					goto IL_0138;
				}
				yield return TaskResult.Fail;
				break;
				continue;
				end_IL_0146:
				break;
			}
		}
	}
}
