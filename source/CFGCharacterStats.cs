using System;
using UnityEngine;

[Serializable]
public class CFGCharacterStats
{
	public static int MAX_HELATH_MIN = 1;

	public static int MAX_HELATH_MAX = 20;

	public static int AIM_MIN = 1;

	public static int AIM_MAX = 100;

	public static int DEFENCE_MIN = 1;

	public static int DEFENCE_MAX = 100;

	public static int MOVEMENT_MIN = 1;

	public static int MOVEMENT_MAX = 20;

	public static int SIGHT_MIN = 1;

	public static int SIGHT_MAX = 50;

	public int m_MaxHealth = 6;

	public int m_Aim = 75;

	public int m_Defense = 10;

	public int m_Movement = 15;

	public int m_Sight = 27;

	public int m_MaxLuck = 100;

	public int m_LuckReplenishTurn = 20;

	public int m_LuckReplenishHit = 10;

	public int m_Damage;

	public CFGCharacterStats()
	{
	}

	public CFGCharacterStats(CFGCharacterStats rhs)
	{
		SetMaxHealth(rhs.m_MaxHealth);
		SetAim(rhs.m_Aim);
		SetDefense(rhs.m_Defense);
		SetMovement(rhs.m_Movement);
		SetSight(rhs.m_Sight);
		SetMaxLuck(rhs.m_MaxLuck);
		m_LuckReplenishTurn = rhs.m_LuckReplenishTurn;
		m_LuckReplenishHit = rhs.m_LuckReplenishHit;
		m_Damage = rhs.m_Damage;
	}

	public CFGCharacterStats(CFGDef_Buff rhs)
	{
		m_MaxHealth = rhs.Mod_MaxHP;
		m_Aim = rhs.Mod_Aim;
		m_Defense = rhs.Mod_Defense;
		m_Movement = rhs.Mod_Movement;
		m_Sight = rhs.Mod_Sight;
		m_MaxLuck = rhs.Mod_MaxLuck;
		m_Damage = rhs.Mod_Damage;
		m_LuckReplenishTurn = rhs.LuckChange;
	}

	public void SetMaxHealth(int NewVal)
	{
		m_MaxHealth = Mathf.Clamp(NewVal, MAX_HELATH_MIN, MAX_HELATH_MAX);
	}

	public void SetAim(int NewVal)
	{
		m_Aim = Mathf.Clamp(NewVal, AIM_MIN, AIM_MAX);
	}

	public void SetDefense(int NewVal)
	{
		m_Defense = Mathf.Clamp(NewVal, DEFENCE_MIN, DEFENCE_MAX);
	}

	public void SetMovement(int NewVal)
	{
		m_Movement = Mathf.Clamp(NewVal, MOVEMENT_MIN, MOVEMENT_MAX);
	}

	public void SetSight(int NewVal)
	{
		m_Sight = Mathf.Clamp(NewVal, SIGHT_MIN, SIGHT_MAX);
	}

	public void SetMaxLuck(int NewVal)
	{
		m_MaxLuck = Mathf.Clamp(NewVal, 1, 120);
	}

	public void Clear()
	{
		m_MaxHealth = 0;
		m_Aim = 0;
		m_Defense = 0;
		m_Movement = 0;
		m_Sight = 0;
		m_MaxLuck = 0;
		m_LuckReplenishTurn = 0;
		m_LuckReplenishHit = 0;
		m_Damage = 0;
	}

	public bool IsClear()
	{
		return m_MaxHealth == 0 && m_Aim == 0 && m_Defense == 0 && m_Movement == 0 && m_Sight == 0 && m_MaxLuck == 0 && m_LuckReplenishTurn == 0 && m_Damage == 0 && m_LuckReplenishHit == 0;
	}

	public void AddStats(CFGCharacterStats stats)
	{
		m_MaxHealth += stats.m_MaxHealth;
		m_Aim += stats.m_Aim;
		m_Defense += stats.m_Defense;
		m_Movement += stats.m_Movement;
		m_Sight += stats.m_Sight;
		m_MaxLuck += stats.m_MaxLuck;
		m_LuckReplenishTurn += stats.m_LuckReplenishTurn;
		m_LuckReplenishHit += stats.m_LuckReplenishHit;
		m_Damage += stats.m_Damage;
	}

	public void AddStatsFromItem(string ItemID)
	{
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(ItemID);
		if (itemDefinition != null)
		{
			m_MaxHealth += itemDefinition.Mod_MaxHP;
			m_Aim += itemDefinition.Mod_Aim;
			m_Defense += itemDefinition.Mod_Defense;
			m_Movement += itemDefinition.Mod_Movement;
			m_MaxLuck += itemDefinition.Mod_MaxLuck;
			m_Sight += itemDefinition.Mod_Sight;
			m_Damage += itemDefinition.Mod_Damage;
		}
	}

	public void OnSerialize(CFG_SG_Node ParentNode, bool IsWriting)
	{
		CFG_SG_Node cFG_SG_Node = null;
		cFG_SG_Node = ((!IsWriting) ? ParentNode.FindSubNode("Stats") : ParentNode.AddSubNode("Stats"));
		if (cFG_SG_Node != null)
		{
			cFG_SG_Node.Serialize(IsWriting, "MaxHealth", ref m_MaxHealth);
			cFG_SG_Node.Serialize(IsWriting, "Aim", ref m_Aim);
			cFG_SG_Node.Serialize(IsWriting, "Def", ref m_Defense);
			cFG_SG_Node.Serialize(IsWriting, "Movement", ref m_Movement);
			cFG_SG_Node.Serialize(IsWriting, "Sight", ref m_Sight);
			cFG_SG_Node.Serialize(IsWriting, "MaxLuck", ref m_MaxLuck);
			cFG_SG_Node.Serialize(IsWriting, "LuckTurn", ref m_LuckReplenishTurn);
			cFG_SG_Node.Serialize(IsWriting, "LuckHit", ref m_LuckReplenishHit);
			cFG_SG_Node.Serialize(IsWriting, "Damage", ref m_Damage);
		}
	}
}
