using System;
using UnityEngine;

[Serializable]
public class CFGVariableScope
{
	public string name;

	public int level;

	[HideInInspector]
	public bool readOnly;

	public string ID => GetID(name);

	public CFGVariableScope(string name, int level, bool readOnly = true)
	{
		this.name = name;
		this.level = level;
		this.readOnly = readOnly;
	}

	public static string GetID(string scopeName)
	{
		if (!string.IsNullOrEmpty(scopeName))
		{
			return scopeName.ToLower();
		}
		return string.Empty;
	}
}
