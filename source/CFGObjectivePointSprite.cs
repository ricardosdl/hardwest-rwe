using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFGObjectivePointSprite : CFGPointSprite
{
	private void Start()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (!(component == null))
		{
			int currentFloorLevel = (int)component.CurrentFloorLevel;
			UpdateMaterial(currentFloorLevel);
		}
	}

	private void UpdateMaterial(int _floorLevel)
	{
		if (m_Material == null)
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			m_Material.SetInt("_FloorDiff", 0);
			return;
		}
		CFGCell characterCell = CFGCellMap.GetCharacterCell(base.transform.position);
		if (characterCell != null)
		{
			int value = characterCell.Floor - _floorLevel;
			int value2 = Math.Sign(value);
			m_Material.SetInt("_FloorDiff", value2);
		}
	}

	public static void OnCurrentFloorLevelChanged()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component == null)
		{
			return;
		}
		int currentFloorLevel = (int)component.CurrentFloorLevel;
		foreach (KeyValuePair<int, HashSet<CFGPointSprite>> pointSprite in CFGPointSprite.m_PointSprites)
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
			foreach (CFGPointSprite item in pointSprite.Value)
			{
				if (item != null && item.enabled && (bool)item.gameObject && item is CFGObjectivePointSprite)
				{
					(item as CFGObjectivePointSprite).UpdateMaterial(currentFloorLevel);
				}
			}
		}
	}
}
