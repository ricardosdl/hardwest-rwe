public class CFGFlowAct_CastToFloat : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Int;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_Result;

	public override void Activated()
	{
		m_Result = m_Int;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_CastToFloat);
		flowActionInfo.DisplayName = "Cast To Float";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
