using System.Collections.Generic;
using UnityEngine;

public class CFGTurnManager : CFGSingletonResourcePrefab<CFGTurnManager>
{
	public const int PLAYERCOUNT = 2;

	private uint m_Turn;

	private int m_StartedTurn = -1;

	private CFGOwner m_CurrentOwner;

	private bool m_bSetupStage = true;

	public bool InSetupStage
	{
		get
		{
			return m_bSetupStage;
		}
		set
		{
			if (m_bSetupStage == value)
			{
				return;
			}
			m_bSetupStage = value;
			if (!(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null))
			{
				return;
			}
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.UpdateStagePanel();
			if (!m_bSetupStage)
			{
				CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.OnCombatStart();
				foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
				{
					if (!(character == null) && character.IsAlive && !(character.Owner == null) && character.Owner.IsPlayer)
					{
						if (character.Imprisoned)
						{
							character.ActionPoints = 0;
						}
						else if (!character.HasFinalizedTurn)
						{
							character.ActionPoints++;
						}
					}
				}
				CFGSoundDef.Play2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CombatStart);
				CFGSelectionManager.Instance.UpdateRangeVis();
				return;
			}
			OnEndCombat();
			foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				if (!(character2 == null) && character2.IsAlive && character2.ActionPoints >= 1)
				{
					character2.ActionPoints = 1;
				}
			}
			CFGSoundDef.Play2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_SetupStart);
		}
	}

	public uint Turn
	{
		get
		{
			return m_Turn;
		}
		private set
		{
			m_Turn = value;
		}
	}

	public int StartedTurn
	{
		get
		{
			return m_StartedTurn;
		}
		private set
		{
			m_StartedTurn = value;
		}
	}

	public bool IsPlayerTurn => (bool)m_CurrentOwner && m_CurrentOwner.IsPlayer;

	public CFGOwner CurrentOwner
	{
		get
		{
			return m_CurrentOwner;
		}
		private set
		{
			m_CurrentOwner = value;
		}
	}

	private void OnEndCombat()
	{
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (!(character == null) && !character.IsDead)
			{
				character.AIState = EAIState.Passive;
				if (character.CharacterData != null)
				{
					character.CharacterData.SuspicionLevel = -1;
					character.CharacterData.SubduedCount = -1;
				}
				character.OnEndCombat();
			}
		}
	}

	public void StartTurn()
	{
		StartedTurn = (int)Turn;
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.StartTurn(CurrentOwner);
		CFGSelectionManager.Instance.OnNewTurn(CurrentOwner);
	}

	public void EndTurn(bool bUpdateTurnCounter)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component != null)
		{
			component.ClearFocus();
		}
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo.m_ActivateOnEndTurn)
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.UpdateBodyCount();
		}
		for (int i = 0; i < CFGCharacterList.GetCharacterCount(); i++)
		{
			CFGCharacterList.GetCharacterData(i)?.UpdateBuffs();
		}
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.EndTurn(CurrentOwner);
		SelectNextOwner();
		if (bUpdateTurnCounter && m_CurrentOwner.IsPlayer)
		{
			Turn++;
			Debug.Log("Start turn " + Turn);
		}
		StartTurn();
	}

	public void SetupCurrentOwner(CFGOwner owner)
	{
		CurrentOwner = owner;
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		bool flag = (bool)CurrentOwner && CurrentOwner.IsPlayer;
		if ((bool)component)
		{
			component.SetLock(ELockReason.NonPlayerTurn, !flag);
		}
		if (flag || !(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null) || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters == null)
		{
			return;
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters)
		{
			character.SensedByPlayer = false;
		}
	}

	private void SelectNextOwner()
	{
		List<CFGOwner> list = new List<CFGOwner>(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Owners);
		list.Remove(null);
		list.Sort((CFGOwner x, CFGOwner y) => x.TurnOrder.CompareTo(y.TurnOrder));
		if (CurrentOwner == null)
		{
			if (list.Count > 0)
			{
				SetupCurrentOwner(list[0]);
			}
			return;
		}
		int num = list.IndexOf(CurrentOwner);
		if (num >= 0)
		{
			if (++num >= list.Count)
			{
				num = 0;
			}
			SetupCurrentOwner(list[num]);
		}
	}

	public void OnNewMission()
	{
		m_Turn = 0u;
		m_bSetupStage = true;
	}

	public void OnDeserialize(uint Turn, int StartedTurn)
	{
		m_Turn = Turn;
		m_StartedTurn = StartedTurn;
		SetupCurrentOwner(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner);
		if (Turn > StartedTurn)
		{
			StartTurn();
		}
	}
}
