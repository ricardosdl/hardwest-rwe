using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CFGTestCameraWalker : MonoBehaviour
{
	[SerializeField]
	private float m_MoveSpeed = 10f;

	[SerializeField]
	private float m_RotationSpeed = 180f;

	[SerializeField]
	private Terrain m_Terrain;

	private Vector3 m_LastPos = Vector3.zero;

	private float m_LastY;

	private List<CamTransformUpdateInfo> m_CamTransformUpdateInfo = new List<CamTransformUpdateInfo>();

	public Terrain Terrain
	{
		get
		{
			return m_Terrain;
		}
		private set
		{
			m_Terrain = value;
		}
	}

	private void Start()
	{
		m_LastPos = base.transform.position;
		if ((bool)Terrain)
		{
			m_LastY = Terrain.SampleHeight(base.transform.position);
		}
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.Z, KeyCode.Insert, Vector3.zero, 0f, Vector3.up, 0f - m_RotationSpeed));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.X, KeyCode.Delete, Vector3.zero, 0f, Vector3.up, m_RotationSpeed));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.A, KeyCode.LeftArrow, Vector3.left, m_MoveSpeed, Vector3.up, 0f));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.D, KeyCode.RightArrow, Vector3.right, m_MoveSpeed, Vector3.up, 0f));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.W, KeyCode.UpArrow, Vector3.forward, m_MoveSpeed, Vector3.up, 0f));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.S, KeyCode.DownArrow, Vector3.back, m_MoveSpeed, Vector3.up, 0f));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.E, KeyCode.PageUp, Vector3.up, m_MoveSpeed, Vector3.up, 0f));
		m_CamTransformUpdateInfo.Add(new CamTransformUpdateInfo(base.transform, KeyCode.Q, KeyCode.PageDown, Vector3.down, m_MoveSpeed, Vector3.up, 0f));
	}

	private void Update()
	{
		m_LastPos = base.transform.position;
		foreach (CamTransformUpdateInfo item in m_CamTransformUpdateInfo)
		{
			item.UpdateRotation();
		}
		Quaternion rotation = base.transform.rotation;
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		foreach (CamTransformUpdateInfo item2 in m_CamTransformUpdateInfo)
		{
			item2.UpdateMove();
		}
		base.transform.rotation = rotation;
		if ((bool)Terrain)
		{
			Vector3 vector = base.transform.position;
			Vector3 position = Terrain.GetPosition();
			Vector3 size = Terrain.terrainData.size;
			if (vector.x < position.x + size.x && vector.x > position.x && vector.z < position.z + size.z && vector.z > position.z)
			{
				float num = m_Terrain.SampleHeight(vector);
				vector.y += num - m_LastY;
				m_LastY = num;
			}
			else
			{
				vector = m_LastPos;
			}
			base.transform.position = vector;
		}
	}
}
