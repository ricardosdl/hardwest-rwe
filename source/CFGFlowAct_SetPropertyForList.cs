using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGFlowAct_SetPropertyForList : CFGFlowGameAction
{
	[HideInInspector]
	public string m_PropertyName = string.Empty;

	[SerializeField]
	public CFGType m_SearchType = new CFGType(typeof(GameObject));

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "List", typeof(CFGFlowVar_ObjectList), null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Property", typeof(CFGFlowVariable), null, string.Empty);
	}

	public override void OnUnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
		if (toConnector == m_Vars[0])
		{
			ResetSearchType();
		}
	}

	public void SetSearchType(Type type)
	{
		CFGType cFGType = new CFGType(type);
		if (!object.Equals(m_SearchType, cFGType))
		{
			m_SearchType = cFGType;
			ResetPropertyConnector();
		}
	}

	protected void ResetSearchType()
	{
		m_SearchType = new CFGType(typeof(GameObject));
		ResetPropertyConnector();
	}

	public void ResetPropertyConnector()
	{
		m_PropertyName = string.Empty;
		if (m_Vars[1].IsLinked())
		{
			m_NeedAttention = true;
			foreach (CFGFlowObject varLink in m_Vars[1].m_VarLinks)
			{
				varLink.m_NeedAttention = true;
			}
			m_Vars[1].UnLinkAll();
		}
		m_Vars[1].m_ConnName = "Property";
		m_Vars[1].m_ValueType = new CFGType(typeof(void));
		m_Vars[1].m_FlowVarType = new CFGType(typeof(CFGFlowVariable));
	}

	public void SetPropertyConnector(PropertyInfo prop)
	{
		m_PropertyName = prop.Name;
		m_Vars[1].m_ConnName = prop.Name;
		m_Vars[1].m_ValueType = new CFGType(prop.PropertyType);
		m_Vars[1].m_FlowVarType = new CFGType(CFGFlowVariable.GetFlowVarType(prop.PropertyType));
	}

	public override void Activated()
	{
		base.Activated();
		CFGFlowVar_ObjectList cFGFlowVar_ObjectList = m_Vars[0].m_VarLinks.FirstOrDefault() as CFGFlowVar_ObjectList;
		if (cFGFlowVar_ObjectList == null)
		{
			LogError("Variable not connected or invalid type");
			return;
		}
		if (!m_Vars[1].IsLinkedToSingleVariable())
		{
			LogError("Invalid link->property value.");
			return;
		}
		foreach (UnityEngine.Object item in cFGFlowVar_ObjectList.m_Value)
		{
			object variableOfTypeFromObject = GetVariableOfTypeFromObject(item, m_SearchType.SystemType);
			if (variableOfTypeFromObject == null)
			{
				LogError("Could not get variable of chosen type from " + item);
				continue;
			}
			PropertyInfo property = variableOfTypeFromObject.GetType().GetProperty(m_PropertyName);
			if (property == null)
			{
				LogError($"{m_PropertyName} not found in {m_SearchType.SystemType}. Re-check connected variables and types.");
			}
			if (m_Vars[1].IsLinkedToSingleVariable())
			{
				property.SetValue(variableOfTypeFromObject, m_Vars[1].m_Value, null);
			}
			else
			{
				LogError("Invalid link -> property value.");
			}
		}
	}

	public object GetVariableOfTypeFromObject(UnityEngine.Object o, Type varType)
	{
		if (varType == null)
		{
			return null;
		}
		if (varType.IsSubclassOf(typeof(Component)) || varType == typeof(Component))
		{
			if (o is GameObject)
			{
				if (o == null)
				{
					return null;
				}
				return (o as GameObject).GetComponent(varType);
			}
		}
		else if (varType.IsSubclassOf(typeof(UnityEngine.Object)) || varType == typeof(UnityEngine.Object))
		{
			return o;
		}
		return null;
	}

	public override bool IsOK()
	{
		return !string.IsNullOrEmpty(m_PropertyName) && base.IsOK();
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_SetPropertyForList);
		flowActionInfo.DisplayName = "Set Property For List";
		flowActionInfo.CategoryName = "Property";
		return flowActionInfo;
	}
}
