public class CFGFlowAct_Gate : CFGFlowGameAction
{
	public bool m_Open = true;

	public int m_AutoCloseCount;

	private bool m_AutoClosed;

	public override bool AllowMultiInFrame => true;

	public override bool DefaultOutput => false;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Open", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Close", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "Toggle", null, null, string.Empty);
	}

	public override void Activated()
	{
		if (m_Inputs[1].m_HasImpulse)
		{
			m_Open = true;
		}
		else if (m_Inputs[2].m_HasImpulse)
		{
			m_Open = false;
		}
		else if (m_Inputs[3].m_HasImpulse)
		{
			m_Open = !m_Open;
		}
		if (m_Inputs[0].m_HasImpulse)
		{
			if (m_Open)
			{
				m_Outputs[0].m_HasImpulse = true;
			}
			if (!m_AutoClosed && m_AutoCloseCount > 0 && m_ActivateCount >= m_AutoCloseCount)
			{
				m_Open = false;
				m_AutoClosed = true;
			}
		}
	}

	public override bool UpdateFlow(float deltaTime)
	{
		return true;
	}

	public override void DeActivated()
	{
		if (m_Outputs[0].m_HasImpulse)
		{
			m_Outputs[0].ActivateConnector();
		}
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_Gate);
		flowActionInfo.DisplayName = "Gate";
		flowActionInfo.CategoryName = "Misc";
		return flowActionInfo;
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.Gate;
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
		cFG_SG_Node.Attrib_Set("Item1", m_Open);
		cFG_SG_Node.Attrib_Set("Item2", m_AutoClosed);
		cFG_SG_Node.Attrib_Set("Count", m_AutoCloseCount);
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		m_Open = _FlowObject.Attrib_Get("Item1", m_Open);
		m_AutoClosed = _FlowObject.Attrib_Get("Item2", m_AutoClosed);
		m_AutoCloseCount = _FlowObject.Attrib_Get("Count", m_AutoCloseCount);
		return true;
	}
}
