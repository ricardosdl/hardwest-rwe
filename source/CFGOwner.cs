using System.Collections.Generic;
using UnityEngine;

public class CFGOwner : CFGGameObject
{
	public delegate void OnTurnDelegate(CFGOwner owner);

	public delegate void OnCharacterKilledDelegate(CFGOwner owner, CFGCharacter character);

	public delegate void OnOwnerCharacterReceivedDmg(CFGCharacter DamageGiver, CFGCharacter DamageReceiver);

	private HashSet<CFGCharacter> m_Characters = new HashSet<CFGCharacter>();

	[SerializeField]
	private int m_TurnOrder;

	[SerializeField]
	private CFGSoundDef m_StartTurnSoundDef;

	[CFGFlowCode(Category = "Owner", Title = "On Start Turn")]
	public OnTurnDelegate m_StartTurnCallback;

	[CFGFlowCode(Category = "Owner", Title = "On End Turn")]
	public OnTurnDelegate m_EndTurnCallback;

	[CFGFlowCode(Category = "Owner", Title = "On Owner Character Killed")]
	public OnCharacterKilledDelegate m_OnCharacterKilledCallback;

	[CFGFlowCode(Category = "Owner", Title = "On Owner Character Received Dmg")]
	public OnOwnerCharacterReceivedDmg m_OnOwnerCharacterReceivedDmg;

	public override ESerializableType SerializableType => ESerializableType.Owner;

	public int TurnOrder => m_TurnOrder;

	public HashSet<CFGCharacter> Characters => m_Characters;

	public virtual bool IsPlayer => false;

	public virtual bool IsAi => false;

	public static CFGOwner GetOwnerByUUID(int uuid)
	{
		if (uuid == 0)
		{
			return null;
		}
		foreach (CFGOwner owner in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Owners)
		{
			if ((bool)owner && owner.UniqueID == uuid)
			{
				return owner;
			}
		}
		return null;
	}

	public void AddCharacter(CFGCharacter character)
	{
		if ((bool)character)
		{
			m_Characters.Add(character);
		}
	}

	public void RemoveCharacter(CFGCharacter character)
	{
		if ((bool)character)
		{
			m_Characters.Remove(character);
		}
	}

	public HashSet<CFGCharacter> GetEnemys()
	{
		HashSet<CFGCharacter> hashSet = new HashSet<CFGCharacter>();
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if ((bool)character && character.IsAlive && character.Owner != this && !character.Imprisoned)
			{
				hashSet.Add(character);
			}
		}
		return hashSet;
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner == this && !IsPlayer && !IsAi)
		{
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.EndTurn(bUpdateTurnCounter: true);
		}
	}

	public void OnCharacterKilled(CFGCharacter character, bool bSilent = false)
	{
		if (character != null)
		{
			RemoveCharacter(character);
			if (m_OnCharacterKilledCallback != null && !bSilent)
			{
				m_OnCharacterKilledCallback(this, character);
			}
			if (CFGSelectionManager.Instance.ConeOfViewManager != null)
			{
				CFGSelectionManager.Instance.ConeOfViewManager.DestroyCone(character);
			}
		}
	}

	public void OnCharacterDamaged(CFGCharacter DamageGiver, CFGCharacter DamageReceiver)
	{
		if (m_OnOwnerCharacterReceivedDmg != null)
		{
			m_OnOwnerCharacterReceivedDmg(DamageGiver, DamageReceiver);
		}
	}

	public override void StartTurn(CFGOwner owner)
	{
		base.StartTurn(owner);
		if (owner == this)
		{
			if (m_StartTurnCallback != null)
			{
				m_StartTurnCallback(this);
			}
			CFGSoundDef.Play2D(m_StartTurnSoundDef);
		}
	}

	public override void EndTurn(CFGOwner owner)
	{
		base.EndTurn(owner);
		if (owner == this && m_EndTurnCallback != null)
		{
			m_EndTurnCallback(this);
		}
	}
}
