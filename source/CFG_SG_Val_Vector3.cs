using System;
using UnityEngine;

internal class CFG_SG_Val_Vector3 : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Vector3;

	public override void FromString(string Source, Type _SysType)
	{
		Value = Vector3.zero;
		string[] array = Source.Split(',');
		if (array.Length >= 3)
		{
			try
			{
				Value = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
			}
			catch
			{
			}
			_Type = typeof(Vector3);
		}
	}

	public override string ToString()
	{
		return $"{((Vector3)Value).x},{((Vector3)Value).y},{((Vector3)Value).z}";
	}
}
