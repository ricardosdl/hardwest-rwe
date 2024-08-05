using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGBarterScreenNew : CFGPanel
{
	private enum ECurrentPlayerItemSource
	{
		Nothing,
		Weapon,
		Usable,
		NonUsable,
		Talisman,
		Other
	}

	public List<CFGImageExtension> m_PlayerItemsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_NPCItemsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_PlayerOfferItemsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_NPCOfferItemsFrame = new List<CFGImageExtension>();

	public Text m_PlayerItemsFrameTxtW;

	public Text m_NPCItemsFrameTxtW;

	public Text m_PlayerOfferItemsFrameTxtW;

	public Text m_NPCOfferItemsFrameTxtW;

	public Text m_PlayerItemsFrameTxtG;

	public Text m_NPCItemsFrameTxtG;

	public Text m_PlayerOfferItemsFrameTxtG;

	public Text m_NPCOfferItemsFrameTxtG;

	public CFGImageExtension m_PlayerItemsFrameUp;

	public CFGImageExtension m_NPCItemsFrameUp;

	public CFGImageExtension m_PlayerOfferItemsFrameUp;

	public CFGImageExtension m_NPCOfferItemsFrameUp;

	public List<CFGImageExtension> m_FrameInfo = new List<CFGImageExtension>();

	public Text m_FrameTxtW;

	public Text m_FrameTxtG;

	public Text m_BarterTradeText;

	public Text m_InfoFrameTxtW;

	public Text m_InfoFrameTxtG;

	public Text m_SplitItemsTxtW;

	public Text m_SplitItemsTxtG;

	public Text m_SplitItemsCountTxt;

	public Text m_SplitItemsAmountTxt;

	public CFGButtonExtension m_LBButtonPad;

	public CFGButtonExtension m_RBButtonPad;

	public CFGButtonExtension m_LTButtonPad;

	public CFGButtonExtension m_RTButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_XButtonPad;

	public CFGButtonExtension m_VerticalButtonPad;

	public CFGButtonExtension m_RButtonPad;

	public CFGButtonExtension m_XButtonPadWarning;

	public CFGButtonExtension m_BButtonPadWarning;

	public CFGButtonExtension m_LTButtonPadSplit;

	public CFGButtonExtension m_RTButtonPadSplit;

	public CFGButtonExtension m_DLeftButtonPadSplit;

	public CFGButtonExtension m_DRightButtonPadSplit;

	public CFGButtonExtension m_BButtonPadSplit;

	public CFGButtonExtension m_AButtonPadSplit;

	public CFGButtonExtension m_XButtonPadSplit;

	public Text m_PlayersSumText;

	public Text m_NPCSumText;

	public Image m_AdjustFrame;

	public List<CFGButtonExtension> m_CharacterButtons = new List<CFGButtonExtension>();

	public List<Image> m_CharacterFrames = new List<Image>();

	public CFGImageExtension m_NPCIcon;

	public List<CFGButtonExtension> m_CharacterInventoryButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_NPCInventoryButtons = new List<CFGButtonExtension>();

	public CFGButtonExtension m_TradeButton;

	public CFGButtonExtension m_CloseButton;

	public GameObject m_ClosePopup;

	public Animator m_ClosePopupAnimator;

	public CFGButtonExtension m_PopupCloseBtn;

	public CFGButtonExtension m_PopupCancelBtn;

	public Text m_ClosePopupText;

	public Text m_ClosePopupTitle;

	public List<CFGImageExtension> m_CharItemsIcons = new List<CFGImageExtension>();

	public List<Text> m_CharItemsNames = new List<Text>();

	public List<Text> m_CharItemsCounts = new List<Text>();

	public List<Text> m_CharItemsPrices = new List<Text>();

	public List<CFGImageExtension> m_NPCItemsIcons = new List<CFGImageExtension>();

	public List<Text> m_NPCItemsNames = new List<Text>();

	public List<Text> m_NPCItemsCounts = new List<Text>();

	public List<Text> m_NPCItemsPrices = new List<Text>();

	public List<CFGImageExtension> m_CharItemsOfferIcons = new List<CFGImageExtension>();

	public List<Text> m_CharItemsOfferNames = new List<Text>();

	public List<Text> m_CharItemsOfferCounts = new List<Text>();

	public List<Text> m_CharItemsOfferPrices = new List<Text>();

	public List<CFGImageExtension> m_NPCItemsOfferIcons = new List<CFGImageExtension>();

	public List<Text> m_NPCItemsOfferNames = new List<Text>();

	public List<Text> m_NPCItemsOfferCounts = new List<Text>();

	public List<Text> m_NPCItemsOfferPrices = new List<Text>();

	public GameObject m_ItemInfoPanel;

	public GameObject m_SplitPopupPanel;

	private Animator m_SplitPopupAnimator;

	public GameObject m_SplitPopupPanelBg;

	public CFGImageExtension m_ItemInfoIcon;

	public Text m_ItemInfoCount;

	public Text m_ItemInfoName;

	public Text m_ItemInfoDescription;

	public List<Text> m_ItemInfoStatsNames = new List<Text>();

	public List<Text> m_ItemInfoStatsValues = new List<Text>();

	public Text m_SplitItemName;

	public CFGImageExtension m_SplitItemIcon;

	public Text m_SplitItemPricePerOne;

	public Text m_SplitItemCount;

	public Text m_SplitItemCurrentPrice;

	public CFGButtonExtension m_SplitItemConfirm;

	public CFGButtonExtension m_SplitItemClose;

	public CFGButtonExtension m_SplitItemAdjust;

	public CFGButtonExtension m_SplitItemMin;

	public CFGButtonExtension m_SplitItemMinus;

	public CFGButtonExtension m_SplitItemMax;

	public CFGButtonExtension m_SplitItemPlus;

	public List<Scrollbar> m_Scrollbars = new List<Scrollbar>();

	public List<float> m_PrevScrollbars = new List<float>();

	public Text m_PlayerSum;

	public Text m_NPCSum;

	public Text m_BarterHeader;

	public Text m_PopupHeader;

	public GameObject m_InventoryItem;

	public GameObject m_PlayerInventoryParent;

	public GameObject m_NPCInventoryParent;

	public GameObject m_PlayerOfferParent;

	public GameObject m_NPCOfferParent;

	private int m_SplitItemCurrent;

	private int m_SplitItemMaxPossible;

	private List<CFGStoreItem> m_PlayerOffer = new List<CFGStoreItem>();

	private List<CFGStoreItem> m_NPCOffer = new List<CFGStoreItem>();

	private List<CFGStoreItem> m_PlayerCurrentItems = new List<CFGStoreItem>();

	private List<CFGStoreItem> m_NPCCurrentItems = new List<CFGStoreItem>();

	private List<CFGButtonExtension> m_PlayerOfferButtons = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_NPCOfferButtons = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_PlayerCurrentItemsButtons = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_NPCCurrentItemsButtons = new List<CFGButtonExtension>();

	private CFGStoreItem m_CurrentSelectedItem;

	private CFGButtonExtension m_CurrentSelectedButton;

	private List<CFGCharacterData> m_CharacterDataList = new List<CFGCharacterData>();

	private bool m_NeedRefresh = true;

	private int m_SplitSpeed = 1;

	private int m_CurrentSplitCount = 1;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private float m_NextReadAnalog;

	private CFGJoyMenuController m_Controller_Main = new CFGJoyMenuController();

	private CFGJoyMenuController m_Controller_PlayerCategory = new CFGJoyMenuController();

	private CFGJoyMenuController m_Controller_PlayerChars = new CFGJoyMenuController();

	private CFGJoyMenuController m_Controller_ShopkeepCategory = new CFGJoyMenuController();

	private int m_LastListNr = -1;

	private Vector3 m_YPosBtn = default(Vector3);

	private float m_ItemHeight;

	private bool m_IsInitalized;

	private bool m_NeedRefreshInfoFrame;

	public List<GameObject> m_HeatInfo = new List<GameObject>();

	public Text m_HeatText;

	private bool m_bIsOnPlayerInventory = true;

	protected override void Start()
	{
		base.Start();
		m_HeatText.gameObject.SetActive(value: false);
		m_ClosePopupAnimator = m_ClosePopup.GetComponent<Animator>();
		m_SplitPopupAnimator = m_SplitPopupPanel.GetComponent<Animator>();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_LBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_RBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LTButtonPad.gameObject.SetActive(flag);
			m_RTButtonPad.gameObject.SetActive(flag);
			m_BButtonPad.gameObject.SetActive(flag);
			m_AButtonPad.gameObject.SetActive(flag);
			m_XButtonPad.gameObject.SetActive(flag);
			m_VerticalButtonPad.gameObject.SetActive(flag);
			if (m_RButtonPad != null)
			{
				m_RButtonPad.gameObject.SetActive(flag);
			}
			m_XButtonPadWarning.gameObject.SetActive(flag);
			m_BButtonPadWarning.gameObject.SetActive(flag);
			m_LTButtonPadSplit.gameObject.SetActive(flag);
			m_RTButtonPadSplit.gameObject.SetActive(flag);
			m_DLeftButtonPadSplit.gameObject.SetActive(flag);
			m_DRightButtonPadSplit.gameObject.SetActive(flag);
			m_BButtonPadSplit.gameObject.SetActive(flag);
			m_AButtonPadSplit.gameObject.SetActive(flag);
			m_XButtonPadSplit.gameObject.SetActive(flag);
			m_TradeButton.gameObject.SetActive(!flag);
			m_CloseButton.gameObject.SetActive(!flag);
			m_PopupCancelBtn.gameObject.SetActive(!flag);
			m_PopupCloseBtn.gameObject.SetActive(!flag);
			m_SplitItemConfirm.gameObject.SetActive(!flag);
			m_SplitItemClose.gameObject.SetActive(!flag);
			m_AdjustFrame.gameObject.SetActive(flag);
			m_SplitItemMin.gameObject.SetActive(!flag);
			m_SplitItemMinus.gameObject.SetActive(!flag);
			m_SplitItemMax.gameObject.SetActive(!flag);
			m_SplitItemPlus.gameObject.SetActive(!flag);
			m_BarterTradeText.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
			if (m_LastInput != EInputMode.Gamepad)
			{
				SetMouseListHighlight();
			}
		}
		m_YPosBtn = m_RBButtonPad.transform.position;
		m_CharacterDataList = CFGCharacterList.GetTeamCharactersList();
		for (int i = 0; i < m_CharacterButtons.Count; i++)
		{
			m_CharacterButtons[i].transform.parent.gameObject.SetActive(i < m_CharacterDataList.Count);
			m_CharacterFrames[i].gameObject.SetActive(i < m_CharacterDataList.Count);
			m_CharacterButtons[i].enabled = i < m_CharacterDataList.Count;
			if (i < m_CharacterDataList.Count)
			{
				m_CharacterButtons[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
				m_CharacterButtons[i].IconNumber = m_CharacterDataList[i].ImageIDX;
				m_CharacterButtons[i].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_character", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CharacterDataList[i].Definition.NameID));
				m_CharacterButtons[i].m_Data = i;
				m_CharacterButtons[i].m_ButtonClickedCallback = OnCharClick;
			}
		}
		float width = m_CharacterButtons[0].gameObject.GetComponent<RectTransform>().rect.width;
		m_RBButtonPad.transform.position = new Vector3(m_YPosBtn.x - (float)(6 - m_CharacterDataList.Count) * width * 1.2f, m_RBButtonPad.transform.position.y, m_RBButtonPad.transform.position.z);
		m_SplitItemIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		m_ItemInfoIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		m_NPCInventoryButtons[0].m_ButtonClickedCallback = NpcInventoryClick;
		m_NPCInventoryButtons[1].m_ButtonClickedCallback = NpcWeaponsClick;
		m_NPCInventoryButtons[2].m_ButtonClickedCallback = NpcUsableClick;
		m_NPCInventoryButtons[3].m_ButtonClickedCallback = NpcTalismanClick;
		m_NPCInventoryButtons[4].m_ButtonClickedCallback = NpcNonUsableClick;
		m_CharacterInventoryButtons[0].m_ButtonClickedCallback = PlayerInventoryClick;
		m_CharacterInventoryButtons[1].m_ButtonClickedCallback = PlayerWeaponsClick;
		m_CharacterInventoryButtons[2].m_ButtonClickedCallback = PlayerUsableClick;
		m_CharacterInventoryButtons[3].m_ButtonClickedCallback = PlayerTalismanClick;
		m_CharacterInventoryButtons[4].m_ButtonClickedCallback = PlayerNonUsableClick;
		m_CharacterInventoryButtons[0].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_all");
		m_CharacterInventoryButtons[1].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_weapons");
		m_CharacterInventoryButtons[2].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_usables");
		m_CharacterInventoryButtons[3].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_talismans");
		m_CharacterInventoryButtons[4].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_misc");
		m_NPCInventoryButtons[0].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_all");
		m_NPCInventoryButtons[1].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_weapons");
		m_NPCInventoryButtons[2].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_usables");
		m_NPCInventoryButtons[3].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_talismans");
		m_NPCInventoryButtons[4].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_misc");
		m_TradeButton.m_ButtonClickedCallback = OnTradeClick;
		m_CloseButton.m_ButtonClickedCallback = OnExit;
		m_SplitItemAdjust.m_ButtonClickedCallback = OnAdjustClick;
		m_SplitItemClose.m_ButtonClickedCallback = OnCloseSplit;
		m_SplitItemConfirm.m_ButtonClickedCallback = OnConfirmSplit;
		m_SplitItemMinus.m_ButtonStaysClickedCallback = OnSplitMinus;
		m_SplitItemPlus.m_ButtonStaysClickedCallback = OnSplitPlus;
		m_SplitItemMinus.m_ButtonOutCallback = OnSplitOut;
		m_SplitItemPlus.m_ButtonOutCallback = OnSplitOut;
		m_SplitItemMinus.m_ButtonClickedCallback = OnSplitOut;
		m_SplitItemPlus.m_ButtonClickedCallback = OnSplitOut;
		m_SplitItemMax.m_ButtonClickedCallback = OnSplitMax;
		m_SplitItemMin.m_ButtonClickedCallback = OnSplitMin;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			m_NPCCurrentItems.Add(shopGood);
		}
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			m_PlayerCurrentItems.Add(playerGood);
		}
		m_ItemInfoPanel.SetActive(value: false);
		m_CharacterInventoryButtons[0].IsSelected = true;
		m_NPCInventoryButtons[0].IsSelected = true;
		m_PopupCloseBtn.m_ButtonClickedCallback = OnExit;
		m_PopupCancelBtn.m_ButtonClickedCallback = OnClosePopup;
		foreach (Scrollbar scrollbar in m_Scrollbars)
		{
			scrollbar.value = 0f;
			scrollbar.value = 1f;
			scrollbar.value = 0f;
			scrollbar.value = 1f;
		}
		InitControllerSupport();
		m_NPCInventoryParent.GetComponent<CFGButtonExtension>().m_ButtonDropCallback = OnDropInventoryNPC;
		m_PlayerInventoryParent.GetComponent<CFGButtonExtension>().m_ButtonDropCallback = OnDropInventoryPlayer;
		m_NPCOfferParent.GetComponent<CFGButtonExtension>().m_ButtonDropCallback = OnDropOfferNPC;
		m_PlayerOfferParent.GetComponent<CFGButtonExtension>().m_ButtonDropCallback = OnDropOfferPlayer;
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_BarterScreen, bEnable: true);
		SetMouseListHighlight();
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
	}

	public void OnClosePopup(int a)
	{
		if ((bool)m_ClosePopupAnimator)
		{
			m_ClosePopupAnimator.SetTrigger("Zamkniecie");
		}
		Invoke("ClosePopupDelayed", 0.5f);
	}

	private void ClosePopupDelayed()
	{
		m_ClosePopup.SetActive(value: false);
	}

	public void OnOpenPopup(int a)
	{
		m_ClosePopup.SetActive(value: true);
		if ((bool)m_ClosePopupAnimator)
		{
			m_ClosePopupAnimator.SetTrigger("Otwarcie");
		}
	}

	public void OnDropOfferPlayer(int nr)
	{
		if (!m_CurrentSelectedButton.IsInUseMeState && (CFGEconomy.m_PlayerGoods.Contains(m_CurrentSelectedItem) || m_PlayerCurrentItems.Contains(m_CurrentSelectedItem)))
		{
			OnMoveClick(0);
		}
	}

	public void OnDropOfferNPC(int nr)
	{
		if (!m_CurrentSelectedButton.IsInUseMeState && CFGEconomy.m_ShopGoods.Contains(m_CurrentSelectedItem))
		{
			OnMoveClick(0);
		}
	}

	public void OnDropInventoryPlayer(int nr)
	{
		if (m_CurrentSelectedButton.IsInUseMeState && (CFGEconomy.m_PlayerGoods.Contains(m_CurrentSelectedItem) || m_PlayerOffer.Contains(m_CurrentSelectedItem)))
		{
			OnMoveClick(0);
		}
	}

	public void OnDropInventoryNPC(int nr)
	{
		if (m_CurrentSelectedButton.IsInUseMeState && CFGEconomy.m_ShopGoods.Contains(m_CurrentSelectedItem))
		{
			OnMoveClick(0);
		}
	}

	public override void SetLocalisation()
	{
		m_BarterHeader.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(CFGEconomy.m_CurrentStore.m_ShopID + "_name");
		m_PopupHeader.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_closepopup_header");
		m_PopupCloseBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_pendingoffer_revertoffer");
		m_PopupCancelBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_pendingoffer_cancel");
		m_ClosePopupText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_closepopup_text");
		m_ClosePopupTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_closepopup_title");
		m_PlayerItemsFrameTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_players_items");
		m_PlayerItemsFrameTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_players_items");
		m_PlayerOfferItemsFrameTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_players_offer_items");
		m_PlayerOfferItemsFrameTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_players_offer_items");
		m_NPCItemsFrameTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_shop_items");
		m_NPCItemsFrameTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_shop_items");
		m_NPCOfferItemsFrameTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_shop_offer_items");
		m_NPCOfferItemsFrameTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_shop_offer_items");
		m_PlayersSumText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_player_offer_value");
		m_NPCSumText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_shop_offer_value");
		m_HeatText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_heat");
		m_XButtonPadWarning.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_pendingoffer_revertoffer");
		m_BButtonPadWarning.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_pendingoffer_cancel");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_transferitem");
		m_XButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_trade");
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_close");
		m_AButtonPadSplit.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_confirm");
		m_BButtonPadSplit.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_close");
		m_XButtonPadSplit.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_split_adjust");
		m_VerticalButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_changecategory_player");
		if (m_RButtonPad != null)
		{
			m_RButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_barter_changecategory_shop");
		}
		m_TradeButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_button_trade");
		m_InfoFrameTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_item_details");
		m_InfoFrameTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_item_details");
		m_CloseButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_button_exit");
		m_SplitItemsTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_title");
		m_SplitItemsTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_title");
		m_SplitItemsCountTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_count");
		m_SplitItemsAmountTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_total");
		m_SplitItemConfirm.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_confirm");
		m_SplitItemClose.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_cancel");
		m_SplitItemAdjust.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_split_adjust");
	}

	public void OnCharClick(int number)
	{
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterButtons[number].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		List<CFGDef_Item> list = new List<CFGDef_Item>();
		List<string> list2 = new List<string>();
		foreach (CFGStoreItem item in m_PlayerOffer)
		{
			if (item.User == m_CharacterDataList[number])
			{
				list2.Add(item.ItemDef.ItemID);
			}
		}
		if (m_CharacterDataList[number].Weapon1 != string.Empty && !list2.Contains(m_CharacterDataList[number].Weapon1))
		{
			list.Add(CFGStaticDataContainer.GetItemDefinition(m_CharacterDataList[number].Weapon1));
		}
		if (m_CharacterDataList[number].Weapon2 != string.Empty && !list2.Contains(m_CharacterDataList[number].Weapon2))
		{
			list.Add(CFGStaticDataContainer.GetItemDefinition(m_CharacterDataList[number].Weapon2));
		}
		if (m_CharacterDataList[number].Item1 != string.Empty && !list2.Contains(m_CharacterDataList[number].Item1))
		{
			list.Add(CFGStaticDataContainer.GetItemDefinition(m_CharacterDataList[number].Item1));
		}
		if (m_CharacterDataList[number].Item2 != string.Empty && !list2.Contains(m_CharacterDataList[number].Item2))
		{
			list.Add(CFGStaticDataContainer.GetItemDefinition(m_CharacterDataList[number].Item2));
		}
		if (m_CharacterDataList[number].Talisman != string.Empty && !list2.Contains(m_CharacterDataList[number].Talisman))
		{
			list.Add(CFGStaticDataContainer.GetItemDefinition(m_CharacterDataList[number].Talisman));
		}
		foreach (CFGDef_Item item2 in list)
		{
			CFGDef_Item cFGDef_Item = item2;
			if (cFGDef_Item != null)
			{
				int BuyPrice = 0;
				int SellPrice = 0;
				CFGEconomy.PlayerGoodGetPrice(cFGDef_Item, out BuyPrice, out SellPrice);
				CFGStoreItem cFGStoreItem = new CFGStoreItem(cFGDef_Item, BuyPrice, SellPrice, 1, IsPlayerItem: true, m_CharacterDataList[number]);
				if (cFGStoreItem != null)
				{
					m_PlayerCurrentItems.Add(cFGStoreItem);
				}
			}
		}
		m_NeedRefresh = true;
	}

	public void UnequipItem(CFGStoreItem item)
	{
		if (item != null && item.User != null)
		{
			if (item.User.Weapon1 == item.ItemID)
			{
				item.User.EquipItem(EItemSlot.Weapon1, null);
			}
			if (item.User.Weapon2 == item.ItemID)
			{
				item.User.EquipItem(EItemSlot.Weapon2, null);
			}
			if (item.User.Item1 == item.ItemID)
			{
				item.User.EquipItem(EItemSlot.Item1, null);
			}
			if (item.User.Item2 == item.ItemID)
			{
				item.User.EquipItem(EItemSlot.Item2, null);
			}
			if (item.User.Talisman == item.ItemID)
			{
				item.User.EquipItem(EItemSlot.Talisman, null);
			}
		}
	}

	public void OnTradeClick(int a)
	{
		foreach (CFGStoreItem item in m_PlayerOffer)
		{
			if (item.User != null)
			{
				UnequipItem(item);
				item.TransactionItemCount = 1;
				CFGEconomy.m_PlayerGoods.Add(item);
			}
		}
		CFGEconomy.TradeItems();
		foreach (CFGStoreItem item2 in m_PlayerOffer)
		{
			item2.TransactionItemCount = 0;
		}
		foreach (CFGStoreItem item3 in m_NPCOffer)
		{
			item3.TransactionItemCount = 0;
		}
		m_PlayerOffer.Clear();
		m_NPCOffer.Clear();
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			m_NPCCurrentItems.Add(shopGood);
		}
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			m_PlayerCurrentItems.Add(playerGood);
		}
		m_NeedRefresh = true;
		m_CurrentSelectedItem = null;
		m_CurrentSelectedButton = null;
		CFGAudioManager.Instance.PlaySound2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_UITrade, CFGAudioManager.Instance.m_MixInterface);
	}

	public void OnMoveClick(int a)
	{
		if (m_CurrentSelectedItem == null || m_CurrentSelectedItem.SellPrice < 0 || m_CurrentSelectedItem.SellPrice <= 0)
		{
			return;
		}
		if (m_PlayerOffer.Contains(m_CurrentSelectedItem) && m_CurrentSelectedItem.TransactionItemCount == 1 && m_CurrentSelectedButton.IsInUseMeState)
		{
			m_CurrentSelectedItem.TransactionItemCount = 0;
			m_PlayerOffer.Remove(m_CurrentSelectedItem);
			m_PlayerCurrentItems.Clear();
			foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
			{
				m_PlayerCurrentItems.Add(playerGood);
			}
			m_NeedRefresh = true;
			m_CurrentSelectedItem = null;
			m_CurrentSelectedButton = null;
			return;
		}
		if (m_NPCOffer.Contains(m_CurrentSelectedItem) && m_CurrentSelectedItem.TransactionItemCount == 1 && m_CurrentSelectedButton.IsInUseMeState)
		{
			m_CurrentSelectedItem.TransactionItemCount = 0;
			m_NPCOffer.Remove(m_CurrentSelectedItem);
			m_NPCCurrentItems.Clear();
			foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
			{
				m_NPCCurrentItems.Add(shopGood);
			}
			m_NeedRefresh = true;
			m_CurrentSelectedItem = null;
			m_CurrentSelectedButton = null;
			return;
		}
		if (m_CurrentSelectedItem.ItemCount - m_CurrentSelectedItem.TransactionItemCount == 1)
		{
			m_CurrentSelectedItem.TransactionItemCount += 1;
			if (m_PlayerCurrentItems.Contains(m_CurrentSelectedItem) && !m_PlayerOffer.Contains(m_CurrentSelectedItem))
			{
				m_PlayerOffer.Add(m_CurrentSelectedItem);
			}
			else if (m_NPCCurrentItems.Contains(m_CurrentSelectedItem) && !m_NPCOffer.Contains(m_CurrentSelectedItem))
			{
				m_NPCOffer.Add(m_CurrentSelectedItem);
			}
			m_CurrentSelectedItem = null;
			m_CurrentSelectedButton = null;
			m_NeedRefresh = true;
			return;
		}
		m_SplitPopupPanel.SetActive(value: true);
		m_SplitPopupPanelBg.SetActive(value: true);
		if ((bool)m_SplitPopupAnimator)
		{
			m_SplitPopupAnimator.SetTrigger("Otwarcie");
		}
		m_SplitItemName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CurrentSelectedItem.ItemDef.ItemID + "_name");
		m_SplitItemIcon.IconNumber = m_CurrentSelectedItem.ItemDef.ShopIcon;
		m_SplitItemPricePerOne.text = m_CurrentSelectedItem.SellPrice.ToString();
		m_SplitItemCurrent = 1;
		if (m_CurrentSelectedButton.IsInUseMeState)
		{
			m_SplitItemMaxPossible = m_CurrentSelectedItem.ItemCount;
			m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		}
		else
		{
			m_SplitItemMaxPossible = m_CurrentSelectedItem.ItemCount - m_CurrentSelectedItem.TransactionItemCount;
			m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		}
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = num.ToString();
		if (!m_CurrentSelectedButton.IsInUseMeState)
		{
			if (m_NPCOffer.Contains(m_CurrentSelectedItem) || m_NPCCurrentItems.Contains(m_CurrentSelectedItem))
			{
				m_SplitItemAdjust.gameObject.SetActive(NeededCountPlayer() <= (float)m_SplitItemMaxPossible && NeededCountPlayer() > 0f && m_LastInput != EInputMode.Gamepad);
			}
			else
			{
				m_SplitItemAdjust.gameObject.SetActive(NeededCountNPC() <= (float)m_SplitItemMaxPossible && NeededCountNPC() > 0f && m_LastInput != EInputMode.Gamepad);
			}
		}
		else if (m_NPCOffer.Contains(m_CurrentSelectedItem) || m_NPCCurrentItems.Contains(m_CurrentSelectedItem))
		{
			m_SplitItemAdjust.gameObject.SetActive(NeededCountPlayer() >= (float)(-m_SplitItemMaxPossible) && NeededCountPlayer() < 0f && m_LastInput != EInputMode.Gamepad);
		}
		else
		{
			m_SplitItemAdjust.gameObject.SetActive(NeededCountNPC() >= (float)(-m_SplitItemMaxPossible) && NeededCountNPC() < 0f && m_LastInput != EInputMode.Gamepad);
		}
		if (m_SplitItemAdjust.gameObject.activeSelf && CFGOptions.Gameplay.BarterAutoAdjust)
		{
			OnAdjustClick(0);
		}
	}

	public void OnConfirmSplit(int a)
	{
		if (m_CurrentSelectedButton.IsInUseMeState)
		{
			m_CurrentSelectedItem.TransactionItemCount -= m_SplitItemCurrent;
		}
		else
		{
			m_CurrentSelectedItem.TransactionItemCount += m_SplitItemCurrent;
		}
		if (m_PlayerCurrentItems.Contains(m_CurrentSelectedItem) && !m_PlayerOffer.Contains(m_CurrentSelectedItem) && !m_CurrentSelectedButton.IsInUseMeState)
		{
			m_PlayerOffer.Add(m_CurrentSelectedItem);
		}
		else if (m_NPCCurrentItems.Contains(m_CurrentSelectedItem) && !m_NPCOffer.Contains(m_CurrentSelectedItem) && !m_CurrentSelectedButton.IsInUseMeState)
		{
			m_NPCOffer.Add(m_CurrentSelectedItem);
		}
		else if (m_SplitItemCurrent == 0 || m_CurrentSelectedItem.TransactionItemCount == 0)
		{
			if (m_PlayerOffer.Contains(m_CurrentSelectedItem))
			{
				m_PlayerOffer.Remove(m_CurrentSelectedItem);
				m_PlayerCurrentItems.Clear();
				foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
				{
					m_PlayerCurrentItems.Add(playerGood);
				}
			}
			else if (m_NPCOffer.Contains(m_CurrentSelectedItem))
			{
				m_NPCOffer.Remove(m_CurrentSelectedItem);
				m_NPCCurrentItems.Clear();
				foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
				{
					m_NPCCurrentItems.Add(shopGood);
				}
			}
		}
		if (m_PlayerOffer.Contains(m_CurrentSelectedItem))
		{
			m_PlayerCurrentItems.Clear();
			foreach (CFGStoreItem playerGood2 in CFGEconomy.m_PlayerGoods)
			{
				m_PlayerCurrentItems.Add(playerGood2);
			}
		}
		else if (m_NPCOffer.Contains(m_CurrentSelectedItem))
		{
			m_NPCCurrentItems.Clear();
			foreach (CFGStoreItem shopGood2 in CFGEconomy.m_ShopGoods)
			{
				m_NPCCurrentItems.Add(shopGood2);
			}
		}
		m_CurrentSelectedItem = null;
		m_CurrentSelectedButton = null;
		m_NeedRefresh = true;
		Invoke("OnCloseSplitDelayed", 0.5f);
		if ((bool)m_SplitPopupAnimator)
		{
			m_SplitPopupAnimator.SetTrigger("Zamkniecie");
		}
	}

	public void OnCloseSplit(int a)
	{
		Invoke("OnCloseSplitDelayed", 0.5f);
		if ((bool)m_SplitPopupAnimator)
		{
			m_SplitPopupAnimator.SetTrigger("Zamkniecie");
		}
	}

	private void OnCloseSplitDelayed()
	{
		m_SplitPopupPanel.SetActive(value: false);
		m_SplitPopupPanelBg.SetActive(value: false);
	}

	public float NeededCountPlayer()
	{
		int num = 0;
		int num2 = 0;
		foreach (CFGStoreItem item in m_NPCOffer)
		{
			num += item.TransactionItemCount * item.SellPrice;
		}
		foreach (CFGStoreItem item2 in m_PlayerOffer)
		{
			num2 += item2.TransactionItemCount * item2.SellPrice;
		}
		return (num2 - num) / m_CurrentSelectedItem.SellPrice;
	}

	public float NeededCountNPC()
	{
		int num = 0;
		int num2 = 0;
		foreach (CFGStoreItem item in m_NPCOffer)
		{
			num += item.TransactionItemCount * item.SellPrice;
		}
		foreach (CFGStoreItem item2 in m_PlayerOffer)
		{
			num2 += item2.TransactionItemCount * item2.SellPrice;
		}
		return (num - num2) / m_CurrentSelectedItem.SellPrice;
	}

	public void OnAdjustClick(int a)
	{
		float num = ((!m_NPCOffer.Contains(m_CurrentSelectedItem) && !m_NPCCurrentItems.Contains(m_CurrentSelectedItem)) ? NeededCountNPC() : NeededCountPlayer());
		if (!m_CurrentSelectedButton.IsInUseMeState)
		{
			if (num <= (float)m_SplitItemMaxPossible && num > 0f)
			{
				m_SplitItemCurrent = (int)num;
			}
		}
		else if (num < 0f)
		{
			m_SplitItemCurrent = (int)Mathf.Abs(num);
		}
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num2 = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = num2.ToString();
	}

	public void OnSplitPlus(int a)
	{
		if (a == 1)
		{
			if (m_SplitItemCurrent < m_SplitItemMaxPossible)
			{
				m_SplitItemCurrent++;
			}
		}
		else if (m_SplitItemCurrent + m_CurrentSplitCount <= m_SplitItemMaxPossible)
		{
			m_SplitItemCurrent += m_CurrentSplitCount;
		}
		else
		{
			if (m_SplitItemCurrent == m_SplitItemMaxPossible)
			{
				return;
			}
			m_SplitItemCurrent = m_SplitItemMaxPossible;
		}
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = ((num < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : num.ToString());
	}

	public void OnSplitOut(int a)
	{
		m_CurrentSplitCount = 1;
	}

	public void OnSplitMinus(int a)
	{
		if (a == 1)
		{
			if (m_SplitItemCurrent > 1)
			{
				m_SplitItemCurrent--;
			}
		}
		else if (m_SplitItemCurrent > m_CurrentSplitCount)
		{
			m_SplitItemCurrent -= m_CurrentSplitCount;
		}
		else
		{
			if (m_SplitItemCurrent <= 1)
			{
				return;
			}
			m_SplitItemCurrent = 1;
		}
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = ((num < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : num.ToString());
	}

	private void Split_Mod(int Delta)
	{
		m_SplitItemCurrent += Delta;
		if (m_SplitItemCurrent < 0)
		{
			m_SplitItemCurrent = 0;
		}
		if (m_SplitItemCurrent >= m_SplitItemMaxPossible)
		{
			m_SplitItemCurrent = m_SplitItemMaxPossible;
		}
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = ((num < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : num.ToString());
	}

	public void OnSplitMin(int a)
	{
		m_SplitItemCurrent = 1;
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = ((num < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : num.ToString());
	}

	public void OnSplitMax(int a)
	{
		m_SplitItemCurrent = m_SplitItemMaxPossible;
		m_SplitItemCount.text = m_SplitItemCurrent.ToString();
		int num = m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice;
		m_SplitItemCurrentPrice.text = ((num < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : num.ToString());
	}

	public void NpcWeaponsClick(int a)
	{
		if (m_NPCInventoryButtons[a].IsSelected)
		{
			NpcInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension nPCInventoryButton in m_NPCInventoryButtons)
		{
			nPCInventoryButton.IsSelected = false;
		}
		m_NPCInventoryButtons[1].IsSelected = true;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			if (shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon)
			{
				m_NPCCurrentItems.Add(shopGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void NpcUsableClick(int a)
	{
		if (m_NPCInventoryButtons[a].IsSelected)
		{
			NpcInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension nPCInventoryButton in m_NPCInventoryButtons)
		{
			nPCInventoryButton.IsSelected = false;
		}
		m_NPCInventoryButtons[2].IsSelected = true;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			if (shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.Useable)
			{
				m_NPCCurrentItems.Add(shopGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void NpcTalismanClick(int a)
	{
		if (m_NPCInventoryButtons[a].IsSelected)
		{
			NpcInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension nPCInventoryButton in m_NPCInventoryButtons)
		{
			nPCInventoryButton.IsSelected = false;
		}
		m_NPCInventoryButtons[3].IsSelected = true;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			if (shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
			{
				m_NPCCurrentItems.Add(shopGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void NpcNonUsableClick(int a)
	{
		if (m_NPCInventoryButtons[a].IsSelected)
		{
			NpcInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension nPCInventoryButton in m_NPCInventoryButtons)
		{
			nPCInventoryButton.IsSelected = false;
		}
		m_NPCInventoryButtons[4].IsSelected = true;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			if (shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.TradeGood || shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.QuestItem || shopGood.ItemDef.ItemType == CFGDef_Item.EItemType.Unknown)
			{
				m_NPCCurrentItems.Add(shopGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void NpcInventoryClick(int a)
	{
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension nPCInventoryButton in m_NPCInventoryButtons)
		{
			nPCInventoryButton.IsSelected = false;
		}
		m_NPCInventoryButtons[0].IsSelected = true;
		m_NPCCurrentItems.Clear();
		foreach (CFGStoreItem shopGood in CFGEconomy.m_ShopGoods)
		{
			m_NPCCurrentItems.Add(shopGood);
		}
		m_NeedRefresh = true;
	}

	public void PlayerWeaponsClick(int a)
	{
		if (m_CharacterInventoryButtons[a].IsSelected)
		{
			PlayerInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterInventoryButtons[1].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			if (playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon)
			{
				m_PlayerCurrentItems.Add(playerGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void PlayerUsableClick(int a)
	{
		if (m_CharacterInventoryButtons[a].IsSelected)
		{
			PlayerInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterInventoryButtons[2].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			if (playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.Useable)
			{
				m_PlayerCurrentItems.Add(playerGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void PlayerTalismanClick(int a)
	{
		if (m_CharacterInventoryButtons[a].IsSelected)
		{
			PlayerInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterInventoryButtons[3].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			if (playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
			{
				m_PlayerCurrentItems.Add(playerGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void PlayerNonUsableClick(int a)
	{
		if (m_CharacterInventoryButtons[a].IsSelected)
		{
			PlayerInventoryClick(0);
			return;
		}
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterInventoryButtons[4].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			if (playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.TradeGood || playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.QuestItem || playerGood.ItemDef.ItemType == CFGDef_Item.EItemType.Unknown)
			{
				m_PlayerCurrentItems.Add(playerGood);
			}
		}
		m_NeedRefresh = true;
	}

	public void PlayerInventoryClick(int a)
	{
		foreach (CFGButtonExtension characterButton in m_CharacterButtons)
		{
			characterButton.IsSelected = false;
		}
		foreach (CFGButtonExtension characterInventoryButton in m_CharacterInventoryButtons)
		{
			characterInventoryButton.IsSelected = false;
		}
		m_CharacterInventoryButtons[0].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGStoreItem playerGood in CFGEconomy.m_PlayerGoods)
		{
			m_PlayerCurrentItems.Add(playerGood);
		}
		m_NeedRefresh = true;
	}

	public void ShowInfo()
	{
		m_ItemInfoPanel.SetActive(value: true);
		if (m_CurrentSelectedItem == null)
		{
			m_HeatInfo[0].transform.parent.gameObject.SetActive(value: false);
		}
		if (m_CurrentSelectedItem != null && m_CurrentSelectedItem.ItemDef != null)
		{
			m_HeatInfo[0].transform.parent.gameObject.SetActive(m_CurrentSelectedItem.ItemDef.Heat > 0 || m_CurrentSelectedItem.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon);
			m_HeatText.gameObject.SetActive(m_CurrentSelectedItem.ItemDef.Heat > 0 || m_CurrentSelectedItem.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon);
			for (int i = 0; i < m_HeatInfo.Count; i++)
			{
				m_HeatInfo[i].SetActive(i < m_CurrentSelectedItem.ItemDef.Heat);
			}
		}
		if (m_CurrentSelectedItem != null && m_CurrentSelectedItem.ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_CurrentSelectedItem.ItemDef.ItemID);
			if (itemDefinition != null)
			{
				m_ItemInfoStatsNames[0].text = string.Empty;
				m_ItemInfoStatsNames[1].text = string.Empty;
				m_ItemInfoStatsNames[2].text = string.Empty;
				m_ItemInfoStatsValues[0].text = string.Empty;
				m_ItemInfoStatsValues[1].text = string.Empty;
				m_ItemInfoStatsValues[2].text = string.Empty;
				string text = string.Empty;
				if (itemDefinition.Mod_Aim != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition.Mod_Defense != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition.Mod_Sight != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition.Mod_Damage != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition.Mod_Movement != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition.Mod_MaxHP != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition.Mod_MaxLuck != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition.Heat != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text);
			}
		}
		else if (m_CurrentSelectedItem != null && m_CurrentSelectedItem.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon)
		{
			CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_CurrentSelectedItem.ItemDef.ItemID);
			m_ItemInfoStatsNames[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_class_tactical_details");
			m_ItemInfoStatsNames[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_dmg_tactical_details");
			m_ItemInfoStatsNames[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_maxammo_tactical_details");
			m_ItemInfoStatsValues[0].text = weapon.WeaponClass.GetLocalizedName();
			m_ItemInfoStatsValues[1].text = weapon.Damage.ToString();
			m_ItemInfoStatsValues[2].text = weapon.Ammo.ToString();
			for (int j = 0; j < m_HeatInfo.Count; j++)
			{
				m_HeatInfo[j].SetActive(j < weapon.Heat);
			}
			string text3 = string.Empty;
			if (weapon.AllowsFanning)
			{
				text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
			}
			if (weapon.AllowsScoped)
			{
				if (text3 != string.Empty)
				{
					text3 += ", ";
				}
				text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
			}
			if (weapon.AllowsCone)
			{
				if (text3 != string.Empty)
				{
					text3 += ", ";
				}
				text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
			}
			if (!weapon.AllowsRicochet)
			{
				if (text3 != string.Empty)
				{
					text3 += ", ";
				}
				text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
			}
			if (!weapon.ShotEndsTurn)
			{
				if (text3 != string.Empty)
				{
					text3 += ", ";
				}
				text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
			}
			if (weapon.AimModifier != 0)
			{
				if (text3 != string.Empty)
				{
					text3 += ", ";
				}
				string text2 = text3;
				text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
			}
			m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", m_CurrentSelectedItem.ItemDef.ItemID + "_desc", text3, weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
		}
		else
		{
			m_ItemInfoStatsNames[0].text = string.Empty;
			m_ItemInfoStatsNames[1].text = string.Empty;
			m_ItemInfoStatsNames[2].text = string.Empty;
			m_ItemInfoStatsValues[0].text = string.Empty;
			m_ItemInfoStatsValues[1].text = string.Empty;
			m_ItemInfoStatsValues[2].text = string.Empty;
		}
	}

	public void OnItemPlayerHover(int number)
	{
		OnItemPlayerClick(number);
		ClearItemNew(m_PlayerCurrentItems[number].ItemDef.ItemID);
		m_PlayerCurrentItemsButtons[number].IsSelected = false;
		ShowInfo();
		m_ItemInfoIcon.IconNumber = m_CharItemsIcons[number].IconNumber;
		m_ItemInfoCount.text = m_CharItemsCounts[number].text;
		m_ItemInfoName.text = m_CharItemsNames[number].text;
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerCurrentItems[number].ItemDef.ItemID + "_desc");
		if (m_PlayerCurrentItems[number] != null && m_PlayerCurrentItems[number].ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_PlayerCurrentItems[number].ItemDef.ItemID);
			if (itemDefinition != null)
			{
				m_ItemInfoStatsNames[0].text = string.Empty;
				m_ItemInfoStatsNames[1].text = string.Empty;
				m_ItemInfoStatsNames[2].text = string.Empty;
				m_ItemInfoStatsValues[0].text = string.Empty;
				m_ItemInfoStatsValues[1].text = string.Empty;
				m_ItemInfoStatsValues[2].text = string.Empty;
				string text = string.Empty;
				if (itemDefinition.Mod_Aim != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition.Mod_Defense != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition.Mod_Sight != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition.Mod_Damage != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition.Mod_Movement != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition.Mod_MaxHP != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition.Mod_MaxLuck != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition.Heat != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text);
			}
		}
		if (m_PlayerCurrentItems[number] == null || m_PlayerCurrentItems[number].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon)
		{
			return;
		}
		CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_PlayerCurrentItems[number].ItemID);
		if (weapon == null)
		{
			return;
		}
		string text3 = string.Empty;
		if (weapon.AllowsFanning)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
		}
		if (weapon.AllowsScoped)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
		}
		if (weapon.AllowsCone)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
		}
		if (!weapon.AllowsRicochet)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
		}
		if (!weapon.ShotEndsTurn)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
		}
		if (weapon.AimModifier != 0)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			string text2 = text3;
			text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
		}
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerCurrentItems[number].ItemDef.ItemID + "_desc"), text3, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
	}

	public void OnItemPlayerClick(int number)
	{
		m_CurrentSelectedItem = m_PlayerCurrentItems[number];
		m_CurrentSelectedButton = m_PlayerCurrentItemsButtons[number];
	}

	public void OnItemNpcHover(int number)
	{
		OnItemNpcClick(number);
		ShowInfo();
		m_ItemInfoIcon.IconNumber = m_NPCItemsIcons[number].IconNumber;
		m_ItemInfoCount.text = m_NPCItemsCounts[number].text;
		m_ItemInfoName.text = m_NPCItemsNames[number].text;
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_NPCCurrentItems[number].ItemDef.ItemID + "_desc");
		if (m_NPCCurrentItems[number] != null && m_NPCCurrentItems[number].ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_NPCCurrentItems[number].ItemDef.ItemID);
			if (itemDefinition != null)
			{
				m_ItemInfoStatsNames[0].text = string.Empty;
				m_ItemInfoStatsNames[1].text = string.Empty;
				m_ItemInfoStatsNames[2].text = string.Empty;
				m_ItemInfoStatsValues[0].text = string.Empty;
				m_ItemInfoStatsValues[1].text = string.Empty;
				m_ItemInfoStatsValues[2].text = string.Empty;
				string text = string.Empty;
				if (itemDefinition.Mod_Aim != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition.Mod_Defense != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition.Mod_Sight != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition.Mod_Damage != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition.Mod_Movement != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition.Mod_MaxHP != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition.Mod_MaxLuck != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition.Heat != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text);
			}
		}
		if (m_NPCCurrentItems[number] == null || m_NPCCurrentItems[number].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon)
		{
			return;
		}
		CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_NPCCurrentItems[number].ItemID);
		if (weapon == null)
		{
			return;
		}
		string text3 = string.Empty;
		if (weapon.AllowsFanning)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
		}
		if (weapon.AllowsScoped)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
		}
		if (weapon.AllowsCone)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
		}
		if (!weapon.AllowsRicochet)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
		}
		if (!weapon.ShotEndsTurn)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
		}
		if (weapon.AimModifier != 0)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			string text2 = text3;
			text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
		}
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_NPCCurrentItems[number].ItemDef.ItemID + "_desc"), text3, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
	}

	public void OnItemNpcClick(int number)
	{
		m_CurrentSelectedItem = m_NPCCurrentItems[number];
		m_CurrentSelectedButton = m_NPCCurrentItemsButtons[number];
	}

	public void OnItemOfferPlayerHover(int number)
	{
		OnItemOfferPlayerClick(number);
		ShowInfo();
		m_ItemInfoPanel.SetActive(value: true);
		m_ItemInfoIcon.IconNumber = m_CharItemsOfferIcons[number].IconNumber;
		m_ItemInfoCount.text = m_CharItemsOfferCounts[number].text;
		m_ItemInfoName.text = m_CharItemsOfferNames[number].text;
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerOffer[number].ItemDef.ItemID + "_desc");
		if (m_PlayerOffer[number] != null && m_PlayerOffer[number].ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_PlayerOffer[number].ItemDef.ItemID);
			if (itemDefinition != null)
			{
				m_ItemInfoStatsNames[0].text = string.Empty;
				m_ItemInfoStatsNames[1].text = string.Empty;
				m_ItemInfoStatsNames[2].text = string.Empty;
				m_ItemInfoStatsValues[0].text = string.Empty;
				m_ItemInfoStatsValues[1].text = string.Empty;
				m_ItemInfoStatsValues[2].text = string.Empty;
				string text = string.Empty;
				if (itemDefinition.Mod_Aim != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition.Mod_Defense != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition.Mod_Sight != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition.Mod_Damage != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition.Mod_Movement != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition.Mod_MaxHP != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition.Mod_MaxLuck != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition.Heat != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text);
			}
		}
		if (m_PlayerOffer[number] == null || m_PlayerOffer[number].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon)
		{
			return;
		}
		CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_PlayerOffer[number].ItemID);
		if (weapon == null)
		{
			return;
		}
		string text3 = string.Empty;
		if (weapon.AllowsFanning)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
		}
		if (weapon.AllowsScoped)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
		}
		if (weapon.AllowsCone)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
		}
		if (!weapon.AllowsRicochet)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
		}
		if (!weapon.ShotEndsTurn)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
		}
		if (weapon.AimModifier != 0)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			string text2 = text3;
			text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
		}
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerOffer[number].ItemDef.ItemID + "_desc"), text3, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
	}

	public void OnItemOfferPlayerClick(int number)
	{
		m_CurrentSelectedItem = m_PlayerOffer[number];
		m_CurrentSelectedButton = m_PlayerOfferButtons[number];
	}

	public void OnItemOfferNpcHover(int number)
	{
		OnItemOfferNpcClick(number);
		ShowInfo();
		m_ItemInfoPanel.SetActive(value: true);
		m_ItemInfoIcon.IconNumber = m_NPCItemsOfferIcons[number].IconNumber;
		m_ItemInfoCount.text = m_NPCItemsOfferCounts[number].text;
		m_ItemInfoName.text = m_NPCItemsOfferNames[number].text;
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_NPCOffer[number].ItemDef.ItemID + "_desc");
		if (m_CurrentSelectedItem != null && m_CurrentSelectedItem.ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_CurrentSelectedItem.ItemDef.ItemID);
			if (itemDefinition != null)
			{
				m_ItemInfoStatsNames[0].text = string.Empty;
				m_ItemInfoStatsNames[1].text = string.Empty;
				m_ItemInfoStatsNames[2].text = string.Empty;
				m_ItemInfoStatsValues[0].text = string.Empty;
				m_ItemInfoStatsValues[1].text = string.Empty;
				m_ItemInfoStatsValues[2].text = string.Empty;
				string text = string.Empty;
				if (itemDefinition.Mod_Aim != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition.Mod_Defense != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition.Mod_Sight != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition.Mod_Damage != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition.Mod_Movement != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition.Mod_MaxHP != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition.Mod_MaxLuck != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition.Heat != 0)
				{
					string text2 = text;
					text = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text);
			}
		}
		if (m_NPCOffer[number] == null || m_NPCOffer[number].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon)
		{
			return;
		}
		CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_NPCOffer[number].ItemID);
		if (weapon == null)
		{
			return;
		}
		string text3 = string.Empty;
		if (weapon.AllowsFanning)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
		}
		if (weapon.AllowsScoped)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
		}
		if (weapon.AllowsCone)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
		}
		if (!weapon.AllowsRicochet)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
		}
		if (!weapon.ShotEndsTurn)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
		}
		if (weapon.AimModifier != 0)
		{
			if (text3 != string.Empty)
			{
				text3 += ", ";
			}
			string text2 = text3;
			text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
		}
		m_ItemInfoDescription.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_NPCOffer[number].ItemDef.ItemID + "_desc"), text3, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
	}

	public void OnItemOfferNpcClick(int number)
	{
		m_CurrentSelectedItem = m_NPCOffer[number];
		m_CurrentSelectedButton = m_NPCOfferButtons[number];
	}

	public void OnExit(int a)
	{
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			backpackItem.NewCount = 0;
			backpackItem.RemovedCount = 0;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance)
		{
			instance.UnloadBarterScreen();
		}
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_BarterScreen, bEnable: false);
	}

	protected bool IsItemNew(string item_id)
	{
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemID == item_id && backpackItem.NewCount > 0)
			{
				return true;
			}
		}
		return false;
	}

	protected void ClearItemNew(string item_id)
	{
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemID == item_id && backpackItem.NewCount > 0)
			{
				backpackItem.NewCount = 0;
				backpackItem.RemovedCount = 0;
			}
		}
	}

	protected void SpawnItemList(ref List<CFGStoreItem> items, List<CFGButtonExtension> btns, GameObject parent, List<CFGImageExtension> icons, List<Text> names, List<Text> prices, List<Text> count, bool offer, bool player)
	{
		items.Sort(CompareItemsForSort);
		foreach (CFGImageExtension icon in icons)
		{
			Object.Destroy(icon.transform.parent.gameObject);
		}
		icons.Clear();
		names.Clear();
		prices.Clear();
		count.Clear();
		btns.Clear();
		float num = (m_InventoryItem.GetComponent<RectTransform>().anchorMax.y - m_InventoryItem.GetComponent<RectTransform>().anchorMin.y) * base.transform.parent.GetComponent<RectTransform>().rect.height;
		RectTransform component = parent.GetComponent<RectTransform>();
		float num2 = 0.35f * (float)Screen.height / 900f;
		if (!offer)
		{
			num2 = 0.14f * (float)Screen.height / 900f;
		}
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, (!(1f >= num2 * (float)items.Count)) ? (num2 * (float)items.Count) : 1f);
		component.offsetMax = new Vector2(1f, 1f);
		component.offsetMin = new Vector2(0f, 0f);
		List<CFGStoreItem> list = new List<CFGStoreItem>();
		if (!offer)
		{
			foreach (CFGStoreItem item in items)
			{
				if (item.ItemCount - item.TransactionItemCount > 0)
				{
					list.Add(item);
				}
			}
			items = list;
		}
		else
		{
			list = items;
		}
		for (int i = 0; i < list.Count; i++)
		{
			int num3 = 0;
			num3 = (offer ? list[i].TransactionItemCount : (list[i].ItemCount - list[i].TransactionItemCount));
			GameObject gameObject = Object.Instantiate(m_InventoryItem);
			gameObject.name = "SPAWN item";
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.SetParent(parent.transform, worldPositionStays: false);
			component2.transform.localPosition = new Vector3(0f, 0f, 0f);
			float num4 = 1f - (float)i * (num / parent.GetComponent<RectTransform>().rect.height);
			float y = num4 - num / parent.GetComponent<RectTransform>().rect.height;
			component2.offsetMax = new Vector2(0f, 1f);
			component2.offsetMin = new Vector2(0f, 1f);
			component2.anchorMax = new Vector2(1f, num4);
			component2.anchorMin = new Vector2(0f, y);
			m_ItemHeight = component2.rect.height;
			CFGButtonExtension componentInChildren = gameObject.GetComponentInChildren<CFGButtonExtension>();
			btns.Add(componentInChildren);
			componentInChildren.m_ShopItem = list[i];
			componentInChildren.m_Data = i;
			componentInChildren.IsInUseMeState = offer;
			componentInChildren.IsDraggable = true;
			componentInChildren.m_DragParent = base.gameObject.transform;
			componentInChildren.m_ShouldDragSelectItem = true;
			if (offer)
			{
				if (player)
				{
					componentInChildren.m_OnDoubleClickCallback = OnDropInventoryPlayer;
					componentInChildren.m_OnRightClickCallback = OnDropInventoryPlayer;
					componentInChildren.IsSelected = false;
				}
				else
				{
					componentInChildren.m_OnDoubleClickCallback = OnDropInventoryNPC;
					componentInChildren.m_OnRightClickCallback = OnDropInventoryNPC;
					componentInChildren.IsSelected = false;
				}
			}
			else if (player)
			{
				componentInChildren.m_OnDoubleClickCallback = OnDropOfferPlayer;
				componentInChildren.m_OnRightClickCallback = OnDropOfferPlayer;
				componentInChildren.IsSelected = IsItemNew(list[i].ItemDef.ItemID);
			}
			else
			{
				componentInChildren.m_OnDoubleClickCallback = OnDropOfferNPC;
				componentInChildren.m_OnRightClickCallback = OnDropOfferNPC;
				componentInChildren.IsSelected = false;
			}
			Image[] componentsInChildren = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image in componentsInChildren)
			{
				if (image.name == "Image$")
				{
					image.gameObject.SetActive(list[i].ItemDef.ItemID != "cash");
				}
			}
			Image[] componentsInChildren2 = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image2 in componentsInChildren2)
			{
				if (image2.name == "ImageBGblack")
				{
					image2.enabled = i % 2 == 0;
				}
			}
			if (offer)
			{
				if (player)
				{
					componentInChildren.m_ButtonOverCallback = OnItemOfferPlayerHover;
					componentInChildren.m_ButtonClickedCallback = OnItemOfferPlayerClick;
				}
				else
				{
					componentInChildren.m_ButtonOverCallback = OnItemOfferNpcHover;
					componentInChildren.m_ButtonClickedCallback = OnItemOfferNpcClick;
				}
			}
			else if (player)
			{
				componentInChildren.m_ButtonOverCallback = OnItemPlayerHover;
				componentInChildren.m_ButtonClickedCallback = OnItemPlayerClick;
			}
			else
			{
				componentInChildren.m_ButtonOverCallback = OnItemNpcHover;
				componentInChildren.m_ButtonClickedCallback = OnItemNpcClick;
			}
			CFGImageExtension[] componentsInChildren3 = gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension in componentsInChildren3)
			{
				if (cFGImageExtension.name == "ImageITEM")
				{
					cFGImageExtension.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
					cFGImageExtension.IconNumber = list[i].ItemDef.ShopIcon;
					icons.Add(cFGImageExtension);
					componentInChildren.m_SpriteList = cFGImageExtension.m_SpriteList;
					componentInChildren.IconNumber = list[i].ItemDef.ShopIcon;
				}
				else if (cFGImageExtension.name == "Ikonka")
				{
					cFGImageExtension.IconNumber = ((list[i].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon) ? ((list[i].ItemDef.ItemType == CFGDef_Item.EItemType.Talisman) ? 2 : ((list[i].ItemDef.ItemType == CFGDef_Item.EItemType.Useable) ? 1 : 3)) : 0);
				}
			}
			Text[] componentsInChildren4 = gameObject.GetComponentsInChildren<Text>();
			foreach (Text text in componentsInChildren4)
			{
				if (text.name == "txtNazwa")
				{
					text.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(list[i].ItemDef.ItemID + "_name");
					names.Add(text);
				}
				else if (text.name == "txtIlosc")
				{
					string text2 = ((!(list[i].ItemDef.ItemID == "cash")) ? "x" : string.Empty);
					text.text = num3 + text2;
					count.Add(text);
				}
				else if (text.name == "txtCena")
				{
					if (list[i].ItemDef.ItemID == "cash")
					{
						text.text = string.Empty;
					}
					else
					{
						text.text = ((list[i].SellPrice < 1) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_itemprice_notapplicable") : list[i].SellPrice.ToString());
					}
					prices.Add(text);
				}
			}
		}
		m_NeedRefreshInfoFrame = true;
	}

	private int CompareItemsForSort(CFGStoreItem item1, CFGStoreItem item2)
	{
		if (item1.SellPrice > item2.SellPrice)
		{
			return 1;
		}
		if (item1.SellPrice < item2.SellPrice)
		{
			return -1;
		}
		return string.Compare(item1.ItemID, item2.ItemID);
	}

	public override void Update()
	{
		base.Update();
		if (m_NeedRefresh)
		{
			m_PrevScrollbars.Clear();
			for (int i = 0; i < m_Scrollbars.Count; i++)
			{
				m_PrevScrollbars.Add(m_Scrollbars[i].value);
			}
			SpawnItemList(ref m_NPCOffer, m_NPCOfferButtons, m_NPCOfferParent, m_NPCItemsOfferIcons, m_NPCItemsOfferNames, m_NPCItemsOfferPrices, m_NPCItemsOfferCounts, offer: true, player: false);
			SpawnItemList(ref m_NPCCurrentItems, m_NPCCurrentItemsButtons, m_NPCInventoryParent, m_NPCItemsIcons, m_NPCItemsNames, m_NPCItemsPrices, m_NPCItemsCounts, offer: false, player: false);
			SpawnItemList(ref m_PlayerOffer, m_PlayerOfferButtons, m_PlayerOfferParent, m_CharItemsOfferIcons, m_CharItemsOfferNames, m_CharItemsOfferPrices, m_CharItemsOfferCounts, offer: true, player: true);
			SpawnItemList(ref m_PlayerCurrentItems, m_PlayerCurrentItemsButtons, m_PlayerInventoryParent, m_CharItemsIcons, m_CharItemsNames, m_CharItemsPrices, m_CharItemsCounts, offer: false, player: true);
			m_NeedRefresh = false;
			for (int j = 0; j < m_Scrollbars.Count; j++)
			{
				m_Scrollbars[j].value = 0f;
				m_Scrollbars[j].value = (m_IsInitalized ? m_PrevScrollbars[j] : 1f);
			}
			m_IsInitalized = true;
			if (m_Controller_Main == null)
			{
			}
		}
		bool flag = CalculateOffer();
		m_TradeButton.enabled = flag;
		m_XButtonPad.enabled = flag;
		if (m_PlayerOffer.Count > 0 || m_NPCOffer.Count > 0)
		{
			m_CloseButton.m_ButtonClickedCallback = OnOpenPopup;
		}
		else
		{
			m_CloseButton.m_ButtonClickedCallback = OnExit;
		}
		int num = 0;
		if (m_SplitPopupPanel.activeSelf && m_CurrentSelectedButton != null)
		{
			num = ((!m_CurrentSelectedButton.IsInUseMeState) ? (num + m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice) : (num - m_SplitItemCurrent * m_CurrentSelectedItem.SellPrice));
		}
		m_PlayerSum.text = string.Empty + (OfferValuePlayer() + ((!CFGEconomy.m_ShopGoods.Contains(m_CurrentSelectedItem)) ? num : 0));
		m_NPCSum.text = string.Empty + (OfferValueNPC() + (CFGEconomy.m_ShopGoods.Contains(m_CurrentSelectedItem) ? num : 0));
		UpdateController();
		bool flag2 = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_SplitPopupPanel.activeSelf)
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis < 0f)
			{
				OnSplitMinus(5);
				m_SplitItemMinus.SimulateClickGraphicAndSoundOnly();
			}
			if (axis > 0f)
			{
				OnSplitPlus(5);
				m_SplitItemPlus.SimulateClickGraphicAndSoundOnly();
			}
		}
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_LBButtonPad.gameObject.SetActive(flag2 && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_RBButtonPad.gameObject.SetActive(flag2 && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LTButtonPad.gameObject.SetActive(flag2);
			m_RTButtonPad.gameObject.SetActive(flag2);
			m_BButtonPad.gameObject.SetActive(flag2);
			m_AButtonPad.gameObject.SetActive(flag2);
			m_XButtonPad.gameObject.SetActive(flag2);
			m_VerticalButtonPad.gameObject.SetActive(flag2);
			if (m_RButtonPad != null)
			{
				m_RButtonPad.gameObject.SetActive(flag2);
			}
			m_XButtonPadWarning.gameObject.SetActive(flag2);
			m_BButtonPadWarning.gameObject.SetActive(flag2);
			m_LTButtonPadSplit.gameObject.SetActive(flag2);
			m_RTButtonPadSplit.gameObject.SetActive(flag2);
			m_DLeftButtonPadSplit.gameObject.SetActive(flag2);
			m_DRightButtonPadSplit.gameObject.SetActive(flag2);
			m_BButtonPadSplit.gameObject.SetActive(flag2);
			m_AButtonPadSplit.gameObject.SetActive(flag2);
			m_XButtonPadSplit.gameObject.SetActive(flag2);
			m_TradeButton.gameObject.SetActive(!flag2);
			m_CloseButton.gameObject.SetActive(!flag2);
			m_PopupCancelBtn.gameObject.SetActive(!flag2);
			m_PopupCloseBtn.gameObject.SetActive(!flag2);
			m_SplitItemConfirm.gameObject.SetActive(!flag2);
			m_SplitItemClose.gameObject.SetActive(!flag2);
			m_AdjustFrame.gameObject.SetActive(flag2);
			m_SplitItemMin.gameObject.SetActive(!flag2);
			m_SplitItemMinus.gameObject.SetActive(!flag2);
			m_SplitItemMax.gameObject.SetActive(!flag2);
			m_SplitItemPlus.gameObject.SetActive(!flag2);
			m_BarterTradeText.gameObject.SetActive(!flag2);
			m_LastInput = CFGInput.LastReadInputDevice;
			if (m_LastInput != EInputMode.Gamepad)
			{
				SetMouseListHighlight();
			}
		}
		if (m_LastInput == EInputMode.Gamepad && m_Controller_Main.CurrentList > -1 && m_Controller_Main.CurrentListObject != null && m_Controller_Main.CurrentListObject.m_CurrentItem > -1 && (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0f))
		{
			float num2 = 0f;
			int num3 = 0;
			if (m_Controller_Main.CurrentList == 0)
			{
				num2 = m_ItemHeight / m_PlayerInventoryParent.GetComponent<RectTransform>().rect.height;
				num3 = m_PlayerCurrentItems.Count - 1;
			}
			else if (m_Controller_Main.CurrentList == 1)
			{
				num2 = m_ItemHeight / m_PlayerOfferParent.GetComponent<RectTransform>().rect.height;
				num3 = m_PlayerOffer.Count - 1;
			}
			else if (m_Controller_Main.CurrentList == 2)
			{
				num2 = m_ItemHeight / m_NPCOfferParent.GetComponent<RectTransform>().rect.height;
				num3 = m_NPCOffer.Count - 1;
			}
			else if (m_Controller_Main.CurrentList == 3)
			{
				num2 = m_ItemHeight / m_NPCInventoryParent.GetComponent<RectTransform>().rect.height;
				num3 = m_NPCCurrentItems.Count - 1;
			}
			m_Scrollbars[m_Controller_Main.CurrentList].value = 1f;
			m_Scrollbars[m_Controller_Main.CurrentList].value = 1f - (float)m_Controller_Main.CurrentListObject.m_CurrentItem * num2;
			if (m_Controller_Main.CurrentListObject.m_CurrentItem == num3)
			{
				m_Scrollbars[m_Controller_Main.CurrentList].value = 1f;
				m_Scrollbars[m_Controller_Main.CurrentList].value = 0f;
			}
		}
		if (m_NeedRefreshInfoFrame)
		{
			ShowInfo();
			m_ItemInfoPanel.SetActive(value: true);
			m_ItemInfoIcon.IconNumber = 0;
			m_ItemInfoCount.text = string.Empty;
			m_ItemInfoName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("barter_screen_no_item_selected");
			m_ItemInfoDescription.text = string.Empty;
			m_HeatInfo[0].transform.parent.gameObject.SetActive(value: false);
			m_ItemInfoStatsNames[0].text = string.Empty;
			m_ItemInfoStatsNames[1].text = string.Empty;
			m_ItemInfoStatsNames[2].text = string.Empty;
			m_ItemInfoStatsValues[0].text = string.Empty;
			m_ItemInfoStatsValues[1].text = string.Empty;
			m_ItemInfoStatsValues[2].text = string.Empty;
			m_NeedRefreshInfoFrame = false;
		}
		m_ItemInfoIcon.gameObject.SetActive(m_ItemInfoIcon.IconNumber != 0);
	}

	private int OfferValuePlayer()
	{
		int num = 0;
		foreach (CFGStoreItem item in m_PlayerOffer)
		{
			num += item.TransactionItemCount * item.SellPrice;
		}
		return num;
	}

	private int OfferValueNPC()
	{
		int num = 0;
		foreach (CFGStoreItem item in m_NPCOffer)
		{
			num += item.TransactionItemCount * item.SellPrice;
		}
		return num;
	}

	private bool CalculateOffer()
	{
		if (m_PlayerOffer.Count == 0 || m_NPCOffer.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (CFGStoreItem item in m_PlayerOffer)
		{
			num += item.TransactionItemCount * item.SellPrice;
		}
		int num2 = 0;
		foreach (CFGStoreItem item2 in m_NPCOffer)
		{
			num2 += item2.TransactionItemCount * item2.SellPrice;
		}
		if (num >= num2)
		{
			return true;
		}
		return false;
	}

	public void SetGamepadListHighlight(int nr)
	{
		foreach (CFGImageExtension item in m_PlayerItemsFrame)
		{
			item.IconNumber = ((nr == 0) ? 1 : 0);
		}
		foreach (CFGImageExtension item2 in m_PlayerOfferItemsFrame)
		{
			item2.IconNumber = ((nr == 1) ? 1 : 0);
		}
		foreach (CFGImageExtension item3 in m_NPCItemsFrame)
		{
			item3.IconNumber = ((nr == 3) ? 1 : 0);
		}
		foreach (CFGImageExtension item4 in m_NPCOfferItemsFrame)
		{
			item4.IconNumber = ((nr == 2) ? 1 : 0);
		}
		m_PlayerItemsFrameTxtW.gameObject.SetActive(nr != 0);
		m_NPCItemsFrameTxtW.gameObject.SetActive(nr != 3);
		m_PlayerOfferItemsFrameTxtW.gameObject.SetActive(nr != 1);
		m_NPCOfferItemsFrameTxtW.gameObject.SetActive(nr != 2);
		m_PlayerItemsFrameTxtG.gameObject.SetActive(nr == 0);
		m_NPCItemsFrameTxtG.gameObject.SetActive(nr == 3);
		m_PlayerOfferItemsFrameTxtG.gameObject.SetActive(nr == 1);
		m_NPCOfferItemsFrameTxtG.gameObject.SetActive(nr == 2);
		m_PlayerItemsFrameUp.IconNumber = ((nr == 0) ? 1 : 0);
		m_NPCItemsFrameUp.IconNumber = ((nr == 3) ? 1 : 0);
		m_PlayerOfferItemsFrameUp.IconNumber = ((nr == 1) ? 1 : 0);
		m_NPCOfferItemsFrameUp.IconNumber = ((nr == 2) ? 1 : 0);
		foreach (CFGImageExtension item5 in m_FrameInfo)
		{
			item5.IconNumber = 0;
		}
		m_FrameTxtG.gameObject.SetActive(value: false);
		m_FrameTxtW.gameObject.SetActive(value: true);
	}

	public void SetMouseListHighlight()
	{
		foreach (CFGImageExtension item in m_PlayerItemsFrame)
		{
			item.IconNumber = 0;
		}
		foreach (CFGImageExtension item2 in m_PlayerOfferItemsFrame)
		{
			item2.IconNumber = 0;
		}
		foreach (CFGImageExtension item3 in m_NPCItemsFrame)
		{
			item3.IconNumber = 0;
		}
		foreach (CFGImageExtension item4 in m_NPCOfferItemsFrame)
		{
			item4.IconNumber = 0;
		}
		m_PlayerItemsFrameTxtW.gameObject.SetActive(value: false);
		m_NPCItemsFrameTxtW.gameObject.SetActive(value: false);
		m_PlayerOfferItemsFrameTxtW.gameObject.SetActive(value: true);
		m_NPCOfferItemsFrameTxtW.gameObject.SetActive(value: true);
		m_PlayerItemsFrameTxtG.gameObject.SetActive(value: true);
		m_NPCItemsFrameTxtG.gameObject.SetActive(value: true);
		m_PlayerOfferItemsFrameTxtG.gameObject.SetActive(value: false);
		m_NPCOfferItemsFrameTxtG.gameObject.SetActive(value: false);
		m_PlayerItemsFrameUp.IconNumber = 1;
		m_NPCItemsFrameUp.IconNumber = 1;
		m_PlayerOfferItemsFrameUp.IconNumber = 0;
		m_NPCOfferItemsFrameUp.IconNumber = 0;
		foreach (CFGImageExtension item5 in m_FrameInfo)
		{
			item5.IconNumber = 0;
		}
		m_FrameTxtG.gameObject.SetActive(value: true);
		m_FrameTxtW.gameObject.SetActive(value: false);
	}

	private void InitControllerSupport()
	{
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_Controller_PlayerChars.AddList(m_CharacterButtons, OnController_PlayerCharSelChange, null);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_UP_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_Down_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_UP_Digital = EJoyButton.LeftBumper;
			cFGJoyMenuButtonList.m_Down_Digital = EJoyButton.RightBumper;
		}
		cFGJoyMenuButtonList = m_Controller_PlayerCategory.AddList(m_CharacterInventoryButtons, OnController_PlayerCategorySelChange, null);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_UP_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_Down_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_UP_Digital = EJoyButton.DPad_Top;
			cFGJoyMenuButtonList.m_Down_Digital = EJoyButton.DPad_Bottom;
		}
		InitMainControllerList(m_Controller_Main.AddList(m_PlayerCurrentItemsButtons, OnMoveList, OnController_MovePlayerItem));
		InitMainControllerList(m_Controller_Main.AddList(m_PlayerOfferButtons, OnMoveList, OnController_MovePlayerItem));
		InitMainControllerList(m_Controller_Main.AddList(m_NPCOfferButtons, OnMoveList, OnController_MovePlayerItem));
		InitMainControllerList(m_Controller_Main.AddList(m_NPCCurrentItemsButtons, OnMoveList, OnController_MovePlayerItem));
		m_Controller_Main.SetListChangeButtons(EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.LA_Left, EJoyButton.LA_Right, EJoyButton.LeftTrigger, EJoyButton.RightTrigger);
	}

	private void InitMainControllerList(CFGJoyMenuButtonList list)
	{
		if (list != null)
		{
			list.m_UP_Analog = EJoyButton.LA_Top;
			list.m_UP_Digital = EJoyButton.Unknown;
			list.m_Down_Digital = EJoyButton.Unknown;
			list.m_Down_Analog = EJoyButton.LA_Bottom;
		}
	}

	private void OnMoveList(int item)
	{
		CFGJoyMenuButtonList currentListObject = m_Controller_Main.CurrentListObject;
		if (currentListObject != null)
		{
			if (currentListObject.m_Buttons[item].m_ButtonClickedCallback != null)
			{
				currentListObject.m_Buttons[item].m_ButtonClickedCallback(item);
			}
			currentListObject.m_Buttons[item].OnPointerEnter(null);
		}
	}

	private void OnController_ListChanged(int OldListID, int NewListID, bool bPrev)
	{
	}

	private void OnController_MovePlayerItem(int a, bool bSecondary)
	{
		if (!bSecondary)
		{
			OnMoveClick(0);
			m_AButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	private void OnController_NPCCategorySelChange(int a)
	{
		m_NPCInventoryButtons[a].m_ButtonClickedCallback(a);
		if (m_RButtonPad != null)
		{
			m_RButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	private void OnController_PlayerCategorySelChange(int a)
	{
		m_CharacterInventoryButtons[a].m_ButtonClickedCallback(a);
		m_VerticalButtonPad.SimulateClickGraphicAndSoundOnly();
		m_NPCInventoryButtons[a].m_ButtonClickedCallback(a);
		if (m_RButtonPad != null)
		{
			m_RButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	private void OnController_PlayerCharSelChange(int a)
	{
		int num = -1;
		for (int i = 0; i < m_CharacterButtons.Count; i++)
		{
			if (m_CharacterButtons[i].IsSelected)
			{
				num = i;
			}
		}
		if (m_CharacterButtons[a].m_ButtonClickedCallback != null)
		{
			m_CharacterButtons[a].m_ButtonClickedCallback(a);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
		{
			m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
		{
			m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	private void SimulateClickDelayedConfirmSplit()
	{
		m_SplitItemConfirm.m_ButtonClickedCallback(0);
	}

	private void SimulateClickDelayedCloseSplit()
	{
		m_SplitItemClose.m_ButtonClickedCallback(0);
	}

	private void SimulateClickDelayedClose()
	{
		m_CloseButton.m_ButtonClickedCallback(0);
	}

	private void SimulateClickDelayedPopupClose()
	{
		m_PopupCloseBtn.m_ButtonClickedCallback(0);
	}

	private void SimulateClickDelayedPopupCancel()
	{
		m_PopupCancelBtn.m_ButtonClickedCallback(0);
	}

	private void UpdateController()
	{
		if (CFGButtonExtension.IsWaitingForClick)
		{
			return;
		}
		if ((bool)m_ClosePopup && m_ClosePopup.activeSelf)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
			{
				Invoke("SimulateClickDelayedPopupCancel", 0.5f);
				m_BButtonPadWarning.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				Invoke("SimulateClickDelayedPopupClose", 0.5f);
				m_XButtonPadWarning.SimulateClickGraphicAndSoundOnly();
			}
			return;
		}
		if (m_SplitPopupPanel.activeSelf)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
			{
				Invoke("SimulateClickDelayedCloseSplit", 0.5f);
				m_BButtonPadSplit.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				Invoke("SimulateClickDelayedConfirmSplit", 0.5f);
				m_AButtonPadSplit.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LeftTrigger) > 0.4f && Time.time > m_NextReadAnalog)
			{
				m_SplitItemMin.m_ButtonClickedCallback(0);
				m_LTButtonPadSplit.SimulateClickGraphicAndSoundOnly();
				m_NextReadAnalog = Time.time + 0.4f;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightTrigger) > 0.4f && Time.time > m_NextReadAnalog)
			{
				m_SplitItemMax.m_ButtonClickedCallback(0);
				m_RTButtonPadSplit.SimulateClickGraphicAndSoundOnly();
				m_NextReadAnalog = Time.time + 0.4f;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left) > 0f)
			{
				OnSplitMinus(1);
				m_DLeftButtonPadSplit.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right) > 0f)
			{
				OnSplitPlus(1);
				m_DRightButtonPadSplit.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
			{
				m_SplitItemAdjust.m_ButtonClickedCallback(0);
				m_XButtonPadSplit.SimulateClickGraphicAndSoundOnly();
			}
			float num = CFGJoyManager.ReadAsButton(EJoyButton.LA_Left);
			if (num < 0.2f)
			{
				num = 0f;
			}
			float num2 = CFGJoyManager.ReadAsButton(EJoyButton.LA_Right);
			if (num2 < 0.2f)
			{
				num2 = 0f;
			}
			num2 -= num;
			num2 *= 5f;
			if (num2 != 0f)
			{
				int delta = (int)num2;
				Split_Mod(delta);
			}
			return;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
		{
			Invoke("SimulateClickDelayedClose", 0.5f);
			m_BButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
		{
			if (m_XButtonPad.enabled)
			{
				m_TradeButton.m_ButtonClickedCallback(0);
				m_XButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.RightTrigger) > 0.5f)
		{
			m_RTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		else if (CFGJoyManager.ReadAsButton(EJoyButton.LeftTrigger) > 0.5f)
		{
			m_LTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		if (m_bIsOnPlayerInventory)
		{
			m_Controller_PlayerCategory.UpdateInput();
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
			{
				m_bIsOnPlayerInventory = false;
				m_CharacterButtons[0].m_ButtonClickedCallback(0);
				m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
				m_Controller_PlayerCategory.DeactivateList(0);
			}
			else if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
			{
				m_bIsOnPlayerInventory = false;
				if (m_CharacterButtons[m_CharacterButtons.Count - 1].m_ButtonClickedCallback != null)
				{
					m_CharacterButtons[m_CharacterButtons.Count - 1].m_ButtonClickedCallback(m_CharacterButtons.Count - 1);
				}
				m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
				m_Controller_PlayerCategory.DeactivateList(0);
			}
		}
		else
		{
			m_Controller_PlayerChars.UpdateInput();
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Bottom) > 0f)
			{
				m_CharacterInventoryButtons[0].m_ButtonClickedCallback(0);
				m_VerticalButtonPad.SimulateClickGraphicAndSoundOnly();
				m_bIsOnPlayerInventory = true;
				m_Controller_PlayerChars.DeactivateList(0);
			}
			else if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Top) > 0f)
			{
				m_bIsOnPlayerInventory = true;
				m_Controller_PlayerChars.DeactivateList(0);
				m_CharacterInventoryButtons[0].m_ButtonClickedCallback(0);
				m_VerticalButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		m_Controller_Main.UpdateInput();
		if (m_Controller_Main.CurrentList != m_LastListNr)
		{
			SetGamepadListHighlight(m_Controller_Main.CurrentList);
			m_LastListNr = m_Controller_Main.CurrentList;
		}
		m_Controller_ShopkeepCategory.UpdateInput();
	}
}
