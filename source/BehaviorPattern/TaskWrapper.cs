using System.Collections.Generic;
using UnityEngine;

namespace BehaviorPattern;

public abstract class TaskWrapper<T> : TaskBase<T> where T : struct
{
	protected bool m_Reset;

	protected bool bReset;

	protected IEnumerator<T> m_Iterator;

	public bool Continue => !bReset;

	public abstract bool Active { get; }

	public TaskWrapper(IEnumerator<T> iterator)
	{
		m_Iterator = iterator;
	}

	public override bool Update()
	{
		bReset = m_Reset;
		m_Reset = false;
		bool flag = m_Iterator.MoveNext();
		bReset = false;
		if (flag)
		{
			m_Current = m_Iterator.Current;
		}
		else
		{
			Debug.LogWarning("standard iterator behavior not supported");
		}
		return flag;
	}

	public override void Abort()
	{
		m_Reset = true;
	}
}
