using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern;

public class BehaviorInstance
{
	public List<BehaviorExec> m_Data = new List<BehaviorExec>();

	public int InstanceMemory()
	{
		m_Data.Add(null);
		return m_Data.Count - 1;
	}

	public int InstanceMemory(BehaviorExec data)
	{
		m_Data.Add(data);
		return m_Data.Count - 1;
	}

	public void SetMemory(int idx, BehaviorExec data)
	{
		m_Data[idx] = data;
	}

	public BehaviorExec GetMemory(int idx)
	{
		return m_Data[idx];
	}

	public virtual void StartBehave()
	{
		Debug.LogWarning("this function should be implemented");
	}

	public virtual void UpdateSchedule(Scheduler schedule)
	{
		for (int num = schedule.Count - 1; num >= 0; num--)
		{
			if (schedule[num] is BehaviorExec behaviorExec && behaviorExec.Update())
			{
				TaskResult current = behaviorExec.Current;
				if (current != TaskResult.Running)
				{
					schedule[num] = null;
				}
			}
		}
		for (int num2 = schedule.Count - 1; num2 >= 0; num2--)
		{
			if (schedule[num2] == null)
			{
				schedule.RemoveAt(num2);
			}
		}
	}
}
