using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CFGParticleSystemSettings : MonoBehaviour
{
	public enum m_ParentTypes
	{
		AsChild,
		AsParentsChild,
		Detached
	}

	public bool m_AutoDestroy;

	public bool m_DetachOnDie;

	public bool m_DetachOnSpawn;

	public CFGCharacterAnimator.OnEventDelegate m_OnDestroyCallback;

	[Space(20f)]
	public bool m_SpawnFX;

	public Transform m_SpawnObject;

	public float m_SpawnTime;

	public m_ParentTypes m_ParentType;

	private float m_Timer;

	private bool m_IsSpawned;

	private ParticleSystem m_System;

	private void Start()
	{
		if (m_DetachOnSpawn)
		{
			base.gameObject.transform.parent = null;
		}
		m_System = base.gameObject.GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (m_AutoDestroy && (!m_System || !m_System.IsAlive()))
		{
			if (m_OnDestroyCallback != null)
			{
				m_OnDestroyCallback();
				m_OnDestroyCallback = null;
			}
			if (m_DetachOnDie)
			{
				base.gameObject.transform.SetParent(null);
			}
			Object.Destroy(base.gameObject);
		}
		if (!m_SpawnFX || m_IsSpawned)
		{
			return;
		}
		if (m_Timer >= m_SpawnTime)
		{
			Transform transform = Object.Instantiate(m_SpawnObject);
			m_IsSpawned = true;
			switch (m_ParentType)
			{
			case m_ParentTypes.AsChild:
				transform.parent = base.transform;
				break;
			case m_ParentTypes.AsParentsChild:
				transform.parent = ((!(base.transform.parent != null)) ? null : base.transform.parent);
				break;
			case m_ParentTypes.Detached:
				transform.parent = null;
				break;
			}
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
		}
		m_Timer += Time.deltaTime;
	}

	public void DisableLoop()
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].loop = false;
		}
		m_System.loop = false;
	}
}
