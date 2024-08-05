using System;

public class CFGFlowAct_GetPercentOfInt : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Value", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Value;

	[CFGFlowProperty(displayName = "Percent", expectedType = typeof(CFGFlowVar_Int))]
	public int m_Percent;

	[CFGFlowProperty(displayName = "Int", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int m_IntResult;

	[CFGFlowProperty(displayName = "Float", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_FloatResult;

	public override void Activated()
	{
		if (m_Percent == 0)
		{
			m_IntResult = 0;
			m_FloatResult = 0f;
		}
		else
		{
			m_FloatResult = (float)(m_Value * m_Percent) / 100f;
			m_IntResult = (int)Math.Round(m_FloatResult, MidpointRounding.AwayFromZero);
		}
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_GetPercentOfInt);
		flowActionInfo.DisplayName = "Get Percent of Int";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
