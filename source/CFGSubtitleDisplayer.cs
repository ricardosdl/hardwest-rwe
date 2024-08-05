using UnityEngine;

public class CFGSubtitleDisplayer : MonoBehaviour
{
	private Camera m_Cam;

	private RenderTexture m_Tex;

	private void Awake()
	{
		m_Cam = GetComponent<Camera>();
	}

	private void OnGUI()
	{
		if (!(m_Tex == null))
		{
			GUI.depth = -1;
			Graphics.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), m_Tex, null);
		}
	}

	public void StopDisplaying()
	{
		if (m_Cam != null)
		{
			m_Cam.targetTexture = null;
		}
		if (m_Tex != null)
		{
			m_Tex.Release();
			m_Tex = null;
		}
	}

	public void StartDisplaying()
	{
		if (!(m_Cam == null))
		{
			m_Tex = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
			m_Tex.wrapMode = TextureWrapMode.Clamp;
			m_Tex.filterMode = FilterMode.Bilinear;
			m_Cam.targetTexture = m_Tex;
		}
	}
}
