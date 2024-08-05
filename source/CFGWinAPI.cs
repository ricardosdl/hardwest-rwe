using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CFGWinAPI
{
	[StructLayout(LayoutKind.Sequential)]
	public class CWPSTRUCT
	{
		public IntPtr lParam;

		public IntPtr wParam;

		public uint message;

		public IntPtr hWnd;
	}

	public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

	public const int GCL_HCURSOR = -12;

	public const uint LR_DEFAULTCOLOR = 0u;

	public const uint LR_MONOCHROME = 1u;

	public const uint LR_COLOR = 2u;

	public const uint LR_COPYRETURNORIGINAL = 4u;

	public const uint LR_COPYDELETEORIGINAL = 8u;

	public const uint LR_LOADFROMFILE = 16u;

	public const uint LR_LOADTRANSPARENT = 32u;

	public const uint LR_DEFAULTSIZE = 64u;

	public const uint LR_VGACOLOR = 128u;

	public const uint LR_LOADMAP3DCOLORS = 4096u;

	public const uint LR_CREATEDIBSECTION = 8192u;

	public const uint LR_COPYFROMRESOURCE = 16384u;

	public const uint LR_SHARED = 32768u;

	private static PropertyInfo m_systemCopyBufferProperty;

	public static string ClipBoardAsString
	{
		get
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			return (string)systemCopyBufferProperty.GetValue(null, null);
		}
		set
		{
			PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
			systemCopyBufferProperty.SetValue(null, value, null);
		}
	}

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern int GetDesktopWindow();

	[DllImport("user32.dll")]
	public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

	[DllImport("user32.dll")]
	public static extern int SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr SetCursor(IntPtr hCursor);

	[DllImport("user32.dll")]
	public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

	[DllImport("user32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	public static extern IntPtr LoadImage(IntPtr hInstance, string lpImageName, uint uType, int cxDesired, int cyDesired, uint fuLoad);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCursorPos(int _x, int _y);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetCursorPos(ref POINT _point);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetClipCursor(out RECT rcClip);

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClipCursor(ref RECT rcClip);

	private static void SwitchPause(bool bPause)
	{
		CFGGame instance = CFGSingleton<CFGGame>.Instance;
		if (!(instance == null) && instance.IsInGame() && bPause)
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadInGameMenu();
		}
	}

	public static void OnDestroy()
	{
		UnInstallHook_AppActivate();
	}

	public static void InstallHook_AppActivate()
	{
	}

	public static void UnInstallHook_AppActivate()
	{
	}

	private static PropertyInfo GetSystemCopyBufferProperty()
	{
		if (m_systemCopyBufferProperty == null)
		{
			Type typeFromHandle = typeof(GUIUtility);
			m_systemCopyBufferProperty = typeFromHandle.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
			if (m_systemCopyBufferProperty == null)
			{
				throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
			}
		}
		return m_systemCopyBufferProperty;
	}
}
