using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CFGTableDataLoader
{
	private Dictionary<string, int> m_CollumnsID = new Dictionary<string, int>();

	private string[] m_Lines;

	private int m_MinColumns = 1;

	private string[] m_ParsedLine;

	private string m_FileName;

	public int LineCount
	{
		get
		{
			if (m_Lines == null)
			{
				return 0;
			}
			return m_Lines.GetLength(0);
		}
	}

	public int RowCount
	{
		get
		{
			if (m_Lines == null)
			{
				return 0;
			}
			return m_Lines.GetLength(0);
		}
	}

	public string[] ParsedLine => m_ParsedLine;

	public string GetSubString(int ColumnID)
	{
		if (m_ParsedLine == null)
		{
			return null;
		}
		if (ColumnID <= m_ParsedLine.GetLength(0))
		{
			return m_ParsedLine[ColumnID];
		}
		return null;
	}

	public bool IsTrue(int ColumnID)
	{
		if (m_ParsedLine == null || ColumnID >= m_ParsedLine.Length)
		{
			return false;
		}
		if (string.Compare(m_ParsedLine[ColumnID], "true", ignoreCase: true) == 0 || string.Compare(m_ParsedLine[ColumnID], "yes", ignoreCase: true) == 0)
		{
			return true;
		}
		return false;
	}

	public int GetColumnID(string ColumnHeaderCaption, bool ShowWarning = false)
	{
		if (m_CollumnsID == null)
		{
			return -1;
		}
		int value = -1;
		if (!m_CollumnsID.TryGetValue(ColumnHeaderCaption.ToUpper(), out value))
		{
			if (ShowWarning)
			{
				Debug.LogWarning("File: [" + m_FileName + "] has missing column " + ColumnHeaderCaption);
			}
			return -1;
		}
		return value;
	}

	public bool ParseLine(int LineNumber, bool ShowWarnings)
	{
		if (m_Lines == null)
		{
			if (ShowWarnings)
			{
				if (m_FileName == null)
				{
					Debug.LogWarning("No Table File is opened");
				}
				else
				{
					Debug.LogWarning("File [" + m_FileName + "] has no valid text lines");
				}
			}
			return false;
		}
		if (LineNumber >= m_Lines.GetLength(0))
		{
			if (ShowWarnings)
			{
				Debug.LogWarning("File [" + m_FileName + "]: line number out of range : " + LineNumber + " ( file has " + m_Lines.GetLength(0) + " lines)");
			}
			return false;
		}
		m_ParsedLine = m_Lines[LineNumber].Split('\t');
		if (m_ParsedLine == null)
		{
			if (ShowWarnings)
			{
				Debug.LogWarning("File [" + m_FileName + " ] line #" + LineNumber + " has failed to generate array of strings");
			}
			return false;
		}
		if (m_ParsedLine.GetLength(0) < m_MinColumns && ShowWarnings)
		{
			Debug.LogWarning("File [" + m_FileName + " ] line #" + LineNumber + " has less than requested columns! (" + m_ParsedLine.GetLength(0) + " instead of " + m_MinColumns + ")");
		}
		if (m_ParsedLine[0] == string.Empty)
		{
			return false;
		}
		return true;
	}

	public bool OpenTableFile(string path, int MinColumnCount = 1, bool ParseColumns = true)
	{
		m_MinColumns = MinColumnCount;
		if (m_MinColumns < 1)
		{
			m_MinColumns = 1;
		}
		m_Lines = null;
		m_CollumnsID.Clear();
		m_FileName = path;
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {path} does not exist or is empty");
			return false;
		}
		m_Lines = text.ToLines();
		if (m_Lines == null)
		{
			Debug.LogError("ERROR! Cannot find database file: " + path);
			return false;
		}
		if (m_Lines.Length < 2)
		{
			Debug.LogError("ERROR! Database file [+" + path + "] is corrupted!");
			return false;
		}
		if (ParseColumns)
		{
			ParseLine(0, ShowWarnings: true);
			if (m_ParsedLine != null)
			{
				for (int i = 0; i < m_ParsedLine.GetLength(0); i++)
				{
					if (m_ParsedLine[i] != null && m_ParsedLine[i] != string.Empty)
					{
						if (m_CollumnsID.ContainsKey(m_ParsedLine[i]))
						{
							Debug.LogWarning("File: [" + path + "] Collumns Array aleady contains item " + m_ParsedLine[i] + ". Please replace header column : " + i);
						}
						else
						{
							m_CollumnsID.Add(m_ParsedLine[i].ToUpper(), i);
						}
					}
				}
			}
		}
		else
		{
			m_CollumnsID.Clear();
		}
		return true;
	}

	public void Close()
	{
		m_FileName = null;
		m_Lines = null;
		m_MinColumns = 1;
		m_ParsedLine = null;
	}

	public float ParseFloat(int ColumnID, float DefaultVal = 0f)
	{
		if (m_ParsedLine == null || m_ParsedLine.GetLength(0) < ColumnID || m_ParsedLine[ColumnID] == null)
		{
			return DefaultVal;
		}
		float result = DefaultVal;
		if (!float.TryParse(m_ParsedLine[ColumnID], out result))
		{
			return DefaultVal;
		}
		return result;
	}

	public float ParseFloat(string ColumnHeader, float DefaultVal = 0f)
	{
		int columnID = GetColumnID(ColumnHeader);
		if (columnID == -1)
		{
			return DefaultVal;
		}
		return ParseFloat(columnID, DefaultVal);
	}

	public bool ParseBool(int ColumnID, bool DefaultVal)
	{
		if (m_ParsedLine == null || m_ParsedLine.GetLength(0) < ColumnID || m_ParsedLine[ColumnID] == null)
		{
			return DefaultVal;
		}
		bool result = DefaultVal;
		if (!bool.TryParse(m_ParsedLine[ColumnID], out result))
		{
			return DefaultVal;
		}
		return result;
	}

	public int ParseInt(int ColumnID, int DefaultVal = 0)
	{
		if (m_ParsedLine == null || m_ParsedLine.GetLength(0) < ColumnID || m_ParsedLine[ColumnID] == null)
		{
			return DefaultVal;
		}
		int result = DefaultVal;
		if (!int.TryParse(m_ParsedLine[ColumnID], out result))
		{
			return DefaultVal;
		}
		return result;
	}

	public int ParseInt(string ColumnHeader, int DefaultVal = 0)
	{
		int columnID = GetColumnID(ColumnHeader);
		if (columnID == -1)
		{
			return DefaultVal;
		}
		return ParseInt(columnID, DefaultVal);
	}

	public int ParseIntOrPercentage(int ColumnID, int BaseValue)
	{
		if (m_ParsedLine == null || m_ParsedLine.GetLength(0) < ColumnID || string.IsNullOrEmpty(m_ParsedLine[ColumnID]))
		{
			return BaseValue;
		}
		int result = BaseValue;
		int num = m_ParsedLine[ColumnID].IndexOf('%');
		if (num > 0)
		{
			string s = m_ParsedLine[ColumnID].Remove(num, 1);
			float result2 = 100f;
			if (!float.TryParse(s, out result2))
			{
				result2 = 100f;
			}
			result2 = result2 * 0.01f * (float)BaseValue;
			return (int)result2;
		}
		if (!int.TryParse(m_ParsedLine[ColumnID], out result))
		{
			return BaseValue;
		}
		return result;
	}

	public T ParseEnum<T>(int ColumnID, T DefaultValue, bool bToUpper = true)
	{
		if (m_ParsedLine == null || m_ParsedLine.GetLength(0) < ColumnID || m_ParsedLine[ColumnID] == null)
		{
			return DefaultValue;
		}
		T val = DefaultValue;
		try
		{
			string text = m_ParsedLine[ColumnID];
			if (bToUpper)
			{
				text = text.ToUpper();
			}
			return (T)Enum.Parse(typeof(T), text);
		}
		catch
		{
			try
			{
				return (T)Enum.Parse(typeof(T), m_ParsedLine[ColumnID]);
			}
			catch
			{
				return DefaultValue;
			}
		}
	}

	public T ParseEnum<T>(string ColumnHeader, T DefaultValue)
	{
		int columnID = GetColumnID(ColumnHeader);
		if (columnID == -1)
		{
			return DefaultValue;
		}
		return ParseEnum(columnID, DefaultValue);
	}

	public static List<T> CreateListFromFile<T>(string FileName) where T : class, new()
	{
		List<T> list = new List<T>();
		if (list == null)
		{
			return null;
		}
		CFGTableDataLoader tdl = new CFGTableDataLoader();
		if (tdl == null)
		{
			return list;
		}
		if (!tdl.OpenTableFile(FileName))
		{
			return list;
		}
		Type typeFromHandle = typeof(T);
		Dictionary<string, CFGTableField> Fields = GetFields(typeFromHandle, ref tdl);
		if (Fields.Count == 0)
		{
			Debug.LogWarning("Could not find fields to import in structure: " + typeFromHandle.Name);
			return null;
		}
		for (int i = 1; i < tdl.LineCount; i++)
		{
			if (tdl.ParseLine(i, ShowWarnings: true))
			{
				T val = FillObject<T>(ref Fields, typeFromHandle, ref tdl);
				if (val == null)
				{
					return list;
				}
				list.Add(val);
			}
		}
		return list;
	}

	public static Dictionary<TKey, TValue> CreateDictionaryFromFile<TKey, TValue>(string FileName, string KeyAsString) where TValue : class, new()
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		if (dictionary == null)
		{
			return null;
		}
		CFGTableDataLoader tdl = new CFGTableDataLoader();
		if (tdl == null)
		{
			return dictionary;
		}
		if (!tdl.OpenTableFile(FileName))
		{
			Debug.LogWarning("Failed to pars file: " + FileName);
			return dictionary;
		}
		Type typeFromHandle = typeof(TValue);
		Dictionary<string, CFGTableField> Fields = GetFields(typeFromHandle, ref tdl);
		if (Fields == null || Fields.Count == 0)
		{
			Debug.LogWarning("Could not find fields to import in structure: " + typeFromHandle.Name);
			return null;
		}
		int columnID = tdl.GetColumnID(KeyAsString);
		if (columnID == -1)
		{
			Debug.LogWarning("Could not find column with key position!");
			return null;
		}
		for (int i = 1; i < tdl.LineCount; i++)
		{
			if (!tdl.ParseLine(i, ShowWarnings: true))
			{
				continue;
			}
			object obj = ParseObject(tdl.GetSubString(columnID), typeof(TKey));
			if (obj == null)
			{
				Debug.Log("Failed to parse dictionary's key of type [" + typeof(TKey).Name + "] from string: '" + tdl.GetSubString(columnID) + "'");
				continue;
			}
			TKey val = (TKey)obj;
			if (dictionary.ContainsKey(val))
			{
				Debug.LogError("Dictionary already contains key: " + val);
				continue;
			}
			TValue val2 = FillObject<TValue>(ref Fields, typeFromHandle, ref tdl);
			if (val2 == null)
			{
				return dictionary;
			}
			dictionary.Add(val, val2);
		}
		return dictionary;
	}

	private static Dictionary<string, CFGTableField> GetFields(Type objtype, ref CFGTableDataLoader tdl)
	{
		FieldInfo[] fields = objtype.GetFields();
		Dictionary<string, CFGTableField> dictionary = new Dictionary<string, CFGTableField>();
		if (fields == null)
		{
			Debug.LogError("Object has no fields defined!");
			return null;
		}
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.GetCustomAttributes(typeof(CFGTableField), inherit: false) is CFGTableField[] array2 && array2.GetLength(0) == 1)
			{
				if (fieldInfo.FieldType.IsGenericType)
				{
					array2[0].ListObject = fieldInfo.FieldType.GetGenericArguments()[0];
				}
				array2[0].ImportType = fieldInfo.FieldType;
				array2[0].ColumnID = tdl.GetColumnID(array2[0].ColumnName, !array2[0].Optional);
				dictionary.Add(fieldInfo.Name, array2[0]);
			}
		}
		return dictionary;
	}

	private static TObject FillObject<TObject>(ref Dictionary<string, CFGTableField> Fields, Type ObjTye, ref CFGTableDataLoader tdl) where TObject : class, new()
	{
		TObject val = new TObject();
		if (val == null)
		{
			Debug.LogWarning("Failed to allocate new object!");
			return (TObject)null;
		}
		object[] array = new object[1];
		foreach (KeyValuePair<string, CFGTableField> Field in Fields)
		{
			if (Field.Value.ColumnID != -1)
			{
				if (Field.Value.ImportType.IsGenericType)
				{
					array[0] = Field.Value.ImportType.InvokeMember(null, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
					ParseList(tdl.GetSubString(Field.Value.ColumnID), Field.Value.ListObject, Field.Value.ImportType, array[0]);
				}
				else
				{
					array[0] = ParseObject(tdl.GetSubString(Field.Value.ColumnID), Field.Value.ImportType, Field.Value.DefaultValue);
				}
				ObjTye.InvokeMember(Field.Key, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField, null, val, array);
			}
		}
		return val;
	}

	private static void ParseList(string Text, Type objectType, Type ListType, object ListObject)
	{
		if (Text == null || Text == string.Empty)
		{
			return;
		}
		string[] array = Text.Split(',');
		if (array != null && array.Length != 0)
		{
			object[] array2 = new object[1];
			string[] array3 = array;
			foreach (string text in array3)
			{
				array2[0] = ParseObject(text, objectType);
				ListType.InvokeMember("Add", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, ListObject, array2);
			}
		}
	}

	private static object ParseObject(string Text, Type Tp, object defval = null)
	{
		if (Tp.IsGenericType)
		{
			Debug.LogError("eeerororororor");
			return null;
		}
		if (Tp == typeof(bool))
		{
			if (string.Compare("TRUE", Text, ignoreCase: true) == 0 || string.Compare("YES", Text, ignoreCase: true) == 0)
			{
				return true;
			}
			return false;
		}
		if (Tp == typeof(string))
		{
			if (Text == null || Text == string.Empty)
			{
				return defval;
			}
			return Text;
		}
		object result = defval;
		try
		{
			if (Tp == typeof(int))
			{
				result = int.Parse(Text);
			}
			else if (Tp == typeof(uint))
			{
				result = uint.Parse(Text);
			}
			else if (Tp == typeof(float))
			{
				result = float.Parse(Text);
			}
			else if (Tp.IsEnum)
			{
				result = Enum.Parse(Tp, Text, ignoreCase: true);
			}
		}
		catch
		{
			if (Tp.IsEnum)
			{
			}
			return defval;
		}
		return result;
	}
}
