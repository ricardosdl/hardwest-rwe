using UnityEngine;

public class CFGStoreGood
{
	private CFGDef_Item m_ItemDef;

	private int m_BuyPrice = 100000;

	private int m_SellPrice = 100000;

	private float m_SellMod = 1f;

	private float m_BuyMod = 1f;

	private int m_Count;

	public bool IsValid => m_ItemDef != null;

	public int ItemCount
	{
		get
		{
			return m_Count;
		}
		set
		{
			m_Count = Mathf.Max(0, value);
		}
	}

	public int BaseBuyPrice
	{
		get
		{
			return m_BuyPrice;
		}
		set
		{
			m_BuyPrice = value;
		}
	}

	public int BaseSellPrice
	{
		get
		{
			return m_SellPrice;
		}
		set
		{
			m_SellPrice = value;
		}
	}

	public int BuyPrice
	{
		get
		{
			if (m_BuyPrice < 0)
			{
				return m_BuyPrice;
			}
			float num = (float)m_BuyPrice * m_BuyMod;
			return (int)num;
		}
	}

	public int SellPrice
	{
		get
		{
			if (m_SellPrice < 0)
			{
				return m_SellPrice;
			}
			float num = (float)m_SellPrice * m_SellMod;
			return (int)num;
		}
	}

	public CFGDef_Item.EItemType Category
	{
		get
		{
			if (m_ItemDef == null)
			{
				return CFGDef_Item.EItemType.Unknown;
			}
			return m_ItemDef.ItemType;
		}
	}

	public string ItemID
	{
		get
		{
			if (m_ItemDef == null)
			{
				return null;
			}
			return m_ItemDef.ItemID;
		}
	}

	public string LocalizedItemID
	{
		get
		{
			if (m_ItemDef == null)
			{
				return null;
			}
			return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_ItemDef.ItemID);
		}
	}

	public CFGDef_Item ItemDef
	{
		get
		{
			if (m_ItemDef == null)
			{
				return null;
			}
			return m_ItemDef;
		}
	}

	public float SellModifier
	{
		get
		{
			return m_SellMod;
		}
		set
		{
			m_SellMod = Mathf.Max(0f, value);
		}
	}

	public float BuyModifier
	{
		get
		{
			return m_BuyMod;
		}
		set
		{
			m_BuyMod = Mathf.Max(0f, value);
		}
	}

	public CFGStoreGood(CFGDef_Item IDef, CFGDef_Shop.CFGDef_ShopItem ShopIDef)
	{
		m_ItemDef = IDef;
		if (ShopIDef != null)
		{
			if (m_ItemDef == null)
			{
				m_ItemDef = CFGStaticDataContainer.GetItemDefinition(ShopIDef.ItemID);
			}
			m_BuyPrice = ShopIDef.BaseBuy;
			m_SellPrice = ShopIDef.BaseSell;
			m_Count = ShopIDef.Count;
		}
	}

	public CFGStoreGood(CFGDef_Item IDef, int BuyPrice, int SellPrice, int Count)
	{
		m_ItemDef = IDef;
		m_BuyPrice = BuyPrice;
		m_SellPrice = SellPrice;
		m_Count = Count;
	}
}
