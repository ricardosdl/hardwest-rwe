public class CFGDef_Ability
{
	public enum ETurnEndType
	{
		False,
		True,
		Weapon
	}

	public enum EAffected
	{
		Special,
		Self,
		Anyone,
		Everyone,
		Enemy,
		Friend,
		Visible_Enemies,
		Self_Sphere
	}

	public static float ShadowCloak_Range = 3f;

	public static int ShadowCloak_Duration = 2;

	public static int Ricochet_LuckCost = 25;

	[CFGTableField(ColumnName = "name", DefaultValue = ETurnAction.None)]
	public ETurnAction AbilityID = ETurnAction.None;

	[CFGTableField(ColumnName = "ispassive", DefaultValue = false)]
	public bool IsPassive;

	[CFGTableField(ColumnName = "silent", DefaultValue = false)]
	public bool IsSilent;

	[CFGTableField(ColumnName = "needweapon", DefaultValue = false)]
	public bool NeedWeapon;

	[CFGTableField(ColumnName = "FaceTarget", DefaultValue = false)]
	public bool FaceTarget;

	[CFGTableField(ColumnName = "WaitForCam", DefaultValue = false)]
	public bool WaitForCameraFocus;

	[CFGTableField(ColumnName = "instant", DefaultValue = false)]
	public bool IsInstant;

	[CFGTableField(ColumnName = "EndDelay", DefaultValue = 0f)]
	public float Delay;

	[CFGTableField(ColumnName = "UseLOS", DefaultValue = false)]
	public bool UseLOS;

	[CFGTableField(ColumnName = "UseLOF", DefaultValue = false)]
	public bool UseLOF;

	[CFGTableField(ColumnName = "icon", DefaultValue = 0)]
	public int IconID = 5;

	[CFGTableField(ColumnName = "EffectVal", DefaultValue = 0)]
	public int EffectValue;

	[CFGTableField(ColumnName = "apcost", DefaultValue = 0)]
	public int CostAP;

	[CFGTableField(ColumnName = "cooldown", DefaultValue = 0)]
	public int Cooldown;

	[CFGTableField(ColumnName = "range", DefaultValue = 0f)]
	public float Range;

	[CFGTableField(ColumnName = "luckcost", DefaultValue = 0)]
	public int CostLuck;

	[CFGTableField(ColumnName = "endturn", DefaultValue = ETurnEndType.False)]
	public ETurnEndType EndTurn;

	[CFGTableField(ColumnName = "target", DefaultValue = EAffected.Self)]
	public EAffected Affected = EAffected.Self;

	[CFGTableField(ColumnName = "targetstate", DefaultValue = "")]
	public string TargetState = string.Empty;
}
