using System.Collections.Generic;
using UnityEngine;

public class CFGConeOfFireManager
{
	private CFGConeOfFire m_Prefab;

	private float m_dist = 4f;

	private HashSet<CFGCharacter> m_CharacterSource;

	private Dictionary<CFGCharacter, CFGConeOfFire> m_ConesOfFire;

	public Dictionary<CFGCharacter, CFGConeOfFire> ConesOfFire => m_ConesOfFire;

	public CFGConeOfFireManager()
	{
		if (CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null)
		{
			m_CharacterSource = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters;
		}
		m_ConesOfFire = new Dictionary<CFGCharacter, CFGConeOfFire>();
		m_dist = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.spawnDist;
		m_Prefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireFloorFX;
	}

	public void UpdateManageCones()
	{
		if (m_CharacterSource == null && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null)
		{
			m_CharacterSource = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters;
		}
		CFGSelectionManager instance = CFGSelectionManager.Instance;
		if (instance.SelectedCharacter == null || instance.SelectedCharacter.CurrentAction != ETurnAction.None || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn || instance.SelectedCharacter.CanMakeAction(ETurnAction.Move) != 0 || !CFGOptions.Gameplay.ShowConeOfView || instance.SelectedCharacter.HaveAbility(ETurnAction.Disguise))
		{
			DestroyAllCones();
			return;
		}
		float num = (float)instance.SelectedCharacter.CharacterData.TotalHeat * 0.5f;
		foreach (CFGCharacter item in m_CharacterSource)
		{
			bool flag = m_ConesOfFire.ContainsKey(item);
			if (item.GunpointState == EGunpointState.Target || !item.IsVisibleByPlayer())
			{
				if (flag)
				{
					DestroyCone(item);
				}
				continue;
			}
			Vector3 b = item.transform.position + item.transform.forward.normalized * num;
			float num2 = Vector3.Distance(instance.MouseWorldPos, b);
			if (num2 <= m_dist && !flag)
			{
				SpawnNewCone(item);
			}
			else if (num2 > m_dist && flag)
			{
				DestroyCone(item);
			}
		}
	}

	private void SpawnNewCone(CFGCharacter _source)
	{
		if ((bool)m_Prefab && !(CFGSelectionManager.Instance.SelectedCharacter == null))
		{
			float endLength = CFGSelectionManager.Instance.SelectedCharacter.CharacterData.TotalHeat;
			Vector3 position = _source.Position;
			CFGConeOfFire cFGConeOfFire = Object.Instantiate(m_Prefab, position, Quaternion.identity) as CFGConeOfFire;
			if (!(cFGConeOfFire == null))
			{
				cFGConeOfFire.m_ConeAngle = 90f;
				cFGConeOfFire.m_StartLength = 0f;
				cFGConeOfFire.m_EndLength = endLength;
				cFGConeOfFire.m_VisType = EConeVisType.NORMAL;
				cFGConeOfFire.m_VisType2 = EConeVisType2.VIEW;
				cFGConeOfFire.RegenerateMesh();
				cFGConeOfFire.transform.position = position;
				cFGConeOfFire.transform.LookAt(position + _source.transform.forward);
				m_ConesOfFire.Add(_source, cFGConeOfFire);
			}
		}
	}

	public void DestroyCone(CFGCharacter _source)
	{
		if (m_ConesOfFire.TryGetValue(_source, out var value))
		{
			m_ConesOfFire.Remove(_source);
			Object.Destroy(value.gameObject);
		}
	}

	private void DestroyAllCones()
	{
		if (m_ConesOfFire.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<CFGCharacter, CFGConeOfFire> item in m_ConesOfFire)
		{
			Object.Destroy(item.Value.gameObject);
		}
		m_ConesOfFire.Clear();
	}
}
