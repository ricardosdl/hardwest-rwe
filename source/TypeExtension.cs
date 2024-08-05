using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class TypeExtension
{
	public static List<Type> GetAllDerivedTypes(this Type type)
	{
		return Assembly.GetAssembly(type).GetAllDerivedTypes(type);
	}

	public static List<Type> GetAllDerivedTypes(this Assembly assembly, Type type)
	{
		return (from t in assembly.GetTypes()
			where t != type && type.IsAssignableFrom(t)
			select t).ToList();
	}

	public static IEnumerable<Type> TypeHierarchy(this Type type)
	{
		do
		{
			yield return type;
			type = type.BaseType;
		}
		while (type != null);
	}

	public static bool IsClassOrSubclassOf(this Type type, Type c)
	{
		if (type == null || c == null)
		{
			Debug.LogError("Given type was null");
			return false;
		}
		return type.IsSubclassOf(c) || type == c;
	}

	public static bool IsObsolete(this Type type)
	{
		return type.IsDefined(typeof(ObsoleteAttribute), inherit: true);
	}

	public static string GetObsoleteMessage(this Type type)
	{
		if (Attribute.GetCustomAttribute(type, typeof(ObsoleteAttribute)) is ObsoleteAttribute obsoleteAttribute)
		{
			return obsoleteAttribute.Message;
		}
		return string.Empty;
	}

	public static bool IsGenericList(this Type type)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			return true;
		}
		return false;
	}

	public static bool IsGeneric(this Type type, Type t)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == t)
		{
			return true;
		}
		return false;
	}

	public static string PrettyName(this Type t)
	{
		if (t == typeof(bool))
		{
			return "bool";
		}
		if (t == typeof(int))
		{
			return "int";
		}
		if (t == typeof(float))
		{
			return "float";
		}
		if (t == typeof(string))
		{
			return "string";
		}
		if (t == typeof(UnityEngine.Object))
		{
			return "Object";
		}
		if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
		{
			return t.GetGenericArguments()[0].PrettyName() + "List";
		}
		return t.ToString();
	}

	public static Type GetTypeFromSimpleName(string typeName)
	{
		if (typeName == null)
		{
			throw new ArgumentNullException("typeName");
		}
		bool flag = false;
		bool flag2 = false;
		if (typeName.IndexOf("[]") != -1)
		{
			flag = true;
			typeName = typeName.Remove(typeName.IndexOf("[]"), 2);
		}
		if (typeName.IndexOf("?") != -1)
		{
			flag2 = true;
			typeName = typeName.Remove(typeName.IndexOf("?"), 1);
		}
		typeName = typeName.ToLower();
		string text = null;
		switch (typeName)
		{
		case "bool":
		case "boolean":
			text = "System.Boolean";
			break;
		case "byte":
			text = "System.Byte";
			break;
		case "char":
			text = "System.Char";
			break;
		case "datetime":
			text = "System.DateTime";
			break;
		case "datetimeoffset":
			text = "System.DateTimeOffset";
			break;
		case "decimal":
			text = "System.Decimal";
			break;
		case "double":
			text = "System.Double";
			break;
		case "float":
			text = "System.Single";
			break;
		case "int16":
		case "short":
			text = "System.Int16";
			break;
		case "int32":
		case "int":
			text = "System.Int32";
			break;
		case "int64":
		case "long":
			text = "System.Int64";
			break;
		case "object":
			text = "System.Object";
			break;
		case "sbyte":
			text = "System.SByte";
			break;
		case "string":
			text = "System.String";
			break;
		case "timespan":
			text = "System.TimeSpan";
			break;
		case "uint16":
		case "ushort":
			text = "System.UInt16";
			break;
		case "uint32":
		case "uint":
			text = "System.UInt32";
			break;
		case "uint64":
		case "ulong":
			text = "System.UInt64";
			break;
		}
		if (text != null)
		{
			if (flag)
			{
				text += "[]";
			}
			if (flag2)
			{
				text = "System.Nullable`1[" + text + "]";
			}
		}
		else
		{
			text = typeName;
		}
		return Type.GetType(text);
	}

	public static bool IsObsolete(this MethodInfo method)
	{
		return method.IsDefined(typeof(ObsoleteAttribute), inherit: true);
	}

	public static string GetObsoleteMessage(this MethodInfo method)
	{
		if (Attribute.GetCustomAttribute(method, typeof(ObsoleteAttribute)) is ObsoleteAttribute obsoleteAttribute)
		{
			return obsoleteAttribute.Message;
		}
		return string.Empty;
	}

	public static bool IsObsolete(this FieldInfo field)
	{
		return field.IsDefined(typeof(ObsoleteAttribute), inherit: true);
	}
}
