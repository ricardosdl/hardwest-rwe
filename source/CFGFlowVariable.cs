using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class CFGFlowVariable : CFGFlowObject
{
	[HideInInspector]
	public List<CFGFlowConnector> m_Links;

	public virtual object Value { get; set; }

	protected static bool SupportsType(Type varType)
	{
		return false;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_Links == null)
		{
			m_Links = new List<CFGFlowConnector>();
		}
	}

	public override bool IsOK()
	{
		if (m_Links.Any())
		{
			return true;
		}
		return false;
	}

	public static IEnumerable<Type> AllFlowVarTypes()
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly asm in assemblies)
		{
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(typeof(CFGFlowVariable)) && !type.IsAbstract)
				{
					yield return type;
				}
			}
		}
	}

	public static Type GetFlowVarType(Type varType)
	{
		bool flag = false;
		foreach (Type item in AllFlowVarTypes())
		{
			MethodInfo method = item.GetMethod("SupportsType", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (varType != null && method != null)
			{
				object[] parameters = new object[1] { varType };
				flag = (bool)method.Invoke(null, parameters);
			}
			if (flag)
			{
				return item;
			}
		}
		return null;
	}

	public static CFGFlowVariable CreateVariable(Type varType)
	{
		Type flowVarType = GetFlowVarType(varType);
		if (flowVarType != null)
		{
			return ScriptableObject.CreateInstance(flowVarType) as CFGFlowVariable;
		}
		return null;
	}

	public static CFGFlowVariable CreateVariable(Type variableType, CFGFlowSequence parentSequence, Vector2 position)
	{
		Type flowVarType = GetFlowVarType(variableType);
		if (flowVarType != null)
		{
			CFGFlowVariable cFGFlowVariable = ScriptableObject.CreateInstance(variableType) as CFGFlowVariable;
			cFGFlowVariable.m_ParentSequence = parentSequence;
			cFGFlowVariable.m_Position = position;
			return cFGFlowVariable;
		}
		return null;
	}

	public virtual void InitFromInfo(FlowVarInfo info)
	{
	}

	public virtual object GetVariableOfType(Type varType)
	{
		return null;
	}

	public virtual void SetVariable(object varObj)
	{
	}

	public override void UnLink(CFGFlowConn_Var connVar, bool bReversed = false)
	{
		if (bReversed)
		{
			connVar.UnLink(this);
		}
		m_Links.Remove(connVar);
		OnUnLinked(connVar, connVar.m_OwningNode, null);
	}

	public void UnLinkAll()
	{
		foreach (CFGFlowConnector link in m_Links)
		{
			CFGFlowConn_Var cFGFlowConn_Var = link as CFGFlowConn_Var;
			cFGFlowConn_Var.UnLink(this);
			OnUnLinked(link, link.m_OwningNode, null);
		}
		m_Links.Clear();
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

	public static List<FlowVarInfo> GetVariableList()
	{
		List<FlowVarInfo> list = new List<FlowVarInfo>();
		foreach (Type item2 in CFGClassUtil.AllSubClasses(typeof(CFGFlowVariable)))
		{
			bool flag = item2.IsObsolete();
			if (!item2.IsAbstract && !flag)
			{
				FlowVarInfo item = default(FlowVarInfo);
				item.VarType = item2;
				item.Variable = null;
				list.Add(item);
			}
		}
		return list;
	}

	public virtual string GetTypeName()
	{
		return "Unsupported";
	}

	public virtual string GetValueName()
	{
		return "Unsupported";
	}
}
