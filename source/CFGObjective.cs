using System.Collections.Generic;
using UnityEngine;

public class CFGObjective
{
	private bool m_IsDirty;

	private bool m_AlreadyReaded;

	private bool m_IsNew = true;

	private CFGObjectiveData m_ObjectiveData;

	private EObjectiveState m_State;

	private string m_Progress = string.Empty;

	private bool m_IsProgressing;

	private float m_StartTime = -1f;

	private float m_EndTime = -1f;

	private bool m_IsSelected;

	private List<CFGObjectiveLocation> m_Locations = new List<CFGObjectiveLocation>();

	public List<CFGObjectiveLocation> Locations => m_Locations;

	public bool IsNew
	{
		get
		{
			return m_IsNew;
		}
		set
		{
			m_IsNew = value;
		}
	}

	public EObjectiveState State
	{
		get
		{
			return m_State;
		}
		private set
		{
			m_State = value;
		}
	}

	public string Progress
	{
		get
		{
			return m_Progress;
		}
		set
		{
			m_Progress = value;
		}
	}

	public float StartTime
	{
		get
		{
			return m_StartTime;
		}
		private set
		{
			m_StartTime = value;
		}
	}

	public float EndTime
	{
		get
		{
			return m_EndTime;
		}
		private set
		{
			m_EndTime = value;
		}
	}

	public bool IsDirty
	{
		get
		{
			return m_IsDirty;
		}
		set
		{
			m_IsDirty = value;
		}
	}

	public bool AlreadyReaded
	{
		get
		{
			return m_AlreadyReaded;
		}
		set
		{
			m_AlreadyReaded = value;
		}
	}

	public bool IsProgressing
	{
		get
		{
			return State == EObjectiveState.Started && m_IsProgressing;
		}
		set
		{
			if (State == EObjectiveState.Started)
			{
				m_IsProgressing = value;
			}
		}
	}

	public bool IsSelected
	{
		get
		{
			return m_IsSelected;
		}
		set
		{
			m_IsSelected = value;
		}
	}

	public string Id => m_ObjectiveData.m_Id;

	public EObjectiveImportance Importance => m_ObjectiveData.m_Importance;

	public EObjectiveType Type => m_ObjectiveData.m_Type;

	public string DescShort => m_ObjectiveData.m_DescShort;

	public string DescLong => m_ObjectiveData.m_DescLong;

	public EObjectiveScene DesignatedSceneType => m_ObjectiveData.m_ObjectiveScene;

	public CFGObjective(CFGObjectiveData objective_data)
	{
		m_ObjectiveData = objective_data;
	}

	public CFGObjective(CFGObjectiveData objective_data, CFG_SG_Node SGNode)
	{
		m_ObjectiveData = objective_data;
		State = SGNode.Attrib_Get("State", EObjectiveState.Unstarted);
		Progress = SGNode.Attrib_Get("Progress", string.Empty);
		StartTime = SGNode.Attrib_Get("Start", 0f);
		EndTime = SGNode.Attrib_Get("End", 0f);
		m_IsNew = SGNode.Attrib_Get("New", DefVal: false);
		m_AlreadyReaded = SGNode.Attrib_Get("Readed", DefVal: false);
	}

	public bool GetProgressingVal()
	{
		return m_IsProgressing;
	}

	public string GetText()
	{
		return DescShort + " " + Progress;
	}

	public void Start()
	{
		State = EObjectiveState.Started;
		StartTime = Time.time;
		IsDirty = true;
		AlreadyReaded = false;
	}

	public void Complete()
	{
		State = EObjectiveState.Completed;
		EndTime = Time.time;
		IsDirty = true;
		AlreadyReaded = false;
	}

	public void Fail()
	{
		State = EObjectiveState.Failed;
		EndTime = Time.time;
		IsDirty = true;
		AlreadyReaded = false;
	}
}
