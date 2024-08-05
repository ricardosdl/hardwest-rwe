using UnityEngine;

public class CFGCrippler : MonoBehaviour
{
	private float m_Length = 2f;

	private Shader m_Shader;

	private Color m_CripplerColor = new Color(0.7f, 0f, 0f, 1f);

	private Material[] m_OldMaterials;

	private Material[] m_Materials;

	private Renderer[] m_Renderers;

	private float m_Timer;

	private void Start()
	{
		m_Renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		Shader shader = Shader.Find("CFG/Base/Character");
		m_Shader = Shader.Find("CFG/Base/Character/Crippler");
		m_Materials = new Material[m_Renderers.Length];
		m_OldMaterials = new Material[m_Renderers.Length];
		if (m_Shader != null)
		{
			for (int i = 0; i < m_Renderers.Length; i++)
			{
				if (m_Renderers[i].sharedMaterial.shader == shader)
				{
					m_OldMaterials[i] = m_Renderers[i].sharedMaterial;
				}
			}
			for (int j = 0; j < m_OldMaterials.Length; j++)
			{
				if (m_OldMaterials[j] != null)
				{
					m_Materials[j] = Object.Instantiate(m_OldMaterials[j]);
					m_Materials[j].shader = m_Shader;
					m_Renderers[j].material = m_Materials[j];
				}
			}
		}
		m_Timer = m_Length;
		Shader.SetGlobalColor("_CripplerColor", m_CripplerColor);
	}

	private void LateUpdate()
	{
		Shader.SetGlobalFloat("_Crippler", m_Timer / m_Length);
		m_Timer -= Time.deltaTime;
		if (m_Timer <= 0f)
		{
			Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_OldMaterials.Length; i++)
		{
			if (m_OldMaterials[i] != null)
			{
				m_Renderers[i].material = null;
				m_Renderers[i].sharedMaterial = m_OldMaterials[i];
			}
			Object.Destroy(m_Materials[i]);
		}
	}
}
