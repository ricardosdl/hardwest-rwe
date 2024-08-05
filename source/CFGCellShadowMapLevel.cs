using System;
using UnityEngine;

public static class CFGCellShadowMapLevel
{
	public delegate void OnRecalcuate(CFGCellShadowMap Map);

	private static CFGCellShadowMap m_MainMap = new CFGCellShadowMap();

	private static eCellShadowMapType m_MapType = eCellShadowMapType.NoShadowMap;

	public static CFGCellShadowMapChar m_CharacterMap = new CFGCellShadowMapChar();

	private static OnRecalcuate m_OnRecalcFunc = null;

	public static eCellShadowMapType MapType => m_MapType;

	public static CFGCellShadowMap MainMap => m_MainMap;

	public static void RegisterOnRecalc(OnRecalcuate Func)
	{
		m_OnRecalcFunc = (OnRecalcuate)Delegate.Combine(m_OnRecalcFunc, Func);
	}

	public static void UnRegisterOnRecalc(OnRecalcuate Func)
	{
		m_OnRecalcFunc = null;
	}

	public static bool RecalculateArea(int MinZ, int MaxZ, int MinX, int MaxX, int MinFloor, int MaxFloor)
	{
		if (m_MainMap == null || !m_MainMap.CheckIfValid())
		{
			Debug.LogWarning("Cannot recal shadow map - not initialized");
			return false;
		}
		bool result = m_MainMap.Recalculate(MinZ, MaxZ, MinX, MaxX, MinFloor, MaxFloor, bClear: false);
		if (m_OnRecalcFunc != null)
		{
			m_OnRecalcFunc(m_MainMap);
		}
		return result;
	}

	public static bool CreateMainMap(eCellShadowMapType _NewType, Vector3 _SunDir, bool bDarkness)
	{
		m_MapType = eCellShadowMapType.NoShadowMap;
		Vector3 newDir = _SunDir;
		if (newDir.magnitude < 0.5f)
		{
			newDir.Set(1f, 1f, 0f);
			newDir.Normalize();
		}
		if (!CFGCellMap.IsValid)
		{
			Debug.LogError("Cell Map is not prepared!");
			return false;
		}
		if (m_MainMap == null)
		{
			m_MainMap = new CFGCellShadowMap();
			if (m_MainMap == null)
			{
				Debug.LogError("Failed to allocate main shadow map object");
				return false;
			}
		}
		if (m_CharacterMap == null)
		{
			m_CharacterMap = new CFGCellShadowMapChar();
			if (m_CharacterMap == null)
			{
				Debug.LogError("Failed to allocate character shadow map!");
				return false;
			}
		}
		if (!m_MainMap.CreateMap(_NewType, CFGCellMap.ZAxisSize, CFGCellMap.XAxisSize, CFGCellMap.MaxFloor))
		{
			Debug.LogError("Failed to create map");
			return false;
		}
		if (!m_MainMap.SetSunDir(newDir, bForceRecalc: true, bDarkness))
		{
			Debug.LogError("Failed to set sun dir");
			return false;
		}
		RecalculateCharMap();
		m_MapType = _NewType;
		return true;
	}

	private static void RecalculateCharMap()
	{
		if (m_CharacterMap != null)
		{
			m_CharacterMap.Recalculate(m_MainMap.SunDirection, m_MainMap.MapType.GetMultiplier());
		}
	}

	public static bool CreateMainMapFromLS()
	{
		CFGLevelSettings[] array = UnityEngine.Object.FindObjectsOfType<CFGLevelSettings>();
		if (array == null || array.Length == 0 || array[0] == null)
		{
			Debug.LogError("Failed to find level settings object. Cannot recalculate shadow map");
			return false;
		}
		CFGGame.EnableUpdate(Vampire: true, CharShadows: true);
		return CreateMainMap(array[0].m_CellShadowMapType, array[0].GetSunDirection(), array[0].m_Darkness);
	}

	public static float IsInShadow(Vector3 Pos)
	{
		if (m_MapType == eCellShadowMapType.NoShadowMap || m_CharacterMap == null)
		{
			return 0f;
		}
		int num = Mathf.FloorToInt(Pos.y / 2.5f);
		int num2 = 0;
		int deltaZ = Mathf.FloorToInt(Pos.z);
		int deltaX = Mathf.FloorToInt(Pos.x);
		int num3 = 0;
		for (int i = 0; i < m_CharacterMap.m_Cells.Count; i++)
		{
			int num4 = num + m_CharacterMap.m_Cells[i].DF;
			if (num4 >= 0)
			{
				int visiblePoints = m_MainMap.GetVisiblePoints(m_CharacterMap.m_Cells[i], deltaZ, deltaX, num);
				num2 += visiblePoints;
				num3 += m_CharacterMap.m_Cells[i].Count;
			}
		}
		return (float)num2 / (float)num3;
	}

	public static bool CanTargetBeShadowSpotted(CFGCharacter SourceChar, CFGCharacter Target)
	{
		if (SourceChar == null || Target == null || m_MapType == eCellShadowMapType.NoShadowMap || m_CharacterMap == null)
		{
			return false;
		}
		Vector3 position = Target.Transform.position;
		int num = Mathf.FloorToInt(position.y / 2.5f);
		int num2 = Mathf.FloorToInt(position.z);
		int num3 = Mathf.FloorToInt(position.x);
		float num4 = m_MapType.GetMultiplier();
		num4 *= num4;
		num4 *= Mathf.Clamp01(CFGLevelSettings.CellInShadowRatio);
		int num5 = (int)num4;
		for (int i = 0; i < m_CharacterMap.m_Cells.Count; i++)
		{
			int num6 = num + m_CharacterMap.m_Cells[i].DF;
			if (num6 < 0)
			{
				continue;
			}
			int visiblePoints = m_MainMap.GetVisiblePoints(m_CharacterMap.m_Cells[i], num2, num3, num);
			if (visiblePoints >= num5)
			{
				CFGCell cell = CFGCellMap.GetCell(num2 + m_CharacterMap.m_Cells[i].DZ, num3 + m_CharacterMap.m_Cells[i].DX, num6);
				if (CFGCellMap.GetLineOf(SourceChar.CurrentCell, cell, 10000, 16, bUseStartSideSteps: true, bUseEndSideSteps: false) == ELOXHitType.None)
				{
					return true;
				}
			}
		}
		return false;
	}
}
