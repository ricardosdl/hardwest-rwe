using System;
using UnityEngine;

[Serializable]
public class CFGCameraFollowInfo
{
	public float m_Distance = 3f;

	public float m_MaxSpeed = 9f;

	public float m_Acceleration = 32f;

	[SerializeField]
	private Rect m_TemplateRect = default(Rect);

	private Rect m_DeadZone = new Rect(2f, 2f, 2f, 2f);

	public Rect DeadZone
	{
		get
		{
			if (m_DeadZone.width == 2f)
			{
				float x = Mathf.Clamp01(m_TemplateRect.x) * (float)Screen.width;
				float y = Mathf.Clamp01(m_TemplateRect.y) * (float)Screen.height;
				float x2 = Mathf.Clamp01(m_TemplateRect.width) * (float)Screen.width;
				float y2 = Mathf.Clamp01(m_TemplateRect.height) * (float)Screen.height;
				m_DeadZone = new Rect(new Vector2(x, y), new Vector2(x2, y2));
			}
			return m_DeadZone;
		}
	}

	public bool Contains(Vector3 _point)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(_point);
		Vector2 point = new Vector2(vector.x, (float)Screen.height - vector.y);
		return DeadZone.Contains(point);
	}
}
