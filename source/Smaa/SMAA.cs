using UnityEngine;

namespace Smaa;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
[ExecuteInEditMode]
public class SMAA : MonoBehaviour
{
	public HDRMode Hdr;

	public DebugPass DebugPass;

	public QualityPreset Quality = QualityPreset.High;

	public EdgeDetectionMethod DetectionMethod = EdgeDetectionMethod.Luma;

	public bool UsePredication;

	public Preset CustomPreset;

	public PredicationPreset CustomPredicationPreset;

	public Shader Shader;

	public Texture2D AreaTex;

	public Texture2D SearchTex;

	protected Camera m_Camera;

	protected Preset[] m_StdPresets;

	protected Material m_Material;

	public Material Material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(Shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}

	private void OnEnable()
	{
		if (AreaTex == null)
		{
			AreaTex = Resources.Load<Texture2D>("AreaTex");
		}
		if (SearchTex == null)
		{
			SearchTex = Resources.Load<Texture2D>("SearchTex");
		}
		m_Camera = GetComponent<Camera>();
		CreatePresets();
	}

	private void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!Shader || !Shader.isSupported)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (m_Material != null)
		{
			Object.Destroy(m_Material);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		int pixelWidth = m_Camera.pixelWidth;
		int pixelHeight = m_Camera.pixelHeight;
		Preset preset = CustomPreset;
		if (Quality != QualityPreset.Custom)
		{
			preset = m_StdPresets[(int)Quality];
		}
		int detectionMethod = (int)DetectionMethod;
		int pass = 4;
		int pass2 = 5;
		RenderTextureFormat format = source.format;
		if (Hdr == HDRMode.Off)
		{
			format = RenderTextureFormat.ARGB32;
		}
		else if (Hdr == HDRMode.On)
		{
			format = RenderTextureFormat.ARGBHalf;
		}
		Material.SetTexture("_AreaTex", AreaTex);
		Material.SetTexture("_SearchTex", SearchTex);
		Material.SetVector("_Metrics", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, pixelWidth, pixelHeight));
		Material.SetVector("_Params1", new Vector4(preset.Threshold, preset.DepthThreshold, preset.MaxSearchSteps, preset.MaxSearchStepsDiag));
		Material.SetVector("_Params2", new Vector2(preset.CornerRounding, preset.LocalContrastAdaptationFactor));
		Shader.DisableKeyword("USE_PREDICATION");
		if (DetectionMethod == EdgeDetectionMethod.Depth)
		{
			m_Camera.depthTextureMode |= DepthTextureMode.Depth;
		}
		else if (UsePredication)
		{
			m_Camera.depthTextureMode |= DepthTextureMode.Depth;
			Shader.EnableKeyword("USE_PREDICATION");
			Material.SetVector("_Params3", new Vector3(CustomPredicationPreset.Threshold, CustomPredicationPreset.Scale, CustomPredicationPreset.Strength));
		}
		Shader.DisableKeyword("USE_DIAG_SEARCH");
		Shader.DisableKeyword("USE_CORNER_DETECTION");
		if (preset.DiagDetection)
		{
			Shader.EnableKeyword("USE_DIAG_SEARCH");
		}
		if (preset.CornerDetection)
		{
			Shader.EnableKeyword("USE_CORNER_DETECTION");
		}
		int width = pixelWidth;
		int height = pixelHeight;
		CFGRenderTargetUtility.CalcRenderTargetSize(ref width, ref height);
		RenderTexture renderTexture = TempRT(width, height, format);
		RenderTexture renderTexture2 = TempRT(width, height, format);
		Clear(renderTexture);
		Clear(renderTexture2);
		Graphics.Blit(source, renderTexture, Material, detectionMethod);
		if (DebugPass == DebugPass.Edges)
		{
			Graphics.Blit(renderTexture, destination);
		}
		else
		{
			Material.SetVector("_Metrics", new Vector4(1f / (float)width, 1f / (float)height, width, height));
			Graphics.Blit(renderTexture, renderTexture2, Material, pass);
			if (DebugPass == DebugPass.Weights)
			{
				Graphics.Blit(renderTexture2, destination);
			}
			else
			{
				Material.SetTexture("_BlendTex", renderTexture2);
				Material.SetVector("_Metrics", new Vector4(1f / (float)pixelWidth, 1f / (float)pixelHeight, pixelWidth, pixelHeight));
				Graphics.Blit(source, destination, Material, pass2);
			}
		}
		RenderTexture.ReleaseTemporary(renderTexture);
		RenderTexture.ReleaseTemporary(renderTexture2);
	}

	private void Clear(RenderTexture rt)
	{
		Graphics.Blit(rt, rt, Material, 0);
	}

	private RenderTexture TempRT(int width, int height, RenderTextureFormat format)
	{
		int depthBuffer = 0;
		return RenderTexture.GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Linear);
	}

	private void CreatePresets()
	{
		m_StdPresets = new Preset[4];
		m_StdPresets[0] = new Preset
		{
			Threshold = 0.15f,
			MaxSearchSteps = 4
		};
		m_StdPresets[0].DiagDetection = false;
		m_StdPresets[0].CornerDetection = false;
		m_StdPresets[1] = new Preset
		{
			Threshold = 0.1f,
			MaxSearchSteps = 8
		};
		m_StdPresets[1].DiagDetection = false;
		m_StdPresets[1].CornerDetection = false;
		m_StdPresets[2] = new Preset
		{
			Threshold = 0.1f,
			MaxSearchSteps = 16,
			MaxSearchStepsDiag = 8,
			CornerRounding = 25
		};
		m_StdPresets[3] = new Preset
		{
			Threshold = 0.05f,
			MaxSearchSteps = 32,
			MaxSearchStepsDiag = 16,
			CornerRounding = 25
		};
	}
}
