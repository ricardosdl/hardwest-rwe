using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFGPointSprite : MonoBehaviour
{
	public Material m_Material;

	public float m_ScaleY = 0.1f;

	public Rect m_RectUV = new Rect(0f, 0f, 1f, 1f);

	public float m_MinDistance;

	public float m_MaxDistance = -1f;

	public float m_HeightOffset;

	public bool m_AlwaysOnScreen;

	public static bool s_AllVisible = false;

	protected static CFGMultiValueDictionary<int, CFGPointSprite> m_PointSprites = new CFGMultiValueDictionary<int, CFGPointSprite>();

	protected static bool m_Rendered = false;

	public void OnEnable()
	{
		if ((bool)m_Material)
		{
			m_PointSprites.Add(m_Material.GetInstanceID(), this);
		}
	}

	public void OnDisable()
	{
		if ((bool)m_Material)
		{
			m_PointSprites.Remove(m_Material.GetInstanceID(), this);
		}
	}

	private void Update()
	{
		m_Rendered = false;
	}

	public static void Render()
	{
		if (m_Rendered)
		{
			return;
		}
		GL.PushMatrix();
		GL.LoadOrtho();
		foreach (KeyValuePair<int, HashSet<CFGPointSprite>> pointSprite in m_PointSprites)
		{
			IEnumerator enumerator2 = pointSprite.Value.GetEnumerator();
			if (!enumerator2.MoveNext())
			{
				continue;
			}
			CFGPointSprite cFGPointSprite = (CFGPointSprite)enumerator2.Current;
			if (cFGPointSprite == null)
			{
				continue;
			}
			if ((bool)cFGPointSprite.m_Material)
			{
				cFGPointSprite.m_Material.SetPass(0);
			}
			GL.Begin(7);
			GL.Color(Color.white);
			foreach (CFGPointSprite item in pointSprite.Value)
			{
				if (!(item != null) || !item.enabled || !item.gameObject)
				{
					continue;
				}
				Vector3 vector = Camera.main.WorldToViewportPoint(item.transform.position + Vector3.up * item.m_HeightOffset);
				if (!item.m_AlwaysOnScreen && vector.z < 0f && !s_AllVisible)
				{
					continue;
				}
				float num = Vector3.Distance(Camera.main.transform.position, item.transform.position);
				if (!s_AllVisible && (num < item.m_MinDistance || (item.m_MaxDistance >= 0f && num > item.m_MaxDistance)))
				{
					continue;
				}
				float num2 = item.m_ScaleY * 0.5f;
				if (item.m_AlwaysOnScreen || s_AllVisible)
				{
					if (vector.z < 0f)
					{
						vector.y = 1f - vector.y;
						if (vector.y > 0f && vector.y < 0.5f)
						{
							vector.y = 0f;
						}
						else if (vector.y >= 0.5f && vector.y < 1f)
						{
							vector.y = 1f;
						}
					}
					vector.y = Mathf.Clamp(vector.y, num2, 1f - num2);
				}
				float num3 = vector.y - num2;
				float num4 = vector.y + num2;
				if (num3 > 1f || num4 < 0f)
				{
					continue;
				}
				float num5 = num2 / Camera.main.aspect;
				if (item.m_AlwaysOnScreen || s_AllVisible)
				{
					if (vector.z < 0f)
					{
						vector.x = 1f - vector.x;
						if (vector.x > 0f && vector.x < 0.5f)
						{
							vector.x = 0f;
						}
						else if (vector.x >= 0.5f && vector.x < 1f)
						{
							vector.x = 1f;
						}
					}
					vector.x = Mathf.Clamp(vector.x, num5, 1f - num5);
				}
				float num6 = vector.x - num5;
				float num7 = vector.x + num5;
				if (!(num6 > 1f) && !(num7 < 0f))
				{
					GL.TexCoord2(item.m_RectUV.xMin, item.m_RectUV.yMax);
					GL.Vertex3(num6, num4, 0f);
					GL.TexCoord2(item.m_RectUV.xMax, item.m_RectUV.yMax);
					GL.Vertex3(num7, num4, 0f);
					GL.TexCoord2(item.m_RectUV.xMax, item.m_RectUV.yMin);
					GL.Vertex3(num7, num3, 0f);
					GL.TexCoord2(item.m_RectUV.xMin, item.m_RectUV.yMin);
					GL.Vertex3(num6, num3, 0f);
				}
			}
			GL.End();
		}
		GL.PopMatrix();
		m_Rendered = true;
	}
}
