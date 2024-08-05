using System.Collections.Generic;
using UnityEngine;

public class CFGRandomSpawnArea : CFGSerializableObject
{
	public override ESerializableType SerializableType => ESerializableType.SpawnArea;

	public override bool NeedsSaving => false;

	private List<CFGCell> GenerateCellList()
	{
		Collider component = base.transform.GetComponent<Collider>();
		List<CFGCell> list = new List<CFGCell>();
		Vector3 point = default(Vector3);
		Vector3 min = component.bounds.min;
		Vector3 max = component.bounds.max;
		int num = Mathf.Max(0, (int)min.x - 1);
		int num2 = Mathf.Max(0, (int)(min.y / 2.5f) - 1);
		int num3 = Mathf.Max(0, (int)min.z - 1);
		int num4 = Mathf.Min(CFGCellMap.XAxisSize, (int)max.x + 1);
		int num5 = Mathf.Min(CFGCellMap.MaxFloor, (int)(max.y / 2.5f) + 1);
		int num6 = Mathf.Min(CFGCellMap.ZAxisSize, (int)max.z + 1);
		for (int i = num2; i < num5; i++)
		{
			point.y = (float)i * 2.5f + 1.25f;
			for (int j = num; j < num4; j++)
			{
				point.x = 0.5f + (float)j;
				for (int k = num3; k < num6; k++)
				{
					point.z = 0.5f + (float)k;
					if (component.bounds.Contains(point))
					{
						CFGCell cell = CFGCellMap.GetCell(point);
						if (cell != null && cell.CanStandOnThisTile(can_stand_now: true))
						{
							list.Add(cell);
						}
					}
				}
			}
		}
		return list;
	}

	public List<CFGCharacter> SpawnCharacters(List<string> CharacterList, CFGOwner Owner)
	{
		List<CFGCharacter> list = new List<CFGCharacter>();
		if (CharacterList == null || CharacterList.Count == 0 || Owner == null)
		{
			return list;
		}
		List<CFGCell> list2 = GenerateCellList();
		if (list2 == null || list2.Count == 0)
		{
			Debug.LogError("No space for characters!");
			return list;
		}
		for (int i = 0; i < CharacterList.Count; i++)
		{
			int index = Random.Range(0, list2.Count);
			CFGCharacter cFGCharacter = CFGCharacterList.SpawnCharacter(CharacterList[i], Owner, list2[index].WorldPosition, Quaternion.identity, bSingleInstance: false);
			if (cFGCharacter != null)
			{
				list.Add(cFGCharacter);
			}
			list2.RemoveAt(index);
			if (list2.Count == 0)
			{
				if (i < CharacterList.Count - 1)
				{
					Debug.LogError("Failed to spawn all characters!");
				}
				return list;
			}
		}
		return list;
	}
}
