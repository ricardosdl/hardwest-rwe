using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CFGCell
{
	public enum EStairsType
	{
		None,
		EntryPoint,
		Slope,
		ExitPoint
	}

	[Flags]
	public enum ETileFlags : byte
	{
		None = 0,
		NMDiff = 1,
		HasFloor = 2,
		HasLadder = 4,
		Stairs_Low = 8,
		Stairs_High = 0x10,
		IsInterior = 0x20,
		Trigger = 0x40,
		Shadow = 0x80,
		STAIRS = 0x18
	}

	[Flags]
	public enum ECMPFlags : byte
	{
		None = 0,
		NMDiff = 1,
		IsCover = 2,
		IsFullCover = 4,
		Impassable = 8,
		BlockSight = 0x10,
		StopBullet = 0x20,
		Activator = 0x40,
		DoNotApply = 0x80
	}

	[Flags]
	public enum EColumn : byte
	{
		None = 0,
		NorthEast = 1,
		NorthWest = 2,
		SouthWest = 4,
		SouthEast = 8
	}

	public class SPData
	{
		public CFGCellObjectPart m_wall_fc;

		public CFGCellObjectPart m_wall_hc;

		public CFGCellObjectPart m_floor;

		public Transform ptrans;

		public CFGCell cell;
	}

	public const byte IP_TILE_Flags = 0;

	public const byte IP_CENTER_Flags = 1;

	public const byte IP_NORTH_Flags = 2;

	public const byte IP_SOUTH_Flags = 3;

	public const byte IP_EAST_Flags = 4;

	public const byte IP_WEST_Flags = 5;

	public const byte IP_NM_ADD = 6;

	public const byte IP_NM_TILE_Flags = 6;

	public const byte IP_NM_CENTER_Flags = 7;

	public const byte IP_NM_NORTH_Flags = 8;

	public const byte IP_NM_SOUTH_Flags = 9;

	public const byte IP_NM_EAST_Flags = 10;

	public const byte IP_NM_WEST_Flags = 11;

	public const int INVALID_CELL_POSITION = -1;

	public const int FLAGCOUNT = 12;

	public const int DEF_COST_TO_MOVE = 10;

	public const int DEF_COST_TO_MOVE_TRIGGER = 0;

	public const int DEF_COST_TO_MOVE_DIAGONAL = 15;

	public const int DEF_COST_TO_MOVE_VERTICAL = 20;

	public const int DEF_COST_TO_MOVE_STAIRS = 15;

	public int EP = -1;

	public byte[] Flags = new byte[12];

	[NonSerialized]
	private float m_DeltaHeight;

	private EColumn m_CornerColumns;

	[NonSerialized]
	private CFGCellObject m_OwnerObject;

	[NonSerialized]
	private CFGCellObject m_OwnerInterionObject;

	[NonSerialized]
	private CFGCellObject m_TriggerObject;

	[NonSerialized]
	private CFGUsableObject m_UsableObject;

	[NonSerialized]
	private CFGDoorObject m_DoorObject;

	[NonSerialized]
	[HideInInspector]
	private CFGCharacter m_Character;

	[NonSerialized]
	[HideInInspector]
	private CFGCharacter m_MovingChar;

	private int m_CostToMove;

	[NonSerialized]
	public int m_AiValue;

	private bool m_bIsInLight;

	public float DeltaHeight
	{
		get
		{
			return m_DeltaHeight;
		}
		set
		{
			m_DeltaHeight = Mathf.Clamp(value, 0f, 2.5f);
		}
	}

	public EStairsType StairsType
	{
		get
		{
			byte b = 0;
			if ((Flags[0] & 1) == 1 && CFGGame.Nightmare)
			{
				b = 6;
			}
			return (ETileFlags)(byte)(Flags[b] & 0x18) switch
			{
				ETileFlags.Stairs_Low => EStairsType.EntryPoint, 
				ETileFlags.STAIRS => EStairsType.Slope, 
				ETileFlags.Stairs_High => EStairsType.ExitPoint, 
				_ => EStairsType.None, 
			};
		}
	}

	public bool HaveFloor
	{
		get
		{
			if (Floor == 0)
			{
				return true;
			}
			return CheckFlag(0, 2);
		}
	}

	public int Floor
	{
		get
		{
			int num = (EP >> 24) & 0xFF;
			if (EP == -1 || num >= 8)
			{
				return 0;
			}
			return num;
		}
	}

	public int PositionZ => (EP >> 12) & 0xFFF;

	public int PositionX => EP & 0xFFF;

	public Vector3 WorldPosition
	{
		get
		{
			if (EP == -1)
			{
				return new Vector3(float.MinValue, float.MinValue, float.MinValue);
			}
			return new Vector3((float)((EP >> 12) & 0xFFF) + 0.5f, (float)((EP >> 24) & 0xFF) * 2.5f, (float)(EP & 0xFFF) + 0.5f);
		}
	}

	public Vector3 WorldPositionForMovement
	{
		get
		{
			if (EP == -1)
			{
				return new Vector3(float.MinValue, float.MinValue, float.MinValue);
			}
			return new Vector3((float)((EP >> 12) & 0xFFF) + 0.5f, (float)((EP >> 24) & 0xFF) * 2.5f + m_DeltaHeight, (float)(EP & 0xFFF) + 0.5f);
		}
	}

	public bool IsEmpty
	{
		get
		{
			for (int i = 0; i < Flags.Length; i++)
			{
				if (Flags[i] != 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	public CFGCellObject TriggerObject
	{
		get
		{
			return m_TriggerObject;
		}
		set
		{
			m_TriggerObject = value;
		}
	}

	public CFGCellObject OwnerObject
	{
		get
		{
			return m_OwnerObject;
		}
		set
		{
			m_OwnerObject = value;
		}
	}

	public CFGUsableObject UsableObject
	{
		get
		{
			return m_UsableObject;
		}
		set
		{
			m_UsableObject = value;
		}
	}

	public CFGDoorObject DoorObject
	{
		get
		{
			return m_DoorObject;
		}
		set
		{
			m_DoorObject = value;
		}
	}

	public CFGCellObject InteriorObject
	{
		get
		{
			return m_OwnerInterionObject;
		}
		set
		{
			m_OwnerInterionObject = value;
		}
	}

	public CFGCharacter CurrentCharacter
	{
		get
		{
			return m_Character;
		}
		set
		{
			m_Character = value;
		}
	}

	public int CostToMove
	{
		get
		{
			return m_CostToMove;
		}
		protected set
		{
			m_CostToMove = value;
		}
	}

	public bool IsInLight => m_bIsInLight;

	public static ETileFlags StairsToFlag(EStairsType stype)
	{
		return stype switch
		{
			EStairsType.EntryPoint => ETileFlags.Stairs_Low, 
			EStairsType.Slope => ETileFlags.STAIRS, 
			EStairsType.ExitPoint => ETileFlags.Stairs_High, 
			_ => ETileFlags.None, 
		};
	}

	public bool Editor_IsFlagSet(byte FlagType, byte Flag)
	{
		if (FlagType >= 12)
		{
			return false;
		}
		return (Flags[FlagType] & Flag) == Flag;
	}

	public EStairsType Editor_GetStairsType(bool bNightmare)
	{
		ETileFlags eTileFlags = ETileFlags.None;
		return (ETileFlags)((!bNightmare) ? ((byte)(Flags[0] & 0x18)) : ((byte)(Flags[6] & 0x18))) switch
		{
			ETileFlags.Stairs_Low => EStairsType.EntryPoint, 
			ETileFlags.STAIRS => EStairsType.Slope, 
			ETileFlags.Stairs_High => EStairsType.ExitPoint, 
			_ => EStairsType.None, 
		};
	}

	public void Editor_SetStairsType(EStairsType newtype, bool bNightmare)
	{
		byte b = (byte)StairsToFlag(newtype);
		byte b2 = 0;
		if (bNightmare)
		{
			b2 += 6;
		}
		if (b != 0)
		{
			b = (byte)(b | 2u);
		}
		Flags[b2] &= 229;
		Flags[b2] |= b;
	}

	public bool CheckFlag(byte FlagType, byte Flag)
	{
		if ((Flags[FlagType] & 1) == 1 && CFGGame.Nightmare)
		{
			FlagType += 6;
		}
		return (Flags[FlagType] & Flag) == Flag;
	}

	public bool CheckIfClosed()
	{
		if (CheckFlag(1, 8))
		{
			return true;
		}
		if (CheckFlag(2, 8) && CheckFlag(3, 8) && CheckFlag(5, 8) && CheckFlag(4, 8))
		{
			return true;
		}
		return false;
	}

	public bool IsHalfCover(byte FlagType)
	{
		if ((Flags[FlagType] & 1) == 1 && CFGGame.Nightmare)
		{
			FlagType += 6;
		}
		if ((Flags[FlagType] & 2) == 2 && (Flags[FlagType] & 4) != 4)
		{
			return true;
		}
		return false;
	}

	public bool HasColumn(EColumn Column)
	{
		if ((m_CornerColumns & Column) == Column)
		{
			return true;
		}
		return false;
	}

	public void EnableColumn(EColumn Column, bool bEnable)
	{
		if (bEnable)
		{
			m_CornerColumns |= Column;
		}
		else
		{
			m_CornerColumns &= (EColumn)(byte)(~(int)Column);
		}
	}

	public byte ModifyFlagForNightMare(byte BasicFlag)
	{
		if ((Flags[BasicFlag] & 1) == 1 && CFGGame.Nightmare)
		{
			return (byte)(BasicFlag + 6);
		}
		return BasicFlag;
	}

	public ECoverType GetCenterCover()
	{
		byte b = 1;
		if ((Flags[b] & 1) == 1 && CFGGame.Nightmare)
		{
			b += 6;
		}
		if (!Editor_IsFlagSet(b, 2))
		{
			return ECoverType.NONE;
		}
		if (Editor_IsFlagSet(b, 4))
		{
			return ECoverType.FULL;
		}
		return ECoverType.HALF;
	}

	public ECoverType GetBorderCover(EDirection dir)
	{
		byte b = 2;
		switch (dir)
		{
		case EDirection.EAST:
			b = 4;
			break;
		case EDirection.SOUTH:
			b = 3;
			break;
		case EDirection.WEST:
			b = 5;
			break;
		}
		if ((Flags[b] & 1) == 1 && CFGGame.Nightmare)
		{
			b += 6;
		}
		if (!Editor_IsFlagSet(b, 2))
		{
			return ECoverType.NONE;
		}
		if (Editor_IsFlagSet(b, 4))
		{
			return ECoverType.FULL;
		}
		return ECoverType.HALF;
	}

	public CFGCell FindNeighbour(EDirection dir)
	{
		return dir switch
		{
			EDirection.NORTH => CFGCellMap.GetCell(PositionX, PositionZ - 1, Floor), 
			EDirection.EAST => CFGCellMap.GetCell(PositionX + 1, PositionZ, Floor), 
			EDirection.SOUTH => CFGCellMap.GetCell(PositionX, PositionZ + 1, Floor), 
			_ => CFGCellMap.GetCell(PositionX - 1, PositionZ, Floor), 
		};
	}

	public CFGCell FindNeighbour(ECornerDirection dir)
	{
		return dir switch
		{
			ECornerDirection.NE => CFGCellMap.GetCell(PositionX + 1, PositionZ - 1, Floor), 
			ECornerDirection.SE => CFGCellMap.GetCell(PositionX + 1, PositionZ + 1, Floor), 
			ECornerDirection.SW => CFGCellMap.GetCell(PositionX - 1, PositionZ + 1, Floor), 
			_ => CFGCellMap.GetCell(PositionX - 1, PositionZ - 1, Floor), 
		};
	}

	public bool DecodePosition(out int PosX, out int PosZ, out int Floor)
	{
		if (EP == -1)
		{
			Floor = -1;
			PosX = -1;
			PosZ = -1;
			return false;
		}
		Floor = (EP >> 24) & 0xFF;
		PosX = (EP >> 12) & 0xFFF;
		PosZ = EP & 0xFFF;
		return true;
	}

	public void EncodePosition(int Floor, int CellX, int CellZ)
	{
		EP = GetEncodedPosition(Floor, CellX, CellZ);
	}

	public static int GetEncodedPosition(int Floor, int CellX, int CellZ)
	{
		int num = (Floor & 0xFF) << 24;
		num |= (CellX & 0xFFF) << 12;
		return num | (CellZ & 0xFFF);
	}

	private void ClearIdenticalNM(int PosSrc, int PosDest)
	{
		if (Flags[PosSrc] == Flags[PosDest])
		{
			Flags[PosSrc] &= 254;
			Flags[PosDest] = 0;
		}
	}

	public void ClearIdenticalNMSettings(bool bCenter = true, bool bNorth = true, bool bWest = true, bool bSouth = true, bool bEast = true)
	{
		if (bCenter)
		{
			ClearIdenticalNM(1, 7);
		}
		if (bNorth)
		{
			ClearIdenticalNM(2, 8);
		}
		if (bWest)
		{
			ClearIdenticalNM(5, 11);
		}
		if (bEast)
		{
			ClearIdenticalNM(4, 10);
		}
		if (bSouth)
		{
			ClearIdenticalNM(3, 9);
		}
	}

	public void ClearNightmareDiff(bool bCenter = true, bool bNorth = true, bool bWest = true, bool bSouth = true, bool bEast = true)
	{
		if (bCenter)
		{
			Flags[1] &= 254;
			Flags[7] = 0;
		}
		if (bWest)
		{
			Flags[5] &= 254;
			Flags[11] = 0;
		}
		if (bNorth)
		{
			Flags[2] &= 254;
			Flags[8] = 0;
		}
		if (bSouth)
		{
			Flags[3] &= 254;
			Flags[9] = 0;
		}
		if (bEast)
		{
			Flags[4] &= 254;
			Flags[10] = 0;
		}
	}

	public CFGCell CreateRotatedCell(ERotation rotation)
	{
		CFGCell cFGCell = new CFGCell();
		cFGCell.EP = EP;
		cFGCell.Flags[0] = Flags[0];
		cFGCell.Flags[1] = Flags[1];
		cFGCell.Flags[6] = Flags[6];
		cFGCell.Flags[7] = Flags[7];
		int num = 2;
		int num2 = 3;
		int num3 = 5;
		int num4 = 4;
		switch (rotation)
		{
		case ERotation.CW_90:
			num = 5;
			num4 = 2;
			num2 = 4;
			num3 = 3;
			break;
		case ERotation.CW_180:
			num = 3;
			num4 = 5;
			num2 = 2;
			num3 = 4;
			break;
		case ERotation.CW_270:
			num = 4;
			num4 = 3;
			num2 = 5;
			num3 = 2;
			break;
		}
		cFGCell.Flags[2] = Flags[num];
		cFGCell.Flags[8] = Flags[num + 6];
		cFGCell.Flags[3] = Flags[num2];
		cFGCell.Flags[9] = Flags[num2 + 6];
		cFGCell.Flags[4] = Flags[num4];
		cFGCell.Flags[10] = Flags[6 + num4];
		cFGCell.Flags[5] = Flags[num3];
		cFGCell.Flags[11] = Flags[6 + num3];
		return cFGCell;
	}

	public static void SpawnCellTempVis(float zaxis, float xaxis, float floorpos, SPData spi)
	{
		if (!spi.cell)
		{
			return;
		}
		UnityEngine.Object @object = null;
		MonoBehaviour monoBehaviour = null;
		Vector3 zero = Vector3.zero;
		if (floorpos > 1f && spi.cell.Editor_IsFlagSet(0, 2) && (bool)spi.m_floor)
		{
			zero = new Vector3(xaxis, floorpos - 0.1f, zaxis);
			monoBehaviour = UnityEngine.Object.Instantiate(spi.m_floor, zero, Quaternion.identity) as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				monoBehaviour.transform.parent = spi.ptrans;
				monoBehaviour.transform.position = spi.ptrans.position + zero;
			}
		}
		if (spi.cell.Editor_IsFlagSet(2, 2))
		{
			zero = new Vector3(xaxis + 0.05f - 0.5f, floorpos + 0.7f, zaxis);
			if (spi.cell.Editor_IsFlagSet(2, 4))
			{
				@object = spi.m_wall_fc;
				zero.y = floorpos + 1.25f;
			}
			else
			{
				@object = spi.m_wall_hc;
			}
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
				}
			}
		}
		if (spi.cell.Editor_IsFlagSet(3, 2))
		{
			zero = new Vector3(xaxis + 0.95f - 0.5f, floorpos + 0.7f, zaxis);
			if (spi.cell.Editor_IsFlagSet(3, 4))
			{
				@object = spi.m_wall_fc;
				zero.y = floorpos + 1.25f;
			}
			else
			{
				@object = spi.m_wall_hc;
			}
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
					monoBehaviour.GetComponent<Renderer>().sharedMaterial.color = Color.white;
				}
			}
		}
		if (spi.cell.Editor_IsFlagSet(4, 2))
		{
			zero = new Vector3(xaxis, floorpos + 0.7f, zaxis + 0.95f - 0.5f);
			if (spi.cell.Editor_IsFlagSet(4, 4))
			{
				@object = spi.m_wall_fc;
				zero.y = floorpos + 1.25f;
			}
			else
			{
				@object = spi.m_wall_hc;
			}
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
					monoBehaviour.transform.forward = Vector3.left;
				}
			}
		}
		if (!spi.cell.Editor_IsFlagSet(5, 2))
		{
			return;
		}
		zero = new Vector3(xaxis, floorpos + 0.7f, zaxis + 0.05f - 0.5f);
		if (spi.cell.Editor_IsFlagSet(5, 4))
		{
			@object = spi.m_wall_fc;
			zero.y = floorpos + 1.25f;
		}
		else
		{
			@object = spi.m_wall_hc;
		}
		if ((bool)@object)
		{
			monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				monoBehaviour.transform.parent = spi.ptrans;
				monoBehaviour.transform.position = spi.ptrans.position + zero;
				monoBehaviour.transform.forward = Vector3.left;
			}
		}
	}

	public static void SpawnCellDebugVis(float zaxis, float xaxis, float floorpos, SPData spi, byte flagtocheck)
	{
		if (!spi.cell)
		{
			return;
		}
		UnityEngine.Object @object = null;
		MonoBehaviour monoBehaviour = null;
		Vector3 zero = Vector3.zero;
		byte b = 0;
		if (floorpos > 1f && spi.cell.CheckFlag((byte)(0 + b), 2) && (bool)spi.m_floor)
		{
			zero = new Vector3(xaxis, floorpos - 0.1f, zaxis);
			monoBehaviour = UnityEngine.Object.Instantiate(spi.m_floor, zero, Quaternion.identity) as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				monoBehaviour.transform.parent = spi.ptrans;
				monoBehaviour.transform.position = spi.ptrans.position + zero;
			}
		}
		if (spi.cell.CheckFlag((byte)(2 + b), flagtocheck))
		{
			zero = new Vector3(xaxis + 0.05f - 0.5f, floorpos + 0.7f, zaxis);
			@object = spi.m_wall_fc;
			zero.y = floorpos + 1.25f;
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
				}
			}
		}
		if (spi.cell.CheckFlag((byte)(3 + b), flagtocheck))
		{
			zero = new Vector3(xaxis + 0.95f - 0.5f, floorpos + 0.7f, zaxis);
			@object = spi.m_wall_fc;
			zero.y = floorpos + 1.25f;
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
					monoBehaviour.GetComponent<Renderer>().sharedMaterial.color = Color.white;
				}
			}
		}
		if (spi.cell.CheckFlag((byte)(4 + b), flagtocheck))
		{
			zero = new Vector3(xaxis, floorpos + 0.7f, zaxis + 0.95f - 0.5f);
			@object = spi.m_wall_fc;
			zero.y = floorpos + 1.25f;
			if ((bool)@object)
			{
				monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
				if ((bool)monoBehaviour)
				{
					monoBehaviour.transform.parent = spi.ptrans;
					monoBehaviour.transform.position = spi.ptrans.position + zero;
					monoBehaviour.transform.forward = Vector3.left;
				}
			}
		}
		if (!spi.cell.CheckFlag((byte)(5 + b), flagtocheck))
		{
			return;
		}
		zero = new Vector3(xaxis, floorpos + 0.7f, zaxis + 0.05f - 0.5f);
		@object = spi.m_wall_fc;
		zero.y = floorpos + 1.25f;
		if ((bool)@object)
		{
			monoBehaviour = UnityEngine.Object.Instantiate(@object, zero, Quaternion.identity) as MonoBehaviour;
			if ((bool)monoBehaviour)
			{
				monoBehaviour.transform.parent = spi.ptrans;
				monoBehaviour.transform.position = spi.ptrans.position + zero;
				monoBehaviour.transform.forward = Vector3.left;
			}
		}
	}

	public void CharacterReEnter()
	{
		if (!(m_Character == null))
		{
			if (CheckFlag(0, 64) && (bool)m_TriggerObject)
			{
				m_TriggerObject.CallEvent_OnEnterTrigger(m_Character);
			}
			if (CheckFlag(0, 64) && (bool)m_TriggerObject)
			{
				m_TriggerObject.CallEvent_OnStopOnTrigger(m_Character);
			}
		}
	}

	public void CharacterEnter(CFGCharacter Char, CFGCell lastCell)
	{
		m_MovingChar = Char;
		if ((bool)Char)
		{
			Char.m_CurrentCellObject = OwnerObject;
			Char.m_CurrentCellObjectInside = m_OwnerInterionObject;
			if (CheckFlag(0, 64) && (bool)m_TriggerObject && (lastCell == null || lastCell.m_TriggerObject != m_TriggerObject || !lastCell.CheckFlag(0, 64)))
			{
				m_TriggerObject.CallEvent_OnEnterTrigger(Char);
			}
			Char.RecalculateSelfShadow();
		}
	}

	public void CharacterLeave(CFGCharacter Char, CFGCell nextCell)
	{
		if (Char != m_Character && Char != m_MovingChar)
		{
			return;
		}
		if (Char != null)
		{
			Char.m_CurrentCellObject = null;
			Char.m_CurrentCellObjectInside = null;
			if (CheckFlag(0, 64) && (bool)m_TriggerObject && Char.IsAlive && (nextCell == null || nextCell.m_TriggerObject != m_TriggerObject || !nextCell.CheckFlag(0, 64)))
			{
				m_TriggerObject.CallEvent_OnLeaveTrigger(Char);
			}
			Char.RecalculateSelfShadow();
		}
		if (m_Character == Char)
		{
			m_Character = null;
		}
		if (m_MovingChar == Char)
		{
			m_MovingChar = null;
		}
	}

	public void CharacterMoveEnter(CFGCharacter Char)
	{
	}

	public void CharacterMoveEnd(CFGCharacter Char)
	{
		if (!(Char == null))
		{
			if (CheckFlag(0, 64) && (bool)m_TriggerObject)
			{
				m_TriggerObject.CallEvent_OnStopOnTrigger(Char);
			}
			m_Character = Char;
		}
	}

	public void SetCurrentChar(CFGCharacter Char)
	{
		m_Character = Char;
	}

	public bool CanStandOnThisTile(bool can_stand_now)
	{
		if (can_stand_now && m_Character != null)
		{
			return false;
		}
		if (!CheckFlag(0, 2) && !CheckFlag(1, 64))
		{
			return false;
		}
		if (CheckFlag(1, 8))
		{
			return false;
		}
		return true;
	}

	public void GetBestCoverToGlue(CFGCharacter Character, out int cover_type, out EDirection dir)
	{
		CFGCell cFGCell = FindNeighbour(EDirection.NORTH);
		CFGCell cFGCell2 = FindNeighbour(EDirection.EAST);
		CFGCell cFGCell3 = FindNeighbour(EDirection.SOUTH);
		CFGCell cFGCell4 = FindNeighbour(EDirection.WEST);
		ECoverType borderCover = GetBorderCover(EDirection.NORTH);
		ECoverType borderCover2 = GetBorderCover(EDirection.EAST);
		ECoverType borderCover3 = GetBorderCover(EDirection.SOUTH);
		ECoverType borderCover4 = GetBorderCover(EDirection.WEST);
		EBestCoverRotation bestCoverRotation = Character.GetBestCoverRotation();
		if (bestCoverRotation == EBestCoverRotation.Forward)
		{
			cover_type = 0;
			dir = Character.GetBestCoverDirection();
			return;
		}
		if (Character != null && bestCoverRotation != 0 && bestCoverRotation != EBestCoverRotation.Forward && (borderCover == ECoverType.FULL || borderCover2 == ECoverType.FULL || borderCover3 == ECoverType.FULL || borderCover4 == ECoverType.FULL))
		{
			cover_type = 0;
			switch (bestCoverRotation)
			{
			case EBestCoverRotation.Left:
				cover_type = 3;
				break;
			case EBestCoverRotation.Right:
				cover_type = 2;
				break;
			}
			dir = Character.GetBestCoverDirection();
			return;
		}
		EDirection eDirection = EDirection.EAST;
		bool flag = false;
		if (Character != null && bestCoverRotation == EBestCoverRotation.Auto)
		{
			CFGCharacter cFGCharacter = Character.FindClosestVisibleEnemy(out dir);
			if ((bool)cFGCharacter)
			{
				switch (GetBorderCover(dir))
				{
				case ECoverType.NONE:
					if (borderCover3 == ECoverType.NONE && borderCover == ECoverType.NONE && borderCover2 == ECoverType.NONE && borderCover4 == ECoverType.NONE)
					{
						cover_type = 0;
						return;
					}
					break;
				case ECoverType.HALF:
					cover_type = 1;
					return;
				case ECoverType.FULL:
					flag = true;
					eDirection = dir;
					break;
				}
			}
		}
		bool flag2 = false;
		bool flag3 = false;
		if ((!flag && borderCover == ECoverType.FULL) || (flag && eDirection == EDirection.NORTH))
		{
			if (borderCover4 == ECoverType.NONE && cFGCell4 != null && cFGCell4.CanStandOnThisTile(can_stand_now: true) && cFGCell4.GetBorderCover(EDirection.NORTH) != ECoverType.FULL)
			{
				flag2 = true;
			}
			if (borderCover2 == ECoverType.NONE && cFGCell2 != null && cFGCell2.CanStandOnThisTile(can_stand_now: true) && cFGCell2.GetBorderCover(EDirection.NORTH) != ECoverType.FULL)
			{
				flag3 = true;
			}
			if (flag2 || flag3)
			{
				dir = EDirection.SOUTH;
				if (flag2)
				{
					cover_type = 2;
				}
				else
				{
					cover_type = 3;
				}
				return;
			}
		}
		flag2 = false;
		flag3 = false;
		if ((!flag && borderCover2 == ECoverType.FULL) || (flag && eDirection == EDirection.EAST))
		{
			if (borderCover == ECoverType.NONE && cFGCell != null && cFGCell.CanStandOnThisTile(can_stand_now: true) && cFGCell.GetBorderCover(EDirection.EAST) != ECoverType.FULL)
			{
				flag2 = true;
			}
			if (borderCover3 == ECoverType.NONE && cFGCell3 != null && cFGCell3.CanStandOnThisTile(can_stand_now: true) && cFGCell3.GetBorderCover(EDirection.EAST) != ECoverType.FULL)
			{
				flag3 = true;
			}
			if (flag2 || flag3)
			{
				dir = EDirection.WEST;
				if (flag2)
				{
					cover_type = 2;
				}
				else
				{
					cover_type = 3;
				}
				return;
			}
		}
		flag2 = false;
		flag3 = false;
		if ((!flag && borderCover3 == ECoverType.FULL) || (flag && eDirection == EDirection.SOUTH))
		{
			if (borderCover2 == ECoverType.NONE && cFGCell2 != null && cFGCell2.CanStandOnThisTile(can_stand_now: true) && cFGCell2.GetBorderCover(EDirection.SOUTH) != ECoverType.FULL)
			{
				flag2 = true;
			}
			if (borderCover4 == ECoverType.NONE && cFGCell4 != null && cFGCell4.CanStandOnThisTile(can_stand_now: true) && cFGCell4.GetBorderCover(EDirection.SOUTH) != ECoverType.FULL)
			{
				flag3 = true;
			}
			if (flag2 || flag3)
			{
				dir = EDirection.NORTH;
				if (flag2)
				{
					cover_type = 2;
				}
				else
				{
					cover_type = 3;
				}
				return;
			}
		}
		flag2 = false;
		flag3 = false;
		if ((!flag && borderCover4 == ECoverType.FULL) || (flag && eDirection == EDirection.WEST))
		{
			if (borderCover3 == ECoverType.NONE && cFGCell3 != null && cFGCell3.CanStandOnThisTile(can_stand_now: true) && cFGCell3.GetBorderCover(EDirection.WEST) != ECoverType.FULL)
			{
				flag2 = true;
			}
			if (borderCover == ECoverType.NONE && cFGCell != null && cFGCell.CanStandOnThisTile(can_stand_now: true) && cFGCell.GetBorderCover(EDirection.WEST) != ECoverType.FULL)
			{
				flag3 = true;
			}
			if (flag2 || flag3)
			{
				dir = EDirection.EAST;
				if (flag2)
				{
					cover_type = 2;
				}
				else
				{
					cover_type = 3;
				}
				return;
			}
		}
		if (borderCover == ECoverType.HALF)
		{
			cover_type = 1;
			dir = EDirection.NORTH;
		}
		else if (borderCover2 == ECoverType.HALF)
		{
			cover_type = 1;
			dir = EDirection.EAST;
		}
		else if (borderCover3 == ECoverType.HALF)
		{
			cover_type = 1;
			dir = EDirection.SOUTH;
		}
		else if (borderCover4 == ECoverType.HALF)
		{
			cover_type = 1;
			dir = EDirection.WEST;
		}
		else
		{
			cover_type = 0;
			dir = EDirection.NORTH;
		}
	}

	public bool CanBeSeenFromDirection(EDirection FromDir)
	{
		if (CheckFlag(1, 16))
		{
			return false;
		}
		byte flagType = 2;
		switch (FromDir)
		{
		case EDirection.EAST:
			flagType = 4;
			break;
		case EDirection.SOUTH:
			flagType = 3;
			break;
		case EDirection.WEST:
			flagType = 5;
			break;
		}
		return CheckFlag(flagType, 16);
	}

	public List<CFGCell> FindNeighbours(bool exclude_occupied = true)
	{
		List<CFGCell> outList = new List<CFGCell>();
		DecodePosition(out var PosX, out var PosZ, out var Floor);
		bool bCheckF = true;
		bool bCheckF2 = true;
		bool bCheckF3 = true;
		bool bCheckF4 = true;
		CFGCell cFGCell = null;
		int num = 0;
		if (CheckFlag(0, 64) && (bool)OwnerObject)
		{
			num = OwnerObject.Cost2Move_Trigger;
		}
		if (CheckFlag(1, 64))
		{
			cFGCell = CFGCellMap.GetCell(PosZ, PosX, Floor + 1);
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied) && !cFGCell.HaveFloor)
			{
				if ((bool)OwnerObject)
				{
					cFGCell.CostToMove = OwnerObject.Cost2Move_Vertical + num;
				}
				else
				{
					cFGCell.CostToMove = 20 + num;
				}
				outList.Add(cFGCell);
			}
			if (!HaveFloor && Floor > 0)
			{
				cFGCell = CFGCellMap.GetCell(PosZ, PosX, Floor - 1);
				if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
				{
					if ((bool)OwnerObject)
					{
						cFGCell.CostToMove = OwnerObject.Cost2Move_Vertical + num;
					}
					else
					{
						cFGCell.CostToMove = 20 + num;
					}
					outList.Add(cFGCell);
				}
			}
		}
		EStairsType stairsType = StairsType;
		HandleStdDir(PosX, PosZ + 1, Floor, stairsType, ref outList, 4, 5, 2, ref bCheckF, 3, ref bCheckF2, exclude_occupied);
		HandleStdDir(PosX, PosZ - 1, Floor, stairsType, ref outList, 5, 4, 2, ref bCheckF3, 3, ref bCheckF4, exclude_occupied);
		HandleStdDir(PosX - 1, PosZ, Floor, stairsType, ref outList, 2, 3, 4, ref bCheckF, 5, ref bCheckF3, exclude_occupied);
		HandleStdDir(PosX + 1, PosZ, Floor, stairsType, ref outList, 3, 2, 4, ref bCheckF2, 5, ref bCheckF4, exclude_occupied);
		if (bCheckF && !HasColumn(EColumn.NorthEast))
		{
			cFGCell = CFGCellMap.GetCell(PosZ + 1, PosX - 1, Floor);
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 15 + num;
				outList.Add(cFGCell);
			}
		}
		if (bCheckF3 && !HasColumn(EColumn.NorthWest))
		{
			cFGCell = CFGCellMap.GetCell(PosZ - 1, PosX - 1, Floor);
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 15 + num;
				outList.Add(cFGCell);
			}
		}
		if (bCheckF2 && !HasColumn(EColumn.SouthEast))
		{
			cFGCell = CFGCellMap.GetCell(PosZ + 1, PosX + 1, Floor);
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 15 + num;
				outList.Add(cFGCell);
			}
		}
		if (bCheckF4 && !HasColumn(EColumn.SouthWest))
		{
			cFGCell = CFGCellMap.GetCell(PosZ - 1, PosX + 1, Floor);
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 15 + num;
				outList.Add(cFGCell);
			}
		}
		return outList;
	}

	private void HandleStdDir(int px, int pz, int f, EStairsType nfatype, ref List<CFGCell> outList, byte This2OtherFlag, byte Other2ThisFlag, byte CF1, ref bool bCheckF1, byte CF2, ref bool bCheckF2, bool exclude_occupied)
	{
		CFGCell cFGCell = null;
		CFGDoorObject cFGDoorObject = null;
		int num = 0;
		if (CheckFlag(0, 64) && (bool)TriggerObject)
		{
			num = TriggerObject.Cost2Move_Trigger;
		}
		bool flag = !CheckFlag(This2OtherFlag, 8);
		if (!flag)
		{
			bCheckF1 = false;
			bCheckF2 = false;
			if (CheckFlag(This2OtherFlag, 64) && (bool)OwnerObject && (bool)OwnerObject.m_CurrentVisualisation)
			{
				cFGDoorObject = OwnerObject.m_CurrentVisualisation.GetComponent<CFGDoorObject>();
				if ((bool)cFGDoorObject && cFGDoorObject.CanOpen && !cFGDoorObject.BlockForPathfinding)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			cFGCell = CFGCellMap.GetCell(pz, px, f);
			if ((bool)cFGCell)
			{
				switch (nfatype)
				{
				case EStairsType.None:
					if (cFGCell.StairsType == EStairsType.Slope)
					{
						cFGCell = null;
					}
					break;
				case EStairsType.EntryPoint:
					if (cFGCell.StairsType == EStairsType.ExitPoint)
					{
						cFGCell = null;
					}
					break;
				case EStairsType.ExitPoint:
					if (cFGCell.StairsType == EStairsType.Slope || cFGCell.StairsType == EStairsType.EntryPoint)
					{
						cFGCell = null;
					}
					break;
				case EStairsType.Slope:
					if (cFGCell.StairsType == EStairsType.ExitPoint || cFGCell.StairsType == EStairsType.None)
					{
						cFGCell = null;
					}
					break;
				}
			}
			if ((bool)cFGCell && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 10 + num;
				outList.Add(cFGCell);
				if (bCheckF1)
				{
					bool flag2 = (cFGCell.Flags[CF1] & 8) == 8;
					if (flag2 && cFGCell.CheckFlag(CF1, 64) && (bool)cFGCell.OwnerObject && (bool)cFGCell.OwnerObject.m_CurrentVisualisation)
					{
						cFGDoorObject = cFGCell.OwnerObject.m_CurrentVisualisation.GetComponent<CFGDoorObject>();
						if (!cFGDoorObject || (!cFGDoorObject.CanOpen && !cFGDoorObject.BlockForPathfinding))
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						bCheckF1 = false;
					}
				}
				if (bCheckF2)
				{
					bool flag3 = (cFGCell.Flags[CF2] & 8) == 8;
					if (flag3 && cFGCell.CheckFlag(CF2, 64) && (bool)cFGCell.OwnerObject && (bool)cFGCell.OwnerObject.m_CurrentVisualisation)
					{
						cFGDoorObject = cFGCell.OwnerObject.m_CurrentVisualisation.GetComponent<CFGDoorObject>();
						if (!cFGDoorObject || (!cFGDoorObject.CanOpen && !cFGDoorObject.BlockForPathfinding))
						{
							flag3 = false;
						}
					}
					if (flag3)
					{
						bCheckF2 = false;
					}
				}
			}
			else
			{
				bCheckF1 = false;
				bCheckF2 = false;
			}
			if (nfatype == EStairsType.ExitPoint && f > 0)
			{
				cFGCell = CFGCellMap.GetCell(pz, px, f - 1);
				if ((bool)cFGCell && cFGCell.StairsType == EStairsType.Slope && cFGCell.CanStandOnThisTile(exclude_occupied))
				{
					cFGCell.CostToMove = 15 + num;
					outList.Add(cFGCell);
				}
			}
		}
		else
		{
			bCheckF1 = false;
			bCheckF2 = false;
		}
		if (nfatype == EStairsType.Slope)
		{
			cFGCell = CFGCellMap.GetCell(pz, px, f + 1);
			if ((bool)cFGCell && cFGCell.StairsType == EStairsType.ExitPoint && cFGCell.CanStandOnThisTile(exclude_occupied))
			{
				cFGCell.CostToMove = 15 + num;
				outList.Add(cFGCell);
			}
		}
	}

	public override string ToString()
	{
		DecodePosition(out var PosX, out var PosZ, out var Floor);
		return "Cell (X:" + PosX + ", Z:" + PosZ + ") Floor: " + Floor;
	}

	public Vector3 GetToLadderDirection()
	{
		if (!CheckFlag(1, 64))
		{
			return CFGMath.INIFITY3;
		}
		if (CheckFlag(2, 64))
		{
			return new Vector3(-1f, 0f, 0f);
		}
		if (CheckFlag(5, 64))
		{
			return new Vector3(0f, 0f, -1f);
		}
		if (CheckFlag(3, 64))
		{
			return new Vector3(1f, 0f, 0f);
		}
		if (CheckFlag(4, 64))
		{
			return new Vector3(0f, 0f, 1f);
		}
		return CFGMath.INIFITY3;
	}

	public bool CheckForActivators(bool bCheckCenter = false)
	{
		if (bCheckCenter && CheckFlag(1, 64))
		{
			return true;
		}
		if (CheckFlag(2, 64))
		{
			return true;
		}
		if (CheckFlag(5, 64))
		{
			return true;
		}
		if (CheckFlag(3, 64))
		{
			return true;
		}
		if (CheckFlag(4, 64))
		{
			return true;
		}
		return false;
	}

	public bool HasDoorOnBorder(EDirection dir)
	{
		if (OwnerObject == null || !OwnerObject.IsDoor)
		{
			return false;
		}
		if (CheckFlag(1, 64))
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
		return CheckFlag(flagType, 64);
	}

	public bool CheckIfInLight()
	{
		m_bIsInLight = false;
		if (OwnerObject != null && OwnerObject.IsLightForced && CheckFlag(0, 32))
		{
			m_bIsInLight = true;
		}
		else
		{
			int layerMask = 524288;
			Vector3 worldPosition = WorldPosition;
			worldPosition.y += 1.25f;
			bool bIsInLight = false;
			if (Physics.CheckSphere(worldPosition, 0.1f, layerMask))
			{
				bIsInLight = true;
			}
			m_bIsInLight = bIsInLight;
		}
		return m_bIsInLight;
	}

	private void PropagateFlag(ECMPFlags flag, CFGCell nc, CFGCell sc, CFGCell wc, CFGCell ec)
	{
		byte b = (byte)flag;
		if (Editor_IsFlagSet(1, b))
		{
			Flags[2] |= b;
			if ((bool)nc)
			{
				nc.Flags[3] |= b;
			}
			Flags[3] |= b;
			if ((bool)sc)
			{
				sc.Flags[2] |= b;
			}
			Flags[4] |= b;
			if ((bool)ec)
			{
				ec.Flags[5] |= b;
			}
			Flags[5] |= b;
			if ((bool)wc)
			{
				wc.Flags[4] |= b;
			}
		}
		if (Editor_IsFlagSet(7, b))
		{
			Flags[8] |= b;
			if ((bool)nc)
			{
				nc.Flags[9] |= b;
			}
			Flags[9] |= b;
			if ((bool)sc)
			{
				sc.Flags[8] |= b;
			}
			Flags[10] |= b;
			if ((bool)ec)
			{
				ec.Flags[11] |= b;
			}
			Flags[11] |= b;
			if ((bool)wc)
			{
				wc.Flags[10] |= b;
			}
		}
	}

	public void PropagateCenterCover()
	{
		CFGCell cell = CFGCellMap.GetCell(WorldPosition + new Vector3(-1f, 0f, 0f));
		CFGCell cell2 = CFGCellMap.GetCell(WorldPosition + new Vector3(1f, 0f, 0f));
		CFGCell cell3 = CFGCellMap.GetCell(WorldPosition + new Vector3(0f, 0f, -1f));
		CFGCell cell4 = CFGCellMap.GetCell(WorldPosition + new Vector3(0f, 0f, 1f));
		PropagateFlag(ECMPFlags.IsCover, cell, cell2, cell3, cell4);
		PropagateFlag(ECMPFlags.IsFullCover, cell, cell2, cell3, cell4);
		PropagateFlag(ECMPFlags.BlockSight, cell, cell2, cell3, cell4);
		PropagateFlag(ECMPFlags.StopBullet, cell, cell2, cell3, cell4);
	}

	public static implicit operator bool(CFGCell Exists)
	{
		return Exists != null;
	}
}
