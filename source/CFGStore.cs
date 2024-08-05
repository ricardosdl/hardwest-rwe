using System.Collections.Generic;
using UnityEngine;

public class CFGStore
{
	public string m_ShopID;

	public string m_LocalizedShopID;

	public Dictionary<string, CFGStoreGood> m_Goods = new Dictionary<string, CFGStoreGood>();

	public void Init(CFGDef_Shop DefShop)
	{
		if (DefShop != null)
		{
			m_ShopID = DefShop.ShopID;
			m_LocalizedShopID = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ShopID);
			InitItemTable();
			ApplyShopPrices(DefShop);
		}
	}

	public void InitItemTable()
	{
		if (m_Goods == null)
		{
			return;
		}
		m_Goods.Clear();
		foreach (CFGDef_Item value2 in CFGStaticDataContainer.ItemDefinitions.Values)
		{
			CFGStoreGood value = new CFGStoreGood(value2, value2.DefBuyVal, value2.DefSellVal, 0);
			m_Goods.Add(value2.ItemID, value);
		}
	}

	public void ApplyShopPrices(CFGDef_Shop Shop)
	{
		if (Shop == null)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGDef_Shop.CFGDef_ShopItem> item in Shop.ItemList)
		{
			CFGStoreGood value = null;
			if (!m_Goods.TryGetValue(item.Key, out value))
			{
				Debug.LogWarning("Store: " + m_LocalizedShopID + " has unknow item: " + item.Key);
				continue;
			}
			value.BaseBuyPrice = item.Value.BaseBuy;
			value.BaseSellPrice = item.Value.BaseSell;
			value.ItemCount = item.Value.Count;
		}
	}

	public int ModifyItemCount(string ItemID, int DeltaValue)
	{
		if (DeltaValue == 0)
		{
			return -1;
		}
		CFGStoreGood value = null;
		if (!m_Goods.TryGetValue(ItemID, out value))
		{
			return -1;
		}
		value.ItemCount += DeltaValue;
		return value.ItemCount;
	}

	public void SetPricesAsPercentageOfDefault(string ItemID, int Buy_Percent, int Sell_Percent)
	{
		if (m_Goods == null || m_Goods.Count == 0)
		{
			return;
		}
		float buyModifier = (float)Buy_Percent * 0.01f;
		float sellModifier = (float)Sell_Percent * 0.01f;
		if (ItemID == null)
		{
			foreach (CFGStoreGood value2 in m_Goods.Values)
			{
				if (value2 != null && string.Compare(value2.ItemID, CFGInventory.StrCashItemID, ignoreCase: true) != 0)
				{
					if (value2.ItemDef != null)
					{
						value2.BaseBuyPrice = value2.ItemDef.DefBuyVal;
						value2.BaseSellPrice = value2.ItemDef.DefSellVal;
					}
					value2.BuyModifier = buyModifier;
					value2.SellModifier = sellModifier;
				}
			}
			return;
		}
		CFGStoreGood value = null;
		if (!m_Goods.TryGetValue(ItemID, out value))
		{
			Debug.LogWarning("Shop: " + m_ShopID + " has no item " + ItemID);
			return;
		}
		if (value.ItemDef != null)
		{
			value.BaseBuyPrice = value.ItemDef.DefBuyVal;
			value.BaseSellPrice = value.ItemDef.DefSellVal;
		}
		value.BuyModifier = buyModifier;
		value.SellModifier = sellModifier;
	}
}
