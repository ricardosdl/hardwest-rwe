using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class CFGFlowActionBase : CFGFlowNode
{
	private static string[] editorCategories = new string[9] { "Events", "Latent", "Math", "Misc", "Lists", "Property", "Sequence", "Set Variable", "Texts" };

	public virtual bool DefaultOutput => true;

	public static bool IsEditorCategory(string categoryName)
	{
		if (categoryName == null)
		{
			return true;
		}
		string[] array = categoryName.Split(new string[1] { "/" }, StringSplitOptions.None);
		return editorCategories.Contains(array[0]);
	}

	public override void DeActivated()
	{
		if (DefaultOutput)
		{
			m_Outputs[0].m_HasImpulse = true;
		}
		base.DeActivated();
	}

	public static List<FlowActionInfo> GetActionList()
	{
		List<FlowActionInfo> list = new List<FlowActionInfo>();
		foreach (Type item in CFGClassUtil.AllSubClasses(typeof(CFGFlowActionBase)))
		{
			MethodInfo method = item.GetMethod("GetActionList", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				list.AddRange(method.Invoke(null, null) as List<FlowActionInfo>);
			}
		}
		return list;
	}

	public static CFGFlowActionBase CreateAction(Type actionType, CFGFlowSequence parentSequence, Vector2 position)
	{
		CFGFlowActionBase cFGFlowActionBase = ScriptableObject.CreateInstance(actionType) as CFGFlowActionBase;
		cFGFlowActionBase.m_ParentSequence = parentSequence;
		cFGFlowActionBase.m_Position = position;
		return cFGFlowActionBase;
	}
}
