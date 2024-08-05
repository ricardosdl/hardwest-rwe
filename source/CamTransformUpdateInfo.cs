using System;
using UnityEngine;

internal class CamTransformUpdateInfo
{
	private Transform m_Transform;

	private KeyCode m_FirstKey;

	private KeyCode m_SecondKey;

	private Vector3 m_Dir = Vector3.zero;

	private float m_MoveSpeed;

	private Vector3 m_Axis = Vector3.up;

	private float m_RotationSpeed;

	public CamTransformUpdateInfo(Transform transform, KeyCode first_key, KeyCode second_key, Vector3 dir, float move_speed, Vector3 axis, float rotation_speed)
	{
		m_FirstKey = first_key;
		m_SecondKey = second_key;
		m_Dir = dir;
		m_MoveSpeed = move_speed;
		m_Axis = axis;
		m_RotationSpeed = rotation_speed * ((float)Math.PI / 180f);
		m_Transform = transform;
	}

	private bool IsInput()
	{
		return Input.GetKey(m_FirstKey) || Input.GetKey(m_SecondKey);
	}

	public void UpdateMove()
	{
		if (IsInput())
		{
			m_Transform.Translate(m_Dir * m_MoveSpeed * Time.deltaTime);
		}
	}

	public void UpdateRotation()
	{
		if (IsInput())
		{
			m_Transform.Rotate(m_Axis, m_RotationSpeed * Time.deltaTime);
		}
	}
}
