using System.Collections.Generic;
using UnityEngine;

public class CFGStaticDataContainer
{
	public const string DEV_TEST_CAMPAIGN = "DevTestCampaign";

	public const string DEV_TEST_SCENARIO = "DevTestScenario";

	public const string DEV_TEST_TACTICAL = "DevTestTactical";

	public const string MOD_EXT = "_mod.tsv";

	public const string STD_EXT = ".tsv";

	private static Dictionary<string, CFGDef_Item> m_ItemDefinitions = new Dictionary<string, CFGDef_Item>();

	private static Dictionary<string, CFGDef_Buff> m_BuffDefinitions = new Dictionary<string, CFGDef_Buff>();

	private static Dictionary<string, CFGDef_Campaign> m_Campaigns = new Dictionary<string, CFGDef_Campaign>();

	private static Dictionary<string, CFGDef_Tactical> m_Tacticals = new Dictionary<string, CFGDef_Tactical>();

	private static Dictionary<string, CFGDef_Shop> m_Shops = new Dictionary<string, CFGDef_Shop>();

	private static Dictionary<string, CFGDef_Weapon> m_Weapons = new Dictionary<string, CFGDef_Weapon>();

	private static Dictionary<string, CFGDef_Character> m_Characters = new Dictionary<string, CFGDef_Character>();

	private static Dictionary<string, CFGDef_Card> m_Cards = new Dictionary<string, CFGDef_Card>();

	private static Dictionary<string, CFGDef_UsableItem> m_UsableItems = new Dictionary<string, CFGDef_UsableItem>();

	private static Dictionary<ETurnAction, CFGDef_Ability> m_Abilities = new Dictionary<ETurnAction, CFGDef_Ability>();

	private static Dictionary<string, CFGDef_LootNode> m_LootTable = new Dictionary<string, CFGDef_LootNode>();

	private static Dictionary<int, CFGDef_Buff> m_Decays = new Dictionary<int, CFGDef_Buff>();

	private static bool m_bLoaded = false;

	private static bool m_bTactAndCampLoaded = false;

	public static Dictionary<string, CFGDef_Item> ItemDefinitions => m_ItemDefinitions;

	public static Dictionary<string, CFGDef_Shop> ShopList
	{
		get
		{
			if (!m_bLoaded)
			{
				Init();
			}
			return m_Shops;
		}
	}

	public static CFGDef_Campaign GetDLCCampaign(EDLC dlc)
	{
		if (dlc == EDLC.None || m_Campaigns == null || m_Campaigns.Count == 0)
		{
			return null;
		}
		foreach (KeyValuePair<string, CFGDef_Campaign> campaign in m_Campaigns)
		{
			if (campaign.Value.Dlc == dlc)
			{
				return campaign.Value;
			}
		}
		return null;
	}

	public static Dictionary<string, CFGDef_Weapon> Weapons()
	{
		return m_Weapons;
	}

	public static List<string> GenerateAllCardIDList()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CFGDef_Card> card in m_Cards)
		{
			list.Add(card.Key);
		}
		return list;
	}

	public static CFGDef_Card GetCardWithAbility(ETurnAction Ability)
	{
		if (Ability == ETurnAction.None)
		{
			return null;
		}
		if (!m_bLoaded || m_Cards == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Cards == null)
			{
				return null;
			}
		}
		foreach (KeyValuePair<string, CFGDef_Card> card in m_Cards)
		{
			if (card.Value != null && card.Value.AbilityID == Ability)
			{
				return card.Value;
			}
		}
		return null;
	}

	public static CFGDef_Ability GetAbilityDef(ETurnAction Ability)
	{
		if (!m_bLoaded || m_Abilities == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Abilities == null)
			{
				return null;
			}
		}
		CFGDef_Ability value = null;
		if (m_Abilities.TryGetValue(Ability, out value))
		{
			return value;
		}
		return null;
	}

	public static Dictionary<string, CFGDef_UsableItem> GetUsables()
	{
		return m_UsableItems;
	}

	public static CFGDef_UsableItem GetUsableItemDef(string ItemID)
	{
		if (m_UsableItems == null || !m_bLoaded)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_UsableItems == null)
			{
				return null;
			}
		}
		CFGDef_UsableItem value = null;
		if (!m_UsableItems.TryGetValue(ItemID, out value))
		{
			return null;
		}
		return value;
	}

	public static bool GetCampaignAndScenarioList(string SceneName, ref List<string> CampaignList, ref List<string> ScenarioList)
	{
		if (CampaignList == null || ScenarioList == null || string.IsNullOrEmpty(SceneName))
		{
			return false;
		}
		ScenarioList.Clear();
		CampaignList.Clear();
		if (m_Campaigns == null || m_Tacticals == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Campaigns == null || m_Tacticals == null)
			{
				return false;
			}
		}
		foreach (KeyValuePair<string, CFGDef_Tactical> tactical in m_Tacticals)
		{
			if (tactical.Value == null || string.Compare(SceneName, tactical.Value.SceneName, ignoreCase: true) != 0)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < ScenarioList.Count; i++)
			{
				if (string.Compare(tactical.Value.ScenarioID, ScenarioList[i], ignoreCase: true) == 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ScenarioList.Add(tactical.Value.ScenarioID);
			}
		}
		if (ScenarioList.Count == 0)
		{
			return true;
		}
		foreach (KeyValuePair<string, CFGDef_Campaign> campaign in m_Campaigns)
		{
			if (campaign.Value == null)
			{
				continue;
			}
			foreach (CFGDef_Scenario scenario in campaign.Value.ScenarioList)
			{
				if (scenario == null)
				{
					continue;
				}
				for (int j = 0; j < ScenarioList.Count; j++)
				{
					if (string.Compare(ScenarioList[j], scenario.ScenarioID, ignoreCase: true) == 0 && !CampaignList.Contains(campaign.Value.CampaignID))
					{
						CampaignList.Add(campaign.Value.CampaignID);
					}
				}
			}
		}
		return true;
	}

	public static bool GetCampaignAndScenarioFromSceneName(string SceneName, out string CampaignName, out string ScenarioName, out string TacticalID)
	{
		CampaignName = null;
		ScenarioName = null;
		TacticalID = null;
		if (m_Campaigns == null || m_Tacticals == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Campaigns == null || m_Tacticals == null)
			{
				return false;
			}
		}
		string text = null;
		foreach (KeyValuePair<string, CFGDef_Tactical> tactical in m_Tacticals)
		{
			if (tactical.Value == null || string.Compare(SceneName, tactical.Value.SceneName, ignoreCase: true) != 0)
			{
				continue;
			}
			text = tactical.Value.ScenarioID;
			TacticalID = tactical.Value.TacticalID;
			break;
		}
		foreach (KeyValuePair<string, CFGDef_Campaign> campaign in m_Campaigns)
		{
			if (campaign.Value == null)
			{
				continue;
			}
			foreach (CFGDef_Scenario scenario in campaign.Value.ScenarioList)
			{
				if (text != null)
				{
					if (string.Compare(scenario.ScenarioID, text, ignoreCase: true) == 0)
					{
						CampaignName = scenario.CampaignID;
						ScenarioName = text;
						return true;
					}
				}
				else if (string.Compare(scenario.StrategicScene, SceneName, ignoreCase: true) == 0)
				{
					CampaignName = scenario.CampaignID;
					ScenarioName = scenario.ScenarioID;
					return true;
				}
			}
		}
		return false;
	}

	public static CFGDef_Character GetCharacterDefinition(string CharID)
	{
		if (m_Characters == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Characters == null)
			{
				return null;
			}
		}
		CharID = CharID.ToLower();
		CFGDef_Character value = null;
		if (m_Characters.TryGetValue(CharID, out value))
		{
			return value;
		}
		return null;
	}

	public static CFGDef_Item GetItemDefinition(string ItemID)
	{
		if (ItemID == null)
		{
			return null;
		}
		if (m_ItemDefinitions == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_ItemDefinitions == null)
			{
				return null;
			}
		}
		CFGDef_Item value = null;
		if (!m_ItemDefinitions.TryGetValue(ItemID, out value))
		{
			return null;
		}
		return value;
	}

	public static Dictionary<string, CFGDef_Card> GetCards()
	{
		return m_Cards;
	}

	public static CFGDef_Card GetCardDefinition(string CardID)
	{
		if (m_Cards == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Cards == null)
			{
				return null;
			}
		}
		CFGDef_Card value = null;
		if (m_Cards.TryGetValue(CardID, out value))
		{
			return value;
		}
		return null;
	}

	public static Dictionary<string, CFGDef_Buff> GetBuffDefs()
	{
		return m_BuffDefinitions;
	}

	public static CFGDef_Buff GetBuff(string BuffID)
	{
		if (m_BuffDefinitions == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_BuffDefinitions == null)
			{
				return null;
			}
		}
		CFGDef_Buff value = null;
		if (!m_BuffDefinitions.TryGetValue(BuffID, out value))
		{
			return null;
		}
		return value;
	}

	public static CFGDef_Campaign GetCampaign(int Index)
	{
		if (!m_bLoaded)
		{
			Init();
		}
		foreach (KeyValuePair<string, CFGDef_Campaign> campaign in m_Campaigns)
		{
			if (campaign.Value == null || campaign.Value.Index != Index)
			{
				continue;
			}
			return campaign.Value;
		}
		return null;
	}

	public static CFGDef_Campaign GetCampaign(string CampName)
	{
		if (!m_bLoaded)
		{
			Init();
		}
		CFGDef_Campaign value = null;
		if (m_Campaigns.TryGetValue(CampName, out value))
		{
			return value;
		}
		return null;
	}

	public static CFGDef_Tactical GetTacticalByID(string TacticalID)
	{
		if (m_Tacticals == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Tacticals == null)
			{
				return null;
			}
		}
		CFGDef_Tactical value = null;
		m_Tacticals.TryGetValue(TacticalID, out value);
		return value;
	}

	public static CFGDef_Tactical GetTacticalBySceneName(string SceneName)
	{
		if (m_Tacticals == null)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Tacticals == null)
			{
				return null;
			}
		}
		foreach (KeyValuePair<string, CFGDef_Tactical> tactical in m_Tacticals)
		{
			if (string.Compare(tactical.Value.SceneName, SceneName, ignoreCase: true) == 0)
			{
				return tactical.Value;
			}
		}
		return null;
	}

	public static CFGDef_Weapon GetWeapon(string weaponID)
	{
		if (m_Weapons == null || m_Weapons.Count == 0)
		{
			if (!m_bLoaded)
			{
				Init();
			}
			if (m_Weapons == null)
			{
				return null;
			}
		}
		CFGDef_Weapon value = null;
		if (m_Weapons.TryGetValue(weaponID, out value))
		{
			return value;
		}
		return null;
	}

	public static void ApplyLootNode(string LootNodeID)
	{
		Debug.Log("Apply loot node: " + LootNodeID);
		if (m_LootTable != null && m_LootTable.Count != 0 && !string.IsNullOrEmpty(LootNodeID))
		{
			CFGDef_LootNode value = null;
			if (!m_LootTable.TryGetValue(LootNodeID, out value))
			{
				Debug.LogWarning("No loot entry: " + LootNodeID);
			}
			else
			{
				value.AddLoot();
			}
		}
	}

	private static bool BuildDLC_Dict<TKey, TValue>(ref Dictionary<TKey, TValue> Dict, EDLC DlcType, string FileName, string ColumnID) where TValue : class, new()
	{
		if (Dict == null)
		{
			return false;
		}
		string dataPathFor = CFGData.GetDataPathFor(FileName + ".tsv", DlcType);
		if (CFGData.Exists(dataPathFor) && DlcType != 0)
		{
			Debug.Log("File: " + FileName + ": using DLC data " + dataPathFor);
			Dict = CFGTableDataLoader.CreateDictionaryFromFile<TKey, TValue>(dataPathFor, ColumnID);
		}
		else
		{
			Debug.Log("File: " + FileName + ": using game base data");
			Dict = CFGTableDataLoader.CreateDictionaryFromFile<TKey, TValue>(CFGData.GetDataPathFor(FileName + ".tsv"), ColumnID);
		}
		if (DlcType != 0)
		{
			dataPathFor = CFGData.GetDataPathFor(FileName + "_mod.tsv", DlcType);
			if (CFGData.Exists(dataPathFor))
			{
				Dictionary<TKey, TValue> dictionary = CFGTableDataLoader.CreateDictionaryFromFile<TKey, TValue>(dataPathFor, ColumnID);
				if (dictionary != null)
				{
					foreach (KeyValuePair<TKey, TValue> item in dictionary)
					{
						if (Dict.ContainsKey(item.Key))
						{
							Debug.LogError(string.Concat("Dictionary: ", FileName, " from DLC: ", DlcType, " Already contains field: ", item.Key));
						}
						else
						{
							Dict.Add(item.Key, item.Value);
						}
					}
				}
			}
		}
		return true;
	}

	private static void AddCharList(List<CFGDef_Character> NewList)
	{
		if (NewList == null || NewList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < NewList.Count; i++)
		{
			if (NewList[i] != null)
			{
				string key = NewList[i].NameID.ToLower();
				if (m_Characters.ContainsKey(key))
				{
					Debug.Log("Character: " + NewList[i].NameID + " is already registered!");
					continue;
				}
				NewList[i].UpdateDifficultyData();
				m_Characters.Add(key, NewList[i]);
			}
		}
	}

	private static void ReadLootTable(string FileName)
	{
		CFGTableDataLoader cFGTableDataLoader = new CFGTableDataLoader();
		if (!cFGTableDataLoader.OpenTableFile(FileName, 1, ParseColumns: false))
		{
			return;
		}
		if (cFGTableDataLoader.LineCount < 2)
		{
			Debug.Log("LootTable: too few lines!");
			return;
		}
		CFGDef_LootNode cFGDef_LootNode = null;
		for (int i = 0; i < cFGTableDataLoader.LineCount; i++)
		{
			if (!cFGTableDataLoader.ParseLine(i, ShowWarnings: false))
			{
				continue;
			}
			if (string.Compare(cFGTableDataLoader.ParsedLine[0], "[loot]", ignoreCase: true) == 0)
			{
				if (cFGDef_LootNode != null)
				{
					cFGDef_LootNode = null;
				}
				string text = cFGTableDataLoader.ParsedLine[1];
				if (m_LootTable.ContainsKey(text))
				{
					Debug.LogWarning("Loot Node [" + text + "] at line " + i + " is already defined!");
				}
				else
				{
					cFGDef_LootNode = new CFGDef_LootNode();
					if (cFGDef_LootNode == null)
					{
						break;
					}
					cFGDef_LootNode.m_Name = text;
					m_LootTable.Add(text, cFGDef_LootNode);
				}
			}
			else if (cFGDef_LootNode != null)
			{
				string text2 = cFGTableDataLoader.ParsedLine[0];
				int value = cFGTableDataLoader.ParseInt(1);
				if (cFGDef_LootNode.m_Items.ContainsKey(text2))
				{
					Debug.LogWarning("LootNode: " + cFGDef_LootNode.m_Name + " already has defined item " + text2);
				}
				else
				{
					cFGDef_LootNode.m_Items.Add(text2, value);
				}
			}
		}
	}

	public static bool InitDLCData(EDLC DlcType = EDLC.None)
	{
		Debug.Log("Initializing gameplay definitions for DLC: " + DlcType);
		m_ItemDefinitions = new Dictionary<string, CFGDef_Item>();
		m_BuffDefinitions = new Dictionary<string, CFGDef_Buff>();
		m_Weapons = new Dictionary<string, CFGDef_Weapon>();
		m_Abilities = new Dictionary<ETurnAction, CFGDef_Ability>();
		m_Shops = new Dictionary<string, CFGDef_Shop>();
		m_LootTable = new Dictionary<string, CFGDef_LootNode>();
		m_Cards = new Dictionary<string, CFGDef_Card>();
		if (!BuildDLC_Dict(ref m_ItemDefinitions, DlcType, "itemdefs", "id"))
		{
			return false;
		}
		if (!BuildDLC_Dict(ref m_BuffDefinitions, DlcType, "buffs", "id"))
		{
			return false;
		}
		if (!BuildDLC_Dict(ref m_Weapons, DlcType, "weapons", "id"))
		{
			return false;
		}
		if (!BuildDLC_Dict(ref m_Abilities, DlcType, "abilities", "name"))
		{
			return false;
		}
		if (!BuildDLC_Dict(ref m_UsableItems, DlcType, "usable", "ItemID"))
		{
			return false;
		}
		if (!BuildDLC_Dict(ref m_Cards, DlcType, "cards", "card_id"))
		{
			return false;
		}
		string text = null;
		m_Characters = new Dictionary<string, CFGDef_Character>();
		List<CFGDef_Character> list = CFGTableDataLoader.CreateListFromFile<CFGDef_Character>(CFGData.GetDataPathFor("characters.tsv", DlcType));
		if (list == null)
		{
			list = CFGTableDataLoader.CreateListFromFile<CFGDef_Character>(CFGData.GetDataPathFor("characters.tsv"));
		}
		AddCharList(list);
		if (DlcType != 0)
		{
			list = CFGTableDataLoader.CreateListFromFile<CFGDef_Character>(CFGData.GetDataPathFor("characters_mod.tsv", DlcType));
			if (list != null)
			{
				AddCharList(list);
			}
		}
		text = CFGData.GetDataPathFor("loot.tsv", DlcType);
		if (CFGData.Exists(text))
		{
			ReadLootTable(text);
		}
		else
		{
			text = CFGData.GetDataPathFor("loot.tsv");
			ReadLootTable(text);
		}
		if (DlcType != 0)
		{
			text = CFGData.GetDataPathFor("loot_mod.tsv", DlcType);
			if (CFGData.Exists(text))
			{
				ReadLootTable(text);
			}
		}
		text = CFGData.GetDataPathFor("shopdefs.tsv", DlcType);
		if (CFGData.Exists(text))
		{
			LoadShopDefs(text);
		}
		else
		{
			text = CFGData.GetDataPathFor("shopdefs.tsv");
			if (CFGData.Exists(text))
			{
				LoadShopDefs(text);
			}
		}
		if (DlcType != 0)
		{
			text = CFGData.GetDataPathFor("shopdefs_mod.tsv", DlcType);
			if (CFGData.Exists(text))
			{
				LoadShopDefs(text);
			}
		}
		if (m_UsableItems != null)
		{
			foreach (KeyValuePair<string, CFGDef_UsableItem> usableItem in m_UsableItems)
			{
				if (usableItem.Value != null)
				{
					usableItem.Value.FinalizeImport();
				}
			}
		}
		if (m_Cards != null && m_Cards.Count > 0)
		{
			foreach (KeyValuePair<string, CFGDef_Card> card in m_Cards)
			{
				if (card.Value != null)
				{
					card.Value.GenerateStats();
				}
			}
		}
		CFGDef_Ability value = null;
		if (m_Abilities != null && m_Abilities.TryGetValue(ETurnAction.ShadowCloak, out value))
		{
			CFGDef_Ability.ShadowCloak_Duration = value.EffectValue;
			CFGDef_Ability.ShadowCloak_Range = value.Range;
		}
		if (m_Abilities != null && m_Abilities.TryGetValue(ETurnAction.Ricochet, out value))
		{
			CFGDef_Ability.Ricochet_LuckCost = value.CostLuck;
		}
		AcquireWeaponsPrefabs();
		AcquireBuffsPrefabs();
		bool flag = false;
		string text2 = "Loaded: ";
		if (m_ItemDefinitions == null || m_ItemDefinitions.Count == 0)
		{
			text2 += " NO items";
			flag = true;
		}
		else
		{
			text2 = text2 + " " + m_ItemDefinitions.Count + " items";
		}
		if (m_BuffDefinitions == null || m_BuffDefinitions.Count == 0)
		{
			text2 += " NO buffs";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_BuffDefinitions.Count + " buffs";
		}
		if (m_Weapons == null || m_Weapons.Count == 0)
		{
			text2 += " NO WEAPONS";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_Weapons.Count + " weapons";
		}
		if (m_Characters == null || m_Characters.Count == 0)
		{
			text2 += " NO CHARACTERS";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_Characters.Count + " characters";
		}
		if (m_Cards == null || m_Cards.Count == 0)
		{
			text2 += " NO CARDS";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_Cards.Count + " cards";
		}
		if (m_Abilities == null || m_Abilities.Count == 0)
		{
			text2 += " NO ABILITIES";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_Abilities.Count + " abilities";
		}
		if (m_LootTable == null || m_LootTable.Count == 0)
		{
			text2 += " NO LOOT TABLE";
			flag = true;
		}
		else
		{
			text2 = text2 + ", " + m_LootTable.Count + " loot nodes";
		}
		if (flag)
		{
			Debug.LogError(text2);
		}
		else
		{
			Debug.Log(text2);
		}
		if (m_Decays == null)
		{
			m_Decays = new Dictionary<int, CFGDef_Buff>();
		}
		m_Decays.Clear();
		foreach (KeyValuePair<string, CFGDef_Buff> buffDefinition in m_BuffDefinitions)
		{
			if (buffDefinition.Value.BuffType == EBuffType.DecayBuffDLC)
			{
				int key = 0;
				try
				{
					key = int.Parse(buffDefinition.Value.SpecialEffect);
				}
				catch
				{
				}
				if (m_Decays.ContainsKey(key))
				{
					Debug.LogWarning("Cannot register decay " + buffDefinition.Value.BuffID + " special value (int) must be unique");
				}
				else
				{
					m_Decays.Add(key, buffDefinition.Value);
				}
			}
		}
		return true;
	}

	private static bool LoadShopDefs(string FileName)
	{
		CFGTableDataLoader cFGTableDataLoader = new CFGTableDataLoader();
		if (!cFGTableDataLoader.OpenTableFile(FileName, 1, ParseColumns: false))
		{
			return false;
		}
		if (cFGTableDataLoader.LineCount < 2)
		{
			Debug.Log("ShopDefs: too few lines!");
			return false;
		}
		CFGDef_Shop cFGDef_Shop = null;
		for (int i = 0; i < cFGTableDataLoader.LineCount; i++)
		{
			if (!cFGTableDataLoader.ParseLine(i, ShowWarnings: false))
			{
				continue;
			}
			if (string.Compare(cFGTableDataLoader.ParsedLine[0], "[shop]", ignoreCase: true) == 0)
			{
				if (cFGDef_Shop != null)
				{
					cFGDef_Shop = null;
				}
				string text = cFGTableDataLoader.ParsedLine[1];
				if (m_Shops.ContainsKey(text))
				{
					Debug.LogWarning("Shop [" + text + "] at line " + i + " is already defined!");
					continue;
				}
				int count = cFGTableDataLoader.ParseInt(2);
				bool flag = false;
				string subString = cFGTableDataLoader.GetSubString(3);
				if (subString != null && string.Compare("nocash", subString, ignoreCase: true) == 0)
				{
					flag = true;
				}
				cFGDef_Shop = new CFGDef_Shop();
				cFGDef_Shop.ShopID = text;
				cFGDef_Shop.AcceptGold = !flag;
				cFGDef_Shop.ItemList.Add(CFGInventory.StrCashItemID, new CFGDef_Shop.CFGDef_ShopItem(CFGInventory.StrCashItemID, 1, 1, count, bCoreItem: true));
				m_Shops.Add(cFGDef_Shop.ShopID, cFGDef_Shop);
			}
			else if (cFGDef_Shop != null)
			{
				string text2 = cFGTableDataLoader.ParsedLine[0];
				CFGDef_Item itemDefinition = GetItemDefinition(text2);
				if (itemDefinition == null)
				{
					Debug.LogWarning("Shop: " + cFGDef_Shop.ShopID + " has undefined item " + text2);
					continue;
				}
				if (cFGDef_Shop.ItemList.ContainsKey(text2))
				{
					Debug.LogWarning("Shop: " + cFGDef_Shop.ShopID + " already has defined item " + text2);
					continue;
				}
				int sell = cFGTableDataLoader.ParseIntOrPercentage(1, itemDefinition.DefSellVal);
				int buy = cFGTableDataLoader.ParseIntOrPercentage(2, itemDefinition.DefBuyVal);
				int count2 = cFGTableDataLoader.ParseIntOrPercentage(3, itemDefinition.DefCount);
				cFGDef_Shop.ItemList.Add(text2, new CFGDef_Shop.CFGDef_ShopItem(text2, buy, sell, count2));
			}
		}
		return true;
	}

	public static CFGDef_Buff GetBestDecayBuff(int DecayLevel)
	{
		if (m_Decays == null || m_Decays.Count == 0)
		{
			return null;
		}
		int num = -1;
		CFGDef_Buff result = null;
		foreach (KeyValuePair<int, CFGDef_Buff> decay in m_Decays)
		{
			if (decay.Key <= DecayLevel && decay.Key >= num)
			{
				num = decay.Key;
				result = decay.Value;
			}
		}
		return result;
	}

	public static bool Init()
	{
		if (m_bLoaded)
		{
			return true;
		}
		m_bLoaded = true;
		m_Tacticals = CFGTableDataLoader.CreateDictionaryFromFile<string, CFGDef_Tactical>(CFGData.GetDataPathFor("tacticals.tsv"), "TacticalID");
		List<CFGDef_Scenario> list = CFGTableDataLoader.CreateListFromFile<CFGDef_Scenario>(CFGData.GetDataPathFor("scenarios.tsv"));
		CFGDef_Tactical cFGDef_Tactical = new CFGDef_Tactical();
		if (cFGDef_Tactical != null)
		{
			cFGDef_Tactical.ScenarioID = "DevTestScenario";
			cFGDef_Tactical.TacticalID = "DevTestTactical";
			m_Tacticals.Add("DevTestTactical", cFGDef_Tactical);
		}
		m_Campaigns = CFGTableDataLoader.CreateDictionaryFromFile<string, CFGDef_Campaign>(CFGData.GetDataPathFor("campaigns.tsv"), "CampaignID");
		CFGDef_Campaign cFGDef_Campaign = new CFGDef_Campaign();
		if (cFGDef_Campaign != null)
		{
			cFGDef_Campaign.CampaignID = "DevTestCampaign";
			CFGDef_Scenario cFGDef_Scenario = new CFGDef_Scenario();
			if (cFGDef_Scenario != null)
			{
				cFGDef_Scenario.ScenarioID = "DevTestScenario";
				cFGDef_Scenario.CampaignID = "DevTestCampaign";
				cFGDef_Scenario.StrategicScene = null;
				cFGDef_Scenario.StartTacticalID = null;
				cFGDef_Campaign.ScenarioList.Add(cFGDef_Scenario);
				m_Campaigns.Add("DevTestCampaign", cFGDef_Campaign);
			}
		}
		CFGDef_Campaign value = null;
		if (m_Campaigns == null || m_Campaigns.Count == 0)
		{
			Debug.Log("No campaign definitions were found (File: campaigns.tsv)");
		}
		else
		{
			int num = 0;
			if (list != null && list.Count > 0)
			{
				foreach (CFGDef_Scenario item in list)
				{
					if (item != null)
					{
						if (string.IsNullOrEmpty(item.ScenarioID))
						{
							Debug.LogError("Scenario has empty name (ScenarioID)");
						}
						else if (string.IsNullOrEmpty(item.CampaignID))
						{
							Debug.LogError("Scenario [" + item.ScenarioID + "] has empty campaign name (CampaignID)");
						}
						else if (!m_Campaigns.TryGetValue(item.CampaignID, out value))
						{
							Debug.LogWarning("Scenario: " + item.ScenarioID + " cannot be assigned to a non-existing campaign [" + item.CampaignID + "]");
						}
						else
						{
							value.ScenarioList.Add(item);
							num++;
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("There are no scenario definitions to assign!");
			}
			Debug.Log("Loaded " + m_Campaigns.Count + " campaign definitions with total scenario count of " + num + " and " + m_Tacticals.Count + " tactical scenes");
		}
		return true;
	}

	public static bool InitTactAndCampOnly()
	{
		if (m_bTactAndCampLoaded)
		{
			return true;
		}
		m_Tacticals = CFGTableDataLoader.CreateDictionaryFromFile<string, CFGDef_Tactical>(CFGData.GetDataPathFor("tacticals.tsv"), "TacticalID");
		m_Campaigns = CFGTableDataLoader.CreateDictionaryFromFile<string, CFGDef_Campaign>(CFGData.GetDataPathFor("campaigns.tsv"), "CampaignID");
		List<CFGDef_Scenario> list = CFGTableDataLoader.CreateListFromFile<CFGDef_Scenario>(CFGData.GetDataPathFor("scenarios.tsv"));
		CFGDef_Tactical cFGDef_Tactical = new CFGDef_Tactical();
		if (cFGDef_Tactical != null)
		{
			cFGDef_Tactical.ScenarioID = "DevTestScenario";
			cFGDef_Tactical.TacticalID = "DevTestTactical";
			m_Tacticals.Add("DevTestTactical", cFGDef_Tactical);
		}
		CFGDef_Campaign cFGDef_Campaign = new CFGDef_Campaign();
		if (cFGDef_Campaign != null)
		{
			cFGDef_Campaign.CampaignID = "DevTestCampaign";
			CFGDef_Scenario cFGDef_Scenario = new CFGDef_Scenario();
			if (cFGDef_Scenario != null)
			{
				cFGDef_Scenario.ScenarioID = "DevTestScenario";
				cFGDef_Scenario.CampaignID = "DevTestCampaign";
				cFGDef_Scenario.StrategicScene = null;
				cFGDef_Scenario.StartTacticalID = null;
				cFGDef_Campaign.ScenarioList.Add(cFGDef_Scenario);
				m_Campaigns.Add("DevTestCampaign", cFGDef_Campaign);
			}
		}
		CFGDef_Campaign value = null;
		if (m_Campaigns == null || m_Campaigns.Count == 0)
		{
			Debug.Log("No campaign definitions were found (File: campaigns.tsv)");
		}
		else
		{
			int num = 0;
			if (list != null && list.Count > 0)
			{
				foreach (CFGDef_Scenario item in list)
				{
					if (item != null)
					{
						if (string.IsNullOrEmpty(item.ScenarioID))
						{
							Debug.LogError("Scenario has empty name (ScenarioID)");
						}
						else if (string.IsNullOrEmpty(item.CampaignID))
						{
							Debug.LogError("Scenario [" + item.ScenarioID + "] has empty campaign name (CampaignID)");
						}
						else if (!m_Campaigns.TryGetValue(item.CampaignID, out value))
						{
							Debug.LogWarning("Scenario: " + item.ScenarioID + " cannot be assigned to a non-existing campaign [" + item.CampaignID + "]");
						}
						else
						{
							value.ScenarioList.Add(item);
							num++;
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("There are no scenario definitions to assign!");
			}
			Debug.Log("Loaded " + m_Campaigns.Count + " campaign definitions with total scenario count of " + num + " and " + m_Tacticals.Count + " tactical scenes");
		}
		m_bTactAndCampLoaded = true;
		return true;
	}

	private static void AcquireWeaponsPrefabs()
	{
		if (m_Weapons == null || m_Weapons.Count == 0)
		{
			return;
		}
		foreach (CFGDef_Weapon value in m_Weapons.Values)
		{
			if (value != null)
			{
				value.Prefab_Bullet = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<CFGBullet>(value.BulletPrefab);
				value.Prefab_WeaponVisualisation = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<CFGWeaponVisualisation>(value.VisualisationPrefab);
				value.Prefab_PS = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<ParticleSystem>(value.FxOnFirePrefab);
				if (value.Prefab_Bullet == null)
				{
					Debug.LogWarning("Weapon " + value.ItemID + " has invalid bullet prefab!");
				}
				if (value.Prefab_WeaponVisualisation == null)
				{
					Debug.LogWarning("Weapon " + value.ItemID + " has invalid weapon_vis prefab!");
				}
				if (value.Prefab_PS == null)
				{
					Debug.LogWarning("Weapon " + value.ItemID + " has invalio on_fire prefab!");
				}
			}
		}
	}

	private static void AcquireBuffsPrefabs()
	{
		if (m_BuffDefinitions == null || m_BuffDefinitions.Count == 0)
		{
			return;
		}
		foreach (CFGDef_Buff value in m_BuffDefinitions.Values)
		{
			if (value == null)
			{
				continue;
			}
			if (value.GainSound != string.Empty)
			{
				value.m_GainSoundDef = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<CFGSoundDef>(value.GainSound);
				if (value.m_GainSoundDef == null)
				{
					Debug.LogWarning("Buff " + value.BuffID + " has invalid gain sound prefab!");
				}
			}
			if (value.LoseSound != string.Empty)
			{
				value.m_LoseSoundDef = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<CFGSoundDef>(value.LoseSound);
				if (value.m_LoseSoundDef == null)
				{
					Debug.LogWarning("Buff " + value.BuffID + " has invalid lose sound prefab!");
				}
			}
		}
	}

	public static bool IsValidItemID(string ItemID)
	{
		if (string.IsNullOrEmpty(ItemID))
		{
			return false;
		}
		if (m_ItemDefinitions == null)
		{
			Init();
			if (m_ItemDefinitions == null)
			{
				return false;
			}
		}
		if (m_ItemDefinitions.ContainsKey(ItemID))
		{
			return true;
		}
		return false;
	}
}
