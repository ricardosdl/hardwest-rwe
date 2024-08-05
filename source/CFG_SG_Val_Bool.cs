using System;

internal class CFG_SG_Val_Bool : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Bool;

	public override void FromString(string Source, Type _SysType)
	{
		_Type = typeof(bool);
		if (string.Compare(Source, "true", ignoreCase: true) == 0)
		{
			Value = true;
		}
		else
		{
			Value = false;
		}
	}

	public override string ToString()
	{
		if ((bool)Value)
		{
			return "true";
		}
		return "false";
	}
}
