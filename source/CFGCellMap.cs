using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class CFGCellMap
{
	public const string SID_LA_LAVis = "LAVis";

	public const string SID_LA_Both = "Both";

	public const string SID_LA_Normal = "nrm";

	public const string SID_LA_Nightmare = "night";

	public const string SID_FLOOR = "floor_";

	public const float HALFCOVERHEIGTH = 0.25f;

	public const float FLOOR_EPSILON = 0.1f;

	public static bool m_bLOS_UseSideStepsForStartPoint = true;

	public static bool m_bLOS_UseSideStepsForEndPoint = true;

	public static bool m_bLOS_UseHalfCovers = true;

	public static ECoverType m_MinSidestepCoverType = ECoverType.FULL;

	public static float SHOOT_HEIGHT = 1.7f;

	private static CFGCellObject m_RootObject = null;

	public static CFGCell[] m_Map = null;

	private static int m_Width = -1;

	private static int m_Height = -1;

	private static int m_Floors = -1;

	private static List<CFGCellObject> m_PostApply = new List<CFGCellObject>();

	private static Vector2 m_LastIntersection = Vector2.zero;

	private static float m_LastIntersectionY = 1.25f;

	private static float WallSize = 0.1f;

	private static float Wall1Size = 0.9f;

	public static int ZAxisSize => m_Width;

	public static int XAxisSize => m_Height;

	public static int MaxFloor => m_Floors;

	public static bool IsValid => m_Map != null && m_Width > 0 && m_Height > 0;

	public static Vector3 LastIntersection => new Vector3(m_LastIntersection.y, m_LastIntersectionY, m_LastIntersection.x);

	public static CFGCell GetCell(int ZAxis, int XAxis, int Floor)
	{
		if (m_Map == null || ZAxis >= m_Width || XAxis >= m_Height || Floor >= m_Floors || ZAxis < 0 || XAxis < 0 || Floor < 0)
		{
			return null;
		}
		int num = Floor * (m_Width * m_Height) + XAxis * m_Width + ZAxis;
		if (num >= m_Map.Length)
		{
			return null;
		}
		return m_Map[num];
	}

	public static CFGCell GetCell(Vector3 Point)
	{
		return GetCell((int)Point.z, (int)Point.x, (int)(Point.y / 2.5f));
	}

	public static CFGCell GetCharacterCell(Vector3 Point)
	{
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			return GetCell((int)Point.z, (int)Point.x, 0);
		}
		return GetCell((int)Point.z, (int)Point.x, (int)((Point.y + 0.08f) / 2.5f));
	}

	public static CFGCell GetCell(float x, float y, float z)
	{
		return GetCell((int)z, (int)x, (int)(y / 2.5f));
	}

	public static void DestroyMap()
	{
		m_Map = null;
		m_Width = -1;
		m_Height = -1;
		m_Floors = -1;
	}

	public static void CreateMap(CFGCellObject cobject, CFGCell.SPData rdata = null, bool bAlternate = false, bool bHandleVis = true, bool bRecalcShadows = false)
	{
		DestroyMap();
		CreateDefaults();
		SHOOT_HEIGHT = 1.7f;
		if (Application.isPlaying && (bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance)
		{
			SHOOT_HEIGHT = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ShootHeight;
			SetWallSize(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_WallSize);
			Debug.Log("Wall size set to " + WallSize);
		}
		if (cobject == null)
		{
			return;
		}
		m_Width = cobject.Size_Z;
		m_Height = cobject.Size_X;
		m_Floors = cobject.Floors;
		m_Map = new CFGCell[m_Width * m_Height * m_Floors];
		if (m_Map == null)
		{
			Debug.LogError("Failed to allocate memory for cellmap");
			DestroyMap();
			return;
		}
		m_PostApply.Clear();
		ApplyHierarchy(cobject, bAlternate, bHandleVis);
		ApplyDualStateObjects(bAlternate, bHandleVis);
		if (rdata != null)
		{
			SpawnTempVis(rdata);
		}
		RecalculateHeights();
		CFGCellShadowMapLevel.CreateMainMapFromLS();
		RecalculateColumns(0, m_Width, 0, m_Height, 0, m_Floors);
	}

	public static void CreateDebugMap(CFGCellObject cobject, CFGCell.SPData rdata, bool bAlternate, byte flagToCheck)
	{
		DestroyMap();
		CreateDefaults();
		if (cobject == null)
		{
			return;
		}
		m_Width = cobject.Size_Z;
		m_Height = cobject.Size_X;
		m_Floors = cobject.Floors;
		m_Map = new CFGCell[m_Width * m_Height * m_Floors];
		if (m_Map == null)
		{
			Debug.LogError("Failed to allocate memory for cellmap");
			DestroyMap();
			return;
		}
		ApplyHierarchy(cobject, bAlternate, bHandleVis: false);
		ApplyDualStateObjects(bAlternate, bHandleVis: false);
		if (rdata != null)
		{
			SpawnDebugVis(rdata, flagToCheck);
		}
		RecalculateHeights();
	}

	private static void SpawnDebugVis(CFGCell.SPData rdata, byte flag)
	{
		for (int i = 0; i < m_RootObject.Floors; i++)
		{
			for (int j = 0; j < m_Width; j++)
			{
				for (int k = 0; k < m_Height; k++)
				{
					rdata.cell = GetCell(j, k, i);
					if (rdata.cell != null)
					{
						CFGCell.SpawnCellDebugVis(j, k, (float)i * 2.5f, rdata, flag);
					}
				}
			}
		}
	}

	private static void ClearDeltaHeights()
	{
		for (int i = 0; i < m_RootObject.Floors; i++)
		{
			for (int j = 0; j < m_Width; j++)
			{
				for (int k = 0; k < m_Height; k++)
				{
					CFGCell cell = GetCell(j, k, i);
					if ((bool)cell)
					{
						cell.DeltaHeight = 0f;
					}
				}
			}
		}
	}

	private static void RecalculateHeights()
	{
		ClearDeltaHeights();
		for (int i = 0; i < m_RootObject.Floors; i++)
		{
			for (int j = 0; j < m_Width; j++)
			{
				for (int k = 0; k < m_Height; k++)
				{
					CFGCell cell = GetCell(j, k, i);
					if (cell != null && cell.StairsType == CFGCell.EStairsType.EntryPoint && cell.DeltaHeight == 0f)
					{
						CFGCell cFGCell = null;
						cFGCell = GetCell(j + 1, k, i);
						if (cFGCell != null && cFGCell.StairsType == CFGCell.EStairsType.Slope)
						{
							HandleZStairs(j, 1, k, i);
						}
						cFGCell = GetCell(j - 1, k, i);
						if (cFGCell != null && cFGCell.StairsType == CFGCell.EStairsType.Slope)
						{
							HandleZStairs(j, -1, k, i);
						}
						cFGCell = GetCell(j, k - 1, i);
						if (cFGCell != null && cFGCell.StairsType == CFGCell.EStairsType.Slope)
						{
							HandleXStairs(k, -1, j, i);
						}
						cFGCell = GetCell(j, k + 1, i);
						if (cFGCell != null && cFGCell.StairsType == CFGCell.EStairsType.Slope)
						{
							HandleXStairs(k, 1, j, i);
						}
					}
				}
			}
		}
	}

	private static void HandleZStairs(int StartZ, int add, int x, int f)
	{
		int num = StartZ + add;
		for (int i = 1; i < 10; i++)
		{
			CFGCell cell = GetCell(num, x, f);
			if (cell == null)
			{
				return;
			}
			if (cell.StairsType == CFGCell.EStairsType.None)
			{
				cell = GetCell(num, x, f + 1);
				if (cell == null || cell.StairsType != CFGCell.EStairsType.ExitPoint)
				{
					return;
				}
				float num2 = 0f;
				float num3 = 2.5f / (float)(i - 1);
				for (int j = StartZ; j != num; j += add)
				{
					cell = GetCell(j, x, f);
					if (cell != null)
					{
						cell.DeltaHeight = num2;
					}
					num2 = ((num2 != 0f) ? (num2 + num3) : (num2 + 0.5f * num3));
				}
			}
			num += add;
		}
		Debug.Log("Failed to determine stairs");
	}

	private static void HandleXStairs(int StartX, int add, int z, int f)
	{
		int i = 1;
		int num = StartX + add;
		for (; i < 10; i++)
		{
			CFGCell cell = GetCell(z, num, f);
			if (cell == null)
			{
				return;
			}
			if (cell.StairsType == CFGCell.EStairsType.None)
			{
				cell = GetCell(z, num, f + 1);
				if (cell == null || cell.StairsType != CFGCell.EStairsType.ExitPoint)
				{
					return;
				}
				float num2 = 0f;
				float num3 = 2.5f / (float)(i - 1);
				for (int j = StartX; j != num; j += add)
				{
					cell = GetCell(z, j, f);
					if (cell != null)
					{
						cell.DeltaHeight = num2;
					}
					num2 = ((num2 != 0f) ? (num2 + num3) : (num2 + 0.5f * num3));
				}
			}
			num += add;
		}
		Debug.Log("Failed to determine stairs");
	}

	private static void SpawnTempVis(CFGCell.SPData rdata)
	{
		for (int i = 0; i < m_RootObject.Floors; i++)
		{
			for (int j = 0; j < m_Width; j++)
			{
				for (int k = 0; k < m_Height; k++)
				{
					rdata.cell = GetCell(j, k, i);
					if (rdata.cell == null)
					{
						Debug.LogError("empty " + j.ToString() + " " + k + " f = " + i);
					}
					else
					{
						CFGCell.SpawnCellTempVis(j, k, (float)i * 2.5f, rdata);
					}
				}
			}
		}
	}

	private static void CreateDefaults()
	{
	}

	private static void CreateHierarchy(CFGCellObject target)
	{
		m_RootObject = target;
	}

	private static void ApplyHierarchy(CFGCellObject target, bool bAlternate = false, bool bHandleVis = true)
	{
		CreateHierarchy(target);
		if (!(target == null))
		{
			ApplySubObjects(target.transform, bAlternate, bHandleVis);
		}
	}

	private static void ApplySubObjects(Transform target, bool bAlternate = false, bool bHandleVis = true)
	{
		if (target == null)
		{
			return;
		}
		CFGCellObject component = target.GetComponent<CFGCellObject>();
		if ((bool)component)
		{
			if (!component.gameObject.activeSelf)
			{
				return;
			}
			if (component.HasAlternateState)
			{
				if (!m_PostApply.Contains(component))
				{
					m_PostApply.Add(component);
				}
			}
			else
			{
				ApplyObject(component, bAlternate, bHandleVis);
			}
		}
		CFGCellObjectVisualisation component2 = target.GetComponent<CFGCellObjectVisualisation>();
		if ((bool)component2)
		{
			return;
		}
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (child == null)
			{
				continue;
			}
			if (component != null && string.Compare("LAVis", child.name, ignoreCase: true) == 0)
			{
				if (!bHandleVis)
				{
					continue;
				}
				for (int j = 0; j < child.childCount; j++)
				{
					Transform child2 = child.GetChild(j);
					if (!(child2 == null) && (string.Compare(child2.name, "Both", ignoreCase: true) == 0 || string.Compare(child2.name, "nrm", ignoreCase: true) == 0 || string.Compare(child2.name, "night", ignoreCase: true) == 0))
					{
						ProcessLAObjects(child2, Mathf.FloorToInt(component.transform.position.y / 2.5f), component);
					}
				}
			}
			else
			{
				ApplySubObjects(child, bAlternate);
			}
		}
	}

	private static void ProcessLAObjectShadowcasters(Transform target, CFGCellObjectPart part)
	{
		if (target == null || part == null)
		{
			return;
		}
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (!(child == null) && child.name.StartsWith("shadowcaster"))
			{
				part.HasShadowCaster = true;
				Renderer component = child.GetComponent<Renderer>();
				if ((bool)component)
				{
					component.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
					component.receiveShadows = false;
				}
			}
		}
	}

	private static void ProcessLAObjects(Transform target, int firstfloor, CFGCellObject cobj)
	{
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (child == null)
			{
				continue;
			}
			CFGWorldDetailsLevel component = child.GetComponent<CFGWorldDetailsLevel>();
			if (component != null)
			{
				ProcessLAObjects(child, firstfloor, cobj);
				continue;
			}
			CFGCellObjectPart cFGCellObjectPart = child.GetComponent<CFGCellObjectPart>();
			if (cFGCellObjectPart == null)
			{
				cFGCellObjectPart = child.gameObject.AddComponent<CFGCellObjectPart>();
			}
			if (!(cFGCellObjectPart == null))
			{
				cFGCellObjectPart.Floor = (int)((child.position.y + 0.4f) / 2.5f);
				cFGCellObjectPart.Parent = cobj;
				ProcessLAObjectShadowcasters(child, cFGCellObjectPart);
			}
		}
	}

	public static void ApplySingleObject(CFGCellObject target, bool bAlternate = false, bool bHandleVis = true, ERotation UserRotation = ERotation.None)
	{
		if (!(target == null))
		{
			ApplyObject(target, bAlternate, bHandleVis, UserRotation);
			target.RecalculateShadowMap();
		}
	}

	private static void ApplyDualStateObjects(bool bAlternate, bool bHandleVis = true)
	{
		if (m_PostApply == null || m_PostApply.Count == 0)
		{
			return;
		}
		foreach (CFGCellObject item in m_PostApply)
		{
			ApplyObject(item, bAlternate, bHandleVis);
		}
		m_PostApply.Clear();
	}

	private static void ApplyObject(CFGCellObject target, bool bAlternate = false, bool bHandleVis = true, ERotation UserRotation = ERotation.None)
	{
		if (target.ObjectType == CFGCellObject.EObjectType.MainMap && target == m_RootObject)
		{
			Apply_MainObject();
			return;
		}
		if (target.CheckIfShouldBeDisabled())
		{
			target.SetDisabled();
			return;
		}
		bAlternate |= target.ShouldApplyAlternateState;
		int num = Mathf.FloorToInt(target.transform.position.x);
		int num2 = Mathf.FloorToInt(target.transform.position.z);
		int num3 = Mathf.FloorToInt(target.transform.position.y / 2.5f);
		byte borderflagstopreserve = 127;
		bool flag = target.HasAlternateState && target.Overdraw;
		CFGCellObjectVisualisation cFGCellObjectVisualisation = target.m_CurrentVisualisation;
		if (cFGCellObjectVisualisation == null)
		{
			cFGCellObjectVisualisation = target.m_CurrentTempVisualisation;
		}
		if (!bHandleVis)
		{
			cFGCellObjectVisualisation = null;
		}
		int num4 = 0;
		ERotation rotation = target.Rotation;
		ERotation rotation2 = rotation;
		if (UserRotation != 0)
		{
			int num5;
			for (num5 = (int)rotation + (int)UserRotation; num5 > 3; num5 -= 4)
			{
			}
			rotation2 = (ERotation)num5;
		}
		for (int i = 0; i < target.Floors; i++)
		{
			for (int j = 0; j < target.Size_Z; j++)
			{
				for (int k = 0; k < target.Size_X; k++)
				{
					CFGCell cell = target.GetCell(j, k, i, bAlternate, bExisitingOnly: true);
					int num6 = -1;
					int num7 = -1;
					switch (rotation)
					{
					case ERotation.None:
						num7 = num + k;
						num6 = num2 + j;
						break;
					case ERotation.CW_90:
						num7 = num + j;
						num6 = num2 - k;
						break;
					case ERotation.CW_180:
						num7 = num - k;
						num6 = num2 - j;
						break;
					case ERotation.CW_270:
						num7 = num - j;
						num6 = num2 + k;
						break;
					}
					if (num6 < 0 || num6 >= m_Width || num7 < 0 || num7 >= m_Height)
					{
						continue;
					}
					int num8 = (num3 + i) * (m_Width * m_Height) + num7 * m_Width + num6;
					if (num8 < 0 || num8 >= m_Map.Length)
					{
						continue;
					}
					if (m_Map[num8] == null)
					{
						Debug.LogWarning("empty cell at " + num2 + " " + num + " f = " + num3);
						continue;
					}
					m_Map[num8].OwnerObject = target;
					if (cell == null)
					{
						continue;
					}
					CFGCellObject interiorObject = m_Map[num8].InteriorObject;
					int eP = m_Map[num8].EP;
					CFGCell cFGCell = cell.CreateRotatedCell(rotation2);
					bool flag2 = m_Map[num8].Editor_IsFlagSet(0, 32);
					bool flag3 = m_Map[num8].Editor_IsFlagSet(0, 64);
					m_Map[num8].Flags[0] = cFGCell.Flags[0];
					m_Map[num8].Flags[6] = cFGCell.Flags[6];
					if (flag3)
					{
						m_Map[num8].Flags[0] |= 64;
						m_Map[num8].Flags[6] |= 64;
					}
					else if (m_Map[num8].Editor_IsFlagSet(0, 64))
					{
						m_Map[num8].TriggerObject = target;
					}
					if (flag2)
					{
						m_Map[num8].Flags[0] |= 32;
						m_Map[num8].Flags[6] |= 32;
					}
					if (!flag)
					{
						if (!cFGCell.Editor_IsFlagSet(1, 128))
						{
							m_Map[num8].Flags[1] |= cFGCell.Flags[1];
							m_Map[num8].Flags[7] |= cFGCell.Flags[7];
						}
						if (!cFGCell.Editor_IsFlagSet(2, 128))
						{
							m_Map[num8].Flags[2] |= cFGCell.Flags[2];
							m_Map[num8].Flags[8] |= cFGCell.Flags[8];
						}
						if (!cFGCell.Editor_IsFlagSet(5, 128))
						{
							m_Map[num8].Flags[5] |= cFGCell.Flags[5];
							m_Map[num8].Flags[11] |= cFGCell.Flags[11];
						}
						if (!cFGCell.Editor_IsFlagSet(4, 128))
						{
							m_Map[num8].Flags[4] |= cFGCell.Flags[4];
							m_Map[num8].Flags[10] |= cFGCell.Flags[10];
						}
						if (!cFGCell.Editor_IsFlagSet(3, 128))
						{
							m_Map[num8].Flags[3] |= cFGCell.Flags[3];
							m_Map[num8].Flags[9] |= cFGCell.Flags[9];
						}
					}
					else if (flag)
					{
						if (!cFGCell.Editor_IsFlagSet(1, 128))
						{
							m_Map[num8].Flags[1] = cFGCell.Flags[1];
							m_Map[num8].Flags[7] = cFGCell.Flags[7];
						}
						if (!cFGCell.Editor_IsFlagSet(2, 128))
						{
							m_Map[num8].Flags[2] = cFGCell.Flags[2];
							m_Map[num8].Flags[8] = cFGCell.Flags[8];
						}
						if (!cFGCell.Editor_IsFlagSet(5, 128))
						{
							m_Map[num8].Flags[5] = cFGCell.Flags[5];
							m_Map[num8].Flags[11] = cFGCell.Flags[11];
						}
						if (!cFGCell.Editor_IsFlagSet(4, 128))
						{
							m_Map[num8].Flags[4] = cFGCell.Flags[4];
							m_Map[num8].Flags[10] = cFGCell.Flags[10];
						}
						if (!cFGCell.Editor_IsFlagSet(3, 128))
						{
							m_Map[num8].Flags[3] = cFGCell.Flags[3];
							m_Map[num8].Flags[9] = cFGCell.Flags[9];
						}
					}
					m_Map[num8].EP = eP;
					m_Map[num8].OwnerObject = target;
					m_Map[num8].InteriorObject = interiorObject;
					if (target.IsUsable)
					{
						m_Map[num8].UsableObject = target.GetUsable();
					}
					if (target.IsDoor)
					{
						m_Map[num8].DoorObject = target.GetDoor();
					}
					if (cell.Editor_IsFlagSet(0, 32))
					{
						m_Map[num8].InteriorObject = target;
					}
					if (m_Map[num8].Floor == 0)
					{
						m_Map[num8].Flags[0] |= 2;
						m_Map[num8].Flags[6] |= 2;
					}
					if (!m_Map[num8].Editor_IsFlagSet(1, 64) && (m_Map[num8].Editor_IsFlagSet(2, 64) || m_Map[num8].Editor_IsFlagSet(3, 64) || m_Map[num8].Editor_IsFlagSet(5, 64) || m_Map[num8].Editor_IsFlagSet(4, 64)))
					{
						m_Map[num8].Flags[0] |= 2;
					}
					m_Map[num8].PropagateCenterCover();
					m_Map[num8].CheckIfInLight();
				}
			}
			int num9 = -1;
			int value = -1;
			int num10 = -1;
			int value2 = -1;
			switch (rotation)
			{
			case ERotation.None:
				num9 = num2;
				value = num9 + target.Size_Z;
				num10 = num;
				value2 = num10 + target.Size_X;
				break;
			case ERotation.CW_90:
				num9 = num2 - target.Size_X + 1;
				value = num2 + 1;
				num10 = num;
				value2 = num10 + target.Size_Z;
				break;
			case ERotation.CW_180:
				num9 = num2 - target.Size_Z + 1;
				value = num2 + 1;
				num10 = num - target.Size_X + 1;
				value2 = num + 1;
				break;
			case ERotation.CW_270:
				num9 = num2 - target.Size_Z + 1;
				value = num2 + 1;
				num10 = num - target.Size_Z + 1;
				value2 = num + 1;
				num9 = num2;
				value = num2 + target.Size_X;
				num10 = num - target.Size_Z + 1;
				value2 = num + 1;
				break;
			}
			num10 = Mathf.Clamp(num10, 0, m_Height);
			value2 = Mathf.Clamp(value2, 0, m_Height);
			num9 = Mathf.Clamp(num9, 0, m_Width);
			value = Mathf.Clamp(value, 0, m_Width);
			for (int l = num9; l < value; l++)
			{
				CFGCell cell2;
				CFGCell cell3;
				if (num10 > 0)
				{
					cell2 = GetCell(l, num10 - 1, num3 + i);
					cell3 = GetCell(l, num10, num3 + i);
					CombineBorder(cell3, cell2, flag, 2, 3, borderflagstopreserve);
				}
				cell2 = GetCell(l, value2, num3 + i);
				cell3 = GetCell(l, value2 - 1, num3 + i);
				CombineBorder(cell3, cell2, flag, 3, 2, borderflagstopreserve);
			}
			for (int m = num10; m < value2; m++)
			{
				CFGCell cell2 = GetCell(value, m, num3 + i);
				CFGCell cell3 = GetCell(value - 1, m, num3 + i);
				CombineBorder(cell3, cell2, flag, 4, 5, borderflagstopreserve);
				if (num9 > 0)
				{
					cell2 = GetCell(num9 - 1, m, num3 + i);
					cell3 = GetCell(num9, m, num3 + i);
					CombineBorder(cell3, cell2, flag, 5, 4, borderflagstopreserve);
				}
			}
			string value3 = "floor_" + i;
			if (!cFGCellObjectVisualisation)
			{
				continue;
			}
			bool flag4 = false;
			if (cFGCellObjectVisualisation.transform.childCount == 0)
			{
				flag4 = true;
			}
			else if (cFGCellObjectVisualisation.transform.childCount == 1)
			{
				Transform child = cFGCellObjectVisualisation.transform.GetChild(0);
				if ((bool)child && child.name.StartsWith("shadowcaster"))
				{
					flag4 = true;
				}
			}
			if (flag4)
			{
				CFGCellObjectPart cFGCellObjectPart = cFGCellObjectVisualisation.GetComponent<CFGCellObjectPart>();
				if (cFGCellObjectPart == null)
				{
					cFGCellObjectPart = cFGCellObjectVisualisation.gameObject.AddComponent<CFGCellObjectPart>();
				}
				if ((bool)cFGCellObjectPart)
				{
					cFGCellObjectPart.Parent = target;
					cFGCellObjectPart.Floor = num3 + i;
					num4++;
				}
			}
			for (int n = 0; n < cFGCellObjectVisualisation.transform.childCount; n++)
			{
				Transform child2 = cFGCellObjectVisualisation.transform.GetChild(n);
				if (child2 == null)
				{
					continue;
				}
				if (child2.name.Contains(value3))
				{
					num4 += ProcessVisualisation<CFGCellObjectPart>(cFGCellObjectVisualisation, child2, num3 + i, target);
				}
				if (child2.name.StartsWith("shadowcaster"))
				{
					if ((bool)cFGCellObjectVisualisation)
					{
						cFGCellObjectVisualisation.HasShadowCaster = true;
					}
					Renderer component = child2.GetComponent<Renderer>();
					if ((bool)component)
					{
						component.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
						component.receiveShadows = false;
					}
				}
			}
		}
		if ((bool)cFGCellObjectVisualisation && num4 == 0)
		{
			num4 += ProcessVisualisation<CFGCellObjectPart>(cFGCellObjectVisualisation, cFGCellObjectVisualisation.transform, num3, target);
		}
		bool flag5 = false;
		if (target.m_VisualisationSource != null && bHandleVis && num4 == 0)
		{
			flag5 = true;
		}
		if (flag5)
		{
			Debug.LogWarning("CFGCellMap object is missing visualisation: " + target.name);
		}
	}

	private static void CombineBorder(CFGCell src, CFGCell dst, bool bOverride, byte SRCFlag, byte DSTFlag, byte borderflagstopreserve)
	{
		if ((bool)src && (bool)dst)
		{
			if (bOverride)
			{
				dst.Flags[DSTFlag] = (byte)(src.Flags[SRCFlag] & borderflagstopreserve);
				dst.Flags[DSTFlag + 6] = (byte)(src.Flags[SRCFlag + 6] & borderflagstopreserve);
				return;
			}
			byte b = (byte)((src.Flags[SRCFlag] | dst.Flags[DSTFlag]) & borderflagstopreserve);
			dst.Flags[DSTFlag] |= b;
			src.Flags[SRCFlag] |= b;
			b = (byte)((src.Flags[SRCFlag + 6] | dst.Flags[DSTFlag + 6]) & borderflagstopreserve);
			dst.Flags[DSTFlag + 6] |= b;
			src.Flags[SRCFlag + 6] |= b;
		}
	}

	public static void AddVisScripts(CFGCellObjectVisualisation VisObj, CFGCellObject Parent)
	{
		if (VisObj == null)
		{
			return;
		}
		int num = Mathf.FloorToInt(Parent.transform.position.y / 2.5f);
		int num2 = 0;
		if (VisObj.transform.childCount == 0)
		{
			CFGCellObjectPart cFGCellObjectPart = VisObj.GetComponent<CFGCellObjectPart>();
			if (cFGCellObjectPart == null)
			{
				cFGCellObjectPart = VisObj.gameObject.AddComponent<CFGCellObjectPart>();
			}
			if ((bool)cFGCellObjectPart)
			{
				cFGCellObjectPart.Parent = Parent;
				cFGCellObjectPart.Floor = num;
			}
			return;
		}
		for (int i = 0; i < VisObj.transform.childCount; i++)
		{
			Transform child = VisObj.transform.GetChild(i);
			if ((bool)child && !child.name.Contains("floor_"))
			{
				CFGCellObjectPart cFGCellObjectPart2 = child.GetComponent<CFGCellObjectPart>();
				if (cFGCellObjectPart2 == null)
				{
					cFGCellObjectPart2 = child.gameObject.AddComponent<CFGCellObjectPart>();
				}
				if ((bool)cFGCellObjectPart2)
				{
					cFGCellObjectPart2.Parent = Parent;
					cFGCellObjectPart2.Floor = num;
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			string value = "floor_" + j;
			for (int k = 0; k < VisObj.transform.childCount; k++)
			{
				Transform child2 = VisObj.transform.GetChild(k);
				if (child2 == null)
				{
					continue;
				}
				if (child2.name.Contains(value))
				{
					num2 += ProcessVisualisation<CFGCellObjectPart>(VisObj, child2, num + j, Parent);
				}
				if (child2.name.StartsWith("shadowcaster"))
				{
					if ((bool)VisObj)
					{
						VisObj.HasShadowCaster = true;
					}
					Renderer component = child2.GetComponent<Renderer>();
					if ((bool)component)
					{
						component.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
						component.receiveShadows = false;
					}
				}
			}
		}
	}

	private static int ProcessVisualisation<T>(CFGCellObjectVisualisation VisObj, Transform Trans, int Floor, CFGCellObject Parent) where T : CFGVeilingObject
	{
		int num = 0;
		for (int i = 0; i < Trans.childCount; i++)
		{
			Transform child = Trans.GetChild(i);
			if (child == null)
			{
				continue;
			}
			if (string.Compare(child.name, "LAVis", ignoreCase: true) == 0)
			{
				Debug.LogWarning("LA Vis name detected in visualisation (" + VisObj.name + "). Please move it up in the hierarchy!");
				continue;
			}
			if (child.name.StartsWith("shadowcaster"))
			{
				if ((bool)VisObj)
				{
					VisObj.HasShadowCaster = true;
				}
				Renderer component = child.GetComponent<Renderer>();
				if ((bool)component)
				{
					component.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
					component.receiveShadows = false;
				}
				continue;
			}
			T[] components = child.GetComponents<T>();
			if (components == null || components.Length == 0)
			{
				CFGCellObjectVisualisationFloor component2 = child.GetComponent<CFGCellObjectVisualisationFloor>();
				if (!component2)
				{
					T val = child.gameObject.AddComponent<T>();
					if ((bool)val)
					{
						val.Floor = Floor;
						val.Parent = Parent;
						num++;
					}
				}
			}
			else
			{
				T[] array = components;
				for (int j = 0; j < array.Length; j++)
				{
					T val2 = array[j];
					val2.Floor = Floor;
					val2.Parent = Parent;
					num++;
				}
			}
		}
		return num;
	}

	private static void Apply_MainObject()
	{
		for (int i = 0; i < m_Floors; i++)
		{
			for (int j = 0; j < ZAxisSize; j++)
			{
				for (int k = 0; k < XAxisSize; k++)
				{
					CFGCell cell = m_RootObject.GetCell(j, k, i, bAlternate: false, bExisitingOnly: true);
					CFGCell cFGCell = new CFGCell();
					if (cFGCell == null)
					{
						return;
					}
					cFGCell.EncodePosition(i, k, j);
					if (cell != null)
					{
						for (int l = 0; l < 12; l++)
						{
							cFGCell.Flags[l] = cell.Flags[l];
						}
					}
					if (i == 0)
					{
						cFGCell.Flags[0] |= 2;
						cFGCell.Flags[6] |= 2;
					}
					int num = i * (m_Width * m_Height) + k * m_Width + j;
					m_Map[num] = cFGCell;
					m_Map[num].OwnerObject = m_RootObject;
				}
			}
		}
	}

	private static void DumpMap()
	{
	}

	public static CFGCellObject FindSceneMainObject()
	{
		CFGCellObject[] array = Object.FindObjectsOfType<CFGCellObject>();
		if (array == null || array.Length == 0)
		{
			array = Object.FindObjectsOfType(typeof(CFGCellObject)) as CFGCellObject[];
			if (array == null || array.Length == 0)
			{
				return null;
			}
		}
		CFGCellObject[] array2 = array;
		foreach (CFGCellObject cFGCellObject in array2)
		{
			if (cFGCellObject.ObjectType == CFGCellObject.EObjectType.MainMap)
			{
				return cFGCellObject;
			}
		}
		return null;
	}

	public static bool CanSeeFromTileInDir(CFGCell tile, EDirection dir)
	{
		if (tile == null)
		{
			return false;
		}
		byte flagType = 2;
		switch (dir)
		{
		case EDirection.EAST:
			flagType = 4;
			break;
		case EDirection.WEST:
			flagType = 5;
			break;
		case EDirection.SOUTH:
			flagType = 3;
			break;
		}
		if (tile.CheckFlag(flagType, 16))
		{
			return false;
		}
		return true;
	}

	public static bool CanPassFromTileInDir(CFGCell tile, EDirection dir)
	{
		if (tile == null)
		{
			return false;
		}
		byte flagType = 2;
		switch (dir)
		{
		case EDirection.EAST:
			flagType = 4;
			break;
		case EDirection.WEST:
			flagType = 5;
			break;
		case EDirection.SOUTH:
			flagType = 3;
			break;
		}
		if (tile.CheckFlag(flagType, 8))
		{
			if (tile.CheckFlag(flagType, 64) && (bool)tile.OwnerObject && (bool)tile.OwnerObject.m_CurrentVisualisation)
			{
				CFGDoorObject component = tile.OwnerObject.m_CurrentVisualisation.GetComponent<CFGDoorObject>();
				if (component == null || !component.CanOpen)
				{
					return false;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public static int Distance(CFGCell start, CFGCell end)
	{
		return Mathf.FloorToInt(Vector3.Distance(start.WorldPosition, end.WorldPosition));
	}

	public static ECoverType GetTargetCover(CFGCell start_tile, CFGCell end_tile)
	{
		if (start_tile == null || end_tile == null)
		{
			return ECoverType.FULL;
		}
		CFGCell[] tiles = new CFGCell[3] { start_tile, null, null };
		Vector3 vector = start_tile.WorldPosition + Vector3.up * 1.25f;
		Vector3 vector2 = end_tile.WorldPosition + Vector3.up * 1.25f;
		float num = Mathf.Abs(vector.x - vector2.x);
		float num2 = Mathf.Abs(vector.z - vector2.z);
		float num3 = Mathf.Abs(vector.y - vector2.y);
		if (num < 1.1f && num2 < 1.1f && num3 < 3f)
		{
			return ECoverType.NONE;
		}
		FillUpTable(start_tile, (vector2 - vector).normalized, ref tiles);
		ECoverType centerCover = end_tile.GetCenterCover();
		ECoverType eCoverType = ECoverType.FULL;
		for (int i = 0; i < tiles.Length; i++)
		{
			if (tiles[i] == null)
			{
				continue;
			}
			ECoverType eCoverType2 = end_tile.GetCenterCover();
			for (int j = 0; j < 4; j++)
			{
				EDirection dir = (EDirection)j;
				Vector3 forward = dir.GetForward();
				if (Vector3.Dot(forward, (end_tile.WorldPosition - tiles[i].WorldPosition).normalized) < 0f)
				{
					eCoverType2 = ECoverTypeExtension.Max(eCoverType2, end_tile.GetBorderCover(dir));
				}
			}
			eCoverType = ECoverTypeExtension.Min(eCoverType, eCoverType2);
		}
		if (eCoverType == ECoverType.NONE)
		{
			return centerCover;
		}
		return ECoverTypeExtension.Max(eCoverType, centerCover);
	}

	public static ELOXHitType GetLineOf(CFGCell start_tile, CFGCell end_tile, int distance, byte Flag, bool bUseStartSideSteps, bool bUseEndSideSteps)
	{
		if (start_tile == null || end_tile == null || Distance(start_tile, end_tile) > distance)
		{
			return ELOXHitType.TileBody;
		}
		Vector3 vector = start_tile.WorldPosition + Vector3.up * 1.25f;
		Vector3 vector2 = end_tile.WorldPosition + Vector3.up * 1.25f;
		CFGCell[] tiles = new CFGCell[3] { start_tile, null, null };
		CFGCell[] tiles2 = new CFGCell[3] { end_tile, null, null };
		if (bUseStartSideSteps)
		{
			FillUpTable(start_tile, (vector2 - vector).normalized, ref tiles);
		}
		if (bUseEndSideSteps)
		{
			FillUpTable(end_tile, (vector - vector2).normalized, ref tiles2);
		}
		ELOXHitType lOXHit = GetLOXHit(tiles[0].WorldPosition, ref tiles2, Flag);
		if (lOXHit == ELOXHitType.None)
		{
			return lOXHit;
		}
		if (tiles[1] != null)
		{
			lOXHit = GetLOXHit(tiles[1].WorldPosition, ref tiles2, Flag);
			if (lOXHit == ELOXHitType.None)
			{
				return lOXHit;
			}
			lOXHit = GetLOXHit(tiles[0].WorldPosition + (tiles[1].WorldPosition - tiles[0].WorldPosition) * 0.5f, ref tiles2, Flag);
			if (lOXHit == ELOXHitType.None)
			{
				return lOXHit;
			}
		}
		if (tiles[2] != null)
		{
			lOXHit = GetLOXHit(tiles[2].WorldPosition, ref tiles2, Flag);
			if (lOXHit == ELOXHitType.None)
			{
				return lOXHit;
			}
			lOXHit = GetLOXHit(tiles[0].WorldPosition + (tiles[2].WorldPosition - tiles[0].WorldPosition) * 0.5f, ref tiles2, Flag);
			if (lOXHit == ELOXHitType.None)
			{
				return lOXHit;
			}
		}
		return ELOXHitType.TileBody;
	}

	public static ELOXHitType GetLOXHit(Vector3 StartPos, ref CFGCell[] end_tiles, byte Flag)
	{
		Vector3 vector = Vector3.up * SHOOT_HEIGHT;
		Vector3 begin = StartPos + vector;
		for (int i = 0; i < end_tiles.Length; i++)
		{
			if (end_tiles[i] != null)
			{
				ELOXHitType eLOXHitType = NEW_GetLineOf(begin, end_tiles[i].WorldPosition + vector, Flag);
				if (eLOXHitType == ELOXHitType.None)
				{
					return eLOXHitType;
				}
			}
		}
		if (end_tiles[0] == null)
		{
			return ELOXHitType.TileBody;
		}
		Vector3 vector2 = end_tiles[0].WorldPosition + vector;
		if (end_tiles[1] != null)
		{
			Vector3 vector3 = (end_tiles[1].WorldPosition - end_tiles[0].WorldPosition) * 0.5f;
			ELOXHitType eLOXHitType = NEW_GetLineOf(begin, vector2 + vector3, Flag);
			if (eLOXHitType == ELOXHitType.None)
			{
				return eLOXHitType;
			}
		}
		if (end_tiles[2] != null)
		{
			Vector3 vector4 = (end_tiles[2].WorldPosition - end_tiles[0].WorldPosition) * 0.5f;
			ELOXHitType eLOXHitType = NEW_GetLineOf(begin, vector4 + vector2, Flag);
			if (eLOXHitType == ELOXHitType.None)
			{
				return eLOXHitType;
			}
		}
		return ELOXHitType.TileBody;
	}

	public static ELOXHitType GetLineOfAutoSideSteps(CFGIAttackable Shooter, CFGIAttackable Target, byte Flag, CFGCell start_tile = null, CFGCell end_tile = null, int distance = 0)
	{
		if (start_tile == null && Shooter == null)
		{
			return ELOXHitType.None;
		}
		if (end_tile == null && Target == null)
		{
			return ELOXHitType.None;
		}
		bool bUseEndSideSteps = false;
		bool bUseStartSideSteps = false;
		if (start_tile == null)
		{
			start_tile = Shooter.CurrentCell;
		}
		if (start_tile == null)
		{
			return ELOXHitType.None;
		}
		if (end_tile == null)
		{
			end_tile = Target.CurrentCell;
		}
		if (end_tile == null)
		{
			return ELOXHitType.None;
		}
		if (distance < 1 && Shooter != null)
		{
			distance = 10000;
			CFGCharacter cFGCharacter = Shooter as CFGCharacter;
			if (cFGCharacter != null)
			{
				distance = cFGCharacter.BuffedSight;
			}
		}
		if (m_bLOS_UseSideStepsForEndPoint && Target != null)
		{
			CFGCharacter cFGCharacter2 = Target as CFGCharacter;
			if (cFGCharacter2 != null)
			{
				bUseEndSideSteps = true;
			}
		}
		if (m_bLOS_UseSideStepsForStartPoint && Shooter != null)
		{
			CFGCharacter cFGCharacter3 = Shooter as CFGCharacter;
			if (cFGCharacter3 != null)
			{
				bUseStartSideSteps = true;
			}
		}
		return GetLineOf(start_tile, end_tile, distance, Flag, bUseStartSideSteps, bUseEndSideSteps);
	}

	public static ELOXHitType GetLineOfSightAutoSideSteps(CFGIAttackable Shooter, CFGIAttackable Target, CFGCell start_tile = null, CFGCell end_tile = null, int distance = 0)
	{
		return GetLineOfAutoSideSteps(Shooter, Target, 16, start_tile, end_tile, distance);
	}

	public static ELOXHitType GetLineOfSightNoSidesteps(CFGCell start_tile, CFGCell end_tile, int distance = 10000)
	{
		return GetLineOf(start_tile, end_tile, distance, 16, bUseStartSideSteps: false, bUseEndSideSteps: false);
	}

	public static ELOXHitType GetLineOfFireAutoSideSteps(CFGIAttackable Shooter, CFGIAttackable Target, CFGCell start_tile = null, CFGCell end_tile = null, int distance = 0)
	{
		return GetLineOfAutoSideSteps(Shooter, Target, 32, start_tile, end_tile, distance);
	}

	public static void FillUpTable(CFGCell tile, Vector3 direction, ref CFGCell[] tiles)
	{
		tiles[1] = null;
		tiles[2] = null;
		float num = Vector3.Dot(direction, new Vector3(-1f, 0f, 0f));
		float num2 = Vector3.Dot(direction, new Vector3(0f, 0f, 1f));
		EDirection eDirection = EDirection.EAST;
		float num3 = Mathf.Abs(num2);
		float num4 = Mathf.Abs(num);
		float num5 = Mathf.Abs(num3 - num4);
		if (num5 < 0.3f)
		{
			int num6 = 1;
			eDirection = EDirection.EAST;
			if (num2 < 0f)
			{
				eDirection = EDirection.WEST;
			}
			if (num6 < 3 && FillUpTable_Dir(tile, eDirection, EDirection.SOUTH, ref tiles[num6]))
			{
				num6++;
			}
			if (num6 < 3 && FillUpTable_Dir(tile, eDirection, EDirection.NORTH, ref tiles[num6]))
			{
				num6++;
			}
			eDirection = EDirection.SOUTH;
			if (num > 0f)
			{
				eDirection = EDirection.NORTH;
			}
			if (num6 < 3 && FillUpTable_Dir(tile, eDirection, EDirection.EAST, ref tiles[num6]))
			{
				num6++;
			}
			if (num6 < 3 && FillUpTable_Dir(tile, eDirection, EDirection.WEST, ref tiles[num6]))
			{
				num6++;
			}
		}
		else if (num3 > num4)
		{
			eDirection = ((!(num2 > 0f)) ? EDirection.WEST : EDirection.EAST);
			FillUpTable_Dir(tile, eDirection, EDirection.SOUTH, ref tiles[1]);
			FillUpTable_Dir(tile, eDirection, EDirection.NORTH, ref tiles[2]);
		}
		else
		{
			eDirection = ((!(num > 0f)) ? EDirection.SOUTH : EDirection.NORTH);
			FillUpTable_Dir(tile, eDirection, EDirection.EAST, ref tiles[1]);
			FillUpTable_Dir(tile, eDirection, EDirection.WEST, ref tiles[2]);
		}
	}

	private static bool FillUpTable_Dir(CFGCell tile, EDirection ToTarget, EDirection ToSide, ref CFGCell Side)
	{
		Side = null;
		switch (tile.GetBorderCover(ToTarget))
		{
		case ECoverType.NONE:
			if (m_MinSidestepCoverType != 0)
			{
				return false;
			}
			break;
		case ECoverType.HALF:
			if (m_MinSidestepCoverType == ECoverType.FULL)
			{
				return false;
			}
			break;
		}
		CFGCell cell = GetCell(tile.WorldPosition + ToSide.GetForward());
		if ((bool)cell && cell.HaveFloor && CanPassFromTileInDir(tile, ToSide))
		{
			Side = cell;
			return true;
		}
		return false;
	}

	private static bool CanStandOnBorder(Vector3 Position)
	{
		return false;
	}

	public static bool CanMoveInStraightLine(CFGCell start_tile, CFGCell end_tile, CFGCharacter Mover)
	{
		if (start_tile == null || end_tile == null)
		{
			return false;
		}
		int num = 0;
		int num2 = 0;
		float y = start_tile.WorldPosition.y;
		CFGCell cFGCell = start_tile;
		CFGCell cFGCell2 = end_tile;
		CFGCell cFGCell3 = start_tile;
		CFGCell cFGCell4 = end_tile;
		byte b = 5;
		byte b2 = 4;
		byte b3 = 2;
		byte b4 = 3;
		if (start_tile.PositionX < end_tile.PositionX)
		{
			cFGCell = start_tile;
			cFGCell2 = end_tile;
			num2 = 1;
			b = 4;
			b2 = 5;
		}
		else if (start_tile.PositionX > end_tile.PositionX)
		{
			cFGCell = end_tile;
			cFGCell2 = start_tile;
			num2 = -1;
		}
		if (start_tile.PositionZ < end_tile.PositionZ)
		{
			cFGCell3 = start_tile;
			cFGCell4 = end_tile;
			num = 1;
			b3 = 3;
			b4 = 2;
		}
		else if (start_tile.PositionZ > end_tile.PositionZ)
		{
			cFGCell3 = end_tile;
			cFGCell4 = start_tile;
			num = -1;
		}
		Vector2 p = new Vector2(start_tile.WorldPosition.z, start_tile.WorldPosition.x);
		Vector2 p2 = new Vector2(end_tile.WorldPosition.z, end_tile.WorldPosition.x);
		byte flag = 8;
		int num3 = Mathf.Abs(start_tile.PositionX - end_tile.PositionX);
		if (Mathf.Abs(start_tile.PositionZ - end_tile.PositionZ) == num3)
		{
			CFGCell.EColumn eColumn = CFGCell.EColumn.None;
			EFloorLevelType floor = (EFloorLevelType)start_tile.Floor;
			CFGCell cFGCell5 = null;
			CFGCell cFGCell6 = null;
			if (start_tile.PositionX < end_tile.PositionX)
			{
				cFGCell5 = start_tile;
				cFGCell6 = end_tile;
			}
			else
			{
				cFGCell5 = end_tile;
				cFGCell6 = start_tile;
			}
			num2 = 1;
			b = 4;
			b2 = 5;
			if (cFGCell5.PositionZ > cFGCell6.PositionZ)
			{
				num = -1;
				b3 = 2;
				b4 = 3;
				eColumn = CFGCell.EColumn.NorthEast;
			}
			else
			{
				num = 1;
				b3 = 3;
				b4 = 2;
				eColumn = CFGCell.EColumn.SouthEast;
			}
			for (int i = 0; i <= num3; i++)
			{
				int num4 = cFGCell5.PositionX + num2 * i;
				int num5 = cFGCell5.PositionZ + num * i;
				m_LastIntersection = new Vector2((float)num4 + 0.5f, (float)num5 + 0.5f);
				CFGCell cell = GetCell(num4, num5, (int)floor);
				if (cell == null)
				{
					return false;
				}
				if (cell.CurrentCharacter != null)
				{
					return false;
				}
				if (cell.CheckFlag(1, flag))
				{
					return false;
				}
				if (i != num3)
				{
					if (cell.HasColumn(eColumn))
					{
						return false;
					}
					if (cell.CheckFlag(b, flag))
					{
						return false;
					}
					if (cell.CheckFlag(b3, flag))
					{
						return false;
					}
				}
				if (i != 0)
				{
					if (cell.CheckFlag(b2, flag))
					{
						return false;
					}
					if (cell.CheckFlag(b4, flag))
					{
						return false;
					}
				}
			}
			return true;
		}
		Vector2 p3 = new Vector2(cFGCell.WorldPosition.z - 0.5f, cFGCell.WorldPosition.x);
		Vector2 p4 = new Vector2(cFGCell2.WorldPosition.z + 0.5f, cFGCell.WorldPosition.x);
		for (int num5 = cFGCell3.PositionZ; num5 < cFGCell4.PositionZ; num5++)
		{
			p3.y = (float)num5 + 1f;
			p4.y = p3.y;
			if (!CFGMath.CheckSegmentIntersectionPoint(p3, p4, p, p2, out m_LastIntersection))
			{
				continue;
			}
			CFGCell cell = GetCell(new Vector3(m_LastIntersection.y, cFGCell.WorldPosition.y, m_LastIntersection.x));
			if (cell == null)
			{
				continue;
			}
			if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
			{
				return false;
			}
			if (cell.CheckFlag(1, flag))
			{
				return false;
			}
			if (cell.CheckFlag(2, flag))
			{
				return false;
			}
			float num6 = m_LastIntersection.x - Mathf.Floor(m_LastIntersection.x);
			if (num6 < 0.5f)
			{
				if (cell.CheckFlag(5, flag))
				{
					return false;
				}
				if (cell.HasColumn(CFGCell.EColumn.NorthWest))
				{
					return false;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y - 1f, y, m_LastIntersection.x));
				if ((bool)cell && cell.CheckFlag(5, flag))
				{
					return false;
				}
			}
			if (num6 > 0.5f)
			{
				if (cell.CheckFlag(4, flag))
				{
					return false;
				}
				if (cell.HasColumn(CFGCell.EColumn.NorthEast))
				{
					return false;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y - 1f, y, m_LastIntersection.x));
				if ((bool)cell && cell.CheckFlag(4, flag))
				{
					return false;
				}
			}
			if (num6 < 0.3f)
			{
				cell = GetCell(new Vector3(m_LastIntersection.y, cFGCell.WorldPosition.y, m_LastIntersection.x - 1f));
				if (cell != null)
				{
					if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
					{
						return false;
					}
					if (cell.CheckFlag(1, flag))
					{
						return false;
					}
					if (cell.CheckFlag(2, flag))
					{
						return false;
					}
				}
			}
			else
			{
				if (!(num6 > 0.7f))
				{
					continue;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y, cFGCell.WorldPosition.y, m_LastIntersection.x + 1f));
				if (cell != null)
				{
					if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
					{
						return false;
					}
					if (cell.CheckFlag(1, flag))
					{
						return false;
					}
					if (cell.CheckFlag(2, flag))
					{
						return false;
					}
				}
			}
		}
		p3 = new Vector2(cFGCell3.WorldPosition.x, cFGCell3.WorldPosition.x - 0.5f);
		p4 = new Vector2(cFGCell4.WorldPosition.x, cFGCell4.WorldPosition.x + 0.5f);
		for (int num5 = cFGCell.PositionX; num5 < cFGCell2.PositionX; num5++)
		{
			p3.x = (float)num5 + 1f;
			p4.x = p3.x;
			if (!CFGMath.CheckSegmentIntersectionPoint(p3, p4, p, p2, out m_LastIntersection))
			{
				continue;
			}
			CFGCell cell = GetCell(new Vector3(m_LastIntersection.y, y, m_LastIntersection.x));
			if (cell == null)
			{
				continue;
			}
			if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
			{
				return false;
			}
			if (cell.CheckFlag(1, flag))
			{
				return false;
			}
			if (cell.CheckFlag(5, flag))
			{
				return false;
			}
			float num7 = m_LastIntersection.y - Mathf.Floor(m_LastIntersection.y);
			if (num7 < 0.5f)
			{
				if (cell.CheckFlag(2, flag))
				{
					return false;
				}
				if (cell.HasColumn(CFGCell.EColumn.NorthWest))
				{
					return false;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y, y, m_LastIntersection.x - 1f));
				if ((bool)cell && cell.CheckFlag(2, flag))
				{
					return false;
				}
			}
			if (num7 > 0.5f)
			{
				if (cell.CheckFlag(3, flag))
				{
					return false;
				}
				if (cell.HasColumn(CFGCell.EColumn.SouthWest))
				{
					return false;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y, y, m_LastIntersection.x - 1f));
				if ((bool)cell && cell.CheckFlag(3, flag))
				{
					return false;
				}
			}
			if (num7 < 0.3f)
			{
				cell = GetCell(new Vector3(m_LastIntersection.y - 1f, cFGCell.WorldPosition.y, m_LastIntersection.x));
				if (cell != null)
				{
					if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
					{
						return false;
					}
					if (cell.CheckFlag(1, flag))
					{
						return false;
					}
					if (cell.CheckFlag(5, flag))
					{
						return false;
					}
				}
			}
			else
			{
				if (!(num7 > 0.7f))
				{
					continue;
				}
				cell = GetCell(new Vector3(m_LastIntersection.y + 1f, cFGCell.WorldPosition.y, m_LastIntersection.x));
				if (cell != null)
				{
					if (cell.CurrentCharacter != null && Mover != cell.CurrentCharacter)
					{
						return false;
					}
					if (cell.CheckFlag(1, flag))
					{
						return false;
					}
					if (cell.CheckFlag(5, flag))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static EFloorLevelType GetFloor(float y)
	{
		if (y >= 4.9f)
		{
			return EFloorLevelType.SECOND;
		}
		if (y >= 2.4f)
		{
			return EFloorLevelType.FIRST;
		}
		return EFloorLevelType.ZERO;
	}

	public static void ToggleAdditionalArt(bool bNormal)
	{
		CFGCellObject cFGCellObject = FindSceneMainObject();
		if (cFGCellObject == null)
		{
			Debug.LogWarning("Failed to find main map");
		}
		else
		{
			H_ToggleAdditionalArt(cFGCellObject.transform, bNormal);
		}
	}

	private static void H_ToggleAdditionalArt(Transform t, bool bNormal)
	{
		if (t == null)
		{
			return;
		}
		CFGCellObject component = t.GetComponent<CFGCellObject>();
		if (component != null)
		{
			component.SpawnAdditionalVisualisation(bNormal);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child == null)
			{
				continue;
			}
			CFGCellObjectVisualisation component2 = child.GetComponent<CFGCellObjectVisualisation>();
			if (component2 != null)
			{
				continue;
			}
			if (string.Compare("LAVis", child.name, ignoreCase: true) == 0 && (bool)component)
			{
				if (bNormal)
				{
					ToggleArtDir(child, "nrm", "night");
				}
				else
				{
					ToggleArtDir(child, "night", "nrm");
				}
			}
			else
			{
				H_ToggleAdditionalArt(child, bNormal);
			}
		}
	}

	private static void ToggleArtDir(Transform t, string nameON, string nameOFF)
	{
		if (t == null)
		{
			return;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (!(child == null))
			{
				if (string.Compare(nameON, child.name, ignoreCase: true) == 0)
				{
					child.gameObject.SetActive(value: true);
				}
				else if (string.Compare(nameOFF, child.name, ignoreCase: true) == 0)
				{
					child.gameObject.SetActive(value: false);
				}
			}
		}
	}

	public static void RecalculateColumns(int StartZ, int EndZ, int StartX, int EndX, int StartFloor, int EndFloor)
	{
		int num = Mathf.Min(StartFloor, EndFloor);
		int num2 = Mathf.Max(StartFloor, EndFloor);
		int num3 = Mathf.Min(StartZ, EndZ);
		int num4 = Mathf.Max(StartZ, EndZ);
		int num5 = Mathf.Min(StartX, EndX);
		int num6 = Mathf.Max(StartX, EndX);
		if (num3 < 0)
		{
			num3 = 0;
		}
		if (num5 < 0)
		{
			num5 = 0;
		}
		if (num4 > ZAxisSize)
		{
			num4 = ZAxisSize;
		}
		if (num6 > XAxisSize)
		{
			num6 = XAxisSize;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num2 > MaxFloor)
		{
			num2 = MaxFloor;
		}
		Vector3 direction = new Vector3(1f, 0f, 1f);
		direction.Normalize();
		int layerMask = 262144;
		RaycastHit hitInfo = default(RaycastHit);
		Vector3 vector = default(Vector3);
		for (int i = num; i < num2; i++)
		{
			vector.y = (float)i * 2.5f + 1.25f;
			for (int j = num5; j < num6; j++)
			{
				vector.x = (float)j + 0.5f;
				for (int k = num3; k < num4; k++)
				{
					vector.z = (float)k + 0.5f;
					CFGCell cell = GetCell(vector);
					CFGCell cell2 = GetCell(vector + new Vector3(0f, 0f, 1f));
					CFGCell cell3 = GetCell(vector + new Vector3(1f, 0f, 0f));
					CFGCell cell4 = GetCell(vector + new Vector3(1f, 0f, 1f));
					bool bEnable = Physics.Raycast(vector, direction, out hitInfo, 1f, layerMask);
					if ((bool)cell)
					{
						cell.EnableColumn(CFGCell.EColumn.SouthEast, bEnable);
					}
					if ((bool)cell2)
					{
						cell2.EnableColumn(CFGCell.EColumn.SouthWest, bEnable);
					}
					if ((bool)cell3)
					{
						cell3.EnableColumn(CFGCell.EColumn.NorthEast, bEnable);
					}
					if ((bool)cell4)
					{
						cell4.EnableColumn(CFGCell.EColumn.NorthWest, bEnable);
					}
				}
			}
		}
	}

	public static List<CFGCell> GetActivatorCells(CFGCellObject Target)
	{
		if (Target == null)
		{
			return null;
		}
		List<CFGCell> list = new List<CFGCell>();
		if (list == null)
		{
			return null;
		}
		int num = Mathf.FloorToInt(Target.transform.position.x);
		int num2 = Mathf.FloorToInt(Target.transform.position.z);
		int num3 = Mathf.FloorToInt(Target.transform.position.y / 2.5f);
		ERotation rotation = Target.Rotation;
		ECoverMirroring coverMirror = Target.m_CoverMirror;
		if (coverMirror == ECoverMirroring.FaceNorthSide)
		{
			CFGCell cell = GetCell(num2, num, num3);
			if (cell == null)
			{
				return null;
			}
			CFGCell cell2 = GetCell(num2, num - 1, num3);
			if ((bool)cell2 && !cell2.CheckFlag(1, 8) && !list.Contains(cell2))
			{
				list.Add(cell2);
			}
			cell2 = GetCell(num2 - 1, num, num3);
			if ((bool)cell2 && !cell2.CheckFlag(1, 8) && !list.Contains(cell2))
			{
				list.Add(cell2);
			}
			cell2 = GetCell(num2, num + 1, num3);
			if ((bool)cell2 && !cell2.CheckFlag(1, 8) && !list.Contains(cell2))
			{
				list.Add(cell2);
			}
			cell2 = GetCell(num2 + 1, num, num3);
			if ((bool)cell2 && !cell2.CheckFlag(1, 8) && !list.Contains(cell2))
			{
				list.Add(cell2);
			}
			return list;
		}
		for (int i = 0; i < Target.Floors; i++)
		{
			for (int j = 0; j < Target.Size_Z; j++)
			{
				for (int k = 0; k < Target.Size_X; k++)
				{
					int num4 = -1;
					int num5 = -1;
					switch (rotation)
					{
					case ERotation.None:
						num5 = num + k;
						num4 = num2 + j;
						break;
					case ERotation.CW_90:
						num5 = num + j;
						num4 = num2 - k;
						break;
					case ERotation.CW_180:
						num5 = num - k;
						num4 = num2 - j;
						break;
					case ERotation.CW_270:
						num5 = num - j;
						num4 = num2 + k;
						break;
					}
					if (num4 < 0 || num4 >= m_Width || num5 < 0 || num5 >= m_Height)
					{
						continue;
					}
					int num6 = num3 + i;
					int num7 = num6 * (m_Width * m_Height) + num5 * m_Width + num4;
					if (num7 < 0 || num7 >= m_Map.Length)
					{
						continue;
					}
					if (m_Map[num7] == null)
					{
						Debug.LogWarning("empty cell at " + num4 + " " + num5 + " f = " + num3);
						continue;
					}
					CFGCell cFGCell = m_Map[num7];
					if (cFGCell == null)
					{
						return null;
					}
					bool flag = cFGCell.CheckFlag(2, 64);
					bool flag2 = cFGCell.CheckFlag(3, 64);
					bool flag3 = cFGCell.CheckFlag(5, 64);
					bool flag4 = cFGCell.CheckFlag(4, 64);
					if (flag || (flag2 && coverMirror == ECoverMirroring.MirrorSouthToNorth))
					{
						CFGCell cell3 = GetCell(num4, num5 - 1, num6);
						if ((bool)cell3 && !cell3.CheckFlag(1, 8) && !list.Contains(cell3))
						{
							list.Add(cell3);
						}
					}
					if (flag3 || (flag4 && coverMirror == ECoverMirroring.MirrorEastToWest))
					{
						CFGCell cell4 = GetCell(num4 - 1, num5, num6);
						if ((bool)cell4 && !cell4.CheckFlag(1, 8) && !list.Contains(cell4))
						{
							list.Add(cell4);
						}
					}
					if (flag2 || (flag && coverMirror == ECoverMirroring.MirrorNorthToSouth))
					{
						CFGCell cell5 = GetCell(num4, num5 + 1, num6);
						if ((bool)cell5 && !cell5.CheckFlag(1, 8) && !list.Contains(cell5))
						{
							list.Add(cell5);
						}
					}
					if (flag4 || (flag3 && coverMirror == ECoverMirroring.MirrorWestToEast))
					{
						CFGCell cell6 = GetCell(num4 + 1, num5, num6);
						if ((bool)cell6 && !cell6.CheckFlag(1, 8) && !list.Contains(cell6))
						{
							list.Add(cell6);
						}
					}
				}
			}
		}
		return list;
	}

	public static ELOXHitType NEW_GetLineOf(Vector3 Begin, Vector3 End, byte Flag)
	{
		Vector3 PlanePt = new Vector3(0f, 2.5f, 0f);
		Vector3 PlaneNormal = Vector3.up;
		Vector3 LineStart = Begin;
		Vector3 LineEnd = End;
		Vector3 Point;
		for (int i = 1; i < 4; i++)
		{
			PlanePt.y = (float)i * 2.5f;
			if (CFGMath.LineSegmentIntersectionPlane(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, out Point) != 1)
			{
				continue;
			}
			Point.y = Point.y;
			CFGCell cell = GetCell(Point);
			if ((bool)cell && cell.HaveFloor)
			{
				m_LastIntersection = new Vector2(Point.z, Point.x);
				m_LastIntersectionY = Point.y;
				return ELOXHitType.Floor;
			}
			float num = Point.x - (float)(int)Point.x;
			float num2 = Point.z - (float)(int)Point.z;
			m_LastIntersection.x = Point.z;
			m_LastIntersection.y = Point.x;
			m_LastIntersectionY = Point.y;
			if (num < 0.05f)
			{
				cell = GetCell(Point + new Vector3(-1f, 0f, 0f));
				if ((bool)cell && cell.HaveFloor)
				{
					return ELOXHitType.Floor;
				}
			}
			if (num > 0.95f)
			{
				cell = GetCell(Point + new Vector3(1f, 0f, 0f));
				if ((bool)cell && cell.HaveFloor)
				{
					return ELOXHitType.Floor;
				}
			}
			if (num2 < 0.05f)
			{
				cell = GetCell(Point + new Vector3(0f, 0f, -1f));
				if ((bool)cell && cell.HaveFloor)
				{
					return ELOXHitType.Floor;
				}
			}
			if (num2 > 0.95f)
			{
				cell = GetCell(Point + new Vector3(0f, 0f, 1f));
				if ((bool)cell && cell.HaveFloor)
				{
					return ELOXHitType.Floor;
				}
			}
		}
		if (End.z < Begin.z)
		{
			LineStart = End;
			LineEnd = Begin;
		}
		int num3 = (int)LineStart.z;
		int num4 = (int)LineEnd.z;
		if (num3 != num4)
		{
			PlaneNormal = new Vector3(0f, 0f, -1f);
			PlanePt = Vector3.zero;
			for (int j = num3 + 1; j <= num4; j++)
			{
				PlanePt.z = j;
				if (CFGMath.LineSegmentIntersectionPlane(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, out Point) == 1)
				{
					ELOXHitType eLOXHitType = CanStandAtPoint(Point, Flag);
					if (eLOXHitType != 0)
					{
						m_LastIntersection = new Vector2(Point.z, Point.x);
						m_LastIntersectionY = Point.y;
						return eLOXHitType;
					}
				}
			}
		}
		LineStart = Begin;
		LineEnd = End;
		if (End.x < Begin.x)
		{
			LineStart = End;
			LineEnd = Begin;
		}
		num3 = (int)LineStart.x;
		num4 = (int)LineEnd.x;
		if (num3 != num4)
		{
			PlaneNormal = new Vector3(-1f, 0f, 0f);
			PlanePt = Vector3.zero;
			for (int k = num3 + 1; k <= num4; k++)
			{
				PlanePt.x = k;
				if (CFGMath.LineSegmentIntersectionPlane(ref LineStart, ref LineEnd, ref PlanePt, ref PlaneNormal, out Point) == 1)
				{
					ELOXHitType eLOXHitType2 = CanStandAtPoint(Point, Flag);
					if (eLOXHitType2 != 0)
					{
						m_LastIntersection = new Vector2(Point.z, Point.x);
						m_LastIntersectionY = Point.y;
						return eLOXHitType2;
					}
				}
			}
		}
		return ELOXHitType.None;
	}

	public static void SetWallSize(float fNewSize)
	{
		WallSize = Mathf.Clamp(fNewSize, 0.01f, 0.4f);
		Wall1Size = 1f - WallSize;
	}

	public static ELOXHitType CanStandAtPoint(Vector3 Position, byte Flag)
	{
		float num = Position.x - (float)(int)Position.x;
		float f = Position.y / 2.5f;
		float num2 = Position.z - (float)(int)Position.z;
		bool checkCover = false;
		if (m_bLOS_UseHalfCovers)
		{
			float num3 = Position.y - Mathf.Floor(f) * 2.5f;
			if (num3 < 0.25f)
			{
				checkCover = true;
			}
		}
		CFGCell othercell = null;
		CFGCell othercell2 = null;
		if (num < WallSize)
		{
			othercell2 = GetCell(Position - new Vector3(1f, 0f, 0f));
		}
		else if (num > Wall1Size)
		{
			othercell2 = GetCell(Position + new Vector3(1f, 0f, 0f));
		}
		if (num2 < WallSize)
		{
			othercell = GetCell(Position - new Vector3(0f, 0f, 1f));
		}
		else if (num2 > Wall1Size)
		{
			othercell = GetCell(Position + new Vector3(0f, 0f, 1f));
		}
		CFGCell cell = GetCell(Position);
		if (cell == null)
		{
			return ELOXHitType.TileBody;
		}
		if (cell.CheckFlag(1, Flag))
		{
			return ELOXHitType.TileBody;
		}
		if (num2 < WallSize)
		{
			if (CheckWall(cell, 5, Flag, checkCover, othercell2))
			{
				return ELOXHitType.ZAxisWall;
			}
		}
		else if (num2 > Wall1Size && CheckWall(cell, 4, Flag, checkCover, othercell2))
		{
			return ELOXHitType.ZAxisWall;
		}
		if (num < WallSize)
		{
			if (CheckWall(cell, 2, Flag, checkCover, othercell))
			{
				return ELOXHitType.XAxisWall;
			}
		}
		else if (num > Wall1Size && CheckWall(cell, 3, Flag, checkCover, othercell))
		{
			return ELOXHitType.XAxisWall;
		}
		return ELOXHitType.None;
	}

	private static bool CheckWall(CFGCell cell, byte Wall, byte Flag, bool CheckCover, CFGCell othercell)
	{
		if (cell.CheckFlag(Wall, Flag))
		{
			return true;
		}
		if (CheckCover && cell.CheckFlag(Wall, 2))
		{
			return true;
		}
		if ((bool)othercell)
		{
			if (othercell.CheckFlag(Wall, Flag))
			{
				return true;
			}
			if (CheckCover && othercell.CheckFlag(Wall, 2))
			{
				return true;
			}
		}
		return false;
	}
}
