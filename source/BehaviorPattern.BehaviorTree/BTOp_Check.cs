using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Check")]
[Category("Custom Op")]
public class BTOp_Check : BTOpBase
{
	public List<BBVariable> m_Variables = new List<BBVariable>();

	[Body("", StringArg = "GetChecks")]
	public SerializedMethod m_Method;

	public override BehaviorExec InstanceMemory(BehaviorComponent agent, ref BTInstance mem)
	{
		return base.InstanceMemory(agent, ref mem);
	}

	protected override bool RawEvaluate(BehaviorComponent agent)
	{
		return true;
	}

	public virtual void ChildChanged(BehaviorComponent agent, bool value)
	{
	}
}
