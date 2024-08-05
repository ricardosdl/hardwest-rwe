using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGFlowConn_Var : CFGFlowConnector
{
	public CFGType m_FlowVarType;

	public CFGType m_ValueType;

	public int m_MaxVars;

	public object m_Value;

	public string m_FieldName;

	public FieldInfo m_Field;

	public List<CFGFlowObject> m_VarLinks;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_VarLinks == null)
		{
			m_VarLinks = new List<CFGFlowObject>();
		}
		if (m_OwningNode != null && m_FieldName != string.Empty)
		{
			m_Field = m_OwningNode.GetType().GetField(m_FieldName);
		}
	}

	public virtual void Link(CFGFlowObject targetObj)
	{
		if (!m_VarLinks.Contains(targetObj))
		{
			m_VarLinks.Add(targetObj);
			if (targetObj is CFGFlowVariable)
			{
				(targetObj as CFGFlowVariable).m_Links.Add(this);
			}
			else if (targetObj is CFGFlowEventBase)
			{
				(targetObj as CFGFlowEventBase).m_Links.Add(this);
			}
		}
	}

	public virtual void UnLink(CFGFlowObject targetObj, bool bReversed = false)
	{
		m_VarLinks.Remove(targetObj);
		if (bReversed)
		{
			if (targetObj is CFGFlowVariable)
			{
				(targetObj as CFGFlowVariable).UnLink(this);
			}
			else if (targetObj is CFGFlowEventBase)
			{
				(targetObj as CFGFlowEventBase).UnLink(this);
			}
		}
		if (m_OwningNode != null)
		{
			m_OwningNode.OnUnLinked(null, targetObj, this);
		}
	}

	public override void UnLinkAll()
	{
		base.UnLinkAll();
		for (int i = 0; i < m_VarLinks.Count; i++)
		{
			if (m_VarLinks[i] != null)
			{
				m_VarLinks[i].UnLink(this);
			}
		}
		m_VarLinks.Clear();
	}

	public static CFGFlowConn_Var CreateVariableConnector(Type varType)
	{
		if (varType != null)
		{
			CFGFlowConn_Var cFGFlowConn_Var = ScriptableObject.CreateInstance<CFGFlowConn_Var>();
			cFGFlowConn_Var.m_FlowVarType = new CFGType(varType);
			return cFGFlowConn_Var;
		}
		return null;
	}

	[Obsolete("Not working")]
	public CFGFlowVariable CreateVariableForConnector(Vector2 offset)
	{
		Type flowVarType = CFGFlowVariable.GetFlowVarType(m_FlowVarType.SystemType);
		CFGFlowVariable cFGFlowVariable = ScriptableObject.CreateInstance(flowVarType) as CFGFlowVariable;
		cFGFlowVariable.m_ParentSequence = m_OwningNode.m_ParentSequence;
		cFGFlowVariable.m_Position = m_OwningNode.m_Position + m_Position + offset;
		return cFGFlowVariable;
	}

	public bool SupportsFlowObject(CFGFlowObject flowObj)
	{
		if (flowObj is CFGFlowVar_Enum)
		{
			CFGFlowVar_Enum cFGFlowVar_Enum = flowObj as CFGFlowVar_Enum;
			if (m_ValueType.SystemType != typeof(Enum) && m_ValueType.SystemType != cFGFlowVar_Enum.m_EnumType.SystemType)
			{
				return false;
			}
		}
		if (flowObj.GetType() == m_FlowVarType.SystemType || flowObj.GetType().IsSubclassOf(m_FlowVarType.SystemType))
		{
			return true;
		}
		CFGFlowVar_Ref cFGFlowVar_Ref = flowObj as CFGFlowVar_Ref;
		if (cFGFlowVar_Ref != null)
		{
			if (cFGFlowVar_Ref.IsEmptyReference())
			{
				return true;
			}
			if (cFGFlowVar_Ref.GetValueType().IsClassOrSubclassOf(m_ValueType.SystemType))
			{
				return true;
			}
			if (cFGFlowVar_Ref.GetValueType().IsClassOrSubclassOf(typeof(UnityEngine.Object)) && m_FlowVarType.SystemType == typeof(CFGFlowVar_Object))
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsLinkedTo(CFGFlowObject obj)
	{
		return m_VarLinks.Contains(obj);
	}

	public bool IsLinkedToSingleVariable()
	{
		if (m_VarLinks.Count == 1 && m_VarLinks[0] is CFGFlowVariable)
		{
			return true;
		}
		return false;
	}

	public override bool IsLinked()
	{
		return m_VarLinks.Any();
	}
}
