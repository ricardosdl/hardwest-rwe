using System;
using System.Collections.Generic;
using System.Reflection;
using Smaa;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CFGSpawnPostProcessCamera : MonoBehaviour
{
	public LayerMask m_LayersNormal;

	public LayerMask m_LayersNightmare;

	public Camera PostProcessCamera;

	public Camera NightmareCamPrefab;

	public bool m_StartInNightmare;

	public Camera m_BlendCameraPrefab;

	[HideInInspector]
	public bool m_IsBlending;

	public bool m_UseConstantFog = true;

	private List<Component> m_InheritedComponents = new List<Component>();

	private bool m_BlendToNightmare;

	private Camera m_Camera;

	private Camera m_BlendCamera;

	private RenderTexture m_NewModeRT;

	private RenderTexture m_OldModeRT;

	private CFGNightmareBlend m_BlendEffect;

	private CFGFadeToColor camera_fade;

	private List<Light> m_NormalLights;

	private List<Light> m_NightmareLights;

	private CFGConstantFog m_ConstFogComp;

	private CFGConstantClippingPlane m_ConstClippingPlane;

	public CFGConstantClippingPlane ConstanceClippingPlane => m_ConstClippingPlane;

	public void SetupNightmareMode(bool nightmare, bool onLevelStart)
	{
		m_BlendToNightmare = nightmare;
		if (m_NormalLights == null || m_NightmareLights == null)
		{
			m_NormalLights = new List<Light>();
			m_NightmareLights = new List<Light>();
			Light[] array = UnityEngine.Object.FindObjectsOfType(typeof(Light)) as Light[];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.layer == 16)
				{
					m_NormalLights.Add(array[i]);
				}
				if (array[i].gameObject.layer == 15)
				{
					m_NightmareLights.Add(array[i]);
				}
			}
		}
		if (!m_Camera)
		{
			m_Camera = GetComponent<Camera>();
		}
		BlendingEffect(onLevelStart);
		AddConstantFogAndClippingPlaneComponents();
	}

	private void Start()
	{
		bool flag = false;
		ETacticalNightmareMode eTacticalNightmareMode = ETacticalNightmareMode.Auto;
		if (CFGSingleton<CFGGame>.Instance != null && !CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGGame>.Instance.SessionSingle != null && CFGSingleton<CFGGame>.Instance.SessionSingle.CurrentTactical != null)
		{
			eTacticalNightmareMode = CFGSingleton<CFGGame>.Instance.SessionSingle.CurrentTactical.m_InitNightmareMode;
		}
		switch (eTacticalNightmareMode)
		{
		case ETacticalNightmareMode.Auto:
			flag = m_StartInNightmare;
			break;
		case ETacticalNightmareMode.Yes:
			flag = true;
			break;
		case ETacticalNightmareMode.No:
			flag = false;
			break;
		}
		CFGGame.SetNightmareMode(flag, onLevelStart: true);
	}

	public void BlendingEffectEnd()
	{
		if (m_IsBlending)
		{
			m_IsBlending = false;
			UnityEngine.Object.Destroy(m_BlendCamera.gameObject);
			m_Camera.targetTexture = null;
			m_OldModeRT.Release();
			m_NewModeRT.Release();
			UnityEngine.Object.Destroy(m_OldModeRT);
			UnityEngine.Object.Destroy(m_NewModeRT);
		}
	}

	private void BlendingEffect(bool onLevelStart)
	{
		if (!m_IsBlending && !onLevelStart && (bool)m_BlendCameraPrefab)
		{
			m_IsBlending = true;
			if (!m_OldModeRT)
			{
				m_OldModeRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			}
			if (!m_NewModeRT)
			{
				m_NewModeRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			}
			m_Camera.targetTexture = m_OldModeRT;
			m_Camera.Render();
			m_Camera.targetTexture = m_NewModeRT;
			m_BlendCamera = UnityEngine.Object.Instantiate(m_BlendCameraPrefab);
			m_BlendCamera.gameObject.transform.parent = base.transform;
			m_BlendCamera.transform.localPosition = Vector3.zero;
			m_BlendCamera.transform.localEulerAngles = Vector3.zero;
			m_BlendCamera.fieldOfView = m_Camera.fieldOfView;
			m_BlendCamera.nearClipPlane = m_Camera.nearClipPlane;
			m_BlendCamera.farClipPlane = m_Camera.farClipPlane;
			m_BlendCamera.depthTextureMode = DepthTextureMode.None;
			m_BlendCamera.depth = m_Camera.depth + 1f;
			m_BlendEffect = m_BlendCamera.GetComponent<CFGNightmareBlend>();
			m_BlendEffect.m_OldModeRT = m_OldModeRT;
			m_BlendEffect.m_NewModeRT = m_NewModeRT;
			m_BlendEffect.m_PostProcessScript = this;
			m_BlendEffect.m_BlendToNightmare = m_BlendToNightmare;
			CFGSoundDef.Play2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_NightmareTransformation);
		}
		CFGLevelSettings levelSettings = CFGSingleton<CFGGame>.Instance.LevelSettings;
		switch (m_BlendToNightmare)
		{
		case false:
			CopyCameraComponents(m_Camera, PostProcessCamera, m_InheritedComponents);
			m_Camera.cullingMask &= ~(int)m_LayersNightmare;
			m_Camera.cullingMask |= m_LayersNormal;
			if (m_NightmareLights != null && m_NormalLights != null)
			{
				for (int k = 0; k < m_NormalLights.Count; k++)
				{
					m_NormalLights[k].enabled = true;
				}
				for (int l = 0; l < m_NightmareLights.Count; l++)
				{
					m_NightmareLights[l].enabled = false;
				}
			}
			if (levelSettings != null)
			{
				RenderSettings.ambientLight = levelSettings.m_NormalAmbientLight;
			}
			break;
		case true:
			CopyCameraComponents(m_Camera, NightmareCamPrefab, m_InheritedComponents);
			m_Camera.cullingMask &= ~(int)m_LayersNormal;
			m_Camera.cullingMask |= m_LayersNightmare;
			if (m_NightmareLights != null && m_NormalLights != null)
			{
				for (int i = 0; i < m_NormalLights.Count; i++)
				{
					m_NormalLights[i].enabled = false;
				}
				for (int j = 0; j < m_NightmareLights.Count; j++)
				{
					m_NightmareLights[j].enabled = true;
				}
			}
			if (levelSettings != null)
			{
				RenderSettings.ambientLight = levelSettings.m_NightmareAmbientLight;
			}
			break;
		}
		RespawnCFGFadeToColor();
	}

	private void RespawnCFGFadeToColor()
	{
		if (camera_fade != null)
		{
			UnityEngine.Object.Destroy(camera_fade);
			camera_fade = null;
		}
		if (camera_fade == null)
		{
			camera_fade = m_Camera.gameObject.AddComponent<CFGFadeToColor>();
			camera_fade.enabled = false;
		}
	}

	private void CopyCameraComponents(Camera targetCamera, Camera sourceCamera, List<Component> componentsList)
	{
		if (!targetCamera || !sourceCamera)
		{
			return;
		}
		Component[] components = sourceCamera.GetComponents(typeof(Component));
		if (components.Length <= 0)
		{
			return;
		}
		if (componentsList != null)
		{
			foreach (Component components2 in componentsList)
			{
				UnityEngine.Object.Destroy(components2);
			}
			componentsList.Clear();
		}
		Component[] array = components;
		foreach (Component component in array)
		{
			if (component.GetType() != typeof(Transform) && component.GetType() != typeof(Camera) && component.GetType() != typeof(CFGSetDepthTextureMode) && component.GetType() != typeof(CFGSelectionManager) && component.GetType() != typeof(CFGCamera) && component.GetType() != typeof(CFGSpawnPostProcessCamera) && component.GetType() != typeof(AudioListener) && component.GetType() != typeof(GUILayer) && component.GetType() != typeof(FlareLayer) && (CFGOptions.Graphics.PostProcessing <= 0 || (component.GetType() != typeof(ScreenSpaceAmbientObscurance) && component.GetType() != typeof(UnityStandardAssets.ImageEffects.ContrastEnhance) && component.GetType() != typeof(NoiseEffect) && component.GetType() != typeof(NoiseAndGrain) && component.GetType() != typeof(NoiseAndScratches))) && (CFGOptions.Graphics.PostProcessing <= 1 || (component.GetType() != typeof(AntialiasingAsPostEffect) && component.GetType() != typeof(SMAA) && component.GetType() != typeof(SSAOEffect) && component.GetType() != typeof(ScreenSpaceAmbientOcclusion) && component.GetType() != typeof(BloomAndLensFlares) && component.GetType() != typeof(Bloom) && component.GetType() != typeof(BloomOptimized) && component.GetType() != typeof(TiltShift) && component.GetType() != typeof(Vignetting) && component.GetType() != typeof(VignetteAndChromaticAberration))))
			{
				Type type = component.GetType();
				FieldInfo[] fields = type.GetFields();
				Component component2 = targetCamera.gameObject.AddComponent(type);
				componentsList?.Add(component2);
				FieldInfo[] array2 = fields;
				foreach (FieldInfo fieldInfo in array2)
				{
					fieldInfo.SetValue(component2, fieldInfo.GetValue(component));
				}
			}
		}
	}

	private void AddConstantFogAndClippingPlaneComponents()
	{
		if (m_UseConstantFog)
		{
			if (m_ConstFogComp == null)
			{
				m_ConstFogComp = base.gameObject.AddComponent<CFGConstantFog>();
			}
			m_ConstFogComp.UpdateData();
			if (m_ConstClippingPlane == null)
			{
				m_ConstClippingPlane = base.gameObject.AddComponent<CFGConstantClippingPlane>();
			}
			m_ConstClippingPlane.UpdateData();
		}
	}
}
