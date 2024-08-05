using System.Collections.Generic;
using UnityEngine;

public static class CFGInventory
{
	private class CFGTeamContainer
	{
		public List<CFGItem> m_BackpackItems = new List<CFGItem>();

		public Dictionary<string, CCardStatus> m_CollectedCards = new Dictionary<string, CCardStatus>();

		public void OnSerialize(CFG_SG_Node node)
		{
			node.Attrib_Set("Items", m_BackpackItems.Count);
			for (int i = 0; i < m_BackpackItems.Count; i++)
			{
				CFG_SG_Node cFG_SG_Node = node.AddSubNode("Item");
				if (cFG_SG_Node == null)
				{
					return;
				}
				cFG_SG_Node.Attrib_Set("Name", m_BackpackItems[i].ItemID);
				cFG_SG_Node.Attrib_Set("Count", m_BackpackItems[i].Count);
				cFG_SG_Node.Attrib_Set("New", m_BackpackItems[i].NewCount);
				cFG_SG_Node.Attrib_Set("Removed", m_BackpackItems[i].RemovedCount);
			}
			foreach (KeyValuePair<string, CCardStatus> collectedCard in m_CollectedCards)
			{
				CFG_SG_Node cFG_SG_Node2 = node.AddSubNode("Card");
				if (cFG_SG_Node2 == null)
				{
					break;
				}
				cFG_SG_Node2.Attrib_Set("ID", collectedCard.Key);
				cFG_SG_Node2.Attrib_Set("State", collectedCard.Value.Status);
				cFG_SG_Node2.Attrib_Set("New", collectedCard.Value.IsNew);
			}
		}

		public bool OnDeserialize(CFG_SG_Node node)
		{
			m_BackpackItems.Clear();
			m_CollectedCards.Clear();
			for (int i = 0; i < node.SubNodeCount; i++)
			{
				CFG_SG_Node subNode = node.GetSubNode(i);
				if (subNode == null)
				{
					Debug.LogError("Null subnode");
				}
				else if (string.Compare(subNode.Name, "Item", ignoreCase: true) == 0)
				{
					string text = subNode.Attrib_Get<string>("Name", null);
					if (text == null)
					{
						Debug.LogError("Failed to find attrib 'name' ");
						continue;
					}
					int num = subNode.Attrib_Get("Count", 0);
					int newCount = subNode.Attrib_Get("New", 0);
					int removedCount = subNode.Attrib_Get("Removed", 0);
					if (!string.IsNullOrEmpty(text) && num > 0)
					{
						CFGItem cFGItem = new CFGItem();
						if (cFGItem != null)
						{
							cFGItem.ItemID = text;
							cFGItem.ItemDef = CFGStaticDataContainer.GetItemDefinition(text);
							cFGItem.Count = num;
							cFGItem.RemovedCount = removedCount;
							cFGItem.NewCount = newCount;
							m_BackpackItems.Add(cFGItem);
						}
					}
					else if (num > 0)
					{
						Debug.LogError("failed to add item: " + text);
					}
				}
				else if (string.Compare(subNode.Name, "Card", ignoreCase: true) == 0)
				{
					string text2 = subNode.Attrib_Get<string>("ID", null);
					ECardStatus eCardStatus = subNode.Attrib_Get("State", ECardStatus.Collected);
					bool isNew = subNode.Attrib_Get("New", DefVal: false, bReport: false);
					if (text2 != null && !m_CollectedCards.ContainsKey(text2))
					{
						CCardStatus cCardStatus = new CCardStatus();
						if (cCardStatus != null && eCardStatus != 0)
						{
							cCardStatus.Status = eCardStatus;
							cCardStatus.IsNew = isNew;
							m_CollectedCards.Add(text2, cCardStatus);
						}
					}
				}
				else
				{
					Debug.LogError("Inventory: Failed to deserialize node: " + subNode.Name);
				}
			}
			return true;
		}
	}

	private static CFGTeamContainer m_Container_T1 = new CFGTeamContainer();

	private static CFGTeamContainer m_Container_T2 = new CFGTeamContainer();

	public static string StrCashItemID = "cash";

	public static List<CFGItem> BackpackItems
	{
		get
		{
			if (CFGCharacterList.CurrentTeamID == 1)
			{
				return m_Container_T2.m_BackpackItems;
			}
			return m_Container_T1.m_BackpackItems;
		}
	}

	public static Dictionary<string, CCardStatus> CollectedCards
	{
		get
		{
			if (CFGCharacterList.CurrentTeamID == 1)
			{
				return m_Container_T2.m_CollectedCards;
			}
			return m_Container_T1.m_CollectedCards;
		}
	}

	public static void Cash_Add(int Amount, bool SetAsNew)
	{
		if (Amount >= 1)
		{
			AddItem(StrCashItemID, Amount, SetAsNew: false);
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
			{
				string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_cash_received") + Amount;
				CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 4, 1);
			}
		}
	}

	public static void Cash_Remove(int Amount, bool SetAsNew)
	{
		if (Amount >= 1)
		{
			RemoveItem(StrCashItemID, Amount, SetAsNew: false);
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null)
			{
				string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_cash_lost") + Amount;
				CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 4, 1);
			}
		}
	}

	public static int Cash_Get()
	{
		for (int i = 0; i < BackpackItems.Count; i++)
		{
			if (string.Compare(BackpackItems[i].ItemID, StrCashItemID, ignoreCase: true) == 0)
			{
				return BackpackItems[i].Count;
			}
		}
		return 0;
	}

	public static void Cash_Set(int Amount)
	{
		for (int i = 0; i < BackpackItems.Count; i++)
		{
			if (string.Compare(BackpackItems[i].ItemID, StrCashItemID, ignoreCase: true) == 0)
			{
				BackpackItems[i].Count = Mathf.Max(0, Amount);
				return;
			}
		}
		AddItem(StrCashItemID, Amount, SetAsNew: false);
	}

	public static void Reset()
	{
		Debug.Log("Inventory: Reset");
		if (m_Container_T1 != null)
		{
			m_Container_T1.m_BackpackItems.Clear();
			m_Container_T1.m_CollectedCards.Clear();
		}
		if (m_Container_T2 != null)
		{
			m_Container_T2.m_BackpackItems.Clear();
			m_Container_T2.m_CollectedCards.Clear();
		}
	}

	public static void AddItem(string ItemID, int Count, bool SetAsNew, bool bUseCallBack = true, bool shownotification = false)
	{
		if (Count < 1)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < BackpackItems.Count; i++)
		{
			if (string.Compare(BackpackItems[i].ItemID, ItemID, ignoreCase: true) == 0)
			{
				BackpackItems[i].Count += Count;
				if (SetAsNew)
				{
					BackpackItems[i].NewCount += Count;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			CFGItem cFGItem = new CFGItem();
			cFGItem.ItemDef = CFGStaticDataContainer.GetItemDefinition(ItemID);
			if (cFGItem.ItemDef != null && cFGItem.ItemDef.ItemType == CFGDef_Item.EItemType.Card)
			{
				string cardID = ItemID.Replace("_item", string.Empty);
				CollectCard(cardID);
			}
			else
			{
				cFGItem.ItemID = ItemID;
				cFGItem.Count = Count;
				if (SetAsNew)
				{
					cFGItem.NewCount = Count;
				}
				BackpackItems.Add(cFGItem);
			}
		}
		if (bUseCallBack)
		{
			CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
			if (playerOwner != null && playerOwner.m_OnReceivedItemCallback != null)
			{
				playerOwner.m_OnReceivedItemCallback(ItemID, Count);
			}
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && ItemID != "cash" && shownotification)
		{
			string text = ((Count <= 1) ? string.Empty : (Count + "x "));
			CFGWindowMgr.m_ItemsForPopup.Add(text + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(ItemID + "_name"));
			CFGWindowMgr.m_LastItem = CFGStaticDataContainer.GetItemDefinition(ItemID).ShopIcon;
		}
	}

	public static bool RemoveItem(string ItemID, int Count, bool SetAsNew, bool shownotification = false)
	{
		for (int i = 0; i < BackpackItems.Count; i++)
		{
			if (string.Compare(BackpackItems[i].ItemID, ItemID, ignoreCase: true) != 0)
			{
				continue;
			}
			if (BackpackItems[i].Count <= Count)
			{
				BackpackItems.RemoveAt(i);
				if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && ItemID != "cash" && shownotification)
				{
					string text = ((Count >= -1) ? string.Empty : (-Count + "x "));
					string text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_item_lost") + text + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(ItemID + "_name");
					CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text2, 3, CFGStaticDataContainer.GetItemDefinition(ItemID).ShopIcon);
				}
				return true;
			}
			BackpackItems[i].Count -= Count;
			if (SetAsNew)
			{
				BackpackItems[i].RemovedCount += Count;
				if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && ItemID != "cash" && shownotification)
				{
					string text3 = ((Count <= 1) ? string.Empty : (Count + "x "));
					string text4 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_item_lost") + text3 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(ItemID + "_name");
					CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text4, 3, CFGStaticDataContainer.GetItemDefinition(ItemID).ShopIcon);
				}
			}
			return false;
		}
		return false;
	}

	public static int Item_GetCount(string item_id)
	{
		for (int i = 0; i < BackpackItems.Count; i++)
		{
			if (string.Compare(BackpackItems[i].ItemID, item_id, ignoreCase: true) == 0)
			{
				return BackpackItems[i].Count;
			}
		}
		return 0;
	}

	public static bool IsCardCollected(string CardID)
	{
		CCardStatus value = null;
		if (!CollectedCards.TryGetValue(CardID, out value))
		{
			return false;
		}
		if (value.Status == ECardStatus.NotCollected)
		{
			return false;
		}
		return true;
	}

	public static bool IsCardInUse(string CardID)
	{
		CCardStatus value = null;
		if (!CollectedCards.TryGetValue(CardID, out value))
		{
			return false;
		}
		return value.Status == ECardStatus.InUse;
	}

	public static bool IsCardAvailable(string CardID)
	{
		CCardStatus value = null;
		if (!CollectedCards.TryGetValue(CardID, out value))
		{
			return false;
		}
		return value.Status == ECardStatus.Collected;
	}

	public static void ResetCards()
	{
		CollectedCards.Clear();
	}

	public static void MarkCardAsNotCollected(string CardID)
	{
		if (CollectedCards != null)
		{
			CCardStatus value = null;
			if (CollectedCards.TryGetValue(CardID, out value))
			{
				value.Status = ECardStatus.NotCollected;
			}
		}
	}

	public static bool CollectCard(string CardID, ECardStatus NewStatus = ECardStatus.Collected, bool bForceSilent = false)
	{
		CCardStatus value = null;
		if (CollectedCards.TryGetValue(CardID, out value))
		{
			if (value.Status == ECardStatus.NotCollected)
			{
				value.Status = NewStatus;
				value.IsNew = true;
			}
		}
		else
		{
			value = new CCardStatus();
			value.Status = NewStatus;
			if (bForceSilent)
			{
				value.IsNew = false;
			}
			else
			{
				value.IsNew = true;
			}
			CollectedCards.Add(CardID, value);
		}
		CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(CardID);
		if (cardDefinition == null)
		{
			Debug.LogError("No card: " + CardID);
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && cardDefinition != null && !bForceSilent)
		{
			CFGWindowMgr.m_CardsForPopup.Add(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.CardID));
		}
		return true;
	}

	public static bool MoveCardToCharacter(string CharacterID, string CardID, int CardSlot)
	{
		if (CardSlot < -1 || CardSlot >= 5)
		{
			Debug.LogWarning("Valid card slots are 0 through " + 5);
			return false;
		}
		if (string.IsNullOrEmpty(CardID))
		{
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("Failed to find character data " + CharacterID);
			return false;
		}
		CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(CardID);
		if (cardDefinition == null)
		{
			Debug.LogWarning("Card ID [" + CardID + "] is invalid");
			return false;
		}
		if (CardSlot == -1)
		{
			CardSlot = characterData.GetFirstFreeCardSlot();
			if (CardSlot < 0)
			{
				Debug.LogWarning("Failed to find free card slot");
				return false;
			}
		}
		CCardStatus value = null;
		CFGDef_Card card = characterData.GetCard(CardSlot);
		if (card != null)
		{
			if (CollectedCards.TryGetValue(card.CardID, out value))
			{
				value.Status = ECardStatus.Collected;
				value.IsNew = false;
			}
			else
			{
				value = new CCardStatus();
				value.IsNew = false;
				value.Status = ECardStatus.Collected;
				CollectedCards.Add(card.CardID, value);
			}
			characterData.RemoveCard(CardSlot);
		}
		characterData.SetCard(CardSlot, cardDefinition);
		value = null;
		if (!CollectedCards.TryGetValue(CardID, out value))
		{
			value = new CCardStatus();
			value.IsNew = false;
			value.Status = ECardStatus.InUse;
			CollectedCards.Add(cardDefinition.CardID, value);
		}
		else
		{
			value.IsNew = false;
			value.Status = ECardStatus.InUse;
		}
		return true;
	}

	public static bool MoveCardFromCharacter(string CharacterID, int CardSlot)
	{
		if (CardSlot < 0 || CardSlot >= 5)
		{
			Debug.LogWarning("Valid card slots are 0 through " + 5);
			return false;
		}
		CFGCharacterData characterData = CFGCharacterList.GetCharacterData(CharacterID);
		if (characterData == null)
		{
			Debug.LogWarning("Failed to find character data " + CharacterID);
			return false;
		}
		CFGDef_Card card = characterData.GetCard(CardSlot);
		if (card == null)
		{
			return false;
		}
		CCardStatus value = null;
		if (CollectedCards.TryGetValue(card.CardID, out value))
		{
			value.IsNew = false;
			value.Status = ECardStatus.Collected;
		}
		else
		{
			value = new CCardStatus();
			value.IsNew = false;
			value.Status = ECardStatus.Collected;
			CollectedCards.Add(card.CardID, value);
		}
		characterData.RemoveCard(CardSlot);
		return true;
	}

	public static void ResetNewStatusOnCards()
	{
		foreach (KeyValuePair<string, CCardStatus> collectedCard in CollectedCards)
		{
			collectedCard.Value.IsNew = false;
		}
	}

	public static void CollectAllCards()
	{
		if (CFGGame.DlcType != 0)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGDef_Card> card in CFGStaticDataContainer.GetCards())
		{
			bool flag = IsCardCollected(card.Key);
			bool flag2 = IsCardInUse(card.Key);
			if (!flag && !flag2)
			{
				CollectCard(card.Key);
			}
		}
	}

	public static void AddRandomCards(int Count)
	{
		if (Count < 1)
		{
			return;
		}
		int num = Count;
		List<string> list = CFGStaticDataContainer.GenerateAllCardIDList();
		if (list == null || list.Count == 0)
		{
			return;
		}
		int num2 = 0;
		while (num2 < list.Count)
		{
			if (!IsCardCollected(list[num2]))
			{
				num2++;
			}
			else
			{
				list.RemoveAt(num2);
			}
		}
		while (list.Count > 0 && num > 0)
		{
			int index = Random.Range(0, list.Count);
			CollectCard(list[index]);
			list.RemoveAt(index);
			num--;
		}
	}

	public static void OnSerialize(CFG_SG_Node node, int TeamID)
	{
		switch (TeamID)
		{
		case 0:
			m_Container_T1.OnSerialize(node);
			break;
		case 1:
			m_Container_T2.OnSerialize(node);
			break;
		}
	}

	public static bool OnDeserialize(CFG_SG_Node node, int TeamID)
	{
		switch (TeamID)
		{
		case 0:
			return m_Container_T1.OnDeserialize(node);
		case 1:
			return m_Container_T2.OnDeserialize(node);
		default:
			Debug.LogError("Invalid team id");
			return false;
		}
	}
}
