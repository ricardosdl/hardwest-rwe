using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGGameplaySettings : CFGSingletonResourcePrefab<CFGGameplaySettings>
{
	[Serializable]
	public class PrefabItem
	{
		public string m_ClassID = string.Empty;

		public Transform m_Object;
	}

	public static int s_CtH_TopTreshold = 80;

	public static int s_CtH_BottomTreshold = 20;

	public static int s_PointBlankDistance = 4;

	public static int s_BlindShotMod = -10;

	public Color m_FadingColor = Color.black;

	public float m_ConeOfFireRange = 10f;

	public LayerMask m_StrategicTerrainLayer = 0;

	public int m_mode_kill_replenish_luck_amount = 10;

	public int m_mode_constant_luck_regen_amount = 10;

	public BrokenWindowElements m_BrokenWindowElements;

	public StrategicLocation m_StrategicLocation;

	public float m_LuckSplashTimeOffset = 1f;

	public RicochetHelper m_RicochetHelpers;

	public CursorHelper m_CursorHelpers;

	public Transform m_CursorHelperPrefab;

	public Transform m_CursorHelperInaccesiblePrefab;

	public Transform m_CursorHelperEntrancePrefab;

	public Material m_CursorHelperNormalMat;

	public Material m_CursorHelperInaccessibleMat;

	public Material m_CursorHelperEntranceMat;

	public CFGGameObject m_GamepadCursor;

	public CFGLocationFog m_LocationFogPrefab;

	public GameObject m_LocationRevealFxPrefab;

	public GameObject m_LocationNewMarker;

	public Transform m_ActivatorVisPrefab;

	public Transform m_BestCellActivatorVisPrefab;

	public Transform m_ActivatorInactiveVisPrefab;

	public Transform m_FullCoverVisPrefab;

	public Transform m_HalfCoverVisPrefab;

	public Material m_Range1Material;

	public Material m_Range2Material;

	public Material m_PathVisMat;

	public Material m_PathVisMat2;

	public Material m_ActiveGrenadePathMat;

	public Material m_InactiveGrenadePathMat;

	public Shader m_InvisibleShader;

	public Transform m_SceneMarkerPrefab;

	public Transform m_RangeHelperPrefab;

	public Transform m_ReactionShotPrefab;

	public float m_WallSize = 0.1f;

	public float m_ShootHeight = 1.7f;

	public Transform m_OrderConfirmationFxPrefab;

	public Transform m_OrderConfirmationFxStrategicPrefab;

	public Material m_RicochetLineMat;

	public TextMesh m_FloatingTextPrefab;

	public CFGShootableObject m_MissTarget;

	public CFGConeOfFire m_ConeOfFireFloorFX;

	public Transform m_GroundBloodFxPrefab;

	public Transform m_EnemySpottedFxPrefab;

	public SoundDefs m_SoundDefs;

	public UIPrefab m_UIPrefab;

	public Camera m_SubtitleCamera;

	public float m_HorsemanFastMaxSpeed = 10f;

	public float m_HorsemanFastAcc = 10f;

	public CFGCameraFollowInfo m_StrategicHorsemanFollowInfo = new CFGCameraFollowInfo();

	public CFGCameraFollowInfo m_StrategicMapFollowInfo = new CFGCameraFollowInfo();

	public CFGCameraFollowInfo m_TacticalMapFollowInfo = new CFGCameraFollowInfo();

	public CFGSetupStageInfo m_SetupStage;

	public List<PrefabItem> m_PrefabMap;

	public CFGReactionShootInfo m_RectionShootInfo = new CFGReactionShootInfo();

	public CFGAbilityData m_AbilityData = new CFGAbilityData();

	public CFGDisintegrationInfo m_DisinegrationInfo = new CFGDisintegrationInfo();

	public List<CFGCharacterIdPrefab> m_CharacterPrefabMap;

	public CFGAchievementData m_AchievementData = new CFGAchievementData();

	public List<CFGCharacterIdPrefab> CharacterIdPrefabMap => m_CharacterPrefabMap;

	public CFGCharacter GetCharacterPrefab(string character_id)
	{
		if (string.IsNullOrEmpty(character_id))
		{
			return null;
		}
		character_id = character_id.ToLower();
		for (int i = 0; i < m_CharacterPrefabMap.Count; i++)
		{
			if (m_CharacterPrefabMap[i] != null && m_CharacterPrefabMap[i].m_Id != null && m_CharacterPrefabMap[i].m_Id.ToLower() == character_id)
			{
				return m_CharacterPrefabMap[i].m_Prefab;
			}
		}
		return null;
	}

	public bool IsCharacterDefined(string character_id)
	{
		character_id = character_id.ToLower();
		foreach (CFGCharacterIdPrefab item in m_CharacterPrefabMap)
		{
			if (item != null && item.m_Id.ToLower() == character_id && item.m_Prefab != null)
			{
				return true;
			}
		}
		return false;
	}

	public Transform GetPrefabObject(string ClassID)
	{
		foreach (PrefabItem item in m_PrefabMap)
		{
			if (string.Compare(ClassID, item.m_ClassID, ignoreCase: true) == 0)
			{
				return item.m_Object;
			}
		}
		return null;
	}

	public T GetPrefabObjectComponent<T>(string ClassID) where T : Component
	{
		foreach (PrefabItem item in m_PrefabMap)
		{
			if (string.Compare(ClassID, item.m_ClassID, ignoreCase: true) != 0 || item.m_Object == null)
			{
				continue;
			}
			return item.m_Object.GetComponent<T>();
		}
		return (T)null;
	}
}
