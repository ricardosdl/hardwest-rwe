using System;
using System.Linq;

public class CFGFlowConn_Exec : CFGFlowConnector
{
	public bool m_HasImpulse;

	[NonSerialized]
	public int m_ActivateCount;

	public virtual void ActivateConnector()
	{
		foreach (CFGFlowConn_Exec link in m_Links)
		{
			link.m_HasImpulse = true;
		}
	}

	public override void UnLinkAll()
	{
		foreach (CFGFlowConnector link in m_Links)
		{
			link.m_Links.Remove(this);
		}
		base.UnLinkAll();
	}

	public override bool IsLinkedTo(CFGFlowObject obj)
	{
		return m_Links.Any((CFGFlowConnector conn) => conn.m_OwningNode == obj) || base.IsLinkedTo(obj);
	}
}
