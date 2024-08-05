using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGFlowAct_Property : CFGFlowGameAction
{
	[HideInInspector]
	public string m_PropertyName = string.Empty;

	[SerializeField]
	public CFGType m_SearchType = new CFGType(typeof(GameObject));

	protected object obj;

	protected PropertyInfo prop;

	public CFGFlowVariable Target => m_Vars[0].m_VarLinks.FirstOrDefault() as CFGFlowVariable;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Target", typeof(CFGFlowVar_Object), typeof(GameObject), string.Empty);
	}

	public override void OnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
		if (fromConnector == m_Vars[0])
		{
			SetSearchType(GetTypeFromObject(toObject));
		}
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

	public Type GetTypeFromObject(CFGFlowObject o)
	{
		Type result = null;
		if (o == null)
		{
			return null;
		}
		CFGFlowVar_Object cFGFlowVar_Object = o as CFGFlowVar_Object;
		if (cFGFlowVar_Object != null)
		{
			result = cFGFlowVar_Object.m_VarType.SystemType;
		}
		CFGFlowVar_Ref cFGFlowVar_Ref = o as CFGFlowVar_Ref;
		if (cFGFlowVar_Ref != null)
		{
			CFGVarDef_Object cFGVarDef_Object = cFGFlowVar_Ref.RefDef as CFGVarDef_Object;
			if (cFGVarDef_Object != null)
			{
				if (cFGVarDef_Object.objectType == null)
				{
					cFGVarDef_Object.SetValueType(cFGVarDef_Object.Value);
				}
				result = cFGFlowVar_Ref.RefDef.ValueType;
			}
		}
		return result;
	}

	public override void Activated()
	{
		base.Activated();
		obj = null;
		prop = null;
		CFGFlowVariable cFGFlowVariable = m_Vars[0].m_VarLinks.FirstOrDefault() as CFGFlowVariable;
		if (cFGFlowVariable == null)
		{
			LogError("Variable not connected or invalid type");
			return;
		}
		obj = cFGFlowVariable.GetVariableOfType(m_SearchType.SystemType);
		if (obj == null)
		{
			LogError("Could not get variable of chosen type");
			return;
		}
		prop = obj.GetType().GetProperty(m_PropertyName);
		if (prop == null)
		{
			LogError($"{m_PropertyName} not found in {m_SearchType.SystemType}. Re-check connected variables and types.");
		}
	}

	public override bool IsOK()
	{
		return !string.IsNullOrEmpty(m_PropertyName) && base.IsOK();
	}
}
