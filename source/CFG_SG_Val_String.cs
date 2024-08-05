using System;

internal class CFG_SG_Val_String : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.String;

	public override void FromString(string Source, Type _SysType)
	{
		Value = Source;
		_Type = typeof(string);
	}

	public override string ToString()
	{
		return (Value != null) ? ((string)Value) : string.Empty;
	}
}
