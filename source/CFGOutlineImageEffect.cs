using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/CFG Outline")]
[RequireComponent(typeof(Camera))]
public class CFGOutlineImageEffect : MonoBehaviour
{
	[Range(1E-05f, 0.0008f)]
	public float OutlineHardness = 0.05f;

	[Range(0f, 5f)]
	public float OutlineSize = 2f;

	public float OutlineBoost = 1f;

	public Color OutlineColor = Color.black;

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
			Debug.LogWarning("CFG Outline Image Effect cannot find Hidden/CFGOutline shader! It may be removed.");
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
			currentMaterial.SetFloat("_OutlineHardness", OutlineHardness);
			currentMaterial.SetFloat("_OutlineSize", OutlineSize);
			currentMaterial.SetFloat("_OutlineBoost", OutlineBoost);
			currentMaterial.SetColor("_Color", OutlineColor);
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
