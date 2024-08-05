using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public static class CFGInput
{
	public const int CURRENT_VERSION = 3;

	private static string m_FileName = string.Empty;

	private static bool m_bHaveFocus = true;

	private static bool m_bReadInput = true;

	public static Dictionary<EActionCommand, CFGActionItem> m_AllActions = new Dictionary<EActionCommand, CFGActionItem>();

	private static EInputMode m_LastReadInputDevice = EInputMode.Auto;

	private static EInputMode m_ExclusiveInputDevice = EInputMode.Auto;

	private static List<EActionCommand> m_LastTimeUnbind = new List<EActionCommand>();

	private static bool m_bInitialized = false;

	public static KeyCode[] ValidUserKeys = new KeyCode[114]
	{
		KeyCode.LeftShift,
		KeyCode.Backspace,
		KeyCode.Tab,
		KeyCode.Clear,
		KeyCode.Exclaim,
		KeyCode.DoubleQuote,
		KeyCode.Hash,
		KeyCode.Dollar,
		KeyCode.Ampersand,
		KeyCode.Quote,
		KeyCode.LeftParen,
		KeyCode.RightParen,
		KeyCode.Asterisk,
		KeyCode.Plus,
		KeyCode.Comma,
		KeyCode.Minus,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9,
		KeyCode.Colon,
		KeyCode.Semicolon,
		KeyCode.Less,
		KeyCode.Equals,
		KeyCode.Greater,
		KeyCode.Question,
		KeyCode.At,
		KeyCode.LeftBracket,
		KeyCode.Backslash,
		KeyCode.RightBracket,
		KeyCode.Caret,
		KeyCode.Underscore,
		KeyCode.BackQuote,
		KeyCode.A,
		KeyCode.B,
		KeyCode.C,
		KeyCode.D,
		KeyCode.E,
		KeyCode.F,
		KeyCode.G,
		KeyCode.H,
		KeyCode.I,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.P,
		KeyCode.Q,
		KeyCode.R,
		KeyCode.S,
		KeyCode.T,
		KeyCode.U,
		KeyCode.V,
		KeyCode.W,
		KeyCode.X,
		KeyCode.Y,
		KeyCode.Z,
		KeyCode.Delete,
		KeyCode.Keypad0,
		KeyCode.Keypad1,
		KeyCode.Keypad2,
		KeyCode.Keypad3,
		KeyCode.Keypad4,
		KeyCode.Keypad5,
		KeyCode.Keypad6,
		KeyCode.Keypad7,
		KeyCode.Keypad8,
		KeyCode.Keypad9,
		KeyCode.KeypadPeriod,
		KeyCode.KeypadDivide,
		KeyCode.KeypadMultiply,
		KeyCode.KeypadMinus,
		KeyCode.KeypadPlus,
		KeyCode.KeypadEnter,
		KeyCode.KeypadEquals,
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.RightArrow,
		KeyCode.LeftArrow,
		KeyCode.Insert,
		KeyCode.Home,
		KeyCode.End,
		KeyCode.PageUp,
		KeyCode.PageDown,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12,
		KeyCode.F13,
		KeyCode.F14,
		KeyCode.F15,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6
	};

	private static bool m_ActionsClear = false;

	private static float m_ActionsClearTime = 0f;

	public static EInputMode ExclusiveInputDevice
	{
		get
		{
			return m_ExclusiveInputDevice;
		}
		set
		{
			if (value != m_ExclusiveInputDevice)
			{
				m_ExclusiveInputDevice = value;
				if (value == EInputMode.Gamepad)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}
		}
	}

	public static EInputMode LastReadInputDevice
	{
		get
		{
			if (m_ExclusiveInputDevice == EInputMode.Auto)
			{
				return m_LastReadInputDevice;
			}
			return m_ExclusiveInputDevice;
		}
	}

	public static List<EActionCommand> LastUnbindList => m_LastTimeUnbind;

	public static string FileName
	{
		get
		{
			return m_FileName;
		}
		set
		{
			m_FileName = value;
		}
	}

	public static float GamepadDeadZone
	{
		get
		{
			float num = CFGOptions.Input.GamepadDZ;
			if (LastReadInputDevice == EInputMode.KeyboardAndMouse)
			{
				num *= 2f;
			}
			if (num > 0.8f)
			{
				num = 0.8f;
			}
			return num;
		}
	}

	private static void EnableEventSystem(bool bEnable)
	{
		if (EventSystem.current.currentInputModule != null)
		{
			EventSystem.current.currentInputModule.enabled = bEnable;
		}
	}

	public static CFGActionItem GetItem(EActionCommand cmd)
	{
		m_AllActions.TryGetValue(cmd, out var value);
		return value;
	}

	public static List<CFGActionItem> GetSortedActionsByGroup(int Group, bool HideUnbindable, bool Sort = true)
	{
		List<CFGActionItem> list = new List<CFGActionItem>();
		if (list == null)
		{
			return null;
		}
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value != null && (!HideUnbindable || !allAction.Value.HasFlag(CFGActionItem.EFlag.Unbindable)) && allAction.Value.MenuGroup == Group)
			{
				list.Add(allAction.Value);
			}
		}
		if (Sort)
		{
			list.Sort(CompareAI);
		}
		return list;
	}

	private static int CompareAI(CFGActionItem it1, CFGActionItem it2)
	{
		if (it1.MenuItem > it2.MenuItem)
		{
			return 1;
		}
		if (it1.MenuItem < it2.MenuItem)
		{
			return -1;
		}
		return 0;
	}

	public static Dictionary<EActionCommand, CFGActionItem> GetActionsByGroup(int Group, bool HideUnbindable)
	{
		Dictionary<EActionCommand, CFGActionItem> dictionary = new Dictionary<EActionCommand, CFGActionItem>();
		if (dictionary == null)
		{
			return null;
		}
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value != null && HideUnbindable && allAction.Value.HasFlag(CFGActionItem.EFlag.Unbindable) && allAction.Value.MenuGroup == Group)
			{
				dictionary.Add(allAction.Key, allAction.Value);
			}
		}
		return dictionary;
	}

	public static string GetKeyCombo(EActionCommand cmd, bool First = true)
	{
		CFGActionItem item = GetItem(cmd);
		if (item == null)
		{
			return string.Empty;
		}
		if (First)
		{
			return item.ToText_First();
		}
		return item.ToText_Second();
	}

	public static void Reset_ToDefaults_GD()
	{
		Reset_ToHardCoded();
		CFGAction_BindList cFGAction_BindList = new CFGAction_BindList();
		if (cFGAction_BindList.LoadFromTable(CFGData.GetDataPathFor("keys.tsv")) && cFGAction_BindList.m_Actions.Count > 0)
		{
			InitFromBindList(cFGAction_BindList);
		}
		else
		{
			Debug.LogWarning("Failed to load standard keybindings!");
		}
	}

	public static void Unbind_All()
	{
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			allAction.Value.BindKey(First: true, ECFGKeyCode.None, CFGKey.EModFlag.None);
			allAction.Value.BindKey(First: false, ECFGKeyCode.None, CFGKey.EModFlag.None);
		}
	}

	public static void Reset_ToHardCoded()
	{
		Unbind_All();
		BindKey(EActionCommand.Exit, bFirst: true, ECFGKeyCode.Escape, bAlt: false, bCtrl: false, EJoyButton.Start);
		BindKey(EActionCommand.Confirm, bFirst: true, ECFGKeyCode.Return, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.SYS_ToggleMusic, bFirst: true, ECFGKeyCode.Y, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.SYS_ToggleSound, bFirst: true, ECFGKeyCode.U, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.SYS_ToggleHUD, bFirst: true, ECFGKeyCode.H, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.EndTurn, bFirst: true, ECFGKeyCode.Backspace, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.SelectNext, bFirst: true, ECFGKeyCode.Tab, bAlt: false, bCtrl: false, EJoyButton.DPad_Right, EJoyButton.KeyB);
		BindKey(EActionCommand.SelectPrevious, bFirst: true, ECFGKeyCode.LeftShift, bAlt: false, bCtrl: false, EJoyButton.DPad_Right, EJoyButton.KeyA);
		BindKey(EActionCommand.Character1, bFirst: true, ECFGKeyCode.F1, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Character2, bFirst: true, ECFGKeyCode.F2, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Character3, bFirst: true, ECFGKeyCode.F3, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Character4, bFirst: true, ECFGKeyCode.F4, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Camera_RotateLeft, bFirst: true, ECFGKeyCode.Q, bAlt: false, bCtrl: false, EJoyButton.LeftTrigger);
		BindKey(EActionCommand.Camera_RotateRight, bFirst: true, ECFGKeyCode.E, bAlt: false, bCtrl: false, EJoyButton.RightTrigger);
		BindKey(EActionCommand.Camera_PanLeft, bFirst: true, ECFGKeyCode.A, bAlt: false, bCtrl: false, EJoyButton.LA_Left);
		BindKey(EActionCommand.Camera_PanRight, bFirst: true, ECFGKeyCode.D, bAlt: false, bCtrl: false, EJoyButton.LA_Right);
		BindKey(EActionCommand.Camera_PanForward, bFirst: true, ECFGKeyCode.W, bAlt: false, bCtrl: false, EJoyButton.LA_Top);
		BindKey(EActionCommand.Camera_PanBack, bFirst: true, ECFGKeyCode.S, bAlt: false, bCtrl: false, EJoyButton.LA_Bottom);
		BindKey(EActionCommand.Camera_PanUp, bFirst: true, ECFGKeyCode.PageUp, bAlt: false, bCtrl: false, EJoyButton.RightBumper);
		BindKey(EActionCommand.Camera_PanDown, bFirst: true, ECFGKeyCode.PageDown, bAlt: false, bCtrl: false, EJoyButton.LeftBumper);
		BindKey(EActionCommand.Camera_ChangeFocus, bFirst: true, ECFGKeyCode.G, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.ToggleCharacterInfo, bFirst: true, ECFGKeyCode.I, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Skip_Dialog, bFirst: true, ECFGKeyCode.Space, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Skip_DialogLine, bFirst: true, ECFGKeyCode.Space, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_Window, bFirst: true, ECFGKeyCode.BackQuote, bAlt: false, bCtrl: false, EJoyButton.KeyX, EJoyButton.LeftBumper);
		BindKey(EActionCommand.Dev_Win, bFirst: true, ECFGKeyCode.RightBracket, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.Dev_Lose, bFirst: true, ECFGKeyCode.LeftBracket, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.Dev_SelfHeal, bFirst: true, ECFGKeyCode.Keypad1, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_SelfHeal_Large, bFirst: true, ECFGKeyCode.Keypad1, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.Dev_SelfDamage, bFirst: true, ECFGKeyCode.Keypad2, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_SelfDamage_Large, bFirst: true, ECFGKeyCode.Keypad2, bAlt: true, bCtrl: false);
		BindKey(EActionCommand.Dev_Invulnerable, bFirst: true, ECFGKeyCode.Keypad3, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_MassHeal, bFirst: true, ECFGKeyCode.Keypad8, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_AddItems, bFirst: true, ECFGKeyCode.Keypad9, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_TeleportSelectedChar, bFirst: true, ECFGKeyCode.Keypad7, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_RegenAP, bFirst: true, ECFGKeyCode.Keypad6, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.Dev_UnlockAllScenarios, bFirst: true, ECFGKeyCode.U, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.EXP_QuickSave, bFirst: true, ECFGKeyCode.F10, bAlt: false, bCtrl: false);
		BindKey(EActionCommand.EXP_QuickLoad, bFirst: true, ECFGKeyCode.F11, bAlt: false, bCtrl: false);
		CFGJoyManager.UpdateOtherKeys(CFGOptions.Input.GamepadProfile);
	}

	public static void InitFromBindList(CFGAction_BindList bprof)
	{
		if (!m_bInitialized)
		{
			return;
		}
		foreach (KeyValuePair<EActionCommand, CFGAction_BindList.sAInfo> action in bprof.m_Actions)
		{
			CFGAction_BindList.sAInfo value = action.Value;
			CFGActionItem item = GetItem(value.cmd);
			if (item != null)
			{
				item.BindKey(First: true, value.kA, value.FlagsA);
				item.BindKey(First: false, value.kB, value.FlagsB);
				item.JoyButton_P1_A = value.JB_Profile1_B1;
				item.JoyButton_P1_B = value.JB_Profile1_B2;
				item.JoyButton_P2_A = value.JB_Profile2_B1;
				item.JoyButton_P2_B = value.JB_Profile2_B2;
				item.JoyButton_P3_A = value.JB_Profile3_B1;
				item.JoyButton_P3_B = value.JB_Profile3_B2;
				item.JoyButton_P4_A = value.JB_Profile4_B1;
				item.JoyButton_P4_B = value.JB_Profile4_B2;
				if (value.cmd == EActionCommand.Dev_Window)
				{
					item.JoyButton_P1_A = EJoyButton.LeftBumper;
					item.JoyButton_P1_B = EJoyButton.RightBumper;
				}
				item.ChangeSettings(action.Value.Activationtype, action.Value.HoldDuration);
				item.SetMenuGroups(action.Value.MenuGroup, action.Value.MenuItem, !action.Value.BindableByPlayer);
			}
		}
		CFGJoyManager.UpdateOtherKeys(CFGOptions.Input.GamepadProfile);
	}

	public static bool Init(string _FileName)
	{
		if (m_bInitialized)
		{
			return true;
		}
		ChangeInputMode(EInputMode.KeyboardAndMouse);
		if (string.IsNullOrEmpty(_FileName))
		{
			Debug.LogWarning("CFGInput::Init() must have proper filename set without path!");
			return false;
		}
		m_bInitialized = true;
		if (!RegisterAll())
		{
			return false;
		}
		m_FileName = CFGApplication.ProfileDir + _FileName;
		if (!LoadINI())
		{
			SaveINI();
		}
		CFGJoyManager.Init();
		return true;
	}

	public static void Update()
	{
		if (m_ActionsClear && Time.time > m_ActionsClearTime)
		{
			m_ActionsClearTime = 0f;
			m_ActionsClear = false;
		}
		if (m_ExclusiveInputDevice == EInputMode.Gamepad && m_bHaveFocus)
		{
			Cursor.lockState = CursorLockMode.Locked;
			SetCursorPos(0, 0);
			Cursor.visible = false;
		}
		bool bCtrlDown = Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl);
		bool bAltDown = Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt);
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value != null)
			{
				allAction.Value.CheckInput(bCtrlDown, bAltDown);
			}
		}
		if (ExclusiveInputDevice != EInputMode.Gamepad && !Input.anyKeyDown)
		{
		}
	}

	private static void InitKeyItem(EActionCommand cmd, CFGActionItem.EFlag Flags, CFGKey.EActivationType ActivationType, CFGActionItem.EGroup Group, float HoldDuration = 0f)
	{
		if (!m_AllActions.ContainsKey(cmd))
		{
			CFGActionItem cFGActionItem = new CFGActionItem();
			cFGActionItem.Initialize(cmd, ActivationType, Group, Flags, HoldDuration);
			m_AllActions.Add(cmd, cFGActionItem);
		}
	}

	private static bool RegisterAll()
	{
		m_AllActions.Clear();
		CFGActionItem.EFlag eFlag = CFGActionItem.EFlag.AllowMods;
		CFGActionItem.EGroup group = CFGActionItem.EGroup.Global;
		CFGKey.EActivationType activationType = CFGKey.EActivationType.OnRelease;
		InitKeyItem(EActionCommand.Exit, eFlag | CFGActionItem.EFlag.Unbindable, activationType, group);
		InitKeyItem(EActionCommand.Confirm, eFlag | CFGActionItem.EFlag.Unbindable, activationType, group);
		InitKeyItem(EActionCommand.SelectNext, eFlag, activationType, group);
		InitKeyItem(EActionCommand.SelectPrevious, eFlag, activationType, group);
		InitKeyItem(EActionCommand.EndTurn, eFlag, activationType, group);
		InitKeyItem(EActionCommand.ToggleCharacterInfo, eFlag, activationType, group);
		InitKeyItem(EActionCommand.WeaponChange, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Reload, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Character1, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Character2, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Character3, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Character4, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option1, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option2, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option3, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option4, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option5, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option6, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option7, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option8, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option9, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Option10, eFlag, activationType, group);
		InitKeyItem(EActionCommand.EXP_QuickSave, eFlag, activationType, group);
		InitKeyItem(EActionCommand.EXP_QuickLoad, eFlag, activationType, group);
		eFlag = CFGActionItem.EFlag.AllowMods;
		InitKeyItem(EActionCommand.Camera_ChangeFocus, eFlag, activationType, group);
		activationType = CFGKey.EActivationType.Continuous;
		eFlag = CFGActionItem.EFlag.None;
		InitKeyItem(EActionCommand.Camera_PanLeft, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_PanRight, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_PanForward, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_PanBack, eFlag, activationType, group);
		eFlag = CFGActionItem.EFlag.AllowMods;
		activationType = CFGKey.EActivationType.OnPress;
		InitKeyItem(EActionCommand.Camera_RotateLeft, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_RotateRight, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_PanUp, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Camera_PanDown, eFlag, activationType, group);
		eFlag = CFGActionItem.EFlag.AllowMods | CFGActionItem.EFlag.Unbindable;
		InitKeyItem(EActionCommand.Skip_DialogLine, eFlag, CFGKey.EActivationType.OnPress, group);
		InitKeyItem(EActionCommand.Skip_Dialog, eFlag, CFGKey.EActivationType.Hold, group, 2f);
		InitKeyItem(EActionCommand.SYS_ToggleMusic, eFlag, activationType, group);
		InitKeyItem(EActionCommand.SYS_ToggleSound, eFlag, activationType, group);
		InitKeyItem(EActionCommand.SYS_ToggleHUD, eFlag, activationType, group);
		eFlag = CFGActionItem.EFlag.AllowMods | CFGActionItem.EFlag.IsDeveloper | CFGActionItem.EFlag.Unbindable;
		activationType = CFGKey.EActivationType.OnRelease;
		InitKeyItem(EActionCommand.Dev_Window, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_Win, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_Lose, eFlag, activationType, group);
		group = CFGActionItem.EGroup.Global;
		eFlag = CFGActionItem.EFlag.IsDeveloper | CFGActionItem.EFlag.Unbindable;
		InitKeyItem(EActionCommand.Dev_SelfDamage, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_SelfHeal, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_Invulnerable, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_AddItems, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_MassHeal, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_TeleportSelectedChar, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_RegenAP, eFlag, activationType, group);
		eFlag = CFGActionItem.EFlag.AllowMods | CFGActionItem.EFlag.RequireAlt | CFGActionItem.EFlag.IsDeveloper | CFGActionItem.EFlag.Unbindable;
		InitKeyItem(EActionCommand.Dev_SelfDamage_Large, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_SelfHeal_Large, eFlag, activationType, group);
		InitKeyItem(EActionCommand.Dev_UnlockAllScenarios, eFlag, activationType, group);
		EActionCommand[] array = Enum.GetValues(typeof(EActionCommand)) as EActionCommand[];
		EActionCommand[] array2 = array;
		foreach (EActionCommand eActionCommand in array2)
		{
			if (eActionCommand != 0 && !m_AllActions.ContainsKey(eActionCommand))
			{
				Debug.LogWarning("Missing Action Command: " + eActionCommand.ToString() + ". Please update CFGActionProfile.RegisterAll()");
			}
		}
		Reset_ToDefaults_GD();
		return true;
	}

	public static List<EActionCommand> GetActionsWithoutKeys()
	{
		List<EActionCommand> list = new List<EActionCommand>();
		if (list == null || m_AllActions == null || m_AllActions == null)
		{
			return null;
		}
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value != null)
			{
				CFGActionItem value = allAction.Value;
				if (value.KeyA.KeyValue == ECFGKeyCode.None)
				{
					list.Add(value.ActionCommand);
				}
			}
		}
		return list;
	}

	public static bool IsActivated(EActionCommand cmd)
	{
		if (!m_bReadInput || m_ActionsClear)
		{
			return false;
		}
		return GetItem(cmd)?.Activated ?? false;
	}

	public static float ActivationValue(EActionCommand cmd, bool bOnKeyUp = true)
	{
		if (!m_bReadInput || m_ActionsClear)
		{
			return 0f;
		}
		return GetItem(cmd)?.AxisValue ?? 0f;
	}

	public static void EnableInput(bool bEnable)
	{
		m_bReadInput = bEnable;
	}

	public static bool ClearBind(EActionCommand _Cmd, bool bFirst)
	{
		CFGActionItem item = GetItem(_Cmd);
		if (item == null)
		{
			return false;
		}
		item.ClearBind(bFirst);
		return true;
	}

	public static bool BindKey(EActionCommand _Cmd, bool bFirst, ECFGKeyCode _Key, bool bAlt, bool bCtrl, EJoyButton JB1 = EJoyButton.Unknown, EJoyButton JB2 = EJoyButton.Unknown)
	{
		m_LastTimeUnbind.Clear();
		CFGActionItem item = GetItem(_Cmd);
		if (item == null)
		{
			Debug.LogError("Action command has not been set up: " + _Cmd);
			return false;
		}
		bool first = bFirst;
		if (!item.HasFlag(CFGActionItem.EFlag.AllowMods))
		{
			bAlt = false;
			bCtrl = false;
		}
		if (item.HasFlag(CFGActionItem.EFlag.RequireAlt))
		{
			bAlt = true;
		}
		if (item.HasFlag(CFGActionItem.EFlag.RequireCtrl))
		{
			bCtrl = true;
		}
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value.UnBind(_Key, bAlt, bCtrl, item.Group, item.ActivationType))
			{
				Debug.Log("Unbind key: " + allAction.Value.ActionCommand);
				m_LastTimeUnbind.Add(allAction.Value.ActionCommand);
			}
		}
		if (item.KeyA.KeyValue == ECFGKeyCode.None)
		{
			first = true;
		}
		item.BindKey(first, _Key, bAlt, bCtrl);
		if (JB1 != 0 && JB2 != 0)
		{
			item.JoyButton_P1_A = JB1;
			item.JoyButton_P1_B = JB2;
		}
		return true;
	}

	public static List<EActionCommand> GetGlobalActions()
	{
		List<EActionCommand> list = new List<EActionCommand>();
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			if (allAction.Value.Group == CFGActionItem.EGroup.Global && !allAction.Value.HasFlag(CFGActionItem.EFlag.Unbindable))
			{
				list.Add(allAction.Key);
			}
		}
		return list;
	}

	public static bool SaveINI(string FileName)
	{
		List<string> list = new List<string>();
		list.Add(";Keybindings");
		list.Add(string.Empty);
		list.Add("[General]");
		foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
		{
			string item = allAction.Value.ActionCommand.ToString() + ": KeyA=" + MakeIniKeyCode(allAction.Value.KeyA) + " KeyB=" + MakeIniKeyCode(allAction.Value.KeyB);
			list.Add(item);
		}
		File.WriteAllLines(FileName, list.ToArray());
		return true;
	}

	private static string MakeIniKeyCode(CFGKey key)
	{
		return key.KeyValue.ToString() + "," + (int)key.Flags;
	}

	public static bool SaveINI()
	{
		if (m_FileName == null || m_FileName == string.Empty)
		{
			Debug.LogError("Set CFGInput.FileName before calling CFGInput.SaveINI");
			return false;
		}
		List<string> list = new List<string>();
		list.Add(";Keybindings for Hard-West");
		list.Add(";v." + 3);
		foreach (int value in Enum.GetValues(typeof(CFGActionItem.EGroup)))
		{
			int num = 0;
			foreach (KeyValuePair<EActionCommand, CFGActionItem> allAction in m_AllActions)
			{
				if (allAction.Value.Group == (CFGActionItem.EGroup)value)
				{
					string text = "Bind (";
					text += allAction.Value.ActionCommand;
					text += ",";
					string text2 = text;
					text = text2 + allAction.Value.KeyA.KeyValue.ToString() + "," + (int)allAction.Value.KeyA.Flags + ",";
					text2 = text;
					text = text2 + allAction.Value.KeyB.KeyValue.ToString() + "," + (int)allAction.Value.KeyB.Flags + ")";
					if (num == 0)
					{
						list.Add(string.Empty);
						list.Add("[" + ((CFGActionItem.EGroup)value).ToString() + "]");
					}
					list.Add(text);
					num++;
				}
			}
		}
		try
		{
			CFGOptions.MakeProfileDir();
			File.WriteAllLines(m_FileName, list.ToArray());
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write input configuration: " + ex);
		}
		return true;
	}

	public static bool LoadINI()
	{
		if (string.IsNullOrEmpty(m_FileName))
		{
			return false;
		}
		string[] array = null;
		if (File.Exists(m_FileName))
		{
			array = File.ReadAllLines(m_FileName);
		}
		if (array == null || array.Length == 0)
		{
			SaveINI();
			return false;
		}
		char[] separator = new char[4] { '(', ')', ',', ' ' };
		int result = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim();
			if (text2 == null || text2 == string.Empty || text2.StartsWith("#"))
			{
				continue;
			}
			if (text2.StartsWith(";"))
			{
				if (text2.StartsWith(";v."))
				{
					string s = text2.Substring(3, text2.Length - 3);
					if (!int.TryParse(s, out result))
					{
					}
				}
			}
			else
			{
				if (text2.StartsWith("["))
				{
					continue;
				}
				int num = text2.IndexOf("(");
				if (num < 0)
				{
					Debug.LogWarning("Input: " + m_FileName + " has invalid line: " + text2);
					continue;
				}
				if (!text2.EndsWith(")"))
				{
					Debug.LogWarning("Input: " + m_FileName + " Syntax error (no ')'): " + text2);
					continue;
				}
				string[] array3 = text2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (string.Compare("bind", array3[0], ignoreCase: true) == 0)
				{
					EActionCommand eActionCommand = EActionCommand.None;
					try
					{
						eActionCommand = (EActionCommand)(int)Enum.Parse(typeof(EActionCommand), array3[1], ignoreCase: true);
					}
					catch
					{
					}
					CFGActionItem item = GetItem(eActionCommand);
					if (item == null)
					{
						Debug.LogWarning("No action for : " + array3[1] + " command = " + eActionCommand);
					}
					else if (array3.Length < 6)
					{
						Debug.LogWarning("too few words in : " + text2 + " command = " + eActionCommand);
					}
					else
					{
						ParseKeyAndFlags(out var keyc, out var flags, array3[2], array3[3]);
						item.BindKey(First: true, keyc, (CFGKey.EModFlag)flags);
						ParseKeyAndFlags(out keyc, out flags, array3[4], array3[5]);
						item.BindKey(First: false, keyc, (CFGKey.EModFlag)flags);
					}
				}
			}
		}
		if (result < 3)
		{
			Debug.LogWarning("Key Config is out of date. Reseting to GD Defaults...");
			Reset_ToDefaults_GD();
			return false;
		}
		return true;
	}

	private static void ParseKeyAndFlags(out ECFGKeyCode keyc, out int flags, string word1, string word2)
	{
		keyc = ECFGKeyCode.None;
		flags = 0;
		try
		{
			keyc = (ECFGKeyCode)(int)Enum.Parse(typeof(ECFGKeyCode), word1, ignoreCase: true);
			flags = int.Parse(word2);
		}
		catch
		{
		}
	}

	public static void ChangeInputMode(EInputMode NewIM)
	{
		if (m_ExclusiveInputDevice == EInputMode.Auto && NewIM != m_LastReadInputDevice)
		{
			m_LastReadInputDevice = NewIM;
			CFGSingleton<CFGWindowMgr>.Instance.OnInputModeChange(NewIM);
			switch (m_LastReadInputDevice)
			{
			case EInputMode.Gamepad:
				Cursor.visible = false;
				break;
			case EInputMode.KeyboardAndMouse:
				Cursor.visible = true;
				break;
			}
		}
	}

	public static void OnApplicationChangeFocus(bool bHaveFocus)
	{
		if (m_ExclusiveInputDevice == EInputMode.Gamepad && bHaveFocus)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		m_bHaveFocus = bHaveFocus;
	}

	public static void ClearActions(float fTime = 1f)
	{
		m_ActionsClear = true;
		m_ActionsClearTime = Time.time + fTime;
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetCursorPos(int _x, int _y);
}
