using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CFGHighlighterBlurCutout : MonoBehaviour
{
	public enum BlurType
	{
		StandardGauss,
		SgxGauss
	}

	[Range(0f, 2f)]
	public int downsample = 1;

	[Range(0f, 10f)]
	public float blurSize = 3f;

	[Range(1f, 4f)]
	public int blurIterations = 2;

	public BlurType blurType;

	public Shader blurShader;

	private Material blurMaterial;

	public Shader m_CutoutShader;

	private Material m_CutoutMaterial;

	[HideInInspector]
	public float m_Boost;

	[HideInInspector]
	public RenderTexture m_RTEnemyAlly;

	private bool m_canWork;

	public void CheckResources()
	{
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		if ((bool)blurMaterial)
		{
			blurMaterial.SetFloat("_Boost", m_Boost);
		}
		if ((bool)m_CutoutShader)
		{
			m_CutoutMaterial = new Material(m_CutoutShader);
			m_CutoutMaterial.name = "ImageEffectMaterial";
			m_CutoutMaterial.hideFlags = HideFlags.HideAndDontSave;
			m_CutoutMaterial.SetFloat("_Boost", m_Boost);
		}
		if ((bool)blurMaterial && (bool)blurShader && (bool)m_CutoutMaterial && (bool)m_CutoutShader)
		{
			m_canWork = true;
		}
	}

	private void Start()
	{
		CheckResources();
	}

	public void OnDisable()
	{
		if ((bool)blurMaterial)
		{
			Object.DestroyImmediate(blurMaterial);
		}
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!m_canWork)
		{
			Graphics.Blit(source, destination);
			Debug.Log($"Shader not assigned in gameobject {base.name}");
			return;
		}
		int width = source.width >> downsample;
		int height = source.height >> downsample;
		float num = 1f / (1f * (float)(1 << downsample));
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num, (0f - blurSize) * num, 0f, 0f));
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(m_RTEnemyAlly, renderTexture, blurMaterial, 0);
		blurMaterial.SetTexture("_AllyTex", m_RTEnemyAlly);
		blurMaterial.SetTexture("_Source", source);
		int num2 = ((blurType != 0) ? 2 : 0);
		for (int i = 0; i < blurIterations; i++)
		{
			float num3 = (float)i * 1f;
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num + num3, (0f - blurSize) * num - num3, 0f, 0f));
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, blurMaterial, 1 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			if (i != blurIterations - 1)
			{
				Graphics.Blit(renderTexture, temporary, blurMaterial, 2 + num2);
			}
			else
			{
				Graphics.Blit(renderTexture, destination, blurMaterial, 5);
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			base.enabled = false;
			return null;
		}
		if (s.isSupported && (bool)m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if ((bool)m2Create)
		{
			return m2Create;
		}
		return null;
	}
}
