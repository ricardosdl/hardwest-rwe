using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/CFG Nightmare Blend")]
public class CFGNightmareBlend : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve m_BlendCurve = AnimationCurve.Linear(0f, 2f, 2f, 0f);

	[HideInInspector]
	public RenderTexture m_NewModeRT;

	[HideInInspector]
	public RenderTexture m_OldModeRT;

	[HideInInspector]
	public CFGSpawnPostProcessCamera m_PostProcessScript;

	public Texture2D BurnTexture;

	public Texture2D BurnGradient;

	public Texture2D NoiseTexture;

	public Shader m_Shader;

	[HideInInspector]
	public bool m_BlendToNightmare;

	private Material m_Material;

	private float m_Timer;

	private void Start()
	{
		if (m_Shader == null)
		{
			Debug.LogWarning("CFG Burn Film Image Effect cannot find Hidden/CFGBurnFilm shader! It may be removed.");
			base.enabled = false;
		}
		else
		{
			if (!m_Shader.isSupported)
			{
				base.enabled = false;
			}
			if (m_Material == null)
			{
				m_Material = new Material(m_Shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
		}
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		m_BlendCurve.preWrapMode = WrapMode.Once;
		m_BlendCurve.postWrapMode = WrapMode.Once;
		if ((bool)m_Shader && (bool)m_Material)
		{
			CFGLevelSettings levelSettings = CFGSingleton<CFGGame>.Instance.LevelSettings;
			m_Material.SetColor("_AmbientOld", (!m_BlendToNightmare) ? levelSettings.m_NightmareAmbientLight : levelSettings.m_NormalAmbientLight);
			m_Material.SetColor("_AmbientNew", (!m_BlendToNightmare) ? levelSettings.m_NormalAmbientLight : levelSettings.m_NightmareAmbientLight);
			m_Material.SetTexture("_BurnTex", BurnTexture);
			m_Material.SetTexture("_BurnGradient", BurnGradient);
			m_Material.SetTexture("_NoiseTex", NoiseTexture);
		}
	}

	private void LateUpdate()
	{
		if (m_Timer < m_BlendCurve[m_BlendCurve.length - 1].time)
		{
			m_Timer += Time.deltaTime;
		}
		else
		{
			m_PostProcessScript.BlendingEffectEnd();
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)m_Shader)
		{
			m_Material.SetFloat("_BurnState", m_BlendCurve.Evaluate(m_Timer));
			m_Material.SetTexture("_OldRT", m_OldModeRT);
			m_Material.SetTexture("_NewRT", m_NewModeRT);
			Graphics.Blit(src, dst, m_Material);
		}
		else
		{
			Debug.LogWarning("A shader has been deleted");
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
}
