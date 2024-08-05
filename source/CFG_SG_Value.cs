using System;
using UnityEngine;

public class CFG_SG_Value
{
	public enum eSG_ValueType
	{
		Unknown,
		String,
		Int,
		Vector3,
		Quaternion,
		guid,
		Bool,
		Enum,
		Uint32,
		Float
	}

	protected object Value;

	protected Type _Type;

	public virtual eSG_ValueType ValueType => eSG_ValueType.Unknown;

	public Type ObjectType => _Type;

	public object GenericValue => Value;

	public void Set<T>(object NewValue)
	{
		Value = NewValue;
		_Type = typeof(T);
	}

	public void Set(object NewValue, Type _NewType)
	{
		Value = NewValue;
		_Type = _NewType;
	}

	public virtual void FromString(string Source, Type _SysType)
	{
	}

	public override string ToString()
	{
		return string.Empty;
	}

	public static CFG_SG_Value Create(eSG_ValueType ValType)
	{
		return ValType switch
		{
			eSG_ValueType.Int => new CFG_SG_Val_Int(), 
			eSG_ValueType.guid => new CFG_SG_Val_GUID(), 
			eSG_ValueType.Quaternion => new CFG_SG_Val_Quat(), 
			eSG_ValueType.String => new CFG_SG_Val_String(), 
			eSG_ValueType.Vector3 => new CFG_SG_Val_Vector3(), 
			eSG_ValueType.Bool => new CFG_SG_Val_Bool(), 
			eSG_ValueType.Enum => new CFG_SG_Val_Enum(), 
			eSG_ValueType.Uint32 => new CFG_SG_Val_Uint32(), 
			eSG_ValueType.Float => new CFG_SG_Val_Float(), 
			_ => null, 
		};
	}

	public static CFG_SG_Value Create(Type tp)
	{
		if (tp == typeof(int))
		{
			return new CFG_SG_Val_Int();
		}
		if (tp == typeof(CFGGUID))
		{
			return new CFG_SG_Val_GUID();
		}
		if (tp == typeof(Quaternion))
		{
			return new CFG_SG_Val_Quat();
		}
		if (tp == typeof(string))
		{
			return new CFG_SG_Val_String();
		}
		if (tp == typeof(Vector3))
		{
			return new CFG_SG_Val_Vector3();
		}
		if (tp == typeof(bool))
		{
			return new CFG_SG_Val_Bool();
		}
		if (tp.IsEnum)
		{
			return new CFG_SG_Val_Enum();
		}
		if (tp == typeof(uint))
		{
			return new CFG_SG_Val_Uint32();
		}
		if (tp == typeof(float))
		{
			return new CFG_SG_Val_Float();
		}
		return null;
	}

	public static eSG_ValueType GetType(Type T, bool bShowMsg = false)
	{
		if (T == typeof(int))
		{
			return eSG_ValueType.Int;
		}
		if (T == typeof(string))
		{
			return eSG_ValueType.String;
		}
		if (T == typeof(Vector3))
		{
			return eSG_ValueType.Vector3;
		}
		if (T == typeof(Quaternion))
		{
			return eSG_ValueType.Quaternion;
		}
		if (T == typeof(CFGGUID))
		{
			return eSG_ValueType.guid;
		}
		if (T == typeof(bool))
		{
			return eSG_ValueType.Bool;
		}
		if (T == typeof(uint))
		{
			return eSG_ValueType.Uint32;
		}
		if (T.IsEnum)
		{
			return eSG_ValueType.Enum;
		}
		if (T == typeof(float))
		{
			return eSG_ValueType.Float;
		}
		if (bShowMsg)
		{
			Debug.LogWarning("Unsupported serialization type: " + T);
		}
		return eSG_ValueType.Unknown;
	}

	public static eSG_ValueType GetType<T>(bool bShowMsg = false)
	{
		if (typeof(T) == typeof(int))
		{
			return eSG_ValueType.Int;
		}
		if (typeof(T) == typeof(string))
		{
			return eSG_ValueType.String;
		}
		if (typeof(T) == typeof(Vector3))
		{
			return eSG_ValueType.Vector3;
		}
		if (typeof(T) == typeof(Quaternion))
		{
			return eSG_ValueType.Quaternion;
		}
		if (typeof(T) == typeof(CFGGUID))
		{
			return eSG_ValueType.guid;
		}
		if (typeof(T) == typeof(bool))
		{
			return eSG_ValueType.Bool;
		}
		if (typeof(T) == typeof(uint))
		{
			return eSG_ValueType.Uint32;
		}
		if (typeof(T).IsEnum)
		{
			return eSG_ValueType.Enum;
		}
		if (typeof(T) == typeof(float))
		{
			return eSG_ValueType.Float;
		}
		if (bShowMsg)
		{
			Debug.LogWarning("Unsupported serialization type: " + typeof(T));
		}
		return eSG_ValueType.Unknown;
	}
}
