using UnityEngine;

public class CFGFlowAct_ArithmeticInt : CFGFlowGameAction
{
	public EArithmeticOp m_Op;

	[CFGFlowProperty(displayName = "A", expectedType = typeof(CFGFlowVar_Int))]
	public int m_A;

	[CFGFlowProperty(displayName = "B", expectedType = typeof(CFGFlowVar_Int))]
	public int m_B;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	[HideInInspector]
	public int m_IntResult;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	[HideInInspector]
	public float m_FloatResult;

	public override void Activated()
	{
		switch (m_Op)
		{
		case EArithmeticOp.EAO_Add:
			m_IntResult = m_A + m_B;
			m_FloatResult = m_IntResult;
			break;
		case EArithmeticOp.EAO_Subtract:
			m_IntResult = m_A - m_B;
			m_FloatResult = m_IntResult;
			break;
		case EArithmeticOp.EAO_Multiply:
			m_IntResult = m_A * m_B;
			m_FloatResult = m_IntResult;
			break;
		case EArithmeticOp.EAO_Divide:
			if (m_B != 0)
			{
				m_FloatResult = (float)m_A / (float)m_B;
				m_IntResult = (int)m_FloatResult;
			}
			else
			{
				m_IntResult = 0;
				m_FloatResult = 0f;
			}
			break;
		}
	}

	public override string GetDisplayName()
	{
		return m_Op switch
		{
			EArithmeticOp.EAO_Add => "Add Int", 
			EArithmeticOp.EAO_Subtract => "Subtract Int", 
			EArithmeticOp.EAO_Multiply => "Multiply Int", 
			EArithmeticOp.EAO_Divide => "Divide Int", 
			_ => base.GetDisplayName(), 
		};
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ArithmeticInt);
		flowActionInfo.DisplayName = "Arithmetic Int";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
