#define USE_ERROR_REPORTING
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGDialogSystem : CFGSingletonResourcePrefab<CFGDialogSystem>
{
	private bool m_bDialogPaused;

	private AudioSource m_DialogsAudioSource;

	private Dictionary<string, CFGDialogSpeaker> m_Speakers = new Dictionary<string, CFGDialogSpeaker>();

	private Dictionary<string, CFGDialog> m_Dialogs = new Dictionary<string, CFGDialog>();

	private Queue<CFGDialog> m_DialogsQueue = new Queue<CFGDialog>();

	private CFGDialog m_CurrentDialog;

	private int m_CurrentLineIdx = -1;

	private float m_PauseDelta;

	private AudioClip m_NoSoundClip;

	public bool PlayDialog(string dialog_id)
	{
		return PlayDialog(dialog_id, null);
	}

	public void PauseDialog(bool bPause)
	{
		if (!(m_DialogsAudioSource == null) && m_CurrentDialog != null)
		{
			m_bDialogPaused = bPause;
			if (bPause)
			{
				Debug.Log("Dialog: Pause");
				m_DialogsAudioSource.Pause();
			}
			else if (m_PauseDelta == 0f)
			{
				Debug.Log("Dialog: Play");
				m_DialogsAudioSource.Play();
			}
		}
	}

	public bool PlayDialog(string dialog_id, CFGDialog.OnDialogEndDelegate callback)
	{
		CFGDialog dialog = GetDialog(dialog_id);
		if (dialog == null || dialog.m_Lines == null)
		{
			return false;
		}
		dialog.m_OnDialogEndCallback = (CFGDialog.OnDialogEndDelegate)Delegate.Combine(dialog.m_OnDialogEndCallback, callback);
		if (m_CurrentDialog == null)
		{
			PlayDialog(dialog);
		}
		else
		{
			QueueDialog(dialog);
		}
		return true;
	}

	public bool StopCurrentDialog(bool UseCallback)
	{
		if (m_CurrentDialog != null)
		{
			if (m_CurrentLineIdx >= 0)
			{
				CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
				if (instance != null)
				{
					instance.UnloadDialogPanel();
				}
			}
			if (m_CurrentDialog.m_OnDialogEndCallback != null)
			{
				if (UseCallback)
				{
					m_CurrentDialog.m_OnDialogEndCallback();
				}
				m_CurrentDialog.m_OnDialogEndCallback = null;
			}
			m_CurrentDialog = null;
			m_CurrentLineIdx = -1;
			m_PauseDelta = 0f;
			m_DialogsAudioSource.Stop();
			m_bDialogPaused = false;
			return true;
		}
		return false;
	}

	public bool IsPlayingDialog()
	{
		return m_CurrentDialog != null;
	}

	public bool AreDialogsInQueue()
	{
		return m_DialogsQueue.Count > 0;
	}

	public void PlayAlert(string alert_id)
	{
		string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(alert_id);
		ShowAlert(localizedText);
	}

	public void PlayAlert(string alert_id, string param0)
	{
		string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(alert_id, param0);
		ShowAlert(localizedText);
	}

	public void PlayAlert(string alert_id, string param0, string param1)
	{
		string localizedText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(alert_id, param0, param1);
		ShowAlert(localizedText);
	}

	public override void Init()
	{
		base.Init();
		m_DialogsAudioSource = base.gameObject.AddComponent<AudioSource>();
		m_DialogsAudioSource.priority = 0;
		m_DialogsAudioSource.outputAudioMixerGroup = CFGAudioManager.Instance.m_MixDialogs;
		m_NoSoundClip = GetAudioClip("nosound");
		ClearData();
		LoadSpeakers();
		LoadLanguageFiles();
	}

	public void LoadSpeakers()
	{
		m_Speakers.Clear();
		LoadSpeakers(CFGData.GetDataPathFor("speakers.tsv", CFGGame.DlcType));
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
		m_Dialogs.Clear();
		string languageCode = CFGOptions.Gameplay.Language.GetLanguageCode();
		LoadDialogs(CFGData.GetDataPathFor("dialogs.tsv", CFGGame.DlcType), languageCode);
	}

	public bool IsCustomLanguageValid()
	{
		if (string.IsNullOrEmpty(CFGOptions.Gameplay.CustomLanguage))
		{
			Debug.LogError("Custom language not set");
			return false;
		}
		string dataPathFor = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage), CFGGame.DlcType);
		if (!Directory.Exists(dataPathFor))
		{
			if (CFGGame.DlcType != 0)
			{
				Debug.LogError("Custom language for DLC not found");
			}
			else
			{
				Debug.LogError("Custom language not found");
			}
			return false;
		}
		return true;
	}

	public void LoadCustomLanguageFiles()
	{
		m_Dialogs.Clear();
		string dataPathFor = CFGData.GetDataPathFor(Path.Combine("Texts", CFGOptions.Gameplay.CustomLanguage), CFGGame.DlcType);
		LoadCustomDialogs(Path.Combine(dataPathFor, "dialogs.tsv"));
	}

	private bool LoadCustomDialogs(string path)
	{
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {path} does not exist or is empty");
			return false;
		}
		string[] array = text.ToLines();
		if (array.Length == 0)
		{
			Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - dialogs file (" + path + ") is empty");
			return false;
		}
		string text2 = string.Empty;
		List<CFGDialogLine> list = new List<CFGDialogLine>();
		CFGDialog cFGDialog = null;
		string empty = string.Empty;
		int num = array[0].Split('\t').Count();
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			string[] array3 = text3.Split('\t');
			string text4 = array3[0].Trim();
			if (string.IsNullOrEmpty(text4) || text4.Equals("dialog_id", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			if (text4 != text2)
			{
				if (list.Count > 0)
				{
					if (cFGDialog == null)
					{
						Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - something is terribly wrong in " + path);
						continue;
					}
					cFGDialog.m_Lines = new CFGDialogLine[list.Count];
					for (int j = 0; j < list.Count; j++)
					{
						cFGDialog.m_Lines[j] = list[j];
					}
					list.Clear();
				}
				if (!m_Dialogs.ContainsKey(text4))
				{
					CFGDialog cFGDialog2 = new CFGDialog();
					m_Dialogs.Add(text4, cFGDialog2);
					cFGDialog = cFGDialog2;
					text2 = text4;
				}
				else
				{
					Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - duplicated dialog id (" + text4 + ") in " + path);
				}
			}
			CFGDialogLine cFGDialogLine = new CFGDialogLine();
			cFGDialogLine.m_Speaker = GetSpeaker(array3[5].Trim());
			cFGDialogLine.m_AudioClip = GetAudioClip(empty + array3[2].Trim());
			if (cFGDialogLine.m_AudioClip == null)
			{
				cFGDialogLine.m_AudioClip = m_NoSoundClip;
				if (float.TryParse(array3[3], out var result))
				{
					cFGDialogLine.m_Duration = result;
				}
			}
			cFGDialogLine.m_Text = ((!string.IsNullOrEmpty(array3[1])) ? array3[1] : "!!! NO TEXT IN DIALOGS DATA !!!");
			if (!string.IsNullOrEmpty(array3[4]))
			{
				if (float.TryParse(array3[4], out var result2))
				{
					cFGDialogLine.m_PauseTime = result2;
				}
				else
				{
					cFGDialogLine.m_PauseTime = 0f;
					Debug.LogWarning("WARNING! CFGDialogSystem::LoadDialogs() - cannot parse pause time (" + array3[4] + ") in " + path);
				}
			}
			list.Add(cFGDialogLine);
		}
		if (list.Count > 0 && cFGDialog != null)
		{
			cFGDialog.m_Lines = new CFGDialogLine[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				cFGDialog.m_Lines[k] = list[k];
			}
			list.Clear();
		}
		return true;
	}

	private void ClearData()
	{
		m_Speakers.Clear();
		m_Dialogs.Clear();
	}

	private void Update()
	{
		if (m_CurrentDialog != null)
		{
			if (CFGInput.IsActivated(EActionCommand.Skip_Dialog))
			{
				SkipCurrentDialog();
			}
			else if (CFGInput.IsActivated(EActionCommand.Skip_DialogLine))
			{
				SkipCurrentLine();
			}
		}
		if (m_CurrentDialog != null && !m_bDialogPaused)
		{
			m_DialogsAudioSource.volume = 1f;
			if (CFGOptions.Game.SkipDialogs && m_DialogsAudioSource.isPlaying)
			{
				m_PauseDelta = m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_Duration + m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_PauseTime;
				m_DialogsAudioSource.Stop();
			}
			if (m_DialogsAudioSource.isPlaying)
			{
				return;
			}
			if (m_CurrentLineIdx >= 0 && m_PauseDelta < m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_Duration + m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_PauseTime)
			{
				m_PauseDelta += Time.deltaTime;
				return;
			}
			m_PauseDelta = 0f;
			m_CurrentLineIdx++;
			if (m_CurrentLineIdx < m_CurrentDialog.m_Lines.Length)
			{
				CFGDialogLine dialog_line = m_CurrentDialog.m_Lines[m_CurrentLineIdx];
				PlayLine(dialog_line);
				return;
			}
			CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
			if (instance != null)
			{
				instance.UnloadDialogPanel();
			}
			if (m_CurrentDialog.m_OnDialogEndCallback != null)
			{
				m_CurrentDialog.m_OnDialogEndCallback();
				m_CurrentDialog.m_OnDialogEndCallback = null;
			}
			m_CurrentDialog = null;
			m_CurrentLineIdx = -1;
		}
		else if (!m_bDialogPaused && m_DialogsQueue.Count > 0)
		{
			m_CurrentDialog = m_DialogsQueue.Dequeue();
		}
	}

	private bool LoadSpeakers(string path)
	{
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
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			string[] array3 = text2.Split('\t');
			string text3 = array3[0];
			if (text3 == string.Empty || text3 == "speaker_id")
			{
				continue;
			}
			if (!m_Speakers.ContainsKey(text3))
			{
				CFGDialogSpeaker cFGDialogSpeaker = new CFGDialogSpeaker();
				cFGDialogSpeaker.m_Name = array3[2];
				if (!int.TryParse(array3[1], out cFGDialogSpeaker.m_ScaleformId))
				{
					Debug.LogWarning("WARNING! CFGDialogSystem::LoadSpeakers() - error occured while parsing picture id for " + text3);
				}
				m_Speakers.Add(text3, cFGDialogSpeaker);
			}
			else
			{
				Debug.LogError("ERROR! CFGDialogSystem::LoadSpeakers() - duplicated speaker id (" + text3 + ") in " + path);
			}
		}
		return true;
	}

	private bool ParseDialogsFileHeader(string row, string lang_id, out int dialog_id_col, out int speaker_id_col, out int text_col, out int voice_id_col, out int duration_col, out int post_pause_col)
	{
		string[] array = row.Split('\t');
		dialog_id_col = -1;
		speaker_id_col = -1;
		text_col = -1;
		voice_id_col = -1;
		duration_col = -1;
		post_pause_col = -1;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			switch (text)
			{
			case "dialog_id":
				dialog_id_col = i;
				continue;
			case "speaker_id":
				speaker_id_col = i;
				continue;
			case "filename":
				voice_id_col = i;
				continue;
			case "duration":
				duration_col = i;
				continue;
			case "postpause":
				post_pause_col = i;
				continue;
			}
			if (text == lang_id)
			{
				text_col = i;
			}
		}
		return dialog_id_col != -1 && speaker_id_col != -1 && text_col != -1 && voice_id_col != -1 && duration_col != -1 && post_pause_col != -1;
	}

	private bool LoadDialogs(string path, string lang_id)
	{
		string text = CFGData.ReadAllText(path);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {path} does not exist or is empty");
			return false;
		}
		string[] array = text.ToLines();
		if (array.Length == 0)
		{
			Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - dialogs file (" + path + ") is empty");
			return false;
		}
		if (!ParseDialogsFileHeader(array[0], lang_id, out var dialog_id_col, out var speaker_id_col, out var text_col, out var voice_id_col, out var duration_col, out var post_pause_col))
		{
			Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - wrong header in dialogs file (" + path + ")");
			return false;
		}
		string text2 = string.Empty;
		string empty = string.Empty;
		CFGDialog cFGDialog = null;
		ArrayList arrayList = new ArrayList();
		int num = array[0].Split('\t').Count();
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (array2.Count() != num)
			{
				Debug.LogWarning("WARNING! CFGDialogSystem::LoadDialogs() - row " + i + " has wrong columns count. Often reason: line break in localized text.");
				continue;
			}
			string text3 = array2[dialog_id_col].Trim();
			if (text3 == string.Empty)
			{
				continue;
			}
			if (text3 != text2)
			{
				if (arrayList.Count > 0)
				{
					if (cFGDialog == null)
					{
						Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - something is terribly wrong in " + path);
						continue;
					}
					cFGDialog.m_Lines = new CFGDialogLine[arrayList.Count];
					for (int j = 0; j < arrayList.Count; j++)
					{
						cFGDialog.m_Lines[j] = arrayList[j] as CFGDialogLine;
					}
					arrayList.Clear();
				}
				if (!m_Dialogs.ContainsKey(text3))
				{
					CFGDialog cFGDialog2 = new CFGDialog();
					m_Dialogs.Add(text3, cFGDialog2);
					cFGDialog = cFGDialog2;
					text2 = text3;
				}
				else
				{
					Debug.LogError("ERROR! CFGDialogSystem::LoadDialogs() - duplicated dialog id (" + text3 + ") in " + path);
				}
			}
			CFGDialogLine cFGDialogLine = new CFGDialogLine();
			cFGDialogLine.m_Speaker = GetSpeaker(array2[speaker_id_col].Trim());
			cFGDialogLine.m_AudioClip = GetAudioClip(empty + array2[voice_id_col].Trim());
			if (cFGDialogLine.m_AudioClip == null)
			{
				cFGDialogLine.m_AudioClip = m_NoSoundClip;
				if (float.TryParse(array2[duration_col], out var result))
				{
					cFGDialogLine.m_Duration = result;
				}
			}
			cFGDialogLine.m_Text = ((!(array2[text_col] != string.Empty)) ? "!!! NO TEXT IN DIALOGS DATA !!!" : array2[text_col]);
			if (array2[post_pause_col] != string.Empty)
			{
				if (float.TryParse(array2[post_pause_col], out var result2))
				{
					cFGDialogLine.m_PauseTime = result2;
				}
				else
				{
					cFGDialogLine.m_PauseTime = 0f;
					Debug.LogWarning("WARNING! CFGDialogSystem::LoadDialogs() - cannot parse pause time (" + array2[post_pause_col] + ") in " + path);
				}
			}
			arrayList.Add(cFGDialogLine);
		}
		if (arrayList.Count > 0 && cFGDialog != null)
		{
			cFGDialog.m_Lines = new CFGDialogLine[arrayList.Count];
			for (int k = 0; k < arrayList.Count; k++)
			{
				cFGDialog.m_Lines[k] = arrayList[k] as CFGDialogLine;
			}
			arrayList.Clear();
		}
		return true;
	}

	private CFGDialogSpeaker GetSpeaker(string speaker_id)
	{
		CFGDialogSpeaker value = null;
		m_Speakers.TryGetValue(speaker_id, out value);
		if (value == null)
		{
			Debug.LogError("ERROR! CFGDialogSystem::GetSpeaker() - no speaker with id = " + speaker_id);
			if (m_Speakers.Count > 0)
			{
				return m_Speakers.ElementAt(0).Value;
			}
		}
		return value;
	}

	private AudioClip GetAudioClip(string voice_id)
	{
		return Resources.Load("Voice/" + voice_id) as AudioClip;
	}

	private CFGDialog GetDialog(string dialog_id)
	{
		CFGDialog value = null;
		m_Dialogs.TryGetValue(dialog_id, out value);
		return value;
	}

	private void PlayDialog(CFGDialog dialog)
	{
		m_CurrentDialog = dialog;
	}

	private void QueueDialog(CFGDialog dialog)
	{
		m_DialogsQueue.Enqueue(dialog);
	}

	private void PlayLine(CFGDialogLine dialog_line)
	{
		m_DialogsAudioSource.clip = dialog_line.m_AudioClip;
		m_DialogsAudioSource.Play();
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance != null)
		{
			if (instance.m_DialogPanel == null)
			{
				instance.LoadDialogPanel();
			}
			instance.m_DialogPanel.SetCurrentDialog(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(dialog_line.m_Speaker.m_Name), dialog_line.m_Text, dialog_line.m_Speaker.m_ScaleformId);
		}
	}

	private void ShowAlert(string alert_text)
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if ((bool)instance && (bool)instance.m_SetupStagePanel)
		{
			instance.m_SetupStagePanel.ShowAlert(alert_text);
		}
	}

	public void SkipCurrentLine()
	{
		Debug.Log("Skip one dialog line");
		if (m_CurrentDialog == null || m_DialogsAudioSource == null || m_CurrentLineIdx < 0 || m_CurrentLineIdx >= m_CurrentDialog.m_Lines.Length)
		{
			Debug.Log("FAILED to skip dialog line");
			return;
		}
		m_PauseDelta = m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_Duration + m_CurrentDialog.m_Lines[m_CurrentLineIdx].m_PauseTime;
		m_DialogsAudioSource.Stop();
	}

	public void SkipCurrentDialog()
	{
		Debug.Log("Skip dialog");
		StopCurrentDialog(UseCallback: true);
	}
}
