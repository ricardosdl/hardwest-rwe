using System;

internal class CFG_SG_Val_Int : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Int;

	public override void FromString(string Source, Type _SysType)
	{
		int result = 0;
		int.TryParse(Source, out result);
		Value = result;
		_Type = typeof(int);
	}

	public override string ToString()
	{
		return ((int)Value).ToString();
	}
}
