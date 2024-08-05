public class CFGFlowAct_AbortSequence : CFGFlowGameAction
{
	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_ParentSequence.m_Outputs.Count >= 2)
		{
			m_ParentSequence.StopExecution();
		}
	}

	public override void DeActivated()
	{
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AbortSequence);
		flowActionInfo.DisplayName = "Abort Sequence";
		flowActionInfo.CategoryName = "Sequence";
		return flowActionInfo;
	}
}
