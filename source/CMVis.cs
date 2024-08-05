using System.Collections.Generic;
using UnityEngine;

public class CMVis : CMVisData
{
	public class RenderObject
	{
		public Mesh mesh;

		public Matrix4x4 matrix;

		public Material material;

		public RenderObjectType flagType;

		public Texture2D texture;

		public byte flag;

		public Color color = Color.white;

		public RenderObject()
		{
		}

		public RenderObject(Mesh mesh, Matrix4x4 matrix, Material material, MaterialPropertyBlock props)
		{
			this.mesh = mesh;
			this.matrix = matrix;
			this.material = material;
		}
	}

	private static CMVis instance;

	public bool enabled;

	public byte[] borderFlagTypes = new byte[4] { 2, 4, 3, 5 };

	private int z;

	private int x;

	private int floor;

	private CFGCell cell;

	private Vector3 position;

	private int colorTagID;

	private int textureTagID;

	private bool initialized;

	private List<RenderObject> renderList = new List<RenderObject>();

	private Dictionary<CFGCell.ETileFlags, Mesh> tileMeshes = new Dictionary<CFGCell.ETileFlags, Mesh>();

	private Dictionary<CFGCell.ETileFlags, Matrix4x4> tileMatrices = new Dictionary<CFGCell.ETileFlags, Matrix4x4>();

	private Dictionary<CFGCell.ECMPFlags, Mesh> centerMeshes = new Dictionary<CFGCell.ECMPFlags, Mesh>();

	private Dictionary<CFGCell.ECMPFlags, Matrix4x4> centerMatrices = new Dictionary<CFGCell.ECMPFlags, Matrix4x4>();

	private Dictionary<CFGCell.ECMPFlags, Mesh> borderMeshes = new Dictionary<CFGCell.ECMPFlags, Mesh>();

	public static CMVis Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CMVis();
			}
			return instance;
		}
	}

	private Color GetColor(byte flagType, byte flag)
	{
		if (flagType == 0 && flag == 2)
		{
			return floorColor;
		}
		return Color.white;
	}

	public override void Init()
	{
		base.Init();
		if (!initialized)
		{
			colorTagID = Shader.PropertyToID("_Color");
			textureTagID = Shader.PropertyToID("_MainTex");
			Reset();
			initialized = true;
			if (!Application.isPlaying)
			{
				Recalculate();
				ShowVisualization();
			}
		}
	}

	public void Reset()
	{
		LoadDefaults();
		FillTile();
		FillCenter();
		FillBorders();
	}

	public void Recalculate()
	{
		DecodeCellMap();
	}

	public void Draw()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		foreach (RenderObject render in renderList)
		{
			materialPropertyBlock.Clear();
			materialPropertyBlock.AddColor(colorTagID, render.color);
			Material material = render.material ?? transparent;
			if (render.texture != null)
			{
				materialPropertyBlock.Clear();
				materialPropertyBlock.AddTexture(textureTagID, render.texture);
			}
			Graphics.DrawMesh(render.mesh, render.matrix, material, 0, null, 0, materialPropertyBlock);
		}
	}

	public void DecodeCellMap()
	{
		FillTile();
		FillCenter();
		FillBorders();
		renderList.Clear();
		if (!CFGCellMap.IsValid)
		{
			CFGCellMap.CreateMap(CFGCellMap.FindSceneMainObject());
		}
		for (floor = startFloor; floor < endFloor; floor++)
		{
			for (z = 0; z < CFGCellMap.ZAxisSize; z++)
			{
				for (x = 0; x < CFGCellMap.XAxisSize; x++)
				{
					cell = CFGCellMap.GetCell(z, x, floor);
					if (cell != null && !cell.IsEmpty)
					{
						position = cell.WorldPosition;
						if (showFloors)
						{
							DecodeFloors();
						}
						if (showInteriors)
						{
							DecodeTileFlags(CFGCell.ETileFlags.IsInterior, interiorColor);
						}
						if (showTriggers)
						{
							DecodeTileFlags(CFGCell.ETileFlags.Trigger, triggerColor);
						}
						if (showCovers)
						{
							DecodeCenterCover();
							DecodeBorders(CFGCell.ECMPFlags.IsCover);
						}
						if (showImpassables)
						{
							DecodeCenters(CFGCell.ECMPFlags.Impassable, Color.blue, solid, null);
							DecodeBorders(CFGCell.ECMPFlags.Impassable, Color.blue, solid, 2.49f, 0.19f);
						}
						if (showBlockSights)
						{
							DecodeCenters(CFGCell.ECMPFlags.BlockSight, Color.white, textured, blockSightTex);
							DecodeTexturedBorders(CFGCell.ECMPFlags.BlockSight, blockSightTex);
						}
						if (showBlockBullets)
						{
							DecodeCenters(CFGCell.ECMPFlags.StopBullet, Color.white, textured, blockBulletsTex);
							DecodeTexturedBorders(CFGCell.ECMPFlags.StopBullet, blockBulletsTex, 1.68f);
						}
						if (showSwitches)
						{
							DecodeBorders(CFGCell.ECMPFlags.Activator, Color.magenta, transparent);
						}
					}
				}
			}
		}
	}

	private void DecodeFloors()
	{
		CFGCell.EStairsType eStairsType = cell.Editor_GetStairsType(nightmare);
		RenderObject renderObject = new RenderObject();
		if (eStairsType == CFGCell.EStairsType.None)
		{
			byte flagType = GetFlagType(0);
			CFGCell.ETileFlags eTileFlags = CFGCell.ETileFlags.HasFloor;
			if (cell.Editor_IsFlagSet(flagType, (byte)eTileFlags))
			{
				renderObject.mesh = plane;
				renderObject.matrix = GetOffsettedMatrix(tileMatrices[eTileFlags]);
				renderObject.color = GetTileColor(eTileFlags);
				renderObject.flagType = RenderObjectType.FLOORS;
				renderList.Add(renderObject);
			}
		}
		else
		{
			CFGCell.ETileFlags eTileFlags2 = CFGCell.StairsToFlag(eStairsType);
			renderObject.mesh = tileMeshes[eTileFlags2];
			renderObject.matrix = GetOffsettedMatrix(tileMatrices[eTileFlags2]);
			renderObject.color = GetTileColor(eTileFlags2);
			renderList.Add(renderObject);
		}
	}

	private void DecodeTileFlags(CFGCell.ETileFlags flag, Color color)
	{
		byte flagType = GetFlagType(0);
		if (cell.Editor_IsFlagSet(flagType, (byte)flag))
		{
			RenderObject renderObject = new RenderObject();
			renderObject.mesh = tileMeshes[flag];
			renderObject.matrix = GetOffsettedMatrix(tileMatrices[flag], position);
			renderObject.color = color;
			renderObject.flagType = GetTypeFromTileFlag(flag);
			if (flag == CFGCell.ETileFlags.IsInterior)
			{
				renderObject.material = textured;
				renderObject.texture = interiorTex;
			}
			renderList.Add(renderObject);
		}
	}

	private void DecodeCenterCover()
	{
		byte flagType = GetFlagType(1);
		RenderObject renderObject = new RenderObject();
		if (cell.Editor_IsFlagSet(flagType, 4))
		{
			renderObject.mesh = centerMeshes[CFGCell.ECMPFlags.IsFullCover];
			renderObject.matrix = GetOffsettedMatrix(centerMatrices[CFGCell.ECMPFlags.IsFullCover], position);
			renderObject.color = fullCoverColor;
			renderList.Add(renderObject);
		}
		else if (cell.Editor_IsFlagSet(flagType, 2))
		{
			renderObject.mesh = centerMeshes[CFGCell.ECMPFlags.IsCover];
			renderObject.matrix = GetOffsettedMatrix(centerMatrices[CFGCell.ECMPFlags.IsCover], position);
			renderObject.color = halfCoverColor;
			renderList.Add(renderObject);
		}
	}

	private void DecodeCenters(CFGCell.ECMPFlags flag, Color color, Material mat, Texture2D tex)
	{
		byte flagType = GetFlagType(1);
		if (cell.Editor_IsFlagSet(flagType, (byte)flag))
		{
			RenderObject renderObject = new RenderObject();
			renderObject.mesh = centerMeshes[flag];
			renderObject.color = color;
			renderObject.matrix = GetOffsettedMatrix(centerMatrices[flag], position);
			renderObject.material = mat;
			renderObject.texture = tex;
			renderList.Add(renderObject);
		}
	}

	private void DecodeBorders(CFGCell.ECMPFlags flag, Color color, Material material = null, float height = 2.5f, float thick = 0.2f, Texture2D tex = null)
	{
		byte[] array = borderFlagTypes;
		foreach (byte flagType in array)
		{
			byte flagType2 = GetFlagType(flagType);
			if (cell.Editor_IsFlagSet(flagType2, (byte)flag))
			{
				RenderObject renderObject = new RenderObject();
				renderObject.mesh = borderMeshes[flag];
				renderObject.color = color;
				renderObject.material = material;
				renderObject.texture = tex;
				renderObject.matrix = Matrix4x4.TRS(position + WallPos(flagType2, height / 2f), Quaternion.identity, WallScale(flagType2, height, thick));
				renderList.Add(renderObject);
			}
		}
	}

	private void DecodeTexturedBorders(CFGCell.ECMPFlags flag, Texture2D tex, float hOffset = 0.84f)
	{
		byte[] array = borderFlagTypes;
		foreach (byte flagType in array)
		{
			byte flagType2 = GetFlagType(flagType);
			if (cell.Editor_IsFlagSet(flagType2, (byte)flag))
			{
				RenderObject renderObject = new RenderObject();
				renderObject.mesh = plane;
				renderObject.material = textured;
				renderObject.texture = tex;
				Vector3 pos = cell.WorldPosition + WallPos(flagType2, hOffset, 0.12f);
				Quaternion q = Rotate(flagType2);
				renderObject.matrix = Matrix4x4.TRS(pos, q, texScale);
				renderList.Add(renderObject);
			}
		}
	}

	private void DecodeBorders(CFGCell.ECMPFlags flag)
	{
		byte[] array = borderFlagTypes;
		foreach (byte flagType in array)
		{
			byte flagType2 = GetFlagType(flagType);
			RenderObject renderObject = new RenderObject();
			renderObject.mesh = cube;
			float num = 2.5f;
			if (cell.Editor_IsFlagSet(flagType2, 4))
			{
				renderObject.color = fullCoverColor;
				renderList.Add(renderObject);
			}
			else
			{
				if (!cell.Editor_IsFlagSet(flagType2, 2))
				{
					continue;
				}
				num = 1.25f;
				renderObject.color = halfCoverColor;
				renderList.Add(renderObject);
			}
			renderObject.matrix = Matrix4x4.TRS(position + WallPos(flagType2, num / 2f), Quaternion.identity, WallScale(flagType2, num, 0.2f));
			renderList.Add(renderObject);
		}
	}

	private Matrix4x4 GetOffsettedMatrix(Matrix4x4 mat)
	{
		Matrix4x4 result = mat;
		result.m03 += position.x;
		result.m13 += position.y;
		result.m23 += position.z;
		return result;
	}

	private Matrix4x4 GetOffsettedMatrix(Matrix4x4 mat, Vector3 offset)
	{
		Matrix4x4 result = mat;
		result.m03 += offset.x;
		result.m13 += offset.y;
		result.m23 += offset.z;
		return result;
	}

	private byte GetFlagType(byte flagType)
	{
		return (!nightmare) ? flagType : ((byte)(flagType + 6));
	}

	public bool CheckFlag(byte FlagType, byte Flag)
	{
		if ((cell.Flags[FlagType] & 1) == 1 && nightmare)
		{
			FlagType += 6;
		}
		return (cell.Flags[FlagType] & Flag) == Flag;
	}

	private Color GetColorForFlag(byte flag)
	{
		return Color.white;
	}

	public void ShowVisualization()
	{
		if (!(CallbacksProvider.Instance == null) && !CallbacksProvider.Instance.IsOnUpdateRegistered(Draw))
		{
			CallbacksProvider.Instance.OnUpdate += Draw;
			enabled = CallbacksProvider.Instance.IsOnUpdateRegistered(Draw);
		}
	}

	public void HideVisualization()
	{
		if (CallbacksProvider.Instance != null && CallbacksProvider.Instance.IsOnUpdateRegistered(Draw))
		{
			enabled = false;
			CallbacksProvider.Instance.OnUpdate -= Draw;
		}
	}

	private void FillTile()
	{
		tileMeshes = new Dictionary<CFGCell.ETileFlags, Mesh>();
		tileMatrices = new Dictionary<CFGCell.ETileFlags, Matrix4x4>();
		tileMeshes.Add(CFGCell.ETileFlags.HasFloor, plane);
		tileMatrices.Add(CFGCell.ETileFlags.HasFloor, Matrix4x4.TRS(new Vector3(0f, 0.11f, 0f), Quaternion.identity, Vector3.one * 0.9f));
		tileMeshes.Add(CFGCell.ETileFlags.Stairs_Low, cube);
		tileMatrices.Add(CFGCell.ETileFlags.Stairs_Low, Matrix4x4.TRS(new Vector3(0f, 5f / 12f, 0f), Quaternion.identity, new Vector3(1f, 5f / 6f, 1f)));
		tileMeshes.Add(CFGCell.ETileFlags.STAIRS, cube);
		tileMatrices.Add(CFGCell.ETileFlags.STAIRS, Matrix4x4.TRS(new Vector3(0f, 5f / 6f, 0f), Quaternion.identity, new Vector3(1f, 1.6666666f, 1f)));
		tileMeshes.Add(CFGCell.ETileFlags.Stairs_High, plane);
		tileMatrices.Add(CFGCell.ETileFlags.Stairs_High, Matrix4x4.TRS(new Vector3(0f, 0.11f, 0f), Quaternion.identity, Vector3.one * 0.85f));
		tileMeshes.Add(CFGCell.ETileFlags.IsInterior, plane);
		tileMatrices.Add(CFGCell.ETileFlags.IsInterior, Matrix4x4.TRS(new Vector3(0f, 0.13f, 0f), Quaternion.identity, Vector3.one * 0.8f));
		tileMeshes.Add(CFGCell.ETileFlags.Trigger, cube);
		tileMatrices.Add(CFGCell.ETileFlags.Trigger, Matrix4x4.TRS(new Vector3(0f, 1.25f, 0f), Quaternion.identity, new Vector3(1f, 2.5f, 1f)));
	}

	private void FillCenter()
	{
		centerMeshes = new Dictionary<CFGCell.ECMPFlags, Mesh>();
		centerMatrices = new Dictionary<CFGCell.ECMPFlags, Matrix4x4>();
		centerMeshes.Add(CFGCell.ECMPFlags.IsCover, cube);
		centerMatrices.Add(CFGCell.ECMPFlags.IsCover, Matrix4x4.TRS(new Vector3(0f, 0.625f, 0f), Quaternion.identity, new Vector3(1f, 1.25f, 1f)));
		centerMeshes.Add(CFGCell.ECMPFlags.IsFullCover, cube);
		centerMatrices.Add(CFGCell.ECMPFlags.IsFullCover, Matrix4x4.TRS(new Vector3(0f, 1.25f, 0f), Quaternion.identity, new Vector3(1f, 2.5f, 1f)));
		centerMeshes.Add(CFGCell.ECMPFlags.Impassable, ex);
		centerMatrices.Add(CFGCell.ECMPFlags.Impassable, Matrix4x4.TRS(new Vector3(0.5f, 0f, -0.5f), Quaternion.identity, Vector3.one));
		centerMeshes.Add(CFGCell.ECMPFlags.BlockSight, cube);
		centerMatrices.Add(CFGCell.ECMPFlags.BlockSight, Matrix4x4.TRS(new Vector3(0f, texScale.y, 0f), Quaternion.identity, texScale));
		centerMeshes.Add(CFGCell.ECMPFlags.StopBullet, cube);
		centerMatrices.Add(CFGCell.ECMPFlags.StopBullet, Matrix4x4.TRS(new Vector3(0f, texScale.y * 2f, 0f), Quaternion.identity, texScale));
		centerMeshes.Add(CFGCell.ECMPFlags.Activator, lever);
		centerMatrices.Add(CFGCell.ECMPFlags.Activator, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one));
	}

	private void FillBorders()
	{
		borderMeshes = new Dictionary<CFGCell.ECMPFlags, Mesh>();
		borderMeshes.Add(CFGCell.ECMPFlags.IsCover, cube);
		borderMeshes.Add(CFGCell.ECMPFlags.IsFullCover, cube);
		borderMeshes.Add(CFGCell.ECMPFlags.Impassable, cube);
		borderMeshes.Add(CFGCell.ECMPFlags.BlockSight, plane);
		borderMeshes.Add(CFGCell.ECMPFlags.StopBullet, plane);
		borderMeshes.Add(CFGCell.ECMPFlags.Activator, lever);
	}

	private Vector3 WallPos(byte cellBorderFlag, float hOffset = 0f, float iOffset = 0f)
	{
		switch (cellBorderFlag)
		{
		case 1:
		case 7:
			return new Vector3(0f, hOffset, 0f);
		case 2:
		case 8:
			return new Vector3(0f - (0.5f - iOffset), hOffset, 0f);
		case 4:
		case 10:
			return new Vector3(0f, hOffset, 0.5f - iOffset);
		case 3:
		case 9:
			return new Vector3(0.5f - iOffset, hOffset, 0f);
		case 5:
		case 11:
			return new Vector3(0f, hOffset, 0f - (0.5f - iOffset));
		default:
			return new Vector3(0f, hOffset, 0f);
		}
	}

	private Vector3 WallScale(byte cellBorderFlag, float heightScale = 1f, float thicknessScale = 0.1f)
	{
		switch (cellBorderFlag)
		{
		case 1:
		case 7:
			return Vector3.one * 0.3f;
		case 2:
		case 8:
			return new Vector3(thicknessScale, heightScale, 1f);
		case 4:
		case 10:
			return new Vector3(1f, heightScale, thicknessScale);
		case 3:
		case 9:
			return new Vector3(thicknessScale, heightScale, 1f);
		case 5:
		case 11:
			return new Vector3(1f, heightScale, thicknessScale);
		default:
			return Vector3.one;
		}
	}

	private Quaternion Rotate(byte cellBorderFlag)
	{
		switch (cellBorderFlag)
		{
		case 1:
		case 7:
			return Quaternion.Euler(0f, 90f, 0f);
		case 2:
		case 8:
			return Quaternion.Euler(90f, 90f, 0f);
		case 4:
		case 10:
			return Quaternion.Euler(90f, 180f, 0f);
		case 3:
		case 9:
			return Quaternion.Euler(90f, 270f, 0f);
		case 5:
		case 11:
			return Quaternion.Euler(90f, 0f, 0f);
		default:
			return Quaternion.Euler(0f, 0f, 0f);
		}
	}
}
