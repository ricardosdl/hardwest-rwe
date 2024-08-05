using System;

public class CFGFlowCond_CompareEnum_v2 : CFGFlowGameCondition
{
	[CFGFlowProperty(displayName = "A", expectedType = typeof(CFGFlowVar_Enum))]
	public Enum m_A;

	[CFGFlowProperty(displayName = "B", expectedType = typeof(CFGFlowVar_Enum))]
	public Enum m_B;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A == B", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "A != B", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_A != null && m_B != null && m_A.GetType() == m_B.GetType())
		{
			CFGFlowVar_Enum cFGFlowVar_Enum = m_Vars[0].m_VarLinks[0] as CFGFlowVar_Enum;
			CFGFlowVar_Enum cFGFlowVar_Enum2 = m_Vars[1].m_VarLinks[0] as CFGFlowVar_Enum;
			if (cFGFlowVar_Enum != null && cFGFlowVar_Enum2 != null)
			{
				if (cFGFlowVar_Enum.m_ProperValue == cFGFlowVar_Enum2.m_ProperValue)
				{
					m_Outputs[0].m_HasImpulse = true;
				}
				else
				{
					m_Outputs[1].m_HasImpulse = true;
				}
				return;
			}
		}
		LogError("Could not compare enums");
	}

	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_CompareEnum_v2);
		flowConditionInfo.DisplayName = "Compare Enum";
		flowConditionInfo.CategoryName = "Comparison";
		return flowConditionInfo;
	}
}
