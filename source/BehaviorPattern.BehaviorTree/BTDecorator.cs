using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("Decorator")]
public abstract class BTDecorator : BTNode
{
	public BTNode m_Child;

	public override void GetNodes(ref List<BTNode> nodes)
	{
		base.GetNodes(ref nodes);
		if (m_Child != null)
		{
			m_Child.GetNodes(ref nodes);
		}
	}
}
