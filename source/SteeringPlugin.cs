using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SteeringPlugin
{
	public float m_MaxSpeed = 1f;

	public float m_Acc = 5f;

	private float m_CurrentSpeed;

	private float m_CurrentDist;

	private float m_Dist;

	private Vector3 m_StartPOS = Vector2.zero;

	private CFGPathSimplifier m_Path;

	private int m_PathNode;

	private EMoveStatus m_Status;

	private bool m_HadWeapon;

	[NonSerialized]
	public CFGCharacter m_Character;

	protected Vector3 m_Forward = CFGMath.INIFITY3;

	public SteerData m_SteerData = default(SteerData);

	private bool m_ForceRun;

	private bool m_WaitingToReachDest;

	private Vector3 m_DestToReach = CFGMath.INIFITY3;

	private EMoveStatus m_NextMS;

	public CFGPathSimplifier Path => m_Path;

	public SteeringPlugin()
	{
		m_Acc = Mathf.Clamp(m_Acc, 1f, 20f);
		m_Acc = 1f;
	}

	public Vector3 GetNextPosition()
	{
		if (m_PathNode + 1 >= m_Path.m_Positions.Count)
		{
			return CFGMath.INIFITY3;
		}
		return m_Path.m_Positions[m_PathNode + 1];
	}

	public Vector3 GetNextNextPosition()
	{
		if (m_PathNode + 2 >= m_Path.m_Positions.Count)
		{
			return CFGMath.INIFITY3;
		}
		return m_Path.m_Positions[m_PathNode + 2];
	}

	public void InitPath(LinkedList<CFGCell> Path, bool ForceRun)
	{
		m_Path = new CFGPathSimplifier();
		if (m_Path == null)
		{
			return;
		}
		m_Path.Caclculate(Path);
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
			{
				m_Path.CalculateSeeReactions(m_Character, CheckOnly: false);
			}
			else
			{
				m_Path.CalculateReactions(m_Character, CheckOnly: false);
			}
		}
		else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			m_Path.MakeSmooth();
		}
		m_PathNode = 0;
		m_Status = EMoveStatus.Moving;
		if (m_Character != null)
		{
			UpdateMovementStatus();
		}
		m_ForceRun = ForceRun;
		m_WaitingToReachDest = false;
		if (m_Path.m_Positions.Count > 1)
		{
			m_Character.Steering_MoveThroughDoors(m_Path.m_Positions[m_PathNode + 1], m_Path.m_Positions[m_PathNode]);
		}
		m_Dist = 0f;
		m_CurrentDist = 0f;
		m_StartPOS = new Vector3(m_Character.transform.position.x, 0f, m_Character.transform.position.z);
		m_Dist += (m_Path.m_Positions[0] - m_StartPOS).magnitude;
		for (int i = 1; i < m_Path.m_Positions.Count; i++)
		{
			m_Dist += (m_Path.m_Positions[i] - m_Path.m_Positions[i - 1]).magnitude;
		}
	}

	private void CheckIfHadWeapon()
	{
		m_HadWeapon = false;
		if ((bool)m_Character && (bool)m_Character.CurrentWeapon && m_Character.CurrentWeapon.Visualisation != null)
		{
			m_HadWeapon = true;
		}
	}

	public void FinishPath()
	{
		m_Path = null;
		m_PathNode = -1;
		m_Status = EMoveStatus.Stopped;
	}

	public bool ReachedDestination()
	{
		if (m_Path == null || m_PathNode < 0)
		{
			return false;
		}
		if (m_PathNode >= m_Path.m_Positions.Count - 1)
		{
			if (m_WaitingToReachDest)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool ReachedTotalGoal()
	{
		if (m_Path == null || m_Path.m_Positions.Count == 0)
		{
			return true;
		}
		return ReachedGoal(m_Path.m_Positions[m_Path.m_Positions.Count - 1]);
	}

	private bool ReachedGoal(Vector3 Position)
	{
		if (m_Character == null || m_Character.CurrentCell == null)
		{
			return true;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			Vector3 position = m_Character.Transform.position;
			position.y = Position.y;
			return Vector3.Distance(Position, position) < m_MaxSpeed * Time.deltaTime;
		}
		CFGCell cell = CFGCellMap.GetCell(Position);
		if (cell == null)
		{
			return true;
		}
		if (cell == m_Character.CurrentCell)
		{
			Vector3 position2 = m_Character.Transform.position;
			if (cell.StairsType == CFGCell.EStairsType.Slope)
			{
				position2.y = cell.WorldPosition.y;
			}
			return Vector3.Distance(cell.WorldPosition, position2) < m_MaxSpeed * Time.deltaTime;
		}
		return false;
	}

	public void TickPlugin(float deltaTime)
	{
		if (m_Character == null || m_Path == null)
		{
			m_SteerData.m_IsMoving = false;
			m_Status = EMoveStatus.Stopped;
			if (m_Character != null && m_Character.CharacterAnimator == null)
			{
				m_SteerData.m_Speed = 0f;
			}
		}
		else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			HandleMovementOnStrategic(deltaTime);
		}
		else
		{
			if (m_PathNode >= m_Path.m_Positions.Count - 1 && !m_WaitingToReachDest)
			{
				return;
			}
			if (m_Status == EMoveStatus.Stopped)
			{
				UpdateMovementStatus();
			}
			int num = m_PathNode + 1;
			if (num > m_Path.m_Positions.Count - 1)
			{
				num = m_Path.m_Positions.Count - 1;
			}
			Vector3 vector = m_Path.m_Positions[num];
			Vector3 a = m_Path.m_Positions[m_Path.m_Positions.Count - 1];
			Vector3 vector2 = vector;
			vector2.y = m_Character.Transform.position.y;
			Vector2 a2 = new Vector2(vector.x, vector.z);
			Vector2 b = new Vector2(m_Character.Transform.position.x, m_Character.Transform.position.z);
			float num2 = 1f;
			if (!CFGOptions.Gameplay.AlwaysRunOnTactical)
			{
				if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage && !m_ForceRun)
				{
					num2 = 0.502f;
				}
			}
			else
			{
				num2 *= CFGOptions.DevOptions.SetupStateMoveSpeedMul;
			}
			if (m_Character.HasCrippledMovement)
			{
				num2 = 0.502f;
			}
			float num3 = m_MaxSpeed * num2;
			float num4 = Vector3.Distance(a, m_Character.Transform.position);
			if (num4 < 2f)
			{
				num3 = Mathf.Lerp(2f, num3, num4 / 2f);
			}
			else if (m_PathNode < m_Path.m_Positions.Count - 2)
			{
				Vector3 vector3 = GetNextNextPosition() - GetNextPosition();
				if (vector3.magnitude > 0f && Mathf.Abs(Vector3.Dot(Vector3.up, vector3.normalized)) > 0.9f)
				{
					num4 = Vector3.Distance(GetNextPosition(), m_Character.Transform.position);
					if (num4 < 2f)
					{
						num3 = Mathf.Lerp(2f, num3, num4 / 2f);
					}
				}
			}
			if (!m_Character.IsAlive)
			{
				num3 = 0f;
			}
			m_SteerData.m_Speed = num3 / m_MaxSpeed;
			Vector3 vector4 = m_Character.Transform.InverseTransformDirection((vector - m_Character.Transform.position).normalized);
			float num5 = deltaTime * num3;
			if (m_WaitingToReachDest)
			{
				Vector3 a3 = m_Path.m_Positions[m_PathNode - 1];
				Vector3 vector5 = m_Path.m_Positions[m_PathNode];
				float num6 = Vector3.Distance(a3, vector5);
				float num7 = Vector3.Distance(a3, m_Character.transform.position);
				num7 += num5;
				if (m_Status != EMoveStatus.Moving)
				{
					num7 += 0.15f;
				}
				if (num7 < num6)
				{
					if (m_Status != EMoveStatus.Moving)
					{
						return;
					}
					float num8 = Vector2.Distance(a2, b);
					if (m_PathNode == m_Path.m_Positions.Count - 1)
					{
					}
					if (num8 > 0.1f)
					{
						if (num8 > 0.7f)
						{
							vector2 -= m_Character.Transform.position;
							vector2.Normalize();
							vector2 = Vector3.RotateTowards(m_Character.Transform.forward, vector2, Time.deltaTime * 15f, 0f).normalized;
							m_Character.Transform.forward = vector2;
						}
						else
						{
							m_Character.Transform.LookAt(vector2, Vector3.up);
						}
					}
					return;
				}
				m_WaitingToReachDest = false;
				m_DestToReach = CFGMath.INIFITY3;
				m_Character.transform.position = vector5;
				if (m_PathNode == m_Path.m_Positions.Count - 1)
				{
					m_SteerData.m_IsMoving = false;
					m_Status = EMoveStatus.Stopped;
					if (m_Character.CharacterAnimator == null)
					{
						m_SteerData.m_Speed = 0f;
					}
					m_WaitingToReachDest = false;
					return;
				}
				if (m_Status == EMoveStatus.Moving)
				{
					m_Status = m_NextMS;
					if (m_Status != EMoveStatus.Moving)
					{
						m_SteerData.m_IsMoving = false;
					}
				}
			}
			num4 = Vector3.Distance(vector, m_Character.Transform.position);
			bool flag = false;
			float num9 = 0f;
			flag = true;
			bool flag2 = false;
			switch (m_Status)
			{
			case EMoveStatus.Moving:
			{
				float num10 = Vector2.Distance(a2, b);
				if (num10 > 0.1f)
				{
					if (num10 > 0.7f)
					{
						vector2 -= m_Character.Transform.position;
						vector2.Normalize();
						vector2 = Vector3.RotateTowards(m_Character.Transform.forward, vector2, Time.deltaTime * 15f, 0f).normalized;
						m_Character.Transform.forward = vector2;
					}
					else
					{
						m_Character.Transform.LookAt(vector2, Vector3.up);
					}
				}
				m_SteerData.m_IsOnLadder = false;
				if (num5 > num4)
				{
					num9 = num5 - num4;
					num5 = num4;
					if (num10 > 0.001f)
					{
						m_WaitingToReachDest = true;
						m_DestToReach = m_Path.m_Positions[m_PathNode];
						m_NextMS = EMoveStatus.Moving;
					}
					m_PathNode++;
					flag2 = true;
				}
				if (vector4.magnitude < 0.01f)
				{
					m_SteerData.m_IsMoving = false;
					if (m_Character.CharacterAnimator == null)
					{
						m_SteerData.m_Speed = 0f;
					}
					break;
				}
				m_SteerData.m_IsMoving = true;
				if (m_Character.CharacterAnimator == null)
				{
					m_Character.Transform.Translate(vector4 * num5);
					break;
				}
				m_Character.Transform.Translate(new Vector3(0f, vector4.y * num5, 0f));
				m_Character.CharacterAnimator.WantedDirection = m_Character.Transform.forward;
				break;
			}
			case EMoveStatus.RotatingToLadder_Up:
			case EMoveStatus.RotatingToLadder_Down:
				CheckRotation();
				break;
			case EMoveStatus.HolsteringWeapon_Up:
			case EMoveStatus.HolsteringWeapon_Down:
				m_SteerData.m_IsMoving = false;
				m_SteerData.m_IsOnLadder = false;
				if (m_Status == EMoveStatus.HolsteringWeapon_Down)
				{
					m_Status = EMoveStatus.WaitingForHolsterAnim_Down;
				}
				else
				{
					m_Status = EMoveStatus.WaitingForHolsterAnim_Up;
				}
				CheckIfHadWeapon();
				if (m_Character.CurrentWeapon == null || m_Character.CurrentWeapon.Visualisation == null)
				{
					OnWeaponHolsterAnimEnd();
				}
				else
				{
					m_Character.CharacterAnimator.PlayWeaponChange0(RemoveWeapon);
				}
				break;
			case EMoveStatus.WaitingForHolsterAnim_Up:
			case EMoveStatus.WaitingForWeaponUp:
			case EMoveStatus.WaitingForHolsterAnim_Down:
			case EMoveStatus.WaitingForWeaponDown:
				m_SteerData.m_IsMoving = false;
				m_SteerData.m_IsOnLadder = false;
				if (m_Character.CharacterAnimator.IsInIdle())
				{
					OnWeaponHolsterAnimEnd();
				}
				break;
			case EMoveStatus.StartClimbingUp:
				if (m_Character.CharacterAnimator.IsInIdle())
				{
					m_Status = EMoveStatus.Climbing_Up;
					m_SteerData.m_IsOnLadder = true;
					m_SteerData.m_IsMoving = false;
					if (m_HadWeapon)
					{
						m_Character.CharacterAnimator.PlayLadderUp(OnLadderUpEnd_Weapon);
					}
					else
					{
						m_Character.CharacterAnimator.PlayLadderUp(OnLadderUpEnd);
					}
				}
				break;
			case EMoveStatus.StartClimbingDown:
				if (m_Character.CharacterAnimator.IsInIdle())
				{
					m_SteerData.m_IsOnLadder = true;
					m_SteerData.m_IsMoving = false;
					m_Status = EMoveStatus.Climbing_Down;
					if (m_HadWeapon)
					{
						m_Character.CharacterAnimator.PlayLadderDown(OnLadderDownEnd_Weapon);
					}
					else
					{
						m_Character.CharacterAnimator.PlayLadderDown(OnLadderDownEnd);
					}
				}
				break;
			case EMoveStatus.Climbing_Up:
			case EMoveStatus.Climbing_Down:
				m_SteerData.m_Speed = 0f;
				m_SteerData.m_IsMoving = false;
				m_SteerData.m_IsOnLadder = true;
				break;
			case EMoveStatus.RotatingAfterClimb:
				CheckRotation();
				break;
			}
			if (flag2)
			{
				UpdateMovementStatus();
				if (m_Status == EMoveStatus.Moving && m_PathNode + 1 < m_Path.m_Positions.Count)
				{
					m_Character.Steering_MoveThroughDoors(m_Path.m_Positions[m_PathNode + 1], m_Path.m_Positions[m_PathNode]);
				}
				if (m_Status != EMoveStatus.Moving && m_WaitingToReachDest)
				{
					m_NextMS = m_Status;
					m_Status = EMoveStatus.Moving;
					m_SteerData.m_IsMoving = true;
				}
			}
			if (m_Path != null && m_PathNode >= m_Path.m_Positions.Count - 1 && !m_WaitingToReachDest)
			{
				m_SteerData.m_IsMoving = false;
				m_Status = EMoveStatus.Stopped;
				if (m_Character.CharacterAnimator == null)
				{
					m_SteerData.m_Speed = 0f;
				}
			}
		}
	}

	private void OnWeaponHolsterAnimEnd()
	{
		Debug.Log(string.Concat("On holster end ", m_Status, " time  ", Time.time));
		switch (m_Status)
		{
		case EMoveStatus.WaitingForHolsterAnim_Down:
			RemoveWeapon();
			m_Status = EMoveStatus.StartClimbingDown;
			break;
		case EMoveStatus.WaitingForHolsterAnim_Up:
			RemoveWeapon();
			m_Status = EMoveStatus.StartClimbingUp;
			break;
		case EMoveStatus.WaitingForWeaponUp:
			SpawnWeapon();
			OnLadderUpEnd();
			break;
		case EMoveStatus.WaitingForWeaponDown:
			SpawnWeapon();
			OnLadderDownEnd();
			break;
		}
	}

	private void SpawnWeapon()
	{
		if ((bool)m_Character.CurrentWeapon)
		{
			m_Character.CurrentWeapon.SpawnVisualisation(m_Character.RightHand);
		}
	}

	private void RemoveWeapon()
	{
		if ((bool)m_Character.CurrentWeapon)
		{
			m_Character.CurrentWeapon.RemoveVisualisation();
		}
	}

	private void OnLadderDownEnd()
	{
		Debug.Log("On ladder down end " + Time.time);
		m_PathNode += 2;
		if (m_PathNode >= m_Path.m_Positions.Count - 1)
		{
			m_Status = EMoveStatus.Moving;
			return;
		}
		EMoveStatus eMoveStatus = NextMoveType();
		if (eMoveStatus == EMoveStatus.Moving)
		{
			m_Forward = m_Path.m_Positions[m_PathNode + 1] - m_Path.m_Positions[m_PathNode];
			m_Forward.y = 0f;
			m_Forward.Normalize();
			m_Character.CharacterAnimator.WantedDirection = m_Forward;
			m_Status = EMoveStatus.RotatingAfterClimb;
		}
	}

	private void OnLadderDownEnd_Weapon()
	{
		m_Status = EMoveStatus.WaitingForWeaponDown;
		if (m_Character.CurrentWeapon.TwoHanded)
		{
			m_Character.CharacterAnimator.PlayWeaponChange2(null, SpawnWeapon);
		}
		else
		{
			m_Character.CharacterAnimator.PlayWeaponChange1(null, SpawnWeapon);
		}
	}

	private void OnLadderUpEnd_Weapon()
	{
		m_Status = EMoveStatus.WaitingForWeaponUp;
		if (m_Character.CurrentWeapon.TwoHanded)
		{
			m_Character.CharacterAnimator.PlayWeaponChange2(null, SpawnWeapon);
		}
		else
		{
			m_Character.CharacterAnimator.PlayWeaponChange1(null, SpawnWeapon);
		}
	}

	private void OnLadderUpEnd()
	{
		Debug.Log("On ladder up end " + Time.time);
		m_PathNode++;
		if (m_PathNode >= m_Path.m_Positions.Count - 1)
		{
			m_Status = EMoveStatus.Moving;
			return;
		}
		EMoveStatus eMoveStatus = NextMoveType();
		if (eMoveStatus == EMoveStatus.Moving)
		{
			m_Forward = m_Path.m_Positions[m_PathNode + 1] - m_Path.m_Positions[m_PathNode];
			m_Forward.y = 0f;
			m_Forward.Normalize();
			m_Status = EMoveStatus.RotatingAfterClimb;
			m_Character.CharacterAnimator.WantedDirection = m_Forward;
		}
	}

	private void CheckRotation()
	{
		if (m_Forward.y != 0f)
		{
			Debug.LogError("Failed rotation: " + m_Forward);
			m_Status = EMoveStatus.Moving;
			return;
		}
		float num = Vector3.Dot(m_Character.Transform.forward, m_Forward);
		if (num > 0.98f && m_Character.CharacterAnimator.IsInIdle())
		{
			switch (m_Status)
			{
			case EMoveStatus.RotatingToLadder_Down:
				m_Status = EMoveStatus.HolsteringWeapon_Down;
				break;
			case EMoveStatus.RotatingToLadder_Up:
				m_Status = EMoveStatus.HolsteringWeapon_Up;
				break;
			case EMoveStatus.RotatingAfterClimb:
				m_Status = EMoveStatus.Moving;
				break;
			}
		}
	}

	private void UpdateMovementStatus()
	{
		switch (m_Status)
		{
		case EMoveStatus.Stopped:
			if (m_SteerData.m_IsMoving && m_Path != null && m_Path.m_Positions != null && m_Path.m_Positions.Count > 0 && m_PathNode < m_Path.m_Positions.Count - 1)
			{
				m_Status = EMoveStatus.Moving;
				UpdateMovementStatus();
			}
			m_SteerData.m_IsOnLadder = false;
			break;
		case EMoveStatus.Climbing_Up:
		case EMoveStatus.Climbing_Down:
			m_SteerData.m_IsOnLadder = true;
			m_SteerData.m_IsMoving = false;
			m_Status = NextMoveType();
			if (m_Status != EMoveStatus.Moving)
			{
				UpdateMovementStatus();
				break;
			}
			m_Forward = m_Path.m_Positions[m_PathNode + 1] - m_Path.m_Positions[m_PathNode];
			m_Forward.y = 0f;
			m_Forward.Normalize();
			m_Status = EMoveStatus.RotatingAfterClimb;
			m_Character.CharacterAnimator.WantedDirection = m_Forward;
			m_Character.CharacterAnimator.SetDirectionDiff(0f);
			break;
		case EMoveStatus.Moving:
			m_SteerData.m_IsOnLadder = false;
			m_Status = NextMoveType();
			if (m_Status != EMoveStatus.Moving)
			{
				UpdateMovementStatus();
			}
			if (m_Status != EMoveStatus.Moving)
			{
				m_SteerData.m_IsMoving = false;
			}
			break;
		case EMoveStatus.RotatingToLadder_Up:
		case EMoveStatus.RotatingToLadder_Down:
			m_SteerData.m_IsMoving = false;
			m_SteerData.m_IsOnLadder = false;
			m_Character.CharacterAnimator.WantedDirection = m_Forward;
			m_Character.CharacterAnimator.SetDirectionDiff(0f);
			break;
		case EMoveStatus.HolsteringWeapon_Up:
		case EMoveStatus.HolsteringWeapon_Down:
			m_SteerData.m_IsOnLadder = false;
			break;
		case EMoveStatus.WaitingForHolsterAnim_Up:
		case EMoveStatus.StartClimbingUp:
		case EMoveStatus.WaitingForWeaponUp:
		case EMoveStatus.WaitingForHolsterAnim_Down:
		case EMoveStatus.StartClimbingDown:
			break;
		}
	}

	private EMoveStatus NextMoveType()
	{
		if (m_PathNode + 1 >= m_Path.m_Positions.Count)
		{
			return EMoveStatus.Moving;
		}
		CFGCell currentCell = m_Character.CurrentCell;
		if (currentCell == null)
		{
			return EMoveStatus.Moving;
		}
		float num = Vector3.Dot((m_Path.m_Positions[m_PathNode + 1] - m_Path.m_Positions[m_PathNode]).normalized, Vector3.up);
		if (num > 0.95f)
		{
			if (!currentCell.CheckFlag(1, 64))
			{
				return EMoveStatus.Moving;
			}
			m_Forward = currentCell.GetToLadderDirection();
			if (m_Forward.y != 0f)
			{
				return EMoveStatus.Moving;
			}
			return EMoveStatus.RotatingToLadder_Up;
		}
		if (m_PathNode + 2 >= m_Path.m_Positions.Count)
		{
			return EMoveStatus.Moving;
		}
		currentCell = CFGCellMap.GetCell(m_Path.m_Positions[m_PathNode + 1]);
		if (!currentCell.CheckFlag(1, 64))
		{
			return EMoveStatus.Moving;
		}
		num = Vector3.Dot((m_Path.m_Positions[m_PathNode + 2] - m_Path.m_Positions[m_PathNode + 1]).normalized, Vector3.down);
		if (num > 0.95f)
		{
			m_Forward = -currentCell.GetToLadderDirection();
			if (m_Forward.y != 0f)
			{
				return EMoveStatus.Moving;
			}
			return EMoveStatus.RotatingToLadder_Down;
		}
		return EMoveStatus.Moving;
	}

	private void CalculateSpeed(float _currentDist, float _dist, float _deltaTime)
	{
		float num = m_CurrentSpeed / m_Acc;
		float num2 = m_CurrentSpeed * num + m_Acc * num * num / 2f;
		float num3 = _dist - _currentDist;
		float num4 = ((!CFGOptions.Gameplay.PawnSuperSpeed) ? m_MaxSpeed : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_HorsemanFastMaxSpeed);
		float num5 = ((!CFGOptions.Gameplay.PawnSuperSpeed) ? m_Acc : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_HorsemanFastAcc);
		if (num2 < 0.9f * num3)
		{
			m_CurrentSpeed += num5 * _deltaTime;
			if (m_CurrentSpeed > num4)
			{
				m_CurrentSpeed = num4;
			}
		}
		else
		{
			m_CurrentSpeed -= 0.5f * num5 * _deltaTime;
			if (m_CurrentSpeed < 1.5f)
			{
				m_CurrentSpeed = 1.5f;
			}
		}
	}

	private void HandleMovementOnStrategic(float deltaTime)
	{
		m_SteerData.m_IsMoving = true;
		float num = 1f;
		float num2 = ((!CFGOptions.Gameplay.PawnSuperSpeed) ? m_MaxSpeed : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_HorsemanFastMaxSpeed);
		float num3 = num2 * num;
		if (m_PathNode >= m_Path.m_Positions.Count - 1)
		{
			return;
		}
		Vector3 vector = m_Path.m_Positions[m_PathNode + 1];
		Vector3 vector2 = m_Path.m_Positions[m_Path.m_Positions.Count - 1];
		vector2.y = m_Character.Transform.position.y;
		Vector3 vector3 = new Vector3(m_Character.transform.position.x, 0f, m_Character.transform.position.z);
		if (m_PathNode == 0)
		{
			m_CurrentDist = (m_StartPOS - vector3).magnitude;
		}
		else
		{
			m_CurrentDist = (m_StartPOS - m_Path.m_Positions[0]).magnitude;
			m_CurrentDist += (vector3 - m_Path.m_Positions[m_PathNode]).magnitude;
		}
		for (int i = 1; i < m_PathNode + 1; i++)
		{
			m_CurrentDist += (m_Path.m_Positions[i] - m_Path.m_Positions[i - 1]).magnitude;
		}
		CalculateSpeed(m_CurrentDist, m_Dist, deltaTime);
		if (!CFGOptions.Gameplay.PawnSuperSpeed)
		{
			num3 = m_CurrentSpeed;
		}
		m_SteerData.m_Speed = num3 / num2;
		m_Character.transform.rotation = Quaternion.identity;
		float num4 = deltaTime * num3;
		float num5 = Vector3.Distance(vector, vector3);
		do
		{
			vector3 = new Vector3(m_Character.transform.position.x, 0f, m_Character.transform.position.z);
			num5 = Vector3.Distance(vector, vector3);
			float num6 = num4;
			if (num6 > num5)
			{
				num6 = num5;
			}
			Vector3 normalized = (vector - m_Character.Transform.position).normalized;
			normalized.y = 0f;
			normalized.Normalize();
			bool flag = false;
			if (CFGSingleton<CFGGame>.Instance.LevelSettings != null)
			{
				Rect mapBorders = CFGSingleton<CFGGame>.Instance.LevelSettings.m_MapBorders;
				Vector2 point = new Vector2(m_Character.Position.x, m_Character.Position.z);
				float num7 = 1f;
				mapBorders.xMin += num7;
				mapBorders.xMax -= num7;
				mapBorders.yMin += num7;
				mapBorders.yMax -= num7;
				if (!mapBorders.Contains(point))
				{
					flag = true;
				}
			}
			if (normalized.magnitude < 0.01f)
			{
				m_SteerData.m_IsMoving = false;
				if (m_Character.CharacterAnimator == null)
				{
					m_SteerData.m_Speed = 0f;
				}
				m_CurrentSpeed = 0f;
				num4 = -1f;
				continue;
			}
			if (m_Forward.x == float.PositiveInfinity)
			{
				m_Forward = normalized;
			}
			else if (num5 < 1f || flag)
			{
				m_Forward = normalized;
			}
			else
			{
				float maxRadiansDelta = 15f * Time.deltaTime;
				m_Forward = Vector3.RotateTowards(m_Forward, normalized, maxRadiansDelta, 1f);
				m_Forward.y = 0f;
				m_Forward.Normalize();
			}
			if (m_Character.CharacterAnimator == null)
			{
				m_Character.Transform.Translate(m_Forward * num6);
			}
			else
			{
				m_Character.Transform.Translate(new Vector3(0f, m_Forward.y * num6, 0f));
				m_Character.CharacterAnimator.WantedDirection = m_Character.Transform.forward;
			}
			num4 -= num5;
			if (num4 > 0f)
			{
				m_PathNode++;
				if (m_PathNode < m_Path.m_Positions.Count - 1)
				{
					vector = m_Path.m_Positions[m_PathNode + 1];
				}
			}
		}
		while (num4 > 0f);
	}
}
