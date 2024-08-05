using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class SerializedMethod
{
	[SerializeField]
	private MethodInfo m_SystemMethod;

	[SerializeField]
	private SerializedType m_DeclaringType;

	[SerializeField]
	private string m_MethodName;

	[SerializeField]
	private List<SerializedType> m_MethodTypes = new List<SerializedType>();

	public MethodAttributes MethodAttributes
	{
		get
		{
			if (SystemMethod != null)
			{
				return SystemMethod.Attributes;
			}
			return MethodAttributes.PrivateScope;
		}
	}

	public MethodInfo SystemMethod
	{
		get
		{
			if (m_SystemMethod == null && DeclaringType != null && DeclaringType.SystemType != null)
			{
				List<Type> list = new List<Type>();
				foreach (SerializedType methodType in m_MethodTypes)
				{
					list.Add(methodType);
				}
				m_SystemMethod = DeclaringType.SystemType.GetMethod(m_MethodName, list.ToArray());
			}
			return m_SystemMethod;
		}
	}

	public SerializedType DeclaringType => m_DeclaringType;

	public string RefName => m_MethodName;

	public string MethodSignature
	{
		get
		{
			if (SystemMethod != null)
			{
				return SystemMethod.ToString();
			}
			return "UNKNOWN";
		}
	}

	public SerializedMethod(MethodInfo inMethod)
	{
		m_DeclaringType = new SerializedType(inMethod.DeclaringType);
		m_SystemMethod = inMethod;
		m_MethodName = inMethod.Name;
		ParameterInfo[] parameters = inMethod.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			m_MethodTypes.Add(parameterInfo.ParameterType);
		}
	}

	public override string ToString()
	{
		if (SystemMethod != null)
		{
			return SystemMethod.ToString();
		}
		return RefName;
	}

	public static implicit operator SerializedMethod(MethodInfo m)
	{
		return new SerializedMethod(m);
	}

	public static implicit operator MethodInfo(SerializedMethod m)
	{
		return m.SystemMethod;
	}
}
