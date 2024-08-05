#define USE_ERROR_REPORTING
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CFG_SG_Manager
{
	public enum eLoadType
	{
		NoLoad,
		ReturnToStrategic,
		ResetTactical,
		LoadTactical,
		LoadStrategic
	}

	public delegate void OnSaveEnd();

	public const string SAVE_EXT = ".hws";

	private const string QUICKSAVE = "QuickSave";

	private const string AUTOSAVE = "AutoSave";

	private const string STRATEGIC_NAME = "Current";

	private const EContainerFormat SAVE_FORMAT = EContainerFormat.Encoded;

	private static List<CFG_SG_SaveGame> m_SaveGames = new List<CFG_SG_SaveGame>();

	private static CFG_SG_SaveGame m_CurrentSG = null;

	private static CFG_SG_SaveGame m_RestorePoint_Strategic = new CFG_SG_SaveGame("Current");

	private static eLoadType m_LoadType = eLoadType.NoLoad;

	public static OnSaveEnd m_SaveEnd = null;

	private static string m_LastQuickSaveName = null;

	public static CFG_SG_SaveGame RestorePoint_Strategic => m_RestorePoint_Strategic;

	public static eLoadType LoadType => m_LoadType;

	public static bool IsLoading => m_CurrentSG != null;

	public static CFG_SG_SaveGame.eSG_Source SGSource
	{
		get
		{
			if (m_CurrentSG == null)
			{
				return CFG_SG_SaveGame.eSG_Source.Unknown;
			}
			return m_CurrentSG.SaveGameSource;
		}
	}

	public static CFG_SG_SaveGame.eSG_Mode SGMode
	{
		get
		{
			if (m_CurrentSG == null)
			{
				return CFG_SG_SaveGame.eSG_Mode.Unknown;
			}
			return m_CurrentSG.SaveGameMode;
		}
	}

	public static bool HasStrategicRestorePoint
	{
		get
		{
			if (m_RestorePoint_Strategic == null || m_RestorePoint_Strategic.ContentContainer == null || m_RestorePoint_Strategic.ContentContainer.MainNode == null)
			{
				return false;
			}
			CFG_SG_Node cFG_SG_Node = m_RestorePoint_Strategic.ContentContainer.MainNode.FindSubNode("Strategic");
			if (cFG_SG_Node == null || cFG_SG_Node.SubNodeCount == 0)
			{
				return false;
			}
			return true;
		}
	}

	public static bool IsLoadingStrategic => m_RestorePoint_Strategic == m_CurrentSG && m_CurrentSG != null;

	public static bool IsLoadingSaveGame => m_RestorePoint_Strategic != m_CurrentSG && m_CurrentSG != null;

	public static CFG_SG_SaveGame CurrentSaveGame => m_CurrentSG;

	public static string LastQuickSaveName => m_LastQuickSaveName;

	public static int IndexOf(string _FileName)
	{
		if (m_SaveGames == null || m_SaveGames.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < m_SaveGames.Count; i++)
		{
			if (string.Compare(_FileName, m_SaveGames[i].FileName, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool SynchronizeScene()
	{
		Debug.Log("Data Sync...");
		if (m_CurrentSG == null || m_LoadType == eLoadType.NoLoad)
		{
			return false;
		}
		CFGSerializableObject.FirstFreeID = m_CurrentSG.FirstFreeUID;
		bool result = true;
		switch (m_LoadType)
		{
		case eLoadType.ReturnToStrategic:
			result = m_CurrentSG.OnReturnToStrategic_Sync();
			break;
		case eLoadType.ResetTactical:
			result = m_CurrentSG.OnRestartTactical();
			break;
		case eLoadType.LoadTactical:
			result = m_CurrentSG.OnFullGameLoad(bStrategic: false);
			break;
		case eLoadType.LoadStrategic:
			result = m_CurrentSG.OnFullGameLoad(bStrategic: true);
			break;
		}
		return result;
	}

	public static bool DoPostSync()
	{
		Debug.Log("Data Post Sync...");
		if (m_CurrentSG == null || m_LoadType == eLoadType.NoLoad)
		{
			return false;
		}
		bool result = false;
		switch (m_LoadType)
		{
		case eLoadType.ReturnToStrategic:
			result = m_CurrentSG.OnReturnToStrategic_PostSync();
			break;
		case eLoadType.ResetTactical:
			result = m_CurrentSG.OnResetTactical_PostSync();
			break;
		case eLoadType.LoadTactical:
			result = m_CurrentSG.OnFullGameLoad_PostSync(bStrategic: false);
			break;
		case eLoadType.LoadStrategic:
			result = m_CurrentSG.OnFullGameLoad_PostSync(bStrategic: true);
			break;
		}
		return result;
	}

	public static void Sync_StrategiOnlyForInvalidTactRestart()
	{
		if (m_CurrentSG != null && IsLoading)
		{
			m_CurrentSG.OnRestoreStrategicDataOnly();
			FinalizeLoading();
		}
	}

	public static bool FinalizeLoading()
	{
		if (m_LoadType != 0 && m_LoadType != eLoadType.ResetTactical)
		{
			CFGSingletonResourcePrefab<CFGTurnManager>.Instance.SetupCurrentOwner(CFGSingletonResourcePrefab<CFGObjectManager>.Instance.PlayerOwner);
			CFGSelectionManager.Instance.OnPostSerialize();
		}
		Debug.Log("Finalize Loading");
		if (m_LoadType == eLoadType.LoadTactical && m_CurrentSG != null)
		{
			m_RestorePoint_Strategic = m_CurrentSG;
		}
		m_LoadType = eLoadType.NoLoad;
		m_CurrentSG = null;
		return true;
	}

	private static bool LoadGame(CFG_SG_SaveGame Save)
	{
		if (Save == null)
		{
			return false;
		}
		CFG_SG_SaveGame.eSG_Type saveGameType = Save.SaveGameType;
		if (saveGameType == CFG_SG_SaveGame.eSG_Type.SinglePlayer)
		{
			CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
			if (cFGSessionSingle == null)
			{
				cFGSessionSingle = CFGSingleton<CFGGame>.Instance.CreateSessionSingle();
				if (cFGSessionSingle == null)
				{
					return false;
				}
			}
			m_CurrentSG = Save;
			return m_LoadType switch
			{
				eLoadType.ReturnToStrategic => cFGSessionSingle.LoadLevelFromSaveGame(cFGSessionSingle.CampaignName, Save.ScenarioName, Save.ScenarioName, null, IsTactical: false, bIsReturnToStrategic: true), 
				eLoadType.ResetTactical => cFGSessionSingle.LoadLevelFromSaveGame(cFGSessionSingle.CampaignName, Save.ScenarioName, Application.loadedLevelName, cFGSessionSingle.CurrentTactical.TacticalID, IsTactical: true, bIsReturnToStrategic: false), 
				eLoadType.LoadStrategic => cFGSessionSingle.LoadLevelFromSaveGame(Save.CampaignName, Save.ScenarioName, Save.SceneName, Save.TacticalID, IsTactical: false, bIsReturnToStrategic: false), 
				eLoadType.LoadTactical => cFGSessionSingle.LoadLevelFromSaveGame(Save.CampaignName, Save.ScenarioName, Save.SceneName, Save.TacticalID, IsTactical: true, bIsReturnToStrategic: false), 
				_ => true, 
			};
		}
		CFGError.ReportError("Unsupported savegame type: " + Save.SaveGameType, CFGError.ErrorCode.FileIsCorrupted);
		return false;
	}

	public static bool CreateRestorePoint_Strategic()
	{
		Debug.Log("CreateRestorePoint_Strategic");
		m_RestorePoint_Strategic.InitData(CFG_SG_SaveGame.eSG_Type.SinglePlayer);
		m_RestorePoint_Strategic.ContentContainer.Reset();
		m_RestorePoint_Strategic.OnStrategic_StoreData();
		m_RestorePoint_Strategic.UpdateData_ModeAndSource(CFG_SG_SaveGame.eSG_Mode.Strategic, CFG_SG_SaveGame.eSG_Source.StandardSave);
		m_RestorePoint_Strategic.WriteHeaderContent();
		return true;
	}

	public static bool ReturnToRestorePoint_Strategic()
	{
		Debug.Log("ReturnToRestorePoint_Strategic");
		if (m_RestorePoint_Strategic.ContentContainer == null)
		{
			CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
			if (sessionSingle == null)
			{
				return false;
			}
			m_RestorePoint_Strategic.InitData(CFG_SG_SaveGame.eSG_Type.SinglePlayer);
			m_RestorePoint_Strategic.WriteHeaderContent();
			m_RestorePoint_Strategic.SceneName = sessionSingle.GetStrategicMapName();
			m_RestorePoint_Strategic.CampaignName = sessionSingle.CampaignName;
		}
		m_LoadType = eLoadType.ReturnToStrategic;
		LoadGame(m_RestorePoint_Strategic);
		return true;
	}

	public static bool RestartTactical()
	{
		Debug.Log("RestartTactical");
		m_LoadType = eLoadType.ResetTactical;
		if (m_RestorePoint_Strategic.ContentContainer == null)
		{
			CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
			if (sessionSingle == null)
			{
				return false;
			}
			m_RestorePoint_Strategic.InitData(CFG_SG_SaveGame.eSG_Type.SinglePlayer);
			m_RestorePoint_Strategic.WriteHeaderContent();
			m_RestorePoint_Strategic.SceneName = Application.loadedLevelName;
			m_RestorePoint_Strategic.CampaignName = sessionSingle.CampaignName;
		}
		m_RestorePoint_Strategic.OnResetTactical_Sync();
		LoadGame(m_RestorePoint_Strategic);
		return true;
	}

	public static bool LoadSaveGameFile(string strFileName)
	{
		CFG_SG_SaveGame cFG_SG_SaveGame = new CFG_SG_SaveGame(strFileName);
		cFG_SG_SaveGame.LoadData();
		if (!cFG_SG_SaveGame.IsContentValid)
		{
			Debug.LogError("File content is invalid: " + strFileName);
			return false;
		}
		switch (cFG_SG_SaveGame.SaveGameMode)
		{
		case CFG_SG_SaveGame.eSG_Mode.Tactical:
			m_LoadType = eLoadType.LoadTactical;
			break;
		case CFG_SG_SaveGame.eSG_Mode.Strategic:
			m_LoadType = eLoadType.LoadStrategic;
			break;
		}
		return LoadGame(cFG_SG_SaveGame);
	}

	public static void RemoveOldSaveGames()
	{
		string[] files = Directory.GetFiles(CFGApplication.ProfileDir, "*.*", SearchOption.TopDirectoryOnly);
		string[] array = files;
		foreach (string fileName in array)
		{
			if (IsOldSaveFile(fileName))
			{
				DeleteFileIfExists(fileName);
			}
		}
	}

	private static bool IsOldSaveFile(string FileName)
	{
		if (!File.Exists(FileName))
		{
			return false;
		}
		if (FileName.EndsWith(".sav"))
		{
			return true;
		}
		if (!FileName.EndsWith(".hws"))
		{
			return false;
		}
		byte[] array = new byte[4];
		if (array == null)
		{
			return false;
		}
		FileStream fileStream = File.OpenRead(FileName);
		if (fileStream == null)
		{
			return false;
		}
		if (fileStream.Length < 4)
		{
			fileStream.Close();
			return true;
		}
		fileStream.Read(array, 0, 4);
		fileStream.Close();
		if ((array[1] == 164 && array[2] == 184 && array[3] == 34) || (array[1] == 82 && array[2] == 31 && array[3] == 195))
		{
			if (array[0] <= 6)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool QuickSaveExist()
	{
		string path = CFGApplication.ProfileDir + "QuickSave.hws";
		return CFGData.Exists(path);
	}

	public static bool LoadQuickSave()
	{
		if ((bool)CFGSelectionManager.Instance && CFGSelectionManager.Instance.LockStatus == ELockReason.Wnd_MissionEnd && (bool)CFGSingleton<CFGWindowMgr>.Instance)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadMissionEnd();
		}
		string text = CFGApplication.ProfileDir + "QuickSave.hws";
		if (CFGData.Exists(text))
		{
			CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.GetSession() as CFGSessionSingle;
			if (cFGSessionSingle != null)
			{
				cFGSessionSingle.PreQuickLoad();
			}
			Debug.Log("Loading quicksave: " + text);
			return LoadSaveGameFile(text);
		}
		return false;
	}

	public static bool LoadLastQuickSave()
	{
		if (m_LastQuickSaveName == null)
		{
			string[] files = Directory.GetFiles(CFGApplication.ProfileDir, "*.hws", SearchOption.TopDirectoryOnly);
			if (files == null)
			{
				return false;
			}
			DateTime value = DateTime.MinValue;
			string[] array = files;
			foreach (string text in array)
			{
				if (text.EndsWith(".hws") && text.Contains("QuickSave"))
				{
					DateTime lastWriteTime = File.GetLastWriteTime(text);
					if (lastWriteTime.CompareTo(value) > 0)
					{
						m_LastQuickSaveName = text;
						value = lastWriteTime;
					}
				}
			}
			if (m_LastQuickSaveName == null)
			{
				return false;
			}
		}
		Debug.Log("Loading quicksave: " + m_LastQuickSaveName);
		return LoadSaveGameFile(m_LastQuickSaveName);
	}

	public static string GetLastSaveName(bool bCheckVersions)
	{
		string[] files = Directory.GetFiles(CFGApplication.ProfileDir, "*.hws", SearchOption.TopDirectoryOnly);
		if (files == null)
		{
			return null;
		}
		string result = null;
		DateTime value = DateTime.MinValue;
		string[] array = files;
		foreach (string text in array)
		{
			if (text.Contains("QuickSave") || !text.EndsWith(".hws"))
			{
				continue;
			}
			DateTime lastWriteTime = File.GetLastWriteTime(text);
			if (lastWriteTime.CompareTo(value) <= 0)
			{
				continue;
			}
			if (bCheckVersions)
			{
				EDLC dlc = EDLC.None;
				string text2 = text.ToLower();
				if (text2.Contains("camp_02"))
				{
					dlc = EDLC.DLC1;
				}
				if (!CFGApplication.IsDLCInstalled(dlc))
				{
					continue;
				}
			}
			result = text;
			value = lastWriteTime;
		}
		return result;
	}

	public static bool CreateSaveGame(CFG_SG_SaveGame.eSG_Type SGType, CFG_SG_SaveGame.eSG_Mode Mode, CFG_SG_SaveGame.eSG_Source Source, string Name)
	{
		CFG_SG_SaveGame cFG_SG_SaveGame = new CFG_SG_SaveGame(Name);
		cFG_SG_SaveGame.InitData(CFG_SG_SaveGame.eSG_Type.SinglePlayer);
		switch (Mode)
		{
		case CFG_SG_SaveGame.eSG_Mode.Tactical:
		{
			if (m_RestorePoint_Strategic.ContentContainer == null || m_RestorePoint_Strategic.ContentContainer.MainNode == null)
			{
				Debug.LogWarning("Saving Tactical: No strategic info!");
			}
			cFG_SG_SaveGame.OnTactical_StoreData();
			CFG_SG_Node cFG_SG_Node = null;
			if (m_RestorePoint_Strategic != null && m_RestorePoint_Strategic.ContentContainer != null && m_RestorePoint_Strategic.ContentContainer.MainNode != null)
			{
				cFG_SG_Node = m_RestorePoint_Strategic.ContentContainer.MainNode.FindSubNode("Strategic");
			}
			if (cFG_SG_Node != null)
			{
				cFG_SG_SaveGame.ContentContainer.MainNode.CreateCloneNode(cFG_SG_Node, "Strategic");
			}
			break;
		}
		case CFG_SG_SaveGame.eSG_Mode.Strategic:
			cFG_SG_SaveGame.OnStrategic_StoreData();
			break;
		}
		cFG_SG_SaveGame.UpdateData_ModeAndSource(Mode, Source);
		cFG_SG_SaveGame.WriteHeaderContent();
		cFG_SG_SaveGame.ContentContainer.SaveData(Name, EContainerFormat.Encoded);
		if (Source == CFG_SG_SaveGame.eSG_Source.QuickSave)
		{
			m_LastQuickSaveName = Name;
		}
		if (m_SaveEnd != null)
		{
			m_SaveEnd();
		}
		return true;
	}

	public static bool ResetStrategicState()
	{
		Debug.Log("ResetStrategicState");
		m_RestorePoint_Strategic = new CFG_SG_SaveGame("Current");
		if (m_RestorePoint_Strategic == null)
		{
			return false;
		}
		return true;
	}

	public static bool Exists(string _FileName)
	{
		return IndexOf(_FileName) != -1;
	}

	private static void ProcessSaveFile(string FileName, bool bLoadInfo)
	{
		if (Exists(FileName))
		{
			Debug.Log("SaveGame Already Exists: " + FileName);
			return;
		}
		CFG_SG_SaveGame cFG_SG_SaveGame = new CFG_SG_SaveGame(FileName);
		if (bLoadInfo)
		{
			cFG_SG_SaveGame.LoadData();
		}
		m_SaveGames.Add(cFG_SG_SaveGame);
	}

	public static string GetMostRecentSave(string CampaignID, string ScenarioID, bool bCheckVersions)
	{
		string result = null;
		DateTime value = DateTime.MinValue;
		foreach (int value2 in Enum.GetValues(typeof(CFG_SG_SaveGame.eSG_Source)))
		{
			if (value2 == 3)
			{
				continue;
			}
			string text = GenerateSaveGameName(CampaignID, ScenarioID, (CFG_SG_SaveGame.eSG_Source)value2);
			if (text == null || !File.Exists(text))
			{
				continue;
			}
			DateTime lastWriteTime = File.GetLastWriteTime(text);
			if (lastWriteTime.CompareTo(value) <= 0)
			{
				continue;
			}
			if (bCheckVersions)
			{
				CFG_SG_SaveGame cFG_SG_SaveGame = new CFG_SG_SaveGame(text);
				cFG_SG_SaveGame.LoadData();
				if (!cFG_SG_SaveGame.IsContentValid)
				{
					continue;
				}
			}
			value = lastWriteTime;
			result = text;
		}
		return result;
	}

	public static bool ContinueScenario(string CampaignID, string ScenarioID)
	{
		if (CampaignID == null || ScenarioID == null)
		{
			return false;
		}
		string mostRecentSave = GetMostRecentSave(CampaignID, ScenarioID, bCheckVersions: false);
		if (mostRecentSave == null)
		{
			Debug.Log("Failed to find any file to continue for campaign " + CampaignID + " Scenario: " + ScenarioID);
			return false;
		}
		return LoadSaveGameFile(mostRecentSave);
	}

	public static string GenerateSaveGameName(string CampaignName, string ScenarioName, CFG_SG_SaveGame.eSG_Source _Source)
	{
		if (string.IsNullOrEmpty(CampaignName) || string.IsNullOrEmpty(ScenarioName))
		{
			return null;
		}
		string text = CFGApplication.ProfileDir + CampaignName + "_" + ScenarioName + "_";
		switch (_Source)
		{
		case CFG_SG_SaveGame.eSG_Source.AutoSave:
		case CFG_SG_SaveGame.eSG_Source.AutoSaveOnLoad:
			text += "AutoSave";
			break;
		case CFG_SG_SaveGame.eSG_Source.QuickSave:
			text = CFGApplication.ProfileDir + "QuickSave";
			break;
		}
		return text + ".hws";
	}

	public static void RemoveScenarioProgress(string CampaignName, string ScenarioName)
	{
		foreach (int value in Enum.GetValues(typeof(CFG_SG_SaveGame.eSG_Source)))
		{
			DeleteFileIfExists(GenerateSaveGameName(CampaignName, ScenarioName, (CFG_SG_SaveGame.eSG_Source)value));
		}
	}

	public static void DeleteFileIfExists(string FileName)
	{
		if (FileName != null && File.Exists(FileName))
		{
			Debug.Log("Deleting file: " + FileName);
			File.Delete(FileName);
		}
	}

	private static void SortSaveGames()
	{
		m_SaveGames.Sort();
	}

	public static void LoadSGList(bool bLoadInfo = false)
	{
		Debug.Log("Scanning save games...");
		m_SaveGames.Clear();
		string[] files = Directory.GetFiles(CFGApplication.ProfileDir, "*.hws");
		string[] array = files;
		foreach (string fileName in array)
		{
			ProcessSaveFile(fileName, bLoadInfo);
		}
		SortSaveGames();
	}
}
