using UnityEngine;

public class CFGLight : MonoBehaviour
{
	private Light m_Light;

	private void Awake()
	{
		m_Light = base.gameObject.AddComponent<Light>();
		m_Light.hideFlags = HideFlags.NotEditable;
		m_Light.type = LightType.Point;
		m_Light.shadows = LightShadows.Hard;
		SetLight(0f, 0f);
	}

	public void SetColor(Color lightColor)
	{
		m_Light.color = lightColor;
	}

	public void SetLight(float lightIntensity, float lightRange)
	{
		m_Light.intensity = lightIntensity;
		m_Light.range = lightRange;
	}

	public void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}
}
