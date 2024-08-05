using UnityEngine;

public class CFGValue_Int : CFGValue
{
	public int m_Value;

	public int m_Min;

	public int m_Max;

	public int m_Original;

	public CFGValue_Int()
	{
		m_Type = EType.Int;
	}

	public CFGValue_Int(int Val, int Min, int Max)
	{
		m_Type = EType.Int;
		m_Value = Val;
		m_Min = Min;
		m_Max = Max;
		m_Original = Val;
	}

	public override bool IsModified()
	{
		return m_Original != m_Value;
	}

	public bool Set(int NewVal)
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
