using UnityEngine;

public class CFGDef_Weapon
{
	[CFGTableField(ColumnName = "id", DefaultValue = "")]
	public string ItemID = string.Empty;

	[CFGTableField(ColumnName = "IconID", DefaultValue = 0)]
	public int IconID;

	[CFGTableField(ColumnName = "range", DefaultValue = EWeaponClass.SHORT)]
	public EWeaponClass WeaponClass = EWeaponClass.SHORT;

	[CFGTableField(ColumnName = "ConeAngle", DefaultValue = 0f)]
	public float ConeAngle;

	[CFGTableField(ColumnName = "damage", DefaultValue = 0)]
	public int Damage;

	[CFGTableField(ColumnName = "ammo", DefaultValue = 0)]
	public int Ammo;

	[CFGTableField(ColumnName = "ammo_per_reload", DefaultValue = 0)]
	public int AmmoPerReload;

	[CFGTableField(ColumnName = "aim_modifier", DefaultValue = 0)]
	public int AimModifier;

	[CFGTableField(ColumnName = "defense_modifier", DefaultValue = 0)]
	public int DefenseModifier;

	[CFGTableField(ColumnName = "full_cover_div", DefaultValue = 4)]
	public int FullCoverDiv = 4;

	[CFGTableField(ColumnName = "half_cover_div", DefaultValue = 2)]
	public int HalfCoverDiv = 2;

	[CFGTableField(ColumnName = "notice_distance", DefaultValue = 10)]
	public int NoticeDistance = 10;

	[CFGTableField(ColumnName = "findable", DefaultValue = false)]
	public bool Findable;

	[CFGTableField(ColumnName = "shot_ends_turn", DefaultValue = true)]
	public bool ShotEndsTurn = true;

	[CFGTableField(ColumnName = "allows_ricochet", DefaultValue = true)]
	public bool AllowsRicochet = true;

	[CFGTableField(ColumnName = "allows_fanning", DefaultValue = true)]
	public bool AllowsFanning = true;

	[CFGTableField(ColumnName = "allows_cone", DefaultValue = true)]
	public bool AllowsCone = true;

	[CFGTableField(ColumnName = "allows_scoped", DefaultValue = true)]
	public bool AllowsScoped = true;

	[CFGTableField(ColumnName = "special", DefaultValue = "")]
	public string Special = string.Empty;

	[CFGTableField(ColumnName = "two-handed", DefaultValue = false)]
	public bool TwoHanded;

	[CFGTableField(ColumnName = "vis_prefab", DefaultValue = "")]
	public string VisualisationPrefab = string.Empty;

	[CFGTableField(ColumnName = "bullet_prefab", DefaultValue = "")]
	public string BulletPrefab = string.Empty;

	[CFGTableField(ColumnName = "fire_prefab", DefaultValue = "")]
	public string FxOnFirePrefab = string.Empty;

	[CFGTableField(ColumnName = "w_heat", DefaultValue = "")]
	public int Heat = 5;

	public CFGWeaponVisualisation Prefab_WeaponVisualisation;

	public CFGBullet Prefab_Bullet;

	public ParticleSystem Prefab_PS;
}
