public class CFGFlowCond_CompareBool : CFGFlowGameCondition
{
	[CFGFlowProperty(displayName = "Bool", expectedType = typeof(CFGFlowVar_Bool))]
	public bool m_BoolTarget;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "True", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "False", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_BoolTarget)
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
		flowConditionInfo.Type = typeof(CFGFlowCond_CompareBool);
		flowConditionInfo.DisplayName = "Compare Bool";
		flowConditionInfo.CategoryName = "Comparison";
		return flowConditionInfo;
	}
}
