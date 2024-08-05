public class CFGFlowAct_Log : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Target", expectedType = typeof(CFGFlowVariable))]
	public object Target;

	public string m_Text;

	public override void Activated()
	{
		Log("Flow log: " + m_Text + " " + Target);
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_Log);
		flowActionInfo.DisplayName = "Log";
		flowActionInfo.CategoryName = "Misc";
		return flowActionInfo;
	}
}
