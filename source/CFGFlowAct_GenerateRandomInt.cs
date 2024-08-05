using UnityEngine;

public class CFGFlowAct_GenerateRandomInt : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Min", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Min;

	[CFGFlowProperty(displayName = "Max", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Max;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int m_Result;

	public override void Activated()
	{
		m_Result = Random.Range(m_Min, m_Max);
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_GenerateRandomInt);
		flowActionInfo.DisplayName = "Generate Random Int";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
