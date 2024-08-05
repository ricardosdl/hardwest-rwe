using System.Collections.Generic;
using Core;

namespace BehaviorPattern.BehaviorTree;

[Category("Composite")]
public abstract class BTComposite : BTNode
{
	public List<BTNode> m_Children = new List<BTNode>();

	public override void GetNodes(ref List<BTNode> nodes)
	{
		base.GetNodes(ref nodes);
		foreach (BTNode child in m_Children)
		{
			if (child != null)
			{
				child.GetNodes(ref nodes);
			}
		}
	}

	public override void Abort(BehaviorComponent agent)
	{
		AbortChildren(agent);
		base.Abort(agent);
	}

	public virtual void AbortChildren(BehaviorComponent agent)
	{
		foreach (BTNode child in m_Children)
		{
			child.Abort(agent);
		}
	}
}
