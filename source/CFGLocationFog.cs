using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CFGLocationFog : MonoBehaviour
{
	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	public float m_PushForce = 1f;

	public float m_LifetimeLoss = 0.01f;

	public float m_SizeGain = 0.01f;

	private void Start()
	{
		InitializeIfNeeded();
		m_System.enableEmission = false;
		m_System.loop = false;
	}

	private void LateUpdate()
	{
		InitializeIfNeeded();
		int particles = m_System.GetParticles(m_Particles);
		for (int i = 0; i < particles; i++)
		{
			Vector3 vector = m_Particles[i].position - base.transform.position;
			Vector3.Normalize(vector);
			vector.y = 0f;
			m_Particles[i].velocity += vector * m_PushForce;
			m_Particles[i].lifetime -= m_LifetimeLoss;
			m_Particles[i].size += m_SizeGain;
		}
		m_System.SetParticles(m_Particles, particles);
	}

	private void InitializeIfNeeded()
	{
		if (m_System == null)
		{
			m_System = GetComponent<ParticleSystem>();
		}
		if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
		{
			m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
		}
	}
}
