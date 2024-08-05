using UnityEngine;

public class CFGMassLightAnimator : MonoBehaviour
{
	public Light[] m_Lights;

	[SerializeField]
	private AnimationCurve m_IntensityCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AnimationCurve m_RangeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float m_Offset;

	[Space(20f)]
	public bool m_AutoDestroy = true;

	private float[] m_Intensities;

	private float[] m_Ranges;

	private float m_Timer;

	private float m_IntensityValue;

	private float m_RangeValue;

	private void Start()
	{
		if (m_Lights.Length == 0)
		{
			Debug.LogWarning("Mass Light Animator without Lights assigned. This is dumb!", base.gameObject);
			Object.Destroy(base.gameObject);
		}
		GetLightsValues();
		UpdateLights();
		m_Timer += m_Offset;
	}

	private void GetLightsValues()
	{
		m_Intensities = new float[m_Lights.Length];
		m_Ranges = new float[m_Lights.Length];
		for (int i = 0; i < m_Lights.Length; i++)
		{
			if (m_Lights[i] != null)
			{
				m_Intensities[i] = m_Lights[i].intensity;
				m_Ranges[i] = m_Lights[i].range;
			}
		}
	}

	private void UpdateLights()
	{
		for (int i = 0; i < m_Lights.Length; i++)
		{
			if (m_Lights[i] != null)
			{
				m_Lights[i].intensity = m_Intensities[i] * m_IntensityValue;
				m_Lights[i].range = m_Ranges[i] * m_RangeValue;
			}
		}
	}

	private void Update()
	{
		m_IntensityValue = m_IntensityCurve.Evaluate(m_Timer);
		m_RangeValue = m_RangeCurve.Evaluate(m_Timer);
		UpdateLights();
		m_Timer += Time.deltaTime;
		if (m_AutoDestroy && m_Timer >= m_IntensityCurve[m_IntensityCurve.length - 1].time && m_Timer >= m_RangeCurve[m_RangeCurve.length - 1].time)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_Lights.Length; i++)
		{
			Object.Destroy(m_Lights[i].gameObject);
		}
	}
}
