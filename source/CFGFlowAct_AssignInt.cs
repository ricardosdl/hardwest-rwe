public class CFGFlowAct_AssignInt : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Value;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int m_Result;

	public override void Activated()
	{
		m_Result = m_Value;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AssignInt);
		flowActionInfo.DisplayName = "Assign Int";
		flowActionInfo.CategoryName = "Set Variable";
		return flowActionInfo;
	}
}
