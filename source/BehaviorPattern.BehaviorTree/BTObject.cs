using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

public abstract class BTObject : ScriptableObject
{
	protected int m_MemoryOffset;

	[SerializeField]
	[HideInInspector]
	private BehaviorTree m_BTAsset;

	[HideInInspector]
	[SerializeField]
	private Blackboard m_BBAsset;

	public BehaviorTree BT => m_BTAsset;

	public Blackboard BB => m_BBAsset;

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
	}

	public virtual BehaviorExec InstanceMemory(BehaviorComponent agent, ref BTInstance mem)
	{
		BehaviorExec behaviorExec = new NodeExec(Exec(agent));
		m_MemoryOffset = mem.InstanceMemory(behaviorExec);
		return behaviorExec;
	}

	public virtual IEnumerator<TaskResult> Exec(BehaviorComponent agent)
	{
		yield break;
	}
}
