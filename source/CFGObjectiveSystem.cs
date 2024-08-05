#define USE_ERROR_REPORTING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGObjectiveSystem : CFGSingletonResourcePrefab<CFGObjectiveSystem>
{
	[SerializeField]
	private float m_LocScaleY = 0.06f;

	[SerializeField]
	private float m_LocHeightOffset;

	[SerializeField]
	private List<Material> m_LocMats;

	[SerializeField]
	private List<Material> m_LocOptMats;

	[SerializeField]
	private AudioClip m_SoundOnNewObj;

	[SerializeField]
	private AudioClip m_SoundOnObjComplete;

	[SerializeField]
	private AudioClip m_SoundOnObjFail;

	[SerializeField]
	private AudioClip m_SoundOnNewHint;

	[SerializeField]
	private AudioClip m_SoundOnMissionComplete;

	[SerializeField]
	private AudioClip m_SoundOnMissionFail;

	private float m_LastNewObjSoundTime;

	private Dictionary<string, CFGObjective> m_DisabledObjectives = new Dictionary<string, CFGObjective>();

	private Dictionary<string, CFGObjectiveData> m_Objectives = new Dictionary<string, CFGObjectiveData>();

	private Dictionary<string, CFGObjective> m_CurrentObjectives = new Dictionary<string, CFGObjective>();

	public void Select(string objective_id)
	{
		if (m_CurrentObjectives.TryGetValue(objective_id, out var value))
		{
			if (!value.IsSelected)
			{
				foreach (CFGObjective value2 in m_CurrentObjectives.Values)
				{
					bool flag = value2.Id == objective_id;
					if (flag && value2.IsSelected)
					{
						break;
					}
					value2.IsSelected = flag;
				}
				OnObjectivesChanged();
				return;
			}
			foreach (CFGObjective value3 in m_CurrentObjectives.Values)
			{
				value3.IsSelected = false;
			}
			OnObjectivesChanged();
		}
		else
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::Select() - there is no objective with Id: " + objective_id);
		}
	}

	public void OnNewMission()
	{
		if (m_CurrentObjectives == null || m_CurrentObjectives.Count == 0)
		{
			return;
		}
		foreach (CFGObjective value in m_CurrentObjectives.Values)
		{
			value.IsNew = false;
		}
	}

	public void RemoveNewObjectives()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CFGObjective> currentObjective in m_CurrentObjectives)
		{
			if (currentObjective.Value.IsNew)
			{
				if (currentObjective.Value.DesignatedSceneType != 0)
				{
					currentObjective.Value.Complete();
					HideObjectiveLocations(currentObjective.Value);
					list.Add(currentObjective.Key);
				}
				else
				{
					currentObjective.Value.IsNew = false;
				}
			}
		}
		foreach (string item in list)
		{
			m_CurrentObjectives.Remove(item);
		}
		bool flag = false;
		if (list.Count > 0)
		{
			flag = true;
		}
		else
		{
			foreach (CFGObjective value in m_CurrentObjectives.Values)
			{
				if (value.IsDirty)
				{
					flag = true;
					value.IsDirty = false;
				}
			}
		}
		if (flag)
		{
			OnObjectivesChanged();
		}
	}

	public bool ObjectiveStart(string objective_id, bool bSilent = false)
	{
		if (m_CurrentObjectives.ContainsKey(objective_id))
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::ObjectiveStart() - objective " + objective_id + " is already on list");
			return false;
		}
		if (!m_Objectives.ContainsKey(objective_id))
		{
			Debug.LogWarning("Warning! CFGObjectiveSystem::ObjectiveStart() - objective " + objective_id + " is not loaded. Creating new default objective...");
			m_Objectives.Add(objective_id, new CFGObjectiveData(objective_id, EObjectiveImportance.Main, EObjectiveType.Generic, EObjectiveScene.Both));
		}
		CFGObjective cFGObjective = new CFGObjective(m_Objectives[objective_id]);
		cFGObjective.Start();
		m_CurrentObjectives.Add(objective_id, cFGObjective);
		if (!bSilent)
		{
			ShowPopup("<b>New Objective:</b>", cFGObjective.GetText(), (int)(cFGObjective.Type + 2), 0);
			if (Time.time - m_LastNewObjSoundTime > 0.2f)
			{
				PlaySound(m_SoundOnNewObj);
				m_LastNewObjSoundTime = Time.time;
			}
			OnObjectivesChanged();
		}
		return true;
	}

	public bool ObjectiveComplete(string objective_id, bool bSilent = false)
	{
		CFGObjective value = null;
		if (!m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (!bSilent)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::ObjectiveComplete() - objective " + objective_id + " is not started");
				return false;
			}
			ObjectiveStart(objective_id, bSilent: true);
			if (!m_CurrentObjectives.TryGetValue(objective_id, out value))
			{
				return false;
			}
		}
		if (!bSilent)
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::ObjectiveComplete() - objective " + objective_id + " is already ended");
				return false;
			}
			ShowPopup("<b>Objective Completed:</b>", value.GetText(), (int)(value.Type + 2), 1);
			PlaySound(m_SoundOnObjComplete);
		}
		value.Complete();
		HideObjectiveLocations(value);
		if (bSilent)
		{
			m_CurrentObjectives.Remove(objective_id);
		}
		OnObjectivesChanged();
		return true;
	}

	public void CompleteAllObjectives()
	{
		foreach (CFGObjective value in m_CurrentObjectives.Values)
		{
			value.Complete();
			HideObjectiveLocations(value);
		}
	}

	public bool ObjectiveFail(string objective_id)
	{
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::ObjectiveFail() - objective " + objective_id + " is already ended");
				return false;
			}
			value.Fail();
			HideObjectiveLocations(value);
			ShowPopup("<b>Objective Failed:</b>", value.GetText(), (int)(value.Type + 2), 0);
			PlaySound(m_SoundOnObjFail);
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::ObjectiveFail() - objective " + objective_id + " is not started");
		return false;
	}

	public bool IsObjectiveActive(string objective_id)
	{
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			return value.State == EObjectiveState.Started;
		}
		return false;
	}

	public bool SetObjectiveIsProgressing(string objective_id, bool is_progressing)
	{
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveIsProgressing() - objective " + objective_id + " is already ended");
				return false;
			}
			value.IsProgressing = is_progressing;
			value.IsDirty = true;
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveIsProgressing() - objective " + objective_id + " is not started");
		return false;
	}

	public bool SetObjectiveSpecialText(string objective_id, string special_text)
	{
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveSpecialText() - objective " + objective_id + " is already ended");
				return false;
			}
			value.Progress = special_text;
			value.IsDirty = true;
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveSpecialText() - objective " + objective_id + " is not started");
		return false;
	}

	public bool SetObjectiveTimerValue(string objective_id, float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		string special_text = string.Format("{0}:{1}", timeSpan.Minutes.ToString("00"), timeSpan.Seconds.ToString("00"));
		return SetObjectiveSpecialText(objective_id, special_text);
	}

	public bool SetObjectiveLocations(string objective_id, List<Transform> transforms)
	{
		if (transforms == null)
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveLocations() - given transform list is null for objective " + objective_id);
			return false;
		}
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveLocations() - objective " + objective_id + " is already ended");
				return false;
			}
			HideObjectiveLocations(value);
			value.Locations.Clear();
			foreach (Transform transform in transforms)
			{
				if (transform != null)
				{
					int uID32FromTransform = GetUID32FromTransform(transform);
					if (uID32FromTransform == 0)
					{
						Debug.LogError("Objective : " + objective_id + ": provided transform has transform without serializable object. Please add at least CFGEmptyObject");
					}
					CFGObjectiveLocation cFGObjectiveLocation = new CFGObjectiveLocation();
					Material pssMaterial = GetPssMaterial(value.Type, value.Importance);
					cFGObjectiveLocation.m_PointSprite = AddPss(transform, pssMaterial);
					cFGObjectiveLocation.m_Origin = transform;
					cFGObjectiveLocation.m_OriginUUID = uID32FromTransform;
					cFGObjectiveLocation.m_OriginScene = Application.loadedLevelName;
					value.Locations.Add(cFGObjectiveLocation);
				}
			}
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::SetObjectiveLocations() - objective " + objective_id + " is not started");
		return false;
	}

	public bool AddObjectiveLocation(string objective_id, Transform tr)
	{
		if (tr == null)
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::AddObjectiveLocation() - given transform is null for objective " + objective_id);
			return false;
		}
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::AddObjectiveLocation() - objective " + objective_id + " is already ended");
				return false;
			}
			CFGObjectiveLocation cFGObjectiveLocation = new CFGObjectiveLocation();
			Material pssMaterial = GetPssMaterial(value.Type, value.Importance);
			cFGObjectiveLocation.m_PointSprite = AddPss(tr, pssMaterial);
			cFGObjectiveLocation.m_Origin = tr;
			int uID32FromTransform = GetUID32FromTransform(tr);
			if (uID32FromTransform == 0)
			{
				Debug.LogError("Objective : " + objective_id + ": provided transform has transform without serializable object. Please add at least CFGEmptyObject");
			}
			cFGObjectiveLocation.m_OriginUUID = uID32FromTransform;
			cFGObjectiveLocation.m_OriginScene = Application.loadedLevelName;
			value.Locations.Add(cFGObjectiveLocation);
			if (objective_id.Equals("s7_checkonflorence"))
			{
				CFGFlowSequence mainSequence = CFGSequencerBase.GetMainSequence();
				if (mainSequence != null)
				{
					mainSequence.ActivateSubsequencesByName("NEIGHBOURHOUSE");
				}
			}
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::AddObjectiveLocation() - objective " + objective_id + " is not started");
		return false;
	}

	public bool RemoveObjectiveLocation(string objective_id, Transform tr)
	{
		if (tr == null)
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::RemoveObjectiveLocation() - given transform is null for objective " + objective_id);
			return false;
		}
		CFGObjective value = null;
		if (m_CurrentObjectives.TryGetValue(objective_id, out value))
		{
			if (value.State != EObjectiveState.Started)
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::RemoveObjectiveLocation() - objective " + objective_id + " is already ended");
				return false;
			}
			HashSet<CFGObjectiveLocation> hashSet = new HashSet<CFGObjectiveLocation>();
			foreach (CFGObjectiveLocation location in value.Locations)
			{
				if (location != null && location.m_Origin == tr)
				{
					if (location.m_PointSprite != null)
					{
						UnityEngine.Object.Destroy(location.m_PointSprite);
					}
					hashSet.Add(location);
				}
			}
			foreach (CFGObjectiveLocation item in hashSet)
			{
				value.Locations.Remove(item);
			}
			return true;
		}
		Debug.LogError("ERROR! CFGObjectiveSystem::RemoveObjectiveLocation() - objective " + objective_id + " is not started");
		return false;
	}

	public void ShowHint(string hint_id, bool bShowWindow = true)
	{
		string objective_text = string.Empty;
		if (!GetObjectiveText(hint_id, out objective_text))
		{
			Debug.LogWarning("WARNING! CFGObjectiveSystem::ShowHint() - unlocalized hint id (" + hint_id + ")");
		}
		PlaySound(m_SoundOnNewHint);
	}

	public void OnMissionComplete()
	{
		PlaySound(m_SoundOnMissionComplete);
		HideAllObjectiveLocations();
	}

	public void OnMissionFail()
	{
		PlaySound(m_SoundOnMissionFail);
		HideAllObjectiveLocations();
	}

	public override void Init()
	{
		base.Init();
		ClearData();
		LoadObjectives();
	}

	public void LoadObjectives()
	{
		Debug.Log("Loading objectives for : " + CFGGame.DlcType);
		LoadObjectiveTexts(CFGData.GetDataPathFor("objectives.tsv", CFGGame.DlcType));
	}

	public void OnLanguageChange(bool bForce)
	{
		if (!bForce)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGObjectiveData> objective in m_Objectives)
		{
			if (objective.Value != null)
			{
				objective.Value.UpdateLocalizedTexts();
			}
		}
	}

	private void ClearData()
	{
		m_Objectives.Clear();
	}

	public void OnNewCampaign()
	{
		m_CurrentObjectives.Clear();
		m_DisabledObjectives.Clear();
	}

	public void OnNewScenario()
	{
		OnNewCampaign();
	}

	private void Update()
	{
		List<string> list = (from pair in m_CurrentObjectives
			where pair.Value.State != EObjectiveState.Started && Time.time - pair.Value.EndTime > 5f
			select pair.Key).ToList();
		foreach (string item in list)
		{
			m_CurrentObjectives.Remove(item);
		}
		if (list.Count > 0)
		{
			OnObjectivesChanged();
			return;
		}
		bool flag = false;
		foreach (CFGObjective value in m_CurrentObjectives.Values)
		{
			if (value.IsDirty)
			{
				flag = true;
			}
		}
		if (flag)
		{
			OnObjectivesChanged();
		}
		foreach (CFGObjective value2 in m_CurrentObjectives.Values)
		{
			if (value2.IsDirty)
			{
				value2.IsDirty = false;
			}
		}
	}

	private static void PlaySound(AudioClip audio_clip)
	{
		if (!(audio_clip == null))
		{
			GameObject gameObject = new GameObject();
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.priority = 0;
			audioSource.outputAudioMixerGroup = CFGAudioManager.Instance.m_MixInterface;
			audioSource.clip = audio_clip;
			audioSource.Play();
			UnityEngine.Object.Destroy(gameObject, audio_clip.length);
		}
	}

	private bool ParseObjectivesFileHeader(string row, out int objective_id_col, out int importance_col, out int type_col, out int scene_col)
	{
		string[] array = row.Split('\t');
		objective_id_col = -1;
		importance_col = -1;
		type_col = -1;
		scene_col = -1;
		for (int i = 0; i < array.Length; i++)
		{
			switch (array[i])
			{
			case "objective_id":
				objective_id_col = i;
				break;
			case "importance":
				importance_col = i;
				break;
			case "icon":
				type_col = i;
				break;
			case "type":
				scene_col = i;
				break;
			}
		}
		return objective_id_col != -1 && importance_col != -1 && type_col != -1 && scene_col != -1;
	}

	private bool LoadObjectiveTexts(string filePath)
	{
		m_Objectives.Clear();
		string text = CFGData.ReadAllText(filePath);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError($"{GetType().Name}::{MethodBase.GetCurrentMethod().Name} - resource: {filePath} does not exist or is empty");
			return false;
		}
		string[] array = text.ToLines();
		if (!ParseObjectivesFileHeader(array[0], out var objective_id_col, out var importance_col, out var type_col, out var scene_col))
		{
			Debug.LogError("ERROR! CFGObjectiveSystem::LoadObjectiveTexts() - wrong header in objective texts file (" + filePath + ")");
			return false;
		}
		for (int i = 1; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('\t');
			if (string.IsNullOrEmpty(array2[0].Trim()))
			{
				continue;
			}
			string text2 = array2[objective_id_col].Trim();
			if (text2 == string.Empty)
			{
				continue;
			}
			if (m_Objectives.ContainsKey(text2))
			{
				Debug.LogError("ERROR! CFGObjectiveSystem::LoadObjectiveTexts() - duplicated objective id (" + text2 + ") in " + filePath);
				continue;
			}
			EObjectiveImportance importance = array2[importance_col].ToObjectiveImportance();
			EObjectiveType type = array2[type_col].ToObjectiveType();
			EObjectiveScene oScene;
			try
			{
				oScene = (EObjectiveScene)(int)Enum.Parse(typeof(EObjectiveScene), array2[scene_col], ignoreCase: true);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to parse objective scene type, Line: " + i + "\nException: " + ex);
				oScene = EObjectiveScene.Both;
			}
			CFGObjectiveData cFGObjectiveData = new CFGObjectiveData(text2, importance, type, oScene);
			if (cFGObjectiveData != null)
			{
				cFGObjectiveData.UpdateLocalizedTexts();
				m_Objectives.Add(text2, cFGObjectiveData);
			}
		}
		return true;
	}

	private bool GetObjectiveText(string objective_id, out string objective_text)
	{
		if (m_Objectives.TryGetValue(objective_id, out var value))
		{
			objective_text = value.m_DescShort;
			return true;
		}
		objective_text = "&" + objective_id + "&";
		return false;
	}

	private int ObjectivesComparison(CFGObjective a, CFGObjective b)
	{
		int num = a.Importance.CompareTo(b.Importance);
		if (num != 0)
		{
			return num;
		}
		int num2 = a.StartTime.CompareTo(b.StartTime);
		if (num2 != 0)
		{
			return num2;
		}
		return a.DescShort.CompareTo(b.DescShort);
	}

	public void OnObjectivesChanged(bool UseCustomSceneType = false, EObjectiveScene CustomST = EObjectiveScene.Both)
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (!instance || !(instance.m_ObjectivePanel != null))
		{
			return;
		}
		List<CFGObjective> list = m_CurrentObjectives.Values.ToList();
		int num = 0;
		EObjectiveScene eObjectiveScene = EObjectiveScene.Strategic;
		if (UseCustomSceneType)
		{
			if (CustomST == EObjectiveScene.Strategic)
			{
				eObjectiveScene = EObjectiveScene.Tactical;
			}
		}
		else if (CFGSingleton<CFGGame>.Instance.GetGameState() == EGameState.Strategic)
		{
			eObjectiveScene = EObjectiveScene.Tactical;
		}
		while (num < list.Count)
		{
			if (list[num] != null && list[num].DesignatedSceneType != eObjectiveScene)
			{
				num++;
			}
			else
			{
				list.RemoveAt(num);
			}
		}
		list.Sort(ObjectivesComparison);
		list.Reverse();
		instance.m_ObjectivePanel.SetObjectivesNumber(Math.Min(list.Count, instance.m_ObjectivePanel.m_ObjsTexts.Count));
		for (num = 0; num < instance.m_ObjectivePanel.m_ObjsTexts.Count; num++)
		{
			if (num < list.Count)
			{
				instance.m_ObjectivePanel.m_ObjsTexts[num].text = list[num].GetText();
				instance.m_ObjectivePanel.SetObjState(num, (int)list[num].State, (int)list[num].Type, list[num].IsDirty, list[num].Importance);
			}
		}
	}

	private void ShowPopup(string text, string desc, int icon, int popup)
	{
	}

	private void HidePopup(int popup)
	{
	}

	private Material GetPssMaterial(EObjectiveType objective_type, EObjectiveImportance imp)
	{
		List<Material> list = ((imp != 0) ? m_LocOptMats : m_LocMats);
		if (list == null || (int)objective_type >= list.Count)
		{
			return null;
		}
		return list[(int)objective_type];
	}

	private CFGPointSprite AddPss(Transform t, Material material)
	{
		CFGPointSprite cFGPointSprite = t.gameObject.AddComponent<CFGObjectivePointSprite>();
		cFGPointSprite.m_Material = new Material(material);
		cFGPointSprite.m_ScaleY = m_LocScaleY;
		cFGPointSprite.m_HeightOffset = m_LocHeightOffset;
		cFGPointSprite.m_AlwaysOnScreen = true;
		cFGPointSprite.OnEnable();
		return cFGPointSprite;
	}

	private void HideObjectiveLocations(CFGObjective objective)
	{
		if (objective == null)
		{
			return;
		}
		foreach (CFGObjectiveLocation location in objective.Locations)
		{
			if (location != null && location.m_PointSprite != null)
			{
				UnityEngine.Object.Destroy(location.m_PointSprite);
				location.m_PointSprite = null;
			}
		}
	}

	private void HideAllObjectiveLocations()
	{
		foreach (CFGObjective value in m_CurrentObjectives.Values)
		{
			HideObjectiveLocations(value);
		}
	}

	public void DeactivateObjective(string ObjectiveName)
	{
		if (m_DisabledObjectives.ContainsKey(ObjectiveName))
		{
			Debug.Log("Objective: " + ObjectiveName + " is already deactivated");
			return;
		}
		bool flag = false;
		foreach (KeyValuePair<string, CFGObjective> currentObjective in m_CurrentObjectives)
		{
			if (string.Compare(currentObjective.Key, ObjectiveName, ignoreCase: true) == 0)
			{
				currentObjective.Value.IsSelected = false;
				m_DisabledObjectives.Add(currentObjective.Key, currentObjective.Value);
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_CurrentObjectives.Remove(ObjectiveName);
			OnObjectivesChanged();
		}
		else
		{
			Debug.Log("Failed to find objective: " + ObjectiveName);
		}
	}

	public void ActivateObjective(string ObjectiveName)
	{
		if (m_CurrentObjectives.ContainsKey(ObjectiveName))
		{
			Debug.Log("Objective: " + ObjectiveName + " is already on current list");
			return;
		}
		bool flag = false;
		foreach (KeyValuePair<string, CFGObjective> disabledObjective in m_DisabledObjectives)
		{
			if (string.Compare(disabledObjective.Key, ObjectiveName, ignoreCase: true) == 0)
			{
				m_CurrentObjectives.Add(disabledObjective.Key, disabledObjective.Value);
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_DisabledObjectives.Remove(ObjectiveName);
			OnObjectivesChanged();
		}
		else
		{
			Debug.Log("Failed to find disabled objective: " + ObjectiveName);
		}
	}

	public void FinalizeObjectiveLoad()
	{
		OnObjectivesChanged();
	}

	public void OnLevelEnd()
	{
		HideAllObjectiveLocations();
	}

	public void OnLevelStart()
	{
		if (m_CurrentObjectives == null || m_CurrentObjectives.Count == 0)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGObjective> currentObjective in m_CurrentObjectives)
		{
			if (currentObjective.Value.Locations == null || currentObjective.Value.Locations.Count == 0 || currentObjective.Value.State != EObjectiveState.Started)
			{
				continue;
			}
			CFGObjective value = currentObjective.Value;
			foreach (CFGObjectiveLocation location in currentObjective.Value.Locations)
			{
				if (location != null && location.m_OriginUUID != 0 && string.Compare(location.m_OriginScene, Application.loadedLevelName, ignoreCase: true) == 0)
				{
					CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(location.m_OriginUUID, ESerializableType.NotSerializable);
					if (cFGSerializableObject != null)
					{
						Material pssMaterial = GetPssMaterial(value.Type, value.Importance);
						location.m_Origin = cFGSerializableObject.transform;
						location.m_PointSprite = AddPss(cFGSerializableObject.transform, pssMaterial);
					}
				}
			}
		}
	}

	public void OnTacticalEnd()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, CFGObjective> currentObjective in m_CurrentObjectives)
		{
			if (currentObjective.Value.DesignatedSceneType == EObjectiveScene.Tactical)
			{
				list.Add(currentObjective.Key);
			}
		}
		foreach (string item in list)
		{
			ObjectiveComplete(item, bSilent: true);
		}
		list.Clear();
		foreach (KeyValuePair<string, CFGObjective> disabledObjective in m_DisabledObjectives)
		{
			if (disabledObjective.Value.DesignatedSceneType == EObjectiveScene.Tactical)
			{
				list.Add(disabledObjective.Key);
			}
		}
		foreach (string item2 in list)
		{
			m_DisabledObjectives.Remove(item2);
		}
	}

	public bool OnSerialize(CFG_SG_Node ObjNode)
	{
		CFG_SG_Node cFG_SG_Node = ObjNode.AddSubNode("Current");
		CFG_SG_Node cFG_SG_Node2 = ObjNode.AddSubNode("Disabled");
		if (cFG_SG_Node == null || cFG_SG_Node2 == null)
		{
			return false;
		}
		foreach (KeyValuePair<string, CFGObjective> currentObjective in m_CurrentObjectives)
		{
			CFG_SG_Node cFG_SG_Node3 = cFG_SG_Node.AddSubNode("Item");
			if (cFG_SG_Node3 == null)
			{
				return false;
			}
			cFG_SG_Node3.Attrib_Set("Name", currentObjective.Key);
			if (!WriteObjectiveData(cFG_SG_Node3, currentObjective.Value))
			{
				return false;
			}
		}
		foreach (KeyValuePair<string, CFGObjective> disabledObjective in m_DisabledObjectives)
		{
			CFG_SG_Node cFG_SG_Node4 = cFG_SG_Node2.AddSubNode("Item");
			if (cFG_SG_Node4 == null)
			{
				return false;
			}
			cFG_SG_Node4.Attrib_Set("Name", disabledObjective.Key);
			if (!WriteObjectiveData(cFG_SG_Node4, disabledObjective.Value))
			{
				return false;
			}
		}
		return true;
	}

	private bool WriteObjectiveData(CFG_SG_Node nd, CFGObjective obd)
	{
		nd.Attrib_Set("ID", obd.Id);
		nd.Attrib_Set("State", obd.State);
		nd.Attrib_Set("Progress", obd.Progress);
		nd.Attrib_Set("Start", obd.StartTime);
		nd.Attrib_Set("End", obd.EndTime);
		nd.Attrib_Set("New", obd.IsNew);
		nd.Attrib_Set("Readed", obd.AlreadyReaded);
		foreach (CFGObjectiveLocation location in obd.Locations)
		{
			CFG_SG_Node cFG_SG_Node = nd.AddSubNode("Location");
			if (cFG_SG_Node == null)
			{
				return false;
			}
			int originUUID = location.m_OriginUUID;
			if (originUUID == 0)
			{
				if (location.m_Origin == null)
				{
					Debug.LogError("NULL location origin!");
				}
				CFGError.ReportError("Objective : " + obd.Id + " could not find its location on scene", CFGError.ErrorCode.None, CFGError.ErrorType.Warning);
				CFGError.Show();
			}
			cFG_SG_Node.Attrib_Set("TargetUUID", originUUID);
			cFG_SG_Node.Attrib_Set("Name", location.m_OriginScene);
		}
		return true;
	}

	public bool OnDeserialize(CFG_SG_Node ObjNode)
	{
		if (ObjNode == null)
		{
			return false;
		}
		m_CurrentObjectives.Clear();
		m_DisabledObjectives.Clear();
		CFG_SG_Node cFG_SG_Node = ObjNode.FindSubNode("Current");
		CFG_SG_Node cFG_SG_Node2 = ObjNode.FindSubNode("Disabled");
		m_CurrentObjectives.Clear();
		m_DisabledObjectives.Clear();
		if (cFG_SG_Node != null && !DeserializeObjectiveList(ref m_CurrentObjectives, cFG_SG_Node, bActive: true))
		{
			return false;
		}
		if (cFG_SG_Node2 != null && !DeserializeObjectiveList(ref m_DisabledObjectives, cFG_SG_Node2, bActive: false))
		{
			return false;
		}
		EObjectiveScene customST = EObjectiveScene.Tactical;
		if (CFG_SG_Manager.CurrentSaveGame != null && CFG_SG_Manager.CurrentSaveGame.SaveGameMode == CFG_SG_SaveGame.eSG_Mode.Strategic)
		{
			customST = EObjectiveScene.Strategic;
		}
		OnObjectivesChanged(UseCustomSceneType: true, customST);
		return true;
	}

	private bool DeserializeObjectiveList(ref Dictionary<string, CFGObjective> ObjList, CFG_SG_Node node, bool bActive)
	{
		for (int i = 0; i < node.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = node.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Item", ignoreCase: true) != 0)
			{
				continue;
			}
			string text = subNode.Attrib_Get("ID", string.Empty);
			if (ObjList.ContainsKey(text))
			{
				Debug.LogError("ERROR! - objective " + text + " is already on list");
				continue;
			}
			if (!m_Objectives.ContainsKey(text))
			{
				Debug.LogWarning("Warning! objective " + text + " is not loaded. Creating new default objective...");
				m_Objectives.Add(text, new CFGObjectiveData(text, EObjectiveImportance.Main, EObjectiveType.Generic, EObjectiveScene.Both));
			}
			List<CFGObjectiveLocation> list = new List<CFGObjectiveLocation>();
			for (int j = 0; j < subNode.SubNodeCount; j++)
			{
				CFG_SG_Node subNode2 = subNode.GetSubNode(j);
				if (subNode2 != null && string.Compare(subNode2.Name, "Location", ignoreCase: true) == 0)
				{
					int num = subNode2.Attrib_Get("TargetUUID", 0);
					if (num == 0)
					{
						Debug.LogWarning("Objective: " + text + " has location without serialization script!");
						continue;
					}
					string originScene = subNode2.Attrib_Get<string>("Name", null);
					CFGObjectiveLocation cFGObjectiveLocation = new CFGObjectiveLocation();
					cFGObjectiveLocation.m_Origin = null;
					cFGObjectiveLocation.m_OriginScene = originScene;
					cFGObjectiveLocation.m_OriginUUID = num;
					cFGObjectiveLocation.m_PointSprite = null;
					list.Add(cFGObjectiveLocation);
				}
			}
			CFGObjective cFGObjective = new CFGObjective(m_Objectives[text], subNode);
			m_CurrentObjectives.Add(text, cFGObjective);
			if (list.Count > 0 && cFGObjective.State == EObjectiveState.Started)
			{
				cFGObjective.Locations.AddRange(list);
			}
			if (!bActive)
			{
				DeactivateObjective(text);
			}
		}
		return true;
	}

	private int GetUID32FromTransform(Transform trans)
	{
		if (trans == null)
		{
			return 0;
		}
		CFGSerializableObject component = trans.GetComponent<CFGSerializableObject>();
		if (component == null || component.SerializableType == ESerializableType.NotSerializable)
		{
			Debug.LogWarning("Transform has no Serializable Gameobject attached. Please add CFGEmptyObject to the objective target");
			return 0;
		}
		if (!component.IsUniqueID_OK)
		{
			Debug.LogWarning("Location's game object has invalid Unique ID");
		}
		return component.UniqueID;
	}
}
