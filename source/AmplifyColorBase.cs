using System;
using System.Collections.Generic;
using AmplifyColor;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
	public const int LutSize = 32;

	public const int LutWidth = 1024;

	public const int LutHeight = 32;

	public Quality QualityLevel = Quality.Standard;

	public float BlendAmount;

	public Texture LutTexture;

	public Texture LutBlendTexture;

	public Texture MaskTexture;

	public bool UseVolumes;

	public float ExitVolumeBlendTime = 1f;

	public Transform TriggerVolumeProxy;

	public LayerMask VolumeCollisionMask = -1;

	private Shader shaderBase;

	private Shader shaderBlend;

	private Shader shaderBlendCache;

	private Shader shaderMask;

	private Shader shaderBlendMask;

	private RenderTexture blendCacheLut;

	private Texture2D defaultLut;

	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	private Quality qualityLevel = Quality.Standard;

	private Material materialBase;

	private Material materialBlend;

	private Material materialBlendCache;

	private Material materialMask;

	private Material materialBlendMask;

	private bool blending;

	private float blendingTime;

	private float blendingTimeCountdown;

	private Action onFinishBlend;

	private bool volumesBlending;

	private float volumesBlendingTime;

	private float volumesBlendingTimeCountdown;

	private Texture volumesLutBlendTexture;

	private float volumesBlendAmount;

	private Texture worldLUT;

	private AmplifyColorVolumeBase currentVolumeLut;

	private RenderTexture midBlendLUT;

	private bool blendingFromMidBlend;

	private VolumeEffect worldVolumeEffects;

	private VolumeEffect currentVolumeEffects;

	private VolumeEffect blendVolumeEffects;

	private float effectVolumesBlendAdjust;

	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();

	private AmplifyColorTriggerProxy actualTriggerProxy;

	[HideInInspector]
	public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

	public Texture2D DefaultLut => (!(defaultLut == null)) ? defaultLut : CreateDefaultLut();

	public bool IsBlending => blending;

	private float effectVolumesBlendAdjusted => Mathf.Clamp01((!(effectVolumesBlendAdjust < 0.99f)) ? 1f : ((volumesBlendAmount - effectVolumesBlendAdjust) / (1f - effectVolumesBlendAdjust)));

	public bool WillItBlend => LutTexture != null && LutBlendTexture != null && !blending;

	private void ReportMissingShaders()
	{
		Debug.LogError("[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
		base.enabled = false;
	}

	private void ReportNotSupported()
	{
		Debug.LogError("[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses.");
		base.enabled = false;
	}

	private bool CheckShader(Shader s)
	{
		if (s == null)
		{
			ReportMissingShaders();
			return false;
		}
		if (!s.isSupported)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	private bool CheckShaders()
	{
		return CheckShader(shaderBase) && CheckShader(shaderBlend) && CheckShader(shaderBlendCache) && CheckShader(shaderMask) && CheckShader(shaderBlendMask);
	}

	private bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			ReportNotSupported();
			return false;
		}
		return true;
	}

	private void OnEnable()
	{
		if (CheckSupport() && CreateMaterials())
		{
			Texture2D texture2D = LutTexture as Texture2D;
			Texture2D texture2D2 = LutBlendTexture as Texture2D;
			if ((texture2D != null && texture2D.mipmapCount > 1) || (texture2D2 != null && texture2D2.mipmapCount > 1))
			{
				Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
			}
		}
	}

	private void OnDisable()
	{
		if (actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(actualTriggerProxy.gameObject);
			actualTriggerProxy = null;
		}
		ReleaseMaterials();
		ReleaseTextures();
	}

	private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
	{
		volumesLutBlendTexture = blendTargetLUT;
		volumesBlendAmount = 0f;
		volumesBlendingTime = blendTimeInSec;
		volumesBlendingTimeCountdown = blendTimeInSec;
		volumesBlending = true;
	}

	public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
	{
		LutBlendTexture = blendTargetLUT;
		BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		blendingTime = blendTimeInSec;
		blendingTimeCountdown = blendTimeInSec;
		blending = true;
	}

	private void Start()
	{
		worldLUT = LutTexture;
		worldVolumeEffects = EffectFlags.GenerateEffectData(this);
		blendVolumeEffects = (currentVolumeEffects = worldVolumeEffects);
	}

	private void Update()
	{
		if (volumesBlending)
		{
			volumesBlendAmount = (volumesBlendingTime - volumesBlendingTimeCountdown) / volumesBlendingTime;
			volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
			if (volumesBlendAmount >= 1f)
			{
				LutTexture = volumesLutBlendTexture;
				volumesBlendAmount = 0f;
				volumesBlending = false;
				volumesLutBlendTexture = null;
				effectVolumesBlendAdjust = 0f;
				currentVolumeEffects = blendVolumeEffects;
				currentVolumeEffects.SetValues(this);
				if (blendingFromMidBlend && midBlendLUT != null)
				{
					midBlendLUT.DiscardContents();
				}
				blendingFromMidBlend = false;
			}
		}
		else
		{
			volumesBlendAmount = Mathf.Clamp01(volumesBlendAmount);
		}
		if (blending)
		{
			BlendAmount = (blendingTime - blendingTimeCountdown) / blendingTime;
			blendingTimeCountdown -= Time.smoothDeltaTime;
			if (BlendAmount >= 1f)
			{
				LutTexture = LutBlendTexture;
				BlendAmount = 0f;
				blending = false;
				LutBlendTexture = null;
				if (onFinishBlend != null)
				{
					onFinishBlend();
				}
			}
		}
		else
		{
			BlendAmount = Mathf.Clamp01(BlendAmount);
		}
		if (UseVolumes)
		{
			if (actualTriggerProxy == null)
			{
				GameObject gameObject = new GameObject(base.name + "+ACVolumeProxy");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				GameObject gameObject2 = gameObject;
				actualTriggerProxy = gameObject2.AddComponent<AmplifyColorTriggerProxy>();
				actualTriggerProxy.OwnerEffect = this;
			}
			UpdateVolumes();
		}
		else if (actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(actualTriggerProxy.gameObject);
			actualTriggerProxy = null;
		}
	}

	public void EnterVolume(AmplifyColorVolumeBase volume)
	{
		if (!enteredVolumes.Contains(volume))
		{
			enteredVolumes.Insert(0, volume);
		}
	}

	public void ExitVolume(AmplifyColorVolumeBase volume)
	{
		if (enteredVolumes.Contains(volume))
		{
			enteredVolumes.Remove(volume);
		}
	}

	private void UpdateVolumes()
	{
		if (volumesBlending)
		{
			currentVolumeEffects.BlendValues(this, blendVolumeEffects, effectVolumesBlendAdjusted);
		}
		Transform transform = ((!(TriggerVolumeProxy == null)) ? TriggerVolumeProxy : base.transform);
		if (actualTriggerProxy.transform.parent != transform)
		{
			actualTriggerProxy.Reference = transform;
			actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
		}
		AmplifyColorVolumeBase amplifyColorVolumeBase = null;
		int num = int.MinValue;
		foreach (AmplifyColorVolumeBase enteredVolume in enteredVolumes)
		{
			if (enteredVolume.Priority > num)
			{
				amplifyColorVolumeBase = enteredVolume;
				num = enteredVolume.Priority;
			}
		}
		if (!(amplifyColorVolumeBase != currentVolumeLut))
		{
			return;
		}
		currentVolumeLut = amplifyColorVolumeBase;
		Texture texture = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.LutTexture : worldLUT);
		float num2 = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EnterBlendTime : ExitVolumeBlendTime);
		if (volumesBlending && !blendingFromMidBlend && texture == LutTexture)
		{
			LutTexture = volumesLutBlendTexture;
			volumesLutBlendTexture = texture;
			volumesBlendingTimeCountdown = num2 * ((volumesBlendingTime - volumesBlendingTimeCountdown) / volumesBlendingTime);
			volumesBlendingTime = num2;
			currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted);
			effectVolumesBlendAdjust = 1f - volumesBlendAmount;
			volumesBlendAmount = 1f - volumesBlendAmount;
		}
		else
		{
			if (volumesBlending)
			{
				materialBlendCache.SetFloat("_lerpAmount", volumesBlendAmount);
				if (blendingFromMidBlend)
				{
					Graphics.Blit(midBlendLUT, blendCacheLut);
					materialBlendCache.SetTexture("_RgbTex", blendCacheLut);
				}
				else
				{
					materialBlendCache.SetTexture("_RgbTex", LutTexture);
				}
				materialBlendCache.SetTexture("_LerpRgbTex", (!(volumesLutBlendTexture != null)) ? defaultLut : volumesLutBlendTexture);
				Graphics.Blit(midBlendLUT, midBlendLUT, materialBlendCache);
				blendCacheLut.DiscardContents();
				currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(EffectFlags, currentVolumeEffects, blendVolumeEffects, effectVolumesBlendAdjusted);
				effectVolumesBlendAdjust = 0f;
				blendingFromMidBlend = true;
			}
			VolumesBlendTo(texture, num2);
		}
		blendVolumeEffects = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EffectContainer.GetVolumeEffect(this) : worldVolumeEffects);
		if (blendVolumeEffects == null)
		{
			blendVolumeEffects = worldVolumeEffects;
		}
	}

	private void SetupShader()
	{
		colorSpace = QualitySettings.activeColorSpace;
		qualityLevel = QualityLevel;
		string text = ((colorSpace != ColorSpace.Linear) ? string.Empty : "Linear");
		shaderBase = Shader.Find("Hidden/Amplify Color/Base" + text);
		shaderBlend = Shader.Find("Hidden/Amplify Color/Blend" + text);
		shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
		shaderMask = Shader.Find("Hidden/Amplify Color/Mask" + text);
		shaderBlendMask = Shader.Find("Hidden/Amplify Color/BlendMask" + text);
	}

	private void ReleaseMaterials()
	{
		if (materialBase != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBase);
			materialBase = null;
		}
		if (materialBlend != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlend);
			materialBlend = null;
		}
		if (materialBlendCache != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlendCache);
			materialBlendCache = null;
		}
		if (materialMask != null)
		{
			UnityEngine.Object.DestroyImmediate(materialMask);
			materialMask = null;
		}
		if (materialBlendMask != null)
		{
			UnityEngine.Object.DestroyImmediate(materialBlendMask);
			materialBlendMask = null;
		}
	}

	private Texture2D CreateDefaultLut()
	{
		defaultLut = new Texture2D(1024, 32, TextureFormat.RGB24, mipmap: false, linear: true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		defaultLut.name = "DefaultLut";
		defaultLut.hideFlags = HideFlags.DontSave;
		defaultLut.anisoLevel = 1;
		defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] array = new Color32[32768];
		for (int i = 0; i < 32; i++)
		{
			int num = i * 32;
			for (int j = 0; j < 32; j++)
			{
				int num2 = num + j * 1024;
				for (int k = 0; k < 32; k++)
				{
					float num3 = (float)k / 31f;
					float num4 = (float)j / 31f;
					float num5 = (float)i / 31f;
					byte r = (byte)(num3 * 255f);
					byte g = (byte)(num4 * 255f);
					byte b = (byte)(num5 * 255f);
					ref Color32 reference = ref array[num2 + k];
					reference = new Color32(r, g, b, byte.MaxValue);
				}
			}
		}
		defaultLut.SetPixels32(array);
		defaultLut.Apply();
		return defaultLut;
	}

	private void CreateHelperTextures()
	{
		ReleaseTextures();
		blendCacheLut = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		blendCacheLut.name = "BlendCacheLut";
		blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		blendCacheLut.useMipMap = false;
		blendCacheLut.anisoLevel = 0;
		blendCacheLut.Create();
		midBlendLUT = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		midBlendLUT.name = "MidBlendLut";
		midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		midBlendLUT.useMipMap = false;
		midBlendLUT.anisoLevel = 0;
		midBlendLUT.Create();
		CreateDefaultLut();
	}

	private bool CheckMaterialAndShader(Material material, string name)
	{
		if (material == null || material.shader == null)
		{
			Debug.LogError("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
			base.enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogError("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
			base.enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return base.enabled;
	}

	private void SwitchToMobile(Material mat)
	{
		mat.EnableKeyword("AC_QUALITY_MOBILE");
		mat.DisableKeyword("AC_QUALITY_STANDARD");
	}

	private void SwitchToStandard(Material mat)
	{
		mat.EnableKeyword("AC_QUALITY_STANDARD");
		mat.DisableKeyword("AC_QUALITY_MOBILE");
	}

	private bool CreateMaterials()
	{
		SetupShader();
		if (!CheckShaders())
		{
			return false;
		}
		ReleaseMaterials();
		materialBase = new Material(shaderBase);
		materialBlend = new Material(shaderBlend);
		materialBlendCache = new Material(shaderBlendCache);
		materialMask = new Material(shaderMask);
		materialBlendMask = new Material(shaderBlendMask);
		CheckMaterialAndShader(materialBase, "BaseMaterial");
		CheckMaterialAndShader(materialBlend, "BlendMaterial");
		CheckMaterialAndShader(materialBlendCache, "BlendCacheMaterial");
		CheckMaterialAndShader(materialMask, "MaskMaterial");
		CheckMaterialAndShader(materialBlendMask, "BlendMaskMaterial");
		if (!base.enabled)
		{
			return false;
		}
		if (QualityLevel == Quality.Mobile)
		{
			SwitchToMobile(materialBase);
			SwitchToMobile(materialBlend);
			SwitchToMobile(materialBlendCache);
			SwitchToMobile(materialMask);
			SwitchToMobile(materialBlendMask);
		}
		else
		{
			SwitchToStandard(materialBase);
			SwitchToStandard(materialBlend);
			SwitchToStandard(materialBlendCache);
			SwitchToStandard(materialMask);
			SwitchToStandard(materialBlendMask);
		}
		CreateHelperTextures();
		return true;
	}

	private void ReleaseTextures()
	{
		if (blendCacheLut != null)
		{
			UnityEngine.Object.DestroyImmediate(blendCacheLut);
			blendCacheLut = null;
		}
		if (midBlendLUT != null)
		{
			UnityEngine.Object.DestroyImmediate(midBlendLUT);
			midBlendLUT = null;
		}
		if (defaultLut != null)
		{
			UnityEngine.Object.DestroyImmediate(defaultLut);
			defaultLut = null;
		}
	}

	public static bool ValidateLutDimensions(Texture lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
				result = false;
			}
			else if (lut.anisoLevel != 0)
			{
				lut.anisoLevel = 0;
			}
		}
		return result;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		BlendAmount = Mathf.Clamp01(BlendAmount);
		if (colorSpace != QualitySettings.activeColorSpace || qualityLevel != QualityLevel)
		{
			CreateMaterials();
		}
		bool flag = ValidateLutDimensions(LutTexture);
		bool flag2 = ValidateLutDimensions(LutBlendTexture);
		bool flag3 = LutTexture == null && LutBlendTexture == null && volumesLutBlendTexture == null;
		if (!flag || !flag2 || flag3)
		{
			Graphics.Blit(source, destination);
			return;
		}
		Texture texture = ((!(LutTexture == null)) ? LutTexture : defaultLut);
		Texture lutBlendTexture = LutBlendTexture;
		int pass = (GetComponent<Camera>().hdr ? 1 : 0);
		bool flag4 = BlendAmount != 0f || blending;
		bool flag5 = flag4 || (flag4 && lutBlendTexture != null);
		bool flag6 = flag5;
		Material material = ((flag5 || volumesBlending) ? ((!(MaskTexture != null)) ? materialBlend : materialBlendMask) : ((!(MaskTexture != null)) ? materialBase : materialMask));
		material.SetFloat("_lerpAmount", BlendAmount);
		if (MaskTexture != null)
		{
			material.SetTexture("_MaskTex", MaskTexture);
		}
		if (volumesBlending)
		{
			volumesBlendAmount = Mathf.Clamp01(volumesBlendAmount);
			materialBlendCache.SetFloat("_lerpAmount", volumesBlendAmount);
			if (blendingFromMidBlend)
			{
				materialBlendCache.SetTexture("_RgbTex", midBlendLUT);
			}
			else
			{
				materialBlendCache.SetTexture("_RgbTex", texture);
			}
			materialBlendCache.SetTexture("_LerpRgbTex", (!(volumesLutBlendTexture != null)) ? defaultLut : volumesLutBlendTexture);
			Graphics.Blit(texture, blendCacheLut, materialBlendCache);
		}
		if (flag6)
		{
			materialBlendCache.SetFloat("_lerpAmount", BlendAmount);
			RenderTexture renderTexture = null;
			if (volumesBlending)
			{
				renderTexture = RenderTexture.GetTemporary(blendCacheLut.width, blendCacheLut.height, blendCacheLut.depth, blendCacheLut.format, RenderTextureReadWrite.Linear);
				Graphics.Blit(blendCacheLut, renderTexture);
				materialBlendCache.SetTexture("_RgbTex", renderTexture);
			}
			else
			{
				materialBlendCache.SetTexture("_RgbTex", texture);
			}
			materialBlendCache.SetTexture("_LerpRgbTex", (!(lutBlendTexture != null)) ? defaultLut : lutBlendTexture);
			Graphics.Blit(texture, blendCacheLut, materialBlendCache);
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			material.SetTexture("_RgbBlendCacheTex", blendCacheLut);
		}
		else if (volumesBlending)
		{
			material.SetTexture("_RgbBlendCacheTex", blendCacheLut);
		}
		else
		{
			if (texture != null)
			{
				material.SetTexture("_RgbTex", texture);
			}
			if (lutBlendTexture != null)
			{
				material.SetTexture("_LerpRgbTex", lutBlendTexture);
			}
		}
		Graphics.Blit(source, destination, material, pass);
		if (flag6 || volumesBlending)
		{
			blendCacheLut.DiscardContents();
		}
	}
}
