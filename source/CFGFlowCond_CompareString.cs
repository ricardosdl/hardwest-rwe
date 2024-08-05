public class CFGFlowCond_CompareString : CFGFlowGameCondition
{
	[CFGFlowProperty(displayName = "A", expectedType = typeof(CFGFlowVar_String))]
	public string m_A;

	[CFGFlowProperty(displayName = "B", expectedType = typeof(CFGFlowVar_String))]
	public string m_B;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A == B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A != B", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_A == null)
		{
			m_A = string.Empty;
		}
		if (m_B == null)
		{
			m_B = string.Empty;
		}
		if (m_A == m_B)
		{
			m_Outputs[0].m_HasImpulse = true;
		}
		else
		{
			m_Outputs[1].m_HasImpulse = true;
		}
	}

	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_CompareString);
		flowConditionInfo.DisplayName = "Compare String";
		flowConditionInfo.CategoryName = "Comparison";
		return flowConditionInfo;
	}
}
