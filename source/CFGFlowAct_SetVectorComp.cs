using UnityEngine;

public class CFGFlowAct_SetVectorComp : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "X", expectedType = typeof(CFGFlowVar_Float))]
	public float m_X;

	[CFGFlowProperty(displayName = "Y", expectedType = typeof(CFGFlowVar_Float))]
	public float m_Y;

	[CFGFlowProperty(displayName = "Z", expectedType = typeof(CFGFlowVar_Float))]
	public float m_Z;

	[CFGFlowProperty(displayName = "Vector", expectedType = typeof(CFGFlowVar_Vector), bWritable = true)]
	public Vector3 m_Vector;

	public override void Activated()
	{
		m_Vector.x = m_X;
		m_Vector.y = m_Y;
		m_Vector.z = m_Z;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_SetVectorComp);
		flowActionInfo.DisplayName = "Set Vector Components";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
