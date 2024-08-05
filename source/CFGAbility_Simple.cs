using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CFGAbility_Simple : CFGAbility
{
	public CFGDef_Ability m_Definition;

	public override int PassiveIconID
	{
		get
		{
			if (m_Definition == null)
			{
				return 0;
			}
			return m_Definition.AbilityID switch
			{
				ETurnAction.Disguise => 0, 
				ETurnAction.Vengeance => 1, 
				ETurnAction.Crippler => 2, 
				ETurnAction.Hearing => 3, 
				ETurnAction.Vampire => 4, 
				ETurnAction.Jinx => 5, 
				ETurnAction.ShadowCloak => 6, 
				ETurnAction.Intimidate => 7, 
				_ => 0, 
			};
		}
	}

	public override string TextID
	{
		get
		{
			if (m_Definition == null)
			{
				return "Undefined item";
			}
			return m_Definition.AbilityID.ToString();
		}
	}

	public CFGAbility_Simple(ETurnAction Ability, int UseLimit = int.MinValue, int Cooldown = 0)
	{
		m_Definition = CFGStaticDataContainer.GetAbilityDef(Ability);
		m_SelectableTargets = eTargetableType.None;
		m_AOE_Type = eAOE_Type.None;
		m_AffectedTypes = eTargetableType.None;
		m_ConeAngleDOTReq = 0f;
		m_CooldownLeft = 0;
		if (m_Definition != null)
		{
			m_Cooldown = m_Definition.Cooldown;
			m_bNeedWeapon = m_Definition.NeedWeapon;
			m_bUseLOS = m_Definition.UseLOS;
			m_bUseLOF = m_Definition.UseLOF;
			m_RequiredTargetStates = eTargetStates.None;
			m_bIsInstant = m_Definition.IsInstant;
			m_bIsPassive = m_Definition.IsPassive;
			m_bRotateTowardEnemy = m_Definition.FaceTarget;
			m_IconID = m_Definition.IconID;
			if (m_Definition.TargetState.Contains("w"))
			{
				m_RequiredTargetStates |= eTargetStates.Wounded;
			}
			if (m_Definition.TargetState.Contains("u"))
			{
				m_RequiredTargetStates |= eTargetStates.Unlooted;
			}
			if (m_Definition.TargetState.Contains("s"))
			{
				m_RequiredTargetStates |= eTargetStates.InShadow;
			}
			if (m_Definition.TargetState.Contains("d"))
			{
				m_RequiredTargetStates |= eTargetStates.Dead;
			}
			switch (m_Definition.Affected)
			{
			case CFGDef_Ability.EAffected.Enemy:
				m_SelectableTargets = eTargetableType.Enemy;
				m_AffectedTypes = eTargetableType.Enemy;
				break;
			case CFGDef_Ability.EAffected.Everyone:
				m_SelectableTargets = eTargetableType.Self;
				m_AOE_Type = eAOE_Type.Everyone;
				m_AffectedTypes = eTargetableType.Self | eTargetableType.Enemy | eTargetableType.Friendly;
				break;
			case CFGDef_Ability.EAffected.Friend:
				m_SelectableTargets = eTargetableType.Friendly;
				m_AffectedTypes = eTargetableType.Friendly;
				break;
			case CFGDef_Ability.EAffected.Self:
				m_SelectableTargets = eTargetableType.Self;
				m_AffectedTypes = eTargetableType.Self;
				break;
			case CFGDef_Ability.EAffected.Self_Sphere:
				m_SelectableTargets = eTargetableType.Self;
				m_AffectedTypes = eTargetableType.Enemy;
				m_AOE_Type = eAOE_Type.Sphere;
				break;
			case CFGDef_Ability.EAffected.Visible_Enemies:
				m_SelectableTargets = eTargetableType.Self;
				m_AOE_Type = eAOE_Type.VisibleEnemies;
				m_AffectedTypes = eTargetableType.Enemy;
				break;
			case CFGDef_Ability.EAffected.Anyone:
				m_SelectableTargets = eTargetableType.Enemy | eTargetableType.Friendly;
				m_AffectedTypes = eTargetableType.Enemy | eTargetableType.Friendly;
				break;
			}
		}
		m_WorksOnTheSameFloorOnly = false;
		m_MaxUsesPerTactical = 0;
		if (Cooldown > -1)
		{
			m_CooldownLeft = Cooldown;
		}
		if (UseLimit < 0)
		{
			m_UsesPerTacticalLeft = m_MaxUsesPerTactical;
		}
		else
		{
			m_UsesPerTacticalLeft = UseLimit;
		}
	}

	public override bool IsSilent()
	{
		if (m_Definition == null)
		{
			return true;
		}
		return m_Definition.IsSilent;
	}

	protected override float GetDelay()
	{
		if (m_Definition == null)
		{
			return 0f;
		}
		return m_Definition.Delay;
	}

	public override float GetRange()
	{
		if (m_Definition == null)
		{
			return 0f;
		}
		switch (m_Definition.Affected)
		{
		case CFGDef_Ability.EAffected.Everyone:
		case CFGDef_Ability.EAffected.Visible_Enemies:
			return 10000f;
		default:
			return m_Definition.Range;
		}
	}

	public override float GetEffectVal()
	{
		if (m_Definition == null)
		{
			return 0f;
		}
		return m_Definition.EffectValue;
	}

	public override float GetAOERadiusOrAngle()
	{
		if (m_Definition == null)
		{
			return 0f;
		}
		CFGDef_Ability.EAffected affected = m_Definition.Affected;
		if (affected == CFGDef_Ability.EAffected.Self_Sphere)
		{
			return m_Definition.Range;
		}
		return 0f;
	}

	public override int GetLuckCost()
	{
		if (m_Definition == null)
		{
			return 0;
		}
		return m_Definition.CostLuck;
	}

	protected override bool OnUse()
	{
		if (m_Definition == null || m_Parent == null)
		{
			return false;
		}
		if (m_Definition.IsPassive)
		{
			Debug.LogWarning("Cannot activate passive ability: " + m_Definition.AbilityID);
			return false;
		}
		bool flag = false;
		switch (m_Definition.AbilityID)
		{
		case ETurnAction.Courage:
			flag = ApplyCourage();
			break;
		case ETurnAction.Transfusion:
			flag = ApplyTransfusion();
			break;
		case ETurnAction.Smell:
			flag = ApplySmell();
			break;
		case ETurnAction.Equalization:
			flag = ApplyEqualization();
			break;
		case ETurnAction.Finder:
			flag = ApplyFinder();
			break;
		case ETurnAction.Prayer:
			flag = ApplyPrayer();
			break;
		case ETurnAction.Dodge:
			flag = ApplyDodge();
			break;
		case ETurnAction.ShadowKill:
			flag = ApplyShadowKill();
			break;
		case ETurnAction.Shriek:
			flag = ApplyShriek();
			break;
		case ETurnAction.Cannibal:
			flag = ApplyCannibal();
			break;
		case ETurnAction.Penetrate:
			flag = ApplyPenetrate();
			break;
		case ETurnAction.ArteryShot:
			flag = ApplyArteryShot();
			break;
		case ETurnAction.MultiShot:
			flag = ApplyMultiShot();
			break;
		case ETurnAction.Demon:
			flag = ApplyDemon();
			break;
		case ETurnAction.RewardedKill:
			flag = ApplyRewardedKill();
			break;
		}
		flag = true;
		m_Parent.Luck -= m_Definition.CostLuck;
		return flag;
	}

	private bool ApplyRewardedKill()
	{
		if ((bool)m_Parent && m_Parent.CharacterData != null)
		{
			m_Parent.CharacterData.AddBuff("rewardedkill", EBuffSource.Ability);
		}
		StartDelay();
		return true;
	}

	private bool ApplyDodge()
	{
		if ((bool)m_Parent && m_Parent.CharacterData != null)
		{
			m_Parent.CharacterData.AddBuff("dodge", EBuffSource.Ability);
		}
		StartDelay();
		return true;
	}

	private bool ApplyShadowKill()
	{
		StartDelay();
		if (m_Target == null || m_Parent == null)
		{
			return false;
		}
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData == null || CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ShadowKillBullet == null)
		{
			return false;
		}
		Vector3 position = m_Parent.Position;
		position.y += 1.2f;
		CFGBullet cFGBullet = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ShadowKillBullet, position, Quaternion.identity) as CFGBullet;
		CFGCharacter tgtchar = m_Target as CFGCharacter;
		if (tgtchar != null)
		{
			cFGBullet.m_OnBulletHitCallback = (CFGBullet.OnBulletHit)Delegate.Combine(cFGBullet.m_OnBulletHitCallback, (CFGBullet.OnBulletHit)delegate
			{
				CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_ShadowKill, tgtchar.Transform);
			});
		}
		cFGBullet.InitBullet(m_Parent, m_Target, bHit: true, m_Definition.EffectValue, 100);
		return true;
	}

	private bool ApplyShriek()
	{
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayShriek(OnShriek);
		return true;
	}

	private void OnShriek()
	{
		foreach (CFGIAttackable otherTarget in m_OtherTargets)
		{
			otherTarget.TakeDamage(m_Definition.EffectValue, m_Parent, bSilent: false);
		}
		CFGWindowObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(CFGWindowObject)) as CFGWindowObject[];
		Vector3 position = m_Parent.Position;
		if (array != null)
		{
			CFGWindowObject[] array2 = array;
			foreach (CFGWindowObject cFGWindowObject in array2)
			{
				float num = Vector3.Distance(cFGWindowObject.transform.position, position);
				if (num < m_Definition.Range)
				{
					cFGWindowObject.BreakWindow(position);
				}
			}
		}
		if (m_Parent == null || CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ShriekFX == null)
		{
			OnEndAction();
			return;
		}
		Transform transform = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ShriekFX, m_Parent.Position, m_Parent.Rotation) as Transform;
		transform.SetParent(m_Parent.transform);
		CFGParticleSystemSettings component = transform.GetComponent<CFGParticleSystemSettings>();
		if (component == null)
		{
			OnEndAction();
		}
		else
		{
			component.m_OnDestroyCallback = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(component.m_OnDestroyCallback, new CFGCharacterAnimator.OnEventDelegate(OnEndAction));
		}
	}

	private bool ApplyCannibal()
	{
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if (cFGCharacter == null)
		{
			StartDelay();
			return false;
		}
		m_Parent.Heal(m_Definition.EffectValue, bSilent: false);
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayCannibal(OnCannibal, OnEndAction);
		if (m_Parent.CharacterData != null)
		{
			m_Parent.CharacterData.AddBuff("wellfed", EBuffSource.Ability);
		}
		return true;
	}

	private void OnCannibal()
	{
		if (!(m_Parent == null))
		{
			m_Parent.gameObject.AddComponent<CFGCannibal>();
		}
	}

	private void OnEndAction()
	{
		StartDelay();
	}

	private bool ApplyPenetrate()
	{
		if (m_Target == null || m_Parent == null || m_Parent.CurrentWeapon == null || m_Parent.CharacterAnimator == null)
		{
			StartDelay();
			return false;
		}
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayShoot(OnPenetrateShot, OnEndAction);
		AddPenetrateShotGunFX();
		return true;
	}

	private void OnPenetrateShot()
	{
		int num = m_Parent.CurrentWeapon.Damage;
		if (m_Parent.CharacterData != null)
		{
			num += m_Parent.CharacterData.BuffedDamage;
		}
		m_Parent.CurrentWeapon.Shoot(100, num, m_Parent, m_Target, null, null, bSupportShot: false, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateBullet, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateMuzzleFlash);
		CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_PenetrateShot, (!(m_Parent.CurrentWeapon.Visualisation != null)) ? m_Parent.Transform : m_Parent.CurrentWeapon.Visualisation.transform);
	}

	private void AddPenetrateShotGunFX()
	{
		CFGGun currentWeapon = m_Parent.CurrentWeapon;
		if (currentWeapon != null && !(currentWeapon.Visualisation == null))
		{
			Transform transform = null;
			transform = ((!currentWeapon.TwoHanded) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateShotRevolver : (currentWeapon.m_Definition.VisualisationPrefab switch
			{
				"WeaponVis_Shotgun" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateShotShotgun, 
				"WeaponVis_Rifle" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateShotRifle, 
				"WeaponVis_SniperRifle" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PenetrateShotSniperRifle, 
				_ => null, 
			}));
			if (!(transform == null))
			{
				transform = UnityEngine.Object.Instantiate(transform, currentWeapon.Visualisation.transform.position, currentWeapon.Visualisation.transform.rotation) as Transform;
				transform.SetParent(currentWeapon.Visualisation.transform);
			}
		}
	}

	private bool ApplyArteryShot()
	{
		if (m_Target == null || m_Parent == null || m_Parent.CurrentWeapon == null || m_Parent.CharacterAnimator == null)
		{
			StartDelay();
			return false;
		}
		AddArteryShotGunFX();
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayShoot(OnArteryShoot, OnEndAction);
		return true;
	}

	private void AddArteryShotGunFX()
	{
		CFGGun currentWeapon = m_Parent.CurrentWeapon;
		if (currentWeapon != null && !(currentWeapon.Visualisation == null))
		{
			Transform transform = null;
			transform = ((!currentWeapon.TwoHanded) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ArteryShotRevolver : (currentWeapon.m_Definition.VisualisationPrefab switch
			{
				"WeaponVis_Shotgun" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ArteryShotShotgun, 
				"WeaponVis_Rifle" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ArteryShotRifle, 
				"WeaponVis_SniperRifle" => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ArteryShotSniperRifle, 
				_ => null, 
			}));
			if (!(transform == null))
			{
				transform = UnityEngine.Object.Instantiate(transform, currentWeapon.Visualisation.transform.position, currentWeapon.Visualisation.transform.rotation) as Transform;
				transform.SetParent(currentWeapon.Visualisation.transform);
			}
		}
	}

	private void OnArteryShoot()
	{
		int damage = m_Parent.CalcDamage(m_Target, ETurnAction.ArteryShot);
		int finalChanceToHit = m_Parent.GetFinalChanceToHit(m_Target, null, null, null, ETurnAction.ArteryShot);
		bool flag = m_Parent.CurrentWeapon.Shoot(finalChanceToHit, damage, m_Parent, m_Target, null, null, bSupportShot: false, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_ArteryShotBullet);
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if (cFGCharacter != null && cFGCharacter.CharacterData != null && flag)
		{
			cFGCharacter.CharacterData.AddBuff("arteryshot", EBuffSource.Ability);
		}
	}

	private bool ApplyMultiShot()
	{
		if (m_Parent == null)
		{
			StartDelay();
			return false;
		}
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayMultiShot(OnStartMultiShot, OnMultiShotSpawnFX, OnFinishMultiShot, m_Parent.MultiShotSingle, OnPostMultiShot);
		return true;
	}

	private void OnPostMultiShot()
	{
		OnEndAction();
		if (!(m_Parent == null))
		{
			m_Parent.OnFinishMultiShot();
		}
	}

	private void OnStartMultiShot()
	{
		if (!(m_Parent == null) && !(m_Parent.LeftHand == null))
		{
			if (m_Parent.CurrentWeapon.TwoHanded || m_Parent.CurrentWeapon.Visualisation == null)
			{
				m_Parent.MultiShotWeapon1 = new CFGGun();
				m_Parent.MultiShotWeapon1.m_Definition = CFGStaticDataContainer.GetWeapon("revolver_coltarmyrusty");
				m_Parent.MultiShotWeapon1.SpawnVisualisation(m_Parent.LeftHand);
				m_Parent.MultiShotWeapon2 = new CFGGun();
				m_Parent.MultiShotWeapon2.m_Definition = CFGStaticDataContainer.GetWeapon("revolver_coltarmyrusty");
				m_Parent.MultiShotWeapon2.SpawnVisualisation(m_Parent.RightHand);
			}
			else
			{
				m_Parent.MultiShotWeapon1 = new CFGGun();
				m_Parent.MultiShotWeapon1.m_Definition = m_Parent.CurrentWeapon.m_Definition;
				m_Parent.MultiShotWeapon1.SpawnVisualisation(m_Parent.LeftHand);
				m_Parent.MultiShotWeapon2 = m_Parent.CurrentWeapon;
			}
		}
	}

	private void OnMultiShotSpawnFX()
	{
		if (!(m_Parent == null) && m_Parent.MultiShotWeapon1 != null && !(m_Parent.MultiShotWeapon1.Visualisation == null) && m_Parent.MultiShotWeapon2 != null && !(m_Parent.MultiShotWeapon2.Visualisation == null) && !(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotFX == null))
		{
			Transform transform = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotFX, m_Parent.MultiShotWeapon1.Visualisation.transform.position, m_Parent.MultiShotWeapon1.Visualisation.transform.rotation) as Transform;
			Transform transform2 = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotFX, m_Parent.MultiShotWeapon2.Visualisation.transform.position, m_Parent.MultiShotWeapon2.Visualisation.transform.rotation) as Transform;
			transform.SetParent(m_Parent.MultiShotWeapon1.Visualisation.transform);
			transform2.SetParent(m_Parent.MultiShotWeapon2.Visualisation.transform);
		}
	}

	private void OnFinishMultiShot()
	{
		if (!(m_Parent == null) && m_Parent.MultiShotWeapon1 != null && m_Parent.MultiShotWeapon2 != null)
		{
			m_Parent.MultiShotWeapon1.RemoveVisualisation();
			if (m_Parent.CurrentWeapon.TwoHanded)
			{
				m_Parent.MultiShotWeapon2.RemoveVisualisation();
			}
			m_Parent.MultiShotWeapon1 = null;
			m_Parent.MultiShotWeapon2 = null;
		}
	}

	private bool ApplyDemon()
	{
		StartDelay();
		if (m_Parent == null || m_Parent.CharacterData == null)
		{
			return false;
		}
		m_Parent.CharacterData.AddBuff("demonpower", EBuffSource.Ability);
		m_Parent.gameObject.AddComponent<CFGDemon>();
		return false;
	}

	private bool ApplyPrayer()
	{
		if (m_Parent == null || m_Parent.CharacterAnimator == null || m_Parent.CharacterData == null)
		{
			StartDelay();
			return false;
		}
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayPrayer(OnPrayer);
		return false;
	}

	private void PrayerLogic()
	{
		if (m_Parent == null || m_Parent.CharacterData == null)
		{
			return;
		}
		List<string> list = new List<string>();
		string[] array = new string[4] { "prayeraim", "prayersight", "prayermovement", "prayerheal" };
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!m_Parent.CharacterData.HasBuff(text))
			{
				list.Add(text);
			}
		}
		if (list.Count != 0)
		{
			string buff_id = list[UnityEngine.Random.Range(0, list.Count)];
			m_Parent.CharacterData.AddBuff(buff_id, EBuffSource.Ability);
		}
	}

	private void OnPrayer()
	{
		PrayerLogic();
		if (m_Parent != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PrayerFX != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PrayerFX.GetComponent<CFGParticleSystemSettings>() != null)
		{
			CFGParticleSystemSettings component = (UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_PrayerFX, m_Parent.transform.position, m_Parent.transform.rotation) as Transform).GetComponent<CFGParticleSystemSettings>();
			component.transform.SetParent(m_Parent.transform);
			component.transform.localPosition = new Vector3(0f, 0f, 0f);
			component.m_OnDestroyCallback = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(component.m_OnDestroyCallback, new CFGCharacterAnimator.OnEventDelegate(OnEndAction));
		}
		else
		{
			OnEndAction();
		}
	}

	private bool ApplyEqualization()
	{
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.Turn == 0 && (bool)m_Parent && (bool)m_Parent.Owner && m_Parent.Owner.IsPlayer)
		{
			CFGAchievmentTracker.EqualizationUsedInFirstTurn = true;
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character == null || character.IsDead)
			{
				continue;
			}
			character.TakeDamage(character.Hp - 1, m_Parent, bSilent: true);
			if (character.CharacterAnimator != null)
			{
				m_EndWaitForAnim = true;
				character.CharacterAnimator.PlayTransfusion(OnEndAction);
			}
			else
			{
				StartDelay();
			}
			character.FlagNeedFlash = true;
			if (character.BestDetectionType == EBestDetectionType.NotDetected && (!(character.Owner != null) || !character.Owner.IsPlayer))
			{
				continue;
			}
			Vector3 position = character.Position;
			position.y += 1.2f;
			Vector3 position2 = character.transform.position;
			position2.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_equalization") + "</color>", plus: false, 1, -1, character);
			if (instance.m_AbilityData != null && instance.m_AbilityData.m_EqualizationFX != null)
			{
				Transform transform = character.Transform.Find("Chest");
				if (transform != null)
				{
					Transform transform2 = UnityEngine.Object.Instantiate(instance.m_AbilityData.m_EqualizationFX, transform.position, transform.rotation) as Transform;
					transform2.parent = transform;
				}
			}
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component != null && instance.m_AbilityData.m_EqualizationPP != null)
		{
			Component[] components = instance.m_AbilityData.m_EqualizationPP.GetComponents(typeof(Component));
			Component[] array = components;
			foreach (Component component2 in array)
			{
				Type type = component2.GetType();
				if (type != typeof(Transform))
				{
					FieldInfo[] fields = type.GetFields();
					Component obj = component.gameObject.AddComponent(type);
					FieldInfo[] array2 = fields;
					foreach (FieldInfo fieldInfo in array2)
					{
						fieldInfo.SetValue(obj, fieldInfo.GetValue(component2));
					}
				}
			}
		}
		return true;
	}

	private bool ApplyFinder()
	{
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if (cFGCharacter == null)
		{
			StartDelay();
			return false;
		}
		string text = string.Empty;
		List<string> list = new List<string>();
		Dictionary<string, CFGDef_Weapon> dictionary = CFGStaticDataContainer.Weapons();
		foreach (KeyValuePair<string, CFGDef_Weapon> item in dictionary)
		{
			if (item.Value.Findable)
			{
				list.Add(item.Key);
			}
		}
		if (list.Count > 0)
		{
			text = list[UnityEngine.Random.Range(0, list.Count)];
		}
		if (text == string.Empty)
		{
			Debug.Log("Failed to roll new weapon");
			StartDelay();
			return false;
		}
		Debug.Log("Found weapon: " + text);
		if (!m_Parent.CharacterData.Gun1IsTemporary)
		{
			if (m_Parent.CharacterData.Gun2IsTemporary)
			{
				m_Parent.CharacterData.SwapWeapons();
			}
			else
			{
				CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_Parent.CharacterData.Weapon1);
				if (itemDefinition == null)
				{
					Debug.Log(" Could not move weapon to backpack !");
				}
				else
				{
					CFGInventory.AddItem(itemDefinition.ItemID, 1, SetAsNew: false);
					Debug.Log("Moved weapon to backpack: " + itemDefinition.ItemID);
				}
			}
		}
		m_Parent.CharacterData.EquipItem(EItemSlot.Weapon1, text, bTemporary: true);
		m_EndWaitForAnim = true;
		CFGCharacterAnimator.OnEventDelegate a = null;
		a = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(a, new CFGCharacterAnimator.OnEventDelegate(OnEndAction));
		a = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(a, (CFGCharacterAnimator.OnEventDelegate)delegate
		{
			m_Parent.EquipWeapon();
		});
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayFinder(a);
		cFGCharacter.IsCorpseLooted = true;
		return true;
	}

	private bool ApplySmell()
	{
		StartDelay();
		CFGOwner owner = m_Parent.Owner;
		if (owner == null)
		{
			Debug.LogWarning("Cannot apply Smell: caster has no owner!");
			return false;
		}
		int num = 0;
		foreach (CFGIAttackable otherTarget in m_OtherTargets)
		{
			CFGCharacter cFGCharacter = otherTarget as CFGCharacter;
			if (cFGCharacter == null || cFGCharacter.IsVisibleByPlayer() || cFGCharacter.IsDead || cFGCharacter.Hp == cFGCharacter.MaxHp)
			{
				continue;
			}
			if (!m_Parent.SensedEnemies.Contains(cFGCharacter))
			{
				m_Parent.SensedEnemies.Add(cFGCharacter);
				num++;
			}
			if (cFGCharacter.CharacterData != null)
			{
				cFGCharacter.CharacterData.AddBuff("revealed", EBuffSource.UsableAction);
			}
			Transform smellFX = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_SmellFX;
			if (smellFX != null && smellFX.GetComponent<CFGParticleSystemSettings>() != null)
			{
				smellFX = UnityEngine.Object.Instantiate(smellFX, cFGCharacter.Position, cFGCharacter.Rotation) as Transform;
				smellFX.SetParent(cFGCharacter.transform);
				if (!m_EndWaitForAnim)
				{
					m_EndWaitForAnim = true;
					CFGParticleSystemSettings component = smellFX.GetComponent<CFGParticleSystemSettings>();
					component.m_OnDestroyCallback = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(component.m_OnDestroyCallback, new CFGCharacterAnimator.OnEventDelegate(OnEndAction));
				}
			}
		}
		CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Smell, m_Parent.Transform);
		if (num > 0)
		{
			Vector3 position = m_Parent.transform.position;
			position.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_smell", num.ToString()) + "</color>", plus: false, 1, -1, m_Parent);
		}
		else
		{
			Vector3 position2 = m_Parent.transform.position;
			position2.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("floating_smell_notargets") + "</color>", plus: false, 1, -1, m_Parent);
		}
		return true;
	}

	private bool ApplyTransfusion()
	{
		if (m_Target == null || m_Parent == null || m_Parent.CharacterAnimator == null)
		{
			StartDelay();
			return false;
		}
		m_EndWaitForAnim = true;
		m_Parent.OnTransfusionMake();
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if ((bool)cFGCharacter)
		{
			Transform transform;
			Transform transform2;
			if (cFGCharacter.Hp >= m_Parent.Hp)
			{
				transform = cFGCharacter.transform.FindChild("Chest");
				transform2 = m_Parent.transform.FindChild("Chest");
			}
			else
			{
				transform = m_Parent.transform.FindChild("Chest");
				transform2 = cFGCharacter.transform.FindChild("Chest");
			}
			if (transform2 != null && transform != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_TransfusionFX != null)
			{
				Transform transform3 = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_TransfusionFX, transform.position, transform.rotation) as Transform;
				transform3.SetParent(transform);
				CFGLightning component = transform3.GetComponent<CFGLightning>();
				if (component != null)
				{
					component.SetTargets(transform2, OnEndAction);
				}
				else
				{
					OnEndAction();
				}
			}
			else
			{
				OnEndAction();
			}
			int hp = m_Parent.Hp;
			m_Parent.SetHP(m_Target.Hp, m_Parent, bSilent: true);
			cFGCharacter.SetHP(hp, m_Parent, bSilent: true);
			cFGCharacter.OnTransfusionRecive();
			return true;
		}
		StartDelay();
		return false;
	}

	private bool ApplyCourage()
	{
		if (m_Parent == null || m_Parent.CharacterAnimator == null)
		{
			StartDelay();
			return false;
		}
		m_EndWaitForAnim = true;
		m_Parent.CharacterAnimator.PlayCourage(OnCourage, OnEndAction);
		return true;
	}

	private void CourageLogic()
	{
		if (!(m_Parent == null) && m_Parent.CharacterData != null)
		{
			m_Parent.CharacterData.RemBuff("courage2");
			m_Parent.CharacterData.RemBuff("courage3");
			m_Parent.CharacterData.RemBuff("courage4");
			m_Parent.CharacterData.RemBuff("courage5");
			int num = m_Parent.VisibleEnemies.Count;
			if (num > 5)
			{
				num = 5;
			}
			switch (num)
			{
			case 0:
			case 1:
				Debug.LogWarning("Not enough targets for the buff " + num);
				break;
			case 2:
				m_Parent.CharacterData.AddBuff("courage2", EBuffSource.Ability);
				break;
			case 3:
				m_Parent.CharacterData.AddBuff("courage3", EBuffSource.Ability);
				break;
			case 4:
				m_Parent.CharacterData.AddBuff("courage4", EBuffSource.Ability);
				break;
			default:
				m_Parent.CharacterData.AddBuff("courage5", EBuffSource.Ability);
				break;
			}
		}
	}

	private void OnCourage()
	{
		CourageLogic();
		Transform transform = m_Parent.transform.FindChild("Chest");
		if (transform != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_CourageFX != null)
		{
			Transform transform2 = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_CourageFX, transform.position, transform.rotation) as Transform;
			transform2.SetParent(transform);
		}
	}

	protected override bool DoSerialize(CFG_SG_Node nd)
	{
		return true;
	}

	public bool CanDeserialize(CFG_SG_Node Node)
	{
		if (Node == null)
		{
			return false;
		}
		return false;
	}

	public void OnDeserialize(CFG_SG_Node Node)
	{
		SerializeCommon(Node, bWrite: false);
	}

	public override bool ShouldRemove()
	{
		if (m_MaxUsesPerTactical > 0 && m_UsesPerTacticalLeft == 0)
		{
			return true;
		}
		return false;
	}
}
