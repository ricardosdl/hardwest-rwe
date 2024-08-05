using System;

public abstract class CFGFlowVar_Typed<T> : CFGFlowVariable
{
	public T m_Value;

	public override object Value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = (T)Value;
		}
	}

	public new static bool SupportsType(Type varType)
	{
		return varType == typeof(T);
	}

	public override object GetVariableOfType(Type varType)
	{
		if (varType == typeof(void) || varType == typeof(T) || typeof(T).IsSubclassOf(varType))
		{
			return m_Value;
		}
		if (m_Value is IConvertible && Convert.ChangeType(m_Value, varType) != null)
		{
			return m_Value;
		}
		return base.GetVariableOfType(varType);
	}

	public override void SetVariable(object varObj)
	{
		m_Value = (T)varObj;
	}

	public override string GetValueName()
	{
		if (m_Value == null)
		{
			return "NONE";
		}
		return m_Value.ToString();
	}
}
