using UnityEngine;

public class CFGValue_Float : CFGValue
{
	public float m_Value;

	public float m_Min;

	public float m_Max;

	public float m_Original;

	public CFGValue_Float()
	{
		m_Type = EType.Float;
	}

	public CFGValue_Float(float Val, float Min, float Max)
	{
		m_Type = EType.Float;
		m_Value = Val;
		m_Min = Min;
		m_Max = Max;
	}

	public override bool IsModified()
	{
		return m_Value != m_Original;
	}

	public bool Set(float NewVal)
	{
		if (m_Type != EType.Int)
		{
			Debug.LogWarning("CFGValue::" + m_Name + " is not of INT type. Cannot set. ");
			return false;
		}
		if ((m_Flags & EFlags.IncrementOnly) == EFlags.IncrementOnly && NewVal <= m_Value)
		{
			return true;
		}
		if ((m_Flags & EFlags.MinDefined) == EFlags.MinDefined && NewVal <= m_Min)
		{
			return true;
		}
		if ((m_Flags & EFlags.MaxDefined) == EFlags.MaxDefined && NewVal > m_Max)
		{
			return true;
		}
		m_Value = NewVal;
		return true;
	}
}
