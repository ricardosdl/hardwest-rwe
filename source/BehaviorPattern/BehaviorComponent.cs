using System;
using UnityEngine;

namespace BehaviorPattern;

[AddComponentMenu("Behavior Component")]
public class BehaviorComponent : MonoBehaviour
{
	public bool m_RunBehaviorOnStart;

	[SerializeField]
	private BehaviorAsset m_BehaviorAsset;

	private BehaviorInstance m_Behavior;

	public Blackboard m_BlackboardAsset;

	[NonSerialized]
	[HideInInspector]
	protected Blackboard m_BBInstance;

	private bool m_Running;

	private Scheduler m_Schedule = new Scheduler();

	public BehaviorAsset BehAsset
	{
		get
		{
			return m_BehaviorAsset;
		}
		set
		{
			m_BehaviorAsset = value;
		}
	}

	public BehaviorInstance Behavior => m_Behavior;

	public Blackboard BB => m_BBInstance;

	public bool Running => m_Running;

	protected virtual void Start()
	{
		if (m_BlackboardAsset != null)
		{
			m_BBInstance = m_BlackboardAsset.Instantiate(this);
			PostBlackboardInit();
		}
		if (m_RunBehaviorOnStart)
		{
			StartLogic();
		}
	}

	protected virtual void PostBlackboardInit()
	{
	}

	public void StartLogic()
	{
		if (m_BehaviorAsset.IObject == null)
		{
			return;
		}
		m_Behavior = m_BehaviorAsset.IObject.GetInstance(this);
		if (Behavior != null)
		{
			BehaviorExec behaviorExec = m_BehaviorAsset.IObject.StartRoot(this);
			if (behaviorExec != null)
			{
				AddTask(behaviorExec);
				m_Running = true;
			}
		}
	}

	public void StopLogic()
	{
		m_Schedule.Clear();
		m_Behavior = null;
		m_Running = false;
	}

	public void AddTask(TaskBase<TaskResult> task)
	{
		m_Schedule.Add(task);
	}

	public void RemoveTask(TaskBase<TaskResult> task)
	{
		int num = m_Schedule.FindIndex((TaskBase<TaskResult> p) => p == task);
		if (num != -1)
		{
			m_Schedule[num] = null;
		}
	}

	protected void RemoveTask(int idx)
	{
		m_Schedule[idx] = null;
	}

	protected virtual void Update()
	{
		if (m_Running)
		{
			UpdateSchedule(Time.deltaTime);
			if (m_Schedule.Count == 0)
			{
				m_Running = false;
			}
		}
	}

	public void UpdateSchedule(float deltaTime)
	{
		Behavior.UpdateSchedule(m_Schedule);
	}
}
