using System;

public class CFGActionItem
{
	public enum EGroup
	{
		Global,
		Menus
	}

	[Flags]
	public enum EFlag
	{
		None = 0,
		AllowMods = 2,
		RequireCtrl = 4,
		RequireAlt = 8,
		IsDeveloper = 0x20,
		IsSystem = 0x40,
		Unbindable = 0x80
	}

	public EJoyButton JoyButton_P1_A;

	public EJoyButton JoyButton_P1_B;

	public EJoyButton JoyButton_P2_A;

	public EJoyButton JoyButton_P2_B;

	public EJoyButton JoyButton_P3_A;

	public EJoyButton JoyButton_P3_B;

	public EJoyButton JoyButton_P4_A;

	public EJoyButton JoyButton_P4_B;

	private EFlag m_Flags;

	private CFGKey m_KeyA = new CFGKey();

	private CFGKey m_KeyB = new CFGKey();

	private int m_MenuGroup;

	private int m_MenuItem;

	private EActionCommand m_Cmd;

	private bool m_bActivated;

	private float m_JoyAxisValue;

	private CFGKey.EActivationType m_ActivationType = CFGKey.EActivationType.OnRelease;

	private EGroup m_Group;

	public EActionCommand ActionCommand => m_Cmd;

	public int ActionCommandAsInt => (int)m_Cmd;

	public float AxisValue => m_JoyAxisValue;

	public bool Activated => m_bActivated;

	public int MenuGroup => m_MenuGroup;

	public int MenuItem => m_MenuItem;

	public EGroup Group => m_Group;

	public CFGKey KeyA => m_KeyA;

	public CFGKey KeyB => m_KeyB;

	public CFGKey.EActivationType ActivationType => m_ActivationType;

	public bool HasFlag(EFlag _Flag)
	{
		return (m_Flags & _Flag) == _Flag;
	}

	public void SetMenuGroups(int MenuGroup, int MenuItem, bool bUnBindableByUser)
	{
		m_MenuGroup = MenuGroup;
		m_MenuItem = MenuItem;
		if (bUnBindableByUser)
		{
			m_Flags |= EFlag.Unbindable;
		}
	}

	public string ToText_First()
	{
		return m_KeyA.ToText();
	}

	public string ToText_Second()
	{
		return m_KeyB.ToText();
	}

	public string ToTextLong_First()
	{
		return m_KeyA.ToTextLong();
	}

	public string ToTextLong_Second()
	{
		return m_KeyB.ToTextLong();
	}

	public bool UnBind(ECFGKeyCode kc, bool bAlt, bool bCtrl, EGroup Group, CFGKey.EActivationType ActivationType)
	{
		if (Group != m_Group)
		{
			return false;
		}
		bool flag = m_KeyA.UnBind(kc, bAlt, bCtrl);
		bool flag2 = m_KeyB.UnBind(kc, bAlt, bCtrl);
		if (flag && !flag2)
		{
			m_KeyA.Bind(m_KeyB.KeyValue, m_KeyB.Flags);
			m_KeyB.Bind(ECFGKeyCode.None, CFGKey.EModFlag.None);
		}
		return flag || flag2;
	}

	public void ClearBind(bool bFirst)
	{
		if (bFirst)
		{
			if (m_KeyA != null)
			{
				m_KeyA.ClearBind(m_KeyB);
			}
		}
		else if (m_KeyB != null)
		{
			m_KeyB.ClearBind(null);
		}
	}

	public void ChangeSettings(CFGKey.EActivationType ActivationType, float HoldDuration)
	{
		if (m_KeyA != null)
		{
			m_KeyA.Init(ActivationType, HoldDuration);
		}
		if (m_KeyB != null)
		{
			m_KeyB.Init(ActivationType, HoldDuration);
		}
		m_ActivationType = ActivationType;
	}

	public void Initialize(EActionCommand Cmd, CFGKey.EActivationType ActivationType, EGroup Group, EFlag Flags, float HoldDuration)
	{
		m_Group = Group;
		m_Flags = Flags;
		m_Cmd = Cmd;
		if (m_KeyA != null)
		{
			m_KeyA.Init(ActivationType, HoldDuration);
		}
		if (m_KeyB != null)
		{
			m_KeyB.Init(ActivationType, HoldDuration);
		}
		m_ActivationType = ActivationType;
	}

	public void BindKey(bool First, ECFGKeyCode kc, CFGKey.EModFlag Flags)
	{
		CFGKey.EModFlag eModFlag = CFGKey.EModFlag.None;
		if (HasFlag(EFlag.AllowMods))
		{
			eModFlag = Flags;
			if (HasFlag(EFlag.RequireAlt))
			{
				eModFlag |= CFGKey.EModFlag.Mod_Alt;
			}
			if (HasFlag(EFlag.RequireCtrl))
			{
				eModFlag |= CFGKey.EModFlag.Mod_Ctrl;
			}
		}
		if (First)
		{
			m_KeyA.Bind(kc, eModFlag);
		}
		else
		{
			m_KeyB.Bind(kc, eModFlag);
		}
	}

	public void BindKey(bool First, ECFGKeyCode kc, bool bAlt, bool bCtrl)
	{
		CFGKey.EModFlag eModFlag = CFGKey.EModFlag.None;
		if (HasFlag(EFlag.AllowMods))
		{
			if (bAlt)
			{
				eModFlag |= CFGKey.EModFlag.Mod_Alt;
			}
			if (bCtrl)
			{
				eModFlag |= CFGKey.EModFlag.Mod_Ctrl;
			}
			if (HasFlag(EFlag.RequireAlt))
			{
				eModFlag |= CFGKey.EModFlag.Mod_Alt;
			}
			if (HasFlag(EFlag.RequireCtrl))
			{
				eModFlag |= CFGKey.EModFlag.Mod_Ctrl;
			}
		}
		if (First)
		{
			m_KeyA.Bind(kc, eModFlag);
		}
		else
		{
			m_KeyB.Bind(kc, eModFlag);
		}
	}

	private float GetJoystick(int ProfileID)
	{
		EJoyButton eJoyButton = EJoyButton.Unknown;
		EJoyButton eJoyButton2 = EJoyButton.Unknown;
		switch (ProfileID)
		{
		case 1:
			eJoyButton = JoyButton_P1_A;
			eJoyButton2 = JoyButton_P1_B;
			break;
		case 2:
			eJoyButton = JoyButton_P2_A;
			eJoyButton2 = JoyButton_P2_B;
			break;
		case 3:
			eJoyButton = JoyButton_P3_A;
			eJoyButton2 = JoyButton_P3_B;
			break;
		case 4:
			eJoyButton = JoyButton_P4_A;
			eJoyButton2 = JoyButton_P4_B;
			break;
		default:
			return 0f;
		}
		if (eJoyButton == EJoyButton.Unknown)
		{
			return 0f;
		}
		bool flag = m_ActivationType == CFGKey.EActivationType.Continuous;
		bool flag2 = false;
		if (eJoyButton != 0 && eJoyButton2 != 0)
		{
			flag2 = true;
		}
		bool bUseUp = m_ActivationType == CFGKey.EActivationType.OnRelease;
		float num = CFGJoyManager.ReadAsButton(eJoyButton, flag || flag2, bUseUp);
		if (eJoyButton2 == EJoyButton.Unknown)
		{
			return num;
		}
		if (num < 0.5f)
		{
			return 0f;
		}
		float num2 = CFGJoyManager.ReadAsButton(eJoyButton2, flag, bUseUp);
		if (num2 < 0.5f)
		{
			return 0f;
		}
		return 1f;
	}

	public bool CheckInput(bool bCtrlDown, bool bAltDown)
	{
		m_bActivated = false;
		EInputMode eInputMode = EInputMode.Auto;
		float gamepadDeadZone = CFGInput.GamepadDeadZone;
		m_JoyAxisValue = GetJoystick(CFGOptions.Input.GamepadProfile);
		if (m_JoyAxisValue > gamepadDeadZone)
		{
			m_JoyAxisValue = (m_JoyAxisValue - gamepadDeadZone) / (1f - gamepadDeadZone);
			m_bActivated = true;
			eInputMode = EInputMode.Gamepad;
		}
		else
		{
			m_JoyAxisValue = 0f;
		}
		if (CFGInput.ExclusiveInputDevice == EInputMode.KeyboardAndMouse)
		{
			if (!HasFlag(EFlag.IsDeveloper))
			{
				m_bActivated = false;
				m_JoyAxisValue = 0f;
			}
			eInputMode = EInputMode.Auto;
		}
		if (CFGInput.ExclusiveInputDevice != EInputMode.Gamepad)
		{
			bool flag = m_KeyA.CheckInput(bCtrlDown, bAltDown) | m_KeyB.CheckInput(bCtrlDown, bAltDown);
			if (flag)
			{
				eInputMode = EInputMode.KeyboardAndMouse;
				m_JoyAxisValue = 1f;
			}
			m_bActivated |= flag;
		}
		if (eInputMode != 0)
		{
			CFGInput.ChangeInputMode(eInputMode);
		}
		return m_bActivated;
	}
}
