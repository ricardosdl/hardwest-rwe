using System.Collections;
using UnityEngine;

public class CFGDemon : MonoBehaviour
{
	private const float m_Speed = -8f;

	private AnimationCurve m_FadeInCurve = AnimationCurve.EaseInOut(0f, 1400f, 3f, 35f);

	private AnimationCurve m_FadeoOutCurve = AnimationCurve.EaseInOut(0f, 1400f, 3f, 35f);

	private Shader m_Shader;

	private Material[] m_OldMaterials;

	private Material[] m_Materials;

	private Renderer[] m_Renderers;

	private float m_Timer;

	private CFGParticleSystemSettings m_System;

	private CFGGameplaySettings m_gs;

	public void Disable()
	{
		StartCoroutine("FadeOut");
	}

	private void OnEnable()
	{
		m_gs = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		if (!(m_gs != null))
		{
			return;
		}
		m_FadeInCurve = m_gs.m_AbilityData.m_DemonCurveFadeIn;
		m_FadeoOutCurve = m_gs.m_AbilityData.m_DemnCurveFadeOut;
		m_Renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		Shader shader = Shader.Find("CFG/Base/Character");
		m_Shader = Shader.Find("CFG/Base/Character Demon");
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
					m_Materials[j].SetTexture("_BurnGradient", m_gs.m_AbilityData.m_DemonGradient);
					m_Materials[j].SetTexture("_NoiseTex", m_gs.m_AbilityData.m_DemonNoise);
					m_Materials[j].SetFloat("_Speed", -8f);
					m_Materials[j].SetFloat("_Power", m_FadeInCurve.keys[m_FadeInCurve.length - 1].value);
					m_Renderers[j].material = m_Materials[j];
				}
			}
		}
		Transform transform = Object.Instantiate(m_gs.m_AbilityData.m_DemonFX, base.transform.position, base.transform.rotation) as Transform;
		transform.SetParent(base.transform);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		Transform transform2 = base.transform.FindChild("Chest");
		if (transform2 != null)
		{
			Transform transform3 = Object.Instantiate(m_gs.m_AbilityData.m_DemonAmbient, transform2.position, transform2.rotation) as Transform;
			transform3.SetParent(transform2);
			transform3.localPosition = Vector3.zero;
			transform3.localEulerAngles = Vector3.zero;
			m_System = transform3.GetComponentInChildren<CFGParticleSystemSettings>();
		}
		StartCoroutine("FadeIn");
	}

	private IEnumerator FadeIn()
	{
		m_Timer = 0f;
		bool isFadingIn = true;
		while (isFadingIn)
		{
			for (int i = 0; i < m_Materials.Length; i++)
			{
				if (m_Materials[i] != null)
				{
					m_Materials[i].SetFloat("_Power", m_FadeInCurve.Evaluate(m_Timer));
				}
			}
			if (m_Timer >= m_FadeInCurve.keys[m_FadeInCurve.length - 1].time)
			{
				isFadingIn = false;
			}
			m_Timer += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator FadeOut()
	{
		if (m_System != null)
		{
			m_System.DisableLoop();
		}
		Transform fx = Object.Instantiate(m_gs.m_AbilityData.m_DemonFX, base.transform.position, base.transform.rotation) as Transform;
		fx.SetParent(base.transform);
		fx.localPosition = Vector3.zero;
		fx.localEulerAngles = Vector3.zero;
		m_Timer = 0f;
		bool isFadingOut = true;
		while (isFadingOut)
		{
			for (int i = 0; i < m_Materials.Length; i++)
			{
				if (m_Materials[i] != null)
				{
					m_Materials[i].SetFloat("_Power", m_FadeoOutCurve.Evaluate(m_Timer));
				}
			}
			if (m_Timer >= m_FadeoOutCurve.keys[m_FadeoOutCurve.length - 1].time)
			{
				isFadingOut = false;
			}
			m_Timer += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(this);
	}

	private void OnDestroy()
	{
		if (m_OldMaterials == null)
		{
			return;
		}
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
