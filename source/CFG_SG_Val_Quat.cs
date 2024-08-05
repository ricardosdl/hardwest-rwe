using System;
using UnityEngine;

internal class CFG_SG_Val_Quat : CFG_SG_Value
{
	public override eSG_ValueType ValueType => eSG_ValueType.Quaternion;

	public override void FromString(string Source, Type _SysType)
	{
		Value = Quaternion.identity;
		string[] array = Source.Split(',');
		if (array.Length >= 4)
		{
			try
			{
				Quaternion identity = Quaternion.identity;
				identity.x = float.Parse(array[0]);
				identity.y = float.Parse(array[1]);
				identity.z = float.Parse(array[2]);
				identity.w = float.Parse(array[3]);
				Value = identity;
			}
			catch
			{
			}
			_Type = typeof(Quaternion);
		}
	}

	public override string ToString()
	{
		return $"{((Quaternion)Value).x},{((Quaternion)Value).y},{((Quaternion)Value).z},{((Quaternion)Value).w}";
	}
}
