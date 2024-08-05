using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGRangeBorders : MonoBehaviour
{
	private class TileInfo
	{
		public CFGCell tile;

		public CFGCell[] neighbours = new CFGCell[8];

		public int neighboursAmount;

		public int[] dirs = new int[8];

		public bool final;

		public TileInfo(CFGCell tile)
		{
			this.tile = tile;
			neighboursAmount = 0;
			for (int i = 0; i < 8; i++)
			{
				dirs[i] = 1;
			}
		}

		public void CheckFinal()
		{
			if (final)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (dirs[i] == 0)
				{
					num++;
				}
				else if (dirs[i] == 2)
				{
					num2++;
				}
			}
			neighboursAmount = 8 - num;
			if (num == 6)
			{
				for (int j = 0; j < 8; j++)
				{
					if (dirs[j] != 0)
					{
						dirs[j] = 2;
					}
				}
				final = true;
			}
			else
			{
				if (num2 != 2)
				{
					return;
				}
				for (int k = 0; k < 8; k++)
				{
					if (dirs[k] != 2)
					{
						dirs[k] = 0;
					}
				}
				final = true;
			}
		}

		public Vector3[] ReturnBorderPoints(HashSet<CFGCell> b_tiles)
		{
			Vector3[] result = new Vector3[0];
			float num = 0.01f;
			if (neighboursAmount >= 2)
			{
				result = new Vector3[1] { tile.WorldPosition };
			}
			else if (neighboursAmount == 1)
			{
				if (b_tiles == null || b_tiles.Count == 0)
				{
					return null;
				}
				CFGCell cFGCell = neighbours.FirstOrDefault((CFGCell m) => m != null && b_tiles.Contains(m));
				if (cFGCell == null)
				{
					return null;
				}
				Vector3 vector = tile.WorldPosition - cFGCell.WorldPosition;
				Vector3 vector2 = Quaternion.Euler(0f, -90f, 0f) * vector * num;
				result = new Vector3[2]
				{
					tile.WorldPosition + vector2,
					tile.WorldPosition - vector2
				};
			}
			else if (neighboursAmount == 0)
			{
				num *= 10f;
				result = new Vector3[4]
				{
					tile.WorldPosition + new Vector3(num, 0f, num),
					tile.WorldPosition + new Vector3(num, 0f, 0f - num),
					tile.WorldPosition + new Vector3(0f - num, 0f, 0f - num),
					tile.WorldPosition + new Vector3(0f - num, 0f, num)
				};
			}
			return result;
		}
	}

	private class RangeBorder
	{
		private int m_Floor;

		private int m_Range;

		private List<Mesh> m_MeshContainer;

		private List<Material> m_MatContainer;

		public int Floor => m_Floor;

		public int Range => m_Range;

		public RangeBorder(int _floor, int _range)
		{
			m_Floor = (int)Mathf.Clamp(_floor, 0f, 2f);
			m_Range = (int)Mathf.Clamp(_range, 0f, 1f);
			m_MeshContainer = new List<Mesh>();
			m_MatContainer = new List<Material>();
		}

		private bool SegmentExist(Vector3 p1, Vector3 p2, List<Vector3> points)
		{
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 vector = points[i];
				Vector3 vector2 = points[(i + 1) % points.Count];
				if (p1 == vector && p2 == vector2)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearMeshes()
		{
			foreach (Mesh item in m_MeshContainer)
			{
				item.Clear(keepVertexLayout: false);
			}
			m_MeshContainer.Clear();
			m_MatContainer.Clear();
		}

		public void GenerateMesh(List<Vector3> points, List<Vector3> exlude_points, float _thickness)
		{
			Mesh mesh = new Mesh();
			float num = 0f;
			if (points.Count < 3)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector2> list2 = new List<Vector2>();
			List<int> list3 = new List<int>();
			int num2 = 0;
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 vector = points[i];
				Vector3 vector2 = points[(i + 1) % points.Count];
				if (exlude_points != null && SegmentExist(vector, vector2, exlude_points))
				{
					continue;
				}
				Vector3 vector3 = ((i != 0) ? points[i - 1] : points[points.Count - 1]);
				Vector3 vector4 = points[(i + 2) % points.Count];
				Vector3 vector5 = Vector3.Normalize(vector3 - vector);
				Vector3 vector6 = Vector3.Normalize(vector2 - vector);
				Vector3 vector7 = Vector3.Normalize(Vector3.Cross(vector6, Vector3.up));
				float num3 = Vector3.Dot(vector5, vector6);
				Vector3 vector8;
				if (num3 > -0.999f && num3 < 0.999f)
				{
					vector8 = Vector3.Normalize(vector5 + vector6);
					if (Vector3.Cross(vector5, vector6).y < 0f)
					{
						vector8 = -vector8;
					}
					vector8 *= _thickness / Vector3.Dot(vector7, vector8);
				}
				else
				{
					vector8 = vector7 * _thickness;
				}
				Vector3 vector9 = Vector3.Normalize(vector - vector2);
				Vector3 vector10 = Vector3.Normalize(vector4 - vector2);
				float num4 = Vector3.Dot(vector9, vector10);
				Vector3 vector11;
				if (num4 > -0.999f && num4 < 0.999f)
				{
					vector11 = Vector3.Normalize(vector9 + vector10);
					if (Vector3.Cross(vector9, vector10).y < 0f)
					{
						vector11 = -vector11;
					}
					vector11 *= _thickness / Vector3.Dot(vector7, vector11);
				}
				else
				{
					vector11 = vector7 * _thickness;
				}
				list.Add(vector + vector8);
				list.Add(vector2 + vector11);
				list.Add(vector2);
				list.Add(vector);
				float num5 = num + (vector2 - vector).magnitude;
				Vector2 item = new Vector2(num, 1f);
				Vector2 item2 = new Vector2(num5, 1f);
				Vector2 item3 = new Vector2(num5, 0f);
				Vector2 item4 = new Vector2(num, 0f);
				list2.Add(item);
				list2.Add(item2);
				list2.Add(item3);
				list2.Add(item4);
				num = num5;
				list3.Add(num2);
				list3.Add(num2 + 1);
				list3.Add(num2 + 2);
				list3.Add(num2);
				list3.Add(num2 + 2);
				list3.Add(num2 + 3);
				num2 += 4;
			}
			mesh.vertices = list.ToArray();
			mesh.uv = list2.ToArray();
			mesh.triangles = list3.ToArray();
			m_MeshContainer.Add(mesh);
			UpdateMaterials(num);
		}

		private void UpdateMaterials(float _length)
		{
			Material material = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_Range1Material);
			Material material2 = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_Range2Material);
			material.SetFloat("_CurveLength", _length);
			material2.SetFloat("_CurveLength", _length);
			m_MatContainer.Add(material);
			m_MatContainer.Add(material2);
		}

		public void Draw(int _rangeId)
		{
			_rangeId = Mathf.Clamp(_rangeId, 0, 1);
			for (int i = 0; i < m_MeshContainer.Count; i++)
			{
				Graphics.DrawMesh(m_MeshContainer[i], new Vector3(0f, 0.05f, 0f), Quaternion.identity, m_MatContainer[2 * i + _rangeId], 1);
			}
		}
	}

	public static bool s_DrawRangeBorders = true;

	[SerializeField]
	private float m_Thickness = 0.2f;

	private HashSet<CFGCell> m_Range1_tiles;

	private HashSet<CFGCell> m_Range2_tiles;

	public static bool s_ShowAp2Region;

	private RangeBorder[] m_RangeBordersA;

	public void RegenerateMeshes(bool checkTiles)
	{
		if (m_RangeBordersA == null)
		{
			m_RangeBordersA = new RangeBorder[6]
			{
				new RangeBorder(0, 0),
				new RangeBorder(0, 1),
				new RangeBorder(1, 0),
				new RangeBorder(1, 1),
				new RangeBorder(2, 0),
				new RangeBorder(2, 1)
			};
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if (component == null || component2 == null || component2.SelectedCharacter == null || component2.SelectedCharacter.ActionPoints == 0 || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			return;
		}
		CFGCell characterCell = CFGCellMap.GetCharacterCell(component2.SelectedCharacter.Transform.position);
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(characterCell, component2.SelectedCharacter.HalfMovement);
		if (checkTiles && component2.SelectedCharacter.ActionPoints == 1 && m_Range1_tiles != null && m_Range1_tiles.SetEquals(hashSet))
		{
			return;
		}
		m_Range1_tiles = hashSet;
		if (component2.SelectedCharacter.ActionPoints == 2)
		{
			HashSet<CFGCell> hashSet2 = CFGCellDistanceFinder.FindCellsInDistance(characterCell, component2.SelectedCharacter.DashingMovement);
			if (checkTiles && m_Range2_tiles != null && m_Range2_tiles.SetEquals(hashSet2))
			{
				return;
			}
			m_Range2_tiles = hashSet2;
		}
		List<Vector3>[] array = new List<Vector3>[3];
		foreach (RangeBorder item in m_RangeBordersA.Where((RangeBorder m) => m.Range == 0))
		{
			HashSet<CFGCell> tiles = new HashSet<CFGCell>(m_Range1_tiles);
			item.ClearMeshes();
			do
			{
				array[item.Floor] = PreprocessTiles(ref tiles, (EFloorLevelType)item.Floor, characterCell);
				item.GenerateMesh(array[item.Floor], null, m_Thickness);
			}
			while (tiles.Count > 0 && array[item.Floor].Count > 0);
		}
		if (component2.SelectedCharacter.ActionPoints != 2)
		{
			return;
		}
		foreach (RangeBorder item2 in m_RangeBordersA.Where((RangeBorder m) => m.Range == 1))
		{
			HashSet<CFGCell> tiles2 = new HashSet<CFGCell>(m_Range2_tiles);
			item2.ClearMeshes();
			List<Vector3> list = new List<Vector3>();
			do
			{
				list = PreprocessTiles(ref tiles2, (EFloorLevelType)item2.Floor, characterCell);
				item2.GenerateMesh(list, array[item2.Floor], m_Thickness);
			}
			while (tiles2.Count > 0 && list.Count > 0);
		}
	}

	private void Awake()
	{
		m_RangeBordersA = new RangeBorder[6]
		{
			new RangeBorder(0, 0),
			new RangeBorder(0, 1),
			new RangeBorder(1, 0),
			new RangeBorder(1, 1),
			new RangeBorder(2, 0),
			new RangeBorder(2, 1)
		};
	}

	private void Update()
	{
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			return;
		}
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!s_DrawRangeBorders || component == null || component.CurrentAction != ETurnAction.None || component.TargetedCharacter != null || component.SelectedCharacter == null || (component.SelectedCharacter.CanMakeAction(ETurnAction.Move) != 0 && (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && component.SelectedCharacter.CurrentAction != ETurnAction.ChangeWeapon))))
		{
			return;
		}
		CFGCamera camera = Camera.main.GetComponent<CFGCamera>();
		if (camera == null)
		{
			return;
		}
		if (component.SelectedCharacter.ActionPoints == 2)
		{
			foreach (RangeBorder item in m_RangeBordersA.Where((RangeBorder m) => m.Range == 0 && m.Floor <= (int)camera.CurrentFloorLevel))
			{
				item.Draw(0);
			}
			if (!s_ShowAp2Region)
			{
				return;
			}
			{
				foreach (RangeBorder item2 in m_RangeBordersA.Where((RangeBorder m) => m.Range == 1 && m.Floor <= (int)camera.CurrentFloorLevel))
				{
					item2.Draw(1);
				}
				return;
			}
		}
		foreach (RangeBorder item3 in m_RangeBordersA.Where((RangeBorder m) => m.Range == 0 && m.Floor <= (int)camera.CurrentFloorLevel))
		{
			item3.Draw(1);
		}
	}

	private List<Vector3> PreprocessTiles(ref HashSet<CFGCell> tiles, EFloorLevelType floor_level, CFGCell char_tile)
	{
		Dictionary<CFGCell, TileInfo> dictionary = new Dictionary<CFGCell, TileInfo>();
		HashSet<CFGCell> hashSet = new HashSet<CFGCell>();
		List<Vector3> list = new List<Vector3>();
		list.Clear();
		tiles.RemoveWhere((CFGCell m) => m.StairsType == CFGCell.EStairsType.Slope);
		foreach (CFGCell tile in tiles)
		{
			if (tile.Floor == (int)floor_level)
			{
				CFGCell cell = CFGCellMap.GetCell(tile.PositionX + 1, tile.PositionZ, tile.Floor);
				CFGCell cell2 = CFGCellMap.GetCell(tile.PositionX + 1, tile.PositionZ + 1, tile.Floor);
				CFGCell cell3 = CFGCellMap.GetCell(tile.PositionX, tile.PositionZ + 1, tile.Floor);
				CFGCell cell4 = CFGCellMap.GetCell(tile.PositionX - 1, tile.PositionZ + 1, tile.Floor);
				CFGCell cell5 = CFGCellMap.GetCell(tile.PositionX - 1, tile.PositionZ, tile.Floor);
				CFGCell cell6 = CFGCellMap.GetCell(tile.PositionX - 1, tile.PositionZ - 1, tile.Floor);
				CFGCell cell7 = CFGCellMap.GetCell(tile.PositionX, tile.PositionZ - 1, tile.Floor);
				CFGCell cell8 = CFGCellMap.GetCell(tile.PositionX + 1, tile.PositionZ - 1, tile.Floor);
				bool flag = cell != null && tiles.Contains(cell);
				bool flag2 = cell3 != null && tiles.Contains(cell3);
				bool flag3 = cell5 != null && tiles.Contains(cell5);
				bool flag4 = cell7 != null && tiles.Contains(cell7);
				if (!flag || !flag2 || !flag3 || !flag4)
				{
					hashSet.Add(tile);
					TileInfo tileInfo = new TileInfo(tile);
					tileInfo.neighbours[0] = cell;
					tileInfo.neighbours[1] = cell2;
					tileInfo.neighbours[2] = cell3;
					tileInfo.neighbours[3] = cell4;
					tileInfo.neighbours[4] = cell5;
					tileInfo.neighbours[5] = cell6;
					tileInfo.neighbours[6] = cell7;
					tileInfo.neighbours[7] = cell8;
					dictionary.Add(tile, tileInfo);
				}
			}
		}
		if (dictionary.Count == 0)
		{
			return list;
		}
		HashSet<CFGCell> hashSet2 = new HashSet<CFGCell>();
		foreach (TileInfo value2 in dictionary.Values)
		{
			for (int i = 0; i < 8; i++)
			{
				CFGCell cFGCell = value2.neighbours[i];
				if (cFGCell == null || !hashSet.Contains(cFGCell))
				{
					value2.dirs[i] = 0;
				}
			}
			value2.CheckFinal();
		}
		Vector3 char_pos = char_tile.WorldPosition;
		float max_dist = 0f;
		TileInfo tileInfo2 = null;
		TileInfo[] array = dictionary.Values.Where((TileInfo m) => m.final).ToArray();
		if (array != null && array.Length > 0)
		{
			max_dist = array.Max((TileInfo m) => Vector3.Distance(char_pos, m.tile.WorldPosition));
			tileInfo2 = array.FirstOrDefault((TileInfo m) => Vector3.Distance(char_pos, m.tile.WorldPosition) == max_dist);
		}
		else
		{
			array = dictionary.Values.Where((TileInfo m) => m.neighboursAmount == 0 || m.neighboursAmount == 1 || m.neighboursAmount == 3).ToArray();
			if (array == null || array.Length == 0)
			{
				return list;
			}
			max_dist = array.Max((TileInfo m) => Vector3.Distance(char_pos, m.tile.WorldPosition));
			tileInfo2 = dictionary.Values.FirstOrDefault((TileInfo m) => Vector3.Distance(char_pos, m.tile.WorldPosition) == max_dist);
			for (int j = 0; j < 8; j++)
			{
				if (tileInfo2.dirs[j] != 0)
				{
					tileInfo2.dirs[j] = 2;
				}
			}
		}
		if (tileInfo2 == null)
		{
			return list;
		}
		Vector3[] array2 = tileInfo2.ReturnBorderPoints(hashSet);
		foreach (Vector3 item in array2)
		{
			list.Add(item);
		}
		hashSet2.Add(tileInfo2.tile);
		if (tileInfo2.neighboursAmount == 0)
		{
			if (hashSet.Count > hashSet2.Count)
			{
				tiles = RemoveSingleBorder(hashSet, hashSet2);
			}
			else
			{
				tiles.Clear();
			}
			return list;
		}
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		for (int l = 0; l < 8; l++)
		{
			if (tileInfo2.dirs[l] == 2)
			{
				if (num == -1)
				{
					num = l;
				}
				else if (num2 == -1)
				{
					num2 = l;
				}
				else if (num3 == -1)
				{
					num3 = l;
				}
			}
		}
		if (num2 == -1 && tileInfo2.neighboursAmount == 1)
		{
			num2 = num;
		}
		if (tileInfo2.neighboursAmount == 3)
		{
			int[] source = new int[3] { num, num2, num3 };
			source = source.Where((int m) => m % 2 == 0).ToArray();
			if (source != null && source.Length == 2)
			{
				num = source[0];
				num2 = source[1];
			}
			else
			{
				Debug.LogWarning("something wrong with dirs");
			}
		}
		TileInfo tileInfo3 = dictionary[tileInfo2.neighbours[num]];
		TileInfo tileInfo4 = dictionary[tileInfo2.neighbours[num2]];
		Vector3 normalized = (tileInfo2.tile.WorldPosition - char_pos).normalized;
		Vector3 normalized2 = (tileInfo3.tile.WorldPosition - tileInfo2.tile.WorldPosition).normalized;
		Vector3 normalized3 = (tileInfo4.tile.WorldPosition - tileInfo2.tile.WorldPosition).normalized;
		Vector3 vector = Vector3.Cross(normalized, normalized2);
		Vector3 vector2 = Vector3.Cross(normalized, normalized3);
		int num4 = -1;
		int num5 = -1;
		TileInfo value = null;
		if (vector.y * vector2.y < 0f || (vector.y == 0f && vector2.y == 0f))
		{
			if (vector.y > vector2.y)
			{
				value = tileInfo3;
				num4 = num;
				num5 = (num + 4) % 8;
			}
			else
			{
				value = tileInfo4;
				num4 = num2;
				num5 = (num2 + 4) % 8;
			}
		}
		else
		{
			float magnitude = (tileInfo3.tile.WorldPosition - char_pos).magnitude;
			float magnitude2 = (tileInfo4.tile.WorldPosition - char_pos).magnitude;
			if ((vector.y > 0f && vector2.y > 0f) || vector.y + vector2.y > 0f)
			{
				if (magnitude > magnitude2)
				{
					value = tileInfo3;
					num4 = num;
					num5 = (num + 4) % 8;
				}
				else
				{
					value = tileInfo4;
					num4 = num2;
					num5 = (num2 + 4) % 8;
				}
			}
			else if ((vector.y < 0f && vector2.y < 0f) || vector.y + vector2.y < 0f)
			{
				if (magnitude < magnitude2)
				{
					value = tileInfo3;
					num4 = num;
					num5 = (num + 4) % 8;
				}
				else
				{
					value = tileInfo4;
					num4 = num2;
					num5 = (num2 + 4) % 8;
				}
			}
		}
		value.dirs[num5] = 2;
		Vector3[] array3 = value.ReturnBorderPoints(hashSet);
		foreach (Vector3 item2 in array3)
		{
			list.Add(item2);
		}
		hashSet2.Add(value.tile);
		int num6 = 0;
		int num7 = 0;
		while ((value != tileInfo2 || num5 == num4) && num6 < 1000 && num7 < 2)
		{
			num6++;
			int num8 = -1;
			for (int num9 = 0; num9 < 8; num9++)
			{
				num8 = (num5 + 1 + num9) % 8;
				if (value.dirs[num8] != 0)
				{
					break;
				}
			}
			value.dirs[num8] = 2;
			if (num8 < 0 || num8 >= value.neighbours.Length)
			{
				Debug.LogWarning("NEXT DIR: " + num8);
			}
			if (!dictionary.TryGetValue(value.neighbours[num8], out value))
			{
				Debug.LogError("ERROR! CFGRangeBorders.PreprocessTiles() - something is wrong");
				break;
			}
			num5 = (num8 + 4) % 8;
			value.dirs[num5] = 2;
			if (value != tileInfo2 || num5 == num4)
			{
				Vector3[] array4 = value.ReturnBorderPoints(hashSet);
				foreach (Vector3 item3 in array4)
				{
					list.Add(item3);
				}
				hashSet2.Add(value.tile);
			}
			if (value == tileInfo2)
			{
				num7++;
			}
		}
		if (hashSet.Count > hashSet2.Count)
		{
			tiles = RemoveSingleBorder(hashSet, hashSet2);
		}
		else
		{
			tiles.Clear();
		}
		return list;
	}

	private HashSet<CFGCell> RemoveSingleBorder(HashSet<CFGCell> allTiles, HashSet<CFGCell> borderTiles)
	{
		HashSet<CFGCell> hashSet = new HashSet<CFGCell>();
		CFGCell tile;
		foreach (CFGCell allTile in allTiles)
		{
			tile = allTile;
			if (!borderTiles.Contains(tile))
			{
				bool flag = borderTiles.Any((CFGCell m) => m.PositionX > tile.PositionX);
				bool flag2 = borderTiles.Any((CFGCell m) => m.PositionX < tile.PositionX);
				bool flag3 = borderTiles.Any((CFGCell m) => m.PositionZ > tile.PositionZ);
				bool flag4 = borderTiles.Any((CFGCell m) => m.PositionZ < tile.PositionZ);
				if (!flag || !flag2 || !flag3 || !flag4)
				{
					hashSet.Add(tile);
				}
			}
		}
		return hashSet;
	}

	public void ManageRangeVis(CFGCell cellUnderCursor)
	{
		if (m_Range1_tiles == null || cellUnderCursor == null)
		{
			s_ShowAp2Region = false;
		}
		else
		{
			s_ShowAp2Region = !m_Range1_tiles.Contains(cellUnderCursor);
		}
	}
}
