using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

public class BehaviorTree : BTObject, IBehaviorAsset
{
	[SerializeField]
	private BTNode m_Root;

	[Body("Blackboard")]
	public Blackboard m_Blackboard;

	[Body("Class", TypeArg = typeof(BehaviorComponent))]
	public SerializedType m_ComponentClass = typeof(BehaviorComponent);

	public Type CustomClass => m_ComponentClass;

	protected override void OnEnable()
	{
	}

	public BehaviorInstance GetInstance(BehaviorComponent agent)
	{
		BTInstance mem = new BTInstance(this);
		if (m_Root != null)
		{
			List<BTNode> nodes = new List<BTNode>();
			m_Root.GetNodes(ref nodes);
			foreach (BTNode item in nodes)
			{
				item.InstanceMemory(agent, ref mem);
			}
		}
		return mem;
	}

	public void GetInstance(BehaviorComponent agent, ref BehaviorInstance instance)
	{
		BTInstance mem = instance as BTInstance;
		if (!(m_Root != null))
		{
			return;
		}
		List<BTNode> nodes = new List<BTNode>();
		m_Root.GetNodes(ref nodes);
		foreach (BTNode item in nodes)
		{
			item.InstanceMemory(agent, ref mem);
		}
	}

	public BehaviorExec StartRoot(BehaviorComponent agent)
	{
		if (m_Root != null)
		{
			return m_Root.StartTask(agent);
		}
		return null;
	}

	public BehaviorExec GetRoot(BehaviorComponent agent)
	{
		if (m_Root != null)
		{
			return m_Root.GetTask(agent);
		}
		return null;
	}
}
