using System;

internal class CFG_SG_Val_GUID : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.guid;

	public override void FromString(string Source, Type _SysType)
	{
		Value = new CFGGUID();
		((CFGGUID)Value).FromString(Source);
		_Type = typeof(CFGGUID);
	}

	public override string ToString()
	{
		if (Value == null)
		{
			return string.Empty;
		}
		return ((CFGGUID)Value).ToString();
	}
}
