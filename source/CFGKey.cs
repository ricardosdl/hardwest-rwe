using System;
using UnityEngine;

public class CFGKey
{
	[Flags]
	public enum EModFlag
	{
		None = 0,
		Mod_Ctrl = 1,
		Mod_Alt = 2
	}

	public enum EActivationType
	{
		Continuous,
		OnPress,
		OnRelease,
		Hold
	}

	public const float ReadDelay = 0.15f;

	private ECFGKeyCode m_KC_Value;

	private EModFlag m_ModFlags;

	private bool m_Active;

	private bool m_WasActive;

	private float m_NextRead = -1f;

	private float m_HoldDuration;

	private float m_HoldStart = -1f;

	private EActivationType m_ActivationType = EActivationType.OnRelease;

	public ECFGKeyCode KeyValue => m_KC_Value;

	public EModFlag Flags => m_ModFlags;

	public void Init(EActivationType ActivationType, float HoldDuration)
	{
		m_ActivationType = ActivationType;
		m_HoldDuration = HoldDuration;
	}

	public void Bind(ECFGKeyCode KCode, EModFlag ModFlags)
	{
		m_KC_Value = KCode;
		m_ModFlags = ModFlags;
		m_Active = false;
		m_WasActive = false;
	}

	public void ClearBind(CFGKey other)
	{
		m_ModFlags = EModFlag.None;
		m_KC_Value = ECFGKeyCode.None;
		if (other != null)
		{
			m_KC_Value = other.m_KC_Value;
			if (m_KC_Value != 0)
			{
				m_ModFlags = other.m_ModFlags;
			}
			other.m_KC_Value = ECFGKeyCode.None;
			other.m_ModFlags = EModFlag.None;
		}
	}

	public bool HasFlag(EModFlag _Flag)
	{
		return (m_ModFlags & _Flag) == _Flag;
	}

	public string ToText()
	{
		string text = string.Empty;
		if (HasFlag(EModFlag.Mod_Ctrl))
		{
			text += "Ctrl+";
		}
		if (HasFlag(EModFlag.Mod_Alt))
		{
			text += "Alt+";
		}
		return text + m_KC_Value.ToShortString();
	}

	public string ToTextLong()
	{
		string text = string.Empty;
		if (HasFlag(EModFlag.Mod_Ctrl))
		{
			text += "Ctrl+";
		}
		if (HasFlag(EModFlag.Mod_Alt))
		{
			text += "Alt+";
		}
		return text + m_KC_Value.ToLongString();
	}

	public bool UnBind(ECFGKeyCode kc, bool bAlt, bool bCtrl)
	{
		if (m_KC_Value != kc)
		{
			return false;
		}
		if (HasFlag(EModFlag.Mod_Alt))
		{
			if (!bAlt)
			{
				return false;
			}
		}
		else if (bAlt)
		{
			return false;
		}
		if (HasFlag(EModFlag.Mod_Ctrl))
		{
			if (!bCtrl)
			{
				return false;
			}
		}
		else if (bCtrl)
		{
			return false;
		}
		m_KC_Value = ECFGKeyCode.None;
		m_ModFlags &= ~(EModFlag.Mod_Ctrl | EModFlag.Mod_Alt);
		return true;
	}

	public bool CheckMods(bool bCtrlDown, bool bAltDown)
	{
		EModFlag eModFlag = EModFlag.None;
		if (bCtrlDown)
		{
			eModFlag |= EModFlag.Mod_Ctrl;
		}
		if (bAltDown)
		{
			eModFlag |= EModFlag.Mod_Alt;
		}
		if ((m_ModFlags ^ eModFlag) == EModFlag.None)
		{
			return true;
		}
		return false;
	}

	public bool CheckInput(bool bCtrlDown, bool bAltDown)
	{
		m_Active = false;
		if (m_KC_Value == ECFGKeyCode.None)
		{
			return false;
		}
		if (m_ActivationType != 0 && Time.time < m_NextRead && (m_ActivationType != EActivationType.Hold || m_HoldStart < 0f))
		{
			m_WasActive = false;
			return false;
		}
		bool flag = ReadStatus();
		switch (m_ActivationType)
		{
		case EActivationType.Continuous:
			m_Active = flag;
			break;
		case EActivationType.Hold:
			if (!flag)
			{
				break;
			}
			if (m_WasActive)
			{
				if (Time.time > m_HoldStart)
				{
					m_Active = true;
					m_HoldStart = -1f;
				}
			}
			else
			{
				m_HoldStart = Time.time + m_HoldDuration;
			}
			break;
		case EActivationType.OnPress:
			if (flag && !m_WasActive)
			{
				m_Active = true;
				flag = false;
			}
			break;
		case EActivationType.OnRelease:
			if (!flag && m_WasActive)
			{
				m_Active = true;
				flag = false;
			}
			break;
		}
		m_WasActive = flag;
		if (m_Active)
		{
			m_Active = CheckMods(bCtrlDown, bAltDown);
		}
		if (m_Active)
		{
			m_NextRead = Time.time + 0.15f;
		}
		return m_Active;
	}

	protected bool ReadStatus()
	{
		return m_KC_Value switch
		{
			ECFGKeyCode.MouseWheelUp => Input.GetAxis("Mouse ScrollWheel") > 0f, 
			ECFGKeyCode.MouseWheelDown => Input.GetAxis("Mouse ScrollWheel") < 0f, 
			_ => Input.GetKey((KeyCode)m_KC_Value), 
		};
	}
}
