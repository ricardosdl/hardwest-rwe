using UnityEngine;

public class CFGFlowAct_ArithmeticFloat : CFGFlowGameAction
{
	public EArithmeticOp m_Op;

	[CFGFlowProperty(displayName = "A", expectedType = typeof(CFGFlowVar_Float))]
	public float m_A;

	[CFGFlowProperty(displayName = "B", expectedType = typeof(CFGFlowVar_Float))]
	public float m_B;

	[HideInInspector]
	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_FloatResult;

	[CFGFlowProperty(displayName = "Result", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	[HideInInspector]
	public int m_IntResult;

	public override void Activated()
	{
		switch (m_Op)
		{
		case EArithmeticOp.EAO_Add:
			m_FloatResult = m_A + m_B;
			break;
		case EArithmeticOp.EAO_Subtract:
			m_FloatResult = m_A - m_B;
			break;
		case EArithmeticOp.EAO_Multiply:
			m_FloatResult = m_A * m_B;
			break;
		case EArithmeticOp.EAO_Divide:
			if (m_B != 0f)
			{
				m_FloatResult = m_A / m_B;
			}
			else
			{
				m_IntResult = 0;
			}
			break;
		}
		m_IntResult = (int)m_FloatResult;
	}

	public override string GetDisplayName()
	{
		return m_Op switch
		{
			EArithmeticOp.EAO_Add => "Add Float", 
			EArithmeticOp.EAO_Subtract => "Subtract Float", 
			EArithmeticOp.EAO_Multiply => "Multiply Float", 
			EArithmeticOp.EAO_Divide => "Divide Float", 
			_ => base.GetDisplayName(), 
		};
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ArithmeticFloat);
		flowActionInfo.DisplayName = "Arithmetic Float";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
