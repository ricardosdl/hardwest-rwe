using System.Collections.Generic;

public class CFGDef_Scenario
{
	[CFGTableField(ColumnName = "ScenarioID", DefaultValue = "")]
	public string ScenarioID = string.Empty;

	[CFGTableField(ColumnName = "CampaignID", DefaultValue = "")]
	public string CampaignID = string.Empty;

	[CFGTableField(ColumnName = "PrerequisiteScenarioID", DefaultValue = null)]
	public List<string> PrerequisiteScenarioID = new List<string>();

	[CFGTableField(ColumnName = "ImageID", DefaultValue = 0)]
	public int ImageID;

	[CFGTableField(ColumnName = "Index", DefaultValue = 0)]
	public int Index;

	[CFGTableField(ColumnName = "StartTacticalID", DefaultValue = "")]
	public string StartTacticalID = string.Empty;

	[CFGTableField(ColumnName = "StrategicScene", DefaultValue = "")]
	public string StrategicScene = string.Empty;
}
