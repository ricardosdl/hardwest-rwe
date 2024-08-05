using System.Collections.Generic;
using UnityEngine;

public class CFGDef_LootNode
{
	public const string rcard = "[random_cards]";

	public string m_Name;

	public Dictionary<string, int> m_Items = new Dictionary<string, int>();

	public void AddLoot()
	{
		if (m_Items == null || m_Items.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in m_Items)
		{
			if (string.Compare("[random_cards]", item.Key.ToString(), ignoreCase: true) == 0)
			{
				CFGInventory.AddRandomCards(item.Value);
				continue;
			}
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(item.Key);
			if (itemDefinition != null)
			{
				if (item.Key == CFGInventory.StrCashItemID)
				{
					if (item.Value < 0)
					{
						CFGInventory.Cash_Remove(-item.Value, SetAsNew: true);
					}
					else
					{
						CFGInventory.Cash_Add(item.Value, SetAsNew: true);
					}
				}
				else if (item.Value < 0)
				{
					CFGInventory.RemoveItem(item.Key, -item.Value, SetAsNew: true, shownotification: true);
				}
				else
				{
					CFGInventory.AddItem(item.Key, item.Value, SetAsNew: true, bUseCallBack: true, shownotification: true);
				}
			}
			else
			{
				CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(item.Key);
				if (cardDefinition != null)
				{
					CFGInventory.CollectCard(item.Key);
				}
				else
				{
					Debug.LogWarning("Loot table [" + m_Name + "] has invalid entry: " + item.Key);
				}
			}
		}
	}
}
