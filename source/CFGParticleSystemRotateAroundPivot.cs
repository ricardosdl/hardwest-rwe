using UnityEngine;

[ExecuteInEditMode]
public class CFGParticleSystemRotateAroundPivot : MonoBehaviour
{
	public Vector3 m_RotationSpeed = Vector3.one;

	public bool m_UseDistanceDrag = true;

	public float m_DistanceDrag = 0.1f;

	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	private void LateUpdate()
	{
		InitializeParticles();
		int particles = m_System.GetParticles(m_Particles);
		for (int i = 0; i < particles; i++)
		{
			if (m_UseDistanceDrag)
			{
				Vector3 rotationSpeed = m_RotationSpeed;
				float num = Vector3.Distance(Vector3.zero, m_Particles[i].position) * m_DistanceDrag;
				rotationSpeed.x *= num;
				rotationSpeed.y *= num;
				rotationSpeed.z *= num;
				m_Particles[i].position = RotateAroundPivot(m_Particles[i].position, Vector3.zero, rotationSpeed);
			}
			else
			{
				m_Particles[i].position = RotateAroundPivot(m_Particles[i].position, Vector3.zero, m_RotationSpeed);
			}
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

	private Vector3 RotateAroundPivot(Vector3 Point, Vector3 Pivot, Vector3 Euler)
	{
		return RotateAroundPivot(Point, Pivot, Quaternion.Euler(Euler));
	}

	private Vector3 RotateAroundPivot(Vector3 Point, Vector3 Pivot, Quaternion Angle)
	{
		return Angle * (Point - Pivot) + Pivot;
	}
}
