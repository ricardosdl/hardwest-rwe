using System.Collections.Generic;
using UnityEngine;

public static class CFGJoyManager
{
	public enum EGamepadType
	{
		Unknown,
		X360,
		NVidia_Shield,
		AmazonController,
		XboxOne
	}

	public enum EControllerType
	{
		Unknown,
		XBox360,
		XBox360_Afterglow,
		Logitech_F710,
		NVidia_Shield,
		NykoPlaypadPro,
		AmazonFireGameController
	}

	private const int AXIS_COUNT = 15;

	private static string[] mPADS = new string[6] { "Controller (Wireless Gamepad F710)", "XBOX 360 For Windows", "Afterglow Gamepad for Xbox 360", "NVIDIA Corporation NVIDIA Controller v", "Broadcom Bluetooth HID", "Amazon Fire Game Controller" };

	private static Dictionary<EJoyButton, CFGJBReader> m_BindMap = null;

	private static EGamepadType m_GamePad = EGamepadType.Unknown;

	private static EJoyButton m_CURSORMOVE_UP = EJoyButton.Unknown;

	private static EJoyButton m_CURSORMOVE_DOWN = EJoyButton.Unknown;

	private static EJoyButton m_CURSORMOVE_LEFT = EJoyButton.Unknown;

	private static EJoyButton m_CURSORMOVE_RIGHT = EJoyButton.Unknown;

	private static Dictionary<EJoyAction, CFGJoyAction> m_JoyActions = new Dictionary<EJoyAction, CFGJoyAction>();

	private static float m_fLastCheck = -1f;

	private static Vector3 m_LastMousePos = new Vector3(-1000f, -1f, -1000f);

	private static bool m_JoyActionsClear = false;

	private static float m_JoyActionsClearTime = 0f;

	public static EJoyButton ButtonForCM_UP => m_CURSORMOVE_UP;

	public static EJoyButton ButtonForCM_DOWN => m_CURSORMOVE_DOWN;

	public static EJoyButton ButtonForCM_LEFT => m_CURSORMOVE_LEFT;

	public static EJoyButton ButtonForCM_RIGHT => m_CURSORMOVE_RIGHT;

	public static EGamepadType GamePadType => m_GamePad;

	public static float ReadAsButton(EJoyButton Button, bool bContinous = false, bool bUseUp = false)
	{
		if (m_BindMap == null)
		{
			return 0f;
		}
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			return 0f;
		}
		CFGJBReader value = null;
		if (m_BindMap.TryGetValue(Button, out value))
		{
			float value2 = value.GetValue(bContinous, bUseUp);
			if (value2 > CFGInput.GamepadDeadZone)
			{
				CFGInput.ChangeInputMode(EInputMode.Gamepad);
			}
			return (!(value2 > CFGInput.GamepadDeadZone)) ? 0f : value2;
		}
		return 0f;
	}

	public static void ChangeGamePadProfile(int NewProfile)
	{
		if (NewProfile < 1 || NewProfile > 4)
		{
			Debug.LogWarning("Gamepad profile must be 1, 2, 3 or 4");
			return;
		}
		CFGOptions.Input.GamepadProfile = NewProfile;
		UpdateOtherKeys(NewProfile);
	}

	public static void Set_Amazon_Controller()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A3", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A3", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A13", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A12", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_AxisButton("J1_A5", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_AxisButton("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_AxisButton("J1_A6", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_AxisButton("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button0));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button1));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button2));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button3));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button4));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button8));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button9));
		}
	}

	public static void Set_NVIDIA_SHIELD()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A3", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A3", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A7", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A8", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_AxisButton("J1_A5", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_AxisButton("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_AxisButton("J1_A6", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_AxisButton("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button0));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button1));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button2));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button3));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button4));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.Escape));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.Joystick1Button10));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button8));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button9));
		}
	}

	public static void Set_PC_Windows_X360()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A5", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A3", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A3", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_AxisButton("J1_A6", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_AxisButton("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_AxisButton("J1_A7", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_AxisButton("J1_A7", Positive: false));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button0));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button1));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button2));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button3));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button4));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.Joystick1Button6));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.Joystick1Button7));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button8));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button9));
		}
	}

	public static void Set_PC_Windows_XboxOne()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A5", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A9", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A10", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_AxisButton("J1_A6", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_AxisButton("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_AxisButton("J1_A7", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_AxisButton("J1_A7", Positive: false));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button0));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button1));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button2));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button3));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button4));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.Joystick1Button6));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.Joystick1Button7));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button8));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button9));
		}
	}

	public static void Set_Mac_X360()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A3", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A3", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_Button(KeyCode.Joystick1Button7));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_Button(KeyCode.Joystick1Button8));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_Button(KeyCode.Joystick1Button6));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button16));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button17));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button18));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button19));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button13));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button14));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.Joystick1Button10));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.Joystick1Button9));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button11));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button12));
		}
	}

	public static void Set_X360_Linux()
	{
		m_BindMap = new Dictionary<EJoyButton, CFGJBReader>();
		if (m_BindMap != null)
		{
			m_BindMap.Add(EJoyButton.LA_Left, new CFGJBReader_Axis("J1_A1", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Right, new CFGJBReader_Axis("J1_A1", Positive: true));
			m_BindMap.Add(EJoyButton.LA_Top, new CFGJBReader_Axis("J1_A2", Positive: false));
			m_BindMap.Add(EJoyButton.LA_Bottom, new CFGJBReader_Axis("J1_A2", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Left, new CFGJBReader_Axis("J1_A4", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Right, new CFGJBReader_Axis("J1_A4", Positive: true));
			m_BindMap.Add(EJoyButton.RA_Top, new CFGJBReader_Axis("J1_A5", Positive: false));
			m_BindMap.Add(EJoyButton.RA_Bottom, new CFGJBReader_Axis("J1_A5", Positive: true));
			m_BindMap.Add(EJoyButton.LeftTrigger, new CFGJBReader_Axis("J1_A3", Positive: true));
			m_BindMap.Add(EJoyButton.RightTrigger, new CFGJBReader_Axis("J1_A6", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Left, new CFGJBReader_AxisButton("J1_A7", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Right, new CFGJBReader_AxisButton("J1_A7", Positive: true));
			m_BindMap.Add(EJoyButton.DPad_Top, new CFGJBReader_AxisButton("J1_A8", Positive: false));
			m_BindMap.Add(EJoyButton.DPad_Bottom, new CFGJBReader_AxisButton("J1_A8", Positive: true));
			m_BindMap.Add(EJoyButton.KeyA, new CFGJBReader_Button(KeyCode.Joystick1Button0));
			m_BindMap.Add(EJoyButton.KeyB, new CFGJBReader_Button(KeyCode.Joystick1Button1));
			m_BindMap.Add(EJoyButton.KeyX, new CFGJBReader_Button(KeyCode.Joystick1Button2));
			m_BindMap.Add(EJoyButton.KeyY, new CFGJBReader_Button(KeyCode.Joystick1Button3));
			m_BindMap.Add(EJoyButton.LeftBumper, new CFGJBReader_Button(KeyCode.Joystick1Button4));
			m_BindMap.Add(EJoyButton.RightBumper, new CFGJBReader_Button(KeyCode.Joystick1Button5));
			m_BindMap.Add(EJoyButton.Back, new CFGJBReader_Button(KeyCode.Joystick1Button6));
			m_BindMap.Add(EJoyButton.Start, new CFGJBReader_Button(KeyCode.Joystick1Button7));
			m_BindMap.Add(EJoyButton.Home, new CFGJBReader_Button(KeyCode.None));
			m_BindMap.Add(EJoyButton.LA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button9));
			m_BindMap.Add(EJoyButton.RA_Button, new CFGJBReader_Button(KeyCode.Joystick1Button10));
		}
	}

	public static void UpdateOtherKeys(int Profile)
	{
		m_CURSORMOVE_UP = EJoyButton.LA_Top;
		m_CURSORMOVE_DOWN = EJoyButton.LA_Bottom;
		m_CURSORMOVE_RIGHT = EJoyButton.LA_Right;
		m_CURSORMOVE_LEFT = EJoyButton.LA_Left;
		if (GetJoyButton(EActionCommand.Camera_PanForward, Profile) == EJoyButton.LA_Top)
		{
			m_CURSORMOVE_UP = EJoyButton.RA_Top;
		}
		if (GetJoyButton(EActionCommand.Camera_PanBack, Profile) == EJoyButton.LA_Bottom)
		{
			m_CURSORMOVE_DOWN = EJoyButton.RA_Bottom;
		}
		if (GetJoyButton(EActionCommand.Camera_PanRight, Profile) == EJoyButton.LA_Right)
		{
			m_CURSORMOVE_RIGHT = EJoyButton.RA_Right;
		}
		if (GetJoyButton(EActionCommand.Camera_PanLeft, Profile) == EJoyButton.LA_Left)
		{
			m_CURSORMOVE_LEFT = EJoyButton.RA_Left;
		}
	}

	private static EJoyButton GetJoyButton(EActionCommand cmd, int ProfileID)
	{
		CFGActionItem item = CFGInput.GetItem(cmd);
		if (item == null)
		{
			return EJoyButton.Unknown;
		}
		return ProfileID switch
		{
			1 => item.JoyButton_P1_A, 
			2 => item.JoyButton_P2_A, 
			3 => item.JoyButton_P3_A, 
			4 => item.JoyButton_P4_A, 
			_ => EJoyButton.Unknown, 
		};
	}

	private static EGamepadType GamePadTypeFromName(string Name)
	{
		if (string.IsNullOrEmpty(Name))
		{
			return EGamepadType.Unknown;
		}
		if (Name == "Controller (Xbox One For Windows)")
		{
			return EGamepadType.XboxOne;
		}
		if (Name == "Amazon Fire Game Controller")
		{
			return EGamepadType.AmazonController;
		}
		string text = Name.ToLower();
		if (text.Contains("xbox 360") || text.Contains(mPADS[0]) || text.Contains("xbox") || text.Contains("x-box") || (text.Contains("microsoft") && text.Contains("360")))
		{
			return EGamepadType.X360;
		}
		if (text.Contains("nvidia") || text.Contains(mPADS[4]))
		{
			return EGamepadType.NVidia_Shield;
		}
		return EGamepadType.Unknown;
	}

	private static EGamepadType GetGamePadType()
	{
		string[] joystickNames = Input.GetJoystickNames();
		if (joystickNames == null || joystickNames.Length == 0)
		{
			return EGamepadType.Unknown;
		}
		return GamePadTypeFromName(joystickNames[0]);
	}

	public static void UpdateGamePadType()
	{
		EGamepadType gamePadType = GetGamePadType();
		if (gamePadType == m_GamePad)
		{
			return;
		}
		m_GamePad = gamePadType;
		Debug.Log("Changed controller to: " + m_GamePad);
		switch (m_GamePad)
		{
		case EGamepadType.XboxOne:
			Set_PC_Windows_XboxOne();
			break;
		case EGamepadType.X360:
			if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				Set_X360_Linux();
			}
			else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				Set_Mac_X360();
			}
			else
			{
				Set_PC_Windows_X360();
			}
			break;
		case EGamepadType.NVidia_Shield:
			Set_NVIDIA_SHIELD();
			break;
		case EGamepadType.AmazonController:
			Set_Amazon_Controller();
			break;
		case EGamepadType.Unknown:
			return;
		}
		UpdateOtherKeys(CFGOptions.Input.GamepadProfile);
	}

	public static void Init()
	{
		UpdateGamePadType();
		InitAction(EJoyAction.Dev_PrevPanel, EJoyButton.KeyA, EJoyButton.LeftBumper);
		InitAction(EJoyAction.Dev_NextPanel, EJoyButton.KeyA, EJoyButton.RightBumper);
		InitAction(EJoyAction.EXP_QuickSave, EJoyButton.LeftTrigger, EJoyButton.RightBumper);
		InitAction(EJoyAction.EXP_QuickLoad, EJoyButton.LeftTrigger, EJoyButton.LeftBumper);
		string text = "controller.tsv";
		List<CFGControllerAction> list = CFGTableDataLoader.CreateListFromFile<CFGControllerAction>(CFGData.GetDataPathFor(text));
		if (list == null || list.Count == 0)
		{
			Debug.LogWarning("Failed to read controller actions from " + text);
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Action == EJoyAction.None)
			{
				Debug.LogWarning("Controller actions: 'None' is not supported! #" + i);
				continue;
			}
			if (m_JoyActions.ContainsKey(list[i].Action))
			{
				Debug.LogWarning(string.Concat("Controller actions: [", list[i].Action, "] is already defined!"));
				continue;
			}
			CFGJoyAction cFGJoyAction = new CFGJoyAction(list[i].Key1_B1, list[i].Key1_B2, list[i].Key2_B1, list[i].Key2_B2, list[i].Key3_B1, list[i].Key3_B2, EJoyButton.Unknown, EJoyButton.Unknown);
			if (cFGJoyAction == null)
			{
				return;
			}
			m_JoyActions.Add(list[i].Action, cFGJoyAction);
		}
		Debug.Log("Controller: Initialized " + m_JoyActions.Count + " actions!");
	}

	private static void InitAction(EJoyAction JA, EJoyButton p1b1, EJoyButton p1b2, EJoyButton p2b1 = EJoyButton.Unknown, EJoyButton p2b2 = EJoyButton.Unknown, EJoyButton p3b1 = EJoyButton.Unknown, EJoyButton p3b2 = EJoyButton.Unknown, EJoyButton p4b1 = EJoyButton.Unknown, EJoyButton p4b2 = EJoyButton.Unknown)
	{
		if (JA != 0 && !m_JoyActions.ContainsKey(JA))
		{
			CFGJoyAction cFGJoyAction = new CFGJoyAction(p1b1, p1b2, p2b1, p2b2, p3b1, p3b2, p4b1, p4b2);
			if (cFGJoyAction != null)
			{
				m_JoyActions.Add(JA, cFGJoyAction);
			}
		}
	}

	public static void OnUpdate()
	{
		if (m_fLastCheck + 1f < Time.time)
		{
			UpdateGamePadType();
			m_fLastCheck = Time.time;
		}
		Vector3 mousePosition = Input.mousePosition;
		float num = Vector3.Distance(mousePosition, m_LastMousePos);
		if (num > 3f || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			CFGInput.ChangeInputMode(EInputMode.KeyboardAndMouse);
		}
		m_LastMousePos = mousePosition;
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			return;
		}
		if (m_BindMap != null)
		{
			foreach (KeyValuePair<EJoyButton, CFGJBReader> item in m_BindMap)
			{
				if (item.Value != null)
				{
					item.Value.Read();
				}
			}
		}
		if (m_JoyActionsClear && Time.time > m_JoyActionsClearTime)
		{
			m_JoyActionsClearTime = 0f;
			m_JoyActionsClear = false;
		}
		if (!m_JoyActionsClear)
		{
			UpdateActions();
		}
	}

	private static void UpdateActions()
	{
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			return;
		}
		foreach (KeyValuePair<EJoyAction, CFGJoyAction> joyAction in m_JoyActions)
		{
			joyAction.Value.Read();
		}
	}

	public static bool IsActivated(EJoyAction action)
	{
		if (m_JoyActionsClear)
		{
			return false;
		}
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			return false;
		}
		CFGJoyAction value = null;
		if (!m_JoyActions.TryGetValue(action, out value))
		{
			return false;
		}
		bool isActive = value.IsActive;
		if (isActive)
		{
			CFGInput.ChangeInputMode(EInputMode.Gamepad);
		}
		return isActive;
	}

	public static float GetComposedValue(EJoyAction action)
	{
		if (m_JoyActionsClear)
		{
			return 0f;
		}
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			return 0f;
		}
		CFGJoyAction value = null;
		if (!m_JoyActions.TryGetValue(action, out value))
		{
			return 0f;
		}
		float composedValue = value.GetComposedValue();
		if (composedValue != 0f)
		{
			CFGInput.ChangeInputMode(EInputMode.Gamepad);
		}
		return composedValue;
	}

	public static void ClearJoyActions(float fTime = 1f)
	{
		foreach (KeyValuePair<EJoyAction, CFGJoyAction> joyAction in m_JoyActions)
		{
			joyAction.Value.IsActive = false;
		}
		m_JoyActionsClear = true;
		m_JoyActionsClearTime = Time.time + fTime;
	}
}
