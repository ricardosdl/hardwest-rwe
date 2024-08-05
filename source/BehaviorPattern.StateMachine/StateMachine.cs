using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern.StateMachine;

public class StateMachine : ScriptableObject, IBehaviorAsset
{
	public List<StateObject> m_States = new List<StateObject>();

	public Type CustomClass => null;

	public BehaviorInstance GetInstance(BehaviorComponent agent)
	{
		return null;
	}

	public void GetInstance(BehaviorComponent agent, ref BehaviorInstance instance)
	{
	}

	public BehaviorExec GetRoot(BehaviorComponent agent)
	{
		return null;
	}

	public BehaviorExec StartRoot(BehaviorComponent agent)
	{
		return null;
	}
}
