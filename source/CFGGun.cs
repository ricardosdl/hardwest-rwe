using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGGun
{
	public const string TEMP_WEAPON = "revolver_coltarmyrusty";

	public bool m_bTemporary;

	public CFGDef_Weapon m_Definition;

	private int m_CurrentAmmo = 1;

	private CFGWeaponVisualisation m_Visualisation;

	private CFGCharacter m_Owner;

	public bool IsValid
	{
		get
		{
			if (m_Definition == null)
			{
				return false;
			}
			return true;
		}
	}

	public CFGWeaponVisualisation Visualisation
	{
		get
		{
			return m_Visualisation;
		}
		private set
		{
			m_Visualisation = value;
		}
	}

	public EWeaponClass Class => (m_Definition == null) ? EWeaponClass.SHORT : m_Definition.WeaponClass;

	public int TexId => (m_Definition == null) ? 10 : m_Definition.IconID;

	public int HitChance => (m_Definition != null) ? m_Definition.AimModifier : 0;

	public int Damage => (m_Definition != null) ? m_Definition.Damage : 0;

	public int FullCoverDiv
	{
		get
		{
			if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoCoverReduction))
			{
				return 1;
			}
			return (m_Definition == null) ? 4 : m_Definition.FullCoverDiv;
		}
	}

	public int HalfCoverDiv
	{
		get
		{
			if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoCoverReduction))
			{
				return 1;
			}
			return (m_Definition == null) ? 2 : m_Definition.HalfCoverDiv;
		}
	}

	public int NoticeDistance => (m_Definition == null) ? 10 : m_Definition.NoticeDistance;

	public int CurrentAmmo
	{
		get
		{
			return m_CurrentAmmo;
		}
		set
		{
			m_CurrentAmmo = Mathf.Clamp(value, 0, AmmoCapacity);
		}
	}

	public int AmmoCapacity => (m_Definition != null) ? m_Definition.Ammo : 0;

	public int AmmoPerReload => (m_Definition != null) ? m_Definition.AmmoPerReload : 0;

	public bool TwoHanded => m_Definition != null && m_Definition.TwoHanded;

	public int Heat => (m_Definition != null) ? m_Definition.Heat : 0;

	public bool ShotEndsTurn => m_Definition == null || m_Definition.ShotEndsTurn;

	public bool AllowsRicochet
	{
		get
		{
			if (m_Definition == null)
			{
				return true;
			}
			if (m_Definition.AllowsRicochet && m_Definition.ConeAngle < 1f)
			{
				return true;
			}
			return false;
		}
	}

	public void SetDefinition(string ItemID)
	{
		if (ItemID == null || string.IsNullOrEmpty(ItemID))
		{
			m_Definition = null;
			m_CurrentAmmo = 0;
			return;
		}
		m_Definition = CFGStaticDataContainer.GetWeapon(ItemID);
		if (m_Definition == null)
		{
			Debug.LogWarning("Failed to find weapon definition: " + ItemID);
		}
		CurrentAmmo = AmmoCapacity;
	}

	public bool CanReload()
	{
		return CurrentAmmo < AmmoCapacity;
	}

	public void Reload()
	{
		if (CanReload())
		{
			CurrentAmmo = Mathf.Min(CurrentAmmo + AmmoPerReload, AmmoCapacity);
		}
	}

	public bool CanShoot()
	{
		return CurrentAmmo > 0;
	}

	private CFGIAttackable CreateMissTarget(CFGCharacter attacker)
	{
		if (attacker == null || m_Visualisation == null || m_Visualisation.BarrelEndPoint == null)
		{
			return null;
		}
		CFGShootableObject missTarget = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_MissTarget;
		if (missTarget == null)
		{
			return null;
		}
		Vector3 forward = m_Visualisation.BarrelEndPoint.forward;
		forward.y = 0f;
		Vector3 position = attacker.Transform.position + forward.normalized * 5f;
		CFGIAttackable cFGIAttackable = UnityEngine.Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_MissTarget, position, default(Quaternion)) as CFGIAttackable;
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Unregister(cFGIAttackable as CFGShootableObject);
		return cFGIAttackable;
	}

	public bool Shoot(int chance_to_hit, int damage, CFGCharacter attacker, CFGIAttackable target, CFGBullet.OnBulletHit callback, List<CFGRicochetObject> RicochetObjects = null, bool bSupportShot = false, CFGBullet _specialBullet = null, ParticleSystem _specialMuzzleFlash = null)
	{
		if (target == null || !target.IsAlive)
		{
			target = CreateMissTarget(attacker);
		}
		if (target == null || !CanShoot())
		{
			return false;
		}
		bool flag;
		if (chance_to_hit > CFGGameplaySettings.s_CtH_TopTreshold)
		{
			flag = true;
		}
		else if (chance_to_hit < CFGGameplaySettings.s_CtH_BottomTreshold)
		{
			flag = false;
		}
		else if (chance_to_hit <= target.Luck)
		{
			flag = false;
			target.Luck -= chance_to_hit;
		}
		else
		{
			flag = true;
		}
		if (m_Definition != null && (m_Definition.Prefab_Bullet != null || _specialBullet != null) && m_Visualisation != null)
		{
			Vector3 position = ((!m_Visualisation.BarrelEndPoint) ? m_Visualisation.transform.position : m_Visualisation.BarrelEndPoint.position);
			CFGBullet original = ((!(_specialBullet != null)) ? m_Definition.Prefab_Bullet : _specialBullet);
			CFGBullet cFGBullet = UnityEngine.Object.Instantiate(original, position, Quaternion.identity) as CFGBullet;
			cFGBullet.InitBullet(attacker, target, flag, damage, chance_to_hit, RicochetObjects);
			cFGBullet.m_OnBulletHitCallback = (CFGBullet.OnBulletHit)Delegate.Combine(cFGBullet.m_OnBulletHitCallback, callback);
			if (!bSupportShot)
			{
				CFGSoundDef.Play(m_Visualisation.m_FireSoundDef, position);
			}
		}
		else
		{
			if (flag)
			{
				target.TakeDamage(damage, attacker, bSilent: false);
			}
			callback?.Invoke(null);
		}
		if (!bSupportShot)
		{
			if (!CFGCheats.InfiniteAmmo)
			{
				CurrentAmmo--;
			}
			if ((m_Definition.Prefab_PS != null || _specialMuzzleFlash != null) && Visualisation != null)
			{
				Transform transform = ((!(Visualisation.BarrelEndPoint != null)) ? Visualisation.transform : Visualisation.BarrelEndPoint);
				ParticleSystem original2 = ((!(_specialMuzzleFlash != null)) ? m_Definition.Prefab_PS : _specialMuzzleFlash);
				ParticleSystem particleSystem = UnityEngine.Object.Instantiate(original2, transform.position, transform.rotation) as ParticleSystem;
				if (particleSystem.GetComponent<CFGMuzzleFlash>() == null)
				{
					particleSystem.transform.parent = transform;
				}
				else
				{
					particleSystem.GetComponent<CFGMuzzleFlash>().Reparent(transform);
				}
			}
		}
		return flag;
	}

	public void FakeShoot(CFGCharacter attacker, CFGIAttackable target, bool bHit, CFGBullet _specialBullet = null, ParticleSystem _specialMuzzleFlash = null)
	{
		if (target == null)
		{
			target = CreateMissTarget(attacker);
		}
		if (target == null)
		{
			return;
		}
		if (m_Definition != null && (m_Definition.Prefab_Bullet != null || _specialBullet != null) && m_Visualisation != null)
		{
			Vector3 position = ((!m_Visualisation.BarrelEndPoint) ? m_Visualisation.transform.position : m_Visualisation.BarrelEndPoint.position);
			CFGSoundDef.Play(m_Visualisation.m_FireSoundDef, position);
			CFGBullet original = ((!(_specialBullet != null)) ? m_Definition.Prefab_Bullet : _specialBullet);
			CFGBullet cFGBullet = UnityEngine.Object.Instantiate(original, position, Quaternion.identity) as CFGBullet;
			cFGBullet.InitBullet(attacker, target, bHit, 0, 100);
		}
		if ((m_Definition.Prefab_PS != null || _specialMuzzleFlash != null) && Visualisation != null)
		{
			Transform transform = ((!(Visualisation.BarrelEndPoint != null)) ? Visualisation.transform : Visualisation.BarrelEndPoint);
			ParticleSystem original2 = ((!(_specialMuzzleFlash != null)) ? m_Definition.Prefab_PS : _specialMuzzleFlash);
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(original2, transform.position, transform.rotation) as ParticleSystem;
			if (particleSystem.GetComponent<CFGMuzzleFlash>() == null)
			{
				particleSystem.transform.parent = transform;
			}
			else
			{
				particleSystem.GetComponent<CFGMuzzleFlash>().Reparent(transform);
			}
		}
	}

	public void SpawnVisualisation(Transform parent)
	{
		RemoveVisualisation();
		if (m_Definition == null || m_Definition.Prefab_WeaponVisualisation == null)
		{
			Debug.LogWarning("Weapon: no prefab was found!");
			return;
		}
		Visualisation = UnityEngine.Object.Instantiate(m_Definition.Prefab_WeaponVisualisation);
		if (Visualisation == null)
		{
			Debug.LogError("Failed to spawn weapon prefab visualisation: " + m_Definition.Prefab_WeaponVisualisation);
			return;
		}
		if (parent != null && parent.parent != null)
		{
			m_Owner = parent.parent.GetComponent<CFGCharacter>();
		}
		if (m_Owner != null)
		{
			m_Owner.AddToShaderBackup(Visualisation.transform);
			if (m_Owner.OutlineState != 0)
			{
				m_Owner.GiveOutline(EOutlineType.NONE);
			}
			if (m_Owner.IsInvisible)
			{
				m_Owner.SetInvisible();
			}
		}
		Visualisation.transform.parent = parent;
		Visualisation.transform.localPosition = Vector3.zero;
		Visualisation.transform.localRotation = Quaternion.identity;
	}

	public void RemoveVisualisation()
	{
		if (Visualisation == null)
		{
			return;
		}
		if (m_Owner != null)
		{
			m_Owner.RemoveFromShaderBackup(Visualisation.transform);
		}
		Visualisation.transform.parent = null;
		Transform barrelEndPoint = Visualisation.BarrelEndPoint;
		if ((bool)barrelEndPoint)
		{
			Transform[] componentsInChildren = barrelEndPoint.GetComponentsInChildren<Transform>();
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (!(transform == barrelEndPoint))
				{
					transform.SetParent(null);
				}
			}
		}
		UnityEngine.Object.Destroy(Visualisation.gameObject);
		Visualisation = null;
		m_Owner = null;
	}

	public bool OnSerialize(CFG_SG_Node parent, string NodeName, bool IsWriting)
	{
		CFG_SG_Node cFG_SG_Node = null;
		if (IsWriting && m_Definition == null)
		{
			return true;
		}
		if (IsWriting)
		{
			cFG_SG_Node = parent.AddSubNode(NodeName);
			if (cFG_SG_Node == null)
			{
				return false;
			}
			cFG_SG_Node.Serialize(IsWriting, "ID", ref m_Definition.ItemID);
		}
		else
		{
			cFG_SG_Node = parent.FindSubNode(NodeName);
			if (cFG_SG_Node == null)
			{
				SetDefinition(null);
				return true;
			}
			string definition = cFG_SG_Node.Attrib_Get<string>("ID", null);
			SetDefinition(definition);
		}
		cFG_SG_Node.Serialize(IsWriting, "Count", ref m_CurrentAmmo);
		cFG_SG_Node.Serialize(IsWriting, "Temp", ref m_bTemporary);
		return true;
	}

	public static implicit operator bool(CFGGun exists)
	{
		return exists != null;
	}
}
