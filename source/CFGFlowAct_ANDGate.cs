public class CFGFlowAct_ANDGate : CFGFlowGameAction
{
	protected int[] m_ActivateLinks;

	protected bool[] m_Passes;

	public override bool AllowMultiInFrame => false;

	public override bool DefaultOutput => false;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_Inputs.Count > 0 && m_Inputs[0] != null)
		{
			m_ActivateLinks = new int[m_Inputs[0].m_Links.Count];
			m_Passes = new bool[m_Inputs[0].m_Links.Count];
		}
	}

	public override void Activated()
	{
		for (int i = 0; i < m_Inputs[0].m_Links.Count; i++)
		{
			CFGFlowConn_Exec cFGFlowConn_Exec = m_Inputs[0].m_Links[i] as CFGFlowConn_Exec;
			if (cFGFlowConn_Exec != null && cFGFlowConn_Exec.m_ActivateCount > m_ActivateLinks[i])
			{
				m_Passes[i] = true;
			}
		}
		for (int j = 0; j < m_Passes.GetLength(0); j++)
		{
			if (!m_Passes[j])
			{
				return;
			}
		}
		for (int k = 0; k < m_Inputs[0].m_Links.Count; k++)
		{
			CFGFlowConn_Exec cFGFlowConn_Exec2 = m_Inputs[0].m_Links[k] as CFGFlowConn_Exec;
			if (cFGFlowConn_Exec2 != null)
			{
				m_ActivateLinks[k] = cFGFlowConn_Exec2.m_ActivateCount;
				m_Passes[k] = false;
			}
		}
		m_Outputs[0].m_HasImpulse = true;
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
		flowActionInfo.Type = typeof(CFGFlowAct_ANDGate);
		flowActionInfo.DisplayName = "AND Gate";
		flowActionInfo.CategoryName = "Misc";
		return flowActionInfo;
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.ANDGate;
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
		cFG_SG_Node.Attrib_Set("Count", m_Passes.Length);
		for (int i = 0; i < m_Passes.Length; i++)
		{
			string attName = "ID" + i;
			cFG_SG_Node.Attrib_Set(attName, m_Passes[i]);
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		int num = _FlowObject.Attrib_Get("Count", -1);
		if (num != m_Passes.Length)
		{
			LogWarning("AND Gate: mismatch of length!");
			return true;
		}
		for (int i = 0; i < num; i++)
		{
			string attName = "ID" + i;
			bool flag = _FlowObject.Attrib_Get(attName, m_Passes[i]);
			m_Passes[i] = flag;
		}
		return true;
	}
}
