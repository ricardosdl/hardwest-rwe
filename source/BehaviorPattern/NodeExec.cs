using System.Collections.Generic;

namespace BehaviorPattern;

public class NodeExec : BehaviorExec
{
	public NodeExec(IEnumerator<TaskResult> iterator)
		: base(iterator)
	{
	}
}
