using System.Collections.Generic;
using System.Runtime.InteropServices;

public class BTInstance
{
	public List<object> m_Data = new List<object>();

	public int InstanceMemory<T>([Optional] T data)
	{
		m_Data.Add(data);
		return m_Data.Count - 1;
	}

	public void SetMemory<T>(int idx, T data)
	{
		m_Data[idx] = data;
	}

	public T GetMemory<T>(int idx)
	{
		return (T)m_Data[idx];
	}
}
