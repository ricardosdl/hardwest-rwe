public class CFGFlowAct_AssignFloat : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Float))]
	public float m_Value;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_Result;

	public override void Activated()
	{
		m_Result = m_Value;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AssignFloat);
		flowActionInfo.DisplayName = "Assign Float";
		flowActionInfo.CategoryName = "Set Variable";
		return flowActionInfo;
	}
}
