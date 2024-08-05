#define USE_ERROR_REPORTING
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CFGTextManager : CFGSingletonResourcePrefab<CFGTextManager>
{
	private Dictionary<string, string> m_Texts = new Dictionary<string, string>();

	private Dictionary<string, string> m_Formats = new Dictionary<string, string>();

	private Dictionary<string, string> m_TtStyles = new Dictionary<string, string>();

	public string GetLocalizedText(string text_id, params string[] args)
	{
		if (text_id != null && text_id.Length > 1 && text_id[0] == '^')
		{
			return text_id.Substring(1);
		}
		string value = string.Empty;
		if (!m_Texts.TryGetValue(text_id, out value))
		{
			return "@" + text_id + "@";
		}
		if (value == string.Empty)
		{
			return "#" + text_id + "#";
		}
		for (int i = 0; i < args.Length; i++)
		{
			value = value.Replace("[%" + i + "]", args[i]);
		}
		return value.Replace("<br>", "\n");
	}

	public string GetTextFormat(string text_id)
	{
		string value;
		return (!m_Formats.TryGetValue(text_id, out value)) ? string.Empty : value;
	}

	public override void Init()
	{
		base.Init();
		LoadTextStyles(CFGData.GetDataPathFor("tt_styles.tsv"));
		LoadLanguageFiles();
	}

	public void LoadLanguageFiles()
	{
		if (CFGOptions.Gameplay.Language == ELanguage.Custom)
		{
			if (IsCustomLanguageValid())
			{
				LoadCustomLanguageFiles();
				return;
			}
			CFGOptions.Gameplay.Language = ELanguage.English;
			CFGOptions.Save();
			CFGError.ReportError("Custom language is invalid -> reverting to English", CFGError.ErrorCode.None, CFGError.ErrorType.Info);
			CFGError.Show();
		}
		string languageCode = CFGOptions.Gameplay.Language.GetLanguageCode();
		m_Texts.Clear();
		string[] array = CFGData.ReadAllTextsFromDirectory(CFGData.GetDataPathFor("Texts"), "texts_*.tsv");
		foreach (string text in array)
		{
			LoadTexts(text, languageCode);
		}
		if (CFGGame.DlcType != 0)
		{
			string[] array2 = CFGData.ReadAllTextsFromDirectory(CFGData.GetDataPathFor("Texts", CFGGame.DlcType), "texts_*.tsv");
			foreach (string text2 in array2)
			{
				LoadTexts(text2, languageCode);
			}
		}
	}

	public bool IsCustomLanguageValid()
	{
		if (string.IsNullOrEmpty(CFGOptions.Gameplay.CustomLanguage))
		{
			Debug.LogError("Custom language not set");
			return false;
		}
		string dataPathFor = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage));
		string dataPathFor2 = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage), CFGGame.DlcType);
		if (!Directory.Exists(dataPathFor) || (CFGGame.DlcType != 0 && !Directory.Exists(dataPathFor2)))
		{
			Debug.LogError("Custom language not found");
			return false;
		}
		return true;
	}

	public void LoadCustomLanguageFiles()
	{
		m_Texts.Clear();
		string dataPathFor = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage));
		string dataPathFor2 = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage), CFGGame.DlcType);
		string[] array = CFGData.ReadAllTextsFromDirectory(dataPathFor, "texts_*.tsv");
		foreach (string texts in array)
		{
			LoadCustomTexts(texts);
		}
		if (CFGGame.DlcType != 0)
		{
			string[] array2 = CFGData.ReadAllTextsFromDirectory(dataPathFor2, "texts_*.tsv");
			foreach (string texts2 in array2)
			{
				LoadCustomTexts(texts2);
			}
		}
	}

	private void LoadCustomTexts(string texts)
	{
		if (string.IsNullOrEmpty(texts))
		{
			return;
		}
		string[] array = texts.ToLines();
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split('\t');
			if (array3.Length < 2)
			{
				continue;
			}
			string text2 = array3[0].Trim();
			if (!string.IsNullOrEmpty(text2) && !text2.Equals("text_id", StringComparison.OrdinalIgnoreCase))
			{
				if (array3.Length >= 3)
				{
					m_Formats[text2] = array3[2].Trim();
				}
				if (!m_Texts.ContainsKey(text2))
				{
					m_Texts.Add(text2, ProcessStyles(array3[1]));
				}
				else
				{
					Debug.LogWarning("WARNING! CFGTextManager::LoadTexts() - duplicated text id (" + text2 + ")");
				}
			}
		}
	}

	public bool HasText(string Key)
	{
		if (m_Texts == null)
		{
			return false;
		}
		return m_Texts.ContainsKey(Key);
	}

	private bool LoadTexts(string text, string lang_id)
	{
		string[] array = text.ToLines();
		if (array.Length == 0)
		{
			return false;
		}
		int num = 0;
		int num2 = 1;
		int text_col = -1;
		if (GetTextCol(array[0], lang_id, out text_col) == -1)
		{
			return false;
		}
		int num3 = array[0].Split('\t').Length;
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			string text2 = array2[num].Trim();
			if (text2 == string.Empty)
			{
				continue;
			}
			if (array2.Length != num3)
			{
				Debug.LogWarning("WARNING! CFGTextManager::LoadTexts() - row " + i + " (id - " + text2 + ") has wrong columns count. Often reason: line break in localized text.");
			}
			else
			{
				m_Formats[text2] = array2[num2].Trim();
				if (!m_Texts.ContainsKey(text2))
				{
					m_Texts.Add(text2, ProcessStyles(array2[text_col]));
				}
				else
				{
					Debug.LogWarning("WARNING! CFGTextManager::LoadTexts() - duplicated text id (" + text2 + ")");
				}
			}
		}
		return true;
	}

	private bool LoadTextStyles(string path)
	{
		m_TtStyles.Clear();
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {path} does not exist or is empty");
			return false;
		}
		string[] array = text.ToLines();
		if (array.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			string text2 = array2[0];
			if (!(text2 == string.Empty))
			{
				if (!m_TtStyles.ContainsKey(text2))
				{
					m_TtStyles.Add(text2, array2[1]);
				}
				else
				{
					Debug.LogError("ERROR! CFGTextManager::LoadTextStyles() - duplicated text style id (" + text2 + ") in " + path);
				}
			}
		}
		return true;
	}

	private int GetTextCol(string row, string lang_id, out int text_col)
	{
		text_col = -1;
		string[] array = row.Split('\t');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == lang_id)
			{
				text_col = i;
			}
		}
		return text_col;
	}

	private string ProcessStyles(string source)
	{
		foreach (KeyValuePair<string, string> ttStyle in m_TtStyles)
		{
			source = source.Replace(ttStyle.Key, ttStyle.Value);
		}
		return source;
	}
}
