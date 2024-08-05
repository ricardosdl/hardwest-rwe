public class CFGFlowEvent_SequenceActivated : CFGFlowGameEvent
{
	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
	}

	public new static FlowEventInfo GetEventInfo()
	{
		FlowEventInfo flowEventInfo = new FlowEventInfo();
		flowEventInfo.Type = typeof(CFGFlowEvent_SequenceActivated);
		flowEventInfo.DisplayName = "Sequence Activated";
		return flowEventInfo;
	}

	public override void Activated()
	{
		m_Outputs[0].m_HasImpulse = true;
	}
}
