using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGFlowEventBase : CFGFlowNode
{
	private static string[] editorCategories = new string[1] { string.Empty };

	[HideInInspector]
	public string m_EventName;

	[HideInInspector]
	public List<CFGFlowConnector> m_Links = new List<CFGFlowConnector>();

	public static bool IsEditorCategory(string categoryName)
	{
		if (categoryName == null)
		{
			return true;
		}
		string[] array = categoryName.Split(new string[1] { "/" }, StringSplitOptions.None);
		return editorCategories.Contains(array[0]);
	}

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
	}

	public static FlowEventInfo GetEventInfo()
	{
		return new FlowEventInfo();
	}

	public virtual bool RegisterEvent(object inObject)
	{
		return false;
	}

	public virtual void UnRegisterEvent(object inObject)
	{
	}

	public static List<FlowEventInfo> GetEventList()
	{
		List<FlowEventInfo> list = new List<FlowEventInfo>();
		foreach (Type item in CFGClassUtil.AllSubClasses(typeof(CFGFlowEventBase)))
		{
			MethodInfo method = item.GetMethod("GetEventList", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				list.AddRange(method.Invoke(null, null) as List<FlowEventInfo>);
			}
		}
		return list;
	}

	public override void UnLink(CFGFlowConn_Var connVar, bool bReversed = false)
	{
		if (bReversed)
		{
			connVar.UnLink(this);
		}
		m_Links.Remove(connVar);
	}

	public override void OnRemove()
	{
		base.OnRemove();
		foreach (CFGFlowConn_Var link in m_Links)
		{
			link.UnLink(this);
		}
		m_Links.Clear();
	}

	public override void DeActivated()
	{
		m_Outputs[0].m_HasImpulse = true;
		base.DeActivated();
	}

	public override void ReconnectLinksTo(CFGFlowNode node)
	{
		base.ReconnectLinksTo(node);
		List<CFGFlowConn_Var> list = new List<CFGFlowConn_Var>();
		foreach (CFGFlowConnector link in m_Links)
		{
			if (link is CFGFlowConn_Var)
			{
				list.Add(link as CFGFlowConn_Var);
				continue;
			}
			node.m_NeedAttention = true;
			link.m_OwningNode.m_NeedAttention = true;
		}
		foreach (CFGFlowConn_Var item in list)
		{
			item.UnLinkAll();
			item.Link(node);
		}
	}

	public override bool IsOK()
	{
		if (!base.IsOK())
		{
			return false;
		}
		if (!m_Links.Any())
		{
			return false;
		}
		return true;
	}

	public static CFGFlowEventBase CreateEvent(Type actionType, CFGFlowSequence parentSequence, Vector2 position)
	{
		CFGFlowEventBase cFGFlowEventBase = ScriptableObject.CreateInstance(actionType) as CFGFlowEventBase;
		cFGFlowEventBase.m_ParentSequence = parentSequence;
		cFGFlowEventBase.m_Position = position;
		return cFGFlowEventBase;
	}
}
