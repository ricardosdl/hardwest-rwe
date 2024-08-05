using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CFGConeOfFire : MonoBehaviour
{
	[Range(1f, 179f)]
	public float m_ConeAngle = 30f;

	public float m_StartLength = 2f;

	public float m_EndLength = 12f;

	public float m_BorderWidth = 0.025f;

	public float m_HeightOffset = 0.01f;

	public EConeVisType m_VisType = EConeVisType.NORMAL;

	public EConeVisType2 m_VisType2;

	[SerializeField]
	private Material m_Mat_30;

	[SerializeField]
	private Material m_Mat_45;

	[SerializeField]
	private Material m_Mat_90;

	[SerializeField]
	private Material m_ConeShotMat_30;

	[SerializeField]
	private Material m_ConeShotMat_45;

	[SerializeField]
	private Material m_ConeShotMat_90;

	private Mesh m_Mesh;

	private Material m_Mat;

	private Vector3[] m_Vertices;

	private Vector2[] m_Uv;

	private CFGCell m_CellUnder;

	private float m_LastShowNormal = -1f;

	private void Start()
	{
		RegenerateMesh();
		m_CellUnder = CFGCellMap.GetCell(base.transform.position);
	}

	private void Rebuild()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		Material material = (int)m_ConeAngle switch
		{
			30 => (m_VisType2 != 0) ? m_ConeShotMat_30 : m_Mat_30, 
			45 => (m_VisType2 != 0) ? m_ConeShotMat_45 : m_Mat_45, 
			90 => (m_VisType2 != 0) ? m_ConeShotMat_90 : m_Mat_90, 
			_ => null, 
		};
		if (material != null)
		{
			m_Mat = new Material(material);
			m_Mat.SetFloat("_ShowNormal", (float)m_VisType);
		}
		else
		{
			m_Mat = new Material(Shader.Find("Diffuse"));
		}
		component.sharedMaterial = m_Mat;
		UpdateMaterial();
	}

	private void Update()
	{
		UpdateMaterial();
	}

	private void UpdateMaterial()
	{
		if (!(m_Mat == null))
		{
			float num = 0f;
			num = ((m_VisType != 0 && !CheckIfCursorIn()) ? 1f : 0f);
			if (m_LastShowNormal != num)
			{
				m_LastShowNormal = num;
				m_Mat.SetFloat("_ShowNormal", m_LastShowNormal);
			}
		}
	}

	public void RegenerateMesh()
	{
		Rebuild();
		GetComponent<MeshFilter>().mesh = (m_Mesh = new Mesh());
		m_Mesh.Clear();
		m_Mesh.name = "Cone Mesh";
		m_Mesh.subMeshCount = 1;
		m_Vertices = new Vector3[4]
		{
			new Vector3(0f - m_EndLength, m_HeightOffset, 0f - m_EndLength),
			new Vector3(m_EndLength, m_HeightOffset, 0f - m_EndLength),
			new Vector3(m_EndLength, m_HeightOffset, m_EndLength),
			new Vector3(0f - m_EndLength, m_HeightOffset, m_EndLength)
		};
		int[] triangles = new int[6] { 0, 2, 1, 0, 3, 2 };
		m_Uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
		m_Mesh.vertices = m_Vertices;
		m_Mesh.triangles = triangles;
		m_Mesh.uv = m_Uv;
	}

	private bool CheckIfCursorIn()
	{
		if (CFGSelectionManager.Instance.CellUnderCursor != null && m_CellUnder != null && CFGSelectionManager.Instance.CellUnderCursor.Floor != m_CellUnder.Floor)
		{
			return false;
		}
		float num = Vector3.Distance(base.transform.position, CFGSelectionManager.Instance.MouseWorldPos);
		if (num > m_EndLength)
		{
			return false;
		}
		if (CFGSelectionManager.Instance.CellUnderCursor == m_CellUnder)
		{
			return true;
		}
		Vector3 lhs = Vector3.Normalize(CFGSelectionManager.Instance.MouseWorldPos - base.transform.position);
		float num2 = 57.29578f * Mathf.Acos(Mathf.Clamp(Vector3.Dot(lhs, base.transform.forward.normalized), -1f, 1f));
		if (num2 <= m_ConeAngle / 2f)
		{
			return true;
		}
		return false;
	}

	public void RegenerateMeshMy()
	{
		GetComponent<MeshFilter>().mesh = (m_Mesh = new Mesh());
		m_Mat.SetFloat("_StartLength", m_StartLength);
		m_Mat.SetFloat("_EndLength", m_EndLength);
		m_Mesh.Clear();
		m_Mesh.name = "Cone Mesh";
		m_Mesh.subMeshCount = 2;
		m_Vertices = new Vector3[11];
		float num = Mathf.Tan((float)Math.PI / 180f * (m_ConeAngle / 2f)) * m_EndLength;
		ref Vector3 reference = ref m_Vertices[0];
		reference = new Vector3(0f, m_HeightOffset, 0f);
		ref Vector3 reference2 = ref m_Vertices[1];
		reference2 = new Vector3(0f - num, m_HeightOffset, m_EndLength);
		ref Vector3 reference3 = ref m_Vertices[2];
		reference3 = new Vector3(num, m_HeightOffset, m_EndLength);
		float num2 = Mathf.Sin((float)Math.PI / 180f * (90f - m_ConeAngle / 2f));
		float num3 = Mathf.Cos((float)Math.PI / 180f * (90f - m_ConeAngle / 2f));
		ref Vector3 reference4 = ref m_Vertices[3];
		reference4 = new Vector3(0f - (m_StartLength * num3 - m_BorderWidth), m_HeightOffset + 0.01f, m_StartLength * num2);
		ref Vector3 reference5 = ref m_Vertices[4];
		reference5 = new Vector3(0f - (m_StartLength * num3 + m_BorderWidth), m_HeightOffset + 0.01f, m_StartLength * num2);
		ref Vector3 reference6 = ref m_Vertices[5];
		reference6 = new Vector3(0f - (m_EndLength * num3 + m_BorderWidth), m_HeightOffset + 0.01f, m_EndLength * num2);
		ref Vector3 reference7 = ref m_Vertices[6];
		reference7 = new Vector3(0f - (m_EndLength * num3 - m_BorderWidth), m_HeightOffset + 0.01f, m_EndLength * num2);
		ref Vector3 reference8 = ref m_Vertices[7];
		reference8 = new Vector3(m_StartLength * num3 + m_BorderWidth, m_HeightOffset + 0.01f, m_StartLength * num2);
		ref Vector3 reference9 = ref m_Vertices[8];
		reference9 = new Vector3(m_StartLength * num3 - m_BorderWidth, m_HeightOffset + 0.01f, m_StartLength * num2);
		ref Vector3 reference10 = ref m_Vertices[9];
		reference10 = new Vector3(m_EndLength * num3 - m_BorderWidth, m_HeightOffset + 0.01f, m_EndLength * num2);
		ref Vector3 reference11 = ref m_Vertices[10];
		reference11 = new Vector3(m_EndLength * num3 + m_BorderWidth, m_HeightOffset + 0.01f, m_EndLength * num2);
		m_Mesh.vertices = m_Vertices;
		int[] triangles = new int[3] { 0, 1, 2 };
		int[] triangles2 = new int[12]
		{
			3, 4, 5, 3, 5, 6, 7, 8, 9, 7,
			9, 10
		};
		m_Mesh.SetTriangles(triangles, 0);
		m_Mesh.SetTriangles(triangles2, 1);
		Vector2[] array = new Vector2[m_Vertices.Length];
		ref Vector2 reference12 = ref array[3];
		reference12 = new Vector2(1f, 0f);
		ref Vector2 reference13 = ref array[4];
		reference13 = new Vector2(0f, 0f);
		ref Vector2 reference14 = ref array[5];
		reference14 = new Vector2(0f, 1f);
		ref Vector2 reference15 = ref array[6];
		reference15 = new Vector2(1f, 1f);
		ref Vector2 reference16 = ref array[7];
		reference16 = new Vector2(1f, 0f);
		ref Vector2 reference17 = ref array[8];
		reference17 = new Vector2(0f, 0f);
		ref Vector2 reference18 = ref array[9];
		reference18 = new Vector2(0f, 1f);
		ref Vector2 reference19 = ref array[10];
		reference19 = new Vector2(1f, 1f);
		m_Mesh.uv = array;
	}
}
