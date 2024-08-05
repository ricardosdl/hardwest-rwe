using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CFGPlanarReflection : MonoBehaviour
{
	private enum TextureDownsample
	{
		x1 = 100,
		x0_5 = 50,
		x0_25 = 25
	}

	[Range(0f, 1f)]
	[SerializeField]
	private float m_ReflectionStrength = 0.35f;

	private TextureDownsample m_Resolution = TextureDownsample.x0_5;

	public bool m_IsGlobal;

	public bool m_OverrideClippingHeight;

	public float m_ClippingHeight;

	private Material m_Material;

	private CFGPlanarReflectionCamera m_ReflCam;

	private Shader m_Shader;

	private Renderer m_Renderer;

	public float GetReflectionStrength()
	{
		return m_ReflectionStrength;
	}

	private void OnEnable()
	{
		if (!SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
		{
			Debug.LogWarning("This system does not support Render Textures or RenderTextureFormat.R8. Disabling CFGPlanarReflection.", base.gameObject);
			base.enabled = false;
		}
		else if (CFGOptions.Graphics.WaterReflections != 3)
		{
			m_Shader = Shader.Find("CFG/Custom/Planar Reflection");
			m_Renderer = base.gameObject.GetComponent<Renderer>();
			if (m_Renderer != null)
			{
				m_Material = m_Renderer.material;
			}
			m_ReflCam = new GameObject("PlanarReflectionCamera").AddComponent<CFGPlanarReflectionCamera>();
			m_ReflCam.gameObject.hideFlags = HideFlags.HideAndDontSave;
			if (m_IsGlobal)
			{
				Shader.EnableKeyword("WATER_REFLECTIONS");
			}
			else
			{
				Shader.DisableKeyword("WATER_REFLECTIONS");
				m_Material.EnableKeyword("WATER_REFLECTIONS");
			}
			switch (CFGOptions.Graphics.WaterReflections)
			{
			case 0:
				m_Resolution = TextureDownsample.x1;
				break;
			case 1:
				m_Resolution = TextureDownsample.x0_5;
				break;
			case 2:
				m_Resolution = TextureDownsample.x0_25;
				break;
			}
			float num = (float)m_Resolution / 100f * (float)Screen.width;
			float num2 = (float)m_Resolution / 100f * (float)Screen.height;
			m_ReflCam.SetOwner(this);
			m_ReflCam.SetInitialParameters(m_Shader, ref m_Material, (int)num, (int)num2);
		}
		else
		{
			Shader.DisableKeyword("WATER_REFLECTIONS");
			base.enabled = false;
		}
	}

	private void Update()
	{
		m_ReflCam.gameObject.SetActive(IsVisible());
	}

	public float GetClippingHeight()
	{
		if (m_OverrideClippingHeight)
		{
			return m_ClippingHeight;
		}
		return base.transform.position.y;
	}

	private void OnDisable()
	{
		if ((bool)m_ReflCam)
		{
			Object.Destroy(m_ReflCam.gameObject);
		}
		if (m_IsGlobal)
		{
			Shader.DisableKeyword("WATER_REFLECTIONS");
		}
		else
		{
			m_Material.DisableKeyword("WATER_REFLECTIONS");
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_ReflCam)
		{
			Object.Destroy(m_ReflCam.gameObject);
		}
		if (m_IsGlobal)
		{
			Shader.DisableKeyword("WATER_REFLECTIONS");
		}
		else
		{
			m_Material.DisableKeyword("WATER_REFLECTIONS");
		}
	}

	private bool IsVisible()
	{
		if ((bool)m_Renderer)
		{
			if (m_Renderer.isVisible)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
