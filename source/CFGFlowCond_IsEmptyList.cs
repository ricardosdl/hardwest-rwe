using System.Collections.Generic;

public abstract class CFGFlowCond_IsEmptyList<T> : CFGFlowGameCondition
{
	public List<T> m_List;

	public int m_Count;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Object List", CFGFlowVariable.GetFlowVarType(typeof(List<T>)), typeof(List<T>), "m_List");
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Entries Count", typeof(CFGFlowVar_Int), typeof(int), "m_Count");
		m_Inputs[0].m_ConnName = "Test if Empty";
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Empty", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Not Empty", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_List != null)
		{
			if (m_List.Count <= 0)
			{
				m_Outputs[0].m_HasImpulse = true;
			}
			else
			{
				m_Outputs[1].m_HasImpulse = true;
			}
			m_Count = m_List.Count;
		}
		else
		{
			m_Outputs[0].m_HasImpulse = true;
			m_Count = 0;
		}
	}
}
