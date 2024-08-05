using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGCharacterCardsBar : UIBehaviour
{
	public CFGImageExtension m_CharacterImage;

	public CFGImageExtension m_CharacterImageSelected;

	public List<CFGButtonExtension> m_CharacterCards = new List<CFGButtonExtension>();

	public List<CFGButtonExtension> m_CharacterCardsSelected = new List<CFGButtonExtension>();

	public Text m_CharacterName;

	public CFGMaskedProgressBar m_HPBar;

	public CFGMaskedProgressBar m_LuckBar;

	public Text m_HP;

	public Text m_Luck;

	public Text m_AimName;

	public Text m_DefName;

	public Text m_SightName;

	public Text m_MovementName;

	public Text m_AimVal;

	public Text m_DefVal;

	public Text m_SightVal;

	public Text m_MovementVal;

	public CFGImageExtension m_CardsSetIcon;

	public GameObject m_PanelNormal;

	public GameObject m_PanelSelected;

	public int m_Data;

	public CFGButtonExtension m_CharacterButton;

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < m_CharacterCards.Count; i++)
		{
			m_CharacterCards[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
			m_CharacterCardsSelected[i].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
		}
		m_CharacterImage.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_CharacterImageSelected.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_AimName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("aim_tactical_details");
		m_DefName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("defense_tactical_details");
		m_MovementName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("movement_tactical_details");
		m_SightName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("sight_tactical_details");
	}

	public void SetCardDatas()
	{
		for (int i = 0; i < m_CharacterCards.Count; i++)
		{
			m_CharacterCards[i].m_Data = i + 5 * m_Data;
			m_CharacterCardsSelected[i].m_Data = i + 5 * m_Data;
		}
	}

	public void ToggleSelect(bool selected)
	{
		m_PanelNormal.SetActive(!selected);
		m_PanelSelected.SetActive(selected);
	}

	public void SetCharacterImage(int img_nr)
	{
		m_CharacterImage.IconNumber = img_nr;
		m_CharacterImageSelected.IconNumber = img_nr;
	}

	public void SetCharacterCard(int nr, int value, string text)
	{
		m_CharacterCards[nr].IconNumber = value;
		m_CharacterCards[nr].m_DataString = text;
		m_CharacterCardsSelected[nr].IconNumber = value;
		m_CharacterCardsSelected[nr].m_DataString = text;
	}
}
