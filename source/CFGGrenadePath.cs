using System.Collections.Generic;
using UnityEngine;

public class CFGGrenadePath
{
	public List<Vector3> m_Waypoints = new List<Vector3>();

	public bool Calculate(Vector3 StartPoint, Vector3 EndPoint, float MaxDistance, bool realThrow = false)
	{
		m_Waypoints.Clear();
		Vector3 vector = StartPoint;
		if (!realThrow)
		{
			vector += Vector3.up;
		}
		float y = (vector + (EndPoint - vector) * 0.5f + Vector3.up * 2f).y;
		float num = EndPoint.y - vector.y;
		float num2 = Mathf.Sqrt(y);
		float num3 = Mathf.Sqrt(y - num);
		float num4 = 0.5f * num2 / num3;
		Vector3 vector2 = vector + (EndPoint - vector) * num4 + Vector3.up * 2f;
		vector2.y = y;
		float num5 = y - vector.y;
		m_Waypoints.Add(vector);
		Vector3 vector3 = vector2;
		vector3.y = vector.y;
		Vector3 vector4 = Vector3.Normalize(vector - vector3);
		float num6 = Vector3.Distance(vector3, vector);
		int num7 = 10;
		for (int i = 0; i < num7; i++)
		{
			float num8 = 1f - 1f / (float)num7 * (float)i;
			Vector3 item = vector3 + vector4 * num6 * num8;
			item.y = vector2.y - num6 * num8 * num6 * num8 * num5 / (num6 * num6);
			if (item.y > vector.y)
			{
				m_Waypoints.Add(item);
			}
		}
		m_Waypoints.Add(vector2);
		num5 = y - EndPoint.y;
		vector3.y = EndPoint.y;
		Vector3 vector5 = Vector3.Normalize(EndPoint - vector3);
		num6 = Vector3.Distance(vector3, EndPoint);
		for (int j = 0; j < num7; j++)
		{
			float num9 = 1f / (float)num7 * (float)j;
			Vector3 item2 = vector3 + vector5 * num6 * num9;
			item2.y = vector2.y - num6 * num9 * num6 * num9 * num5 / (num6 * num6);
			if (item2.y > EndPoint.y)
			{
				m_Waypoints.Add(item2);
			}
		}
		m_Waypoints.Add(EndPoint);
		return true;
	}

	public void Copy(CFGGrenadePath Other)
	{
		m_Waypoints.Clear();
		if (Other != null && Other.m_Waypoints != null && Other.m_Waypoints.Count != 0)
		{
			for (int i = 0; i < Other.m_Waypoints.Count; i++)
			{
				m_Waypoints.Add(Other.m_Waypoints[i]);
			}
		}
	}

	private Vector3 CatmullRomVec3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 normalized = (p2 - p1).normalized;
		Vector3 normalized2 = (p3 - p2).normalized;
		p0 = p1;
		p3 = p2;
		float num = Vector3.Distance(p1, p2);
		float a = 1.4f;
		if (Vector3.Dot(normalized, normalized2) < 0.2f)
		{
			a = 0.35f;
		}
		float b = num * 0.4f;
		p1 += ((!(normalized.sqrMagnitude > 0f)) ? Vector3.zero : (normalized * Mathf.Min(a, b)));
		p2 -= ((!(normalized2.sqrMagnitude > 0f)) ? Vector3.zero : (normalized2 * Mathf.Min(a, b)));
		CFGMath.Bezier(out var result, p0, p1, p2, p3, t);
		return result;
	}
}
