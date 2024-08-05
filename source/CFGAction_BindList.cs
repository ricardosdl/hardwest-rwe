using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CFGAction_BindList
{
	public class sAInfo
	{
		public EActionCommand cmd;

		public ECFGKeyCode kA;

		public ECFGKeyCode kB;

		public CFGKey.EModFlag FlagsA;

		public CFGKey.EModFlag FlagsB;

		public EJoyButton JB_Profile1_B1;

		public EJoyButton JB_Profile1_B2;

		public EJoyButton JB_Profile2_B1;

		public EJoyButton JB_Profile2_B2;

		public EJoyButton JB_Profile3_B1;

		public EJoyButton JB_Profile3_B2;

		public EJoyButton JB_Profile4_B1;

		public EJoyButton JB_Profile4_B2;

		public int MenuGroup;

		public int MenuItem;

		public bool BindableByPlayer = true;

		public CFGKey.EActivationType Activationtype = CFGKey.EActivationType.OnRelease;

		public float HoldDuration;

		internal sAInfo(EActionCommand _Cmd, ECFGKeyCode _KeyA, ECFGKeyCode _KeyB, CFGKey.EModFlag _FlagsA, CFGKey.EModFlag _FlagsB)
		{
			cmd = _Cmd;
			kA = _KeyA;
			kB = _KeyB;
			FlagsA = _FlagsA;
			FlagsB = _FlagsB;
		}

		internal sAInfo(sAInfo other)
		{
			cmd = other.cmd;
			kA = other.kA;
			kB = other.kB;
			FlagsA = other.FlagsA;
			FlagsB = other.FlagsB;
		}
	}

	public string m_Name = string.Empty;

	public Dictionary<EActionCommand, sAInfo> m_Actions = new Dictionary<EActionCommand, sAInfo>();

	public bool Init()
	{
		if (m_Actions == null)
		{
			m_Actions = new Dictionary<EActionCommand, sAInfo>();
		}
		if (m_Actions == null)
		{
			Debug.Log("Failed to allocate memory for key-binding's profile");
			return false;
		}
		m_Actions.Clear();
		return true;
	}

	public bool InitAsClone(CFGAction_BindList src)
	{
		if (!Init())
		{
			return false;
		}
		foreach (KeyValuePair<EActionCommand, sAInfo> action in src.m_Actions)
		{
			if (action.Value != null)
			{
				m_Actions.Add(action.Key, new sAInfo(action.Value));
			}
		}
		return true;
	}

	public sAInfo GetItem(EActionCommand cmd)
	{
		if (m_Actions == null)
		{
			return null;
		}
		sAInfo value = null;
		m_Actions.TryGetValue(cmd, out value);
		return value;
	}

	public bool SaveToIni(string FileName)
	{
		List<string> list = new List<string>();
		list.Add(";Keybindings");
		list.Add("[General]");
		foreach (KeyValuePair<EActionCommand, sAInfo> action in m_Actions)
		{
			string item = action.Value.cmd.ToString() + ": KeyA=" + MakeIniKeyCode(action.Value.kA, action.Value.FlagsA) + " KeyB=" + MakeIniKeyCode(action.Value.kB, action.Value.FlagsB);
			list.Add(item);
		}
		File.WriteAllLines(FileName, list.ToArray());
		return true;
	}

	private string MakeIniKeyCode(ECFGKeyCode key, CFGKey.EModFlag flag)
	{
		string text = key.ToString();
		int num = (int)flag;
		return text + "," + num;
	}

	public bool LoadFromIni(string FileName)
	{
		return true;
	}

	public bool LoadFromTable(string FileName)
	{
		CFGTableDataLoader cFGTableDataLoader = new CFGTableDataLoader();
		m_Actions.Clear();
		if (!cFGTableDataLoader.OpenTableFile(FileName))
		{
			return false;
		}
		int columnID = cFGTableDataLoader.GetColumnID("Action");
		int columnID2 = cFGTableDataLoader.GetColumnID("KeyA_Code");
		int columnID3 = cFGTableDataLoader.GetColumnID("KeyA_Mod");
		int columnID4 = cFGTableDataLoader.GetColumnID("KeyB_Code");
		int columnID5 = cFGTableDataLoader.GetColumnID("KeyB_Mod");
		int columnID6 = cFGTableDataLoader.GetColumnID("MenuGroup");
		int columnID7 = cFGTableDataLoader.GetColumnID("MenuItem");
		int columnID8 = cFGTableDataLoader.GetColumnID("Bindable");
		int columnID9 = cFGTableDataLoader.GetColumnID("ActivationType");
		int columnID10 = cFGTableDataLoader.GetColumnID("HoldDuration");
		int columnID11 = cFGTableDataLoader.GetColumnID("JoyP1_B1");
		int columnID12 = cFGTableDataLoader.GetColumnID("JoyP1_B2");
		int columnID13 = cFGTableDataLoader.GetColumnID("JoyP2_B1");
		int columnID14 = cFGTableDataLoader.GetColumnID("JoyP2_B2");
		int columnID15 = cFGTableDataLoader.GetColumnID("JoyP3_B1");
		int columnID16 = cFGTableDataLoader.GetColumnID("JoyP3_B2");
		int columnID17 = cFGTableDataLoader.GetColumnID("JoyP4_B1");
		int columnID18 = cFGTableDataLoader.GetColumnID("JoyP4_B2");
		if (columnID11 == -1)
		{
			return false;
		}
		if (columnID12 == -1)
		{
			return false;
		}
		if (columnID13 == -1)
		{
			return false;
		}
		if (columnID14 == -1)
		{
			return false;
		}
		if (columnID15 == -1)
		{
			return false;
		}
		if (columnID16 == -1)
		{
			return false;
		}
		if (columnID17 == -1)
		{
			return false;
		}
		if (columnID18 == -1)
		{
			return false;
		}
		if (columnID == -1)
		{
			return false;
		}
		if (columnID2 == -1)
		{
			return false;
		}
		if (columnID4 == -1)
		{
			return false;
		}
		if (columnID3 == -1)
		{
			return false;
		}
		if (columnID5 == -1)
		{
			return false;
		}
		if (columnID6 == -1)
		{
			return false;
		}
		if (columnID7 == -1)
		{
			return false;
		}
		if (columnID8 == -1)
		{
			return false;
		}
		if (columnID9 == -1)
		{
			return false;
		}
		if (columnID10 == -1)
		{
			return false;
		}
		for (int i = 1; i < cFGTableDataLoader.LineCount; i++)
		{
			if (!cFGTableDataLoader.ParseLine(i, ShowWarnings: true) || cFGTableDataLoader.ParsedLine[0] == string.Empty)
			{
				continue;
			}
			EActionCommand eActionCommand = cFGTableDataLoader.ParseEnum(columnID, EActionCommand.None);
			if (eActionCommand != 0 && !m_Actions.ContainsKey(eActionCommand))
			{
				sAInfo sAInfo = new sAInfo(eActionCommand, cFGTableDataLoader.ParseEnum(columnID2, ECFGKeyCode.None), cFGTableDataLoader.ParseEnum(columnID4, ECFGKeyCode.None), GetMod(cFGTableDataLoader.ParsedLine[columnID3]), GetMod(cFGTableDataLoader.ParsedLine[columnID5]));
				if (sAInfo != null)
				{
					sAInfo.MenuGroup = cFGTableDataLoader.ParseInt(columnID6);
					sAInfo.MenuItem = cFGTableDataLoader.ParseInt(columnID7);
					sAInfo.BindableByPlayer = cFGTableDataLoader.ParseBool(columnID8, DefaultVal: true);
					sAInfo.Activationtype = cFGTableDataLoader.ParseEnum(columnID9, CFGKey.EActivationType.OnRelease);
					sAInfo.HoldDuration = cFGTableDataLoader.ParseFloat(columnID10);
					sAInfo.JB_Profile1_B1 = cFGTableDataLoader.ParseEnum(columnID11, EJoyButton.Unknown);
					sAInfo.JB_Profile1_B2 = cFGTableDataLoader.ParseEnum(columnID12, EJoyButton.Unknown);
					sAInfo.JB_Profile2_B1 = cFGTableDataLoader.ParseEnum(columnID13, EJoyButton.Unknown);
					sAInfo.JB_Profile2_B2 = cFGTableDataLoader.ParseEnum(columnID14, EJoyButton.Unknown);
					sAInfo.JB_Profile3_B1 = cFGTableDataLoader.ParseEnum(columnID15, EJoyButton.Unknown);
					sAInfo.JB_Profile3_B2 = cFGTableDataLoader.ParseEnum(columnID16, EJoyButton.Unknown);
					sAInfo.JB_Profile4_B1 = cFGTableDataLoader.ParseEnum(columnID17, EJoyButton.Unknown);
					sAInfo.JB_Profile4_B2 = cFGTableDataLoader.ParseEnum(columnID18, EJoyButton.Unknown);
					m_Actions.Add(eActionCommand, sAInfo);
				}
			}
		}
		return true;
	}

	private CFGKey.EModFlag GetMod(string str)
	{
		if (str == null)
		{
			return CFGKey.EModFlag.None;
		}
		if (string.Compare("alt", str, ignoreCase: true) == 0)
		{
			return CFGKey.EModFlag.Mod_Alt;
		}
		if (string.Compare("ctrl", str, ignoreCase: true) == 0)
		{
			return CFGKey.EModFlag.Mod_Ctrl;
		}
		return CFGKey.EModFlag.None;
	}
}
