using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class CFGSmokePlumeController : MonoBehaviour
{
	public bool m_SimpleAutodestroy;

	public float m_AutodestroyMinDelay = 1f;

	private float m_AutodestroyTimer;

	[Space(20f)]
	[Header("EMISSION CONTROLLS")]
	public Vector2 m_LifetimeRange = Vector2.zero;

	public Vector2 m_DelayRange = Vector2.zero;

	private float m_LifetimeTimer;

	private float m_Lifetime;

	private float m_DelayTimer;

	private float m_Delay;

	private bool m_SmokeEnd;

	private TrailRenderer_Base m_TrailComponent;

	private void Start()
	{
		m_TrailComponent = GetComponent<TrailRenderer_Base>();
		m_Lifetime = Random.Range(m_LifetimeRange.x, m_LifetimeRange.y);
		m_Delay = Random.Range(m_DelayRange.x, m_DelayRange.y);
	}

	private void LateUpdate()
	{
		if (!m_SimpleAutodestroy)
		{
			if (!m_SmokeEnd)
			{
				if (m_DelayTimer >= m_Delay)
				{
					m_TrailComponent.Emit = true;
					if (m_LifetimeTimer >= m_Lifetime)
					{
						m_TrailComponent.Emit = false;
						m_SmokeEnd = true;
					}
					else
					{
						m_LifetimeTimer += Time.deltaTime;
					}
				}
				else
				{
					m_DelayTimer += Time.deltaTime;
					m_TrailComponent.Emit = false;
				}
			}
			else if (m_TrailComponent.NumSegments() == 0)
			{
				Object.Destroy(base.gameObject);
			}
		}
		if (m_SimpleAutodestroy)
		{
			if (m_AutodestroyTimer >= m_AutodestroyMinDelay && m_TrailComponent.NumSegments() == 0)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				m_AutodestroyTimer += Time.deltaTime;
			}
		}
	}
}
