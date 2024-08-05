using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

public class CFGApplication : MonoBehaviour
{
	private const string BUILD_NAME_FILE = "build_name.txt";

	private static string s_BuildName = string.Empty;

	private static string m_DataDir = string.Empty;

	private static string m_ProfileDir = string.Empty;

	private static bool m_bInitialized;

	private static bool m_bDLC1Installed;

	public static string BuildName
	{
		get
		{
			return s_BuildName;
		}
		private set
		{
			s_BuildName = value;
		}
	}

	public static string DataDir => m_DataDir;

	public static string ProfileDir => m_ProfileDir;

	public static bool IsDLCInstalled(EDLC Dlc)
	{
		return Dlc switch
		{
			EDLC.None => true, 
			EDLC.DLC1 => m_bDLC1Installed, 
			_ => false, 
		};
	}

	private void Awake()
	{
		ReadBuildName("build_name.txt");
		Debug.Log("Build version: " + BuildName, this);
		SetDllPath();
	}

	private static void CheckDLC()
	{
		m_bDLC1Installed = false;
		if (CFGData.Exists(CFGData.GetDataPathFor("content.dat", EDLC.DLC1)))
		{
			Debug.LogFormat("Found DLC1");
			m_bDLC1Installed = true;
		}
	}

	private void Start()
	{
		Debug.Log("Application started.");
		string message = "Unity version: " + Application.unityVersion + "\nRuntime platform: " + Application.platform.ToString() + "\nOS: " + SystemInfo.operatingSystem + "\nCPU: " + SystemInfo.processorType + ", count: " + SystemInfo.processorCount + "\nRAM: " + SystemInfo.systemMemorySize + " MB\nVideo: " + SystemInfo.graphicsDeviceName + "\nVRAM: " + SystemInfo.graphicsMemorySize + " MB\nVideo API: " + SystemInfo.graphicsDeviceVersion + "\nShader model: " + SystemInfo.graphicsShaderLevel.ToString();
		Debug.Log(message);
		Init();
	}

	public static void Init()
	{
		if (!m_bInitialized)
		{
			m_bInitialized = true;
			CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
			cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			m_DataDir = Directory.GetCurrentDirectory() + "/";
			string arg = "0";
			if (CFGSingleton<CFGSteam>.Instance.Initialized)
			{
				arg = CFGSingleton<CFGSteam>.Instance.SteamUserID.ToString();
			}
			m_ProfileDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/HardWest/{arg}/";
			CFGOptions.MakeProfileDir();
			CFGStaticDataContainer.Init();
			CFGInput.Init("keys.ini");
			CFGOptions.FileName = m_ProfileDir + "options.ini";
			CFGOptions.Load();
			CFGOptions.Graphics.ApplyToUnity();
			CFGOptions.Audio.ApplyToUnity();
			CFG_SG_Manager.RemoveOldSaveGames();
			CheckDLC();
			bool flag = false;
			if (CFGSingleton<CFGSteam>.Instance.Initialized)
			{
				flag = true;
				Debug.Log("Steam initialized.");
			}
			CFGSingleton<CFGWindowMgr>.Instance.Init();
			CFGVariableContainer.Instance.Init();
			CFGVariableContainer.Instance.LoadValuesGlobal(null, bCampaign: false, bProfile: true);
			CFGSingleton<CFGGame>.Instance.Init();
			CFGAchievmentTracker.InitData();
		}
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

	private void ReadBuildName(string file_name)
	{
		try
		{
			BuildName = File.ReadAllText(file_name);
		}
		catch
		{
			BuildName = "UNKNOWN";
		}
	}

	private void OnApplicationQuit()
	{
		CFGWinAPI.UnInstallHook_AppActivate();
	}
}
