using UnityEngine;

public class CFGFlowAct_GetVectorComp : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Vector", expectedType = typeof(CFGFlowVar_Vector))]
	public Vector3 m_Vector;

	[CFGFlowProperty(displayName = "X", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_X;

	[CFGFlowProperty(displayName = "Y", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_Y;

	[CFGFlowProperty(displayName = "Z", expectedType = typeof(CFGFlowVar_Float), bWritable = true)]
	public float m_Z;

	public override void Activated()
	{
		m_X = m_Vector.x;
		m_Y = m_Vector.y;
		m_Z = m_Vector.z;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_GetVectorComp);
		flowActionInfo.DisplayName = "Get Vector Components";
		flowActionInfo.CategoryName = "Math";
		return flowActionInfo;
	}
}
