using System;
using UnityEngine;

public class CFGCurve : MonoBehaviour
{
	[HideInInspector]
	public Transform[] points;

	public static Vector3 Interpolate(Vector3[] p, float pos)
	{
		int num = p.Length - 3;
		if (num <= 0)
		{
			return new Vector3(0f, 0f, 0f);
		}
		int num2 = Mathf.Min(Mathf.FloorToInt(pos * (float)num), num - 1);
		float num3 = pos * (float)num - (float)num2;
		Vector3 vector = p[num2];
		Vector3 vector2 = p[num2 + 1];
		Vector3 vector3 = p[num2 + 2];
		Vector3 vector4 = p[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	public Vector3 GetForwardNormal(float p, float sampleDist)
	{
		float length = GetLength();
		Vector3 position = GetPosition(p);
		Vector3 position2 = GetPosition(p + sampleDist / length);
		Vector3 position3 = GetPosition(p - sampleDist / length);
		Vector3 normalized = (position2 - position).normalized;
		Vector3 normalized2 = (position3 - position).normalized;
		Vector3 result = Vector3.Slerp(normalized, -normalized2, 0.5f);
		result.Normalize();
		return result;
	}

	public float GetLength()
	{
		if (points.Length < 3)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 1; i < points.Length - 2; i++)
		{
			if (!points[i] || !points[i + 1])
			{
				return 0f;
			}
			num += Vector3.Distance(points[i].position, points[i + 1].position);
		}
		return num;
	}

	public Vector3 GetPosition(float pos, bool clamp)
	{
		if (clamp)
		{
			pos = Mathf.Clamp(pos, 0f, 1f);
		}
		Vector3 result;
		try
		{
			int num = points.Length - 3;
			if (num <= 0)
			{
				return points[0].position;
			}
			int num2 = Mathf.Min(Mathf.FloorToInt(pos * (float)num), num - 1);
			float num3 = pos * (float)num - (float)num2;
			Vector3 position = points[num2].position;
			Vector3 position2 = points[num2 + 1].position;
			Vector3 position3 = points[num2 + 2].position;
			Vector3 position4 = points[num2 + 3].position;
			return 0.5f * ((-position + 3f * position2 - 3f * position3 + position4) * (num3 * num3 * num3) + (2f * position - 5f * position2 + 4f * position3 - position4) * (num3 * num3) + (-position + position3) * num3 + 2f * position2);
		}
		catch (Exception)
		{
			result = new Vector3(0f, 0f, 0f);
		}
		return result;
	}

	public Vector3 GetPosition(float pos)
	{
		return GetPosition(pos, clamp: true);
	}

	public void Main()
	{
	}

	public void OnDrawGizmos()
	{
		if (points == null || points.Length < 3)
		{
			return;
		}
		Vector3 position = base.transform.position;
		bool flag = true;
		position = points[0].position;
		Gizmos.color = new Color(0f, 0.25f, 0.4f, 0.75f);
		for (int i = 0; (float)i < (float)points.Length * 8f; i++)
		{
			Transform transform = points[(int)((float)i / 8f)];
			float pos = (float)i / ((float)points.Length * 8f);
			Vector3 position2 = GetPosition(pos);
			if (!flag)
			{
				Gizmos.DrawLine(position, position2);
			}
			position = position2;
			flag = false;
		}
	}

	public void Start()
	{
		UpdatePoints();
		if (Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	public void Update()
	{
		UpdatePoints();
	}

	public void UpdatePoints()
	{
		points = GetComponentsInChildren<Transform>();
	}
}
