using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/CFG Burn Film")]
[RequireComponent(typeof(Camera))]
public class CFGBurnFilm : MonoBehaviour
{
	[Range(0f, 2f)]
	public float BurnState;

	public Texture2D BurnTexture;

	public Texture2D BurnGradient;

	public Texture2D NoiseTexture;

	public Shader shader;

	private Material material;

	private Material currentMaterial
	{
		get
		{
			if (material == null)
			{
				material = new Material(shader);
				material.hideFlags = HideFlags.HideAndDontSave;
			}
			return material;
		}
	}

	private void Start()
	{
		if (shader == null)
		{
			Debug.LogWarning("CFG Burn Film Image Effect cannot find Hidden/CFGBurnFilm shader! It may be removed.");
			base.enabled = false;
		}
		else if (!shader.isSupported)
		{
			base.enabled = false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if ((bool)shader)
		{
			currentMaterial.SetFloat("_BurnState", BurnState);
			currentMaterial.SetTexture("_BurnTex", BurnTexture);
			currentMaterial.SetTexture("_BurnGradient", BurnGradient);
			currentMaterial.SetTexture("_NoiseTex", NoiseTexture);
			Graphics.Blit(src, dst, currentMaterial);
		}
		else
		{
			Debug.LogWarning("A shader has been deleted");
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if ((bool)currentMaterial)
		{
			Object.DestroyImmediate(currentMaterial);
		}
	}
}
