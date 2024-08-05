using UnityEngine;

[ExecuteInEditMode]
public class ObjectBlur : MonoBehaviour
{
	public Shader curShader;

	public RenderTexture renderTex;

	private Material curMaterial;

	private Material material
	{
		get
		{
			if (curMaterial == null)
			{
				curMaterial = new Material(curShader);
				curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return curMaterial;
		}
	}

	private void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!curShader || !curShader.isSupported)
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (curMaterial != null)
		{
			material.SetTexture("_depthTex", renderTex);
		}
		if (curShader != null)
		{
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnDisable()
	{
		if ((bool)curMaterial)
		{
			Object.DestroyImmediate(curMaterial);
			curMaterial = null;
		}
	}
}
