using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

public class BTInstance : BehaviorInstance
{
	public BehaviorTree m_Template;

	public BTInstance(BehaviorTree asset)
	{
		m_Template = asset;
	}

	public override void StartBehave()
	{
		Debug.LogWarning("this function should be implemented");
	}
}
