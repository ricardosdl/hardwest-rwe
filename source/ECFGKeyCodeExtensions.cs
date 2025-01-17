public static class ECFGKeyCodeExtensions
{
	public static bool CanPlayerBind(this ECFGKeyCode val)
	{
		if (val == ECFGKeyCode.None || val == ECFGKeyCode.Return || val == ECFGKeyCode.Escape || val == ECFGKeyCode.Space)
		{
			return true;
		}
		return false;
	}

	public static bool IsModifier(this ECFGKeyCode val)
	{
		switch (val)
		{
		case ECFGKeyCode.RightControl:
		case ECFGKeyCode.LeftControl:
		case ECFGKeyCode.RightAlt:
		case ECFGKeyCode.LeftAlt:
			return true;
		default:
			return false;
		}
	}

	public static string ToLongString(this ECFGKeyCode val)
	{
		return val switch
		{
			ECFGKeyCode.None => string.Empty, 
			ECFGKeyCode.Backspace => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_backspace"), 
			ECFGKeyCode.Delete => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_delete"), 
			ECFGKeyCode.Tab => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_tab"), 
			ECFGKeyCode.Clear => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_clear"), 
			ECFGKeyCode.Return => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_return"), 
			ECFGKeyCode.Pause => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_pause"), 
			ECFGKeyCode.Escape => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_escape"), 
			ECFGKeyCode.Space => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_space"), 
			ECFGKeyCode.Keypad0 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad0"), 
			ECFGKeyCode.Keypad1 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad1"), 
			ECFGKeyCode.Keypad2 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad2"), 
			ECFGKeyCode.Keypad3 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad3"), 
			ECFGKeyCode.Keypad4 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad4"), 
			ECFGKeyCode.Keypad5 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad5"), 
			ECFGKeyCode.Keypad6 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad6"), 
			ECFGKeyCode.Keypad7 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad7"), 
			ECFGKeyCode.Keypad8 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad8"), 
			ECFGKeyCode.Keypad9 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypad9"), 
			ECFGKeyCode.KeypadPeriod => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadperiod"), 
			ECFGKeyCode.KeypadDivide => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypaddivide"), 
			ECFGKeyCode.KeypadMultiply => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadmultiply"), 
			ECFGKeyCode.KeypadMinus => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadminus"), 
			ECFGKeyCode.KeypadPlus => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadplus"), 
			ECFGKeyCode.KeypadEnter => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadenter"), 
			ECFGKeyCode.KeypadEquals => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_keypadequals"), 
			ECFGKeyCode.UpArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_uparrow"), 
			ECFGKeyCode.DownArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_downarrow"), 
			ECFGKeyCode.RightArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightarrow"), 
			ECFGKeyCode.LeftArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftarrow"), 
			ECFGKeyCode.Insert => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_insert"), 
			ECFGKeyCode.Home => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_home"), 
			ECFGKeyCode.End => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_end"), 
			ECFGKeyCode.PageUp => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_pageup"), 
			ECFGKeyCode.PageDown => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_pagedown"), 
			ECFGKeyCode.Alpha0 => "0", 
			ECFGKeyCode.Alpha1 => "1", 
			ECFGKeyCode.Alpha2 => "2", 
			ECFGKeyCode.Alpha3 => "3", 
			ECFGKeyCode.Alpha4 => "4", 
			ECFGKeyCode.Alpha5 => "5", 
			ECFGKeyCode.Alpha6 => "6", 
			ECFGKeyCode.Alpha7 => "7", 
			ECFGKeyCode.Alpha8 => "8", 
			ECFGKeyCode.Alpha9 => "9", 
			ECFGKeyCode.Exclaim => "!", 
			ECFGKeyCode.DoubleQuote => "\"", 
			ECFGKeyCode.Hash => "#", 
			ECFGKeyCode.Dollar => "$", 
			ECFGKeyCode.Ampersand => "&", 
			ECFGKeyCode.Quote => "'", 
			ECFGKeyCode.LeftParen => "(", 
			ECFGKeyCode.RightParen => ")", 
			ECFGKeyCode.Asterisk => "*", 
			ECFGKeyCode.Plus => "+", 
			ECFGKeyCode.Comma => ",", 
			ECFGKeyCode.Minus => "-", 
			ECFGKeyCode.Period => ".", 
			ECFGKeyCode.Slash => "/", 
			ECFGKeyCode.Colon => ":", 
			ECFGKeyCode.Semicolon => ";", 
			ECFGKeyCode.Less => "<", 
			ECFGKeyCode.Equals => "=", 
			ECFGKeyCode.Greater => ">", 
			ECFGKeyCode.Question => "?", 
			ECFGKeyCode.At => "@", 
			ECFGKeyCode.LeftBracket => "[", 
			ECFGKeyCode.Backslash => "\\", 
			ECFGKeyCode.RightBracket => "]", 
			ECFGKeyCode.Caret => "^", 
			ECFGKeyCode.Underscore => "_", 
			ECFGKeyCode.BackQuote => "`", 
			ECFGKeyCode.NumLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_numlock"), 
			ECFGKeyCode.CapsLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_capslock"), 
			ECFGKeyCode.ScrollLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_scrolllock"), 
			ECFGKeyCode.RightShift => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightshift"), 
			ECFGKeyCode.LeftShift => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftshift"), 
			ECFGKeyCode.RightControl => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightcontrol"), 
			ECFGKeyCode.LeftControl => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftcontrol"), 
			ECFGKeyCode.RightAlt => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightalt"), 
			ECFGKeyCode.LeftAlt => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftalt"), 
			ECFGKeyCode.LeftApple => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftapple"), 
			ECFGKeyCode.LeftWindows => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_leftwindows"), 
			ECFGKeyCode.RightApple => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightapple"), 
			ECFGKeyCode.RightWindows => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_rightwindows"), 
			ECFGKeyCode.AltGr => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_altgr"), 
			ECFGKeyCode.Help => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_help"), 
			ECFGKeyCode.Print => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_print"), 
			ECFGKeyCode.SysReq => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_sysreq"), 
			ECFGKeyCode.Break => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_break"), 
			ECFGKeyCode.Menu => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_menu"), 
			ECFGKeyCode.Mouse0 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse0"), 
			ECFGKeyCode.Mouse1 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse1"), 
			ECFGKeyCode.Mouse2 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse2"), 
			ECFGKeyCode.Mouse3 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse3"), 
			ECFGKeyCode.Mouse4 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse4"), 
			ECFGKeyCode.Mouse5 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse5"), 
			ECFGKeyCode.Mouse6 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mouse6"), 
			ECFGKeyCode.MouseWheelUp => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mousewheelup"), 
			ECFGKeyCode.MouseWheelDown => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("kc_mousewheeldown"), 
			_ => val.ToString(), 
		};
	}

	public static string ToShortString(this ECFGKeyCode val)
	{
		return val switch
		{
			ECFGKeyCode.None => string.Empty, 
			ECFGKeyCode.Backspace => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_backspace"), 
			ECFGKeyCode.Delete => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_delete"), 
			ECFGKeyCode.Tab => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_tab"), 
			ECFGKeyCode.Clear => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_clear"), 
			ECFGKeyCode.Return => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_return"), 
			ECFGKeyCode.Pause => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_pause"), 
			ECFGKeyCode.Escape => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_escape"), 
			ECFGKeyCode.Space => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_space"), 
			ECFGKeyCode.Keypad0 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad0"), 
			ECFGKeyCode.Keypad1 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad1"), 
			ECFGKeyCode.Keypad2 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad2"), 
			ECFGKeyCode.Keypad3 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad3"), 
			ECFGKeyCode.Keypad4 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad4"), 
			ECFGKeyCode.Keypad5 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad5"), 
			ECFGKeyCode.Keypad6 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad6"), 
			ECFGKeyCode.Keypad7 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad7"), 
			ECFGKeyCode.Keypad8 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad8"), 
			ECFGKeyCode.Keypad9 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypad9"), 
			ECFGKeyCode.KeypadPeriod => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadperiod"), 
			ECFGKeyCode.KeypadDivide => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypaddivide"), 
			ECFGKeyCode.KeypadMultiply => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadmultiply"), 
			ECFGKeyCode.KeypadMinus => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadminus"), 
			ECFGKeyCode.KeypadPlus => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadplus"), 
			ECFGKeyCode.KeypadEnter => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadenter"), 
			ECFGKeyCode.KeypadEquals => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_keypadequals"), 
			ECFGKeyCode.UpArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_uparrow"), 
			ECFGKeyCode.DownArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_downarrow"), 
			ECFGKeyCode.RightArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightarrow"), 
			ECFGKeyCode.LeftArrow => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftarrow"), 
			ECFGKeyCode.Insert => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_insert"), 
			ECFGKeyCode.Home => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_home"), 
			ECFGKeyCode.End => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_end"), 
			ECFGKeyCode.PageUp => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_pageup"), 
			ECFGKeyCode.PageDown => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_pagedown"), 
			ECFGKeyCode.Alpha0 => "0", 
			ECFGKeyCode.Alpha1 => "1", 
			ECFGKeyCode.Alpha2 => "2", 
			ECFGKeyCode.Alpha3 => "3", 
			ECFGKeyCode.Alpha4 => "4", 
			ECFGKeyCode.Alpha5 => "5", 
			ECFGKeyCode.Alpha6 => "6", 
			ECFGKeyCode.Alpha7 => "7", 
			ECFGKeyCode.Alpha8 => "8", 
			ECFGKeyCode.Alpha9 => "9", 
			ECFGKeyCode.Exclaim => "!", 
			ECFGKeyCode.DoubleQuote => "\"", 
			ECFGKeyCode.Hash => "#", 
			ECFGKeyCode.Dollar => "$", 
			ECFGKeyCode.Ampersand => "&", 
			ECFGKeyCode.Quote => "'", 
			ECFGKeyCode.LeftParen => "(", 
			ECFGKeyCode.RightParen => ")", 
			ECFGKeyCode.Asterisk => "*", 
			ECFGKeyCode.Plus => "+", 
			ECFGKeyCode.Comma => ",", 
			ECFGKeyCode.Minus => "-", 
			ECFGKeyCode.Period => ".", 
			ECFGKeyCode.Slash => "/", 
			ECFGKeyCode.Colon => ":", 
			ECFGKeyCode.Semicolon => ";", 
			ECFGKeyCode.Less => "<", 
			ECFGKeyCode.Equals => "=", 
			ECFGKeyCode.Greater => ">", 
			ECFGKeyCode.Question => "?", 
			ECFGKeyCode.At => "@", 
			ECFGKeyCode.LeftBracket => "[", 
			ECFGKeyCode.Backslash => "\\", 
			ECFGKeyCode.RightBracket => "]", 
			ECFGKeyCode.Caret => "^", 
			ECFGKeyCode.Underscore => "_", 
			ECFGKeyCode.BackQuote => "`", 
			ECFGKeyCode.NumLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_numlock"), 
			ECFGKeyCode.CapsLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_capslock"), 
			ECFGKeyCode.ScrollLock => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_scrolllock"), 
			ECFGKeyCode.RightShift => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightshift"), 
			ECFGKeyCode.LeftShift => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftshift"), 
			ECFGKeyCode.RightControl => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightcontrol"), 
			ECFGKeyCode.LeftControl => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftcontrol"), 
			ECFGKeyCode.RightAlt => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightalt"), 
			ECFGKeyCode.LeftAlt => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftalt"), 
			ECFGKeyCode.LeftApple => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftapple"), 
			ECFGKeyCode.LeftWindows => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_leftwindows"), 
			ECFGKeyCode.RightApple => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightapple"), 
			ECFGKeyCode.RightWindows => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_rightwindows"), 
			ECFGKeyCode.AltGr => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_altgr"), 
			ECFGKeyCode.Help => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_help"), 
			ECFGKeyCode.Print => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_print"), 
			ECFGKeyCode.SysReq => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_sysreq"), 
			ECFGKeyCode.Break => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_break"), 
			ECFGKeyCode.Menu => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_menu"), 
			ECFGKeyCode.Mouse0 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse0"), 
			ECFGKeyCode.Mouse1 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse1"), 
			ECFGKeyCode.Mouse2 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse2"), 
			ECFGKeyCode.Mouse3 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse3"), 
			ECFGKeyCode.Mouse4 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse4"), 
			ECFGKeyCode.Mouse5 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse5"), 
			ECFGKeyCode.Mouse6 => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mouse6"), 
			ECFGKeyCode.MouseWheelUp => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mousewheelup"), 
			ECFGKeyCode.MouseWheelDown => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("skc_mousewheeldown"), 
			_ => val.ToString(), 
		};
	}
}
