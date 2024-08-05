using System;
using UnityEngine;

public class CFGFlowVar_Enum : CFGFlowVar_Typed<Enum>
{
	[HideInInspector]
	public CFGType m_EnumType;

	[HideInInspector]
	public int m_ProperValue;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_EnumType != null && m_EnumType.SystemType != null)
		{
			string text = Enum.GetName(m_EnumType.SystemType, m_ProperValue);
			if (text == null)
			{
				LogWarning(string.Concat("Unable to find enum of type ", m_EnumType.SystemType, " with value ", m_ProperValue));
			}
			else
			{
				m_Value = (Enum)Enum.Parse(m_EnumType.SystemType, text);
			}
		}
	}

	public new static bool SupportsType(Type varType)
	{
		if (varType.IsEnum)
		{
			return true;
		}
		return false;
	}

	public override object GetVariableOfType(Type varType)
	{
		if (varType.IsEnum || varType == typeof(Enum))
		{
			return m_Value;
		}
		return null;
	}

	public override void SetVariable(object varObj)
	{
		m_Value = (Enum)varObj;
		m_EnumType = new CFGType(varObj.GetType());
		m_ProperValue = (int)varObj;
	}

	public override string GetValueName()
	{
		if (m_Value != null)
		{
			return m_Value.ToString();
		}
		return "Please reassign";
	}

	public override string GetTypeName()
	{
		if (m_EnumType != null && m_EnumType.SystemType != null)
		{
			return m_EnumType.SystemType.ToString();
		}
		return "Unsupported";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarEnum;
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
		if (m_Value == null)
		{
			cFG_SG_Node.Attrib_Set("Disabled", 1);
		}
		else
		{
			cFG_SG_Node.Attrib_Add("Value", m_Value.GetType(), m_Value.ToString());
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		if (_FlowObject.Attrib_Get("Disabled", 0, bReport: false) == 0)
		{
			object _Value = m_Value;
			if (_FlowObject.Attrib_GetWithType("Value", m_Value.GetType(), ref _Value))
			{
				m_Value = (Enum)_Value;
			}
		}
		else
		{
			m_Value = null;
		}
		return true;
	}
}
