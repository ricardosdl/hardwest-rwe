using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class CFGFlowConditionBase : CFGFlowNode
{
	public static List<FlowConditionInfo> GetConditionList()
	{
		List<FlowConditionInfo> list = new List<FlowConditionInfo>();
		foreach (Type item in CFGClassUtil.AllSubClasses(typeof(CFGFlowConditionBase)))
		{
			MethodInfo method = item.GetMethod("GetConditionList", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				list.AddRange(method.Invoke(null, null) as List<FlowConditionInfo>);
			}
		}
		return list;
	}

	public static CFGFlowConditionBase CreateCondition(Type actionType, CFGFlowSequence parentSequence, Vector2 position)
	{
		CFGFlowConditionBase cFGFlowConditionBase = ScriptableObject.CreateInstance(actionType) as CFGFlowConditionBase;
		cFGFlowConditionBase.m_ParentSequence = parentSequence;
		cFGFlowConditionBase.m_Position = position;
		return cFGFlowConditionBase;
	}
}
