using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/CFG Fade To Color")]
public class CFGFadeToColor : MonoBehaviour
{
	public enum FadeType
	{
		fadeIn,
		fadeOut
	}

	public delegate void OnFadeEndDelegate();

	[HideInInspector]
	public bool m_IsFading = true;

	[SerializeField]
	private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 1f, 2f, 0f);

	[SerializeField]
	private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 0f, 2f, 1f);

	private OnFadeEndDelegate m_OnFadeEndCallback;

	private FadeType m_CurrentFadeType;

	private AnimationCurve m_BlendCurve;

	private Color m_Color = Color.white;

	private Shader m_Shader;

	private Material m_Material;

	private float m_Timer;

	private float m_Speed = 1f;

	public void SetFade(FadeType setFadeType, Color setColor, float speed, OnFadeEndDelegate callback)
	{
		m_Color = setColor;
		m_CurrentFadeType = setFadeType;
		m_Speed = speed;
		switch (m_CurrentFadeType)
		{
		case FadeType.fadeIn:
			m_BlendCurve = fadeInCurve;
			CFGSingleton<CFGWindowMgr>.Instance.SetHUDAlpha(0f);
			break;
		case FadeType.fadeOut:
			m_BlendCurve = fadeOutCurve;
			CFGSingleton<CFGWindowMgr>.Instance.SetHUDAlpha(1f);
			break;
		}
		m_OnFadeEndCallback = callback;
		m_Timer = 0f;
		m_IsFading = true;
		base.enabled = true;
	}

	private void OnEnable()
	{
		m_Shader = Shader.Find("Hidden/CFGFadeToColor");
		if (m_Shader == null)
		{
			Debug.LogWarning("CFGFadeToColor (camera image effect) cannot find Hidden/CFGFadeToColor shader! It may be removed.");
			base.enabled = false;
		}
		else
		{
			m_Material = new Material(m_Shader);
			m_Material.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!m_Shader.isSupported)
		{
			Debug.LogWarning("This device does not support CFGFadeToColor shader. Disabling CFGFadeToColor", base.gameObject);
			base.enabled = false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("This device does not support Image Effects. Disabling CFGFadeToColor.", base.gameObject);
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if ((bool)m_Material)
		{
			Object.DestroyImmediate(m_Material);
		}
		m_Material = null;
		m_IsFading = false;
	}

	private void Start()
	{
		fadeInCurve.preWrapMode = WrapMode.Once;
		fadeInCurve.postWrapMode = WrapMode.Once;
		fadeOutCurve.preWrapMode = WrapMode.Once;
		fadeOutCurve.postWrapMode = WrapMode.Once;
	}

	private void Update()
	{
		if (!m_IsFading)
		{
			return;
		}
		m_Timer += Time.deltaTime * m_Speed;
		if (m_Timer >= m_BlendCurve[m_BlendCurve.length - 1].time)
		{
			m_Timer = m_BlendCurve[m_BlendCurve.length - 1].time;
			CFGSingleton<CFGWindowMgr>.Instance.SetHUDAlpha(1f - m_BlendCurve.Evaluate(m_Timer));
			if (m_OnFadeEndCallback != null)
			{
				m_OnFadeEndCallback();
				m_OnFadeEndCallback = null;
			}
			m_IsFading = false;
		}
		else
		{
			CFGSingleton<CFGWindowMgr>.Instance.SetHUDAlpha(1f - m_BlendCurve.Evaluate(m_Timer));
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)m_Shader && m_BlendCurve != null)
		{
			float value = m_BlendCurve.Evaluate(m_Timer);
			m_Material.SetColor("_Color", m_Color);
			m_Material.SetFloat("_BlendVal", value);
			Graphics.Blit(src, dst, m_Material);
			return;
		}
		if (m_BlendCurve == null)
		{
			Debug.LogWarning("CFGFadeToColor: GameplayCamera has two CFGFadeToColor effects assigned when only one is allowed. Delete CFGFadeToColor from your Post-Process camera!", base.gameObject);
		}
		if (m_Shader == null)
		{
			Debug.LogWarning("CFGFadeToColor: A shader has been deleted!", base.gameObject);
		}
		base.enabled = false;
	}
}
