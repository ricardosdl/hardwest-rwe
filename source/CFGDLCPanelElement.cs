using UnityEngine;

public class CFGDLCPanelElement : MonoBehaviour
{
	[SerializeField]
	private CFGButtonExtension m_CharacterIcon;

	[SerializeField]
	private CFGButtonExtension m_DecayIcon;

	[SerializeField]
	private CFGButtonExtension m_HandBuffIcon;

	[SerializeField]
	private RectTransform m_Rect;

	public Vector3 StartPosition { get; set; }

	public float Height
	{
		get
		{
			if (m_Rect == null)
			{
				return 0f;
			}
			return m_Rect.rect.height;
		}
	}

	private void Start()
	{
		if (!(m_CharacterIcon == null) && !(m_HandBuffIcon == null) && !(m_DecayIcon == null))
		{
			m_CharacterIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
			m_HandBuffIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
			m_DecayIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
		}
	}

	public void UpdateData(CFGCharacterData characterData)
	{
		if (characterData != null && !(m_CharacterIcon == null) && !(m_HandBuffIcon == null) && !(m_DecayIcon == null))
		{
			m_CharacterIcon.IconNumber = characterData.ImageIDX;
			m_CharacterIcon.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(characterData.Definition.NameID);
			CFGDef_Buff buffHandBonus = CFGCharacterData.GetBuffHandBonus(characterData.CardHandBonus);
			int iconNumber;
			string text_id;
			if (buffHandBonus != null)
			{
				iconNumber = buffHandBonus.Icon;
				text_id = buffHandBonus.BuffID;
			}
			else if (characterData.IsStateSet(ECharacterStateFlag.ImmuneToDecay))
			{
				iconNumber = 116;
				text_id = "gui_dlc1_tooltip_cannothands";
			}
			else
			{
				iconNumber = 1;
				text_id = "gui_dlc1_tooltip_no_hand";
			}
			m_HandBuffIcon.IconNumber = iconNumber;
			m_HandBuffIcon.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id);
			int totalDecayLevel = characterData.TotalDecayLevel;
			CFGDef_Buff bestDecayBuff = CFGStaticDataContainer.GetBestDecayBuff(totalDecayLevel);
			int iconNumber2;
			string text_id2;
			if (bestDecayBuff != null)
			{
				iconNumber2 = bestDecayBuff.Icon;
				text_id2 = bestDecayBuff.BuffID;
			}
			else
			{
				iconNumber2 = 116;
				text_id2 = "gui_dlc1_tooltip_decayimmunity";
			}
			m_DecayIcon.IconNumber = iconNumber2;
			m_DecayIcon.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id2);
		}
	}
}
