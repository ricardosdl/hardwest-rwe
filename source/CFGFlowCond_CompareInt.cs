public class CFGFlowCond_CompareInt : CFGFlowGameCondition
{
	[CFGFlowProperty(displayName = "A", expectedType = typeof(CFGFlowVar_Int))]
	public int m_A;

	[CFGFlowProperty(displayName = "B", expectedType = typeof(CFGFlowVar_Int))]
	public int m_B;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A <= B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A > B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A == B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A < B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A >= B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A != B", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_A < m_B)
		{
			m_Outputs[3].m_HasImpulse = true;
			m_Outputs[0].m_HasImpulse = true;
			m_Outputs[5].m_HasImpulse = true;
		}
		else if (m_A > m_B)
		{
			m_Outputs[1].m_HasImpulse = true;
			m_Outputs[4].m_HasImpulse = true;
			m_Outputs[5].m_HasImpulse = true;
		}
		else if (m_A == m_B)
		{
			m_Outputs[2].m_HasImpulse = true;
			m_Outputs[0].m_HasImpulse = true;
			m_Outputs[4].m_HasImpulse = true;
		}
	}

	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_CompareInt);
		flowConditionInfo.DisplayName = "Compare Int";
		flowConditionInfo.CategoryName = "Comparison";
		return flowConditionInfo;
	}
}
