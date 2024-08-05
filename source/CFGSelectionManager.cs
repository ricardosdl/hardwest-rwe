using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CFGSelectionManager : CFGSingletonBase
{
	private enum ECursorHelper
	{
		None,
		Move,
		MoveInactive,
		MoveVert,
		Shoot,
		ShootInactive,
		Use,
		UseInactive,
		CreateCover,
		CreateCoverInactive,
		Gunpoint,
		GunpointInactive,
		Select,
		OpenDoor,
		OpenDoorInactive,
		OpenDoorLocked,
		OpenDoorGamepad,
		ShadowKill,
		ShadowKillInactive,
		Penetrate,
		PenetrateInactive,
		Transfusion,
		TransfusionInactive,
		ArteryShot,
		ArteryShotInactive,
		MoveStairsUp,
		MoveStairsDown,
		Throwable,
		ThrowableInactive
	}

	private enum m_StrategicCursors
	{
		Normal,
		Inaccesible,
		Entrance
	}

	public enum EControllerMode
	{
		Unknown = 0,
		Global_Tactical = 1,
		Global_Strategic = 2,
		Target_Selected = 10,
		Tactical_CharacterDetails = 11
	}

	private const float DIALOGSKIP_TIMEPOINT = 1.2f;

	private Camera m_Camera;

	private ETurnAction m_CurrentAction = ETurnAction.None;

	private EPlayerHudLimiterMode m_AllowedMode;

	private ETurnAction m_AllowedTurnAction = ETurnAction.None;

	private EPlayerActionLimit m_ActionLimit;

	private CFGCharacter m_ActionLimitTargetCharacter;

	private CFGUsableObject m_ActionLimitTargetUsable;

	private CFGCell m_ActionLimitTargetCell;

	private ELockReason m_LockReason;

	private bool m_IsChangingWeapon;

	private Vector3 m_MouseWorldPos = Vector3.zero;

	private bool m_CharactersVisibilityNeedUpdate = true;

	private static CFGSelectionManager s_Instance;

	private Vector3 AOECIRCLEHELPERADD = new Vector3(0f, 0.05f, 0f);

	private Transform m_CursorHelper;

	private Transform m_CursorHelper_Action;

	private Transform m_CursorHelper2;

	private Transform m_CursorHelper3;

	private ECursorHelper m_CursorHelperState;

	private ECursorHelper m_CursorHelperAction;

	private bool m_PathVisGenerated;

	private bool m_GrenadePathVisGenerated;

	private CFGGameObject m_GameObjectUnderCursor;

	private Transform m_FullCoverVisPrefab;

	private Transform m_HalfCoverVisPrefab;

	private HashSet<Transform> m_CoversVis = new HashSet<Transform>();

	private CFGConeOfFire m_ConeVisualization;

	private HashSet<Transform> m_FinderHelpersVis;

	private HashSet<Transform> m_CannibalHelpersVis;

	private Transform m_RangeHelper;

	private Transform m_AOE_CircleHelper;

	private List<Transform> m_MultiRH = new List<Transform>();

	private CFGConeOfFireManager m_ConeViewM;

	private EControllerMode m_LastControllerMode;

	private EControllerMode m_CurrentControllerMode;

	private EInputMode m_LastInputMode;

	private float m_Controller_NextRead;

	private Vector3 m_LastMousePos = CFGMath.INIFITY3;

	private Vector3 m_LastGamepadPos = CFGMath.INIFITY3;

	private Vector3 m_LastRealGamepadPos = CFGMath.INIFITY3;

	private CFGGameObject m_GamepadCursor;

	private bool m_bTurnEndDisabled;

	private float m_TimeOfDialogSkipStart = -1f;

	private CFGCell m_BestCellForCurrentAction;

	private CFGCharacter m_SelectedCharacter;

	private CFGIAttackable m_TargetedObject;

	private List<CFGIAttackable> m_AOEPotentialObjects = new List<CFGIAttackable>();

	private List<CFGIAttackable> m_AOEObjects = new List<CFGIAttackable>();

	private List<CFGGameObject> m_TargetableObjects = new List<CFGGameObject>();

	private CFGAbility m_TargetingAbility;

	private CFGCell m_UC_Cell;

	private CFGCellObject m_UC_CellObject;

	private CFGCellObject m_UC_CellObjectWithInside;

	private int m_CurrentTargetableObject = -2;

	private float m_LastTileUnderCursorChangeTime;

	private CFGGameObject m_GameObjectUnderControllerCursor;

	private List<CFGRicochetObject> m_RicochetObjects = new List<CFGRicochetObject>();

	private HashSet<CFGRicochetObject> m_AvailableRicochetObjects = new HashSet<CFGRicochetObject>();

	private List<CFGCharacter> m_PossibleTargets = new List<CFGCharacter>();

	private List<CFGIAttackable> m_PossibleOtherTargets = new List<CFGIAttackable>();

	private List<CFGCharacter> m_CharactersToShowOnEnemyBar = new List<CFGCharacter>();

	private Vector3 m_LastRicochetDir = Vector3.zero;

	private Vector3 m_FreeTargetingPoint = Vector3.zero;

	private CFGCell m_FreeTargetingCell;

	private bool m_FreeTargetingCanShoot;

	private bool m_FreeTargetingEnabled;

	public bool IsLocked => m_LockReason != ELockReason.NoLock;

	public ELockReason LockStatus => m_LockReason;

	public EPlayerHudLimiterMode AllowedMode
	{
		get
		{
			return m_AllowedMode;
		}
		set
		{
			m_AllowedMode = value;
			if (m_AllowedMode != EPlayerHudLimiterMode.Confirm)
			{
				CancelOrdering();
			}
		}
	}

	public ETurnAction CurrentAction => m_CurrentAction;

	public CFGUsableObject PlayerActionLimiterUsable => m_ActionLimitTargetUsable;

	public CFGCell PlayerActionLimiterCell => m_ActionLimitTargetCell;

	public EPlayerActionLimit PlayerActionLimiter
	{
		get
		{
			return m_ActionLimit;
		}
		private set
		{
			m_ActionLimit = value;
			CancelOrdering();
			if (m_ActionLimit != EPlayerActionLimit.Enemy)
			{
				m_ActionLimitTargetCharacter = null;
			}
			if (m_ActionLimit != EPlayerActionLimit.MoveToTile)
			{
				m_ActionLimitTargetCell = null;
			}
			if (m_ActionLimit != EPlayerActionLimit.Usable)
			{
				m_ActionLimitTargetUsable = null;
			}
		}
	}

	public CFGCharacter PlayerActionLimiterCharacter => m_ActionLimitTargetCharacter;

	public ETurnAction AllowedAction
	{
		get
		{
			return m_AllowedTurnAction;
		}
		set
		{
			m_AllowedTurnAction = value;
			if (m_CurrentAction != ETurnAction.None && m_CurrentAction != m_AllowedTurnAction)
			{
				CancelOrdering();
			}
		}
	}

	public Camera Camera
	{
		get
		{
			return m_Camera;
		}
		private set
		{
			m_Camera = value;
		}
	}

	public Vector3 MouseWorldPos => m_MouseWorldPos;

	public static CFGSelectionManager Instance
	{
		get
		{
			if (s_Instance == null)
			{
				InitInstance();
			}
			return s_Instance;
		}
	}

	public CFGConeOfFireManager ConeOfViewManager => m_ConeViewM;

	public CFGGameObject GamepadCursor => m_GamepadCursor;

	public bool EndTurnDisabled
	{
		get
		{
			return m_bTurnEndDisabled;
		}
		set
		{
			m_bTurnEndDisabled = value;
		}
	}

	public CFGCharacter SelectedCharacter => m_SelectedCharacter;

	public CFGCharacter TargetedCharacter => m_TargetedObject as CFGCharacter;

	public CFGIAttackable TargetedObject => m_TargetedObject;

	public List<CFGRicochetObject> RicochetObjects => m_RicochetObjects;

	public HashSet<CFGRicochetObject> AvailableRicochetObjects => m_AvailableRicochetObjects;

	public List<CFGCharacter> PossibleTargets => m_PossibleTargets;

	public bool IsUsingRicochet => m_CurrentAction == ETurnAction.Ricochet;

	public CFGCell CellUnderCursor => m_UC_Cell;

	public CFGCellObject CellObjectUnderCursor => m_UC_CellObject;

	public CFGCellObject InsideCellObjectUnderCursor => m_UC_CellObjectWithInside;

	public CFGGameObject ObjectUnderCursor => m_GameObjectUnderCursor;

	private List<CFGCharacter> CharactersToShowOnEnemyBar => m_CharactersToShowOnEnemyBar;

	public List<CFGIAttackable> AOEObjects => m_AOEObjects;

	public bool CharactersVisibilityNeedUpdate()
	{
		return m_CharactersVisibilityNeedUpdate;
	}

	public void MarkCharactersVisibilityNeedUpdate()
	{
		m_CharactersVisibilityNeedUpdate = true;
	}

	public void ClearCharacterVisibilityNeedUpdate()
	{
		m_CharactersVisibilityNeedUpdate = false;
	}

	public void SetLock(ELockReason Reason, bool bLock, bool bCancelOrdering = true)
	{
		if (Reason != 0)
		{
			if (bLock)
			{
				m_LockReason |= Reason;
			}
			else
			{
				m_LockReason &= ~Reason;
			}
			if (m_LockReason == ELockReason.NoLock && bCancelOrdering)
			{
				CancelOrdering();
			}
		}
	}

	public void PlayerActionLimiter_Reset()
	{
		PlayerActionLimiter = EPlayerActionLimit.Default;
	}

	public void PlayerActionLimiter_DisableAll()
	{
		PlayerActionLimiter = EPlayerActionLimit.Nothing;
	}

	public void PlayerActionLimiter_SetToMove(Transform DestCellPos)
	{
		if (DestCellPos == null)
		{
			PlayerActionLimiter = EPlayerActionLimit.Default;
			return;
		}
		m_ActionLimitTargetCell = CFGCellMap.GetCell(DestCellPos.position);
		if (m_ActionLimitTargetCell == null)
		{
			PlayerActionLimiter = EPlayerActionLimit.Default;
		}
		else
		{
			PlayerActionLimiter = EPlayerActionLimit.MoveToTile;
		}
	}

	public void PlayerActionLimiter_EnemyCharacter(CFGCharacter Enemy)
	{
		if (Enemy == null || Enemy.IsDead || Enemy.Owner == null || !Enemy.Owner.IsAi)
		{
			Debug.LogError("PlayerActionLimiter_EnemyCharacter: Invalid input character!");
			PlayerActionLimiter = EPlayerActionLimit.Default;
		}
		else
		{
			PlayerActionLimiter = EPlayerActionLimit.Enemy;
			m_ActionLimitTargetCharacter = Enemy;
		}
	}

	public void PlayerActionLimiter_Usable(CFGUsableObject Usable)
	{
		if (Usable == null)
		{
			Debug.LogError("PlayerActionLimiter_Usable: NULL input object!");
			PlayerActionLimiter = EPlayerActionLimit.Nothing;
		}
		else
		{
			PlayerActionLimiter = EPlayerActionLimit.Usable;
			m_ActionLimitTargetUsable = Usable;
		}
	}

	public void PlayerActionLimiter_Reload()
	{
		PlayerActionLimiter = EPlayerActionLimit.Reload;
	}

	public void PlayerActionLimiter_Fanning()
	{
		PlayerActionLimiter = EPlayerActionLimit.Fanning;
	}

	public bool IsEnabled()
	{
		return !IsLocked;
	}

	public void OnCurrentFloorLevelChanged()
	{
		CFGObjectivePointSprite.OnCurrentFloorLevelChanged();
		CFGCharacter.UpdateAllCharactersOutliner();
	}

	public void OnCurrentCharacterTranslate()
	{
		if (!(m_SelectedCharacter == null) && m_SelectedCharacter.CurrentCell != null)
		{
			m_LastGamepadPos = m_SelectedCharacter.transform.position;
			m_LastRealGamepadPos = m_LastGamepadPos;
			CancelOrdering();
			ConeOfFire_GeneratePotentialList();
		}
	}

	public void OnCharacterActionFinished(CFGCharacter character, ETurnAction lastAction = ETurnAction.None)
	{
		OnTacticalSituationChanged();
		if (lastAction != ETurnAction.ChangeWeapon)
		{
			UpdateRangeVis();
		}
		if (character == m_SelectedCharacter)
		{
			if (lastAction != ETurnAction.ChangeWeapon && CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				CFGCamera component = GetComponent<CFGCamera>();
				if (component != null)
				{
					component.ChangeFocus(m_SelectedCharacter, 0.5f);
				}
			}
			if (m_CurrentAction != ETurnAction.None && character.CanMakeAction(m_CurrentAction) != 0)
			{
				CancelOrdering();
			}
			ConeOfFire_GeneratePotentialList();
			if (m_ConeVisualization != null)
			{
				m_ConeVisualization.transform.position = character.CurrentCell.WorldPosition;
				if (TargetedObject != null)
				{
					ConeOfFireViz_RotateTowards(TargetedObject.CurrentCell.WorldPosition);
				}
			}
		}
		MarkCharactersVisibilityNeedUpdate();
	}

	public void OnGridChanged()
	{
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character.CurrentAction == ETurnAction.None)
			{
				character.ProcessFacingToCover();
			}
		}
		UpdateRangeVis();
		CFGCharacter.UpdateAllCharactersOutliner();
		MarkCharactersVisibilityNeedUpdate();
	}

	public void OnEnemyClick(int id)
	{
		if (!IsLocked && id < m_CharactersToShowOnEnemyBar.Count)
		{
			OnEnemyClick(m_CharactersToShowOnEnemyBar[id]);
		}
	}

	public void OnEnemyMarkHover(int id, bool hover)
	{
		if (!(m_SelectedCharacter != null) || (m_TargetedObject != null && m_CurrentAction != ETurnAction.Ricochet))
		{
			return;
		}
		List<CFGCharacter> charactersToShowOnEnemyBar = CharactersToShowOnEnemyBar;
		if (id < charactersToShowOnEnemyBar.Count)
		{
			CFGCharacter cFGCharacter = charactersToShowOnEnemyBar[id];
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus((!hover) ? m_SelectedCharacter : cFGCharacter, 0.5f, force: true);
			}
		}
	}

	public void OnCharacterChangedWeapon()
	{
		m_IsChangingWeapon = false;
		ConeOfFire_GeneratePotentialList();
		UpdateFreeTargetingEnabled();
	}

	public void OnStrategicStart()
	{
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		if (instance != null && m_CursorHelper == null && instance.m_CursorHelperPrefab != null)
		{
			m_CursorHelper = UnityEngine.Object.Instantiate(instance.m_CursorHelperPrefab);
		}
	}

	public void OnStrategicEnd()
	{
		RemoveVisualisations();
	}

	public void OnTacticalStart()
	{
		if (base.gameObject.GetComponent<CFGRangeBorders>() == null)
		{
			base.gameObject.AddComponent<CFGRangeBorders>();
		}
		if (base.gameObject.GetComponent<CFGPathVis>() == null)
		{
			base.gameObject.AddComponent<CFGPathVis>();
		}
		if (base.gameObject.GetComponent<CFGGrenadePathVis>() == null)
		{
			base.gameObject.AddComponent<CFGGrenadePathVis>();
		}
	}

	public void OnTacticalEnd()
	{
		RemoveVisualisations();
	}

	public void OnTacticalSituationChanged()
	{
		if ((bool)CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner.IsPlayer && m_SelectedCharacter != null && (m_SelectedCharacter.IsDead || (m_SelectedCharacter.ActionPoints == 0 && m_SelectedCharacter.CurrentAction == ETurnAction.None)))
		{
			CFGCharacter nextActiveCharacter = GetNextActiveCharacter();
			if (nextActiveCharacter != null)
			{
				SelectCharacter(nextActiveCharacter, focus: true);
			}
			else if (IsAnyPlayerCharacterAlive())
			{
				CFGSingletonResourcePrefab<CFGTurnManager>.Instance.EndTurn(bUpdateTurnCounter: true);
			}
		}
	}

	public void OnCharacterSpawned(CFGCharacter character)
	{
		foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character2.CurrentAction == ETurnAction.None)
			{
				character2.ProcessFacingToCover();
			}
		}
		UpdateRangeVis(checkTiles: true);
		MarkCharactersVisibilityNeedUpdate();
	}

	public void OnCharacterDied(CFGCharacter character)
	{
		OnTacticalSituationChanged();
		UpdateRangeVis();
		MarkCharactersVisibilityNeedUpdate();
	}

	public void OnNewTurn(CFGOwner owner)
	{
		if (owner != null && owner.IsPlayer)
		{
			OnTacticalSituationChanged();
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus(m_SelectedCharacter, 0.5f);
			}
			if (CFGOptions.Gameplay.ResetCharQueueOnNewTurn)
			{
				SelectCharacter(GetFirstActiveCharacter(), focus: true);
			}
		}
		UpdateRangeVis();
	}

	public void CancelOrdering()
	{
		ConeOfFireViz_Destroy();
		m_TargetedObject = null;
		m_AvailableRicochetObjects.Clear();
		m_RicochetObjects.Clear();
		m_PossibleTargets.Clear();
		m_CurrentAction = ETurnAction.None;
		m_FreeTargetingCanShoot = false;
		m_FreeTargetingEnabled = false;
		DestroyRangeHelper();
		DestroyAOEHelper();
		DestroyCannibalHelpers(0);
		DestroyFinderHelpers(0);
		m_AOEObjects.Clear();
		m_TargetingAbility = null;
		m_CurrentControllerMode = EControllerMode.Global_Tactical;
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			m_CurrentControllerMode = EControllerMode.Global_Strategic;
		}
		ConeOfFireViz_Destroy();
		if (CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings != null)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.CloseMoreInfo(0);
		}
		CFGPathVis component = GetComponent<CFGPathVis>();
		if (component != null)
		{
			component.ClearMesh();
		}
		CFGGrenadePathVis component2 = GetComponent<CFGGrenadePathVis>();
		if (component2 != null)
		{
			component2.ClearMesh();
		}
		DestroyReactionShotHelpers();
	}

	public CFGCharacter GetFirstActiveCharacter()
	{
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner == null || playerOwner.Characters.Count == 0)
		{
			return null;
		}
		List<CFGCharacter> list = new List<CFGCharacter>(playerOwner.Characters);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].ActionPoints > 0)
			{
				return list[i];
			}
		}
		if (m_SelectedCharacter == null)
		{
			return null;
		}
		return (m_SelectedCharacter.ActionPoints <= 0) ? null : m_SelectedCharacter;
	}

	public CFGCharacter GetNextActiveCharacter()
	{
		if (m_SelectedCharacter == null)
		{
			return null;
		}
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner == null || playerOwner.Characters.Count == 0)
		{
			return null;
		}
		List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
		int num = teamCharactersList.IndexOf(m_SelectedCharacter.CharacterData);
		for (int i = num + 1; i < teamCharactersList.Count; i++)
		{
			if (teamCharactersList[i].ActionPoints > 0 && teamCharactersList[i].IsAlive && (bool)teamCharactersList[i].CurrentModel)
			{
				return teamCharactersList[i].CurrentModel;
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (teamCharactersList[j].ActionPoints > 0 && teamCharactersList[j].IsAlive && (bool)teamCharactersList[j].CurrentModel)
			{
				return teamCharactersList[j].CurrentModel;
			}
		}
		return (m_SelectedCharacter.ActionPoints <= 0 || !m_SelectedCharacter.IsAlive) ? null : m_SelectedCharacter;
	}

	public CFGCharacter GetPrevActiveCharacter()
	{
		if (m_SelectedCharacter == null)
		{
			return null;
		}
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner == null || playerOwner.Characters.Count == 0)
		{
			return null;
		}
		List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
		int num = teamCharactersList.IndexOf(m_SelectedCharacter.CharacterData);
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			if (teamCharactersList[num2].ActionPoints > 0 && teamCharactersList[num2].IsAlive && (bool)teamCharactersList[num2].CurrentModel)
			{
				return teamCharactersList[num2].CurrentModel;
			}
		}
		for (int num3 = teamCharactersList.Count - 1; num3 > num; num3--)
		{
			if (teamCharactersList[num3].ActionPoints > 0 && teamCharactersList[num3].IsAlive && (bool)teamCharactersList[num3].CurrentModel)
			{
				return teamCharactersList[num3].CurrentModel;
			}
		}
		return (m_SelectedCharacter.ActionPoints <= 0) ? null : m_SelectedCharacter;
	}

	private void Awake()
	{
		SetLock(ELockReason.FadeOut, bLock: true);
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		if (instance != null)
		{
			m_FullCoverVisPrefab = instance.m_FullCoverVisPrefab;
			m_HalfCoverVisPrefab = instance.m_HalfCoverVisPrefab;
		}
	}

	private void OnEnable()
	{
		Camera = GetComponent<Camera>();
		CFGSingletonResourcePrefab<CFGObjectManager>.InitInstance();
		CFGSingletonResourcePrefab<CFGTurnManager>.InitInstance();
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character != null && character.Owner != null && character.Owner.IsPlayer)
			{
				SelectCharacter(character, focus: true);
				break;
			}
		}
		m_ConeViewM = new CFGConeOfFireManager();
	}

	private void Update()
	{
		if (CFGTimer.IsPaused_Gameplay)
		{
			return;
		}
		CFGGame instance = CFGSingleton<CFGGame>.Instance;
		if (instance.IsInStrategic() && m_SelectedCharacter == null)
		{
			foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				if (character != null && character.Owner != null && character.Owner.IsPlayer)
				{
					SelectCharacter(character, focus: true);
					break;
				}
			}
		}
		CFGCell cFGCell = GetCellUnderCursor();
		CFGGameObject gameObjectUnderCursor = GetGameObjectUnderCursor();
		if (gameObjectUnderCursor != m_GameObjectUnderCursor)
		{
			if ((bool)m_GameObjectUnderCursor)
			{
				m_GameObjectUnderCursor.IsUnderCursor = false;
				m_GameObjectUnderCursor.OnCursorLeave();
			}
			if (m_GameObjectUnderControllerCursor != null && CFGInput.LastReadInputDevice == EInputMode.KeyboardAndMouse)
			{
				m_GameObjectUnderControllerCursor.IsUnderCursor = false;
				m_GameObjectUnderControllerCursor.OnCursorLeave();
			}
			m_GameObjectUnderCursor = gameObjectUnderCursor;
			if ((bool)m_GameObjectUnderCursor)
			{
				m_GameObjectUnderCursor.IsUnderCursor = true;
				m_GameObjectUnderCursor.OnCursorEnter();
			}
		}
		if ((bool)m_GameObjectUnderCursor && m_GameObjectUnderCursor.NameId != "Horseman")
		{
			CFGCell cell = CFGCellMap.GetCell(m_GameObjectUnderCursor.transform.position);
			if (cell != null)
			{
				cFGCell = cell;
			}
		}
		if (cFGCell != m_UC_Cell)
		{
			DestroyCoversVis();
			m_UC_Cell = cFGCell;
			if (m_UC_Cell != null)
			{
				if (CFGOptions.Gameplay.ShowCoverIcons && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn && CurrentAction == ETurnAction.None && TargetedCharacter == null && m_SelectedCharacter != null && m_SelectedCharacter.CurrentAction == ETurnAction.None)
				{
					SpawnCoversVis(m_UC_Cell);
				}
				m_PathVisGenerated = false;
				m_GrenadePathVisGenerated = false;
			}
			else
			{
				m_PathVisGenerated = true;
				m_GrenadePathVisGenerated = true;
			}
			if (instance.IsInGame())
			{
				if (m_CurrentAction == ETurnAction.None)
				{
					CFGPathVis component = GetComponent<CFGPathVis>();
					if (component != null)
					{
						component.ClearMesh();
					}
					CFGGrenadePathVis component2 = GetComponent<CFGGrenadePathVis>();
					if (component2 != null)
					{
						component2.ClearMesh();
					}
					DestroyReactionShotHelpers();
				}
				CFGRangeBorders component3 = GetComponent<CFGRangeBorders>();
				if (component3 != null)
				{
					component3.ManageRangeVis(CellUnderCursor);
				}
			}
			m_LastTileUnderCursorChangeTime = Time.time;
		}
		if (instance.IsInGame())
		{
			ConeOfViewManager.UpdateManageCones();
			if (m_CurrentAction != ETurnAction.None)
			{
				UpdateFreeTargetingFromMouse();
				if (m_TargetingAbility != null && m_UC_Cell != null && m_SelectedCharacter.CurrentAction == ETurnAction.None)
				{
					switch (m_TargetingAbility.AnimationType)
					{
					case eAbilityAnimation.Throw1:
					case eAbilityAnimation.Throw2:
					case eAbilityAnimation.ThrowAuto:
					{
						CFGGrenadePathVis component4 = GetComponent<CFGGrenadePathVis>();
						if (component4 != null)
						{
							component4.RegenerateMesh(100f, m_FreeTargetingCanShoot);
						}
						SpawnRangeHelper(m_SelectedCharacter.Transform.position, m_TargetingAbility.GetRange());
						if (m_RangeHelper != null && m_FreeTargetingCell != null)
						{
							Vector3 position = m_SelectedCharacter.Transform.position;
							position.y = m_FreeTargetingCell.WorldPosition.y + 0.3f;
							m_RangeHelper.position = position;
						}
						m_GrenadePathVisGenerated = true;
						break;
					}
					}
				}
			}
			else if (!m_PathVisGenerated && m_UC_Cell != null && (Time.time - m_LastTileUnderCursorChangeTime > CFGOptions.Gameplay.PathShowDelay || CFGInput.LastReadInputDevice == EInputMode.Gamepad))
			{
				CFGPathVis component5 = GetComponent<CFGPathVis>();
				if (component5 != null)
				{
					component5.RegenerateMesh();
				}
				m_PathVisGenerated = true;
			}
		}
		UpdateInput();
		UpdateCursorHelper();
		UpdateFreeTargetingEnabled();
		UpdateHUD();
		if (IsLocked)
		{
			if (CFGInput.IsActivated(EActionCommand.Exit))
			{
				instance.OnEscapeKey();
			}
			return;
		}
		RenderRicochetHelpers();
		CFGFadeToColor componentInChildren = GetComponentInChildren<CFGFadeToColor>();
		if ((componentInChildren == null || !componentInChildren.enabled) && !CFGSingleton<CFGWindowMgr>.Instance.IsCursorOverUI() && Input.GetMouseButtonDown(0) && CFGInput.ExclusiveInputDevice != EInputMode.Gamepad)
		{
			CFGInput.ChangeInputMode(EInputMode.KeyboardAndMouse);
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				if (CFGOptions.Input.AllowLMBOnStrategic && IsEnabled())
				{
					OnRightClick(0);
				}
			}
			else
			{
				OnLMB();
			}
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			FindBestCellForCurrentAction();
		}
	}

	public void OnGameplayPause()
	{
	}

	public void OnGameplayUnPause()
	{
	}

	private bool IsAnyPlayerCharacterAlive()
	{
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner != null)
		{
			foreach (CFGCharacter character in playerOwner.Characters)
			{
				if (character.IsAlive)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsPlayerCharacter(CFGCharacter character)
	{
		return (bool)character && (bool)character.Owner && character.Owner.IsPlayer;
	}

	private void OnAttackableClick(CFGIAttackable target)
	{
		if (m_ActionLimit != 0)
		{
			if (m_ActionLimit != EPlayerActionLimit.Enemy)
			{
				return;
			}
			CFGCharacter cFGCharacter = target as CFGCharacter;
			if (cFGCharacter != m_ActionLimitTargetCharacter || cFGCharacter == null)
			{
				return;
			}
		}
		CFGCamera component = GetComponent<CFGCamera>();
		if (component == null || m_SelectedCharacter.GunpointState != 0 || m_SelectedCharacter.Imprisoned)
		{
			return;
		}
		if (m_TargetingAbility != null)
		{
			Usable_HandleOnAttackableClick(target, component);
		}
		else
		{
			if (target == null || !target.IsAlive)
			{
				return;
			}
			switch (m_CurrentAction)
			{
			case ETurnAction.Ricochet:
				if (m_PossibleOtherTargets.Contains(target))
				{
					m_TargetedObject = target;
					component.ChangeFocus(target as CFGGameObject, 0.5f, force: true);
				}
				break;
			case ETurnAction.None:
			case ETurnAction.Shoot:
			case ETurnAction.AltFire_Fanning:
			case ETurnAction.AltFire_ScopedShot:
			case ETurnAction.AltFire_ConeShot:
				if ((m_TargetedObject == null || m_TargetedObject != target) && (SelectedCharacter.IsOtherTargetVisible(target) || SelectedCharacter.HasLineOfFireTo(m_TargetedObject, m_CurrentAction)))
				{
					m_TargetedObject = target;
					component.ChangeFocus(SelectedCharacter.Transform.position, target.Position, 0.5f, force: true);
					if (m_CurrentAction == ETurnAction.None)
					{
						m_CurrentAction = ETurnAction.Shoot;
						GenerateTargetableObjectList();
					}
					m_FreeTargetingEnabled = false;
					if (m_CurrentAction == ETurnAction.AltFire_ConeShot)
					{
						InitConeOnTarget();
					}
					if (m_AllowedTurnAction != ETurnAction.None && m_CurrentAction != m_AllowedTurnAction)
					{
						CancelOrdering();
					}
				}
				break;
			}
		}
	}

	private void OnEnemyClick(CFGCharacter enemy)
	{
		switch (m_ActionLimit)
		{
		default:
			return;
		case EPlayerActionLimit.Enemy:
			if (m_ActionLimitTargetCharacter != enemy)
			{
				return;
			}
			break;
		case EPlayerActionLimit.Fanning:
			if (m_CurrentAction != ETurnAction.AltFire_Fanning)
			{
				return;
			}
			break;
		case EPlayerActionLimit.Reload:
			if (m_CurrentAction != ETurnAction.Reload)
			{
				return;
			}
			break;
		case EPlayerActionLimit.Nothing:
		case EPlayerActionLimit.MoveToTile:
		case EPlayerActionLimit.Usable:
			return;
		case EPlayerActionLimit.Default:
			break;
		}
		if (m_AllowedMode != 0)
		{
			if (m_AllowedTurnAction == ETurnAction.None || (m_CurrentAction != ETurnAction.None && m_AllowedTurnAction != m_CurrentAction))
			{
				return;
			}
			switch (m_AllowedTurnAction)
			{
			default:
				return;
			case ETurnAction.Shoot:
				if (m_CurrentAction == ETurnAction.None)
				{
					return;
				}
				break;
			case ETurnAction.AltFire_Fanning:
				if (m_CurrentAction == ETurnAction.None)
				{
					return;
				}
				break;
			}
		}
		CFGCamera component = GetComponent<CFGCamera>();
		if (component == null || m_SelectedCharacter.GunpointState != 0 || m_SelectedCharacter.Imprisoned)
		{
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.Ricochet:
			if (m_PossibleTargets.Contains(enemy))
			{
				m_TargetedObject = enemy;
				component.ChangeFocus(enemy, 0.5f, force: true);
			}
			break;
		case ETurnAction.Gunpoint:
		{
			EActionResult eActionResult = m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, enemy);
			eActionResult &= ~EActionResult.InvalidTarget;
			if ((eActionResult & ~EActionResult.NotInCone) == 0)
			{
				m_TargetedObject = enemy;
				component.ChangeFocus(SelectedCharacter.Transform.position, enemy.Transform.position, 0.5f, force: true);
			}
			break;
		}
		case ETurnAction.None:
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			if ((m_TargetedObject != null && m_TargetedObject == enemy) || (!SelectedCharacter.IsEnemyVisible(enemy) && !SelectedCharacter.HasLineOfFireTo(enemy, m_CurrentAction)))
			{
				break;
			}
			if (m_CurrentAction == ETurnAction.AltFire_ConeShot)
			{
				m_TargetedObject = null;
			}
			else
			{
				m_TargetedObject = enemy;
				component.ChangeFocus(SelectedCharacter.Transform.position, enemy.Transform.position, 0.5f, force: true);
			}
			if (m_CurrentAction == ETurnAction.None)
			{
				if (m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, enemy) == EActionResult.Success)
				{
					m_CurrentAction = ETurnAction.Gunpoint;
					SpawnRangeHelper(m_SelectedCharacter.Transform.position, 6f);
					GenerateTargetableObjectList();
				}
				else
				{
					m_CurrentAction = ETurnAction.Shoot;
					GenerateTargetableObjectList();
				}
			}
			m_FreeTargetingEnabled = false;
			if (m_CurrentAction == ETurnAction.AltFire_ConeShot)
			{
				InitConeOnTarget();
			}
			break;
		}
	}

	private void OnSelectionChanged()
	{
		UpdateRangeVis();
		ConeOfFire_GeneratePotentialList();
		if (!(CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities == null))
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.ClearCannibalAndFinderEvent();
		}
	}

	public static void GlobalLock(ELockReason Reason, bool bEnable)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if ((bool)component)
		{
			component.SetEnabled(!bEnable);
		}
		CFGSelectionManager component2 = Camera.main.GetComponent<CFGSelectionManager>();
		if ((bool)component2)
		{
			component2.SetLock(Reason, bEnable, bCancelOrdering: false);
		}
	}

	public static void InitInstance()
	{
		if (s_Instance == null)
		{
			s_Instance = UnityEngine.Object.FindObjectOfType(typeof(CFGSelectionManager)) as CFGSelectionManager;
		}
	}

	public static bool IsInstanceInitialized()
	{
		return s_Instance != null;
	}

	public void OnCharacterButtonClick(int id)
	{
		if (!IsLocked)
		{
			List<CFGCharacterData> teamCharactersListTactical = CFGCharacterList.GetTeamCharactersListTactical();
			if (id < teamCharactersListTactical.Count && id >= 0)
			{
				SelectCharacter(teamCharactersListTactical[id].CurrentModel, focus: true);
			}
		}
	}

	public void OnCharacterBigButtonClick()
	{
		if (!IsLocked)
		{
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus(m_SelectedCharacter, 0.5f);
			}
		}
	}

	public void OnActionClick(int id)
	{
		if (IsLocked)
		{
			return;
		}
		switch (id)
		{
		case 0:
			m_TargetedObject = null;
			if (!m_bTurnEndDisabled)
			{
				CFGSingletonResourcePrefab<CFGTurnManager>.Instance.EndTurn(bUpdateTurnCounter: true);
			}
			break;
		case 1:
		{
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null && !component.RotationDisabled)
			{
				component.RotateLeft(0.5f);
			}
			break;
		}
		case 2:
		{
			CFGCamera component2 = GetComponent<CFGCamera>();
			if (component2 != null && !component2.RotationDisabled)
			{
				component2.RotateRight(0.5f);
			}
			break;
		}
		case 3:
		{
			CFGCharacter prevActiveCharacter = GetPrevActiveCharacter();
			if (prevActiveCharacter != null)
			{
				SelectCharacter(prevActiveCharacter, focus: true);
			}
			break;
		}
		case 4:
		{
			CFGCharacter nextActiveCharacter = GetNextActiveCharacter();
			if (nextActiveCharacter != null)
			{
				SelectCharacter(nextActiveCharacter, focus: true);
			}
			break;
		}
		}
	}

	public void ActivateAbility(ETurnAction Action, bool bInstant)
	{
		if (IsLocked || m_SelectedCharacter == null)
		{
			return;
		}
		if (Action == CurrentAction && !bInstant)
		{
			CancelOrdering();
			return;
		}
		switch (m_ActionLimit)
		{
		case EPlayerActionLimit.Reload:
			if (Action != ETurnAction.Reload)
			{
				return;
			}
			break;
		case EPlayerActionLimit.Fanning:
			if (Action != ETurnAction.AltFire_Fanning)
			{
				return;
			}
			break;
		}
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner != null && playerOwner.m_OnActionActivatedCallback != null)
		{
			playerOwner.m_OnActionActivatedCallback(Action);
		}
		switch (Action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			if (m_CharactersToShowOnEnemyBar.Count > 0)
			{
				if (m_CurrentAction != Action)
				{
					CancelOrdering();
					m_CurrentAction = Action;
					GenerateTargetableObjectList();
					OnEnemyClick(0);
				}
			}
			else if (m_SelectedCharacter.OtherTargets.Count > 0)
			{
				if (m_CurrentAction != ETurnAction.Shoot)
				{
					CancelOrdering();
				}
				m_CurrentAction = Action;
				GenerateTargetableObjectList();
				OnAttackableClick(m_SelectedCharacter.OtherTargets[0]);
			}
			else
			{
				CancelOrdering();
				m_CurrentAction = Action;
				GenerateTargetableObjectList();
				m_FreeTargetingEnabled = true;
				UpdateFreeTargetingEnabled();
				if (Action == ETurnAction.AltFire_ConeShot)
				{
					CFGGun currentWeapon = m_SelectedCharacter.CurrentWeapon;
					if (currentWeapon != null && currentWeapon.m_Definition != null && currentWeapon.m_Definition.ConeAngle > 1f)
					{
						Vector3 worldPosition = m_SelectedCharacter.CurrentCell.WorldPosition;
						ConeOfFireViz_Init(currentWeapon.m_Definition.ConeAngle, worldPosition, worldPosition + new Vector3(2f, 0f, 0f), CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireRange);
					}
				}
			}
			SelectTargetableObject(m_TargetedObject);
			break;
		case ETurnAction.Ricochet:
			CancelOrdering();
			m_CurrentAction = ETurnAction.Ricochet;
			GenerateAvailableRicochetObjects(null);
			GenerateTargetableObjectList();
			return;
		case ETurnAction.Reload:
			CancelOrdering();
			if (m_SelectedCharacter.CurrentAction == ETurnAction.None)
			{
				if (bInstant && m_SelectedCharacter.CanMakeAction(ETurnAction.Reload) == EActionResult.Success)
				{
					m_SelectedCharacter.MakeAction(ETurnAction.Reload);
					return;
				}
				m_CurrentAction = ETurnAction.Reload;
			}
			break;
		case ETurnAction.Gunpoint:
			CancelOrdering();
			m_CurrentAction = ETurnAction.Gunpoint;
			GenerateTargetableObjectList();
			SpawnRangeHelper(m_SelectedCharacter.Transform.position, 6f);
			m_CurrentTargetableObject = -1;
			SelectNextTargetable();
			break;
		case ETurnAction.SuicideShot:
			CancelOrdering();
			m_CurrentAction = ETurnAction.SuicideShot;
			m_CurrentTargetableObject = -1;
			break;
		}
		if (Action.IsStdNonSpecial())
		{
			ActivateStdNonSpecialAbility(Action, bInstant);
			SelectTargetableObject(m_TargetedObject);
		}
		if (m_ActionLimit != 0)
		{
			CFGInput.ClearActions(0.5f);
			CFGJoyManager.ClearJoyActions(0.5f);
		}
	}

	public void OnAbilityClick(int id)
	{
		if (IsLocked)
		{
			return;
		}
		bool flag = false;
		if (!Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
		{
			if (Input.GetMouseButtonUp(2) && !Input.GetMouseButtonUp(0))
			{
				flag = true;
			}
			if (Input.GetMouseButtonUp(0) || flag)
			{
				ActivateAbility((ETurnAction)id, flag);
			}
		}
	}

	public void OnConfirmationAttackClick(int a)
	{
		if (IsLocked)
		{
			return;
		}
		CFGInput.ClearActions(0.5f);
		CFGJoyManager.ClearJoyActions(0.5f);
		switch (m_AllowedMode)
		{
		case EPlayerHudLimiterMode.Nothing:
			return;
		case EPlayerHudLimiterMode.SpecificTurnActionOnly:
			if (m_CurrentAction != m_AllowedTurnAction)
			{
				return;
			}
			break;
		}
		if (CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_AButtonPad.gameObject.activeSelf && !CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_AButtonPad.enabled)
		{
			return;
		}
		CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
		if (playerOwner != null && playerOwner.m_OnConfirmClicked != null)
		{
			playerOwner.m_OnConfirmClicked(m_CurrentAction);
		}
		if (m_TargetingAbility != null)
		{
			Usable_Activate();
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.SuicideShot:
			SelectedCharacter.MakeAction(ETurnAction.SuicideShot);
			break;
		case ETurnAction.Gunpoint:
		{
			CFGCharacter cFGCharacter = m_TargetedObject as CFGCharacter;
			if (!(cFGCharacter == null) && SelectedCharacter.MakeAction(ETurnAction.Gunpoint, cFGCharacter) == EActionResult.Success)
			{
				CancelOrdering();
			}
			return;
		}
		case ETurnAction.Ricochet:
			if (SelectedCharacter.GetChanceToHit(m_TargetedObject, m_RicochetObjects, null, null, ETurnAction.Ricochet) == 0)
			{
				return;
			}
			SelectedCharacter.MakeAction(ETurnAction.Ricochet, m_TargetedObject, m_RicochetObjects);
			break;
		case ETurnAction.Shoot:
			if (SelectedCharacter.GetChanceToHit(m_TargetedObject, null, null, null, ETurnAction.Shoot) == 0)
			{
				return;
			}
			SelectedCharacter.MakeAction(ETurnAction.Shoot, m_TargetedObject, null);
			break;
		case ETurnAction.AltFire_ConeShot:
			if (m_TargetedObject != null)
			{
				SelectedCharacter.MakeAction(ETurnAction.AltFire_ConeShot, m_TargetedObject, m_AOEObjects);
			}
			else
			{
				SelectedCharacter.MakeAction(ETurnAction.Miss_Shoot, m_FreeTargetingCell, null);
			}
			break;
		case ETurnAction.AltFire_Fanning:
			if (SelectedCharacter.GetChanceToHit(m_TargetedObject, null, null, null, ETurnAction.AltFire_Fanning) == 0)
			{
				return;
			}
			SelectedCharacter.MakeAction(ETurnAction.AltFire_Fanning, m_TargetedObject, null);
			break;
		case ETurnAction.AltFire_ScopedShot:
			if (SelectedCharacter.GetChanceToHit(m_TargetedObject, null, null, null, ETurnAction.AltFire_ScopedShot) == 0)
			{
				return;
			}
			SelectedCharacter.MakeAction(ETurnAction.AltFire_ScopedShot, m_TargetedObject, null);
			break;
		case ETurnAction.Reload:
			m_SelectedCharacter.MakeAction(CurrentAction);
			break;
		}
		CancelOrdering();
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.gameObject.SetActive(value: false);
		}
		switch (m_CurrentAction)
		{
		default:
			m_SelectedCharacter.FlagNeedFlash = true;
			break;
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			break;
		}
		CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashCharacterBig();
	}

	public void OnChangeWeaponClick()
	{
		if (!IsLocked && SelectedCharacter != null)
		{
			m_IsChangingWeapon = true;
			SelectedCharacter.MakeAction(ETurnAction.ChangeWeapon);
		}
	}

	private void GenerateCharactersToShowOnEnemyBar()
	{
		m_CharactersToShowOnEnemyBar.Clear();
		if (!(m_SelectedCharacter != null))
		{
			return;
		}
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (!(aiOwner != null))
		{
			return;
		}
		foreach (CFGCharacter character in aiOwner.Characters)
		{
			if (character.BestDetectionType != 0 && m_SelectedCharacter.HasLineOfFireTo(character, ETurnAction.Shoot))
			{
				m_CharactersToShowOnEnemyBar.Add(character);
			}
		}
		m_SelectedCharacter.SortVisibleEnemiesList(ref m_CharactersToShowOnEnemyBar);
	}

	private void UpdateFreeTargetingEnabled()
	{
		if (m_CurrentAction == ETurnAction.AltFire_ConeShot)
		{
			m_FreeTargetingEnabled = true;
		}
	}

	private void UpdateTermsOfShooting_MoreInfo()
	{
		CFGCell cFGCell = null;
		bool flag = false;
		if (IsUsingRicochet && m_RicochetObjects != null && m_RicochetObjects.Count > 0)
		{
			cFGCell = m_RicochetObjects[m_RicochetObjects.Count - 1].Cell;
			flag = true;
		}
		else
		{
			cFGCell = m_SelectedCharacter.CurrentCell;
		}
		if (cFGCell == null)
		{
			return;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		CFGTextManager instance2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance;
		if (m_TargetedObject == null)
		{
			return;
		}
		CFGCell currentCell = m_TargetedObject.CurrentCell;
		ECoverType eCoverType = CFGCharacter.GetTargetCover(cFGCell, currentCell);
		if (m_TargetedObject.GetCoverMult() == 0)
		{
			eCoverType = ECoverType.NONE;
		}
		int num = 0;
		instance.m_TermsOfShootings.SetParam(num++, "<b><color=white>" + instance2.GetLocalizedText(m_SelectedCharacter.NameId) + "</color></b>", string.Empty);
		instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_charaim"), m_SelectedCharacter.BuffedAim.ToString());
		int turnActionChanceToHitMod = CFGCharacter.GetTurnActionChanceToHitMod(m_CurrentAction);
		if (turnActionChanceToHitMod != 0)
		{
			string text_id = string.Empty;
			switch (m_CurrentAction)
			{
			case ETurnAction.AltFire_ScopedShot:
				text_id = "tactical_targeting_scoped";
				break;
			case ETurnAction.AltFire_Fanning:
				text_id = "tactical_targeting_fanning";
				break;
			case ETurnAction.AltFire_ConeShot:
				text_id = "tactical_targeting_cone";
				break;
			case ETurnAction.Penetrate:
				text_id = "tactical_targeting_penetrate";
				break;
			}
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText(text_id), turnActionChanceToHitMod.ToString());
		}
		int floorAimMod = ((EFloorLevelType)m_SelectedCharacter.CurrentCell.Floor).GetFloorAimMod((EFloorLevelType)currentCell.Floor);
		if (floorAimMod != 0)
		{
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_elevation"), floorAimMod.ToString());
		}
		if (CFGCellMap.GetLineOfSightAutoSideSteps(m_SelectedCharacter, m_TargetedObject, cFGCell, currentCell, 1000) != 0)
		{
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_blindshot"), CFGGameplaySettings.s_BlindShotMod.ToString());
		}
		bool flag2 = false;
		if (m_SelectedCharacter.CurrentWeapon != null && m_SelectedCharacter.CurrentWeapon.m_Definition != null)
		{
			instance.m_TermsOfShootings.SetParam(num++, string.Empty, string.Empty);
			instance.m_TermsOfShootings.SetParam(num++, "<b><color=white>" + instance2.GetLocalizedText(m_SelectedCharacter.CurrentWeapon.m_Definition.ItemID + "_name") + "</color></b>", string.Empty);
			if (m_SelectedCharacter.CurrentWeapon.HitChance != 0)
			{
				instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_weaponaim"), m_SelectedCharacter.CurrentWeapon.HitChance.ToString());
			}
			int num2 = 0;
			CFGCell start = m_SelectedCharacter.CurrentCell;
			if (flag)
			{
				for (int i = 0; i < m_RicochetObjects.Count; i++)
				{
					if (m_RicochetObjects[i] != null && m_RicochetObjects[i].Cell != null)
					{
						num2 += CFGCellMap.Distance(start, m_RicochetObjects[i].Cell);
						start = m_RicochetObjects[i].Cell;
					}
				}
			}
			num2 += CFGCellMap.Distance(start, currentCell);
			flag2 = num2 <= CFGGameplaySettings.s_PointBlankDistance;
			int distMod = m_SelectedCharacter.CurrentWeapon.Class.GetDistMod(num2);
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_distancemodifier"), distMod.ToString());
		}
		instance.m_TermsOfShootings.SetParam(num++, string.Empty, string.Empty);
		instance.m_TermsOfShootings.SetParam(num++, "<b><color=white>" + instance2.GetLocalizedText(m_TargetedObject.NameId) + "</color></b>", string.Empty);
		int num3 = -m_TargetedObject.BuffedDefense;
		instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_targetdef"), num3.ToString());
		int aimBonus = m_TargetedObject.GetAimBonus();
		if (aimBonus != 0)
		{
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_shootable"), aimBonus.ToString());
		}
		if (eCoverType != 0)
		{
			instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText((eCoverType != ECoverType.HALF) ? "tactical_targeting_targetfullcover" : "tactical_targeting_targethalfcover"), eCoverType.GetAimMod().ToString());
		}
		while (num < instance.m_TermsOfShootings.m_ParamNames.Count - 2)
		{
			instance.m_TermsOfShootings.SetParam(num++, string.Empty, string.Empty);
		}
		string param_name = string.Empty;
		if (flag2)
		{
			param_name = "<color=#96BE46>" + instance2.GetLocalizedText("tactical_targeting_pointblank") + "</color>";
		}
		else
		{
			int baseChanceToHit = SelectedCharacter.GetBaseChanceToHit(m_TargetedObject, (!flag) ? null : m_RicochetObjects, cFGCell, currentCell, m_CurrentAction);
			if (baseChanceToHit < CFGGameplaySettings.s_CtH_BottomTreshold)
			{
				param_name = "<color=#EA4242>" + instance2.GetLocalizedText("tactical_targeting_rounddown") + "</color>";
			}
			else if (baseChanceToHit > CFGGameplaySettings.s_CtH_TopTreshold)
			{
				param_name = "<color=#96BE46>" + instance2.GetLocalizedText("tactical_targeting_roundup") + "</color>";
			}
		}
		instance.m_TermsOfShootings.SetParam(num++, param_name, string.Empty);
		int chanceToHit = SelectedCharacter.GetChanceToHit(m_TargetedObject, (!flag) ? null : m_RicochetObjects, cFGCell, currentCell, m_CurrentAction);
		instance.m_TermsOfShootings.SetParam(num++, instance2.GetLocalizedText("tactical_targeting_total"), chanceToHit.ToString());
		instance.m_TermsOfShootings.m_BgCover.transform.parent.gameObject.SetActive(!m_FreeTargetingEnabled);
		instance.m_TermsOfShootings.m_Name2.text = instance2.GetLocalizedText(m_SelectedCharacter.NameId);
		instance.m_TermsOfShootings.MaxHP = m_TargetedObject.MaxHp;
		instance.m_TermsOfShootings.HP = m_TargetedObject.Hp;
		instance.m_TermsOfShootings.Cover = (int)eCoverType;
	}

	private bool IsActionLimited(ETurnAction action)
	{
		if (m_ActionLimit == EPlayerActionLimit.Nothing)
		{
			return true;
		}
		if (m_AllowedMode == EPlayerHudLimiterMode.Default)
		{
			return false;
		}
		if (m_AllowedMode != EPlayerHudLimiterMode.SpecificTurnActionOnly)
		{
			return true;
		}
		if (m_AllowedTurnAction != ETurnAction.None && m_AllowedTurnAction != action)
		{
			return true;
		}
		return false;
	}

	private void AddTurnActionButton(ETurnAction action, int icon, string tooltip, ref int ShortCut, bool AND = true, bool OR = false)
	{
		CFGSkillButtonsData cFGSkillButtonsData = new CFGSkillButtonsData();
		cFGSkillButtonsData.icon = icon;
		cFGSkillButtonsData.data = (int)action;
		cFGSkillButtonsData.selected = m_CurrentAction == action;
		cFGSkillButtonsData.useme_state = false;
		if (IsActionLimited(action))
		{
			cFGSkillButtonsData.enabled = false;
		}
		else
		{
			cFGSkillButtonsData.enabled = (m_SelectedCharacter.CanMakeAction(action) == EActionResult.Success || OR) && AND;
		}
		cFGSkillButtonsData.icon_list = 0;
		string text = CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)ShortCut);
		ShortCut++;
		string text2 = string.Empty;
		if (action == ETurnAction.Ricochet && CFGDef_Ability.Ricochet_LuckCost > m_SelectedCharacter.Luck)
		{
			text2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_noluck", "<color=#ff4646>" + CFGDef_Ability.Ricochet_LuckCost + "</color>");
		}
		cFGSkillButtonsData.tooltip_id = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(tooltip, text) + " " + text2;
		CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData);
	}

	private void UpdateSelectionPanel()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		CFGTextManager instance2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance;
		instance.m_HUD.m_NameChar.text = instance2.GetLocalizedText(m_SelectedCharacter.NameId);
		instance.m_HUD.m_Avatar.IconNumber = m_SelectedCharacter.ImageIDX;
		instance.m_HUD.m_Hp.text = m_SelectedCharacter.Hp + "/" + m_SelectedCharacter.MaxHp;
		instance.m_HUD.FlashingHP(m_SelectedCharacter.Hp < 2);
		instance.m_HUD.m_HpBar.SetProgress(m_SelectedCharacter.Hp * 100 / m_SelectedCharacter.MaxHp);
		instance.m_HUD.m_Luck.text = m_SelectedCharacter.Luck + "/" + m_SelectedCharacter.MaxLuck;
		instance.m_HUD.m_LuckBar.SetProgress(m_SelectedCharacter.Luck * 100 / m_SelectedCharacter.MaxLuck);
		instance.m_HUD.m_ImprisonedBig.gameObject.SetActive(m_SelectedCharacter.Imprisoned);
		instance.m_HUD.m_HeatMarkers[0].transform.parent.parent.gameObject.SetActive(!m_SelectedCharacter.Imprisoned && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage);
		for (int i = 0; i < instance.m_HUD.m_HeatMarkers.Count; i++)
		{
			instance.m_HUD.m_HeatMarkers[i].gameObject.SetActive(i < m_SelectedCharacter.CharacterData.TotalHeat);
		}
		List<CFGCharacterData> teamCharactersListTactical = CFGCharacterList.GetTeamCharactersListTactical();
		instance.m_HUD.SetCharactersVisible(teamCharactersListTactical.Count);
		int num = 20;
		for (int j = 0; j < instance.m_HUD.m_CharButtons.Count; j++)
		{
			if (j < teamCharactersListTactical.Count)
			{
				CFGCharacterData cFGCharacterData = teamCharactersListTactical[j];
				if (cFGCharacterData == null || cFGCharacterData.Definition == null)
				{
					continue;
				}
				instance.m_HUD.m_CharButtons[j].IconNumber = cFGCharacterData.ImageIDX;
				instance.m_HUD.m_CharExtButtons[j].interactable = true;
				if ((bool)cFGCharacterData.CurrentModel)
				{
					instance.m_HUD.m_ApPointList[j * 2 + 1].gameObject.SetActive(cFGCharacterData.CurrentModel.ActionPoints > 0);
					instance.m_HUD.m_ApPointList[j * 2].gameObject.SetActive(cFGCharacterData.CurrentModel.ActionPoints > 1);
					instance.m_HUD.m_ApPointListBG[j * 2 + 1].gameObject.SetActive(cFGCharacterData.CurrentModel.MaxActionPoints > 1 && !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage);
					instance.m_HUD.m_ApPointListBG[j * 2].gameObject.SetActive(cFGCharacterData.CurrentModel.MaxActionPoints > 1);
					instance.m_HUD.m_Imprisoned[j].gameObject.SetActive(cFGCharacterData.CurrentModel.Imprisoned);
					instance.m_HUD.m_Imprisoned[j].IconNumber = ((cFGCharacterData.CurrentModel == m_SelectedCharacter) ? 1 : 0);
					instance.m_HUD.m_CharExtButtons[j].m_TooltipText = instance2.GetLocalizedText("tac_selcharacter", instance2.GetLocalizedText(cFGCharacterData.CurrentModel.NameId), CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
					num++;
					if (cFGCharacterData.CurrentModel.GunpointState == EGunpointState.Executor)
					{
						instance.m_HUD.SetCharacterAbilityIcons(j, 0);
					}
					else
					{
						instance.m_HUD.SetCharacterAbilityIcons(j, -1);
					}
				}
			}
			else
			{
				instance.m_HUD.m_CharExtButtons[j].interactable = false;
				instance.m_HUD.m_CharExtButtons[j].m_TooltipText = string.Empty;
				instance.m_HUD.m_ApPointList[j * 2 + 1].gameObject.SetActive(value: false);
				instance.m_HUD.m_ApPointList[j * 2].gameObject.SetActive(value: false);
			}
		}
		num = 24;
		instance.m_HUDAbilities.m_SkillButtonsData.Clear();
		if (!m_SelectedCharacter.Imprisoned)
		{
			if (m_SelectedCharacter.HaveAbility(ETurnAction.Gunpoint))
			{
				CFGSkillButtonsData cFGSkillButtonsData = new CFGSkillButtonsData();
				cFGSkillButtonsData.icon = 4;
				cFGSkillButtonsData.data = 7;
				cFGSkillButtonsData.selected = m_CurrentAction == ETurnAction.Gunpoint;
				cFGSkillButtonsData.useme_state = false;
				if (IsActionLimited(ETurnAction.Gunpoint))
				{
					cFGSkillButtonsData.enabled = false;
				}
				else
				{
					cFGSkillButtonsData.enabled = m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint) == EActionResult.Success;
				}
				cFGSkillButtonsData.tooltip_id = instance2.GetLocalizedText("gunpoint_tooltip", CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
				cFGSkillButtonsData.cooldown = 0;
				cFGSkillButtonsData.uses_count = 0;
				cFGSkillButtonsData.icon_list = 0;
				instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData);
				num++;
			}
			int num2 = m_CharactersToShowOnEnemyBar.Count + m_SelectedCharacter.OtherTargets.Count;
			if (m_SelectedCharacter.CurrentWeapon != null)
			{
				CFGSkillButtonsData cFGSkillButtonsData2 = new CFGSkillButtonsData();
				cFGSkillButtonsData2.icon = 1;
				cFGSkillButtonsData2.data = 1;
				cFGSkillButtonsData2.selected = m_CurrentAction == ETurnAction.Shoot;
				cFGSkillButtonsData2.useme_state = false;
				if (IsActionLimited(ETurnAction.Shoot))
				{
					cFGSkillButtonsData2.enabled = false;
				}
				else
				{
					cFGSkillButtonsData2.enabled = m_SelectedCharacter.ActionPoints > 0 && num2 > 0;
				}
				cFGSkillButtonsData2.tooltip_id = instance2.GetLocalizedText("shoot_tooltip", CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
				cFGSkillButtonsData2.cooldown = 0;
				cFGSkillButtonsData2.uses_count = 0;
				cFGSkillButtonsData2.icon_list = 0;
				num++;
				instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData2);
			}
			if (m_SelectedCharacter.HasAltAttack(ETurnAction.AltFire_Fanning))
			{
				CFGSkillButtonsData cFGSkillButtonsData3 = new CFGSkillButtonsData();
				cFGSkillButtonsData3.icon = 12;
				cFGSkillButtonsData3.data = 16;
				cFGSkillButtonsData3.selected = m_CurrentAction == ETurnAction.AltFire_Fanning;
				cFGSkillButtonsData3.useme_state = false;
				if (IsActionLimited(ETurnAction.AltFire_Fanning))
				{
					cFGSkillButtonsData3.enabled = false;
				}
				else
				{
					cFGSkillButtonsData3.enabled = m_SelectedCharacter.ActionPoints > 0 && num2 > 0;
				}
				cFGSkillButtonsData3.tooltip_id = instance2.GetLocalizedText("altfire_fanning_tooltip", CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
				cFGSkillButtonsData3.cooldown = 0;
				cFGSkillButtonsData3.uses_count = 0;
				cFGSkillButtonsData3.icon_list = 0;
				num++;
				instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData3);
			}
			if (m_SelectedCharacter.HasAltAttack(ETurnAction.AltFire_ScopedShot))
			{
				CFGSkillButtonsData cFGSkillButtonsData4 = new CFGSkillButtonsData();
				cFGSkillButtonsData4.icon = 13;
				cFGSkillButtonsData4.data = 17;
				cFGSkillButtonsData4.selected = m_CurrentAction == ETurnAction.AltFire_ScopedShot;
				cFGSkillButtonsData4.useme_state = false;
				if (IsActionLimited(ETurnAction.AltFire_ScopedShot))
				{
					cFGSkillButtonsData4.enabled = false;
				}
				else
				{
					cFGSkillButtonsData4.enabled = m_SelectedCharacter.ActionPoints > 0 && num2 > 0;
				}
				cFGSkillButtonsData4.tooltip_id = instance2.GetLocalizedText("altfire_scopedshot_tooltip", CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
				cFGSkillButtonsData4.cooldown = 0;
				cFGSkillButtonsData4.uses_count = 0;
				cFGSkillButtonsData4.icon_list = 0;
				num++;
				instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData4);
			}
			if (m_SelectedCharacter.HasAltAttack(ETurnAction.AltFire_ConeShot))
			{
				AddTurnActionButton(ETurnAction.AltFire_ConeShot, 11, "altfire_coneshot_tooltip", ref num, num2 > 0);
			}
			if (m_SelectedCharacter.HaveAbility(ETurnAction.Ricochet) && !m_SelectedCharacter.HasAltAttack(ETurnAction.AltFire_ConeShot))
			{
				AddTurnActionButton(ETurnAction.Ricochet, 7, "ricochet_tooltip", ref num, AND: true, OR: true);
			}
			if ((bool)m_SelectedCharacter.CurrentWeapon && m_SelectedCharacter.CurrentWeapon.CanReload())
			{
				CFGSkillButtonsData cFGSkillButtonsData5 = new CFGSkillButtonsData();
				cFGSkillButtonsData5.icon = 2;
				cFGSkillButtonsData5.data = 4;
				cFGSkillButtonsData5.selected = m_CurrentAction == ETurnAction.Reload;
				cFGSkillButtonsData5.useme_state = !m_SelectedCharacter.CurrentWeapon.CanShoot() && m_SelectedCharacter.ActionPoints > 0;
				if (IsActionLimited(ETurnAction.Reload))
				{
					cFGSkillButtonsData5.enabled = false;
				}
				else
				{
					cFGSkillButtonsData5.enabled = m_SelectedCharacter.CanMakeAction(ETurnAction.Reload) == EActionResult.Success;
				}
				cFGSkillButtonsData5.tooltip_id = instance2.GetLocalizedText("reload_tooltip", CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num));
				cFGSkillButtonsData5.cooldown = 0;
				cFGSkillButtonsData5.uses_count = 0;
				cFGSkillButtonsData5.icon_list = 0;
				num++;
				instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData5);
			}
			foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability3 in m_SelectedCharacter.Abilities)
			{
				if (ability3.Key == ETurnAction.Use_Item1 || ability3.Key == ETurnAction.Use_Item2 || ability3.Key == ETurnAction.Use_Talisman)
				{
					CFGAbility ability = ability3.Value.Ability;
					CFGAbility_Item cFGAbility_Item = SelectedCharacter.GetAbility(ability3.Key) as CFGAbility_Item;
					string text = string.Empty;
					if (cFGAbility_Item != null)
					{
						text = instance2.GetLocalizedText(cFGAbility_Item.TextID + "_name").ToUpper();
					}
					CFGSkillButtonsData cFGSkillButtonsData6 = new CFGSkillButtonsData();
					cFGSkillButtonsData6.icon = ability.IconID;
					cFGSkillButtonsData6.data = (int)ability3.Key;
					cFGSkillButtonsData6.selected = m_CurrentAction == ability3.Key;
					cFGSkillButtonsData6.useme_state = false;
					cFGSkillButtonsData6.cooldown = ability.CooldownLeft;
					cFGSkillButtonsData6.uses_count = ability.UsesPerTacticalLeft;
					cFGSkillButtonsData6.icon_list = 1;
					if (IsActionLimited(ability3.Key))
					{
						cFGSkillButtonsData6.enabled = false;
					}
					else
					{
						bool flag = m_SelectedCharacter.CanMakeAction(ability3.Key) == EActionResult.Success;
						cFGSkillButtonsData6.enabled = flag;
					}
					string text2 = CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num);
					num++;
					cFGSkillButtonsData6.tooltip_id = text + " " + text2;
					instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData6);
				}
			}
			foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability4 in m_SelectedCharacter.Abilities)
			{
				if (ability4.Key == ETurnAction.Use_Item1 || ability4.Key == ETurnAction.Use_Item2 || ability4.Key == ETurnAction.Use_Talisman)
				{
					continue;
				}
				string text3 = null;
				ETurnAction key = ability4.Key;
				if (key == ETurnAction.Ricochet)
				{
					continue;
				}
				text3 = instance2.GetLocalizedText(ability4.Key.ToString().ToLower() + "_tooltip");
				CFGAbility ability2 = ability4.Value.Ability;
				if (ability2 != null && !ability2.IsPassive)
				{
					CFGSkillButtonsData cFGSkillButtonsData7 = new CFGSkillButtonsData();
					cFGSkillButtonsData7.icon = ability2.IconID;
					cFGSkillButtonsData7.data = (int)ability4.Key;
					cFGSkillButtonsData7.selected = m_CurrentAction == ability4.Key;
					cFGSkillButtonsData7.useme_state = false;
					cFGSkillButtonsData7.cooldown = ability2.CooldownLeft;
					cFGSkillButtonsData7.uses_count = ability2.UsesPerTacticalLeft;
					cFGSkillButtonsData7.icon_list = 0;
					if (IsActionLimited(ability4.Key))
					{
						cFGSkillButtonsData7.enabled = false;
					}
					else
					{
						bool flag2 = true;
						cFGSkillButtonsData7.enabled = flag2;
					}
					string text4 = CFGStrategicExploratorButtons.FormatShortcut((EActionCommand)num);
					num++;
					string text5 = ((ability2.GetLuckCost() <= m_SelectedCharacter.Luck) ? string.Empty : ("\n" + instance2.GetLocalizedText("tac_noluck", "<color=#ff4646>" + ability2.GetLuckCost() + "</color>")));
					cFGSkillButtonsData7.tooltip_id = text3 + " " + text4 + text5;
					instance.m_HUDAbilities.m_SkillButtonsData.Add(cFGSkillButtonsData7);
				}
			}
			if (m_SelectedCharacter.HaveAbility(ETurnAction.SuicideShot))
			{
				AddTurnActionButton(ETurnAction.SuicideShot, 35, "suicideshot_tooltip", ref num);
			}
		}
		instance.m_HUDAbilities.SetSkillsButtons();
	}

	private void UpdateWeaponsPanel()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		CFGGun currentWeapon = m_SelectedCharacter.CurrentWeapon;
		CFGGun cFGGun = ((m_SelectedCharacter.CurrentWeapon != m_SelectedCharacter.FirstWeapon) ? m_SelectedCharacter.FirstWeapon : m_SelectedCharacter.SecondWeapon);
		bool interactable = cFGGun != null && m_SelectedCharacter.CanMakeAction(ETurnAction.ChangeWeapon) == EActionResult.Success;
		foreach (CFGButtonExtension gunButton in instance.m_HUD.m_GunButtons)
		{
			gunButton.interactable = interactable;
		}
		CFGGun cFGGun2;
		CFGGun cFGGun3;
		if (m_SelectedCharacter.CurrentAction == ETurnAction.ChangeWeapon)
		{
			cFGGun2 = ((!m_IsChangingWeapon) ? cFGGun : currentWeapon);
			cFGGun3 = ((!m_IsChangingWeapon) ? currentWeapon : cFGGun);
		}
		else
		{
			cFGGun2 = currentWeapon;
			cFGGun3 = cFGGun;
		}
		instance.m_HUD.m_GunName.gameObject.SetActive(cFGGun2 != null && cFGGun2.m_Definition != null);
		instance.m_HUD.m_Ammo.gameObject.SetActive(cFGGun2 != null && cFGGun2.m_Definition != null);
		if ((bool)cFGGun2 && cFGGun2.m_Definition != null)
		{
			instance.m_HUD.m_GunName.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cFGGun2.m_Definition.ItemID + "_name");
			instance.m_HUD.m_Ammo.text = cFGGun2.CurrentAmmo + "/" + cFGGun2.AmmoCapacity;
		}
		instance.m_HUD.m_GunLeft.IconNumber = cFGGun2?.TexId ?? 0;
		instance.m_HUD.m_GunRight.IconNumber = cFGGun3?.TexId ?? 0;
		if (!(instance.m_HUD.m_WeaponMark != null) || !instance.m_HUD.m_WeaponMark.gameObject.activeSelf)
		{
			return;
		}
		if (CellUnderCursor != null)
		{
			int num = 0;
			CFGCell start = SelectedCharacter.CurrentCell;
			if (IsUsingRicochet && m_RicochetObjects != null && m_RicochetObjects.Count > 0)
			{
				for (int i = 0; i < m_RicochetObjects.Count; i++)
				{
					if (m_RicochetObjects[i] != null && m_RicochetObjects[i].Cell != null)
					{
						num += CFGCellMap.Distance(start, m_RicochetObjects[i].Cell);
						start = m_RicochetObjects[i].Cell;
					}
				}
			}
			num += CFGCellMap.Distance(start, CellUnderCursor);
			instance.m_HUD.m_WeaponMark.SetWeaponMarkPosition(Mathf.Clamp01((float)num / 40f));
		}
		if (m_SelectedCharacter.CurrentWeapon != null)
		{
			instance.m_HUD.m_WeaponMark.SetParams(m_SelectedCharacter.CurrentWeapon.Class.GetMinDist(), m_SelectedCharacter.CurrentWeapon.Class.GetMaxDist(), Mathf.Abs(m_SelectedCharacter.CurrentWeapon.Class.GetPenaltyMod()), 40);
		}
	}

	private void UpdateEnemiesPanel()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn || m_SelectedCharacter.CurrentAction == ETurnAction.Move)
		{
			instance.m_HUDEnemyPanel.SetEnemiesPanelVisibility(visible: false);
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.None:
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		{
			instance.m_HUDEnemyPanel.SetEnemiesPanelVisibility(visible: true);
			GenerateCharactersToShowOnEnemyBar();
			List<CFGCharacter> charactersToShowOnEnemyBar = CharactersToShowOnEnemyBar;
			for (int i = 0; i < 16; i++)
			{
				if (!(instance.m_HUDEnemyPanel.m_EnemyMarks[i] == null) && !(instance.m_HUDEnemyPanel.m_EnemyMarks[i].gameObject == null))
				{
					if (charactersToShowOnEnemyBar.Count - 1 - i >= 0)
					{
						CFGCharacter cFGCharacter = charactersToShowOnEnemyBar[charactersToShowOnEnemyBar.Count - 1 - i];
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].gameObject.SetActive(value: true);
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].m_IsSelected = m_TargetedObject == cFGCharacter;
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].MaxHP = cFGCharacter.MaxHp;
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].SetHP(cFGCharacter.Hp, (cFGCharacter.BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible);
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].m_HitChance.SetProgress(m_SelectedCharacter.GetChanceToHit(cFGCharacter, null, null, null, m_CurrentAction));
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].m_Data = charactersToShowOnEnemyBar.Count - 1 - i;
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].SetVisibleToPlayerState(CFGCellMap.GetLineOfSightAutoSideSteps(m_SelectedCharacter, cFGCharacter) == ELOXHitType.None);
					}
					else
					{
						instance.m_HUDEnemyPanel.m_EnemyMarks[i].gameObject.SetActive(value: false);
					}
				}
			}
			break;
		}
		default:
			instance.m_HUDEnemyPanel.SetEnemiesPanelVisibility(visible: false);
			break;
		}
	}

	private void UpdateTermsOfShooting_ConfirmButton()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		bool flag2 = !m_FreeTargetingEnabled;
		instance.m_TermsOfShootings.m_ConfirmButton.gameObject.SetActive(flag2 && !flag);
		instance.m_TermsOfShootings.m_AButtonPad.gameObject.SetActive(flag);
		if (flag2 || flag)
		{
			bool flag3 = m_SelectedCharacter.CanMakeAction(m_CurrentAction, m_TargetedObject, m_RicochetObjects) == EActionResult.Success;
			ETurnAction currentAction = m_CurrentAction;
			if ((currentAction == ETurnAction.AltFire_Fanning || currentAction == ETurnAction.AltFire_ScopedShot || currentAction == ETurnAction.Shoot || currentAction == ETurnAction.Ricochet || currentAction == ETurnAction.ArteryShot) && m_SelectedCharacter.GetChanceToHit(m_TargetedObject, m_RicochetObjects, null, null, m_CurrentAction) == 0)
			{
				flag3 = false;
			}
			if (flag)
			{
				instance.m_TermsOfShootings.m_AButtonPad.enabled = flag3;
			}
			else
			{
				instance.m_TermsOfShootings.m_ConfirmButton.enabled = flag3;
			}
		}
	}

	private string GetTargetTypeString(eTargetableType type)
	{
		string text = string.Empty;
		if ((type & eTargetableType.Enemy) == eTargetableType.Enemy)
		{
			text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_target_enemy");
		}
		if ((type & eTargetableType.Friendly) == eTargetableType.Friendly)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_target_ally");
		}
		if ((type & eTargetableType.Cell) == eTargetableType.Cell)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_target_tile");
		}
		if ((type & eTargetableType.Self) == eTargetableType.Self)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("Self");
		}
		if ((type & eTargetableType.Other) == eTargetableType.Other)
		{
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("Other");
		}
		return text;
	}

	private string UpdateTermsOfShooting_ActionInfo()
	{
		EActionResult eActionResult = m_SelectedCharacter.CanMakeAction(m_CurrentAction);
		if ((eActionResult & EActionResult.NotEnoughAP) == EActionResult.NotEnoughAP)
		{
			return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("targeting_problemreason_lowap");
		}
		if (m_CurrentAction.IsStandard())
		{
			CFGAbility ability = m_SelectedCharacter.GetAbility(m_CurrentAction);
			if (ability != null)
			{
				if (!ability.IsReadyToUse)
				{
					return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("targeting_problemreason_cooldown", ability.CooldownLeft.ToString());
				}
				int luckCost = ability.GetLuckCost();
				if (m_SelectedCharacter.Luck - luckCost < 0)
				{
					return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("targeting_problemreason_lowluck", luckCost.ToString());
				}
			}
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		string result = string.Empty;
		switch (m_CurrentAction)
		{
		case ETurnAction.Reload:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("reload_desc_targeting", Mathf.Min(m_SelectedCharacter.CurrentWeapon.AmmoCapacity - m_SelectedCharacter.CurrentWeapon.CurrentAmmo, m_SelectedCharacter.CurrentWeapon.AmmoPerReload).ToString());
			break;
		case ETurnAction.Smell:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("smell_desc_targeting", m_SelectedCharacter.Abilities[m_CurrentAction].Ability.GetRange().ToString());
			break;
		case ETurnAction.ShadowKill:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("shadowkill_desc_targeting", m_SelectedCharacter.Abilities[m_CurrentAction].Ability.GetEffectVal().ToString());
			break;
		case ETurnAction.Shriek:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("shriek_desc_targeting", CFGStaticDataContainer.GetAbilityDef(ETurnAction.Shriek).EffectValue.ToString(), m_SelectedCharacter.Abilities[m_CurrentAction].Ability.GetRange().ToString());
			break;
		case ETurnAction.Cannibal:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("cannibal_desc_targeting", m_SelectedCharacter.Abilities[m_CurrentAction].Ability.GetEffectVal().ToString());
			break;
		case ETurnAction.Gunpoint:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("gunpoint_desc_targeting", m_SelectedCharacter.CharacterData.TotalHeat.ToString());
			if (m_TargetedObject != null)
			{
				EActionResult status = m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, m_TargetedObject);
				if (status.CheckFlag(EActionResult.InvalidTarget))
				{
					result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("targeting_gunpoint_immune");
				}
				else if (status.CheckFlag(EActionResult.NotInCone))
				{
					result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("targeting_gunpoint_insight");
				}
			}
			break;
		case ETurnAction.Ricochet:
			if (m_SelectedCharacter.VisibleRicochetObjects.Count == 0)
			{
				result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ricochet_desc_targeting_none");
			}
			else if (RicochetObjects.Count > 0)
			{
				if (m_TargetedObject != null)
				{
					int chanceToHit = m_SelectedCharacter.GetChanceToHit(m_TargetedObject, m_RicochetObjects, null, null, m_CurrentAction);
					result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText((chanceToHit <= 0) ? "tactical_targeting_0cth" : "ricochet_desc_targeting3");
				}
				else
				{
					result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ricochet_desc_targeting2");
				}
			}
			else
			{
				result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ricochet_desc_targeting");
			}
			break;
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
			result = ((!instance.m_TermsOfShootings.m_ChanceToHitValue.text.Contains(">0%<")) ? ((!m_SelectedCharacter.CurrentWeapon || m_SelectedCharacter.CurrentWeapon.CanShoot()) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CurrentAction.ToString().ToLower() + "_desc_targeting") : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_targeting_noammo")) : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_targeting_0cth"));
			break;
		case ETurnAction.AltFire_Fanning:
			result = ((!instance.m_TermsOfShootings.m_ChanceToHitValue.text.Contains(">0%<")) ? ((!m_SelectedCharacter.CurrentWeapon || m_SelectedCharacter.CurrentWeapon.CanShoot()) ? CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("altfire_fanning_desc_targeting", CFGCharacter.GetTurnActionChanceToHitMod(ETurnAction.AltFire_Fanning).ToString()) : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_targeting_noammo")) : CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_targeting_0cth"));
			break;
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
			if (m_TargetingAbility is CFGAbility_Item cFGAbility_Item)
			{
				result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(cFGAbility_Item.TextID + "_desc");
			}
			break;
		default:
			result = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_CurrentAction.ToString().ToLower() + "_desc_targeting");
			break;
		}
		return result;
	}

	private void UpdateTermsOfShooting()
	{
		CFGTextManager instance = CFGSingletonResourcePrefab<CFGTextManager>.Instance;
		CFGWindowMgr instance2 = CFGSingleton<CFGWindowMgr>.Instance;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		string text = string.Empty;
		string empty = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		string text7 = string.Empty;
		CFGCell cFGCell = ((m_CurrentAction != ETurnAction.Ricochet || m_RicochetObjects == null || m_RicochetObjects.Count <= 0) ? m_SelectedCharacter.CurrentCell : m_RicochetObjects[m_RicochetObjects.Count - 1].Cell);
		CFGCell cFGCell2 = ((m_TargetedObject == null) ? null : m_TargetedObject.CurrentCell);
		ECoverType eCoverType = ((m_TargetedObject != null && m_TargetedObject.GetCoverMult() != 0) ? CFGCharacter.GetTargetCover(cFGCell, cFGCell2) : ECoverType.NONE);
		switch (m_CurrentAction)
		{
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
		{
			if (m_TargetedObject == null)
			{
				break;
			}
			bool flag7 = m_SelectedCharacter.HasLineOfFireTo(m_TargetedObject, m_CurrentAction);
			flag2 = true;
			text3 = instance.GetLocalizedText((!flag7) ? "tactical_targeting_nolineoffire" : "tactical_targeting_chancetohit");
			flag3 = (flag4 = (flag6 = flag7));
			if (!flag3)
			{
				break;
			}
			int chanceToHit2 = m_SelectedCharacter.GetChanceToHit(m_TargetedObject, null, m_SelectedCharacter.CurrentCell, m_TargetedObject.CurrentCell, m_CurrentAction);
			text4 = ((chanceToHit2 <= 0) ? "<color=#EA4242>0%</color>" : (chanceToHit2 + "%"));
			text5 = instance.GetLocalizedText("tactical_targeting_damage");
			text6 = m_SelectedCharacter.CalcDamage(m_TargetedObject, m_CurrentAction).ToString();
			if (m_CurrentAction != ETurnAction.Penetrate)
			{
				switch (eCoverType)
				{
				case ECoverType.NONE:
				{
					float num = CFGCellMap.Distance(cFGCell, cFGCell2);
					text7 = instance.GetLocalizedText((!(num <= 1f)) ? "tactical_targeting_dmg" : "tactical_targeting_dmgadjacent");
					break;
				}
				case ECoverType.HALF:
					text7 = instance.GetLocalizedText("tactical_targeting_dmghalfcover", m_SelectedCharacter.CurrentWeapon.HalfCoverDiv.ToString());
					break;
				default:
					text7 = instance.GetLocalizedText("tactical_targeting_dmgfullcover", m_SelectedCharacter.CurrentWeapon.FullCoverDiv.ToString());
					break;
				}
			}
			break;
		}
		case ETurnAction.AltFire_ConeShot:
		{
			flag2 = (flag3 = true);
			flag4 = false;
			text3 = instance.GetLocalizedText("tactical_targeting_chancetohit");
			List<int> list = new List<int>();
			for (int i = 0; i < m_AOEObjects.Count; i++)
			{
				int chanceToHit3 = m_SelectedCharacter.GetChanceToHit(m_AOEObjects[i], null, null, null, ETurnAction.AltFire_ConeShot);
				list.Add(chanceToHit3);
			}
			list.Sort();
			if (list.Count > 1 && list[0] != list[list.Count - 1])
			{
				text4 = list[0] + "-" + list[list.Count - 1] + "%";
			}
			else if (list.Count > 0)
			{
				text4 = ((list[0] <= 0) ? "<color=#EA4242>0%</color>" : (list[0] + "%"));
			}
			text5 = instance.GetLocalizedText("tactical_targeting_base_damage");
			text6 = (m_SelectedCharacter.CurrentWeapon.Damage + CFGCharacter.GetActionDamageMod(ETurnAction.AltFire_ConeShot)).ToString();
			break;
		}
		case ETurnAction.Ricochet:
			flag2 = (flag3 = (flag4 = (flag6 = RicochetObjects.Count > 0 && m_TargetedObject != null)));
			flag5 = RicochetObjects.Count > 0;
			if (flag3)
			{
				text3 = instance.GetLocalizedText("tactical_targeting_chancetohit");
				int chanceToHit = m_SelectedCharacter.GetChanceToHit(m_TargetedObject, m_RicochetObjects, null, null, ETurnAction.Ricochet);
				text4 = ((chanceToHit <= 0) ? "<color=#EA4242>0%</color>" : (chanceToHit + "%"));
				text5 = instance.GetLocalizedText("tactical_targeting_damage");
				text6 = m_SelectedCharacter.CalcDamage(m_TargetedObject, ETurnAction.Ricochet, m_RicochetObjects[m_RicochetObjects.Count - 1].Cell).ToString();
				text7 = eCoverType switch
				{
					ECoverType.NONE => instance.GetLocalizedText("tactical_targeting_dmg"), 
					ECoverType.HALF => instance.GetLocalizedText("tactical_targeting_dmghalfcover", m_SelectedCharacter.CurrentWeapon.HalfCoverDiv.ToString()), 
					_ => instance.GetLocalizedText("tactical_targeting_dmgfullcover", m_SelectedCharacter.CurrentWeapon.FullCoverDiv.ToString()), 
				};
			}
			break;
		}
		if (m_CurrentAction == ETurnAction.Use_Item1 || m_CurrentAction == ETurnAction.Use_Item2 || m_CurrentAction == ETurnAction.Use_Talisman)
		{
			if (m_TargetingAbility is CFGAbility_Item cFGAbility_Item)
			{
				text = instance.GetLocalizedText(cFGAbility_Item.TextID + "_name");
			}
		}
		else
		{
			text = instance.GetLocalizedText(m_CurrentAction.ToString().ToLower() + "_name");
		}
		empty = UpdateTermsOfShooting_ActionInfo();
		CFGAbility ability = m_SelectedCharacter.GetAbility(m_CurrentAction);
		if (m_TargetedObject != null)
		{
			if (ability == null || !ability.IsSelfCastOnly)
			{
				flag = true;
				text2 = instance.GetLocalizedText(m_TargetedObject.NameId);
			}
		}
		else if (RicochetObjects.Count > 0)
		{
			flag = true;
			text2 = instance.GetLocalizedText(RicochetObjects[RicochetObjects.Count - 1].NameId);
		}
		bool EndTurn = false;
		int aPCost = m_SelectedCharacter.GetAPCost(m_CurrentAction, out EndTurn);
		int luck = ability?.GetLuckCost() ?? 0;
		instance2.m_TermsOfShootings.m_AbilityName.text = text;
		instance2.m_TermsOfShootings.m_EnemyInfo.text = empty;
		instance2.m_TermsOfShootings.AP = aPCost;
		instance2.m_TermsOfShootings.m_EndTurnIcon.gameObject.SetActive(EndTurn);
		instance2.m_TermsOfShootings.Luck = luck;
		instance2.m_TermsOfShootings.m_Name1.gameObject.SetActive(flag);
		if (flag)
		{
			instance2.m_TermsOfShootings.m_Name1.text = text2;
		}
		instance2.m_TermsOfShootings.m_ChanceToHitText.gameObject.SetActive(flag2);
		if (flag2)
		{
			instance2.m_TermsOfShootings.m_ChanceToHitText.text = text3;
		}
		instance2.m_TermsOfShootings.m_ChanceToHitValue.gameObject.SetActive(flag3);
		instance2.m_TermsOfShootings.m_DamageText.gameObject.SetActive(flag3);
		instance2.m_TermsOfShootings.m_DamageVal.gameObject.SetActive(flag3);
		if (flag3)
		{
			instance2.m_TermsOfShootings.m_ChanceToHitValue.text = text4;
			instance2.m_TermsOfShootings.m_DamageText.text = text5;
			instance2.m_TermsOfShootings.m_DamageVal.text = text6;
		}
		instance2.m_TermsOfShootings.m_Info.gameObject.SetActive(flag4);
		if (flag4)
		{
			instance2.m_TermsOfShootings.m_Info.text = text7;
		}
		bool flag8 = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		UpdateTermsOfShooting_ConfirmButton();
		instance2.m_TermsOfShootings.m_NextTargetButton.gameObject.SetActive(flag && !flag8);
		instance2.m_TermsOfShootings.m_PrevTargetButton.gameObject.SetActive(flag && !flag8);
		instance2.m_TermsOfShootings.m_RevertButton.gameObject.SetActive(flag5 && !flag8);
		instance2.m_TermsOfShootings.m_MoreInfoButton.gameObject.SetActive(flag6 && !flag8);
		instance2.m_TermsOfShootings.m_LTButtonPad.gameObject.SetActive(flag && flag8);
		instance2.m_TermsOfShootings.m_RTButtonPad.gameObject.SetActive(flag && flag8);
		instance2.m_TermsOfShootings.m_BBButtonPad.gameObject.SetActive(flag5 && flag8);
		instance2.m_TermsOfShootings.m_LBButtonPad.gameObject.SetActive(flag6 && flag8);
		if (instance2.m_TermsOfShootings.m_MoreInfoWindow.activeSelf)
		{
			UpdateTermsOfShooting_MoreInfo();
		}
	}

	public void OnNextTarget(int a)
	{
		SelectNextTargetable();
	}

	public void OnPrevTarget(int a)
	{
		SelectPrevTargetable();
	}

	private void UpdateHUD()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (!(instance == null) && !(instance.m_HUD == null) && !(instance.m_ActionButtonsPanel == null) && CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			bool isPlayerTurn = CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn;
			instance.m_HUDAbilities.SetEnemyTurnPanelVisibility(!isPlayerTurn);
			bool flag = isPlayerTurn && m_SelectedCharacter != null && m_SelectedCharacter.CharacterData != null && m_SelectedCharacter.IsAlive && m_SelectedCharacter.CurrentAction != ETurnAction.Move;
			if (instance.m_ObjectivePanel != null)
			{
				instance.m_ObjectivePanel.gameObject.SetActive(flag);
			}
			instance.m_HUDEnemyPanel.SetEnemiesPanelVisibility(flag);
			instance.m_HUD.SetSelectionPanelVisibility(flag);
			instance.m_HUD.SetMainBtnPanelVisibility(flag && !m_bTurnEndDisabled);
			instance.m_HUD.SetGunPanelVisibility(flag && !m_SelectedCharacter.Imprisoned);
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null)
			{
				instance.m_ActionButtonsPanel.SetPanelVisible(flag && !component.RotationDisabled && !EndTurnDisabled);
			}
			if (m_SelectedCharacter != null && m_SelectedCharacter.CharacterData != null)
			{
				UpdateSelectionPanel();
				UpdateWeaponsPanel();
				UpdateEnemiesPanel();
			}
			bool flag2 = flag && m_CurrentAction != ETurnAction.None;
			instance.m_TermsOfShootings.gameObject.SetActive(flag2);
			if (flag2)
			{
				UpdateTermsOfShooting();
			}
			if (m_CurrentAction != ETurnAction.None && CFGInput.IsActivated(EActionCommand.Confirm))
			{
				HandleConfirmOrder();
			}
		}
	}

	public void SpawnReactionShotHelpers(CFGPathSimplifier Path)
	{
		DestroyReactionShotHelpers();
		if (m_SelectedCharacter == null || Path == null)
		{
			return;
		}
		Transform reactionShotPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ReactionShotPrefab;
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			if (!CFGOptions.Gameplay.ShowConeOfView || m_SelectedCharacter.HaveAbility(ETurnAction.Disguise))
			{
				return;
			}
			Path.CalculateSeeReactions(m_SelectedCharacter, CheckOnly: true);
			if (!Path.HasReactions)
			{
				return;
			}
			List<MoveAction> seelist = Path.m_Reactions;
			CFGConeOfFire cFGConeOfFire = null;
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
			{
				cFGConeOfFire = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.VisConeHelper;
			}
			if (cFGConeOfFire == null)
			{
				cFGConeOfFire = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireFloorFX;
			}
			if (cFGConeOfFire == null)
			{
				return;
			}
			for (int i = 0; i < seelist.Count; i++)
			{
				if (seelist[i].Action == EMOVEACTION.SUSPICIOUS_CHECK && !(seelist[i].Source == null) && !m_MultiRH.Any((Transform m) => m.transform.position == seelist[i].Source.Position))
				{
					SpawnConeOfView(seelist[i].Source, cFGConeOfFire);
				}
			}
		}
		else
		{
			if (!CFGOptions.Gameplay.ShowReactionRange)
			{
				return;
			}
			Path.CalculateReactions(m_SelectedCharacter, CheckOnly: true);
			if (!Path.HasReactions || reactionShotPrefab == null)
			{
				return;
			}
			float num = 5f;
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo != null)
			{
				num = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RectionShootInfo.m_Range;
			}
			List<MoveAction> reactions = Path.m_Reactions;
			for (int j = 0; j < reactions.Count; j++)
			{
				float num2 = num;
				if (reactions[j].Action == EMOVEACTION.SHOOT_AT && !(reactions[j].Source == null))
				{
					CFGCell currentCell = reactions[j].Source.CurrentCell;
					CFGCell currentCell2 = m_SelectedCharacter.CurrentCell;
					if (currentCell != null && currentCell2 != null && num > 0f)
					{
						float num3 = Mathf.Abs(currentCell.WorldPosition.y - currentCell2.WorldPosition.y);
						float f = Mathf.Asin(num3 / num);
						num2 = num * Mathf.Cos(f);
					}
					num2 *= 2f;
					Vector3 position = reactions[j].Source.Position;
					position.y = reactions[j].Position.y;
					position.y += 0.3f;
					Transform transform = UnityEngine.Object.Instantiate(reactionShotPrefab, position, Quaternion.identity) as Transform;
					if (transform == null)
					{
						break;
					}
					transform.localScale = new Vector3(num2, num2, num2);
					m_MultiRH.Add(transform);
				}
			}
		}
	}

	private void SpawnConeOfView(CFGCharacter Source, CFGConeOfFire prefab = null)
	{
		if (Source == null || m_SelectedCharacter == null || m_SelectedCharacter.HaveAbility(ETurnAction.Disguise) || !CFGOptions.Gameplay.ShowConeOfView)
		{
			return;
		}
		if (ConeOfViewManager.ConesOfFire.TryGetValue(Source, out var value))
		{
			value.m_VisType = EConeVisType.IGNORE_DIST;
			m_MultiRH.Add(value.transform);
			return;
		}
		if (prefab == null)
		{
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
			{
				prefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.VisConeHelper;
			}
			if (prefab == null)
			{
				prefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireFloorFX;
			}
			if (prefab == null)
			{
				return;
			}
		}
		float endLength = m_SelectedCharacter.CharacterData.TotalHeat;
		Vector3 position = Source.Position;
		CFGConeOfFire cFGConeOfFire = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity) as CFGConeOfFire;
		if (!(cFGConeOfFire == null))
		{
			cFGConeOfFire.m_ConeAngle = 90f;
			cFGConeOfFire.m_StartLength = 0f;
			cFGConeOfFire.m_EndLength = endLength;
			cFGConeOfFire.m_VisType = EConeVisType.IGNORE_DIST;
			cFGConeOfFire.m_VisType2 = EConeVisType2.VIEW;
			cFGConeOfFire.RegenerateMesh();
			cFGConeOfFire.transform.position = position;
			cFGConeOfFire.transform.LookAt(position + Source.transform.forward);
			m_MultiRH.Add(cFGConeOfFire.transform);
		}
	}

	private void DestroyReactionShotHelpers()
	{
		if (m_MultiRH == null || m_MultiRH.Count == 0)
		{
			return;
		}
		int i;
		for (i = 0; i < m_MultiRH.Count; i++)
		{
			if (!(m_MultiRH[i] == null))
			{
				CFGConeOfFire cFGConeOfFire = ConeOfViewManager.ConesOfFire.Values.FirstOrDefault((CFGConeOfFire m) => m.transform == m_MultiRH[i]);
				if (cFGConeOfFire != null)
				{
					cFGConeOfFire.m_VisType = EConeVisType.NORMAL;
				}
				else
				{
					UnityEngine.Object.Destroy(m_MultiRH[i].gameObject);
				}
			}
		}
		m_MultiRH.Clear();
	}

	private void RemoveVisualisations()
	{
		if (m_CursorHelper != null)
		{
			UnityEngine.Object.Destroy(m_CursorHelper.gameObject);
			m_CursorHelper = null;
		}
	}

	private void InitConeOnTarget()
	{
		CFGGun currentWeapon = m_SelectedCharacter.CurrentWeapon;
		if (currentWeapon != null && currentWeapon.m_Definition != null && currentWeapon.m_Definition.ConeAngle > 1f)
		{
			Vector3 destPoint = m_SelectedCharacter.CurrentCell.WorldPosition + new Vector3(2f, 0f, 0f);
			if (m_TargetedObject != null)
			{
				destPoint = m_TargetedObject.CurrentCell.WorldPosition;
			}
			ConeOfFireViz_Init(currentWeapon.m_Definition.ConeAngle, m_SelectedCharacter.CurrentCell.WorldPosition, destPoint, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireRange);
		}
	}

	private void ConeOfFireViz_Init(float fAngle, Vector3 Position, Vector3 DestPoint, float Range)
	{
		CFGConeOfFire coneOfFireFloorFX = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireFloorFX;
		if (coneOfFireFloorFX == null)
		{
			Debug.LogWarning("Cone of Fire FX is not set in the GameplaySettings");
			return;
		}
		if (m_ConeVisualization == null)
		{
			m_ConeVisualization = UnityEngine.Object.Instantiate(coneOfFireFloorFX, Position, Quaternion.identity) as CFGConeOfFire;
			if (m_ConeVisualization == null)
			{
				Debug.LogError("Failed to instantiate cone of fire vizualization!");
				return;
			}
		}
		m_ConeVisualization.transform.position = Position;
		m_ConeVisualization.m_ConeAngle = fAngle;
		m_ConeVisualization.m_EndLength = Range;
		m_ConeVisualization.m_VisType = EConeVisType.IGNORE_DIST;
		m_ConeVisualization.m_VisType2 = EConeVisType2.FIRE;
		m_ConeVisualization.RegenerateMesh();
		ConeOfFireViz_RotateTowards(DestPoint);
	}

	private void ConeOfFireViz_RotateTowards(Vector3 DestPoint)
	{
		if (!(m_ConeVisualization == null))
		{
			DestPoint.y = m_ConeVisualization.transform.position.y;
			float num = Vector3.Distance(m_ConeVisualization.transform.position, DestPoint);
			if (!(num < 0.3f))
			{
				m_ConeVisualization.transform.LookAt(DestPoint);
				m_FreeTargetingPoint = DestPoint;
				m_FreeTargetingCell = CFGCellMap.GetCell(m_FreeTargetingPoint);
				ConeOfFire_GenerateTargetList();
			}
		}
	}

	private void ConeOfFireViz_Destroy()
	{
		if (!(m_ConeVisualization == null))
		{
			UnityEngine.Object.Destroy(m_ConeVisualization.gameObject);
		}
	}

	private ECursorHelper GetCursorHelperForCharacterUnderCursor(ref CFGGameObject game_object, CFGGameObject Target)
	{
		game_object = null;
		CFGCharacter cFGCharacter = Target as CFGCharacter;
		if (cFGCharacter == null && m_UC_Cell != null)
		{
			cFGCharacter = m_UC_Cell.CurrentCharacter;
		}
		if (cFGCharacter == null || !cFGCharacter.IsAlive || cFGCharacter.Owner == null)
		{
			return ECursorHelper.None;
		}
		if (m_ActionLimit != 0)
		{
			if (m_ActionLimit != EPlayerActionLimit.Enemy)
			{
				return ECursorHelper.None;
			}
			if (cFGCharacter != m_ActionLimitTargetCharacter)
			{
				return ECursorHelper.None;
			}
		}
		if (cFGCharacter.Owner.IsPlayer)
		{
			game_object = cFGCharacter;
			return ECursorHelper.Select;
		}
		if (!cFGCharacter.IsVisibleByPlayer())
		{
			return ECursorHelper.None;
		}
		game_object = cFGCharacter;
		switch (m_CurrentAction)
		{
		case ETurnAction.Ricochet:
			if (m_PossibleOtherTargets.Contains(cFGCharacter))
			{
				return ECursorHelper.Shoot;
			}
			return ECursorHelper.ShootInactive;
		case ETurnAction.ShadowKill:
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.ShadowKill, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter))
			{
				return ECursorHelper.ShadowKill;
			}
			return ECursorHelper.ShadowKillInactive;
		case ETurnAction.Penetrate:
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.Penetrate, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter))
			{
				return ECursorHelper.Penetrate;
			}
			return ECursorHelper.PenetrateInactive;
		case ETurnAction.Transfusion:
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.Transfusion, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter))
			{
				return ECursorHelper.Transfusion;
			}
			return ECursorHelper.TransfusionInactive;
		case ETurnAction.ArteryShot:
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.ArteryShot, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter))
			{
				return ECursorHelper.ArteryShot;
			}
			return ECursorHelper.ArteryShotInactive;
		case ETurnAction.Gunpoint:
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, cFGCharacter) == EActionResult.Success)
			{
				return ECursorHelper.Gunpoint;
			}
			return ECursorHelper.GunpointInactive;
		case ETurnAction.None:
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter) && (m_CurrentAction == ETurnAction.Gunpoint || m_CurrentAction == ETurnAction.None))
			{
				return ECursorHelper.Gunpoint;
			}
			if (m_SelectedCharacter.CanMakeAction(ETurnAction.Shoot, cFGCharacter) == EActionResult.Success && m_SelectedCharacter.IsEnemyVisible(cFGCharacter))
			{
				return ECursorHelper.Shoot;
			}
			return ECursorHelper.ShootInactive;
		default:
			return ECursorHelper.None;
		}
	}

	private ECursorHelper ObtainCursorState(ref CFGGameObject game_object, out ECursorHelper SecondHelper)
	{
		SecondHelper = ECursorHelper.None;
		if (!CFGRangeBorders.s_DrawRangeBorders)
		{
			return ECursorHelper.None;
		}
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn && m_SelectedCharacter != null && m_SelectedCharacter.CurrentAction == ETurnAction.None)
		{
			if (m_FreeTargetingEnabled)
			{
				switch (m_CurrentAction)
				{
				case ETurnAction.Shoot:
				case ETurnAction.AltFire_Fanning:
				case ETurnAction.AltFire_ScopedShot:
				case ETurnAction.AltFire_ConeShot:
					if (m_ConeVisualization == null)
					{
						if (m_FreeTargetingCanShoot)
						{
							return ECursorHelper.Shoot;
						}
						return ECursorHelper.ShootInactive;
					}
					return ECursorHelper.None;
				case ETurnAction.Use_Item1:
				case ETurnAction.Use_Item2:
				case ETurnAction.Use_Talisman:
					if (m_TargetingAbility != null)
					{
						if (m_TargetingAbility.AnimationType == eAbilityAnimation.Throw1 || m_TargetingAbility.AnimationType == eAbilityAnimation.Throw2 || m_TargetingAbility.AnimationType == eAbilityAnimation.ThrowAuto)
						{
							if (m_FreeTargetingCanShoot)
							{
								return ECursorHelper.Throwable;
							}
							return ECursorHelper.ThrowableInactive;
						}
						if (m_FreeTargetingCanShoot)
						{
							return ECursorHelper.Use;
						}
						return ECursorHelper.UseInactive;
					}
					return ECursorHelper.None;
				}
			}
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
			{
				return GetCursorStateForController(ref game_object, out SecondHelper);
			}
			ECursorHelper cursorHelperForCharacterUnderCursor = GetCursorHelperForCharacterUnderCursor(ref game_object, m_GameObjectUnderCursor);
			if (game_object != null)
			{
				return cursorHelperForCharacterUnderCursor;
			}
			CFGUsableObject cFGUsableObject = m_GameObjectUnderCursor as CFGUsableObject;
			CFGRicochetObject cFGRicochetObject = m_GameObjectUnderCursor as CFGRicochetObject;
			CFGDoorObject cFGDoorObject = m_GameObjectUnderCursor as CFGDoorObject;
			CFGShootableObject cFGShootableObject = m_GameObjectUnderCursor as CFGShootableObject;
			if (cFGUsableObject != null)
			{
				if (m_ActionLimit == EPlayerActionLimit.Default || (m_ActionLimit == EPlayerActionLimit.Usable && cFGUsableObject == m_ActionLimitTargetUsable))
				{
					game_object = cFGUsableObject;
					if (m_SelectedCharacter.CanMakeAction(ETurnAction.Use, cFGUsableObject) == EActionResult.Success)
					{
						return (!cFGUsableObject.IsDynamicCover) ? ECursorHelper.Use : ECursorHelper.CreateCover;
					}
					if (cFGUsableObject.CanBeUsed())
					{
						return (!cFGUsableObject.IsDynamicCover) ? ECursorHelper.UseInactive : ECursorHelper.CreateCoverInactive;
					}
				}
				return ECursorHelper.None;
			}
			if (cFGDoorObject != null)
			{
				game_object = cFGDoorObject;
				if (cFGDoorObject.IsLocked && m_ActionLimit == EPlayerActionLimit.Default)
				{
					return ECursorHelper.OpenDoorLocked;
				}
				return ECursorHelper.None;
			}
			if (cFGShootableObject != null && cFGShootableObject.IsAlive)
			{
				if (m_ActionLimit == EPlayerActionLimit.Default)
				{
					game_object = cFGShootableObject;
					if (IsUsingRicochet)
					{
						if (m_PossibleOtherTargets.Contains(cFGShootableObject))
						{
							return ECursorHelper.Shoot;
						}
					}
					else if (m_SelectedCharacter.OtherTargets.Contains(cFGShootableObject))
					{
						return ECursorHelper.Shoot;
					}
				}
				return ECursorHelper.ShootInactive;
			}
			if (cFGRicochetObject != null && m_SelectedCharacter.HaveAbility(ETurnAction.Ricochet))
			{
				if (m_ActionLimit == EPlayerActionLimit.Default)
				{
					game_object = cFGRicochetObject;
					if (m_SelectedCharacter.CanMakeAction(ETurnAction.Ricochet) == EActionResult.Success)
					{
						if (IsUsingRicochet)
						{
							if (AvailableRicochetObjects.Contains(cFGRicochetObject))
							{
								return ECursorHelper.Shoot;
							}
						}
						else if (m_SelectedCharacter.VisibleRicochetObjects.Contains(cFGRicochetObject))
						{
							return ECursorHelper.Shoot;
						}
					}
				}
				return ECursorHelper.ShootInactive;
			}
			if (m_UC_Cell != null && !m_UC_Cell.CheckFlag(1, 8) && !m_UC_Cell.CheckFlag(1, 2))
			{
				if (m_UC_Cell.StairsType != CFGCell.EStairsType.Slope)
				{
					if (m_ActionLimit == EPlayerActionLimit.Default || (m_ActionLimit == EPlayerActionLimit.MoveToTile && m_UC_Cell == m_ActionLimitTargetCell))
					{
						return (m_SelectedCharacter.CanMakeAction(ETurnAction.Move) == EActionResult.Success && m_SelectedCharacter.IsCellInMoveRange(m_UC_Cell)) ? ECursorHelper.Move : ECursorHelper.MoveInactive;
					}
					return ECursorHelper.MoveInactive;
				}
				CFGCamera component = Camera.main.GetComponent<CFGCamera>();
				if (component != null && CurrentAction == ETurnAction.None && m_UC_Cell.StairsType == CFGCell.EStairsType.Slope)
				{
					if (m_UC_Cell.Floor >= (int)component.CurrentFloorLevel)
					{
						return ECursorHelper.MoveStairsUp;
					}
					return ECursorHelper.MoveStairsDown;
				}
			}
		}
		return ECursorHelper.None;
	}

	private ECursorHelper GetCursorStateForController(ref CFGGameObject TargetGameObject, out ECursorHelper State2)
	{
		ECursorHelper eCursorHelper = ECursorHelper.None;
		TargetGameObject = null;
		State2 = ECursorHelper.None;
		if (m_UC_Cell != null && !m_UC_Cell.CheckFlag(1, 8) && !m_UC_Cell.CheckFlag(1, 2))
		{
			if (m_UC_Cell.StairsType != CFGCell.EStairsType.Slope)
			{
				eCursorHelper = ((m_ActionLimit != 0 && (m_ActionLimit != EPlayerActionLimit.MoveToTile || m_UC_Cell != m_ActionLimitTargetCell)) ? ECursorHelper.MoveInactive : ((m_SelectedCharacter.CanMakeAction(ETurnAction.Move) == EActionResult.Success && m_SelectedCharacter.IsCellInMoveRange(m_UC_Cell)) ? ECursorHelper.Move : ECursorHelper.MoveInactive));
			}
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component != null && CurrentAction == ETurnAction.None && m_UC_Cell.StairsType == CFGCell.EStairsType.Slope)
			{
				if (m_UC_Cell.Floor >= (int)component.CurrentFloorLevel)
				{
					return ECursorHelper.MoveStairsUp;
				}
				return ECursorHelper.MoveStairsDown;
			}
		}
		CFGGameObject gameObjectUnderCell = GetGameObjectUnderCell();
		ECursorHelper cursorHelperForCharacterUnderCursor = GetCursorHelperForCharacterUnderCursor(ref TargetGameObject, gameObjectUnderCell);
		if (TargetGameObject != null)
		{
			return cursorHelperForCharacterUnderCursor;
		}
		if (gameObjectUnderCell == null)
		{
			return eCursorHelper;
		}
		CFGDoorObject cFGDoorObject = gameObjectUnderCell as CFGDoorObject;
		if (cFGDoorObject != null)
		{
			if (m_ActionLimit == EPlayerActionLimit.Default)
			{
				TargetGameObject = cFGDoorObject;
				if (cFGDoorObject.IsLocked)
				{
					State2 = ECursorHelper.OpenDoorLocked;
					return eCursorHelper;
				}
				if ((bool)cFGDoorObject.CanCharacterUse(m_SelectedCharacter, bUseEvent: false, bCheckPath: true, out var UseMode) && !cFGDoorObject.IsOpened)
				{
					if (UseMode != 0)
					{
						State2 = ECursorHelper.OpenDoorGamepad;
					}
					return eCursorHelper;
				}
			}
			return eCursorHelper;
		}
		CFGUsableObject cFGUsableObject = gameObjectUnderCell as CFGUsableObject;
		if ((bool)cFGUsableObject)
		{
			if (m_ActionLimit == EPlayerActionLimit.Default || (m_ActionLimit == EPlayerActionLimit.Usable && cFGUsableObject == m_ActionLimitTargetUsable))
			{
				TargetGameObject = cFGUsableObject;
				if (m_SelectedCharacter.CanMakeAction(ETurnAction.Use, cFGUsableObject) == EActionResult.Success)
				{
					State2 = ((!cFGUsableObject.IsDynamicCover) ? ECursorHelper.Use : ECursorHelper.CreateCover);
				}
				else if (cFGUsableObject.CanBeUsed())
				{
					State2 = ((!cFGUsableObject.IsDynamicCover) ? ECursorHelper.UseInactive : ECursorHelper.CreateCoverInactive);
				}
				switch (eCursorHelper)
				{
				case ECursorHelper.Move:
					if (State2 == ECursorHelper.UseInactive)
					{
						State2 = ECursorHelper.None;
					}
					return eCursorHelper;
				case ECursorHelper.MoveInactive:
					if (State2 == ECursorHelper.UseInactive && !m_UC_Cell.CheckIfClosed())
					{
						State2 = ECursorHelper.None;
						return eCursorHelper;
					}
					return ECursorHelper.None;
				}
			}
			return ECursorHelper.None;
		}
		CFGRicochetObject cFGRicochetObject = gameObjectUnderCell as CFGRicochetObject;
		if ((bool)cFGRicochetObject)
		{
			if (m_ActionLimit == EPlayerActionLimit.Default)
			{
				TargetGameObject = cFGRicochetObject;
				if (IsUsingRicochet)
				{
					if (AvailableRicochetObjects.Contains(cFGRicochetObject))
					{
						return ECursorHelper.Shoot;
					}
				}
				else if (m_SelectedCharacter.CanMakeAction(ETurnAction.Ricochet) == EActionResult.Success && m_SelectedCharacter.VisibleRicochetObjects.Contains(cFGRicochetObject))
				{
					return ECursorHelper.Shoot;
				}
			}
			return ECursorHelper.ShootInactive;
		}
		CFGShootableObject cFGShootableObject = gameObjectUnderCell as CFGShootableObject;
		if ((bool)cFGShootableObject)
		{
			if (m_ActionLimit == EPlayerActionLimit.Default)
			{
				TargetGameObject = cFGShootableObject;
				if (IsUsingRicochet)
				{
					if (m_PossibleOtherTargets.Contains(cFGShootableObject))
					{
						return ECursorHelper.Shoot;
					}
				}
				else if (m_SelectedCharacter.OtherTargets.Contains(cFGShootableObject))
				{
					return ECursorHelper.Shoot;
				}
			}
			return ECursorHelper.ShootInactive;
		}
		return ECursorHelper.None;
	}

	private void HideControllerHelpers()
	{
		if (m_CursorHelper2 != null)
		{
			UnityEngine.Object.Destroy(m_CursorHelper2.gameObject);
			m_CursorHelper2 = null;
		}
		if (m_CursorHelper3 != null)
		{
			UnityEngine.Object.Destroy(m_CursorHelper3.gameObject);
			m_CursorHelper3 = null;
		}
	}

	private void UpdateControllerHelpers()
	{
		HideControllerHelpers();
		if (CFGInput.LastReadInputDevice != EInputMode.Gamepad || m_UC_Cell == null)
		{
			return;
		}
		float num = m_LastRealGamepadPos.y - m_LastGamepadPos.y;
		if (num < 1f)
		{
			return;
		}
		int Floor = 0;
		int PosX = 0;
		int PosZ = 0;
		m_UC_Cell.DecodePosition(out PosX, out PosZ, out Floor);
		Vector3 vector = new Vector3((float)PosX + 0.5f, (float)Floor * 2.5f, (float)PosZ + 0.5f);
		Transform anotherFloor = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_AnotherFloor;
		if (anotherFloor == null)
		{
			return;
		}
		Vector3 lastRealGamepadPos = m_LastRealGamepadPos;
		if (num > 3f)
		{
			lastRealGamepadPos = vector;
			lastRealGamepadPos.y += 5f;
			m_CursorHelper3 = UnityEngine.Object.Instantiate(anotherFloor);
			if (m_CursorHelper3 != null)
			{
				m_CursorHelper3.position = lastRealGamepadPos;
			}
		}
		if (num > 1f)
		{
			lastRealGamepadPos = vector;
			lastRealGamepadPos.y += 2.5f;
			m_CursorHelper2 = UnityEngine.Object.Instantiate(anotherFloor);
			if (m_CursorHelper2 != null)
			{
				m_CursorHelper2.position = lastRealGamepadPos;
			}
		}
	}

	private Transform GetCursorHelperFromState(ECursorHelper helper)
	{
		return helper switch
		{
			ECursorHelper.Move => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Move, 
			ECursorHelper.MoveInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_MoveInactive, 
			ECursorHelper.MoveVert => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_MoveVert, 
			ECursorHelper.Shoot => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Shoot, 
			ECursorHelper.ShootInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ShootInactive, 
			ECursorHelper.Use => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Use, 
			ECursorHelper.UseInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_UseInactive, 
			ECursorHelper.CreateCover => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_CreateCover, 
			ECursorHelper.CreateCoverInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_CreateCoverInactive, 
			ECursorHelper.Gunpoint => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Gunpoint, 
			ECursorHelper.GunpointInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_GunpointInactive, 
			ECursorHelper.Select => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Select, 
			ECursorHelper.OpenDoor => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_OpenDoor, 
			ECursorHelper.OpenDoorInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_OpenDoorInactive, 
			ECursorHelper.OpenDoorLocked => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_OpenDoorLocked, 
			ECursorHelper.OpenDoorGamepad => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_OpenDoorGamepad, 
			ECursorHelper.ShadowKill => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ShadowKill, 
			ECursorHelper.ShadowKillInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ShadowKillInactive, 
			ECursorHelper.Penetrate => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Penetrate, 
			ECursorHelper.PenetrateInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_PenetrateInactive, 
			ECursorHelper.Transfusion => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Transfusion, 
			ECursorHelper.TransfusionInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_TransfusionInactive, 
			ECursorHelper.ArteryShot => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ArteryShot, 
			ECursorHelper.ArteryShotInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ArteryShotInactive, 
			ECursorHelper.MoveStairsUp => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_MoveStairsUp, 
			ECursorHelper.MoveStairsDown => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_MoveStairsDown, 
			ECursorHelper.Throwable => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_Throwable, 
			ECursorHelper.ThrowableInactive => CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_CursorHelpers.m_ThrowableInactive, 
			_ => null, 
		};
	}

	private void UpdateCursorHelperPosition(CFGGameObject Target, Transform Helper, bool bUseCellPos)
	{
		if (Target == null)
		{
			return;
		}
		Collider component = Target.GetComponent<Collider>();
		if (component != null && (m_CursorHelperState == ECursorHelper.Use || m_CursorHelperState == ECursorHelper.UseInactive || m_CursorHelperAction == ECursorHelper.Use || m_CursorHelperAction == ECursorHelper.UseInactive || m_CursorHelperState == ECursorHelper.CreateCover || m_CursorHelperState == ECursorHelper.CreateCoverInactive || m_CursorHelperAction == ECursorHelper.CreateCover || m_CursorHelperAction == ECursorHelper.CreateCoverInactive))
		{
			Vector3 center = component.bounds.center;
			center.y += component.bounds.extents.y;
			Helper.position = center;
			return;
		}
		if (bUseCellPos && m_UC_Cell != null)
		{
			Helper.position = m_UC_Cell.WorldPosition;
		}
		else
		{
			Helper.position = Target.Transform.position;
		}
		if (Target as CFGCharacter == null)
		{
			Helper.rotation = Target.Transform.rotation;
		}
	}

	private void UpdateCursorHelper()
	{
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			UpdateCursorHelper_Strategic();
			return;
		}
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && m_SelectedCharacter != null && m_UC_Cell != null && m_UC_Cell.CurrentCharacter != null && m_UC_Cell.CurrentCharacter.Owner != null && m_UC_Cell.CurrentCharacter.Owner.IsAi && m_UC_Cell.CurrentCharacter.IsVisibleByPlayer() && m_UC_Cell.CurrentCharacter.GunpointState != EGunpointState.Target)
		{
			DestroyReactionShotHelpers();
			if (m_CurrentAction == ETurnAction.None && m_SelectedCharacter.CanMakeAction(ETurnAction.Move) == EActionResult.Success)
			{
				SpawnConeOfView(m_UC_Cell.CurrentCharacter);
			}
		}
		UpdateControllerHelpers();
		CFGGameObject game_object = null;
		ECursorHelper SecondHelper = ECursorHelper.None;
		ECursorHelper eCursorHelper = ObtainCursorState(ref game_object, out SecondHelper);
		if (m_CursorHelperAction != SecondHelper)
		{
			if (m_CursorHelper_Action != null)
			{
				UnityEngine.Object.Destroy(m_CursorHelper_Action.gameObject);
				m_CursorHelper_Action = null;
			}
			m_CursorHelperAction = SecondHelper;
			Transform cursorHelperFromState = GetCursorHelperFromState(m_CursorHelperAction);
			if (cursorHelperFromState != null)
			{
				m_CursorHelper_Action = UnityEngine.Object.Instantiate(cursorHelperFromState);
			}
		}
		if (m_CursorHelper_Action != null)
		{
			if (m_UC_Cell != null)
			{
				int Floor = 0;
				int PosX = 0;
				int PosZ = 0;
				m_UC_Cell.DecodePosition(out PosX, out PosZ, out Floor);
				Vector3 position = new Vector3((float)PosX + 0.5f, (float)Floor * 2.5f, (float)PosZ + 0.5f);
				m_CursorHelper_Action.position = position;
			}
			if (game_object != null)
			{
				UpdateCursorHelperPosition(game_object, m_CursorHelper_Action, bUseCellPos: false);
			}
		}
		ECursorHelper eCursorHelper2 = eCursorHelper;
		if (eCursorHelper2 == ECursorHelper.Shoot || eCursorHelper2 == ECursorHelper.ShootInactive)
		{
			HideControllerHelpers();
		}
		if (m_CursorHelperState != eCursorHelper)
		{
			if (m_CursorHelper != null)
			{
				UnityEngine.Object.Destroy(m_CursorHelper.gameObject);
				m_CursorHelper = null;
			}
			m_CursorHelperState = eCursorHelper;
			Transform cursorHelperFromState2 = GetCursorHelperFromState(m_CursorHelperState);
			if (cursorHelperFromState2 != null)
			{
				m_CursorHelper = UnityEngine.Object.Instantiate(cursorHelperFromState2);
			}
		}
		if (m_CursorHelper == null)
		{
			return;
		}
		if (game_object != null)
		{
			UpdateCursorHelperPosition(game_object, m_CursorHelper, bUseCellPos: true);
		}
		else if (m_UC_Cell != null)
		{
			int Floor2 = 0;
			int PosX2 = 0;
			int PosZ2 = 0;
			m_UC_Cell.DecodePosition(out PosX2, out PosZ2, out Floor2);
			Vector3 position2 = new Vector3((float)PosX2 + 0.5f, (float)Floor2 * 2.5f, (float)PosZ2 + 0.5f);
			m_CursorHelper.position = position2;
			if (m_CursorHelperState == ECursorHelper.MoveStairsUp || m_CursorHelperState == ECursorHelper.MoveStairsDown)
			{
				m_CursorHelper.position += new Vector3(0f, m_UC_Cell.DeltaHeight, 0f);
			}
		}
	}

	private void SetCursorPrefab_Strategic(m_StrategicCursors cursorType)
	{
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		Transform transform = null;
		string text = string.Empty;
		switch (cursorType)
		{
		case m_StrategicCursors.Normal:
			transform = instance.m_CursorHelperPrefab;
			text = "Str_Cursor_Normal";
			break;
		case m_StrategicCursors.Inaccesible:
			transform = instance.m_CursorHelperInaccesiblePrefab;
			text = "Str_Cursor_Inaccesible";
			break;
		case m_StrategicCursors.Entrance:
			transform = instance.m_CursorHelperEntrancePrefab;
			text = "Str_Cursor_Entrance";
			break;
		}
		if (m_CursorHelper.gameObject.tag != text && transform != null)
		{
			UnityEngine.Object.Destroy(m_CursorHelper.gameObject);
			m_CursorHelper = UnityEngine.Object.Instantiate(transform);
		}
	}

	private void UpdateCursorHelper_Strategic()
	{
		if (m_CursorHelper == null)
		{
			return;
		}
		bool flag = (CFGInput.LastReadInputDevice == EInputMode.KeyboardAndMouse) & CFGSingleton<CFGWindowMgr>.Instance.IsCursorOverUI();
		if (m_UC_Cell == null || !(m_SelectedCharacter != null) || flag)
		{
			return;
		}
		int Floor = 0;
		int PosX = 0;
		int PosZ = 0;
		m_UC_Cell.DecodePosition(out PosX, out PosZ, out Floor);
		Vector3 position = new Vector3((float)PosX + 0.5f, (float)Floor * 2.5f, (float)PosZ + 0.5f);
		if (!m_UC_Cell.CheckFlag(1, 8) && !m_UC_Cell.CheckFlag(1, 2))
		{
			CFGLocationObject cFGLocationObject = ObjectUnderCursor as CFGLocationObject;
			if (cFGLocationObject != null && cFGLocationObject.State == ELocationState.OPEN)
			{
				SetCursorPrefab_Strategic(m_StrategicCursors.Entrance);
			}
			else
			{
				SetCursorPrefab_Strategic(m_StrategicCursors.Normal);
			}
			if ((ObjectUnderCursor == null || ObjectUnderCursor.NameId == "Horseman" || ((bool)cFGLocationObject && cFGLocationObject.State == ELocationState.HIDDEN)) && m_LastGamepadPos != CFGMath.INIFITY3)
			{
				position = GetPointOnStrategic(m_LastGamepadPos);
			}
		}
		else
		{
			SetCursorPrefab_Strategic(m_StrategicCursors.Inaccesible);
			position = GetPointOnStrategic(m_LastGamepadPos);
		}
		position.y += 0.06f;
		m_CursorHelper.position = position;
	}

	private Vector3 GetPointOnStrategic(Vector3 point)
	{
		Vector3 result = point;
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			Vector3 vector = Vector3.zero;
			vector.y = -1f;
			float num = 1.01E+10f;
			bool flag = false;
			Ray ray = new Ray(point + new Vector3(0f, 20f, 0f), Vector3.down);
			RaycastHit[] array = Physics.RaycastAll(ray);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.collider.GetComponent<AmplifyColorVolume>() != null || raycastHit.distance > num || raycastHit.collider.name == "Horseman")
				{
					continue;
				}
				if (raycastHit.collider.name.Contains("terrain"))
				{
					vector = raycastHit.point;
					flag = true;
					num = raycastHit.distance;
					continue;
				}
				CFGLocationObject component = raycastHit.collider.GetComponent<CFGLocationObject>();
				if (!component || component.State != ELocationState.HIDDEN)
				{
					float y = raycastHit.collider.transform.position.y;
					if (y >= 0f && (vector.y < 0f || y < vector.y))
					{
						vector = raycastHit.collider.transform.position;
						flag = false;
						num = raycastHit.distance;
					}
				}
			}
			if (flag)
			{
				result = vector;
			}
		}
		return result;
	}

	private void SpawnRangeHelper(Vector3 position, float range)
	{
		if (!(m_RangeHelper != null))
		{
			Transform rangeHelperPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RangeHelperPrefab;
			if (rangeHelperPrefab != null)
			{
				position.y += 0.3f;
				m_RangeHelper = UnityEngine.Object.Instantiate(rangeHelperPrefab, position, Quaternion.identity) as Transform;
				range *= 2f;
				m_RangeHelper.localScale = new Vector3(range, range, range);
			}
		}
	}

	private void DestroyRangeHelper()
	{
		if (m_RangeHelper != null)
		{
			UnityEngine.Object.Destroy(m_RangeHelper.gameObject);
			m_RangeHelper = null;
		}
	}

	private void SpawnAOECircleHelper(Vector3 position, float range)
	{
		if (m_AOE_CircleHelper != null)
		{
			return;
		}
		Transform transform = null;
		if (m_TargetingAbility != null && CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData != null)
		{
			switch (m_TargetingAbility.AOECircleHelper)
			{
			case eAOECircleHelper.Default:
				transform = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_AOE_Circle_Default;
				break;
			case eAOECircleHelper.Explosion:
				transform = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_AOE_Circle_Explosion;
				break;
			}
		}
		if (transform == null)
		{
			transform = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RangeHelperPrefab;
		}
		if (!(transform == null))
		{
			position.y += 1.3f;
			m_AOE_CircleHelper = UnityEngine.Object.Instantiate(transform, position, Quaternion.identity) as Transform;
			range *= 2f;
			m_AOE_CircleHelper.localScale = new Vector3(range, range, range);
		}
	}

	private void DestroyAOEHelper()
	{
		if (!(m_AOE_CircleHelper == null))
		{
			UnityEngine.Object.Destroy(m_AOE_CircleHelper.gameObject);
			m_AOE_CircleHelper = null;
		}
	}

	private void RenderRicochetHelpers()
	{
		if (!IsUsingRicochet || m_SelectedCharacter == null || m_SelectedCharacter.CanMakeAction(ETurnAction.Ricochet) != 0)
		{
			return;
		}
		Material ricochetLineMat = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RicochetLineMat;
		Color color = new Color(0.94f, 0.72f, 0.35f, 1f);
		Vector3 start = m_SelectedCharacter.Transform.position;
		start.y += 1.1f;
		for (int i = 0; i < m_RicochetObjects.Count; i++)
		{
			Vector3 position = m_RicochetObjects[i].Transform.position;
			position.y += 1.1f;
			CFGQuadRenderer.DrawQuadBB(start, position, 1f, color, ricochetLineMat);
			start = position;
		}
		if (m_TargetedObject == null)
		{
			CFGGameObject cFGGameObject = ((!(m_GameObjectUnderCursor != null)) ? GetGameObjectUnderCell() : m_GameObjectUnderCursor);
			CFGRicochetObject cFGRicochetObject = cFGGameObject as CFGRicochetObject;
			CFGCharacter cFGCharacter = cFGGameObject as CFGCharacter;
			if (cFGRicochetObject != null || (cFGCharacter != null && cFGCharacter.BestDetectionType != 0 && cFGCharacter.IsAlive))
			{
				Vector3 position = cFGGameObject.Transform.position;
				position.y += 1.1f;
				if ((cFGRicochetObject != null && m_AvailableRicochetObjects.Contains(cFGRicochetObject)) || (cFGCharacter != null && cFGCharacter.IsAlive && m_PossibleOtherTargets.Contains(cFGCharacter)))
				{
					CFGQuadRenderer.DrawQuadBB(start, position, 1f, color, ricochetLineMat);
				}
				else
				{
					CFGQuadRenderer.DrawQuadBB(start, position, 1f, Color.red, ricochetLineMat);
				}
			}
			else if (m_UC_Cell != null)
			{
				Vector3 position = m_UC_Cell.WorldPosition;
				position.y += 1.1f;
				Color gray = Color.gray;
				gray.a = 0.5f;
				CFGQuadRenderer.DrawQuadBB(start, position, 1f, gray, ricochetLineMat);
			}
		}
		else
		{
			Vector3 position = m_TargetedObject.Position;
			position.y += 1.1f;
			CFGQuadRenderer.DrawQuadBB(start, position, 1f, color, ricochetLineMat);
		}
	}

	public void RenderFinderHelpers(int _pom)
	{
		DestroyFinderHelpers(0);
		if (SelectedCharacter == null || SelectedCharacter.CurrentCell == null || CurrentAction == ETurnAction.Cannibal)
		{
			return;
		}
		HashSet<CFGCharacter> characters = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters;
		if (characters == null)
		{
			return;
		}
		CFGCharacter[] array = characters.Where((CFGCharacter m) => m.IsDead && !m.IsCorpseLooted && m.isActiveAndEnabled && m.IsVisibleByPlayer()).ToArray();
		if (array == null || array.Length == 0)
		{
			return;
		}
		if (m_FinderHelpersVis == null)
		{
			m_FinderHelpersVis = new HashSet<Transform>();
		}
		CFGCharacter[] array2 = array;
		CFGCharacter corpse;
		for (int i = 0; i < array2.Length; i++)
		{
			corpse = array2[i];
			if (corpse.CurrentCell != null && corpse.CurrentCell != SelectedCharacter.CurrentCell && !m_FinderHelpersVis.Any((Transform m) => m.position == corpse.CurrentCell.WorldPosition))
			{
				Transform transform = ((!SelectedCharacter.IsCellInMoveRange(corpse.CurrentCell)) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorInactiveVisPrefab : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorVisPrefab);
				if (!(transform == null))
				{
					Transform transform2 = UnityEngine.Object.Instantiate(transform);
					transform2.position = corpse.CurrentCell.WorldPosition;
					m_FinderHelpersVis.Add(transform2);
				}
			}
		}
	}

	public void DestroyFinderHelpers(int _pom)
	{
		if (m_FinderHelpersVis == null || CurrentAction == ETurnAction.Finder)
		{
			return;
		}
		foreach (Transform finderHelpersVi in m_FinderHelpersVis)
		{
			UnityEngine.Object.Destroy(finderHelpersVi.gameObject);
		}
		m_FinderHelpersVis.Clear();
	}

	public void RenderCannibalHelpers(int _pom)
	{
		DestroyCannibalHelpers(0);
		if (SelectedCharacter == null || SelectedCharacter.CurrentCell == null || CurrentAction == ETurnAction.Finder)
		{
			return;
		}
		HashSet<CFGCharacter> characters = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters;
		if (characters == null)
		{
			return;
		}
		CFGCharacter[] array = characters.Where((CFGCharacter m) => m.IsDead && m.isActiveAndEnabled && m.IsVisibleByPlayer()).ToArray();
		if (array == null || array.Length == 0)
		{
			return;
		}
		if (m_CannibalHelpersVis == null)
		{
			m_CannibalHelpersVis = new HashSet<Transform>();
		}
		CFGCharacter[] array2 = array;
		CFGCharacter corpse;
		for (int i = 0; i < array2.Length; i++)
		{
			corpse = array2[i];
			if (corpse.CurrentCell != null && corpse.CurrentCell != SelectedCharacter.CurrentCell && !m_CannibalHelpersVis.Any((Transform m) => m.position == corpse.CurrentCell.WorldPosition))
			{
				Transform transform = ((!SelectedCharacter.IsCellInMoveRange(corpse.CurrentCell)) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorInactiveVisPrefab : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActivatorVisPrefab);
				if (!(transform == null))
				{
					Transform transform2 = UnityEngine.Object.Instantiate(transform);
					transform2.position = corpse.CurrentCell.WorldPosition;
					m_CannibalHelpersVis.Add(transform2);
				}
			}
		}
	}

	public void DestroyCannibalHelpers(int _pom)
	{
		if (m_CannibalHelpersVis == null || CurrentAction == ETurnAction.Cannibal)
		{
			return;
		}
		foreach (Transform cannibalHelpersVi in m_CannibalHelpersVis)
		{
			UnityEngine.Object.Destroy(cannibalHelpersVi.gameObject);
		}
		m_CannibalHelpersVis.Clear();
	}

	private void SpawnOrderConfirmationFx(Vector3 position)
	{
		if (!CFGRangeBorders.s_DrawRangeBorders)
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			Transform orderConfirmationFxPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_OrderConfirmationFxPrefab;
			if (orderConfirmationFxPrefab != null)
			{
				UnityEngine.Object.Instantiate(orderConfirmationFxPrefab, position, Quaternion.identity);
			}
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_ConfirmMoveTactical, position);
			return;
		}
		Transform orderConfirmationFxStrategicPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_OrderConfirmationFxStrategicPrefab;
		if (orderConfirmationFxStrategicPrefab != null && m_CursorHelper != null)
		{
			UnityEngine.Object.Instantiate(orderConfirmationFxStrategicPrefab, m_CursorHelper.position, Quaternion.identity);
		}
		CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_ConfirmMoveStrategic, position);
		if ((bool)m_CursorHelper)
		{
			m_CursorHelper.GetComponent<CFGStrategicCursor>().PlayAnimation();
		}
	}

	public void UpdateRangeVis(bool checkTiles = false)
	{
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			CFGRangeBorders component = GetComponent<CFGRangeBorders>();
			if (component != null)
			{
				component.RegenerateMeshes(checkTiles);
			}
		}
	}

	private void SpawnCoversVis(CFGCell tile)
	{
		if (tile == null)
		{
			return;
		}
		tile.DecodePosition(out var PosX, out var PosZ, out var Floor);
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				SpawnCoversVisAtTile(CFGCellMap.GetCell(PosZ + j, PosX + i, Floor), j == 0 && i == 0);
			}
		}
	}

	private void SpawnCoversVisAtTile(CFGCell tile, bool is_center)
	{
		if (tile != null && tile.HaveFloor && !tile.CheckFlag(1, 2) && !tile.CheckFlag(1, 8) && tile.StairsType != CFGCell.EStairsType.Slope)
		{
			tile.DecodePosition(out var PosX, out var PosZ, out var Floor);
			CFGCell cell = CFGCellMap.GetCell(PosZ, PosX - 1, Floor);
			CFGCell cell2 = CFGCellMap.GetCell(PosZ + 1, PosX, Floor);
			CFGCell cell3 = CFGCellMap.GetCell(PosZ, PosX + 1, Floor);
			CFGCell cell4 = CFGCellMap.GetCell(PosZ - 1, PosX, Floor);
			SpawnCoversVisAtTile(tile, cell, cell2, cell3, cell4, (!is_center) ? 0.1f : 1f);
		}
	}

	private void SpawnCoversVisAtTile(CFGCell tile, CFGCell nb_n, CFGCell nb_e, CFGCell nb_s, CFGCell nb_w, float alpha)
	{
		Vector3 worldPosition = tile.WorldPosition;
		ECoverType eCoverType = ((nb_n == null) ? tile.GetBorderCover(EDirection.NORTH) : ECoverTypeExtension.Max(tile.GetBorderCover(EDirection.NORTH), nb_n.GetCenterCover()));
		if (eCoverType != 0 && !tile.HasDoorOnBorder(EDirection.NORTH))
		{
			Transform transform = UnityEngine.Object.Instantiate((eCoverType != ECoverType.FULL) ? m_HalfCoverVisPrefab : m_FullCoverVisPrefab, worldPosition, Quaternion.Euler(0f, 270f, 0f)) as Transform;
			Transform child = transform.GetChild(0);
			if (child != null && child.GetComponent<Renderer>() != null && child.GetComponent<Renderer>().material != null)
			{
				Color color = child.GetComponent<Renderer>().material.GetColor("_TintColor");
				color.a = alpha;
				child.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			}
			m_CoversVis.Add(transform);
		}
		ECoverType eCoverType2 = ((nb_e == null) ? tile.GetBorderCover(EDirection.EAST) : ECoverTypeExtension.Max(tile.GetBorderCover(EDirection.EAST), nb_e.GetCenterCover()));
		if (eCoverType2 != 0 && !tile.HasDoorOnBorder(EDirection.EAST))
		{
			Transform transform2 = UnityEngine.Object.Instantiate((eCoverType2 != ECoverType.FULL) ? m_HalfCoverVisPrefab : m_FullCoverVisPrefab, worldPosition, Quaternion.Euler(0f, 0f, 0f)) as Transform;
			Transform child2 = transform2.GetChild(0);
			if (child2 != null && child2.GetComponent<Renderer>() != null && child2.GetComponent<Renderer>().material != null)
			{
				Color color2 = child2.GetComponent<Renderer>().material.GetColor("_TintColor");
				color2.a = alpha;
				child2.GetComponent<Renderer>().material.SetColor("_TintColor", color2);
			}
			m_CoversVis.Add(transform2);
		}
		ECoverType eCoverType3 = ((nb_s == null) ? tile.GetBorderCover(EDirection.SOUTH) : ECoverTypeExtension.Max(tile.GetBorderCover(EDirection.SOUTH), nb_s.GetCenterCover()));
		if (eCoverType3 != 0 && !tile.HasDoorOnBorder(EDirection.SOUTH))
		{
			Transform transform3 = UnityEngine.Object.Instantiate((eCoverType3 != ECoverType.FULL) ? m_HalfCoverVisPrefab : m_FullCoverVisPrefab, worldPosition, Quaternion.Euler(0f, 90f, 0f)) as Transform;
			Transform child3 = transform3.GetChild(0);
			if (child3 != null && child3.GetComponent<Renderer>() != null && child3.GetComponent<Renderer>().material != null)
			{
				Color color3 = child3.GetComponent<Renderer>().material.GetColor("_TintColor");
				color3.a = alpha;
				child3.GetComponent<Renderer>().material.SetColor("_TintColor", color3);
			}
			m_CoversVis.Add(transform3);
		}
		ECoverType eCoverType4 = ((nb_w == null) ? tile.GetBorderCover(EDirection.WEST) : ECoverTypeExtension.Max(tile.GetBorderCover(EDirection.WEST), nb_w.GetCenterCover()));
		if (eCoverType4 != 0 && !tile.HasDoorOnBorder(EDirection.WEST))
		{
			Transform transform4 = UnityEngine.Object.Instantiate((eCoverType4 != ECoverType.FULL) ? m_HalfCoverVisPrefab : m_FullCoverVisPrefab, worldPosition, Quaternion.Euler(0f, 180f, 0f)) as Transform;
			Transform child4 = transform4.GetChild(0);
			if (child4 != null && child4.GetComponent<Renderer>() != null && child4.GetComponent<Renderer>().material != null)
			{
				Color color4 = child4.GetComponent<Renderer>().material.GetColor("_TintColor");
				color4.a = alpha;
				child4.GetComponent<Renderer>().material.SetColor("_TintColor", color4);
			}
			m_CoversVis.Add(transform4);
		}
	}

	private void DestroyCoversVis()
	{
		foreach (Transform coversVi in m_CoversVis)
		{
			UnityEngine.Object.Destroy(coversVi.gameObject);
		}
		m_CoversVis.Clear();
	}

	public void ResetControllerToDefault()
	{
		m_CurrentControllerMode = m_LastControllerMode;
	}

	private bool UpdateTargetCellPos(EJoyButton jb, Vector3 DeltaMove)
	{
		switch (m_CurrentAction)
		{
		case ETurnAction.AltFire_ConeShot:
			return false;
		default:
			if (m_FreeTargetingEnabled)
			{
				break;
			}
			return false;
		case ETurnAction.None:
		case ETurnAction.Ricochet:
			break;
		}
		if (m_UC_Cell == null)
		{
			CFGCell cFGCell = null;
			if (m_SelectedCharacter != null)
			{
				cFGCell = m_SelectedCharacter.CurrentCell;
			}
			if (cFGCell == null)
			{
				cFGCell = GetCellFromRay(new Ray(Camera.main.transform.position, Camera.main.transform.forward));
			}
			if (cFGCell == null)
			{
				return false;
			}
			m_UC_Cell = cFGCell;
			m_LastGamepadPos = cFGCell.WorldPosition;
			m_LastRealGamepadPos = m_LastGamepadPos;
		}
		float num = CFGJoyManager.ReadAsButton(jb, bContinous: true);
		float gamepadDeadZone = CFGInput.GamepadDeadZone;
		if (num < gamepadDeadZone)
		{
			return false;
		}
		num = (num - gamepadDeadZone) / (1f - gamepadDeadZone);
		CFGInput.ChangeInputMode(EInputMode.Gamepad);
		float num2 = CFGOptions.Input.GamepadCursorSpeed * Time.deltaTime * num;
		m_LastRealGamepadPos.x = Mathf.Clamp(m_LastRealGamepadPos.x + DeltaMove.x * num2, 0f, CFGCellMap.XAxisSize);
		m_LastRealGamepadPos.z = Mathf.Clamp(m_LastRealGamepadPos.z + DeltaMove.z * num2, 0f, CFGCellMap.ZAxisSize);
		m_LastRealGamepadPos.y = Mathf.Clamp(m_LastRealGamepadPos.y + DeltaMove.y * num2, 0f, (float)CFGCellMap.MaxFloor * 2.5f);
		return true;
	}

	private void UpdateInputFromController()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (m_CurrentControllerMode == EControllerMode.Unknown)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				m_CurrentControllerMode = EControllerMode.Global_Tactical;
			}
			else
			{
				m_CurrentControllerMode = EControllerMode.Global_Strategic;
			}
		}
		if (CFGInput.LastReadInputDevice != m_LastInputMode)
		{
			if (m_LastInputMode != 0)
			{
				if (CFGSingleton<CFGGame>.Instance.IsInGame())
				{
					if (m_CurrentAction != ETurnAction.None)
					{
						m_CurrentControllerMode = EControllerMode.Target_Selected;
					}
					else
					{
						m_CurrentControllerMode = EControllerMode.Global_Tactical;
					}
				}
				else
				{
					m_CurrentControllerMode = EControllerMode.Global_Strategic;
				}
			}
			m_LastInputMode = CFGInput.LastReadInputDevice;
		}
		switch (m_CurrentControllerMode)
		{
		case EControllerMode.Global_Tactical:
			SwichCharacterFromController(bTarget: false);
			UpdateInputFromController_TacticalGlobal();
			break;
		case EControllerMode.Global_Strategic:
			UpdateInputFromController_StrategicGlobal();
			break;
		case EControllerMode.Target_Selected:
			SwichCharacterFromController(bTarget: true);
			UpdateInputFromController_TacticalTargetSelected();
			break;
		}
	}

	private void SwitchSkillFromController()
	{
		if (Time.time < m_Controller_NextRead || m_AllowedMode == EPlayerHudLimiterMode.Confirm)
		{
			return;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance == null || instance.m_HUD == null || instance.m_HUDAbilities.m_SkillButtonsData == null || instance.m_HUDAbilities.m_SkillButtonsData.Count == 0)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < instance.m_HUDAbilities.m_SkillButtonsData.Count; i++)
		{
			if (instance.m_HUDAbilities.m_SkillButtonsData[i] != null && instance.m_HUDAbilities.m_SkillButtonsData[i].enabled)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		if (m_CurrentAction == ETurnAction.None)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightTrigger) > 0f)
			{
				m_Controller_NextRead = Time.time + 0.3f;
				if (m_CurrentAction == ETurnAction.None)
				{
					TryAndActivateFirstAbility();
				}
				if (CFGJoyManager.ReadAsButton(EJoyButton.RightTrigger) > 0f)
				{
					if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad != null)
					{
						CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad.SimulateClickGraphicAndSoundOnly();
					}
				}
				else if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad != null)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad.SimulateClickGraphicAndSoundOnly();
				}
			}
		}
		else
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.RightTrigger) > 0f)
			{
				SwitchAbility(1);
				m_Controller_NextRead = Time.time + 0.3f;
				if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad != null)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_RTButtonPad.SimulateClickGraphicAndSoundOnly();
				}
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.LeftTrigger) > 0f)
			{
				SwitchAbility(-1);
				m_Controller_NextRead = Time.time + 0.3f;
				if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad != null)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUDAbilities.m_LTButtonPad.SimulateClickGraphicAndSoundOnly();
				}
			}
		}
		if (m_CurrentAction == ETurnAction.None)
		{
			m_CurrentControllerMode = EControllerMode.Global_Tactical;
		}
		else
		{
			m_CurrentControllerMode = EControllerMode.Target_Selected;
		}
	}

	private void SwichCharacterFromController(bool bTarget)
	{
		if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
		{
			if (bTarget)
			{
				SelectNextTargetable();
				return;
			}
			SelectCharacter(GetNextActiveCharacter(), focus: true);
			if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_RBButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_RBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		else
		{
			if (!(CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f))
			{
				return;
			}
			if (bTarget)
			{
				SelectPrevTargetable();
				return;
			}
			SelectCharacter(GetPrevActiveCharacter(), focus: true);
			if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_LBButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
	}

	private void UpdateInputFromController_SkillSelect()
	{
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Cancel))
		{
			m_CurrentControllerMode = EControllerMode.Global_Tactical;
		}
	}

	private void UpdateInputFromController_StrategicGlobal()
	{
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Action) && m_UC_Cell != null && m_SelectedCharacter != null)
		{
			if (m_UC_Cell == m_SelectedCharacter.CurrentCell && (bool)m_UC_Cell.OwnerObject && m_UC_Cell.OwnerObject.Usage == CFGCellObject.eCOUsage.Location)
			{
				m_UC_Cell.CharacterReEnter();
			}
			else if (m_SelectedCharacter.CurrentAction == ETurnAction.Move && CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				m_SelectedCharacter.ChangeMovement(m_UC_Cell);
				SpawnOrderConfirmationFx(m_UC_Cell.WorldPosition);
			}
			else if (m_SelectedCharacter.MakeAction(ETurnAction.Move, m_UC_Cell) == EActionResult.Success)
			{
				SpawnOrderConfirmationFx(m_UC_Cell.WorldPosition);
			}
			else
			{
				CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_MoveImpossible, m_UC_Cell.WorldPosition);
			}
		}
		else if (CFGJoyManager.IsActivated(EJoyAction.Exit))
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadCharacterScreen(combat_loadout: false, null);
		}
		else
		{
			UpdateInputFromController_DialogSkip(EJoyAction.EquipTal);
		}
	}

	private void UpdateInputFromController_Cone()
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		Vector3 vector = Vector3.left;
		Vector3 vector2 = Vector3.forward;
		if (component != null)
		{
			vector = CFGMath.ProjectedVectorOnPlane(-component.transform.right, Vector3.up);
			vector2 = CFGMath.ProjectedVectorOnPlane(component.transform.forward, Vector3.up);
		}
		float num = CFGJoyManager.ReadAsButton(CFGJoyManager.ButtonForCM_LEFT, bContinous: true);
		num -= CFGJoyManager.ReadAsButton(CFGJoyManager.ButtonForCM_RIGHT, bContinous: true);
		float num2 = CFGJoyManager.ReadAsButton(CFGJoyManager.ButtonForCM_UP, bContinous: true);
		num2 -= CFGJoyManager.ReadAsButton(CFGJoyManager.ButtonForCM_DOWN, bContinous: true);
		Vector3 vector3 = num * vector + num2 * vector2;
		float magnitude = vector3.magnitude;
		if (magnitude > 0.65f)
		{
			vector3.Normalize();
			ConeOfFireViz_RotateTowards(m_SelectedCharacter.Position + vector3 * 2f);
		}
	}

	private void UpdateInputFromController_RicochetAction()
	{
		CFGGameObject gameObjectUnderCell = GetGameObjectUnderCell();
		CFGRicochetObject cFGRicochetObject = gameObjectUnderCell as CFGRicochetObject;
		if (cFGRicochetObject != null && m_TargetableObjects.Contains(gameObjectUnderCell))
		{
			if (m_RicochetObjects.Count == 0)
			{
				BeginRicochetShoot(cFGRicochetObject);
			}
			else
			{
				TryAndAddRicochetObject(cFGRicochetObject);
			}
			m_UC_Cell = cFGRicochetObject.Cell;
			m_LastGamepadPos = m_UC_Cell.WorldPosition;
			m_LastRealGamepadPos = m_LastGamepadPos;
		}
		else if (m_RicochetObjects.Count != 0 && gameObjectUnderCell is CFGIAttackable cFGIAttackable && m_TargetableObjects.Contains(gameObjectUnderCell))
		{
			if (cFGIAttackable == m_TargetedObject)
			{
				OnConfirmationAttackClick(0);
			}
			else
			{
				m_TargetedObject = cFGIAttackable;
			}
		}
	}

	private void HandleConfirmOrder()
	{
		if (AllowedMode != 0)
		{
		}
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_ConfirmButton != null && CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_ConfirmButton.enabled)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_ConfirmButton.SimulateClickGraphicAndSoundOnly();
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.Ricochet:
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
			{
				UpdateInputFromController_RicochetAction();
			}
			else
			{
				OnConfirmationAttackClick(0);
			}
			return;
		case ETurnAction.AltFire_ConeShot:
			if (m_AOEObjects.Count > 0 && m_TargetedObject == null)
			{
				m_TargetedObject = m_AOEObjects[0];
				m_AOEObjects.RemoveAt(0);
			}
			break;
		}
		OnConfirmationAttackClick(0);
	}

	private void UpdateInputFromController_TacticalTargetSelected()
	{
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Cancel))
		{
			if (m_AllowedMode == EPlayerHudLimiterMode.Confirm)
			{
				return;
			}
			if (m_CurrentAction == ETurnAction.Ricochet && m_RicochetObjects.Count > 0)
			{
				OnRightClick(0);
				if (m_RicochetObjects.Count > 0)
				{
					CFGRicochetObject cFGRicochetObject = m_RicochetObjects[m_RicochetObjects.Count - 1];
					if ((bool)cFGRicochetObject)
					{
						MoveCursorToCell(cFGRicochetObject.Cell, cFGRicochetObject);
					}
				}
			}
			else
			{
				CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
				if ((bool)instance && (bool)instance.m_TermsOfShootings && (bool)instance.m_TermsOfShootings.m_MoreInfoWindow && instance.m_TermsOfShootings.m_MoreInfoWindow.activeSelf)
				{
					instance.m_TermsOfShootings.CloseMoreInfo(0);
					return;
				}
				CancelOrdering();
				m_CurrentControllerMode = EControllerMode.Global_Tactical;
			}
			return;
		}
		if (m_FreeTargetingEnabled)
		{
			if ((bool)m_ConeVisualization)
			{
				UpdateInputFromController_Cone();
			}
			if ((bool)m_AOE_CircleHelper)
			{
				m_FreeTargetingCell = m_UC_Cell;
				UpdateAOEHelper();
			}
			if (!(m_ConeVisualization != null) && !(m_AOE_CircleHelper != null))
			{
				m_FreeTargetingCanShoot = false;
				if (m_UC_Cell != null)
				{
				}
				if (m_UC_Cell != null && CFGCellMap.GetLineOf(m_SelectedCharacter.CurrentCell, m_UC_Cell, 10000, 32, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None)
				{
					m_FreeTargetingCanShoot = true;
					if (m_TargetingAbility != null)
					{
						m_FreeTargetingCanShoot = false;
						m_TargetedObject = null;
						if (GetGameObjectUnderCell() is CFGIAttackable cFGIAttackable && m_TargetingAbility.CanTargetAbilityOn(cFGIAttackable))
						{
							m_FreeTargetingCanShoot = true;
							m_TargetedObject = cFGIAttackable;
						}
					}
				}
			}
		}
		if (CFGJoyManager.IsActivated(EJoyAction.MoreInfo))
		{
			CFGWindowMgr instance2 = CFGSingleton<CFGWindowMgr>.Instance;
			if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_LBButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.m_LBButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if ((bool)instance2 && (bool)instance2.m_TermsOfShootings && (bool)instance2.m_TermsOfShootings.m_MoreInfoWindow)
			{
				if (instance2.m_TermsOfShootings.m_MoreInfoWindow.activeSelf)
				{
					instance2.m_TermsOfShootings.CloseMoreInfo(0);
				}
				else
				{
					instance2.m_TermsOfShootings.OpenMoreInfo(0);
				}
			}
		}
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Action))
		{
			if (!CFGSingleton<CFGWindowMgr>.Instance.IsAnyTutorialActive())
			{
				HandleConfirmOrder();
			}
		}
		else if (m_AllowedMode != EPlayerHudLimiterMode.Confirm)
		{
			if (CFGJoyManager.IsActivated(EJoyAction.Exit))
			{
				m_LastControllerMode = m_CurrentControllerMode;
				m_CurrentControllerMode = EControllerMode.Tactical_CharacterDetails;
				CFGSingleton<CFGWindowMgr>.Instance.LoadTacticalCharacterDetails();
			}
			SwitchSkillFromController();
		}
	}

	private void UpdateInputFromController_DialogSkip(EJoyAction Action)
	{
		if (CFGJoyManager.IsActivated(Action))
		{
			if (m_TimeOfDialogSkipStart < 0f)
			{
				m_TimeOfDialogSkipStart = Time.time;
				return;
			}
			float num = Time.time - m_TimeOfDialogSkipStart;
			if (num > 1.2f)
			{
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.SkipCurrentDialog();
				m_TimeOfDialogSkipStart = -1f;
			}
		}
		else if (m_TimeOfDialogSkipStart > 0f)
		{
			float num2 = Time.time - m_TimeOfDialogSkipStart;
			if (num2 < 1.2f)
			{
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.SkipCurrentLine();
				m_TimeOfDialogSkipStart = -1f;
			}
			else
			{
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.SkipCurrentDialog();
				m_TimeOfDialogSkipStart = -1f;
			}
		}
	}

	private void UpdateInputFromController_TacticalGlobal()
	{
		if (CFGJoyManager.IsActivated(EJoyAction.Default_Action) && m_UC_Cell != null && m_SelectedCharacter != null)
		{
			if (CFGSingleton<CFGWindowMgr>.Instance.IsAnyTutorialActive())
			{
				return;
			}
			CFGGameObject gameObjectUnderCell = GetGameObjectUnderCell();
			CFGCharacter cFGCharacter = gameObjectUnderCell as CFGCharacter;
			if (cFGCharacter != null && cFGCharacter.IsAlive)
			{
				if (cFGCharacter.Owner != null)
				{
					if (cFGCharacter.Owner.IsAi)
					{
						OnEnemyClick(cFGCharacter);
						m_CurrentControllerMode = EControllerMode.Target_Selected;
					}
					else if (cFGCharacter.Owner.IsPlayer)
					{
						SelectCharacter(cFGCharacter, focus: true);
					}
				}
				return;
			}
			CFGRicochetObject cFGRicochetObject = gameObjectUnderCell as CFGRicochetObject;
			if (cFGRicochetObject != null)
			{
				BeginRicochetShoot(cFGRicochetObject);
				return;
			}
			if (gameObjectUnderCell is CFGIAttackable { IsAlive: not false } cFGIAttackable)
			{
				OnAttackableClick(cFGIAttackable);
				m_CurrentControllerMode = EControllerMode.Target_Selected;
				return;
			}
			if (m_UC_Cell.UsableObject != null)
			{
				CFGUsableObject usableObject = m_UC_Cell.UsableObject;
				if ((bool)usableObject)
				{
					EUseMode Mode;
					CFGCell cFGCell = usableObject.CanCharacterUse(m_SelectedCharacter, bCheckWithMovement: true, out Mode);
					if ((bool)cFGCell)
					{
						switch (Mode)
						{
						case EUseMode.CanUse:
							usableObject.TryUse(m_SelectedCharacter);
							return;
						case EUseMode.CantUse:
							return;
						}
						m_SelectedCharacter.MakeAction(ETurnAction.Move, cFGCell, usableObject);
					}
					return;
				}
			}
			if ((m_ActionLimit == EPlayerActionLimit.MoveToTile && m_ActionLimitTargetCell == m_UC_Cell) || m_ActionLimit == EPlayerActionLimit.Default)
			{
				if (m_SelectedCharacter.MakeAction(ETurnAction.Move, m_UC_Cell) == EActionResult.Success)
				{
					SpawnOrderConfirmationFx(m_UC_Cell.WorldPosition);
				}
				else
				{
					CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_MoveImpossible, m_UC_Cell.WorldPosition);
				}
			}
			if (CurrentAction != ETurnAction.None || CellUnderCursor == null)
			{
				return;
			}
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (!(component == null))
			{
				if (m_CursorHelperState == ECursorHelper.MoveStairsUp)
				{
					component.MoveToFloor(CellUnderCursor.Floor + 1);
				}
				else if (m_CursorHelperState == ECursorHelper.MoveStairsDown)
				{
					component.MoveToFloor(CellUnderCursor.Floor);
				}
			}
			return;
		}
		if (CFGJoyManager.IsActivated(EJoyAction.CharacterDetails) && m_UC_Cell != null && (bool)m_UC_Cell.OwnerObject && m_UC_Cell.OwnerObject.IsDoor)
		{
			if (m_AllowedMode == EPlayerHudLimiterMode.Confirm)
			{
				return;
			}
			CFGDoorObject component2 = m_UC_Cell.OwnerObject.GetComponent<CFGDoorObject>();
			if (component2 == null && (bool)m_UC_Cell.OwnerObject.m_CurrentVisualisation)
			{
				component2 = m_UC_Cell.OwnerObject.m_CurrentVisualisation.GetComponent<CFGDoorObject>();
			}
			if ((bool)component2 && !component2.IsOpened)
			{
				EUseMode UseMode;
				CFGCell cFGCell2 = component2.CanCharacterUse(m_SelectedCharacter, bUseEvent: true, bCheckPath: true, out UseMode);
				if ((bool)cFGCell2)
				{
					if (UseMode == EUseMode.CanUse)
					{
						component2.Open(m_SelectedCharacter);
						return;
					}
					m_SelectedCharacter.MakeAction(ETurnAction.Move, cFGCell2, component2);
				}
				return;
			}
		}
		if (CFGJoyManager.IsActivated(EJoyAction.Default_OtherAction))
		{
			if (m_AllowedMode != EPlayerHudLimiterMode.Confirm)
			{
				OnChangeWeaponClick();
				if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_YButtonPad != null)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_YButtonPad.SimulateClickGraphicAndSoundOnly();
				}
			}
		}
		else if (CFGJoyManager.IsActivated(EJoyAction.Exit))
		{
			if (m_AllowedMode != EPlayerHudLimiterMode.Confirm)
			{
				m_LastControllerMode = m_CurrentControllerMode;
				m_CurrentControllerMode = EControllerMode.Tactical_CharacterDetails;
				CFGSingleton<CFGWindowMgr>.Instance.LoadTacticalCharacterDetails();
			}
		}
		else
		{
			UpdateInputFromController_DialogSkip(EJoyAction.EquipUsbl1);
			SwitchSkillFromController();
		}
	}

	private void TryAndActivateFirstAbility()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance == null || instance.m_HUDAbilities == null || instance.m_HUDAbilities.m_SkillButtonsData == null || instance.m_HUDAbilities.m_SkillButtonsData.Count == 0)
		{
			return;
		}
		List<CFGSkillButtonsData> skillButtonsData = instance.m_HUDAbilities.m_SkillButtonsData;
		foreach (CFGSkillButtonsData item in skillButtonsData)
		{
			if (item.enabled)
			{
				ActivateAbility((ETurnAction)item.data, bInstant: false);
				if (m_CurrentAction != ETurnAction.None)
				{
					break;
				}
			}
		}
	}

	private void SwitchAbility(int Add)
	{
		int activeAbilityButton = GetActiveAbilityButton();
		if (activeAbilityButton == -1)
		{
			return;
		}
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance == null || instance.m_HUDAbilities == null || instance.m_HUDAbilities.m_SkillButtonsData == null || instance.m_HUDAbilities.m_SkillButtonsData.Count == 0)
		{
			return;
		}
		List<CFGSkillButtonsData> skillButtonsData = instance.m_HUDAbilities.m_SkillButtonsData;
		int num = activeAbilityButton;
		do
		{
			num += Add;
			if (num < 0)
			{
				num = skillButtonsData.Count - 1;
			}
			if (num >= skillButtonsData.Count)
			{
				num = 0;
			}
			if (num == activeAbilityButton)
			{
				return;
			}
		}
		while (!skillButtonsData[num].enabled);
		ActivateAbility((ETurnAction)skillButtonsData[num].data, bInstant: false);
	}

	private int GetActiveAbilityButton()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance == null || instance.m_HUDAbilities == null || instance.m_HUDAbilities.m_SkillButtonsData == null || instance.m_HUDAbilities.m_SkillButtonsData.Count == 0)
		{
			return -1;
		}
		List<CFGSkillButtonsData> skillButtonsData = instance.m_HUDAbilities.m_SkillButtonsData;
		for (int i = 0; i < skillButtonsData.Count; i++)
		{
			if (skillButtonsData[i].selected)
			{
				return i;
			}
		}
		return -1;
	}

	private CFGCell OnGPMove()
	{
		CFGCell cell = CFGCellMap.GetCell(m_LastGamepadPos);
		if (cell == null)
		{
			return null;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (m_GamepadCursor == null)
		{
			m_GamepadCursor = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_GamepadCursor;
		}
		if (m_GamepadCursor != null)
		{
			Vector3 lastRealGamepadPos = m_LastRealGamepadPos;
			lastRealGamepadPos.y = component.CurrentTargetPoint.y;
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				m_GamepadCursor.Transform.position = lastRealGamepadPos;
			}
			else
			{
				m_GamepadCursor.Transform.position = m_LastRealGamepadPos;
			}
		}
		if (cell != m_UC_Cell)
		{
			CFGCameraFollowInfo cFGCameraFollowInfo = ((!CFGSingleton<CFGGame>.Instance.IsInStrategic()) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_TacticalMapFollowInfo : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_StrategicMapFollowInfo);
			if (m_GamepadCursor != null && component.Focus != m_GamepadCursor && cFGCameraFollowInfo != null)
			{
				component.ChangeFocus(m_GamepadCursor, 0.5f, cFGCameraFollowInfo, force: true);
			}
		}
		return cell;
	}

	private void FocusOn(CFGGameObject tgt, bool bCancelOrder)
	{
		if (bCancelOrder)
		{
			CancelOrdering();
		}
		if (!(tgt == null))
		{
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus(tgt, 0.5f, force: true);
			}
		}
	}

	public void OnRightClick(int a)
	{
		if (IsLocked || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn || m_AllowedMode == EPlayerHudLimiterMode.Confirm)
		{
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.Ricochet:
		{
			if (m_RicochetObjects.Count == 0)
			{
				FocusOn(m_SelectedCharacter, bCancelOrder: true);
				break;
			}
			if (m_TargetedObject != null)
			{
				m_TargetedObject = null;
				FocusOn(m_RicochetObjects[m_RicochetObjects.Count - 1], bCancelOrder: false);
				break;
			}
			CFGRicochetObject item = m_RicochetObjects[m_RicochetObjects.Count - 1];
			m_AvailableRicochetObjects.Add(item);
			m_RicochetObjects.RemoveAt(m_RicochetObjects.Count - 1);
			item = null;
			if (m_RicochetObjects.Count > 0)
			{
				item = m_RicochetObjects[m_RicochetObjects.Count - 1];
				m_AvailableRicochetObjects.Add(item);
				m_RicochetObjects.RemoveAt(m_RicochetObjects.Count - 1);
			}
			TryAndAddRicochetObject(item);
			if (item == null)
			{
				GenerateTargetableObjectList();
			}
			FocusOn((!(item != null)) ? ((CFGGameObject)m_SelectedCharacter) : ((CFGGameObject)item), bCancelOrder: false);
			break;
		}
		default:
			FocusOn(m_SelectedCharacter, bCancelOrder: true);
			break;
		case ETurnAction.None:
			if (m_GameObjectUnderCursor as CFGUsableObject != null)
			{
				CFGUsableObject cFGUsableObject = m_GameObjectUnderCursor as CFGUsableObject;
				if ((m_ActionLimit == EPlayerActionLimit.Default || (m_ActionLimit == EPlayerActionLimit.Usable && m_ActionLimitTargetUsable == cFGUsableObject)) && (bool)cFGUsableObject)
				{
					EUseMode Mode;
					CFGCell cFGCell = cFGUsableObject.CanCharacterUse(m_SelectedCharacter, bCheckWithMovement: true, out Mode);
					if ((bool)cFGCell)
					{
						switch (Mode)
						{
						case EUseMode.CanUse:
							cFGUsableObject.TryUse(m_SelectedCharacter);
							break;
						default:
							m_SelectedCharacter.MakeAction(ETurnAction.Move, cFGCell, cFGUsableObject);
							break;
						case EUseMode.CantUse:
							break;
						}
					}
					break;
				}
			}
			if (m_GameObjectUnderCursor as CFGShootableObject != null)
			{
				if (m_ActionLimit == EPlayerActionLimit.Default)
				{
					OnAttackableClick(m_GameObjectUnderCursor as CFGIAttackable);
				}
			}
			else if (m_GameObjectUnderCursor as CFGDoorObject != null)
			{
				if (m_ActionLimit == EPlayerActionLimit.Default)
				{
					CFGDoorObject cFGDoorObject = m_GameObjectUnderCursor as CFGDoorObject;
					if ((bool)cFGDoorObject && !cFGDoorObject.IsOpened)
					{
						EUseMode UseMode;
						CFGCell cFGCell2 = cFGDoorObject.CanCharacterUse(m_SelectedCharacter, bUseEvent: true, bCheckPath: true, out UseMode);
						if ((bool)cFGCell2)
						{
							if (UseMode == EUseMode.CanUse)
							{
								cFGDoorObject.Open(m_SelectedCharacter);
								break;
							}
							m_SelectedCharacter.MakeAction(ETurnAction.Move, cFGCell2, cFGDoorObject);
						}
						break;
					}
				}
			}
			else if (m_GameObjectUnderCursor as CFGLocationObject != null)
			{
				if (m_ActionLimit == EPlayerActionLimit.Default)
				{
					CFGCell characterCell = CFGCellMap.GetCharacterCell(m_GameObjectUnderCursor.transform.position);
					CFGCell characterCell2 = CFGCellMap.GetCharacterCell(m_SelectedCharacter.transform.position);
					if (characterCell != null)
					{
						if (characterCell == characterCell2)
						{
							characterCell.CharacterReEnter();
						}
						else
						{
							m_SelectedCharacter.MakeAction(ETurnAction.Move, characterCell);
						}
					}
				}
			}
			else if (m_UC_Cell != null && m_SelectedCharacter != null && (m_ActionLimit == EPlayerActionLimit.Default || (m_ActionLimit == EPlayerActionLimit.MoveToTile && m_UC_Cell == m_ActionLimitTargetCell)))
			{
				if (m_SelectedCharacter.CurrentAction == ETurnAction.Move && CFGSingleton<CFGGame>.Instance.IsInStrategic())
				{
					m_SelectedCharacter.ChangeMovement(m_UC_Cell);
					SpawnOrderConfirmationFx(m_UC_Cell.WorldPosition);
				}
				else if (m_SelectedCharacter.MakeAction(ETurnAction.Move, m_UC_Cell) == EActionResult.Success)
				{
					SpawnOrderConfirmationFx(m_UC_Cell.WorldPosition);
				}
				else
				{
					CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_MoveImpossible, m_UC_Cell.WorldPosition);
				}
			}
			if (m_CursorHelper != null && m_CursorHelper.GetComponent<Animation>() != null)
			{
				m_CursorHelper.GetComponent<Animation>().Play();
			}
			if ((bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings.gameObject.SetActive(value: false);
			}
			break;
		}
	}

	private void UpdateFreeTargetingFromMouse()
	{
		if (!m_FreeTargetingEnabled || CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			return;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (m_ConeVisualization != null || m_AOE_CircleHelper != null)
		{
			Plane plane = default(Plane);
			Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
			plane.SetNormalAndPosition(Vector3.up, new Vector3(0f, 2.5f * (float)component.CurrentFloorLevel, 0f));
			if (!plane.Raycast(ray, out var enter))
			{
				return;
			}
			Vector3 point = ray.GetPoint(enter);
			float num = Vector3.Distance(m_FreeTargetingPoint, point);
			if (num < 0.01f)
			{
				return;
			}
			CFGCell freeTargetingCell = m_FreeTargetingCell;
			if (m_TargetingAbility != null && (m_TargetingAbility.AnimationType == eAbilityAnimation.Throw1 || m_TargetingAbility.AnimationType == eAbilityAnimation.Throw2 || m_TargetingAbility.AnimationType == eAbilityAnimation.ThrowAuto))
			{
				m_FreeTargetingCell = CellUnderCursor;
			}
			else
			{
				m_FreeTargetingCell = CFGCellMap.GetCell(point);
			}
			if (m_FreeTargetingCell == null)
			{
				return;
			}
			if (m_ConeVisualization != null)
			{
				if (m_FreeTargetingCell != m_SelectedCharacter.CurrentCell)
				{
					m_FreeTargetingPoint = point;
					ConeOfFireViz_RotateTowards(m_FreeTargetingPoint);
					m_FreeTargetingCanShoot = true;
				}
				else
				{
					m_FreeTargetingCell = freeTargetingCell;
				}
				return;
			}
			if (m_TargetingAbility != null && (m_TargetingAbility.AnimationType == eAbilityAnimation.Throw1 || m_TargetingAbility.AnimationType == eAbilityAnimation.Throw2 || m_TargetingAbility.AnimationType == eAbilityAnimation.ThrowAuto) && CellUnderCursor != null)
			{
				m_FreeTargetingPoint = CellUnderCursor.WorldPosition;
			}
			else
			{
				m_FreeTargetingPoint = point;
			}
			if (m_TargetingAbility != null && m_AOE_CircleHelper != null && m_FreeTargetingCell != null && freeTargetingCell != m_FreeTargetingCell)
			{
				UpdateAOEHelper();
			}
			return;
		}
		m_FreeTargetingCanShoot = false;
		if (m_UC_Cell != null)
		{
		}
		if (m_UC_Cell == null || CFGCellMap.GetLineOf(m_SelectedCharacter.CurrentCell, m_UC_Cell, 10000, 32, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) != 0)
		{
			return;
		}
		m_FreeTargetingCanShoot = true;
		if (m_TargetingAbility != null)
		{
			m_FreeTargetingCanShoot = false;
			m_TargetedObject = null;
			if (GetGameObjectUnderCell() is CFGIAttackable cFGIAttackable && m_TargetingAbility.CanTargetAbilityOn(cFGIAttackable))
			{
				m_FreeTargetingCanShoot = true;
				m_TargetedObject = cFGIAttackable;
			}
		}
	}

	private void UpdateAOEHelper()
	{
		if (m_FreeTargetingCell == null)
		{
			return;
		}
		m_AOE_CircleHelper.transform.position = m_FreeTargetingCell.WorldPosition + AOECIRCLEHELPERADD;
		m_FreeTargetingCanShoot = m_TargetingAbility.IsCellInRange(m_FreeTargetingCell);
		CFGGrenadePathVis component = GetComponent<CFGGrenadePathVis>();
		if (component != null)
		{
			component.m_IsTargetInRange = m_FreeTargetingCanShoot;
		}
		if (m_FreeTargetingCanShoot && CFGCellMap.GetLineOfSightAutoSideSteps(m_SelectedCharacter, null, null, m_FreeTargetingCell, 10000) != 0)
		{
			m_FreeTargetingCanShoot = false;
		}
		if (m_TargetingAbility.AnimationType == eAbilityAnimation.Throw1 || m_TargetingAbility.AnimationType == eAbilityAnimation.Throw2 || m_TargetingAbility.AnimationType == eAbilityAnimation.ThrowAuto)
		{
			MeshRenderer component2 = m_AOE_CircleHelper.GetComponent<MeshRenderer>();
			if ((bool)component2)
			{
				component2.material = ((!m_FreeTargetingCanShoot) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_AOE_Circle_Explosion_InactiveMat : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_AOE_Circle_Explosion_ActiveMat);
			}
		}
		if (m_FreeTargetingCanShoot)
		{
			m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingCell.WorldPosition);
		}
		else
		{
			m_AOEObjects.Clear();
		}
	}

	private void HandleFreeTargetingShootLMB()
	{
		if (m_ConeVisualization != null)
		{
			if (m_AOEObjects.Count > 0)
			{
				m_TargetedObject = m_AOEObjects[0];
				m_AOEObjects.RemoveAt(0);
				OnConfirmationAttackClick(0);
			}
			else
			{
				SelectedCharacter.MakeAction(ETurnAction.Miss_Shoot, m_UC_Cell, null);
				CancelOrdering();
			}
			return;
		}
		m_TargetedObject = m_GameObjectUnderCursor as CFGIAttackable;
		if (m_TargetedObject == null)
		{
			if (m_FreeTargetingCanShoot)
			{
				SelectedCharacter.MakeAction(ETurnAction.Miss_Shoot, m_UC_Cell, m_AOEObjects);
				CancelOrdering();
			}
		}
		else if (CFGSingletonResourcePrefab<CFGObjectManager>.Instance.IsVisibleByPlayer(m_TargetedObject))
		{
			CFGCharacter cFGCharacter = m_TargetedObject as CFGCharacter;
			CFGIAttackable targetedObject = m_TargetedObject;
			m_TargetedObject = null;
			m_FreeTargetingEnabled = false;
			if (cFGCharacter != null)
			{
				OnEnemyClick(cFGCharacter);
			}
			else
			{
				OnAttackableClick(targetedObject);
			}
		}
		else if (m_FreeTargetingCanShoot)
		{
			OnConfirmationAttackClick(0);
		}
	}

	private void ActivateAbility(int AbilitySlot)
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (!(instance == null) && !(instance.m_HUDAbilities == null) && instance.m_HUDAbilities.m_SkillButtonsData != null && instance.m_HUDAbilities.m_SkillButtonsData.Count != 0)
		{
			List<CFGSkillButtonsData> skillButtonsData = instance.m_HUDAbilities.m_SkillButtonsData;
			if (AbilitySlot >= 0 && AbilitySlot < skillButtonsData.Count && skillButtonsData[AbilitySlot] != null && skillButtonsData[AbilitySlot].enabled)
			{
				ActivateAbility((ETurnAction)skillButtonsData[AbilitySlot].data, bInstant: false);
			}
		}
	}

	private void UpdateInput()
	{
		if (IsLocked)
		{
			return;
		}
		CFGGame instance = CFGSingleton<CFGGame>.Instance;
		UpdateInputFromController();
		if (instance.IsInGame())
		{
			if (CFGInput.IsActivated(EActionCommand.Character1))
			{
				OnCharacterButtonClick(0);
			}
			if (CFGInput.IsActivated(EActionCommand.Character2))
			{
				OnCharacterButtonClick(1);
			}
			if (CFGInput.IsActivated(EActionCommand.Character3))
			{
				OnCharacterButtonClick(2);
			}
			if (CFGInput.IsActivated(EActionCommand.Character4))
			{
				OnCharacterButtonClick(3);
			}
			if (CFGInput.IsActivated(EActionCommand.Option1))
			{
				ActivateAbility(0);
			}
			if (CFGInput.IsActivated(EActionCommand.Option2))
			{
				ActivateAbility(1);
			}
			if (CFGInput.IsActivated(EActionCommand.Option3))
			{
				ActivateAbility(2);
			}
			if (CFGInput.IsActivated(EActionCommand.Option4))
			{
				ActivateAbility(3);
			}
			if (CFGInput.IsActivated(EActionCommand.Option5))
			{
				ActivateAbility(4);
			}
			if (CFGInput.IsActivated(EActionCommand.Option6))
			{
				ActivateAbility(5);
			}
			if (CFGInput.IsActivated(EActionCommand.Option7))
			{
				ActivateAbility(6);
			}
			if (CFGInput.IsActivated(EActionCommand.Option8))
			{
				ActivateAbility(7);
			}
			if (CFGInput.IsActivated(EActionCommand.Option9))
			{
				ActivateAbility(8);
			}
			if (CFGInput.IsActivated(EActionCommand.Option10))
			{
				ActivateAbility(9);
			}
			if (CFGInput.IsActivated(EActionCommand.WeaponChange))
			{
				OnChangeWeaponClick();
			}
			if ((bool)m_SelectedCharacter && (bool)m_SelectedCharacter.CurrentWeapon && m_SelectedCharacter.CurrentWeapon.CurrentAmmo < m_SelectedCharacter.CurrentWeapon.AmmoCapacity && CFGInput.IsActivated(EActionCommand.Reload) && m_SelectedCharacter.CanMakeAction(ETurnAction.Reload) == EActionResult.Success)
			{
				m_CurrentAction = ETurnAction.Reload;
				OnConfirmationAttackClick(0);
			}
			CFGTermsOfShootings termsOfShootings = CFGSingleton<CFGWindowMgr>.Instance.m_TermsOfShootings;
			if (CFGInput.IsActivated(EActionCommand.SelectNext))
			{
				if (termsOfShootings != null && termsOfShootings.isActiveAndEnabled)
				{
					SelectNextTargetable();
				}
				else
				{
					CFGCharacter nextActiveCharacter = GetNextActiveCharacter();
					if (nextActiveCharacter != null)
					{
						SelectCharacter(nextActiveCharacter, focus: true);
					}
				}
			}
			if (CFGInput.IsActivated(EActionCommand.SelectPrevious))
			{
				if (termsOfShootings != null && termsOfShootings.isActiveAndEnabled)
				{
					SelectPrevTargetable();
				}
				else
				{
					CFGCharacter prevActiveCharacter = GetPrevActiveCharacter();
					if (prevActiveCharacter != null)
					{
						SelectCharacter(prevActiveCharacter, focus: true);
					}
				}
			}
			if (CFGInput.IsActivated(EActionCommand.EndTurn) && !m_bTurnEndDisabled)
			{
				m_TargetedObject = null;
				CFGSingletonResourcePrefab<CFGTurnManager>.Instance.EndTurn(bUpdateTurnCounter: true);
			}
			if ((CFGInput.IsActivated(EActionCommand.Camera_PanUp) || CFGInput.IsActivated(EActionCommand.Camera_PanDown)) && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_VerticalPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_VerticalPad.SimulateClickGraphicAndSoundOnly();
			}
			if ((CFGInput.IsActivated(EActionCommand.Camera_RotateRight) || CFGInput.IsActivated(EActionCommand.Camera_RotateLeft)) && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_HorizontalPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_HorizontalPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGInput.IsActivated(EActionCommand.EndTurn) && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUD != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_XButtonPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.m_XButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGInput.IsActivated(EActionCommand.Exit) && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel != null && CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel.m_BackPad != null)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUDEnemyPanel.m_StartPad.SimulateClickGraphicAndSoundOnly();
			}
		}
		if (!CFGInput.IsActivated(EActionCommand.Exit))
		{
			return;
		}
		if (m_AllowedMode == EPlayerHudLimiterMode.Confirm)
		{
			instance.OnEscapeKey();
			return;
		}
		ETurnAction currentAction = m_CurrentAction;
		if (currentAction != ETurnAction.None)
		{
			CancelOrdering();
			if (m_SelectedCharacter != null)
			{
				CFGCamera component = GetComponent<CFGCamera>();
				if (component != null)
				{
					component.ChangeFocus(m_SelectedCharacter, 0.5f, force: true);
				}
			}
		}
		else
		{
			instance.OnEscapeKey();
		}
	}

	private void OnLMB()
	{
		if (IsLocked)
		{
			return;
		}
		CFGGameObject cFGGameObject = m_GameObjectUnderCursor;
		if (cFGGameObject == null && m_UC_Cell != null)
		{
			cFGGameObject = GetGameObjectUnderCell();
		}
		if (m_CurrentAction.IsStdNonSpecial())
		{
			if (m_SelectedCharacter != null && m_TargetingAbility != null)
			{
				OnAttackableClick(cFGGameObject as CFGIAttackable);
			}
			return;
		}
		if (AllowedMode != 0)
		{
			switch (m_AllowedMode)
			{
			case EPlayerHudLimiterMode.Nothing:
			case EPlayerHudLimiterMode.Confirm:
				break;
			case EPlayerHudLimiterMode.SpecificTurnActionOnly:
				if (m_CurrentAction == m_AllowedTurnAction)
				{
				}
				break;
			}
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			if (m_FreeTargetingEnabled)
			{
				HandleFreeTargetingShootLMB();
				return;
			}
			break;
		}
		bool flag = true;
		if (m_CurrentAction == ETurnAction.Ricochet)
		{
			flag = false;
		}
		if (CurrentAction == ETurnAction.None && CellUnderCursor != null)
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component == null)
			{
				return;
			}
			if (m_CursorHelperState == ECursorHelper.MoveStairsUp)
			{
				component.MoveToFloor(CellUnderCursor.Floor + 1);
			}
			else if (m_CursorHelperState == ECursorHelper.MoveStairsDown)
			{
				component.MoveToFloor(CellUnderCursor.Floor);
			}
		}
		if (cFGGameObject == null)
		{
			return;
		}
		CFGCharacter cFGCharacter = cFGGameObject as CFGCharacter;
		if ((bool)cFGCharacter && cFGCharacter.IsAlive && cFGCharacter.Owner != null)
		{
			if (cFGCharacter.Owner.IsPlayer)
			{
				if (flag)
				{
					SelectCharacter(cFGCharacter, focus: true);
				}
				return;
			}
			if (m_SelectedCharacter != null && cFGCharacter.Owner.IsAi && cFGCharacter.BestDetectionType != 0)
			{
				ETurnAction currentAction = m_CurrentAction;
				if (currentAction != ETurnAction.Ricochet)
				{
					OnEnemyClick(cFGCharacter);
				}
				else if (m_RicochetObjects.Count > 0)
				{
					OnEnemyClick(cFGCharacter);
				}
				return;
			}
		}
		if (m_SelectedCharacter == null)
		{
			return;
		}
		CFGRicochetObject cFGRicochetObject = cFGGameObject as CFGRicochetObject;
		if ((bool)cFGRicochetObject && m_SelectedCharacter.CanMakeAction(ETurnAction.Ricochet) == EActionResult.Success && m_TargetedObject == null)
		{
			if (m_CurrentAction == ETurnAction.Ricochet)
			{
				TryAndAddRicochetObject(cFGRicochetObject);
			}
			else if (m_CurrentAction == ETurnAction.None)
			{
				BeginRicochetShoot(cFGRicochetObject);
			}
		}
		else if (cFGGameObject is CFGIAttackable target)
		{
			OnAttackableClick(target);
		}
	}

	public void OnSerialize(CFG_SG_Node node)
	{
		int value = 0;
		int value2 = 0;
		CFGSerializableObject cFGSerializableObject = m_TargetedObject as CFGSerializableObject;
		if (m_TargetedObject != null)
		{
			value2 = cFGSerializableObject.UniqueID;
		}
		if (m_SelectedCharacter != null)
		{
			value = m_SelectedCharacter.UniqueID;
		}
		node.Attrib_Set("TargetUUID", value2);
		node.Attrib_Set("SelectionUUID", value);
		int value3 = 0;
		int value4 = 0;
		int PosX = -1;
		int PosZ = -1;
		int Floor = -1;
		if (m_ActionLimitTargetCharacter != null)
		{
			value3 = m_ActionLimitTargetCharacter.UniqueID;
		}
		if (m_ActionLimitTargetUsable != null)
		{
			value4 = m_ActionLimitTargetUsable.UniqueID;
		}
		if (m_ActionLimitTargetCell != null)
		{
			m_ActionLimitTargetCell.DecodePosition(out PosX, out PosZ, out Floor);
		}
		node.Attrib_Set("PlayerActionLimiter", PlayerActionLimiter);
		node.Attrib_Set("ActionLimitTargetCharacter", value3);
		node.Attrib_Set("ActionLimitTargetUsable", value4);
		node.Attrib_Set("ActionLimitTargetCell", new Vector3(PosX, PosZ, Floor));
		node.Attrib_Set("AllowedMode", AllowedMode);
		node.Attrib_Set("AllowedAction", AllowedAction);
	}

	public bool OnDeserialize(CFG_SG_Node node)
	{
		int uUID = node.Attrib_Get("TargetUUID", 0);
		int uUID2 = node.Attrib_Get("SelectionUUID", 0);
		PlayerActionLimiter = node.Attrib_Get("PlayerActionLimiter", EPlayerActionLimit.Default);
		int uUID3 = node.Attrib_Get("ActionLimitTargetCharacter", 0);
		int uUID4 = node.Attrib_Get("ActionLimitTargetUsable", 0);
		m_ActionLimitTargetCharacter = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(uUID3, ESerializableType.NotSerializable);
		m_ActionLimitTargetUsable = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGUsableObject>(uUID4, ESerializableType.NotSerializable);
		Vector3 vector = node.Attrib_Get("ActionLimitTargetCell", new Vector3(-1f, -1f, -1f));
		m_ActionLimitTargetCell = CFGCellMap.GetCell(vector.x, vector.y, vector.z);
		AllowedMode = node.Attrib_Get("AllowedMode", EPlayerHudLimiterMode.Default);
		AllowedAction = node.Attrib_Get("AllowedAction", ETurnAction.None);
		CFGCharacter targetedObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(uUID, ESerializableType.NotSerializable);
		CFGCharacter character = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGCharacter>(uUID2, ESerializableType.NotSerializable);
		SelectCharacter(character, CFGSingleton<CFGGame>.Instance.IsInStrategic());
		m_TargetedObject = targetedObject;
		return true;
	}

	public bool OnPostSerialize()
	{
		CFGCharacter selectedCharacter = m_SelectedCharacter;
		m_SelectedCharacter = null;
		SelectCharacter(selectedCharacter, CFGSingleton<CFGGame>.Instance.IsInStrategic());
		return true;
	}

	public void SelectCharacter(CFGCharacter character, bool focus)
	{
		if ((m_ActionLimit != 0 && m_SelectedCharacter != null && !m_SelectedCharacter.Imprisoned) || (m_AllowedMode != 0 && m_SelectedCharacter != null && !m_SelectedCharacter.Imprisoned))
		{
			return;
		}
		if (character != m_SelectedCharacter)
		{
			CancelOrdering();
			if ((bool)m_SelectedCharacter)
			{
				m_SelectedCharacter.UnSelect();
			}
			m_SelectedCharacter = character;
			if ((bool)m_SelectedCharacter)
			{
				m_SelectedCharacter.Select();
				if (m_SelectedCharacter.CurrentCell != null)
				{
					m_LastRealGamepadPos = m_SelectedCharacter.CurrentCell.WorldPosition;
					m_LastGamepadPos = m_LastRealGamepadPos;
				}
				if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
				{
					m_UC_Cell = m_SelectedCharacter.CurrentCell;
					if (m_UC_Cell != null)
					{
						m_LastRealGamepadPos = m_UC_Cell.WorldPosition;
						m_LastGamepadPos = m_LastRealGamepadPos;
					}
				}
			}
			OnSelectionChanged();
			CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
			UpdateFreeTargetingEnabled();
			if ((bool)instance && (bool)instance.m_HUD)
			{
				List<CFGCharacterData> teamCharactersListTactical = CFGCharacterList.GetTeamCharactersListTactical();
				for (int i = 0; i < teamCharactersListTactical.Count; i++)
				{
					CFGCharacterData cFGCharacterData = teamCharactersListTactical[i];
					if (cFGCharacterData != null && cFGCharacterData.CurrentModel == m_SelectedCharacter)
					{
						instance.m_HUD.SelectCharacter(i);
						break;
					}
				}
			}
		}
		if (focus)
		{
			CFGCamera component = GetComponent<CFGCamera>();
			if (component != null && (!component.IsChangingFocus() || component.Focus != m_SelectedCharacter))
			{
				component.ChangeFocus(m_SelectedCharacter, 0.5f, force: true);
			}
		}
	}

	private void GenerateTargetableObjectList()
	{
		m_CurrentTargetableObject = -2;
		m_TargetableObjects.Clear();
		bool flag = true;
		if (m_SelectedCharacter == null)
		{
			return;
		}
		switch (m_CurrentAction)
		{
		case ETurnAction.Ricochet:
			foreach (CFGRicochetObject availableRicochetObject in m_AvailableRicochetObjects)
			{
				m_TargetableObjects.Add(availableRicochetObject);
			}
			if (m_RicochetObjects.Count <= 0)
			{
				break;
			}
			foreach (CFGIAttackable possibleOtherTarget in m_PossibleOtherTargets)
			{
				CFGGameObject cFGGameObject2 = possibleOtherTarget as CFGGameObject;
				if ((bool)cFGGameObject2)
				{
					m_TargetableObjects.Add(cFGGameObject2);
				}
			}
			break;
		case ETurnAction.Gunpoint:
			foreach (CFGCharacter item in m_CharactersToShowOnEnemyBar)
			{
				EActionResult eActionResult3 = m_SelectedCharacter.CanMakeAction(ETurnAction.Gunpoint, item);
				eActionResult3 &= ~EActionResult.InvalidTarget;
				if ((eActionResult3 & ~EActionResult.NotInCone) == 0)
				{
					m_TargetableObjects.Add(item);
				}
			}
			flag = false;
			break;
		case ETurnAction.AltFire_ConeShot:
			return;
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
			foreach (CFGCharacter item2 in m_CharactersToShowOnEnemyBar)
			{
				EActionResult eActionResult = m_SelectedCharacter.CanMakeAction(m_CurrentAction, item2);
				if (eActionResult == EActionResult.Success || eActionResult == EActionResult.NoAmmo || eActionResult == EActionResult.NotEnoughAP)
				{
					m_TargetableObjects.Add(item2);
				}
			}
			flag = false;
			foreach (CFGIAttackable otherTarget in m_SelectedCharacter.OtherTargets)
			{
				CFGGameObject cFGGameObject = otherTarget as CFGGameObject;
				if (!(cFGGameObject == null))
				{
					EActionResult eActionResult2 = m_SelectedCharacter.CanMakeAction(m_CurrentAction, otherTarget);
					if (eActionResult2 == EActionResult.Success || eActionResult2 == EActionResult.NoAmmo || eActionResult2 == EActionResult.NotEnoughAP)
					{
						m_TargetableObjects.Add(cFGGameObject);
					}
				}
			}
			break;
		}
		if (m_TargetingAbility != null)
		{
			foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
			{
				if (m_TargetingAbility.CanTargetAbilityOn(character) && m_TargetingAbility.CheckLineOfSightAndFire(character))
				{
					m_TargetableObjects.Add(character);
				}
			}
			foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
			{
				CFGGameObject cFGGameObject3 = otherAttackableObject as CFGGameObject;
				if (!(cFGGameObject3 == null) && m_TargetingAbility.CanTargetAbilityOn(otherAttackableObject) && m_TargetingAbility.CheckLineOfSightAndFire(otherAttackableObject))
				{
					m_TargetableObjects.Add(cFGGameObject3);
				}
			}
		}
		if (flag)
		{
			m_TargetableObjects.Sort(SortTargets);
		}
		if (m_TargetableObjects.Count > 0)
		{
			m_CurrentTargetableObject = -1;
		}
	}

	private int SortTargets(CFGGameObject ta, CFGGameObject tb)
	{
		if (ta == null)
		{
			return -1;
		}
		if (tb == null)
		{
			return 1;
		}
		float num = Vector3.Distance(ta.transform.position, m_SelectedCharacter.CurrentCell.WorldPosition);
		float value = Vector3.Distance(tb.transform.position, m_SelectedCharacter.CurrentCell.WorldPosition);
		return num.CompareTo(value);
	}

	private void SelectTargetableObject(CFGIAttackable tgt)
	{
		if (tgt == null)
		{
			return;
		}
		CFGGameObject cFGGameObject = tgt as CFGGameObject;
		if (cFGGameObject == null)
		{
			return;
		}
		for (int i = 0; i < m_TargetableObjects.Count; i++)
		{
			if (m_TargetableObjects[i] == cFGGameObject)
			{
				m_CurrentTargetableObject = i;
				MoveCursorToCell(tgt.CurrentCell, null);
				break;
			}
		}
	}

	private void SelectNextTargetable()
	{
		if (m_ActionLimit == EPlayerActionLimit.Default && !m_FreeTargetingEnabled && m_CurrentTargetableObject >= -1)
		{
			m_CurrentTargetableObject++;
			if (m_CurrentTargetableObject >= m_TargetableObjects.Count)
			{
				m_CurrentTargetableObject = 0;
			}
			ActivateCurrentTargetable();
		}
	}

	private void SelectPrevTargetable()
	{
		if (m_ActionLimit == EPlayerActionLimit.Default && !m_FreeTargetingEnabled && m_CurrentTargetableObject >= -1)
		{
			m_CurrentTargetableObject--;
			if (m_CurrentTargetableObject < 0)
			{
				m_CurrentTargetableObject = m_TargetableObjects.Count - 1;
			}
			ActivateCurrentTargetable();
		}
	}

	private void ActivateCurrentTargetable()
	{
		if (m_CurrentTargetableObject < 0 || m_CurrentTargetableObject >= m_TargetableObjects.Count || m_CurrentAction == ETurnAction.None || m_ActionLimit != 0)
		{
			return;
		}
		CFGIAttackable cFGIAttackable = m_TargetableObjects[m_CurrentTargetableObject] as CFGIAttackable;
		if (m_TargetingAbility != null && cFGIAttackable != null)
		{
			OnAttackableClick(cFGIAttackable);
			MoveCursorToCell(cFGIAttackable.CurrentCell, cFGIAttackable as CFGGameObject);
			return;
		}
		CFGCharacter cFGCharacter = m_TargetableObjects[m_CurrentTargetableObject] as CFGCharacter;
		if (cFGCharacter != null)
		{
			OnEnemyClick(cFGCharacter);
			MoveCursorToCell(cFGCharacter.CurrentCell, cFGCharacter);
			return;
		}
		CFGRicochetObject cFGRicochetObject = m_TargetableObjects[m_CurrentTargetableObject] as CFGRicochetObject;
		if (cFGRicochetObject != null)
		{
			MoveCursorToCell(cFGRicochetObject.Cell, cFGRicochetObject);
			m_TargetedObject = null;
		}
		CFGShootableObject cFGShootableObject = m_TargetableObjects[m_CurrentTargetableObject] as CFGShootableObject;
		if (cFGShootableObject != null)
		{
			OnAttackableClick(cFGShootableObject);
			MoveCursorToCell(cFGShootableObject.CurrentCell, cFGShootableObject);
		}
	}

	private void MoveCursorToCell(CFGCell Cell, CFGGameObject Focus)
	{
		if (Cell == null)
		{
			return;
		}
		m_UC_Cell = Cell;
		m_LastGamepadPos = m_UC_Cell.WorldPosition;
		m_LastRealGamepadPos = m_LastGamepadPos;
		if ((bool)Focus)
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus(Focus);
			}
		}
	}

	public bool IsCharacterInAOETargetsList(CFGCharacter character)
	{
		return m_AOEObjects.Contains(character);
	}

	private bool CheckRicochetObject(CFGRicochetObject ro, CFGRicochetObject currentRO, EFloorLevelType floor)
	{
		if (m_RicochetObjects == null)
		{
			m_RicochetObjects = new List<CFGRicochetObject>();
		}
		if ((bool)ro && ro.Cell == null)
		{
			ro.UpdateCell();
			if (ro.Cell == null)
			{
				Debug.LogWarning("Could not find cell for ricochet object. Action aborted.");
				return false;
			}
		}
		if (ro == null || ro == currentRO || m_RicochetObjects.Contains(ro))
		{
			return false;
		}
		if (currentRO != null)
		{
			if (CFGCellMap.GetLineOf(currentRO.Cell, ro.Cell, 1000000, 32, bUseStartSideSteps: false, bUseEndSideSteps: false) != 0)
			{
				return false;
			}
			Vector3 lhs = currentRO.Cell.WorldPosition - ro.Cell.WorldPosition;
			lhs.Normalize();
			float num = Vector3.Dot(lhs, m_LastRicochetDir);
			float num2 = Mathf.Cos((float)Math.PI / 180f * currentRO.Angle * 0.5f);
			if (num < num2)
			{
				return false;
			}
		}
		m_AvailableRicochetObjects.Add(ro);
		return true;
	}

	private void CheckRicochetTarget(CFGIAttackable ch, EFloorLevelType floor, CFGRicochetObject currentRO)
	{
		if (ch == null || !ch.IsAlive || ch.CurrentCell == null)
		{
			return;
		}
		CFGCharacter cFGCharacter = ch as CFGCharacter;
		if (cFGCharacter == null)
		{
			CFGPlayerOwner playerOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner;
			bool flag = false;
			if (playerOwner != null)
			{
				foreach (CFGCharacter character in playerOwner.Characters)
				{
					if (character == null || !character.OtherTargets.Contains(ch))
					{
						continue;
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		else if (cFGCharacter.BestDetectionType == EBestDetectionType.NotDetected)
		{
			return;
		}
		Vector3 lhs = currentRO.Cell.WorldPosition - ch.CurrentCell.WorldPosition;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, m_LastRicochetDir);
		float num2 = Mathf.Cos((float)Math.PI / 180f * currentRO.Angle * 0.5f);
		if (!(num < num2) && CFGCellMap.GetLineOf(currentRO.Cell, ch.CurrentCell, 1000000, 32, bUseStartSideSteps: false, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None)
		{
			if (cFGCharacter != null)
			{
				m_PossibleTargets.Add(cFGCharacter);
			}
			m_PossibleOtherTargets.Add(ch);
		}
	}

	private void GenerateAvailableRicochetObjects(CFGRicochetObject currentRO)
	{
		m_AvailableRicochetObjects.Clear();
		m_PossibleTargets.Clear();
		m_PossibleOtherTargets.Clear();
		EFloorLevelType floor;
		if (currentRO == null)
		{
			if (!(m_SelectedCharacter != null))
			{
				return;
			}
			floor = (EFloorLevelType)m_SelectedCharacter.CurrentCell.Floor;
			{
				foreach (CFGRicochetObject visibleRicochetObject in m_SelectedCharacter.VisibleRicochetObjects)
				{
					CheckRicochetObject(visibleRicochetObject, null, floor);
				}
				return;
			}
		}
		floor = (EFloorLevelType)currentRO.Cell.Floor;
		foreach (CFGRicochetObject ricochetObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RicochetObjects)
		{
			CheckRicochetObject(ricochetObject, currentRO, floor);
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character != null && character.IsAlive && ((character.Owner != null && character.Owner.IsAi) || character.Owner == null))
			{
				CheckRicochetTarget(character, floor, currentRO);
			}
		}
		foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
		{
			CFGOwner owner = otherAttackableObject.GetOwner();
			if (otherAttackableObject != null && otherAttackableObject.IsAlive && ((owner != null && owner.IsAi) || owner == null))
			{
				CheckRicochetTarget(otherAttackableObject, floor, currentRO);
			}
		}
		m_SelectedCharacter.SortVisibleEnemiesList(ref m_PossibleTargets);
	}

	private void TryAndAddRicochetObject(CFGRicochetObject newobject)
	{
		if (!(newobject == null) && !m_RicochetObjects.Contains(newobject) && m_AvailableRicochetObjects.Contains(newobject))
		{
			m_RicochetObjects.Add(newobject);
			if (m_RicochetObjects.Count < 2)
			{
				m_LastRicochetDir = m_SelectedCharacter.CurrentCell.WorldPosition - newobject.Cell.WorldPosition;
			}
			else
			{
				m_LastRicochetDir = m_RicochetObjects[m_RicochetObjects.Count - 2].Cell.WorldPosition - newobject.Cell.WorldPosition;
			}
			m_LastRicochetDir.Normalize();
			GenerateAvailableRicochetObjects(newobject);
			GenerateTargetableObjectList();
		}
	}

	private void BeginRicochetShoot(CFGRicochetObject robject)
	{
		if (m_ActionLimit == EPlayerActionLimit.Default && !(robject == null) && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn && m_SelectedCharacter != null && m_SelectedCharacter.CurrentAction == ETurnAction.None && m_SelectedCharacter.ActionPoints > 0 && m_SelectedCharacter.HaveAbility(ETurnAction.Ricochet) && m_SelectedCharacter.CurrentWeapon != null && m_SelectedCharacter.CurrentWeapon.CanShoot() && m_SelectedCharacter.VisibleRicochetObjects.Contains(robject))
		{
			m_CurrentAction = ETurnAction.Ricochet;
			m_AvailableRicochetObjects.Clear();
			m_RicochetObjects.Clear();
			GenerateAvailableRicochetObjects(null);
			TryAndAddRicochetObject(robject);
		}
	}

	private void ConeOfFire_GeneratePotentialList()
	{
		m_AOEPotentialObjects.Clear();
		if (m_SelectedCharacter == null || m_SelectedCharacter.CurrentCell == null || m_SelectedCharacter.CurrentWeapon == null)
		{
			return;
		}
		CFGDef_Weapon definition = m_SelectedCharacter.CurrentWeapon.m_Definition;
		if (definition == null)
		{
			return;
		}
		CFGCell currentCell = m_SelectedCharacter.CurrentCell;
		Vector3 worldPosition = currentCell.WorldPosition;
		int floor = currentCell.Floor;
		float coneOfFireRange = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ConeOfFireRange;
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character == null || (character.Owner != null && character.Owner.IsPlayer) || !character.IsAlive)
			{
				continue;
			}
			CFGCell currentCell2 = character.CurrentCell;
			if (currentCell2 != null && currentCell2.Floor == floor && m_SelectedCharacter.GetChanceToHit(character, null, currentCell, currentCell2, ETurnAction.AltFire_ConeShot) != 0 && CFGCellMap.GetLineOf(currentCell, currentCell2, 10000, 32, bUseStartSideSteps: true, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None)
			{
				float num = Vector3.Distance(worldPosition, character.CurrentCell.WorldPosition);
				if (!(num > coneOfFireRange))
				{
					m_AOEPotentialObjects.Add(character);
				}
			}
		}
		foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
		{
			if (otherAttackableObject == null || !otherAttackableObject.IsAlive)
			{
				continue;
			}
			CFGCell currentCell3 = otherAttackableObject.CurrentCell;
			if (currentCell3 != null && currentCell3.Floor == floor && CFGCellMap.GetLineOf(currentCell, currentCell3, 10000, 32, bUseStartSideSteps: true, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None && m_SelectedCharacter.GetChanceToHit(otherAttackableObject, null, currentCell, currentCell3, ETurnAction.Shoot) != 0)
			{
				float num2 = Vector3.Distance(worldPosition, otherAttackableObject.CurrentCell.WorldPosition);
				if (!(num2 > coneOfFireRange))
				{
					m_AOEPotentialObjects.Add(otherAttackableObject);
				}
			}
		}
	}

	private void ConeOfFire_GenerateTargetList()
	{
		List<CFGIAttackable> old_list = new List<CFGIAttackable>(m_AOEObjects);
		m_AOEObjects.Clear();
		if (m_TargetingAbility != null)
		{
			m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingPoint);
			SetFlagsFlashes(old_list, m_AOEObjects);
		}
		else
		{
			if (m_AOEPotentialObjects.Count == 0)
			{
				return;
			}
			CFGCell currentCell = m_SelectedCharacter.CurrentCell;
			Vector3 worldPosition = currentCell.WorldPosition;
			float fReqDot = Mathf.Cos((float)Math.PI / 180f * m_ConeVisualization.m_ConeAngle * 0.5f);
			CFGCell[] tiles = new CFGCell[3] { m_SelectedCharacter.CurrentCell, null, null };
			Vector3 forward = m_ConeVisualization.transform.forward;
			CFGCellMap.FillUpTable(m_SelectedCharacter.CurrentCell, forward, ref tiles);
			List<CFGIAttackable> list = null;
			for (int i = 0; i < 3; i++)
			{
				List<CFGIAttackable> list2 = ConeOfFire_GenerateTargetListFromCell(tiles[i], worldPosition, forward, fReqDot);
				if (list2 != null && list2.Count != 0 && (list == null || list2.Count > list.Count))
				{
					list = list2;
				}
			}
			if (list != null)
			{
				foreach (CFGIAttackable item in list)
				{
					m_AOEObjects.Add(item);
				}
			}
			SetFlagsFlashes(old_list, m_AOEObjects);
		}
	}

	private List<CFGIAttackable> ConeOfFire_GenerateTargetListFromCell(CFGCell cell, Vector3 scpos, Vector3 fwd, float fReqDot)
	{
		if (cell == null)
		{
			return null;
		}
		List<CFGIAttackable> list = new List<CFGIAttackable>();
		for (int i = 0; i < m_AOEPotentialObjects.Count; i++)
		{
			if (m_AOEPotentialObjects[i] != null && m_AOEPotentialObjects[i].IsAlive)
			{
				Vector3 worldPosition = m_AOEPotentialObjects[i].CurrentCell.WorldPosition;
				Vector3 normalized = (worldPosition - scpos).normalized;
				float num = Vector3.Dot(normalized, fwd);
				if (!(num < fReqDot))
				{
					list.Add(m_AOEPotentialObjects[i]);
				}
			}
		}
		return list;
	}

	private void SetFlagsFlashes(List<CFGIAttackable> old_list, List<CFGIAttackable> new_list)
	{
		foreach (CFGIAttackable item in new_list)
		{
			if (!old_list.Contains(item))
			{
				CFGCharacter cFGCharacter = item as CFGCharacter;
				if ((bool)cFGCharacter)
				{
					cFGCharacter.FlagNeedFlash2 = true;
				}
			}
		}
	}

	private CFGGameObject GetGameObjectUnderCell()
	{
		if (m_UC_Cell == null)
		{
			return null;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			Ray ray = new Ray(m_LastGamepadPos + Vector3.up * 10f, Vector3.down);
			RaycastHit[] array = Physics.RaycastAll(ray);
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					CFGLocationObject component = array[i].collider.GetComponent<CFGLocationObject>();
					if (component != null && component.State != ELocationState.HIDDEN)
					{
						return component;
					}
				}
			}
			return null;
		}
		if (m_UC_Cell.CurrentCharacter != null && m_UC_Cell.CurrentCharacter.IsAlive)
		{
			return m_UC_Cell.CurrentCharacter;
		}
		foreach (CFGRicochetObject ricochetObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RicochetObjects)
		{
			if (ricochetObject != null && ricochetObject.Cell.EP == m_UC_Cell.EP)
			{
				if (ricochetObject.Cell != m_UC_Cell)
				{
					Debug.LogError("EP is the same, objects are not!");
				}
				return ricochetObject;
			}
		}
		foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
		{
			if (otherAttackableObject != null && otherAttackableObject.CurrentCell == m_UC_Cell)
			{
				return otherAttackableObject as CFGGameObject;
			}
		}
		if ((bool)m_UC_Cell.DoorObject)
		{
			return m_UC_Cell.DoorObject;
		}
		if ((bool)m_UC_Cell.UsableObject)
		{
			return m_UC_Cell.UsableObject;
		}
		return null;
	}

	private CFGCellObject GetCellObjectUnderCursor()
	{
		CFGCellObject cFGCellObject = ((!(m_SelectedCharacter != null)) ? null : m_SelectedCharacter.m_CurrentCellObjectInside);
		RaycastHit[] array = Physics.RaycastAll(Camera.ScreenPointToRay(Input.mousePosition));
		for (int i = 0; i < array.Length; i++)
		{
			CFGCellObjectPart component = array[i].collider.GetComponent<CFGCellObjectPart>();
			if (component != null && component.Parent != cFGCellObject)
			{
				return component.Parent;
			}
		}
		return null;
	}

	private CFGGameObject GetGameObjectUnderCursor()
	{
		CFGCamera component = GetComponent<CFGCamera>();
		if (component == null)
		{
			return null;
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				return null;
			}
			if (m_UC_Cell != null)
			{
				return GetGameObjectUnderCell();
			}
			return null;
		}
		if (CFGSingleton<CFGWindowMgr>.Instance.IsCursorOverUI())
		{
			return null;
		}
		CFGGameObject cFGGameObject = null;
		Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
		CFGCell cellFromRay = GetCellFromRay(ray);
		if (Physics.Raycast(ray, out var hitInfo, 1000f))
		{
			if ((bool)hitInfo.collider)
			{
				cFGGameObject = hitInfo.collider.GetComponent<CFGGameObject>();
				if (!cFGGameObject && hitInfo.collider.transform.parent != null)
				{
					cFGGameObject = hitInfo.collider.transform.parent.GetComponent<CFGGameObject>();
				}
				if ((bool)cFGGameObject)
				{
					CFGCell characterCell = CFGCellMap.GetCharacterCell(cFGGameObject.Transform.position);
					if (characterCell != null && characterCell.Floor > (int)component.CurrentFloorLevel)
					{
						cFGGameObject = null;
					}
					if ((bool)cFGGameObject && (bool)characterCell && (bool)cellFromRay && characterCell.Floor < cellFromRay.Floor)
					{
						cFGGameObject = null;
					}
					CFGLocationObject cFGLocationObject = cFGGameObject as CFGLocationObject;
					if ((bool)cFGLocationObject && cFGLocationObject.State == ELocationState.LOCKED)
					{
						cFGLocationObject = null;
					}
				}
			}
			if (!cFGGameObject)
			{
				RaycastHit[] array = Physics.RaycastAll(ray, 1000f);
				for (int i = 0; i < array.Length; i++)
				{
					cFGGameObject = array[i].collider.GetComponent<CFGGameObject>();
					if (!cFGGameObject && array[i].collider.transform.parent != null)
					{
						cFGGameObject = array[i].collider.transform.parent.GetComponent<CFGGameObject>();
					}
					if ((bool)cFGGameObject)
					{
						CFGCell characterCell2 = CFGCellMap.GetCharacterCell(cFGGameObject.Transform.position);
						if (characterCell2 != null && characterCell2.Floor > (int)component.CurrentFloorLevel)
						{
							cFGGameObject = null;
						}
						if ((bool)cFGGameObject && (bool)cellFromRay && (bool)characterCell2 && characterCell2.Floor < cellFromRay.Floor)
						{
							cFGGameObject = null;
						}
					}
					CFGLocationObject cFGLocationObject2 = cFGGameObject as CFGLocationObject;
					if ((bool)cFGLocationObject2 && cFGLocationObject2.State == ELocationState.LOCKED)
					{
						cFGLocationObject2 = null;
					}
					if ((bool)cFGGameObject)
					{
						break;
					}
				}
			}
		}
		return cFGGameObject;
	}

	private CFGCell GetCellUnderCursor()
	{
		if (!IsEnabled())
		{
			return null;
		}
		if (!CFGCellMap.IsValid)
		{
			return null;
		}
		CFGCell cFGCell = null;
		bool flag = false;
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		Vector3 vector = Vector3.left;
		Vector3 vector2 = Vector3.forward;
		if (component != null)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				vector = CFGMath.DiagonalAlignedProjectedVectorOnPlane(-component.transform.right, Vector3.up);
				vector2 = CFGMath.DiagonalAlignedProjectedVectorOnPlane(component.transform.forward, Vector3.up);
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				vector = CFGMath.ProjectedVectorOnPlane(-component.transform.right, Vector3.up);
				vector2 = CFGMath.ProjectedVectorOnPlane(component.transform.forward, Vector3.up);
			}
		}
		flag |= UpdateTargetCellPos(CFGJoyManager.ButtonForCM_LEFT, vector);
		flag |= UpdateTargetCellPos(CFGJoyManager.ButtonForCM_RIGHT, -vector);
		flag |= UpdateTargetCellPos(CFGJoyManager.ButtonForCM_UP, vector2);
		flag |= UpdateTargetCellPos(CFGJoyManager.ButtonForCM_DOWN, -vector2);
		float num = (float)component.CurrentFloorLevel * 2.5f;
		bool flag2 = false;
		if (num != m_LastRealGamepadPos.y)
		{
			m_LastRealGamepadPos.y = num;
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
			{
				flag2 = true;
			}
		}
		if (flag || flag2)
		{
			m_LastGamepadPos = m_LastRealGamepadPos;
			CFGCell cell = CFGCellMap.GetCell(m_LastRealGamepadPos);
			if (cell != null && !cell.HaveFloor && cell.Floor > 0)
			{
				m_LastGamepadPos.y -= 2.5f;
				cell = CFGCellMap.GetCell(m_LastGamepadPos);
				if ((bool)cell && !cell.HaveFloor && cell.Floor > 0)
				{
					m_LastGamepadPos.y -= 2.5f;
					if (m_LastGamepadPos.y < 0f)
					{
						m_LastGamepadPos.y = 0f;
					}
				}
			}
		}
		if (flag)
		{
			cFGCell = OnGPMove();
			if (cFGCell != null)
			{
				Update_GameObjectUnderControllerCursor();
				return cFGCell;
			}
		}
		Vector3 mousePosition = Input.mousePosition;
		if (CFGInput.ExclusiveInputDevice != EInputMode.Gamepad)
		{
			float num2 = Vector3.Distance(mousePosition, m_LastMousePos);
			m_LastMousePos = mousePosition;
			if (num2 > 0.001f && !float.IsInfinity(num2))
			{
				CFGInput.ChangeInputMode(EInputMode.KeyboardAndMouse);
			}
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			Vector3 lastGamepadPos = m_LastGamepadPos;
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				lastGamepadPos.y = 0f;
			}
			cFGCell = CFGCellMap.GetCell(lastGamepadPos);
			if (cFGCell != null)
			{
				m_UC_Cell = cFGCell;
			}
			Update_GameObjectUnderControllerCursor();
			if (flag2)
			{
				return OnGPMove();
			}
			return m_UC_Cell;
		}
		CFGInput.ChangeInputMode(EInputMode.KeyboardAndMouse);
		return GetCellFromRay(Camera.ScreenPointToRay(mousePosition));
	}

	private void Update_GameObjectUnderControllerCursor()
	{
		CFGGameObject gameObjectUnderCell = GetGameObjectUnderCell();
		if (gameObjectUnderCell != m_GameObjectUnderControllerCursor)
		{
			if (m_GameObjectUnderCursor != gameObjectUnderCell && m_GameObjectUnderCursor != null)
			{
				m_GameObjectUnderCursor.IsUnderCursor = false;
				m_GameObjectUnderCursor.OnCursorLeave();
				m_GameObjectUnderCursor = null;
			}
			if (m_GameObjectUnderControllerCursor != null)
			{
				m_GameObjectUnderControllerCursor.IsUnderCursor = false;
				m_GameObjectUnderControllerCursor.OnCursorLeave();
			}
			m_GameObjectUnderControllerCursor = gameObjectUnderCell;
			if (m_GameObjectUnderControllerCursor != null)
			{
				m_GameObjectUnderControllerCursor.IsUnderCursor = false;
				m_GameObjectUnderControllerCursor.OnCursorEnter();
			}
		}
	}

	private CFGCell GetCellFromRay(Ray r)
	{
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		CFGCell cFGCell = null;
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			Vector3 vector = Vector3.zero;
			vector.y = -1f;
			float num = 1.01E+10f;
			bool flag = false;
			RaycastHit[] array = Physics.RaycastAll(r);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.collider.GetComponent<AmplifyColorVolume>() != null || raycastHit.distance > num || raycastHit.collider.name == "Horseman")
				{
					continue;
				}
				if (raycastHit.collider.name.Contains("terrain"))
				{
					vector = raycastHit.point;
					flag = true;
					num = raycastHit.distance;
					continue;
				}
				CFGLocationObject component2 = raycastHit.collider.GetComponent<CFGLocationObject>();
				if (!component2 || component2.State != ELocationState.HIDDEN)
				{
					float y = raycastHit.collider.transform.position.y;
					if (y >= 0f && (vector.y < 0f || y < vector.y))
					{
						vector = raycastHit.collider.transform.position;
						flag = false;
						num = raycastHit.distance;
					}
				}
			}
			if (flag)
			{
				cFGCell = CFGCellMap.GetCharacterCell(vector);
				m_LastGamepadPos = vector;
				return cFGCell;
			}
			cFGCell = CFGCellMap.GetCharacterCell(vector);
		}
		else
		{
			Plane plane = default(Plane);
			for (int num2 = (int)component.CurrentFloorLevel; num2 >= 0; num2--)
			{
				plane.SetNormalAndPosition(Vector3.up, new Vector3(0f, 2.5f * (float)num2, 0f));
				if (plane.Raycast(r, out var enter))
				{
					Vector3 point = (m_MouseWorldPos = r.GetPoint(enter));
					Shader.SetGlobalVector("_WorldCursorPos", new Vector4(point.x, point.y, point.z, 1f));
					point.y += 0.01f;
					cFGCell = CFGCellMap.GetCell(point);
					if ((bool)cFGCell && (num2 == 0 || cFGCell.CheckFlag(0, 2)))
					{
						return cFGCell;
					}
				}
			}
		}
		return null;
	}

	private void FindBestCellForCurrentAction()
	{
	}

	private void Usable_Activate()
	{
		if (!m_FreeTargetingCanShoot)
		{
			return;
		}
		if (m_ConeVisualization == null && m_AOE_CircleHelper == null && m_TargetedObject == null)
		{
			if (!m_TargetingAbility.IsSelfCast)
			{
				Debug.LogError("Useable: " + m_AOEObjects.Count + " tgts " + m_TargetingAbility.SelectableTargetTypes);
				return;
			}
			m_TargetedObject = m_SelectedCharacter;
		}
		if (m_AOEObjects.Contains(m_TargetedObject))
		{
			m_AOEObjects.Remove(m_TargetedObject);
		}
		SelectedCharacter.MakeAction(m_CurrentAction, m_TargetedObject, m_FreeTargetingCell, m_AOEObjects);
		CancelOrdering();
	}

	private void Usable_HandleOnAttackableClick(CFGIAttackable target, CFGCamera camera)
	{
		if (target == null || m_TargetingAbility.CanCastOnCellOnly())
		{
			if (m_FreeTargetingEnabled && m_TargetingAbility.CanCastOnCell(m_UC_Cell))
			{
				Usable_Activate();
			}
			return;
		}
		if (m_TargetingAbility.GetAOEType() == eAOE_Type.Cone)
		{
			CFGCharacter cFGCharacter = target as CFGCharacter;
			if ((bool)cFGCharacter && cFGCharacter == m_SelectedCharacter)
			{
				return;
			}
		}
		if (!m_TargetingAbility.CanTargetAbilityOn(target))
		{
			return;
		}
		m_FreeTargetingCanShoot = true;
		m_FreeTargetingCell = target.CurrentCell;
		m_FreeTargetingPoint = m_FreeTargetingCell.WorldPosition;
		m_TargetedObject = target;
		m_FreeTargetingEnabled = false;
		camera.ChangeFocus(target as CFGGameObject, 0.5f, force: true);
		switch (m_TargetingAbility.GetAOEType())
		{
		case eAOE_Type.Cone:
			ConeOfFireViz_RotateTowards(target.CurrentCell.WorldPosition);
			break;
		case eAOE_Type.Circle:
		case eAOE_Type.Sphere:
			if (m_AOE_CircleHelper != null)
			{
				if (CurrentAction != ETurnAction.Shriek)
				{
					m_AOE_CircleHelper.transform.position = m_TargetedObject.CurrentCell.WorldPosition + AOECIRCLEHELPERADD;
				}
				m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingPoint);
			}
			break;
		case eAOE_Type.None:
			break;
		}
	}

	private void ActivateStdNonSpecialAbility(ETurnAction Action, bool bInstant)
	{
		CancelOrdering();
		if (m_SelectedCharacter.CurrentAction != ETurnAction.None)
		{
			return;
		}
		CFGAbility ability = m_SelectedCharacter.GetAbility(Action);
		if (ability == null || ability.IsPassive)
		{
			return;
		}
		m_TargetingAbility = ability;
		m_CurrentAction = Action;
		GenerateTargetableObjectList();
		Vector3 worldPosition = m_SelectedCharacter.CurrentCell.WorldPosition;
		m_FreeTargetingCanShoot = false;
		SetUsableFreeTargeting();
		CFGIAttackable targetedObject = m_TargetedObject;
		if (targetedObject != null)
		{
			worldPosition = targetedObject.CurrentCell.WorldPosition;
			m_FreeTargetingCanShoot = true;
		}
		else if (m_TargetingAbility.GetAOEType() == eAOE_Type.Cone)
		{
			worldPosition += Vector3.forward;
			m_FreeTargetingCanShoot = true;
		}
		else
		{
			m_FreeTargetingCanShoot = m_SelectedCharacter.CanMakeAction(m_CurrentAction, m_TargetedObject, m_UC_Cell, m_AOEObjects) == EActionResult.Success;
		}
		if (ability.IsSelfCastOnly && bInstant)
		{
			Usable_Activate();
			return;
		}
		m_FreeTargetingCell = CFGCellMap.GetCell(worldPosition);
		m_FreeTargetingPoint = m_FreeTargetingCell.WorldPosition;
		switch (m_TargetingAbility.GetAOEType())
		{
		case eAOE_Type.VisibleEnemies:
		case eAOE_Type.Everyone:
			m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingPoint);
			break;
		case eAOE_Type.Cone:
			ConeOfFireViz_Init(m_TargetingAbility.GetAOERadiusOrAngle(), m_SelectedCharacter.CurrentCell.WorldPosition, worldPosition, m_TargetingAbility.GetRange());
			break;
		case eAOE_Type.Circle:
		case eAOE_Type.Sphere:
			SpawnAOECircleHelper(worldPosition, m_TargetingAbility.GetAOERadiusOrAngle());
			m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingPoint);
			break;
		}
		switch (CurrentAction)
		{
		case ETurnAction.Cannibal:
			RenderCannibalHelpers(0);
			break;
		case ETurnAction.Finder:
			RenderFinderHelpers(0);
			break;
		}
	}

	private void ActivateItemOnTargeted()
	{
		if (m_TargetingAbility.CanCastOnType(eTargetableType.Cell))
		{
			m_SelectedCharacter.MakeAction(m_CurrentAction, m_TargetedObject, m_UC_Cell, m_AOEObjects);
		}
		else if (m_TargetingAbility.CanCastOn(m_TargetedObject))
		{
			m_SelectedCharacter.MakeAction(m_CurrentAction, m_TargetedObject, m_UC_Cell, m_AOEObjects);
		}
	}

	private void SelecteAbilityDefTarget()
	{
		m_TargetedObject = m_TargetingAbility.GetFirstValidTarget();
		if (m_TargetedObject == null && m_TargetingAbility.CanCastOnType(eTargetableType.Self) && m_TargetingAbility.IsTargetInfluenced(m_SelectedCharacter, m_SelectedCharacter.CurrentCell, m_SelectedCharacter) && (m_TargetingAbility.GetAOEType() == eAOE_Type.Circle || m_TargetingAbility.GetAOEType() == eAOE_Type.Sphere))
		{
			m_TargetedObject = m_SelectedCharacter;
		}
	}

	private void SetUsableFreeTargeting()
	{
		if (m_TargetingAbility == null || !m_TargetingAbility.CanSelectTarget())
		{
			SelecteAbilityDefTarget();
			m_FreeTargetingEnabled = false;
			return;
		}
		bool flag = false;
		switch (m_CurrentAction)
		{
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
			m_FreeTargetingEnabled = false;
			flag = true;
			break;
		default:
			m_FreeTargetingEnabled = false;
			break;
		}
		if (m_FreeTargetingEnabled)
		{
			m_TargetedObject = null;
			return;
		}
		if (m_TargetedObject == null)
		{
			SelecteAbilityDefTarget();
			if (m_TargetedObject == null)
			{
				if (flag)
				{
					m_FreeTargetingEnabled = true;
				}
				return;
			}
		}
		CFGCamera component = GetComponent<CFGCamera>();
		component.ChangeFocus(SelectedCharacter.Transform.position, m_TargetedObject.Position, 0.5f, force: true);
		m_FreeTargetingCanShoot = true;
		m_FreeTargetingCell = CFGCellMap.GetCell(m_TargetedObject.CurrentCell.WorldPosition);
		m_FreeTargetingPoint = m_FreeTargetingCell.WorldPosition;
		switch (m_TargetingAbility.GetAOEType())
		{
		case eAOE_Type.Cone:
			ConeOfFireViz_RotateTowards(m_TargetedObject.CurrentCell.WorldPosition);
			break;
		case eAOE_Type.Circle:
		case eAOE_Type.Sphere:
			if (m_AOE_CircleHelper != null)
			{
				m_AOE_CircleHelper.transform.position = m_TargetedObject.CurrentCell.WorldPosition + AOECIRCLEHELPERADD;
				m_TargetingAbility.GenerateTargetList(m_FreeTargetingCell, ref m_AOEObjects, m_FreeTargetingPoint);
			}
			break;
		}
	}
}
