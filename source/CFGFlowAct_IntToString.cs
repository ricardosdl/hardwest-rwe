public class CFGFlowAct_IntToString : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Int;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_String), bWritable = true)]
	public string m_Result;

	public override void Activated()
	{
		m_Result = m_Int.ToString();
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_IntToString);
		flowActionInfo.DisplayName = "Int To String";
		flowActionInfo.CategoryName = "Texts";
		return flowActionInfo;
	}
}
