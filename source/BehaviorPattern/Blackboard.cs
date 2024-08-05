using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern;

public class Blackboard : ScriptableObject
{
	[NonSerialized]
	public BehaviorComponent m_Owner;

	[NonSerialized]
	public Blackboard m_Asset;

	[SerializeField]
	public List<BBProperty> m_Entries = new List<BBProperty>();

	private Dictionary<string, BBProperty> m_EntryMap = new Dictionary<string, BBProperty>();

	protected void OnEnable()
	{
	}

	public BBProperty<T> GetProperty<T>(string withName)
	{
		if (m_EntryMap.ContainsKey(withName) && m_EntryMap[withName] is BBProperty<T>)
		{
			return (BBProperty<T>)m_EntryMap[withName];
		}
		return null;
	}

	public BBProperty GetProperty(string withName)
	{
		int num = m_Entries.FindIndex((BBProperty p) => p.Name == withName);
		if (num != -1)
		{
			return m_Entries[num];
		}
		return null;
	}

	public bool SetProperty<T>(string withName, T value)
	{
		BBProperty<T> property = GetProperty<T>(withName);
		if (property != null)
		{
			property.Value = value;
			return true;
		}
		return false;
	}

	public Blackboard Instantiate(BehaviorComponent newOnwer)
	{
		Blackboard blackboard = UnityEngine.Object.Instantiate(this);
		if (blackboard != null)
		{
			for (int i = 0; i < m_Entries.Count; i++)
			{
				BBProperty bBProperty = UnityEngine.Object.Instantiate(m_Entries[i]);
				bBProperty.Name = m_Entries[i].Name;
				bBProperty.m_Owner = blackboard;
				blackboard.m_Entries[i] = bBProperty;
				blackboard.m_EntryMap.Add(bBProperty.Name, bBProperty);
			}
			blackboard.m_Asset = this;
			blackboard.name = base.name + "(instance)";
			blackboard.m_Owner = newOnwer;
			return blackboard;
		}
		Debug.LogWarning("Couldn't create blackboard instance !");
		return null;
	}
}
