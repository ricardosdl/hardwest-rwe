using System.Collections;
using UnityEngine;

public class CFGCellShadowMap
{
	protected eCellShadowMapType m_MapType;

	protected int m_Width;

	protected int m_Height;

	protected int m_Floors;

	protected int m_AllocatedWidth;

	protected BitArray m_ShadowMap;

	protected byte[] m_ShadowMapLDet;

	protected Vector3 m_SunDir = Vector3.left;

	protected bool m_bDarkness;

	private int errcnt;

	public int Width => m_Width;

	public int Height => m_Height;

	public int Floors => m_Floors;

	public int WidthInTexels => m_Width * m_MapType.GetMultiplier();

	public int HeightInTexels => m_Height * m_MapType.GetMultiplier();

	public Vector3 SunDirection => m_SunDir;

	public eCellShadowMapType MapType => m_MapType;

	public bool CheckIfValid()
	{
		if (m_ShadowMap == null || m_ShadowMapLDet == null)
		{
			return false;
		}
		if (m_Width == 0 || m_Height == 0 || m_AllocatedWidth == 0 || m_Floors == 0)
		{
			return false;
		}
		return true;
	}

	public void DestroyData()
	{
		m_Width = 0;
		m_Height = 0;
		m_Floors = 0;
		m_AllocatedWidth = 0;
		m_ShadowMap = null;
		m_ShadowMapLDet = null;
	}

	public void ClearMap()
	{
		if (m_ShadowMap != null)
		{
			m_ShadowMap.SetAll(value: false);
		}
		if (m_ShadowMapLDet != null)
		{
			for (int i = 0; i < m_ShadowMapLDet.Length; i++)
			{
				m_ShadowMapLDet[i] = 0;
			}
		}
	}

	public bool CreateMap(eCellShadowMapType _MapType, int MapZSizeInTiles, int MapXSizeInTiles, int Floors)
	{
		if (_MapType == eCellShadowMapType.NoShadowMap)
		{
			DestroyData();
			return true;
		}
		int multiplier = _MapType.GetMultiplier();
		int num = multiplier * ((MapZSizeInTiles + 7) / 8);
		bool flag = true;
		if (_MapType == m_MapType && m_AllocatedWidth == num && m_Height == MapXSizeInTiles && Floors == m_Floors)
		{
			flag = true;
		}
		if (flag)
		{
			DestroyData();
			int num2 = MapZSizeInTiles * MapXSizeInTiles * multiplier * multiplier;
			m_ShadowMap = new BitArray(Floors * num2);
			if (m_ShadowMap == null)
			{
				DestroyData();
				Debug.LogError("Failed to allocate memory for the shadow map");
				return false;
			}
			m_ShadowMapLDet = new byte[Floors * (MapXSizeInTiles * MapZSizeInTiles)];
			if (m_ShadowMapLDet == null)
			{
				DestroyData();
				Debug.LogError("Failed to allocate memory for the shadow low detail map");
				return false;
			}
			m_AllocatedWidth = num;
			m_Floors = Floors;
			m_Width = MapZSizeInTiles;
			m_Height = MapXSizeInTiles;
			m_MapType = _MapType;
		}
		ClearMap();
		return true;
	}

	public bool SetSunDir(Vector3 NewDir, bool bForceRecalc, bool bDarkness)
	{
		m_bDarkness = bDarkness;
		if (NewDir.x == m_SunDir.x && NewDir.y == m_SunDir.y && NewDir.z == m_SunDir.z)
		{
			if (bForceRecalc && CheckIfValid())
			{
				return Recalculate();
			}
			return true;
		}
		m_SunDir = NewDir;
		if (CheckIfValid())
		{
			return Recalculate();
		}
		return true;
	}

	public bool Recalculate(int MinZ = -1, int MaxZ = -1, int MinX = -1, int MaxX = -1, int MinFloor = -1, int MaxFloor = -1, bool bClear = true)
	{
		if (!CheckIfValid())
		{
			return false;
		}
		if (bClear)
		{
			ClearMap();
		}
		if (MinZ < 0)
		{
			MinZ = 0;
		}
		if (MaxZ < 0)
		{
			MaxZ = m_Width;
		}
		if (MinX < 0)
		{
			MinX = 0;
		}
		if (MaxX < 0)
		{
			MaxX = m_Height;
		}
		if (MinFloor < 0)
		{
			MinFloor = 0;
		}
		if (MaxFloor < 0)
		{
			MaxFloor = m_Floors;
		}
		for (int i = MinFloor; i < MaxFloor; i++)
		{
			Recalc(i, MinZ, MaxZ, MinX, MaxX);
		}
		return true;
	}

	public bool GetShadowPoint(int _Floor, int ZAxis, int XAxis)
	{
		int multiplier = m_MapType.GetMultiplier();
		int num = m_Width * m_Height * multiplier * multiplier;
		int index = _Floor * num + XAxis * m_Width * multiplier + ZAxis;
		return m_ShadowMap[index];
	}

	public int GetVisiblePoints(CFGCellShadowMapChar.CFGCellShadow shadowcell, int DeltaZ, int DeltaX, int DeltaF)
	{
		if (shadowcell == null)
		{
			return 0;
		}
		int num = 0;
		int multiplier = m_MapType.GetMultiplier();
		int num2 = m_Width * m_Height * multiplier * multiplier;
		num2 *= shadowcell.DF + DeltaF;
		for (int i = 0; i < shadowcell.Resolution; i++)
		{
			for (int j = 0; j < shadowcell.Resolution; j++)
			{
				int num3 = num2 + ((shadowcell.DX + DeltaX) * multiplier + i) * m_Width * multiplier + (DeltaZ + shadowcell.DZ) * multiplier + j;
				if (num3 >= 0 && num3 < m_ShadowMap.Length && shadowcell.Array[i * shadowcell.Resolution + j] && !m_ShadowMap[num3])
				{
					num++;
				}
			}
		}
		return num;
	}

	private void SetShadowPoint(int _Floor, int ZAxis, int XAxis, bool bVal)
	{
		int multiplier = m_MapType.GetMultiplier();
		int num = m_Width * m_Height * multiplier * multiplier;
		int index = _Floor * num + XAxis * m_Width * multiplier + ZAxis;
		m_ShadowMap[index] = bVal;
	}

	private void Recalc(int _Floor, int StartZ = -1, int EndZ = -1, int StartX = -1, int EndX = -1)
	{
		if (StartX < 0)
		{
			StartX = 0;
		}
		if (StartZ < 0)
		{
			StartZ = 0;
		}
		if (EndX < 0)
		{
			EndX = m_Height + 1;
		}
		if (EndZ < 0)
		{
			EndZ = m_Width + 1;
		}
		int num = StartX * m_MapType.GetMultiplier();
		int num2 = StartZ * m_MapType.GetMultiplier();
		int num3 = EndZ * m_MapType.GetMultiplier();
		int num4 = EndX * m_MapType.GetMultiplier();
		float y = (float)_Floor * 2.5f;
		float num5 = 1f / (float)m_MapType.GetMultiplier();
		float num6 = num5 * 0.5f;
		for (int i = num2; i < num3; i++)
		{
			for (int j = num; j < num4; j++)
			{
				Vector3 vector = new Vector3((float)j * num5 + num6, y, (float)i * num5 + num6);
				CFGCell cell = CFGCellMap.GetCell(vector);
				bool flag = false;
				if ((bool)cell && cell.IsInLight)
				{
					flag = true;
				}
				if (cell == null || !cell.HaveFloor || flag)
				{
					SetShadowPoint(_Floor, i, j, bVal: false);
				}
				else if (m_bDarkness && (bool)cell && !cell.CheckFlag(0, 32))
				{
					SetShadowPoint(_Floor, i, j, bVal: true);
				}
				else
				{
					SetShadowPoint(_Floor, i, j, !SunVisible(vector));
				}
			}
		}
	}

	private bool CheckLine(ref Vector3 RayStart, ref Vector3 RayEnd, ref Vector3 PlanePT, ref Vector3 PlaneNormal, Vector3 AddV, byte wallflag, byte otherflag, float fAdd)
	{
		CFGCell cFGCell = null;
		Vector3 zero = Vector3.zero;
		Vector3 vector = RayEnd - RayStart;
		Vector3 zero2 = Vector3.zero;
		float num = Vector3.Dot(PlaneNormal, vector);
		if (Mathf.Abs(num) < 0.001f)
		{
			return true;
		}
		int num2 = CFGCellMap.m_Map.Length;
		if (num2 < 1)
		{
			return true;
		}
		bool nightmare = CFGGame.Nightmare;
		byte b = 20;
		Vector3 vector2 = new Vector3(0.5f * PlaneNormal.x * fAdd, 0f, 0.5f * PlaneNormal.z * fAdd);
		Vector3 vector3 = vector.normalized * -0.01f;
		int maxFloor = CFGCellMap.MaxFloor;
		while (true)
		{
			zero2 = RayStart - PlanePT;
			float num3 = 0f - Vector3.Dot(PlaneNormal, zero2);
			float num4 = num3 / num;
			if ((double)num4 < 0.0 || (double)num4 > 1.0)
			{
				break;
			}
			zero = RayStart + num4 * vector;
			zero += vector3;
			int num5 = (int)zero.z;
			int num6 = (int)zero.x;
			int num7 = (int)(zero.y / 2.5f);
			cFGCell = null;
			if (num5 >= m_Width || num6 >= m_Height || num7 >= maxFloor || num5 < 0 || num6 < 0 || num7 < 0)
			{
				errcnt++;
				return true;
			}
			int num8 = num7 * (m_Width * m_Height) + num6 * m_Width + num5;
			if (num8 < num2)
			{
				cFGCell = CFGCellMap.m_Map[num8];
				if (cFGCell != null)
				{
					byte b2 = ((!nightmare || (cFGCell.Flags[wallflag] & 1) != 1) ? cFGCell.Flags[wallflag] : cFGCell.Flags[wallflag + 6]);
					if ((b2 & b) != 0)
					{
						return false;
					}
					if ((b2 & 2u) != 0)
					{
						float num9 = (float)cFGCell.Floor * 2.5f + 1.25f;
						if (zero.y < num9)
						{
							return false;
						}
					}
				}
			}
			float num10 = Mathf.Repeat(Vector3.Dot(zero, AddV), 1f);
			if (num10 < 0.05f || num10 > 0.95f)
			{
				zero += vector2;
				num5 = (int)zero.z;
				num6 = (int)zero.x;
				cFGCell = null;
				if (num5 < m_Width && num6 < m_Height && num7 < m_Floors && num5 >= 0 && num6 >= 0 && num7 >= 0)
				{
					num8 = num7 * (m_Width * m_Height) + num6 * m_Width + num5;
					if (num8 < num2)
					{
						cFGCell = CFGCellMap.m_Map[num8];
						if (cFGCell != null)
						{
							byte b3 = ((!nightmare || (cFGCell.Flags[wallflag] & 1) != 1) ? cFGCell.Flags[wallflag] : cFGCell.Flags[wallflag + 6]);
							if ((b3 & b) != 0)
							{
								return false;
							}
							if ((b3 & 2u) != 0)
							{
								float num11 = (float)cFGCell.Floor * 2.5f + 1.25f;
								if (zero.y < num11)
								{
									return false;
								}
							}
						}
					}
				}
			}
			PlanePT += PlaneNormal;
			if (PlanePT.z < 0f || PlanePT.z > (float)(CFGCellMap.ZAxisSize + 1) || PlanePT.y < 0f || PlanePT.y > 10f || PlanePT.x < 0f || PlanePT.x > (float)(CFGCellMap.XAxisSize + 1))
			{
				return true;
			}
		}
		return true;
	}

	private bool SunVisible(Vector3 StartPt)
	{
		int num = (int)(StartPt.y / 2.5f) + 1;
		Vector3 vector = -m_SunDir.normalized;
		Vector3 LineStart = StartPt;
		Vector3 LineEnd = StartPt + vector * 1000f;
		Vector3 PlanePt = new Vector3(0f, 2.5f * (float)num, 0f);
		Vector3 PlaneNormal = Vector3.up;
		Vector3 Point = Vector3.zero;
		CFGCell cFGCell = null;
		for (int i = num; i < 4; i++)
		{
			PlanePt.y = (float)i * 2.5f;
			if (CFGMath.LineSegmentIntersectionPlane(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, out Point) == 1)
			{
				Point.y += 0.1f;
				cFGCell = CFGCellMap.GetCell(Point);
				if ((bool)cFGCell && cFGCell.HaveFloor)
				{
					return false;
				}
			}
		}
		if (vector.z < -1E-05f)
		{
			PlanePt.Set(0f, 0f, Mathf.Floor(StartPt.z));
			PlaneNormal = new Vector3(0f, 0f, -1f);
			if (!CheckLine(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, new Vector3(1f, 0f, 0f), 5, 5, -1f))
			{
				return false;
			}
		}
		else if (vector.z > 1E-05f)
		{
			PlanePt.Set(0f, 0f, 1f + Mathf.Floor(StartPt.z));
			PlaneNormal = new Vector3(0f, 0f, 1f);
			if (!CheckLine(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, new Vector3(1f, 0f, 0f), 4, 4, -1f))
			{
				return false;
			}
		}
		if (vector.x > 1E-05f)
		{
			PlanePt.Set(1f + Mathf.Floor(StartPt.x), 0f, 0f);
			PlaneNormal = new Vector3(1f, 0f, 0f);
			if (!CheckLine(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, new Vector3(0f, 0f, 1f), 3, 3, -1f))
			{
				return false;
			}
		}
		else if (vector.x < -1E-05f)
		{
			PlanePt.Set(Mathf.Floor(StartPt.x), 0f, 0f);
			PlaneNormal = new Vector3(-1f, 0f, 0f);
			if (!CheckLine(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, new Vector3(0f, 0f, 1f), 2, 2, -1f))
			{
				return false;
			}
		}
		return true;
	}
}
