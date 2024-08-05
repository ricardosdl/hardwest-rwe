using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGFlowAct_AttachEvent : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Target", expectedType = typeof(CFGFlowVariable), maxLinks = byte.MaxValue)]
	public List<object> m_Targets = new List<object>();

	[CFGFlowProperty(displayName = "Event", expectedType = typeof(CFGFlowCustomEvent))]
	public CFGFlowCustomEvent m_Event;

	public override void Activated()
	{
		if (m_Vars[0].m_VarLinks.Count <= 0)
		{
			return;
		}
		foreach (CFGFlowObject varLink in m_Vars[0].m_VarLinks)
		{
			CFGFlowVariable cFGFlowVariable = varLink as CFGFlowVariable;
			if (cFGFlowVariable != null && m_Event != null)
			{
				object variableOfType = cFGFlowVariable.GetVariableOfType(m_Event.m_ContextType.SystemType);
				if (variableOfType == null || !(variableOfType is MonoBehaviour))
				{
					LogWarning("Failed while attaching event!");
					break;
				}
				if (!m_Event.IsEventRegistered(variableOfType))
				{
					m_Event.RegisterEvent(variableOfType);
					continue;
				}
				LogWarning("event " + m_Event.m_EventName + " already registered for " + variableOfType);
			}
		}
	}

	public override bool IsVariableValuesValid()
	{
		if (!IsAllVariableConnectorsLinked())
		{
			return false;
		}
		CFGFlowCustomEvent cFGFlowCustomEvent = m_Vars[1].m_VarLinks[0] as CFGFlowCustomEvent;
		Type systemType = cFGFlowCustomEvent.m_ContextType.SystemType;
		foreach (CFGFlowObject varLink in m_Vars[0].m_VarLinks)
		{
			if (!(varLink is CFGFlowVar_Object))
			{
				return false;
			}
			CFGFlowVar_Object cFGFlowVar_Object = varLink as CFGFlowVar_Object;
			Type systemType2 = cFGFlowVar_Object.m_VarType.SystemType;
			if (!systemType2.IsClassOrSubclassOf(systemType))
			{
				return false;
			}
		}
		return true;
	}

	public override bool IsOK()
	{
		return base.IsConnectedToFlow && IsVariableValuesValid();
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AttachEvent);
		flowActionInfo.DisplayName = "Attach Event";
		flowActionInfo.CategoryName = "Events";
		return flowActionInfo;
	}
}
