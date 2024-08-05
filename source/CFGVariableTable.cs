using System.Collections.Generic;
using UnityEngine;

public class CFGVariableTable
{
	private Dictionary<string, int> m_TB_Int = new Dictionary<string, int>();

	private Dictionary<string, string> m_TB_String = new Dictionary<string, string>();

	public Dictionary<string, int> Table_Int => m_TB_Int;

	public Dictionary<string, string> Table_String => m_TB_String;

	public void Clear()
	{
		m_TB_Int.Clear();
		m_TB_String.Clear();
	}

	public void Set_Int(string Name, int Value)
	{
		if (Name == null || Name == string.Empty)
		{
			Debug.LogWarning("Name is empty!");
			return;
		}
		if (m_TB_Int.ContainsKey(Name))
		{
			m_TB_Int.Remove(Name);
		}
		m_TB_Int.Add(Name, Value);
	}

	public int Get_Int(string Name)
	{
		int value = 0;
		if (m_TB_Int.TryGetValue(Name, out value))
		{
			return value;
		}
		return 0;
	}

	public void Set_String(string Name, string Value)
	{
		if (Name == null || Name == string.Empty)
		{
			Debug.LogWarning("Name is empty!");
			return;
		}
		if (m_TB_String.ContainsKey(Name))
		{
			m_TB_String.Remove(Name);
		}
		if (Value != null)
		{
			m_TB_String.Add(Name, Value);
		}
	}

	public string Get_String(string Name)
	{
		string value = string.Empty;
		if (m_TB_String.TryGetValue(Name, out value))
		{
			return value;
		}
		return string.Empty;
	}

	public void MakeCloneOf(CFGVariableTable Other)
	{
		Clear();
		foreach (KeyValuePair<string, int> item in Other.m_TB_Int)
		{
			m_TB_Int.Add(item.Key, item.Value);
		}
		foreach (KeyValuePair<string, string> item2 in Other.m_TB_String)
		{
			m_TB_String.Add(item2.Key, item2.Value);
		}
	}
}
