using System.Collections.Generic;
using UnityEngine;

public class CFGUsableObject : CFGGameObject
{
	public delegate void OnUseDelegate(CFGUsableObject usable_obj, CFGCharacter Char);

	[SerializeField]
	private bool m_OneTime;

	private bool m_bCoverApplied;

	[SerializeField]
	private bool m_ApplyAlternateState;

	[SerializeField]
	private EDynamicCoverType m_DynamicCoverType = EDynamicCoverType.NONE;

	public CFGSoundDef m_OnUseSoundDef;

	[SerializeField]
	private Transform m_OnUseFX;

	private bool m_Used;

	private ERotation m_Rotation;

	private bool m_bEnabled = true;

	private List<Transform> m_ActivatorHelpers = new List<Transform>();

	[CFGFlowCode(Category = "Objects", Title = "On Try Use")]
	public OnUseDelegate m_OnTryUseCallback;

	public bool OneTime => m_OneTime;

	public bool Used
	{
		get
		{
			return m_Used;
		}
		private set
		{
			m_Used = value;
			if (m_OneTime && (bool)GetComponent<Collider>())
			{
				GetComponent<Collider>().enabled = !m_Used;
			}
		}
	}

	public bool IsDynamicCover => m_ApplyAlternateState;

	public EDynamicCoverType DynamicCoverType => m_DynamicCoverType;

	public override ESerializableType SerializableType => ESerializableType.Usable;

	public override bool NeedsSaving => true;

	public bool CanBeUsed()
	{
		if (!m_bEnabled)
		{
			return false;
		}
		return !OneTime || !Used;
	}

	public override void OnCursorEnter()
	{
		base.OnCursorEnter();
		if (m_ActivatorHelpers.Count > 0)
		{
			foreach (Transform activatorHelper in m_ActivatorHelpers)
			{
				Object.Destroy(activatorHelper.gameObject);
			}
			m_ActivatorHelpers.Clear();
		}
		if (!CFGRangeBorders.s_DrawRangeBorders)
		{
			return;
		}
		List<CFGCell> activatorCells = GetActivatorCells();
		CFGCharacter selectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
		if (selectedCharacter == null)
		{
			return;
		}
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(selectedCharacter.CurrentCell, (selectedCharacter.ActionPoints != 2) ? selectedCharacter.HalfMovement : selectedCharacter.DashingMovement);
		LinkedList<CFGCell> bestPathTo = GetBestPathTo(selectedCharacter);
		CFGCell cFGCell = null;
		if (bestPathTo != null && bestPathTo.Count >= 1)
		{
			cFGCell = bestPathTo.Last.Value;
		}
		foreach (CFGCell item in activatorCells)
		{
			if (item.HaveFloor)
			{
				Transform transform = ((item == cFGCell) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_BestCellActivatorVisPrefab : ((!hashSet.Contains(item)) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorInactiveVisPrefab : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorVisPrefab));
				if (!(transform == null))
				{
					int Floor = 0;
					int PosX = 0;
					int PosZ = 0;
					item.DecodePosition(out PosX, out PosZ, out Floor);
					Vector3 position = new Vector3((float)PosX + 0.5f, (float)Floor * 2.5f, (float)PosZ + 0.5f);
					Transform transform2 = Object.Instantiate(transform);
					transform2.position = position;
					m_ActivatorHelpers.Add(transform2);
				}
			}
		}
	}

	public override void OnCursorLeave()
	{
		foreach (Transform activatorHelper in m_ActivatorHelpers)
		{
			Object.Destroy(activatorHelper.gameObject);
		}
		m_ActivatorHelpers.Clear();
	}

	public void SetEnabled(bool bEnabled)
	{
		m_bEnabled = bEnabled;
	}

	public void SetEnabledWithCollider(bool bEnabled)
	{
		m_bEnabled = bEnabled;
		Collider component = GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = bEnabled;
		}
	}

	public void TryUse(CFGCharacter UserChar)
	{
		Debug.Log("Usable: TryUse");
		if (CanBeUsed())
		{
			if (m_OnTryUseCallback != null)
			{
				m_OnTryUseCallback(this, UserChar);
			}
			else if ((bool)UserChar)
			{
				UserChar.MakeAction(ETurnAction.Use, this);
			}
		}
	}

	public void DoUse(CFGCharacter UserChar)
	{
		Debug.Log("Usable: DoUse");
		if (!CanBeUsed())
		{
			return;
		}
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null && base.transform.parent != null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
		}
		if (component == null)
		{
			return;
		}
		Animation component2 = GetComponent<Animation>();
		if (component2 == null)
		{
			CFGCellObjectPart componentInChildren = base.transform.GetComponentInChildren<CFGCellObjectPart>();
			if (componentInChildren != null && (bool)componentInChildren.GetComponent<Animation>())
			{
				component2 = componentInChildren.GetComponent<Animation>();
			}
		}
		m_Rotation = ERotation.None;
		if (UserChar != null)
		{
			m_Rotation = CalculateUserRotation(GetPlayerFacing(UserChar), component.m_CoverMirror, component.Rotation);
		}
		if (component2 != null)
		{
			if (component.m_CoverMirror != 0)
			{
				switch (m_Rotation)
				{
				case ERotation.None:
					component2.clip = component2.GetClip("cover_serving_trolley_a_anim_E");
					break;
				case ERotation.CW_90:
					component2.clip = component2.GetClip("cover_serving_trolley_a_anim_S");
					break;
				case ERotation.CW_180:
					component2.clip = component2.GetClip("cover_serving_trolley_a_anim_W");
					break;
				case ERotation.CW_270:
					component2.clip = component2.GetClip("cover_serving_trolley_a_anim_N");
					break;
				}
			}
			component2.Play();
		}
		CFGSoundDef.Play(m_OnUseSoundDef, base.Transform.position);
		if (m_OnUseFX != null)
		{
			Object.Instantiate(m_OnUseFX, base.transform.position, base.transform.rotation);
		}
		Used = !Used;
		ApplyGrid(m_Rotation);
	}

	protected override void Start()
	{
		base.Start();
		if (CFGOptions.Gameplay.InteractiveObjectsGlow >= 2)
		{
			base.gameObject.AddComponent<CFGInteractiveObjVis>();
		}
	}

	public static ERotation CalculateUserRotation(EDirection PlayerFacing, ECoverMirroring MirroringType, ERotation ObjectRotation)
	{
		switch (MirroringType)
		{
		case ECoverMirroring.NoMirroring:
			return ERotation.None;
		case ECoverMirroring.MirrorWestToEast:
			switch (ObjectRotation)
			{
			case ERotation.None:
				if (PlayerFacing == EDirection.EAST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_90:
				if (PlayerFacing == EDirection.SOUTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_180:
				if (PlayerFacing == EDirection.WEST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_270:
				if (PlayerFacing == EDirection.NORTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			default:
				return ERotation.None;
			}
		case ECoverMirroring.MirrorEastToWest:
			switch (ObjectRotation)
			{
			case ERotation.None:
				if (PlayerFacing == EDirection.WEST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_90:
				if (PlayerFacing == EDirection.NORTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_180:
				if (PlayerFacing == EDirection.EAST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_270:
				if (PlayerFacing == EDirection.SOUTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			default:
				return ERotation.None;
			}
		case ECoverMirroring.MirrorSouthToNorth:
			switch (ObjectRotation)
			{
			case ERotation.None:
				if (PlayerFacing == EDirection.NORTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_90:
				if (PlayerFacing == EDirection.EAST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_180:
				if (PlayerFacing == EDirection.SOUTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_270:
				if (PlayerFacing == EDirection.WEST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			default:
				return ERotation.None;
			}
		case ECoverMirroring.MirrorNorthToSouth:
			switch (ObjectRotation)
			{
			case ERotation.None:
				if (PlayerFacing == EDirection.SOUTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_90:
				if (PlayerFacing == EDirection.WEST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_180:
				if (PlayerFacing == EDirection.NORTH)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			case ERotation.CW_270:
				if (PlayerFacing == EDirection.EAST)
				{
					return ERotation.CW_180;
				}
				return ERotation.None;
			default:
				return ERotation.None;
			}
		case ECoverMirroring.FaceNorthSide:
			return ObjectRotation switch
			{
				ERotation.None => PlayerFacing switch
				{
					EDirection.NORTH => ERotation.None, 
					EDirection.SOUTH => ERotation.CW_180, 
					EDirection.EAST => ERotation.CW_90, 
					EDirection.WEST => ERotation.CW_270, 
					_ => ERotation.None, 
				}, 
				ERotation.CW_90 => PlayerFacing switch
				{
					EDirection.NORTH => ERotation.CW_270, 
					EDirection.SOUTH => ERotation.CW_90, 
					EDirection.EAST => ERotation.None, 
					EDirection.WEST => ERotation.CW_180, 
					_ => ERotation.None, 
				}, 
				ERotation.CW_180 => PlayerFacing switch
				{
					EDirection.NORTH => ERotation.CW_180, 
					EDirection.SOUTH => ERotation.None, 
					EDirection.EAST => ERotation.CW_270, 
					EDirection.WEST => ERotation.CW_90, 
					_ => ERotation.None, 
				}, 
				ERotation.CW_270 => PlayerFacing switch
				{
					EDirection.NORTH => ERotation.CW_90, 
					EDirection.SOUTH => ERotation.CW_270, 
					EDirection.EAST => ERotation.CW_180, 
					EDirection.WEST => ERotation.None, 
					_ => ERotation.None, 
				}, 
				_ => ERotation.None, 
			};
		default:
			return ERotation.None;
		}
	}

	private void ApplyGrid(ERotation UserRotation)
	{
		if (m_bCoverApplied || !m_ApplyAlternateState)
		{
			return;
		}
		m_bCoverApplied = true;
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null && base.transform.parent != null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
		}
		if (component == null)
		{
			Debug.Log("Cannot apply alternate cover state!");
			return;
		}
		Debug.Log("Apply alternate cover state!");
		CFGCellMap.ApplySingleObject(component, bAlternate: true, bHandleVis: true, UserRotation);
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if (component2 != null)
		{
			component2.OnGridChanged();
		}
	}

	public CFGCell CanCharacterUse(CFGCharacter toon, bool bCheckWithMovement, out EUseMode Mode)
	{
		Mode = EUseMode.CantUse;
		if (!CanBeUsed())
		{
			return null;
		}
		CFGCell cFGCell = null;
		CFGCell cFGCell2 = null;
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
		}
		toon.CurrentCell.DecodePosition(out var PosX, out var PosZ, out var Floor);
		cFGCell2 = CFGCellMap.GetCell(PosZ + 1, PosX, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == component && cFGCell2.CheckFlag(5, 64))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ - 1, PosX, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == component && cFGCell2.CheckFlag(4, 64))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ, PosX + 1, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == component && cFGCell2.CheckFlag(2, 64))
		{
			cFGCell = cFGCell2;
		}
		cFGCell2 = CFGCellMap.GetCell(PosZ, PosX - 1, Floor);
		if ((bool)cFGCell2 && cFGCell2.OwnerObject == component && cFGCell2.CheckFlag(3, 64))
		{
			cFGCell = cFGCell2;
		}
		if (cFGCell == null && bCheckWithMovement)
		{
			LinkedList<CFGCell> bestPathTo = GetBestPathTo(toon);
			if (bestPathTo != null)
			{
				cFGCell = bestPathTo.Last.Value;
				if (cFGCell != null)
				{
					if (cFGCell.EP == toon.CurrentCell.EP)
					{
						Mode = EUseMode.CanUse;
					}
					else
					{
						Mode = EUseMode.MustWalk;
					}
				}
			}
		}
		else
		{
			Mode = EUseMode.CanUse;
		}
		return cFGCell;
	}

	public LinkedList<CFGCell> GetBestPathTo(CFGCharacter Dude)
	{
		LinkedList<CFGCell> result = null;
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(Dude.CurrentCell, (Dude.ActionPoints != 2) ? Dude.HalfMovement : Dude.DashingMovement);
		List<CFGCell> activatorCells = GetActivatorCells();
		NavigationComponent component = Dude.GetComponent<NavigationComponent>();
		if (component == null)
		{
			return null;
		}
		int num = 100000;
		float num2 = 100000f;
		if (activatorCells != null && activatorCells.Count > 0)
		{
			foreach (CFGCell item in activatorCells)
			{
				if (!hashSet.Contains(item))
				{
					continue;
				}
				NavGoal_At navGoal_At = new NavGoal_At(item);
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

	private EDirection GetPlayerFacing(CFGCharacter UserChar)
	{
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
		}
		UserChar.CurrentCell.DecodePosition(out var PosX, out var PosZ, out var Floor);
		CFGCell cell = CFGCellMap.GetCell(PosZ + 1, PosX, Floor);
		if ((bool)cell && cell.OwnerObject == component && cell.CheckFlag(5, 64))
		{
			return EDirection.WEST;
		}
		cell = CFGCellMap.GetCell(PosZ - 1, PosX, Floor);
		if ((bool)cell && cell.OwnerObject == component && cell.CheckFlag(4, 64))
		{
			return EDirection.EAST;
		}
		cell = CFGCellMap.GetCell(PosZ, PosX + 1, Floor);
		if ((bool)cell && cell.OwnerObject == component && cell.CheckFlag(2, 64))
		{
			return EDirection.NORTH;
		}
		cell = CFGCellMap.GetCell(PosZ, PosX - 1, Floor);
		if ((bool)cell && cell.OwnerObject == component && cell.CheckFlag(3, 64))
		{
			return EDirection.SOUTH;
		}
		return EDirection.NORTH;
	}

	public List<CFGCell> GetActivatorCells()
	{
		CFGCellObject component = GetComponent<CFGCellObject>();
		if (component == null && base.transform.parent != null)
		{
			component = base.transform.parent.GetComponent<CFGCellObject>();
			if (component == null)
			{
				return null;
			}
		}
		return CFGCellMap.GetActivatorCells(component);
	}

	public Vector3 GetTargetForCharacterRotation(CFGCharacter Dude)
	{
		if (Dude == null)
		{
			return base.transform.position;
		}
		Vector3 position = Dude.Position;
		CFGCell cFGCell = null;
		cFGCell = CheckCellForCharRot(position + new Vector3(0f, 0f, 1f));
		if ((bool)cFGCell)
		{
			return cFGCell.WorldPosition;
		}
		cFGCell = CheckCellForCharRot(position + new Vector3(0f, 0f, -1f));
		if ((bool)cFGCell)
		{
			return cFGCell.WorldPosition;
		}
		cFGCell = CheckCellForCharRot(position + new Vector3(-1f, 0f, 0f));
		if ((bool)cFGCell)
		{
			return cFGCell.WorldPosition;
		}
		cFGCell = CheckCellForCharRot(position + new Vector3(1f, 0f, 0f));
		if ((bool)cFGCell)
		{
			return cFGCell.WorldPosition;
		}
		return base.transform.position;
	}

	private CFGCell CheckCellForCharRot(Vector3 Pos)
	{
		CFGCell cell = CFGCellMap.GetCell(Pos);
		if (cell == null || cell.CanStandOnThisTile(can_stand_now: false) || cell.OwnerObject == null || !cell.OwnerObject.IsUsable)
		{
			return null;
		}
		CFGUsableObject usable = cell.OwnerObject.GetUsable();
		return (!(usable == this)) ? null : cell;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Applied", m_bCoverApplied);
		cFG_SG_Node.Attrib_Set("Used", m_Used);
		cFG_SG_Node.Attrib_Set("Active", m_bEnabled);
		cFG_SG_Node.Attrib_Set("Rot", m_Rotation);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		m_bCoverApplied = Node.Attrib_Get("Applied", m_bCoverApplied);
		m_Used = Node.Attrib_Get("Used", m_Used);
		m_Rotation = Node.Attrib_Get("Rot", m_Rotation);
		if (m_Used)
		{
			CFGCellObject component = GetComponent<CFGCellObject>();
			if (component == null && base.transform.parent != null)
			{
				component = base.transform.parent.GetComponent<CFGCellObject>();
			}
			Animation component2 = GetComponent<Animation>();
			if (component2 == null)
			{
				CFGCellObjectPart componentInChildren = base.transform.GetComponentInChildren<CFGCellObjectPart>();
				if (componentInChildren != null && (bool)componentInChildren.GetComponent<Animation>())
				{
					component2 = componentInChildren.GetComponent<Animation>();
				}
			}
			if (component2 != null)
			{
				if (component != null && component.m_CoverMirror != 0)
				{
					switch (m_Rotation)
					{
					case ERotation.None:
						component2.clip = component2.GetClip("cover_serving_trolley_a_anim_E");
						break;
					case ERotation.CW_90:
						component2.clip = component2.GetClip("cover_serving_trolley_a_anim_S");
						break;
					case ERotation.CW_180:
						component2.clip = component2.GetClip("cover_serving_trolley_a_anim_W");
						break;
					case ERotation.CW_270:
						component2.clip = component2.GetClip("cover_serving_trolley_a_anim_N");
						break;
					}
				}
				component2.Play();
				foreach (AnimationState item in component2)
				{
					item.normalizedTime = 1f;
				}
			}
			if (m_OnUseFX != null)
			{
				Object.Instantiate(m_OnUseFX, base.transform.position, base.transform.rotation);
			}
		}
		m_bEnabled = Node.Attrib_Get("Active", m_bEnabled);
		return true;
	}
}
