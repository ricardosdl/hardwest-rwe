using UnityEngine;

public class CFGEqualizationEffect : MonoBehaviour
{
	public Shader m_Shader;

	public Texture2D m_BloodTexture;

	public Texture2D m_DistortTexture;

	public Color m_TintColor = Color.red;

	public Color m_ScreenColor = Color.red;

	public AnimationCurve m_OpacityCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public AnimationCurve m_DistortCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public AnimationCurve m_BlendCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public bool m_AutoDestroy;

	private Material m_Material;

	private float m_Timer;

	private void Start()
	{
		m_Timer = 0f;
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

	private void Update()
	{
		if (m_Timer >= m_OpacityCurve[m_OpacityCurve.length - 1].time)
		{
			base.enabled = false;
		}
		else
		{
			m_Timer += Time.deltaTime;
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)m_Shader && (bool)m_Material)
		{
			m_Material.SetTexture("_BloodTex", m_BloodTexture);
			m_Material.SetTexture("_DistortTex", m_DistortTexture);
			m_Material.SetFloat("_DistortPower", m_DistortCurve.Evaluate(m_Timer));
			m_Material.SetFloat("_ColorBlend", m_BlendCurve.Evaluate(m_Timer));
			m_Material.SetColor("_TintColor", m_TintColor);
			m_Material.SetColor("_ScreenColor", m_ScreenColor);
			m_Material.SetFloat("_Opacity", m_OpacityCurve.Evaluate(m_Timer));
			Graphics.Blit(src, dst, m_Material);
		}
		else
		{
			Graphics.Blit(src, dst);
		}
	}

	private void OnDisable()
	{
		if ((bool)m_Material)
		{
			Object.DestroyImmediate(m_Material);
		}
		if (m_AutoDestroy)
		{
			Object.Destroy(this);
		}
	}
}
