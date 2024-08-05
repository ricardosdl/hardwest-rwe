public class CFGValue_String : CFGValue
{
	public string m_Value = string.Empty;

	public string m_Original = string.Empty;

	public CFGValue_String()
	{
		m_Type = EType.String;
	}

	public CFGValue_String(string Val)
	{
		m_Type = EType.Float;
		m_Value = Val;
		m_Original = Val;
	}

	public override bool IsModified()
	{
		return m_Value != m_Original;
	}

	public bool Set(string NewVal)
	{
		if (NewVal == null)
		{
			m_Value = string.Empty;
		}
		else
		{
			m_Value = NewVal;
		}
		return true;
	}
}
