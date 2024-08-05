using System.Collections.Generic;
using UnityEngine;

public class CFGDef_Shop
{
	public class CFGDef_ShopItem
	{
		public string ItemID = string.Empty;

		public int BaseBuy;

		public int BaseSell;

		public int Count;

		public bool m_CoreItem;

		public CFGDef_ShopItem()
		{
		}

		public CFGDef_ShopItem(string _ID, int _Buy, int _Sell, int _Count, bool bCoreItem = false)
		{
			ItemID = _ID;
			BaseBuy = _Buy;
			BaseSell = _Sell;
			Count = _Count;
			m_CoreItem = bCoreItem;
		}
	}

	public string ShopID = string.Empty;

	private float m_Mod_Buy = 1f;

	private float m_Mod_Sell = 1f;

	public bool AcceptGold = true;

	private Dictionary<CFGDef_Item.EItemType, float> m_SellMods;

	private Dictionary<CFGDef_Item.EItemType, float> m_BuyMods;

	public Dictionary<string, CFGDef_ShopItem> ItemList = new Dictionary<string, CFGDef_ShopItem>();

	public float BuyMod => m_Mod_Buy;

	public float SellMod => m_Mod_Sell;

	public void SetGlobalMods(float BuyMod, float SellMod)
	{
		m_Mod_Buy = Mathf.Max(0f, BuyMod);
		m_Mod_Sell = Mathf.Max(0f, SellMod);
	}

	public void SetCategoryMods(CFGDef_Item.EItemType Category, float BuyMod, float SellMod)
	{
		if (m_SellMods == null)
		{
			m_SellMods = new Dictionary<CFGDef_Item.EItemType, float>();
		}
		if (m_BuyMods == null)
		{
			m_BuyMods = new Dictionary<CFGDef_Item.EItemType, float>();
		}
		if (m_BuyMods != null && m_SellMods != null)
		{
			if (m_BuyMods.ContainsKey(Category))
			{
				m_BuyMods.Remove(Category);
			}
			if (m_SellMods.ContainsKey(Category))
			{
				m_SellMods.Remove(Category);
			}
			m_BuyMods.Add(Category, BuyMod);
			m_SellMods.Add(Category, SellMod);
		}
	}

	public int GetBuyValue(int BaseBuy, CFGDef_Item.EItemType Category)
	{
		float value = m_Mod_Buy;
		m_BuyMods.TryGetValue(Category, out value);
		return (int)(Mathf.Clamp(value, 0f, 1000000f) * (float)BaseBuy);
	}

	public int GetSellValue(int BaseSell, CFGDef_Item.EItemType Category)
	{
		float value = m_Mod_Sell;
		m_SellMods.TryGetValue(Category, out value);
		return (int)(Mathf.Clamp(m_Mod_Sell, 0f, 1000000f) * (float)BaseSell);
	}

	public CFGDef_ShopItem ModifyItemCount(string itemID, int DeltaValue)
	{
		if (DeltaValue == 0 || string.IsNullOrEmpty(itemID))
		{
			return null;
		}
		CFGDef_ShopItem value = null;
		if (!ItemList.TryGetValue(itemID, out value))
		{
			if (DeltaValue < 0)
			{
				return null;
			}
			value = new CFGDef_ShopItem();
			if (value == null)
			{
				return null;
			}
			value.ItemID = itemID;
			value.Count = DeltaValue;
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(itemID);
			if (itemDefinition == null)
			{
				value.BaseBuy = 100;
				value.BaseSell = 80;
			}
			else
			{
				value.BaseBuy = itemDefinition.DefSellVal;
				value.BaseSell = itemDefinition.DefBuyVal;
			}
			ItemList.Add(itemID, value);
			return value;
		}
		value.Count = Mathf.Max(0, value.Count + DeltaValue);
		if (value.Count < 1 && !value.m_CoreItem)
		{
			return null;
		}
		return value;
	}
}
