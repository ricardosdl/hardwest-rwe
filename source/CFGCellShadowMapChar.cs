using System.Collections.Generic;
using UnityEngine;

public class CFGCellShadowMapChar
{
	public class CFGCellShadow
	{
		public const int ARRAYSIZE = 256;

		public int DZ;

		public int DX;

		public int DF;

		public bool[] Array = new bool[256];

		public int Resolution;

		public int Count;

		public void Clear()
		{
			Count = 0;
			if (Array != null)
			{
				for (int i = 0; i < 256; i++)
				{
					Array[i] = false;
				}
			}
		}

		public int Recalculate(int ZPos, int XPos, int Floor, int Res, Vector3 SunDir, Vector3 BoxMin, Vector3 BoxMax)
		{
			Clear();
			DZ = ZPos;
			DX = XPos;
			DF = Floor;
			Resolution = Res;
			float num = 1f / (float)Res;
			float num2 = 1f / ((float)Res * 2f);
			Vector3 vector = new Vector3((float)XPos + num2, (float)Floor * 2.5f, (float)ZPos + num2);
			Vector3 zero = Vector3.zero;
			zero.y = vector.y;
			zero.z = vector.z;
			Count = 0;
			for (int i = 0; i < Res; i++)
			{
				zero.x = vector.x;
				for (int j = 0; j < Res; j++)
				{
					bool flag = RayIntersects(zero, -SunDir, BoxMin, BoxMax);
					Array[j * Res + i] = flag;
					if (flag)
					{
						Count++;
					}
					zero.x += num;
				}
				zero.z += num;
			}
			return Count;
		}

		public bool RayIntersects(Vector3 RayStart, Vector3 RayDir, Vector3 BoxMin, Vector3 BoxMax)
		{
			if (RayStart.x >= BoxMin.x && RayStart.x <= BoxMax.x && RayStart.y >= BoxMin.y && RayStart.y <= BoxMax.y && RayStart.z >= BoxMin.z && RayStart.z <= BoxMax.z)
			{
				return true;
			}
			Vector3 vector = new Vector3(-1f, -1f, -1f);
			if (RayStart.x < BoxMin.x && RayStart.x != 0f)
			{
				vector.x = (BoxMin.x - RayStart.x) / RayDir.x;
			}
			else if (RayStart.x > BoxMax.x && RayDir.x != 0f)
			{
				vector.x = (BoxMax.x - RayStart.x) / RayDir.x;
			}
			if (RayStart.y < BoxMin.y && RayDir.y != 0f)
			{
				vector.y = (BoxMin.y - RayStart.y) / RayDir.y;
			}
			else if (RayStart.y > BoxMax.y && RayDir.y != 0f)
			{
				vector.y = (BoxMax.y - RayStart.y) / RayDir.y;
			}
			if (RayStart.z < BoxMin.z && RayDir.z != 0f)
			{
				vector.z = (BoxMin.z - RayStart.z) / RayDir.z;
			}
			else if (RayStart.z > BoxMax.z && RayDir.z != 0f)
			{
				vector.z = (BoxMax.z - RayStart.z) / RayDir.z;
			}
			float num;
			if (vector.x > vector.y && vector.x > vector.z)
			{
				if (vector.x < 0f)
				{
					return false;
				}
				num = RayStart.z + vector.x * RayDir.z;
				if (num < BoxMin.z || num > BoxMax.z)
				{
					return false;
				}
				num = RayStart.y + vector.x * RayDir.y;
				if (num < BoxMin.y || num > BoxMax.y)
				{
					return false;
				}
				return true;
			}
			if (vector.y > vector.x && vector.y > vector.z)
			{
				if (vector.y < 0f)
				{
					return false;
				}
				num = RayStart.z + vector.y * RayDir.z;
				if (num < BoxMin.z || num > BoxMax.z)
				{
					return false;
				}
				num = RayStart.x + vector.y * RayDir.x;
				if (num < BoxMin.x || num > BoxMax.x)
				{
					return false;
				}
				return true;
			}
			if (vector.z < 0f)
			{
				return false;
			}
			num = RayStart.x + vector.z * RayDir.x;
			if (num < BoxMin.x || num > BoxMax.x)
			{
				return false;
			}
			num = RayStart.y + vector.z * RayDir.y;
			if (num < BoxMin.y || num > BoxMax.y)
			{
				return false;
			}
			return true;
		}
	}

	public List<CFGCellShadow> m_Cells = new List<CFGCellShadow>();

	public int m_Resolution = 1;

	public bool Init(eCellShadowMapType ShadowType)
	{
		if (m_Cells == null)
		{
			m_Cells = new List<CFGCellShadow>();
		}
		if (m_Cells == null)
		{
			return false;
		}
		m_Cells.Clear();
		m_Resolution = 1;
		return true;
	}

	private static void UpdateMinMax(ref Vector3 _Min, ref Vector3 _Max, Vector3 _Point, Vector3 _Dir, float minf)
	{
		Vector3 PlaneNormal = new Vector3(0f, 1f, 0f);
		Vector3 PlanePt = new Vector3(0f, -2.5f * minf, 0f);
		Vector3 LineEnd = _Point + _Dir * 1000f;
		if (CFGMath.LineSegmentIntersectionPlane(ref _Point, ref LineEnd, ref PlanePt, ref PlaneNormal, out var Point) == 1)
		{
			_Min.x = Mathf.Min(_Min.x, Point.x);
			_Min.y = Mathf.Min(_Min.y, Point.y);
			_Min.z = Mathf.Min(_Min.z, Point.z);
			_Min.x = Mathf.Min(_Min.x, _Point.x);
			_Min.y = Mathf.Min(_Min.y, _Point.y);
			_Min.z = Mathf.Min(_Min.z, _Point.z);
			_Max.x = Mathf.Max(_Max.x, Point.x);
			_Max.y = Mathf.Max(_Max.y, Point.y);
			_Max.z = Mathf.Max(_Max.z, Point.z);
			_Max.x = Mathf.Max(_Max.x, _Point.x);
			_Max.y = Mathf.Max(_Max.y, _Point.y);
			_Max.z = Mathf.Max(_Max.z, _Point.z);
		}
	}

	public static void CalcBox(float ZMin, float ZMax, float XMin, float XMax, float YMin, float YMax, Vector3 SunDir, out Vector3 CalcMin, out Vector3 CalcMax)
	{
		Vector3 _Min = new Vector3(100000f, 10000f, 10000f);
		Vector3 _Max = new Vector3(-100000f, -10000f, -10000f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMin, YMin, ZMin), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMax, YMin, ZMin), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMax, YMin, ZMax), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMin, YMin, ZMax), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMin, YMax, ZMin), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMax, YMax, ZMin), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMax, YMax, ZMax), SunDir, 0f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(XMin, YMax, ZMax), SunDir, 0f);
		CalcMin = _Min;
		CalcMax = _Max;
	}

	public void Recalculate(Vector3 SunDir, int Resolution)
	{
		m_Cells.Clear();
		m_Resolution = Resolution;
		float chararacterHeightForShadow = CFGLevelSettings.ChararacterHeightForShadow;
		Vector3 boxMin = new Vector3(0f, 0f, 0f);
		Vector3 boxMax = new Vector3(1f, chararacterHeightForShadow, 1f);
		CFGCellShadow cFGCellShadow = new CFGCellShadow();
		int num = -10;
		int num2 = -10;
		int num3 = 11;
		int num4 = 11;
		int num5 = -2;
		int num6 = 0;
		Vector3 _Min = new Vector3(100000f, 10000f, 10000f);
		Vector3 _Max = new Vector3(-100000f, -10000f, -10000f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(0f, 0f, 0f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(1f, 0f, 0f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(1f, 0f, 1f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(0f, 0f, 1f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(0f, chararacterHeightForShadow, 0f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(1f, chararacterHeightForShadow, 0f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(1f, chararacterHeightForShadow, 1f), SunDir, 2f);
		UpdateMinMax(ref _Min, ref _Max, new Vector3(0f, chararacterHeightForShadow, 1f), SunDir, 2f);
		num = (int)_Min.z;
		num2 = (int)_Min.x;
		num4 = (int)_Max.x + 1;
		num3 = (int)_Max.z + 1;
		int num7 = 0;
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num; j < num3; j++)
			{
				for (int k = num2; k < num4; k++)
				{
					if (cFGCellShadow.Recalculate(j, k, i, Resolution, SunDir, boxMin, boxMax) > 0)
					{
						m_Cells.Add(cFGCellShadow);
						cFGCellShadow = new CFGCellShadow();
						if (cFGCellShadow == null)
						{
							return;
						}
					}
					num7++;
				}
			}
		}
	}
}
