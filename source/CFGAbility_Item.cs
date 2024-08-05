using UnityEngine;

public class CFGAbility_Item : CFGAbility
{
	public CFGDef_UsableItem m_Item;

	private bool m_bWaitingForGrenade;

	public override eAbilityAnimation AnimationType
	{
		get
		{
			if (m_Item == null)
			{
				return eAbilityAnimation.None;
			}
			return m_Item.Animation;
		}
	}

	public override string TextID
	{
		get
		{
			if (m_Item == null)
			{
				return "Undefined item";
			}
			return m_Item.ItemID;
		}
	}

	public override eAOECircleHelper AOECircleHelper
	{
		get
		{
			if (m_Item == null)
			{
				return eAOECircleHelper.Default;
			}
			return m_Item.AOECircleHelper;
		}
	}

	public CFGAbility_Item(CFGDef_UsableItem SourceItem, int UseLimit = int.MinValue, int CoolDown = int.MinValue)
	{
		if (SourceItem != null)
		{
			m_Item = SourceItem;
			m_IconID = SourceItem.Icon_Hud;
			m_MaxUsesPerTactical = m_Item.UseLimit;
			if (UseLimit == int.MinValue)
			{
				m_UsesPerTacticalLeft = m_MaxUsesPerTactical;
			}
			else
			{
				m_UsesPerTacticalLeft = UseLimit;
			}
			m_Cooldown = m_Item.Cooldown;
			if (base.Cooldown < 0)
			{
				m_CooldownLeft = 0;
			}
			else
			{
				m_CooldownLeft = CoolDown;
			}
			m_bNeedWeapon = false;
			m_bIsPassive = false;
			m_WorksOnTheSameFloorOnly = false;
			m_SelectableTargets = m_Item.SelectableTargets;
			m_AOE_Type = m_Item.AOE_Type;
			m_bUseLOS = m_Item.UseLOS;
			SetConeDot(m_Item.Radius);
			m_bRotateTowardEnemy = false;
			switch (m_Item.Animation)
			{
			case eAbilityAnimation.Throw1:
			case eAbilityAnimation.Throw2:
			case eAbilityAnimation.ThrowAuto:
				m_bRotateTowardEnemy = true;
				break;
			}
			switch (m_AOE_Type)
			{
			case eAOE_Type.Circle:
			case eAOE_Type.Sphere:
			case eAOE_Type.VisibleEnemies:
			case eAOE_Type.Everyone:
				m_WorksOnTheSameFloorOnly = false;
				break;
			case eAOE_Type.Cone:
				m_WorksOnTheSameFloorOnly = true;
				break;
			}
			m_AffectedTypes = eTargetableType.None;
			if (m_Item.Self_Action != 0)
			{
				m_AffectedTypes |= eTargetableType.Self;
			}
			if (m_Item.Enemy_Action != 0)
			{
				m_AffectedTypes |= eTargetableType.Enemy;
			}
			if (m_Item.Friend_Action != 0)
			{
				m_AffectedTypes |= eTargetableType.Friendly;
			}
			if (m_Item.Other_Action != 0)
			{
				m_AffectedTypes |= eTargetableType.Other;
			}
		}
	}

	public override bool IsSilent()
	{
		if (m_Item == null)
		{
			return true;
		}
		return !m_Item.CausesCombat;
	}

	public override float GetRange()
	{
		if (m_Item == null)
		{
			return 0f;
		}
		return m_Item.Range;
	}

	public override float GetEffectVal()
	{
		return 0f;
	}

	public override float GetAOERadiusOrAngle()
	{
		if (m_Item == null)
		{
			return 0f;
		}
		if (m_Item.AOE_Type == eAOE_Type.None)
		{
			return 0f;
		}
		return m_Item.Radius;
	}

	protected override bool OnUse()
	{
		if (m_Item == null)
		{
			return false;
		}
		CFGAchievmentTracker.OnItemUse(m_Item.ItemID);
		if (m_Item.Animation != 0 && m_Parent != null)
		{
			eAbilityAnimation eAbilityAnimation2 = m_Item.Animation;
			switch (eAbilityAnimation2)
			{
			case eAbilityAnimation.Throw1:
			case eAbilityAnimation.Throw2:
			case eAbilityAnimation.ThrowAuto:
			{
				Vector3 zero = Vector3.zero;
				if (m_Target != null)
				{
					zero = m_Target.Position;
				}
				else
				{
					if (m_TargetCell == null)
					{
						m_Parent.AbilityAnim_Play(eAbilityAnimation2);
						return false;
					}
					zero = m_TargetCell.WorldPosition;
				}
				if (eAbilityAnimation2 == eAbilityAnimation.ThrowAuto)
				{
					eAbilityAnimation2 = eAbilityAnimation.Throw1;
					if (Vector3.Distance(m_Parent.Position, zero) > 5f)
					{
						eAbilityAnimation2 = eAbilityAnimation.Throw2;
					}
				}
				m_Parent.AbilityAnim_Play(eAbilityAnimation2);
				return true;
			}
			}
			m_Parent.AbilityAnim_Play(eAbilityAnimation2);
		}
		StartDelay();
		ApplyEffect();
		return true;
	}

	public override bool SpawnGrenade()
	{
		Vector3 zero = Vector3.zero;
		if (m_Target != null)
		{
			zero = m_Target.Position;
		}
		else
		{
			if (m_TargetCell == null)
			{
				return false;
			}
			zero = m_TargetCell.WorldPosition;
		}
		CFGGrenade prefabObjectComponent = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.GetPrefabObjectComponent<CFGGrenade>(m_Item.GrenadeName);
		if (prefabObjectComponent == null)
		{
			Debug.LogError("Failed to find grenade prefab [" + m_Item.GrenadeName + "]. Please register it in the GameplaySettings, Prefab Map list");
			return false;
		}
		CFGGrenade cFGGrenade = Object.Instantiate(prefabObjectComponent);
		if (cFGGrenade == null)
		{
			Debug.LogError("Failed to instantiate grenade!" + m_Item.GrenadeName);
			return false;
		}
		m_bWaitingForGrenade = true;
		cFGGrenade.transform.parent = m_Parent.RightHand;
		cFGGrenade.transform.localPosition = new Vector3(0.0592f / m_Parent.RightHand.localScale.x, -0.0779f / m_Parent.RightHand.localScale.y, -0.0315f / m_Parent.RightHand.localScale.z);
		cFGGrenade.transform.localRotation = Quaternion.Euler(358.3411f, 359.1724f, 26.5182f);
		cFGGrenade.m_CB_Stopped = OnGrenadeStopped;
		cFGGrenade.SetupPath(m_Parent.Position, zero);
		m_Parent.m_Grenade = cFGGrenade;
		cFGGrenade.m_Definition = m_Item;
		return true;
	}

	private void OnGrenadeStopped()
	{
		ApplyEffect();
		m_bWaitingForGrenade = false;
		if (!(m_Parent == null))
		{
			m_Parent.m_Grenade = null;
		}
	}

	protected void ApplyEffect()
	{
		CFGIAttackable parent = m_Parent;
		if (parent != null)
		{
			if (m_OtherTargets != null && m_OtherTargets.Contains(parent))
			{
				m_OtherTargets.Remove(parent);
			}
			if (m_Target == parent)
			{
				m_Target = null;
			}
		}
		if (m_Parent != null && IsTargetInfluenced(m_Parent, m_Parent.CurrentCell, m_Parent))
		{
			m_Item.ApplyOnTarget(m_Parent, m_Parent);
			if (m_OtherTargets != null && m_OtherTargets.Contains(m_Parent))
			{
				m_OtherTargets.Remove(m_Parent);
			}
		}
		if (m_Target != null && IsTargetInfluenced(m_Parent, m_Target.CurrentCell, m_Target))
		{
			m_Item.ApplyOnTarget(m_Parent, m_Target);
		}
		if (m_OtherTargets == null || m_OtherTargets.Count <= 0)
		{
			return;
		}
		foreach (CFGIAttackable otherTarget in m_OtherTargets)
		{
			if (IsTargetInfluenced(m_Parent, otherTarget.CurrentCell, otherTarget))
			{
				m_Item.ApplyOnTarget(m_Parent, otherTarget);
			}
		}
	}

	protected override bool DoSerialize(CFG_SG_Node nd)
	{
		if (m_Item == null)
		{
			return false;
		}
		nd.Attrib_Set("Name", m_Item.ItemID);
		return true;
	}

	public bool CanDeserialize(CFG_SG_Node Node)
	{
		if (Node == null)
		{
			return false;
		}
		string text = Node.Attrib_Get<string>("Name", null);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (string.Compare(text, m_Item.ItemID, ignoreCase: true) == 0)
		{
			return true;
		}
		return false;
	}

	public void OnDeserialize(CFG_SG_Node Node)
	{
		SerializeCommon(Node, bWrite: false);
	}

	protected override float GetDelay()
	{
		if (m_Item == null)
		{
			return 0f;
		}
		return m_Item.Delay;
	}

	public override bool ShouldRemove()
	{
		if (!ShouldFinishAction())
		{
			return false;
		}
		if (m_bWaitingForGrenade)
		{
			return false;
		}
		if (m_MaxUsesPerTactical > 0 && m_UsesPerTacticalLeft == 0)
		{
			if (m_Item.Replenish)
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
