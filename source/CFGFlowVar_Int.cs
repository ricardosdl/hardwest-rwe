using System;

public class CFGFlowVar_Int : CFGFlowVar_Typed<int>
{
	public new static bool SupportsType(Type varType)
	{
		return varType.IsEnum || CFGFlowVar_Typed<int>.SupportsType(varType);
	}

	public override object GetVariableOfType(Type varType)
	{
		if (varType.IsEnum)
		{
			return m_Value;
		}
		return base.GetVariableOfType(varType);
	}

	public override void SetVariable(object varObj)
	{
		if (varObj.GetType().IsEnum)
		{
			m_Value = (int)varObj;
		}
		base.SetVariable(varObj);
	}

	public override string GetTypeName()
	{
		return "Int";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarInt;
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
