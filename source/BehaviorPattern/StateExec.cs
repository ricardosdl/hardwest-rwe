using System.Collections.Generic;

namespace BehaviorPattern;

public class StateExec : BehaviorExec
{
	public StateExec(IEnumerator<TaskResult> iterator)
		: base(iterator)
	{
	}

	public override bool Update()
	{
		bool flag = base.Update();
		if (!flag)
		{
		}
		return flag;
	}
}
