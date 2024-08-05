public class CFGDef_Buff
{
	[CFGTableField(ColumnName = "id", DefaultValue = "")]
	public string BuffID = string.Empty;

	[CFGTableField(ColumnName = "special_effect", DefaultValue = "")]
	public string SpecialEffect = string.Empty;

	[CFGTableField(ColumnName = "type", DefaultValue = EBuffType.Unknown)]
	public EBuffType BuffType;

	[CFGTableField(ColumnName = "AutoEnd", DefaultValue = EBuffAutoEndType.Never)]
	public EBuffAutoEndType AutoEnd;

	[CFGTableField(ColumnName = "Icon", DefaultValue = 0)]
	public int Icon;

	[CFGTableField(ColumnName = "HPChange", DefaultValue = 0)]
	public int HPChange;

	[CFGTableField(ColumnName = "sight_modifier", DefaultValue = 0)]
	public int Mod_Sight;

	[CFGTableField(ColumnName = "LuckChange", DefaultValue = 0)]
	public int LuckChange;

	[CFGTableField(ColumnName = "aim_modifier", DefaultValue = 0)]
	public int Mod_Aim;

	[CFGTableField(ColumnName = "damage_modifier", DefaultValue = 0)]
	public int Mod_Damage;

	[CFGTableField(ColumnName = "defense_modifier", DefaultValue = 0)]
	public int Mod_Defense;

	[CFGTableField(ColumnName = "perm_buff", DefaultValue = 0)]
	public int PermBuff;

	[CFGTableField(ColumnName = "movement_modifier", DefaultValue = 0)]
	public int Mod_Movement;

	[CFGTableField(ColumnName = "maxhp_modifier", DefaultValue = 0)]
	public int Mod_MaxHP;

	[CFGTableField(ColumnName = "maxluck_modifier", DefaultValue = 0)]
	public int Mod_MaxLuck;

	[CFGTableField(ColumnName = "Duration", DefaultValue = 0)]
	public int Duration;

	[CFGTableField(ColumnName = "font_color", DefaultValue = "neutral")]
	public string Color_Font = "neutral";

	[CFGTableField(ColumnName = "flag_positive", DefaultValue = false)]
	public bool Flag_Positive;

	[CFGTableField(ColumnName = "flag_negative", DefaultValue = false)]
	public bool Flag_Negative;

	[CFGTableField(ColumnName = "instant_effect", DefaultValue = false)]
	public bool InstantApply;

	[CFGTableField(ColumnName = "healable", DefaultValue = false)]
	public bool Healable;

	[CFGTableField(ColumnName = "gain_sound", DefaultValue = "")]
	public string GainSound = string.Empty;

	[CFGTableField(ColumnName = "lose_sound", DefaultValue = "")]
	public string LoseSound = string.Empty;

	public CFGSoundDef m_GainSoundDef;

	public CFGSoundDef m_LoseSoundDef;
}
