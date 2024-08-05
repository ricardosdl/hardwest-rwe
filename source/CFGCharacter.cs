using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CFGCharacter : CFGGameObject, CFGIAttackable
{
	private enum EAbilityAnimState
	{
		Stopped,
		Preparing,
		WaitingForWeaponRemove,
		WaitingForAnimEnd,
		WaitingForWeaponReequip
	}

	public enum EActionState
	{
		None,
		Rotating,
		Preparing,
		Doing,
		Ending
	}

	[Serializable]
	public class CharPartColorGroup
	{
		public Renderer m_Renderer;

		public Color[] m_Colors;
	}

	[Serializable]
	public class CharPartsGroup
	{
		public enum EPartsGroupType
		{
			OneFromList,
			OneFromListOrNone,
			AnyCombination
		}

		public EPartsGroupType m_GroupType = EPartsGroupType.OneFromListOrNone;

		public Renderer[] m_Parts;
	}

	[Serializable]
	public class CharPartPreset
	{
		public Renderer m_Part;

		public Color m_Color = Color.white;
	}

	[Serializable]
	public class CharCustomPreset
	{
		public CharPartPreset[] m_VisibleParts;
	}

	private enum EDeathDirection
	{
		FRONT,
		BACK,
		LEFT,
		RIGHT
	}

	private struct DeathAnimation
	{
		public EDeathDirection m_DeathDirection;

		public List<EDeathDirection> m_AllowedShotDirections;

		public DeathAnimation(EDeathDirection _DeathDirection, List<EDeathDirection> _AllowedShotDirections)
		{
			m_DeathDirection = _DeathDirection;
			m_AllowedShotDirections = _AllowedShotDirections;
		}
	}

	private struct IdAngle
	{
		public int id;

		public float angle;

		public Vector3 dir;

		public IdAngle(int _id, float _angle, Vector3 _dir)
		{
			id = _id;
			angle = _angle;
			dir = _dir;
		}
	}

	public delegate void OnReceivedDmgDelegate(int damage, CFGCharacter attacker);

	public delegate void OnCharacterKilledDelegate(CFGCharacter character, CFGCharacter killer);

	public delegate void OnCharacterGunpointStateChangedDelegate(CFGCharacter character, EGunpointState new_state);

	public delegate void OnCharacterSuicide(CFGCharacter character);

	public delegate void OnCharacterActionFinishedDelegate(CFGCharacter character, ETurnAction action);

	public const float CHRACTER_HEIGHT = 1.73f;

	private const float MOVEMENT_CONST = 0.625f;

	[CFGFlowCode(Category = "Character", Title = "On Character Received Dmg")]
	public OnReceivedDmgDelegate m_OnReceivedDmgCallback;

	[CFGFlowCode(Category = "Character", Title = "On Character Killed")]
	public OnCharacterKilledDelegate m_OnCharacterKilledCallback;

	[CFGFlowCode(Category = "Character", Title = "On Character Gunpoint State Changed")]
	public OnCharacterGunpointStateChangedDelegate m_OnCharacterGunpointStateChangedCallback;

	[CFGFlowCode(Category = "Character", Title = "On Character Suicide")]
	public OnCharacterSuicide m_OnCharacterSuicide;

	private CFGCharacterData m_CharData;

	private bool m_bCharDataIsTemp;

	private bool m_IsSelected;

	private bool m_IsVisible = true;

	private bool m_IsVisible2 = true;

	private bool m_IsOnCorrectFloor = true;

	private bool m_WasCustomized;

	private bool m_HasOutline;

	private bool m_IsInvisible;

	private EOutlineType m_OutlineState;

	private bool m_ShadowSpottedReported;

	private EBestDetectionType m_VisibilityState = EBestDetectionType.StartVal;

	private bool m_bCheckForHideInShadows = true;

	private bool m_bCheckForEnemyShadows = true;

	private bool m_bInShadow;

	private EBestCoverRotation m_BestCoverRotation;

	private EDirection m_BestCoverDirection;

	private bool m_BestCoverRotationLocked;

	private int m_TempChanceToHit;

	private float m_NextShadowSpottedAllowedTime;

	private CFGCell m_CurrentCell;

	private CFGCell m_LastCell;

	private Dictionary<Renderer, Shader> m_RenderersShaderDic;

	private LinkedList<CFGCell> m_Path = new LinkedList<CFGCell>();

	private EBestDetectionType m_BestDetectionType;

	private List<CFGCharacter> m_VisibileEnemies = new List<CFGCharacter>();

	private List<CFGCharacter> m_ShadowSpottedEnemies = new List<CFGCharacter>();

	private List<CFGCharacter> m_SensedEnemies = new List<CFGCharacter>();

	private List<CFGRicochetObject> m_VisibleRicochetObjects = new List<CFGRicochetObject>();

	private List<CFGIAttackable> m_VisibleOtherTargets = new List<CFGIAttackable>();

	private CFGGun m_CurrentWeapon;

	private CFGGun m_MultiShotWeapon1;

	private CFGGun m_MultiShotWeapon2;

	private Transform m_RightHand;

	private Transform m_LeftHand;

	private float m_HiddenShadowTexelsRatio;

	private float m_DisintegrateStartTime = -1f;

	private float m_DisintegrateEndTime = -1f;

	private CFGCharacterAnimator m_CharacterAnimator;

	private bool m_FlagNeedFlash;

	private bool m_FlagNeedFlash2;

	private bool m_JinxedFlagNeedFlash;

	private bool m_IntimidateFlagNeedFlash;

	private bool m_FlagNeedFlashing;

	private bool m_HideFlag;

	private CFGSplash m_LuckSplash;

	private float m_LastLuckSplashTime;

	private int m_EavesdropRoll;

	[HideInInspector]
	public CFGCellObject m_CurrentCellObject;

	[HideInInspector]
	public CFGCellObject m_CurrentCellObjectInside;

	[SerializeField]
	private CFGOwner m_Owner;

	[SerializeField]
	private CFGSoundDef m_HitSoundDef;

	[SerializeField]
	private CFGSoundDef m_DeathSoundDef;

	[SerializeField]
	private CFGSoundDef m_GunpointTargetSoundDef;

	[SerializeField]
	private CFGSoundDef m_IntimidatedSoundDef;

	[SerializeField]
	public SteeringPlugin m_Steering = new SteeringPlugin();

	[HideInInspector]
	public CFGGrenade m_Grenade;

	private Transform m_SpottedFx;

	public Action OnShadowSpottedReport;

	private static int m_MaxEavesdropText = -2;

	private EAbilityAnimState m_AbilityAnim_State;

	private bool m_AbilityAnim_HadWeapon;

	private eAbilityAnimation m_AbilityAnim_Type;

	public OnCharacterActionFinishedDelegate m_OnCharacterActionFinishedCallback;

	private ETurnAction m_CurrentAction = ETurnAction.None;

	private EActionState m_CurrentActionState;

	private bool m_IsWaitingForCameraFocus;

	private bool m_IsWaitingForFacingTarget;

	private bool m_IsWaitingForWeaponEquipped;

	private CFGGameObject m_ActionTarget;

	private List<CFGRicochetObject> m_TempRicoList;

	private List<CFGIAttackable> m_AOETargets;

	private CFGCell m_MissShootCell;

	private float m_NextOrderTime = -1f;

	private bool m_bShadowCloaked;

	private int m_MultiShotCounter;

	private ETurnAction m_TargetForAction = ETurnAction.None;

	private float m_FocusCameraEnd;

	private int m_ReactionShootDmg;

	private CFGIAttackable m_ReactionShootTgt;

	public Renderer[] m_PartsAlwaysVisible;

	public CharPartsGroup[] m_PartsGroups;

	public CharPartColorGroup[] m_PartsColorGroups;

	public CharCustomPreset[] m_CustomPresets;

	private List<GameObject> pomList = new List<GameObject>();

	public int EavesdropRoll => m_EavesdropRoll;

	[CFGFlowCode]
	public bool IsImmuneToGunpoint
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsStateSet(ECharacterStateFlag.ImmuneToGunpoint);
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.SetState(ECharacterStateFlag.ImmuneToGunpoint, value);
			}
		}
	}

	[CFGFlowCode]
	public bool IsInvulnerable
	{
		get
		{
			return m_CharData != null && m_CharData.Invulnerable;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.Invulnerable = value;
			}
		}
	}

	[CFGFlowCode(HideSetter = true)]
	public int Suspicion
	{
		get
		{
			if (m_CharData == null)
			{
				return -1;
			}
			return m_CharData.SuspicionLevel;
		}
	}

	[CFGFlowCode]
	public bool Imprisoned
	{
		get
		{
			if (m_CharData != null)
			{
				return m_CharData.IsStateSet(ECharacterStateFlag.Imprisoned);
			}
			return false;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.SetState(ECharacterStateFlag.Imprisoned, value);
			}
			if (!IsAlive)
			{
				return;
			}
			if (value)
			{
				if (CurrentWeapon != null)
				{
					CurrentWeapon.RemoveVisualisation();
				}
				ActionPoints = 0;
				if (m_CharacterAnimator != null && !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
				{
					m_CharacterAnimator.PlayWeaponChange0(AbilityAnim_OnWeaponRemove);
				}
			}
			else
			{
				if (m_CharacterAnimator != null)
				{
					m_CharacterAnimator.ResetTrigger("ChangeWeapon0");
				}
				if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
				{
					ActionPoints = 1;
				}
				else
				{
					ActionPoints = MaxActionPoints;
				}
				EquipWeapon();
			}
		}
	}

	[CFGFlowCode]
	public int AiTeam
	{
		get
		{
			return (m_CharData != null) ? m_CharData.AITeam : (-1);
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.AITeam = value;
			}
		}
	}

	[CFGFlowCode(HideSetter = true)]
	public CFGOwner Owner
	{
		get
		{
			return m_Owner;
		}
		private set
		{
			m_Owner = value;
		}
	}

	[CFGFlowCode(HideSetter = true)]
	public int Hp
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.Hp;
		}
		private set
		{
			if (m_CharData != null)
			{
				m_CharData.Hp = value;
			}
		}
	}

	[CFGFlowCode(HideSetter = true)]
	public int MaxHp
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.BuffedMaxHP;
		}
	}

	[CFGFlowCode]
	public int Luck
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.Luck;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.SetLuck(value, bAllowSplash: true);
			}
		}
	}

	[CFGFlowCode(HideSetter = true)]
	public int MaxLuck
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.MaxLuck;
		}
	}

	[CFGFlowCode]
	public int ActionPoints
	{
		get
		{
			return (m_CharData != null) ? m_CharData.ActionPoints : 0;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.ActionPoints = value;
			}
		}
	}

	public bool IsInDemonForm => m_CharData != null && m_CharData.IsStateSet(ECharacterStateFlag.InDemonForm);

	public bool IsCriticalCharacter
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsStateSet(ECharacterStateFlag.IsCritical);
		}
	}

	public float DeathTime
	{
		get
		{
			if (m_CharData == null)
			{
				return 0f;
			}
			return m_CharData.DeathTime;
		}
	}

	public bool CanDisintegrateBody
	{
		get
		{
			if (IsAlive)
			{
				return false;
			}
			if (m_DisintegrateStartTime <= 0f)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsInjured
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsStateSet(ECharacterStateFlag.Injured);
		}
	}

	public bool HasFinalizedTurn
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsStateSet(ECharacterStateFlag.TurnFinishedAndLocked);
		}
	}

	public bool SensedByPlayer
	{
		get
		{
			return m_CharData != null && m_CharData.IsStateSet(ECharacterStateFlag.SensendByPlayer);
		}
		set
		{
			if (m_CharData != null && value != m_CharData.IsStateSet(ECharacterStateFlag.SensendByPlayer))
			{
				m_CharData.SetState(ECharacterStateFlag.SensendByPlayer, value);
				if (value)
				{
					m_BestDetectionType |= EBestDetectionType.Sensed;
				}
			}
		}
	}

	public EOutlineType OutlineState => m_OutlineState;

	public bool IsInvisible => m_IsInvisible;

	public CFGGun MultiShotWeapon1
	{
		get
		{
			return m_MultiShotWeapon1;
		}
		set
		{
			m_MultiShotWeapon1 = value;
		}
	}

	public CFGGun MultiShotWeapon2
	{
		get
		{
			return m_MultiShotWeapon2;
		}
		set
		{
			m_MultiShotWeapon2 = value;
		}
	}

	public EAIState AIState
	{
		get
		{
			if (m_CharData == null)
			{
				return EAIState.Passive;
			}
			return m_CharData.AIState;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.AIState = value;
			}
		}
	}

	public bool IsCorpseLooted
	{
		get
		{
			if (IsAlive || m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsStateSet(ECharacterStateFlag.CorpseLooted);
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.SetState(ECharacterStateFlag.CorpseLooted, value);
			}
		}
	}

	public bool FlagNeedFlash
	{
		get
		{
			return m_FlagNeedFlash;
		}
		set
		{
			m_FlagNeedFlash = value;
		}
	}

	public bool FlagNeedFlash2
	{
		get
		{
			return m_FlagNeedFlash2;
		}
		set
		{
			m_FlagNeedFlash2 = value;
		}
	}

	public bool JinxedFlagNeedFlash
	{
		get
		{
			return m_JinxedFlagNeedFlash;
		}
		set
		{
			m_JinxedFlagNeedFlash = value;
		}
	}

	public bool IntimidateFlagNeedFlash
	{
		get
		{
			return m_IntimidateFlagNeedFlash;
		}
		set
		{
			m_IntimidateFlagNeedFlash = value;
		}
	}

	public bool FlagNeedFlashing
	{
		get
		{
			return m_FlagNeedFlashing;
		}
		set
		{
			m_FlagNeedFlashing = value;
		}
	}

	public bool HideFlag
	{
		get
		{
			return m_HideFlag;
		}
		set
		{
			m_HideFlag = value;
		}
	}

	public override ESerializableType SerializableType => ESerializableType.Character;

	public float SelfShadowRatio => m_HiddenShadowTexelsRatio;

	public bool IsInShadow => m_bInShadow;

	public bool ShouldCheckForHideInShadows
	{
		get
		{
			if (CFGCellShadowMapLevel.MapType == eCellShadowMapType.NoShadowMap)
			{
				return false;
			}
			return m_bCheckForHideInShadows;
		}
	}

	public bool ShouldCheckForEnemyShadows
	{
		get
		{
			if (CFGCellShadowMapLevel.MapType == eCellShadowMapType.NoShadowMap)
			{
				return false;
			}
			if (CFGSingleton<CFGGame>.Instance.IsDarkness)
			{
				return false;
			}
			return m_bCheckForEnemyShadows;
		}
	}

	public CFGCharacterData CharacterData => m_CharData;

	public CFGCharacterAnimator CharacterAnimator => m_CharacterAnimator;

	public int HalfMovement
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return Mathf.FloorToInt((float)m_CharData.BuffedMovement * 0.625f);
		}
	}

	public int DashingMovement
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return Mathf.FloorToInt((float)m_CharData.BuffedMovement * 0.625f * 2f);
		}
	}

	public int BuffedAim
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.BuffedAim;
		}
	}

	public int BuffedDefense
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.BuffedDefense;
		}
	}

	public int BuffedSight
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.BuffedSight;
		}
	}

	public Dictionary<string, CFGBuff>.ValueCollection Buffs
	{
		get
		{
			if (m_CharData == null)
			{
				return null;
			}
			return m_CharData.Buffs.Values;
		}
	}

	public int ImageIDX
	{
		get
		{
			if (m_CharData == null)
			{
				return 0;
			}
			return m_CharData.ImageIDX;
		}
	}

	public CFGCell CurrentCell
	{
		get
		{
			return m_CurrentCell;
		}
		private set
		{
			m_CurrentCell = value;
			if (m_CurrentCell != m_LastCell)
			{
				OnCellChange();
			}
		}
	}

	public CFGCell LastCell => m_LastCell;

	public bool IsDead
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.IsDead;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.IsDead = value;
			}
		}
	}

	public bool IsAlive
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return !m_CharData.IsDead;
		}
		set
		{
			if (m_CharData != null)
			{
				m_CharData.IsDead = !value;
			}
		}
	}

	public int MaxActionPoints
	{
		get
		{
			if (m_CharData == null)
			{
				return 2;
			}
			return m_CharData.MaxAP;
		}
	}

	public CFGGun CurrentWeapon
	{
		get
		{
			return m_CurrentWeapon;
		}
		set
		{
			m_CurrentWeapon = value;
		}
	}

	public CFGGun UnusedWeapon => SecondWeapon;

	public CFGGun FirstWeapon => (m_CharData == null) ? null : m_CharData.Gun1;

	public CFGGun SecondWeapon => (m_CharData == null) ? null : m_CharData.Gun2;

	public Transform RightHand => m_RightHand;

	public Transform LeftHand => m_LeftHand;

	public EBestDetectionType BestDetectionType
	{
		get
		{
			return m_BestDetectionType;
		}
		set
		{
			m_BestDetectionType = value;
		}
	}

	public EBestDetectionType VisibilityState => m_VisibilityState;

	public bool IsSelected
	{
		get
		{
			return m_IsSelected;
		}
		private set
		{
			m_IsSelected = value;
		}
	}

	public List<CFGCharacter> VisibleEnemies => m_VisibileEnemies;

	public List<CFGCharacter> ShadowSpottedEnemies => m_ShadowSpottedEnemies;

	public List<CFGCharacter> SensedEnemies => m_SensedEnemies;

	public List<CFGRicochetObject> VisibleRicochetObjects => m_VisibleRicochetObjects;

	public List<CFGIAttackable> OtherTargets => m_VisibleOtherTargets;

	[CFGFlowCode(HideSetter = true)]
	public EGunpointState GunpointState
	{
		get
		{
			return (m_CharData != null) ? m_CharData.GunpointState : EGunpointState.None;
		}
		set
		{
			if (m_CharData != null && m_CharData.GunpointState != value)
			{
				if (m_Owner != null && m_Owner.IsAi && m_CharData.GunpointState == EGunpointState.Target && value == EGunpointState.None)
				{
					CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_releasedgunpoint", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
				}
				m_CharData.GunpointState = value;
				CFGCharacter cFGCharacter = m_ActionTarget as CFGCharacter;
				if (m_CharacterAnimator != null && (value != EGunpointState.Executor || (cFGCharacter != null && cFGCharacter.GunpointState != EGunpointState.Target)))
				{
					m_CharacterAnimator.ChangeGunpointState(m_CharData.GunpointState);
				}
				if (m_OnCharacterGunpointStateChangedCallback != null)
				{
					m_OnCharacterGunpointStateChangedCallback(this, m_CharData.GunpointState);
				}
			}
		}
	}

	public Dictionary<ETurnAction, CAbilityInfo> Abilities
	{
		get
		{
			if (m_CharData == null)
			{
				return null;
			}
			return m_CharData.Abilities;
		}
	}

	public Vector3 Position => base.Transform.position;

	public Quaternion Rotation => base.Transform.rotation;

	public bool IsDodging
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.HasBuff("dodge");
		}
	}

	public bool IsBestCoverRotationLocked
	{
		get
		{
			return m_BestCoverRotationLocked;
		}
		set
		{
			m_BestCoverRotationLocked = value;
		}
	}

	public ETurnAction TargetForAction
	{
		get
		{
			return m_TargetForAction;
		}
		set
		{
			m_TargetForAction = value;
		}
	}

	public bool IsShadowCloaked => m_bShadowCloaked;

	public bool CanDoReactionShot
	{
		get
		{
			if (m_CharData != null)
			{
				return m_CharData.IsStateSet(ECharacterStateFlag.CanDoReactionShoot);
			}
			return false;
		}
		set
		{
			if (m_CharData != null && (!value || !CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoReactionShot)))
			{
				m_CharData.SetState(ECharacterStateFlag.CanDoReactionShoot, value);
			}
		}
	}

	public bool HasCrippledMovement
	{
		get
		{
			if (m_CharData == null)
			{
				return false;
			}
			return m_CharData.HasBuff("crippledmovement");
		}
	}

	public ETurnAction CurrentAction
	{
		get
		{
			return m_CurrentAction;
		}
		private set
		{
			m_CurrentAction = value;
		}
	}

	public EBestCoverRotation GetBestCoverRotation()
	{
		return m_BestCoverRotation;
	}

	public EDirection GetBestCoverDirection()
	{
		return m_BestCoverDirection;
	}

	public void HACKPlayGunpointAnimation()
	{
		if (m_CharData.GunpointState != 0 && m_CharacterAnimator != null)
		{
			m_CharacterAnimator.ChangeGunpointState(m_CharData.GunpointState);
		}
	}

	public static ECoverType GetTargetCover(CFGCell start_tile, CFGCell end_tile)
	{
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			return ECoverType.NONE;
		}
		return CFGCellMap.GetTargetCover(start_tile, end_tile);
	}

	public Transform FindChildRecursively(Transform parent, string name)
	{
		if (parent == null)
		{
			return null;
		}
		Transform transform = parent.Find(name);
		if (transform != null)
		{
			return transform;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			transform = FindChildRecursively(parent.GetChild(i), name);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	public bool HasAltAttack(ETurnAction AltAttack)
	{
		return AltAttack switch
		{
			ETurnAction.AltFire_Fanning => (bool)CurrentWeapon && CurrentWeapon.m_Definition != null && CurrentWeapon.m_Definition.AllowsFanning, 
			ETurnAction.AltFire_ConeShot => (bool)CurrentWeapon && CurrentWeapon.m_Definition != null && CurrentWeapon.m_Definition.AllowsCone, 
			ETurnAction.AltFire_ScopedShot => (bool)CurrentWeapon && CurrentWeapon.m_Definition != null && CurrentWeapon.m_Definition.AllowsScoped, 
			_ => false, 
		};
	}

	public bool HaveAbility(ETurnAction ability)
	{
		if (m_CharData == null || m_CharData.Abilities == null)
		{
			return false;
		}
		if (m_CharData.Abilities.ContainsKey(ability))
		{
			return true;
		}
		return false;
	}

	public bool ShouldFinishMove()
	{
		if (!IsAlive)
		{
			return true;
		}
		return m_Steering.ReachedDestination();
	}

	public bool IsVisibleByPlayer()
	{
		if (Owner == null || Owner.IsPlayer)
		{
			return true;
		}
		if ((m_BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible)
		{
			return true;
		}
		if ((m_BestDetectionType & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted)
		{
			return true;
		}
		return false;
	}

	public bool IsVisibleByEnemy()
	{
		if (Owner == null)
		{
			return true;
		}
		CFGOwner cFGOwner = ((!Owner.IsPlayer) ? ((CFGOwner)CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner) : ((CFGOwner)CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner));
		if (cFGOwner == null)
		{
			return false;
		}
		foreach (CFGCharacter character in cFGOwner.Characters)
		{
			if (character.IsEnemyVisible(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsEnemyVisible(CFGCharacter ch)
	{
		return (bool)ch && VisibleEnemies.Contains(ch);
	}

	public bool IsOtherTargetVisible(CFGIAttackable _Target)
	{
		return _Target != null && OtherTargets.Contains(_Target);
	}

	public ECoverType GetCoverState()
	{
		if (CurrentCell == null)
		{
			return ECoverType.NONE;
		}
		ECoverType result = ECoverType.NONE;
		switch (CurrentCell.GetBorderCover(EDirection.EAST))
		{
		case ECoverType.FULL:
			return ECoverType.FULL;
		case ECoverType.HALF:
			result = ECoverType.HALF;
			break;
		}
		switch (CurrentCell.GetBorderCover(EDirection.WEST))
		{
		case ECoverType.FULL:
			return ECoverType.FULL;
		case ECoverType.HALF:
			result = ECoverType.HALF;
			break;
		}
		switch (CurrentCell.GetBorderCover(EDirection.NORTH))
		{
		case ECoverType.FULL:
			return ECoverType.FULL;
		case ECoverType.HALF:
			result = ECoverType.HALF;
			break;
		}
		switch (CurrentCell.GetBorderCover(EDirection.SOUTH))
		{
		case ECoverType.FULL:
			return ECoverType.FULL;
		case ECoverType.HALF:
			result = ECoverType.HALF;
			break;
		}
		return result;
	}

	public void SetOwner(CFGOwner owner)
	{
		if ((bool)Owner)
		{
			Owner.RemoveCharacter(this);
		}
		Owner = owner;
		if ((bool)Owner)
		{
			Owner.AddCharacter(this);
		}
		if (m_CharData != null)
		{
			m_CharData.OnNewOwner();
		}
	}

	public void ChangeOwner(CFGOwner NewOwner)
	{
		if ((bool)Owner)
		{
			Owner.RemoveCharacter(this);
		}
		Owner = NewOwner;
		if ((bool)Owner)
		{
			Owner.AddCharacter(this);
			if (Owner.IsAi)
			{
				CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
				if ((bool)aiOwner && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
				{
					aiOwner.OnCharacterActionFinished(this, ETurnAction.None);
				}
			}
		}
		if (m_CharData != null)
		{
			if (m_CharData.PositionInTeam >= 0)
			{
				CFGCharacterList.RemoveFromTeam(m_CharData.PositionInTeam, bRemoveEquipment: true);
			}
			if ((bool)Owner && Owner.IsPlayer)
			{
				m_CharData.SetState(ECharacterStateFlag.TempTactical, Value: false);
				Debug.Log("Assign to team: " + CFGCharacterList.AssignToTeam(m_CharData));
			}
		}
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component != null && component.SelectedCharacter == this)
		{
			CFGCharacter cFGCharacter = component.GetNextActiveCharacter();
			if (cFGCharacter == this)
			{
				cFGCharacter = null;
			}
			component.SelectCharacter(cFGCharacter, focus: false);
		}
	}

	public bool HasLineOfFireTo(CFGIAttackable target, ETurnAction Action)
	{
		if (Action == ETurnAction.Penetrate)
		{
			return true;
		}
		if (m_CharData == null || target == null)
		{
			return false;
		}
		if (CFGCellMap.GetLineOf(CurrentCell, target.CurrentCell, m_CharData.BuffedSight, 32, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) != 0)
		{
			return false;
		}
		return true;
	}

	private void GenerateBestHitPositions(CFGIAttackable Target, CFGRicochetObject ROTarget, out CFGCell BestStart, out CFGCell BestEnd)
	{
		CFGCell[] tiles = new CFGCell[3] { CurrentCell, null, null };
		CFGCell[] tiles2 = new CFGCell[3];
		BestStart = null;
		BestEnd = null;
		CFGCharacter cFGCharacter = Target as CFGCharacter;
		Vector3 normalized = (Target.Position - Position).normalized;
		if (Target == null)
		{
			if (!(ROTarget != null))
			{
				return;
			}
			tiles2[0] = ROTarget.Cell;
		}
		else
		{
			tiles2[0] = Target.CurrentCell;
		}
		CFGCellMap.FillUpTable(CurrentCell, normalized, ref tiles);
		if (cFGCharacter != null && ROTarget == null)
		{
			CFGCellMap.FillUpTable(tiles2[0], -normalized, ref tiles2);
		}
	}

	public int GetBaseChanceToHit(CFGIAttackable target, List<CFGRicochetObject> TempTargets, CFGCell StartCell, CFGCell EndCell, ETurnAction Action)
	{
		if (!CFGCellMap.IsValid || target == null || CurrentWeapon == null)
		{
			return 0;
		}
		CFGCell cFGCell = EndCell;
		if (cFGCell == null)
		{
			cFGCell = target.CurrentCell;
			if (cFGCell == null)
			{
				return 0;
			}
		}
		CFGCell cFGCell2 = StartCell;
		if (cFGCell2 == null)
		{
			if (TempTargets != null && TempTargets.Count > 0)
			{
				cFGCell2 = TempTargets[TempTargets.Count - 1].Cell;
			}
			if (cFGCell2 == null)
			{
				cFGCell2 = CurrentCell;
			}
			if (cFGCell2 == null)
			{
				return 0;
			}
		}
		if (TempTargets == null)
		{
			if (!HasLineOfFireTo(target, Action))
			{
				return 0;
			}
			if (m_Owner != null && m_Owner.IsAi && CFGCellMap.GetLineOfSightAutoSideSteps(this, target, cFGCell2, cFGCell, 1000) != 0)
			{
				return 0;
			}
		}
		int num = 0;
		CFGCell start = CurrentCell;
		if (TempTargets != null)
		{
			for (int i = 0; i < TempTargets.Count; i++)
			{
				if (TempTargets[i] != null && TempTargets[i].Cell != null)
				{
					num += CFGCellMap.Distance(start, TempTargets[i].Cell);
					start = TempTargets[i].Cell;
				}
			}
		}
		num += CFGCellMap.Distance(start, cFGCell);
		if (num <= CFGGameplaySettings.s_PointBlankDistance)
		{
			return 100;
		}
		int buffedAim = m_CharData.BuffedAim;
		int buffedDefense = target.BuffedDefense;
		int hitChance = CurrentWeapon.HitChance;
		int distMod = CurrentWeapon.Class.GetDistMod(num);
		int num2 = GetTargetCover(cFGCell2, cFGCell).GetAimMod() * target.GetCoverMult();
		int floorAimMod = ((EFloorLevelType)CurrentCell.Floor).GetFloorAimMod((EFloorLevelType)cFGCell.Floor);
		int num3 = 0;
		if (TempTargets == null && CFGCellMap.GetLineOfSightAutoSideSteps(this, target, cFGCell2, cFGCell, 1000) != 0)
		{
			num3 = CFGGameplaySettings.s_BlindShotMod;
		}
		int aimBonus = target.GetAimBonus();
		return buffedAim - buffedDefense + hitChance + distMod + num2 + floorAimMod + num3 + aimBonus;
	}

	public static int GetTurnActionChanceToHitMod(ETurnAction Action)
	{
		return Action switch
		{
			ETurnAction.AltFire_ScopedShot => 100, 
			ETurnAction.AltFire_Fanning => -20, 
			ETurnAction.AltFire_ConeShot => -20, 
			ETurnAction.Penetrate => 100, 
			_ => 0, 
		};
	}

	public int GetFinalChanceToHit(CFGIAttackable target, List<CFGRicochetObject> TempTargets, CFGCell StartCell, CFGCell EndCell, ETurnAction Action)
	{
		int num = GetChanceToHit(target, TempTargets, StartCell, EndCell, Action);
		if (num < 100 && target != null && target.IsDodging)
		{
			num = 0;
		}
		return num;
	}

	public int GetChanceToHit(CFGIAttackable target, List<CFGRicochetObject> TempTargets, CFGCell StartCell, CFGCell EndCell, ETurnAction Action)
	{
		int baseChanceToHit = GetBaseChanceToHit(target, TempTargets, StartCell, EndCell, Action);
		int turnActionChanceToHitMod = GetTurnActionChanceToHitMod(Action);
		baseChanceToHit += turnActionChanceToHitMod;
		if (baseChanceToHit < CFGGameplaySettings.s_CtH_BottomTreshold)
		{
			baseChanceToHit = 0;
		}
		if (baseChanceToHit > CFGGameplaySettings.s_CtH_TopTreshold)
		{
			baseChanceToHit = 100;
		}
		return baseChanceToHit;
	}

	public static int GetActionDamageMod(ETurnAction Action)
	{
		if (Action == ETurnAction.AltFire_ConeShot)
		{
			return -1;
		}
		return 0;
	}

	public int CalcDamage(CFGIAttackable target, ETurnAction Action, CFGCell RicochetSource = null, bool bIgnoreCovers = false, CFGCell EndCell = null)
	{
		if (target == null || CurrentWeapon == null)
		{
			return 0;
		}
		int num = 0;
		num = ((Action != ETurnAction.MultiShot) ? CurrentWeapon.Damage : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotDamage);
		if (m_CharData != null)
		{
			num += m_CharData.BuffedDamage;
		}
		num += GetActionDamageMod(Action);
		if (Action == ETurnAction.Penetrate)
		{
			bIgnoreCovers = true;
		}
		if (CFGGame.Difficulty != EDifficulty.Normal && m_CharData != null && m_CharData.Definition != null)
		{
			num = ((CFGGame.Difficulty != EDifficulty.Hard) ? (num + m_CharData.Definition.DmgModEasy) : (num + m_CharData.Definition.DmgModHard));
		}
		bool flag = !bIgnoreCovers;
		if (flag && CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.NoCoverReduction))
		{
			flag = false;
		}
		if (flag)
		{
			switch (GetTargetCover((RicochetSource != null) ? RicochetSource : CurrentCell, (EndCell != null) ? EndCell : target.CurrentCell))
			{
			case ECoverType.FULL:
				num /= CurrentWeapon.FullCoverDiv;
				break;
			case ECoverType.HALF:
				num /= CurrentWeapon.HalfCoverDiv;
				break;
			}
		}
		if (num < 1)
		{
			num = 1;
		}
		return num;
	}

	public int CalcDamageSecond(CFGIAttackable target, ETurnAction Action, CFGCell RicochetSource = null, bool bIgnoreCovers = false)
	{
		if (target == null || SecondWeapon == null)
		{
			return 0;
		}
		int num = SecondWeapon.Damage;
		if (m_CharData != null)
		{
			num += m_CharData.BuffedDamage;
		}
		num += GetActionDamageMod(Action);
		if (CFGGame.Difficulty != EDifficulty.Normal && m_CharData != null && m_CharData.Definition != null)
		{
			num = ((CFGGame.Difficulty != EDifficulty.Hard) ? (num + m_CharData.Definition.DmgModEasy) : (num + m_CharData.Definition.DmgModHard));
		}
		if (!bIgnoreCovers)
		{
			switch (GetTargetCover((RicochetSource != null) ? RicochetSource : CurrentCell, target.CurrentCell))
			{
			case ECoverType.FULL:
				num /= SecondWeapon.FullCoverDiv;
				break;
			case ECoverType.HALF:
				num /= SecondWeapon.HalfCoverDiv;
				break;
			}
		}
		if (num < 1)
		{
			num = 1;
		}
		return num;
	}

	public bool IsCellInMoveRange(CFGCell cell)
	{
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(CurrentCell, (ActionPoints != 2) ? HalfMovement : DashingMovement);
		return hashSet.Contains(cell);
	}

	public void AssignToCharacterData(CFGCharacterData chardata, bool IsReInint)
	{
		if (m_CharData != null && m_bCharDataIsTemp)
		{
			CFGCharacterList.UnRegisterCharacter(m_CharData);
		}
		m_CharData = chardata;
		if (chardata == null)
		{
			return;
		}
		if (!IsReInint)
		{
			chardata.AssignModel(this);
		}
		EquipWeapon();
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in m_CharData.Abilities)
		{
			if (ability.Value.Ability != null)
			{
				ability.Value.Ability.Init(this);
			}
		}
	}

	public void EquipWeapon(bool bCheckInGame = true)
	{
		if (m_CharData == null || (bCheckInGame && !CFGSingleton<CFGGame>.Instance.IsInGame()))
		{
			return;
		}
		if ((bool)CurrentWeapon)
		{
			CurrentWeapon.RemoveVisualisation();
			CurrentWeapon = null;
		}
		CurrentWeapon = FirstWeapon;
		if (CurrentWeapon == null)
		{
			return;
		}
		bool flag = ShouldEquipWeapon();
		if (flag)
		{
			CurrentWeapon.SpawnVisualisation(m_RightHand);
		}
		m_CharData.SetWeaponBuff(CurrentWeapon);
		if (m_CharacterAnimator != null && flag)
		{
			if (CurrentWeapon.TwoHanded)
			{
				m_CharacterAnimator.PlayWeaponChange2();
			}
			else
			{
				m_CharacterAnimator.PlayWeaponChange1();
			}
		}
	}

	public void ProcessFacingToCover(bool instant_change = false)
	{
		if (!(m_CharacterAnimator != null) || CurrentWeapon == null || (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && !CurrentAction.CanCharacterBeGluedinSetupStage()))
		{
			return;
		}
		CurrentCell.GetBestCoverToGlue(this, out var cover_type, out var dir);
		if (cover_type != 0)
		{
			if (instant_change)
			{
				base.Transform.forward = dir.GetForward();
				if (!m_CharacterAnimator.ForceNoCoverTemporarily && (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || CurrentAction.CanCharacterBeGluedinSetupStage()) && CurrentAction.CanCharacterBeGlued() && !Imprisoned && CurrentCell != null && GunpointState == EGunpointState.None)
				{
					m_CharacterAnimator.CurrentCoverState = cover_type;
				}
			}
			m_CharacterAnimator.WantedDirection = dir.GetForward();
		}
		else
		{
			m_CharacterAnimator.WantedDirection = base.Transform.forward;
		}
	}

	public void Select()
	{
		if (!IsSelected)
		{
			IsSelected = true;
		}
	}

	public void UnSelect()
	{
		if (IsSelected)
		{
			IsSelected = false;
		}
	}

	private void OnTargetKilled(CFGCharacter Target, bool bSilent)
	{
		if (Target == null || m_CharData == null)
		{
			return;
		}
		if (m_CharData.HasBuff("halfdead") && Target == m_CharData.Nemesis)
		{
			m_CharData.RemBuff("halfdead");
			m_CharData.Nemesis = null;
			if (!bSilent)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_vengeance_nemesiskilled") + "</color>", plus: false, 1, -1, this);
			}
		}
		if (!m_CharData.HasBuff("rewardedkill"))
		{
			return;
		}
		int amount = 0;
		int num = 0;
		int actionPoints = ActionPoints;
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		if (instance.m_AbilityData != null)
		{
			amount = Mathf.Clamp(instance.m_AbilityData.m_RewardedKill_HP, 0, 100);
			num = Mathf.Clamp(instance.m_AbilityData.m_RewardedKill_AP, 0, 2);
			if (instance.m_AbilityData.m_RewardedKill_FX != null)
			{
				Transform transform = base.Transform.Find("Chest");
				if (transform != null)
				{
					Transform transform2 = UnityEngine.Object.Instantiate(instance.m_AbilityData.m_RewardedKill_FX, transform.position, transform.rotation) as Transform;
					transform2.parent = transform;
				}
			}
			Transform transform3 = ((CurrentWeapon == null || !CurrentWeapon.TwoHanded) ? instance.m_AbilityData.m_RewardedKill_FX_w1 : instance.m_AbilityData.m_RewardedKill_FX_w2);
			if (transform3 != null)
			{
				Transform transform4 = base.Transform.Find("Dummy_weapon_hand");
				if (transform4 != null)
				{
					Transform transform5 = UnityEngine.Object.Instantiate(transform3, transform4.position, transform4.rotation) as Transform;
					transform5.parent = transform4;
				}
			}
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_RewardedKill, base.Transform);
		}
		Heal(amount, bSilent: false);
		ActionPoints += num;
		if (ActionPoints >= MaxActionPoints)
		{
			ActionPoints = MaxActionPoints;
		}
		int num2 = ActionPoints - actionPoints;
		if (num2 != 0)
		{
			Vector3 position2 = base.transform.position;
			position2.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + string.Format("+{0} {1}", num2, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_ap")) + "</color>", plus: false, 1, -1, this);
		}
	}

	public bool TakeDamage(int dmg, CFGCharacter damage_giver, bool bSilent, [Optional] Vector3 recoilDir)
	{
		if (dmg < 1)
		{
			return false;
		}
		bool isDead = IsDead;
		if ((bool)Owner && damage_giver != null)
		{
			Owner.OnCharacterDamaged(damage_giver, this);
		}
		if (m_CharData != null)
		{
			if (!m_CharData.Invulnerable && !CFGCheats.GlobalInvulnerability)
			{
				if (dmg >= Hp && !m_CharData.HasBuff("halfdead") && HaveAbility(ETurnAction.Vengeance) && damage_giver != null && damage_giver.CharacterData != null)
				{
					Hp = 1;
					m_CharData.AddBuff("halfdead", EBuffSource.Ability);
					damage_giver.CharacterData.AddBuff("nemesis", EBuffSource.Ability);
					m_CharData.Nemesis = damage_giver;
				}
				else if (Hp > 0)
				{
					Hp -= dmg;
					if (Hp < 1 && (damage_giver == null || damage_giver.CurrentAction != ETurnAction.MultiShot))
					{
						if (damage_giver != null && damage_giver != this)
						{
							damage_giver.OnTargetKilled(this, bSilent);
						}
						if (m_OnCharacterKilledCallback != null)
						{
							m_OnCharacterKilledCallback(this, damage_giver);
							m_OnCharacterKilledCallback = null;
						}
						if (recoilDir == Vector3.zero && damage_giver != null)
						{
							recoilDir = (base.transform.position - damage_giver.Transform.position).normalized;
						}
						if (bSilent)
						{
							SilentDie(recoilDir);
						}
						else
						{
							Die(recoilDir);
						}
					}
				}
			}
			if (!bSilent && !isDead)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, dmg.ToString(), plus: false, 0, 0, this);
				if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUD && Owner != null && Owner.IsPlayer)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashHP();
				}
			}
			if (damage_giver == null || damage_giver.CharacterData == null || !damage_giver.HaveAbility(ETurnAction.Jinx))
			{
				m_CharData.SetLuck(m_CharData.Luck + m_CharData.BuffedLuckReplenishPerHit, Hp > 0);
			}
		}
		if (m_OnReceivedDmgCallback != null)
		{
			m_OnReceivedDmgCallback(dmg, damage_giver);
		}
		if (Hp > 0 || (damage_giver != null && damage_giver.CurrentAction == ETurnAction.MultiShot))
		{
			if (damage_giver != null && damage_giver.CurrentAction == ETurnAction.MultiShot && Hp <= 0)
			{
				HideFlag = true;
			}
			if (!bSilent)
			{
				if (m_CharacterAnimator != null && (m_Steering == null || !m_Steering.m_SteerData.m_IsMoving))
				{
					m_CharacterAnimator.PlayHit();
				}
				CFGSoundDef.Play(m_HitSoundDef, base.Transform);
			}
			if (m_CharData != null && m_CharData.Definition != null && m_CharData.PositionInTeam >= 0 && !IsInjured && damage_giver != null && damage_giver.Owner != null && damage_giver.Owner.IsAi)
			{
				int num = m_CharData.Definition.InjuryResistance;
				if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.MoreWounds))
				{
					num = 2;
				}
				if (dmg >= num)
				{
					m_CharData.AddInjury(bCombatInjury: true);
					if (!bSilent)
					{
						CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CharacterCriticalHit, base.Transform);
					}
				}
			}
			else if (!bSilent)
			{
				CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CharacterHit, base.Transform);
			}
			return false;
		}
		if (!bSilent)
		{
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_CharacterHit, base.Transform);
		}
		if (damage_giver != null && CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.KillsReplenishLuck))
		{
			damage_giver.Luck += CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_mode_kill_replenish_luck_amount;
		}
		return true;
	}

	public void Heal(int Amount, bool bSilent)
	{
		if (IsDead)
		{
			return;
		}
		if (!bSilent)
		{
			Vector3 position = base.Transform.position;
			position.y += 1.9f;
			if (Hp < MaxHp)
			{
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, Amount.ToString(), plus: true, 0, 0, this);
				if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUD && Owner != null && Owner.IsPlayer)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashHP();
				}
				if (Hp + Amount >= MaxHp)
				{
					m_FlagNeedFlash = true;
				}
			}
		}
		Hp += Amount;
	}

	public void Die([Optional] Vector3 recoilDir)
	{
		if (m_CurrentAction != ETurnAction.None)
		{
			m_ActionTarget = null;
			FinishCurrentAction();
		}
		IsDead = true;
		TakeOutline(m_OutlineState);
		if (IsCriticalCharacter)
		{
			CFGSingleton<CFGGame>.Instance.MissionFail("tactical_critical_character_death_fail", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
		}
		if ((bool)Owner)
		{
			Owner.OnCharacterKilled(this);
		}
		if ((bool)CurrentCell)
		{
			CurrentCell.CharacterLeave(this, null);
		}
		int positionInTeam = CFGCharacterList.GetPositionInTeam(base.NameId);
		if (positionInTeam >= 0)
		{
			CFGCharacterList.RemoveFromTeam(positionInTeam, bRemoveEquipment: true);
		}
		CFGSelectionManager.Instance.OnCharacterDied(this);
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_charkilled", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
		if (m_Steering != null)
		{
			m_Steering.m_SteerData.m_IsMoving = false;
		}
		m_Path.Clear();
		DoDeathAnimationStuff(recoilDir);
		CFGSoundDef.Play(m_DeathSoundDef, base.Transform.position);
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.shadowCastingMode = ShadowCastingMode.On;
		}
		m_VisibilityState |= EBestDetectionType.Visible;
		if (m_SpottedFx != null)
		{
			m_SpottedFx.parent = null;
			UnityEngine.Object.Destroy(m_SpottedFx.gameObject);
		}
		if (m_CharData != null)
		{
			m_CharData.DeathTime = CFGTimer.MissionTime;
		}
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo != null)
		{
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo.m_RemoveNearbyBodies)
			{
				CFGSingletonResourcePrefab<CFGObjectManager>.Instance.DisintegrateNearBodies(this);
			}
			if (!CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo.m_ActivateOnEndTurn)
			{
				CFGSingletonResourcePrefab<CFGObjectManager>.Instance.UpdateBodyCount();
			}
		}
		else
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.DisintegrateNearBodies(this);
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.UpdateBodyCount();
		}
	}

	public void DelayedDeath(CFGCharacter damage_giver, [Optional] Vector3 recoilDir)
	{
		if (damage_giver != null && damage_giver != this)
		{
			damage_giver.OnTargetKilled(this, bSilent: false);
		}
		if (m_OnCharacterKilledCallback != null)
		{
			m_OnCharacterKilledCallback(this, damage_giver);
			m_OnCharacterKilledCallback = null;
		}
		Die(recoilDir);
	}

	public void SilentDie([Optional] Vector3 recoilDir)
	{
		if (m_CurrentAction != ETurnAction.None)
		{
			m_ActionTarget = null;
			FinishCurrentAction();
		}
		if ((bool)CurrentCell)
		{
			CurrentCell.CharacterLeave(this, null);
		}
		if ((bool)Owner)
		{
			Owner.OnCharacterKilled(this, bSilent: true);
		}
		if (IsCriticalCharacter)
		{
			CFGSingleton<CFGGame>.Instance.MissionFail("tactical_critical_character_death_fail", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
		}
		int positionInTeam = CFGCharacterList.GetPositionInTeam(base.NameId);
		if (positionInTeam >= 0)
		{
			CFGCharacterList.RemoveFromTeam(positionInTeam, bRemoveEquipment: true);
		}
		DoDeathAnimationStuff(recoilDir);
		if ((bool)m_CurrentCell && m_CurrentCell.CurrentCharacter == this)
		{
			m_CurrentCell.CurrentCharacter = null;
		}
		IsDead = true;
		TakeOutline(m_OutlineState);
	}

	public void Eradicate()
	{
		if ((bool)CurrentCell)
		{
			CurrentCell.CharacterLeave(this, null);
		}
		if (m_CharData != null)
		{
			m_CharData.CurrentModel = null;
			m_CharData.Hp = 0;
			m_CharData.IsDead = true;
		}
		if ((bool)Owner)
		{
			Owner.OnCharacterKilled(this, bSilent: true);
		}
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters.Remove(this);
		CFGSingletonResourcePrefab<CFGObjectManager>.Instance.GameObjects.Remove(this);
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component != null)
		{
			component.OnCharacterDied(this);
		}
		int positionInTeam = CFGCharacterList.GetPositionInTeam(base.NameId);
		if (positionInTeam >= 0)
		{
			CFGCharacterList.RemoveFromTeam(positionInTeam, bRemoveEquipment: true);
		}
		DoDeathAnimationStuff(Vector3.zero);
		IsDead = true;
		TakeOutline(m_OutlineState);
		UnityEngine.Object.Destroy(base.gameObject);
		m_CharData = null;
	}

	public int CompareVisibleEnemies(CFGCharacter char_a, CFGCharacter char_b)
	{
		int tempChanceToHit = char_a.m_TempChanceToHit;
		int tempChanceToHit2 = char_b.m_TempChanceToHit;
		if (tempChanceToHit == tempChanceToHit2)
		{
			return 0;
		}
		if (tempChanceToHit > tempChanceToHit2)
		{
			return -1;
		}
		return 1;
	}

	public void SortVisibleEnemiesList(ref List<CFGCharacter> list)
	{
		foreach (CFGCharacter item in list)
		{
			item.m_TempChanceToHit = GetChanceToHit(item, null, null, null, ETurnAction.Shoot);
		}
		list.Sort(CompareVisibleEnemies);
	}

	public void Translate(Transform Dest)
	{
		if (!(Dest == null))
		{
			Translate(Dest.position, Dest.rotation);
		}
	}

	public void Translate(Vector3 Pos, Quaternion Rot)
	{
		Vector3 vector = new Vector3(Mathf.Floor(Pos.x) + 0.5f, Pos.y, Mathf.Floor(Pos.z) + 0.5f);
		CFGCell cFGCell = CFGCellMap.GetCharacterCell(vector);
		if (cFGCell == null)
		{
			Debug.LogWarning("Translate:: invalid position: " + Pos);
		}
		else
		{
			if (cFGCell == CurrentCell)
			{
				return;
			}
			if (!cFGCell.CanStandOnThisTile(can_stand_now: true))
			{
				cFGCell.DecodePosition(out var PosX, out var PosZ, out var Floor);
				Vector3 worldPosition = cFGCell.WorldPosition;
				cFGCell = null;
				float num = 100000f;
				for (int i = -5; i < 5; i++)
				{
					for (int j = -5; j < 5; j++)
					{
						CFGCell cell = CFGCellMap.GetCell(PosZ + j, i + PosX, Floor);
						if (cell != null && cell.CanStandOnThisTile(can_stand_now: true) && cell != CurrentCell)
						{
							float num2 = Vector3.Distance(cell.WorldPosition, worldPosition);
							if (num2 < num)
							{
								num = num2;
								cFGCell = cell;
							}
						}
					}
				}
				if (cFGCell == null)
				{
					Debug.LogWarning("Failed to find available target for translate");
					return;
				}
			}
			if ((bool)CurrentCell)
			{
				CurrentCell.CharacterLeave(this, cFGCell);
			}
			base.Transform.position = vector;
			base.Transform.rotation = Rot;
			CFGCell currentCell = CurrentCell;
			CurrentCell = cFGCell;
			if ((bool)CurrentCell)
			{
				CurrentCell.CharacterEnter(this, currentCell);
				CurrentCell.CharacterMoveEnd(this);
			}
			ProcessFacingToCover(instant_change: true);
			CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
			if (component != null && component.SelectedCharacter == this)
			{
				component.OnCharacterActionFinished(this);
				component.OnCurrentCharacterTranslate();
			}
			UpdateShadowCloaks();
			ApplyJinx();
			UpdateIntimidate();
			CFGObjectManager.ProcessFacingToCoverAllCharacters();
		}
	}

	public void RecalculateSelfShadow()
	{
		m_bInShadow = false;
		if (CFGSingleton<CFGGame>.Instance.IsDarkness)
		{
			m_bInShadow = true;
			return;
		}
		m_HiddenShadowTexelsRatio = 0f;
		if (CFGCellShadowMapLevel.MapType != 0)
		{
			m_HiddenShadowTexelsRatio = Mathf.Clamp01(1f - CFGCellShadowMapLevel.IsInShadow(base.transform.position));
			if (m_HiddenShadowTexelsRatio >= CFGLevelSettings.HideInShadowRatio)
			{
				m_bInShadow = true;
			}
		}
	}

	public void OnStartBuff(CFGDef_Buff Buff)
	{
		if (Buff != null)
		{
			if (string.Compare(Buff.BuffID, "demonpower", ignoreCase: true) == 0)
			{
			}
			if (Buff.BuffID == "crippleddefense" || Buff.BuffID == "crippledsight" || Buff.BuffID == "crippledmovement" || Buff.BuffID == "crippledaim")
			{
				AddCripplerFX();
			}
			Vector3 position = base.Transform.position;
			position.y += 3.9f;
			string empty = string.Empty;
			empty = ((Buff.Color_Font == "positive") ? "<color=#96be46>" : ((!(Buff.Color_Font == "negative")) ? "<color=#fadc5a>" : "<color=#ff4646>"));
			string empty2 = string.Empty;
			empty2 = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(Buff.BuffID);
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, empty + empty2 + "</color>", plus: false, 1, Buff.Icon, this);
			if (Buff.BuffID == "intimidated")
			{
				CFGSoundDef.Play(m_IntimidatedSoundDef, base.Transform);
			}
			else
			{
				CFGSoundDef.Play(Buff.m_GainSoundDef, base.Transform);
			}
		}
	}

	public void SwitchToDemon()
	{
		if (m_CharData != null)
		{
			m_CharData.SetState(ECharacterStateFlag.InDemonForm, Value: true);
			int aPCostForAction = GetAPCostForAction(ETurnAction.Demon, null);
			SpendActionPoints(aPCostForAction);
			if (base.gameObject.GetComponent<CFGDemon>() == null)
			{
				base.gameObject.AddComponent<CFGDemon>();
			}
			MakeAction(ETurnAction.FocusCamera, 3f);
		}
	}

	public void SwitchToHuman()
	{
		if (m_CharData != null)
		{
			m_CharData.SetState(ECharacterStateFlag.InDemonForm, Value: false);
			CFGDemon component = base.gameObject.GetComponent<CFGDemon>();
			if (!(component == null))
			{
				component.Disable();
			}
		}
	}

	public void OnEndBuff(CFGDef_Buff Buff, bool bRemoved)
	{
		if (Buff == null)
		{
			return;
		}
		CFGSoundDef.Play(Buff.m_LoseSoundDef, base.Transform);
		if (!bRemoved && string.Compare(Buff.BuffID, "halfdead", ignoreCase: true) == 0)
		{
			if (m_CharData != null && m_CharData.Nemesis != null && m_CharData.Nemesis.CharacterData != null)
			{
				m_CharData.Nemesis.CharacterData.RemBuff("nemesis");
			}
			TakeDamage(MaxHp * 2, null, bSilent: false);
			Vector3 position = base.Transform.position;
			position.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_vengeance_nemesisfailed") + "</color>", plus: false, 1, -1, this);
		}
		else
		{
			if (string.Compare(Buff.BuffID, "demonpower", ignoreCase: true) == 0)
			{
				SwitchToHuman();
				m_FlagNeedFlash = true;
			}
			if (string.Compare(Buff.BuffID, "revealed", ignoreCase: true) == 0)
			{
				m_FlagNeedFlash = true;
			}
		}
	}

	public override void StartTurn(CFGOwner owner)
	{
		m_ShadowSpottedReported = false;
		if (Owner != null && Owner.IsAi && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn && (VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted && (VisibilityState & EBestDetectionType.Visible) != EBestDetectionType.Visible)
		{
			OnShadowSpottedReport = ReportShadowSpotted;
		}
		base.StartTurn(owner);
		if (!(owner == Owner))
		{
			return;
		}
		ActionPoints = 0;
		if (IsAlive && !Imprisoned)
		{
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				ActionPoints = 1;
			}
			else
			{
				ActionPoints = MaxActionPoints;
			}
			if (GunpointState == EGunpointState.Executor)
			{
				GunpointState = EGunpointState.None;
				if (CurrentWeapon != null && CurrentWeapon.Visualisation != null)
				{
					m_CharacterAnimator.PlayWeaponChange0(AbilityAnim_OnWeaponRemove);
				}
			}
		}
		if (m_CharData != null)
		{
			m_CharData.SetState(ECharacterStateFlag.TurnFinishedAndLocked, Value: false);
		}
		if (m_CharData != null)
		{
			m_CharData.OnStartTurn();
		}
		ApplyJinx();
		if ((bool)owner && owner.IsAi)
		{
			switch (AIState)
			{
			case EAIState.Suspicious:
				UpdateSuspicousState();
				break;
			case EAIState.Subdued:
				UpdateSubduedState();
				break;
			}
		}
	}

	public override void EndTurn(CFGOwner owner)
	{
		base.EndTurn(owner);
		if (!IsAlive)
		{
			UpdateDisintegrate(AllowStart: true);
		}
		if (owner == Owner)
		{
			if (m_CharData != null)
			{
				m_CharData.OnEndTurn();
			}
			if (Owner == null || !Owner.IsPlayer)
			{
				CanDoReactionShot = true;
			}
			UpdateVampire();
		}
	}

	public void OnEndCombat()
	{
		if (!(CharacterAnimator == null) && !(Owner == null) && !Owner.IsAi)
		{
			CharacterAnimator.PlayWeaponChange0(AbilityAnim_OnWeaponRemove);
		}
	}

	private void UpdateSubduedState()
	{
		if (IsAlive && m_CharData != null && m_CharData.SubduedCount >= 0)
		{
			if (VisibleEnemies.Count > 0)
			{
				m_CharData.SubduedCount--;
			}
			else
			{
				m_CharData.SubduedCount -= 2;
			}
			if (m_CharData.SubduedCount == 1)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_abouttoattack"), plus: false, 1, -1, this);
			}
			if (m_CharData.SubduedCount < 1)
			{
				StartCombat();
			}
			if (CFGOptions.DevOptions.SuspicionNotifications)
			{
				string val = "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_subdue_count", m_CharData.SubduedCount.ToString(), m_CharData.SubduedLimit.ToString()) + "</color>";
				Vector3 position2 = base.Transform.position;
				position2.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, val, plus: false, 1, -1, this);
			}
		}
	}

	private void UpdateSuspicousState()
	{
		if (m_CharData == null)
		{
			return;
		}
		CFGCharacter cFGCharacter = null;
		if (m_VisibileEnemies.Count > 0)
		{
			cFGCharacter = m_VisibileEnemies[0];
		}
		if ((bool)cFGCharacter)
		{
			m_CharData.SuspicionLevel--;
			if (m_CharData.SuspicionLevel == 1)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_abouttoattack"), plus: false, 1, -1, this);
			}
			RotateToward(cFGCharacter.Position);
			if (m_CharData.SuspicionLevel < 1)
			{
				StartCombat();
			}
		}
	}

	private void StartCombat()
	{
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = false;
	}

	public override void OnTacticalEnd()
	{
		base.OnTacticalEnd();
	}

	public override void PrePreUpdateLogic()
	{
		if (!CFGTimer.IsPaused_Gameplay && CFGSelectionManager.Instance.CharactersVisibilityNeedUpdate())
		{
			m_BestDetectionType = EBestDetectionType.NotDetected;
		}
	}

	public override void PreUpdateLogic()
	{
		if (!CFGTimer.IsPaused_Gameplay && CFGSelectionManager.Instance.CharactersVisibilityNeedUpdate())
		{
			UpdateTargetsLists();
		}
	}

	public void UpdateEavesdrop(HashSet<CFGCharacter> enemies, float fEDRange, int EDChance)
	{
		Vector3 position = Position;
		foreach (CFGCharacter enemy in enemies)
		{
			if (enemy == null || enemy.BestDetectionType != 0 || enemy.IsDead)
			{
				continue;
			}
			float num = Vector3.Distance(position, enemy.Position);
			if (!(num > fEDRange) && enemy.EavesdropRoll <= EDChance)
			{
				enemy.CharacterData.AddBuff("revealed", EBuffSource.Ability);
				enemy.BestDetectionType = EBestDetectionType.Heard;
				string text = GenerateRNDEavesdropText();
				if (text != null)
				{
					Vector3 position2 = enemy.transform.position;
					position2.y += 1.9f;
					CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + text + "</color>", plus: false, 1, -1, enemy);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!IsAlive)
		{
			bool allowStart = false;
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo != null && !CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo.m_ActivateOnEndTurn)
			{
				allowStart = true;
			}
			UpdateDisintegrate(allowStart);
		}
	}

	public override void UpdateLogic()
	{
		base.UpdateLogic();
		if (CFGTimer.IsPaused_Gameplay)
		{
			return;
		}
		UpdateVisibility();
		if (!IsDead)
		{
			UpdateCurrentAction();
			UpdateSteering();
			UpdateFloorStuff();
			if (m_CharData != null && m_CurrentAction == ETurnAction.None)
			{
				m_CharData.UpdateAbilities();
			}
			if (m_CurrentAction == ETurnAction.None)
			{
				UpdateBestCoverRotation();
			}
			if (m_CurrentCell != m_LastCell)
			{
				m_LastCell = m_CurrentCell;
			}
		}
	}

	private void UpdateTargetsLists()
	{
		VisibleEnemies.Clear();
		VisibleRicochetObjects.Clear();
		SensedEnemies.Clear();
		m_VisibleOtherTargets.Clear();
		if (Owner == null || m_CharData == null || (!Owner.IsPlayer && !Owner.IsAi) || IsDead)
		{
			return;
		}
		float num = 0f;
		CFGAbility ability = GetAbility(ETurnAction.Hearing);
		if (ability != null && ability.IsPassive)
		{
			num = ability.GetRange();
		}
		bool isPlayer = Owner.IsPlayer;
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (!character || !character.gameObject.activeInHierarchy || !(character.Owner != Owner) || character.Imprisoned)
			{
				continue;
			}
			if (character.IsAlive)
			{
				if (CFGCellMap.GetLineOf(CurrentCell, character.CurrentCell, BuffedSight, 16, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None)
				{
					if (character.IsShadowCloaked && Owner != character.Owner)
					{
						if (!(Vector3.Distance(base.Transform.position, character.Transform.position) < CFGDef_Ability.ShadowCloak_Range))
						{
							continue;
						}
						character.LeaveShadowCloak(this);
					}
					if (isPlayer)
					{
						character.SensedByPlayer = true;
						SensedEnemies.Add(character);
					}
					VisibleEnemies.Add(character);
					character.m_BestDetectionType |= EBestDetectionType.Visible;
					continue;
				}
				if (ShouldCheckForEnemyShadows && ((character.BestDetectionType & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted || CFGCellShadowMapLevel.CanTargetBeShadowSpotted(this, character)))
				{
					if (isPlayer)
					{
						character.SensedByPlayer = true;
					}
					character.m_BestDetectionType |= EBestDetectionType.ShadowSpotted;
				}
				if (num > 0f && Vector3.Distance(base.Transform.position, character.Transform.position) <= num && isPlayer)
				{
					character.SensedByPlayer = true;
					character.m_BestDetectionType |= EBestDetectionType.Heard;
				}
				if (character.CharacterData != null && character.CharacterData.HasBuff("revealed") && isPlayer)
				{
					character.SensedByPlayer = true;
					character.m_BestDetectionType |= EBestDetectionType.Smelled;
				}
				if (isPlayer && character.SensedByPlayer)
				{
					SensedEnemies.Add(character);
					character.m_BestDetectionType |= EBestDetectionType.Sensed;
				}
			}
			else if (isPlayer)
			{
				if (CFGCellMap.GetLineOfSightAutoSideSteps(this, character, null, null, BuffedSight) == ELOXHitType.None)
				{
					character.m_BestDetectionType |= EBestDetectionType.Visible;
				}
				else if (ShouldCheckForEnemyShadows && CFGCellShadowMapLevel.CanTargetBeShadowSpotted(this, character))
				{
					character.m_BestDetectionType |= EBestDetectionType.ShadowSpotted;
				}
			}
		}
		SortVisibleEnemiesList(ref m_VisibileEnemies);
		SortVisibleEnemiesList(ref m_SensedEnemies);
		if (HaveAbility(ETurnAction.Ricochet) && (bool)CurrentWeapon && CurrentWeapon.AllowsRicochet)
		{
			foreach (CFGRicochetObject ricochetObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RicochetObjects)
			{
				if (!(ricochetObject == null) && CFGCellMap.GetLineOf(CurrentCell, ricochetObject.Cell, 100000, 16, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, bUseEndSideSteps: false) == ELOXHitType.None)
				{
					VisibleRicochetObjects.Add(ricochetObject);
				}
			}
		}
		foreach (CFGIAttackable otherAttackableObject in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.OtherAttackableObjects)
		{
			if (otherAttackableObject != null && otherAttackableObject.IsAlive && CFGCellMap.GetLineOf(CurrentCell, otherAttackableObject.CurrentCell, BuffedSight, 16, CFGCellMap.m_bLOS_UseSideStepsForStartPoint, bUseEndSideSteps: false) == ELOXHitType.None)
			{
				m_VisibleOtherTargets.Add(otherAttackableObject);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_CharacterAnimator = GetComponent<CFGCharacterAnimator>();
		m_RightHand = FindChildRecursively(base.Transform, "Dummy_weapon_hand");
		if (m_RightHand == null)
		{
			m_RightHand = base.transform;
		}
		GameObject gameObject = new GameObject();
		Transform transform = FindChildRecursively(base.Transform, "Dummy_rifle_target");
		if (!(transform == null))
		{
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = new Vector3(0.486f, -0.005f, 0.029f);
			gameObject.transform.localRotation = Quaternion.Euler(270f, 77.57f, 0f);
			m_LeftHand = gameObject.transform;
		}
	}

	protected override void Start()
	{
		base.Start();
		CFGDef_Character characterDefinition = CFGStaticDataContainer.GetCharacterDefinition(base.NameId);
		if (m_CharData == null)
		{
			bool flag = false;
			if ((bool)Owner && Owner.IsPlayer)
			{
				flag = true;
			}
			CFGCharacterData cFGCharacterData = CFGCharacterList.RegisterNewCharacter(base.NameId, flag, TempTactical: true, null);
			if (cFGCharacterData == null && flag)
			{
				cFGCharacterData = CFGCharacterList.GetCharacterData(base.NameId);
			}
			m_bCharDataIsTemp = true;
			AssignToCharacterData(cFGCharacterData, IsReInint: false);
		}
		int preset_idx = characterDefinition?.PresetIdx ?? (-1);
		SetupCustomization(preset_idx);
		if (!CFG_SG_Manager.IsLoading)
		{
			if (Imprisoned)
			{
				ActionPoints = 0;
			}
			else if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				ActionPoints = 1;
			}
			else
			{
				ActionPoints = MaxActionPoints;
			}
		}
		if (m_Steering != null)
		{
			m_Steering.m_Character = this;
		}
		m_EavesdropRoll = UnityEngine.Random.Range(0, 100);
		CFGCell characterCell = CFGCellMap.GetCharacterCell(base.Transform.position);
		if ((bool)characterCell && IsAlive)
		{
			CurrentCell = characterCell;
			CurrentCell.CharacterEnter(this, null);
			CurrentCell.CharacterMoveEnd(this);
			ProcessFacingToCover(instant_change: true);
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			GameObject gameObject = CFGSingleton<CFGWindowMgr>.Instance.SpawnCharacterFlag();
			if (gameObject != null)
			{
				CFGCharacterIndicator component = gameObject.GetComponent<CFGCharacterIndicator>();
				component.m_Character = this;
			}
		}
		CFGSelectionManager.Instance.OnCharacterSpawned(this);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		CurrentCell = CFGCellMap.GetCharacterCell(base.Transform.position);
		if ((bool)Owner)
		{
			Owner.AddCharacter(this);
		}
	}

	protected void OnDrawGizmos()
	{
		if (m_Path == null)
		{
			return;
		}
		for (LinkedListNode<CFGCell> linkedListNode = m_Path.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			Gizmos.DrawSphere(linkedListNode.Value.WorldPosition + Vector3.up * 1.73f, 0.5f);
			if (linkedListNode != m_Path.First)
			{
				Gizmos.DrawLine(linkedListNode.Previous.Value.WorldPosition, linkedListNode.Value.WorldPosition);
			}
		}
	}

	private bool ShouldEquipWeapon()
	{
		return Owner == null || Owner.IsAi || HaveAbility(ETurnAction.Disguise) || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage;
	}

	private void SpendActionPoints(int amount)
	{
		if ((bool)Owner && Owner.IsPlayer)
		{
			CFGAchievmentTracker.LastAPCost = amount;
			CFGAchievmentTracker.AddAPUsed(amount);
		}
		if (amount > 0 && !CFGSingleton<CFGGame>.Instance.IsInStrategic() && !CFGCheats.InfiniteAP)
		{
			ActionPoints -= Mathf.Min(amount, ActionPoints);
		}
	}

	private void UpdateVisibility(bool force = false)
	{
		bool flag = Owner == null || Owner.IsPlayer || m_BestDetectionType != EBestDetectionType.NotDetected;
		if (m_IsVisible2 != flag || force)
		{
			m_IsVisible2 = flag;
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component != null)
			{
				if (flag)
				{
					if ((component.Focus == null || (CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.CurrentCharacter == this)) && Owner != null && Owner == CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner)
					{
						component.ChangeFocus(this);
					}
				}
				else if (component.Focus == this)
				{
					component.ClearFocus();
				}
			}
		}
		if (!m_WasCustomized)
		{
			return;
		}
		bool flag2 = false;
		CFGCamera component2 = Camera.main.GetComponent<CFGCamera>();
		if (component2 != null && CurrentCell != null)
		{
			flag2 = (int)component2.CurrentFloorLevel >= CurrentCell.Floor;
		}
		bool flag3 = (BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible && flag2;
		if (m_IsVisible != flag3 || force || m_IsOnCorrectFloor != flag2 || CFGCheats.AllCharactersVisible)
		{
			m_IsVisible = flag3;
			if (Owner is CFGAiOwner)
			{
				if ((m_IsVisible && flag2) || CFGCheats.AllCharactersVisible)
				{
					SetVisible();
					ManageOutliner(force: true);
				}
				else
				{
					SetInvisible();
				}
			}
		}
		if (Owner != null && Owner.IsAi)
		{
			if (m_IsOnCorrectFloor != flag2 || m_VisibilityState != m_BestDetectionType)
			{
				m_IsOnCorrectFloor = flag2;
				if (!m_IsOnCorrectFloor && (BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible)
				{
					if (IsAlive)
					{
						GiveOutline(EOutlineType.FLOOR);
					}
				}
				else if ((BestDetectionType & EBestDetectionType.Visible) != EBestDetectionType.Visible)
				{
					TakeOutline(m_OutlineState);
				}
				else
				{
					TakeOutline(EOutlineType.FLOOR);
				}
			}
			if (m_VisibilityState != m_BestDetectionType)
			{
				if ((m_VisibilityState & EBestDetectionType.Visible) != EBestDetectionType.Visible && (m_BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible)
				{
					m_FlagNeedFlash = true;
				}
				m_VisibilityState = m_BestDetectionType;
				if ((m_BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible)
				{
					foreach (Renderer key in m_RenderersShaderDic.Keys)
					{
						if (!(key == null))
						{
							key.shadowCastingMode = ShadowCastingMode.On;
						}
					}
				}
				if (((m_VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted || (m_VisibilityState & EBestDetectionType.Sensed) == EBestDetectionType.Sensed || (m_VisibilityState & EBestDetectionType.Heard) == EBestDetectionType.Heard || (m_VisibilityState & EBestDetectionType.Smelled) == EBestDetectionType.Smelled) && (m_VisibilityState & EBestDetectionType.Visible) != EBestDetectionType.Visible)
				{
					if (IsAlive && m_SpottedFx == null)
					{
						Transform enemySpottedFxPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_EnemySpottedFxPrefab;
						if (enemySpottedFxPrefab != null)
						{
							m_SpottedFx = UnityEngine.Object.Instantiate(enemySpottedFxPrefab, base.Transform.position, Quaternion.identity) as Transform;
							m_SpottedFx.parent = base.Transform;
						}
					}
				}
				else if (m_SpottedFx != null)
				{
					m_SpottedFx.parent = null;
					UnityEngine.Object.Destroy(m_SpottedFx.gameObject);
					m_SpottedFx = null;
				}
				if ((m_VisibilityState & EBestDetectionType.Heard) == EBestDetectionType.Heard)
				{
					m_FlagNeedFlash = true;
				}
				if ((m_VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted && (m_BestDetectionType & EBestDetectionType.Visible) != EBestDetectionType.Visible)
				{
					foreach (Renderer key2 in m_RenderersShaderDic.Keys)
					{
						if (!(key2 == null))
						{
							key2.shadowCastingMode = ShadowCastingMode.On;
						}
					}
				}
				else
				{
					m_ShadowSpottedReported = false;
					if (!m_IsVisible)
					{
						foreach (Renderer key3 in m_RenderersShaderDic.Keys)
						{
							if (!(key3 == null))
							{
								key3.shadowCastingMode = ShadowCastingMode.Off;
							}
						}
					}
				}
			}
		}
		else if (Owner != null && m_IsOnCorrectFloor != flag2)
		{
			m_IsOnCorrectFloor = flag2;
			if (m_IsOnCorrectFloor)
			{
				SetVisible();
				TakeOutline(EOutlineType.FLOOR);
			}
			else
			{
				SetInvisible();
				if (IsAlive)
				{
					GiveOutline(EOutlineType.FLOOR);
				}
			}
		}
		if (OnShadowSpottedReport != null && (m_VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			OnShadowSpottedReport();
			OnShadowSpottedReport = null;
		}
	}

	public void ReportShadowSpotted()
	{
		if (!m_ShadowSpottedReported && (m_VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted && !IsDead && (m_BestDetectionType & EBestDetectionType.Visible) != EBestDetectionType.Visible)
		{
			Vector3 position = base.transform.position;
			position.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("floating_shadowspotting"), plus: false, 1, -1, this);
			m_ShadowSpottedReported = true;
		}
	}

	public void CreateShaderBackup()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		if (m_RenderersShaderDic == null)
		{
			m_RenderersShaderDic = new Dictionary<Renderer, Shader>();
		}
		else
		{
			m_RenderersShaderDic.Clear();
		}
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			m_RenderersShaderDic.Add(renderer, renderer.material.shader);
		}
	}

	public void RemoveFromShaderBackup(Transform _object)
	{
		if (!(_object == null) && m_RenderersShaderDic != null)
		{
			Renderer[] componentsInChildren = _object.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer key in array)
			{
				m_RenderersShaderDic.Remove(key);
			}
		}
	}

	public void AddToShaderBackup(Transform _object)
	{
		if (!(_object == null) && m_RenderersShaderDic != null)
		{
			Renderer[] componentsInChildren = _object.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				m_RenderersShaderDic.Add(renderer, renderer.material.shader);
			}
		}
	}

	public void SetInvisible()
	{
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_InvisibleShader == null || m_RenderersShaderDic == null || m_RenderersShaderDic.Count == 0)
		{
			return;
		}
		m_IsInvisible = true;
		foreach (Renderer key in m_RenderersShaderDic.Keys)
		{
			if (!(key == null))
			{
				key.material.shader = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_InvisibleShader;
			}
		}
		Projector[] componentsInChildren = GetComponentsInChildren<Projector>();
		Projector[] array = componentsInChildren;
		foreach (Projector projector in array)
		{
			projector.enabled = false;
		}
		Transform transform = base.Transform.Find("Chest");
		if (!(transform == null))
		{
			ParticleSystemRenderer[] componentsInChildren2 = transform.GetComponentsInChildren<ParticleSystemRenderer>();
			ParticleSystemRenderer[] array2 = componentsInChildren2;
			foreach (ParticleSystemRenderer particleSystemRenderer in array2)
			{
				particleSystemRenderer.enabled = false;
			}
		}
	}

	private void SetVisible()
	{
		if (m_RenderersShaderDic == null || m_RenderersShaderDic.Count == 0)
		{
			return;
		}
		m_IsInvisible = false;
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (m_RenderersShaderDic.TryGetValue(renderer, out var value))
			{
				renderer.material.shader = value;
			}
		}
		Projector[] componentsInChildren2 = GetComponentsInChildren<Projector>();
		Projector[] array2 = componentsInChildren2;
		foreach (Projector projector in array2)
		{
			projector.enabled = true;
		}
		Transform transform = base.Transform.Find("Chest");
		if (!(transform == null))
		{
			ParticleSystemRenderer[] componentsInChildren3 = transform.GetComponentsInChildren<ParticleSystemRenderer>();
			ParticleSystemRenderer[] array3 = componentsInChildren3;
			foreach (ParticleSystemRenderer particleSystemRenderer in array3)
			{
				particleSystemRenderer.enabled = true;
			}
		}
	}

	public void GiveOutline(EOutlineType _type)
	{
		if (m_RenderersShaderDic == null)
		{
			return;
		}
		int layer = ((!(Owner is CFGPlayerOwner)) ? 21 : 20);
		if (base.gameObject != null)
		{
			base.gameObject.layer = layer;
		}
		foreach (Renderer key in m_RenderersShaderDic.Keys)
		{
			if (!(key == null) && !(key is ParticleSystemRenderer))
			{
				key.gameObject.layer = layer;
			}
		}
		m_HasOutline = true;
		m_OutlineState |= _type;
	}

	private void TakeOutline(EOutlineType _type)
	{
		if (m_RenderersShaderDic == null)
		{
			return;
		}
		m_OutlineState &= ~_type;
		if (m_OutlineState != 0)
		{
			return;
		}
		if (base.gameObject != null)
		{
			base.gameObject.layer = 0;
		}
		foreach (Renderer key in m_RenderersShaderDic.Keys)
		{
			if (!(key == null) && !(key is ParticleSystemRenderer))
			{
				key.gameObject.layer = 0;
			}
		}
		m_HasOutline = false;
	}

	private void UpdateFloorStuff()
	{
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			float num = (float)CFGCellObject.GetFloor(base.Transform.position.y) * 2.5f;
			float num2 = num;
			float y = base.Transform.position.y;
			if (m_CurrentCellObjectInside != null && CurrentCell.HaveFloor)
			{
				num2 = num2;
			}
			if (y > num - 0.01f && y < num + 0.01f)
			{
				base.Transform.position += Vector3.up * Mathf.Min(Time.deltaTime * 0.1f, Mathf.Abs(y - num2)) * ((!(num2 > y)) ? (-1f) : 1f);
			}
		}
		else
		{
			Vector3 position = base.Transform.position;
			position.y += 220f;
			Ray ray = new Ray(position, Vector3.down);
			int layerMask = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_StrategicTerrainLayer;
			if (Physics.Raycast(ray, out var hitInfo, 1024f, layerMask))
			{
				base.Transform.position = hitInfo.point;
			}
		}
	}

	private void UpdateSteering()
	{
		CFGCell characterCell = CFGCellMap.GetCharacterCell(base.Transform.position);
		if (characterCell != CurrentCell)
		{
			if (m_CharData != null && m_CharData.HasBuff("arteryshot") && m_CharData != null && m_Steering != null && !m_Steering.m_SteerData.m_IsOnLadder)
			{
				m_CharData.ArteryDistance++;
				if (m_CharData.ArteryDistance > 4)
				{
					CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(ETurnAction.ArteryShot);
					int dmg = 8;
					if (abilityDef != null)
					{
						dmg = abilityDef.EffectValue;
					}
					if (TakeDamage(dmg, null, bSilent: false))
					{
						return;
					}
					m_CharData.RemBuff("arteryshot");
					m_CharData.ArteryDistance = 0;
				}
			}
			if ((bool)CurrentCell)
			{
				CurrentCell.CharacterLeave(this, characterCell);
			}
			CFGCell currentCell = CurrentCell;
			CurrentCell = characterCell;
			if ((bool)CurrentCell)
			{
				CurrentCell.CharacterMoveEnter(this);
				CurrentCell.CharacterEnter(this, currentCell);
			}
			m_EavesdropRoll = UnityEngine.Random.Range(0, 100);
		}
		if (m_Steering != null)
		{
			m_Steering.TickPlugin(Time.deltaTime);
			if (m_Steering.m_SteerData.m_IsMoving && (bool)CurrentCell && (bool)Owner && Owner.IsPlayer)
			{
				UpdateReactionShoot(characterCell);
			}
		}
	}

	public void Steering_MoveThroughDoors(Vector3 nextpos, Vector3 curpos)
	{
		CFGCell cell = CFGCellMap.GetCell(curpos);
		if (cell == null || cell.OwnerObject == null || !cell.OwnerObject.IsDoor)
		{
			return;
		}
		CFGDoorObject door = cell.OwnerObject.GetDoor();
		if (door == null || !door.CanPlayAnim || nextpos.x == float.PositiveInfinity)
		{
			return;
		}
		CFGCell cell2 = CFGCellMap.GetCell(nextpos);
		if (cell2 == null || cell2.Floor != cell.Floor)
		{
			return;
		}
		CFGDoorObject door2 = cell2.OwnerObject.GetDoor();
		if (!(door2 == door))
		{
			return;
		}
		float num = Vector3.Distance(cell2.WorldPosition, cell.WorldPosition);
		if (num < 2f)
		{
			cell.DecodePosition(out var PosX, out var PosZ, out var _);
			cell2.DecodePosition(out var PosX2, out var PosZ2, out var _);
			bool flag = false;
			if (PosX == PosX2 + 1 && cell.CheckFlag(2, 64))
			{
				flag = true;
			}
			if (PosZ == PosZ2 + 1 && cell.CheckFlag(5, 64))
			{
				flag = true;
			}
			if (PosX == PosX2 - 1 && cell.CheckFlag(3, 64))
			{
				flag = true;
			}
			if (PosZ == PosZ2 - 1 && cell.CheckFlag(4, 64))
			{
				flag = true;
			}
			if (flag)
			{
				door.OnCharacterMoveThrough(this);
			}
		}
	}

	private void Steering_MoveThroughDoors()
	{
		if (CurrentCell == null || CurrentCell.OwnerObject == null || !CurrentCell.OwnerObject.IsDoor)
		{
			return;
		}
		CFGDoorObject door = CurrentCell.OwnerObject.GetDoor();
		if (door == null || !door.CanPlayAnim)
		{
			return;
		}
		Vector3 nextNextPosition = m_Steering.GetNextNextPosition();
		if (nextNextPosition.x == float.PositiveInfinity)
		{
			return;
		}
		Vector3 worldPosition = CurrentCell.WorldPosition;
		Vector3 normalized = (nextNextPosition - worldPosition).normalized;
		nextNextPosition = worldPosition + normalized * 0.8f;
		CFGCell cell = CFGCellMap.GetCell(nextNextPosition);
		if (cell == null || cell.Floor != CurrentCell.Floor)
		{
			return;
		}
		CFGDoorObject door2 = cell.OwnerObject.GetDoor();
		if (!(door2 == door))
		{
			return;
		}
		float num = Vector3.Distance(cell.WorldPosition, CurrentCell.WorldPosition);
		if (num < 2f)
		{
			CurrentCell.DecodePosition(out var PosX, out var PosZ, out var _);
			cell.DecodePosition(out var PosX2, out var PosZ2, out var _);
			bool flag = false;
			if (PosX == PosX2 + 1 && CurrentCell.CheckFlag(2, 64))
			{
				flag = true;
			}
			if (PosZ == PosZ2 + 1 && CurrentCell.CheckFlag(5, 64))
			{
				flag = true;
			}
			if (PosX == PosX2 - 1 && CurrentCell.CheckFlag(3, 64))
			{
				flag = true;
			}
			if (PosZ == PosZ2 - 1 && CurrentCell.CheckFlag(4, 64))
			{
				flag = true;
			}
			if (flag)
			{
				door.OnCharacterMoveThrough(this);
			}
		}
	}

	private void CheckEnemiesSuspicion(ETurnAction ended_action)
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (!(aiOwner == null) && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			if (!ended_action.IsSilent(this))
			{
				aiOwner.RaiseAlarm();
			}
			else if (ended_action == ETurnAction.Move && !HaveAbility(ETurnAction.Disguise))
			{
			}
		}
	}

	public Transform GetDamagePivot()
	{
		Transform transform = base.Transform.Find("Chest");
		if (transform == null)
		{
			transform = base.Transform.Find("Dummy_rifle_back");
		}
		if (transform == null)
		{
			transform = base.Transform;
		}
		return transform;
	}

	public CFGOwner GetOwner()
	{
		return m_Owner;
	}

	public void ApplyBuffAction(CFGDef_UsableItem.eActionType Action, string Param, CFGCharacter caster)
	{
		if (Action == CFGDef_UsableItem.eActionType.None || CharacterData == null)
		{
			return;
		}
		int num = 0;
		if (Param != null)
		{
			try
			{
				num = int.Parse(Param);
			}
			catch
			{
			}
		}
		switch (Action)
		{
		case CFGDef_UsableItem.eActionType.AddBuff:
			CharacterData.AddBuff(Param, EBuffSource.UsableAction);
			break;
		case CFGDef_UsableItem.eActionType.RemoveBuff:
			CharacterData.RemBuff(Param);
			break;
		case CFGDef_UsableItem.eActionType.RemoveAllBuffs:
			CharacterData.RemoveBuffs(EBuffSource.UsableAction);
			CharacterData.RemoveBuffs(EBuffSource.Script);
			break;
		case CFGDef_UsableItem.eActionType.PSet_MaxHealth:
		{
			int num2 = num - MaxHp;
			if (num2 > 0)
			{
				CFGAchievmentTracker.EqualizationUsedInFirstTurn = false;
			}
			m_CharData.BaseStats.SetMaxHealth(num);
			m_CharData.Hp = Mathf.Max(m_CharData.Hp + num2, 1);
			break;
		}
		case CFGDef_UsableItem.eActionType.PAdd_MaxHealth:
			CFGAchievmentTracker.EqualizationUsedInFirstTurn = false;
			m_CharData.BaseStats.SetMaxHealth(num + m_CharData.BaseStats.m_MaxHealth);
			m_CharData.Hp = Mathf.Max(m_CharData.Hp + num, 1);
			break;
		case CFGDef_UsableItem.eActionType.PSet_MaxLuck:
			m_CharData.BaseStats.SetMaxLuck(num);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_MaxLuck:
			m_CharData.BaseStats.SetMaxLuck(m_CharData.BaseStats.m_MaxLuck + num);
			break;
		case CFGDef_UsableItem.eActionType.PSet_Movement:
			m_CharData.BaseStats.SetMovement(num);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_Movement:
			m_CharData.BaseStats.SetMovement(m_CharData.BaseStats.m_Movement + num);
			break;
		case CFGDef_UsableItem.eActionType.PSet_Sight:
			m_CharData.BaseStats.SetSight(num);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_Sight:
			m_CharData.BaseStats.SetSight(m_CharData.BaseStats.m_Sight + num);
			break;
		case CFGDef_UsableItem.eActionType.PSet_Aim:
			m_CharData.BaseStats.SetAim(num);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_Aim:
			m_CharData.BaseStats.SetAim(m_CharData.BaseStats.m_Aim + num);
			break;
		case CFGDef_UsableItem.eActionType.PSet_Defense:
			m_CharData.BaseStats.SetDefense(num);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_Defense:
			m_CharData.BaseStats.SetDefense(m_CharData.BaseStats.m_Defense + num);
			break;
		case CFGDef_UsableItem.eActionType.HP_Mod:
			if (num < 0)
			{
				bool flag = TakeDamage(-num, caster, bSilent: false);
				break;
			}
			Heal(num, bSilent: false);
			CFGAchievmentTracker.EqualizationUsedInFirstTurn = false;
			break;
		case CFGDef_UsableItem.eActionType.PSet_Luck:
			m_CharData.SetLuck(num, bAllowSplash: true);
			break;
		case CFGDef_UsableItem.eActionType.PAdd_Luck:
			m_CharData.SetLuck(m_CharData.Luck + num, bAllowSplash: true);
			break;
		}
	}

	public int GetCoverMult()
	{
		return 1;
	}

	public int GetAimBonus()
	{
		return 0;
	}

	public CFGAbility GetAbility(ETurnAction Ability)
	{
		if (m_CharData == null || m_CharData.Abilities == null)
		{
			return null;
		}
		CAbilityInfo value = null;
		if (!m_CharData.Abilities.TryGetValue(Ability, out value))
		{
			return null;
		}
		return value.Ability;
	}

	public void UpdateVampire()
	{
		if (!HaveAbility(ETurnAction.Vampire) || m_CharData == null)
		{
			return;
		}
		if (CFGGame.Nightmare)
		{
			m_CharData.RemBuff("shadowregen");
			if (!m_CharData.HasBuff("nightmareregen"))
			{
				m_CharData.AddBuff("nightmareregen", EBuffSource.Ability);
			}
		}
		else if (IsInShadow)
		{
			m_CharData.RemBuff("nightmareregen");
			if (!m_CharData.HasBuff("shadowregen"))
			{
				m_CharData.AddBuff("shadowregen", EBuffSource.Ability);
			}
		}
	}

	public void UpdateIntimidate()
	{
		if (Owner == null || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner == null || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner == null || m_CharData == null)
		{
			return;
		}
		CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(ETurnAction.Intimidate);
		if (abilityDef == null)
		{
			return;
		}
		HashSet<CFGCharacter> hashSet = null;
		hashSet = ((!Owner.IsAi) ? CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters : CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner.Characters);
		Vector3 position = Position;
		if (HaveAbility(ETurnAction.Intimidate))
		{
			foreach (CFGCharacter item in hashSet)
			{
				if (item.IsAlive && item.CharacterData != null && item.CharacterData.Definition.CanPanic && !item.CharacterData.HasBuff("intimidated"))
				{
					Vector3 position2 = item.Position;
					float num = Vector3.Distance(position2, position);
					if (!(num > abilityDef.Range) && (!abilityDef.UseLOS || CFGCellMap.GetLineOf(CurrentCell, item.CurrentCell, 10000, 16, bUseStartSideSteps: false, bUseEndSideSteps: false) == ELOXHitType.None))
					{
						item.CharacterData.AddBuff("intimidated", EBuffSource.Ability);
						m_IntimidateFlagNeedFlash = true;
					}
				}
			}
		}
		if (!m_CharData.Definition.CanPanic)
		{
			return;
		}
		foreach (CFGCharacter item2 in hashSet)
		{
			if (item2.IsAlive && item2.HaveAbility(ETurnAction.Intimidate))
			{
				Vector3 position3 = item2.Position;
				float num2 = Vector3.Distance(position3, position);
				if (!(num2 > abilityDef.Range) && (!abilityDef.UseLOS || CFGCellMap.GetLineOf(CurrentCell, item2.CurrentCell, 10000, 16, bUseStartSideSteps: false, bUseEndSideSteps: false) == ELOXHitType.None))
				{
					CharacterData.AddBuff("intimidated", EBuffSource.Ability);
					item2.m_IntimidateFlagNeedFlash = true;
					break;
				}
			}
		}
	}

	public void OnGameplayPause()
	{
		if (m_CurrentAction != ETurnAction.None)
		{
			Animator component = GetComponent<Animator>();
			if ((bool)component)
			{
				component.speed = 0f;
			}
		}
	}

	public void OnGameplayUnPause()
	{
		if (m_CurrentAction != ETurnAction.None)
		{
			Animator component = GetComponent<Animator>();
			if ((bool)component)
			{
				component.speed = 1f;
			}
		}
	}

	public void DisintegrateBody()
	{
		if (!CanDisintegrateBody)
		{
			return;
		}
		float num = 0f;
		if (m_CharData != null && m_CharData.DeathTime + 4f > CFGTimer.MissionTime)
		{
			num = 4f;
			CFGDisintegrationInfo disinegrationInfo = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo;
			if (disinegrationInfo != null)
			{
				num = disinegrationInfo.m_DelayAfterDeath;
			}
		}
		m_DisintegrateStartTime = CFGTimer.MissionTime + num;
	}

	private void UpdateDisintegrate(bool AllowStart)
	{
		if (this == null || m_DisintegrateStartTime <= 0f || m_DisintegrateStartTime > CFGTimer.MissionTime)
		{
			return;
		}
		DisintegrationEffect();
		if (m_DisintegrateEndTime < 0f)
		{
			if (!AllowStart)
			{
				return;
			}
			m_DisintegrateEndTime = m_DisintegrateStartTime + 3f;
			CFGDisintegrationInfo disinegrationInfo = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_DisinegrationInfo;
			if (disinegrationInfo == null)
			{
				return;
			}
			m_DisintegrateEndTime = m_DisintegrateStartTime + disinegrationInfo.m_DisintegrationTime;
			if (disinegrationInfo.m_FX != null)
			{
				Transform damagePivot = GetDamagePivot();
				if (damagePivot == null)
				{
					UnityEngine.Object.Instantiate(disinegrationInfo.m_FX, Position, base.transform.rotation);
				}
				else
				{
					UnityEngine.Object.Instantiate(disinegrationInfo.m_FX, damagePivot.position, damagePivot.rotation);
				}
			}
			if (disinegrationInfo.m_WeaponAmmo != null)
			{
				Transform damagePivot2 = GetDamagePivot();
				if (damagePivot2 == null)
				{
					UnityEngine.Object.Instantiate(disinegrationInfo.m_WeaponAmmo, Position, base.transform.rotation);
				}
				else
				{
					UnityEngine.Object.Instantiate(disinegrationInfo.m_WeaponAmmo, damagePivot2.position, damagePivot2.rotation);
				}
			}
		}
		else if (!(m_DisintegrateEndTime > CFGTimer.MissionTime))
		{
			if (m_CharData != null)
			{
				m_CharData.CurrentModel = null;
			}
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.UnregisterImmediatly(this);
			UnityEngine.Object.Destroy(base.gameObject);
			m_CharData = null;
		}
	}

	private void DisintegrationEffect()
	{
		if (!(m_DisintegrateEndTime < CFGTimer.MissionTime))
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component != null && (int)component.CurrentFloorLevel >= CFGCellMap.GetCharacterCell(base.transform.position + new Vector3(0f, 0.4f, 0f)).Floor)
			{
				float num = 0.4f / (m_DisintegrateEndTime - m_DisintegrateStartTime);
				base.transform.position -= new Vector3(0f, num * Time.deltaTime, 0f);
			}
			else
			{
				m_DisintegrateEndTime = CFGTimer.MissionTime;
			}
		}
	}

	public CFGCharacter GetNearestSuspiciousEnemy()
	{
		if (m_VisibileEnemies == null || m_VisibileEnemies.Count == 0)
		{
			return null;
		}
		CFGCharacter result = null;
		float num = 10000f;
		float num2 = 0.7f;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
		{
			float num3 = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.DetectionAngle * 0.5f;
			num2 = Mathf.Cos(num3 * ((float)Math.PI / 180f));
		}
		float num4 = m_CharData.TotalHeat;
		foreach (CFGCharacter visibleEnemy in VisibleEnemies)
		{
			if ((bool)visibleEnemy && visibleEnemy.IsAlive)
			{
				float num5 = Vector3.Distance(Position, visibleEnemy.Position);
				Vector3 normalized = (visibleEnemy.Position - Position).normalized;
				float num6 = Vector3.Dot(base.transform.forward, normalized);
				if (!(num5 > num4) && !(num5 >= num) && !(num6 < num2))
				{
					num = num5;
					result = visibleEnemy;
				}
			}
		}
		return result;
	}

	public void HandleSuspiciousMovement(CFGCharacter looktarget)
	{
		if (AIState == EAIState.Subdued || AIState == EAIState.InCombat || m_CharData == null || Owner == null)
		{
			return;
		}
		if ((bool)looktarget)
		{
			RotateToward(looktarget.Position);
		}
		string text = null;
		if (AIState == EAIState.Suspicious)
		{
			m_CharData.SuspicionLevel--;
			if (CFGOptions.DevOptions.SuspicionNotifications)
			{
				text = "<color=white> DevSuspicous Check: " + m_CharData.SuspicionLevel + " of " + m_CharData.SuspicionLimit + "</color>";
			}
			if (m_CharData.SuspicionLevel < 1)
			{
				StartCombat();
				return;
			}
			if (m_CharData.SuspicionLevel == 1)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_abouttoattack"), plus: false, 1, -1, this);
			}
		}
		else
		{
			AIState = EAIState.Suspicious;
			m_CharData.SuspicionLevel = m_CharData.SuspicionLimit;
			CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_suspicion", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
			text = "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_suspicion") + "</color>";
		}
		if (text != null)
		{
			Vector3 position2 = base.Transform.position;
			position2.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, text, plus: false, 1, -1, this);
		}
		float num = 0.7f;
		float num2 = 10f;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
		{
			num2 = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.ChainNotifyDistance;
			float num3 = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.DetectionAngle * 0.5f;
			num = Mathf.Cos(num3 * ((float)Math.PI / 180f));
		}
		foreach (CFGCharacter character in Owner.Characters)
		{
			if (character == this || character.AIState != 0 || CFGCellMap.GetLineOfSightAutoSideSteps(character, this) != 0)
			{
				continue;
			}
			float num4 = Vector3.Distance(Position, character.Position);
			if (!(num4 > num2))
			{
				Vector3 lhs = Position - character.Position;
				lhs.Normalize();
				float num5 = Vector3.Dot(lhs, base.transform.forward.normalized);
				if (!(num5 < num))
				{
					character.HandleSuspiciousMovement(this);
				}
			}
		}
	}

	public void EnterSubduedState(CFGCharacter ManWithGun)
	{
		if (AIState != EAIState.InCombat && m_CharData != null && ManWithGun.CharacterData != null)
		{
			bool flag = AIState == EAIState.Subdued;
			AIState = EAIState.Subdued;
			if (m_CharData.SubduedCount > 0)
			{
				m_CharData.SubduedCount += ManWithGun.CharacterData.TotalHeat;
			}
			else
			{
				m_CharData.SubduedCount = ManWithGun.CharacterData.TotalHeat;
			}
			if (m_CharData.SubduedCount > m_CharData.SubduedLimit)
			{
				m_CharData.SubduedCount = m_CharData.SubduedLimit;
			}
			string text = null;
			if (!flag)
			{
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_gunpointstart", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId));
				text = "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_subdued") + "</color>";
			}
			else if (CFGOptions.DevOptions.SuspicionNotifications)
			{
				text = "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_subdue_count", m_CharData.SubduedCount.ToString(), m_CharData.SubduedLimit.ToString()) + "</color>";
			}
			if (text != null)
			{
				Vector3 position = base.Transform.position;
				position.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, text, plus: false, 1, -1, this);
			}
			CFGSoundDef.Play(m_GunpointTargetSoundDef, base.Transform);
		}
	}

	public void UpdateBestCoverRotation()
	{
		EBestCoverRotation bestCoverRotation = m_BestCoverRotation;
		m_BestCoverRotation = EBestCoverRotation.Auto;
		if (bestCoverRotation != m_BestCoverRotation)
		{
			ProcessFacingToCover();
		}
	}

	public void CalclualteBestCoverRotation()
	{
		CFGCharacter cFGCharacter = null;
		float num = 1000000f;
		foreach (CFGCharacter visibileEnemy in m_VisibileEnemies)
		{
			float num2 = Vector3.Distance(visibileEnemy.Position, Position);
			Vector3 normalized = (visibileEnemy.Position - Position).normalized;
			if (num2 < num && Vector3.Dot(normalized, base.Transform.forward) < 0f && CFGCellMap.GetLineOfSightAutoSideSteps(this, visibileEnemy) == ELOXHitType.None)
			{
				num = num2;
				cFGCharacter = visibileEnemy;
			}
		}
		if (cFGCharacter == null)
		{
			SetCoverRotation(EBestCoverRotation.Auto);
			return;
		}
		EBestCoverRotation rotationForTarget = GetRotationForTarget(cFGCharacter);
		SetCoverRotation(rotationForTarget);
	}

	public bool IsInFullCover()
	{
		if (m_CurrentCell == null)
		{
			return false;
		}
		ECoverType borderCover = m_CurrentCell.GetBorderCover(EDirection.NORTH);
		if (borderCover == ECoverType.FULL)
		{
			return true;
		}
		ECoverType borderCover2 = m_CurrentCell.GetBorderCover(EDirection.EAST);
		if (borderCover2 == ECoverType.FULL)
		{
			return true;
		}
		ECoverType borderCover3 = m_CurrentCell.GetBorderCover(EDirection.SOUTH);
		if (borderCover3 == ECoverType.FULL)
		{
			return true;
		}
		ECoverType borderCover4 = m_CurrentCell.GetBorderCover(EDirection.WEST);
		if (borderCover4 == ECoverType.FULL)
		{
			return true;
		}
		return false;
	}

	public void SetCoverRotation(EBestCoverRotation Rotation)
	{
		if (!IsInFullCover() || CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			return;
		}
		bool flag = false;
		if (Rotation != m_BestCoverRotation)
		{
			flag = true;
		}
		m_BestCoverRotation = Rotation;
		switch (m_BestCoverRotation)
		{
		}
		if (flag)
		{
			ProcessFacingToCover();
		}
	}

	public EBestCoverRotation GetRotationForPosition(Vector3 TgtPosition, EDirection CoverWall, float fFWDMin = 0f)
	{
		if (Vector3.Distance(TgtPosition, Position) < 0.5f)
		{
			return EBestCoverRotation.Auto;
		}
		Vector3 normalized = (TgtPosition - Position).normalized;
		CFGCell cell = CFGCellMap.GetCell(TgtPosition);
		CFGCell[] tiles = new CFGCell[3] { CurrentCell, null, null };
		CFGCellMap.FillUpTable(CurrentCell, normalized, ref tiles);
		CFGCell[] tiles2 = new CFGCell[3] { cell, null, null };
		CFGCellMap.FillUpTable(cell, -normalized, ref tiles2);
		Vector3 lhs = CoverWall switch
		{
			EDirection.WEST => new Vector3(1f, 0f, 0f), 
			EDirection.EAST => new Vector3(-1f, 0f, 0f), 
			EDirection.NORTH => new Vector3(0f, 0f, -1f), 
			_ => new Vector3(0f, 0f, 1f), 
		};
		EDirection dir_Clockwise = CoverWall.GetDir_Clockwise();
		EDirection dir_CounterClockwise = CoverWall.GetDir_CounterClockwise();
		if ((bool)cell)
		{
			CFGCell cFGCell = null;
			float num = 100000f;
			byte flag = 16;
			for (int i = 1; i < 3; i++)
			{
				if (tiles[i] == null)
				{
					continue;
				}
				bool flag2 = false;
				if (CFGCellMap.GetLineOf(tiles[i], cell, 10000, flag, bUseStartSideSteps: false, bUseEndSideSteps: true) == ELOXHitType.None)
				{
					flag2 = true;
				}
				if (!flag2 && CFGCellMap.GetLOXHit(tiles[0].WorldPosition + (tiles[i].WorldPosition - tiles[0].WorldPosition) * 0.5f, ref tiles2, flag) == ELOXHitType.None)
				{
					flag2 = true;
				}
				if (flag2)
				{
					float num2 = Vector3.Distance(tiles[i].WorldPosition, TgtPosition);
					if (num2 < num)
					{
						num = num2;
						cFGCell = tiles[i];
					}
				}
			}
			if ((bool)cFGCell)
			{
				normalized = (cFGCell.WorldPosition - CurrentCell.WorldPosition).normalized;
				if (Vector3.Dot(lhs, normalized) > 0f)
				{
					if (CFGCellMap.CanPassFromTileInDir(CurrentCell, dir_Clockwise))
					{
						return EBestCoverRotation.Right;
					}
				}
				else if (CFGCellMap.CanPassFromTileInDir(CurrentCell, dir_CounterClockwise))
				{
					return EBestCoverRotation.Left;
				}
			}
		}
		return EBestCoverRotation.Auto;
	}

	public EBestCoverRotation GetRotationForTarget(CFGCharacter Target, float fFWDMin = 0f)
	{
		if (Target == null || Target.CurrentCell == null)
		{
			return EBestCoverRotation.Auto;
		}
		Vector3 vector = Target.CurrentCell.WorldPosition + Vector3.up;
		if (Vector3.Distance(vector, Position) < 0.5f)
		{
			return EBestCoverRotation.Auto;
		}
		Vector3 normalized = (vector - Position).normalized;
		CFGCell[] tiles = new CFGCell[3] { CurrentCell, null, null };
		CFGCellMap.FillUpTable(CurrentCell, normalized, ref tiles);
		for (int i = 0; i < 3; i++)
		{
			if (tiles[i] != null && CFGCellMap.GetLineOf(tiles[i], Target.CurrentCell, 10000, 16, bUseStartSideSteps: false, bUseEndSideSteps: true) == ELOXHitType.None)
			{
				normalized = (tiles[i].WorldPosition - CurrentCell.WorldPosition).normalized;
				float num = Vector3.Dot(base.transform.right, normalized);
				if (num > 0.7f)
				{
					return EBestCoverRotation.Right;
				}
				if (num < -0.7f)
				{
					return EBestCoverRotation.Left;
				}
			}
		}
		return EBestCoverRotation.Auto;
	}

	public void SetHP(int NewVal, CFGCharacter Source, bool bSilent)
	{
		if (NewVal < Hp)
		{
			TakeDamage(Hp - NewVal, Source, bSilent);
		}
		else if (NewVal > Hp)
		{
			Heal(NewVal - Hp, bSilent);
		}
	}

	public void SpawnLuckSplash(int LuckVal)
	{
		if (LuckVal == 0 || (BestDetectionType == EBestDetectionType.NotDetected && Owner != null && Owner.IsAi))
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y += 1.9f;
		if (m_LuckSplash != null && CFGTimer.MissionTime < m_LastLuckSplashTime + CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_LuckSplashTimeOffset)
		{
			int result = 0;
			int.TryParse(m_LuckSplash.m_Text.text, out result);
			if (m_LuckSplash.m_Splash.IconNumber == 3)
			{
				result = -result;
			}
			result += LuckVal;
			string text = Mathf.Abs(result).ToString();
			m_LuckSplash.m_Text.text = text;
			m_LuckSplash.m_TextCenter.text = text;
			m_LuckSplash.m_BuffRecivedText.text = text;
			if (result > 0)
			{
				m_LuckSplash.m_Splash.IconNumber = 2;
			}
			else
			{
				m_LuckSplash.m_Splash.IconNumber = 3;
			}
		}
		else
		{
			m_LuckSplash = CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, Mathf.Abs(LuckVal).ToString(), LuckVal > 0, 0, 2, this);
			m_LastLuckSplashTime = CFGTimer.MissionTime;
		}
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_HUD && Owner != null && Owner.IsPlayer)
		{
			CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashLuck();
		}
	}

	public override bool OnDeserialize(CFG_SG_Node Node)
	{
		if (IsDead)
		{
			SilentDie();
		}
		if (IsInDemonForm && base.gameObject.GetComponent<CFGDemon>() == null)
		{
			base.gameObject.AddComponent<CFGDemon>();
		}
		return true;
	}

	public void SetCurrentCell()
	{
		CFGCell characterCell = CFGCellMap.GetCharacterCell(base.Transform.position);
		if ((bool)characterCell && IsAlive)
		{
			CurrentCell = characterCell;
			CurrentCell.SetCurrentChar(this);
		}
	}

	public static string GetEavesdropText(int id)
	{
		id = Mathf.Clamp(id, 0, 99);
		return $"eavesdrop_text_{id:D2}";
	}

	private void DetectMaxEavesdropText()
	{
		m_MaxEavesdropText = -1;
		for (int i = 1; i < 100; i++)
		{
			string eavesdropText = GetEavesdropText(i);
			if (eavesdropText == null || !CFGSingletonResourcePrefab<CFGTextManager>.Instance.HasText(eavesdropText))
			{
				break;
			}
			m_MaxEavesdropText = i;
		}
	}

	private string GenerateRNDEavesdropText()
	{
		if (m_MaxEavesdropText < 0)
		{
			DetectMaxEavesdropText();
			if (m_MaxEavesdropText < 0)
			{
				Debug.LogWarning("Failed to find eavesdrop texts!");
				return "Detected by eavesdrop";
			}
			Debug.Log("Max eavesdrop texts: " + m_MaxEavesdropText);
		}
		int id = UnityEngine.Random.Range(1, m_MaxEavesdropText);
		return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(GetEavesdropText(id));
	}

	private void OnCellChange()
	{
		RecalcualteShadowCloak(this);
		ManageOutliner();
		CFGSelectionManager instance = CFGSelectionManager.Instance;
		if (instance != null)
		{
			instance.MarkCharactersVisibilityNeedUpdate();
		}
	}

	private bool IsCoveredByArt()
	{
		if (CurrentCell == null)
		{
			return false;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component == null)
		{
			return false;
		}
		ECornerDirection dir = ECornerDirection.NW;
		EDirection eDirection = EDirection.NORTH;
		EDirection eDirection2 = EDirection.WEST;
		switch ((int)component.CurrentAngleH)
		{
		case 130:
			dir = ECornerDirection.NE;
			eDirection = EDirection.EAST;
			eDirection2 = EDirection.NORTH;
			break;
		case 220:
			dir = ECornerDirection.SE;
			eDirection = EDirection.SOUTH;
			eDirection2 = EDirection.EAST;
			break;
		case 310:
			dir = ECornerDirection.SW;
			eDirection = EDirection.WEST;
			eDirection2 = EDirection.SOUTH;
			break;
		}
		byte flagType = 0;
		switch (eDirection)
		{
		case EDirection.NORTH:
			flagType = 2;
			break;
		case EDirection.SOUTH:
			flagType = 3;
			break;
		case EDirection.EAST:
			flagType = 4;
			break;
		case EDirection.WEST:
			flagType = 5;
			break;
		}
		byte flagType2 = 0;
		switch (eDirection2)
		{
		case EDirection.NORTH:
			flagType2 = 2;
			break;
		case EDirection.SOUTH:
			flagType2 = 3;
			break;
		case EDirection.EAST:
			flagType2 = 4;
			break;
		case EDirection.WEST:
			flagType2 = 5;
			break;
		}
		CFGCell cFGCell = CurrentCell.FindNeighbour(dir);
		CFGCell cFGCell2 = CurrentCell.FindNeighbour(eDirection);
		CFGCell cFGCell3 = CurrentCell.FindNeighbour(eDirection2);
		if (cFGCell != null && cFGCell.GetCenterCover() == ECoverType.FULL)
		{
			return true;
		}
		bool flag = CurrentCell.GetBorderCover(eDirection) == ECoverType.FULL || CurrentCell.CheckFlag(flagType, 16) || (cFGCell2 != null && (cFGCell2.CheckFlag(1, 16) || cFGCell2.GetCenterCover() == ECoverType.FULL));
		bool flag2 = CurrentCell.GetBorderCover(eDirection2) == ECoverType.FULL || CurrentCell.CheckFlag(flagType2, 16) || (cFGCell3 != null && (cFGCell3.CheckFlag(1, 16) || cFGCell3.GetCenterCover() == ECoverType.FULL));
		if (flag && flag2)
		{
			return true;
		}
		if (flag && cFGCell3 != null && (cFGCell3.GetBorderCover(eDirection) == ECoverType.FULL || cFGCell3.CheckFlag(flagType, 16) || cFGCell3.CheckFlag(1, 16) || cFGCell3.GetCenterCover() == ECoverType.FULL))
		{
			return true;
		}
		if (flag2 && cFGCell2 != null && (cFGCell2.GetBorderCover(eDirection2) == ECoverType.FULL || cFGCell2.CheckFlag(flagType2, 16) || cFGCell2.CheckFlag(1, 16) || cFGCell2.GetCenterCover() == ECoverType.FULL))
		{
			return true;
		}
		if (CharacterAnimator != null && (CharacterAnimator.CurrentCoverState == 2 || CharacterAnimator.CurrentCoverState == 3))
		{
			CurrentCell.GetBestCoverToGlue(this, out var _, out var dir2);
			dir2 = dir2.Opposite();
			if ((dir2 == eDirection && flag) || (dir2 == eDirection2 && flag2))
			{
				return true;
			}
		}
		if (cFGCell == null || cFGCell2 == null || cFGCell3 == null)
		{
			return false;
		}
		int num = (int)(component.CurrentFloorLevel - CurrentCell.Floor);
		if (num <= 0)
		{
			return false;
		}
		CFGCell[] source = new CFGCell[4]
		{
			CFGCellMap.GetCell(CurrentCell.PositionX, CurrentCell.PositionZ, CurrentCell.Floor + 1),
			CFGCellMap.GetCell(cFGCell.PositionX, cFGCell.PositionZ, cFGCell.Floor + 1),
			CFGCellMap.GetCell(cFGCell2.PositionX, cFGCell2.PositionZ, cFGCell2.Floor + 1),
			CFGCellMap.GetCell(cFGCell3.PositionX, cFGCell3.PositionZ, cFGCell3.Floor + 1)
		};
		if (source.All((CFGCell m) => m?.HaveFloor ?? false))
		{
			return true;
		}
		if (num == 2)
		{
			int num2 = 1 * (cFGCell.PositionX - CurrentCell.PositionX);
			int num3 = 1 * (cFGCell.PositionZ - CurrentCell.PositionZ);
			CFGCell[] source2 = new CFGCell[4]
			{
				CFGCellMap.GetCell(CurrentCell.PositionX + num2, CurrentCell.PositionZ + num3, CurrentCell.Floor + 2),
				CFGCellMap.GetCell(cFGCell.PositionX + num2, cFGCell.PositionZ + num3, cFGCell.Floor + 2),
				CFGCellMap.GetCell(cFGCell2.PositionX + num2, cFGCell2.PositionZ + num3, cFGCell2.Floor + 2),
				CFGCellMap.GetCell(cFGCell3.PositionX + num2, cFGCell3.PositionZ + num3, cFGCell3.Floor + 2)
			};
			return source2.All((CFGCell m) => m?.HaveFloor ?? false);
		}
		return false;
	}

	public static void UpdateAllCharactersOutliner()
	{
		HashSet<CFGCharacter> characters = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters;
		foreach (CFGCharacter item in characters)
		{
			item.ManageOutliner();
		}
	}

	public void ManageOutliner(bool force = false)
	{
		if ((!(Owner is CFGAiOwner) || (m_VisibilityState & EBestDetectionType.Visible) == EBestDetectionType.Visible || force) && !IsDead && !CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			if (IsCoveredByArt())
			{
				GiveOutline(EOutlineType.COVERED_BY_ART);
			}
			else
			{
				TakeOutline(EOutlineType.COVERED_BY_ART);
			}
		}
	}

	public CFGCharacter FindClosestVisibleEnemy(out EDirection DirToTgt)
	{
		DirToTgt = EDirection.EAST;
		CFGCharacter cFGCharacter = null;
		float num = 100000f;
		Vector3 position = Position;
		int num2 = -1;
		EDirection eDirection = EDirection.EAST;
		EDirection eDirection2 = EDirection.EAST;
		ECoverType borderCover = m_CurrentCell.GetBorderCover(EDirection.NORTH);
		ECoverType borderCover2 = m_CurrentCell.GetBorderCover(EDirection.SOUTH);
		ECoverType borderCover3 = m_CurrentCell.GetBorderCover(EDirection.WEST);
		ECoverType borderCover4 = m_CurrentCell.GetBorderCover(EDirection.EAST);
		foreach (CFGCharacter sensedEnemy in m_SensedEnemies)
		{
			if (sensedEnemy == null || sensedEnemy.IsDead || !HasLineOfFireTo(sensedEnemy, ETurnAction.Shoot))
			{
				continue;
			}
			Vector3 position2 = sensedEnemy.Position;
			Vector3 vector = position2 - position;
			vector.y = 0f;
			if (!(vector.magnitude < 0.5f))
			{
				ECoverType eCoverType = ECoverType.NONE;
				Vector3 lhs = sensedEnemy.Position - position;
				lhs.y = 0f;
				lhs.Normalize();
				float num3 = -1f;
				float num4 = Vector3.Dot(lhs, EDirection.NORTH.GetForward());
				if (num4 > 0f && borderCover > eCoverType)
				{
					eCoverType = borderCover;
					eDirection = EDirection.NORTH;
					num3 = num4;
				}
				num4 = Vector3.Dot(lhs, EDirection.SOUTH.GetForward());
				if (num4 > 0f && (borderCover2 > eCoverType || (borderCover2 == eCoverType && num4 >= num3)))
				{
					eCoverType = borderCover2;
					eDirection = EDirection.SOUTH;
					num3 = num4;
				}
				num4 = Vector3.Dot(lhs, EDirection.EAST.GetForward());
				if (num4 > 0f && (borderCover4 > eCoverType || (borderCover4 == eCoverType && num4 >= num3)))
				{
					eCoverType = borderCover4;
					eDirection = EDirection.EAST;
					num3 = num4;
				}
				num4 = Vector3.Dot(lhs, EDirection.WEST.GetForward());
				if (num4 > 0f && (borderCover3 > eCoverType || (borderCover3 == eCoverType && num4 >= num3)))
				{
					eCoverType = borderCover3;
					eDirection = EDirection.WEST;
					num3 = num4;
				}
				int num5 = (int)eCoverType;
				float num6 = Vector3.Distance(position, position2);
				if (num2 <= -1 || (num5 >= num2 && !(num6 >= num)))
				{
					num = num6;
					cFGCharacter = sensedEnemy;
					num2 = num5;
					eDirection2 = eDirection;
				}
			}
		}
		if ((bool)cFGCharacter)
		{
			if (num2 == 0)
			{
			}
			DirToTgt = eDirection2;
		}
		return cFGCharacter;
	}

	public bool AbilityAnim_Play(eAbilityAnimation AnimType)
	{
		if (AnimType == eAbilityAnimation.None)
		{
			return false;
		}
		m_AbilityAnim_Type = AnimType;
		m_AbilityAnim_State = EAbilityAnimState.Preparing;
		return true;
	}

	private void AbilityAnim_Update()
	{
		if (m_CharacterAnimator == null)
		{
			m_AbilityAnim_State = EAbilityAnimState.Stopped;
			m_AbilityAnim_Type = eAbilityAnimation.None;
			return;
		}
		switch (m_AbilityAnim_State)
		{
		case EAbilityAnimState.Preparing:
			AbilityAnim_Prepare();
			break;
		case EAbilityAnimState.WaitingForWeaponRemove:
			AbilityAnim_WeaponRemoveAnimEnd();
			break;
		case EAbilityAnimState.WaitingForAnimEnd:
			AbilityAnim_MainAnimEnd();
			break;
		case EAbilityAnimState.WaitingForWeaponReequip:
			AbilityAnim_WeaponSpawnAnimEnd();
			break;
		}
	}

	private void AbilityAnim_Prepare()
	{
		if (m_CharacterAnimator.IsInIdle())
		{
			m_AbilityAnim_HadWeapon = false;
			if ((bool)CurrentWeapon && (bool)CurrentWeapon.Visualisation)
			{
				m_AbilityAnim_HadWeapon = true;
			}
			bool flag = m_AbilityAnim_Type.NeedWeaponRemoved();
			if (m_AbilityAnim_HadWeapon && flag)
			{
				m_AbilityAnim_State = EAbilityAnimState.WaitingForWeaponRemove;
				m_CharacterAnimator.PlayWeaponChange0(AbilityAnim_OnWeaponRemove);
			}
			else
			{
				m_AbilityAnim_HadWeapon = false;
				AbilityAnim_WeaponRemoveAnimEnd();
			}
		}
	}

	private void AbilityAnim_OnWeaponRemove()
	{
		if ((bool)CurrentWeapon)
		{
			CurrentWeapon.RemoveVisualisation();
		}
	}

	private void AbilityAnim_OnWeaponSpawn()
	{
		if ((bool)CurrentWeapon)
		{
			CurrentWeapon.SpawnVisualisation(RightHand);
		}
	}

	private void AbilityAnim_MainAnimEnd()
	{
		if (!m_CharacterAnimator.IsInIdle())
		{
			return;
		}
		if (m_AbilityAnim_HadWeapon)
		{
			m_AbilityAnim_State = EAbilityAnimState.WaitingForWeaponReequip;
			if (CurrentWeapon.TwoHanded)
			{
				CharacterAnimator.PlayWeaponChange2(null, AbilityAnim_OnWeaponSpawn);
			}
			else
			{
				CharacterAnimator.PlayWeaponChange1(null, AbilityAnim_OnWeaponSpawn);
			}
		}
		else
		{
			AbilityAnim_WeaponSpawnAnimEnd();
		}
	}

	private void AbilityAnim_ThrowEnd()
	{
		CFGAbility_Item cFGAbility_Item = GetAbility(m_CurrentAction) as CFGAbility_Item;
		if (cFGAbility_Item == null)
		{
			AbilityAnim_MainAnimEnd();
		}
	}

	private void AbilityAnim_ReleaseGrenade()
	{
		CFGGrenade componentInChildren = RightHand.GetComponentInChildren<CFGGrenade>();
		if (componentInChildren == null)
		{
			Debug.LogError("Failed to find grenade!");
			AbilityAnim_MainAnimEnd();
		}
		else
		{
			componentInChildren.StartMoving();
		}
	}

	private bool AbilityAnim_SpawnGrenade()
	{
		return GetAbility(m_CurrentAction)?.SpawnGrenade() ?? false;
	}

	private void AbilityAnim_WeaponRemoveAnimEnd()
	{
		if (!m_CharacterAnimator.IsInIdle())
		{
			return;
		}
		if (m_AbilityAnim_HadWeapon)
		{
			AbilityAnim_OnWeaponRemove();
		}
		switch (m_AbilityAnim_Type)
		{
		case eAbilityAnimation.Consume:
			m_CharacterAnimator.PlayConsume(AbilityAnim_MainAnimEnd);
			break;
		case eAbilityAnimation.Throw1:
		case eAbilityAnimation.Throw2:
			AbilityAnim_SpawnGrenade();
			if (m_AbilityAnim_Type == eAbilityAnimation.Throw1)
			{
				m_CharacterAnimator.PlayThrow1(AbilityAnim_ReleaseGrenade);
			}
			else
			{
				m_CharacterAnimator.PlayThrow2(AbilityAnim_ReleaseGrenade);
			}
			break;
		}
		m_AbilityAnim_State = EAbilityAnimState.WaitingForAnimEnd;
	}

	private void AbilityAnim_WeaponSpawnAnimEnd()
	{
		if (m_CharacterAnimator.IsInIdle())
		{
			m_AbilityAnim_State = EAbilityAnimState.Stopped;
			m_AbilityAnim_Type = eAbilityAnimation.None;
			m_AbilityAnim_HadWeapon = false;
			GetAbility(m_CurrentAction)?.StartDelay();
		}
	}

	public void SetNextActionDelay()
	{
		m_NextOrderTime = Time.time + 0.5f;
	}

	public EActionResult CanMakeAction(ETurnAction action)
	{
		if (m_NextOrderTime > 0f)
		{
			if (Time.time < m_NextOrderTime)
			{
				return EActionResult.Busy;
			}
			m_NextOrderTime = -1f;
		}
		EActionResult eActionResult = EActionResult.Success;
		if (IsDead)
		{
			eActionResult |= EActionResult.Dead;
		}
		if (CurrentAction != ETurnAction.None || m_CurrentActionState != 0)
		{
			eActionResult |= EActionResult.Busy;
		}
		if (Imprisoned)
		{
			eActionResult |= EActionResult.Imprisoned;
		}
		if (eActionResult != 0)
		{
			return eActionResult;
		}
		if (action == ETurnAction.Move)
		{
			if (CFGSingleton<CFGGame>.Instance.IsInGame() && ActionPoints == 0)
			{
				return EActionResult.NotEnoughAP;
			}
		}
		else
		{
			int aPCostForAction = GetAPCostForAction(action, null);
			if (ActionPoints < aPCostForAction)
			{
				return EActionResult.NotEnoughAP;
			}
		}
		if (GunpointState == EGunpointState.Executor)
		{
			return EActionResult.Busy;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame() && m_CharacterAnimator == null)
		{
			return EActionResult.Failed;
		}
		switch (action)
		{
		case ETurnAction.SuicideShot:
			if (!HaveAbility(ETurnAction.SuicideShot))
			{
				return EActionResult.NoAbility;
			}
			return EActionResult.Success;
		case ETurnAction.ChangeWeapon:
			if (UnusedWeapon == null || UnusedWeapon.m_Definition == null)
			{
				return EActionResult.NoWeapon;
			}
			return EActionResult.Success;
		case ETurnAction.Shoot:
		case ETurnAction.Miss_Shoot:
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (!CurrentWeapon.CanShoot())
			{
				return EActionResult.NoAmmo;
			}
			return EActionResult.Success;
		case ETurnAction.AltFire_Fanning:
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (CurrentWeapon.CurrentAmmo < 2)
			{
				return EActionResult.NoAmmo;
			}
			if (CurrentWeapon.m_Definition == null)
			{
				return EActionResult.Failed;
			}
			if (!CurrentWeapon.m_Definition.AllowsFanning)
			{
				return EActionResult.NoAbility;
			}
			return EActionResult.Success;
		case ETurnAction.AltFire_ConeShot:
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (CurrentWeapon.CurrentAmmo < 1)
			{
				return EActionResult.NoAmmo;
			}
			if (CurrentWeapon.m_Definition == null)
			{
				return EActionResult.Failed;
			}
			if (!CurrentWeapon.m_Definition.AllowsCone)
			{
				return EActionResult.NoAbility;
			}
			return EActionResult.Success;
		case ETurnAction.AltFire_ScopedShot:
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (CurrentWeapon.CurrentAmmo < 1)
			{
				return EActionResult.NoAmmo;
			}
			if (CurrentWeapon.m_Definition == null)
			{
				return EActionResult.Failed;
			}
			if (!CurrentWeapon.m_Definition.AllowsScoped)
			{
				return EActionResult.NoAbility;
			}
			return EActionResult.Success;
		case ETurnAction.Ricochet:
			if (Luck < CFGDef_Ability.Ricochet_LuckCost)
			{
				return EActionResult.NotEnoughLuck;
			}
			if (!HaveAbility(ETurnAction.Ricochet))
			{
				return EActionResult.NoAbility;
			}
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (!CurrentWeapon.CanShoot())
			{
				return EActionResult.NoAmmo;
			}
			if (!CurrentWeapon.AllowsRicochet)
			{
				return EActionResult.NoAbility;
			}
			return EActionResult.Success;
		case ETurnAction.Gunpoint:
			if (!HaveAbility(ETurnAction.Gunpoint))
			{
				return EActionResult.NoAbility;
			}
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (!CurrentWeapon.CanShoot())
			{
				return EActionResult.NoAmmo;
			}
			if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				return EActionResult.NotInSetupStage;
			}
			return EActionResult.Success;
		case ETurnAction.Reload:
			if (CurrentWeapon == null)
			{
				return EActionResult.NoWeapon;
			}
			if (!CurrentWeapon.CanReload())
			{
				return EActionResult.InvalidTarget;
			}
			return EActionResult.Success;
		case ETurnAction.FocusCamera:
			return EActionResult.Success;
		case ETurnAction.Demon:
			if (m_CharData != null && m_CharData.HasBuff("demonpower"))
			{
				return EActionResult.Busy;
			}
			break;
		case ETurnAction.ShadowCloak:
			return EActionResult.Failed;
		case ETurnAction.Courage:
			if (m_VisibileEnemies.Count < 2)
			{
				return EActionResult.NoTargetInRange;
			}
			if (ActionPoints == 0)
			{
				return EActionResult.NotEnoughAP;
			}
			break;
		case ETurnAction.MultiShot:
			if (m_VisibileEnemies.Count < 1)
			{
				return EActionResult.NoTargetInRange;
			}
			break;
		case ETurnAction.Finder:
		case ETurnAction.ShadowKill:
		case ETurnAction.Cannibal:
		{
			CFGAbility ability = GetAbility(action);
			if (ability == null)
			{
				return EActionResult.NoAbility;
			}
			CFGIAttackable firstValidTarget = ability.GetFirstValidTarget();
			if (firstValidTarget == null)
			{
				return EActionResult.NoTargetInRange;
			}
			break;
		}
		case ETurnAction.Smell:
		case ETurnAction.RewardedKill:
			if (ActionPoints == 0)
			{
				return EActionResult.NotEnoughAP;
			}
			break;
		}
		if (action.IsStandard())
		{
			CFGAbility ability2 = GetAbility(action);
			if (ability2 == null)
			{
				return EActionResult.NoAbility;
			}
			if (ability2.NeedWeapon)
			{
				if (CurrentWeapon == null)
				{
					return EActionResult.NoWeapon;
				}
				if (!CurrentWeapon.CanShoot())
				{
					return EActionResult.NoAmmo;
				}
			}
			int luckCost = ability2.GetLuckCost();
			if (Luck - luckCost < 0)
			{
				eActionResult |= EActionResult.NotEnoughLuck;
			}
			if (!ability2.IsReadyToUse)
			{
				eActionResult |= EActionResult.OnCooldown;
			}
			return eActionResult;
		}
		return eActionResult;
	}

	public EActionResult CanMakeAction(ETurnAction action, params object[] args)
	{
		EActionResult eActionResult = CanMakeAction(action);
		if (eActionResult != 0)
		{
			return eActionResult;
		}
		if (ActionPoints < GetAPCostForAction(action, args))
		{
			return EActionResult.NotEnoughAP;
		}
		switch (action)
		{
		case ETurnAction.Move:
		{
			CFGCell cFGCell2 = (CFGCell)args[0];
			if (cFGCell2 == null || cFGCell2.CurrentCharacter != null || cFGCell2.StairsType == CFGCell.EStairsType.Slope)
			{
				return EActionResult.InvalidTarget;
			}
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				int distance = ((ActionPoints <= 1) ? HalfMovement : DashingMovement);
				if (!CFGCellDistanceFinder.FindCellsInDistance(CurrentCell, distance).Contains(cFGCell2))
				{
					return EActionResult.OutOfRange;
				}
			}
			NavigationComponent component = GetComponent<NavigationComponent>();
			if (component == null)
			{
				return EActionResult.Failed;
			}
			NavGoal_At navGoal_At = new NavGoal_At(cFGCell2);
			if (!component.GeneratePath(CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out m_Path))
			{
				return EActionResult.InvalidTarget;
			}
			return EActionResult.Success;
		}
		case ETurnAction.Miss_Shoot:
		{
			CFGCell cFGCell = (CFGCell)args[0];
			if (cFGCell != null)
			{
				return EActionResult.Success;
			}
			return EActionResult.InvalidTarget;
		}
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.ArteryShot:
		{
			CFGIAttackable cFGIAttackable3 = (CFGIAttackable)args[0];
			if (cFGIAttackable3 == null)
			{
				return EActionResult.InvalidTarget;
			}
			if (!cFGIAttackable3.IsAlive)
			{
				return EActionResult.TargetDead;
			}
			if (!HasLineOfFireTo(cFGIAttackable3, action))
			{
				return EActionResult.NoLineOfFire;
			}
			return EActionResult.Success;
		}
		case ETurnAction.Ricochet:
		{
			CFGIAttackable cFGIAttackable2 = (CFGIAttackable)args[0];
			if (cFGIAttackable2 == null)
			{
				return EActionResult.InvalidTarget;
			}
			if (!cFGIAttackable2.IsAlive)
			{
				return EActionResult.TargetDead;
			}
			List<CFGRicochetObject> list = (List<CFGRicochetObject>)args[1];
			if (list == null)
			{
				return EActionResult.InvalidTarget;
			}
			if (list.Count == 0)
			{
				return EActionResult.NoTargetInRange;
			}
			return EActionResult.Success;
		}
		case ETurnAction.Use:
		{
			CFGUsableObject cFGUsableObject = (CFGUsableObject)args[0];
			if (cFGUsableObject == null)
			{
				return EActionResult.InvalidTarget;
			}
			EUseMode Mode = EUseMode.CantUse;
			if (cFGUsableObject.CanCharacterUse(this, bCheckWithMovement: true, out Mode) != null && Mode != 0)
			{
				return EActionResult.Success;
			}
			return EActionResult.Failed;
		}
		case ETurnAction.Penetrate:
		{
			if (args == null || args.Length == 0)
			{
				return EActionResult.Failed;
			}
			CFGIAttackable cFGIAttackable = (CFGIAttackable)args[0];
			if (cFGIAttackable == null)
			{
				return EActionResult.Failed;
			}
			if (!cFGIAttackable.IsAlive)
			{
				return EActionResult.TargetDead;
			}
			break;
		}
		case ETurnAction.Gunpoint:
		{
			CFGCharacter cFGCharacter2 = (CFGCharacter)args[0];
			if (cFGCharacter2 == null)
			{
				return EActionResult.Failed;
			}
			if (cFGCharacter2.IsDead)
			{
				return EActionResult.TargetDead;
			}
			if (!IsEnemyVisible(cFGCharacter2))
			{
				return EActionResult.TargetNotVisible;
			}
			if (CFGCellMap.Distance(CurrentCell, cFGCharacter2.CurrentCell) > 5)
			{
				return EActionResult.OutOfRange;
			}
			if (cFGCharacter2.IsImmuneToGunpoint)
			{
				eActionResult = EActionResult.InvalidTarget;
			}
			CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
			if (aiOwner != null)
			{
				foreach (CFGCharacter character in aiOwner.Characters)
				{
					if (!(character == cFGCharacter2) && character.GunpointState != EGunpointState.Target)
					{
						Vector3 normalized = (base.Transform.position - character.Transform.position).normalized;
						float num = Vector3.Dot(character.Transform.forward, normalized);
						if (num > 0.5f && CFGCellMap.GetLineOfSightAutoSideSteps(this, character, null, null, 8) == ELOXHitType.None)
						{
							eActionResult |= EActionResult.NotInCone;
							break;
						}
					}
				}
			}
			return eActionResult;
		}
		case ETurnAction.Transfusion:
		{
			CFGCharacter cFGCharacter = (CFGCharacter)args[0];
			if (cFGCharacter == null)
			{
				return EActionResult.InvalidTarget;
			}
			if (cFGCharacter.IsDead)
			{
				return EActionResult.TargetDead;
			}
			if (CFGCellMap.GetLineOfSightAutoSideSteps(this, cFGCharacter) != 0)
			{
				return EActionResult.NoLineOfSight;
			}
			return EActionResult.Success;
		}
		}
		return EActionResult.Success;
	}

	public static CFGCharacter GetEnemyForFinder(CFGCharacter src)
	{
		if (src == null || src.Owner == null)
		{
			return null;
		}
		CFGOwner owner = src.Owner;
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (!(character == null) && !character.IsAlive && !character.IsCorpseLooted && (!(character.Owner != null) || !(character.Owner == owner)))
			{
				CFGCell cell = CFGCellMap.GetCell(character.Position);
				if (cell != null && cell == src.CurrentCell)
				{
					return character;
				}
			}
		}
		return null;
	}

	public int GetAPCost(ETurnAction action, out bool EndTurn)
	{
		int result = 2;
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			result = 1;
		}
		EndTurn = false;
		switch (action)
		{
		case ETurnAction.FocusCamera:
			return 0;
		case ETurnAction.AltFire_ConeShot:
			if ((bool)CurrentWeapon)
			{
				EndTurn = CurrentWeapon.ShotEndsTurn;
			}
			return 1;
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
			EndTurn = true;
			return result;
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
			if ((bool)CurrentWeapon)
			{
				EndTurn = CurrentWeapon.ShotEndsTurn;
			}
			return 1;
		case ETurnAction.Gunpoint:
			return ActionPoints;
		case ETurnAction.Reload:
			return 1;
		case ETurnAction.ChangeWeapon:
			return 0;
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
			return 1;
		default:
			if (action.IsStandard())
			{
				CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(action);
				if (abilityDef == null)
				{
					return 0;
				}
				switch (abilityDef.EndTurn)
				{
				case CFGDef_Ability.ETurnEndType.True:
					EndTurn = true;
					break;
				case CFGDef_Ability.ETurnEndType.Weapon:
					EndTurn = CurrentWeapon.ShotEndsTurn;
					break;
				}
				if (abilityDef.CostAP > 1)
				{
					return result;
				}
				return abilityDef.CostAP;
			}
			return 0;
		}
	}

	public int GetAPCostForAction(ETurnAction action, params object[] args)
	{
		int result = 2;
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			result = 1;
		}
		switch (action)
		{
		case ETurnAction.SuicideShot:
			return 0;
		case ETurnAction.Move:
		{
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				return 0;
			}
			CFGCell item = (CFGCell)args[0];
			if (CFGCellDistanceFinder.FindCellsInDistance(CurrentCell, HalfMovement).Contains(item))
			{
				return 1;
			}
			if (ActionPoints > 1 && CFGCellDistanceFinder.FindCellsInDistance(CurrentCell, DashingMovement).Contains(item))
			{
				return 2;
			}
			break;
		}
		case ETurnAction.Use:
			return 0;
		case ETurnAction.AltFire_ConeShot:
			if (CurrentWeapon == null)
			{
				return 4;
			}
			return (!CurrentWeapon.ShotEndsTurn) ? 1 : Mathf.Max(ActionPoints, 1);
		case ETurnAction.AltFire_Fanning:
			return result;
		case ETurnAction.AltFire_ScopedShot:
			return result;
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
			if (CurrentWeapon == null)
			{
				return 4;
			}
			return (!CurrentWeapon.ShotEndsTurn) ? 1 : Mathf.Max(ActionPoints, 1);
		case ETurnAction.Gunpoint:
			return Mathf.Max(ActionPoints, 1);
		case ETurnAction.End:
			return (args == null || args.Length <= 0) ? ActionPoints : ((int)args[0]);
		case ETurnAction.Reload:
			return 1;
		case ETurnAction.ChangeWeapon:
			return 0;
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
			return 1;
		case ETurnAction.FocusCamera:
			return 0;
		}
		if (action.IsStandard())
		{
			CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(action);
			if (abilityDef == null)
			{
				return 0;
			}
			if (abilityDef.CostAP > 1)
			{
				return result;
			}
			if (abilityDef.CostAP == 1)
			{
				if (abilityDef.EndTurn == CFGDef_Ability.ETurnEndType.Weapon && (bool)CurrentWeapon && CurrentWeapon.ShotEndsTurn)
				{
					return Mathf.Max(ActionPoints, 1);
				}
				if (abilityDef.EndTurn == CFGDef_Ability.ETurnEndType.True)
				{
					return Mathf.Max(ActionPoints, 1);
				}
				return 1;
			}
		}
		return 0;
	}

	public static bool ShouldCancelOrdering(ETurnAction action)
	{
		if (action == ETurnAction.ChangeWeapon)
		{
			return false;
		}
		return true;
	}

	private EBestCoverRotation GetBestRotationForAction(ETurnAction action, params object[] args)
	{
		if (m_CharacterAnimator == null || m_CharacterAnimator.CurrentCoverState == 0)
		{
			return EBestCoverRotation.Auto;
		}
		switch (action)
		{
		default:
			return EBestCoverRotation.Auto;
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
		case ETurnAction.MultiShot:
		{
			ECoverType eCoverType = ECoverType.NONE;
			ECoverType eCoverType2 = ECoverType.NONE;
			EDirection eDirection = EDirection.EAST;
			EDirection eDirection2 = EDirection.EAST;
			if (args == null || args.Length == 0)
			{
				return EBestCoverRotation.Auto;
			}
			Vector3 zero = Vector3.zero;
			CFGCell cFGCell = null;
			if (!(args[0] is CFGIAttackable { Position: var vector, CurrentCell: var cFGCell2 } cFGIAttackable))
			{
				return EBestCoverRotation.Auto;
			}
			if (action == ETurnAction.Ricochet && args.Length > 1)
			{
				List<CFGRicochetObject> list = (List<CFGRicochetObject>)args[1];
				if (list != null && list.Count > 0)
				{
					cFGCell2 = list[0].Cell;
					if (cFGCell2 != null)
					{
						vector = cFGCell2.WorldPosition;
					}
					else
					{
						cFGCell2 = cFGIAttackable.CurrentCell;
					}
				}
			}
			Vector3 normalized = (vector - Position).normalized;
			normalized.y = 0f;
			float num = Vector3.Dot(normalized, new Vector3(-1f, 0f, 0f));
			float num2 = Vector3.Dot(normalized, new Vector3(0f, 0f, 1f));
			float num3 = Mathf.Abs(num2);
			float num4 = Mathf.Abs(num);
			float num5 = Mathf.Abs(num3 - num4);
			if (num5 < 0.3f)
			{
				eDirection = EDirection.EAST;
				if (num2 < 0f)
				{
					eDirection = EDirection.WEST;
				}
				eCoverType = CurrentCell.GetBorderCover(eDirection);
				eDirection2 = EDirection.SOUTH;
				if (num > 0f)
				{
					eDirection2 = EDirection.NORTH;
				}
				eCoverType2 = CurrentCell.GetBorderCover(eDirection2);
			}
			else
			{
				eDirection = ((num3 > num4) ? ((!(num2 > 0f)) ? EDirection.WEST : EDirection.EAST) : ((!(num > 0f)) ? EDirection.SOUTH : EDirection.NORTH));
				eCoverType = CurrentCell.GetBorderCover(eDirection);
			}
			if (eCoverType == ECoverType.FULL)
			{
				EBestCoverRotation rotationForPosition = GetRotationForPosition(cFGCell2.WorldPosition, eDirection);
				if (rotationForPosition != 0)
				{
					m_BestCoverDirection = eDirection.Opposite();
					return rotationForPosition;
				}
			}
			if (eCoverType2 == ECoverType.FULL)
			{
				EBestCoverRotation rotationForPosition2 = GetRotationForPosition(cFGCell2.WorldPosition, eDirection2);
				if (rotationForPosition2 != 0)
				{
					m_BestCoverDirection = eDirection2.Opposite();
					return rotationForPosition2;
				}
			}
			if (eCoverType == ECoverType.HALF)
			{
				if (eCoverType2 != ECoverType.FULL)
				{
					return EBestCoverRotation.Auto;
				}
				m_BestCoverDirection = eDirection;
				return EBestCoverRotation.Forward;
			}
			if (eCoverType2 == ECoverType.HALF)
			{
				if (eCoverType != ECoverType.FULL)
				{
					return EBestCoverRotation.Auto;
				}
				m_BestCoverDirection = eDirection2;
				return EBestCoverRotation.Forward;
			}
			return EBestCoverRotation.Auto;
		}
		}
	}

	public EActionResult MakeAction(ETurnAction action, params object[] args)
	{
		EActionResult eActionResult = CanMakeAction(action, args);
		if (eActionResult != 0)
		{
			return eActionResult;
		}
		CurrentAction = action;
		m_CurrentActionState = EActionState.Preparing;
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && CurrentAction.CanCharacterBeGluedinSetupStage())
		{
			CurrentCell.GetBestCoverToGlue(this, out var cover_type, out var _);
			if (cover_type == 2 || cover_type == 3)
			{
				m_CharacterAnimator.CurrentCoverState = cover_type;
			}
		}
		EBestCoverRotation bestRotationForAction = GetBestRotationForAction(action, args);
		if (bestRotationForAction != 0)
		{
			m_BestCoverRotation = bestRotationForAction;
			m_CurrentActionState = EActionState.Rotating;
			ProcessFacingToCover(instant_change: true);
		}
		if (action != ETurnAction.Demon)
		{
			SpendActionPoints(GetAPCostForAction(action, args));
			if (action == ETurnAction.Ricochet)
			{
				Luck -= CFGDef_Ability.Ricochet_LuckCost;
			}
		}
		bool EndTurn = false;
		GetAPCost(action, out EndTurn);
		m_CharData.SetState(ECharacterStateFlag.TurnFinishedAndLocked, EndTurn);
		OnPreAction(action, args);
		if ((bool)Owner && Owner.IsPlayer && CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
			if ((bool)component)
			{
				component.SetLock(ELockReason.MakeAction, bLock: true, ShouldCancelOrdering(action));
			}
		}
		m_IsWaitingForCameraFocus = ShouldWaitForCameraFocus(action);
		CFGCamera component2 = Camera.main.GetComponent<CFGCamera>();
		CFGCharacter cFGCharacter = m_ActionTarget as CFGCharacter;
		if (action != ETurnAction.End && action != ETurnAction.ChangeWeapon && component2 != null && (IsVisibleByPlayer() || (cFGCharacter != null && cFGCharacter.IsVisibleByPlayer()) || CFGCheats.AllCharactersVisible))
		{
			if (m_ActionTarget != null)
			{
				component2.ChangeFocus(base.Transform.position, m_ActionTarget.Transform.position, 0.5f, force: true, OnCameraFocusEnd);
			}
			else if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				component2.ChangeFocus(this, 0.5f, force: true, OnCameraFocusEnd);
			}
			else
			{
				CFGCameraFollowInfo strategicHorsemanFollowInfo = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_StrategicHorsemanFollowInfo;
				if (CFGOptions.Gameplay.PawnSuperSpeed)
				{
					strategicHorsemanFollowInfo.m_MaxSpeed = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_HorsemanFastMaxSpeed;
					strategicHorsemanFollowInfo.m_Acceleration = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_HorsemanFastAcc;
					strategicHorsemanFollowInfo.m_Distance = 0.05f;
				}
				component2.ChangeFocus(this, 0.5f, strategicHorsemanFollowInfo, force: true, OnCameraFocusEnd);
			}
		}
		else
		{
			OnCameraFocusEnd();
		}
		m_IsWaitingForFacingTarget = ShouldWaitForFacingTarget(action);
		if (action != 0 && !action.IsSilent(this))
		{
			LeaveShadowCloak(null);
		}
		if ((Owner == null || Owner.IsAi) && CurrentAction != 0 && CurrentAction != ETurnAction.Shoot && CurrentAction != ETurnAction.AltFire_ConeShot && CurrentAction != ETurnAction.AltFire_Fanning && CurrentAction != ETurnAction.AltFire_ScopedShot && CurrentAction != ETurnAction.Reload && CurrentAction != ETurnAction.End)
		{
			m_FlagNeedFlash = true;
		}
		switch (action)
		{
		case ETurnAction.Miss_Shoot:
			RotateToward(m_MissShootCell.WorldPosition);
			break;
		case ETurnAction.Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
		case ETurnAction.MultiShot:
			if (m_ActionTarget != null)
			{
				RotateToward(m_ActionTarget.Transform.position);
				m_IsWaitingForFacingTarget = true;
			}
			break;
		case ETurnAction.Ricochet:
			RotateToward(m_TempRicoList[0].Transform.position);
			break;
		case ETurnAction.OpenDoor:
		{
			Vector3 position3 = m_ActionTarget.Transform.position;
			position3.y = base.Transform.position.y;
			m_CharacterAnimator.WantedDirection = (position3 - base.Transform.position).normalized;
			break;
		}
		case ETurnAction.Use:
		{
			Vector3 vector = m_ActionTarget.Transform.position;
			CFGUsableObject cFGUsableObject = m_ActionTarget as CFGUsableObject;
			if ((bool)cFGUsableObject)
			{
				vector = cFGUsableObject.GetTargetForCharacterRotation(this);
			}
			vector.y = base.Transform.position.y;
			m_CharacterAnimator.WantedDirection = (vector - base.Transform.position).normalized;
			break;
		}
		case ETurnAction.Gunpoint:
		{
			Vector3 position2 = m_ActionTarget.Transform.position;
			position2.y = base.Transform.position.y;
			m_CharacterAnimator.WantedDirection = (position2 - base.Transform.position).normalized;
			break;
		}
		default:
		{
			if (!action.IsStdNonSpecial() || args == null)
			{
				break;
			}
			CFGAbility ability = GetAbility(action);
			if (ability != null && ability.RotateTowardEnemy)
			{
				if (args[0] is CFGIAttackable { CurrentCell: not null } cFGIAttackable)
				{
					RotateToward(cFGIAttackable.CurrentCell.WorldPosition);
					m_IsWaitingForFacingTarget = true;
				}
				else if (args.Length > 1 && args[1] is CFGCell cFGCell && cFGCell != CurrentCell)
				{
					RotateToward(cFGCell.WorldPosition);
					Vector3 position = Position;
					position.y = cFGCell.WorldPosition.y;
					m_CharacterAnimator.WantedDirection = (cFGCell.WorldPosition - position).normalized;
					m_IsWaitingForFacingTarget = true;
				}
			}
			break;
		}
		}
		m_IsWaitingForWeaponEquipped = CurrentWeapon != null && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && NeedWeaponEquipped(action);
		if (m_IsWaitingForWeaponEquipped)
		{
			if (CurrentWeapon.TwoHanded)
			{
				m_CharacterAnimator.PlayWeaponChange2(null, OnAnimWeaponEquipped, OnAnimWeaponEquippedEnd);
			}
			else
			{
				m_CharacterAnimator.PlayWeaponChange1(null, OnAnimWeaponEquipped, OnAnimWeaponEquippedEnd);
			}
		}
		return eActionResult;
	}

	public void RotateToward(Vector3 _Position)
	{
		_Position.y = base.Transform.position.y;
		Vector3 normalized = (_Position - base.Transform.position).normalized;
		float num = CFGMath.CalcHorizontalAngle(base.Transform.forward, normalized);
		if (m_CharacterAnimator.CurrentCoverState == 0 || m_BestCoverRotation == EBestCoverRotation.Forward || (m_CharacterAnimator.CurrentCoverState == 1 && Mathf.Abs(num) >= 90f) || (m_CharacterAnimator.CurrentCoverState == 2 && num >= -135f && num <= 90f) || (m_CharacterAnimator.CurrentCoverState == 3 && num >= -90f && num <= 135f))
		{
			if (m_CharacterAnimator.CurrentCoverState != 0 && (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || CurrentAction.CanCharacterBeGluedinSetupStage()) && CurrentAction != ETurnAction.None)
			{
				m_CharacterAnimator.ForceNoCoverTemporarily = true;
			}
			m_CharacterAnimator.WantedDirection = normalized;
			m_CharacterAnimator.SetDirectionDiff(0f);
			return;
		}
		float directionDiff = num;
		if (m_CharacterAnimator.CurrentCoverState == 2 || m_CharacterAnimator.CurrentCoverState == 3)
		{
			Vector3 vector = ((m_CharacterAnimator.CurrentCoverState != 2) ? (base.Transform.position - base.Transform.right) : (base.Transform.position + base.Transform.right));
			_Position.y = vector.y;
			Vector3 normalized2 = (_Position - vector).normalized;
			directionDiff = CFGMath.CalcHorizontalAngle(base.Transform.forward, normalized2);
		}
		m_CharacterAnimator.SetDirectionDiff(directionDiff);
	}

	private static bool ShouldWaitForCameraFocus(ETurnAction action)
	{
		switch (action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.Use:
		case ETurnAction.OpenDoor:
		case ETurnAction.Gunpoint:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
		case ETurnAction.FocusCamera:
			return true;
		default:
			if (action.IsStandard())
			{
				CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(action);
				if (abilityDef != null)
				{
					return abilityDef.WaitForCameraFocus;
				}
			}
			return false;
		}
	}

	private static bool ShouldWaitForFacingTarget(ETurnAction action)
	{
		switch (action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.Use:
		case ETurnAction.OpenDoor:
		case ETurnAction.Gunpoint:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			return true;
		default:
			if (action.IsStandard())
			{
				CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(action);
				if (abilityDef != null)
				{
					return abilityDef.FaceTarget;
				}
			}
			return false;
		}
	}

	private static bool NeedWeaponEquipped(ETurnAction action)
	{
		switch (action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			return true;
		default:
			if (action == ETurnAction.MultiShot)
			{
				CFGCharacter selectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
				if (selectedCharacter != null && selectedCharacter.CurrentWeapon != null && selectedCharacter.CurrentWeapon.Visualisation == null && selectedCharacter.CurrentWeapon.TwoHanded)
				{
					return false;
				}
			}
			if (action == ETurnAction.Gunpoint)
			{
				CFGCharacter selectedCharacter2 = CFGSelectionManager.Instance.SelectedCharacter;
				if (selectedCharacter2 == null)
				{
					return true;
				}
				CFGCharacter cFGCharacter = selectedCharacter2.m_ActionTarget as CFGCharacter;
				if (cFGCharacter != null && cFGCharacter.GunpointState == EGunpointState.Target)
				{
					return false;
				}
				return true;
			}
			if (action.IsStandard())
			{
				CFGDef_Ability abilityDef = CFGStaticDataContainer.GetAbilityDef(action);
				if (abilityDef != null)
				{
					return abilityDef.NeedWeapon;
				}
			}
			return false;
		}
	}

	private bool ShouldEndCurrentAction()
	{
		switch (CurrentAction)
		{
		case ETurnAction.Move:
			return ShouldFinishMove();
		case ETurnAction.FocusCamera:
			if (Time.time > m_FocusCameraEnd)
			{
				return true;
			}
			return false;
		default:
			if (CurrentAction.IsStdNonSpecial())
			{
				CFGAbility ability = GetAbility(CurrentAction);
				if (ability != null)
				{
					return ability.ShouldFinishAction();
				}
			}
			return false;
		}
	}

	private void UpdateCurrentAction()
	{
		if (CurrentAction == ETurnAction.None != (m_CurrentActionState == EActionState.None))
		{
			Debug.LogError(string.Concat("ERROR! Major fuckup in abilities system, report to SauRooN. Current Action = ", CurrentAction, " State = ", m_CurrentActionState));
		}
		else
		{
			if (CurrentAction == ETurnAction.None)
			{
				return;
			}
			switch (m_CurrentActionState)
			{
			case EActionState.Rotating:
				if (m_CharacterAnimator != null)
				{
					if (m_CharacterAnimator.IsInIdle())
					{
						m_CurrentActionState = EActionState.Preparing;
					}
				}
				else
				{
					m_CurrentActionState = EActionState.Preparing;
				}
				break;
			case EActionState.Preparing:
			{
				bool flag = IsCharacterReady();
				if (!m_IsWaitingForCameraFocus && !m_IsWaitingForFacingTarget && !m_IsWaitingForWeaponEquipped && flag)
				{
					m_CurrentActionState = EActionState.Doing;
					OnDoAction();
				}
				if (m_IsWaitingForFacingTarget && (m_CharacterAnimator == null || m_CharacterAnimator.IsFacingRightDirection()))
				{
					m_IsWaitingForFacingTarget = false;
				}
				break;
			}
			case EActionState.Doing:
				UpdateDoingCurrentAction();
				if (m_AbilityAnim_State != 0)
				{
					AbilityAnim_Update();
				}
				else if (ShouldEndCurrentAction())
				{
					m_CurrentActionState = EActionState.Ending;
					FinishCurrentAction();
				}
				break;
			case EActionState.Ending:
				FinishCurrentAction();
				break;
			}
		}
	}

	private void UpdateDoingCurrentAction()
	{
		if (m_CurrentActionState != EActionState.Doing)
		{
			return;
		}
		ETurnAction currentAction = CurrentAction;
		if (currentAction != ETurnAction.Transfusion)
		{
			return;
		}
		CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashCharacterBig(_yellowFlash: false);
		CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashCharacterSmallCurrent(_yellowFlash: false);
		CFGCharacter cFGCharacter = m_ActionTarget as CFGCharacter;
		if (!(cFGCharacter != null) || !(cFGCharacter.Owner != null) || !cFGCharacter.Owner.IsPlayer)
		{
			return;
		}
		List<CFGCharacterData> teamCharactersList = CFGCharacterList.GetTeamCharactersList();
		for (int i = 0; i < teamCharactersList.Count; i++)
		{
			if (teamCharactersList[i].Definition.NameID == cFGCharacter.NameId)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_HUD.FlashCharacterSmall(i, _yellowFlash: false);
				break;
			}
		}
	}

	public void ChangeMovement(CFGCell Target)
	{
		if (m_CurrentAction != 0 || Target == null)
		{
			return;
		}
		NavGoal_At navGoal_At = new NavGoal_At(Target);
		if (navGoal_At != null)
		{
			NavigationComponent component = GetComponent<NavigationComponent>();
			if (component.GeneratePath(CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out m_Path) && m_Steering != null)
			{
				m_Steering.InitPath(m_Path, ShouldRun());
				m_Steering.m_SteerData.m_IsMoving = true;
			}
		}
	}

	private bool ShouldRun()
	{
		if (CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner == null || CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters == null)
		{
			return true;
		}
		if (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
		{
			return true;
		}
		float num = 10f;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage != null)
		{
			num = Mathf.Clamp(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SetupStage.MoveDectectionDist, 1f, 100f);
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters)
		{
			if (!(character == null) && !character.IsDead && character.GunpointState != EGunpointState.Target && CFGCellMap.GetLineOfSightAutoSideSteps(character, this) == ELOXHitType.None)
			{
				float num2 = Vector3.Distance(character.Position, Position);
				if (num2 < num)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void OnCameraFocusEnd()
	{
		m_IsWaitingForCameraFocus = false;
	}

	public void OnEndAction()
	{
		m_CurrentActionState = EActionState.Ending;
	}

	private void FinishCurrentAction()
	{
		OnPostAction();
		if (m_OnCharacterActionFinishedCallback != null)
		{
			m_OnCharacterActionFinishedCallback(this, CurrentAction);
		}
		int num = 0;
		bool flag = false;
		switch (m_CurrentAction)
		{
		case ETurnAction.Demon:
			flag = true;
			break;
		case ETurnAction.Move:
			num = 1;
			UpdateIntimidate();
			UpdateShadowCloaks();
			FinalizeMovementActions();
			if (Owner == null || Owner.IsAi || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
			{
				break;
			}
			foreach (CFGCharacter enemy in Owner.GetEnemys())
			{
				if (enemy.IsAlive && (enemy.VisibilityState & EBestDetectionType.ShadowSpotted) == EBestDetectionType.ShadowSpotted && (enemy.VisibilityState & EBestDetectionType.Visible) != EBestDetectionType.Visible)
				{
					enemy.OnShadowSpottedReport = enemy.ReportShadowSpotted;
				}
			}
			break;
		case ETurnAction.MultiShot:
			m_MultiShotCounter = 0;
			if (CurrentWeapon != null && CurrentWeapon.TwoHanded && CharacterAnimator != null)
			{
				CharacterAnimator.PlayWeaponChange2(null, OnAnimWeaponEquipped);
			}
			break;
		default:
			num = GetAPCostForAction(m_CurrentAction, null);
			break;
		}
		if (m_Steering != null)
		{
			m_Steering.FinishPath();
		}
		if (num > 0)
		{
			ApplyJinx();
		}
		CFGUsableObject cFGUsableObject = null;
		CFGDoorObject cFGDoorObject = null;
		if (m_CurrentAction == ETurnAction.Move && m_ActionTarget != null)
		{
			cFGUsableObject = m_ActionTarget as CFGUsableObject;
			cFGDoorObject = m_ActionTarget as CFGDoorObject;
		}
		ETurnAction currentAction = CurrentAction;
		CurrentAction = ETurnAction.None;
		m_CurrentActionState = EActionState.None;
		if (CurrentAction != ETurnAction.Gunpoint)
		{
			m_ActionTarget = null;
		}
		if (flag)
		{
			SwitchToDemon();
		}
		else if (cFGUsableObject == null)
		{
			CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
			if (component != null)
			{
				if ((bool)Owner && Owner.IsPlayer)
				{
					component.SetLock(ELockReason.MakeAction, bLock: false, ShouldCancelOrdering(currentAction));
				}
				component.OnCharacterActionFinished(this, currentAction);
			}
		}
		if (m_CharacterAnimator != null)
		{
			m_CharacterAnimator.ForceNoCoverTemporarily = false;
		}
		m_AOETargets = null;
		m_MissShootCell = null;
		if (cFGUsableObject != null)
		{
			cFGUsableObject.TryUse(this);
			Invoke("OnFailedToUseAfterMovement", 1.2f);
		}
		else if ((bool)cFGDoorObject)
		{
			cFGDoorObject.Open(this);
			Invoke("OnFailedToUseAfterMovement", 1.2f);
		}
	}

	private void OnFailedToUseAfterMovement()
	{
		if (CurrentAction != ETurnAction.None)
		{
			return;
		}
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component != null)
		{
			if ((bool)Owner && Owner.IsPlayer)
			{
				component.SetLock(ELockReason.MakeAction, bLock: false, ShouldCancelOrdering(ETurnAction.Use));
			}
			component.OnCharacterActionFinished(this, ETurnAction.Use);
		}
	}

	private void OnPreAction(ETurnAction action, params object[] args)
	{
		switch (action)
		{
		case ETurnAction.FocusCamera:
			if (args != null && args.Length > 0)
			{
				m_FocusCameraEnd = (float)args[0] + Time.time;
			}
			else
			{
				m_FocusCameraEnd = Time.time + 2f;
			}
			break;
		case ETurnAction.Move:
		{
			CFGCell atTile = (CFGCell)args[0];
			CFGGameObject actionTarget = null;
			if (args.Length > 1)
			{
				actionTarget = args[1] as CFGGameObject;
			}
			m_ActionTarget = actionTarget;
			NavGoal_At navGoal_At = new NavGoal_At(atTile);
			NavigationComponent component = GetComponent<NavigationComponent>();
			component.GeneratePath(CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out m_Path);
			if (m_Steering != null)
			{
				m_Steering.m_Character = this;
				m_Steering.m_SteerData.m_IsMoving = true;
				m_Steering.InitPath(m_Path, ShouldRun());
			}
			break;
		}
		case ETurnAction.Miss_Shoot:
			m_MissShootCell = (CFGCell)args[0];
			break;
		case ETurnAction.AltFire_ScopedShot:
			m_ActionTarget = (CFGGameObject)args[0];
			m_AOETargets = null;
			break;
		case ETurnAction.MultiShot:
			PrepareMultiShot((List<CFGIAttackable>)args[2]);
			break;
		case ETurnAction.AltFire_ConeShot:
			m_ActionTarget = (CFGGameObject)args[0];
			m_AOETargets = null;
			if (args.Length > 1 && args[1] != null)
			{
				List<CFGIAttackable> list = (List<CFGIAttackable>)args[1];
				if (list != null)
				{
					m_AOETargets = new List<CFGIAttackable>(list);
					if (m_ActionTarget is CFGIAttackable item && m_AOETargets.Contains(item))
					{
						m_AOETargets.Remove(item);
					}
				}
			}
			if (m_AOETargets == null && m_CurrentWeapon != null && m_CurrentWeapon.m_Definition != null && m_CurrentWeapon.m_Definition.ConeAngle > 1f)
			{
				GenerateAOETargets();
			}
			break;
		case ETurnAction.AltFire_Fanning:
			m_ActionTarget = (CFGGameObject)args[0];
			m_AOETargets = null;
			break;
		case ETurnAction.Transfusion:
			GetAbility(action)?.SetUpTargets((CFGIAttackable)args[0], (CFGCell)args[1], (List<CFGIAttackable>)args[2]);
			m_ActionTarget = (CFGGameObject)args[0];
			m_AOETargets = null;
			break;
		case ETurnAction.Shoot:
		case ETurnAction.Use:
		case ETurnAction.OpenDoor:
		case ETurnAction.Gunpoint:
			m_ActionTarget = (CFGGameObject)args[0];
			m_AOETargets = null;
			break;
		case ETurnAction.Ricochet:
		{
			m_ActionTarget = (CFGGameObject)args[0];
			List<CFGRicochetObject> collection = (List<CFGRicochetObject>)args[1];
			m_TempRicoList = new List<CFGRicochetObject>(collection);
			break;
		}
		default:
			if (args != null && args.Length > 0)
			{
				m_ActionTarget = args[0] as CFGGameObject;
			}
			if (action.IsStdNonSpecial())
			{
				GetAbility(action)?.SetUpTargets((CFGIAttackable)args[0], (CFGCell)args[1], (List<CFGIAttackable>)args[2]);
			}
			break;
		}
	}

	private void OnDoAction()
	{
		switch (CurrentAction)
		{
		case ETurnAction.SuicideShot:
			if (m_OnCharacterSuicide != null)
			{
				m_OnCharacterSuicide(this);
			}
			else
			{
				Debug.LogError("Character: Suicide - no callback present!");
			}
			OnEndAction();
			break;
		case ETurnAction.End:
			OnEndAction();
			break;
		case ETurnAction.Miss_Shoot:
			m_CharacterAnimator.PlayShoot(OnMissShoot, OnEndAction);
			break;
		case ETurnAction.Shoot:
			m_CharacterAnimator.PlayShoot(OnAnimFire, OnEndAction);
			break;
		case ETurnAction.AltFire_ConeShot:
			m_CharacterAnimator.PlayShootCone(OnAnimFire_Cone, OnEndAction);
			break;
		case ETurnAction.AltFire_Fanning:
			m_CharacterAnimator.PlayShootFanning(OnAnimFire_Fanning, OnEndAction);
			break;
		case ETurnAction.AltFire_ScopedShot:
			m_CharacterAnimator.PlayShootScoped(OnAnimFire_Scoped, OnEndAction);
			break;
		case ETurnAction.Ricochet:
			m_CharacterAnimator.PlayShoot(OnAnimRicochetFire, OnEndAction);
			break;
		case ETurnAction.Reload:
			m_CharacterAnimator.PlayReload(OnEndAction);
			if (CurrentWeapon.Visualisation != null)
			{
				CFGSoundDef.Play(CurrentWeapon.Visualisation.m_ReloadSoundDef, base.Transform.position);
			}
			break;
		case ETurnAction.ChangeWeapon:
			CFGAudioManager.Instance.PlaySound2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_UIChangeWeapon, CFGAudioManager.Instance.m_MixInterface);
			if (ShouldEquipWeapon())
			{
				if (UnusedWeapon != null)
				{
					if (UnusedWeapon.TwoHanded)
					{
						m_CharacterAnimator.PlayWeaponChange2(OnAnimWeaponChangeHolster, OnAnimWeaponChangeUnholster, OnEndAction);
					}
					else
					{
						m_CharacterAnimator.PlayWeaponChange1(OnAnimWeaponChangeHolster, OnAnimWeaponChangeUnholster, OnEndAction);
					}
				}
				else
				{
					m_CharacterAnimator.PlayWeaponChange0(OnAnimWeaponChangeHolster, OnEndAction);
				}
			}
			else
			{
				OnAnimWeaponChangeHolster();
				OnAnimWeaponChangeUnholster();
				OnEndAction();
			}
			break;
		case ETurnAction.Use:
		{
			CFGUsableObject component = m_ActionTarget.GetComponent<CFGUsableObject>();
			if (!(component == null))
			{
				if (component.IsDynamicCover)
				{
					m_CharacterAnimator.PlayCreateCover(component.DynamicCoverType, OnEndAction);
				}
				else
				{
					m_CharacterAnimator.PlayUse(OnEndAction);
				}
				component.DoUse(this);
			}
			break;
		}
		case ETurnAction.Gunpoint:
		{
			Vector3 position = base.Transform.position;
			position.y += 1.9f;
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_handsup") + "</color>", plus: false, 1, -1, this);
			GunpointState = EGunpointState.Executor;
			CFGCharacter cFGCharacter = m_ActionTarget as CFGCharacter;
			cFGCharacter.GunpointState = EGunpointState.Target;
			cFGCharacter.EnterSubduedState(this);
			Invoke("OnEndAction", 2f);
			break;
		}
		case ETurnAction.OpenDoor:
		{
			CFGDoorObject cFGDoorObject = m_ActionTarget as CFGDoorObject;
			if (cFGDoorObject != null)
			{
				cFGDoorObject.Open(this);
			}
			OnEndAction();
			break;
		}
		}
		if (CurrentAction.IsStdNonSpecial())
		{
			bool flag = false;
			CFGAbility ability = GetAbility(CurrentAction);
			if (ability != null)
			{
				flag = ability.Use();
			}
			if (!flag)
			{
				OnEndAction();
			}
		}
	}

	private void OnPostAction()
	{
		if ((bool)Owner && Owner.IsPlayer)
		{
			CheckEnemiesSuspicion(CurrentAction);
		}
		CFGAbility ability = GetAbility(m_CurrentAction);
		if (ability != null && ability.RotateTowardEnemy)
		{
			ProcessFacingToCover();
		}
		switch (CurrentAction)
		{
		case ETurnAction.Move:
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				UpdateSteering();
			}
			if (CurrentCell != null)
			{
				CurrentCell.CharacterMoveEnd(this);
			}
			ProcessFacingToCover();
			CFGObjectManager.ProcessFacingToCoverAllCharacters();
			break;
		case ETurnAction.Shoot:
		case ETurnAction.Use:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			ProcessFacingToCover();
			break;
		case ETurnAction.Transfusion:
		{
			m_FlagNeedFlashing = false;
			CFGCharacter cFGCharacter = m_ActionTarget as CFGCharacter;
			if ((bool)cFGCharacter)
			{
				cFGCharacter.m_FlagNeedFlashing = false;
			}
			break;
		}
		case ETurnAction.Reload:
			if ((bool)CurrentWeapon)
			{
				CurrentWeapon.Reload();
			}
			break;
		}
		CFGCharacter cFGCharacter2 = m_ActionTarget as CFGCharacter;
		if (cFGCharacter2 != null)
		{
			cFGCharacter2.TargetForAction = ETurnAction.None;
		}
	}

	private void OnAnimRicochetFire()
	{
		if (m_TempRicoList != null)
		{
			CFGCell cell = m_TempRicoList[m_TempRicoList.Count - 1].Cell;
			CFGIAttackable target = m_ActionTarget as CFGIAttackable;
			int damage = CalcDamage(target, ETurnAction.Shoot, cell);
			CurrentWeapon.Shoot(GetFinalChanceToHit(target, m_TempRicoList, null, null, ETurnAction.Ricochet), damage, this, target, null, m_TempRicoList);
			m_TempRicoList = null;
		}
	}

	private void OnMissShoot()
	{
		if (m_MissShootCell != null)
		{
			CurrentWeapon.Shoot(0, 0, this, null, null);
		}
	}

	private void OnAnimFire()
	{
		CFGIAttackable target = m_ActionTarget as CFGIAttackable;
		CurrentWeapon.Shoot(GetFinalChanceToHit(target, null, null, null, m_CurrentAction), CalcDamage(target, m_CurrentAction), this, target, null);
	}

	private void OnAnimFire_Fanning()
	{
		CFGIAttackable target = m_ActionTarget as CFGIAttackable;
		int finalChanceToHit = GetFinalChanceToHit(target, null, null, null, ETurnAction.AltFire_Fanning);
		CurrentWeapon.Shoot(finalChanceToHit, CalcDamage(target, ETurnAction.AltFire_Fanning), this, target, null);
	}

	private void OnAnimFire_Scoped()
	{
		CFGIAttackable target = m_ActionTarget as CFGIAttackable;
		CurrentWeapon.Shoot(100, CalcDamage(target, ETurnAction.AltFire_ScopedShot), this, target, null);
	}

	private void OnAnimFire_Cone()
	{
		if (m_AOETargets != null && m_AOETargets.Count > 0)
		{
			foreach (CFGIAttackable aOETarget in m_AOETargets)
			{
				CurrentWeapon.Shoot(GetFinalChanceToHit(aOETarget, null, null, null, ETurnAction.AltFire_ConeShot), CalcDamage(aOETarget, ETurnAction.AltFire_ConeShot), this, aOETarget, null, null, bSupportShot: true);
			}
		}
		CFGIAttackable target = m_ActionTarget as CFGIAttackable;
		CurrentWeapon.Shoot(GetFinalChanceToHit(target, null, null, null, ETurnAction.AltFire_ConeShot), CalcDamage(target, ETurnAction.AltFire_ConeShot), this, target, null);
	}

	private void OnAnimWeaponChangeHolster()
	{
		if (CurrentWeapon != null)
		{
			CurrentWeapon.RemoveVisualisation();
		}
	}

	private void OnAnimWeaponChangeUnholster()
	{
		if (m_CharData == null)
		{
			return;
		}
		m_CharData.SwapWeapons();
		CurrentWeapon = FirstWeapon;
		if ((bool)CurrentWeapon)
		{
			if (m_CharData != null)
			{
				m_CharData.SetWeaponBuff(CurrentWeapon);
			}
			if (CFGSingleton<CFGGame>.Instance.IsInGame() && ShouldEquipWeapon())
			{
				CurrentWeapon.SpawnVisualisation(m_RightHand);
			}
		}
		CFGSelectionManager.Instance.OnCharacterChangedWeapon();
	}

	public void OnAnimWeaponEquipped()
	{
		CurrentWeapon.SpawnVisualisation(m_RightHand);
	}

	private void OnAnimWeaponEquippedEnd()
	{
		Invoke("OnAnimWeaponEquippedEndDelayed", 1f);
	}

	private void OnAnimWeaponEquippedEndDelayed()
	{
		m_IsWaitingForWeaponEquipped = false;
	}

	private void GenerateAOETargets()
	{
		if (!(m_ActionTarget is CFGIAttackable { CurrentCell: not null } cFGIAttackable))
		{
			return;
		}
		CFGDef_Weapon definition = CurrentWeapon.m_Definition;
		Vector3 CharPos = CurrentCell.WorldPosition;
		Vector3 VecFwd = (cFGIAttackable.CurrentCell.WorldPosition - CharPos).normalized;
		float coneAngle = definition.ConeAngle;
		coneAngle = Mathf.Cos((float)Math.PI / 180f * coneAngle * 0.5f);
		int floor = CurrentCell.Floor;
		float weaponrange = definition.WeaponClass.GetMaxDist();
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			CheckCOETarget(character, floor, weaponrange, coneAngle, ref VecFwd, ref CharPos);
		}
	}

	private void CheckCOETarget(CFGIAttackable Obj, int floor, float weaponrange, float RequiredDot, ref Vector3 VecFwd, ref Vector3 CharPos)
	{
		if (Obj == null || !Obj.IsAlive || Obj == m_ActionTarget as CFGIAttackable)
		{
			return;
		}
		CFGOwner owner = Obj.GetOwner();
		if (owner != null && owner == Owner)
		{
			return;
		}
		CFGCell currentCell = Obj.CurrentCell;
		if (currentCell == null || currentCell.Floor != floor)
		{
			return;
		}
		Vector3 worldPosition = currentCell.WorldPosition;
		float num = Vector3.Distance(worldPosition, CharPos);
		if (num > weaponrange)
		{
			return;
		}
		Vector3 normalized = (worldPosition - CharPos).normalized;
		float num2 = Vector3.Dot(normalized, VecFwd);
		if (!(num2 < RequiredDot) && CFGCellMap.GetLineOf(CurrentCell, currentCell, 10000, 32, bUseStartSideSteps: false, CFGCellMap.m_bLOS_UseSideStepsForEndPoint) == ELOXHitType.None)
		{
			if (m_AOETargets == null)
			{
				m_AOETargets = new List<CFGIAttackable>();
			}
			if (m_AOETargets != null)
			{
				m_AOETargets.Add(Obj);
			}
		}
	}

	private void DoReactionShoot(CFGCharacter target, int damage, int cth)
	{
		if (target == null || !CanDoReactionShot)
		{
			Debug.Log(string.Concat("Failed reaction shot at ", target, " by ", base.NameId, ", ", CurrentCell, " for ", damage));
		}
		else
		{
			Debug.Log(string.Concat("Reaction shot at: ", target, ", ", target.CurrentCell, " by ", base.NameId, ", ", CurrentCell, " for ", damage));
			m_ReactionShootDmg = damage;
			m_ReactionShootTgt = target;
			CanDoReactionShot = false;
			m_CharacterAnimator.PlayShootReaction(OnAnimFire_Reaction);
		}
	}

	private void OnAnimFire_Reaction()
	{
		if (CurrentWeapon.CurrentAmmo == 0)
		{
			CurrentWeapon.CurrentAmmo++;
		}
		CurrentWeapon.Shoot(100, m_ReactionShootDmg, this, m_ReactionShootTgt, null);
		m_ReactionShootTgt = null;
		m_ReactionShootDmg = 0;
		if (Owner == null || ((bool)Owner && !Owner.IsPlayer))
		{
			CheckEnemiesSuspicion(ETurnAction.Shoot);
		}
	}

	private void FinalizeMovementActions()
	{
		if (m_Steering == null || m_Steering.Path == null || m_Steering.Path.m_Reactions == null)
		{
			return;
		}
		foreach (MoveAction reaction in m_Steering.Path.m_Reactions)
		{
			if ((bool)reaction.Source)
			{
				if (reaction.Source.CharacterAnimator != null)
				{
					reaction.Source.CharacterAnimator.ForceNoCoverTemporarily = false;
				}
				reaction.Source.ProcessFacingToCover();
			}
		}
	}

	private void UpdateReactionShoot(CFGCell current)
	{
		if (!CFGSingleton<CFGGame>.Instance.IsInGame() || m_Steering == null || m_Steering.Path == null || IsShadowCloaked || (HaveAbility(ETurnAction.Disguise) && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage))
		{
			return;
		}
		List<MoveAction> reactions = m_Steering.Path.m_Reactions;
		if (reactions == null || reactions.Count == 0)
		{
			return;
		}
		int eP = current.EP;
		int num = 0;
		while (num < reactions.Count)
		{
			if (reactions[num].CellEP != eP || reactions[num].Played)
			{
				num++;
				continue;
			}
			switch (reactions[num].Action)
			{
			case EMOVEACTION.SHOOT_AT:
				if ((bool)reactions[num].Source)
				{
					reactions[num].Source.DoReactionShoot(this, reactions[num].INTVAL2, reactions[num].INTVAL1);
					reactions[num].Source.CanDoReactionShot = false;
				}
				break;
			case EMOVEACTION.ROTATE_AT:
				reactions[num].Source.RotateToward(current.WorldPosition);
				break;
			case EMOVEACTION.SUSPICIOUS_CHECK:
				reactions[num].Source.HandleSuspiciousMovement(this);
				foreach (MoveAction item in reactions)
				{
					if (item.Source == reactions[num].Source && item.Action == reactions[num].Action)
					{
						item.Played = true;
					}
				}
				break;
			}
			reactions[num].Played = true;
		}
	}

	public static void UpdateShadowCloaks()
	{
		if (CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters == null)
		{
			return;
		}
		bool flag = true;
		if ((bool)CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner.IsAi)
		{
			flag = false;
		}
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (character == null || character.IsDead || character.Imprisoned)
			{
				continue;
			}
			if (flag)
			{
				if (character.Owner == null || character.Owner.IsAi)
				{
					continue;
				}
			}
			else if ((bool)character.Owner && character.Owner.IsPlayer)
			{
				continue;
			}
			RecalcualteShadowCloak(character);
		}
	}

	private static void RecalcualteShadowCloak(CFGCharacter user)
	{
		if (!user)
		{
			return;
		}
		if (!user.HaveAbility(ETurnAction.ShadowCloak))
		{
			user.LeaveShadowCloak(null);
			return;
		}
		if (!user.IsInShadow)
		{
			user.LeaveShadowCloak(null);
			return;
		}
		HashSet<CFGCharacter> hashSet = null;
		if ((bool)user.Owner)
		{
			if (user.Owner.IsAi && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner != null)
			{
				hashSet = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner.Characters;
			}
			else if (user.Owner.IsPlayer && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null)
			{
				hashSet = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters;
			}
		}
		if (hashSet == null)
		{
			user.LeaveShadowCloak(null);
			return;
		}
		Vector3 position = user.Position;
		foreach (CFGCharacter item in hashSet)
		{
			if (!(item == null) && !item.IsDead && !item.Imprisoned)
			{
				float num = Vector3.Distance(position, item.Position);
				if (!(num > CFGDef_Ability.ShadowCloak_Range))
				{
					user.LeaveShadowCloak(item);
					return;
				}
			}
		}
		CFGSelectionManager instance = CFGSelectionManager.Instance;
		if (instance != null)
		{
			instance.MarkCharactersVisibilityNeedUpdate();
		}
		user.m_bShadowCloaked = true;
		user.m_CharData.AddBuff("shadowcloak", EBuffSource.Ability);
		Vector3 position2 = user.Transform.position;
		position2.y += 1.9f;
		if (!user.IsShadowCloaked)
		{
			CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_shadowcloak_active"), plus: false, 1, -1, user);
		}
	}

	public void LeaveShadowCloak(CFGCharacter Detector)
	{
		if (IsShadowCloaked)
		{
			m_bShadowCloaked = false;
			if (m_CharData != null)
			{
				m_CharData.RemBuff("shadowcloak");
			}
			Vector3 position = Position;
			position.y += 1.2f;
			if (Detector == null)
			{
				Vector3 position2 = base.transform.position;
				position2.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_shadowcloak_leaving") + "</color>", plus: false, 1, -1, this);
			}
			else
			{
				Vector3 position3 = base.transform.position;
				position3.y += 1.9f;
				CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position3, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_detectedby", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(Detector.NameId)) + "</color>", plus: false, 1, -1, this);
			}
		}
	}

	private int MultiShotComparison(CFGIAttackable a, CFGIAttackable b)
	{
		if (a == null)
		{
			return 1;
		}
		if (b == null)
		{
			return -1;
		}
		float num = Vector3.Distance(a.Position, Position);
		float value = Vector3.Distance(b.Position, Position);
		return num.CompareTo(value);
	}

	private void PrepareMultiShot(List<CFGIAttackable> targets)
	{
		if (CurrentWeapon != null && CurrentWeapon.TwoHanded && CurrentWeapon.Visualisation != null)
		{
			CharacterAnimator.PlayWeaponChange0(AbilityAnim_OnWeaponRemove);
		}
		m_AOETargets = new List<CFGIAttackable>();
		targets.Sort(MultiShotComparison);
		int num = Mathf.Min(6, targets.Count);
		for (int i = 0; i < num; i++)
		{
			m_AOETargets.Add(targets[i]);
		}
		if (m_AOETargets.Count > 0)
		{
			m_ActionTarget = (CFGGameObject)m_AOETargets[0];
		}
		m_MultiShotCounter = 0;
	}

	public void MultiShotSingle()
	{
		MultiShotSingleVis();
		MultiShotSingleLogic();
		m_MultiShotCounter++;
	}

	public void OnFinishMultiShot()
	{
		foreach (CFGCharacter aOETarget in m_AOETargets)
		{
			if (aOETarget.Hp < 1)
			{
				Vector3 normalized = (aOETarget.Transform.position - base.transform.position).normalized;
				aOETarget.DelayedDeath(this, normalized);
			}
		}
	}

	private void MultiShotSingleVis()
	{
		if (m_AOETargets != null)
		{
			((m_MultiShotCounter % 2 != 0) ? MultiShotWeapon1 : MultiShotWeapon2)?.FakeShoot(this, null, bHit: false, CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotBullet);
		}
	}

	private void MultiShotSingleLogic()
	{
		if (m_AOETargets == null || m_AOETargets.Count == 0)
		{
			if (m_CurrentAction != ETurnAction.None)
			{
				OnEndAction();
			}
		}
		else if (m_MultiShotCounter <= m_AOETargets.Count - 1)
		{
			CFGBullet cFGBullet = null;
			if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData != null)
			{
				cFGBullet = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_MultiShotBullet;
			}
			if (cFGBullet == null && (bool)CurrentWeapon && CurrentWeapon.m_Definition != null)
			{
				cFGBullet = CurrentWeapon.m_Definition.Prefab_Bullet;
			}
			int num = CalcDamage(m_AOETargets[m_MultiShotCounter], ETurnAction.MultiShot);
			if (cFGBullet != null)
			{
				Vector3 position = Position;
				position.y += 1.2f;
				CFGBullet cFGBullet2 = UnityEngine.Object.Instantiate(cFGBullet, position, Quaternion.identity) as CFGBullet;
				cFGBullet2.InitBullet(this, m_AOETargets[m_MultiShotCounter], bHit: true, num, 100);
			}
			else
			{
				m_AOETargets[m_MultiShotCounter].TakeDamage(num, this, bSilent: false);
			}
		}
	}

	public void ApplyJinx()
	{
		CFGAbility ability = GetAbility(ETurnAction.Jinx);
		if (ability == null)
		{
			return;
		}
		List<CFGIAttackable> TargetList = new List<CFGIAttackable>();
		ability.GenerateTargetList(CurrentCell, ref TargetList, CurrentCell.WorldPosition);
		if (Owner != null)
		{
			HashSet<CFGCharacter> enemys = Owner.GetEnemys();
			foreach (CFGCharacter item in enemys)
			{
				if (item.CharacterData != null && item.CharacterData.HasBuff("jinxed") && !TargetList.Contains(item))
				{
					item.CharacterData.RemBuff("jinxed");
				}
			}
		}
		if (TargetList.Count == 0)
		{
			return;
		}
		foreach (CFGIAttackable item2 in TargetList)
		{
			CFGCharacter cFGCharacter = item2 as CFGCharacter;
			if (!(cFGCharacter == null) && cFGCharacter.CharacterData != null && !cFGCharacter.IsDead)
			{
				cFGCharacter.CharacterData.AddBuff("jinxed", EBuffSource.Ability);
				cFGCharacter.m_FlagNeedFlash = true;
			}
		}
	}

	private bool IsTargetReadyForAction(CFGCharacter targetedChar)
	{
		bool flag = false;
		ETurnAction currentAction = CurrentAction;
		return true;
	}

	public void OnTransfusionMake()
	{
		m_CharacterAnimator.PlayTransfusion();
		m_FlagNeedFlashing = true;
	}

	public void OnTransfusionRecive()
	{
		m_CharacterAnimator.PlayTransfusion();
		m_FlagNeedFlashing = true;
	}

	public bool IsCharacterReady()
	{
		switch (CurrentAction)
		{
		case ETurnAction.MultiShot:
			if ((CurrentWeapon.TwoHanded && CurrentWeapon.Visualisation != null) || (CurrentWeapon.Visualisation == null && !CharacterAnimator.IsInIdle()))
			{
				return false;
			}
			return true;
		case ETurnAction.Use:
		{
			if (m_ActionTarget == null)
			{
				return true;
			}
			CFGUsableObject component = m_ActionTarget.GetComponent<CFGUsableObject>();
			if (component != null && component.IsDynamicCover && m_CharacterAnimator != null)
			{
				return m_CharacterAnimator.IsInIdle(onlyNoCover: true);
			}
			return true;
		}
		default:
			return true;
		}
	}

	private void AddCripplerFX()
	{
		if (!(base.gameObject.GetComponent<CFGCrippler>() != null))
		{
			base.gameObject.AddComponent<CFGCrippler>();
		}
	}

	public void SetupCustomization(int preset_idx)
	{
		if (preset_idx == -1)
		{
			RandomizePartsVisibility();
			RandomizePartsColors();
		}
		else
		{
			if (m_PartsGroups != null)
			{
				for (int i = 0; i < m_PartsGroups.Length; i++)
				{
					CharPartsGroup charPartsGroup = m_PartsGroups[i];
					if (charPartsGroup.m_Parts == null)
					{
						continue;
					}
					for (int j = 0; j < charPartsGroup.m_Parts.Length; j++)
					{
						if (charPartsGroup.m_Parts[j] != null)
						{
							charPartsGroup.m_Parts[j].gameObject.SetActive(value: false);
						}
					}
				}
			}
			if (m_PartsAlwaysVisible != null)
			{
				for (int k = 0; k < m_PartsAlwaysVisible.Length; k++)
				{
					Renderer renderer = m_PartsAlwaysVisible[k];
					if (renderer != null)
					{
						renderer.gameObject.SetActive(value: true);
					}
				}
			}
			if (m_CustomPresets != null && preset_idx >= 0 && preset_idx < m_CustomPresets.Length)
			{
				CharCustomPreset charCustomPreset = m_CustomPresets[preset_idx];
				if (charCustomPreset.m_VisibleParts != null)
				{
					for (int l = 0; l < charCustomPreset.m_VisibleParts.Length; l++)
					{
						CharPartPreset charPartPreset = charCustomPreset.m_VisibleParts[l];
						if (charPartPreset.m_Part != null)
						{
							charPartPreset.m_Part.gameObject.SetActive(value: true);
							if (charPartPreset.m_Part.material != null)
							{
								charPartPreset.m_Part.material.SetColor("_Color", charPartPreset.m_Color);
							}
						}
					}
				}
			}
		}
		m_WasCustomized = true;
		CreateShaderBackup();
		UpdateVisibility(force: true);
	}

	public void RandomizePartsVisibility()
	{
		if (m_CustomPresets != null)
		{
			for (int i = 0; i < m_CustomPresets.Length; i++)
			{
				CharCustomPreset charCustomPreset = m_CustomPresets[i];
				if (charCustomPreset.m_VisibleParts == null)
				{
					continue;
				}
				for (int j = 0; j < charCustomPreset.m_VisibleParts.Length; j++)
				{
					if (charCustomPreset.m_VisibleParts[j].m_Part != null)
					{
						charCustomPreset.m_VisibleParts[j].m_Part.gameObject.SetActive(value: false);
					}
				}
			}
		}
		if (m_PartsAlwaysVisible != null)
		{
			for (int k = 0; k < m_PartsAlwaysVisible.Length; k++)
			{
				Renderer renderer = m_PartsAlwaysVisible[k];
				if (renderer != null)
				{
					renderer.gameObject.SetActive(value: true);
				}
			}
		}
		if (m_PartsGroups == null)
		{
			return;
		}
		for (int l = 0; l < m_PartsGroups.Length; l++)
		{
			CharPartsGroup charPartsGroup = m_PartsGroups[l];
			if (charPartsGroup.m_Parts == null)
			{
				continue;
			}
			if (charPartsGroup.m_GroupType == CharPartsGroup.EPartsGroupType.AnyCombination)
			{
				for (int m = 0; m < charPartsGroup.m_Parts.Length; m++)
				{
					if (charPartsGroup.m_Parts[m] != null)
					{
						charPartsGroup.m_Parts[m].gameObject.SetActive(UnityEngine.Random.Range(0, 2) == 0);
					}
				}
				continue;
			}
			int num = UnityEngine.Random.Range(0, (charPartsGroup.m_GroupType != 0) ? (charPartsGroup.m_Parts.Length + 1) : charPartsGroup.m_Parts.Length);
			for (int n = 0; n < charPartsGroup.m_Parts.Length; n++)
			{
				if (charPartsGroup.m_Parts[n] != null)
				{
					charPartsGroup.m_Parts[n].gameObject.SetActive(n == num);
				}
			}
		}
	}

	public void RandomizePartsColors()
	{
		if (m_PartsColorGroups == null)
		{
			return;
		}
		for (int i = 0; i < m_PartsColorGroups.Length; i++)
		{
			CharPartColorGroup charPartColorGroup = m_PartsColorGroups[i];
			if (charPartColorGroup.m_Renderer != null && charPartColorGroup.m_Renderer.material != null && charPartColorGroup.m_Colors != null && charPartColorGroup.m_Colors.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, charPartColorGroup.m_Colors.Length);
				charPartColorGroup.m_Renderer.material.SetColor("_Color", charPartColorGroup.m_Colors[num]);
			}
		}
	}

	private CFGCell FindBestCellToDie()
	{
		if (CurrentCell == null || CurrentWeapon == null || CurrentWeapon.Visualisation == null)
		{
			return null;
		}
		foreach (GameObject pom in pomList)
		{
			UnityEngine.Object.Destroy(pom);
		}
		pomList.Clear();
		int positionX = CurrentCell.PositionX;
		int positionZ = CurrentCell.PositionZ;
		int floor = CurrentCell.Floor;
		List<CFGCell> list = new List<CFGCell>();
		list.Add(CFGCellMap.GetCell(positionX + 1, positionZ, floor));
		list.Add(CFGCellMap.GetCell(positionX - 1, positionZ, floor));
		list.Add(CFGCellMap.GetCell(positionX, positionZ - 1, floor));
		list.Add(CFGCellMap.GetCell(positionX, positionZ + 1, floor));
		list.Add(CFGCellMap.GetCell(positionX + 1, positionZ + 1, floor));
		list.Add(CFGCellMap.GetCell(positionX - 1, positionZ - 1, floor));
		list.Add(CFGCellMap.GetCell(positionX - 1, positionZ + 1, floor));
		list.Add(CFGCellMap.GetCell(positionX + 1, positionZ - 1, floor));
		list.RemoveAll((CFGCell m) => m == null);
		List<CFGCell> possibleCells = new List<CFGCell>();
		foreach (CFGCell item2 in list)
		{
			if (item2 != null && item2.CanStandOnThisTile(can_stand_now: false) && item2.HaveFloor && (item2.PositionX <= positionX || (CurrentCell.GetBorderCover(EDirection.EAST) == ECoverType.NONE && !CurrentCell.CheckFlag(4, 8))) && (item2.PositionX >= positionX || (CurrentCell.GetBorderCover(EDirection.WEST) == ECoverType.NONE && !CurrentCell.CheckFlag(5, 8))) && (item2.PositionZ <= positionZ || (CurrentCell.GetBorderCover(EDirection.SOUTH) == ECoverType.NONE && !CurrentCell.CheckFlag(3, 8))) && (item2.PositionZ >= positionZ || (CurrentCell.GetBorderCover(EDirection.NORTH) == ECoverType.NONE && !CurrentCell.CheckFlag(2, 8))))
			{
				possibleCells.Add(item2);
			}
		}
		if (possibleCells == null || possibleCells.Count == 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		if (possibleCells.Any((CFGCell m) => m.CurrentCharacter == null))
		{
			possibleCells.RemoveAll((CFGCell m) => m.CurrentCharacter != null);
		}
		List<Vector3> list2 = new List<Vector3>();
		list2.Add(base.transform.forward);
		list2.Add(-base.transform.forward);
		list2.Add(-base.transform.right);
		list2.Add(base.transform.right);
		List<Vector3> list3 = list2;
		List<IdAngle> list4 = new List<IdAngle>();
		float bestAngle = 180f;
		for (int i = 0; i < list3.Count; i++)
		{
			int id = 0;
			bestAngle = 180f;
			for (int j = 0; j < possibleCells.Count; j++)
			{
				Vector3 normalized = (possibleCells[j].WorldPosition - base.transform.position).normalized;
				float f = Mathf.Clamp(Vector3.Dot(list3[i], normalized), -1f, 1f);
				float num = Mathf.Acos(f) * 57.29578f;
				if (Mathf.Abs(num) < Mathf.Abs(bestAngle))
				{
					id = j;
					bestAngle = num;
				}
			}
			list4.Add(new IdAngle(id, bestAngle, list3[i]));
		}
		list4 = list4.OrderBy((IdAngle m) => m.angle).ToList();
		bestAngle = list4[0].angle;
		List<IdAngle> list5 = list4.Where((IdAngle m) => m.angle == bestAngle).ToList();
		List<IdAngle> list6 = new List<IdAngle>();
		foreach (IdAngle element in list5)
		{
			if (Mathf.Abs(element.angle) < 2f)
			{
				list6.Add(element);
				continue;
			}
			int num2 = list.Count((CFGCell m) => (bool)m != Vector3.Distance(m.WorldPosition, possibleCells[element.id].WorldPosition) <= 1f && m != possibleCells[element.id]);
			if (num2 < 2)
			{
				continue;
			}
			List<CFGCell> list7 = list.Where((CFGCell m) => Vector3.Distance(m.WorldPosition, possibleCells[element.id].WorldPosition) <= 1f && m != possibleCells[element.id]).ToList();
			if (list7.Count >= 2)
			{
				CFGCell cFGCell = list7[0];
				CFGCell cFGCell2 = list7[1];
				CFGCell item = ((!(Vector3.Distance(cFGCell.WorldPosition, base.transform.position + element.dir) < Vector3.Distance(cFGCell2.WorldPosition, base.transform.position + element.dir))) ? cFGCell2 : cFGCell);
				if (possibleCells.Contains(item))
				{
					list6.Add(element);
				}
			}
		}
		if (list6 == null || list6.Count == 0)
		{
			list6 = list5;
		}
		int index2 = UnityEngine.Random.Range(0, list6.Count);
		int id2 = list6[index2].id;
		return possibleCells[id2];
	}

	private int FindBestAnimationToDie(CFGCell deathCell, Vector3 recoilDir)
	{
		int result = 0;
		if (deathCell == null)
		{
			return result;
		}
		Vector3[] array = new Vector3[4]
		{
			base.transform.position + base.transform.forward,
			base.transform.position - base.transform.forward,
			base.transform.position - base.transform.right,
			base.transform.position + base.transform.right
		};
		float num = 10f;
		int num2 = 0;
		EDeathDirection eDeathDirection = EDeathDirection.FRONT;
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = Vector3.Distance(deathCell.WorldPosition, array[i]);
			if (num3 < num)
			{
				num = num3;
				num2 = i;
				eDeathDirection = (EDeathDirection)i;
			}
		}
		List<EDeathDirection> list = new List<EDeathDirection>();
		list.Add(EDeathDirection.BACK);
		list.Add(EDeathDirection.FRONT);
		list.Add(EDeathDirection.LEFT);
		list.Add(EDeathDirection.RIGHT);
		List<EDeathDirection> allowedShotDirections = list;
		List<DeathAnimation> list2 = new List<DeathAnimation>();
		list2.Add(new DeathAnimation(EDeathDirection.BACK, new List<EDeathDirection> { EDeathDirection.BACK }));
		list2.Add(new DeathAnimation(EDeathDirection.BACK, new List<EDeathDirection> { EDeathDirection.BACK }));
		list2.Add(new DeathAnimation(EDeathDirection.FRONT, new List<EDeathDirection> { EDeathDirection.FRONT }));
		list2.Add(new DeathAnimation(EDeathDirection.FRONT, new List<EDeathDirection> { EDeathDirection.FRONT }));
		list2.Add(new DeathAnimation(EDeathDirection.FRONT, allowedShotDirections));
		list2.Add(new DeathAnimation(EDeathDirection.BACK, new List<EDeathDirection>
		{
			EDeathDirection.BACK,
			EDeathDirection.LEFT
		}));
		list2.Add(new DeathAnimation(EDeathDirection.FRONT, new List<EDeathDirection> { EDeathDirection.BACK }));
		list2.Add(new DeathAnimation(EDeathDirection.BACK, new List<EDeathDirection>
		{
			EDeathDirection.BACK,
			EDeathDirection.RIGHT,
			EDeathDirection.FRONT
		}));
		list2.Add(new DeathAnimation(EDeathDirection.BACK, new List<EDeathDirection>
		{
			EDeathDirection.BACK,
			EDeathDirection.RIGHT,
			EDeathDirection.LEFT
		}));
		list2.Add(new DeathAnimation(EDeathDirection.FRONT, allowedShotDirections));
		list2.Add(new DeathAnimation(EDeathDirection.LEFT, allowedShotDirections));
		list2.Add(new DeathAnimation(EDeathDirection.RIGHT, allowedShotDirections));
		list2.Add(new DeathAnimation(EDeathDirection.LEFT, allowedShotDirections));
		List<DeathAnimation> list3 = list2;
		List<int> list4 = new List<int>();
		for (int j = 0; j < list3.Count; j++)
		{
			if (list3[j].m_DeathDirection == eDeathDirection)
			{
				list4.Add(j);
			}
		}
		int num4 = 0;
		EDeathDirection eDeathDirection2 = EDeathDirection.BACK;
		List<int> list5 = new List<int>();
		if (list4.Count > 0 && recoilDir != Vector3.zero)
		{
			List<Vector3> list6 = new List<Vector3>();
			list6.Add(base.transform.forward);
			list6.Add(-base.transform.forward);
			list6.Add(-base.transform.right);
			list6.Add(base.transform.right);
			List<Vector3> list7 = list6;
			float f = 180f;
			eDeathDirection2 = EDeathDirection.BACK;
			for (int k = 0; k < list7.Count; k++)
			{
				float num5 = CFGMath.CalcHorizontalAngle(list7[k], recoilDir);
				if (Mathf.Abs(num5) < Mathf.Abs(f))
				{
					f = num5;
					eDeathDirection2 = (EDeathDirection)k;
				}
			}
			for (int l = 0; l < list3.Count; l++)
			{
				if (list3[l].m_DeathDirection == eDeathDirection && list3[l].m_AllowedShotDirections.Contains(eDeathDirection2))
				{
					list5.Add(l);
				}
			}
		}
		if (list5.Count == 0)
		{
			list5 = list4;
		}
		if (list5.Count > 0)
		{
			num4 = UnityEngine.Random.Range(0, list5.Count);
			return list5[num4];
		}
		return UnityEngine.Random.Range(0, list3.Count);
	}

	private void DoDeathAnimationStuff(Vector3 recoildDir)
	{
		if (!(CharacterAnimator == null))
		{
			recoildDir.y = 0f;
			recoildDir.Normalize();
			CFGCell deathCell = FindBestCellToDie();
			int animationID = FindBestAnimationToDie(deathCell, recoildDir);
			CharacterAnimator.PlayDeath(animationID);
		}
	}

	virtual string CFGIAttackable.get_NameId()
	{
		return base.NameId;
	}
}
