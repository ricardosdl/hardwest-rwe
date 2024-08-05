using UnityEngine;

[ExecuteInEditMode]
public class CFGParticleSystemDrag : MonoBehaviour
{
	public float m_DragForce = 0.1f;

	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	private void FixedUpdate()
	{
		Drag();
	}

	private void Drag()
	{
		InitializeParticles();
		int particles = m_System.GetParticles(m_Particles);
		for (int i = 0; i < particles; i++)
		{
			m_Particles[i].velocity *= m_DragForce;
		}
		m_System.SetParticles(m_Particles, particles);
	}

	private void InitializeParticles()
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
