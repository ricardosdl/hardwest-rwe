using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CFGLevelSettings : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private int m_UID_Version;

	public CFG_SG_SaveGame.eSG_Mode m_LevelType;

	[SerializeField]
	private bool m_FadeInOnStart = true;

	[SerializeField]
	private bool m_StartInNormalMode = true;

	public Color m_NormalAmbientLight = new Color(0.2f, 0.2f, 0.2f, 1f);

	public Color m_NightmareAmbientLight = new Color(0.2f, 0.2f, 0.2f, 1f);

	public GameObject m_NormalEnvSoundsRoot;

	public GameObject m_NightmareEnvSoundsRoot;

	public CFGGameObject m_InitialCameraFocus;

	public Rect m_MapBorders = new Rect(-500f, -500f, 1000f, 1000f);

	public Light m_Sun;

	public eCellShadowMapType m_CellShadowMapType;

	public float m_InitHideInShadowRatio = 0.95f;

	public static float HideInShadowRatio = 0.95f;

	public float m_InitCellInShadowRatio = 0.75f;

	public float m_CharHeightForShadow = 1.8f;

	public bool m_Darkness;

	public static float CellInShadowRatio = 0.75f;

	private static float CharHeightForShadow = 1.8f;

	public float m_InitSunPower = 0.5f;

	public static float SunPower = 0.5f;

	public List<string> m_VariableFileNames = new List<string>();

	public int UID_Version
	{
		get
		{
			return m_UID_Version;
		}
		set
		{
			m_UID_Version = value;
		}
	}

	public bool FadeInOnStart => m_FadeInOnStart;

	public bool StartInNormalMode => m_StartInNormalMode;

	public static float ChararacterHeightForShadow => CharHeightForShadow;

	private void Awake()
	{
		CFGApplication.Init();
		CFGSingleton<CFGGame>.Instance.LevelSettings = this;
		HideInShadowRatio = m_InitHideInShadowRatio;
		CellInShadowRatio = m_InitCellInShadowRatio;
		SunPower = m_InitSunPower;
		CharHeightForShadow = m_CharHeightForShadow;
		SetDllPath();
		if (m_Sun == null || m_Sun.type != LightType.Directional)
		{
			Debug.LogWarning("Sun is not set or is not directional!", this);
		}
	}

	private void Start()
	{
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.SetupCurrentOwner(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner);
		CFGCharacter[] array = UnityEngine.Object.FindObjectsOfType(typeof(CFGCharacter)) as CFGCharacter[];
		foreach (CFGCharacter go in array)
		{
			CFGSingletonResourcePrefab<CFGObjectManager>.Instance.RegisterImmediatly(go);
		}
		CFGSingletonResourcePrefab<CFGTurnManager>.Instance.StartTurn();
	}

	private void OnEnable()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Vector3 center = new Vector3(m_MapBorders.x + m_MapBorders.width * 0.5f, 0f, m_MapBorders.y + m_MapBorders.height * 0.5f);
		Vector3 size = new Vector3(m_MapBorders.width, 1f, m_MapBorders.height);
		Gizmos.DrawWireCube(center, size);
	}

	public Vector3 GetSunDirection()
	{
		if (m_Sun == null || m_Sun.type != LightType.Directional)
		{
			Vector3 result = new Vector3(1f, -1f, 1f);
			result.Normalize();
			return result;
		}
		return m_Sun.transform.forward;
	}

	public static void SetDllPath()
	{
		string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
		string text = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";
		if (!environmentVariable.Contains(text))
		{
			Environment.SetEnvironmentVariable("PATH", environmentVariable + Path.PathSeparator + text, EnvironmentVariableTarget.Process);
		}
	}
}
