using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGCharacterIndicator : UIBehaviour
{
	public float m_OffsetV = 2f;

	public CFGImageExtension m_HoverFlash;

	public CFGImageExtension m_BgCover;

	public Image m_HpMask;

	public CFGImageExtension m_HpBar;

	public CFGImageExtension m_Ap;

	public CFGImageExtension m_Cover;

	public CFGImageExtension m_Frame;

	public Text m_HpText;

	public Text m_HpTextCenter;

	public CFGImageExtension m_GunpointActive;

	public CFGImageExtension m_SuspiciousActive;

	public CFGImageExtension m_AOETarget;

	public CFGImageExtension m_ShadowIcon;

	public List<CFGImageExtension> m_PassiveAbIconsActive = new List<CFGImageExtension>();

	public List<CFGImageExtension> m_PassiveAbIconsInactive = new List<CFGImageExtension>();

	public CFGImageExtension m_PositiveBuffFlag;

	public CFGImageExtension m_NegativeBuffFlag;

	public GameObject m_FlagFlashingAnimation;

	public Animator m_FlagFlashAnimation;

	public Animator m_FlagFlashAnimation2;

	public Animator m_FlashShadowIconAnimation;

	public List<Animator> m_FlashPassiveAbIcons = new List<Animator>();

	public List<GameObject> m_FlashingPassiveAbIcons = new List<GameObject>();

	public CanvasGroup m_AlphaGroup;

	public GameObject m_AllFlag;

	public Text m_CountingText;

	public CFGCharacter m_Character;

	private RectTransform m_RectTransform;

	private bool m_IsPlayer = true;

	private bool m_IsSelected = true;

	private bool m_IsHovered = true;

	private List<ETurnAction> m_Abilities = new List<ETurnAction>();

	private Dictionary<ETurnAction, bool> m_AbilitiesStates = new Dictionary<ETurnAction, bool>();

	public Animator m_TextAnim;

	public int Cover
	{
		set
		{
			m_Cover.gameObject.SetActive(value > 0);
			m_BgCover.gameObject.SetActive(value > 0);
			if (value > 0)
			{
				if (m_IsPlayer)
				{
					m_Cover.IconNumber = value - 1;
				}
				else
				{
					m_Cover.IconNumber = value + 1;
				}
				m_BgCover.IconNumber = (m_IsSelected ? 1 : 0);
			}
		}
	}

	public void SetScreenPosition(float x, float y)
	{
		int fontSize = (int)(20f * (float)Screen.width / 1920f);
		m_HpText.fontSize = fontSize;
		m_HpTextCenter.fontSize = fontSize;
		m_RectTransform.offsetMax = new Vector2(1f, 1f);
		m_RectTransform.offsetMin = new Vector2(0f, 0f);
		float num = m_BgCover.sprite.rect.width / 1920f * 0.5f;
		float num2 = m_BgCover.sprite.rect.height / 1080f;
		m_RectTransform.anchorMin = new Vector2(x - num, y);
		m_RectTransform.anchorMax = new Vector2(x + num, y + num2);
	}

	public void SetGunpointActiveImage(bool visible)
	{
		bool activeSelf = m_GunpointActive.gameObject.activeSelf;
		m_GunpointActive.gameObject.SetActive(visible);
		if (activeSelf != m_GunpointActive.gameObject.activeSelf)
		{
			m_TextAnim.SetTrigger("start");
		}
	}

	public void SetSuspiciousActiveImage(bool visible)
	{
		bool activeSelf = m_SuspiciousActive.gameObject.activeSelf;
		m_SuspiciousActive.gameObject.SetActive(visible);
		if (activeSelf != m_SuspiciousActive.gameObject.activeSelf)
		{
			m_TextAnim.SetTrigger("start");
		}
	}

	public void FlashPassiveAb(ETurnAction ab_type)
	{
		for (int i = 0; i < m_Abilities.Count; i++)
		{
			if (m_Abilities[i] == ab_type)
			{
				m_FlashPassiveAbIcons[i].SetTrigger("UseAbility");
				break;
			}
		}
	}

	public void SetFlashingPassiveAb(ETurnAction ab_type, bool enabled)
	{
		for (int i = 0; i < m_Abilities.Count; i++)
		{
			if (m_Abilities[i] == ab_type)
			{
				m_FlashingPassiveAbIcons[i].SetActive(enabled);
				break;
			}
		}
	}

	public void FlashFlagActiveAbility()
	{
		m_FlagFlashAnimation.SetTrigger("UseAbility");
	}

	public void FlashFlagActiveAbility2()
	{
		m_FlagFlashAnimation2.SetTrigger("UseAbility");
	}

	public void FlashShadowIcon()
	{
		m_FlagFlashAnimation.SetTrigger("UseAbility");
	}

	public void FlashShadow()
	{
		m_FlashShadowIconAnimation.SetTrigger("UseAbility");
	}

	public void LateUpdate()
	{
		if (m_Character == null || m_Character.IsDead || m_Character.gameObject == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Vector3 position = m_Character.transform.position;
		position.y += m_OffsetV;
		Vector3 vector = Camera.main.WorldToViewportPoint(position);
		bool flag = vector.z > 0f && vector.x >= 0f && vector.y >= 0f && vector.x <= 1f && vector.y <= 1f && ShouldBeVisible();
		m_AllFlag.SetActive(flag);
		if (flag)
		{
			SetScreenPosition(vector.x, vector.y);
			CFGSelectionManager instance = CFGSelectionManager.Instance;
			CFGTurnManager instance2 = CFGSingletonResourcePrefab<CFGTurnManager>.Instance;
			if (instance == null || instance2 == null)
			{
				return;
			}
			m_IsPlayer = m_Character.Owner != null && m_Character.Owner.IsPlayer;
			if (m_IsPlayer)
			{
				m_IsSelected = m_Character.IsSelected;
				m_HpBar.IconNumber = 0;
				m_Frame.IconNumber = (m_IsSelected ? 2 : 0);
				Cover = (int)m_Character.GetCoverState();
				m_Ap.gameObject.SetActive(value: true);
				if (m_Character.MaxActionPoints == 2 && !instance2.InSetupStage)
				{
					m_Ap.IconNumber = m_Character.ActionPoints;
				}
				else
				{
					m_Ap.IconNumber = m_Character.ActionPoints + 3;
				}
			}
			else
			{
				if (instance2.IsPlayerTurn)
				{
					m_IsSelected = m_Character == instance.TargetedCharacter;
					bool flag2 = (bool)instance.SelectedCharacter && instance.SelectedCharacter.VisibleEnemies.Contains(m_Character);
					m_HpBar.IconNumber = (flag2 ? 1 : 2);
					Cover = (int)(instance.SelectedCharacter ? CFGCharacter.GetTargetCover(instance.SelectedCharacter.CurrentCell, m_Character.CurrentCell) : ECoverType.NONE);
				}
				else
				{
					CFGAiOwner cFGAiOwner = m_Character.Owner as CFGAiOwner;
					m_IsSelected = (bool)cFGAiOwner && cFGAiOwner.CurrentCharacter == m_Character;
					m_HpBar.IconNumber = 1;
					Cover = 0;
				}
				m_Frame.IconNumber = ((!m_IsSelected) ? 1 : 3);
				m_Ap.gameObject.SetActive(value: false);
			}
			m_IsHovered = m_Character.IsUnderCursor || m_Character.CurrentCell == instance.CellUnderCursor;
			m_HoverFlash.gameObject.SetActive(m_IsHovered);
			m_AlphaGroup.alpha = ((!m_IsSelected && !m_IsHovered) ? 0.3f : 1f);
			int hp = m_Character.Hp;
			string text = ((!m_IsPlayer && (m_Character.BestDetectionType & EBestDetectionType.Visible) != EBestDetectionType.Visible) ? "?" : hp.ToString());
			if (!m_IsPlayer && m_Character.Hp == m_Character.MaxHp && CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.UnknownEnemyHealth))
			{
				text = "?";
			}
			if (m_IsPlayer && m_Character.GunpointState != EGunpointState.Target)
			{
				m_HpText.gameObject.SetActive(value: true);
				m_HpText.text = text;
				m_HpTextCenter.gameObject.SetActive(value: false);
			}
			else
			{
				m_HpText.gameObject.SetActive(value: false);
				m_HpTextCenter.gameObject.SetActive(value: true);
				m_HpTextCenter.text = text;
			}
			m_HpBar.fillAmount = (float)hp / (float)m_Character.MaxHp;
			SetGunpointActiveImage(m_Character.GunpointState == EGunpointState.Target);
			SetSuspiciousActiveImage(m_Character.AIState == EAIState.Suspicious);
			if (m_Character.GunpointState == EGunpointState.Target)
			{
				string text2 = m_CountingText.text;
				m_CountingText.text = m_Character.CharacterData.SubduedCount.ToString();
				if (text2 != m_CountingText.text)
				{
					m_TextAnim.SetTrigger("start");
				}
			}
			else if (m_Character.AIState == EAIState.Suspicious)
			{
				string text3 = m_CountingText.text;
				m_CountingText.text = m_Character.CharacterData.SuspicionLevel.ToString();
				if (text3 != m_CountingText.text)
				{
					m_TextAnim.SetTrigger("start");
				}
			}
			else
			{
				m_CountingText.text = string.Empty;
			}
			if (!m_ShadowIcon.gameObject.activeSelf && m_ShadowIcon.gameObject.activeSelf != m_Character.IsInShadow)
			{
				FlashShadow();
			}
			m_ShadowIcon.gameObject.SetActive(value: true);
			m_ShadowIcon.IconNumber = ((!m_Character.IsInShadow) ? 1 : 0);
			bool activeSelf = m_AOETarget.gameObject.activeSelf;
			m_AOETarget.gameObject.SetActive(instance.IsCharacterInAOETargetsList(m_Character));
			if (!activeSelf && m_AOETarget.gameObject.activeSelf)
			{
				FlashFlagActiveAbility();
			}
			bool active = false;
			bool active2 = false;
			if (m_Character != null && m_Character.Buffs != null)
			{
				foreach (CFGBuff buff in m_Character.Buffs)
				{
					if (buff != null)
					{
						if (buff.m_Def.Flag_Positive)
						{
							active = true;
						}
						if (buff.m_Def.Flag_Negative)
						{
							active2 = true;
						}
					}
				}
			}
			m_PositiveBuffFlag.gameObject.SetActive(active);
			m_NegativeBuffFlag.gameObject.SetActive(active2);
			m_Abilities.Clear();
			Dictionary<ETurnAction, bool> dictionary = new Dictionary<ETurnAction, bool>();
			int num = 0;
			if (m_Character.Abilities != null)
			{
				foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability2 in m_Character.Abilities)
				{
					CFGAbility ability = ability2.Value.Ability;
					if (ability == null || !ability.IsPassive)
					{
						continue;
					}
					m_PassiveAbIconsActive[num].IconNumber = ability.PassiveIconID;
					m_PassiveAbIconsInactive[num].IconNumber = ability.PassiveIconID;
					bool flag3 = IsAbilityActive(ability2.Key, ability);
					bool flag4;
					if (m_AbilitiesStates.ContainsKey(ability2.Key))
					{
						flag4 = m_AbilitiesStates[ability2.Key] != flag3;
						if (ability2.Key == ETurnAction.Vengeance && m_AbilitiesStates[ability2.Key])
						{
							flag4 = true;
						}
					}
					else
					{
						flag4 = true;
					}
					m_PassiveAbIconsActive[num].gameObject.SetActive(flag3);
					m_PassiveAbIconsInactive[num].gameObject.SetActive(!flag3);
					m_Abilities.Add(ability2.Key);
					if (flag4)
					{
						FlashPassiveAb(ability2.Key);
					}
					dictionary.Add(ability2.Key, flag3);
					num++;
					if (num <= 4)
					{
						continue;
					}
					break;
				}
			}
			for (int i = num; i < m_PassiveAbIconsActive.Count; i++)
			{
				m_PassiveAbIconsActive[i].gameObject.SetActive(value: false);
				m_PassiveAbIconsInactive[i].gameObject.SetActive(value: false);
			}
			m_AbilitiesStates = dictionary;
			if (m_Character.Abilities != null && m_Character.Abilities.ContainsKey(ETurnAction.Vengeance))
			{
				if (m_Character.CharacterData.HasBuff("halfdead"))
				{
					SetFlashingPassiveAb(ETurnAction.Vengeance, enabled: true);
				}
				else
				{
					SetFlashingPassiveAb(ETurnAction.Vengeance, enabled: false);
				}
			}
			if (m_Character.FlagNeedFlash)
			{
				FlashFlagActiveAbility();
				m_Character.FlagNeedFlash = false;
			}
			if (m_Character.FlagNeedFlash2)
			{
				FlashFlagActiveAbility2();
				m_Character.FlagNeedFlash2 = false;
			}
			if (m_Character.JinxedFlagNeedFlash)
			{
				FlashPassiveAb(ETurnAction.Jinx);
				m_Character.JinxedFlagNeedFlash = false;
			}
			if (m_Character.IntimidateFlagNeedFlash)
			{
				FlashPassiveAb(ETurnAction.Intimidate);
				m_Character.IntimidateFlagNeedFlash = false;
			}
			m_FlagFlashingAnimation.SetActive(m_Character.Hp < 2 || m_Character.FlagNeedFlashing);
		}
		else
		{
			m_FlagFlashingAnimation.SetActive(value: false);
		}
	}

	protected bool IsAbilityActive(ETurnAction ab_type, CFGAbility ability)
	{
		switch (ab_type)
		{
		case ETurnAction.Disguise:
			return CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage;
		case ETurnAction.Vampire:
			return m_Character.CharacterData.HasBuff("shadowregen") || m_Character.CharacterData.HasBuff("nightmareregen");
		case ETurnAction.Jinx:
		case ETurnAction.Intimidate:
		{
			List<CFGIAttackable> TargetList = new List<CFGIAttackable>();
			ability.GenerateTargetList(m_Character.CurrentCell, ref TargetList, m_Character.CurrentCell.WorldPosition);
			switch (ab_type)
			{
			case ETurnAction.Intimidate:
				return TargetList.Count > 0;
			case ETurnAction.Jinx:
				return m_Character.VisibleEnemies.Count > 0 && TargetList.Count > 0;
			}
			break;
		}
		}
		return ab_type switch
		{
			ETurnAction.Vengeance => m_Character.CharacterData.HasBuff("halfdead"), 
			ETurnAction.ShadowCloak => m_Character.IsShadowCloaked, 
			_ => true, 
		};
	}

	protected override void Start()
	{
		base.Start();
		m_RectTransform = base.gameObject.GetComponent<RectTransform>();
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance != null)
		{
			instance.ReparentMainPanels();
		}
		m_AllFlag.SetActive(value: false);
		SetGunpointActiveImage(visible: false);
		foreach (CFGImageExtension item in m_PassiveAbIconsActive)
		{
			item.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_PassiveAbIconsActive;
		}
		foreach (CFGImageExtension item2 in m_PassiveAbIconsInactive)
		{
			item2.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_PassiveAbIconsUnactive;
		}
		Canvas component = GetComponent<Canvas>();
		if (component != null)
		{
			component.overrideSorting = true;
			component.sortingOrder = 0;
		}
	}

	private bool ShouldBeVisible()
	{
		if (m_Character.gameObject.activeInHierarchy && m_Character.Owner != null)
		{
			CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
			if (sessionSingle == null || sessionSingle.GetMissionStats().finished)
			{
				return false;
			}
			CFGSelectionManager instance = CFGSelectionManager.Instance;
			if (instance == null)
			{
				return false;
			}
			if (m_Character.HideFlag)
			{
				return false;
			}
			if (instance.IsUsingRicochet)
			{
				return instance.PossibleTargets.Contains(m_Character);
			}
			return m_Character.Owner.IsPlayer || m_Character.VisibilityState != EBestDetectionType.NotDetected;
		}
		return false;
	}
}
