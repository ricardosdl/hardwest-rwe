using System;

internal class CFG_SG_Val_Uint32 : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Uint32;

	public override void FromString(string Source, Type _SysType)
	{
		if (_SysType == typeof(uint))
		{
			Value = uint.Parse(Source);
			_Type = _SysType;
		}
	}

	public override string ToString()
	{
		return ((uint)Value).ToString();
	}
}
