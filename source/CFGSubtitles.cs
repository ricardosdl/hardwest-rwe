using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class CFGSubtitles
{
	private class SMS_Line
	{
		public float StartTime;

		public float Duration;

		public string Text = string.Empty;
	}

	private bool m_bIsPlaying;

	private bool m_bHidden;

	private bool m_bUseScaleform;

	private int m_CurrentSLine = -1;

	private float m_MovieStartTime;

	private float m_HideCurrentLineTime = -1f;

	private List<SMS_Line> m_Lines = new List<SMS_Line>();

	public bool Init(string ColumnID, string FileName, bool UseScaleform)
	{
		m_bUseScaleform = UseScaleform;
		if (m_bUseScaleform)
		{
			CFGSingleton<CFGWindowMgr>.Instance.LoadSubtitles();
		}
		LoadSubtitles(FileName, ColumnID);
		return true;
	}

	public void Begin()
	{
		m_CurrentSLine = -2;
		if (m_Lines != null && m_Lines.Count > 0)
		{
			m_CurrentSLine = -1;
			m_MovieStartTime = Time.time;
		}
		m_bIsPlaying = true;
	}

	public void Update()
	{
		if (m_Lines == null || m_Lines.Count == 0 || m_CurrentSLine == -2 || !m_bIsPlaying || !CFGOptions.Gameplay.DisplaySubtitles || m_Lines.Count <= m_CurrentSLine + 1)
		{
			return;
		}
		if (m_MovieStartTime + m_Lines[m_CurrentSLine + 1].StartTime < Time.time)
		{
			m_CurrentSLine++;
			Set_HideTime();
			ShowLine(m_CurrentSLine);
		}
		if (Time.time > m_HideCurrentLineTime && !m_bHidden)
		{
			if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_Subtitles)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_Subtitles.SetText(string.Empty);
			}
			m_bHidden = true;
		}
	}

	public void Finish(bool bUnloadScaleform = true)
	{
		if (m_bUseScaleform && bUnloadScaleform)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadSubtitles();
		}
		m_bIsPlaying = false;
	}

	private void ShowLine(int LineID)
	{
		Debug.Log("[" + Time.time + "] " + m_Lines[LineID].StartTime.ToString() + " " + m_Lines[LineID].Text + " Duration = " + m_Lines[LineID].Duration + " Hide Time: " + m_HideCurrentLineTime);
		if (m_bUseScaleform)
		{
			if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_Subtitles)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_Subtitles.SetText(m_Lines[LineID].Text);
			}
			m_bHidden = false;
		}
	}

	private void Set_HideTime()
	{
		if (m_CurrentSLine < 0 || m_Lines == null || m_CurrentSLine >= m_Lines.Count)
		{
			m_HideCurrentLineTime = m_MovieStartTime;
			return;
		}
		if (m_Lines[m_CurrentSLine].Duration > 0f)
		{
			m_HideCurrentLineTime = m_MovieStartTime + m_Lines[m_CurrentSLine].Duration + m_Lines[m_CurrentSLine].StartTime;
			return;
		}
		if (m_Lines.Count > m_CurrentSLine + 1)
		{
			m_HideCurrentLineTime = m_MovieStartTime + m_Lines[m_CurrentSLine + 1].StartTime;
			return;
		}
		float num = (float)m_Lines[m_CurrentSLine].Text.Length * 0.07f;
		m_HideCurrentLineTime = m_MovieStartTime + m_Lines[m_CurrentSLine].StartTime + num;
	}

	private void LoadSubtitles(string path, string movie_id)
	{
		if (CFGOptions.Gameplay.Language == ELanguage.Custom)
		{
			LoadCustomSubtitles(movie_id);
			return;
		}
		m_Lines.Clear();
		if (!CFGOptions.Gameplay.DisplaySubtitles)
		{
			Debug.Log("Subtitles are disabled in player options. Skipping loading...");
			return;
		}
		Debug.Log("Loading subtitles for movie: " + movie_id + " lang: " + CFGOptions.Gameplay.Language.GetLanguageCode());
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {path} does not exist or is empty");
			return;
		}
		string[] array = text.ToLines();
		int num = 0;
		int text_col = -1;
		string languageCode = CFGOptions.Gameplay.Language.GetLanguageCode();
		if (GetTextCol(array[0], languageCode, out text_col) == -1)
		{
			Debug.LogError("ERROR! CFGStoryMovie::LoadSubtitles() - wrong header in texts file (" + path + ")");
			return;
		}
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			string text2 = array2[num];
			if (!(text2 == string.Empty) && !(text2 != movie_id))
			{
				SMS_Line sMS_Line = new SMS_Line();
				sMS_Line.StartTime = ParseTime(array2[1]);
				sMS_Line.Duration = ParseTime(array2[2]);
				if (sMS_Line.Duration > 0f)
				{
					sMS_Line.Duration -= sMS_Line.StartTime;
				}
				sMS_Line.Text = array2[text_col];
				m_Lines.Add(sMS_Line);
			}
		}
		Debug.Log("Loaded " + m_Lines.Count + " lines of text");
	}

	private void LoadCustomSubtitles(string movie_id)
	{
		m_Lines.Clear();
		if (!CFGOptions.Gameplay.DisplaySubtitles)
		{
			Debug.Log("Subtitles are disabled in player options. Skipping loading...");
			return;
		}
		Debug.Log("Loading subtitles for movie: " + movie_id + " lang: " + CFGOptions.Gameplay.CustomLanguage);
		string dataPathFor = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage), CFGGame.DlcType);
		if (!Directory.Exists(dataPathFor))
		{
			Debug.LogError("CFGSubtitles - Custom language not found");
			return;
		}
		dataPathFor = Path.Combine(dataPathFor, "sm_subtitles.tsv");
		string text = CFGData.ReadAllText(dataPathFor);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {dataPathFor} does not exist or is empty");
			return;
		}
		string[] array = text.ToLines();
		int num = 0;
		int num2 = 1;
		int num3 = 2;
		int num4 = 3;
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			string text2 = array2[num];
			if (!string.IsNullOrEmpty(text2) && !(text2 != movie_id))
			{
				SMS_Line sMS_Line = new SMS_Line();
				sMS_Line.StartTime = ParseTime(array2[num2]);
				sMS_Line.Duration = ParseTime(array2[num3]);
				if (sMS_Line.Duration > 0f)
				{
					sMS_Line.Duration -= sMS_Line.StartTime;
				}
				sMS_Line.Text = array2[num4];
				m_Lines.Add(sMS_Line);
			}
		}
		Debug.Log("Loaded " + m_Lines.Count + " lines of text");
	}

	private float ParseTime(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0f;
		}
		float result = 0f;
		if (text.Contains(":") || text.Contains(","))
		{
			string[] array = text.Split(',', ':');
			switch (array.Length)
			{
			case 1:
				float.TryParse(array[0], out result);
				break;
			case 2:
			case 3:
			{
				float result2 = 0f;
				float result3 = 0f;
				float result4 = 0f;
				float.TryParse(array[0], out result3);
				float.TryParse(array[1], out result2);
				if (array.Length > 2)
				{
					float.TryParse(array[2], out result4);
				}
				result = result3 * 60f + result2 + result4 * 0.001f;
				break;
			}
			default:
				float.TryParse(text, out result);
				break;
			}
			return result;
		}
		float.TryParse(text, out result);
		return result;
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
}
