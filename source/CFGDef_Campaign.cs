using System.Collections.Generic;

public class CFGDef_Campaign
{
	[CFGTableField(ColumnName = "CampaignID", DefaultValue = "")]
	public string CampaignID = string.Empty;

	[CFGTableField(ColumnName = "ImageID", DefaultValue = 0)]
	public int ImageID;

	[CFGTableField(ColumnName = "Index", DefaultValue = 0)]
	public int Index;

	[CFGTableField(ColumnName = "Dlc", DefaultValue = EDLC.None)]
	public EDLC Dlc;

	public List<CFGDef_Scenario> ScenarioList = new List<CFGDef_Scenario>();

	public bool CheckIfValid()
	{
		if (ScenarioList == null || ScenarioList.Count == 0)
		{
			return false;
		}
		return true;
	}

	public CFGDef_Scenario GetScenario(string ScenarioID)
	{
		foreach (CFGDef_Scenario scenario in ScenarioList)
		{
			if (scenario != null && string.Compare(scenario.ScenarioID, ScenarioID, ignoreCase: true) == 0)
			{
				return scenario;
			}
		}
		return null;
	}

	public CFGDef_Scenario GetScenarioByStrategicName(string Strategic)
	{
		foreach (CFGDef_Scenario scenario in ScenarioList)
		{
			if (scenario != null && string.Compare(scenario.StrategicScene, Strategic, ignoreCase: true) == 0)
			{
				return scenario;
			}
		}
		return null;
	}

	public CFGDef_Scenario GetScenarioByScenario(string ScenarioID)
	{
		foreach (CFGDef_Scenario scenario in ScenarioList)
		{
			if (scenario != null && string.Compare(scenario.ScenarioID, ScenarioID, ignoreCase: true) == 0)
			{
				return scenario;
			}
		}
		return null;
	}

	public CFGDef_Scenario GetScenario(int _Index)
	{
		foreach (CFGDef_Scenario scenario in ScenarioList)
		{
			if (scenario != null && _Index == scenario.Index)
			{
				return scenario;
			}
		}
		return null;
	}
}
