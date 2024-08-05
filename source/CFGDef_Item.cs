using System.Collections.Generic;

public class CFGDef_Item
{
	[CFGFlowCode]
	public enum EItemType
	{
		Unknown,
		TradeGood,
		Talisman,
		Weapon,
		Useable,
		QuestItem,
		Card
	}

	[CFGTableField(ColumnName = "id", DefaultValue = "")]
	public string ItemID = string.Empty;

	[CFGTableField(ColumnName = "ItemType", DefaultValue = EItemType.Unknown)]
	public EItemType ItemType;

	[CFGTableField(ColumnName = "ShopIcon", DefaultValue = 0)]
	public int ShopIcon;

	[CFGTableField(ColumnName = "DefBuyVal", DefaultValue = 0)]
	public int DefBuyVal;

	[CFGTableField(ColumnName = "DefSellVal", DefaultValue = 0)]
	public int DefSellVal;

	[CFGTableField(ColumnName = "DefCount", DefaultValue = 0)]
	public int DefCount;

	[CFGTableField(ColumnName = "aim_modifier", DefaultValue = 0)]
	public int Mod_Aim;

	[CFGTableField(ColumnName = "defense_modifier", DefaultValue = 0)]
	public int Mod_Defense;

	[CFGTableField(ColumnName = "perm_buff", DefaultValue = "")]
	public string Perm_Buff = string.Empty;

	[CFGTableField(ColumnName = "movement_modifier", DefaultValue = 0)]
	public int Mod_Movement;

	[CFGTableField(ColumnName = "maxhp_modifier", DefaultValue = 0)]
	public int Mod_MaxHP;

	[CFGTableField(ColumnName = "maxluck_modifier", DefaultValue = 0)]
	public int Mod_MaxLuck;

	[CFGTableField(ColumnName = "sight_modifier", DefaultValue = 0)]
	public int Mod_Sight;

	[CFGTableField(ColumnName = "damage_modifier", DefaultValue = 0)]
	public int Mod_Damage;

	[CFGTableField(ColumnName = "heat_modifier", DefaultValue = 0)]
	public int Heat;

	[CFGTableField(ColumnName = "protect_against", DefaultValue = null)]
	public List<string> m_ForbiddenBuffs;

	public bool ProtectsAgainstBuff(string BuffID)
	{
		if (m_ForbiddenBuffs == null || m_ForbiddenBuffs.Count == 0)
		{
			return false;
		}
		foreach (string forbiddenBuff in m_ForbiddenBuffs)
		{
			if (string.Compare(BuffID, forbiddenBuff, ignoreCase: true) == 0)
			{
				return true;
			}
		}
		return false;
	}
}
