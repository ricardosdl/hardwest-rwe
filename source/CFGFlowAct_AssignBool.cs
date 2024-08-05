public class CFGFlowAct_AssignBool : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Bool))]
	public bool m_Value;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Bool), bWritable = true)]
	public bool m_Result;

	public override void Activated()
	{
		m_Result = m_Value;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AssignBool);
		flowActionInfo.DisplayName = "Assign Bool";
		flowActionInfo.CategoryName = "Set Variable";
		return flowActionInfo;
	}
}
