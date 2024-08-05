using System;
using UnityEngine;

public static class CFGMath
{
	public static readonly Vector2 INIFITY = Vector2.one * float.PositiveInfinity;

	public static readonly Vector3 INIFITY3 = Vector3.one * float.PositiveInfinity;

	public static float CalcHorizontalAngle(Vector3 a, Vector3 b)
	{
		float f = Mathf.Clamp(a.x * b.x + a.z * b.z, -1f, 1f);
		float num = Mathf.Acos(f) * 57.29578f;
		return num * Mathf.Sign(a.z * b.x - a.x * b.z);
	}

	public static Vector3 ProjectedVectorOnPlane(Vector3 vec, Vector3 plane_normal)
	{
		return vec - Vector3.Dot(vec, plane_normal) * plane_normal;
	}

	public static Vector3 DiagonalAlignedProjectedVectorOnPlane(Vector3 vec, Vector3 plane_normal)
	{
		Vector3 rhs = vec - Vector3.Dot(vec, plane_normal) * plane_normal;
		rhs.Normalize();
		Vector3 normalized = (Vector3.forward + Vector3.right).normalized;
		Vector3 normalized2 = (Vector3.back + Vector3.right).normalized;
		float num = Vector3.Dot(normalized, rhs);
		if (num > 0.702f)
		{
			return normalized;
		}
		if (num < -0.702f)
		{
			return -normalized;
		}
		num = Vector3.Dot(normalized2, rhs);
		if (num > 0.702f)
		{
			return normalized2;
		}
		return -normalized2;
	}

	public static Vector3 LinearTween(float t, Vector3 b, Vector3 c, float d)
	{
		return c * t / d + b;
	}

	public static float EaseInOutSine(float t, float b, float c, float d)
	{
		return c * -0.5f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
	}

	public static float EaseInOutSineAngle(float t, float b, float c, float d)
	{
		c = Mathf.Repeat(c, 360f);
		if (c > 180f)
		{
			c -= 360f;
		}
		return c * -0.5f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
	}

	public static Vector3 EaseInOutSine(float t, Vector3 b, Vector3 c, float d)
	{
		return c * -0.5f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
	}

	public static Vector3 EaseInOutQuad(float t, Vector3 b, Vector3 c, float d)
	{
		t /= d / 2f;
		if (t < 1f)
		{
			return c / 2f * t * t + b;
		}
		t -= 1f;
		return -c / 2f * (t * (t - 2f) - 1f) + b;
	}

	public static int LineSegmentIntersectionPlane(ref Vector3 LineStart, ref Vector3 LineEnd, ref Vector3 PlanePt, ref Vector3 PlaneNormal, out Vector3 Point)
	{
		Vector3 vector = LineEnd - LineStart;
		Vector3 rhs = LineStart - PlanePt;
		float num = Vector3.Dot(PlaneNormal, vector);
		float num2 = 0f - Vector3.Dot(PlaneNormal, rhs);
		if (Mathf.Abs(num) < 0.001f)
		{
			Point = INIFITY3;
			if (num2 == 0f)
			{
				return 2;
			}
			return 0;
		}
		float num3 = num2 / num;
		if (num3 < 0f || num3 > 1f)
		{
			Point = INIFITY3;
			return 4;
		}
		Point = LineStart + num3 * vector;
		return 1;
	}

	public static float IntersectRaySphere(Vector3 Origin, Vector3 Dir, Vector3 Center, float Radius)
	{
		Vector3 vector = Center - Origin;
		float num = Vector3.Dot(vector, Dir);
		float num2 = Vector3.Dot(vector, vector);
		float num3 = Radius * Radius;
		float num4 = num2 - num * num;
		uint num5 = uint.MaxValue;
		uint num6 = ((num < 0f) ? num5 : 0u) & ((num2 > num3) ? num5 : 0u);
		num6 |= ((num4 > num3) ? num5 : 0u);
		if (num6 == num5)
		{
			return -1f;
		}
		float num7 = Mathf.Sqrt(num3 - num4);
		float result = num - num7;
		float result2 = num + num7;
		uint num8 = ((num2 <= num3) ? num5 : 0u);
		float num9 = -1f;
		if (num8 == 0)
		{
			return result;
		}
		return result2;
	}

	public static bool CheckSegmentIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 point)
	{
		float num = p1.y - p3.y;
		float num2 = p4.x - p3.x;
		float num3 = p1.x - p3.x;
		float num4 = p4.y - p3.y;
		float num5 = p2.x - p1.x;
		float num6 = p2.y - p1.y;
		float num7 = num5 * num4 - num6 * num2;
		float num8 = num * num2 - num3 * num4;
		if (num7 == 0f)
		{
			point = INIFITY;
			if (num8 == 0f)
			{
				return false;
			}
			return false;
		}
		double num9 = num8 / num7;
		if (num9 < 0.0 || num9 > 1.0)
		{
			point = INIFITY;
			return false;
		}
		double num10 = (num * num5 - num3 * num6) / num7;
		if (num10 < 0.0 || num10 > 1.0)
		{
			point = INIFITY;
			return false;
		}
		point = new Vector2((float)((double)p1.x + num9 * (double)num5), (float)((double)p1.y + num9 * (double)num6));
		return true;
	}

	public static float Determinant(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
	{
		float x = xAxis.x;
		float y = xAxis.y;
		float z = xAxis.z;
		float x2 = yAxis.x;
		float y2 = yAxis.y;
		float z2 = yAxis.z;
		float x3 = zAxis.x;
		float y3 = zAxis.y;
		float z3 = zAxis.z;
		float num = y2 * z3 - y3 * z2;
		float num2 = y3 * z - y * z3;
		float num3 = y * z2 - y2 * z;
		return x * num + x2 * num2 + x3 * num3;
	}

	public static Vector3 Intersection(Plane plane1, Plane plane2, Plane plane3)
	{
		float num = Determinant(plane1.normal, plane2.normal, plane3.normal);
		if (num == 0f)
		{
			return Vector3.zero;
		}
		return (Vector3.Cross(plane2.normal, plane3.normal) * (0f - plane1.distance) + Vector3.Cross(plane3.normal, plane1.normal) * (0f - plane2.distance) + Vector3.Cross(plane1.normal, plane2.normal) * (0f - plane3.distance)) / num;
	}

	public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float num = t * t;
		float num2 = num * t;
		float num3 = t * 0.5f;
		float num4 = num2 * 0.5f;
		float num5 = num2 * 1.5f;
		Vector3 vector = p0 * (num - num4 - num3);
		Vector3 vector2 = p1 * (num5 - num * 2.5f + 1f);
		Vector3 vector3 = p2 * (num * 2f - num5 + num3);
		Vector3 vector4 = p3 * (num4 - num * 0.5f);
		return vector + vector2 + vector3 + vector4;
	}

	public static void Bezier(out Vector3 result, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		if (t < 0f)
		{
			t = 0f;
		}
		else if (t > 1f)
		{
			t = 1f;
		}
		float num = p0.x + (p1.x - p0.x) * t;
		float num2 = p0.y + (p1.y - p0.y) * t;
		float num3 = p0.z + (p1.z - p0.z) * t;
		float num4 = p1.x + (p2.x - p1.x) * t;
		float num5 = p1.y + (p2.y - p1.y) * t;
		float num6 = p1.z + (p2.z - p1.z) * t;
		float num7 = p2.x + (p3.x - p2.x) * t;
		float num8 = p2.y + (p3.y - p2.y) * t;
		float num9 = p2.z + (p3.z - p2.z) * t;
		float num10 = num + (num4 - num) * t;
		float num11 = num2 + (num5 - num2) * t;
		float num12 = num3 + (num6 - num3) * t;
		float num13 = num4 + (num7 - num4) * t;
		float num14 = num5 + (num8 - num5) * t;
		float num15 = num6 + (num9 - num6) * t;
		result.x = num10 + (num13 - num10) * t;
		result.y = num11 + (num14 - num11) * t;
		result.z = num12 + (num15 - num12) * t;
	}
}
