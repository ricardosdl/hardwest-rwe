using UnityEngine;

public class CFGStoreItem
{
	private bool m_bPlayerItem;

	private CFGDef_Item m_ItemDef;

	private int m_Buy;

	private int m_Sell;

	private int m_TransactionItemCnt;

	private CFGCharacterData m_UsingCharacter;

	private int m_ItemCount;

	public CFGDef_Item ItemDef => m_ItemDef;

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

	public int ItemCount
	{
		get
		{
			return m_ItemCount;
		}
		set
		{
			m_ItemCount = value;
		}
	}

	public int TransactionItemCount
	{
		get
		{
			return m_TransactionItemCnt;
		}
		set
		{
			m_TransactionItemCnt = Mathf.Clamp(value, 0, 1000000);
		}
	}

	public int SellPrice => m_Sell;

	public int BuyPrice => m_Buy;

	public CFGCharacterData User => m_UsingCharacter;

	public CFGStoreItem(CFGDef_Item ItemDef, int _BaseBuy, int _BaseSell, int _ItemCount, bool IsPlayerItem, CFGCharacterData _User = null)
	{
		m_bPlayerItem = IsPlayerItem;
		if (!m_bPlayerItem)
		{
			m_Buy = _BaseSell;
			m_Sell = _BaseBuy;
		}
		else
		{
			m_Buy = _BaseBuy;
			m_Sell = _BaseSell;
		}
		m_TransactionItemCnt = 0;
		m_ItemDef = ItemDef;
		m_ItemCount = _ItemCount;
		m_UsingCharacter = _User;
	}
}
