using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core;

public static class TypeExtensions
{
	public static bool IsA(this Type type, Type other)
	{
		if (type == null || other == null)
		{
			return false;
		}
		if (other.IsInterface && !type.IsInterface)
		{
			Type type2 = null;
			Type[] interfaces = type.GetInterfaces();
			if (interfaces != null)
			{
				Type[] array = interfaces;
				foreach (Type type3 in array)
				{
					if (type3.IsA(other))
					{
						type2 = type3;
						break;
					}
				}
			}
			return type2 != null;
		}
		if (type == other || type.IsSubclassOf(other))
		{
			return true;
		}
		if (type.IsGenericType && (type.GetGenericTypeDefinition() == other || type.GetGenericTypeDefinition().IsSubclassOf(other)))
		{
			return true;
		}
		return (type.BaseType != null && type.BaseType.IsGenericType) ? (type.BaseType.GetGenericTypeDefinition() == other || type.BaseType.GetGenericTypeDefinition().IsSubclassOf(other)) : (type.BaseType == other || (type.BaseType != null && type.BaseType.IsSubclassOf(other)));
	}

	public static IEnumerable<Type> DerivingTypes(this Type fromType)
	{
		if (fromType == null)
		{
			return new List<Type>().AsEnumerable();
		}
		return from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from type in assembly.GetTypes()
			where (type.BaseType != null && type.BaseType.IsGenericType) ? (type.BaseType.GetGenericTypeDefinition() == fromType || type.BaseType.GetGenericTypeDefinition().IsSubclassOf(fromType)) : (type.BaseType == fromType || (type.BaseType != null && type.BaseType.IsSubclassOf(fromType)))
			select type;
	}

	public static T GetAttribute<T>(this Type fromType, bool inherit) where T : Attribute
	{
		if (fromType != null)
		{
			object[] customAttributes = fromType.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes.Count() > 0)
			{
				return customAttributes[0] as T;
			}
		}
		return (T)null;
	}

	public static T GetAttribute<T>(this MemberInfo fromMember, bool inherit) where T : Attribute
	{
		if (fromMember != null)
		{
			object[] customAttributes = fromMember.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes.Count() > 0)
			{
				return customAttributes[0] as T;
			}
		}
		return (T)null;
	}

	public static FieldInfo[] GetInstanceFields(this Type type)
	{
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		IEnumerable<FieldInfo> source = from field in type.GetFields(bindingAttr)
			orderby field.MetadataToken
			select field;
		return source.ToArray();
	}

	public static FieldInfo GetInstanceField(this Type type, string name)
	{
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		return type.GetField(name, bindingAttr);
	}

	public static MethodInfo[] GetInstanceMethods(this Type type)
	{
		return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public static object GetDefault(this Type type)
	{
		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}
		return null;
	}

	public static Type GetSafeType(this object target)
	{
		if (target == null)
		{
			return typeof(void);
		}
		return target.GetType();
	}
}
