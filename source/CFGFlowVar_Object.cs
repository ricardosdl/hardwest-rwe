using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGFlowVar_Object : CFGFlowVar_Typed<UnityEngine.Object>
{
	public CFGType m_VarType;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_VarType == null)
		{
			m_VarType = new CFGType(typeof(UnityEngine.Object));
		}
	}

	public new static bool SupportsType(Type varType)
	{
		return varType == typeof(UnityEngine.Object) || varType.IsSubclassOf(typeof(UnityEngine.Object));
	}

	public override object GetVariableOfType(Type varType)
	{
		if (varType == null)
		{
			return null;
		}
		if (varType.IsSubclassOf(typeof(Component)) || varType == typeof(Component))
		{
			if (m_Value is GameObject)
			{
				if (m_Value == null)
				{
					return null;
				}
				return (m_Value as GameObject).GetComponent(varType);
			}
		}
		else if (varType.IsSubclassOf(typeof(UnityEngine.Object)) || varType == typeof(UnityEngine.Object))
		{
			return m_Value;
		}
		return null;
	}

	public override void SetVariable(object varObj)
	{
		if (varObj is Component)
		{
			if (varObj == null || varObj.Equals(null) || (varObj as Component).gameObject == null)
			{
				LogWarning("Assigning null game object !");
				m_Value = null;
			}
			else
			{
				m_Value = (varObj as Component).gameObject;
			}
		}
		else
		{
			m_Value = varObj as UnityEngine.Object;
		}
		if (m_Value != null)
		{
			SetVariableType(m_Value.GetType());
		}
		else
		{
			SetVariableType(typeof(UnityEngine.Object));
		}
	}

	public void SetVariableType(Type newType)
	{
		if (newType == typeof(GameObject))
		{
			Component[] components = (m_Value as GameObject).GetComponents<CFGGameObject>();
			if (components.Length == 1)
			{
				m_VarType = new CFGType(components[0].GetType());
				return;
			}
			components = (m_Value as GameObject).GetComponents<MonoBehaviour>();
			if (components.Length == 1)
			{
				m_VarType = new CFGType(components[0].GetType());
			}
			else
			{
				m_VarType = new CFGType(typeof(GameObject));
			}
		}
		else
		{
			m_VarType = new CFGType(newType);
		}
	}

	public override void OnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
		CFGFlowConn_Var cFGFlowConn_Var = toConnector as CFGFlowConn_Var;
		if (cFGFlowConn_Var != null && cFGFlowConn_Var.m_ConnDir == ConnectorDirection.CD_Output)
		{
			OnReload();
		}
	}

	public override void OnUnLinked(CFGFlowConnector fromConnector, CFGFlowObject toObject, CFGFlowConnector toConnector)
	{
		CFGFlowConn_Var cFGFlowConn_Var = fromConnector as CFGFlowConn_Var;
		if (cFGFlowConn_Var != null && cFGFlowConn_Var.m_ConnDir == ConnectorDirection.CD_Output)
		{
			OnReload();
		}
	}

	public override void OnReload()
	{
		SetVariable(m_Value);
		if (!(m_Value == null))
		{
			return;
		}
		List<Type> list = new List<Type>();
		foreach (CFGFlowConnector link in m_Links)
		{
			if (link is CFGFlowConn_Var && link.m_ConnDir == ConnectorDirection.CD_Output)
			{
				list.Add((link as CFGFlowConn_Var).m_ValueType.SystemType);
			}
		}
		if (list.Count > 0)
		{
			SetVariableType(MostDerivedCommonBase(list));
		}
	}

	public Type MostDerivedCommonBase(IEnumerable<Type> types)
	{
		if (!types.Any())
		{
			return null;
		}
		Dictionary<Type, int> counts = (from t in types.SelectMany((Type t) => t.TypeHierarchy())
			group t by t).ToDictionary((IGrouping<Type, Type> g) => g.Key, (IGrouping<Type, Type> g) => g.Count());
		int total = counts[typeof(object)];
		return types.First().TypeHierarchy().First((Type t) => counts[t] == total);
	}

	public override string GetTypeName()
	{
		if (m_VarType != null && m_VarType.SystemType != null)
		{
			string text = m_VarType.SystemType.ToString();
			int num = text.LastIndexOf(".");
			if (num != -1)
			{
				text = text.Substring(num + 1);
			}
			return text;
		}
		return "Object";
	}

	public override string GetValueName()
	{
		string empty = string.Empty;
		if (m_Value != null)
		{
			empty = m_Value.ToString();
			int num = empty.IndexOf(" ");
			if (num > 0 && num <= empty.Length)
			{
				empty = empty.Substring(0, num);
			}
		}
		else
		{
			empty = "null";
		}
		return empty;
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarObject;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = BaseSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		if (!base.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		int num = -1;
		GameObject gameObject = m_Value as GameObject;
		if (gameObject != null)
		{
			CFGSerializableObject component = gameObject.GetComponent<CFGSerializableObject>();
			if ((bool)component)
			{
				num = component.UniqueID;
			}
			if (num == 0)
			{
				Log("Object has id of 0:" + gameObject.name);
			}
		}
		if (num > -1)
		{
			cFG_SG_Node.Attrib_Set("TargetUUID", num);
		}
		if (!(m_Value != null) || num >= 1 || m_Value is AudioClip)
		{
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		int _Value = -1;
		CFGSerializableObject cFGSerializableObject = null;
		if (_FlowObject.Attrib_Get("TargetUUID", ref _Value, bReportMissing: false))
		{
			cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(_Value, ESerializableType.NotSerializable);
		}
		if (_Value == -1)
		{
			return true;
		}
		if (cFGSerializableObject == null)
		{
			if (_Value != 0)
			{
				LogError("Failed to find object: ID: " + _Value);
			}
			return true;
		}
		m_Value = cFGSerializableObject.gameObject;
		return true;
	}
}
