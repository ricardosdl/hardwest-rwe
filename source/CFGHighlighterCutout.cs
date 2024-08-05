using UnityEngine;

public class CFGHighlighterCutout : MonoBehaviour
{
	[HideInInspector]
	public float m_Boost;

	[HideInInspector]
	public RenderTexture m_RTAlly;

	[HideInInspector]
	public RenderTexture m_RTEnemy;

	public Shader m_Shader;

	private Material m_Material;

	private void Start()
	{
		if ((bool)m_Shader)
		{
			m_Material = new Material(m_Shader);
			m_Material.name = "ImageEffectMaterial";
			m_Material.hideFlags = HideFlags.HideAndDontSave;
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + ": Shader is not assigned. Disabling image effect.", base.gameObject);
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)m_Shader && (bool)m_Material)
		{
			m_Material.SetTexture("_AllyTex", m_RTAlly);
			m_Material.SetTexture("_EnemyTex", m_RTEnemy);
			m_Material.SetFloat("_Boost", m_Boost);
			Graphics.Blit(src, dst, m_Material);
		}
		else
		{
			Graphics.Blit(src, dst);
			Debug.LogWarning(base.gameObject.name + ": Shader is not assigned. Disabling image effect.", base.gameObject);
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if ((bool)m_Material)
		{
			Object.DestroyImmediate(m_Material);
		}
	}

	private void OnEnable()
	{
		if ((bool)m_Shader)
		{
			m_Material = new Material(m_Shader);
			m_Material.name = "ImageEffectMaterial";
			m_Material.hideFlags = HideFlags.HideAndDontSave;
		}
		else
		{
			Debug.LogWarning(base.gameObject.name + ": Shader is not assigned. Disabling image effect.", base.gameObject);
			base.enabled = false;
		}
	}
}
