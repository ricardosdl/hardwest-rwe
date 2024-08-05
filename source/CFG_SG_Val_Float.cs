using System;

internal class CFG_SG_Val_Float : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Float;

	public override void FromString(string Source, Type _SysType)
	{
		float result = 0f;
		float.TryParse(Source, out result);
		Value = result;
		_Type = typeof(float);
	}

	public override string ToString()
	{
		return ((float)Value).ToString();
	}
}
