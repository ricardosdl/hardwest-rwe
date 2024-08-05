using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class CFGFlowNode : CFGFlowObject
{
	public string m_NodeName;

	[HideInInspector]
	[Instanced]
	public List<CFGFlowConn_Exec> m_Inputs = new List<CFGFlowConn_Exec>();

	[Instanced]
	[HideInInspector]
	public List<CFGFlowConn_Exec> m_Outputs = new List<CFGFlowConn_Exec>();

	[Instanced]
	[HideInInspector]
	public List<CFGFlowConn_Var> m_Vars = new List<CFGFlowConn_Var>();

	[HideInInspector]
	public bool m_Active;

	[HideInInspector]
	public int m_ActivateCount;

	[HideInInspector]
	public bool m_Disabled;

	public virtual bool AllowMultiInFrame => true;

	public bool HasInputConnectors => m_Inputs.Any();

	public bool IsInputConnected => m_Inputs.Any((CFGFlowConn_Exec i) => i.IsLinked());

	public bool HasOutputConnectors => m_Outputs.Any();

	public bool IsOutputConnected => m_Outputs.Any((CFGFlowConn_Exec o) => o.IsLinked());

	public bool IsConnectedToItself => HasOutputConnectors && m_Outputs.Any((CFGFlowConn_Exec output) => output.IsLinkedTo(this));

	public bool IsConnectedToFlow => (!HasInputConnectors) ? IsOutputConnected : IsInputConnected;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_Inputs == null)
		{
			m_Inputs = new List<CFGFlowConn_Exec>();
		}
		if (m_Outputs == null)
		{
			m_Outputs = new List<CFGFlowConn_Exec>();
		}
		if (m_Vars == null)
		{
			m_Vars = new List<CFGFlowConn_Var>();
		}
	}

	public void CreateConnector(ConnectorDirection connDir, ConnectorType connType, string connName, Type flowVarType = null, Type valueType = null, string propertyName = "", byte maxLinks = 1)
	{
		CFGFlowConnector cFGFlowConnector = null;
		switch (connType)
		{
		case ConnectorType.CT_Exec:
			cFGFlowConnector = ScriptableObject.CreateInstance<CFGFlowConn_Exec>();
			cFGFlowConnector.m_ConnDir = connDir;
			cFGFlowConnector.m_ConnType = connType;
			cFGFlowConnector.m_OwningNode = this;
			if (connDir == ConnectorDirection.CD_Input)
			{
				m_Inputs.Add(cFGFlowConnector as CFGFlowConn_Exec);
			}
			else
			{
				m_Outputs.Add(cFGFlowConnector as CFGFlowConn_Exec);
			}
			break;
		case ConnectorType.CT_Var:
		{
			CFGFlowConn_Var cFGFlowConn_Var = CFGFlowConn_Var.CreateVariableConnector(flowVarType);
			if (cFGFlowConn_Var != null)
			{
				cFGFlowConn_Var.m_ConnDir = connDir;
				cFGFlowConn_Var.m_ConnType = connType;
				cFGFlowConn_Var.m_OwningNode = this;
				cFGFlowConn_Var.m_MaxVars = maxLinks;
				if (valueType == null)
				{
					valueType = typeof(void);
				}
				cFGFlowConn_Var.m_ValueType = new CFGType(valueType);
				cFGFlowConn_Var.m_FieldName = propertyName;
				cFGFlowConn_Var.m_Field = GetType().GetField(cFGFlowConn_Var.m_FieldName);
				m_Vars.Add(cFGFlowConn_Var);
				cFGFlowConnector = cFGFlowConn_Var;
				break;
			}
			LogWarning("Unable to create variable connector with type : " + flowVarType);
			return;
		}
		}
		cFGFlowConnector.m_ConnType = connType;
		cFGFlowConnector.m_ConnDir = connDir;
		cFGFlowConnector.m_ConnName = connName;
		cFGFlowConnector.m_OwningNode = this;
	}

	public CFGFlowConnector CreateStandaloneConnector(ConnectorDirection connDir, ConnectorType connType, string connName, Type flowVarType = null, Type valueType = null, string propertyName = "", byte maxLinks = 1)
	{
		CFGFlowConnector cFGFlowConnector = null;
		switch (connType)
		{
		case ConnectorType.CT_Exec:
			cFGFlowConnector = ScriptableObject.CreateInstance<CFGFlowConn_Exec>();
			cFGFlowConnector.m_ConnDir = connDir;
			cFGFlowConnector.m_ConnType = connType;
			cFGFlowConnector.m_OwningNode = this;
			return cFGFlowConnector;
		case ConnectorType.CT_Var:
		{
			CFGFlowConn_Var cFGFlowConn_Var = CFGFlowConn_Var.CreateVariableConnector(flowVarType);
			if (cFGFlowConn_Var != null)
			{
				cFGFlowConn_Var.m_ConnDir = connDir;
				cFGFlowConn_Var.m_ConnType = connType;
				cFGFlowConn_Var.m_OwningNode = this;
				cFGFlowConn_Var.m_MaxVars = maxLinks;
				if (valueType == null)
				{
					valueType = typeof(void);
				}
				cFGFlowConn_Var.m_ValueType = new CFGType(valueType);
				cFGFlowConn_Var.m_FieldName = propertyName;
				cFGFlowConn_Var.m_Field = GetType().GetField(cFGFlowConn_Var.m_FieldName);
				cFGFlowConnector = cFGFlowConn_Var;
				break;
			}
			LogWarning("Unable to create variable connector with type : " + flowVarType);
			return null;
		}
		}
		cFGFlowConnector.m_ConnType = connType;
		cFGFlowConnector.m_ConnDir = connDir;
		cFGFlowConnector.m_ConnName = connName;
		cFGFlowConnector.m_OwningNode = this;
		return cFGFlowConnector;
	}

	public virtual void CreateAutoConnectors()
	{
		FieldInfo[] fields = GetType().GetFields();
		for (int i = 0; i < fields.GetLength(0); i++)
		{
			CFGFlowProperty[] array = fields[i].GetCustomAttributes(typeof(CFGFlowProperty), inherit: true) as CFGFlowProperty[];
			if (array.GetLength(0) > 0)
			{
				ConnectorDirection connDir = (array[0].bWritable ? ConnectorDirection.CD_Output : ConnectorDirection.CD_Input);
				Type flowVarType = ((array[0].expectedType != null) ? array[0].expectedType : CFGFlowVariable.GetFlowVarType(fields[i].FieldType));
				CreateConnector(connDir, ConnectorType.CT_Var, array[0].displayName, flowVarType, fields[i].FieldType, fields[i].Name, array[0].maxLinks);
			}
		}
	}

	public virtual void Activated()
	{
	}

	public virtual void DeActivated()
	{
		for (int i = 0; i < m_Outputs.Count; i++)
		{
			if (m_Outputs[i].m_HasImpulse)
			{
				m_Outputs[i].ActivateConnector();
			}
		}
	}

	public virtual void ReActivated()
	{
	}

	public virtual List<CFGFlowNode> GetImpulsedFlow()
	{
		List<CFGFlowNode> list = new List<CFGFlowNode>();
		for (int i = 0; i < m_Outputs.Count && !m_Outputs[i].m_Disabled; i++)
		{
			if (!m_Outputs[i].m_HasImpulse)
			{
				continue;
			}
			m_Outputs[i].m_ActivateCount++;
			foreach (CFGFlowConn_Exec link in m_Outputs[i].m_Links)
			{
				if (link.m_HasImpulse && !link.m_Disabled)
				{
					link.m_ActivateCount++;
					if ((bool)link.m_OwningNode)
					{
						list.Add(link.m_OwningNode);
					}
				}
			}
		}
		return list;
	}

	public int GetImpulsedInputIndex()
	{
		for (int i = 0; i < m_Inputs.Count; i++)
		{
			if (m_Inputs[i].m_HasImpulse)
			{
				return i;
			}
		}
		return -1;
	}

	public void ClearInputs()
	{
		for (int i = 0; i < m_Inputs.Count; i++)
		{
			m_Inputs[i].m_HasImpulse = false;
		}
	}

	public void ClearOutputs()
	{
		for (int i = 0; i < m_Outputs.Count; i++)
		{
			m_Outputs[i].m_HasImpulse = false;
		}
	}

	public virtual void PostDeActivated()
	{
	}

	public virtual bool UpdateFlow(float deltaTime)
	{
		return true;
	}

	public virtual void PublishVariableValues()
	{
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			if (var.m_Field != null)
			{
				var.m_Value = var.m_Field.GetValue(this);
			}
			if (var.m_ConnDir != ConnectorDirection.CD_Output || var.m_Links.Count > 0)
			{
			}
			if (var.m_ConnDir == ConnectorDirection.CD_Output && var.m_VarLinks.Count > 0)
			{
				CFGFlowVariable cFGFlowVariable = var.m_VarLinks[0] as CFGFlowVariable;
				cFGFlowVariable.SetVariable(var.m_Value);
			}
		}
	}

	public virtual void PopulateVariableValues()
	{
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			if (var.m_VarLinks.Count > 0)
			{
				if (var.m_MaxVars == 1)
				{
					if (var.m_VarLinks[0] is CFGFlowVariable)
					{
						CFGFlowVariable cFGFlowVariable = var.m_VarLinks[0] as CFGFlowVariable;
						var.m_Value = cFGFlowVariable.GetVariableOfType(var.m_ValueType.SystemType);
					}
					else
					{
						var.m_Value = var.m_VarLinks[0];
					}
				}
				else
				{
					List<object> list = new List<object>();
					foreach (CFGFlowObject varLink in var.m_VarLinks)
					{
						if (varLink is CFGFlowVariable)
						{
							CFGFlowVariable cFGFlowVariable2 = varLink as CFGFlowVariable;
							list.Add(cFGFlowVariable2.GetVariableOfType(var.m_ValueType.SystemType));
						}
						else
						{
							list.Add(varLink);
						}
					}
					var.m_Value = list;
				}
			}
			if (var.m_Field != null && (var.m_VarLinks.Count > 0 || var.m_Links.Count > 0) && var.m_Field.FieldType != typeof(CFGFlowVariable))
			{
				try
				{
					var.m_Field.SetValue(this, var.m_Value);
				}
				catch (ArgumentException ex)
				{
					LogError(ex.Message);
				}
			}
		}
	}

	public List<string> GetLinkedVariableValues()
	{
		List<string> list = new List<string>();
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			if (var.m_ConnDir != 0)
			{
				continue;
			}
			if (var.IsLinkedToSingleVariable())
			{
				CFGFlowVariable cFGFlowVariable = var.m_VarLinks[0] as CFGFlowVariable;
				object obj = cFGFlowVariable.GetVariableOfType(var.m_ValueType.SystemType);
				if (obj == null)
				{
					list.Add("NULL");
					continue;
				}
				if (obj.GetType() == typeof(string))
				{
					obj = string.Concat("\"", obj, "\"");
				}
				list.Add(obj.ToString());
			}
			else if (var.m_Field != null)
			{
				object obj2 = null;
				if (var.m_Field.FieldType != typeof(string))
				{
					obj2 = ((var.m_Field.GetValue(this) == null) ? "NONE" : var.m_Field.GetValue(this));
				}
				else
				{
					obj2 = var.m_Field.GetValue(this);
					obj2 = ((!string.IsNullOrEmpty(obj2.ToString())) ? string.Concat("\"", obj2, "\"") : "NONE");
				}
				list.Add(obj2.ToString());
			}
			else if (var.m_Value != null)
			{
				if (var.m_Value.GetType() == typeof(string))
				{
					list.Add("\"" + var.m_Value.ToString() + "\"");
				}
				else
				{
					list.Add(var.m_Value.ToString());
				}
			}
			else
			{
				list.Add("NONE");
			}
		}
		return list;
	}

	public List<CFGFlowConn_Exec> GetConnectors(ConnectorDirection direction)
	{
		return (direction != 0) ? m_Outputs : m_Inputs;
	}

	public CFGFlowConn_Exec GetInputByName(string name)
	{
		return GetExecConnByNameAndDirection(name, ConnectorDirection.CD_Input);
	}

	public CFGFlowConn_Exec GetOutputByName(string name)
	{
		return GetExecConnByNameAndDirection(name, ConnectorDirection.CD_Output);
	}

	public CFGFlowConn_Var GetVarConnByName(string name)
	{
		return m_Vars.FirstOrDefault((CFGFlowConn_Var x) => x.m_ConnName == name);
	}

	public CFGFlowConn_Var GetVarConnByFieldName(string name)
	{
		return m_Vars.FirstOrDefault((CFGFlowConn_Var x) => x.m_FieldName == name);
	}

	public CFGFlowConn_Exec GetExecConnByNameAndDirection(string name, ConnectorDirection direction)
	{
		return GetConnectors(direction).FirstOrDefault((CFGFlowConn_Exec x) => x.m_ConnName == name);
	}

	public CFGFlowConn_Var GetVarConnByNameAndType(string name, Type type)
	{
		return m_Vars.FirstOrDefault((CFGFlowConn_Var x) => x.m_ConnName == name && x.m_FlowVarType.SystemType.Equals(type));
	}

	public void ReconnectExecConnectionsTo(CFGFlowNode node, ConnectorDirection direction)
	{
		foreach (CFGFlowConn_Exec connector in GetConnectors(direction))
		{
			CFGFlowConn_Exec execConnByNameAndDirection = node.GetExecConnByNameAndDirection(connector.m_ConnName, direction);
			foreach (CFGFlowConnector link in connector.m_Links)
			{
				if (execConnByNameAndDirection != null)
				{
					execConnByNameAndDirection.Link(link);
					continue;
				}
				link.m_OwningNode.m_NeedAttention = true;
				node.m_NeedAttention = true;
			}
			connector.UnLinkAll();
		}
		if (GetConnectors(direction).Count != node.GetConnectors(direction).Count)
		{
			node.m_NeedAttention = true;
		}
	}

	private void ReconnectVarConnectionsTo(CFGFlowNode node)
	{
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			CFGFlowConn_Var varConnByNameAndType = node.GetVarConnByNameAndType(var.m_ConnName, var.m_FlowVarType.SystemType);
			foreach (CFGFlowObject varLink in var.m_VarLinks)
			{
				if (varConnByNameAndType != null)
				{
					varConnByNameAndType.Link(varLink);
					continue;
				}
				varLink.m_NeedAttention = true;
				node.m_NeedAttention = true;
			}
			var.UnLinkAll();
		}
		if (m_Vars.Count != node.m_Vars.Count())
		{
			node.m_NeedAttention = true;
		}
	}

	protected CFGFlowProperty GetFlowPropertyAttribute(FieldInfo fi, bool inherit)
	{
		CFGFlowProperty[] array = fi.GetCustomAttributes(typeof(CFGFlowProperty), inherit) as CFGFlowProperty[];
		if (array.GetLength(0) > 0)
		{
			return array[0];
		}
		return null;
	}

	public void CopyValuesTo(CFGFlowNode node)
	{
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] fields2 = node.GetType().GetFields();
		for (int i = 0; i < fields.GetLength(0); i++)
		{
			FieldInfo fromFI = fields[i];
			CFGFlowProperty flowPropertyAttribute = GetFlowPropertyAttribute(fromFI, inherit: true);
			if (flowPropertyAttribute == null)
			{
				continue;
			}
			FieldInfo fieldInfo = fields2.FirstOrDefault((FieldInfo item) => item.Name == fromFI.Name && item.FieldType == fromFI.FieldType);
			if (fieldInfo != null)
			{
				CFGFlowProperty flowPropertyAttribute2 = GetFlowPropertyAttribute(fromFI, inherit: true);
				if (flowPropertyAttribute.bWritable == flowPropertyAttribute2.bWritable)
				{
					fieldInfo.SetValue(node, fromFI.GetValue(this));
				}
			}
		}
	}

	public virtual void ReconnectLinksTo(CFGFlowNode node)
	{
		ReconnectExecConnectionsTo(node, ConnectorDirection.CD_Input);
		ReconnectExecConnectionsTo(node, ConnectorDirection.CD_Output);
		ReconnectVarConnectionsTo(node);
	}

	public bool IsAllVariableConnectorsLinked()
	{
		return !m_Vars.Any((CFGFlowConn_Var varConn) => !varConn.IsLinked());
	}

	public virtual bool IsAllVariableConnectorsLinkedOrDefaults()
	{
		bool result = true;
		CFGFlowNode obj = (CFGFlowNode)ScriptableObject.CreateInstance(GetType());
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			if (var.IsLinked() || var.m_Field == null)
			{
				continue;
			}
			object value = var.m_Field.GetValue(this);
			object value2 = var.m_Field.GetValue(obj);
			if (var.m_Field.FieldType.IsValueType)
			{
				if (object.Equals(value, value2))
				{
					continue;
				}
				result = false;
				break;
			}
			if (var.m_Field.FieldType == typeof(string) && !object.Equals(value ?? string.Empty, value2 ?? string.Empty))
			{
				result = false;
				break;
			}
			if (!var.m_Field.FieldType.IsGenericList())
			{
				continue;
			}
			IList list = value2 as IList;
			IList list2 = value as IList;
			bool flag = list == null || list.Count == 0;
			bool flag2 = list2 == null || list2.Count == 0;
			if (flag && flag2)
			{
				continue;
			}
			int num = list?.Count ?? 0;
			int num2 = list2?.Count ?? 0;
			if (num != num2)
			{
				result = false;
				break;
			}
			foreach (object item in list2)
			{
				if (!list.Contains(item))
				{
					result = false;
					break;
				}
			}
		}
		UnityEngine.Object.DestroyImmediate(obj);
		return result;
	}

	public virtual bool IsVariableValuesValid()
	{
		foreach (CFGFlowConn_Var item in m_Vars.Where((CFGFlowConn_Var c) => c.m_ConnDir == ConnectorDirection.CD_Input && c.IsLinkedToSingleVariable()))
		{
			CFGFlowVariable cFGFlowVariable = (CFGFlowVariable)item.m_VarLinks.First();
			if (cFGFlowVariable.GetVariableOfType(item.m_ValueType.SystemType) != null || cFGFlowVariable.Value == null || cFGFlowVariable.Value.GetType() == typeof(UnityEngine.Object) || item.m_ValueType.SystemType == typeof(void) || cFGFlowVariable.Value.GetType().IsClassOrSubclassOf(item.m_ValueType.SystemType))
			{
				continue;
			}
			Log($"Invalid variable linked to [{item.m_ConnName}] -> expected type: [{item.m_ValueType.SystemType}]; given type: [{cFGFlowVariable.Value.GetType()}]");
			return false;
		}
		return true;
	}

	public override bool IsOK()
	{
		bool flag = !IsConnectedToItself;
		bool isConnectedToFlow = IsConnectedToFlow;
		bool flag2 = IsAllVariableConnectorsLinkedOrDefaults();
		bool flag3 = IsVariableValuesValid();
		return isConnectedToFlow && flag2 && flag3 && flag;
	}

	public static CFGFlowNode CreateNode(Type nodeType, CFGFlowSequence parentSequence, Vector2 position)
	{
		CFGFlowNode cFGFlowNode = ScriptableObject.CreateInstance(nodeType) as CFGFlowNode;
		cFGFlowNode.m_ParentSequence = parentSequence;
		cFGFlowNode.m_Position = position;
		return cFGFlowNode;
	}

	public virtual void InitFromInfo(FlowNodeInfo info)
	{
		m_DisplayName = info.DisplayName;
	}

	public void RemoveLinkedNullVariables()
	{
		foreach (CFGFlowConn_Var var in m_Vars)
		{
			var.m_VarLinks.RemoveAll((CFGFlowObject x) => x == null);
		}
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.GenericNode;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		if (GetFOS_Type() == EFOSType.GenericNode)
		{
			Parent = BaseSerialization(Parent);
			if (Parent == null)
			{
				return false;
			}
		}
		Parent.Attrib_Set("Active", m_Active);
		Parent.Attrib_Set("ActivateCount", m_ActivateCount);
		Parent.Attrib_Set("Disabled", m_Disabled);
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		m_Active = _FlowObject.Attrib_Get("Active", m_Active);
		m_ActivateCount = _FlowObject.Attrib_Get("ActivateCount", m_ActivateCount);
		m_Disabled = _FlowObject.Attrib_Get("Disabled", m_Disabled);
		return true;
	}
}
