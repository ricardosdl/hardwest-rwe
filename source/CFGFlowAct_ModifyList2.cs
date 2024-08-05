using System.Collections.Generic;
using UnityEngine;

public abstract class CFGFlowAct_ModifyList2<T> : CFGFlowGameAction
{
	public List<T> m_List;

	public T m_Object;

	[HideInInspector]
	public int m_Count;

	public int m_Index;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Item", CFGFlowVariable.GetFlowVarType(typeof(T)), typeof(T), "m_Object");
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "List", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), "m_List");
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Index", typeof(CFGFlowVar_Int), typeof(int), "m_Index");
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Entries Count", typeof(CFGFlowVar_Int), typeof(int), "m_Count");
		m_Inputs[0].m_ConnName = "Add To List";
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Remove From List", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Empty List", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Replace At Index", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_List == null)
		{
			m_Outputs[0].ActivateConnector();
			return;
		}
		switch (GetImpulsedInputIndex())
		{
		case 0:
			if (IsValidToAdd())
			{
				m_List.Add(m_Object);
			}
			break;
		case 1:
			m_List.Remove(m_Object);
			break;
		case 2:
			m_List.Clear();
			break;
		case 3:
			if (m_List.Count > m_Index)
			{
				m_List[m_Index] = m_Object;
				break;
			}
			LogError("FlowModifyList -> Index out of range (" + m_Index + " out of " + (m_List.Count - 1) + ")");
			break;
		}
		m_Count = m_List.Count;
	}

	protected virtual bool IsValidToAdd()
	{
		return m_Object != null && !m_List.Contains(m_Object);
	}
}
