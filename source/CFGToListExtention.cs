using System.Collections;
using System.Collections.Generic;

public static class CFGToListExtention
{
	public static List<T> ToList<T>(this ArrayList array_list)
	{
		List<T> list = new List<T>(array_list.Count);
		foreach (T item in array_list)
		{
			list.Add(item);
		}
		return list;
	}

	public static List<T> ToList<T>(this HashSet<T> hs)
	{
		List<T> list = new List<T>(hs.Count);
		foreach (T h in hs)
		{
			list.Add(h);
		}
		return list;
	}

	public static List<T> ToList<T>(this LinkedList<T> ll)
	{
		List<T> list = new List<T>(ll.Count);
		foreach (T item in ll)
		{
			list.Add(item);
		}
		return list;
	}
}
