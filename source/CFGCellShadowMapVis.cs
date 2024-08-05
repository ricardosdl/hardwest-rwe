using System;
using UnityEngine;

public class CFGCellShadowMapVis : CFGWindow
{
	private bool m_bVisible;

	private static Mesh m_Mesh;

	private Material m_Material;

	private Texture2D m_VisTex_L0;

	private Texture2D m_VisTex_L1;

	private Texture2D m_VisTex_L2;

	private Texture2D m_VisTexC_L0;

	private Texture2D m_VisTexC_L1;

	private Texture2D m_VisTexC_L2;

	private int m_VisTexSize;

	private float m_VisEdgeSize;

	private int m_VisTexSizeC;

	private float m_VisEdgeSizeC;

	public Shader m_Shader;

	private string[] m_MapTypes;

	private Rect m_MainWindowRect = new Rect((float)Screen.width * 0.1f, (float)Screen.height * 0.1f, 300f, (float)Screen.height - (float)Screen.height * 0.2f);

	private int m_NewType;

	private Vector3 m_NewSunDir = Vector3.zero;

	private string m_NSD_X = "0";

	private string m_NSD_Y = "0";

	private string m_NSD_Z = "0";

	private bool m_bDarkness;

	private bool m_bDisplayedShadowError;

	public override EWindowID GetWindowID()
	{
		return EWindowID.Unknown;
	}

	protected override void OnActivate()
	{
		CFGCellShadowMapLevel.RegisterOnRecalc(UploadMap);
		m_MapTypes = Enum.GetNames(typeof(eCellShadowMapType));
		if (m_NewSunDir.magnitude < 0.1f)
		{
			m_NewSunDir = new Vector3(1f, -1f, 0f);
			m_NewSunDir.Normalize();
		}
		m_NSD_X = m_NewSunDir.x.ToString();
		m_NSD_Y = m_NewSunDir.y.ToString();
		m_NSD_Z = m_NewSunDir.z.ToString();
	}

	protected override void OnDeactivate()
	{
		CFGCellShadowMapLevel.UnRegisterOnRecalc(UploadMap);
	}

	protected override void OnUpdate()
	{
		if (m_bVisible)
		{
			Draw();
		}
	}

	protected override void DrawWindow()
	{
		m_MainWindowRect = GUILayout.Window(1000, m_MainWindowRect, HandleWnd, "CELL Shadow Map Viewer");
	}

	private void HandleWnd(int id)
	{
		CFGCellShadowMap mainMap = CFGCellShadowMapLevel.MainMap;
		if (mainMap == null)
		{
			GUILayout.Label("No map created!");
		}
		else
		{
			GUILayout.Label("Type: " + mainMap.MapType);
			GUILayout.Label("Width: " + mainMap.Width + " " + mainMap.WidthInTexels);
			GUILayout.Label("Height: " + mainMap.Height + " " + mainMap.HeightInTexels);
			GUILayout.Label("Floors: " + mainMap.Floors);
			GUILayout.Label("Sun Dir: " + mainMap.SunDirection);
			GUILayout.Space(10f);
			m_bVisible = GUILayout.Toggle(m_bVisible, "Visible");
			if (m_bVisible && m_VisTex_L0 == null)
			{
				UploadMap(mainMap);
				UploadCharMap(CFGCellShadowMapLevel.m_CharacterMap);
			}
		}
		GUILayout.BeginVertical(GUILayout.Width(200f));
		m_NewType = GUILayout.SelectionGrid(m_NewType, m_MapTypes, 1);
		GUILayout.Label("New Sun Dir - (y must be < 0.0f)");
		m_NSD_X = GUILayout.TextField(m_NSD_X);
		m_NSD_Y = GUILayout.TextField(m_NSD_Y);
		m_NSD_Z = GUILayout.TextField(m_NSD_Z);
		float.TryParse(m_NSD_X, out m_NewSunDir.x);
		float.TryParse(m_NSD_Y, out m_NewSunDir.y);
		float.TryParse(m_NSD_Z, out m_NewSunDir.z);
		if (GUILayout.Button("Reset to current"))
		{
			m_NewSunDir = mainMap.SunDirection;
			m_NSD_X = m_NewSunDir.x.ToString();
			m_NSD_Y = m_NewSunDir.y.ToString();
			m_NSD_Z = m_NewSunDir.z.ToString();
		}
		if (GUILayout.Button("Get from Level Settings"))
		{
			CFGLevelSettings[] array = UnityEngine.Object.FindObjectsOfType<CFGLevelSettings>();
			if (array != null && array.Length > 0 && array[0] != null)
			{
				m_NewSunDir = array[0].GetSunDirection();
				m_NSD_X = m_NewSunDir.x.ToString();
				m_NSD_Y = m_NewSunDir.y.ToString();
				m_NSD_Z = m_NewSunDir.z.ToString();
				m_bDarkness = array[0].m_Darkness;
			}
		}
		GUILayout.EndVertical();
		if (m_NewSunDir.magnitude > 0.1f && m_NewSunDir.y < 0f)
		{
			Vector3 normalized = m_NewSunDir.normalized;
			GUILayout.Label("Normalized: " + normalized);
			if (GUILayout.Button("Recalculate"))
			{
				eCellShadowMapType newType = (eCellShadowMapType)m_NewType;
				CFGCellShadowMapLevel.CreateMainMap(newType, normalized, m_bDarkness);
				CFGCellShadowMapLevel.m_CharacterMap.Recalculate(normalized, newType.GetMultiplier());
				UploadMap(mainMap);
				UploadCharMap(CFGCellShadowMapLevel.m_CharacterMap);
			}
		}
		if (GUILayout.Button("Close Window"))
		{
			Deactivate();
		}
		GUI.DragWindow(new Rect(0f, 0f, m_MainWindowRect.width, 20f));
	}

	public void UploadCharMap(CFGCellShadowMapChar _Map)
	{
		if (_Map != null && _Map.m_Cells != null)
		{
			m_VisTexSizeC = 64 * _Map.m_Resolution;
			m_VisEdgeSizeC = (float)m_VisTexSizeC / (float)_Map.m_Resolution;
			Debug.Log("Creating char shadow vis textures: Size " + m_VisTexSize + ", Mesh edge set to: " + m_VisEdgeSize);
			if (m_VisTexC_L0 == null)
			{
				m_VisTexC_L0 = new Texture2D(m_VisTexSizeC, m_VisTexSizeC, TextureFormat.RGBA32, mipmap: false, linear: false);
			}
			else
			{
				m_VisTexC_L0.Resize(m_VisTexSizeC, m_VisTexSizeC);
			}
			UploadCharTexture(ref m_VisTexC_L0, -2, _Map, m_VisTexSizeC);
			if (m_VisTexC_L1 == null)
			{
				m_VisTexC_L1 = new Texture2D(m_VisTexSizeC, m_VisTexSizeC, TextureFormat.RGBA32, mipmap: false, linear: false);
			}
			else
			{
				m_VisTexC_L1.Resize(m_VisTexSizeC, m_VisTexSizeC);
			}
			UploadCharTexture(ref m_VisTexC_L1, -1, _Map, m_VisTexSizeC);
			if (m_VisTexC_L2 == null)
			{
				m_VisTexC_L2 = new Texture2D(m_VisTexSizeC, m_VisTexSizeC, TextureFormat.RGBA32, mipmap: false, linear: false);
			}
			else
			{
				m_VisTexC_L2.Resize(m_VisTexSizeC, m_VisTexSizeC);
			}
			UploadCharTexture(ref m_VisTexC_L2, 0, _Map, m_VisTexSizeC);
		}
	}

	public void UploadMap(CFGCellShadowMap _Map)
	{
		if (_Map == null || !_Map.CheckIfValid())
		{
			return;
		}
		m_VisTexSize = 16;
		int num = Mathf.Max(_Map.WidthInTexels, _Map.HeightInTexels);
		while (num > m_VisTexSize)
		{
			m_VisTexSize *= 2;
		}
		m_VisEdgeSize = (float)m_VisTexSize / (float)_Map.MapType.GetMultiplier();
		Debug.Log("Creating shadow vis textures: Size " + m_VisTexSize + ", Mesh edge set to: " + m_VisEdgeSize);
		if (m_VisTex_L0 == null)
		{
			m_VisTex_L0 = new Texture2D(m_VisTexSize, m_VisTexSize, TextureFormat.RGBA32, mipmap: false, linear: false);
		}
		else
		{
			m_VisTex_L0.Resize(m_VisTexSize, m_VisTexSize);
		}
		UploadTexture(ref m_VisTex_L0, 0, _Map, num);
		if (_Map.Floors > 0)
		{
			if (m_VisTex_L1 == null)
			{
				m_VisTex_L1 = new Texture2D(m_VisTexSize, m_VisTexSize, TextureFormat.RGBA32, mipmap: false, linear: false);
			}
			else
			{
				m_VisTex_L1.Resize(m_VisTexSize, m_VisTexSize);
			}
			UploadTexture(ref m_VisTex_L1, 1, _Map, num);
		}
		else
		{
			m_VisTex_L1 = null;
		}
		if (_Map.Floors > 1)
		{
			if (m_VisTex_L2 == null)
			{
				m_VisTex_L2 = new Texture2D(m_VisTexSize, m_VisTexSize, TextureFormat.RGBA32, mipmap: false, linear: false);
			}
			else
			{
				m_VisTex_L2.Resize(m_VisTexSize, m_VisTexSize);
			}
			UploadTexture(ref m_VisTex_L2, 2, _Map, num);
		}
		else
		{
			m_VisTex_L2 = null;
		}
	}

	private void UploadCharTexture(ref Texture2D tex, int floor, CFGCellShadowMapChar _Map, int wip)
	{
		if (tex == null)
		{
			return;
		}
		Color color = new Color(1f, 1f, 1f, 0.6f);
		Color color2 = new Color(0f, 0.5f, 0.5f, 0f);
		for (int i = 0; i < wip; i++)
		{
			for (int j = 0; j < wip; j++)
			{
				tex.SetPixel(i, j, color2);
			}
		}
		for (int k = 0; k < _Map.m_Cells.Count; k++)
		{
			CFGCellShadowMapChar.CFGCellShadow cFGCellShadow = _Map.m_Cells[k];
			if (cFGCellShadow == null)
			{
				continue;
			}
			int num = wip / 2 + cFGCellShadow.Resolution * cFGCellShadow.DZ;
			int num2 = wip / 2 + cFGCellShadow.Resolution * cFGCellShadow.DX;
			for (int l = 0; l < cFGCellShadow.Resolution; l++)
			{
				for (int m = 0; m < cFGCellShadow.Resolution; m++)
				{
					if (cFGCellShadow.DF == floor && cFGCellShadow.Array[m * cFGCellShadow.Resolution + l])
					{
						tex.SetPixel(num + l, m_VisTexSizeC - 1 - num2 - m, color);
					}
				}
			}
		}
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.Apply();
	}

	private void UploadTexture(ref Texture2D tex, int floor, CFGCellShadowMap _Map, int wip)
	{
		if (tex == null)
		{
			return;
		}
		Color color = new Color(1f, 0.5f, 0.5f, 0.5f);
		Color color2 = new Color(0f, 0.5f, 0.5f, 0f);
		for (int i = 0; i < wip; i++)
		{
			for (int j = 0; j < wip; j++)
			{
				bool shadowPoint = _Map.GetShadowPoint(floor, i, j);
				tex.SetPixel(i, m_VisTexSize - 1 - j, (!shadowPoint) ? color2 : color);
			}
		}
		tex.filterMode = FilterMode.Point;
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.Apply();
	}

	public static void DrawQuadH(Vector3 pos, float size, Color color, Material material, Texture tex)
	{
		if (m_Mesh == null)
		{
			CreateQuadMesh();
		}
		Quaternion q = Quaternion.Euler(0f, 0f, 90f);
		Vector3 s = new Vector3(size, size, size);
		Matrix4x4 matrix = Matrix4x4.TRS(pos, q, s);
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.AddColor("_Color", color);
		materialPropertyBlock.AddTexture("_MainTex", tex);
		Graphics.DrawMesh(m_Mesh, matrix, material, 11, null, 0, materialPropertyBlock);
	}

	public void Draw()
	{
		if (m_Material == null)
		{
			if (m_Shader == null && !m_bDisplayedShadowError)
			{
				m_bDisplayedShadowError = true;
				Debug.LogError("Failed to create materials - shader is null!");
				return;
			}
			m_Material = new Material(m_Shader);
		}
		int num = 0;
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		if (component != null)
		{
			num = (int)component.CurrentFloorLevel;
		}
		DrawTex(m_VisTex_L0, 0f);
		if (num > 0)
		{
			DrawTex(m_VisTex_L1, 2.5f);
		}
		if (num > 1)
		{
			DrawTex(m_VisTex_L2, 5f);
		}
		int PosX = 0;
		int PosZ = 0;
		int Floor = 2;
		Color color = new Color(10f, 10f, 0f, 1f);
		foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
		{
			if (!(character == null) && character.CurrentCell != null)
			{
				character.CurrentCell.DecodePosition(out PosX, out PosZ, out Floor);
				if (character.Owner != null && character.Owner.IsPlayer)
				{
					color.r = 0f;
					color.g = 10f;
				}
				else
				{
					color.r = 10f;
					color.g = 0f;
				}
				Vector3 zero = Vector3.zero;
				if (Floor > -1)
				{
					zero = new Vector3(PosX, (float)Floor * 2.5f + 0.2f, PosZ);
					DrawQuadH(zero, m_VisEdgeSizeC, color, m_Material, m_VisTexC_L2);
				}
				Floor--;
				if (Floor > -1)
				{
					zero = new Vector3(PosX, (float)Floor * 2.5f + 0.2f, PosZ);
					DrawQuadH(zero, m_VisEdgeSizeC, color, m_Material, m_VisTexC_L1);
				}
				Floor--;
				if (Floor > -1)
				{
					zero = new Vector3(PosX, (float)Floor * 2.5f + 0.2f, PosZ);
					DrawQuadH(zero, m_VisEdgeSizeC, color, m_Material, m_VisTexC_L0);
				}
			}
		}
	}

	private void DrawTex(Texture2D tex, float fFloor)
	{
		if (!(m_Material == null) && !(tex == null))
		{
			float y = 0.2f + fFloor;
			Vector3 pos = new Vector3(m_VisEdgeSize * 0.5f, y, m_VisEdgeSize * 0.5f);
			DrawQuadH(pos, m_VisEdgeSize, new Color(10f, 10f, 10f, 1f), m_Material, tex);
		}
	}

	private static void CreateQuadMesh()
	{
		m_Mesh = new Mesh();
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(0f, -0.5f, -0.5f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, -0.5f, 0.5f)
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		m_Mesh.vertices = vertices;
		m_Mesh.uv = uv;
		m_Mesh.triangles = triangles;
	}
}
