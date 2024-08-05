using System.Collections;
using System.Collections.Generic;

public class CFGPriorityQueue<TPriority, TValue> : IEnumerable, ICollection<KeyValuePair<TPriority, TValue>>, IEnumerable<KeyValuePair<TPriority, TValue>>
{
	private List<KeyValuePair<TPriority, TValue>> m_Heap;

	public int Count => m_Heap.Count;

	public bool IsEmpty => m_Heap.Count == 0;

	public bool IsReadOnly => false;

	public CFGPriorityQueue()
	{
		m_Heap = new List<KeyValuePair<TPriority, TValue>>();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enqueue(TPriority priority, TValue value)
	{
		KeyValuePair<TPriority, TValue> item = new KeyValuePair<TPriority, TValue>(priority, value);
		m_Heap.Add(item);
		MakeHeapFromEnd(m_Heap.Count - 1);
	}

	public TValue Dequeue()
	{
		if (IsEmpty)
		{
			return default(TValue);
		}
		TValue value = m_Heap[0].Value;
		DeleteRoot();
		return value;
	}

	public TValue Peek()
	{
		if (IsEmpty)
		{
			return default(TValue);
		}
		return m_Heap[0].Value;
	}

	public void Add(KeyValuePair<TPriority, TValue> item)
	{
		Enqueue(item.Key, item.Value);
	}

	public void Clear()
	{
		m_Heap.Clear();
	}

	public bool Contains(KeyValuePair<TPriority, TValue> item)
	{
		return m_Heap.Contains(item);
	}

	public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int array_index)
	{
		m_Heap.CopyTo(array, array_index);
	}

	public bool Remove(KeyValuePair<TPriority, TValue> item)
	{
		int num = m_Heap.IndexOf(item);
		if (num < 0)
		{
			return false;
		}
		m_Heap[num] = m_Heap[m_Heap.Count - 1];
		m_Heap.RemoveAt(m_Heap.Count - 1);
		int num2 = MakeHeapFromEnd(num);
		if (num2 == num)
		{
			MakeHeapFromBeginning(num);
		}
		return true;
	}

	public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
	{
		return m_Heap.GetEnumerator();
	}

	private void DeleteRoot()
	{
		if (m_Heap.Count <= 1)
		{
			m_Heap.Clear();
			return;
		}
		m_Heap[0] = m_Heap[m_Heap.Count - 1];
		m_Heap.RemoveAt(m_Heap.Count - 1);
		MakeHeapFromBeginning(0);
	}

	private void MakeHeapFromBeginning(int pos)
	{
		if (pos < 0 || pos >= m_Heap.Count)
		{
			return;
		}
		while (true)
		{
			int num = pos;
			int num2 = LeftChildPos(pos);
			int num3 = RightChildPos(pos);
			if (num2 < m_Heap.Count && Comparer<TPriority>.Default.Compare(m_Heap[num].Key, m_Heap[num2].Key) > 0)
			{
				num = num2;
			}
			if (num3 < m_Heap.Count && Comparer<TPriority>.Default.Compare(m_Heap[num].Key, m_Heap[num3].Key) > 0)
			{
				num = num3;
			}
			if (num != pos)
			{
				SwapElements(num, pos);
				pos = num;
				continue;
			}
			break;
		}
	}

	private int MakeHeapFromEnd(int pos)
	{
		if (pos < 0 || pos >= m_Heap.Count)
		{
			return -1;
		}
		while (pos > 0)
		{
			int num = ParentPos(pos);
			if (Comparer<TPriority>.Default.Compare(m_Heap[num].Key, m_Heap[pos].Key) > 0)
			{
				SwapElements(num, pos);
				pos = num;
				continue;
			}
			break;
		}
		return pos;
	}

	private void SwapElements(int pos1, int pos2)
	{
		KeyValuePair<TPriority, TValue> value = m_Heap[pos1];
		m_Heap[pos1] = m_Heap[pos2];
		m_Heap[pos2] = value;
	}

	private int ParentPos(int pos)
	{
		return (pos - 1) / 2;
	}

	private int LeftChildPos(int pos)
	{
		return 2 * pos + 1;
	}

	private int RightChildPos(int pos)
	{
		return 2 * pos + 2;
	}
}
