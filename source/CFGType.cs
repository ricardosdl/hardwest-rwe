using System;
using UnityEngine;

[Serializable]
public class CFGType
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private string m_AssemblyName;

	[SerializeField]
	private string m_AssemblyQualifiedName;

	private Type m_SystemType;

	public string Name => m_Name;

	public string AssemblyQualifiedName => m_AssemblyQualifiedName;

	public string AssemblyName => m_AssemblyName;

	public Type SystemType
	{
		get
		{
			if (string.IsNullOrEmpty(m_AssemblyQualifiedName))
			{
				return null;
			}
			if (m_SystemType == null || m_SystemType.AssemblyQualifiedName != m_AssemblyQualifiedName)
			{
				m_SystemType = Type.GetType(m_AssemblyQualifiedName);
			}
			return m_SystemType;
		}
	}

	public CFGType(Type systemType)
	{
		m_SystemType = systemType;
		m_Name = systemType.Name;
		m_AssemblyQualifiedName = systemType.AssemblyQualifiedName;
		m_AssemblyName = systemType.Assembly.FullName;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is CFGType cFGType))
		{
			return false;
		}
		return cFGType.SystemType == SystemType;
	}

	public override int GetHashCode()
	{
		if (string.IsNullOrEmpty(m_AssemblyQualifiedName))
		{
			return base.GetHashCode();
		}
		return SystemType.GetHashCode();
	}
}
