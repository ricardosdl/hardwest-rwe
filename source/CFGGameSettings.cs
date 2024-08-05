using System;
using UnityEngine;

public class CFGGameSettings : CFGSingletonResourcePrefab<CFGGameSettings>
{
	[Serializable]
	public class CursorTypes
	{
		public string m_Normal = string.Empty;

		public string m_Inactive = string.Empty;
	}

	public string[] m_StrategicMaps;

	public string[] m_TacticalMaps;

	public AudioClip m_MenuMusic;

	public string m_CursorsDir = "Resources/UI/Cursors/";

	public CursorTypes m_Cursors;

	private int m_LastLevelPrefix;

	public void SetLastLevelPrefix(int prefix)
	{
		m_LastLevelPrefix = prefix;
	}

	public int GetLastLevelPrefix()
	{
		return m_LastLevelPrefix;
	}
}
