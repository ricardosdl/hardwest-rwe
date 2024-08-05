using System;
using System.Collections;
using UnityEngine;

public class CFGConstantFog : MonoBehaviour
{
	private Camera m_Camera;

	private float m_CameraY;

	private float m_StartCameraY;

	private float m_StartNearDist;

	private bool m_Updated;

	public void UpdateData()
	{
		m_Updated = false;
		StartCoroutine(UpdateDataCoroutine());
	}

	private IEnumerator UpdateDataCoroutine()
	{
		yield return new WaitForEndOfFrame();
		if (base.gameObject.GetComponent<GlobalFog>() != null)
		{
			m_StartNearDist = base.gameObject.GetComponent<GlobalFog>().startDistance;
		}
		else
		{
			m_StartNearDist = -1f;
		}
		m_Camera = base.gameObject.GetComponent<Camera>();
		m_CameraY = -1f;
		if (m_Camera != null)
		{
			CFGCamera cfgCam = m_Camera.GetComponent<CFGCamera>();
			if ((bool)cfgCam)
			{
				float min_dist = cfgCam.m_MinDistance;
				float alfa = cfgCam.m_InitAngleV;
				m_StartCameraY = min_dist * Mathf.Sin(alfa * ((float)Math.PI / 180f));
			}
			else
			{
				m_StartCameraY = 11.31f;
			}
		}
		m_Updated = true;
	}

	private void Update()
	{
		if (!CFGSingleton<CFGGame>.Instance.IsInStrategic() && m_Updated && m_Camera != null && m_CameraY != m_Camera.transform.position.y && base.gameObject.GetComponent<GlobalFog>() != null)
		{
			base.gameObject.GetComponent<GlobalFog>().startDistance = m_StartNearDist + (m_Camera.transform.position.y - m_StartCameraY);
			m_CameraY = m_Camera.transform.position.y;
		}
	}
}
