using UnityEngine;

[RequireComponent(typeof(Light))]
public class CFGLightAnimator : MonoBehaviour
{
	public bool m_Autodestroy = true;

	public AnimationCurve m_IntensityCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public WrapMode m_WrapMode = WrapMode.Once;

	private float m_Timer;

	private void Start()
	{
		m_IntensityCurve.postWrapMode = m_WrapMode;
	}

	private void Update()
	{
		float intensity = m_IntensityCurve.Evaluate(m_Timer);
		base.gameObject.GetComponent<Light>().intensity = intensity;
		if (m_Timer > m_IntensityCurve[m_IntensityCurve.length - 1].time && m_Autodestroy)
		{
			Object.Destroy(base.gameObject);
		}
		m_Timer += Time.deltaTime;
	}
}
