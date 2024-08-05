using System;
using System.Collections;
using UnityEngine;

public class CFGConstantClippingPlane : MonoBehaviour
{
	private Camera m_Camera;

	private CFGCamera m_CfgCam;

	private float m_CameraY;

	private float m_MaxCameraY;

	private float m_StartFarDist;

	private bool m_Updated;

	public void UpdateData()
	{
		m_Updated = false;
		StartCoroutine(UpdateDataCoroutine());
	}

	private IEnumerator UpdateDataCoroutine()
	{
		yield return new WaitForEndOfFrame();
		m_Camera = base.gameObject.GetComponent<Camera>();
		m_CameraY = -1f;
		if (m_Camera != null)
		{
			m_StartFarDist = m_Camera.farClipPlane;
			m_CfgCam = m_Camera.GetComponent<CFGCamera>();
			if ((bool)m_CfgCam)
			{
				float min_dist = m_CfgCam.m_MinDistance;
				float alfa = m_CfgCam.m_InitAngleV;
				m_MaxCameraY = 5.375f + min_dist * Mathf.Sin(alfa * ((float)Math.PI / 180f));
			}
			else
			{
				m_MaxCameraY = 16.685001f;
			}
		}
		m_Updated = true;
	}

	public void UpdateClippingPlane()
	{
		if (!CFGSingleton<CFGGame>.Instance.IsInStrategic() && !(m_CfgCam == null) && m_Updated && m_Camera != null && m_CameraY != m_Camera.transform.position.y)
		{
			if (m_Camera.transform.position.y > m_MaxCameraY)
			{
				float initAngleV = m_CfgCam.m_InitAngleV;
				float num = (m_Camera.transform.position.y - m_MaxCameraY) / Mathf.Sin(initAngleV * ((float)Math.PI / 180f));
				m_Camera.farClipPlane = m_StartFarDist + num * 2f;
			}
			else
			{
				m_Camera.farClipPlane = m_StartFarDist;
			}
			m_CameraY = m_Camera.transform.position.y;
		}
	}
}
