using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFGDictionary<TKey, TValue> : ScriptableObject, IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>
{
	[SerializeField]
	private List<TKey> m_Keys = new List<TKey>();

	[SerializeField]
	private List<TValue> m_Values = new List<TValue>();

	public ICollection<TKey> Keys => m_Keys.ToArray();

	public ICollection<TValue> Values => m_Values.ToArray();

	public TValue this[TKey Key]
	{
		get
		{
			if (TryGetIndexOfKey(Key, out var Idx))
			{
				return m_Values[Idx];
			}
			return default(TValue);
		}
		set
		{
			if (TryGetIndexOfKey(Key, out var Idx))
			{
				m_Values[Idx] = value;
			}
		}
	}

	public int Count => m_Keys.Count;

	public bool IsReadOnly => false;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		for (int i = 0; i < m_Keys.Count; i++)
		{
			dictionary.Add(m_Keys[i], m_Values[i]);
		}
		return dictionary.GetEnumerator();
	}

	public void Add(TKey Key, TValue Value)
	{
		if (Key != null && !m_Keys.Contains(Key))
		{
			m_Keys.Add(Key);
			m_Values.Add(Value);
		}
	}

	public bool ContainsKey(TKey Key)
	{
		return m_Keys.Contains(Key);
	}

	public bool Remove(TKey Key)
	{
		int num = m_Keys.FindIndex((TKey k) => k.Equals(Key));
		if (num != -1)
		{
			m_Values.RemoveAt(num);
			m_Keys.RemoveAt(num);
			return true;
		}
		return false;
	}

	public bool TryGetValue(TKey Key, out TValue Value)
	{
		if (TryGetIndexOfKey(Key, out var Idx))
		{
			Value = m_Values[Idx];
			return true;
		}
		Value = default(TValue);
		return false;
	}

	protected bool TryGetIndexOfKey(TKey Key, out int Idx)
	{
		Idx = m_Keys.FindIndex((TKey k) => k.Equals(Key));
		return Idx != -1;
	}

	public void Add(KeyValuePair<TKey, TValue> Pair)
	{
		if (!Pair.Equals(null) && Pair.Key != null && !m_Keys.Contains(Pair.Key))
		{
			m_Keys.Add(Pair.Key);
			m_Values.Add(Pair.Value);
		}
	}

	public void Clear()
	{
		m_Keys.Clear();
		m_Values.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> Pair)
	{
		if (TryGetIndexOfKey(Pair.Key, out var Idx) && m_Values[Idx].Equals(Pair.Value))
		{
			return true;
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] Arr, int Idx)
	{
		throw new NotImplementedException();
	}

	public bool Remove(KeyValuePair<TKey, TValue> Pair)
	{
		if (TryGetIndexOfKey(Pair.Key, out var Idx) && m_Values[Idx].Equals(Pair.Value))
		{
			m_Keys.RemoveAt(Idx);
			m_Values.RemoveAt(Idx);
			return true;
		}
		return false;
	}

	public void RemoveNulls()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_Keys.Count; i++)
		{
			if (m_Keys[i].Equals(null))
			{
				list.Add(i);
			}
		}
		list.Reverse();
		foreach (int item in list)
		{
			m_Keys.RemoveAt(item);
			m_Values.RemoveAt(item);
		}
	}
}
