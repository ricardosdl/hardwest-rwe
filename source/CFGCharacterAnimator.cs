using UnityEngine;

public class CFGCharacterAnimator : MonoBehaviour
{
	public delegate void OnEventDelegate();

	public bool m_FemaleRig;

	public bool m_Debug;

	public string m_DebugState = string.Empty;

	public float m_DebugTime;

	private Animator m_Animator;

	private CFGCharacter m_Character;

	private bool m_WaitingForEnd;

	private Vector3 m_WantedDirection;

	private float m_AngularSpeed;

	private Vector3 m_StartRotation;

	private Transform m_DummyRifleTarget;

	private float m_WeaponAngleDiff;

	private float m_LastStepSoundTime;

	private bool m_FanningShot1;

	private bool m_FanningShot2;

	private OnEventDelegate m_OnTriggeredStateEndCallback;

	private OnEventDelegate m_OnFireCallback;

	private OnEventDelegate m_OnHolsterCallback;

	private OnEventDelegate m_OnUnholsterCallback;

	private OnEventDelegate m_OnSpawnFXCallback;

	private OnEventDelegate m_OnSpawnFX2Callback;

	private OnEventDelegate m_OnDestroyFXCallback;

	private int lastCoverState;

	public Vector3 WantedDirection
	{
		get
		{
			return m_WantedDirection;
		}
		set
		{
			m_WantedDirection = value;
			m_StartRotation = m_Character.transform.forward;
			if (m_WantedDirection.x == 0f && m_WantedDirection.z == 0f)
			{
				m_WantedDirection = m_StartRotation;
			}
			if (!IsFacingRightDirection())
			{
				m_Animator.SetInteger("Cover", 0);
				CurrentCoverState = 0;
			}
		}
	}

	public float AngularSpeed => m_AngularSpeed;

	public int CurrentCoverState { get; set; }

	public bool ForceNoCoverTemporarily { get; set; }

	public void PlayPrayer(OnEventDelegate on_spawn_fx_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnSpawnFXCallback = on_spawn_fx_callback;
		TriggerAnimation("Prayer", on_end_callback);
	}

	public void PlayCannibal(OnEventDelegate on_spawn_fx_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnSpawnFXCallback = on_spawn_fx_callback;
		TriggerAnimation("Cannibal", on_end_callback);
	}

	public void PlayCourage(OnEventDelegate on_spawn_fx_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnSpawnFXCallback = on_spawn_fx_callback;
		TriggerAnimation("Courage", on_end_callback);
	}

	public void PlayFinder(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Finder", on_end_callback);
	}

	public void PlayTransfusion(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Transfusion", on_end_callback);
	}

	public void PlayShoot(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		InitCorrectingWeaponOrientation();
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("Shoot", on_end_callback);
	}

	public void PlayShootFanning(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_FanningShot1 = false;
		m_FanningShot2 = false;
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("ShootFanning", on_end_callback);
	}

	public void PlayShootCone(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		InitCorrectingWeaponOrientation();
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("ShootCone", on_end_callback);
	}

	public void PlayShootScoped(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		InitCorrectingWeaponOrientation();
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("ShootScoped", on_end_callback);
	}

	public void PlayShootReaction(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		InitCorrectingWeaponOrientation();
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("ShootReaction", on_end_callback);
	}

	public void PlayMultiShot(OnEventDelegate on_spawn_fx_callback = null, OnEventDelegate on_spawn_fx_2_callback = null, OnEventDelegate on_destroy_fx_callback = null, OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		InitCorrectingWeaponOrientation();
		m_OnSpawnFXCallback = on_spawn_fx_callback;
		m_OnSpawnFX2Callback = on_spawn_fx_2_callback;
		m_OnDestroyFXCallback = on_destroy_fx_callback;
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("MultiShot", on_end_callback);
	}

	public void PlayReload(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Reload", on_end_callback);
	}

	public void PlayWeaponChange0(OnEventDelegate on_holster_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnHolsterCallback = on_holster_callback;
		TriggerAnimation("ChangeWeapon0", on_end_callback);
	}

	public void PlayWeaponChange1(OnEventDelegate on_holster_callback = null, OnEventDelegate on_unholster_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnHolsterCallback = on_holster_callback;
		m_OnUnholsterCallback = on_unholster_callback;
		TriggerAnimation("ChangeWeapon1", on_end_callback);
	}

	public void PlayWeaponChange2(OnEventDelegate on_holster_callback = null, OnEventDelegate on_unholster_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnHolsterCallback = on_holster_callback;
		m_OnUnholsterCallback = on_unholster_callback;
		TriggerAnimation("ChangeWeapon2", on_end_callback);
	}

	public void PlayUse(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Use", on_end_callback);
	}

	public void PlayCreateCover(EDynamicCoverType coverType, OnEventDelegate on_end_callback = null)
	{
		m_Animator.SetInteger("DynamicCoverType", (int)coverType);
		TriggerAnimation("CreateCover", on_end_callback);
	}

	public void PlayHit(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Hit", on_end_callback);
	}

	public void PlayDeath(int animationID)
	{
		m_Animator.SetInteger("DeathType", animationID);
		TriggerAnimation("Death", null);
	}

	public void PlayLadderUp(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("LadderUp", on_end_callback);
	}

	public void PlayLadderDown(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("LadderDown", on_end_callback);
	}

	public void PlayLadder2Up(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Ladder2Up", on_end_callback);
	}

	public void PlayLadder2Down(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Ladder2Down", on_end_callback);
	}

	public void PlayConsume(OnEventDelegate on_end_callback = null)
	{
		TriggerAnimation("Consume", on_end_callback);
	}

	public void PlayThrow1(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("Throw1", on_end_callback);
	}

	public void PlayThrow2(OnEventDelegate on_fire_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnFireCallback = on_fire_callback;
		TriggerAnimation("Throw2", on_end_callback);
	}

	public void PlayShriek(OnEventDelegate on_spawn_fx_callback = null, OnEventDelegate on_end_callback = null)
	{
		m_OnSpawnFXCallback = on_spawn_fx_callback;
		TriggerAnimation("Shriek", on_end_callback);
	}

	public void ChangeGunpointState(EGunpointState state)
	{
		m_Animator.SetInteger("GunpointState", (int)state);
		if (state != 0)
		{
			m_Animator.SetInteger("Cover", 0);
			CurrentCoverState = 0;
		}
	}

	public void SetDirectionDiff(float dir_diff)
	{
		m_Animator.SetFloat("DirectionDiff", dir_diff);
	}

	public bool IsInIdleState()
	{
		return m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("idle");
	}

	public bool IsFacingRightDirection()
	{
		Vector3 forward = m_Character.Transform.forward;
		Vector3 wantedDirection = m_WantedDirection;
		forward.y = 0f;
		forward.Normalize();
		wantedDirection.y = 0f;
		wantedDirection.Normalize();
		float f = CFGMath.CalcHorizontalAngle(forward, wantedDirection);
		return Mathf.Abs(f) < 2f;
	}

	public void OnShoot()
	{
		if (m_OnFireCallback != null)
		{
			m_OnFireCallback();
			m_OnFireCallback = null;
		}
	}

	public void OnSingleMultiShot()
	{
		if (m_OnFireCallback != null)
		{
			m_OnFireCallback();
		}
	}

	public void OnFanningShoot1()
	{
		if (m_OnFireCallback != null && !m_FanningShot1)
		{
			m_OnFireCallback();
			m_FanningShot1 = true;
		}
	}

	public void OnFanningShoot2()
	{
		if (m_OnFireCallback != null && !m_FanningShot2)
		{
			m_OnFireCallback();
			m_FanningShot2 = true;
		}
	}

	public void OnWeaponHolster()
	{
		if (m_OnHolsterCallback != null)
		{
			m_OnHolsterCallback();
			m_OnHolsterCallback = null;
		}
	}

	public void OnWeaponUnholster()
	{
		if (m_OnUnholsterCallback != null)
		{
			m_OnUnholsterCallback();
			m_OnUnholsterCallback = null;
		}
	}

	public void OnWeaponHolsterUnholster()
	{
		if (m_OnHolsterCallback != null)
		{
			m_OnHolsterCallback();
			m_OnHolsterCallback = null;
		}
		if (m_OnUnholsterCallback != null)
		{
			m_OnUnholsterCallback();
			m_OnUnholsterCallback = null;
		}
	}

	public void OnSpawnFX()
	{
		if (m_OnSpawnFXCallback != null)
		{
			m_OnSpawnFXCallback();
			m_OnSpawnFXCallback = null;
		}
	}

	public void OnSpawnFX2()
	{
		if (m_OnSpawnFX2Callback != null)
		{
			m_OnSpawnFX2Callback();
			m_OnSpawnFX2Callback = null;
		}
	}

	public void OnDestroyFX()
	{
		if (m_OnDestroyFXCallback != null)
		{
			m_OnDestroyFXCallback();
			m_OnDestroyFXCallback = null;
		}
	}

	public void OnStep()
	{
		if (Time.time - m_LastStepSoundTime < 0.1f)
		{
			return;
		}
		if (m_Character != null && m_Character.CurrentCell != null)
		{
			if (m_Character.CurrentCell.Editor_IsFlagSet(0, 32))
			{
				CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_IndoorStep, m_Character.Transform.position);
			}
			else
			{
				CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_OutdoorStep, m_Character.Transform.position);
			}
		}
		m_LastStepSoundTime = Time.time;
	}

	public void OnPlaySound(GameObject go)
	{
		if (go != null)
		{
			CFGSoundDef.Play(go.GetComponent<CFGSoundDef>(), base.transform);
		}
	}

	public void OnPlaySoundShriek()
	{
		if (m_Character != null && m_Character.CurrentAction == ETurnAction.Shriek)
		{
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Shriek, base.transform);
		}
	}

	public void OnPlaySoundTransfusion()
	{
		if (m_Character != null && m_Character.CurrentAction == ETurnAction.Transfusion)
		{
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Transfusion, base.transform);
		}
	}

	public void OnPlaySoundEqualization()
	{
		if (m_Character != null && m_Character.CurrentAction == ETurnAction.Equalization)
		{
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Equalization, base.transform);
		}
	}

	public void SpawnGroundBlood()
	{
		if (m_Character != null && m_Character.Owner != null && (m_Character.Owner.IsPlayer || (m_Character.IsVisibleByPlayer() && (m_Character.VisibilityState & EBestDetectionType.Visible) == EBestDetectionType.Visible)))
		{
			Transform groundBloodFxPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_GroundBloodFxPrefab;
			if (groundBloodFxPrefab != null)
			{
				Transform transform = (Transform)Object.Instantiate(groundBloodFxPrefab, m_Character.GetDamagePivot().position, Quaternion.identity);
				transform.parent = m_Character.transform;
			}
		}
	}

	public bool IsInIdle(bool onlyNoCover = false)
	{
		bool flag = !m_Animator.IsInTransition(0) && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("idle");
		if (onlyNoCover)
		{
			flag = flag && (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("0H_Idle") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("1H_Idle") || m_Animator.GetCurrentAnimatorStateInfo(0).IsName("2H_Idle"));
		}
		return flag;
	}

	public void ResetTrigger(string _triggerName)
	{
		if (!(m_Animator == null))
		{
			m_Animator.ResetTrigger(_triggerName);
		}
	}

	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
		m_Character = GetComponent<CFGCharacter>();
		m_WantedDirection = m_Character.Transform.forward;
		m_StartRotation = m_WantedDirection;
	}

	private void Start()
	{
		if (m_Character != null)
		{
			m_DummyRifleTarget = m_Character.FindChildRecursively(m_Character.Transform, "Dummy_rifle_target");
		}
	}

	private static float CalcWantedAngularSpeed(float angle_diff)
	{
		if (angle_diff > 45f)
		{
			return 108f;
		}
		if (angle_diff > 10f)
		{
			return 62f;
		}
		if (angle_diff >= 2f)
		{
			return 31f;
		}
		return 0f;
	}

	private void Update()
	{
		if (m_Debug)
		{
			m_Animator.speed = 0f;
			m_DebugTime = Mathf.Clamp01(m_DebugTime);
			m_Animator.Play(m_DebugState, 0, m_DebugTime);
		}
		else
		{
			if (!(m_Character != null))
			{
				return;
			}
			if (m_Character.m_Steering != null)
			{
				m_Animator.SetBool("IsMoving", m_Character.m_Steering.m_SteerData.m_IsMoving);
				m_Animator.SetFloat("Speed", m_Character.m_Steering.m_SteerData.m_Speed * 4.2f);
			}
			float f = CFGMath.CalcHorizontalAngle(m_Character.Transform.forward, m_WantedDirection);
			float num = CFGMath.CalcHorizontalAngle(m_StartRotation, m_WantedDirection);
			float num2 = CFGMath.CalcHorizontalAngle(m_StartRotation, m_Character.Transform.forward);
			if (((double)num2 > 0.0 && num2 > num) || (num2 < 0f && num > num2))
			{
				m_AngularSpeed = 0f;
				m_Character.transform.forward = m_WantedDirection;
			}
			else
			{
				float num3 = CalcWantedAngularSpeed(Mathf.Abs(f));
				if (m_AngularSpeed < num3)
				{
					m_AngularSpeed = Mathf.Min(m_AngularSpeed + 160f * Time.deltaTime, num3);
				}
				else if (m_AngularSpeed > num3)
				{
					m_AngularSpeed = Mathf.Max(m_AngularSpeed - 160f * Time.deltaTime, 0f);
				}
			}
			m_Animator.SetFloat("AngularSpeed", m_AngularSpeed * Mathf.Sign(f));
			int cover_type = 0;
			if (!ForceNoCoverTemporarily && (!CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || m_Character.CurrentAction.CanCharacterBeGluedinSetupStage()) && m_Character.CurrentAction.CanCharacterBeGlued() && !m_Character.Imprisoned && m_Character.CurrentCell != null && m_Character.GunpointState == EGunpointState.None && IsFacingRightDirection())
			{
				m_Character.CurrentCell.GetBestCoverToGlue(m_Character, out cover_type, out var _);
			}
			m_Animator.SetInteger("Cover", cover_type);
			CurrentCoverState = cover_type;
			if (lastCoverState != CurrentCoverState)
			{
				m_Character.ManageOutliner();
				lastCoverState = CurrentCoverState;
			}
			m_Animator.SetBool("Imprisoned", m_Character.Imprisoned);
			m_Animator.SetFloat("Woman", (!m_FemaleRig) ? 0f : 1f);
			if (m_Character.CurrentWeapon != null && m_Character.CurrentWeapon.TwoHanded && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("shoot"))
			{
				CorrectWeaponOrientation();
			}
		}
	}

	private void LateUpdate()
	{
		if (m_OnTriggeredStateEndCallback == null)
		{
			return;
		}
		if (m_WaitingForEnd)
		{
			if (!m_Animator.IsInTransition(0) && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
			{
				if (m_OnFireCallback != null)
				{
					m_OnFireCallback();
					m_OnFireCallback = null;
				}
				m_OnTriggeredStateEndCallback();
				m_OnTriggeredStateEndCallback = null;
				m_WaitingForEnd = false;
			}
		}
		else if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("idle") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("glue"))
		{
			m_WaitingForEnd = true;
		}
	}

	private void OnAnimatorMove()
	{
		if (m_Character != null && m_Character.m_Steering != null)
		{
			if ((m_Character.m_Steering.m_SteerData.m_IsMoving && m_Character.m_Steering.m_SteerData.m_Speed > 0f) || m_Character.m_Steering.m_SteerData.m_IsOnLadder)
			{
				Vector3 position = m_Character.Transform.position;
				position += m_Animator.deltaPosition;
				m_Character.Transform.position = position;
			}
			if (!IsFacingRightDirection() && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("idle") && !m_Animator.IsInTransition(0))
			{
				m_Character.Transform.forward = Vector3.RotateTowards(m_Character.Transform.forward, m_WantedDirection, Time.deltaTime * 12f, 0f).normalized;
			}
		}
	}

	private void TriggerAnimation(string trigger, OnEventDelegate callback)
	{
		m_WaitingForEnd = false;
		m_OnTriggeredStateEndCallback = callback;
		m_Animator.SetTrigger(trigger);
	}

	private void InitCorrectingWeaponOrientation()
	{
		if (m_Character != null && m_Character.CurrentWeapon != null && m_Character.CurrentWeapon.TwoHanded)
		{
			CFGWeaponVisualisation visualisation = m_Character.CurrentWeapon.Visualisation;
			if (visualisation != null && visualisation.BarrelEndPoint != null)
			{
				Vector3 to = -visualisation.transform.parent.right;
				Vector3 normalized = (visualisation.BarrelEndPoint.position - visualisation.transform.position).normalized;
				m_WeaponAngleDiff = Vector3.Angle(normalized, to);
			}
		}
	}

	private void CorrectWeaponOrientation()
	{
		CFGWeaponVisualisation cFGWeaponVisualisation = ((m_Character.CurrentWeapon == null) ? null : m_Character.CurrentWeapon.Visualisation);
		if (cFGWeaponVisualisation != null && m_DummyRifleTarget != null)
		{
			Vector3 vector = -cFGWeaponVisualisation.transform.parent.right;
			Vector3 normalized = (m_DummyRifleTarget.position - cFGWeaponVisualisation.transform.position).normalized;
			float num = Vector3.Angle(vector, normalized);
			Vector3 normalized2 = Vector3.Cross(vector, normalized).normalized;
			float f = Vector3.Dot(normalized2, cFGWeaponVisualisation.transform.forward);
			cFGWeaponVisualisation.transform.localEulerAngles = new Vector3(0f, 0f, m_WeaponAngleDiff + num * Mathf.Sign(f));
		}
	}
}
