using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGCellObject : CFGSerializableObject
{
	public enum eCOUsage
	{
		Unknown,
		Generic,
		MainMap,
		Location,
		Door,
		Usable,
		Ricochet
	}

	public enum EObjectType
	{
		Unknown,
		MainMap,
		Object,
		Monohedral
	}

	public delegate void OnCharacterMove(CFGCharacter character);

	public const int MAX_SIZE = 1024;

	public const int MAX_FLOOR = 8;

	public const float FLOOR_HEIGHT = 2.5f;

	public const float CHARACTER_HEIGHT = 1.25f;

	public const float VIS_FLOOR_Y_MOD = 0f;

	[HideInInspector]
	[CFGFlowCode(Category = "Trigger", Title = "On CellObject Trigger Enter")]
	public OnCharacterMove m_Event_OnTriggerEnter;

	[HideInInspector]
	[CFGFlowCode(Category = "Trigger", Title = "On CellObject Trigger Leave")]
	public OnCharacterMove m_Event_OnTriggerLeave;

	[HideInInspector]
	[CFGFlowCode(Category = "Trigger", Title = "On CellObject Trigger Finished")]
	public OnCharacterMove m_Event_OnTriggerStopped;

	[SerializeField]
	private bool m_ForceLight;

	[SerializeField]
	private int m_CostToMove_Vert = 20;

	[SerializeField]
	private int m_CostToMoveOnTriggerDelta;

	[SerializeField]
	protected string m_VariableActivatorScope = string.Empty;

	[SerializeField]
	protected string m_VariableActivatorName = string.Empty;

	public string m_Description = string.Empty;

	[SerializeField]
	[HideInInspector]
	private List<CFGCell> m_Cells;

	[SerializeField]
	[HideInInspector]
	private CFGCell m_SingleCell;

	[HideInInspector]
	[SerializeField]
	private List<CFGCell> m_AlternateCells;

	[HideInInspector]
	[SerializeField]
	private CFGCell m_AlternateSingleCell;

	[HideInInspector]
	[SerializeField]
	private ERotation m_Rotation;

	[HideInInspector]
	[SerializeField]
	private int m_Size_AxisZ = 1;

	[HideInInspector]
	[SerializeField]
	private int m_size_AxisX = 1;

	[HideInInspector]
	[SerializeField]
	private int m_Floors = 8;

	[SerializeField]
	[HideInInspector]
	private int m_StartPosX;

	[HideInInspector]
	[SerializeField]
	private int m_StartPosZ;

	[SerializeField]
	[HideInInspector]
	private int m_MainFloor;

	[HideInInspector]
	[SerializeField]
	private EObjectType m_ObjectType;

	[SerializeField]
	[HideInInspector]
	private bool m_CanChangeState;

	[HideInInspector]
	[SerializeField]
	public ECoverMirroring m_CoverMirror;

	[NonSerialized]
	private Dictionary<int, CFGCell> m_DictCells;

	[NonSerialized]
	private Dictionary<int, CFGCell> m_DictCellsAlt;

	[NonSerialized]
	public List<CFGCellObject> m_Childs = new List<CFGCellObject>();

	[SerializeField]
	public CFGCellObjectVisualisation m_VisualisationSource;

	[SerializeField]
	public CFGCellObjectVisualisation m_VisualisationSource_NormalOnly;

	[SerializeField]
	public CFGCellObjectVisualisation m_VisualisationSource_NMOnly;

	[HideInInspector]
	[SerializeField]
	public CFGCellObjectVisualisation m_CurrentVisualisation;

	[SerializeField]
	[HideInInspector]
	public CFGCellObjectVisualisation m_CurrentVisualisation_Add;

	[HideInInspector]
	[SerializeField]
	public CFGCellObjectVisualisation m_CurrentTempVisualisation;

	[HideInInspector]
	[SerializeField]
	public bool m_Overdraw = true;

	private CFGDoorObject m_Door;

	private CFGUsableObject m_Usable;

	private CFGLocationObject m_Location;

	private CFGRicochetObject m_Ricochet;

	private eCOUsage m_Usage;

	public override ESerializableType SerializableType => ESerializableType.CellObject;

	public override bool NeedsSaving => false;

	public bool IsLightForced => m_ForceLight;

	public int Cost2Move_Vertical => m_CostToMove_Vert;

	public int Cost2Move_Trigger => m_CostToMoveOnTriggerDelta;

	public bool IsMonohedral => m_ObjectType == EObjectType.Monohedral;

	public EObjectType ObjectType => m_ObjectType;

	public bool HasAlternateState => m_CanChangeState;

	public bool Overdraw => m_Overdraw;

	public int StartX => m_StartPosX;

	public int StartZ => m_StartPosZ;

	public int Size_Z => m_Size_AxisZ;

	public int Size_X => m_size_AxisX;

	public int MainFloor
	{
		get
		{
			return m_MainFloor;
		}
		set
		{
			m_MainFloor = Mathf.Clamp(value, 0, 8);
		}
	}

	public int Floors => m_Floors;

	public ERotation Rotation => m_Rotation;

	public eCOUsage Usage => m_Usage;

	public bool IsDoor => m_Door != null;

	public bool IsUsable => m_Usable != null;

	public bool ShouldApplyAlternateState
	{
		get
		{
			if (m_Usage == eCOUsage.Unknown)
			{
				DetectUsage();
			}
			switch (m_Usage)
			{
			case eCOUsage.Location:
				return m_Location.State switch
				{
					ELocationState.OPEN => false, 
					ELocationState.LOCKED => true, 
					ELocationState.HIDDEN => false, 
					_ => true, 
				};
			case eCOUsage.Door:
				if ((bool)m_Door)
				{
					return m_Door.IsOpened;
				}
				break;
			case eCOUsage.Usable:
				if ((bool)m_Usable)
				{
					return m_Usable.IsDynamicCover && m_Usable.Used;
				}
				break;
			}
			return false;
		}
	}

	public void CallEvent_OnEnterTrigger(CFGCharacter Tgt)
	{
		if (m_Event_OnTriggerEnter != null)
		{
			if (m_Usage == eCOUsage.Location && (bool)m_Location && m_Location.State != 0)
			{
				Debug.Log("Location is not open!");
			}
			else
			{
				m_Event_OnTriggerEnter(Tgt);
			}
		}
	}

	public void CallEvent_OnLeaveTrigger(CFGCharacter Tgt)
	{
		if (m_Event_OnTriggerLeave != null)
		{
			if (m_Usage == eCOUsage.Location && (bool)m_Location && m_Location.State != 0)
			{
				Debug.Log("Location is not open!");
			}
			else
			{
				m_Event_OnTriggerLeave(Tgt);
			}
		}
	}

	public void CallEvent_OnStopOnTrigger(CFGCharacter Tgt)
	{
		if (m_Event_OnTriggerStopped == null)
		{
			return;
		}
		if (m_Usage == eCOUsage.Location && (bool)m_Location)
		{
			if (m_Location.State != 0)
			{
				Debug.Log("Location is not open!");
				return;
			}
			m_Location.OnCharacterEnter();
			if ((bool)Tgt)
			{
				Tgt.SetNextActionDelay();
			}
		}
		m_Event_OnTriggerStopped(Tgt);
	}

	public bool CheckIfShouldBeDisabled()
	{
		if (string.IsNullOrEmpty(m_VariableActivatorName) || string.IsNullOrEmpty(m_VariableActivatorScope))
		{
			return false;
		}
		CFGVar variable = CFGVariableContainer.Instance.GetVariable(m_VariableActivatorName, m_VariableActivatorScope);
		if (variable == null || variable.definition == null)
		{
			return false;
		}
		if (variable.definition.ValueType != typeof(bool))
		{
			Debug.LogWarning("Variable " + m_VariableActivatorScope + "::" + m_VariableActivatorName + " is not of Boolean type and cannot be used to disable CFGCellObject!");
			return false;
		}
		bool flag = !(bool)variable.Value;
		Debug.Log("Check for disable + " + base.name + " [" + m_VariableActivatorScope + "::" + m_VariableActivatorName + "] =  " + !flag);
		return flag;
	}

	public void SetDisabled()
	{
		RemoveAdditionalVisualisation();
		RemoveVisuals();
		if ((bool)m_CurrentTempVisualisation)
		{
			m_CurrentTempVisualisation.transform.parent = null;
			UnityEngine.Object.Destroy(m_CurrentTempVisualisation.gameObject);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (string.Compare(child.name, "LAVis", ignoreCase: true) == 0)
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}

	public CFGDoorObject GetDoor()
	{
		return m_Door;
	}

	public CFGUsableObject GetUsable()
	{
		return m_Usable;
	}

	public int GetCellCount(bool bAlternate)
	{
		if (IsMonohedral)
		{
			return 1;
		}
		if (bAlternate)
		{
			if (m_AlternateCells != null)
			{
				return m_AlternateCells.Count;
			}
			return 0;
		}
		if (m_Cells != null)
		{
			return m_Cells.Count;
		}
		return 0;
	}

	public List<CFGCell> GetCellObjects(bool bAlternate)
	{
		if (bAlternate)
		{
			return m_AlternateCells;
		}
		return m_Cells;
	}

	public void EnableTempDictionary(bool bEnable)
	{
		if (!bEnable)
		{
			m_DictCells = null;
			m_DictCellsAlt = null;
			return;
		}
		if (m_DictCells == null)
		{
			m_DictCells = new Dictionary<int, CFGCell>();
		}
		if (m_DictCellsAlt == null)
		{
			m_DictCellsAlt = new Dictionary<int, CFGCell>();
		}
		if (m_DictCells != null)
		{
			m_DictCells.Clear();
			if (m_Cells != null)
			{
				foreach (CFGCell cell in m_Cells)
				{
					if (m_DictCells.ContainsKey(cell.EP))
					{
						Debug.LogWarning("Cloned block exists!");
					}
					else
					{
						m_DictCells.Add(cell.EP, cell);
					}
				}
			}
		}
		if (m_DictCellsAlt == null)
		{
			return;
		}
		m_DictCellsAlt.Clear();
		if (m_AlternateCells == null)
		{
			return;
		}
		foreach (CFGCell alternateCell in m_AlternateCells)
		{
			if (m_DictCellsAlt.ContainsKey(alternateCell.EP))
			{
				Debug.LogWarning("Cloned block exists!");
			}
			else
			{
				m_DictCellsAlt.Add(alternateCell.EP, alternateCell);
			}
		}
	}

	public void Rotate(ERotation NewRotation)
	{
		if (NewRotation != m_Rotation)
		{
			SetRotation(NewRotation);
		}
	}

	public void Move(EDirection Dir)
	{
		switch (Dir)
		{
		case EDirection.NORTH:
			m_StartPosX--;
			break;
		case EDirection.SOUTH:
			m_StartPosX++;
			break;
		case EDirection.EAST:
			m_StartPosZ++;
			break;
		case EDirection.WEST:
			m_StartPosZ--;
			break;
		}
		base.transform.position = new Vector3(m_StartPosX, 0f, m_StartPosZ);
	}

	public bool ChangeType(EObjectType NewType, bool bCanChangeState)
	{
		if (NewType == m_ObjectType && bCanChangeState == m_CanChangeState)
		{
			return true;
		}
		if (NewType == EObjectType.Unknown)
		{
			return false;
		}
		m_CanChangeState = false;
		Debug.Log(string.Concat("Change object type to: ", NewType, " dual state? ", bCanChangeState));
		switch (NewType)
		{
		case EObjectType.MainMap:
		case EObjectType.Object:
			m_SingleCell = null;
			m_AlternateSingleCell = null;
			if (m_Cells == null)
			{
				m_Cells = new List<CFGCell>();
			}
			if (m_Cells == null)
			{
				Debug.LogError("Failed to allocate memory for cells");
				return false;
			}
			if (bCanChangeState && NewType != EObjectType.MainMap)
			{
				m_CanChangeState = true;
				if (m_AlternateCells == null)
				{
					m_AlternateCells = new List<CFGCell>();
					if (m_AlternateCells == null)
					{
						Debug.LogError("Failed to allocate memory for cells");
						return false;
					}
				}
			}
			else
			{
				m_AlternateCells = null;
			}
			EnableTempDictionary(m_DictCells != null);
			break;
		case EObjectType.Monohedral:
			m_Cells = null;
			m_AlternateCells = null;
			m_SingleCell = new CFGCell();
			if (m_SingleCell == null)
			{
				return false;
			}
			if (bCanChangeState)
			{
				m_CanChangeState = true;
				m_AlternateSingleCell = new CFGCell();
				if (m_AlternateSingleCell == null)
				{
					return false;
				}
			}
			else
			{
				m_AlternateSingleCell = null;
			}
			break;
		}
		m_ObjectType = NewType;
		return true;
	}

	public bool SetSize(int NewWidth, int NewHeight, int NewFloors)
	{
		if (m_ObjectType == EObjectType.Unknown)
		{
			return false;
		}
		if (NewWidth == m_Size_AxisZ && NewHeight == m_size_AxisX && NewFloors == m_Floors)
		{
			return true;
		}
		m_Floors = Mathf.Clamp(NewFloors, 0, 8);
		if (m_ObjectType != EObjectType.Monohedral && m_Cells != null)
		{
			int num = 0;
			int PosX = 0;
			int PosZ = 0;
			int Floor = 0;
			while (num < m_Cells.Count)
			{
				bool flag = false;
				if (m_Cells[num] == null || !m_Cells[num].DecodePosition(out PosX, out PosZ, out Floor))
				{
					flag = true;
				}
				else if (PosX < 0 || PosX > m_Size_AxisZ || PosZ < 0 || PosZ > m_size_AxisX || Floor >= m_Floors)
				{
					flag = true;
				}
				if (flag)
				{
					m_Cells.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
		m_Size_AxisZ = NewWidth;
		m_size_AxisX = NewHeight;
		if (m_MainFloor > m_Floors)
		{
			m_MainFloor = 0;
		}
		EnableTempDictionary(m_DictCells != null);
		return true;
	}

	public void RemoveEmptyCells()
	{
		if (m_Cells == null)
		{
			return;
		}
		int num = 0;
		while (num < m_Cells.Count)
		{
			if (m_Cells[num] == null || m_Cells[num].IsEmpty)
			{
				m_Cells.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		EnableTempDictionary(m_DictCells != null);
	}

	public void ClearFloor(int FloorToClear)
	{
		if (m_Cells == null)
		{
			return;
		}
		int num = 0;
		int PosX = 0;
		int PosZ = 0;
		int Floor = 0;
		while (num < m_Cells.Count)
		{
			bool flag = false;
			if (m_Cells[num] == null || !m_Cells[num].DecodePosition(out PosX, out PosZ, out Floor))
			{
				flag = true;
			}
			else if (Floor == FloorToClear)
			{
				flag = true;
			}
			if (flag)
			{
				m_Cells.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		EnableTempDictionary(m_DictCells != null);
	}

	public void ClearMap()
	{
		if (m_Cells != null)
		{
			m_Cells.Clear();
		}
		if (m_AlternateCells != null)
		{
			m_AlternateCells.Clear();
		}
		EnableTempDictionary(m_DictCells != null);
	}

	public void ClearNMDiffFlags()
	{
		if (m_Cells != null)
		{
			foreach (CFGCell cell in m_Cells)
			{
				if ((bool)cell)
				{
					cell.ClearIdenticalNMSettings();
				}
			}
		}
		if (m_AlternateCells != null)
		{
			foreach (CFGCell alternateCell in m_AlternateCells)
			{
				if ((bool)alternateCell)
				{
					alternateCell.ClearIdenticalNMSettings();
				}
			}
		}
		if (m_SingleCell != null)
		{
			m_SingleCell.ClearIdenticalNMSettings();
		}
		if (m_AlternateSingleCell != null)
		{
			m_AlternateSingleCell.ClearIdenticalNMSettings();
		}
	}

	public void MakeWallCovers()
	{
		CorrectWalls(bAlternate: false);
		CorrectWalls(bAlternate: true);
	}

	private void CorrectWalls(bool bAlternate)
	{
		for (int i = 0; i < Floors; i++)
		{
			for (int j = 0; j < m_size_AxisX; j++)
			{
				for (int k = 0; k < m_Size_AxisZ; k++)
				{
					CFGCell cell = GetCell(k, j, i, bAlternate, bExisitingOnly: true);
					if (cell != null)
					{
						if (j > 0)
						{
							CorrectFlags(cell, 2, k, j - 1, 3, bAlternate);
						}
						if (j < m_size_AxisX - 1)
						{
							CorrectFlags(cell, 3, k, j + 1, 2, bAlternate);
						}
						if (k > 0)
						{
							CorrectFlags(cell, 5, k - 1, j, 4, bAlternate);
						}
						if (k < m_Size_AxisZ - 1)
						{
							CorrectFlags(cell, 4, k + 1, j, 5, bAlternate);
						}
					}
				}
			}
		}
	}

	private void CorrectFlags(CFGCell cell, int flag, int x1, int z1, int flag2, bool balt)
	{
		if (cell == null)
		{
			return;
		}
		byte b = (byte)(cell.Flags[flag] & 0x3Eu);
		byte b2 = (byte)(cell.Flags[flag + 6] & 0x3Eu);
		if (b != 0 || b2 != 0)
		{
			Debug.Log("Corecting: " + x1 + " " + z1);
			CFGCell cell2 = GetCell(x1, z1, cell.Floor, balt, bExisitingOnly: false);
			if (cell2 != null)
			{
				cell2.Flags[flag2] |= b;
				cell2.Flags[flag2 + 6] |= b2;
			}
		}
	}

	public void ClearNightmareMode()
	{
		if (m_Cells != null)
		{
			foreach (CFGCell cell in m_Cells)
			{
				if ((bool)cell)
				{
					cell.ClearNightmareDiff();
				}
			}
		}
		if (m_AlternateCells != null)
		{
			foreach (CFGCell alternateCell in m_AlternateCells)
			{
				if ((bool)alternateCell)
				{
					alternateCell.ClearNightmareDiff();
				}
			}
		}
		if (m_SingleCell != null)
		{
			m_SingleCell.ClearNightmareDiff();
		}
		if (m_AlternateSingleCell != null)
		{
			m_AlternateSingleCell.ClearNightmareDiff();
		}
	}

	public CFGCell GetCell(int X, int Z, int Floor, bool bAlternate, bool bExisitingOnly)
	{
		if (IsMonohedral)
		{
			if (bAlternate)
			{
				return m_AlternateSingleCell;
			}
			return m_SingleCell;
		}
		int encodedPosition = CFGCell.GetEncodedPosition(Floor, X, Z);
		CFGCell value = null;
		if (m_DictCellsAlt != null && bAlternate)
		{
			if (m_DictCellsAlt.TryGetValue(encodedPosition, out value))
			{
				return value;
			}
		}
		else if (m_DictCells != null)
		{
			if (m_DictCells.TryGetValue(encodedPosition, out value))
			{
				return value;
			}
		}
		else
		{
			List<CFGCell> list = ((!bAlternate) ? m_Cells : m_AlternateCells);
			if (list == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if ((bool)list[i] && list[i].EP == encodedPosition)
				{
					return list[i];
				}
			}
		}
		if (bExisitingOnly)
		{
			return null;
		}
		value = new CFGCell();
		if ((bool)value)
		{
			value.EP = encodedPosition;
			if (bAlternate)
			{
				m_AlternateCells.Add(value);
				if (m_DictCellsAlt != null)
				{
					m_DictCellsAlt.Add(value.EP, value);
				}
			}
			else
			{
				m_Cells.Add(value);
				if (m_DictCells != null)
				{
					m_DictCells.Add(value.EP, value);
				}
			}
		}
		return value;
	}

	public static int GetFloor(float y)
	{
		float num = y / 2.5f;
		return (int)num;
	}

	public static Vector3 SnapToGrid(Vector3 point)
	{
		return new Vector3(Mathf.Round(point.x), (float)GetFloor(point.y) * 2.5f, Mathf.Round(point.z));
	}

	public void SetPosition(Vector3 NewOrigin)
	{
		Vector3 position = SnapToGrid(NewOrigin);
		base.transform.position = position;
		m_StartPosX = Mathf.RoundToInt(position.x);
		m_StartPosZ = Mathf.RoundToInt(position.z);
	}

	public void SetRotation(ERotation NewRotation)
	{
		m_Rotation = NewRotation;
		base.transform.rotation = Quaternion.identity;
		UpdateVisTransform(m_CurrentVisualisation);
		UpdateVisTransform(m_CurrentTempVisualisation);
		UpdateVisTransform(m_CurrentVisualisation_Add);
	}

	private void UpdateVisTransform(CFGCellObjectVisualisation vis)
	{
		if (!(vis == null))
		{
			Transform transform = vis.transform;
			transform.parent = base.transform;
			transform.rotation = Quaternion.identity;
			switch (m_Rotation)
			{
			case ERotation.None:
				transform.position = base.transform.position + new Vector3(0.5f, 0f, 0.5f);
				transform.forward = Vector3.forward;
				break;
			case ERotation.CW_90:
				transform.position = base.transform.position + new Vector3(0.5f, 0f, 0.5f);
				transform.forward = Vector3.right;
				break;
			case ERotation.CW_180:
				transform.position = base.transform.position + new Vector3(0.5f, 0f, 0.5f);
				transform.forward = Vector3.back;
				break;
			case ERotation.CW_270:
				transform.position = base.transform.position + new Vector3(0.5f, 0f, 0.5f);
				transform.forward = -Vector3.right;
				break;
			}
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			if (ObjectType != EObjectType.MainMap)
			{
				DetectUsage();
				return;
			}
			m_Usage = eCOUsage.MainMap;
			CFGCellMap.CreateMap(this);
		}
	}

	public void DetectUsage()
	{
		m_Usage = eCOUsage.Generic;
		m_Door = null;
		m_Usable = null;
		m_Ricochet = null;
		m_Location = null;
		if (!CheckObjectUsage(base.transform))
		{
			for (int i = 0; i < base.transform.childCount && !CheckObjectUsage(base.transform.GetChild(i)); i++)
			{
			}
		}
	}

	private bool CheckObjectUsage(Transform t)
	{
		if (t == null)
		{
			return false;
		}
		m_Door = t.GetComponent<CFGDoorObject>();
		if (m_Door != null)
		{
			m_Usage = eCOUsage.Door;
			return true;
		}
		m_Usable = t.GetComponent<CFGUsableObject>();
		if ((bool)m_Usable)
		{
			m_Usage = eCOUsage.Usable;
			return true;
		}
		m_Ricochet = t.GetComponent<CFGRicochetObject>();
		if ((bool)m_Ricochet)
		{
			m_Usage = eCOUsage.Ricochet;
			return true;
		}
		m_Location = t.GetComponent<CFGLocationObject>();
		if ((bool)m_Location)
		{
			m_Usage = eCOUsage.Location;
			return true;
		}
		return false;
	}

	public void CreateHierarchy()
	{
		m_Childs.Clear();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child == null)
			{
				continue;
			}
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if (!(gameObject == null))
			{
				CFGCellObject component = gameObject.GetComponent<CFGCellObject>();
				if (!(component == null))
				{
					component.CreateHierarchy();
					m_Childs.Add(component);
				}
			}
		}
	}

	public void SpawnVisuals()
	{
		RemoveVisuals();
		if (!(m_VisualisationSource == null))
		{
			m_CurrentVisualisation = UnityEngine.Object.Instantiate(m_VisualisationSource);
			if (!(m_VisualisationSource == null))
			{
				SetRotation(m_Rotation);
			}
		}
	}

	public void RemoveVisuals(bool bInstant = false)
	{
		if (!(m_CurrentVisualisation == null))
		{
			m_CurrentVisualisation.transform.parent = null;
			if (bInstant)
			{
				UnityEngine.Object.DestroyImmediate(m_CurrentVisualisation.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(m_CurrentVisualisation.gameObject);
			}
			m_CurrentVisualisation = null;
		}
	}

	public void SpawnAdditionalVisualisation(bool bNormal, bool bAddVisScript = true, bool bInstant = false)
	{
		RemoveAdditionalVisualisation(bInstant);
		UnityEngine.Object @object = ((!bNormal) ? m_VisualisationSource_NMOnly : m_VisualisationSource_NormalOnly);
		if (@object == null)
		{
			return;
		}
		m_CurrentVisualisation_Add = UnityEngine.Object.Instantiate(@object) as CFGCellObjectVisualisation;
		if (!(m_CurrentVisualisation_Add == null))
		{
			if (bAddVisScript)
			{
				CFGCellMap.AddVisScripts(m_CurrentVisualisation_Add, this);
			}
			SetRotation(m_Rotation);
		}
	}

	public void RemoveAdditionalVisualisation(bool bInstant = false)
	{
		if (!(m_CurrentVisualisation_Add == null))
		{
			m_CurrentVisualisation_Add.transform.parent = null;
			if (bInstant)
			{
				UnityEngine.Object.DestroyImmediate(m_CurrentVisualisation_Add.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(m_CurrentVisualisation_Add.gameObject);
			}
			m_CurrentVisualisation_Add = null;
		}
	}

	public void RecalculateShadowMap()
	{
		if (CFGCellShadowMapLevel.MapType != 0)
		{
			float num = base.transform.position.z;
			float num2 = num + 1f;
			float num3 = base.transform.position.x;
			float num4 = num3 + 1f;
			float y = base.transform.position.y;
			float yMax = base.transform.position.y + (float)Floors * 2.5f;
			switch (m_Rotation)
			{
			case ERotation.None:
				num2 = num + (float)Size_Z;
				num4 = num3 + (float)Size_X;
				break;
			case ERotation.CW_90:
				num2 = num + 1f;
				num = num2 - (float)Size_X;
				num4 = num3 + (float)Size_Z;
				break;
			case ERotation.CW_180:
				num2 = num + 1f;
				num4 = num3 + 1f;
				num = num2 - (float)Size_Z;
				num3 = num4 - (float)Size_X;
				break;
			case ERotation.CW_270:
				num4 = num3 + 1f;
				num3 = num4 - (float)Size_Z;
				num2 = num + (float)Size_X;
				break;
			}
			CFGCellShadowMapChar.CalcBox(num, num2, num3, num4, y, yMax, CFGCellShadowMapLevel.MainMap.SunDirection, out var CalcMin, out var CalcMax);
			CFGCellShadowMapLevel.RecalculateArea(Mathf.FloorToInt(CalcMin.z), Mathf.FloorToInt(CalcMax.z + 0.99f), Mathf.FloorToInt(CalcMin.x), Mathf.FloorToInt(CalcMax.x + 0.99f), Mathf.FloorToInt(CalcMin.y / 2.5f), Mathf.FloorToInt((CalcMax.y + 2.49f) / 2.5f));
			CFGGame.EnableUpdate(Vampire: true, CharShadows: true);
		}
	}
}
