public class CFGFlowAct_GetProperty : CFGFlowAct_Property
{
	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Property", typeof(CFGFlowVariable), null, string.Empty);
	}

	public override void Activated()
	{
		base.Activated();
		if (prop != null)
		{
			GetProperty();
		}
	}

	protected void GetProperty()
	{
		if (m_Vars[1].IsLinkedToSingleVariable())
		{
			m_Vars[1].m_Value = prop.GetValue(obj, null);
		}
		else
		{
			LogWarning("Output variable do not exists.");
		}
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_GetProperty);
		flowActionInfo.DisplayName = "Get Property";
		flowActionInfo.CategoryName = "Property";
		return flowActionInfo;
	}
}
