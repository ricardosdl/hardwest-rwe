using UnityEngine;
using UnityEngine.Rendering;

public class CFGPlanarReflectionCamera : MonoBehaviour
{
	private CFGPlanarReflection m_Owner;

	private Material m_Material;

	private Camera m_Camera;

	private RenderTexture m_RT;

	private Vector3 m_NewPosition;

	private Vector3 m_NewRotation;

	public void SetOwner(CFGPlanarReflection owner)
	{
		m_Owner = owner;
	}

	public void SetInitialParameters(Shader replacementShader, ref Material surfaceMaterial, int reflectionWidth, int reflectionHeight)
	{
		if ((bool)m_Owner)
		{
			base.transform.SetParent(m_Owner.transform);
		}
		m_Camera = base.gameObject.AddComponent<Camera>();
		m_Camera.renderingPath = RenderingPath.Forward;
		m_Camera.farClipPlane = 150f;
		m_Camera.backgroundColor = new Color(1f, 1f, 1f, 1f);
		m_Camera.clearFlags = CameraClearFlags.Color;
		m_Camera.opaqueSortMode = OpaqueSortMode.FrontToBack;
		m_RT = new RenderTexture(reflectionWidth, reflectionHeight, 16, RenderTextureFormat.R8);
		m_RT.generateMips = false;
		m_RT.filterMode = FilterMode.Bilinear;
		m_RT.wrapMode = TextureWrapMode.Repeat;
		m_Camera.SetReplacementShader(replacementShader, "RenderType");
		m_Material = surfaceMaterial;
		m_Camera.targetTexture = m_RT;
		if (!m_Owner.m_IsGlobal)
		{
			m_Material.SetFloat("_ReflectionStrength", m_Owner.GetReflectionStrength());
		}
		else
		{
			Shader.SetGlobalFloat("_ReflectionStrength", m_Owner.GetReflectionStrength());
		}
	}

	private void SetPlanarReflectionCamera()
	{
		if ((bool)m_Camera && (bool)m_Material && (bool)m_Owner)
		{
			m_NewPosition = Camera.main.transform.position;
			m_NewPosition.y = 0f - (m_NewPosition.y - m_Owner.GetClippingHeight()) + m_Owner.GetClippingHeight();
			base.transform.position = m_NewPosition;
			m_NewRotation = Camera.main.transform.eulerAngles;
			m_NewRotation.x = 0f - m_NewRotation.x;
			m_NewRotation.z += 180f;
			base.transform.eulerAngles = m_NewRotation;
			m_Camera.fieldOfView = Camera.main.fieldOfView;
			m_Camera.cullingMask = Camera.main.cullingMask;
			if (!m_Owner.m_IsGlobal)
			{
				m_Material.SetTexture("_ReflectionTexture", m_RT);
			}
			else
			{
				Shader.SetGlobalTexture("_ReflectionTexture", m_RT);
			}
			Shader.SetGlobalFloat("_ReflectionPlaneY", m_Owner.GetClippingHeight());
		}
		else
		{
			Debug.LogWarning("Camera component or surface material is missing in CFGPlanarReflectionCamera!", base.gameObject);
		}
	}

	private void OnPreRender()
	{
		SetPlanarReflectionCamera();
	}

	private void OnDestroy()
	{
		Object.Destroy(m_RT);
	}
}
