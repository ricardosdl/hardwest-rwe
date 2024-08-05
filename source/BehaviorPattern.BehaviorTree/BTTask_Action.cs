using System;
using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Node("Action")]
public class BTTask_Action : BTTask, ITaskBody
{
	public List<BBVariable> m_Variables = new List<BBVariable>();

	[Body("", StringArg = "GetActions")]
	public SerializedMethod m_Method;

	public T GetValue<T>(BehaviorComponent agent, string argName)
	{
		BBVariable<T> bBVariable = m_Variables.Find((BBVariable p) => p.name == argName) as BBVariable<T>;
		if (bBVariable != null)
		{
			return bBVariable.GetValue(agent);
		}
		return default(T);
	}

	public T GetLocal<T>()
	{
		return default(T);
	}

	public T GetParam<T>(BehaviorComponent agent, string argName)
	{
		BBVariable<T> bBVariable = m_Variables.Find((BBVariable p) => p.name == argName) as BBVariable<T>;
		if (bBVariable != null)
		{
			return bBVariable.GetValue(agent);
		}
		return default(T);
	}

	public BehaviorExec GetTaskInstance(BehaviorComponent agent)
	{
		return GetTask(agent);
	}

	public override BehaviorExec InstanceMemory(BehaviorComponent agent, ref BTInstance mem)
	{
		Delegate @delegate = Delegate.CreateDelegate(typeof(TaskSignature), agent, m_Method.SystemMethod);
		IEnumerator<TaskResult> iterator = ((TaskSignature)@delegate)(agent, this);
		BehaviorExec behaviorExec = new NodeExec(iterator);
		m_MemoryOffset = mem.InstanceMemory(behaviorExec);
		return behaviorExec;
	}
}
