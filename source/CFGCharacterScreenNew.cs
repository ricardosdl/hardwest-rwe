using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGCharacterScreenNew : CFGPanel
{
	public List<CFGImageExtension> m_SlotsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_PlayersItemsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CardsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_BuffsFrame = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_InfoFrame = new List<CFGImageExtension>();

	public CFGImageExtension m_PlayersItemsFrameUp;

	public CFGImageExtension m_CardsFrameUp;

	public CFGImageExtension m_BuffsFrameUp;

	public CFGImageExtension m_InfoFrameUp;

	public Text m_PlayersItemsFrameW;

	public Text m_CardsFrameW;

	public Text m_BuffsFrameW;

	public Text m_InfoFrameW;

	public Text m_PlayersItemsFrameG;

	public Text m_CardsFrameG;

	public Text m_BuffsFrameG;

	public Text m_InfoFrameG;

	public Text m_PanelTitle;

	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_XButtonPad;

	public CFGButtonExtension m_VerticalButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_YButtonPad;

	public CFGButtonExtension m_RTButtonPad;

	public CFGButtonExtension m_LTButtonPad;

	public CFGButtonExtension m_RBButtonPad;

	public CFGButtonExtension m_LBButtonPad;

	public List<CFGButtonExtension> m_Characters = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_InventoryFilterButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_SlotButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_InventoryButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_CardButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_BuffButtons = new List<CFGButtonExtension>();

	public CFGButtonExtension m_NextButton;

	public CFGButtonExtension m_PrevButton;

	public CFGImageExtension m_Avatar;

	public Text m_AvatarName;

	public CFGButtonExtension m_ExitButton;

	public CFGButtonExtension m_CardPanelButton;

	public CFGButtonExtension m_CardPanelUpperButton;

	public CFGMaskedProgressBar m_Luck;

	public CFGMaskedProgressBar m_HP;

	public Text m_Movement;

	public Text m_Aim;

	public Text m_Defense;

	public Text m_Sight;

	public Text m_MovementVal;

	public Text m_AimVal;

	public Text m_DefenseVal;

	public Text m_SightVal;

	public Text m_LuckVal;

	public Text m_HPVal;

	public CFGImageExtension m_MovementIco;

	public CFGImageExtension m_AimIco;

	public CFGImageExtension m_DefenseIco;

	public CFGImageExtension m_SightIco;

	public Text m_TooltipBigName;

	public Text m_TooltipBigDesc;

	public List<Text> m_TooltipBigParamsNames = new List<Text>();

	public List<Text> m_TooltipBigParamsValues = new List<Text>();

	public CFGImageExtension m_TooltipImageBig;

	public CFGImageExtension m_TooltipImageCardBig;

	public CFGImageExtension m_TooltipImageBuffBig;

	public CFGImageExtension m_TooltipCharacterBig;

	private CFGCharacterData m_CurrentChar;

	private List<CFGCharacterData> m_CharacterDataList = new List<CFGCharacterData>();

	private int m_CurrentCategory;

	private bool m_bCardPanelWasActive;

	public GameObject m_BuffElement;

	public GameObject m_BuffsParent;

	public List<GameObject> m_BuffsList = new List<GameObject>();

	public Scrollbar m_Scrollbar;

	public Scrollbar m_ScrollbarBuff;

	public GameObject m_PlayerInventoryParent;

	private List<CFGItem> m_PlayerCurrentItems = new List<CFGItem>();

	private List<CFGBuff> m_PlayerCurrentBuffs = new List<CFGBuff>();

	public GameObject m_InventoryItem;

	private CFGItem m_ItemToEquip;

	private int m_SlotDragged = -1;

	private bool m_CursorOverSlot;

	public CFGButtonExtension m_CardsSetBtn;

	public CFGImageExtension m_CardsSetImgNormal;

	public bool m_CombatLoadout;

	public CFGSequencer m_Sequencer;

	public Scrollbar m_ScrollbarBuffs;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private int m_LastListNr = -1;

	private CFGJoyMenuController m_ControllerInput = new CFGJoyMenuController();

	private CFGJoyMenuController m_ControllerInput_Boons = new CFGJoyMenuController();

	private List<CFGButtonExtension> m_ControllerInput_Card1 = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_ControllerInput_Card2 = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_ControllerInput_Card3 = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_ControllerInput_Card4 = new List<CFGButtonExtension>();

	private List<CFGButtonExtension> m_ControllerInput_Card5 = new List<CFGButtonExtension>();

	private Vector3 m_YPosBtn = default(Vector3);

	private float m_ItemHeight;

	public List<GameObject> m_Heat = new List<GameObject>();

	public List<GameObject> m_HeatInfo = new List<GameObject>();

	public Text m_HeatText;

	public Text m_HeatText2;

	public CFGImageExtension m_CardScreenBtnAnim;

	private bool m_Init;

	private bool m_InitFirstCharacter;

	protected override void Start()
	{
		base.Start();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ExitButton.gameObject.SetActive(!flag);
			m_CardPanelUpperButton.gameObject.SetActive(!flag);
			m_AButtonPad.gameObject.SetActive(flag);
			m_XButtonPad.gameObject.SetActive(flag);
			m_VerticalButtonPad.gameObject.SetActive(flag);
			m_BButtonPad.gameObject.SetActive(flag);
			m_YButtonPad.gameObject.SetActive(flag);
			m_RTButtonPad.gameObject.SetActive(flag);
			m_LTButtonPad.gameObject.SetActive(flag);
			m_RBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LastInput = CFGInput.LastReadInputDevice;
			if (m_LastInput != EInputMode.Gamepad)
			{
				SetMouseListHighlight();
			}
			else
			{
				SetGamepadListHighlight(m_ControllerInput.CurrentList);
			}
		}
		m_YPosBtn = m_RBButtonPad.transform.position;
		m_CharacterDataList = CFGCharacterList.GetTeamCharactersList();
		if (m_CharacterDataList.Count > 0)
		{
			m_CurrentChar = m_CharacterDataList[0];
		}
		m_Characters[0].IsSelected = true;
		m_ExitButton.m_ButtonClickedCallback = OnExit;
		m_CardsSetImgNormal.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
		m_CardsSetBtn.m_ButtonOverCallback = OnCardsSetIn;
		foreach (CFGButtonExtension slotButton in m_SlotButtons)
		{
			slotButton.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
			slotButton.m_ButtonClickedCallback = OnSlotClick;
			slotButton.m_ButtonDropCallback = OnReleaseOnSlot;
			slotButton.m_ButtonOverCallback = OnSlotIn;
			slotButton.m_ButtonOutCallback = OnSlotOut;
			slotButton.m_ButtonDragCallback = OnSlotDrag;
		}
		m_TooltipImageBig.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		for (int i = 0; i < m_Characters.Count; i++)
		{
			m_Characters[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
			m_Characters[i].gameObject.transform.parent.gameObject.SetActive(m_CharacterDataList.Count > i);
			if (m_CharacterDataList.Count > i)
			{
				m_Characters[i].IconNumber = m_CharacterDataList[i].ImageIDX;
			}
			m_Characters[i].m_ButtonClickedCallback = OnCharacterButtonClick;
			m_Characters[i].m_ButtonOverCallback = OnCharacterIn;
			m_Characters[i].m_ButtonOutCallback = OnCharacterOut;
		}
		m_TooltipCharacterBig.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		foreach (CFGButtonExtension cardButton in m_CardButtons)
		{
			cardButton.m_ButtonClickedCallback = OnCardPanelButton;
			cardButton.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
			cardButton.m_ButtonOverCallback = OnCardIn;
			cardButton.m_ButtonOutCallback = OnCardOut;
		}
		m_TooltipImageCardBig.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
		m_CardPanelUpperButton.m_ButtonClickedCallback = OnCardPanelButton;
		m_CardPanelButton.m_ButtonClickedCallback = OnCardPanelButton;
		m_Avatar.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_InventoryFilterButtons[0].m_ButtonClickedCallback = OnInventoryClick;
		m_InventoryFilterButtons[1].m_ButtonClickedCallback = OnWeaponsClick;
		m_InventoryFilterButtons[2].m_ButtonClickedCallback = OnUsableClick;
		m_InventoryFilterButtons[3].m_ButtonClickedCallback = OnTalismanClick;
		m_InventoryFilterButtons[4].m_ButtonClickedCallback = OnNonUsableClick;
		m_InventoryFilterButtons[0].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_all");
		m_InventoryFilterButtons[1].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_weapons");
		m_InventoryFilterButtons[2].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_usables");
		m_InventoryFilterButtons[3].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_talismans");
		m_InventoryFilterButtons[4].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("str_category_misc");
		m_InventoryFilterButtons[0].IsSelected = true;
		OnCharacterIn(0);
		m_NextButton.gameObject.SetActive(m_CharacterDataList.Count > 1);
		m_PrevButton.gameObject.SetActive(m_CharacterDataList.Count > 1);
		m_NextButton.m_ButtonClickedCallback = NextCharacter;
		m_PrevButton.m_ButtonClickedCallback = PrevCharacter;
		CFGButtonExtension component = m_PlayerInventoryParent.GetComponent<CFGButtonExtension>();
		component.m_ButtonDropCallback = OnInvDrop;
		m_TooltipImageBuffBig.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_CharacterScreen, bEnable: true);
		m_ControllerInput.AddList(m_SlotButtons, null, OnController_SlotButton);
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_ControllerInput.AddList(m_InventoryButtons, null, OnController_InventoryButton);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_bAllowEmpty = true;
			cFGJoyMenuButtonList.m_Down_Digital = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_UP_Digital = EJoyButton.Unknown;
		}
		if (m_CardButtons.Count > 4)
		{
			m_ControllerInput_Card1.Add(m_CardButtons[0]);
		}
		if (m_CardButtons.Count > 4)
		{
			m_ControllerInput_Card2.Add(m_CardButtons[1]);
		}
		if (m_CardButtons.Count > 4)
		{
			m_ControllerInput_Card3.Add(m_CardButtons[2]);
		}
		if (m_CardButtons.Count > 4)
		{
			m_ControllerInput_Card4.Add(m_CardButtons[3]);
		}
		if (m_CardButtons.Count > 4)
		{
			m_ControllerInput_Card5.Add(m_CardButtons[4]);
		}
		AddCardList(m_ControllerInput_Card1);
		AddCardList(m_ControllerInput_Card2);
		AddCardList(m_ControllerInput_Card3);
		AddCardList(m_ControllerInput_Card4);
		AddCardList(m_ControllerInput_Card5);
		m_ControllerInput.SetListChangeButtons(EJoyButton.Unknown, EJoyButton.Unknown, EJoyButton.LeftTrigger, EJoyButton.RightTrigger, EJoyButton.LA_Left, EJoyButton.LA_Right);
		m_ControllerInput.m_CB_OnListChanged = OnController_ListChanged;
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component2 = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component2)
		{
			component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component2.rect.width);
			component2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component2.rect.height);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		m_CardScreenBtnAnim.m_OnAnimateCallback = HideAnim;
		bool active = false;
		foreach (CCardStatus value in CFGInventory.CollectedCards.Values)
		{
			if (value.IsNew)
			{
				active = true;
			}
		}
		m_CardScreenBtnAnim.gameObject.SetActive(active);
	}

	public void HideAnim(string a, int b)
	{
		bool active = false;
		foreach (CCardStatus value in CFGInventory.CollectedCards.Values)
		{
			if (value.IsNew)
			{
				active = true;
			}
		}
		m_CardScreenBtnAnim.gameObject.SetActive(active);
	}

	private void AddCardList(List<CFGButtonExtension> CList)
	{
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_ControllerInput.AddList(CList, OnController_CardSelect, OnController_CardButton);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_NaviMode = CFGJoyMenuButtonList.EListNavigationMode.LeftRight;
			cFGJoyMenuButtonList.m_Down_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_UP_Analog = EJoyButton.Unknown;
			cFGJoyMenuButtonList.m_bAllowEmpty = true;
		}
	}

	public void OnInvDrop(int a)
	{
		if (m_SlotDragged == -1)
		{
			return;
		}
		CFGDef_Item charItem = GetCharItem(m_SlotDragged);
		if (charItem == null)
		{
			if (m_ItemToEquip == null)
			{
				return;
			}
		}
		else
		{
			CFGInventory.AddItem(charItem.ItemID, 1, SetAsNew: false);
			SetCharItem(m_SlotDragged, null);
		}
		SetCurrentCharacter(from_pad: false);
		m_SlotDragged = -1;
	}

	public void OnSlotDrag(int nr)
	{
		m_SlotDragged = nr;
	}

	public override void SetLocalisation()
	{
		m_Aim.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("aim_tactical_details");
		m_Defense.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("defense_tactical_details");
		m_Movement.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("movement_tactical_details");
		m_Sight.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("sight_tactical_details");
		m_BuffsFrameG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("buffs_strategic_details");
		m_BuffsFrameW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("buffs_strategic_details");
		m_InfoFrameG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("info_strategic_details");
		m_InfoFrameW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("info_strategic_details");
		m_CardsFrameG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cards_strategic_details");
		m_CardsFrameW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cards_strategic_details");
		m_PlayersItemsFrameW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("inventory_strategic_details");
		m_PlayersItemsFrameG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("inventory_strategic_details");
		m_HeatText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_heat");
		m_HeatText2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_heat");
		m_MovementIco.IconNumber = 2;
		m_AimIco.IconNumber = 0;
		m_DefenseIco.IconNumber = 1;
		m_SightIco.IconNumber = 3;
		if (!m_CombatLoadout)
		{
			m_PanelTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_title");
		}
		else
		{
			m_PanelTitle.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("char_screen_loadout_header");
		}
		if (!m_CombatLoadout)
		{
			m_ExitButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_close");
			m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_close");
		}
		else
		{
			m_ExitButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("char_screen_loadout_close");
			m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("char_screen_loadout_close");
		}
		m_CardPanelUpperButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_opencardscreen");
		m_CardPanelUpperButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_opencardscreen");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_assignitem_slot1");
		m_XButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_assignitem_slot2");
		m_VerticalButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_changecategory");
		m_YButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_entercardscreen");
	}

	public void NextCharacter(int a)
	{
		int num = -1;
		for (int i = 0; i < m_CharacterDataList.Count; i++)
		{
			CFGCharacterData cFGCharacterData = m_CharacterDataList[i];
			if (cFGCharacterData != null && cFGCharacterData == m_CurrentChar)
			{
				num = i + 1;
				break;
			}
		}
		if (m_CharacterDataList.Count <= num)
		{
			num = 0;
		}
		if (m_CharacterDataList[num] == null)
		{
			return;
		}
		m_CurrentChar = m_CharacterDataList[num];
		SetCurrentCharacter(from_pad: true);
		OnCharacterIn(num);
		foreach (CFGButtonExtension character in m_Characters)
		{
			character.IsSelected = false;
		}
		m_Characters[num].IsSelected = true;
	}

	public void PrevCharacter(int a)
	{
		int num = -1;
		for (int i = 0; i < m_CharacterDataList.Count; i++)
		{
			CFGCharacterData cFGCharacterData = m_CharacterDataList[i];
			if (cFGCharacterData != null && cFGCharacterData == m_CurrentChar)
			{
				num = i - 1;
				break;
			}
		}
		if (num < 0)
		{
			num = m_CharacterDataList.Count - 1;
		}
		if (m_CharacterDataList[num] == null)
		{
			return;
		}
		m_CurrentChar = m_CharacterDataList[num];
		SetCurrentCharacter(from_pad: true);
		OnCharacterIn(num);
		foreach (CFGButtonExtension character in m_Characters)
		{
			character.IsSelected = false;
		}
		m_Characters[num].IsSelected = true;
	}

	public void OnWeaponsClick(int nr)
	{
		m_CurrentCategory = 1;
		foreach (CFGButtonExtension inventoryFilterButton in m_InventoryFilterButtons)
		{
			inventoryFilterButton.IsSelected = false;
		}
		m_InventoryFilterButtons[nr].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemDef != null && backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.Weapon)
			{
				m_PlayerCurrentItems.Add(backpackItem);
			}
		}
		SpawnItemList(m_PlayerCurrentItems, m_InventoryButtons, m_PlayerInventoryParent);
	}

	public void OnUsableClick(int nr)
	{
		m_CurrentCategory = 2;
		foreach (CFGButtonExtension inventoryFilterButton in m_InventoryFilterButtons)
		{
			inventoryFilterButton.IsSelected = false;
		}
		m_InventoryFilterButtons[nr].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemDef != null && backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.Useable)
			{
				m_PlayerCurrentItems.Add(backpackItem);
			}
		}
		SpawnItemList(m_PlayerCurrentItems, m_InventoryButtons, m_PlayerInventoryParent);
	}

	public void OnTalismanClick(int nr)
	{
		m_CurrentCategory = 3;
		foreach (CFGButtonExtension inventoryFilterButton in m_InventoryFilterButtons)
		{
			inventoryFilterButton.IsSelected = false;
		}
		m_InventoryFilterButtons[nr].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemDef != null && backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.Talisman)
			{
				m_PlayerCurrentItems.Add(backpackItem);
			}
		}
		SpawnItemList(m_PlayerCurrentItems, m_InventoryButtons, m_PlayerInventoryParent);
	}

	public void OnNonUsableClick(int nr)
	{
		m_CurrentCategory = 4;
		foreach (CFGButtonExtension inventoryFilterButton in m_InventoryFilterButtons)
		{
			inventoryFilterButton.IsSelected = false;
		}
		m_InventoryFilterButtons[nr].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			if (backpackItem.ItemDef != null && (backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.TradeGood || backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.QuestItem || backpackItem.ItemDef.ItemType == CFGDef_Item.EItemType.Unknown))
			{
				m_PlayerCurrentItems.Add(backpackItem);
			}
		}
		SpawnItemList(m_PlayerCurrentItems, m_InventoryButtons, m_PlayerInventoryParent);
	}

	public void OnInventoryClick(int nr)
	{
		m_CurrentCategory = 0;
		foreach (CFGButtonExtension inventoryFilterButton in m_InventoryFilterButtons)
		{
			inventoryFilterButton.IsSelected = false;
		}
		m_InventoryFilterButtons[nr].IsSelected = true;
		m_PlayerCurrentItems.Clear();
		foreach (CFGItem backpackItem in CFGInventory.BackpackItems)
		{
			m_PlayerCurrentItems.Add(backpackItem);
		}
		SpawnItemList(m_PlayerCurrentItems, m_InventoryButtons, m_PlayerInventoryParent);
	}

	public void OnCharacterButtonClick(int num)
	{
		m_CurrentChar = m_CharacterDataList[num];
		foreach (CFGButtonExtension character in m_Characters)
		{
			character.IsSelected = false;
		}
		m_Characters[num].IsSelected = true;
		SetCurrentCharacter(from_pad: false);
	}

	public override void Update()
	{
		base.Update();
		if (!m_InitFirstCharacter)
		{
			SetCurrentCharacter(from_pad: false);
			m_InitFirstCharacter = true;
			return;
		}
		if (m_CurrentChar != null)
		{
			m_Luck.SetProgress(m_CurrentChar.Luck * 100 / m_CurrentChar.MaxLuck);
			m_HP.SetProgress(100);
			for (int i = 0; i < m_HeatInfo.Count; i++)
			{
				m_HeatInfo[i].SetActive(i < m_CurrentChar.TotalHeat);
			}
		}
		if (Input.GetMouseButtonUp(0) && m_ItemToEquip != null && !m_CursorOverSlot)
		{
			m_ItemToEquip = null;
			OnRealeseOutside(0);
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null)
		{
			UpdateController();
		}
		if (CFGInput.IsActivated(EActionCommand.ToggleCharacterInfo))
		{
			m_ExitButton.SimulateClick(bDelayed: true);
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (flag && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CardsPanel == null && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f && m_AButtonPad.enabled)
			{
				if (m_ControllerInput.CurrentList != 1)
				{
					m_AButtonPad.SimulateClickGraphicAndSoundOnlyDisable();
				}
				else
				{
					m_AButtonPad.SimulateClickGraphicAndSoundOnly();
				}
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f && m_XButtonPad.enabled)
			{
				m_XButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (m_ControllerInput.CurrentList == 1 && m_VerticalButtonPad.enabled && (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Bottom) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.DPad_Top) > 0f))
			{
				m_VerticalButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (m_ControllerInput.CurrentList < 2 && CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
			{
				m_BButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (m_ControllerInput.CurrentList < 2 && CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f)
			{
				m_YButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
			{
				m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
			{
				m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		if (flag)
		{
			float width = m_Characters[0].gameObject.GetComponent<RectTransform>().rect.width;
			m_RBButtonPad.transform.position = new Vector3(m_YPosBtn.x - (float)(6 - m_CharacterDataList.Count) * width * 1.2f, m_RBButtonPad.transform.position.y, m_RBButtonPad.transform.position.z);
		}
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ExitButton.gameObject.SetActive(!flag);
			m_CardPanelUpperButton.gameObject.SetActive(!flag);
			m_AButtonPad.gameObject.SetActive(flag);
			m_XButtonPad.gameObject.SetActive(flag);
			m_VerticalButtonPad.gameObject.SetActive(flag);
			m_BButtonPad.gameObject.SetActive(flag);
			m_YButtonPad.gameObject.SetActive(flag);
			m_RTButtonPad.gameObject.SetActive(flag);
			m_LTButtonPad.gameObject.SetActive(flag);
			m_RBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (m_LastInput == EInputMode.Gamepad && m_ControllerInput.CurrentList == 1 && m_ControllerInput.CurrentListObject != null && ((m_ControllerInput.CurrentListObject.m_CurrentItem > -1 && CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0f) || (m_ControllerInput.CurrentListObject.m_CurrentItem > -1 && CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0f)))
		{
			float num = m_ItemHeight / m_PlayerInventoryParent.GetComponent<RectTransform>().rect.height;
			m_Scrollbar.value = 1f;
			m_Scrollbar.value = 1f - (float)m_ControllerInput.CurrentListObject.m_CurrentItem * num;
			if (m_ControllerInput.CurrentListObject.m_CurrentItem == m_PlayerCurrentItems.Count - 1)
			{
				m_Scrollbar.value = 1f;
				m_Scrollbar.value = 0f;
			}
		}
		if (m_LastInput == EInputMode.Gamepad && m_ControllerInput.CurrentList != m_LastListNr && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CardsPanel == null && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null)
		{
			SetGamepadListHighlight(m_ControllerInput.CurrentList);
			m_XButtonPad.enabled = m_ControllerInput.CurrentList == 1;
			m_VerticalButtonPad.enabled = m_ControllerInput.CurrentList == 1;
			m_LastListNr = m_ControllerInput.CurrentList;
		}
		bool flag2 = m_LastInput == EInputMode.Gamepad && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_CardsPanel == null && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null && (m_ControllerInput.CurrentList == 1 || (m_ControllerInput.CurrentList == 0 && m_ControllerInput.CurrentListObject != null && m_ControllerInput.CurrentListObject.m_CurrentItem >= 0 && m_SlotButtons[m_ControllerInput.CurrentListObject.m_CurrentItem].m_Icon.gameObject.activeSelf));
		if (flag2 != m_AButtonPad.enabled)
		{
			m_AButtonPad.enabled = flag2;
			if (m_ControllerInput.CurrentListObject == null)
			{
			}
		}
		if (m_LastInput != EInputMode.Gamepad)
		{
			SetMouseListHighlight();
		}
		else
		{
			SetGamepadListHighlight(m_ControllerInput.CurrentList);
		}
		int num2 = -1;
		for (int j = 0; j < m_InventoryFilterButtons.Count; j++)
		{
			if (m_InventoryFilterButtons[j].IsSelected)
			{
				num2 = j;
			}
		}
		if (m_InventoryFilterButtons[m_CurrentCategory].m_ButtonClickedCallback != null && num2 > -1 && m_CurrentCategory > -1 && m_CurrentCategory != num2)
		{
			m_InventoryFilterButtons[num2].m_ButtonClickedCallback(num2);
		}
		float num3 = CFGJoyManager.ReadAsButton(EJoyButton.RA_Left);
		if (num3 < 0.3f)
		{
			num3 = 0f;
		}
		float num4 = CFGJoyManager.ReadAsButton(EJoyButton.RA_Right);
		if (num4 < 0.3f)
		{
			num4 = 0f;
		}
		if (num3 > 0f)
		{
			float value = m_ScrollbarBuffs.value;
			m_ScrollbarBuffs.value = 0f;
			m_ScrollbarBuffs.value = value - 0.1f;
		}
		else if (num4 > 0f)
		{
			float value2 = m_ScrollbarBuffs.value;
			m_ScrollbarBuffs.value = 0f;
			m_ScrollbarBuffs.value = value2 + 0.1f;
		}
	}

	private void SetMouseListHighlight()
	{
		foreach (CFGImageExtension item in m_SlotsFrame)
		{
			item.IconNumber = 0;
		}
		foreach (CFGImageExtension item2 in m_PlayersItemsFrame)
		{
			item2.IconNumber = 0;
		}
		foreach (CFGImageExtension item3 in m_CardsFrame)
		{
			item3.IconNumber = 0;
		}
		m_PlayersItemsFrameUp.IconNumber = 1;
		m_CardsFrameUp.IconNumber = 1;
		m_BuffsFrameUp.IconNumber = 1;
		m_InfoFrameUp.IconNumber = 1;
		m_PlayersItemsFrameW.gameObject.SetActive(value: false);
		m_CardsFrameW.gameObject.SetActive(value: false);
		m_BuffsFrameW.gameObject.SetActive(value: false);
		m_InfoFrameW.gameObject.SetActive(value: false);
		m_PlayersItemsFrameG.gameObject.SetActive(value: true);
		m_CardsFrameG.gameObject.SetActive(value: true);
		m_BuffsFrameG.gameObject.SetActive(value: true);
		m_InfoFrameG.gameObject.SetActive(value: true);
	}

	public void SetGamepadListHighlight(int nr)
	{
		foreach (CFGImageExtension item in m_SlotsFrame)
		{
			item.IconNumber = ((nr == 0) ? 1 : 0);
		}
		foreach (CFGImageExtension item2 in m_PlayersItemsFrame)
		{
			item2.IconNumber = ((nr == 1) ? 1 : 0);
		}
		foreach (CFGImageExtension item3 in m_CardsFrame)
		{
			item3.IconNumber = ((nr == 2) ? 1 : 0);
		}
		m_PlayersItemsFrameUp.IconNumber = ((nr == 1) ? 1 : 0);
		m_CardsFrameUp.IconNumber = ((nr == 2) ? 1 : 0);
		m_BuffsFrameUp.IconNumber = 0;
		m_InfoFrameUp.IconNumber = 0;
		m_PlayersItemsFrameW.gameObject.SetActive(nr != 1);
		m_CardsFrameW.gameObject.SetActive(nr != 2);
		m_BuffsFrameW.gameObject.SetActive(value: true);
		m_InfoFrameW.gameObject.SetActive(value: true);
		m_PlayersItemsFrameG.gameObject.SetActive(nr == 1);
		m_CardsFrameG.gameObject.SetActive(nr == 2);
		m_BuffsFrameG.gameObject.SetActive(value: false);
		m_InfoFrameG.gameObject.SetActive(value: false);
	}

	private CFGDef_Item GetCharItem(int slot)
	{
		if (m_CurrentChar == null)
		{
			return null;
		}
		string text = null;
		switch (slot)
		{
		case 0:
			text = m_CurrentChar.Weapon1;
			break;
		case 1:
			text = m_CurrentChar.Weapon2;
			break;
		case 2:
			text = m_CurrentChar.Item1;
			break;
		case 3:
			text = m_CurrentChar.Item2;
			break;
		case 4:
			text = m_CurrentChar.Talisman;
			break;
		}
		if (text != null)
		{
			return CFGStaticDataContainer.GetItemDefinition(text);
		}
		return null;
	}

	public void SetCurrentCharacter(bool from_pad)
	{
		m_Avatar.IconNumber = m_CurrentChar.ImageIDX;
		m_AvatarName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CurrentChar.Definition.NameID);
		m_HPVal.text = m_CurrentChar.BuffedMaxHP.ToString();
		m_LuckVal.text = m_CurrentChar.Luck + "/" + m_CurrentChar.MaxLuck;
		m_AimVal.text = m_CurrentChar.BuffedAim.ToString();
		m_DefenseVal.text = m_CurrentChar.BuffedDefense.ToString();
		m_MovementVal.text = m_CurrentChar.BuffedMovement.ToString();
		m_SightVal.text = m_CurrentChar.BuffedSight.ToString();
		for (int i = 0; i < m_SlotButtons.Count; i++)
		{
			CFGDef_Item charItem = GetCharItem(i);
			if (charItem != null)
			{
				m_SlotButtons[i].interactable = true;
				m_SlotButtons[i].m_Icon.gameObject.SetActive(value: true);
				m_SlotButtons[i].IsDraggable = true;
				m_SlotButtons[i].IconNumber = charItem.ShopIcon;
				m_SlotButtons[i].IsInUseMeState = false;
			}
			else
			{
				m_SlotButtons[i].m_Icon.gameObject.SetActive(value: false);
				m_SlotButtons[i].IsDraggable = false;
				m_SlotButtons[i].IsInUseMeState = true;
			}
		}
		foreach (GameObject buffs in m_BuffsList)
		{
			Object.Destroy(buffs);
		}
		m_BuffsList.Clear();
		m_BuffButtons.Clear();
		int num = 0;
		float num2 = 1f;
		float num3 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		float num4 = 0.18f;
		if (num3 >= 1f)
		{
			num4 = 0.15f;
		}
		if (m_CurrentChar.Buffs.Count > 6)
		{
			num2 += (float)(m_CurrentChar.Buffs.Count - 6) * num4 * num3;
		}
		RectTransform component = m_BuffsParent.GetComponent<RectTransform>();
		component.offsetMax = new Vector2(1f, 1f);
		component.offsetMin = new Vector2(0f, 0f);
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(num2, 1f);
		m_PlayerCurrentBuffs.Clear();
		foreach (CFGBuff value in m_CurrentChar.Buffs.Values)
		{
			SpawnBuff(value.m_Def.Icon, (int)value.m_Source, value.ToStrigName(), value.ToStringDesc(tactical: false), num);
			m_PlayerCurrentBuffs.Add(value);
			num++;
		}
		if (!from_pad)
		{
			if (m_CurrentCategory == 0)
			{
				OnInventoryClick(0);
			}
			else if (m_CurrentCategory == 1)
			{
				OnWeaponsClick(1);
			}
			else if (m_CurrentCategory == 2)
			{
				OnUsableClick(2);
			}
			else if (m_CurrentCategory == 3)
			{
				OnTalismanClick(3);
			}
			else
			{
				OnNonUsableClick(4);
			}
		}
		for (int j = 0; j < m_CardButtons.Count; j++)
		{
			if (m_CurrentChar.UnlockedCardSlots <= j)
			{
				m_CardButtons[j].IconNumber = 0;
			}
			else
			{
				CFGDef_Card card = m_CurrentChar.GetCard(j);
				if (card == null)
				{
					m_CardButtons[j].IconNumber = 1;
				}
				else
				{
					m_CardButtons[j].IconNumber = card.ImageID;
				}
			}
			if (m_CurrentChar.CardHandBonus != 0)
			{
				CFGDef_Buff buffHandBonus = CFGCharacterData.GetBuffHandBonus(m_CurrentChar.CardHandBonus);
				if (buffHandBonus != null)
				{
					m_CardsSetBtn.IconNumber = buffHandBonus.Icon;
					m_CardsSetBtn.m_DataString = buffHandBonus.BuffID;
					m_CardsSetImgNormal.IconNumber = buffHandBonus.Icon;
				}
				else
				{
					m_CardsSetImgNormal.IconNumber = 1;
				}
			}
			else
			{
				m_CardsSetImgNormal.IconNumber = 1;
			}
		}
		m_ScrollbarBuff.gameObject.SetActive(m_PlayerCurrentBuffs.Count > 5);
	}

	public void SpawnBuff(int icon, int icon_source, string buff_name, string buff_desc, int buff_nr)
	{
		float num = 0f;
		float num2 = 250f;
		if (Screen.width > 1920)
		{
			num2 = 270f;
		}
		if (m_CurrentChar.Buffs.Values.Count > 6)
		{
			num = (m_CurrentChar.Buffs.Values.Count - 6) * 20;
		}
		GameObject gameObject = Object.Instantiate(m_BuffElement);
		gameObject.transform.SetParent(m_BuffsParent.transform, worldPositionStays: false);
		float num3 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3 * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num3 * component.rect.height);
		}
		num2 *= num3;
		num *= num3;
		gameObject.transform.Translate(num2 * (float)buff_nr - num, 0f, 0f);
		CFGButtonExtension component2 = gameObject.GetComponent<CFGButtonExtension>();
		component2.m_ButtonOverCallback = OnBuffIn;
		component2.m_ButtonOutCallback = OnBuffOut;
		component2.interactable = true;
		component2.m_Data = m_BuffsList.Count;
		CFGImageExtension[] componentsInChildren = gameObject.GetComponentsInChildren<CFGImageExtension>();
		foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
		{
			if (cFGImageExtension.gameObject.name == "ImageBUFF")
			{
				cFGImageExtension.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
				cFGImageExtension.IconNumber = icon;
			}
			if (cFGImageExtension.gameObject.name == "Image")
			{
				cFGImageExtension.gameObject.SetActive(value: false);
			}
		}
		Image[] componentsInChildren2 = gameObject.GetComponentsInChildren<Image>();
		foreach (Image image in componentsInChildren2)
		{
			if (image.name == "ImagePrzyciemnienie")
			{
				image.enabled = buff_nr % 2 == 0;
			}
		}
		Text[] componentsInChildren3 = gameObject.GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren3)
		{
			if (text.gameObject.name == "txtNazwaAblita")
			{
				text.text = buff_name;
			}
			if (text.gameObject.name == "txtOpisAblita")
			{
				text.text = buff_desc;
			}
		}
		m_BuffsList.Add(gameObject);
		m_BuffButtons.Add(component2);
	}

	private void SetCharItem(int slot, CFGDef_Item item)
	{
		if (m_CurrentChar != null)
		{
			string itemID = string.Empty;
			if (item != null)
			{
				itemID = item.ItemID;
			}
			switch (slot)
			{
			case 0:
				m_CurrentChar.EquipItem(EItemSlot.Weapon1, itemID);
				break;
			case 1:
				m_CurrentChar.EquipItem(EItemSlot.Weapon2, itemID);
				break;
			case 2:
				m_CurrentChar.EquipItem(EItemSlot.Item1, itemID);
				break;
			case 3:
				m_CurrentChar.EquipItem(EItemSlot.Item2, itemID);
				break;
			case 4:
				m_CurrentChar.EquipItem(EItemSlot.Talisman, itemID);
				break;
			}
		}
	}

	private bool IsItemAllowedOnSlot(int SlotID, CFGDef_Item item)
	{
		if (item == null || SlotID < 0 || SlotID > 4)
		{
			return false;
		}
		switch (SlotID)
		{
		case 0:
		case 1:
			if (item.ItemType == CFGDef_Item.EItemType.Weapon)
			{
				return true;
			}
			break;
		case 2:
		case 3:
			if (item.ItemType == CFGDef_Item.EItemType.Useable)
			{
				return true;
			}
			break;
		case 4:
			if (item.ItemType == CFGDef_Item.EItemType.Talisman)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public void OnCardPanelButton(int id)
	{
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance)
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadCardsPanel(m_CombatLoadout, m_Sequencer);
			CFGSingleton<CFGWindowMgr>.Instance.UnloadCharacterScreen();
		}
	}

	public void OnCardIn(int id)
	{
		m_TooltipBigName.text = string.Empty;
		m_TooltipBigDesc.text = string.Empty;
		m_Heat[0].transform.parent.gameObject.SetActive(value: false);
		m_HeatText2.gameObject.SetActive(value: false);
		m_TooltipImageCardBig.gameObject.SetActive(value: false);
		m_TooltipImageBuffBig.gameObject.SetActive(value: false);
		m_TooltipImageBig.gameObject.SetActive(value: false);
		m_TooltipCharacterBig.gameObject.SetActive(value: false);
		m_TooltipBigParamsNames[0].text = string.Empty;
		m_TooltipBigParamsNames[1].text = string.Empty;
		m_TooltipBigParamsNames[2].text = string.Empty;
		m_TooltipBigParamsValues[0].text = string.Empty;
		m_TooltipBigParamsValues[1].text = string.Empty;
		m_TooltipBigParamsValues[2].text = string.Empty;
		m_TooltipImageCardBig.gameObject.SetActive(value: true);
		if (m_CurrentChar.UnlockedCardSlots <= id)
		{
			m_TooltipImageCardBig.IconNumber = 0;
			m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_locked");
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_locked_desc");
			return;
		}
		CFGDef_Card card = m_CurrentChar.GetCard(id);
		if (card == null)
		{
			m_TooltipImageCardBig.IconNumber = 1;
			m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_empty");
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_empty_desc");
			return;
		}
		m_TooltipImageCardBig.IconNumber = card.ImageID;
		m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(card.CardID);
		CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(card.CardID);
		string text = string.Empty;
		if (cardDefinition.MaxHealth > 0)
		{
			string text2 = text;
			text = text2 + "+" + cardDefinition.MaxHealth + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_maxhp");
		}
		if (cardDefinition.Aim > 0)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			string text2 = text;
			text = text2 + "+" + cardDefinition.Aim + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_aim");
		}
		if (cardDefinition.Defense > 0)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			string text2 = text;
			text = text2 + "+" + cardDefinition.Defense + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_defense");
		}
		if (cardDefinition.Movement > 0)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			string text2 = text;
			text = text2 + "+" + cardDefinition.Movement + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_movement");
		}
		if (cardDefinition.Sight > 0)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			string text2 = text;
			text = text2 + "+" + cardDefinition.Sight + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_sight");
		}
		if (cardDefinition.MaxLuck > 0)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			string text2 = text;
			text = text2 + "+" + cardDefinition.MaxLuck + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_maxluck");
		}
		string text3 = string.Empty;
		CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(cardDefinition.AbilityID);
		if (abilityDef != null && abilityDef.IsPassive)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cardscreen_ability_passive");
		}
		else if (abilityDef != null && abilityDef.CostLuck == 0)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cardscreen_ability_active");
		}
		else if (abilityDef != null)
		{
			text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cardscreen_ability_active_luck", abilityDef.CostLuck.ToString());
		}
		m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.AbilityID.ToString().ToLower() + "_name"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.AbilityID.ToString().ToLower() + "_desc"), text, text3);
	}

	public void OnCardOut(int id)
	{
	}

	public void OnCharacterIn(int id)
	{
		if (m_CharacterDataList[id] != null)
		{
			m_Heat[0].transform.parent.gameObject.SetActive(value: true);
			m_HeatText2.gameObject.SetActive(value: true);
			for (int i = 0; i < m_Heat.Count; i++)
			{
				m_Heat[i].SetActive(i < m_CharacterDataList[id].TotalHeat);
			}
			m_TooltipImageCardBig.gameObject.SetActive(value: false);
			m_TooltipImageBuffBig.gameObject.SetActive(value: false);
			m_TooltipImageBig.gameObject.SetActive(value: false);
			m_TooltipCharacterBig.gameObject.SetActive(value: false);
			m_TooltipBigParamsNames[0].text = string.Empty;
			m_TooltipBigParamsNames[1].text = string.Empty;
			m_TooltipBigParamsNames[2].text = string.Empty;
			m_TooltipBigParamsValues[0].text = string.Empty;
			m_TooltipBigParamsValues[1].text = string.Empty;
			m_TooltipBigParamsValues[2].text = string.Empty;
			m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CharacterDataList[id].Definition.NameID);
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CharacterDataList[id].Definition.NameID + "_desc", m_CharacterDataList[id].Heat.ToString());
			m_TooltipCharacterBig.IconNumber = m_CharacterDataList[id].ImageIDX;
			m_TooltipCharacterBig.gameObject.SetActive(value: true);
		}
	}

	public void OnCharacterOut(int id)
	{
	}

	public void OnItemIn(int id)
	{
		if (m_PlayerCurrentItems[id].ItemDef == null)
		{
			return;
		}
		m_CursorOverSlot = true;
		m_Heat[0].transform.parent.gameObject.SetActive(m_PlayerCurrentItems[id].ItemDef.Heat > 0 || m_PlayerCurrentItems[id].ItemDef.ItemType == CFGDef_Item.EItemType.Weapon);
		m_HeatText2.gameObject.SetActive(m_PlayerCurrentItems[id].ItemDef.Heat > 0 || m_PlayerCurrentItems[id].ItemDef.ItemType == CFGDef_Item.EItemType.Weapon);
		for (int i = 0; i < m_Heat.Count; i++)
		{
			m_Heat[i].SetActive(i < m_PlayerCurrentItems[id].ItemDef.Heat);
		}
		m_TooltipBigName.text = string.Empty;
		m_TooltipBigDesc.text = string.Empty;
		m_TooltipImageCardBig.gameObject.SetActive(value: false);
		m_TooltipImageBuffBig.gameObject.SetActive(value: false);
		m_TooltipImageBig.gameObject.SetActive(value: false);
		m_TooltipCharacterBig.gameObject.SetActive(value: false);
		m_TooltipBigParamsNames[0].text = string.Empty;
		m_TooltipBigParamsNames[1].text = string.Empty;
		m_TooltipBigParamsNames[2].text = string.Empty;
		m_TooltipBigParamsValues[0].text = string.Empty;
		m_TooltipBigParamsValues[1].text = string.Empty;
		m_TooltipBigParamsValues[2].text = string.Empty;
		m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerCurrentItems[id].ItemDef.ItemID + "_name");
		m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerCurrentItems[id].ItemDef.ItemID + "_desc");
		m_TooltipImageBig.IconNumber = m_PlayerCurrentItems[id].ItemDef.ShopIcon;
		m_TooltipImageBig.gameObject.SetActive(value: true);
		if (m_PlayerCurrentItems[id].ItemDef.ItemType == CFGDef_Item.EItemType.Weapon)
		{
			CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(m_PlayerCurrentItems[id].ItemID);
			if (weapon != null)
			{
				m_TooltipBigParamsNames[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_class_tactical_details");
				m_TooltipBigParamsNames[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_dmg_tactical_details");
				m_TooltipBigParamsNames[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_maxammo_tactical_details");
				m_TooltipBigParamsValues[0].text = weapon.WeaponClass.GetLocalizedName();
				m_TooltipBigParamsValues[1].text = weapon.Damage.ToString();
				m_TooltipBigParamsValues[2].text = weapon.Ammo.ToString();
				for (int j = 0; j < m_Heat.Count; j++)
				{
					m_Heat[j].SetActive(j < weapon.Heat);
				}
				string text = string.Empty;
				if (weapon.AllowsFanning)
				{
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
				}
				if (weapon.AllowsScoped)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
				}
				if (weapon.AllowsCone)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
				}
				if (!weapon.AllowsRicochet)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
				}
				if (!weapon.ShotEndsTurn)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
				}
				if (weapon.AimModifier != 0)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					string text2 = text;
					text = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
				}
				m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_PlayerCurrentItems[id].ItemDef.ItemID + "_desc"), text, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
			}
		}
		if (m_PlayerCurrentItems[id].ItemDef.ItemType != CFGDef_Item.EItemType.Talisman)
		{
			return;
		}
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_PlayerCurrentItems[id].ItemID);
		if (itemDefinition != null)
		{
			string text3 = string.Empty;
			if (itemDefinition.Mod_Aim != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
			}
			if (itemDefinition.Mod_Defense != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
			}
			if (itemDefinition.Mod_Sight != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
			}
			if (itemDefinition.Mod_Damage != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
			}
			if (itemDefinition.Mod_Movement != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
			}
			if (itemDefinition.Mod_MaxHP != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
			}
			if (itemDefinition.Mod_MaxLuck != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
			}
			if (itemDefinition.Heat != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
			}
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text3);
		}
	}

	public void OnItemOut(int id)
	{
		m_CursorOverSlot = false;
		ClearItemNew(m_PlayerCurrentItems[id].ItemDef.ItemID);
		m_InventoryButtons[id].IsSelected = false;
	}

	public void OnCardsSetIn(int id)
	{
		m_TooltipBigName.text = string.Empty;
		m_TooltipBigDesc.text = string.Empty;
		m_TooltipImageCardBig.gameObject.SetActive(value: false);
		m_TooltipImageBuffBig.gameObject.SetActive(value: false);
		m_TooltipImageBig.gameObject.SetActive(value: false);
		m_TooltipCharacterBig.gameObject.SetActive(value: false);
		m_Heat[0].transform.parent.gameObject.SetActive(value: false);
		m_HeatText2.gameObject.SetActive(value: false);
		m_TooltipBigParamsNames[0].text = string.Empty;
		m_TooltipBigParamsNames[1].text = string.Empty;
		m_TooltipBigParamsNames[2].text = string.Empty;
		m_TooltipBigParamsValues[0].text = string.Empty;
		m_TooltipBigParamsValues[1].text = string.Empty;
		m_TooltipBigParamsValues[2].text = string.Empty;
		if (m_CardsSetBtn.m_DataString == string.Empty)
		{
			m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hand_nohand");
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hand_nohand_desc");
			m_TooltipImageBuffBig.gameObject.SetActive(value: false);
			return;
		}
		CFGDef_Buff buff = CFGStaticDataContainer.GetBuff(m_CardsSetBtn.m_DataString.ToLower());
		if (buff != null)
		{
			CFGBuff cFGBuff = new CFGBuff(buff, EBuffSource.CardHandBonus);
			if (cFGBuff != null)
			{
				m_TooltipBigName.text = cFGBuff.ToStrigName();
				m_TooltipBigDesc.text = cFGBuff.ToStringDesc(tactical: false);
			}
		}
		m_TooltipImageBuffBig.IconNumber = m_CardsSetImgNormal.IconNumber;
		m_TooltipImageBuffBig.gameObject.SetActive(value: true);
	}

	public void OnBuffIn(int id)
	{
		m_TooltipBigName.text = string.Empty;
		m_TooltipBigDesc.text = string.Empty;
		m_TooltipImageCardBig.gameObject.SetActive(value: false);
		m_TooltipImageBuffBig.gameObject.SetActive(value: false);
		m_TooltipImageBig.gameObject.SetActive(value: false);
		m_TooltipCharacterBig.gameObject.SetActive(value: false);
		m_TooltipBigParamsNames[0].text = string.Empty;
		m_TooltipBigParamsNames[1].text = string.Empty;
		m_TooltipBigParamsNames[2].text = string.Empty;
		m_TooltipBigParamsValues[0].text = string.Empty;
		m_TooltipBigParamsValues[1].text = string.Empty;
		m_TooltipBigParamsValues[2].text = string.Empty;
		m_TooltipBigName.text = m_PlayerCurrentBuffs[id].ToStrigName();
		m_TooltipBigDesc.text = m_PlayerCurrentBuffs[id].ToStringDesc(tactical: false);
		m_TooltipImageBuffBig.IconNumber = m_PlayerCurrentBuffs[id].m_Def.Icon;
		m_TooltipImageBuffBig.gameObject.SetActive(value: true);
	}

	public void OnBuffOut(int id)
	{
	}

	public void OnSlotIn(int id)
	{
		if (m_SlotButtons[id].m_IsHighlighted)
		{
			m_CursorOverSlot = true;
		}
		m_TooltipBigName.text = string.Empty;
		m_TooltipBigDesc.text = string.Empty;
		m_TooltipImageCardBig.gameObject.SetActive(value: false);
		m_TooltipImageBuffBig.gameObject.SetActive(value: false);
		m_TooltipImageBig.gameObject.SetActive(value: false);
		m_TooltipCharacterBig.gameObject.SetActive(value: false);
		m_TooltipBigParamsNames[0].text = string.Empty;
		m_TooltipBigParamsNames[1].text = string.Empty;
		m_TooltipBigParamsNames[2].text = string.Empty;
		m_TooltipBigParamsValues[0].text = string.Empty;
		m_TooltipBigParamsValues[1].text = string.Empty;
		m_TooltipBigParamsValues[2].text = string.Empty;
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_releaseitem");
		CFGDef_Item charItem = GetCharItem(id);
		if (charItem == null)
		{
			m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_slot_free");
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_slot_free_desc");
			m_TooltipImageBig.gameObject.SetActive(value: false);
			m_Heat[0].transform.parent.gameObject.SetActive(value: false);
			m_HeatText2.gameObject.SetActive(value: false);
			return;
		}
		m_Heat[0].transform.parent.gameObject.SetActive(charItem.Heat > 0 || charItem.ItemType == CFGDef_Item.EItemType.Weapon);
		m_HeatText2.gameObject.SetActive(charItem.Heat > 0 || charItem.ItemType == CFGDef_Item.EItemType.Weapon);
		for (int i = 0; i < m_Heat.Count; i++)
		{
			m_Heat[i].SetActive(i < charItem.Heat);
		}
		m_TooltipBigName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(charItem.ItemID + "_name");
		m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(charItem.ItemID + "_desc");
		m_TooltipImageBig.IconNumber = charItem.ShopIcon;
		m_TooltipImageBig.gameObject.SetActive(value: true);
		if (charItem.ItemType == CFGDef_Item.EItemType.Weapon)
		{
			CFGDef_Weapon weapon = CFGStaticDataContainer.GetWeapon(charItem.ItemID);
			if (weapon != null)
			{
				m_TooltipBigParamsNames[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_class_tactical_details");
				m_TooltipBigParamsNames[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_dmg_tactical_details");
				m_TooltipBigParamsNames[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_maxammo_tactical_details");
				m_TooltipBigParamsValues[0].text = weapon.WeaponClass.GetLocalizedName();
				m_TooltipBigParamsValues[1].text = weapon.Damage.ToString();
				m_TooltipBigParamsValues[2].text = weapon.Ammo.ToString();
				for (int j = 0; j < m_Heat.Count; j++)
				{
					m_Heat[j].SetActive(j < weapon.Heat);
				}
				string text = string.Empty;
				if (weapon.AllowsFanning)
				{
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
				}
				if (weapon.AllowsScoped)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
				}
				if (weapon.AllowsCone)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
				}
				if (!weapon.AllowsRicochet)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
				}
				if (!weapon.ShotEndsTurn)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
				}
				if (weapon.AimModifier != 0)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					string text2 = text;
					text = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + weapon.AimModifier;
				}
				m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(charItem.ItemID + "_desc"), text, weapon.Damage.ToString(), weapon.Heat.ToString(), ((weapon.Damage / weapon.HalfCoverDiv < 1) ? 1 : (weapon.Damage / weapon.HalfCoverDiv)).ToString(), ((weapon.Damage / weapon.FullCoverDiv < 1) ? 1 : (weapon.Damage / weapon.FullCoverDiv)).ToString());
			}
		}
		if (charItem.ItemType != CFGDef_Item.EItemType.Talisman)
		{
			return;
		}
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(charItem.ItemID);
		if (itemDefinition != null)
		{
			string text3 = string.Empty;
			if (itemDefinition.Mod_Aim != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Aim <= 0) ? itemDefinition.Mod_Aim.ToString() : ("+" + itemDefinition.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
			}
			if (itemDefinition.Mod_Defense != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Defense <= 0) ? itemDefinition.Mod_Defense.ToString() : ("+" + itemDefinition.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
			}
			if (itemDefinition.Mod_Sight != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Sight <= 0) ? itemDefinition.Mod_Sight.ToString() : ("+" + itemDefinition.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
			}
			if (itemDefinition.Mod_Damage != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Damage <= 0) ? itemDefinition.Mod_Damage.ToString() : ("+" + itemDefinition.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
			}
			if (itemDefinition.Mod_Movement != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_Movement <= 0) ? itemDefinition.Mod_Movement.ToString() : ("+" + itemDefinition.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
			}
			if (itemDefinition.Mod_MaxHP != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_MaxHP <= 0) ? itemDefinition.Mod_MaxHP.ToString() : ("+" + itemDefinition.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
			}
			if (itemDefinition.Mod_MaxLuck != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Mod_MaxLuck <= 0) ? itemDefinition.Mod_MaxLuck.ToString() : ("+" + itemDefinition.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
			}
			if (itemDefinition.Heat != 0)
			{
				string text2 = text3;
				text3 = text2 + ((itemDefinition.Heat <= 0) ? itemDefinition.Heat.ToString() : ("+" + itemDefinition.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
			}
			m_TooltipBigDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc"), text3);
		}
	}

	public void OnSlotOut(int id)
	{
		m_CursorOverSlot = false;
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_characterscreen_assignitem_slot1");
	}

	private void HighlightSlot(CFGDef_Item item, bool highlight)
	{
		if (item != null)
		{
			if (item.ItemType == CFGDef_Item.EItemType.Weapon)
			{
				m_SlotButtons[0].ToggleHighlight(highlight);
				m_SlotButtons[1].ToggleHighlight(highlight);
			}
			else if (item.ItemType == CFGDef_Item.EItemType.Useable)
			{
				m_SlotButtons[2].ToggleHighlight(highlight);
				m_SlotButtons[3].ToggleHighlight(highlight);
			}
			else if (item.ItemType == CFGDef_Item.EItemType.Talisman)
			{
				m_SlotButtons[4].ToggleHighlight(highlight);
			}
		}
	}

	public void OnSlotClick(int slot)
	{
		OnSlotIn(slot);
	}

	public void OnReleaseOnSlot(int slot)
	{
		if (m_ItemToEquip == null || m_ItemToEquip.ItemDef == null)
		{
			return;
		}
		if (m_ItemToEquip != null && m_ItemToEquip.ItemDef != null)
		{
			if (!IsItemAllowedOnSlot(slot, m_ItemToEquip.ItemDef))
			{
				Debug.LogWarning("Cannot equip " + m_ItemToEquip.ItemID + " on slot " + slot);
				m_ItemToEquip = null;
				return;
			}
			if (m_ItemToEquip != null)
			{
				if (GetCharItem(slot) != null)
				{
					m_SlotDragged = slot;
					OnInvDrop(0);
				}
				Debug.Log("Equip " + m_ItemToEquip.ItemID + " to slot " + slot);
				SetCharItem(slot, m_ItemToEquip.ItemDef);
				CFGInventory.RemoveItem(m_ItemToEquip.ItemID, 1, SetAsNew: false);
				SetCurrentCharacter(from_pad: false);
				HighlightSlot(m_ItemToEquip.ItemDef, highlight: false);
			}
		}
		m_ItemToEquip = null;
	}

	public void OnItemClick(int item)
	{
		if (item < m_PlayerCurrentItems.Count && m_PlayerCurrentItems[item].ItemDef != null)
		{
			ClearItemNew(m_PlayerCurrentItems[item].ItemDef.ItemID);
			if (m_InventoryButtons[item] != null)
			{
				m_InventoryButtons[item].IsSelected = false;
			}
			OnItemIn(item);
		}
	}

	public void OnItemDrag(int item)
	{
		m_ItemToEquip = m_PlayerCurrentItems[item];
		if (!CFGInventory.BackpackItems.Contains(m_ItemToEquip))
		{
			Debug.Log("No item in backpack " + m_ItemToEquip);
		}
		else if (m_ItemToEquip.ItemDef == null)
		{
			Debug.LogWarning("Cannot equip item without definition!" + m_ItemToEquip.ItemDef.ItemID);
		}
		else
		{
			HighlightSlot(m_ItemToEquip.ItemDef, highlight: true);
		}
	}

	public void OnRealeseOutside(int slot)
	{
		m_SlotButtons[0].ToggleHighlight(high: false);
		m_SlotButtons[1].ToggleHighlight(high: false);
		m_SlotButtons[2].ToggleHighlight(high: false);
		m_SlotButtons[3].ToggleHighlight(high: false);
		m_SlotButtons[4].ToggleHighlight(high: false);
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
			instance.UnloadCharacterScreen();
		}
		CFGSelectionManager.GlobalLock(ELockReason.Wnd_CharacterScreen, bEnable: false);
		if (m_CombatLoadout && (bool)m_Sequencer)
		{
			m_Sequencer.StartLoadingTactical();
		}
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

	protected void SpawnItemList(List<CFGItem> items, List<CFGButtonExtension> btns, GameObject parent)
	{
		float value = m_Scrollbar.value;
		foreach (CFGButtonExtension inventoryButton in m_InventoryButtons)
		{
			Object.Destroy(inventoryButton.transform.parent.gameObject);
		}
		btns.Clear();
		float num = (m_InventoryItem.GetComponent<RectTransform>().anchorMax.y - m_InventoryItem.GetComponent<RectTransform>().anchorMin.y) * base.transform.parent.GetComponent<RectTransform>().rect.height;
		RectTransform component = parent.GetComponent<RectTransform>();
		float num2 = 0.35f;
		if (Screen.height < 900)
		{
			num2 = 225f / (float)Screen.height;
		}
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(1f, (!(1f >= num2 * 0.65f * (float)items.Count)) ? (num2 * 0.65f * (float)items.Count) : 1f);
		component.offsetMax = new Vector2(1f, 1f);
		component.offsetMin = new Vector2(0f, 0f);
		for (int i = 0; i < items.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(m_InventoryItem);
			gameObject.name = "SPAWN item";
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.SetParent(parent.transform, worldPositionStays: false);
			component2.transform.localPosition = new Vector3(0f, 0f, 0f);
			float num3 = 1f - (float)i * (num / parent.GetComponent<RectTransform>().rect.height);
			float y = num3 - num / parent.GetComponent<RectTransform>().rect.height;
			component2.offsetMax = new Vector2(0f, 1f);
			component2.offsetMin = new Vector2(0f, 1f);
			component2.anchorMax = new Vector2(1f, num3);
			component2.anchorMin = new Vector2(0f, y);
			m_ItemHeight = component2.rect.height;
			CFGButtonExtension componentInChildren = gameObject.GetComponentInChildren<CFGButtonExtension>();
			btns.Add(componentInChildren);
			componentInChildren.m_Data = i;
			if (items[i].ItemDef != null)
			{
				componentInChildren.IconNumber = items[i].ItemDef.ShopIcon;
			}
			componentInChildren.IsDraggable = true;
			componentInChildren.m_DragParent = parent.transform.parent.parent.parent;
			componentInChildren.m_ButtonDragCallback = OnItemDrag;
			componentInChildren.m_ButtonRealeseOutsideCallback = OnRealeseOutside;
			componentInChildren.m_ButtonOverCallback = OnItemIn;
			componentInChildren.m_ButtonOutCallback = OnItemOut;
			componentInChildren.m_ButtonClickedCallback = OnItemClick;
			componentInChildren.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
			componentInChildren.IsSelected = IsItemNew(items[i].ItemID) && items[i].ItemID != "cash";
			CFGImageExtension[] componentsInChildren = gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
			{
				if (cFGImageExtension.name == "ImageITEM")
				{
					cFGImageExtension.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
					cFGImageExtension.IconNumber = items[i].ItemDef.ShopIcon;
				}
				else if (cFGImageExtension.name == "Ikonka")
				{
					cFGImageExtension.IconNumber = ((items[i].ItemDef.ItemType != CFGDef_Item.EItemType.Weapon) ? ((items[i].ItemDef.ItemType == CFGDef_Item.EItemType.Talisman) ? 2 : ((items[i].ItemDef.ItemType == CFGDef_Item.EItemType.Useable) ? 1 : 3)) : 0);
				}
			}
			Image[] componentsInChildren2 = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image in componentsInChildren2)
			{
				if (image.name == "Image$")
				{
					image.gameObject.SetActive(value: false);
				}
			}
			Image[] componentsInChildren3 = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image2 in componentsInChildren3)
			{
				if (image2.name == "ImageBGblack")
				{
					image2.enabled = i % 2 == 0;
				}
			}
			Text[] componentsInChildren4 = gameObject.GetComponentsInChildren<Text>();
			foreach (Text text in componentsInChildren4)
			{
				if (text.name == "txtNazwa")
				{
					if (items[i].ItemDef != null)
					{
						text.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(items[i].ItemDef.ItemID + "_name");
					}
					else
					{
						text.text = "item deff null";
					}
				}
				else if (text.name == "txtIlosc")
				{
					if (items[i].Count > 1)
					{
						text.text = "x " + items[i].Count;
					}
					else
					{
						text.text = string.Empty;
					}
				}
				else if (text.name == "txtCena")
				{
					text.text = string.Empty;
				}
			}
		}
		m_Scrollbar.value = 0f;
		m_Scrollbar.value = value;
		if (m_ControllerInput.CurrentList == 1 && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			m_ControllerInput.DeactivateList(1);
		}
		if (!m_Init)
		{
			m_Init = true;
			m_Scrollbar.value = 1f;
		}
	}

	private void UpdateController()
	{
		if (CFGButtonExtension.IsWaitingForClick)
		{
			return;
		}
		CFGCardsPanel cardsPanel = CFGSingleton<CFGWindowMgr>.Instance.m_CardsPanel;
		if ((bool)cardsPanel && cardsPanel.isActiveAndEnabled)
		{
			m_bCardPanelWasActive = true;
			return;
		}
		if (m_bCardPanelWasActive && m_ControllerInput.CurrentList == 2 && m_ControllerInput.CurrentListObject != null)
		{
			OnController_CardSelect(m_ControllerInput.CurrentListObject.m_CurrentItem);
		}
		m_bCardPanelWasActive = false;
		m_ControllerInput.UpdateInput();
		m_ControllerInput_Boons.UpdateInput();
		if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
		{
			m_PrevButton.m_ButtonClickedCallback(0);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
		{
			m_NextButton.m_ButtonClickedCallback(0);
		}
		if (m_ControllerInput.CurrentList == 1)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Bottom) > 0.5f)
			{
				SelectOtherCategory(1);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Top) > 0.5f)
			{
				SelectOtherCategory(-1);
			}
		}
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Cancel))
		{
			m_ExitButton.SimulateClick(bDelayed: true);
		}
		if (CFGJoyManager.IsActivated(EJoyAction.Default_OtherAction))
		{
			m_CardPanelButton.m_ButtonClickedCallback(0);
		}
	}

	private void EquipCurrentItem(bool Alternate)
	{
		if (m_InventoryButtons.Count < 1)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < m_InventoryButtons.Count; i++)
		{
			if (m_InventoryButtons[i].IsUnderCursor)
			{
				num = i;
				break;
			}
		}
		if (num == -1 || num >= m_PlayerCurrentItems.Count)
		{
			return;
		}
		if (m_ItemToEquip != null)
		{
			OnRealeseOutside(0);
		}
		m_ItemToEquip = null;
		OnItemDrag(num);
		if (m_ItemToEquip != null)
		{
			switch (m_ItemToEquip.ItemDef.ItemType)
			{
			case CFGDef_Item.EItemType.Talisman:
				OnReleaseOnSlot(4);
				break;
			case CFGDef_Item.EItemType.Useable:
				OnReleaseOnSlot((!Alternate) ? 2 : 3);
				break;
			case CFGDef_Item.EItemType.Weapon:
				OnReleaseOnSlot(Alternate ? 1 : 0);
				break;
			default:
				OnRealeseOutside(0);
				break;
			}
			m_ItemToEquip = null;
		}
	}

	private void SelectOtherCategory(int Add)
	{
		m_CurrentCategory += Add;
		if (m_CurrentCategory < 0)
		{
			m_CurrentCategory = 4;
		}
		if (m_CurrentCategory > 4)
		{
			m_CurrentCategory = 0;
		}
		m_InventoryFilterButtons[m_CurrentCategory].SimulateClick(bDelayed: false, bDeselect: true);
	}

	private void OnController_BuffButton(int ButtonID, bool Secondary)
	{
		if (ButtonID >= 0 && ButtonID < m_BuffButtons.Count && !(m_BuffButtons[ButtonID] == null))
		{
		}
	}

	private void OnController_InventoryButton(int ButtonID, bool Secondary)
	{
		if (ButtonID >= 0 && ButtonID < m_InventoryButtons.Count && !(m_InventoryButtons[ButtonID] == null))
		{
			EquipCurrentItem(Secondary);
		}
	}

	private void OnController_CardButton(int ButtonID, bool Secondary)
	{
		if (!Secondary)
		{
			m_CardPanelButton.m_ButtonClickedCallback(0);
		}
	}

	private void OnController_CardSelect(int ButtonID)
	{
		if (ButtonID >= 0 && ButtonID < 1)
		{
			OnCardIn(m_ControllerInput.CurrentList - 2);
		}
	}

	private void OnController_SlotButton(int ButtonID, bool Secondary)
	{
		if (ButtonID >= 0 && ButtonID < m_SlotButtons.Count && !(m_SlotButtons[ButtonID] == null) && !Secondary)
		{
			OnSlotDrag(ButtonID);
			OnInvDrop(0);
		}
	}

	private void OnController_ListChanged(int OldListID, int NewListID, bool bPrev)
	{
		if (bPrev)
		{
			m_LTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		else
		{
			m_RTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}
}
