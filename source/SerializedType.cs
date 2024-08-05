using System;
using UnityEngine;

[Serializable]
public class SerializedType
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private string m_AssemblyQualifiedName;

	[SerializeField]
	private string m_AssemblyName;

	private Type m_SystemType;

	public string Name => m_Name;

	public string AssemblyQualifiedName => m_AssemblyQualifiedName;

	public string AssemblyName => m_AssemblyName;

	public Type SystemType
	{
		get
		{
			if (m_SystemType == null)
			{
				GetSystemType();
			}
			return m_SystemType;
		}
	}

	public SerializedType(Type systemType)
	{
		m_SystemType = systemType;
		m_Name = systemType.Name;
		m_AssemblyQualifiedName = systemType.AssemblyQualifiedName;
		m_AssemblyName = systemType.Assembly.FullName;
	}

	private void GetSystemType()
	{
		m_SystemType = Type.GetType(m_AssemblyQualifiedName);
	}

	public static implicit operator SerializedType(Type t)
	{
		return new SerializedType(t);
	}

	public static implicit operator Type(SerializedType t)
	{
		return t.SystemType;
	}
}
