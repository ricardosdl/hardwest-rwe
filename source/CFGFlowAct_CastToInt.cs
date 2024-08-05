public class CFGFlowAct_CastToInt : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Float))]
	public float m_Float;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int m_Result;

	public override void Activated()
	{
		m_Result = (int)m_Float;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_CastToInt);
		flowActionInfo.DisplayName = "Cast To Int";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
