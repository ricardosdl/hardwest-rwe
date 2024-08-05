using System.Collections.Generic;

namespace BehaviorPattern;

public abstract class BehaviorExec : TaskWrapper<TaskResult>
{
	public override bool Active => m_Current == TaskResult.Running;

	public BehaviorExec(IEnumerator<TaskResult> iterator)
		: base(iterator)
	{
	}
}
