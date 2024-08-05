using System.Collections.Generic;
using UnityEngine;

public class CFGDoorObject : CFGGameObject
{
	public delegate void OnDoorDelegate(CFGDoorObject door_obj, CFGCharacter Char);

	[SerializeField]
	private bool m_Locked;

	[SerializeField]
	private bool m_IsSwinging;

	private bool m_Opened;

	[SerializeField]
	private bool m_ApplyNewCover;

	[SerializeField]
	private bool m_BlockClosedForPathfinding;

	private float m_LastOpenTime;

	public CFGSoundDef m_OnOpenSoundDef;

	[CFGFlowCode(Category = "Objects", Title = "On Door Failure To Open")]
	public OnDoorDelegate m_OnDoorFailureToOpen;

	[CFGFlowCode(Category = "Objects", Title = "On Door Opened")]
	public OnDoorDelegate m_OnDoorOpened;

	private int m_AnimID = -1;

	[CFGFlowCode]
	public bool BlockForPathfinding
	{
		get
		{
			if (m_Opened)
			{
				return false;
			}
			return m_BlockClosedForPathfinding;
		}
		set
		{
			m_BlockClosedForPathfinding = value;
		}
	}

	public bool IsOpened => m_Opened;

	public bool IsLocked => m_Locked;

	public bool CanOpen
	{
		get
		{
			if (m_Opened || m_IsSwinging)
			{
				return false;
			}
			return !m_Locked;
		}
	}

	public bool CanPlayAnim
	{
		get
		{
			if (!m_Opened)
			{
				return true;
			}
			if (!m_IsSwinging)
			{
				return false;
			}
			if (Time.time > m_LastOpenTime + 1f)
			{
				return true;
			}
			return false;
		}
	}

	public bool CanClose
	{
		get
		{
			if (!m_Opened || m_IsSwinging)
			{
				return false;
			}
			return true;
		}
	}

	public bool CanLock
	{
		get
		{
			if (m_Opened || m_IsSwinging)
			{
				return false;
			}
			return !m_Locked;
		}
	}

	public bool CanBeUsed
	{
		get
		{
			if (m_Opened)
			{
				return CanClose;
			}
			return CanOpen;
		}
	}

	public override ESerializableType SerializableType => ESerializableType.Door;

	public override bool NeedsSaving => true;

	public void OnCharacterMoveThrough(CFGCharacter User)
	{
		if (CanOpen)
		{
			Open(User);
		}
		else if (m_IsSwinging && CanPlayAnim)
		{
			OpenSwing(User);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (CFGOptions.Gameplay.InteractiveObjectsGlow >= 3)
		{
			base.gameObject.AddComponent<CFGInteractiveObjVis>();
		}
	}

	private void DoOpen(CFGCell StartPos)
	{
		PlayAnimation(StartPos);
		m_LastOpenTime = Time.time;
		ApplyNewCover();
		m_Opened = true;
	}

	private void ApplyNewCover()
	{
		if (m_ApplyNewCover)
		{
			if ((bool)GetComponent<Collider>())
			{
				GetComponent<Collider>().enabled = false;
			}
			CFGCellObject component = base.transform.parent.GetComponent<CFGCellObject>();
			if (component == null)
			{
				component = base.transform.GetComponent<CFGCellObject>();
			}
			if ((bool)component)
			{
				CFGCellMap.ApplySingleObject(component, bAlternate: true);
			}
			else
			{
				Debug.LogError("Failed to find parent object");
			}
			CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
			if (component2 != null)
			{
				component2.OnGridChanged();
			}
		}
	}

	public void Use(CFGCell StartPos)
	{
		if (!m_Opened)
		{
			Open(StartPos);
		}
	}

	public void Close(CFGCell StartPos)
	{
	}

	public void Open(CFGCell StartPos)
	{
		if (CanOpen)
		{
			DoOpen(StartPos);
		}
	}

	public void Open(CFGCharacter User)
	{
		if (!CanOpen)
		{
			if (m_OnDoorFailureToOpen != null)
			{
				m_OnDoorFailureToOpen(this, User);
			}
			return;
		}
		if (m_OnDoorOpened != null)
		{
			m_OnDoorOpened(this, User);
		}
		if (User == null)
		{
			DoOpen(null);
		}
		else
		{
			DoOpen(User.CurrentCell);
		}
	}

	private void PlayAnimation(CFGCell UserCell)
	{
		m_AnimID = GetAnimationID(UserCell);
		PlayAnim(bInstant: false);
		CFGSoundDef.Play(m_OnOpenSoundDef, base.Transform.position);
	}

	private void PlayAnim(bool bInstant)
	{
		if (m_AnimID >= 0 && m_AnimID <= 1)
		{
			PlayAnim(base.transform, bInstant);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				PlayAnim(child, bInstant);
			}
		}
	}

	private void PlayAnim(Transform t, bool bInstant)
	{
		if (t == null || !t.GetComponent<Animation>())
		{
			return;
		}
		switch (m_AnimID)
		{
		case 1:
			t.GetComponent<Animation>().clip = t.GetComponent<Animation>().GetClip("Open_180");
			break;
		case 0:
			t.GetComponent<Animation>().clip = t.GetComponent<Animation>().GetClip("Open");
			break;
		}
		t.GetComponent<Animation>().Play();
		if (!bInstant)
		{
			return;
		}
		foreach (AnimationState item in t.GetComponent<Animation>())
		{
			item.time = item.length;
		}
	}

	public void OpenSwing(CFGCharacter User)
	{
		if (m_IsSwinging)
		{
			Debug.Log("Door: Open: Doing door-swing thing on" + base.name);
			PlayAnimation(User.CurrentCell);
		}
	}

	public void Close(CFGCharacter User)
	{
		if (CanClose)
		{
			m_Opened = false;
			if (m_ApplyNewCover)
			{
				CFGCellMap.ApplySingleObject(GetComponent<CFGCellObject>());
			}
		}
	}

	public void SetLocked(bool NewState, bool bOverwrite = false)
	{
		if (m_Opened || (NewState == m_Locked && !bOverwrite))
		{
			return;
		}
		m_Locked = NewState;
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null)
		{
			if ((bool)base.transform.parent)
			{
				component = base.transform.parent.GetComponent<CFGCellObject>();
			}
			if (component == null)
			{
				return;
			}
		}
		foreach (CFGCell cellObject in component.GetCellObjects(bAlternate: false))
		{
			if (m_Locked)
			{
				LockWall(cellObject, 2);
				LockWall(cellObject, 3);
				LockWall(cellObject, 5);
				LockWall(cellObject, 4);
			}
			else
			{
				UnlockWall(cellObject, 2);
				UnlockWall(cellObject, 3);
				UnlockWall(cellObject, 5);
				UnlockWall(cellObject, 4);
			}
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.UpdateRangeVis();
		}
	}

	private void LockWall(CFGCell cell, byte wall)
	{
		if (cell.Editor_IsFlagSet(wall, 64))
		{
			cell.Flags[wall] |= 8;
		}
		if (cell.Editor_IsFlagSet((byte)(wall + 6), 64))
		{
			cell.Flags[wall + 6] |= 8;
		}
	}

	private void UnlockWall(CFGCell cell, byte wall)
	{
		if (cell.Editor_IsFlagSet(wall, 64))
		{
			cell.Flags[wall] &= 247;
		}
		if (cell.Editor_IsFlagSet((byte)(wall + 6), 64))
		{
			cell.Flags[wall + 6] &= 247;
		}
	}

	private int GetAnimationID(CFGCell currentcell)
	{
		int num = 0;
		if (currentcell == null)
		{
			return 0;
		}
		CFGCellObject component = base.transform.GetComponent<CFGCellObject>();
		if (component == null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
		}
		if (component == null)
		{
			Debug.LogWarning("Didnt find parent cell object");
			return 0;
		}
		currentcell.DecodePosition(out var PosX, out var PosZ, out var Floor);
		if (currentcell.OwnerObject == component)
		{
			if (currentcell.CheckFlag(5, 64))
			{
				if (component.Rotation == ERotation.CW_90)
				{
					num = 1;
				}
			}
			else if (currentcell.CheckFlag(4, 64))
			{
				if (component.Rotation == ERotation.CW_270)
				{
					num = 1;
				}
			}
			else if (currentcell.CheckFlag(3, 64))
			{
				if (component.Rotation == ERotation.None)
				{
					num = 1;
				}
			}
			else if (currentcell.CheckFlag(2, 64) && component.Rotation == ERotation.CW_180)
			{
				num = 1;
			}
			return num;
		}
		CFGCell cell = CFGCellMap.GetCell(PosZ + 1, PosX, Floor);
		if ((bool)cell && cell.OwnerObject == component && (cell.CheckFlag(2, 64) || cell.CheckFlag(3, 64)))
		{
			if (component.Rotation == ERotation.CW_180)
			{
				num = 1;
			}
			if (cell.CheckFlag(3, 64))
			{
				num = 1 - num;
			}
			return num;
		}
		cell = CFGCellMap.GetCell(PosZ - 1, PosX, Floor);
		if ((bool)cell && cell.OwnerObject == component && (cell.CheckFlag(2, 64) || cell.CheckFlag(3, 64)))
		{
			if (component.Rotation == ERotation.CW_180)
			{
				num = 1;
			}
			if (cell.CheckFlag(3, 64))
			{
				num = 1 - num;
			}
			return num;
		}
		cell = CFGCellMap.GetCell(PosZ, PosX + 1, Floor);
		if ((bool)cell && cell.OwnerObject == component && (cell.CheckFlag(4, 64) || cell.CheckFlag(5, 64)))
		{
			if (component.Rotation == ERotation.CW_270)
			{
				num = 1;
			}
			if (cell.CheckFlag(5, 64))
			{
				num = 1 - num;
			}
			return num;
		}
		cell = CFGCellMap.GetCell(PosZ, PosX - 1, Floor);
		if ((bool)cell && cell.OwnerObject == component && (cell.CheckFlag(4, 64) || cell.CheckFlag(5, 64)))
		{
			if (component.Rotation == ERotation.CW_270)
			{
				num = 1;
			}
			if (cell.CheckFlag(5, 64))
			{
				num = 1 - num;
			}
			return num;
		}
		return num;
	}

	private CFGCellObject GetCellObject()
	{
		CFGCellObject component = base.transform.GetComponent<CFGCellObject>();
		if (component != null || base.transform.parent == null)
		{
			return component;
		}
		return base.transform.parent.GetComponent<CFGCellObject>();
	}

	public CFGCell CanCharacterUse(CFGCharacter toon, bool bUseEvent, bool bCheckPath, out EUseMode UseMode)
	{
		UseMode = EUseMode.CantUse;
		if (!CanBeUsed)
		{
			if (bUseEvent && m_OnDoorFailureToOpen != null && ClosestCell(toon, bCheckPath, out UseMode) != null)
			{
				m_OnDoorFailureToOpen(this, toon);
				Debug.Log("Cannot be used");
			}
			return null;
		}
		return ClosestCell(toon, bCheckPath, out UseMode);
	}

	private CFGCell ClosestCell(CFGCharacter toon, bool bCheckPath, out EUseMode UseMode)
	{
		UseMode = EUseMode.CantUse;
		if (toon == null)
		{
			return null;
		}
		CFGCell currentCell = toon.CurrentCell;
		if (currentCell == null)
		{
			return null;
		}
		currentCell.DecodePosition(out var PosX, out var PosZ, out var Floor);
		CFGCell cFGCell = null;
		CFGCell cFGCell2 = null;
		CFGCellObject cellObject = GetCellObject();
		if (currentCell.OwnerObject == cellObject && (currentCell.CheckFlag(4, 64) || currentCell.CheckFlag(5, 64) || currentCell.CheckFlag(2, 64) || currentCell.CheckFlag(3, 64)))
		{
			UseMode = EUseMode.CanUse;
			return currentCell;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ + 1, PosX, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == cellObject && (cFGCell2.CheckFlag(2, 64) || cFGCell2.CheckFlag(3, 64)))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ - 1, PosX, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == cellObject && (cFGCell2.CheckFlag(2, 64) || cFGCell2.CheckFlag(3, 64)))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ, PosX + 1, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == cellObject && (cFGCell2.CheckFlag(4, 64) || cFGCell2.CheckFlag(5, 64)))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ, PosX - 1, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == cellObject && (cFGCell2.CheckFlag(4, 64) || cFGCell2.CheckFlag(5, 64)))
		{
			cFGCell = cFGCell2;
		}
		if (cFGCell != null)
		{
			UseMode = EUseMode.CanUse;
			return cFGCell;
		}
		if (bCheckPath)
		{
			LinkedList<CFGCell> bestPathTo = GetBestPathTo(toon);
			if (bestPathTo != null)
			{
				cFGCell = bestPathTo.Last.Value;
				if (cFGCell != null)
				{
					UseMode = EUseMode.MustWalk;
				}
			}
		}
		return cFGCell;
	}

	public List<CFGCell> GetActivatorCells()
	{
		CFGCell cell = CFGCellMap.GetCell(base.transform.position);
		if (cell == null)
		{
			return null;
		}
		List<CFGCell> list = new List<CFGCell>();
		if (list == null)
		{
			return null;
		}
		int num = Mathf.FloorToInt(base.transform.position.x);
		int num2 = Mathf.FloorToInt(base.transform.position.z);
		int floor = Mathf.FloorToInt(base.transform.position.y / 2.5f);
		CFGCell cFGCell = null;
		if (cell.CheckFlag(2, 64))
		{
			cFGCell = CFGCellMap.GetCell(num2 - 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 - 1, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
		}
		else if (cell.CheckFlag(3, 64))
		{
			cFGCell = CFGCellMap.GetCell(num2 - 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 - 1, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
		}
		else if (cell.CheckFlag(4, 64))
		{
			cFGCell = CFGCellMap.GetCell(num2, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 + 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
		}
		else if (cell.CheckFlag(5, 64))
		{
			cFGCell = CFGCellMap.GetCell(num2, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 - 1, num - 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 - 1, num + 1, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
			cFGCell = CFGCellMap.GetCell(num2 - 1, num, floor);
			if ((bool)cFGCell && !list.Contains(cFGCell))
			{
				list.Add(cFGCell);
			}
		}
		return list;
	}

	public LinkedList<CFGCell> GetBestPathTo(CFGCharacter Dude)
	{
		LinkedList<CFGCell> linkedList = null;
		HashSet<CFGCell> cells_in_range = CFGCellDistanceFinder.FindCellsInDistance(Dude.CurrentCell, (Dude.ActionPoints != 2) ? Dude.HalfMovement : Dude.DashingMovement);
		List<CFGCell> activatorCells = GetActivatorCells();
		if (activatorCells == null)
		{
			return null;
		}
		NavigationComponent component = Dude.GetComponent<NavigationComponent>();
		if (component == null)
		{
			return null;
		}
		CFGCell cellUnderCursor = CFGSelectionManager.Instance.CellUnderCursor;
		if (cellUnderCursor != null)
		{
			linkedList = GetBestPathToFromCells(Dude, new List<CFGCell> { cellUnderCursor }, cells_in_range);
			if (linkedList != null)
			{
				return linkedList;
			}
		}
		if (activatorCells != null && activatorCells.Count >= 2)
		{
			List<CFGCell> list = new List<CFGCell>();
			list.Add(activatorCells[activatorCells.Count - 1]);
			list.Add(activatorCells[activatorCells.Count - 2]);
			linkedList = GetBestPathToFromCells(Dude, list, cells_in_range);
			if (linkedList != null)
			{
				return linkedList;
			}
		}
		return GetBestPathToFromCells(Dude, activatorCells, cells_in_range);
	}

	private LinkedList<CFGCell> GetBestPathToFromCells(CFGCharacter Dude, List<CFGCell> cells, HashSet<CFGCell> cells_in_range)
	{
		if (cells_in_range == null)
		{
			return null;
		}
		LinkedList<CFGCell> result = null;
		NavigationComponent component = Dude.GetComponent<NavigationComponent>();
		if (component == null)
		{
			return null;
		}
		int num = 100000;
		float num2 = 100000f;
		if (cells != null && cells.Count > 0)
		{
			foreach (CFGCell cell in cells)
			{
				if (!cells_in_range.Contains(cell))
				{
					continue;
				}
				NavGoal_At navGoal_At = new NavGoal_At(cell);
				LinkedList<CFGCell> path = null;
				if (component.GeneratePath(Dude.CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out path))
				{
					float num3 = Vector3.Distance(Dude.CurrentCell.WorldPosition, path.Last.Value.WorldPosition);
					if (path.Count < num)
					{
						num = path.Count;
						result = path;
						num2 = num3;
					}
					else if (num3 < num2)
					{
						num = path.Count;
						result = path;
						num2 = num3;
					}
				}
			}
		}
		return result;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Opened", m_Opened);
		cFG_SG_Node.Attrib_Set("Locked", m_Locked);
		cFG_SG_Node.Attrib_Set("Blocked", m_BlockClosedForPathfinding);
		cFG_SG_Node.Attrib_Set("ID", m_AnimID);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node DoorNode)
	{
		bool opened = m_Opened;
		m_Opened = DoorNode.Attrib_Get("Opened", m_Opened);
		m_Locked = DoorNode.Attrib_Get("Locked", m_Locked);
		m_BlockClosedForPathfinding = DoorNode.Attrib_Get("Blocked", m_BlockClosedForPathfinding);
		m_AnimID = DoorNode.Attrib_Get("ID", -1);
		if (m_Opened && !opened)
		{
			ApplyNewCover();
		}
		PlayAnim(bInstant: true);
		return true;
	}
}
