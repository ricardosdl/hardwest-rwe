public class CFGFlowVar_String : CFGFlowVar_Typed<string>
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_Value == null)
		{
			m_Value = string.Empty;
		}
	}

	public override string GetTypeName()
	{
		return "String";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarString;
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
		cFG_SG_Node.Attrib_Set("Value", m_Value);
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		if (!_FlowObject.Attrib_Get("Value", ref m_Value))
		{
			m_Value = string.Empty;
		}
		return true;
	}
}
