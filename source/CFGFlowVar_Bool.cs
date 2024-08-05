public class CFGFlowVar_Bool : CFGFlowVar_Typed<bool>
{
	public override string GetTypeName()
	{
		return "Bool";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarBool;
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
		m_Value = _FlowObject.Attrib_Get("Value", m_Value);
		return true;
	}
}
