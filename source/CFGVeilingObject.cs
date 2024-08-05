using UnityEngine;
using UnityEngine.Rendering;

public class CFGVeilingObject : MonoBehaviour
{
	protected CFGCellObject m_Parent;

	private bool m_HasShadowCaster;

	protected int m_Floor = -1;

	private LightShafts m_LightShafts_Script;

	[SerializeField]
	private bool m_HideLight = true;

	protected CFGCamera m_Camera;

	protected float m_Alpha = 1f;

	public CFGCellObject Parent
	{
		get
		{
			return m_Parent;
		}
		set
		{
			m_Parent = value;
		}
	}

	public bool HasShadowCaster
	{
		get
		{
			return m_HasShadowCaster;
		}
		set
		{
			m_HasShadowCaster = value;
		}
	}

	public int Floor
	{
		get
		{
			return m_Floor;
		}
		set
		{
			m_Floor = Mathf.Clamp(value, 0, 8);
		}
	}

	public float Alpha => m_Alpha;

	public void GenerateMaterialPacks()
	{
		UpdateObjectAlpha(m_Alpha);
	}

	private void Start()
	{
		m_Camera = Camera.main.GetComponent<CFGCamera>();
		m_LightShafts_Script = GetComponent<LightShafts>();
		GenerateMaterialPacks();
	}

	private void Update()
	{
		float num = CalcDestAlpha();
		if (m_Alpha != num)
		{
			m_Alpha = num;
			UpdateObjectAlpha(m_Alpha);
		}
	}

	protected virtual float CalcDestAlpha()
	{
		return 1f;
	}

	protected void UpdateObjectAlpha(float alpha)
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		bool flag = m_Parent != null && m_Parent.m_CurrentVisualisation != null && m_Parent.m_CurrentVisualisation.HasShadowCaster;
		if (!flag)
		{
			flag = HasShadowCaster;
		}
		if (alpha == 1f)
		{
			ShadowCastingMode shadowCastingMode = ((!flag) ? ShadowCastingMode.On : ShadowCastingMode.Off);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!renderer.gameObject.name.StartsWith("shadowcaster"))
				{
					renderer.enabled = true;
					renderer.shadowCastingMode = shadowCastingMode;
				}
			}
			if (m_LightShafts_Script != null)
			{
				m_LightShafts_Script.enabled = true;
			}
			if (m_HideLight && GetComponent<Light>() != null)
			{
				GetComponent<Light>().enabled = true;
			}
			if (GetComponent<ParticleSystem>() != null)
			{
				GetComponent<ParticleSystem>().enableEmission = true;
			}
			return;
		}
		if (flag)
		{
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer2 in array2)
			{
				if (!renderer2.gameObject.name.StartsWith("shadowcaster"))
				{
					renderer2.enabled = false;
				}
			}
		}
		else
		{
			Renderer[] array3 = componentsInChildren;
			foreach (Renderer renderer3 in array3)
			{
				if (!renderer3.gameObject.name.StartsWith("shadowcaster"))
				{
					renderer3.enabled = true;
					renderer3.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
				}
			}
		}
		if (m_LightShafts_Script != null)
		{
			m_LightShafts_Script.enabled = false;
		}
		if (m_HideLight && GetComponent<Light>() != null)
		{
			GetComponent<Light>().enabled = false;
		}
		if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().enableEmission = false;
		}
	}
}
