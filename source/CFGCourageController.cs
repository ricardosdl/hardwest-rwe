using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

public class CFGCourageController : MonoBehaviour
{
	public AnimationCurve m_AnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Material m_Material;

	public float m_TrailEmitTime = 0.25f;

	public float m_Autodestroy = 4f;

	private SmokeTrail[] m_Trails;

	private MeshRenderer[] m_Renderers;

	private Material m_MaterialInst;

	private float m_Timer;

	private void Start()
	{
		m_Renderers = base.gameObject.GetComponentsInChildren<MeshRenderer>();
		m_Trails = base.gameObject.GetComponentsInChildren<SmokeTrail>();
		if ((bool)m_Material)
		{
			m_MaterialInst = Object.Instantiate(m_Material);
		}
		for (int i = 0; i < m_Renderers.Length; i++)
		{
			m_Renderers[i].material = m_MaterialInst;
		}
	}

	private void Update()
	{
		Color color = new Color(1f, 1f, 1f, m_AnimationCurve.Evaluate(m_Timer));
		m_MaterialInst.SetColor("_TintColor", color);
		if (m_Timer > m_TrailEmitTime)
		{
			for (int i = 0; i < m_Trails.Length; i++)
			{
				m_Trails[i].Emit = false;
			}
		}
		if (m_Timer >= m_Autodestroy)
		{
			Object.Destroy(base.gameObject);
		}
		m_Timer += Time.deltaTime;
	}
}
