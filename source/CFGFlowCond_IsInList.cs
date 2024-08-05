using System.Collections.Generic;

public abstract class CFGFlowCond_IsInList<T> : CFGFlowGameCondition
{
	public T m_Target;

	public List<T> m_List;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Object To Test", CFGFlowVariable.GetFlowVarType(typeof(T)), typeof(T), "m_Target");
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Object List", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), "m_List");
		m_Inputs[0].m_ConnName = "Test if in List";
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "In List", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Not in List", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_Target != null && m_List != null)
		{
			if (m_List.Contains(m_Target))
			{
				m_Outputs[0].m_HasImpulse = true;
			}
			else
			{
				m_Outputs[1].m_HasImpulse = true;
			}
		}
		else
		{
			m_Outputs[1].m_HasImpulse = true;
		}
	}
}
