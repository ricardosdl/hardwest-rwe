using UnityEngine;

public class CFGStrategicConfirmation : MonoBehaviour
{
	[SerializeField]
	private float m_Lifetime = 4f;

	private float m_Timer;

	[SerializeField]
	private AnimationCurve m_ProjectorScale = AnimationCurve.Linear(0f, 0.5f, 2f, 0.3f);

	[SerializeField]
	private AnimationCurve m_ProjectorAlpha = AnimationCurve.Linear(0f, 1f, 2f, 0f);

	[SerializeField]
	private Projector m_Projector;

	private void Start()
	{
		if ((bool)m_Projector)
		{
			m_Projector.material = Object.Instantiate(m_Projector.material);
		}
	}

	private void Update()
	{
		if ((bool)m_Projector)
		{
			m_Projector.orthographicSize = m_ProjectorScale.Evaluate(m_Timer);
			m_Projector.material.SetColor("_ShadowTint", new Color(1f, 1f, 1f, m_ProjectorAlpha.Evaluate(m_Timer)));
		}
		if (m_Timer >= m_Lifetime)
		{
			Object.Destroy(m_Projector.material);
			Object.Destroy(base.gameObject);
		}
		m_Timer += Time.deltaTime;
	}
}
