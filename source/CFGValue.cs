using System;

public class CFGValue
{
	public enum EType
	{
		Unknown,
		Int,
		Float,
		String
	}

	[Flags]
	public enum EFlags
	{
		None = 0,
		MinDefined = 1,
		MaxDefined = 2,
		IncrementOnly = 4
	}

	public EType m_Type;

	public EFlags m_Flags;

	public string m_Name = string.Empty;

	public virtual bool IsModified()
	{
		return false;
	}
}
