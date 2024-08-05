using System.Collections.Generic;
using UnityEngine;

public class CFGValueTable
{
	public Dictionary<string, CFGValue> m_Table = new Dictionary<string, CFGValue>();

	public bool LoadDefinition(string FileName)
	{
		CFGTableDataLoader cFGTableDataLoader = new CFGTableDataLoader();
		if (cFGTableDataLoader == null)
		{
			return false;
		}
		if (!cFGTableDataLoader.OpenTableFile(FileName))
		{
			return false;
		}
		int columnID = cFGTableDataLoader.GetColumnID("Name");
		int columnID2 = cFGTableDataLoader.GetColumnID("Type");
		int columnID3 = cFGTableDataLoader.GetColumnID("DefaultVal");
		int columnID4 = cFGTableDataLoader.GetColumnID("IncrementOnly");
		int columnID5 = cFGTableDataLoader.GetColumnID("MinValue");
		int columnID6 = cFGTableDataLoader.GetColumnID("MaxValue");
		if (columnID == -1)
		{
			return false;
		}
		if (columnID2 == -1)
		{
			return false;
		}
		if (columnID3 == -1)
		{
			return false;
		}
		if (columnID4 == -1)
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
		for (int i = 1; i < cFGTableDataLoader.LineCount; i++)
		{
			if (!cFGTableDataLoader.ParseLine(i, ShowWarnings: true) || cFGTableDataLoader.ParsedLine[0] == string.Empty)
			{
				continue;
			}
			CFGValue.EType eType = cFGTableDataLoader.ParseEnum(columnID2, CFGValue.EType.Unknown);
			if (eType == CFGValue.EType.Unknown || string.IsNullOrEmpty(cFGTableDataLoader.ParsedLine[columnID]))
			{
				Debug.LogWarning("File [" + FileName + "] line: " + i + " type or name is invalid!");
				continue;
			}
			if (m_Table.ContainsKey(cFGTableDataLoader.ParsedLine[columnID]))
			{
				Debug.LogWarning("File [" + FileName + "] line: " + i + " contains variable <" + cFGTableDataLoader.ParsedLine[columnID] + "> which already exists!");
				continue;
			}
			CFGValue cFGValue = null;
			switch (eType)
			{
			case CFGValue.EType.Int:
			{
				CFGValue_Int cFGValue_Int = new CFGValue_Int();
				if (cFGValue_Int != null)
				{
					if (cFGTableDataLoader.ParsedLine[columnID5] != null && cFGTableDataLoader.ParsedLine[columnID5] != string.Empty)
					{
						cFGValue_Int.m_Flags |= CFGValue.EFlags.MinDefined;
						cFGValue_Int.m_Min = cFGTableDataLoader.ParseInt(columnID5);
					}
					if (cFGTableDataLoader.ParsedLine[columnID6] != null && cFGTableDataLoader.ParsedLine[columnID6] != string.Empty)
					{
						cFGValue_Int.m_Flags |= CFGValue.EFlags.MaxDefined;
						cFGValue_Int.m_Max = cFGTableDataLoader.ParseInt(columnID6);
					}
					if (cFGTableDataLoader.IsTrue(columnID4))
					{
						cFGValue_Int.m_Flags |= CFGValue.EFlags.IncrementOnly;
					}
					cFGValue_Int.m_Value = cFGTableDataLoader.ParseInt(columnID3);
				}
				cFGValue = cFGValue_Int;
				break;
			}
			case CFGValue.EType.Float:
			{
				CFGValue_Float cFGValue_Float = new CFGValue_Float();
				if (cFGValue_Float != null)
				{
					if (cFGTableDataLoader.ParsedLine[columnID5] != null && cFGTableDataLoader.ParsedLine[columnID5] != string.Empty)
					{
						cFGValue_Float.m_Flags |= CFGValue.EFlags.MinDefined;
						cFGValue_Float.m_Min = cFGTableDataLoader.ParseFloat(columnID5);
					}
					if (cFGTableDataLoader.ParsedLine[columnID6] != null && cFGTableDataLoader.ParsedLine[columnID6] != string.Empty)
					{
						cFGValue_Float.m_Flags |= CFGValue.EFlags.MaxDefined;
						cFGValue_Float.m_Max = cFGTableDataLoader.ParseFloat(columnID6);
					}
					if (cFGTableDataLoader.IsTrue(columnID4))
					{
						cFGValue_Float.m_Flags |= CFGValue.EFlags.IncrementOnly;
					}
					cFGValue_Float.m_Value = cFGTableDataLoader.ParseFloat(columnID3);
				}
				cFGValue = cFGValue_Float;
				break;
			}
			case CFGValue.EType.String:
			{
				CFGValue_String cFGValue_String = new CFGValue_String();
				if (cFGValue_String != null)
				{
					cFGValue_String.m_Value = cFGTableDataLoader.ParsedLine[columnID3];
					if (cFGValue_String.m_Value == null)
					{
						cFGValue_String.m_Value = string.Empty;
					}
				}
				cFGValue = cFGValue_String;
				break;
			}
			default:
				Debug.LogWarning("File [" + FileName + "] line: " + i + " contains variable <" + cFGTableDataLoader.ParsedLine[columnID] + "> of unimplemented type: " + eType);
				continue;
			}
			if (cFGValue == null)
			{
				Debug.LogWarning("File [" + FileName + "] line: " + i + " syntax / memory allocation error");
			}
			else
			{
				cFGValue.m_Name = cFGTableDataLoader.ParsedLine[columnID];
				m_Table.Add(cFGValue.m_Name, cFGValue);
			}
		}
		Debug.Log("Finished parsing value definitions from file: " + FileName + ", added " + m_Table.Count + " items");
		return true;
	}

	public bool ApplyModified(ref CFGValueTable DestTable)
	{
		if (DestTable == null || DestTable.m_Table == null)
		{
			return CloneTo(ref DestTable);
		}
		foreach (KeyValuePair<string, CFGValue> item in m_Table)
		{
			if (item.Value.IsModified())
			{
				switch (item.Value.m_Type)
				{
				case CFGValue.EType.Int:
					DestTable.Set_Int(item.Key, ((CFGValue_Int)item.Value).m_Value, ModOriginal: true);
					break;
				case CFGValue.EType.Float:
					DestTable.Set_Float(item.Key, ((CFGValue_Float)item.Value).m_Value, ModOriginal: true);
					break;
				case CFGValue.EType.String:
					DestTable.Set_String(item.Key, ((CFGValue_String)item.Value).m_Value, ModOriginal: true);
					break;
				}
			}
		}
		return true;
	}

	public bool CloneTo(ref CFGValueTable DestTable)
	{
		if (DestTable == null)
		{
			DestTable = new CFGValueTable();
		}
		if (DestTable == null)
		{
			return false;
		}
		if (DestTable.m_Table == null)
		{
			DestTable.m_Table = new Dictionary<string, CFGValue>();
			if (DestTable.m_Table == null)
			{
				return false;
			}
		}
		DestTable.m_Table.Clear();
		foreach (KeyValuePair<string, CFGValue> item in m_Table)
		{
			CFGValue cFGValue = null;
			switch (item.Value.m_Type)
			{
			case CFGValue.EType.Int:
			{
				CFGValue_Int cFGValue_Int = (CFGValue_Int)item.Value;
				cFGValue = new CFGValue_Int(cFGValue_Int.m_Value, cFGValue_Int.m_Min, cFGValue_Int.m_Max);
				break;
			}
			case CFGValue.EType.Float:
			{
				CFGValue_Float cFGValue_Float = (CFGValue_Float)item.Value;
				cFGValue = new CFGValue_Float(cFGValue_Float.m_Value, cFGValue_Float.m_Min, cFGValue_Float.m_Max);
				break;
			}
			case CFGValue.EType.String:
			{
				CFGValue_String cFGValue_String = (CFGValue_String)item.Value;
				cFGValue = new CFGValue_String(cFGValue_String.m_Value);
				break;
			}
			}
			if (cFGValue != null)
			{
				cFGValue.m_Flags = item.Value.m_Flags;
				cFGValue.m_Name = item.Value.m_Name;
				DestTable.m_Table.Add(item.Value.m_Name, cFGValue);
			}
		}
		if (m_Table.Count != DestTable.m_Table.Count)
		{
			Debug.LogWarning("CFGValueTable: failed to clone table " + m_Table.Count + " vs " + DestTable.m_Table.Count + " items");
		}
		return true;
	}

	public bool Set_Int(string Name, int NewValue, bool ModOriginal = false)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return false;
		}
		CFGValue_Int cFGValue_Int = (CFGValue_Int)value;
		if (cFGValue_Int != null)
		{
			cFGValue_Int.Set(NewValue);
			if (ModOriginal)
			{
				cFGValue_Int.m_Original = cFGValue_Int.m_Value;
			}
			return true;
		}
		return false;
	}

	public int Get_Int(string Name)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return 0;
		}
		return ((CFGValue_Int)value)?.m_Value ?? 0;
	}

	public bool Set_Float(string Name, float NewValue, bool ModOriginal = false)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return false;
		}
		CFGValue_Float cFGValue_Float = (CFGValue_Float)value;
		if (cFGValue_Float != null)
		{
			cFGValue_Float.Set(NewValue);
			if (ModOriginal)
			{
				cFGValue_Float.m_Original = cFGValue_Float.m_Value;
			}
			return true;
		}
		return false;
	}

	public float Get_Float(string Name)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return 0f;
		}
		return ((CFGValue_Float)value)?.m_Value ?? 0f;
	}

	public bool Set_String(string Name, string NewValue, bool ModOriginal = false)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return false;
		}
		CFGValue_String cFGValue_String = (CFGValue_String)value;
		if (cFGValue_String != null)
		{
			cFGValue_String.Set(NewValue);
			if (ModOriginal)
			{
				cFGValue_String.m_Original = cFGValue_String.m_Value;
			}
			return true;
		}
		return false;
	}

	public string Get_String(string Name)
	{
		CFGValue value = null;
		if (m_Table == null || !m_Table.TryGetValue(Name, out value))
		{
			return string.Empty;
		}
		CFGValue_String cFGValue_String = (CFGValue_String)value;
		if (cFGValue_String != null)
		{
			return cFGValue_String.m_Value;
		}
		return string.Empty;
	}
}
