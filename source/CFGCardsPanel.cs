using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGCardsPanel : CFGPanel
{
	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_YButtonPad;

	public CFGButtonExtension m_XButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_LBButtonPad;

	public CFGButtonExtension m_RBButtonPad;

	public Text m_CardsPanelTxtW;

	public Text m_CardsPanelTxtG;

	public Text m_InfoPanelTxtW;

	public Text m_InfoPanelTxtG;

	public CFGImageExtension m_CardsPanelUp;

	public CFGImageExtension m_InfoPanelUp;

	public List<CFGImageExtension> m_CardsPanelFrame = new List<CFGImageExtension>();

	public CFGButtonExtension m_CardPanelDropable;

	public List<CFGButtonExtension> m_SkillCards = new List<CFGButtonExtension>();

	public List<CFGImageExtension> m_SkillCardsCharacterIcons = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_SkillCardsCharacterIconsFrames = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CardsSetIcons = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_CardsSetIcons2 = new List<CFGImageExtension>();

	public List<CFGButtonExtension> m_CardsSetButtons = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_CardsSetButtons2 = new List<CFGButtonExtension>();

	public Text m_CardName;

	public Text m_CardDesc;

	public CFGImageExtension m_CardImg;

	public CFGButtonExtension[] m_CompabilityButtons;

	public CFGImageExtension m_BuffImg;

	public CFGButtonExtension m_AssingBtn;

	public CFGButtonExtension m_CancelBtn;

	public CFGButtonExtension m_ExitBtn;

	public List<CFGCharacterCardsBar> m_CharacterCardsBar = new List<CFGCharacterCardsBar>();

	private CFGButtonExtension m_DraggingCard;

	private CFGButtonExtension m_SelectedCard;

	private CFGButtonExtension m_SelectedCharacterCard;

	private CFGButtonExtension m_SelectedCharacterCard2;

	private CFGButtonExtension m_DraggedCharacterCard;

	private CFGButtonExtension m_DraggedCharacterCard2;

	private List<Vector3> m_BarPositions = new List<Vector3>();

	private int m_LastCharActive = -1;

	private int m_TotalCollectedCards;

	private CFGJoyMenuController m_Controller_Cards = new CFGJoyMenuController();

	public List<CFGButtonExtension> m_CardMatrix = new List<CFGButtonExtension>();

	public Text m_Title;

	public Text m_TitleCards;

	public Text m_TitleInfo;

	public Text m_Title2;

	public Text m_TitleCards2;

	public Text m_TitleInfo2;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	public bool m_CombatLoadout;

	public CFGSequencer m_Sequencer;

	private bool m_IsInitialized;

	private bool m_RefreshIcons;

	private bool m_ClickButtonFromCard;

	private int m_CardId;

	private float m_Prc = 1f;

	protected override void Start()
	{
		base.Start();
		float num = (m_Prc = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f));
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		foreach (CFGButtonExtension skillCard in m_SkillCards)
		{
			skillCard.m_ButtonDragCallback = OnCardDrag;
			skillCard.m_ButtonOverCallback = OnSkillCardHover;
			skillCard.m_ButtonClickedCallback = OnSkillCardClick;
			skillCard.m_OnRightClickCallback = AssignCard;
			skillCard.m_OnDoubleClickCallback = AssignCard;
			skillCard.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
		}
		foreach (CFGImageExtension skillCardsCharacterIcon in m_SkillCardsCharacterIcons)
		{
			skillCardsCharacterIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
			skillCardsCharacterIcon.gameObject.SetActive(value: false);
		}
		foreach (CFGImageExtension skillCardsCharacterIconsFrame in m_SkillCardsCharacterIconsFrames)
		{
			skillCardsCharacterIconsFrame.gameObject.SetActive(value: false);
		}
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			m_CharacterCardsBar[i].SetCardDatas();
			for (int j = 0; j < m_CharacterCardsBar[i].m_CharacterCards.Count; j++)
			{
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ButtonDropCallback = OnCardDrop;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ButtonClickedCallback = OnCharacterCardClick;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ButtonOverCallback = OnCharacterCardHover;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ButtonDragCallback = OnCharacterCardDragNonActive;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ButtonRealeseOutsideCallback = OnRealeseOutside;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_OnRightClickCallback = AssignCard;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_OnDoubleClickCallback = AssignCard;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_ShouldDragSelectItem = false;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_Draggable = true;
				m_CharacterCardsBar[i].m_CharacterCards[j].m_NeedBigTooltup = true;
			}
			for (int k = 0; k < m_CharacterCardsBar[i].m_CharacterCardsSelected.Count; k++)
			{
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ButtonDropCallback = OnCardDrop;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ButtonDragCallback = OnCharacterCardDrag;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ButtonOverCallback = OnCharacterCardHover;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ButtonRealeseOutsideCallback = OnRealeseOutside;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_OnRightClickCallback = AssignCard;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_OnDoubleClickCallback = AssignCard;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ShouldDragSelectItem = true;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_Draggable = true;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_ButtonClickedCallback = OnCharacterCardClick;
				m_CharacterCardsBar[i].m_CharacterCardsSelected[k].m_NeedBigTooltup = true;
			}
		}
		m_CardImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
		m_BuffImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
		m_AssingBtn.m_ButtonClickedCallback = AssignCard;
		m_CancelBtn.m_ButtonClickedCallback = OnCancelButtonClick;
		m_CancelBtn.enabled = false;
		m_YButtonPad.enabled = false;
		m_ExitBtn.m_ButtonClickedCallback = OnExitButton;
		m_CharacterCardsBar[0].gameObject.SetActive(value: false);
		m_CharacterCardsBar[1].gameObject.SetActive(value: false);
		m_CharacterCardsBar[2].gameObject.SetActive(value: false);
		m_CharacterCardsBar[3].gameObject.SetActive(value: false);
		foreach (CFGCharacterCardsBar item in m_CharacterCardsBar)
		{
			m_BarPositions.Add(item.transform.position);
		}
		foreach (CFGCharacterCardsBar item2 in m_CharacterCardsBar)
		{
			item2.m_PanelNormal.gameObject.SetActive(value: true);
			item2.m_PanelSelected.gameObject.SetActive(value: false);
			item2.m_CharacterButton.m_ButtonClickedCallback = OnCharacterButtonClick;
		}
		for (int l = 0; l < m_CardsSetIcons.Count; l++)
		{
			CFGImageExtension cFGImageExtension = m_CardsSetIcons[l];
			cFGImageExtension.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
			cFGImageExtension.IconNumber = 0;
			m_CardsSetButtons.Add(cFGImageExtension.gameObject.GetComponent<CFGButtonExtension>());
			cFGImageExtension.gameObject.GetComponent<CFGButtonExtension>().m_ButtonClickedCallback = OnIconIn;
			cFGImageExtension.gameObject.GetComponent<CFGButtonExtension>().m_ButtonOverCallback = OnIconOver;
			cFGImageExtension.gameObject.GetComponent<CFGButtonExtension>().m_Data = l;
		}
		for (int m = 0; m < m_CardsSetIcons2.Count; m++)
		{
			CFGImageExtension cFGImageExtension2 = m_CardsSetIcons2[m];
			cFGImageExtension2.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
			cFGImageExtension2.IconNumber = 0;
			m_CardsSetButtons2.Add(cFGImageExtension2.gameObject.GetComponent<CFGButtonExtension>());
			cFGImageExtension2.gameObject.GetComponent<CFGButtonExtension>().m_ButtonClickedCallback = OnIconIn;
			cFGImageExtension2.gameObject.GetComponent<CFGButtonExtension>().m_ButtonOverCallback = OnIconOver;
			cFGImageExtension2.gameObject.GetComponent<CFGButtonExtension>().m_Data = m;
		}
		UpdateCharacterList();
		UpdateSkillCardsList();
		OnCharacterButtonClick(0);
		for (int n = 0; n < 6; n++)
		{
			m_CardMatrix.Add(m_SkillCards[n * 4]);
		}
		m_CardMatrix.Add(null);
		for (int num2 = 0; num2 < 6; num2++)
		{
			m_CardMatrix.Add(m_SkillCards[1 + num2 * 4]);
		}
		m_CardMatrix.Add(m_SkillCards[m_SkillCards.Count - 2]);
		for (int num3 = 0; num3 < 6; num3++)
		{
			m_CardMatrix.Add(m_SkillCards[2 + num3 * 4]);
		}
		m_CardMatrix.Add(m_SkillCards[m_SkillCards.Count - 1]);
		for (int num4 = 0; num4 < 6; num4++)
		{
			m_CardMatrix.Add(m_SkillCards[3 + num4 * 4]);
		}
		m_CardMatrix.Add(null);
		CFGJoyMenuButtonList cFGJoyMenuButtonList = m_Controller_Cards.AddList(m_CardMatrix, OnCardSelected, null, 7);
		if (cFGJoyMenuButtonList != null)
		{
			cFGJoyMenuButtonList.m_NaviMode = CFGJoyMenuButtonList.EListNavigationMode.Matrix;
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			Canvas.ForceUpdateCanvases();
			m_Controller_Cards.SelectFirstItem(bForce: false);
			SetGamepadListHighlight(is_on_cards: false);
		}
		else
		{
			OnCharacterCardClick(0);
			OnCharacterCardHover(0);
			SetMouseListHighlight();
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_AButtonPad.gameObject.SetActive(flag);
		m_XButtonPad.gameObject.SetActive(flag);
		m_BButtonPad.gameObject.SetActive(flag);
		m_YButtonPad.gameObject.SetActive(flag);
		m_LBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
		m_RBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
		m_ExitBtn.gameObject.SetActive(!flag);
		m_LastInput = CFGInput.LastReadInputDevice;
		m_CardPanelDropable.gameObject.SetActive(value: false);
		m_CardPanelDropable.m_ButtonDropCallback = OnDropOnCardsPanel;
		for (int num5 = 0; num5 < m_CharacterCardsBar.Count; num5++)
		{
			for (int num6 = 0; num6 < m_CharacterCardsBar[num5].m_CharacterCards.Count; num6++)
			{
				if (m_CharacterCardsBar[num5].m_CharacterCards[num6].m_DataString != "Empty" && m_CharacterCardsBar[num5].m_CharacterCards[num6].m_DataString != "Locked")
				{
					SetCardLocked(m_CharacterCardsBar[num5].m_CharacterCards[num6], isLocked: true);
				}
				if (m_CharacterCardsBar[num5].m_CharacterCardsSelected[num6].m_DataString != "Empty" && m_CharacterCardsBar[num5].m_CharacterCardsSelected[num6].m_DataString != "Locked")
				{
					SetCardLocked(m_CharacterCardsBar[num5].m_CharacterCardsSelected[num6], isLocked: true);
				}
			}
		}
		UpdateSkillCardsCharIcons(setLock: true);
		UpdateAlphabet(null);
	}

	public override void SetLocalisation()
	{
		m_ExitBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_close");
		m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_title");
		m_TitleCards.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_header_cards");
		m_TitleInfo.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_header_details");
		m_Title2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_title");
		m_TitleCards2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_header_cards");
		m_TitleInfo2.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_screen_header_details");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_assigncard");
		m_YButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_revert");
		m_XButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_showhand");
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_close");
		m_LBButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_changechar");
	}

	public void OnDropOnCardsPanel(int a)
	{
		if (m_DraggedCharacterCard != null)
		{
			m_DraggedCharacterCard.m_DataString = "Empty";
			m_DraggedCharacterCard.IconNumber = 1;
			m_DraggedCharacterCard2.m_DataString = "Empty";
			m_DraggedCharacterCard2.IconNumber = 1;
			UpdateSkillCardsList();
			UpdateCharactersStats();
			CalcIcons();
			m_DraggedCharacterCard = null;
			m_DraggedCharacterCard2 = null;
		}
		else
		{
			AssignCard(0);
			m_CardPanelDropable.gameObject.SetActive(value: false);
		}
	}

	private void SetGamepadListHighlight(bool is_on_cards)
	{
		foreach (CFGImageExtension item in m_CardsPanelFrame)
		{
			item.IconNumber = (is_on_cards ? 1 : 0);
		}
		m_CardsPanelUp.IconNumber = (is_on_cards ? 1 : 0);
		m_InfoPanelUp.IconNumber = (is_on_cards ? 1 : 0);
		m_CardsPanelTxtG.gameObject.SetActive(is_on_cards);
		m_CardsPanelTxtW.gameObject.SetActive(!is_on_cards);
		m_InfoPanelTxtG.gameObject.SetActive(value: false);
		m_InfoPanelTxtW.gameObject.SetActive(value: true);
	}

	private void SetMouseListHighlight()
	{
		foreach (CFGImageExtension item in m_CardsPanelFrame)
		{
			item.IconNumber = 0;
		}
		m_CardsPanelUp.IconNumber = 1;
		m_InfoPanelUp.IconNumber = 1;
		m_CardsPanelTxtG.gameObject.SetActive(value: true);
		m_CardsPanelTxtW.gameObject.SetActive(value: false);
		m_InfoPanelTxtG.gameObject.SetActive(value: true);
		m_InfoPanelTxtW.gameObject.SetActive(value: false);
	}

	private void OnCardSelected(int Card)
	{
		m_CardMatrix[Card].SimulateClick();
	}

	public void OnIconIn(int nr)
	{
	}

	public void OnIconOver(int nr)
	{
		m_CardImg.gameObject.SetActive(value: false);
		UpdateAlphabet(null);
		if (m_CardsSetButtons[nr].m_DataString.ToLower() == "none")
		{
			m_CardName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hand_nohand");
			m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hand_nohand_desc");
		}
		else
		{
			CFGDef_Buff buff = CFGStaticDataContainer.GetBuff(m_CardsSetButtons[nr].m_DataString.ToLower());
			if (buff != null)
			{
				CFGBuff cFGBuff = new CFGBuff(buff, EBuffSource.CardHandBonus);
				if (cFGBuff != null)
				{
					m_CardName.text = cFGBuff.ToStrigName();
					m_CardDesc.text = cFGBuff.ToStringDesc(tactical: false);
				}
			}
		}
		m_BuffImg.IconNumber = m_CardsSetIcons[nr].IconNumber;
	}

	public void OnCharacterButtonClick(int nr)
	{
		m_LastCharActive = nr;
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			m_CharacterCardsBar[i].m_PanelNormal.gameObject.SetActive(value: true);
			m_CharacterCardsBar[i].m_PanelSelected.gameObject.SetActive(value: false);
			m_CharacterCardsBar[i].m_PanelNormal.gameObject.GetComponent<CanvasGroup>().interactable = true;
			m_CharacterCardsBar[i].m_PanelNormal.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
			m_CharacterCardsBar[i].m_PanelSelected.gameObject.GetComponent<CanvasGroup>().interactable = false;
			m_CharacterCardsBar[i].m_PanelSelected.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
			m_CharacterCardsBar[i].transform.position = m_BarPositions[i];
		}
		m_CharacterCardsBar[nr].m_PanelNormal.gameObject.SetActive(value: false);
		m_CharacterCardsBar[nr].m_PanelSelected.gameObject.SetActive(value: true);
		m_CharacterCardsBar[nr].m_PanelSelected.gameObject.GetComponent<CanvasGroup>().interactable = true;
		m_CharacterCardsBar[nr].m_PanelSelected.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
		if (nr == 0)
		{
			m_CharacterCardsBar[2].transform.position = new Vector3(m_BarPositions[2].x, m_BarPositions[2].y + 78f * m_Prc, m_BarPositions[2].z);
			m_CharacterCardsBar[3].transform.position = new Vector3(m_BarPositions[3].x, m_BarPositions[3].y + 156f * m_Prc, m_BarPositions[3].z);
			if (m_LastInput != EInputMode.Gamepad && !m_ClickButtonFromCard)
			{
				OnCharacterCardClick(0);
				OnCharacterCardHover(0);
			}
		}
		if (nr == 1)
		{
			m_CharacterCardsBar[1].transform.position = new Vector3(m_BarPositions[1].x, m_BarPositions[1].y + 78f * m_Prc, m_BarPositions[1].z);
			m_CharacterCardsBar[2].transform.position = new Vector3(m_BarPositions[2].x, m_BarPositions[2].y + 78f * m_Prc, m_BarPositions[2].z);
			m_CharacterCardsBar[3].transform.position = new Vector3(m_BarPositions[3].x, m_BarPositions[3].y + 156f * m_Prc, m_BarPositions[3].z);
			if (m_LastInput != EInputMode.Gamepad && !m_ClickButtonFromCard)
			{
				OnCharacterCardClick(5);
				OnCharacterCardHover(5);
			}
		}
		if (nr == 2)
		{
			m_CharacterCardsBar[1].transform.position = new Vector3(m_BarPositions[1].x, m_BarPositions[1].y + 78f * m_Prc, m_BarPositions[1].z);
			m_CharacterCardsBar[2].transform.position = new Vector3(m_BarPositions[2].x, m_BarPositions[2].y + 156f * m_Prc, m_BarPositions[2].z);
			m_CharacterCardsBar[3].transform.position = new Vector3(m_BarPositions[3].x, m_BarPositions[3].y + 156f * m_Prc, m_BarPositions[3].z);
			if (m_LastInput != EInputMode.Gamepad)
			{
				OnCharacterCardClick(10);
				OnCharacterCardHover(10);
			}
		}
		if (nr == 3)
		{
			m_CharacterCardsBar[1].transform.position = new Vector3(m_BarPositions[1].x, m_BarPositions[1].y + 78f * m_Prc, m_BarPositions[1].z);
			m_CharacterCardsBar[2].transform.position = new Vector3(m_BarPositions[2].x, m_BarPositions[2].y + 156f * m_Prc, m_BarPositions[2].z);
			m_CharacterCardsBar[3].transform.position = new Vector3(m_BarPositions[3].x, m_BarPositions[3].y + 234f * m_Prc, m_BarPositions[3].z);
			if (m_LastInput != EInputMode.Gamepad && !m_ClickButtonFromCard)
			{
				OnCharacterCardClick(15);
				OnCharacterCardHover(15);
			}
		}
		m_ClickButtonFromCard = false;
	}

	public override void Update()
	{
		base.Update();
		if (!m_IsInitialized)
		{
			for (int i = 0; i < m_CharacterCardsBar.Count; i++)
			{
				foreach (CFGButtonExtension characterCard in m_CharacterCardsBar[i].m_CharacterCards)
				{
					bool isSelected = characterCard.IsSelected;
					characterCard.IsSelected = false;
					characterCard.IsSelected = isSelected;
				}
				foreach (CFGButtonExtension item in m_CharacterCardsBar[i].m_CharacterCardsSelected)
				{
					bool isSelected2 = item.IsSelected;
					item.IsSelected = false;
					item.IsSelected = isSelected2;
				}
			}
			m_IsInitialized = true;
		}
		UpdateCharactersStats();
		if (CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu != null)
		{
			return;
		}
		if (!CFGButtonExtension.IsWaitingForClick)
		{
			m_Controller_Cards.UpdateInput();
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
			{
				m_ExitBtn.m_ButtonClickedCallback(0);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f && m_YButtonPad.enabled)
			{
				m_CancelBtn.m_ButtonClickedCallback(0);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				AssignCard(0);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
			{
				OnIconOver(m_LastCharActive);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
			{
				SelecteNextChar(-1);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
			{
				SelecteNextChar(1);
			}
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyY) > 0f)
			{
				m_YButtonPad.SimulateClickGraphicAndSoundOnlyDisable();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				m_AButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
			{
				m_XButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
			{
				m_BButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
			{
				m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
			{
				m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
			m_AButtonPad.gameObject.SetActive(flag);
			m_BButtonPad.gameObject.SetActive(flag);
			m_XButtonPad.gameObject.SetActive(flag);
			m_YButtonPad.gameObject.SetActive(flag);
			m_LBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_RBButtonPad.gameObject.SetActive(flag && CFGCharacterList.GetTeamCharactersList().Count > 1);
			m_ExitBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			SetGamepadListHighlight(is_on_cards: false);
			m_CardPanelDropable.gameObject.SetActive(value: false);
		}
		else
		{
			SetMouseListHighlight();
		}
		if (m_AssingBtn.m_Label.text == "ASSIGN")
		{
			if (m_SelectedCard != null && m_SkillCardsCharacterIcons[m_SelectedCard.m_Data].gameObject.activeSelf && m_LastCharActive > -1 && m_SkillCardsCharacterIcons[m_SelectedCard.m_Data].IconNumber == m_CharacterCardsBar[m_LastCharActive].m_CharacterImage.IconNumber)
			{
				m_AssingBtn.enabled = false;
			}
			else
			{
				m_AssingBtn.enabled = true;
			}
		}
		else
		{
			m_AssingBtn.enabled = m_SelectedCharacterCard.m_DataString != "Empty" && m_SelectedCharacterCard.m_DataString != "Locked";
		}
		for (int j = 0; j < m_CharacterCardsBar.Count; j++)
		{
			m_CharacterCardsBar[j].SetCardDatas();
			for (int k = 0; k < m_CharacterCardsBar[j].m_CharacterCards.Count; k++)
			{
				bool draggable = m_CharacterCardsBar[j].m_CharacterCards[k].m_DataString != "Locked" && m_CharacterCardsBar[j].m_CharacterCards[k].m_DataString != "Empty" && !m_CharacterCardsBar[j].m_CharacterCards[k].transform.parent.FindChild("lock").gameObject.activeSelf;
				m_CharacterCardsBar[j].m_CharacterCards[k].m_Draggable = draggable;
			}
			for (int l = 0; l < m_CharacterCardsBar[j].m_CharacterCardsSelected.Count; l++)
			{
				bool draggable2 = m_CharacterCardsBar[j].m_CharacterCards[l].m_DataString != "Locked" && m_CharacterCardsBar[j].m_CharacterCards[l].m_DataString != "Empty" && !m_CharacterCardsBar[j].m_CharacterCardsSelected[l].transform.parent.FindChild("lock").gameObject.activeSelf;
				m_CharacterCardsBar[j].m_CharacterCardsSelected[l].m_Draggable = draggable2;
			}
		}
		if (m_RefreshIcons && m_SelectedCard != null)
		{
			if (m_SkillCardsCharacterIcons[m_SelectedCard.m_Data].gameObject.activeSelf)
			{
				m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_releasecard");
			}
			else
			{
				m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_assigncard");
			}
			m_RefreshIcons = false;
		}
	}

	public void UnassignCard(int a)
	{
		if (m_SelectedCharacterCard != null && m_SelectedCharacterCard2 != null && a != 2)
		{
			if (m_SelectedCharacterCard.transform.parent.FindChild("lock").gameObject.activeSelf || m_SelectedCharacterCard2.transform.parent.FindChild("lock").gameObject.activeSelf)
			{
				return;
			}
			m_SelectedCharacterCard.m_DataString = "Empty";
			m_SelectedCharacterCard.IconNumber = 1;
			m_SelectedCharacterCard2.m_DataString = "Empty";
			m_SelectedCharacterCard2.IconNumber = 1;
			UpdateSkillCardsList();
			UpdateCharactersStats();
			CalcIcons();
		}
		if (a == 1)
		{
			m_SelectedCharacterCard = null;
			m_SelectedCharacterCard2 = null;
			CalcIcons();
		}
		if (a == 2)
		{
			m_SelectedCharacterCard.m_DataString = "Empty";
			m_SelectedCharacterCard.IconNumber = 1;
			m_SelectedCharacterCard2.m_DataString = "Empty";
			m_SelectedCharacterCard2.IconNumber = 1;
			m_SelectedCharacterCard = null;
			m_SelectedCharacterCard2 = null;
			CalcIcons();
		}
	}

	public void AssignCard(int a)
	{
		if (m_SelectedCard != null && m_SelectedCard.transform.parent.FindChild("lock").gameObject.activeSelf)
		{
			return;
		}
		if (CFGInput.LastReadInputDevice != EInputMode.Gamepad)
		{
			if (m_SelectedCard == null)
			{
				UnassignCard(0);
				return;
			}
		}
		else
		{
			foreach (CFGCharacterCardsBar item in m_CharacterCardsBar)
			{
				foreach (CFGButtonExtension characterCard in item.m_CharacterCards)
				{
					if (m_SelectedCard.m_DataString == characterCard.m_DataString)
					{
						m_SelectedCharacterCard = characterCard;
					}
				}
				foreach (CFGButtonExtension item2 in item.m_CharacterCardsSelected)
				{
					if (m_SelectedCard.m_DataString == item2.m_DataString)
					{
						m_SelectedCharacterCard2 = item2;
					}
				}
			}
			if (m_SelectedCharacterCard != null)
			{
				UnassignCard(1);
				return;
			}
		}
		if (m_SelectedCard != null && m_AssingBtn.enabled)
		{
			OnAssignButtonClick(0);
		}
	}

	private void SelecteNextChar(int Add)
	{
		if (Add == 0)
		{
			return;
		}
		if (Add < 0)
		{
			m_LastCharActive--;
		}
		else
		{
			m_LastCharActive++;
		}
		int num = 0;
		foreach (CFGCharacterCardsBar item in m_CharacterCardsBar)
		{
			if (item.gameObject.activeSelf)
			{
				num++;
			}
		}
		if (m_LastCharActive < 0)
		{
			m_LastCharActive = num - 1;
		}
		if (m_LastCharActive > num - 1)
		{
			m_LastCharActive = 0;
		}
		OnCharacterButtonClick(m_LastCharActive);
	}

	public void OnExitButton(int data)
	{
		foreach (KeyValuePair<string, CCardStatus> collectedCard in CFGInventory.CollectedCards)
		{
			collectedCard.Value.IsNew = false;
		}
		OnApplyButtonClick(0);
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadCardsPanell();
			CFGSingleton<CFGWindowMgr>.Instance.LoadCharacterScreen(m_CombatLoadout, m_Sequencer);
		}
		CFGJoyManager.ClearJoyActions();
	}

	public void OnCardDrag(int data)
	{
		m_DraggingCard = m_SkillCards[data];
		OnSkillCardClick(data);
		OnSkillCardHover(data);
	}

	public void OnCharacterCardDrag(int data)
	{
		m_CardPanelDropable.gameObject.SetActive(value: true);
		int num = ((data < 0 || data >= 5) ? ((data < 10) ? 1 : ((data >= 15) ? 3 : 2)) : 0);
		m_DraggedCharacterCard = m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num];
		m_DraggedCharacterCard2 = m_CharacterCardsBar[num].m_CharacterCardsSelected[data - 5 * num];
	}

	public void OnCharacterCardDragNonActive(int data)
	{
		int num = ((data < 0 || data >= 5) ? ((data < 10) ? 1 : ((data >= 15) ? 3 : 2)) : 0);
		m_DraggedCharacterCard = m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num];
		m_DraggedCharacterCard2 = m_CharacterCardsBar[num].m_CharacterCardsSelected[data - 5 * num];
		m_CardPanelDropable.gameObject.SetActive(value: true);
	}

	public void OnRealeseOutside(int a)
	{
		m_CardPanelDropable.gameObject.SetActive(value: false);
	}

	private CFGButtonExtension CheckForRemovableCard(CFGButtonExtension basecard, List<CFGButtonExtension> charcards, int pos)
	{
		for (int i = 0; i < charcards.Count; i++)
		{
			CFGButtonExtension cFGButtonExtension = charcards[i];
			if ((bool)cFGButtonExtension && cFGButtonExtension.m_DataString == basecard.m_DataString)
			{
				CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(pos);
				if (teamCharacter != null && teamCharacter.GetCard(i) == null)
				{
					return cFGButtonExtension;
				}
			}
		}
		return null;
	}

	private CFGButtonExtension CharactersWithCard(CFGButtonExtension basecard)
	{
		if (basecard == null)
		{
			return null;
		}
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			CFGButtonExtension cFGButtonExtension = CheckForRemovableCard(basecard, m_CharacterCardsBar[i].m_CharacterCards, i);
			if (cFGButtonExtension != null)
			{
				return cFGButtonExtension;
			}
			cFGButtonExtension = CheckForRemovableCard(basecard, m_CharacterCardsBar[i].m_CharacterCards, i);
			if (cFGButtonExtension != null)
			{
				return cFGButtonExtension;
			}
		}
		return null;
	}

	public void OnCardDrop(int data)
	{
		int num = ((data < 0 || data >= 5) ? ((data < 10) ? 1 : ((data >= 15) ? 3 : 2)) : 0);
		if (m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString == "Locked" || m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].transform.parent.FindChild("lock").gameObject.activeSelf)
		{
			return;
		}
		if (m_DraggedCharacterCard != null && m_DraggedCharacterCard.m_DataString == m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString && m_DraggedCharacterCard.m_DataString != "Empty" && m_DraggedCharacterCard2.m_DataString != "Empty" && m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString != "Empty" && m_DraggingCard == null)
		{
			m_DraggedCharacterCard = null;
			m_DraggedCharacterCard2 = null;
			return;
		}
		if (m_DraggedCharacterCard != null)
		{
			m_CharacterCardsBar[num].SetCharacterCard(data - 5 * num, m_DraggedCharacterCard.IconNumber, m_DraggedCharacterCard.m_DataString);
			m_CancelBtn.enabled = true;
			m_YButtonPad.enabled = true;
			m_DraggedCharacterCard.m_DataString = "Empty";
			m_DraggedCharacterCard.IconNumber = 1;
			m_DraggedCharacterCard2.m_DataString = "Empty";
			m_DraggedCharacterCard2.IconNumber = 1;
			m_DraggedCharacterCard = null;
			m_DraggedCharacterCard2 = null;
			UpdateSkillCardsList();
			UpdateCharactersStats();
			CalcIcons();
			return;
		}
		m_DraggedCharacterCard = null;
		m_DraggedCharacterCard2 = null;
		bool flag = false;
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			for (int j = 0; j < m_CharacterCardsBar[i].m_CharacterCards.Count; j++)
			{
				CFGButtonExtension cFGButtonExtension = m_CharacterCardsBar[i].m_CharacterCards[j];
				if ((bool)cFGButtonExtension && (bool)m_DraggingCard && cFGButtonExtension.m_DataString == m_DraggingCard.m_DataString)
				{
					flag = true;
				}
			}
			for (int k = 0; k < m_CharacterCardsBar[i].m_CharacterCardsSelected.Count; k++)
			{
				CFGButtonExtension cFGButtonExtension2 = m_CharacterCardsBar[i].m_CharacterCardsSelected[k];
				if ((bool)cFGButtonExtension2 && (bool)m_DraggingCard && cFGButtonExtension2.m_DataString == m_DraggingCard.m_DataString)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			foreach (CFGCharacterCardsBar item in m_CharacterCardsBar)
			{
				foreach (CFGButtonExtension characterCard in item.m_CharacterCards)
				{
					if (m_SelectedCard.m_DataString == characterCard.m_DataString)
					{
						m_SelectedCharacterCard = characterCard;
					}
				}
				foreach (CFGButtonExtension item2 in item.m_CharacterCardsSelected)
				{
					if (m_SelectedCard.m_DataString == item2.m_DataString)
					{
						m_SelectedCharacterCard2 = item2;
					}
				}
			}
			UnassignCard(2);
		}
		foreach (CFGCharacterCardsBar item3 in m_CharacterCardsBar)
		{
			foreach (CFGButtonExtension characterCard2 in item3.m_CharacterCards)
			{
				if (characterCard2.m_Data != data)
				{
					continue;
				}
				foreach (CFGButtonExtension skillCard in m_SkillCards)
				{
					if (skillCard.m_DataString == characterCard2.m_DataString)
					{
						m_SkillCardsCharacterIcons[skillCard.m_Data].gameObject.SetActive(value: false);
						m_SkillCardsCharacterIconsFrames[skillCard.m_Data].gameObject.SetActive(value: false);
					}
				}
			}
		}
		if ((bool)m_DraggingCard)
		{
			m_CharacterCardsBar[num].SetCharacterCard(data - 5 * num, m_DraggingCard.IconNumber, m_DraggingCard.m_DataString);
			m_CancelBtn.enabled = true;
			m_YButtonPad.enabled = true;
			CalcIcons();
			UpdateCharactersStats();
			UpdateSkillCardsCharIcons();
		}
		m_DraggingCard = null;
	}

	public void OnCharacterCardClick(int data)
	{
		int num = ((data < 0 || data >= 5) ? ((data < 10) ? 1 : ((data >= 15) ? 3 : 2)) : 0);
		if (m_LastCharActive != num)
		{
			m_ClickButtonFromCard = true;
			m_CardId = data;
			OnCharacterButtonClick(num);
		}
		m_AssingBtn.m_Label.text = "UNASSIGN";
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			foreach (CFGButtonExtension characterCard in m_CharacterCardsBar[i].m_CharacterCards)
			{
				characterCard.IsSelected = false;
			}
			foreach (CFGButtonExtension item in m_CharacterCardsBar[i].m_CharacterCardsSelected)
			{
				item.IsSelected = false;
			}
		}
		foreach (CFGButtonExtension skillCard in m_SkillCards)
		{
			skillCard.IsSelected = false;
		}
		m_SelectedCard = null;
		m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].IsSelected = true;
		m_CharacterCardsBar[num].m_CharacterCardsSelected[data - 5 * num].IsSelected = true;
		m_SelectedCharacterCard = m_CharacterCardsBar[num].m_CharacterCardsSelected[data - 5 * num];
		m_SelectedCharacterCard2 = m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num];
	}

	public void OnCharacterCardHover(int data)
	{
		int num = ((data < 0 || data >= 5) ? ((data < 10) ? 1 : ((data >= 15) ? 3 : 2)) : 0);
		m_CardImg.gameObject.SetActive(value: true);
		string dataString = m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString;
		if (dataString != string.Empty && dataString != "Empty" && dataString != "Locked")
		{
			string text = string.Empty;
			CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString);
			if (cardDefinition != null)
			{
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
				m_CardName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString);
				string text_id = cardDefinition.AbilityID.ToString().ToLower() + "_desc";
				if (string.Compare(cardDefinition.CardID, "blood_dlc1", ignoreCase: true) == 0)
				{
					text_id = "none_dlc_blood_desc";
				}
				if (string.Compare(cardDefinition.CardID, "lymph_dlc1", ignoreCase: true) == 0)
				{
					text_id = "none_dlc_lymph_desc";
				}
				m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.AbilityID.ToString().ToLower() + "_name"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id), text, text3);
			}
			m_CardImg.IconNumber = m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].IconNumber;
			UpdateAlphabet(m_CharacterCardsBar[num].m_CharacterCards[data - 5 * num].m_DataString);
		}
		else if (dataString == "Empty")
		{
			m_CardName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_empty");
			m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_empty_desc_crdscr");
			m_CardImg.IconNumber = 1;
			UpdateAlphabet(null);
		}
		else if (dataString == "Locked")
		{
			m_CardName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_locked");
			m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("card_slot_locked_desc");
			m_CardImg.IconNumber = 0;
			UpdateAlphabet(null);
		}
	}

	public void OnSkillCardClick(int data)
	{
		m_SelectedCharacterCard = null;
		m_SelectedCharacterCard2 = null;
		m_AssingBtn.m_Label.text = "ASSIGN";
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			foreach (CFGButtonExtension characterCard in m_CharacterCardsBar[i].m_CharacterCards)
			{
				characterCard.IsSelected = false;
			}
			foreach (CFGButtonExtension item in m_CharacterCardsBar[i].m_CharacterCardsSelected)
			{
				item.IsSelected = false;
			}
		}
		foreach (CFGButtonExtension skillCard in m_SkillCards)
		{
			skillCard.IsSelected = false;
		}
		m_SkillCards[data].IsSelected = true;
		m_SelectedCard = m_SkillCards[data];
	}

	public void OnSkillCardHover(int data)
	{
		m_CardImg.gameObject.SetActive(value: true);
		m_CardName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_SkillCards[data].m_DataString);
		CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(m_SkillCards[data].m_DataString);
		if (m_SkillCardsCharacterIcons[data].gameObject.activeSelf)
		{
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_releasecard");
		}
		else
		{
			m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_cardscreen_assigncard");
		}
		string text = string.Empty;
		if (cardDefinition != null)
		{
			if (CFGInventory.CollectedCards.ContainsKey(m_SkillCards[data].m_DataString))
			{
				CFGInventory.CollectedCards[m_SkillCards[data].m_DataString].IsNew = false;
				CFGImageExtension[] componentsInChildren = m_SkillCards[data].transform.parent.gameObject.GetComponentsInChildren<CFGImageExtension>();
				foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
				{
					if (cFGImageExtension.name == "New")
					{
						cFGImageExtension.gameObject.SetActive(value: false);
					}
				}
			}
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
			if (cardDefinition.AbilityID != ETurnAction.None)
			{
				m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.AbilityID.ToString().ToLower() + "_name"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cardDefinition.AbilityID.ToString().ToLower() + "_desc"), text, text3);
			}
			else
			{
				string text_id = "none_desc";
				if (string.Compare(cardDefinition.CardID, "blood_dlc1", ignoreCase: true) == 0)
				{
					text_id = "none_dlc_blood_desc";
				}
				if (string.Compare(cardDefinition.CardID, "lymph_dlc1", ignoreCase: true) == 0)
				{
					text_id = "none_dlc_lymph_desc";
				}
				string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id);
				m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("none_name"), localizedText, text, string.Empty);
			}
		}
		else
		{
			m_CardDesc.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_card_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("none_name"), CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("none_desc"), string.Empty, string.Empty);
		}
		m_CardImg.IconNumber = m_SkillCards[data].IconNumber;
		UpdateAlphabet(m_SkillCards[data].m_DataString);
	}

	public void OnApplyButtonClick(int a)
	{
		for (int i = 0; i < 4; i++)
		{
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(i);
			if (teamCharacter == null || teamCharacter.Definition == null)
			{
				continue;
			}
			for (int j = 0; j < 5; j++)
			{
				string dataString = m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString;
				if (dataString != "Locked" && dataString != "Empty")
				{
					CFGInventory.MoveCardToCharacter(teamCharacter.Definition.NameID, m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString, j);
				}
				if (dataString == "Empty" && teamCharacter.GetCard(j) != null)
				{
					CFGInventory.MoveCardFromCharacter(teamCharacter.Definition.NameID, j);
				}
			}
		}
		m_CancelBtn.enabled = false;
		m_YButtonPad.enabled = false;
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen.SetCurrentCharacter(from_pad: false);
		}
	}

	public void OnCancelButtonClick(int a)
	{
		UpdateCharacterList();
		UpdateSkillCardsList();
		m_CancelBtn.enabled = false;
		m_YButtonPad.enabled = false;
	}

	public void OnAssignButtonClick(int data)
	{
		if (m_SelectedCard == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			if (m_CharacterCardsBar[i].m_PanelSelected.gameObject.activeSelf)
			{
				num = i;
			}
		}
		int num2 = -1;
		for (int j = 0; j < m_CharacterCardsBar[num].m_CharacterCards.Count; j++)
		{
			if (m_CharacterCardsBar[num].m_CharacterCards[j].m_DataString == "Empty")
			{
				num2 = j;
				break;
			}
		}
		if (num2 != -1)
		{
			m_DraggingCard = m_SelectedCard;
			OnCardDrop(num2 + 5 * num);
		}
	}

	private void UpdateSkillCardsList()
	{
		for (int i = 0; i < 26; i++)
		{
			m_SkillCards[i].m_DataString = "Locked";
			m_SkillCards[i].enabled = false;
			m_SkillCards[i].IconNumber = 0;
			m_SkillCardsCharacterIcons[i].gameObject.SetActive(value: false);
			m_SkillCardsCharacterIconsFrames[i].gameObject.SetActive(value: false);
		}
		m_TotalCollectedCards = 0;
		foreach (KeyValuePair<string, CCardStatus> collectedCard in CFGInventory.CollectedCards)
		{
			CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(collectedCard.Key);
			if (cardDefinition == null || collectedCard.Value.Status == ECardStatus.NotCollected)
			{
				continue;
			}
			int num = 0;
			m_TotalCollectedCards++;
			if (cardDefinition.CardRank != ECardRank.Rank_Joker)
			{
				num = (int)((int)(cardDefinition.CardRank - 9) * 4 + (cardDefinition.CardColor - 1));
			}
			else if (string.Compare(cardDefinition.CardID, "Jok_red", ignoreCase: true) == 0)
			{
				num = 25;
			}
			else if (string.Compare(cardDefinition.CardID, "Jok_black", ignoreCase: true) == 0)
			{
				num = 24;
			}
			else if (string.Compare(cardDefinition.CardID, "blood_dlc1", ignoreCase: true) == 0)
			{
				num = 25;
			}
			else if (string.Compare(cardDefinition.CardID, "lymph_dlc1", ignoreCase: true) == 0)
			{
				num = 24;
			}
			if (num < 0 || num >= m_SkillCards.Count)
			{
				continue;
			}
			m_SkillCards[num].enabled = true;
			string textID = cardDefinition.TextID;
			m_SkillCards[num].m_DataString = textID;
			m_SkillCards[num].IconNumber = cardDefinition.ImageID;
			CFGImageExtension[] componentsInChildren = m_SkillCards[num].transform.parent.gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension in componentsInChildren)
			{
				if (cFGImageExtension.name == "New" && collectedCard.Value.IsNew)
				{
					cFGImageExtension.gameObject.SetActive(value: true);
				}
			}
		}
		for (int k = 0; k < 26; k++)
		{
			if (m_SkillCards[k].m_DataString == "Locked")
			{
				CFGImageExtension[] componentsInChildren2 = m_SkillCards[k].transform.parent.gameObject.GetComponentsInChildren<CFGImageExtension>();
				foreach (CFGImageExtension cFGImageExtension2 in componentsInChildren2)
				{
					if (cFGImageExtension2.name == "New")
					{
						cFGImageExtension2.gameObject.SetActive(value: false);
					}
				}
			}
			if (!CFGInventory.CollectedCards.ContainsKey(m_SkillCards[k].m_DataString) || CFGInventory.CollectedCards[m_SkillCards[k].m_DataString].IsNew)
			{
				continue;
			}
			CFGImageExtension[] componentsInChildren3 = m_SkillCards[k].transform.parent.gameObject.GetComponentsInChildren<CFGImageExtension>();
			foreach (CFGImageExtension cFGImageExtension3 in componentsInChildren3)
			{
				if (cFGImageExtension3.name == "New")
				{
					cFGImageExtension3.gameObject.SetActive(value: false);
				}
			}
		}
		UpdateSkillCardsCharIcons();
	}

	private void UpdateSkillCardsCharIcons(bool setLock = false)
	{
		for (int i = 0; i < m_CharacterCardsBar.Count; i++)
		{
			foreach (CFGButtonExtension characterCard in m_CharacterCardsBar[i].m_CharacterCards)
			{
				if (!(characterCard.m_DataString != "Empty") || !(characterCard.m_DataString != "Locked"))
				{
					continue;
				}
				foreach (CFGButtonExtension skillCard in m_SkillCards)
				{
					if (skillCard.m_DataString != string.Empty && skillCard.m_DataString == characterCard.m_DataString)
					{
						m_SkillCardsCharacterIcons[skillCard.m_Data].gameObject.SetActive(value: true);
						m_SkillCardsCharacterIconsFrames[skillCard.m_Data].gameObject.SetActive(value: true);
						m_SkillCardsCharacterIcons[skillCard.m_Data].IconNumber = m_CharacterCardsBar[i].m_CharacterImage.IconNumber;
						if (setLock)
						{
							SetCardLocked(skillCard, isLocked: true);
						}
					}
				}
			}
		}
		m_RefreshIcons = true;
	}

	private void UpdateCharacterList()
	{
		for (int i = 0; i < 4; i++)
		{
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(i);
			if (teamCharacter != null)
			{
				m_CharacterCardsBar[i].gameObject.SetActive(value: true);
				if (teamCharacter.Definition != null)
				{
					m_CharacterCardsBar[i].m_CharacterName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharacter.Definition.NameID);
					m_CharacterCardsBar[i].m_CharacterImage.IconNumber = teamCharacter.ImageIDX;
					m_CharacterCardsBar[i].m_CharacterImageSelected.IconNumber = teamCharacter.ImageIDX;
				}
				m_CharacterCardsBar[i].m_AimVal.text = teamCharacter.BuffedAim.ToString();
				m_CharacterCardsBar[i].m_DefVal.text = teamCharacter.BuffedDefense.ToString();
				m_CharacterCardsBar[i].m_MovementVal.text = teamCharacter.BuffedMovement.ToString();
				m_CharacterCardsBar[i].m_SightVal.text = teamCharacter.BuffedSight.ToString();
				m_CharacterCardsBar[i].m_HPBar.SetProgress(100);
				m_CharacterCardsBar[i].m_LuckBar.SetProgress(teamCharacter.Luck * 100 / teamCharacter.MaxLuck);
				m_CharacterCardsBar[i].m_HP.text = teamCharacter.BuffedMaxHP + "/" + teamCharacter.BuffedMaxHP;
				m_CharacterCardsBar[i].m_Luck.text = teamCharacter.Luck + "/" + teamCharacter.MaxLuck;
				for (int j = 0; j < 5; j++)
				{
					if (teamCharacter.UnlockedCardSlots < j + 1)
					{
						m_CharacterCardsBar[i].m_CharacterCards[j].m_VisualsDisabled = true;
						m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString = "Locked";
						m_CharacterCardsBar[i].m_CharacterCards[j].IconNumber = 0;
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].m_VisualsDisabled = true;
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].m_DataString = "Locked";
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].IconNumber = 0;
						continue;
					}
					m_CharacterCardsBar[i].m_CharacterCards[j].m_VisualsDisabled = false;
					m_CharacterCardsBar[i].m_CharacterCardsSelected[j].m_VisualsDisabled = false;
					CFGDef_Card card = teamCharacter.GetCard(j);
					if (card != null)
					{
						m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString = card.TextID;
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].m_DataString = card.TextID;
						m_CharacterCardsBar[i].m_CharacterCards[j].IconNumber = card.ImageID;
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].IconNumber = card.ImageID;
					}
					else
					{
						m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString = "Empty";
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].IconNumber = 1;
						m_CharacterCardsBar[i].m_CharacterCardsSelected[j].m_DataString = "Empty";
						m_CharacterCardsBar[i].m_CharacterCards[j].IconNumber = 1;
					}
				}
			}
			else
			{
				m_CharacterCardsBar[i].gameObject.SetActive(value: false);
			}
			CalcIcons();
		}
	}

	public void CalcIcons()
	{
		CFGDef_Card[] CardArr = new CFGDef_Card[5];
		for (int i = 0; i < 4; i++)
		{
			if (!m_CharacterCardsBar[i].IsActive())
			{
				continue;
			}
			for (int j = 0; j < 5; j++)
			{
				CardArr[j] = CFGStaticDataContainer.GetCardDefinition(m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString);
			}
			ECardHandBonus eCardHandBonus = CFGCharacterData.CalcHandBonus(ref CardArr);
			m_CardsSetIcons[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
			m_CardsSetIcons2[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
			if (eCardHandBonus != 0)
			{
				CFGDef_Buff buffHandBonus = CFGCharacterData.GetBuffHandBonus(eCardHandBonus);
				if (buffHandBonus != null)
				{
					m_CardsSetIcons[i].IconNumber = buffHandBonus.Icon;
					m_CardsSetIcons2[i].IconNumber = buffHandBonus.Icon;
					m_CardsSetButtons[i].m_DataString = buffHandBonus.BuffID;
				}
				else
				{
					m_CardsSetButtons[i].m_DataString = "none";
				}
			}
			else
			{
				m_CardsSetIcons[i].IconNumber = 1;
				m_CardsSetIcons2[i].IconNumber = 1;
				m_CardsSetButtons[i].m_DataString = eCardHandBonus.ToString();
			}
		}
	}

	public void UpdateCharactersStats()
	{
		for (int i = 0; i < 4; i++)
		{
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(i);
			if (teamCharacter == null)
			{
				continue;
			}
			m_CharacterCardsBar[i].gameObject.SetActive(value: true);
			if (teamCharacter.Definition != null)
			{
				m_CharacterCardsBar[i].m_CharacterName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharacter.Definition.NameID);
				m_CharacterCardsBar[i].m_CharacterImage.IconNumber = teamCharacter.ImageIDX;
				m_CharacterCardsBar[i].m_CharacterImageSelected.IconNumber = teamCharacter.ImageIDX;
			}
			int num = teamCharacter.BuffedAim;
			int num2 = teamCharacter.BuffedDefense;
			int num3 = teamCharacter.BuffedMaxHP;
			int num4 = teamCharacter.Hp;
			int num5 = teamCharacter.BuffedMovement;
			int num6 = teamCharacter.BuffedSight;
			int num7 = teamCharacter.Luck;
			int num8 = teamCharacter.BuffedMaxLuck;
			CFGDef_Card[] CardArr = new CFGDef_Card[5];
			for (int j = 0; j < 5; j++)
			{
				CardArr[j] = CFGStaticDataContainer.GetCardDefinition(m_CharacterCardsBar[i].m_CharacterCards[j].m_DataString);
			}
			ECardHandBonus eCardHandBonus = CFGCharacterData.CalcHandBonus(ref CardArr);
			CFGDef_Buff cFGDef_Buff = null;
			ECardHandBonus cardHandBonus = teamCharacter.CardHandBonus;
			CFGDef_Buff cFGDef_Buff2 = null;
			if (eCardHandBonus != 0 && eCardHandBonus != cardHandBonus)
			{
				cFGDef_Buff = CFGCharacterData.GetBuffHandBonus(eCardHandBonus);
				if (cFGDef_Buff != null)
				{
					num += cFGDef_Buff.Mod_Aim;
					num2 += cFGDef_Buff.Mod_Defense;
					num3 += cFGDef_Buff.Mod_MaxHP;
					num5 += cFGDef_Buff.Mod_Movement;
					num6 += cFGDef_Buff.Mod_Sight;
					num4 += cFGDef_Buff.Mod_MaxHP;
					num8 += cFGDef_Buff.Mod_MaxLuck;
					num7 += cFGDef_Buff.Mod_MaxLuck;
				}
			}
			if (cardHandBonus != 0 && eCardHandBonus != cardHandBonus)
			{
				cFGDef_Buff2 = CFGCharacterData.GetBuffHandBonus(cardHandBonus);
				if (cFGDef_Buff2 != null)
				{
					num -= cFGDef_Buff2.Mod_Aim;
					num2 -= cFGDef_Buff2.Mod_Defense;
					num3 -= cFGDef_Buff2.Mod_MaxHP;
					num5 -= cFGDef_Buff2.Mod_Movement;
					num6 -= cFGDef_Buff2.Mod_Sight;
					num4 -= cFGDef_Buff2.Mod_MaxHP;
					num8 -= cFGDef_Buff2.Mod_MaxLuck;
					num7 -= cFGDef_Buff2.Mod_MaxLuck;
				}
			}
			for (int k = 0; k < 5; k++)
			{
				CFGDef_Card card = teamCharacter.GetCard(k);
				bool flag = false;
				for (int l = 0; l < 5; l++)
				{
					string dataString = m_CharacterCardsBar[i].m_CharacterCards[k].m_DataString;
					CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(dataString);
					if (cardDefinition != null && card != null && card.CardID == cardDefinition.CardID)
					{
						flag = true;
						break;
					}
				}
				if (!flag && card != null)
				{
					num -= card.Aim;
					num2 -= card.Defense;
					num3 -= card.MaxHealth;
					num5 -= card.Movement;
					num6 -= card.Sight;
					num4 -= card.MaxHealth;
					num8 -= card.MaxLuck;
					num7 -= card.MaxLuck;
				}
			}
			for (int m = 0; m < 5; m++)
			{
				string dataString2 = m_CharacterCardsBar[i].m_CharacterCards[m].m_DataString;
				CFGDef_Card cardDefinition2 = CFGStaticDataContainer.GetCardDefinition(dataString2);
				if (cardDefinition2 == null)
				{
					continue;
				}
				bool flag2 = false;
				for (int n = 0; n < 5; n++)
				{
					CFGDef_Card card2 = teamCharacter.GetCard(n);
					if (card2 != null && cardDefinition2.CardID == card2.CardID)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					num += cardDefinition2.Aim;
					num2 += cardDefinition2.Defense;
					num3 += cardDefinition2.MaxHealth;
					num5 += cardDefinition2.Movement;
					num6 += cardDefinition2.Sight;
					num4 += cardDefinition2.MaxHealth;
					num8 += cardDefinition2.MaxLuck;
					num7 += cardDefinition2.MaxLuck;
				}
			}
			m_CharacterCardsBar[i].m_AimVal.text = num.ToString();
			m_CharacterCardsBar[i].m_DefVal.text = num2.ToString();
			m_CharacterCardsBar[i].m_MovementVal.text = num5.ToString();
			m_CharacterCardsBar[i].m_SightVal.text = num6.ToString();
			m_CharacterCardsBar[i].m_HPBar.SetProgress(100);
			if (num8 > 0)
			{
				m_CharacterCardsBar[i].m_LuckBar.SetProgress(num7 * 100 / num8);
			}
			else
			{
				m_CharacterCardsBar[i].m_LuckBar.SetProgress(0);
			}
			m_CharacterCardsBar[i].m_HP.text = num3 + "/" + num3;
			m_CharacterCardsBar[i].m_Luck.text = num7 + "/" + num8;
		}
	}

	private void SetCardLocked(CFGButtonExtension card, bool isLocked)
	{
		if (!(card == null))
		{
			if (!CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NonchangableCards))
			{
				isLocked = false;
			}
			card.transform.parent.Find("lock").gameObject.SetActive(isLocked);
			card.m_Draggable = !isLocked;
		}
	}

	private void UpdateAlphabet(string dataString)
	{
		if (m_CompabilityButtons == null || m_CompabilityButtons.Length != 7)
		{
			return;
		}
		for (int i = 0; i < m_CompabilityButtons.Length; i++)
		{
			if (!(m_CompabilityButtons[i] == null))
			{
				m_CompabilityButtons[i].gameObject.SetActive(value: false);
			}
		}
		if (string.IsNullOrEmpty(dataString))
		{
			return;
		}
		CFGDef_Card cardDefinition = CFGStaticDataContainer.GetCardDefinition(dataString);
		if (cardDefinition == null || CFGGame.DlcType == EDLC.None)
		{
			return;
		}
		string compatibility = cardDefinition.Compatibility;
		if (string.IsNullOrEmpty(compatibility))
		{
			return;
		}
		int num = 0;
		for (int j = 0; j < compatibility.Length; j++)
		{
			if (compatibility[j] >= 'a' && compatibility[j] <= 'z' && !(m_CompabilityButtons[num] == null))
			{
				m_CompabilityButtons[num].gameObject.SetActive(value: true);
				m_CompabilityButtons[num].IconNumber = compatibility[j] - 97;
				num++;
				if (num >= m_CompabilityButtons.Length)
				{
					break;
				}
			}
		}
	}
}
