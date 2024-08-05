using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
	public delegate void BTEnd(BehaviorTree agent);

	public BTTemplate m_Template;

	public BTInstance m_Data;

	protected EBTStatus m_Status;

	private List<IBTLatent> m_LatentTasks = new List<IBTLatent>();

	public List<ExecutionTimeObserver> m_ExecTimeObservers;

	private BTEnd OnBTEnd;

	public EBTStatus Status => m_Status;

	public bool Running => m_Status == EBTStatus.Running;

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		ExecuteBT();
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnValidate()
	{
	}

	protected virtual void Update()
	{
		if (m_Status != EBTStatus.Running)
		{
			if (m_Status == EBTStatus.Pending)
			{
				m_Status = EBTStatus.Running;
				m_Status = RunTree();
			}
		}
		else
		{
			Tick(Time.deltaTime);
		}
	}

	protected void Tick(float DeltaTime)
	{
		for (int i = 0; i < m_LatentTasks.Count; i++)
		{
			if (m_LatentTasks[i] == null)
			{
				m_LatentTasks.RemoveAt(i--);
			}
		}
		List<IBTLatent> list = new List<IBTLatent>(m_LatentTasks);
		if (list.Count <= 0)
		{
			return;
		}
		foreach (IBTLatent item in list)
		{
			item.TickLatent(this);
		}
	}

	private bool SetTemplate(BTTemplate template)
	{
		m_Template = template;
		if (m_Template != null)
		{
			return true;
		}
		return false;
	}

	public void RegisterLatent(IBTLatent task)
	{
		m_LatentTasks.Add(task);
	}

	public void UnRegigterLatent(IBTLatent task)
	{
		int num = m_LatentTasks.FindIndex((IBTLatent p) => p == task);
		if (num > -1)
		{
			m_LatentTasks[num] = null;
		}
	}

	private EBTStatus RunTree()
	{
		if (m_Template == null || m_Template.m_Root == null)
		{
			return EBTStatus.Blank;
		}
		if (m_Status != EBTStatus.Pending && m_Status != EBTStatus.Suspended && m_Status != EBTStatus.Stopped)
		{
			EBTResult decorationResult;
			EBTResult eBTResult = m_Template.m_Root.Execute(this, out decorationResult);
			if (decorationResult == EBTResult.Execution)
			{
				return EBTStatus.Pending;
			}
			if (eBTResult == EBTResult.Execution)
			{
				return EBTStatus.Running;
			}
			ExecutionFinished(eBTResult);
		}
		return m_Status;
	}

	public EBTStatus ExecuteBT(BTTemplate tree = null, bool bLazy = false, BTEnd callback = null)
	{
		if (m_Template != null)
		{
			StopTree();
		}
		if (tree != null && !SetTemplate(tree))
		{
			return EBTStatus.Error;
		}
		m_Status = EBTStatus.Running;
		OnBTEnd = callback;
		if (bLazy)
		{
			m_Status = EBTStatus.Pending;
		}
		m_Status = RunTree();
		return m_Status;
	}

	public void StopTree()
	{
		if (Status != EBTStatus.Stopped && Status != 0)
		{
			m_Status = EBTStatus.Stopped;
			m_LatentTasks.Clear();
			if (OnBTEnd != null)
			{
				OnBTEnd(this);
				OnBTEnd = null;
			}
		}
	}

	public void ExecutionFinished(EBTResult result)
	{
		switch (result)
		{
		case EBTResult.Execution:
			m_Status = EBTStatus.Pending;
			return;
		case EBTResult.Abort:
			m_Status = EBTStatus.Stopped;
			break;
		default:
			m_Status = EBTStatus.Finished;
			break;
		}
		StopTree();
	}
}
