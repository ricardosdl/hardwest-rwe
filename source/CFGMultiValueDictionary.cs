using System.Collections.Generic;

public class CFGMultiValueDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
{
	public void Add(TKey key, TValue value)
	{
		HashSet<TValue> value2 = null;
		if (!TryGetValue(key, out value2))
		{
			value2 = new HashSet<TValue>();
			Add(key, value2);
		}
		value2.Add(value);
	}

	public void Remove(TKey key, TValue value)
	{
		HashSet<TValue> value2 = null;
		if (TryGetValue(key, out value2))
		{
			value2.Remove(value);
			if (value2.Count <= 0)
			{
				Remove(key);
			}
		}
	}

	public bool ContainsValue(TKey key, TValue value)
	{
		HashSet<TValue> value2 = null;
		if (TryGetValue(key, out value2))
		{
			return value2.Contains(value);
		}
		return false;
	}
}
