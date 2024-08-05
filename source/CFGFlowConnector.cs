using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CFGFlowConnector : ScriptableObject
{
	public CFGFlowNode m_OwningNode;

	public string m_ConnName;

	public ConnectorType m_ConnType;

	public ConnectorDirection m_ConnDir;

	public Vector2 m_Position;

	public List<CFGFlowConnector> m_Links;

	public bool m_Disabled;

	protected virtual void OnEnable()
	{
		base.hideFlags = HideFlags.HideInHierarchy;
		if (m_Links == null)
		{
			m_Links = new List<CFGFlowConnector>();
		}
	}

	public virtual Color GetColor()
	{
		ConnectorType connType = m_ConnType;
		return Color.white;
	}

	public virtual string GetConnectorLabel()
	{
		return m_ConnName;
	}

	public virtual void Link(CFGFlowConnector targetConn)
	{
		if (!m_Links.Contains(targetConn))
		{
			m_Links.Add(targetConn);
			targetConn.m_Links.Add(this);
		}
	}

	public virtual void UnLink(CFGFlowConnector targetConn, bool bReversed = false)
	{
		m_Links.Remove(targetConn);
		if (targetConn != null && bReversed)
		{
			targetConn.UnLink(this);
		}
	}

	public virtual void UnLinkAll()
	{
		for (int i = 0; i < m_Links.Count; i++)
		{
			m_Links[i].UnLink(this);
		}
		m_Links.Clear();
	}

	public virtual bool IsLinkedTo(CFGFlowConnector conn)
	{
		return m_Links.Contains(conn);
	}

	public virtual bool IsLinkedTo(CFGFlowObject obj)
	{
		return false;
	}

	public virtual bool IsLinked()
	{
		return m_Links.Any();
	}
}
