using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CFGAbility
{
	protected int m_Cooldown;

	protected int m_MaxUsesPerTactical = -1;

	protected int m_UsesPerTacticalLeft = -1;

	protected int m_CooldownLeft;

	protected int m_IconID;

	protected CFGCharacter m_Parent;

	protected CFGIAttackable m_Target;

	protected CFGCell m_TargetCell;

	protected HashSet<CFGIAttackable> m_OtherTargets;

	protected ESideStepType m_SideStep_Start = ESideStepType.Auto;

	protected ESideStepType m_SideStep_End = ESideStepType.Auto;

	private bool m_bStartSideSteps = true;

	private bool m_bEndSideSteps = true;

	protected bool m_WorksOnTheSameFloorOnly;

	protected eTargetableType m_SelectableTargets;

	protected eAOE_Type m_AOE_Type;

	protected eTargetableType m_AffectedTypes;

	protected bool m_bUseLOS;

	protected bool m_bUseLOF;

	protected float m_ConeAngleDOTReq;

	protected bool m_bNeedWeapon;

	protected bool m_bAffectOnlyInShadow;

	protected bool m_bIsPassive;

	protected bool m_bIsInstant;

	protected eTargetStates m_RequiredTargetStates;

	protected bool m_bRotateTowardEnemy;

	private float m_EndTime = -1f;

	protected bool m_EndWaitForAnim;

	public bool NeedWeapon => m_bNeedWeapon;

	public bool IsPassive => m_bIsPassive;

	public bool IsInstant => m_bIsInstant;

	public bool RotateTowardEnemy => m_bRotateTowardEnemy;

	public bool UseLOS => m_bUseLOS;

	public int IconID => m_IconID;

	public virtual int PassiveIconID => 0;

	public virtual eAOECircleHelper AOECircleHelper => eAOECircleHelper.Default;

	public eTargetableType TargetType => m_SelectableTargets;

	public abstract string TextID { get; }

	public virtual eAbilityAnimation AnimationType => eAbilityAnimation.None;

	public int Cooldown
	{
		get
		{
			return m_Cooldown;
		}
		set
		{
			m_Cooldown = Mathf.Clamp(value, 0, 100);
		}
	}

	public int CooldownLeft => m_CooldownLeft;

	public bool IsReadyToUse
	{
		get
		{
			if (m_Cooldown > 0 && m_CooldownLeft > 0)
			{
				return false;
			}
			if (m_MaxUsesPerTactical > 0 && m_UsesPerTacticalLeft < 1)
			{
				return false;
			}
			return true;
		}
	}

	public int MaxUsesPerTactical
	{
		get
		{
			return m_MaxUsesPerTactical;
		}
		set
		{
			m_MaxUsesPerTactical = Mathf.Clamp(value, 0, 1000);
		}
	}

	public int UsesPerTacticalLeft
	{
		get
		{
			if (m_MaxUsesPerTactical < 1)
			{
				return -1;
			}
			return m_UsesPerTacticalLeft;
		}
	}

	public bool IsSelfCast => (m_SelectableTargets & eTargetableType.Self) == eTargetableType.Self;

	public bool IsSelfCastOnly => m_SelectableTargets == eTargetableType.Self;

	public eTargetableType SelectableTargetTypes => m_SelectableTargets;

	protected void SetConeDot(float Angle)
	{
		m_ConeAngleDOTReq = Mathf.Cos((float)Math.PI / 180f * Angle * 0.5f);
	}

	public bool CanCastOnCellOnly()
	{
		return m_SelectableTargets == eTargetableType.Cell;
	}

	public virtual bool ShouldRemove()
	{
		return false;
	}

	public abstract bool IsSilent();

	public bool ShouldFinishAction()
	{
		if (m_EndWaitForAnim)
		{
			return false;
		}
		if (m_EndTime <= 0f)
		{
			return true;
		}
		if (Time.time < m_EndTime)
		{
			return false;
		}
		m_EndTime = -1f;
		return true;
	}

	public virtual bool SpawnGrenade()
	{
		return false;
	}

	public eAOE_Type GetAOEType()
	{
		return m_AOE_Type;
	}

	protected abstract bool OnUse();

	protected abstract bool DoSerialize(CFG_SG_Node nd);

	public abstract float GetRange();

	public abstract float GetEffectVal();

	public abstract float GetAOERadiusOrAngle();

	public virtual int GetLuckCost()
	{
		return 0;
	}

	public virtual int GetAPCost()
	{
		return 0;
	}

	protected abstract float GetDelay();

	public bool CanTargetAbilityOn(CFGIAttackable Target)
	{
		if ((bool)m_Parent)
		{
			CFGCharacter cFGCharacter = Target as CFGCharacter;
			if ((bool)cFGCharacter && !m_Parent.SensedEnemies.Contains(cFGCharacter))
			{
				return false;
			}
		}
		if (!IsTargetInRange(Target))
		{
			return false;
		}
		return CanCastOn(Target);
	}

	public bool CanCastOnType(eTargetableType TargetType)
	{
		if (TargetType == eTargetableType.None)
		{
			return false;
		}
		return (m_SelectableTargets & TargetType) == TargetType;
	}

	private bool CheckStates(CFGIAttackable Target)
	{
		if ((m_RequiredTargetStates & eTargetStates.Dead) == eTargetStates.Dead)
		{
			if (Target.IsAlive)
			{
				return false;
			}
		}
		else if (!Target.IsAlive)
		{
			return false;
		}
		if ((m_RequiredTargetStates & eTargetStates.InShadow) == eTargetStates.InShadow && !Target.IsInShadow)
		{
			return false;
		}
		if ((m_RequiredTargetStates & eTargetStates.Wounded) == eTargetStates.Wounded && Target.Hp == Target.MaxHp)
		{
			return false;
		}
		if ((m_RequiredTargetStates & eTargetStates.Unlooted) == eTargetStates.Unlooted && Target.IsCorpseLooted)
		{
			return false;
		}
		return true;
	}

	private void UpdateSideSteps()
	{
		m_bStartSideSteps = CFGCellMap.m_bLOS_UseSideStepsForStartPoint;
		m_bEndSideSteps = CFGCellMap.m_bLOS_UseSideStepsForEndPoint;
		switch (m_SideStep_Start)
		{
		case ESideStepType.Auto:
		{
			eAOE_Type aOE_Type = m_AOE_Type;
			if (aOE_Type == eAOE_Type.Cone)
			{
				m_bStartSideSteps = false;
			}
			else
			{
				m_bStartSideSteps = true;
			}
			break;
		}
		case ESideStepType.Disable:
			m_bStartSideSteps = false;
			break;
		case ESideStepType.Enable:
			m_bStartSideSteps = true;
			break;
		}
		switch (m_SideStep_End)
		{
		case ESideStepType.Auto:
			switch (m_AOE_Type)
			{
			case eAOE_Type.Circle:
			case eAOE_Type.Cone:
			case eAOE_Type.Sphere:
				m_bEndSideSteps = false;
				break;
			default:
				m_bEndSideSteps = true;
				break;
			}
			break;
		case ESideStepType.Disable:
			m_bEndSideSteps = false;
			break;
		case ESideStepType.Enable:
			m_bEndSideSteps = true;
			break;
		}
	}

	public bool CheckLineOfSightAndFire(CFGIAttackable Target)
	{
		if (!m_bUseLOS && !m_bUseLOF)
		{
			return true;
		}
		if (Target == null)
		{
			return false;
		}
		UpdateSideSteps();
		if (m_bUseLOS && CFGCellMap.GetLineOf(m_Parent.CurrentCell, Target.CurrentCell, 1000, 16, m_bStartSideSteps, m_bEndSideSteps) != 0)
		{
			return false;
		}
		if (m_bUseLOF && CFGCellMap.GetLineOf(m_Parent.CurrentCell, Target.CurrentCell, 1000, 32, m_bStartSideSteps, m_bEndSideSteps) != 0)
		{
			return false;
		}
		return true;
	}

	public bool CanCastOn(CFGIAttackable Target)
	{
		if (Target == null || m_Parent == null || m_SelectableTargets == eTargetableType.None)
		{
			return false;
		}
		CFGCell currentCell = Target.CurrentCell;
		if (currentCell == null && Target == null)
		{
			return false;
		}
		if (!CheckStates(Target))
		{
			return false;
		}
		if ((m_SelectableTargets & eTargetableType.Cell) == eTargetableType.Cell)
		{
			if (currentCell != null && currentCell.HaveFloor)
			{
				return true;
			}
			return false;
		}
		if (Target == m_Parent)
		{
			if ((m_SelectableTargets & eTargetableType.Self) == eTargetableType.Self)
			{
				return true;
			}
			return false;
		}
		if (m_WorksOnTheSameFloorOnly && m_Parent.CurrentCell.Floor != currentCell.Floor)
		{
			return false;
		}
		float range = GetRange();
		if (Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Target.CurrentCell.WorldPosition) > range)
		{
			return false;
		}
		CFGOwner owner = m_Parent.Owner;
		CFGOwner owner2 = Target.GetOwner();
		if ((m_SelectableTargets & eTargetableType.Enemy) == eTargetableType.Enemy && owner != owner2 && (bool)owner && (bool)owner2)
		{
			return true;
		}
		if ((m_SelectableTargets & eTargetableType.Friendly) == eTargetableType.Friendly && owner == owner2 && (bool)owner && (bool)owner2)
		{
			return true;
		}
		if ((m_SelectableTargets & eTargetableType.Other) == eTargetableType.Other && owner2 == null)
		{
			return true;
		}
		return false;
	}

	public bool CanCastOnCell(CFGCell Cell)
	{
		if (Cell == null || m_Parent == null || !CanCastOnType(eTargetableType.Cell))
		{
			return false;
		}
		if (Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Cell.WorldPosition) > GetRange())
		{
			return false;
		}
		return true;
	}

	public void OnEndTactical()
	{
		m_CooldownLeft = 0;
	}

	public void OnEndTurn()
	{
		if (m_CooldownLeft > 0)
		{
			m_CooldownLeft--;
		}
	}

	public bool Init(CFGCharacter _Parent)
	{
		if (_Parent == null)
		{
			return false;
		}
		m_Parent = _Parent;
		return true;
	}

	public void SetUpTargets(CFGIAttackable MainTarget, CFGCell TargetCell, List<CFGIAttackable> OtherTargets)
	{
		Debug.Log(string.Concat("Set up targets ", MainTarget, " cell = ", TargetCell, " other ", (OtherTargets == null) ? "null" : OtherTargets.Count.ToString()));
		m_Target = MainTarget;
		m_TargetCell = TargetCell;
		if (OtherTargets == null)
		{
			m_OtherTargets = null;
		}
		else
		{
			m_OtherTargets = new HashSet<CFGIAttackable>(OtherTargets);
		}
	}

	public bool Use()
	{
		if (m_MaxUsesPerTactical > 0 && m_UsesPerTacticalLeft < 1)
		{
			return false;
		}
		if (m_Cooldown > 0 && m_CooldownLeft > 0)
		{
			return false;
		}
		if (m_MaxUsesPerTactical > 0)
		{
			m_UsesPerTacticalLeft--;
		}
		if (m_Cooldown > 0)
		{
			m_CooldownLeft = m_Cooldown;
		}
		return OnUse();
	}

	public void StartDelay()
	{
		m_EndWaitForAnim = false;
		float delay = GetDelay();
		m_EndTime = -1f;
		if (delay > 0f)
		{
			m_EndTime = Time.time + delay;
		}
	}

	private bool IsDirectTargetValid(CFGIAttackable Target)
	{
		if (Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Target.CurrentCell.WorldPosition) > GetRange())
		{
			return false;
		}
		if (m_bUseLOS && CFGCellMap.GetLineOf(m_Parent.CurrentCell, Target.CurrentCell, 1000, 16, m_bStartSideSteps, m_bEndSideSteps) != 0)
		{
			return false;
		}
		if (m_bUseLOF && CFGCellMap.GetLineOf(m_Parent.CurrentCell, Target.CurrentCell, 1000, 32, m_bStartSideSteps, m_bEndSideSteps) != 0)
		{
			return false;
		}
		if (IsTargetInfluenced(m_Parent, Target.CurrentCell, Target))
		{
			return true;
		}
		return false;
	}

	public CFGIAttackable GetFirstValidTarget()
	{
		if (m_Parent == null)
		{
			return null;
		}
		if (m_SelectableTargets == eTargetableType.Cell)
		{
			return null;
		}
		UpdateSideSteps();
		if ((m_RequiredTargetStates & eTargetStates.Dead) == eTargetStates.Dead)
		{
			foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				if (character == null || character.IsAlive || !IsDirectTargetValid(character))
				{
					continue;
				}
				return character;
			}
		}
		if ((CanCastOnType(eTargetableType.Enemy) || CanCastOnType(eTargetableType.Cell)) && m_Parent.SensedEnemies.Count > 0)
		{
			foreach (CFGCharacter sensedEnemy in m_Parent.SensedEnemies)
			{
				if (sensedEnemy == null || !IsDirectTargetValid(sensedEnemy))
				{
					continue;
				}
				return sensedEnemy;
			}
		}
		if (CanCastOnType(eTargetableType.Friendly) || CanCastOnType(eTargetableType.Cell))
		{
			foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner.Characters)
			{
				if (character2 == m_Parent || character2 == null || !CheckStates(character2) || !IsDirectTargetValid(character2))
				{
					continue;
				}
				return character2;
			}
		}
		if (CanCastOnType(eTargetableType.Other) || CanCastOnType(eTargetableType.Cell))
		{
			foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
			{
				if (otherAttackableObject == null || !CheckStates(otherAttackableObject) || !IsDirectTargetValid(otherAttackableObject))
				{
					continue;
				}
				return otherAttackableObject;
			}
		}
		if (CanCastOnType(eTargetableType.Self) && IsTargetInfluenced(m_Parent, m_Parent.CurrentCell, m_Parent))
		{
			return m_Parent;
		}
		return null;
	}

	public bool GenerateTargetList(CFGCell TargetCell, ref List<CFGIAttackable> TargetList, Vector3 Epicenter)
	{
		if (TargetList == null)
		{
			return false;
		}
		TargetList.Clear();
		if (m_Parent == null || m_AOE_Type == eAOE_Type.None || m_Parent.CurrentCell == null)
		{
			return false;
		}
		float range = GetRange();
		switch (m_AOE_Type)
		{
		case eAOE_Type.Everyone:
			foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				if (CheckStates(character))
				{
					TargetList.Add(character);
				}
			}
			return true;
		case eAOE_Type.VisibleEnemies:
			foreach (CFGCharacter visibleEnemy in m_Parent.VisibleEnemies)
			{
				if (CheckStates(visibleEnemy))
				{
					TargetList.Add(visibleEnemy);
				}
			}
			return true;
		case eAOE_Type.Circle:
		case eAOE_Type.Sphere:
			if (Vector3.Distance(m_Parent.CurrentCell.WorldPosition, TargetCell.WorldPosition) > range)
			{
				return false;
			}
			break;
		case eAOE_Type.Cone:
			if (m_Parent.CurrentCell == TargetCell)
			{
				return false;
			}
			break;
		}
		Vector3 vector = TargetCell.WorldPosition - m_Parent.CurrentCell.WorldPosition;
		if (m_Parent.CurrentCell != TargetCell)
		{
			vector.Normalize();
		}
		foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (CanApply(TargetCell, character2, ref Epicenter))
			{
				TargetList.Add(character2);
			}
		}
		if ((m_AffectedTypes & eTargetableType.Other) == eTargetableType.Other)
		{
			foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
			{
				if (CanApply(TargetCell, otherAttackableObject, ref Epicenter))
				{
					TargetList.Add(otherAttackableObject);
				}
			}
		}
		return true;
	}

	private bool CanApply(CFGCell TargetCell, CFGIAttackable Target, ref Vector3 Epicenter)
	{
		if (Target.CurrentCell == null)
		{
			return false;
		}
		Vector3 lhs = Epicenter - m_Parent.CurrentCell.WorldPosition;
		if (m_Parent.CurrentCell != TargetCell)
		{
			lhs.Normalize();
		}
		float aOERadiusOrAngle = GetAOERadiusOrAngle();
		float range = GetRange();
		switch (m_AOE_Type)
		{
		case eAOE_Type.Circle:
		case eAOE_Type.Sphere:
			if (Vector3.Distance(Epicenter, Target.CurrentCell.WorldPosition) > aOERadiusOrAngle)
			{
				return false;
			}
			if (m_bUseLOS && CFGCellMap.GetLineOfSightAutoSideSteps(null, null, Target.CurrentCell, TargetCell, 10000) != 0)
			{
				return false;
			}
			if (m_bUseLOF && CFGCellMap.GetLineOfFireAutoSideSteps(null, null, Target.CurrentCell, TargetCell, 10000) != 0)
			{
				return false;
			}
			break;
		case eAOE_Type.Cone:
			if (Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Target.CurrentCell.WorldPosition) > range)
			{
				return false;
			}
			if (m_bUseLOS && CFGCellMap.GetLineOfSightAutoSideSteps(null, null, m_Parent.CurrentCell, Target.CurrentCell, 10000) != 0)
			{
				return false;
			}
			if (m_bUseLOF && CFGCellMap.GetLineOfFireAutoSideSteps(null, null, m_Parent.CurrentCell, Target.CurrentCell, 10000) != 0)
			{
				return false;
			}
			if (Target.CurrentCell != m_Parent.CurrentCell)
			{
				Vector3 normalized = (Target.CurrentCell.WorldPosition - m_Parent.CurrentCell.WorldPosition).normalized;
				if (Vector3.Dot(lhs, normalized) < m_ConeAngleDOTReq)
				{
					return false;
				}
			}
			break;
		}
		return IsTargetInfluenced(m_Parent, Target.CurrentCell, Target);
	}

	public bool IsTargetInfluenced(CFGCharacter Self, CFGCell TargetCell, CFGIAttackable Target)
	{
		if (Self == null || !CheckStates(Target))
		{
			return false;
		}
		if (TargetCell == null || Target == null)
		{
			return false;
		}
		if (Target == Self)
		{
			if ((m_AffectedTypes & eTargetableType.Self) == eTargetableType.Self)
			{
				return true;
			}
			return false;
		}
		if (m_WorksOnTheSameFloorOnly && Self.CurrentCell.Floor != TargetCell.Floor)
		{
			return false;
		}
		CFGOwner owner = Self.Owner;
		CFGOwner owner2 = Target.GetOwner();
		if ((m_AffectedTypes & eTargetableType.Enemy) == eTargetableType.Enemy && owner != owner2 && (bool)owner && (bool)owner2)
		{
			return true;
		}
		if ((m_AffectedTypes & eTargetableType.Friendly) == eTargetableType.Friendly && owner == owner2 && (bool)owner && (bool)owner2)
		{
			return true;
		}
		if ((m_AffectedTypes & eTargetableType.Other) == eTargetableType.Other && owner2 == null)
		{
			return true;
		}
		return false;
	}

	public bool CanSelectTarget()
	{
		if (m_SelectableTargets != eTargetableType.Self && m_SelectableTargets != 0)
		{
			return true;
		}
		return false;
	}

	public bool IsTargetInRange(CFGIAttackable Target)
	{
		if (Target == null || m_Parent == null)
		{
			return false;
		}
		float num = Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Target.CurrentCell.WorldPosition);
		if (num <= GetRange())
		{
			return true;
		}
		return false;
	}

	public bool IsCellInRange(CFGCell Cell)
	{
		if (Cell == null || m_Parent == null)
		{
			return false;
		}
		float num = Vector3.Distance(m_Parent.CurrentCell.WorldPosition, Cell.WorldPosition);
		if (num <= GetRange())
		{
			return true;
		}
		return false;
	}

	public bool OnSerialize(CFG_SG_Node Parent)
	{
		if (Parent == null)
		{
			return false;
		}
		CFG_SG_Node cFG_SG_Node = Parent.AddSubNode("Ability");
		if (cFG_SG_Node == null)
		{
			return false;
		}
		SerializeCommon(cFG_SG_Node, bWrite: true);
		return DoSerialize(cFG_SG_Node);
	}

	protected void SerializeCommon(CFG_SG_Node nd, bool bWrite)
	{
		nd.Serialize(bWrite, "TurnLeft", ref m_UsesPerTacticalLeft);
		nd.Serialize(bWrite, "Cooldown", ref m_CooldownLeft);
	}
}
