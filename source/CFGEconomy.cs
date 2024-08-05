#define USE_ERROR_REPORTING
using System.Collections.Generic;
using UnityEngine;

public static class CFGEconomy
{
	public static string StrFateTraderName = "fatetrader";

	public static Dictionary<string, CFGStore> m_Shops = new Dictionary<string, CFGStore>();

	public static CFGStore m_CurrentStore = null;

	public static List<CFGStoreItem> m_ShopGoods = new List<CFGStoreItem>();

	public static List<CFGStoreItem> m_PlayerGoods = new List<CFGStoreItem>();

	public static void Reset()
	{
		Debug.Log("Economy: Reset");
		ResetShops();
		m_CurrentStore = null;
		m_ShopGoods.Clear();
		m_PlayerGoods.Clear();
	}

	public static void ResetShops()
	{
		m_Shops.Clear();
		if (CFGStaticDataContainer.ShopList == null || CFGStaticDataContainer.ShopList.Count == 0)
		{
			CFGStaticDataContainer.Init();
		}
	}

	public static void RegisterAllShops()
	{
		foreach (KeyValuePair<string, CFGDef_Shop> shop in CFGStaticDataContainer.ShopList)
		{
			if (!m_Shops.ContainsKey(shop.Key))
			{
				CFGStore cFGStore = new CFGStore();
				cFGStore.Init(shop.Value);
				m_Shops.Add(shop.Key, cFGStore);
			}
		}
	}

	public static CFGStore RegisterShop(string ShopID)
	{
		if (m_Shops.TryGetValue(ShopID, out var value))
		{
			return value;
		}
		CFGDef_Shop value2 = null;
		if (!CFGStaticDataContainer.ShopList.TryGetValue(ShopID, out value2))
		{
			return null;
		}
		value = new CFGStore();
		if (value == null)
		{
			return null;
		}
		value.Init(value2);
		m_Shops.Add(ShopID, value);
		return value;
	}

	public static bool SelectShop(string ShopID)
	{
		if (ShopID == null)
		{
			Debug.LogWarning("SelectShop: name is NULL");
			return false;
		}
		if (!m_Shops.TryGetValue(ShopID, out m_CurrentStore))
		{
			m_CurrentStore = RegisterShop(ShopID);
			if (m_CurrentStore == null)
			{
				Debug.LogWarning("Failed to register store: " + ShopID);
				return false;
			}
		}
		m_ShopGoods.Clear();
		m_PlayerGoods.Clear();
		CFGDef_Shop value = null;
		CFGStaticDataContainer.ShopList.TryGetValue(m_CurrentStore.m_ShopID, out value);
		bool flag = true;
		if (value != null)
		{
			flag = value.AcceptGold;
		}
		foreach (KeyValuePair<string, CFGStoreGood> good in m_CurrentStore.m_Goods)
		{
			if (good.Value.ItemCount == 0)
			{
				continue;
			}
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(good.Key);
			if (itemDefinition != null)
			{
				CFGStoreItem cFGStoreItem = new CFGStoreItem(itemDefinition, good.Value.BuyPrice, good.Value.SellPrice, good.Value.ItemCount, IsPlayerItem: false);
				if (cFGStoreItem != null)
				{
					m_ShopGoods.Add(cFGStoreItem);
				}
			}
		}
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			CFGDef_Item itemDefinition2 = CFGStaticDataContainer.GetItemDefinition(backpackItem.ItemID);
			if (itemDefinition2 != null && (flag || string.Compare(itemDefinition2.ItemID, CFGInventory.StrCashItemID, ignoreCase: true) != 0))
			{
				int SellPrice = 0;
				int BuyPrice = 0;
				PlayerGoodGetPrice(itemDefinition2, out BuyPrice, out SellPrice);
				CFGStoreItem cFGStoreItem2 = new CFGStoreItem(itemDefinition2, BuyPrice, SellPrice, backpackItem.Count, IsPlayerItem: true);
				if (cFGStoreItem2 != null)
				{
					m_PlayerGoods.Add(cFGStoreItem2);
				}
			}
		}
		return true;
	}

	public static void PlayerGoodGetPrice(CFGDef_Item ItemDef, out int BuyPrice, out int SellPrice)
	{
		if (ItemDef == null)
		{
			BuyPrice = 1000000;
			SellPrice = 1000000;
			return;
		}
		CFGStoreGood value = null;
		if (m_CurrentStore != null && m_CurrentStore.m_Goods.TryGetValue(ItemDef.ItemID, out value))
		{
			BuyPrice = value.BuyPrice;
			SellPrice = value.SellPrice;
		}
		else
		{
			BuyPrice = ItemDef.DefBuyVal;
			SellPrice = ItemDef.DefSellVal;
		}
	}

	public static bool BuyItemsFromShop()
	{
		int num = 0;
		while (num < m_ShopGoods.Count)
		{
			if (m_ShopGoods[num].TransactionItemCount == 0)
			{
				num++;
				continue;
			}
			CFGInventory.AddItem(m_ShopGoods[num].ItemID, m_ShopGoods[num].TransactionItemCount, SetAsNew: false);
			CFGStoreItem cFGStoreItem = null;
			for (int i = 0; i < m_PlayerGoods.Count; i++)
			{
				if (string.Compare(m_PlayerGoods[i].ItemID, m_ShopGoods[num].ItemID, ignoreCase: true) == 0)
				{
					cFGStoreItem = m_PlayerGoods[i];
					break;
				}
			}
			if (cFGStoreItem == null)
			{
				CFGDef_Item itemDef = m_ShopGoods[num].ItemDef;
				if (itemDef == null)
				{
					return false;
				}
				int BuyPrice = 0;
				int SellPrice = 0;
				PlayerGoodGetPrice(itemDef, out BuyPrice, out SellPrice);
				cFGStoreItem = new CFGStoreItem(itemDef, BuyPrice, SellPrice, 0, IsPlayerItem: true);
				if (cFGStoreItem == null)
				{
					return false;
				}
				m_PlayerGoods.Add(cFGStoreItem);
			}
			cFGStoreItem.ItemCount += m_ShopGoods[num].TransactionItemCount;
			if (m_CurrentStore.ModifyItemCount(cFGStoreItem.ItemID, -m_ShopGoods[num].TransactionItemCount) == 0)
			{
				m_ShopGoods.RemoveAt(num);
				continue;
			}
			m_ShopGoods[num].ItemCount -= m_ShopGoods[num].TransactionItemCount;
			m_ShopGoods[num].TransactionItemCount = 0;
			num++;
		}
		return true;
	}

	public static CFGStoreItem FindItemInCurrentShop(string ItemID)
	{
		for (int i = 0; i < m_ShopGoods.Count; i++)
		{
			if (string.Compare(m_ShopGoods[i].ItemID, ItemID, ignoreCase: true) == 0)
			{
				return m_ShopGoods[i];
			}
		}
		return null;
	}

	public static bool GetPrices(CFGDef_Shop Store, string ItemID, out int BuyPrice, out int SellPrice)
	{
		if (!CFGStaticDataContainer.IsValidItemID(ItemID))
		{
			BuyPrice = 1000000;
			SellPrice = 1000000;
			return false;
		}
		CFGDef_Shop.CFGDef_ShopItem value = null;
		if (Store != null && Store.ItemList != null && Store.ItemList.TryGetValue(ItemID, out value))
		{
			BuyPrice = value.BaseBuy;
			SellPrice = value.BaseSell;
			return true;
		}
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition == null)
		{
			BuyPrice = 1000000;
			SellPrice = 1000000;
			return false;
		}
		BuyPrice = itemDefinition.DefBuyVal;
		SellPrice = itemDefinition.DefSellVal;
		return true;
	}

	public static bool SellItemsToShop()
	{
		int num = 0;
		while (num < m_PlayerGoods.Count)
		{
			if (m_PlayerGoods[num].TransactionItemCount == 0 || m_PlayerGoods[num].SellPrice < 1)
			{
				num++;
				continue;
			}
			Debug.Log("Selling " + m_PlayerGoods[num].TransactionItemCount + " items");
			m_CurrentStore.ModifyItemCount(m_PlayerGoods[num].ItemID, m_PlayerGoods[num].TransactionItemCount);
			CFGStoreItem cFGStoreItem = FindItemInCurrentShop(m_PlayerGoods[num].ItemID);
			if (cFGStoreItem == null)
			{
				int BuyPrice = 0;
				int SellPrice = 0;
				PlayerGoodGetPrice(m_PlayerGoods[num].ItemDef, out BuyPrice, out SellPrice);
				cFGStoreItem = new CFGStoreItem(m_PlayerGoods[num].ItemDef, BuyPrice, SellPrice, 0, IsPlayerItem: false);
				if (cFGStoreItem != null)
				{
					m_ShopGoods.Add(cFGStoreItem);
				}
			}
			cFGStoreItem.ItemCount += m_PlayerGoods[num].TransactionItemCount;
			if (CFGInventory.RemoveItem(m_PlayerGoods[num].ItemID, m_PlayerGoods[num].TransactionItemCount, SetAsNew: false))
			{
				m_PlayerGoods.RemoveAt(num);
				continue;
			}
			m_PlayerGoods[num].ItemCount -= m_PlayerGoods[num].TransactionItemCount;
			m_PlayerGoods[num].TransactionItemCount = 0;
			num++;
		}
		return true;
	}

	public static void TradeItems()
	{
		SellItemsToShop();
		BuyItemsFromShop();
	}

	private static CFGStore InitFateTrader()
	{
		CFGStore cFGStore = new CFGStore();
		if (cFGStore == null)
		{
			return null;
		}
		cFGStore.m_ShopID = StrFateTraderName;
		cFGStore.m_LocalizedShopID = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(StrFateTraderName);
		cFGStore.InitItemTable();
		m_Shops.Add(StrFateTraderName, cFGStore);
		return cFGStore;
	}

	public static void UpdateFateTrader()
	{
		if (m_Shops == null || m_Shops.Count == 0)
		{
			Reset();
		}
		CFGStore value = null;
		if (!m_Shops.TryGetValue(StrFateTraderName, out value))
		{
			Debug.Log("Failed to find fate trader shop definition. Adding temporary...");
			value = InitFateTrader();
			if (value == null)
			{
				return;
			}
		}
		if (CFGSingleton<CFGGame>.Instance.SessionSingle == null)
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.SessionSingle.GetCurrentCampaignID() != 1)
		{
			Debug.Log("There is no fate trader support from code for campaign: " + CFGSingleton<CFGGame>.Instance.SessionSingle.CampaignName);
			return;
		}
		int num = 0;
		int num2 = 8;
		int num3 = 3;
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 1; i <= num2; i++)
		{
			for (int j = 1; j <= num3; j++)
			{
				string text = "trinket_s" + i + "_0" + j;
				CFGVar variable = CFGVariableContainer.Instance.GetVariable(text, "campaign");
				if (variable == null)
				{
					Debug.LogWarning("Failed to find fate tradem item def: " + text);
					continue;
				}
				string text2 = variable.Value as string;
				if (!string.IsNullOrEmpty(text2))
				{
					text2 = text2.ToLower();
					CFGStoreGood value2 = null;
					if (value.m_Goods.TryGetValue(text2, out value2))
					{
						value2.ItemCount = 1;
					}
					else
					{
						Debug.LogError("FateTrader: failed to find item definition for id " + text2);
					}
					if (!hashSet.Contains(text2))
					{
						hashSet.Add(text2);
					}
				}
			}
		}
		foreach (KeyValuePair<string, CFGStoreGood> good in value.m_Goods)
		{
			string item = good.Key.ToLower();
			if (!hashSet.Contains(item))
			{
				good.Value.ItemCount = 0;
			}
		}
		Debug.Log("Item added to the fate trader: " + num);
		CFGAchievmentTracker.OnTrinketUnlock();
	}

	public static void ModifyTrinketCount(string VariableName, int NewCount)
	{
		if (m_Shops == null || m_Shops.Count == 0)
		{
			UpdateFateTrader();
			if (m_Shops == null || m_Shops.Count == 0)
			{
				Debug.LogWarning("Not initialized!");
				return;
			}
		}
		CFGStore value = null;
		if (!m_Shops.TryGetValue(StrFateTraderName, out value))
		{
			UpdateFateTrader();
			if (!m_Shops.TryGetValue(StrFateTraderName, out value))
			{
				Debug.LogWarning("Failed to find trade shop definition");
				return;
			}
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(VariableName, "campaign");
		if (variable == null)
		{
			Debug.LogWarning("Failed to find variable: " + VariableName);
			return;
		}
		string text = variable.Value as string;
		if (string.IsNullOrEmpty(text))
		{
			Debug.Log("No item set for " + VariableName);
			return;
		}
		CFGStoreGood value2 = null;
		if (!value.m_Goods.TryGetValue(text, out value2))
		{
			Debug.LogWarning("There is no item in the fate trader shop " + text);
		}
		else
		{
			value2.ItemCount = Mathf.Clamp(NewCount, 1, 99);
		}
	}

	public static void Shop_SetPricesAsPercentOfDefByCategory(string ShopID, CFGDef_Item.EItemType ItemType, int Buy_Percent, int Sell_Percent)
	{
		CFGStore cFGStore = RegisterShop(ShopID);
		if (cFGStore == null)
		{
			Debug.LogWarning("Failed to find shop: " + ShopID);
			return;
		}
		if (cFGStore.m_Goods == null)
		{
			Debug.LogError("No goods");
			return;
		}
		float buyModifier = (float)Buy_Percent * 0.01f;
		float sellModifier = (float)Sell_Percent * 0.01f;
		foreach (KeyValuePair<string, CFGStoreGood> good in cFGStore.m_Goods)
		{
			CFGStoreGood value = good.Value;
			if (value != null && !string.IsNullOrEmpty(value.ItemID) && value.ItemDef != null && !(value.ItemID == CFGInventory.StrCashItemID) && (value.ItemDef.ItemType == ItemType || ItemType == CFGDef_Item.EItemType.Unknown))
			{
				value.BuyModifier = buyModifier;
				value.SellModifier = sellModifier;
				value.BaseBuyPrice = value.ItemDef.DefBuyVal;
				value.BaseSellPrice = value.ItemDef.DefSellVal;
			}
		}
	}

	public static bool OnSerialize(CFG_SG_Node EcoNode)
	{
		CFG_SG_Node cFG_SG_Node = EcoNode.AddSubNode("ShopList");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		foreach (KeyValuePair<string, CFGStore> shop in m_Shops)
		{
			CFG_SG_Node cFG_SG_Node2 = cFG_SG_Node.AddSubNode("Shop");
			if (cFG_SG_Node2 == null)
			{
				return false;
			}
			if (shop.Value.m_ShopID != shop.Key)
			{
				Debug.LogWarning("Shop name mismatch!");
			}
			cFG_SG_Node2.Attrib_Set("Name", shop.Value.m_ShopID);
			if (!WriteShopInfo(cFG_SG_Node2, shop.Value))
			{
				return false;
			}
		}
		return true;
	}

	private static bool WriteShopInfo(CFG_SG_Node sn, CFGStore shop)
	{
		foreach (CFGStoreGood value in shop.m_Goods.Values)
		{
			CFG_SG_Node cFG_SG_Node = sn.AddSubNode("Item");
			if (cFG_SG_Node == null)
			{
				return false;
			}
			cFG_SG_Node.Attrib_Set("Name", value.ItemID);
			cFG_SG_Node.Attrib_Set("Count", value.ItemCount);
			cFG_SG_Node.Attrib_Set("Buy", value.BaseBuyPrice);
			cFG_SG_Node.Attrib_Set("Sell", value.BaseSellPrice);
			cFG_SG_Node.Attrib_Set("BuyMod", value.BuyModifier);
			cFG_SG_Node.Attrib_Set("SellMod", value.SellModifier);
		}
		return true;
	}

	public static bool OnDeserialize(CFG_SG_Node EcoNode)
	{
		m_CurrentStore = null;
		m_ShopGoods.Clear();
		m_Shops.Clear();
		CFG_SG_Node cFG_SG_Node = EcoNode.FindSubNode("ShopList");
		if (cFG_SG_Node == null)
		{
			Debug.LogWarning("Could not find shop list node. Resetting shops to default");
			ResetShops();
		}
		else
		{
			for (int i = 0; i < cFG_SG_Node.SubNodeCount; i++)
			{
				CFG_SG_Node subNode = cFG_SG_Node.GetSubNode(i);
				if (subNode == null || string.Compare("Shop", subNode.Name, ignoreCase: true) != 0)
				{
					Debug.LogWarning("Non-Shop node found on the shop list!");
				}
				else
				{
					Deserialize_Shop(subNode);
				}
			}
		}
		return true;
	}

	private static bool Deserialize_Shop(CFG_SG_Node ndShop)
	{
		CFGStore cFGStore = null;
		string text = ndShop.Attrib_Get("Name", string.Empty);
		CFGDef_Shop value = null;
		if (CFGStaticDataContainer.ShopList.TryGetValue(text, out value))
		{
			cFGStore = new CFGStore();
			if (cFGStore == null)
			{
				return false;
			}
			cFGStore.Init(value);
		}
		else
		{
			if (string.Compare(text, StrFateTraderName, ignoreCase: true) != 0)
			{
				CFGError.ReportError("Failed to deserialize shop: " + text + " Reason: doesnt exist in the shop list", CFGError.ErrorCode.InvalidDefinition, CFGError.ErrorType.Warning);
				return false;
			}
			cFGStore = InitFateTrader();
			if (cFGStore == null)
			{
				return false;
			}
		}
		for (int i = 0; i < ndShop.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = ndShop.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Item", ignoreCase: true) != 0)
			{
				continue;
			}
			string text2 = subNode.Attrib_Get("Name", string.Empty);
			CFGStoreGood value2 = null;
			if (!cFGStore.m_Goods.TryGetValue(text2, out value2))
			{
				Debug.LogWarning("Store : " + text + " has no item : " + text2);
				continue;
			}
			int Val = 0;
			if (subNode.Attrib_Get("Count", 0, out Val))
			{
				value2.ItemCount = Val;
			}
			if (subNode.Attrib_Get("Buy", 0, out Val))
			{
				value2.BaseBuyPrice = Val;
			}
			if (subNode.Attrib_Get("Sell", 0, out Val))
			{
				value2.BaseSellPrice = Val;
			}
			float Val2 = 0f;
			if (subNode.Attrib_Get("BuyMod", 0f, out Val2))
			{
				value2.BuyModifier = Val2;
			}
			if (subNode.Attrib_Get("SellMod", 0f, out Val2))
			{
				value2.SellModifier = Val2;
			}
		}
		if (!m_Shops.ContainsKey(cFGStore.m_ShopID))
		{
			m_Shops.Add(cFGStore.m_ShopID, cFGStore);
		}
		return true;
	}
}
