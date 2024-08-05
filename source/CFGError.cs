using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CFGError : CFGWindow
{
	public enum ErrorCode
	{
		None,
		Fail,
		NoMemory,
		NotInitialized,
		FileDoesNotExists,
		VersionTooOld,
		FileIsCorrupted,
		SaveGameSceneVersionIsTooOld,
		SaveGameFlowGraphNodeCountMismatch,
		FailedToReadBytes,
		FailedToWriteBytes,
		ObjectIsNotSerializable,
		InvalidDefinition
	}

	public enum ErrorType
	{
		Info,
		Warning,
		Critical,
		Fatal
	}

	public class ErrorData
	{
		public ErrorCode ECode;

		public ErrorType EType;

		public string Message;

		public string callstack = string.Empty;
	}

	private static ErrorType m_HighestErrorType = ErrorType.Info;

	private static bool m_bShouldGoBackToMainMenu = false;

	private static int m_currenterror = 0;

	private static List<ErrorData> m_Errors = new List<ErrorData>();

	private static bool m_bIsVisible = false;

	private Rect m_WindowRect = new Rect((float)Screen.width * 0.1f, (float)Screen.height * 0.1f, (float)Screen.width - (float)Screen.width * 0.2f, (float)Screen.height - (float)Screen.height * 0.2f);

	protected float fLastToggle;

	public static bool ShouldGoBackToMainMenu => m_bShouldGoBackToMainMenu;

	public static bool NeedToShutdown => m_HighestErrorType == ErrorType.Fatal;

	public static ErrorType HighestErrorType => m_HighestErrorType;

	private string GetInfoByCode(ErrorCode code)
	{
		return code switch
		{
			ErrorCode.NoMemory => "Failed to allocate memory, create object or something like that.", 
			ErrorCode.FileDoesNotExists => "Required file does not exists", 
			ErrorCode.VersionTooOld => "File is too old and cannot be loaded!", 
			ErrorCode.FileIsCorrupted => "File's content is corrupted. Cannot load/fix it!", 
			ErrorCode.SaveGameSceneVersionIsTooOld => "Current scene is different to scene saved in the savegame. This SaveGame is useless now", 
			ErrorCode.SaveGameFlowGraphNodeCountMismatch => "Current scene and savegame's have different Flow Graph node count. This SaveGame is useless now", 
			_ => string.Empty, 
		};
	}

	public static void ResetBackToMainMenu()
	{
		m_bShouldGoBackToMainMenu = false;
	}

	private static string FormatMessage(string InMessage)
	{
		DateTime now = DateTime.Now;
		return now.Hour + ":" + now.Minute + ":" + now.Second + "." + now.Millisecond + " | " + InMessage;
	}

	[Conditional("USE_INFO_REPORTING")]
	public static void ReportInfo(string Info)
	{
		UnityEngine.Debug.Log(FormatMessage(Info));
	}

	public static void Show()
	{
		m_bIsVisible = true;
	}

	[Conditional("USE_ERROR_REPORTING")]
	public static void ReportError(string Message, ErrorCode Code, ErrorType EType = ErrorType.Critical)
	{
		ErrorData errorData = new ErrorData();
		errorData.EType = EType;
		errorData.ECode = Code;
		errorData.Message = FormatMessage(Message);
		if (m_HighestErrorType < EType)
		{
			m_HighestErrorType = EType;
		}
		if (m_HighestErrorType >= ErrorType.Critical)
		{
			m_bShouldGoBackToMainMenu = true;
			StackTrace stackTrace = new StackTrace();
			errorData.callstack = string.Empty;
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				string callstack = errorData.callstack;
				errorData.callstack = string.Concat(callstack, frame.GetMethod().DeclaringType.ToString(), "::", frame.GetMethod(), "\n");
			}
		}
		if (EType >= ErrorType.Critical)
		{
			CFGSingleton<CFGWindowMgr>.Instance.ActivateWindow(EWindowID.ErrorReport);
		}
		switch (EType)
		{
		case ErrorType.Info:
			UnityEngine.Debug.Log(errorData.Message);
			break;
		case ErrorType.Warning:
			UnityEngine.Debug.LogWarning(errorData.Message);
			break;
		default:
			UnityEngine.Debug.LogError(errorData.Message);
			break;
		}
		m_Errors.Add(errorData);
		m_currenterror = m_Errors.Count - 1;
	}

	public override EWindowID GetWindowID()
	{
		return EWindowID.ErrorReport;
	}

	protected override void OnActivate()
	{
	}

	protected override void OnDeactivate()
	{
	}

	private void Awake()
	{
		SetActive(active: true);
	}

	protected override void OnUpdate()
	{
		if (Input.GetKeyDown(KeyCode.KeypadMinus) && Time.realtimeSinceStartup - 0.2f > fLastToggle)
		{
			fLastToggle = Time.realtimeSinceStartup;
			m_bIsVisible = !m_bIsVisible;
		}
	}

	protected override void DrawWindow()
	{
		if (m_Errors.Count != 0 && (m_bIsVisible || m_HighestErrorType == ErrorType.Fatal || m_bShouldGoBackToMainMenu))
		{
			switch (m_HighestErrorType)
			{
			case ErrorType.Fatal:
				GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
				break;
			case ErrorType.Critical:
				GUI.backgroundColor = new Color(2f, 2f, 2f, 1f);
				break;
			default:
				GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);
				break;
			}
			m_WindowRect = GUILayout.Window((int)GetWindowID(), m_WindowRect, MakeWindow, "Error Window (Press Keypad-7 to Toggle)");
		}
	}

	private void MakeWindow(int id)
	{
		Color backgroundColor = GUI.backgroundColor;
		Color backgroundColor2 = new Color(0.9f, 0.9f, 0.9f, 1f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Message " + (m_currenterror + 1) + "/" + m_Errors.Count + "   Highest error: " + m_HighestErrorType);
		GUILayout.FlexibleSpace();
		GUI.backgroundColor = backgroundColor2;
		if (GUILayout.Button("Prev Message", GUILayout.ExpandWidth(expand: false)))
		{
			m_currenterror--;
		}
		if (GUILayout.Button("Next Message", GUILayout.ExpandWidth(expand: false)))
		{
			m_currenterror++;
		}
		GUI.backgroundColor = backgroundColor;
		m_currenterror = Mathf.Clamp(m_currenterror, 0, m_Errors.Count - 1);
		GUILayout.EndHorizontal();
		GUILayout.Space(30f);
		string text = "Type: " + m_Errors[m_currenterror].EType;
		text = text + "\nCode: " + m_Errors[m_currenterror].ECode;
		text = text + "\nCode Info: " + GetInfoByCode(m_Errors[m_currenterror].ECode);
		text = text + "\nMessage: " + m_Errors[m_currenterror].Message;
		if (m_Errors[m_currenterror].callstack != string.Empty)
		{
			text = text + "\nCallstack:\n" + m_Errors[m_currenterror].callstack;
		}
		GUILayout.TextArea(text);
		GUILayout.FlexibleSpace();
		if (m_HighestErrorType == ErrorType.Fatal)
		{
			GUILayout.Label("Fatal error detected. You shoud restart the application!");
		}
		else if (m_HighestErrorType == ErrorType.Critical)
		{
			if (CFGSingleton<CFGGame>.Instance.GetGameState() == EGameState.MainMenu)
			{
				if (m_bShouldGoBackToMainMenu)
				{
					GUILayout.Label("Critical error detected. You shoud restart the application!");
				}
			}
			else
			{
				GUILayout.Label("Critical error detected. You shoud return to the main menu or restart the application!");
				if (m_bShouldGoBackToMainMenu)
				{
					GUI.backgroundColor = backgroundColor2;
					if (GUILayout.Button("Return to MainMenu"))
					{
						GoToMainMenu();
					}
				}
			}
		}
		else if (GUILayout.Button("Close"))
		{
			m_bIsVisible = false;
		}
		GUILayout.Space(50f);
	}

	public static void GoToMainMenu()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadStrategicExplorator();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadMainMenus();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadCharacterScreen();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadBarterScreen();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadStrategicExplorator();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadInGameMenu();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadLoadingScreen();
		CFGSingleton<CFGGame>.Instance.SetGameState(EGameState.UnloadingMission);
		CFGSingleton<CFGGame>.Instance.GoToMainMenus();
		ResetBackToMainMenu();
	}
}
