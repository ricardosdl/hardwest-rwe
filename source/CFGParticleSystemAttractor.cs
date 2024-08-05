using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class CFGParticleSystemAttractor : MonoBehaviour
{
	public enum MotionMethod
	{
		PositionBased,
		VelocityBased
	}

	public enum OnReachDestination
	{
		Stay,
		Kill,
		Continue
	}

	public MotionMethod m_MotionMethod;

	public OnReachDestination m_OnReachDestination;

	[Space(20f)]
	public bool m_UseTarget = true;

	public Transform m_Target;

	public Vector3 m_TargetPosition = Vector3.zero;

	public float m_Force = 0.05f;

	[SerializeField]
	private float m_Tolerance = 0.3f;

	[SerializeField]
	private bool m_ShowGizmo = true;

	private ParticleSystem m_System;

	private ParticleSystem.Particle[] m_Particles;

	private bool DestinationBehavior(ParticleSystem.Particle[] particles, int index)
	{
		switch (m_OnReachDestination)
		{
		case OnReachDestination.Stay:
			if (Vector3.Distance(particles[index].position, m_Target.position) <= m_Tolerance)
			{
				particles[index].velocity = Vector3.zero;
				return true;
			}
			break;
		case OnReachDestination.Kill:
			if (Vector3.Distance(particles[index].position, m_Target.position) <= m_Tolerance)
			{
				particles[index].lifetime = 0f;
				return true;
			}
			break;
		case OnReachDestination.Continue:
			return false;
		}
		return false;
	}

	private void LateUpdate()
	{
		if (!m_Target)
		{
			return;
		}
		InitializeParticles();
		int particles = m_System.GetParticles(m_Particles);
		Vector3 vector = m_TargetPosition;
		if (m_UseTarget)
		{
			if (m_System.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				vector = m_Target.transform.localPosition;
			}
			if (m_System.simulationSpace == ParticleSystemSimulationSpace.World)
			{
				vector = m_Target.transform.position;
			}
		}
		if (m_MotionMethod == MotionMethod.PositionBased)
		{
			for (int i = 0; i < particles; i++)
			{
				if (!DestinationBehavior(m_Particles, i))
				{
					Vector3 value = vector - m_Particles[i].position;
					Vector3.Normalize(value);
					Vector3 position = Vector3.MoveTowards(m_Particles[i].position, vector, m_Force);
					m_Particles[i].position = position;
				}
			}
		}
		if (m_MotionMethod == MotionMethod.VelocityBased)
		{
			for (int j = 0; j < particles; j++)
			{
				if (!DestinationBehavior(m_Particles, j))
				{
					Vector3 vector2 = vector - m_Particles[j].position;
					Vector3.Normalize(vector2);
					m_Particles[j].velocity += vector2 * m_Force;
				}
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

	private void OnDrawGizmosSelected()
	{
		if ((bool)m_Target && m_ShowGizmo)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(m_Target.position, m_Tolerance);
		}
	}
}
