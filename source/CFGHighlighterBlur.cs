using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CFGHighlighterBlur : MonoBehaviour
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

	public void CheckResources()
	{
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
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
		float num = 1f / (1f * (float)(1 << downsample));
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num, (0f - blurSize) * num, 0f, 0f));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width >> downsample;
		int height = source.height >> downsample;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(source, renderTexture, blurMaterial, 0);
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
			Graphics.Blit(renderTexture, temporary, blurMaterial, 2 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(renderTexture, destination);
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
