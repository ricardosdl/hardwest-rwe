using UnityEngine;

public class CFGCamera : MonoBehaviour
{
	public delegate void OnFocusEndDelegate();

	private const float ACCELLSPD = 3f;

	private const float DECELLSPD = 2.5f;

	private const float FULLSCREEN_SCROLL_ACC = 0.4f;

	private const int m_MaxAngleV = 90;

	private const float m_DefaultChangingFocusTime = 0.5f;

	private const float m_ZoomSpeed = 100f;

	public float m_InitAngleH = 45f;

	public float m_InitAngleV = 35f;

	public float m_InitDistance = 15f;

	public float m_MinDistance = 1f;

	public float m_MaxDistance = 100000f;

	private bool m_IsEnabled = true;

	private bool m_FreeCam;

	private ELockReason m_ControlLockReason;

	private float m_CurrentAngleH;

	private float m_CurrentAngleV;

	private bool m_ChangingAngles;

	private float m_OldAngleH;

	private float m_OldAngleV;

	private float m_DestAngleH;

	private float m_DestAngleV;

	private float m_ChangingAnglesTime;

	private float m_ChangingAnglesDelta;

	private float m_CurrentDistance;

	private bool m_ChangingDistance;

	private float m_OldDistance;

	private float m_DestDistance;

	private EFloorLevelType m_CurrentFloorLevel;

	private int m_FloorLevelOffset;

	private bool m_IsRotatingByPlayer;

	private float m_TempMouseDist;

	private CFGGameObject m_CurrentTarget;

	private Vector3 m_CurrentTargetPoint;

	private Vector3 m_OldTargetPoint;

	private Vector3 m_DestTargetPoint;

	private bool m_ChangingTarget;

	private float m_ChangingFocusTime;

	private float m_ChangingTargetDelta;

	private bool m_FP_Active;

	private Vector3 m_FP_Target = Vector3.zero;

	private Vector3 m_FP_Direction = Vector3.forward;

	private float m_RefVelZoom;

	private CFGSelectionManager m_SelectionManager;

	private float m_SpeedLeft;

	private float m_SpeedRight;

	private float m_SpeedForward;

	private float m_SpeedBack;

	private float m_SpeedUp;

	private float m_SpeedDown;

	private CFGCameraFollowInfo m_CameraFollowInfo;

	private float m_CurrentSpeed;

	private OnFocusEndDelegate m_OnFocusEndCallback;

	private bool m_bRotationDisabled;

	public bool RotationDisabled
	{
		get
		{
			return m_bRotationDisabled;
		}
		set
		{
			m_bRotationDisabled = value;
		}
	}

	public CFGGameObject Focus => m_CurrentTarget;

	public float CurrentAngleH
	{
		get
		{
			return m_CurrentAngleH;
		}
		set
		{
			m_CurrentAngleH = value;
		}
	}

	public float CurrentAngleV
	{
		get
		{
			return m_CurrentAngleV;
		}
		set
		{
			m_CurrentAngleV = value;
		}
	}

	public float CurrentDistance => m_CurrentDistance;

	public float DestDistance
	{
		get
		{
			return m_DestDistance;
		}
		set
		{
			m_DestDistance = (m_CurrentDistance = value);
		}
	}

	public Vector3 CurrentTargetPoint
	{
		get
		{
			return m_CurrentTargetPoint;
		}
		set
		{
			m_CurrentTargetPoint = value;
		}
	}

	public int CurrentTargetPointFloorLevel
	{
		get
		{
			if (m_CurrentTargetPoint.y <= 0.5f)
			{
				return 0;
			}
			if (m_CurrentTargetPoint.y > 3f)
			{
				return 2;
			}
			return 1;
		}
	}

	public EFloorLevelType CurrentFloorLevel
	{
		get
		{
			return m_CurrentFloorLevel;
		}
		set
		{
			if (m_CurrentFloorLevel != value)
			{
				m_CurrentFloorLevel = value;
				m_SelectionManager.OnCurrentFloorLevelChanged();
			}
		}
	}

	public bool IsControlEnabled => m_ControlLockReason == ELockReason.NoLock;

	public ELockReason LockStatus => m_ControlLockReason;

	public void ChangeFocus(CFGGameObject target, bool force = false, OnFocusEndDelegate callback = null)
	{
		ChangeFocus(target, 0.5f, force, callback);
	}

	public void ChangeFocus(CFGGameObject target, float time, bool force = false, OnFocusEndDelegate callback = null)
	{
		if (!m_IsEnabled || target == null || (m_ChangingTarget && !force) || CFGCheats.DisableCameraFocuses)
		{
			callback?.Invoke();
			return;
		}
		m_FP_Active = false;
		m_CameraFollowInfo = null;
		m_FloorLevelOffset = 0;
		Vector3 position = target.transform.position;
		if (Vector3.Distance(position, m_CurrentTargetPoint) > 0.01f && (m_CurrentTarget != target || CFGSingleton<CFGGame>.Instance.IsInGame()))
		{
			m_OldTargetPoint = m_CurrentTargetPoint;
			m_DestTargetPoint = position;
			m_ChangingTarget = true;
			m_ChangingFocusTime = time;
			m_ChangingTargetDelta = 0f;
			if (m_CurrentDistance != m_MinDistance)
			{
				m_ChangingDistance = true;
				m_OldDistance = m_CurrentDistance;
				m_DestDistance = m_MinDistance;
			}
			m_OnFocusEndCallback = callback;
		}
		else
		{
			callback?.Invoke();
		}
		m_CurrentTarget = target;
	}

	public void ChangeFocus(CFGGameObject target, float time, CFGCameraFollowInfo _followInfo, bool force = false, OnFocusEndDelegate callback = null)
	{
		ChangeFocus(target, time, force, callback);
		m_CameraFollowInfo = _followInfo;
		if (m_CurrentTarget != null && m_CameraFollowInfo.Contains(m_CurrentTarget.transform.position))
		{
			m_ChangingTarget = false;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && target != null && target.NameId == "Horseman" && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			m_ChangingTarget = false;
			float num = Vector3.Distance(m_CurrentTargetPoint, target.transform.position);
			float num2 = 0f;
			if (CFGSelectionManager.Instance.GamepadCursor != null)
			{
				num2 = Vector3.Distance(CFGSelectionManager.Instance.GamepadCursor.transform.position, target.transform.position);
			}
			if (num <= 8f && num2 <= 8f)
			{
				ClearFocus();
			}
			else if (num > 8f && num < 16f)
			{
				m_CurrentSpeed = 2f * m_CameraFollowInfo.m_MaxSpeed;
			}
			else
			{
				m_ChangingTarget = true;
			}
		}
	}

	public void ChangeFocus(Vector3 target_pos, bool force = false, OnFocusEndDelegate callback = null)
	{
		if (!m_IsEnabled || (m_ChangingTarget && !force) || CFGCheats.DisableCameraFocuses)
		{
			callback?.Invoke();
			return;
		}
		m_CameraFollowInfo = null;
		m_FP_Active = false;
		m_OnFocusEndCallback = callback;
		m_FloorLevelOffset = 0;
		m_CurrentTarget = null;
		m_OldTargetPoint = m_CurrentTargetPoint;
		m_DestTargetPoint = target_pos;
		m_ChangingTarget = true;
		m_ChangingFocusTime = 0.5f;
		m_ChangingTargetDelta = 0f;
		m_ChangingDistance = false;
	}

	public void FollowPoint(Vector3 target_pos)
	{
		if (!m_ChangingTarget)
		{
			if (!m_FP_Active)
			{
				m_FP_Direction = m_CurrentTargetPoint - target_pos;
				m_FP_Direction.Normalize();
			}
			m_CurrentTarget = null;
			m_FP_Active = true;
			m_FP_Target = target_pos;
			m_FloorLevelOffset = 0;
			m_CurrentTargetPoint.y = target_pos.y;
		}
	}

	public void ChangeFocus(Vector3 target_pos, float distance, float time, bool force = false, OnFocusEndDelegate callback = null)
	{
		if (!m_IsEnabled || (m_ChangingTarget && !force) || CFGCheats.DisableCameraFocuses)
		{
			callback?.Invoke();
			return;
		}
		m_CameraFollowInfo = null;
		m_FP_Active = false;
		m_OnFocusEndCallback = callback;
		m_FloorLevelOffset = 0;
		m_CurrentTarget = null;
		m_OldTargetPoint = m_CurrentTargetPoint;
		m_DestTargetPoint = target_pos;
		m_ChangingTarget = true;
		m_ChangingFocusTime = time;
		m_ChangingTargetDelta = 0f;
		m_ChangingDistance = true;
		m_OldDistance = m_CurrentDistance;
		m_DestDistance = Mathf.Max(distance, GetFocusMinZoom());
		m_DestDistance = Mathf.Clamp(m_DestDistance, m_MinDistance, m_MaxDistance);
	}

	public void ChangeFocus(Vector3 p1, Vector3 p2, float time, bool force = false, OnFocusEndDelegate callback = null)
	{
		if (!m_IsEnabled || (m_ChangingTarget && !force) || CFGCheats.DisableCameraFocuses)
		{
			callback?.Invoke();
			return;
		}
		m_CameraFollowInfo = null;
		m_FP_Active = false;
		Plane plane = new Plane(base.transform.right, base.transform.position);
		Plane plane2 = new Plane(base.transform.up, base.transform.position);
		float distanceToPoint = plane.GetDistanceToPoint(p1);
		float distanceToPoint2 = plane.GetDistanceToPoint(p2);
		float distanceToPoint3 = plane2.GetDistanceToPoint(p1);
		float distanceToPoint4 = plane2.GetDistanceToPoint(p2);
		float num = Mathf.Abs(distanceToPoint2 - distanceToPoint);
		float num2 = Mathf.Abs(distanceToPoint4 - distanceToPoint3);
		float num3 = GetComponent<Camera>().fieldOfView * 0.3f;
		float num4 = num3 * GetComponent<Camera>().aspect;
		Vector3 vector3;
		if (num > num2 * GetComponent<Camera>().aspect)
		{
			Vector3 inPoint;
			Vector3 inPoint2;
			if (distanceToPoint < distanceToPoint2)
			{
				inPoint = p1;
				inPoint2 = p2;
			}
			else
			{
				inPoint = p2;
				inPoint2 = p1;
			}
			Vector3 vector = Quaternion.AngleAxis(0f - num4, base.transform.up) * base.transform.right;
			Vector3 vector2 = Quaternion.AngleAxis(num4, base.transform.up) * base.transform.right;
			Plane plane3 = new Plane(vector.normalized, inPoint);
			Plane plane4 = new Plane(vector2.normalized, inPoint2);
			Plane plane5 = default(Plane);
			plane5.normal = plane2.normal;
			plane5.distance = plane2.distance - (distanceToPoint3 + distanceToPoint4) * 0.5f;
			vector3 = CFGMath.Intersection(plane5, plane3, plane4);
		}
		else
		{
			Vector3 inPoint3;
			Vector3 inPoint4;
			if (distanceToPoint3 < distanceToPoint4)
			{
				inPoint3 = p1;
				inPoint4 = p2;
			}
			else
			{
				inPoint3 = p2;
				inPoint4 = p1;
			}
			Vector3 vector4 = Quaternion.AngleAxis(0f - num3, base.transform.right) * base.transform.up;
			Vector3 vector5 = Quaternion.AngleAxis(num3, base.transform.right) * base.transform.up;
			Plane plane6 = new Plane(vector4.normalized, inPoint4);
			Plane plane7 = new Plane(vector5.normalized, inPoint3);
			Plane plane8 = default(Plane);
			plane8.normal = plane.normal;
			plane8.distance = plane.distance - (distanceToPoint + distanceToPoint2) * 0.5f;
			vector3 = CFGMath.Intersection(plane8, plane6, plane7);
		}
		Plane plane9 = new Plane(Vector3.up, p2);
		Ray ray = new Ray(vector3, base.transform.forward);
		if (plane9.Raycast(ray, out var enter))
		{
			vector3 += base.transform.forward * enter;
		}
		if (Vector3.Distance(vector3, m_CurrentTargetPoint) > 0.01f)
		{
			ChangeFocus(vector3, enter, time, force, callback);
		}
		else
		{
			callback?.Invoke();
		}
	}

	public void SetRotation(float angle, float time)
	{
		if (m_IsEnabled && !m_ChangingAngles)
		{
			m_ChangingAnglesTime = time;
			m_ChangingAnglesDelta = 0f;
			m_ChangingAngles = true;
			m_OldAngleH = m_CurrentAngleH;
			m_OldAngleV = m_CurrentAngleV;
			m_DestAngleH = Mathf.Repeat(angle, 360f);
			m_DestAngleV = m_CurrentAngleV;
		}
	}

	public void RotateLeft(float time)
	{
		if (m_IsEnabled && !m_ChangingAngles)
		{
			m_ChangingAnglesTime = time;
			m_ChangingAnglesDelta = 0f;
			m_ChangingAngles = true;
			m_OldAngleH = m_CurrentAngleH;
			m_OldAngleV = m_CurrentAngleV;
			m_DestAngleH = Mathf.Repeat(m_CurrentAngleH + 90f, 360f);
			m_DestAngleV = m_CurrentAngleV;
		}
	}

	public void RotateRight(float time)
	{
		if (m_IsEnabled && !m_ChangingAngles)
		{
			m_ChangingAnglesTime = time;
			m_ChangingAnglesDelta = 0f;
			m_ChangingAngles = true;
			m_OldAngleH = m_CurrentAngleH;
			m_OldAngleV = m_CurrentAngleV;
			m_DestAngleH = Mathf.Repeat(m_CurrentAngleH - 90f, 360f);
			m_DestAngleV = m_CurrentAngleV;
		}
	}

	public void ClearFocus()
	{
		if (m_CurrentTarget != null)
		{
			ResetSpeeds();
			m_CurrentTarget = null;
			m_CameraFollowInfo = null;
		}
	}

	public bool IsChangingFocus()
	{
		return m_ChangingTarget;
	}

	public void ApplyCurrentTransform(float fDistance = 10f)
	{
		m_CurrentTargetPoint = base.transform.position + base.transform.forward * fDistance;
		m_DestDistance = fDistance;
		m_CurrentDistance = m_DestDistance;
		m_CurrentAngleH = base.transform.eulerAngles.y;
		m_CurrentAngleV = base.transform.eulerAngles.x;
		if (m_CurrentAngleV > 180f)
		{
			m_CurrentAngleV -= 360f;
		}
		m_ChangingAngles = false;
		m_ChangingDistance = false;
		m_ChangingTarget = false;
		m_CurrentTarget = null;
	}

	public void EnableCameraControlLock(ELockReason Reason, bool Enable)
	{
		if (Reason == ELockReason.NoLock)
		{
			return;
		}
		bool flag = !IsControlEnabled;
		if (Enable)
		{
			m_ControlLockReason |= Reason;
		}
		else
		{
			m_ControlLockReason &= ~Reason;
		}
		if (flag && IsControlEnabled)
		{
			if (m_IsRotatingByPlayer)
			{
				m_IsRotatingByPlayer = false;
			}
			m_TempMouseDist = 0f;
		}
	}

	public void SetEnabled(bool enabled)
	{
		if (enabled == m_IsEnabled)
		{
			return;
		}
		if (m_IsEnabled)
		{
			ClearFocus();
			if (m_IsRotatingByPlayer)
			{
				m_IsRotatingByPlayer = false;
			}
			m_TempMouseDist = 0f;
		}
		m_IsEnabled = enabled;
	}

	public bool IsEnabled()
	{
		return m_IsEnabled;
	}

	public void SetFreeCamMode(bool enabled)
	{
		if (m_FreeCam != enabled)
		{
			m_FreeCam = enabled;
			if (m_FreeCam)
			{
				ClearFocus();
			}
			else
			{
				ApplyCurrentTransform();
			}
		}
	}

	public bool IsInFreeCamMode()
	{
		return m_FreeCam;
	}

	private void Awake()
	{
		m_SelectionManager = GetComponent<CFGSelectionManager>();
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}

	private void Start()
	{
		CFGLevelSettings levelSettings = CFGSingleton<CFGGame>.Instance.LevelSettings;
		if (levelSettings != null)
		{
			m_CurrentTarget = levelSettings.m_InitialCameraFocus;
		}
		base.transform.position = new Vector3(0f, 0f, -1f);
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		if (m_CurrentTarget != null)
		{
			m_CurrentTargetPoint = m_CurrentTarget.Transform.position;
		}
		else
		{
			m_CurrentTargetPoint = Vector3.zero;
		}
		m_CurrentAngleH = m_InitAngleH;
		m_CurrentAngleV = m_InitAngleV;
		m_DestDistance = m_InitDistance;
		m_CurrentDistance = m_DestDistance;
		m_ChangingFocusTime = 0.5f;
	}

	private void Update()
	{
		if (m_IsEnabled)
		{
		}
	}

	private void ResetSpeeds()
	{
		m_SpeedLeft = 0f;
		m_SpeedRight = 0f;
		m_SpeedForward = 0f;
		m_SpeedBack = 0f;
		m_SpeedUp = 0f;
		m_SpeedDown = 0f;
	}

	private void UpdateInput()
	{
		if (CFGTimer.IsPaused_Gameplay)
		{
			return;
		}
		float num = Input.GetAxis("Mouse X");
		float num2 = Input.GetAxis("Mouse Y");
		bool flag = false;
		bool flag2 = CFGSingleton<CFGWindowMgr>.Instance.IsCursorOverUI();
		float timeScale = Time.timeScale;
		timeScale = ((!(timeScale > 0.9f)) ? 1f : (1f / timeScale));
		if (CFGInput.ExclusiveInputDevice == EInputMode.Gamepad)
		{
			num = 0f;
			num2 = 0f;
		}
		if (m_SelectionManager.CurrentAction == ETurnAction.None && CFGInput.IsActivated(EActionCommand.Camera_ChangeFocus))
		{
			ChangeFocus(m_SelectionManager.SelectedCharacter);
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame() && !m_bRotationDisabled)
		{
			if (CFGInput.IsActivated(EActionCommand.Camera_RotateLeft))
			{
				RotateLeft(0.5f);
			}
			if (CFGInput.IsActivated(EActionCommand.Camera_RotateRight))
			{
				RotateRight(0.5f);
			}
		}
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			if (CFGInput.IsActivated(EActionCommand.Camera_PanUp))
			{
				m_FloorLevelOffset++;
			}
			if (CFGInput.IsActivated(EActionCommand.Camera_PanDown))
			{
				m_FloorLevelOffset--;
			}
		}
		if (CurrentTargetPointFloorLevel + m_FloorLevelOffset > 2)
		{
			m_FloorLevelOffset = 2 - CurrentTargetPointFloorLevel;
		}
		else if (CurrentTargetPointFloorLevel + m_FloorLevelOffset < 0)
		{
			m_FloorLevelOffset = -CurrentTargetPointFloorLevel;
		}
		CurrentFloorLevel = (EFloorLevelType)(CurrentTargetPointFloorLevel + m_FloorLevelOffset);
		if (!m_ChangingTarget)
		{
			float num3 = CFGOptions.Gameplay.CameraPanningSpeed * Time.deltaTime * (m_DestDistance + 20f) * timeScale;
			float num4 = Time.deltaTime * 100f * timeScale;
			if (CFGInput.ExclusiveInputDevice != EInputMode.Gamepad && (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
			{
				num3 *= 2f;
			}
			float num5 = Time.deltaTime * 3f;
			float num6 = Time.deltaTime * 2.5f;
			bool fullScreen = Screen.fullScreen;
			bool flag3 = CFGInput.LastReadInputDevice == EInputMode.KeyboardAndMouse;
			float num7 = ((!fullScreen || !flag3 || !(Input.mousePosition.x <= 0f)) ? CFGInput.ActivationValue(EActionCommand.Camera_PanLeft) : 0.4f);
			if (num7 > 0.01f)
			{
				if (m_SpeedLeft < num7)
				{
					m_SpeedLeft += num5 * num7;
				}
				else
				{
					m_SpeedLeft -= num6 * num7;
				}
			}
			else
			{
				m_SpeedLeft -= num6;
			}
			num7 = ((!fullScreen || !flag3 || !(Input.mousePosition.x >= (float)(Screen.width - 1))) ? CFGInput.ActivationValue(EActionCommand.Camera_PanRight) : 0.4f);
			if (num7 > 0.01f)
			{
				if (m_SpeedRight < num7)
				{
					m_SpeedRight += num5 * num7;
				}
				else
				{
					m_SpeedRight -= num6 * num7;
				}
			}
			else
			{
				m_SpeedRight -= num6;
			}
			num7 = ((!fullScreen || !flag3 || !(Input.mousePosition.y >= (float)(Screen.height - 2))) ? CFGInput.ActivationValue(EActionCommand.Camera_PanForward) : 0.4f);
			if (num7 > 0.01f)
			{
				if (m_SpeedForward < num7)
				{
					m_SpeedForward += num5 * num7;
				}
				else
				{
					m_SpeedForward -= num6 * num7;
				}
			}
			else
			{
				m_SpeedForward -= num6;
			}
			num7 = ((!fullScreen || !flag3 || !(Input.mousePosition.y <= 1f)) ? CFGInput.ActivationValue(EActionCommand.Camera_PanBack) : 0.4f);
			if (num7 > 0.01f)
			{
				if (m_SpeedBack < num7)
				{
					m_SpeedBack += num5 * num7;
				}
				else
				{
					m_SpeedBack -= num6 * num7;
				}
			}
			else
			{
				m_SpeedBack -= num6;
			}
			m_SpeedLeft = Mathf.Clamp01(m_SpeedLeft);
			m_SpeedRight = Mathf.Clamp01(m_SpeedRight);
			m_SpeedForward = Mathf.Clamp01(m_SpeedForward);
			m_SpeedBack = Mathf.Clamp01(m_SpeedBack);
			m_SpeedUp = Mathf.Clamp01(m_SpeedUp);
			m_SpeedDown = Mathf.Clamp01(m_SpeedDown);
			float num8 = m_SpeedRight - m_SpeedLeft;
			float num9 = m_SpeedForward - m_SpeedBack;
			float num10 = m_SpeedUp - m_SpeedDown;
			if (m_FreeCam)
			{
				if (num8 != 0f)
				{
					Move(Vector3.right * num4 * num8);
				}
				if (num9 != 0f)
				{
					Move(Vector3.forward * num4 * num9);
				}
				if (num10 != 0f)
				{
					Move(Vector3.up * num4 * num10);
				}
			}
			else
			{
				if (num8 != 0f)
				{
					Pan(base.transform.right * num3 * num8);
				}
				if (num9 != 0f)
				{
					Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
					Pan(vector * num3 * num9);
				}
				if (num10 != 0f)
				{
					Pan(Vector3.up * num3 * num10);
				}
			}
		}
		if (CFGInput.ExclusiveInputDevice != EInputMode.Gamepad)
		{
			if (!flag2 && Input.GetMouseButtonUp(2))
			{
				ChangeFocus(m_SelectionManager.SelectedCharacter);
			}
			if (Input.GetMouseButton(1) && !m_IsRotatingByPlayer && !flag2)
			{
				if (m_TempMouseDist > 0.8f)
				{
					m_IsRotatingByPlayer = true;
				}
				else
				{
					m_TempMouseDist += new Vector2(num, num2).magnitude;
				}
			}
			if (Input.GetMouseButtonUp(1))
			{
				if (m_IsRotatingByPlayer)
				{
					m_IsRotatingByPlayer = false;
				}
				else if (!flag2)
				{
					m_SelectionManager.OnRightClick(0);
				}
				CFGInput.ChangeInputMode(EInputMode.KeyboardAndMouse);
				m_TempMouseDist = 0f;
			}
		}
		if (!m_ChangingDistance)
		{
			if (flag)
			{
			}
			m_CurrentDistance = Mathf.SmoothDamp(m_CurrentDistance, m_DestDistance, ref m_RefVelZoom, 0.5f);
		}
		if (m_FreeCam && !m_ChangingAngles && m_IsRotatingByPlayer)
		{
			float num11 = 200f * Time.deltaTime;
			float num12 = num * num11;
			float num13 = num2 * num11;
			m_CurrentAngleH += num12;
			m_CurrentAngleV -= num13;
			m_CurrentAngleH = Mathf.Repeat(m_CurrentAngleH, 360f);
			m_CurrentAngleV = Mathf.Clamp(m_CurrentAngleV, -90f, 90f);
		}
	}

	private void LateUpdate()
	{
		if (!m_IsEnabled)
		{
			return;
		}
		CFGSpawnPostProcessCamera component = GetComponent<CFGSpawnPostProcessCamera>();
		if (component != null && component.m_IsBlending)
		{
			return;
		}
		if (IsControlEnabled)
		{
			UpdateInput();
		}
		UpdateFocus();
		if (m_ChangingDistance)
		{
			float t = ((!(m_ChangingFocusTime > 0f)) ? 1f : (m_ChangingTargetDelta / m_ChangingFocusTime));
			m_CurrentDistance = Mathf.Lerp(m_OldDistance, m_DestDistance, t);
			if (!m_ChangingTarget)
			{
				m_ChangingDistance = false;
			}
		}
		if (m_ChangingAngles)
		{
			m_ChangingAnglesDelta += Time.deltaTime;
			if (m_ChangingAnglesDelta >= m_ChangingAnglesTime)
			{
				m_ChangingAnglesDelta = m_ChangingAnglesTime;
				m_ChangingAngles = false;
			}
			float t2 = ((!(m_ChangingAnglesTime > 0f)) ? 1f : (m_ChangingAnglesDelta / m_ChangingAnglesTime));
			m_CurrentAngleH = CFGMath.EaseInOutSineAngle(m_ChangingAnglesDelta, m_OldAngleH, m_DestAngleH - m_OldAngleH, m_ChangingAnglesTime);
			m_CurrentAngleV = Mathf.LerpAngle(m_OldAngleV, m_DestAngleV, t2);
			m_CurrentAngleH = Mathf.Repeat(m_CurrentAngleH, 360f);
			if (!m_ChangingAngles)
			{
				CFGCharacter.UpdateAllCharactersOutliner();
			}
		}
		Quaternion quaternion = Quaternion.Euler(m_CurrentAngleV, m_CurrentAngleH, 0f);
		base.transform.rotation = quaternion;
		if (!m_FreeCam)
		{
			Vector3 currentTargetPoint = m_CurrentTargetPoint;
			currentTargetPoint.y += 2.5f * (float)m_FloorLevelOffset;
			base.transform.position = currentTargetPoint - quaternion * (Vector3.forward * m_CurrentDistance);
			UpdateClippingPlane();
		}
	}

	private void UpdateShake()
	{
		Vector3 currentTargetPoint = m_CurrentTargetPoint;
		Perlin perlin = new Perlin();
		float num = 0.6f;
		currentTargetPoint.x = m_CurrentTargetPoint.x + perlin.Noise(CFGTimer.MissionTime) * num;
		currentTargetPoint.z = m_CurrentTargetPoint.z + perlin.Noise(CFGTimer.MissionTime + 0.1f) * num;
		m_CurrentTargetPoint = currentTargetPoint;
	}

	private void UpdateFollowPoint()
	{
		if (m_FP_Active)
		{
			float num = Vector3.Distance(m_FP_Target, m_CurrentTargetPoint);
			float gamepadCameraMoveSpeed = CFGOptions.Input.GamepadCameraMoveSpeed;
			float gamepadCameraRotateSpeed = CFGOptions.Input.GamepadCameraRotateSpeed;
			float num2 = gamepadCameraMoveSpeed;
			if (num < 2f && num > 0.01f)
			{
				num2 = Mathf.SmoothStep(gamepadCameraMoveSpeed, gamepadCameraMoveSpeed * 0.5f, 2f / num);
			}
			float num3 = Time.deltaTime * num2;
			if (num < 0.01f || num < num3)
			{
				m_CurrentTargetPoint = m_FP_Target;
				m_FP_Active = false;
			}
			else
			{
				Vector3 normalized = (m_FP_Target - m_CurrentTargetPoint).normalized;
				m_FP_Direction = Vector3.RotateTowards(m_FP_Direction, normalized, gamepadCameraRotateSpeed * Time.deltaTime, 0f);
				m_CurrentTargetPoint += m_FP_Direction * num3;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(m_CurrentTargetPoint + Vector3.up, m_FP_Direction);
	}

	private void UpdateFocus()
	{
		if (m_FP_Active)
		{
			UpdateFollowPoint();
		}
		else if (m_ChangingTarget)
		{
			m_ChangingTargetDelta += Time.deltaTime;
			if (m_ChangingTargetDelta >= m_ChangingFocusTime)
			{
				m_ChangingTargetDelta = m_ChangingFocusTime;
				m_ChangingTarget = false;
				OnChangingTargetEnd();
			}
			if (m_CurrentTarget != null)
			{
				m_DestTargetPoint = m_CurrentTarget.Transform.position;
			}
			m_CurrentTargetPoint = CFGMath.EaseInOutSine(m_ChangingTargetDelta, m_OldTargetPoint, m_DestTargetPoint - m_OldTargetPoint, m_ChangingFocusTime);
		}
		else
		{
			if (!(m_CurrentTarget != null))
			{
				return;
			}
			CFGCharacter cFGCharacter = m_CurrentTarget as CFGCharacter;
			if (m_CameraFollowInfo != null)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(m_CurrentTarget.Transform.position);
				Vector3 vector2 = Camera.main.WorldToScreenPoint(m_CurrentTargetPoint);
				vector.z = 0f;
				vector2.z = 0f;
				float a = Mathf.Abs((vector2.x - vector.x) / (float)Screen.width * 2f);
				float b = Mathf.Abs((vector2.y - vector.y) / (float)Screen.height * 2f);
				Vector3 vector3 = m_CurrentTarget.Transform.position - m_CurrentTargetPoint;
				float num = Mathf.Max(a, b);
				Vector3 normalized = vector3.normalized;
				float num2 = 1f;
				if (m_CameraFollowInfo.m_Distance > 0f)
				{
					num2 = Mathf.Pow(Mathf.Clamp(num / m_CameraFollowInfo.m_Distance, 0f, 1f), 2f);
				}
				float num3 = m_CameraFollowInfo.m_MaxSpeed * num2;
				Vector3 position = m_CurrentTarget.transform.position;
				position.y = 0f;
				if (m_CurrentSpeed < num3 && !m_CameraFollowInfo.Contains(m_CurrentTarget.transform.position))
				{
					m_CurrentSpeed += num2 * m_CameraFollowInfo.m_Acceleration * Time.deltaTime;
				}
				else
				{
					m_CurrentSpeed -= 0.5f * m_CameraFollowInfo.m_Acceleration * Time.deltaTime;
				}
				m_CurrentSpeed = Mathf.Clamp(m_CurrentSpeed, 0f, 2f * m_CameraFollowInfo.m_MaxSpeed);
				Vector3 vector4 = normalized * m_CurrentSpeed * Time.deltaTime;
				if (vector4.magnitude < vector3.magnitude)
				{
					m_CurrentTargetPoint += vector4;
				}
				else
				{
					m_CurrentTargetPoint = m_CurrentTarget.Transform.position;
				}
			}
			else
			{
				m_CurrentTargetPoint = m_CurrentTarget.Transform.position;
			}
		}
	}

	private void OnChangingTargetEnd()
	{
		if (m_OnFocusEndCallback != null)
		{
			m_OnFocusEndCallback();
			m_OnFocusEndCallback = null;
		}
	}

	private float GetFocusMinZoom()
	{
		return 0f;
	}

	private void Pan(Vector3 vector)
	{
		ClearFocus();
		CFGLevelSettings levelSettings = CFGSingleton<CFGGame>.Instance.LevelSettings;
		if (levelSettings != null)
		{
			m_CurrentTargetPoint.x = Mathf.Clamp(m_CurrentTargetPoint.x + vector.x, levelSettings.m_MapBorders.xMin, levelSettings.m_MapBorders.xMax);
			m_CurrentTargetPoint.y += vector.y;
			m_CurrentTargetPoint.z = Mathf.Clamp(m_CurrentTargetPoint.z + vector.z, levelSettings.m_MapBorders.yMin, levelSettings.m_MapBorders.yMax);
		}
	}

	private void Move(Vector3 vector)
	{
		Vector3 position = base.transform.position + base.transform.TransformDirection(vector);
		base.transform.position = position;
		UpdateClippingPlane();
	}

	public void MoveToFloor(int _floor)
	{
		_floor = Mathf.Clamp(_floor, 0, 2);
		int num = (int)(_floor - CurrentFloorLevel);
		m_FloorLevelOffset += num;
	}

	private void UpdateClippingPlane()
	{
		CFGSpawnPostProcessCamera component = GetComponent<CFGSpawnPostProcessCamera>();
		if (!(component == null) && !(component.ConstanceClippingPlane == null))
		{
			component.ConstanceClippingPlane.UpdateClippingPlane();
		}
	}

	public void OnGameplayPause()
	{
	}

	public void OnGameplayUnPause()
	{
	}

	public void OnSerialize(CFG_SG_Node camnode)
	{
		int value = 0;
		if ((bool)m_CurrentTarget)
		{
			value = m_CurrentTarget.UniqueID;
		}
		camnode.Attrib_Set("TargetUUID", value);
		camnode.Attrib_Set("Pos", base.transform.position);
		camnode.Attrib_Set("Rot", base.transform.rotation);
		camnode.Attrib_Set("Sight", m_DestDistance);
		camnode.Attrib_Set("Start", m_CurrentTargetPoint);
		camnode.Attrib_Set("AngleV", m_CurrentAngleV);
		camnode.Attrib_Set("AngleH", m_CurrentAngleH);
	}

	public bool OnDeserialize(CFG_SG_Node camnode)
	{
		if (camnode == null)
		{
			return false;
		}
		int uUID = camnode.Attrib_Get("TargetUUID", 0, bReport: false);
		Vector3 position = camnode.Attrib_Get("Pos", Vector3.zero);
		Quaternion rotation = camnode.Attrib_Get("Rot", Quaternion.identity);
		base.transform.position = position;
		base.transform.rotation = rotation;
		float fDistance = camnode.Attrib_Get("Sight", 10f);
		CFGGameObject currentTarget = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGGameObject>(uUID, ESerializableType.NotSerializable);
		m_CurrentTarget = currentTarget;
		ApplyCurrentTransform(fDistance);
		Vector3 currentTargetPoint = camnode.Attrib_Get("Start", m_CurrentTargetPoint);
		m_CurrentTargetPoint = currentTargetPoint;
		m_CurrentAngleV = camnode.Attrib_Get("AngleV", m_CurrentAngleV);
		m_CurrentAngleH = camnode.Attrib_Get("AngleH", m_CurrentAngleH);
		return true;
	}
}
