public class CFGDef_Tactical
{
	[CFGTableField(ColumnName = "TacticalID", DefaultValue = "")]
	public string TacticalID = string.Empty;

	[CFGTableField(ColumnName = "ScenarioID", DefaultValue = "")]
	public string ScenarioID = string.Empty;

	[CFGTableField(ColumnName = "LoadingScreenID", DefaultValue = -1)]
	public int LoadingScreenID = -1;

	[CFGTableField(ColumnName = "FlowCode", DefaultValue = 0)]
	public int FlowCode;

	[CFGTableField(ColumnName = "Scene", DefaultValue = "")]
	public string SceneName = string.Empty;

	[CFGTableField(ColumnName = "StartInNightmare", DefaultValue = ETacticalNightmareMode.Auto)]
	public ETacticalNightmareMode m_InitNightmareMode;
}
