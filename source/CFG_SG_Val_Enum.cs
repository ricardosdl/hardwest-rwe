using System;

internal class CFG_SG_Val_Enum : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Enum;

	public override void FromString(string Source, Type _SysType)
	{
		if (_SysType.IsEnum)
		{
			Value = Enum.Parse(_SysType, Source, ignoreCase: true);
			_Type = _SysType;
		}
	}

	public override string ToString()
	{
		return Enum.GetName(_Type, Value);
	}
}
