public class CFGFlowEvent_RemoteEvent : CFGFlowGameEvent
{
	public string m_RemoteName = string.Empty;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
	}

	public override string GetDisplayName()
	{
		return base.GetDisplayName() + ((!(m_RemoteName == string.Empty)) ? (": " + m_RemoteName) : string.Empty);
	}

	public new static FlowEventInfo GetEventInfo()
	{
		FlowEventInfo flowEventInfo = new FlowEventInfo();
		flowEventInfo.Type = typeof(CFGFlowEvent_RemoteEvent);
		flowEventInfo.DisplayName = "Remote Event";
		return flowEventInfo;
	}

	public override void Activated()
	{
		m_Outputs[0].m_HasImpulse = true;
	}
}
