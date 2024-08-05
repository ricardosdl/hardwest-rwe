using UnityEngine;

[ExecuteInEditMode]
public class CFGParticleSystemAxisOfRotation : MonoBehaviour
{
	[Range(-1f, 1f)]
	public float m_X;

	[Range(-1f, 1f)]
	public float m_Y;

	[Range(-1f, 1f)]
	public float m_Z;

	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	private void LateUpdate()
	{
		InitializeParticles();
		int particles = m_System.GetParticles(m_Particles);
		for (int i = 0; i < particles; i++)
		{
			m_Particles[i].axisOfRotation = new Vector3(m_X, m_Y, m_Z);
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
