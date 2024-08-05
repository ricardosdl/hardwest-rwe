public class CFGFlowAct_IncDecInt : CFGFlowGameAction
{
	public enum Mode
	{
		Increment,
		Decrement
	}

	public Mode mode;

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, "Value", typeof(CFGFlowVar_Int), typeof(int), string.Empty, byte.MaxValue);
	}

	public override void Activated()
	{
		foreach (CFGFlowObject varLink in m_Vars[0].m_VarLinks)
		{
			CFGFlowVariable cFGFlowVariable = varLink as CFGFlowVariable;
			if (!(cFGFlowVariable == null) && cFGFlowVariable.Value is int)
			{
				int num = (int)cFGFlowVariable.Value;
				switch (mode)
				{
				case Mode.Increment:
					num++;
					break;
				case Mode.Decrement:
					num--;
					break;
				}
				cFGFlowVariable.SetVariable(num);
			}
		}
	}

	public override string GetDisplayName()
	{
		return mode.ToString();
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_IncDecInt);
		flowActionInfo.DisplayName = "Increment\\Decrement Int";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
