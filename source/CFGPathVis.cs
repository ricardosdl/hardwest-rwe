using System.Collections.Generic;
using UnityEngine;

public class CFGPathVis : MonoBehaviour
{
	[SerializeField]
	private float m_Thickness = 0.075f;

	private float m_PathLength;

	private CFGPathSimplifier m_PS = new CFGPathSimplifier();

	private Mesh m_MeshPath;

	private Mesh m_MeshPathOrtho;

	private Material m_Mat;

	private Material m_MatOrtho;

	private bool m_Use2ApMat;

	public void RegenerateMesh()
	{
		if (m_MeshPath == null)
		{
			return;
		}
		m_MeshPath.Clear(keepVertexLayout: false);
		m_MeshPathOrtho.Clear(keepVertexLayout: false);
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!CFGCellMap.IsValid || component == null || component.CurrentAction != ETurnAction.None || component.TargetedCharacter != null || component.CellUnderCursor == null || component.SelectedCharacter == null || component.SelectedCharacter.ActionPoints == 0 || component.SelectedCharacter.Imprisoned || component.SelectedCharacter.GunpointState != 0 || !CFGSingletonResourcePrefab<CFGTurnManager>.Instance.IsPlayerTurn)
		{
			return;
		}
		NavigationComponent component2 = component.SelectedCharacter.GetComponent<NavigationComponent>();
		if (component2 == null)
		{
			return;
		}
		LinkedList<CFGCell> path = null;
		HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(component.SelectedCharacter.CurrentCell, (component.SelectedCharacter.ActionPoints != 2) ? component.SelectedCharacter.HalfMovement : component.SelectedCharacter.DashingMovement);
		bool flag = true;
		CFGUsableObject usableObject = component.CellUnderCursor.UsableObject;
		if (usableObject != null)
		{
			if (component.PlayerActionLimiter == EPlayerActionLimit.Default || (component.PlayerActionLimiter == EPlayerActionLimit.Usable && usableObject == component.PlayerActionLimiterUsable))
			{
				path = usableObject.GetBestPathTo(component.SelectedCharacter);
				if (path != null)
				{
					flag = false;
				}
			}
		}
		else if (component.PlayerActionLimiter == EPlayerActionLimit.Default)
		{
			CFGDoorObject doorObject = component.CellUnderCursor.DoorObject;
			if (doorObject != null)
			{
				path = doorObject.GetBestPathTo(component.SelectedCharacter);
				if (path != null)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			if ((component.PlayerActionLimiter != 0 && (component.PlayerActionLimiter != EPlayerActionLimit.MoveToTile || component.CellUnderCursor != component.PlayerActionLimiterCell)) || !hashSet.Contains(component.CellUnderCursor))
			{
				return;
			}
			NavGoal_At navGoal_At = new NavGoal_At(component.CellUnderCursor);
			if (!component2.GeneratePath(component.SelectedCharacter.CurrentCell, new NavGoalEvaluator[1] { navGoal_At }, out path))
			{
				return;
			}
		}
		if (path != null)
		{
			m_PS.m_Positions.Clear();
			if (CFGOptions.DevOptions.RAWPath)
			{
				m_PS.MakeCopy(path);
			}
			else
			{
				m_PS.Caclculate(path);
			}
			component.SpawnReactionShotHelpers(m_PS);
			if (CFGOptions.DevOptions.SmoothPathVisualization)
			{
				m_PS.MakeSmooth();
			}
			GenerateMesh(m_MeshPath, m_PS.m_Positions, 0f);
			GenerateMesh(m_MeshPathOrtho, m_PS.m_Positions, 90f);
			if (m_Mat != null)
			{
				m_Mat.SetFloat("_CurveLength", m_PathLength);
			}
			if (m_MatOrtho != null)
			{
				m_MatOrtho.SetFloat("_CurveLength", m_PathLength);
			}
		}
	}

	public void ClearMesh()
	{
		if (m_MeshPath != null)
		{
			m_MeshPath.Clear(keepVertexLayout: false);
		}
		if (m_MeshPathOrtho != null)
		{
			m_MeshPathOrtho.Clear(keepVertexLayout: false);
		}
	}

	private void Start()
	{
		m_MeshPath = new Mesh();
		m_MeshPathOrtho = new Mesh();
		m_Mat = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat);
		m_MatOrtho = new Material(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat);
		Color color = m_Mat.GetColor("_TintCol");
		color.a = 0f;
		m_MatOrtho.SetColor("_HighlightCol", color);
	}

	private void Update()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null) && !(component.SelectedCharacter == null) && component.CurrentAction == ETurnAction.None && CFGRangeBorders.s_DrawRangeBorders)
		{
			CFGCharacter selectedCharacter = CFGSelectionManager.Instance.SelectedCharacter;
			if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage || (selectedCharacter != null && selectedCharacter.ActionPoints == 1))
			{
				ManageMaterial(_twoAp: true);
			}
			else
			{
				ManageMaterial(CFGRangeBorders.s_ShowAp2Region);
			}
			Graphics.DrawMesh(m_MeshPath, new Vector3(0f, 0.15f, 0f), Quaternion.identity, m_Mat, 1);
			Graphics.DrawMesh(m_MeshPathOrtho, new Vector3(0f, 0.15f, 0f), Quaternion.identity, m_MatOrtho, 1);
		}
	}

	public void ManageMaterial(bool _twoAp)
	{
		if (!(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat == null) && !(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat2 == null) && !(m_Mat == null) && !(m_MatOrtho == null) && m_Use2ApMat != _twoAp)
		{
			m_Use2ApMat = _twoAp;
			Material material = ((!_twoAp) ? CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat : CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_PathVisMat2);
			Color color = material.GetColor("_TintCol");
			Color color2 = material.GetColor("_ImpulseCol");
			Color color3 = material.GetColor("_HighlightCol");
			float @float = material.GetFloat("_ImpulseLength");
			float float2 = material.GetFloat("_Speed");
			float float3 = material.GetFloat("_Delay");
			float float4 = material.GetFloat("_Atten");
			m_Mat.SetColor("_TintCol", color);
			m_Mat.SetColor("_ImpulseCol", color2);
			m_Mat.SetColor("_HighlightCol", color3);
			m_Mat.SetFloat("_ImpulseLength", @float);
			m_Mat.SetFloat("_Speed", float2);
			m_Mat.SetFloat("_Delay", float3);
			m_Mat.SetFloat("_Atten", float4);
			color.a = 0f;
			m_MatOrtho.SetColor("_TintCol", color);
			m_MatOrtho.SetColor("_ImpulseCol", color2);
			m_MatOrtho.SetColor("_HighlightCol", color3);
			m_MatOrtho.SetFloat("_ImpulseLength", @float);
			m_MatOrtho.SetFloat("_Speed", float2);
			m_MatOrtho.SetFloat("_Delay", float3);
			m_MatOrtho.SetFloat("_Atten", float4);
		}
	}

	private void GenerateMesh(Mesh mesh, List<Vector3> points, float angle)
	{
		m_PathLength = 0f;
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		int num = 0;
		GameObject gameObject = new GameObject();
		gameObject.transform.up = Vector3.up;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector3 vector3 = points[i];
			Vector3 vector4 = points[i + 1];
			Vector3 vector5 = Vector3.Normalize(vector4 - vector3);
			Vector3 vector6 = ((i <= 0) ? (vector3 - vector5) : points[i - 1]);
			Vector3 vector7 = ((i >= points.Count - 2) ? (vector4 + vector5) : points[i + 2]);
			Vector3 vector8 = Vector3.Normalize(vector6 - vector3);
			gameObject.transform.position = vector3;
			gameObject.transform.LookAt(vector4);
			Vector3 vector9 = Vector3.Normalize(Vector3.Cross(vector5, Quaternion.AngleAxis(angle, gameObject.transform.forward) * gameObject.transform.up));
			Vector3 vector10;
			if ((vector8.x == 0f && vector8.z == 0f) || (vector5.x == 0f && vector5.z == 0f))
			{
				vector10 = vector9 * m_Thickness;
			}
			else
			{
				float num2 = Vector2.Dot(new Vector2(vector8.x, vector8.z).normalized, new Vector2(vector5.x, vector5.z).normalized);
				if (num2 > -0.999f && num2 < 0.999f)
				{
					vector10 = Vector3.Normalize(Vector3.Cross(vector5, Quaternion.AngleAxis(angle - 90f, vector5) * gameObject.transform.right));
					vector10 *= m_Thickness / Vector3.Dot(vector9, vector10);
				}
				else
				{
					vector10 = vector9 * m_Thickness;
				}
			}
			Vector3 vector11 = Vector3.Normalize(vector3 - vector4);
			Vector3 vector12 = Vector3.Normalize(vector7 - vector4);
			Vector3 vector13;
			if ((vector11.x == 0f && vector11.z == 0f) || (vector12.x == 0f && vector12.z == 0f))
			{
				vector13 = vector9 * m_Thickness;
			}
			else
			{
				float num3 = Vector2.Dot(new Vector2(vector11.x, vector11.z).normalized, new Vector2(vector12.x, vector12.z).normalized);
				if (num3 > -0.999f && num3 < 0.999f)
				{
					vector13 = Vector3.Normalize(Vector3.Cross(vector5, Quaternion.AngleAxis(angle - 90f, vector5) * gameObject.transform.right));
					vector13 *= m_Thickness / Vector3.Dot(vector9, vector13);
				}
				else
				{
					vector13 = vector9 * m_Thickness;
				}
			}
			if ((vector + vector2 - (vector3 + vector10)).magnitude > m_Thickness && vector2 != Vector3.zero)
			{
				vector3 = vector;
				vector10 = vector2;
				vector13 = -vector13;
			}
			if (vector2 == Vector3.zero)
			{
				vector2 = vector13;
				vector = vector4;
			}
			list.Add(vector3 + vector10 / 2f);
			list.Add(vector4 + vector13 / 2f);
			list.Add(vector4 - vector10 / 2f);
			list.Add(vector3 - vector13 / 2f);
			vector2 = vector13;
			vector = vector4;
			float num4 = m_PathLength + (vector4 - vector3).magnitude;
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
			num += 4;
		}
		Object.Destroy(gameObject);
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
	}
}
