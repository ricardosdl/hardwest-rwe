public class CFGFlowAct_SetProperty : CFGFlowAct_Property
{
	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Property", typeof(CFGFlowVariable), null, string.Empty);
	}

	public override void Activated()
	{
		base.Activated();
		if (prop != null)
		{
			SetProperty();
		}
	}

	protected void SetProperty()
	{
		if (m_Vars[1].IsLinkedToSingleVariable())
		{
			prop.SetValue(obj, m_Vars[1].m_Value, null);
		}
		else
		{
			LogError("Invalid link -> property value.");
		}
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_SetProperty);
		flowActionInfo.DisplayName = "Set Property";
		flowActionInfo.CategoryName = "Property";
		return flowActionInfo;
	}
}
