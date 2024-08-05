using System.Collections.Generic;
using UnityEngine;

public class CFGGrenade : MonoBehaviour
{
	public delegate void OnStoppedMoving();

	public OnStoppedMoving m_CB_Stopped;

	private bool m_bFreeMovement;

	private Vector3 m_StartPoint = Vector3.zero;

	private int m_CurrentPoint = -1;

	private Vector3 m_EndPos = Vector3.zero;

	private CFGGrenadePath m_Path = new CFGGrenadePath();

	[SerializeField]
	private float m_Speed = 4f;

	private bool m_bReachedDest;

	[SerializeField]
	private Transform[] m_DetachableObjects;

	[SerializeField]
	private Transform m_PrefabExplosionFX;

	[SerializeField]
	private CFGSoundDef m_ExplosionSoundDef;

	[SerializeField]
	private float m_DestroyDelay = 1f;

	private float m_Delay;

	[HideInInspector]
	public CFGDef_UsableItem m_Definition;

	public ParticleSystem[] m_FxOnHitPrefab;

	public ParticleSystem[] m_FxOnDiePrefab;

	public Transform m_FxGroundBlood;

	public void SetupPath(Vector3 StartPos, Vector3 EndPos)
	{
		m_StartPoint = StartPos;
		m_EndPos = EndPos;
	}

	public void StartMoving()
	{
		m_StartPoint = base.transform.position;
		m_Path.Calculate(m_StartPoint, m_EndPos, 1000f, realThrow: true);
		m_bFreeMovement = true;
		base.transform.parent = null;
		m_CurrentPoint = -1;
		m_bReachedDest = false;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_bFreeMovement)
		{
			if (m_bReachedDest)
			{
				WaitToDestroy();
			}
			return;
		}
		if (m_CurrentPoint >= m_Path.m_Waypoints.Count - 1)
		{
			m_bReachedDest = true;
			m_bFreeMovement = false;
			OnReachedDest();
			return;
		}
		Vector3 position = base.transform.position;
		float num = Time.deltaTime * m_Speed;
		do
		{
			Vector3 vector = m_Path.m_Waypoints[m_CurrentPoint + 1] - position;
			vector.Normalize();
			float num2 = num;
			float num3 = Vector3.Distance(m_Path.m_Waypoints[m_CurrentPoint + 1], position);
			if (num2 > num3)
			{
				num2 = num3;
				m_CurrentPoint++;
			}
			position += Vector3.ClampMagnitude(vector * num2, num2);
			num -= num3;
		}
		while (m_CurrentPoint + 1 < m_Path.m_Waypoints.Count && num > 0f);
		base.transform.Rotate(base.transform.right, Time.deltaTime * 10f);
		base.transform.position = position;
	}

	private void OnReachedDest()
	{
		SpawnAllFX();
		if (m_CB_Stopped != null)
		{
			m_CB_Stopped();
		}
		if (m_PrefabExplosionFX != null)
		{
			Object.Instantiate(m_PrefabExplosionFX, base.transform.position, default(Quaternion));
		}
		if (m_ExplosionSoundDef != null)
		{
			CFGSoundDef.Play(m_ExplosionSoundDef, base.transform.position);
		}
		for (int i = 0; i < m_DetachableObjects.Length; i++)
		{
			if (m_DetachableObjects[i] != null)
			{
				m_DetachableObjects[i].parent = null;
				m_DetachableObjects[i] = null;
			}
		}
	}

	private void SpawnAllFX()
	{
		CFGCharacter selectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
		if (selectedCharacter == null || m_Definition == null)
		{
			return;
		}
		CFGAbility ability = selectedCharacter.GetAbility(selectedCharacter.CurrentAction);
		if (ability == null)
		{
			return;
		}
		List<CFGIAttackable> TargetList = new List<CFGIAttackable>();
		CFGCell cell = CFGCellMap.GetCell(base.transform.position);
		ability.GenerateTargetList(cell, ref TargetList, base.transform.position);
		foreach (CFGIAttackable item in TargetList)
		{
			CFGCharacter cFGCharacter = item as CFGCharacter;
			if (!(cFGCharacter == null))
			{
				if (m_Definition.Enemy_Action == CFGDef_UsableItem.eActionType.HP_Mod)
				{
					int num = int.Parse(m_Definition.Enemy_Buff);
					bool death = cFGCharacter.Hp <= Mathf.Abs(num) && num < 0;
					SpawnSingleFX(cFGCharacter, death);
				}
				else
				{
					SpawnSingleFX(cFGCharacter, death: false);
				}
			}
		}
	}

	private void SpawnSingleFX(CFGCharacter character, bool death)
	{
		if (!(character != null) || !character.IsVisibleByPlayer() || (character.BestDetectionType & EBestDetectionType.Visible) != EBestDetectionType.Visible)
		{
			return;
		}
		Transform damagePivot = character.GetDamagePivot();
		if (m_FxOnHitPrefab != null && m_FxOnHitPrefab.Length > 0 && damagePivot != null)
		{
			int num = Random.Range(0, m_FxOnHitPrefab.Length);
			if (m_FxOnHitPrefab[num] != null)
			{
				ParticleSystem particleSystem = null;
				particleSystem = Object.Instantiate(m_FxOnHitPrefab[num], damagePivot.position, Quaternion.identity) as ParticleSystem;
				particleSystem.transform.parent = damagePivot;
				particleSystem.transform.LookAt(base.transform.position - base.transform.forward);
			}
		}
		if (death && m_FxOnDiePrefab != null && m_FxOnDiePrefab.Length > 0 && damagePivot != null)
		{
			int num2 = Random.Range(0, m_FxOnDiePrefab.Length);
			if (m_FxOnDiePrefab[num2] != null)
			{
				ParticleSystem particleSystem2 = null;
				particleSystem2 = Object.Instantiate(m_FxOnDiePrefab[num2], damagePivot.position, Quaternion.identity) as ParticleSystem;
				particleSystem2.transform.parent = damagePivot;
				particleSystem2.transform.LookAt(base.transform.position - base.transform.forward);
			}
		}
		if ((bool)m_FxGroundBlood)
		{
			Object.Instantiate(m_FxGroundBlood, character.Position, m_FxGroundBlood.transform.rotation);
		}
	}

	private void WaitToDestroy()
	{
		if (m_Delay >= m_DestroyDelay)
		{
			Object.Destroy(base.gameObject);
		}
		m_Delay += Time.deltaTime;
	}
}
