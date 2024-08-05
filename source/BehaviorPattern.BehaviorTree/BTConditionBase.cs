using Core;
using UnityEngine;

namespace BehaviorPattern.BehaviorTree;

[Category("Condition")]
public abstract class BTConditionBase : BTNode
{
	[Body("Mode")]
	public ConditionMode m_Mode;

	public virtual bool Evaluate(BehaviorComponent agent)
	{
		Debug.Log("why this ?");
		return false;
	}
}
