using UnityEngine;

public class CFGDef_Character
{
	[CFGTableField(ColumnName = "NameId", DefaultValue = "")]
	public string NameID = string.Empty;

	[CFGTableField(ColumnName = "Prefab", DefaultValue = "")]
	public string PrefabSubPath = string.Empty;

	[CFGTableField(ColumnName = "VisPreset", DefaultValue = -1)]
	public int PresetIdx = -1;

	[CFGTableField(ColumnName = "ImageID", DefaultValue = -1)]
	public int ImageID = -1;

	[CFGTableField(ColumnName = "MaxAP", DefaultValue = -1)]
	public int MaxAP = -1;

	[CFGTableField(ColumnName = "InjuryResist", DefaultValue = 0)]
	public int InjuryResistance;

	[CFGTableField(ColumnName = "reaction_range", DefaultValue = 5)]
	public int ReactionRange = 5;

	[CFGTableField(ColumnName = "MaxHealth", DefaultValue = -1)]
	public int MaxHealth = -1;

	[CFGTableField(ColumnName = "MaxHPEasy", DefaultValue = -1)]
	public int MaxHPEasy = -1;

	[CFGTableField(ColumnName = "MaxHPHard", DefaultValue = -1)]
	public int MaxHPHard = -1;

	[CFGTableField(ColumnName = "DmgModEasy", DefaultValue = 0)]
	public int DmgModEasy;

	[CFGTableField(ColumnName = "DmgModHard", DefaultValue = 0)]
	public int DmgModHard;

	[CFGTableField(ColumnName = "Aim", DefaultValue = -1)]
	public int Aim = -1;

	[CFGTableField(ColumnName = "Defense", DefaultValue = -1)]
	public int Defense = -1;

	[CFGTableField(ColumnName = "Movement", DefaultValue = -1)]
	public int Movement = -1;

	[CFGTableField(ColumnName = "Sight", DefaultValue = -1)]
	public int Sight = -1;

	[CFGTableField(ColumnName = "MaxLuck", DefaultValue = -1)]
	public int MaxLuck = -1;

	[CFGTableField(ColumnName = "LR_Turn", DefaultValue = -1)]
	public int LuckR_Turn = -1;

	[CFGTableField(ColumnName = "LR_Hit", DefaultValue = -1)]
	public int LuckR_Hit = -1;

	[CFGTableField(ColumnName = "Weapon1", DefaultValue = "")]
	public string Weapon1 = string.Empty;

	[CFGTableField(ColumnName = "Weapon2", DefaultValue = "")]
	public string Weapon2 = string.Empty;

	[CFGTableField(ColumnName = "Talisman", DefaultValue = "")]
	public string Talisman = string.Empty;

	[CFGTableField(ColumnName = "Item1", DefaultValue = "")]
	public string Item1 = string.Empty;

	[CFGTableField(ColumnName = "Item2", DefaultValue = "")]
	public string Item2 = string.Empty;

	[CFGTableField(ColumnName = "Ability1", DefaultValue = ETurnAction.None)]
	public ETurnAction Ability1 = ETurnAction.None;

	[CFGTableField(ColumnName = "Ability2", DefaultValue = ETurnAction.None)]
	public ETurnAction Ability2 = ETurnAction.None;

	[CFGTableField(ColumnName = "Ability3", DefaultValue = ETurnAction.None)]
	public ETurnAction Ability3 = ETurnAction.None;

	[CFGTableField(ColumnName = "Ability4", DefaultValue = ETurnAction.None)]
	public ETurnAction Ability4 = ETurnAction.None;

	[CFGTableField(ColumnName = "Ability5", DefaultValue = ETurnAction.None)]
	public ETurnAction Ability5 = ETurnAction.None;

	[CFGTableField(ColumnName = "Buff1", DefaultValue = "")]
	public string Buff1 = string.Empty;

	[CFGTableField(ColumnName = "Buff2", DefaultValue = "")]
	public string Buff2 = string.Empty;

	[CFGTableField(ColumnName = "Buff3", DefaultValue = "")]
	public string Buff3 = string.Empty;

	[CFGTableField(ColumnName = "canpanic", DefaultValue = true)]
	public bool CanPanic = true;

	[CFGTableField(ColumnName = "ch_heat", DefaultValue = 0)]
	public int Heat;

	[CFGTableField(ColumnName = "suspicion_limit", DefaultValue = 5)]
	public int SuspicionLimit = 5;

	[CFGTableField(ColumnName = "subdued_limit", DefaultValue = 5)]
	public int SubduedLimit = 5;

	[CFGTableField(ColumnName = "Gunpoint_Immunity", DefaultValue = false)]
	public bool GunpointImmunity;

	[CFGTableField(ColumnName = "achievement_23_data", DefaultValue = "")]
	public string Achiev23Name = string.Empty;

	public string PrefabPath
	{
		get
		{
			if (string.IsNullOrEmpty(PrefabSubPath))
			{
				return null;
			}
			return "Assets/Prefabs/Characters/" + PrefabSubPath + ".prefab";
		}
	}

	public void UpdateDifficultyData()
	{
		if (MaxHPEasy < 0)
		{
			MaxHPEasy = MaxHealth;
		}
		if (MaxHPHard < 0)
		{
			MaxHPHard = MaxHealth;
		}
		MaxHPEasy = Mathf.Clamp(MaxHPEasy, 1, 100);
		MaxHPHard = Mathf.Clamp(MaxHPHard, 1, 100);
	}
}
