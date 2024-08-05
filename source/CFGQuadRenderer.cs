using System;
using UnityEngine;

public static class CFGQuadRenderer
{
	public static bool s_AllHidden;

	private static Mesh m_Mesh;

	public static void DrawQuadBB(Vector3 start, Vector3 end, float width, Color color, Material material)
	{
		if (!(start == end) && !s_AllHidden)
		{
			if (m_Mesh == null)
			{
				CreateQuadMesh();
			}
			Vector3 vector = end - start;
			float num = (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
			vector.Normalize();
			if (!(vector == Vector3.zero))
			{
				Quaternion q = Quaternion.LookRotation(vector, GetOrthogonal(vector));
				Vector3 s = new Vector3(1f, width, num);
				Vector3 pos = (start + end) * 0.5f;
				Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.AddColor("_TintColor", color);
				materialPropertyBlock.AddFloat("_Lenght", num);
				Graphics.DrawMesh(m_Mesh, matrix, material, 1, null, 0, materialPropertyBlock);
			}
		}
	}

	public static void DrawQuadStrip(Vector3 start, Vector3 end, float width, int segments, Material material)
	{
		if (!(start == end) && !s_AllHidden)
		{
			if (m_Mesh == null)
			{
				CreateQuadMesh();
			}
			float num = 1f / (float)segments;
			Vector3 vector = end - start;
			float z = vector.magnitude / (float)segments;
			Vector3 normalized = Vector3.Cross(new Plane(start, end, Camera.main.transform.position).normal, vector).normalized;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, normalized);
			vector.Normalize();
			Quaternion q = quaternion * Quaternion.LookRotation(vector, GetOrthogonal(vector));
			Vector3 s = new Vector3(1f, width, z);
			for (int i = 0; i < segments; i++)
			{
				float num2 = (float)i * num;
				float t = num2 + num;
				Vector3 vector2 = Vector3.Lerp(start, end, num2);
				Vector3 vector3 = Vector3.Lerp(start, end, t);
				Vector3 pos = (vector2 + vector3) * 0.5f;
				Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
				Graphics.DrawMesh(m_Mesh, matrix, material, 11);
			}
		}
	}

	public static void DrawQuadStripV(Vector3 start, Vector3 end, float width, Material material)
	{
		if (!(start == end))
		{
			if (m_Mesh == null)
			{
				CreateQuadMesh();
			}
			Vector3 vector = end - start;
			float magnitude = vector.magnitude;
			Vector3 normalized = Vector3.Cross(new Plane(start, end, Camera.main.transform.position).normal, vector).normalized;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, normalized);
			vector.Normalize();
			Quaternion q = quaternion * Quaternion.LookRotation(vector, GetOrthogonal(vector));
			Vector3 s = new Vector3(1f, width, magnitude);
			Vector3 pos = (start + end) * 0.5f;
			Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
			Graphics.DrawMesh(m_Mesh, matrix, material, 11);
		}
	}

	public static void DrawQuadH(Vector3 pos, float size, Material material)
	{
		if (m_Mesh == null)
		{
			CreateQuadMesh();
		}
		Quaternion q = Quaternion.Euler(0f, 0f, 90f);
		Vector3 s = new Vector3(size, size, size);
		Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
		Graphics.DrawMesh(m_Mesh, matrix, material, 11);
	}

	public static void DrawQuadH(Vector3 pos, float size, Color color, Material material)
	{
		if (m_Mesh == null)
		{
			CreateQuadMesh();
		}
		Quaternion q = Quaternion.Euler(0f, 0f, 90f);
		Vector3 s = new Vector3(size, size, size);
		Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.AddColor("_TintColor", color);
		Graphics.DrawMesh(m_Mesh, matrix, material, 11, null, 0, materialPropertyBlock);
	}

	private static void CreateQuadMesh()
	{
		m_Mesh = new Mesh();
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, -0.5f, 0.5f)
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		m_Mesh.vertices = vertices;
		m_Mesh.uv = uv;
		m_Mesh.triangles = triangles;
	}

	private static Vector3 GetOrthogonal(Vector3 vector)
	{
		if (Mathf.Abs(vector.y) < 0.9f)
		{
			return Vector3.Cross(vector, Vector3.up);
		}
		return Vector3.Cross(vector, Vector3.right);
	}
}
