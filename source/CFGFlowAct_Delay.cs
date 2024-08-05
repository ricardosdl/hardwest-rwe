public class CFGFlowAct_Delay : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Time", expectedType = typeof(CFGFlowVar_Float))]
	public float m_Seconds;

	private float m_TimeLeft;

	public override void Activated()
	{
		m_TimeLeft = m_Seconds;
	}

	public override bool UpdateFlow(float deltaTime)
	{
		if (m_TimeLeft <= 0f)
		{
			return true;
		}
		m_TimeLeft -= deltaTime;
		return false;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_Delay);
		flowActionInfo.DisplayName = "Delay";
		flowActionInfo.CategoryName = "Latent";
		return flowActionInfo;
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.Delay;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = BaseSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		if (!base.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Time", m_TimeLeft);
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		m_TimeLeft = _FlowObject.Attrib_Get("Time", m_TimeLeft);
		return true;
	}
}
