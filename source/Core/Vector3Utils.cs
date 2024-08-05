using UnityEngine;

namespace Core;

public static class Vector3Utils
{
	public static float Magnitude2D(this Vector3 v)
	{
		float y = v.y;
		v.y = 0f;
		float magnitude = v.magnitude;
		v.y = y;
		return magnitude;
	}
}
