using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGTacticalCharacterDetailsPanel : CFGPanel
{
	public CFGButtonExtension m_RBButtonPad;

	public CFGButtonExtension m_LBButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public Text m_AbilitiesTxtW;

	public Text m_AbilitiesTxtG;

	public Text m_BuffsTxtW;

	public Text m_BuffsTxtG;

	public Text m_Title;

	public CFGImageExtension m_AbilitiesUp;

	public CFGImageExtension m_BuffsUp;

	public List<CFGButtonExtension> m_PlayersChars = new List<CFGButtonExtension>();

	public List<CFGImageExtension> m_PlayerImgs = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_PlayerImgsImprisoned = new List<CFGImageExtension>();

	public CFGImageExtension m_CharacterAvatar;

	public CFGImageExtension m_CharacterAvatarImprisoned;

	public Text m_CharacterName;

	public CFGMaskedProgressBar m_Luck;

	public CFGMaskedProgressBar m_HP;

	public Text m_LuckText;

	public Text m_HPText;

	public List<CFGImageExtension> m_StatsIcons = new List<CFGImageExtension>();

	public List<Text> m_StatsDescs = new List<Text>();

	public List<Text> m_StatsValues = new List<Text>();

	public List<CFGImageExtension> m_AbilityIcons = new List<CFGImageExtension>();

	public List<Text> m_AbilityDescs = new List<Text>();

	public List<CFGImageExtension> m_AbilitySource = new List<CFGImageExtension>();

	public List<Text> m_AbilityNames = new List<Text>();

	public List<Image> m_ItemsBg = new List<Image>();

	public List<CFGImageExtension> m_ItemsImgs = new List<CFGImageExtension>();

	public List<Text> m_ItemsNames = new List<Text>();

	public List<Text> m_ItemsText = new List<Text>();

	public CFGButtonExtension m_NextCharButton;

	public CFGButtonExtension m_PrevCharButton;

	public CFGButtonExtension m_CloseButton;

	public CFGCharacter m_SelectedCharacter;

	public bool m_Initialised;

	public Scrollbar m_ScrollbarBuffs;

	public GameObject m_BuffElement;

	public GameObject m_BuffsParent;

	public List<GameObject> m_BuffsList = new List<GameObject>();

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private Vector3 m_YPosBtn = default(Vector3);

	public List<GameObject> m_Heat = new List<GameObject>();

	public Text m_HeatText;

	private List<CFGCharacterData> m_CharacterList = new List<CFGCharacterData>();

	protected override void Start()
	{
		base.Start();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_CharacterList = CFGCharacterList.GetTeamCharactersListTactical();
		if (flag)
		{
			SetGamepadListHighlight();
		}
		else
		{
			SetMouseListHighlight();
		}
		m_LBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
		m_RBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_CloseButton.gameObject.SetActive(!flag);
			m_LBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
			m_RBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
			m_BButtonPad.gameObject.SetActive(flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		m_SelectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
		m_CloseButton.m_ButtonClickedCallback = OnButtonClose;
		m_NextCharButton.m_ButtonClickedCallback = NextCharacter;
		m_PrevCharButton.m_ButtonClickedCallback = PrevCharacter;
		m_CharacterAvatar.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		foreach (CFGImageExtension itemsImg in m_ItemsImgs)
		{
			itemsImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		}
		foreach (CFGImageExtension abilityIcon in m_AbilityIcons)
		{
			abilityIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesIcons;
		}
		foreach (CFGImageExtension playerImg in m_PlayerImgs)
		{
			playerImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		}
		CFGTimer.SetPaused_Gameplay(bPauseGameplay: true);
		SetCharacter();
		for (int i = 0; i < m_PlayersChars.Count; i++)
		{
			m_PlayersChars[i].gameObject.transform.parent.gameObject.SetActive(value: false);
		}
		m_YPosBtn = m_RBButtonPad.transform.position;
		for (int j = 0; j < m_CharacterList.Count; j++)
		{
			m_PlayersChars[j].m_ButtonClickedCallback = SelectCharacter;
			m_PlayersChars[j].gameObject.transform.parent.gameObject.SetActive(value: true);
			m_PlayerImgs[j].IconNumber = 0;
			if (m_CharacterList[j] != null && m_CharacterList[j].CurrentModel != null)
			{
				m_PlayerImgs[j].IconNumber = m_CharacterList[j].CurrentModel.ImageIDX;
				m_PlayerImgsImprisoned[j].gameObject.SetActive(m_CharacterList[j].CurrentModel.Imprisoned);
			}
			float width = m_PlayersChars[0].gameObject.GetComponent<RectTransform>().rect.width;
			m_RBButtonPad.transform.position = new Vector3(m_YPosBtn.x - (float)(6 - m_CharacterList.Count) * width * 1.2f, m_RBButtonPad.transform.position.y, m_RBButtonPad.transform.position.z);
		}
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		List<CFGCharacterData> characterList = m_CharacterList;
		int num2 = -1;
		for (int k = 0; k < characterList.Count; k++)
		{
			CFGCharacterData cFGCharacterData = characterList[k];
			if (cFGCharacterData != null && cFGCharacterData.CurrentModel == m_SelectedCharacter)
			{
				num2 = k;
				break;
			}
		}
		if (num2 >= 0)
		{
			m_PlayersChars[num2].IsSelected = true;
		}
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = false;
		}
	}

	public void SelectCharacter(int char_index)
	{
		foreach (CFGButtonExtension playersChar in m_PlayersChars)
		{
			playersChar.IsSelected = false;
		}
		m_PlayersChars[char_index].IsSelected = true;
		if (m_CharacterList[char_index] != null)
		{
			m_SelectedCharacter = m_CharacterList[char_index].CurrentModel;
			SetCharacter();
		}
	}

	public void NextCharacter(int a)
	{
		List<CFGCharacterData> characterList = m_CharacterList;
		int num = -1;
		for (int i = 0; i < characterList.Count; i++)
		{
			CFGCharacterData cFGCharacterData = characterList[i];
			if (cFGCharacterData != null && cFGCharacterData.CurrentModel == m_SelectedCharacter)
			{
				num = i + 1;
				break;
			}
		}
		if (characterList.Count <= num)
		{
			num = 0;
		}
		if (m_CharacterList[num] != null)
		{
			m_SelectedCharacter = m_CharacterList[num].CurrentModel;
			SetCharacter();
		}
	}

	public void PrevCharacter(int a)
	{
		List<CFGCharacterData> characterList = m_CharacterList;
		int num = -1;
		for (int i = 0; i < characterList.Count; i++)
		{
			CFGCharacterData cFGCharacterData = characterList[i];
			if (cFGCharacterData != null && cFGCharacterData.CurrentModel == m_SelectedCharacter)
			{
				num = i - 1;
				break;
			}
		}
		if (num < 0)
		{
			num = characterList.Count - 1;
		}
		if (m_CharacterList[num] != null)
		{
			m_SelectedCharacter = m_CharacterList[num].CurrentModel;
			SetCharacter();
		}
	}

	public void SetCharacter()
	{
		m_Initialised = false;
		if (!(m_SelectedCharacter != null) || m_SelectedCharacter.CharacterData == null)
		{
			return;
		}
		for (int i = 0; i < m_Heat.Count; i++)
		{
			m_Heat[i].SetActive(i < m_SelectedCharacter.CharacterData.TotalHeat);
		}
		m_CharacterAvatar.IconNumber = m_SelectedCharacter.ImageIDX;
		m_CharacterAvatarImprisoned.gameObject.SetActive(m_SelectedCharacter.Imprisoned);
		m_CharacterName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_SelectedCharacter.NameId);
		m_HPText.text = m_SelectedCharacter.CharacterData.Hp + "/" + m_SelectedCharacter.CharacterData.BuffedMaxHP;
		m_LuckText.text = m_SelectedCharacter.CharacterData.Luck + "/" + m_SelectedCharacter.MaxLuck;
		Debug.Log(m_SelectedCharacter.CharacterData.Luck * 100 / m_SelectedCharacter.CharacterData.MaxLuck);
		m_StatsIcons[0].IconNumber = 0;
		m_StatsIcons[1].IconNumber = 1;
		m_StatsIcons[2].IconNumber = 2;
		m_StatsIcons[3].IconNumber = 3;
		m_StatsValues[0].text = m_SelectedCharacter.CharacterData.BuffedAim.ToString();
		m_StatsValues[1].text = m_SelectedCharacter.CharacterData.BuffedDefense.ToString();
		m_StatsValues[2].text = m_SelectedCharacter.CharacterData.BuffedMovement.ToString();
		m_StatsValues[3].text = m_SelectedCharacter.CharacterData.BuffedSight.ToString();
		foreach (CFGImageExtension itemsImg in m_ItemsImgs)
		{
			itemsImg.IconNumber = 0;
			itemsImg.gameObject.SetActive(value: false);
		}
		foreach (Image item in m_ItemsBg)
		{
			item.gameObject.SetActive(value: true);
		}
		foreach (Text itemsName in m_ItemsNames)
		{
			itemsName.text = string.Empty;
		}
		foreach (Text item2 in m_ItemsText)
		{
			item2.text = string.Empty;
		}
		CFGGun firstWeapon = m_SelectedCharacter.FirstWeapon;
		if (firstWeapon != null && firstWeapon.m_Definition != null)
		{
			m_ItemsBg[0].gameObject.SetActive(value: false);
			m_ItemsImgs[0].IconNumber = firstWeapon.m_Definition.IconID;
			m_ItemsNames[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(firstWeapon.m_Definition.ItemID + "_name");
			CFGDef_Weapon definition = firstWeapon.m_Definition;
			if (definition != null)
			{
				string text = string.Empty;
				if (definition.AllowsFanning)
				{
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
				}
				if (definition.AllowsScoped)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
				}
				if (definition.AllowsCone)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
				}
				if (!definition.AllowsRicochet)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
				}
				if (!definition.ShotEndsTurn)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
				}
				if (definition.AimModifier != 0)
				{
					if (text != string.Empty)
					{
						text += ", ";
					}
					string text2 = text;
					text = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + definition.AimModifier;
				}
				m_ItemsText[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(definition.ItemID + "_desc"), text, definition.Damage.ToString(), definition.Heat.ToString(), ((definition.Damage / definition.HalfCoverDiv < 1) ? 1 : (definition.Damage / definition.HalfCoverDiv)).ToString(), ((definition.Damage / definition.FullCoverDiv < 1) ? 1 : (definition.Damage / definition.FullCoverDiv)).ToString(), definition.Damage.ToString());
			}
			m_ItemsImgs[0].gameObject.SetActive(value: true);
		}
		CFGGun secondWeapon = m_SelectedCharacter.SecondWeapon;
		if (secondWeapon != null && secondWeapon.m_Definition != null)
		{
			m_ItemsBg[1].gameObject.SetActive(value: false);
			m_ItemsImgs[1].IconNumber = secondWeapon.m_Definition.IconID;
			m_ItemsNames[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(secondWeapon.m_Definition.ItemID + "_name");
			CFGDef_Weapon definition2 = secondWeapon.m_Definition;
			if (definition2 != null)
			{
				string text3 = string.Empty;
				if (definition2.AllowsFanning)
				{
					text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_fanning");
				}
				if (definition2.AllowsScoped)
				{
					if (text3 != string.Empty)
					{
						text3 += ", ";
					}
					text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_scopedshot");
				}
				if (definition2.AllowsCone)
				{
					if (text3 != string.Empty)
					{
						text3 += ", ";
					}
					text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_coneshot");
				}
				if (!definition2.AllowsRicochet)
				{
					if (text3 != string.Empty)
					{
						text3 += ", ";
					}
					text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noricochet");
				}
				if (!definition2.ShotEndsTurn)
				{
					if (text3 != string.Empty)
					{
						text3 += ", ";
					}
					text3 += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_noendturn");
				}
				if (definition2.AimModifier != 0)
				{
					if (text3 != string.Empty)
					{
						text3 += ", ";
					}
					string text2 = text3;
					text3 = text2 + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + " " + definition2.AimModifier;
				}
				m_ItemsText[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("characterscreen_weapon_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(definition2.ItemID + "_desc"), text3, definition2.Damage.ToString(), definition2.Heat.ToString(), ((definition2.Damage / definition2.HalfCoverDiv < 1) ? 1 : (definition2.Damage / definition2.HalfCoverDiv)).ToString(), ((definition2.Damage / definition2.FullCoverDiv < 1) ? 1 : (definition2.Damage / definition2.FullCoverDiv)).ToString(), definition2.Damage.ToString());
			}
			m_ItemsImgs[1].gameObject.SetActive(value: true);
		}
		if (m_SelectedCharacter.CharacterData.Item1 != string.Empty)
		{
			CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_SelectedCharacter.CharacterData.Item1);
			if (itemDefinition != null)
			{
				m_ItemsBg[2].gameObject.SetActive(value: false);
				m_ItemsImgs[2].IconNumber = itemDefinition.ShopIcon;
				m_ItemsNames[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_name");
				m_ItemsText[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition.ItemID + "_desc");
				m_ItemsImgs[2].gameObject.SetActive(value: true);
			}
		}
		if (m_SelectedCharacter.CharacterData.Item2 != string.Empty)
		{
			CFGDef_Item itemDefinition2 = CFGStaticDataContainer.GetItemDefinition(m_SelectedCharacter.CharacterData.Item2);
			if (itemDefinition2 != null)
			{
				m_ItemsBg[3].gameObject.SetActive(value: false);
				m_ItemsImgs[3].IconNumber = itemDefinition2.ShopIcon;
				m_ItemsNames[3].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition2.ItemID + "_name");
				m_ItemsText[3].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition2.ItemID + "_desc");
				m_ItemsImgs[3].gameObject.SetActive(value: true);
			}
		}
		if (m_SelectedCharacter.CharacterData.Talisman != string.Empty)
		{
			CFGDef_Item itemDefinition3 = CFGStaticDataContainer.GetItemDefinition(m_SelectedCharacter.CharacterData.Talisman);
			if (itemDefinition3 != null)
			{
				m_ItemsBg[4].gameObject.SetActive(value: false);
				m_ItemsImgs[4].IconNumber = itemDefinition3.ShopIcon;
				m_ItemsNames[4].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition3.ItemID + "_name");
				string text4 = string.Empty;
				if (itemDefinition3.Mod_Aim != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_Aim <= 0) ? itemDefinition3.Mod_Aim.ToString() : ("+" + itemDefinition3.Mod_Aim)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_aim") + "; ";
				}
				if (itemDefinition3.Mod_Defense != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_Defense <= 0) ? itemDefinition3.Mod_Defense.ToString() : ("+" + itemDefinition3.Mod_Defense)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_defense") + "; ";
				}
				if (itemDefinition3.Mod_Sight != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_Sight <= 0) ? itemDefinition3.Mod_Sight.ToString() : ("+" + itemDefinition3.Mod_Sight)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_sight") + "; ";
				}
				if (itemDefinition3.Mod_Damage != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_Damage <= 0) ? itemDefinition3.Mod_Damage.ToString() : ("+" + itemDefinition3.Mod_Damage)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_damage") + "; ";
				}
				if (itemDefinition3.Mod_Movement != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_Movement <= 0) ? itemDefinition3.Mod_Movement.ToString() : ("+" + itemDefinition3.Mod_Movement)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_movement") + "; ";
				}
				if (itemDefinition3.Mod_MaxHP != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_MaxHP <= 0) ? itemDefinition3.Mod_MaxHP.ToString() : ("+" + itemDefinition3.Mod_MaxHP)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxhp") + "; ";
				}
				if (itemDefinition3.Mod_MaxLuck != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Mod_MaxLuck <= 0) ? itemDefinition3.Mod_MaxLuck.ToString() : ("+" + itemDefinition3.Mod_MaxLuck)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_maxluck") + "; ";
				}
				if (itemDefinition3.Heat != 0)
				{
					string text2 = text4;
					text4 = text2 + ((itemDefinition3.Heat <= 0) ? itemDefinition3.Heat.ToString() : ("+" + itemDefinition3.Heat)) + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_param_heat") + "; ";
				}
				m_ItemsText[4].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gui_item_desc", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(itemDefinition3.ItemID + "_desc"), text4);
				m_ItemsImgs[4].gameObject.SetActive(value: true);
			}
		}
		int num = 0;
		foreach (CFGImageExtension abilityIcon in m_AbilityIcons)
		{
			abilityIcon.gameObject.SetActive(value: false);
		}
		foreach (CFGImageExtension item3 in m_AbilitySource)
		{
			item3.IconNumber = 0;
		}
		foreach (Text abilityDesc in m_AbilityDescs)
		{
			abilityDesc.text = string.Empty;
		}
		foreach (Text abilityName in m_AbilityNames)
		{
			abilityName.text = string.Empty;
		}
		foreach (ETurnAction key in m_SelectedCharacter.Abilities.Keys)
		{
			if (num >= m_AbilityIcons.Count)
			{
				break;
			}
			switch (key)
			{
			case ETurnAction.Ricochet:
			case ETurnAction.ShadowCloak:
			case ETurnAction.Disguise:
			case ETurnAction.Transfusion:
			case ETurnAction.Dodge:
			case ETurnAction.Smell:
			case ETurnAction.Equalization:
			case ETurnAction.Vengeance:
			case ETurnAction.Finder:
			case ETurnAction.Crippler:
			case ETurnAction.Hearing:
			case ETurnAction.Vampire:
			case ETurnAction.Prayer:
			case ETurnAction.Jinx:
			case ETurnAction.RewardedKill:
			case ETurnAction.Courage:
			case ETurnAction.ShadowKill:
			case ETurnAction.Shriek:
			case ETurnAction.Cannibal:
			case ETurnAction.Penetrate:
			case ETurnAction.Intimidate:
			case ETurnAction.ArteryShot:
			case ETurnAction.MultiShot:
			case ETurnAction.Demon:
			{
				if (key == ETurnAction.Use_Item1 || key == ETurnAction.Use_Item2 || key == ETurnAction.Use_Talisman)
				{
					m_AbilityIcons[num].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesUsableIcons;
				}
				else
				{
					m_AbilityIcons[num].m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_AbilitiesIcons;
				}
				m_AbilityIcons[num].gameObject.SetActive(value: true);
				m_AbilityIcons[num].IconNumber = 0;
				CFGAbility ability = m_SelectedCharacter.Abilities[key].Ability;
				if (ability != null)
				{
					m_AbilityIcons[num].IconNumber = ability.IconID;
				}
				switch (key)
				{
				case ETurnAction.Gunpoint:
					m_AbilityIcons[num].IconNumber = 4;
					break;
				case ETurnAction.Ricochet:
					m_AbilityIcons[num].IconNumber = 7;
					break;
				case ETurnAction.ShadowCloak:
					m_AbilityIcons[num].IconNumber = 6;
					break;
				}
				m_AbilitySource[num].IconNumber = 0;
				if (key != ETurnAction.Use_Item1 && key != ETurnAction.Use_Item2 && key != ETurnAction.Use_Talisman)
				{
					m_AbilityNames[num].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(key.ToString().ToLower() + "_name");
					m_AbilityDescs[num].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(key.ToString().ToLower() + "_desc");
				}
				else if (m_SelectedCharacter.GetAbility(key) is CFGAbility_Item cFGAbility_Item)
				{
					m_AbilityNames[num].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cFGAbility_Item.TextID);
					m_AbilityDescs[num].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cFGAbility_Item.TextID + "_desc");
				}
				num++;
				break;
			}
			}
		}
		foreach (GameObject buffs in m_BuffsList)
		{
			Object.Destroy(buffs);
		}
		m_BuffsList.Clear();
		int num2 = 0;
		float num3 = 1f;
		float num4 = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		float num5 = 0.18f;
		if (num4 >= 1f)
		{
			num5 = 0.15f;
		}
		if (m_SelectedCharacter.Buffs.Count > 6)
		{
			num3 += (float)(m_SelectedCharacter.Buffs.Count - 6) * num5 * num4;
		}
		RectTransform component = m_BuffsParent.GetComponent<RectTransform>();
		component.offsetMax = new Vector2(1f, 1f);
		component.offsetMin = new Vector2(0f, 0f);
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(num3, 1f);
		foreach (CFGBuff buff in m_SelectedCharacter.Buffs)
		{
			SpawnBuff(buff.m_Def.Icon, (int)buff.m_Source, buff.ToStrigName(), buff.ToStringDesc(tactical: true), num2);
			num2++;
		}
	}

	public void SpawnBuff(int icon, int icon_source, string buff_name, string buff_desc, int buff_nr)
	{
		float num = 0f;
		float num2 = 250f;
		if (Screen.width > 1920)
		{
			num2 = 270f;
		}
		if (m_SelectedCharacter.Buffs.Count > 6)
		{
			num = (m_SelectedCharacter.Buffs.Count - 6) * 20;
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
	}

	public void OnButtonClose(int a)
	{
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExploratorButtons.m_OptionsButton.enabled = true;
		}
		CFGTimer.SetPaused_Gameplay(bPauseGameplay: false);
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadTacticalCharacterDetails();
		}
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component)
		{
			component.ResetControllerToDefault();
		}
	}

	public override void SetLocalisation()
	{
		m_StatsDescs[0].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("aim_tactical_details");
		m_StatsDescs[1].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("defense_tactical_details");
		m_StatsDescs[2].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("movement_tactical_details");
		m_StatsDescs[3].text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("sight_tactical_details");
		m_AbilitiesTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("abilities_tactical_details");
		m_AbilitiesTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("abilities_tactical_details");
		m_BuffsTxtW.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("buffs_tactical_details");
		m_BuffsTxtG.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("buffs_tactical_details");
		m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_title");
		m_HeatText.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("hud_heat");
		m_CloseButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_close");
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("character_screen_close");
	}

	public void SetMouseListHighlight()
	{
		m_AbilitiesTxtW.gameObject.SetActive(value: false);
		m_AbilitiesTxtG.gameObject.SetActive(value: true);
		m_BuffsTxtW.gameObject.SetActive(value: false);
		m_BuffsTxtG.gameObject.SetActive(value: true);
		m_AbilitiesUp.IconNumber = 1;
		m_BuffsUp.IconNumber = 1;
	}

	public void SetGamepadListHighlight()
	{
		m_AbilitiesTxtW.gameObject.SetActive(value: true);
		m_AbilitiesTxtG.gameObject.SetActive(value: false);
		m_BuffsTxtW.gameObject.SetActive(value: true);
		m_BuffsTxtG.gameObject.SetActive(value: false);
		m_AbilitiesUp.IconNumber = 0;
		m_BuffsUp.IconNumber = 0;
	}

	public override void Update()
	{
		base.Update();
		if (!m_Initialised)
		{
			m_Luck.SetProgress(m_SelectedCharacter.CharacterData.Luck * 100 / m_SelectedCharacter.CharacterData.MaxLuck);
			m_HP.SetProgress(m_SelectedCharacter.CharacterData.Hp * 100 / m_SelectedCharacter.CharacterData.BuffedMaxHP);
			m_Initialised = true;
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (flag)
		{
			SetGamepadListHighlight();
		}
		else
		{
			SetMouseListHighlight();
		}
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_CloseButton.gameObject.SetActive(!flag);
			m_LBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
			m_RBButtonPad.gameObject.SetActive(flag && m_CharacterList.Count > 1);
			m_BButtonPad.gameObject.SetActive(flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (Input.GetKeyUp(KeyCode.Escape) || CFGInput.IsActivated(EActionCommand.ToggleCharacterInfo) || CFGJoyManager.IsActivated(EJoyAction.Default_Cancel) || CFGJoyManager.IsActivated(EJoyAction.Exit))
		{
			m_CloseButton.SimulateClick(bDelayed: true);
			m_BButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.IsActivated(EJoyAction.MenuRight))
		{
			List<CFGCharacterData> characterList = m_CharacterList;
			int num = -1;
			for (int i = 0; i < characterList.Count; i++)
			{
				CFGCharacterData cFGCharacterData = characterList[i];
				if (cFGCharacterData != null && cFGCharacterData.CurrentModel == m_SelectedCharacter)
				{
					num = i + 1;
					break;
				}
			}
			if (characterList.Count <= num)
			{
				num = 0;
			}
			m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
			m_PlayersChars[num].SimulateClickGraphicAndSoundOnly();
			m_PlayersChars[num].m_ButtonClickedCallback(num);
		}
		if (CFGJoyManager.IsActivated(EJoyAction.MenuLeft))
		{
			List<CFGCharacterData> characterList2 = m_CharacterList;
			int num2 = -1;
			for (int j = 0; j < characterList2.Count; j++)
			{
				CFGCharacterData cFGCharacterData2 = characterList2[j];
				if (cFGCharacterData2 != null && cFGCharacterData2.CurrentModel == m_SelectedCharacter)
				{
					num2 = j - 1;
					break;
				}
			}
			if (num2 < 0)
			{
				num2 = characterList2.Count - 1;
			}
			m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
			m_PlayersChars[num2].SimulateClickGraphicAndSoundOnly();
			m_PlayersChars[num2].m_ButtonClickedCallback(num2);
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
}
