using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ColorGrading : MonoBehaviour
{
	public bool m_ColorGrading;

	public bool m_Desaturation;

	public string ColorGradingSettings = "================================================";

	public float lerpAmount;

	public Texture textureRamp;

	public Texture textureRamp2;

	public string DesaturationSettings = "================================================";

	public float lerpDes;

	public RenderTexture TacticalTexture;

	public Shader shader;

	public Shader shader2;

	private Material m_Material;

	private Material m_Material2;

	protected Material material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}

	protected Material material2
	{
		get
		{
			if (m_Material2 == null)
			{
				m_Material2 = new Material(shader2);
				m_Material2.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material2;
		}
	}

	protected virtual void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
		else if (!shader || !shader.isSupported)
		{
			base.enabled = false;
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)m_Material)
		{
			Object.DestroyImmediate(m_Material);
		}
		if ((bool)m_Material2)
		{
			Object.DestroyImmediate(m_Material2);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (m_ColorGrading)
		{
			material.SetTexture("_RgbTex", textureRamp);
			material.SetTexture("_LerpRgbTex", textureRamp2);
			material.SetFloat("_lerpAmount", lerpAmount);
			if (m_Desaturation)
			{
				material.SetFloat("_lerpDes", lerpDes);
			}
			else
			{
				material.SetFloat("_lerpDes", 0f);
			}
			Graphics.Blit(source, destination, material);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
