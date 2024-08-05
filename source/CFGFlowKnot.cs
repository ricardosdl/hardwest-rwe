public class CFGFlowKnot : CFGFlowNode
{
	public bool reversed;

	public void Init()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, string.Empty, null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, string.Empty, null, null, string.Empty);
	}

	public override string GetDisplayName()
	{
		return "Joint";
	}

	public override void DeActivated()
	{
		m_Outputs[0].m_HasImpulse = true;
		base.DeActivated();
	}
}
