using System.Collections.Generic;
using UnityEngine;

public class CMVisData
{
	public enum RenderObjectType
	{
		FLOORS,
		LOW_POINTS,
		SLOPES,
		HIGH_POINTS,
		INTERIORS,
		TRIGGERS,
		COVERS,
		FULL_COVERS,
		IMPASSABLES,
		BLOCK_SIGHTS,
		BLOCK_BULLETS,
		SWITCHES
	}

	protected const float HOVER = 0.11f;

	protected const float LOW = 5f / 6f;

	protected const float MEDIUM = 1.6666666f;

	protected const float HIGH = 2.5f;

	protected const float FULL = 2.5f;

	protected const float HALF = 1.25f;

	protected Vector3 texScale = new Vector3(0.84f, 0.84f, 0.84f);

	public Color floorColor = new Color(0f, 1f, 1f, 0.5f);

	public Color lowPointColor = new Color(1f, 1f, 0f, 0.5f);

	public Color slopeColor = new Color(0.6f, 0.2f, 1f, 0.5f);

	public Color highPointColor = new Color(0f, 0.4f, 0f, 0.5f);

	public Color interiorColor = new Color(0f, 1f, 1f, 0.5f);

	public Color triggerColor = new Color(0f, 1f, 0f, 0.5f);

	public Color halfCoverColor = new Color(1f, 0f, 0f, 0.5f);

	public Color fullCoverColor = new Color(1f, 1f, 0f, 0.5f);

	public readonly Dictionary<RenderObjectType, Color> Colors = new Dictionary<RenderObjectType, Color>
	{
		{
			RenderObjectType.FLOORS,
			new Color(0f, 1f, 1f, 0.5f)
		},
		{
			RenderObjectType.LOW_POINTS,
			new Color(1f, 1f, 0f, 0.5f)
		},
		{
			RenderObjectType.SLOPES,
			new Color(0.6f, 0.2f, 1f, 0.5f)
		},
		{
			RenderObjectType.HIGH_POINTS,
			new Color(0f, 0.4f, 0f, 0.5f)
		},
		{
			RenderObjectType.INTERIORS,
			Color.white
		},
		{
			RenderObjectType.TRIGGERS,
			new Color(0f, 1f, 0f, 0.5f)
		}
	};

	public Mesh cube;

	public Mesh plane;

	public Mesh ex;

	public Mesh lever;

	public Material solid;

	public Material transparent;

	public Material textured;

	public Texture2D blockSightTex;

	public Texture2D blockBulletsTex;

	public Texture2D interiorTex;

	public int startFloor;

	public int endFloor = 3;

	public bool nightmare;

	public bool showCovers = true;

	public bool showFloors = true;

	public bool showInteriors = true;

	public bool showImpassables = true;

	public bool showBlockSights = true;

	public bool showBlockBullets = true;

	public bool showSwitches = true;

	public bool showTriggers = true;

	public CMVisData()
	{
		Init();
	}

	public virtual void Init()
	{
	}

	protected RenderObjectType GetTypeFromTileFlag(CFGCell.ETileFlags tileFlag)
	{
		return tileFlag switch
		{
			CFGCell.ETileFlags.HasFloor => RenderObjectType.FLOORS, 
			CFGCell.ETileFlags.Stairs_Low => RenderObjectType.LOW_POINTS, 
			CFGCell.ETileFlags.STAIRS => RenderObjectType.SLOPES, 
			CFGCell.ETileFlags.IsInterior => RenderObjectType.INTERIORS, 
			CFGCell.ETileFlags.Trigger => RenderObjectType.TRIGGERS, 
			_ => RenderObjectType.FLOORS, 
		};
	}

	protected Color GetTileColor(CFGCell.ETileFlags flag)
	{
		return flag switch
		{
			CFGCell.ETileFlags.Stairs_Low => lowPointColor, 
			CFGCell.ETileFlags.STAIRS => slopeColor, 
			CFGCell.ETileFlags.Stairs_High => highPointColor, 
			CFGCell.ETileFlags.HasFloor => floorColor, 
			_ => Color.white, 
		};
	}

	public void LoadDefaults()
	{
		GameObject gameObject = Resources.Load<GameObject>("Prefabs/CMVisDefaults");
		if (gameObject != null)
		{
			CMVisDefaults component = gameObject.GetComponent<CMVisDefaults>();
			blockSightTex = component.blockSightTex;
			blockBulletsTex = component.blockBulletsTex;
			interiorTex = component.interiorTex;
			cube = component.cube;
			plane = component.plane;
			ex = component.x;
			lever = component.lever;
			solid = component.solid;
			transparent = component.transparent;
			textured = component.textured;
		}
	}
}
