using System.Collections.Generic;

public class CFGScenarioButton : CFGButtonExtension
{
	public List<CFGImageExtension> m_Connectors = new List<CFGImageExtension>();

	private bool m_Completed;

	private bool m_Unlocked;

	private int m_CompletedPercent;

	private string m_ScenarioId = string.Empty;

	private string m_ScenarioNameID;

	public bool Completed
	{
		get
		{
			return m_Completed;
		}
		set
		{
			m_Completed = value;
			base.IsInUseMeState = m_Completed;
			foreach (CFGImageExtension connector in m_Connectors)
			{
				connector.IconNumber = (m_Completed ? 1 : 0);
			}
		}
	}

	public bool Unlocked
	{
		get
		{
			return m_Unlocked;
		}
		set
		{
			m_Unlocked = value;
			base.enabled = m_Unlocked;
			SetLabel();
		}
	}

	public int CompletedPercent
	{
		get
		{
			return m_CompletedPercent;
		}
		set
		{
			m_CompletedPercent = value;
			SetLabel();
		}
	}

	public string ScenarioId
	{
		get
		{
			return m_ScenarioId;
		}
		set
		{
			m_ScenarioId = value;
			SetLabel();
		}
	}

	public string ScenarioNameID
	{
		get
		{
			return m_ScenarioNameID;
		}
		set
		{
			m_ScenarioNameID = value;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	public void SetLabel()
	{
		if (m_Label != null)
		{
			if (Unlocked)
			{
				m_Label.text = m_ScenarioId + "\n" + m_CompletedPercent + "%";
			}
			else
			{
				m_Label.text = m_ScenarioId;
			}
		}
	}
}
