using System.Collections.Generic;
using UnityEngine;

public class CFGGrenadePathVis : MonoBehaviour
{
	[SerializeField]
	private float m_Thickness = 0.075f;

	private float m_PathLength;

	private Mesh m_MeshPath;

	private Material m_ActiveMat;

	private Material m_InactiveMat;

	private CFGGrenadePath m_Path = new CFGGrenadePath();

	private bool m_bTargetPointValid = true;

	public bool m_IsTargetInRange;

	private void Start()
	{
		m_MeshPath = new Mesh();
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActiveGrenadePathMat)
		{
			m_ActiveMat = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_ActiveGrenadePathMat);
		}
		if ((bool)CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_InactiveGrenadePathMat)
		{
			m_InactiveMat = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_InactiveGrenadePathMat);
		}
	}

	public void RegenerateMesh(float MaxDistance, bool TargetIsValid)
	{
		m_MeshPath.Clear(keepVertexLayout: false);
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (CFGCellMap.IsValid && !(component == null) && component.CurrentAction != ETurnAction.None && component.CellUnderCursor != null && !(component.SelectedCharacter == null) && !component.SelectedCharacter.Imprisoned && component.SelectedCharacter.GunpointState == EGunpointState.None && CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			m_Path.Calculate(component.SelectedCharacter.Position, component.CellUnderCursor.WorldPosition, MaxDistance);
			m_bTargetPointValid = TargetIsValid;
			GenerateMesh(m_MeshPath, m_Path.m_Waypoints);
			if (m_ActiveMat != null)
			{
				m_ActiveMat.SetFloat("_CurveLength", m_PathLength);
			}
			if (m_InactiveMat != null)
			{
				m_InactiveMat.SetFloat("_CurveLength", m_PathLength);
			}
		}
	}

	public void ClearMesh()
	{
		if (m_MeshPath != null)
		{
			m_MeshPath.Clear(keepVertexLayout: false);
		}
	}

	private void Update()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null) && !(component.SelectedCharacter == null) && component.CurrentAction != ETurnAction.None && m_IsTargetInRange)
		{
			if (m_bTargetPointValid)
			{
				Graphics.DrawMesh(m_MeshPath, Vector3.zero, Quaternion.identity, m_ActiveMat, 1, null, 0, null);
			}
			else
			{
				Graphics.DrawMesh(m_MeshPath, Vector3.zero, Quaternion.identity, m_InactiveMat, 1, null, 0, null);
			}
		}
	}

	private void GenerateMesh(Mesh mesh, List<Vector3> points)
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		int num = 0;
		m_PathLength = 0f;
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector3 vector = points[i];
			Vector3 vector2 = points[i + 1];
			Vector3 vector3 = Vector3.Normalize(vector2 - vector);
			Vector3 vector4 = ((i <= 0) ? (vector - vector3) : points[i - 1]);
			Vector3 vector5 = ((i >= points.Count - 2) ? (vector2 + vector3) : points[i + 2]);
			Vector3 vector6 = Vector3.Normalize(vector4 - vector);
			Vector3 vector7 = ((vector3.x != 0f || vector3.z != 0f) ? Vector3.Normalize(Vector3.Cross(vector3, Vector3.up)) : Vector3.right);
			Vector3 vector8;
			if ((vector6.x == 0f && vector6.z == 0f) || (vector3.x == 0f && vector3.z == 0f))
			{
				vector8 = vector7 * m_Thickness;
			}
			else
			{
				float num2 = Vector2.Dot(new Vector2(vector6.x, vector6.z).normalized, new Vector2(vector3.x, vector3.z).normalized);
				if (num2 > -0.999f && num2 < 0.999f)
				{
					vector8 = Vector3.Normalize(vector6 + vector3);
					if (Vector3.Cross(vector6, vector3).y < 0f)
					{
						vector8 = -vector8;
					}
					vector8 *= m_Thickness / Vector3.Dot(vector7, vector8);
				}
				else
				{
					vector8 = vector7 * m_Thickness;
				}
			}
			Vector3 vector9 = Vector3.Normalize(vector - vector2);
			Vector3 vector10 = Vector3.Normalize(vector5 - vector2);
			Vector3 vector11;
			if ((vector9.x == 0f && vector9.z == 0f) || (vector10.x == 0f && vector10.z == 0f))
			{
				vector11 = vector7 * m_Thickness;
			}
			else
			{
				float num3 = Vector2.Dot(new Vector2(vector9.x, vector9.z).normalized, new Vector2(vector10.x, vector10.z).normalized);
				if (num3 > -0.999f && num3 < 0.999f)
				{
					vector11 = Vector3.Normalize(vector9 + vector10);
					if (Vector3.Cross(vector9, vector10).y < 0f)
					{
						vector11 = -vector11;
					}
					vector11 *= m_Thickness / Vector3.Dot(vector7, vector11);
				}
				else
				{
					vector11 = vector7 * m_Thickness;
				}
			}
			list.Add(vector + vector8);
			list.Add(vector2 + vector11);
			list.Add(vector2);
			list.Add(vector);
			float num4 = m_PathLength + (vector2 - vector).magnitude;
			Vector2 item = new Vector2(m_PathLength, 1f);
			Vector2 item2 = new Vector2(num4, 1f);
			Vector2 item3 = new Vector2(num4, -1f);
			Vector2 item4 = new Vector2(m_PathLength, -1f);
			list2.Add(item);
			list2.Add(item2);
			list2.Add(item3);
			list2.Add(item4);
			m_PathLength = num4;
			list3.Add(num);
			list3.Add(num + 1);
			list3.Add(num + 2);
			list3.Add(num);
			list3.Add(num + 2);
			list3.Add(num + 3);
			list3.Add(num);
			list3.Add(num + 2);
			list3.Add(num + 1);
			list3.Add(num);
			list3.Add(num + 3);
			list3.Add(num + 2);
			num += 4;
		}
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
	}
}
