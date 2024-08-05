public class CFGFlowAct_AssignString : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_String))]
	public string m_Value;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_String), bWritable = true)]
	public string m_Result;

	public override void Activated()
	{
		m_Result = m_Value;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AssignString);
		flowActionInfo.DisplayName = "Assign String";
		flowActionInfo.CategoryName = "Set Variable";
		return flowActionInfo;
	}
}
