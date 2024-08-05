using System;
using System.Collections.Generic;
using UnityEngine;

internal class CFGDevInGameWindow : CFGWindow
{
	private string m_CamDestRotH = "0";

	private string[] m_CharacterIDs;

	private Rect m_MainWindowRect = new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.05f, (float)Screen.width - (float)Screen.width * 0.1f, (float)Screen.height - (float)Screen.height * 0.1f);

	private Rect m_SpawningWindowRect = new Rect(Screen.width / 10, Screen.height / 2 - 50, 300f, 100f);

	private string[] m_PanelNames = new string[14]
	{
		"Picker", "DevOpts", "Spawn", "LoS", "Dialogs", "Objectives", "Audio & Video", "Achievements", "Camera", "AI",
		"Keys", "Joy", "MapVis", "Team"
	};

	private string[] m_TeamNames;

	private EPanel m_CurrentPanel;

	private ETeamPanel m_CurrentTeamPanel;

	private string m_ErrorText = string.Empty;

	private string m_DialogId = string.Empty;

	private bool m_TestingObjectives;

	private int m_ObjectivesCount;

	private string[] m_Objectives = new string[5]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	private CFGListBox m_ResolutionListbox;

	private bool m_Fullscreen;

	private int m_ShadowsQuality;

	private int m_SelectedOwnerIdx;

	private CFGOwner m_SelectedOwner;

	private int m_SelectedCharacterIdx;

	private string m_SelectedCharacterId = string.Empty;

	private bool m_IsInSpawningMode;

	private Vector3 m_CameraStartPos = Vector3.zero;

	private float m_CameraStartAngleH;

	private float m_CameraStartAngleV;

	private float m_CameraStartDist;

	private Vector3 m_CameraEndPos = Vector3.zero;

	private float m_CameraEndAngleH;

	private float m_CameraEndAngleV;

	private float m_CameraEndDist;

	private float m_CameraPlayDuration = 2f;

	private float m_CameraStartFOV = 60f;

	private float m_CameraEndFOV = 60f;

	private float m_CameraInitialFOV;

	private float m_CameraInitialNearClipPlane;

	private float m_CameraInitialFarClipPlane;

	private bool m_CameraPlaySmooth;

	private float m_CameraPlayTime = -1f;

	private CFGCellShadowMapVis m_ShadowMapVis;

	private ELOXHitType los;

	private string m_LoSResult = string.Empty;

	private int startX;

	private int startZ;

	private int startFloor;

	private int endX;

	private int endZ;

	private int endFloor;

	private static bool m_LoSCalculated = false;

	private static Vector3 m_Draw_StartPoint = Vector3.zero;

	private static Vector3 m_Draw_EndPoint = Vector3.zero;

	private static Vector3 m_LastIntersection = Vector3.zero;

	private EActionCommand m_KEY_Selected;

	private Vector2 m_KEY_ScrollPos = Vector2.zero;

	private ELocationState m_LOC_NewState;

	private int m_SelectedChar = -1;

	private Vector2 m_CharsScrollPos = Vector2.zero;

	private string m_GiveItemID = string.Empty;

	private string[] m_TeamIDStr = new string[2] { "Team 1", "Team 2" };

	private string[] m_StrJoyProfiles = new string[4] { "#1", "#2", "#3", "#4" };

	private CFGCharacter m_LastSelectedChar;

	private static string[] allabs = null;

	private static ETurnAction[] allabsta = null;

	private int m_LastSelAbility;

	private string[] m_buffs;

	private int m_buffs_selection;

	private bool mapVisDirty;

	private int m_ShopList_SelectedID;

	private CFGStore m_ShopList_Slected;

	private float m_ShopList_ScrollBarPos;

	private string[] m_ShopList_Strings;

	private int m_ShopList_FirstItem_Global;

	private int m_ShopGoods_FirstItem_Global;

	private float m_ShopGoods_ScrollBarPos;

	private List<CFGStoreGood> m_ShopGoodsAllItems;

	private int m_ShopGoods_SortType;

	private static string[] AllUsables = null;

	private int m_LastSelUsable;

	private int m_FirstItem_Global;

	private float m_ScrollBarPos;

	private List<CFGDef_Item> m_AllItems;

	private int m_Backpack_SortType;

	private string m_LootPack = string.Empty;

	public override EWindowID GetWindowID()
	{
		return EWindowID.DEV_InGameWindow;
	}

	protected override void OnActivate()
	{
		int num = -1;
		string[] array = new string[Screen.resolutions.Length];
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			array[i] = Screen.resolutions[i].width + "x" + Screen.resolutions[i].height + "x" + Screen.resolutions[i].refreshRate;
			if (Screen.resolutions[i].width == Screen.width && Screen.resolutions[i].height == Screen.height && Screen.resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
			{
				num = i;
			}
		}
		m_ResolutionListbox = new CFGListBox(array);
		if (num != -1)
		{
			m_ResolutionListbox.SelectElement(num);
		}
		m_Fullscreen = Screen.fullScreen;
		m_ShadowsQuality = QualitySettings.GetQualityLevel();
		Shader.globalMaximumLOD = Mathf.Clamp(Shader.globalMaximumLOD, 200, 1000);
	}

	protected override void OnDeactivate()
	{
	}

	protected override void OnUpdate()
	{
		int length = Enum.GetValues(typeof(EPanel)).Length;
		if (CFGJoyManager.IsActivated(EJoyAction.Dev_PrevPanel))
		{
			m_CurrentPanel--;
			if (m_CurrentPanel < EPanel.Picker)
			{
				m_CurrentPanel = (EPanel)(length - 1);
			}
		}
		if (CFGJoyManager.IsActivated(EJoyAction.Dev_NextPanel))
		{
			m_CurrentPanel++;
			if ((int)m_CurrentPanel >= length)
			{
				m_CurrentPanel = EPanel.Picker;
			}
		}
	}

	private void Start()
	{
		m_CameraInitialFOV = Camera.main.fieldOfView;
		m_CameraInitialNearClipPlane = Camera.main.nearClipPlane;
		m_CameraInitialFarClipPlane = Camera.main.farClipPlane;
	}

	protected override void DrawWindow()
	{
		if (m_IsInSpawningMode)
		{
			m_SpawningWindowRect = GUILayout.Window((int)GetWindowID(), m_SpawningWindowRect, MakeSpawningWindow, "Spawn character");
		}
		else
		{
			m_MainWindowRect = GUILayout.Window((int)GetWindowID(), m_MainWindowRect, MakeMainWindow, "Developers panel");
		}
	}

	public void SetCurrentPanel(EPanel panel)
	{
		m_CurrentPanel = panel;
	}

	private void UpdateFloatWithSlider(ref float Val, float MinV, float MaxV, string Desc)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(Desc + ": " + Val + "(" + MinV + " - " + MaxV + " )", GUILayout.Width(400f));
		Val = GUILayout.HorizontalSlider(Val, MinV, MaxV, GUILayout.Width(400f));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void MakeMainWindow(int id)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component == null)
		{
			return;
		}
		int length = Enum.GetValues(typeof(EPanel)).Length;
		EPanel ePanel = (EPanel)GUILayout.SelectionGrid((int)m_CurrentPanel, m_PanelNames, length);
		if (ePanel != m_CurrentPanel)
		{
			m_CurrentPanel = ePanel;
			m_ErrorText = string.Empty;
		}
		GUILayout.Space(10f);
		switch (m_CurrentPanel)
		{
		case EPanel.Team:
			MakePanel_Team();
			break;
		case EPanel.Keys:
			OnDraw_Keys();
			break;
		case EPanel.Picker:
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (CFGSingleton<CFGWindowMgr>.Instance.IsCursorOverUI())
			{
				GUI.color = Color.red;
				GUILayout.Label("Cursor over UI");
			}
			else
			{
				GUI.color = Color.green;
				GUILayout.Label("Cursor over game");
			}
			GUI.color = Color.white;
			CFGCell cellUnderCursor = component.CellUnderCursor;
			if (cellUnderCursor != null)
			{
				int PosZ = 0;
				int PosX = 0;
				int Floor = 0;
				cellUnderCursor.DecodePosition(out PosX, out PosZ, out Floor);
				GUILayout.Label("Tile: " + PosX + ", " + PosZ + " (Floor: " + Floor + ")");
				GUILayout.Label("Character: " + cellUnderCursor.CurrentCharacter);
				if (component != null && component.SelectedCharacter != null)
				{
					float num2 = Vector3.Distance(component.SelectedCharacter.CurrentCell.WorldPosition, cellUnderCursor.WorldPosition);
					GUILayout.Label("Distance to selected character: " + num2);
				}
				GUILayout.Label("Parent: " + cellUnderCursor.OwnerObject);
				GUILayout.Label("Parent Interior:" + cellUnderCursor.InteriorObject);
				string text2 = string.Empty;
				if (cellUnderCursor.CheckFlag(1, 8))
				{
					text2 += "Impassable, ";
				}
				if (cellUnderCursor.CheckFlag(1, 64))
				{
					text2 += "Activator(Center), ";
				}
				if (cellUnderCursor.CheckFlag(1, 16))
				{
					text2 += "BlockSight(Center), ";
				}
				if ((bool)cellUnderCursor.CurrentCharacter)
				{
					cellUnderCursor.GetBestCoverToGlue(cellUnderCursor.CurrentCharacter, out var cover_type, out var dir);
					GUILayout.Label("Best cover to glue: " + cover_type + " dir = " + dir);
				}
				GUILayout.Label("delta height: " + cellUnderCursor.DeltaHeight);
				GUILayout.Label("Normal: Flags: " + text2);
				GUILayout.Label("Is In Forced Light: " + cellUnderCursor.IsInLight);
				GUILayout.Label("Floor marked: " + cellUnderCursor.CheckFlag(0, 2));
				GUILayout.Label("Cover: " + cellUnderCursor.GetCenterCover());
				GUILayout.Label(string.Concat("Cover N: ", cellUnderCursor.GetBorderCover(EDirection.NORTH), " Activator: ", cellUnderCursor.CheckFlag(2, 64)));
				GUILayout.Label(string.Concat("Cover E: ", cellUnderCursor.GetBorderCover(EDirection.EAST), " Activator: ", cellUnderCursor.CheckFlag(4, 64)));
				GUILayout.Label(string.Concat("Cover S: ", cellUnderCursor.GetBorderCover(EDirection.SOUTH), " Activator: ", cellUnderCursor.CheckFlag(3, 64)));
				GUILayout.Label(string.Concat("Cover W: ", cellUnderCursor.GetBorderCover(EDirection.WEST), " Activator: ", cellUnderCursor.CheckFlag(5, 64)));
				GUILayout.Label("Usable: " + cellUnderCursor.UsableObject);
				GUILayout.Label("Door: " + cellUnderCursor.DoorObject);
			}
			GUILayout.EndVertical();
			GUILayout.Space(50f);
			GUILayout.BeginVertical();
			CFGGameObject objectUnderCursor = component.ObjectUnderCursor;
			if (objectUnderCursor != null)
			{
				GUILayout.Label("Object: " + objectUnderCursor.name);
				GUILayout.Label("Name: " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(objectUnderCursor.NameId));
				OnDraw_Character(objectUnderCursor as CFGCharacter);
				OnDraw_Location(objectUnderCursor as CFGLocationObject);
				OnDraw_Usable(objectUnderCursor as CFGUsableObject);
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			break;
		}
		case EPanel.DevOptions:
		{
			CFGCheats.GlobalInvulnerability = GUILayout.Toggle(CFGCheats.GlobalInvulnerability, "Global invulnerability");
			CFGCheats.InfiniteLuck = GUILayout.Toggle(CFGCheats.InfiniteLuck, "Infinite luck");
			CFGCheats.InfiniteAP = GUILayout.Toggle(CFGCheats.InfiniteAP, "Infinite AP");
			CFGCheats.InfiniteAmmo = GUILayout.Toggle(CFGCheats.InfiniteAmmo, "Infinite ammo");
			CFGCheats.AllCharactersVisible = GUILayout.Toggle(CFGCheats.AllCharactersVisible, "All characters visible");
			GUILayout.Label("New Game Plus: " + CFGGame.NewGamePlusFlags);
			if (GUILayout.Toggle(CFGCheats.UseOutline, "Use outline") != CFGCheats.UseOutline)
			{
				CFGCheats.UseOutline = !CFGCheats.UseOutline;
				CFGHighlighter component3 = Camera.main.GetComponent<CFGHighlighter>();
				if ((bool)component3)
				{
					component3.enabled = CFGCheats.UseOutline;
				}
			}
			CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
			if (aiOwner != null)
			{
				aiOwner.UpdateAi = GUILayout.Toggle(aiOwner.UpdateAi, "AI enabled");
			}
			CFGCheats.DisableCameraFocuses = GUILayout.Toggle(CFGCheats.DisableCameraFocuses, "Disable camera focuses");
			UpdateFloatWithSlider(ref CFGOptions.DevOptions.SetupStateMoveSpeedMul, 0.2f, 2f, "SetupStage: Move Mul");
			CFGIndicatorCamera.s_IndicatorsEnabled = GUILayout.Toggle(CFGIndicatorCamera.s_IndicatorsEnabled, "Draw indicators");
			CFGRangeBorders.s_DrawRangeBorders = GUILayout.Toggle(CFGRangeBorders.s_DrawRangeBorders, "Draw range borders and cursor");
			if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Unlock all locations"))
				{
					CFGLocationObject[] array = UnityEngine.Object.FindObjectsOfType<CFGLocationObject>();
					if (array == null || array.Length == 0)
					{
						Debug.Log("No location objects were found");
					}
					else
					{
						int num = 0;
						CFGLocationObject[] array2 = array;
						foreach (CFGLocationObject cFGLocationObject in array2)
						{
							if (cFGLocationObject.State != 0)
							{
								cFGLocationObject.SetState(ELocationState.OPEN);
								num++;
							}
						}
						Debug.Log("Changed state of " + num + " location objects of total " + array.Length);
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("CloseStrategicExplorator") && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator)
			{
				CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator.m_ExplorationButtons[0].OnPointerUp(null);
			}
			UpdateFloatWithSlider(ref CFGOptions.Input.GamepadCursorSpeed, 1f, 20f, "GamepadCursorSpeed");
			UpdateFloatWithSlider(ref CFGOptions.Input.GamepadCameraMoveSpeed, 1f, 20f, "GamepadCameraMoveSpeed");
			UpdateFloatWithSlider(ref CFGOptions.Input.GamepadCameraRotateSpeed, 1f, 100f, "GamepadCameraRotateSpeed");
			UpdateFloatWithSlider(ref CFGOptions.Input.GamepadDZ, 0.1f, 0.8f, "Gamepad - Dead Zone");
			GUILayout.Space(10f);
			UpdateFloatWithSlider(ref CFGOptions.Gameplay.PathShowDelay, 0.001f, 2f, "PathShowDelay");
			CFGCellMap.m_MinSidestepCoverType = ((!GUILayout.Toggle(CFGCellMap.m_MinSidestepCoverType == ECoverType.NONE, "Sidesteps without cover")) ? ECoverType.FULL : ECoverType.NONE);
			CFGOptions.Gameplay.InstantTexts = GUILayout.Toggle(CFGOptions.Gameplay.InstantTexts, "Instant texts");
			CFGOptions.Input.AllowLMBOnStrategic = GUILayout.Toggle(CFGOptions.Input.AllowLMBOnStrategic, "Allow LBM on strategic");
			GUILayout.Space(30f);
			GUILayout.Label("Other:");
			GUILayout.BeginHorizontal();
			if (CFGSingleton<CFGGame>.Instance.IsInGame())
			{
				if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage)
				{
					if (GUILayout.Button("Start Combat"))
					{
						CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = false;
						if (Input.GetKey(KeyCode.LeftAlt))
						{
							foreach (CFGCharacter character in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Characters)
							{
								if ((bool)character && character.IsAlive)
								{
									character.CanDoReactionShot = true;
								}
							}
						}
					}
				}
				else if (GUILayout.Button("End Combat"))
				{
					CFGSingletonResourcePrefab<CFGTurnManager>.Instance.InSetupStage = true;
				}
				if (GUILayout.Button("Activate Reactions") && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner != null && CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters != null)
				{
					foreach (CFGCharacter character2 in CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner.Characters)
					{
						character2.CanDoReactionShot = true;
					}
				}
			}
			GUILayout.Space(80f);
			GUILayout.Label("Difficulty Level: " + CFGGame.Difficulty, GUILayout.Width(150f));
			if (GUILayout.Button("Easy", GUILayout.Width(80f)))
			{
				CFGGame.Difficulty = EDifficulty.Easy;
			}
			if (GUILayout.Button("Normal", GUILayout.Width(80f)))
			{
				CFGGame.Difficulty = EDifficulty.Normal;
			}
			if (GUILayout.Button("Hard", GUILayout.Width(80f)))
			{
				CFGGame.Difficulty = EDifficulty.Hard;
			}
			GUILayout.Space(80f);
			GUILayout.Label("Permadeath:");
			if (GUILayout.Button((!CFGGame.Permadeath) ? "Disable" : "Enabled", GUILayout.Width(80f)))
			{
				CFGGame.Permadeath = !CFGGame.Permadeath;
			}
			GUILayout.Space(80f);
			GUILayout.Label("Injuries:");
			if (GUILayout.Button((!CFGGame.InjuriesEnabled) ? "Disable" : "Enabled", GUILayout.Width(80f)))
			{
				CFGGame.InjuriesEnabled = !CFGGame.InjuriesEnabled;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			CFGOptions.DevOptions.SmoothPathVisualization = GUILayout.Toggle(CFGOptions.DevOptions.SmoothPathVisualization, "Smooth Path Visualization");
			CFGOptions.DevOptions.RAWPath = GUILayout.Toggle(CFGOptions.DevOptions.RAWPath, "RAW Paths");
			CFGOptions.DevOptions.SuspicionNotifications = GUILayout.Toggle(CFGOptions.DevOptions.SuspicionNotifications, "Detailed Suspicion / Subdued Notifications");
			float Val = Application.targetFrameRate;
			UpdateFloatWithSlider(ref Val, -1f, 60f, "TargetFrameRate");
			Application.targetFrameRate = (int)Val;
			float Val2 = QualitySettings.vSyncCount;
			UpdateFloatWithSlider(ref Val2, 0f, 2f, "VSCount");
			QualitySettings.vSyncCount = (int)Val2;
			GUILayout.Space(50f);
			if (GUILayout.Button("Save Options"))
			{
				CFGOptions.Save();
			}
			break;
		}
		case EPanel.Spawn:
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Choose Owner:");
			List<CFGOwner> list = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.Owners.ToList();
			string[] array3 = new string[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				array3[k] = list[k].name;
			}
			m_SelectedOwnerIdx = GUILayout.Toolbar(m_SelectedOwnerIdx, array3);
			m_SelectedOwner = list[m_SelectedOwnerIdx];
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Label("Choose character:");
			if (m_CharacterIDs == null)
			{
				List<CFGCharacterIdPrefab> characterIdPrefabMap = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.CharacterIdPrefabMap;
				List<string> list2 = new List<string>();
				foreach (CFGCharacterIdPrefab item in characterIdPrefabMap)
				{
					list2.Add(item.m_Id);
				}
				list2.Sort();
				m_CharacterIDs = new string[characterIdPrefabMap.Count];
				for (int l = 0; l < list2.Count; l++)
				{
					m_CharacterIDs[l] = list2[l];
				}
			}
			m_SelectedCharacterIdx = GUILayout.SelectionGrid(m_SelectedCharacterIdx, m_CharacterIDs, 10, GUILayout.MaxWidth(m_MainWindowRect.width * 0.98f));
			m_SelectedCharacterId = m_CharacterIDs[m_SelectedCharacterIdx];
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Spawn"))
			{
				m_IsInSpawningMode = true;
			}
			CFGCharacter targetedCharacter = component.TargetedCharacter;
			GUI.enabled = targetedCharacter != null;
			string text = "Change Owner ";
			if (targetedCharacter != null)
			{
				text += targetedCharacter.NameId;
			}
			if ((bool)m_SelectedOwner)
			{
				text = text + " to " + m_SelectedOwner.name;
			}
			if (GUILayout.Button(text))
			{
				targetedCharacter.ChangeOwner(m_SelectedOwner);
				if (m_SelectedOwner != null && m_SelectedOwner.IsPlayer)
				{
					bool flag = CFGCharacterList.AssignToTeam(targetedCharacter.NameId);
					Debug.LogError("Assign to team: " + targetedCharacter.NameId + " res = " + flag);
				}
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			break;
		}
		case EPanel.LoS:
			OnDrawLoS();
			break;
		case EPanel.AI:
			OnDrawAI();
			break;
		case EPanel.Dialogs:
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("test alert"))
			{
				CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("test_alert");
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label("Dialog id:");
			GUILayout.BeginHorizontal();
			m_DialogId = GUILayout.TextField(m_DialogId, GUILayout.Width(200f));
			if (m_DialogId == "`")
			{
				m_DialogId = string.Empty;
			}
			if ((GUILayout.Button("Play", GUILayout.ExpandWidth(expand: false)) || (Event.current.type == EventType.KeyDown && Event.current.character == '\n')) && m_DialogId != string.Empty)
			{
				if (CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayDialog(m_DialogId))
				{
					m_ErrorText = string.Empty;
				}
				else
				{
					m_ErrorText = "Dialog " + m_DialogId + " not found";
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			break;
		case EPanel.Objectives:
		{
			m_TestingObjectives = GUILayout.Toggle(m_TestingObjectives, "Test objectives");
			CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
			if (m_TestingObjectives && (bool)instance && instance.m_ObjectivePanel != null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Objectives count: " + m_ObjectivesCount);
				if (GUILayout.Button("-") && m_ObjectivesCount > 0)
				{
					m_ObjectivesCount--;
				}
				if (GUILayout.Button("+") && m_ObjectivesCount < 5)
				{
					m_ObjectivesCount++;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Label("m_PerLineOffset: " + instance.m_ObjectivePanel.m_PerLineOffset2);
				instance.m_ObjectivePanel.m_PerLineOffset2 = GUILayout.HorizontalSlider(instance.m_ObjectivePanel.m_PerLineOffset2, 0f, 100f);
				instance.m_ObjectivePanel.SetObjectivesNumber(m_ObjectivesCount);
				for (int j = 0; j < m_ObjectivesCount; j++)
				{
					m_Objectives[j] = GUILayout.TextField(m_Objectives[j]);
					instance.m_ObjectivePanel.m_ObjsTexts[j].text = m_Objectives[j];
					instance.m_ObjectivePanel.SetObjState(j, 1, 0, is_dirty: false, EObjectiveImportance.Main);
				}
			}
			break;
		}
		case EPanel.Joy:
			OnDrawJoy();
			break;
		case EPanel.MapVis:
			OnDrawMapVis();
			break;
		case EPanel.AudioVideo:
		{
			GUILayout.Box("Video");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Resolution:");
			m_ResolutionListbox.Draw(GUILayout.Width(200f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			m_Fullscreen = GUILayout.Toggle(m_Fullscreen, "Fullscreen");
			if (GUILayout.Button("Apply"))
			{
				int selectedElementIdx = m_ResolutionListbox.GetSelectedElementIdx();
				Screen.SetResolution(Screen.resolutions[selectedElementIdx].width, Screen.resolutions[selectedElementIdx].height, m_Fullscreen, Screen.resolutions[selectedElementIdx].refreshRate);
			}
			CFGOptions.Graphics.VSync = GUILayout.Toggle(CFGOptions.Graphics.VSync, "VSync");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Textures Quality:");
			CFGOptions.Graphics.TexturesQuality = GUILayout.Toolbar(CFGOptions.Graphics.TexturesQuality, new string[6] { "High", "Medium", "Low", "dev 1/8", "dev 1/16", "dev 1/32" }, GUILayout.MaxWidth(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			CFGOptions.Graphics.Aniso = GUILayout.Toggle(CFGOptions.Graphics.Aniso, "Aniso");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Shadows Quality:");
			m_ShadowsQuality = GUILayout.Toolbar(m_ShadowsQuality, new string[4] { "Very High", "High", "Medium", "Low" }, GUILayout.MaxWidth(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Shaders Quality:");
			CFGOptions.Graphics.ShadersQuality = GUILayout.Toolbar(CFGOptions.Graphics.ShadersQuality, new string[3] { "High", "Medium", "Low" }, GUILayout.MaxWidth(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Post-process Quality:");
			CFGOptions.Graphics.PostProcessing = GUILayout.Toolbar(CFGOptions.Graphics.PostProcessing, new string[3] { "High", "Medium", "Low" }, GUILayout.MaxWidth(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("World details:");
			CFGOptions.Graphics.WorldDetails = GUILayout.Toolbar(CFGOptions.Graphics.WorldDetails, new string[3] { "High", "Medium", "Low" }, GUILayout.MaxWidth(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			CFGRenderTargetUtility.m_ResizeRT = GUILayout.Toggle(CFGRenderTargetUtility.m_ResizeRT, "Resize render targets");
			CFGRenderTargetUtilityJS.m_ResizeRT = CFGRenderTargetUtility.m_ResizeRT;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Render sarget size: " + CFGRenderTargetUtility.m_RTHeight, GUILayout.Width(200f));
			CFGRenderTargetUtility.m_RTHeight = (int)GUILayout.HorizontalSlider(CFGRenderTargetUtility.m_RTHeight, 360f, 1080f, GUILayout.Width(400f));
			CFGRenderTargetUtilityJS.m_RTHeight = CFGRenderTargetUtility.m_RTHeight;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Apply & save options"))
			{
				QualitySettings.SetQualityLevel(m_ShadowsQuality, applyExpensiveChanges: true);
				CFGOptions.Graphics.ApplyToUnity();
				CFGOptions.Save();
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Dev Shadow Distance: " + QualitySettings.shadowDistance, GUILayout.Width(400f));
			QualitySettings.shadowDistance = GUILayout.HorizontalSlider(QualitySettings.shadowDistance, 0f, 200f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Dev Shader LOD: " + Shader.globalMaximumLOD + " (200 - 1000)", GUILayout.Width(400f));
			Shader.globalMaximumLOD = (int)GUILayout.HorizontalSlider(Shader.globalMaximumLOD, 200f, 1000f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Box("Audio");
			CFGAudioManager.Instance.m_MainMixer.GetFloat("MasterVolume", out var value);
			CFGAudioManager.Instance.m_MainMixer.GetFloat("MusicVolume", out var value2);
			CFGAudioManager.Instance.m_MainMixer.GetFloat("DialogsVolume", out var value3);
			CFGAudioManager.Instance.m_MainMixer.GetFloat("EnviroVolume", out var value4);
			CFGAudioManager.Instance.m_MainMixer.GetFloat("SFXVolume", out var value5);
			CFGAudioManager.Instance.m_MainMixer.GetFloat("InterfaceVolume", out var value6);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Master Volume: " + value + " dB", GUILayout.Width(400f));
			value = GUILayout.HorizontalSlider(value, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Music Volume: " + value2 + " dB", GUILayout.Width(400f));
			value2 = GUILayout.HorizontalSlider(value2, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Dialogs Volume: " + value3 + " dB", GUILayout.Width(400f));
			value3 = GUILayout.HorizontalSlider(value3, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Enviro Volume: " + value4 + " dB", GUILayout.Width(400f));
			value4 = GUILayout.HorizontalSlider(value4, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("SFX Volume: " + value5 + " dB", GUILayout.Width(400f));
			value5 = GUILayout.HorizontalSlider(value5, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Interface Volume: " + value6 + " dB", GUILayout.Width(400f));
			value6 = GUILayout.HorizontalSlider(value6, -80f, 20f, GUILayout.Width(400f));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			CFGAudioManager.Instance.m_MainMixer.SetFloat("MasterVolume", value);
			CFGAudioManager.Instance.m_MainMixer.SetFloat("MusicVolume", value2);
			CFGAudioManager.Instance.m_MainMixer.SetFloat("DialogsVolume", value3);
			CFGAudioManager.Instance.m_MainMixer.SetFloat("EnviroVolume", value4);
			CFGAudioManager.Instance.m_MainMixer.SetFloat("SFXVolume", value5);
			CFGAudioManager.Instance.m_MainMixer.SetFloat("InterfaceVolume", value6);
			GUILayout.FlexibleSpace();
			break;
		}
		case EPanel.Achievements:
			DrawAchievements();
			break;
		case EPanel.Camera:
		{
			CFGCamera component2 = Camera.main.GetComponent<CFGCamera>();
			if (!(component2 == null))
			{
				GUILayout.Label("Target point: " + component2.CurrentTargetPoint);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Angle H: " + component2.CurrentAngleH, GUILayout.Width(400f));
				component2.CurrentAngleH = GUILayout.HorizontalSlider(component2.CurrentAngleH, -360f, 360f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					component2.CurrentAngleH = component2.m_InitAngleH;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Rotate H in 1.0 sec to: ", GUILayout.Width(400f));
				m_CamDestRotH = GUILayout.TextField(m_CamDestRotH, GUILayout.Width(120f));
				if (GUILayout.Button("Rotate"))
				{
					float result = 0f;
					float.TryParse(m_CamDestRotH, out result);
					component2.SetRotation(result, 1f);
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Angle V: " + component2.CurrentAngleV, GUILayout.Width(400f));
				component2.CurrentAngleV = GUILayout.HorizontalSlider(component2.CurrentAngleV, 0f, 90f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					component2.CurrentAngleV = component2.m_InitAngleV;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Distance: " + component2.DestDistance, GUILayout.Width(400f));
				component2.DestDistance = GUILayout.HorizontalSlider(component2.DestDistance, 0f, 50f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					component2.DestDistance = component2.m_InitDistance;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				if (GUILayout.Button("Save as START", GUILayout.Width(150f)))
				{
					m_CameraStartPos = component2.CurrentTargetPoint;
					m_CameraStartAngleH = component2.CurrentAngleH;
					m_CameraStartAngleV = component2.CurrentAngleV;
					m_CameraStartDist = component2.DestDistance;
					m_CameraStartFOV = Camera.main.fieldOfView;
				}
				if (GUILayout.Button("Load START", GUILayout.Width(150f)))
				{
					component2.CurrentTargetPoint = m_CameraStartPos;
					component2.CurrentAngleH = m_CameraStartAngleH;
					component2.CurrentAngleV = m_CameraStartAngleV;
					component2.DestDistance = m_CameraStartDist;
					Camera.main.fieldOfView = m_CameraStartFOV;
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				if (GUILayout.Button("Save as END", GUILayout.Width(150f)))
				{
					m_CameraEndPos = component2.CurrentTargetPoint;
					m_CameraEndAngleH = component2.CurrentAngleH;
					m_CameraEndAngleV = component2.CurrentAngleV;
					m_CameraEndDist = component2.DestDistance;
					m_CameraEndFOV = Camera.main.fieldOfView;
				}
				if (GUILayout.Button("Load END", GUILayout.Width(150f)))
				{
					component2.CurrentTargetPoint = m_CameraEndPos;
					component2.CurrentAngleH = m_CameraEndAngleH;
					component2.CurrentAngleV = m_CameraEndAngleV;
					component2.DestDistance = m_CameraEndDist;
					Camera.main.fieldOfView = m_CameraEndFOV;
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Duration: " + m_CameraPlayDuration, GUILayout.Width(400f));
				m_CameraPlayDuration = GUILayout.HorizontalSlider(m_CameraPlayDuration, 0.1f, 30f, GUILayout.Width(400f));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("FOV: " + Camera.main.fieldOfView, GUILayout.Width(400f));
				Camera.main.fieldOfView = GUILayout.HorizontalSlider(Camera.main.fieldOfView, 1f, 180f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					Camera.main.fieldOfView = m_CameraInitialFOV;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("NEAR CLIPPING: " + Camera.main.nearClipPlane, GUILayout.Width(400f));
				Camera.main.nearClipPlane = GUILayout.HorizontalSlider(Camera.main.nearClipPlane, 0.01f, 180f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					Camera.main.nearClipPlane = m_CameraInitialNearClipPlane;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("FAR CLIPPING: " + Camera.main.farClipPlane, GUILayout.Width(400f));
				Camera.main.farClipPlane = GUILayout.HorizontalSlider(Camera.main.farClipPlane, 1f, 500f, GUILayout.Width(400f));
				if (GUILayout.Button("Reset"))
				{
					Camera.main.farClipPlane = m_CameraInitialFarClipPlane;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				m_CameraPlaySmooth = GUILayout.Toggle(m_CameraPlaySmooth, "Smoothing");
				if (GUILayout.Button("Play"))
				{
					m_CameraPlayTime = 0f;
				}
				if (m_CameraPlayTime >= 0f)
				{
					GUILayout.HorizontalSlider(m_CameraPlayTime, 0f, m_CameraPlayDuration, GUILayout.Width(400f));
				}
				GUILayout.Space(40f);
				ShowLockStatus("Selection manager's lock", component.LockStatus);
				ShowLockStatus("Camera's lock", component2.LockStatus);
			}
			break;
		}
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUI.color = Color.red;
		GUILayout.Label(m_ErrorText);
		GUI.color = Color.white;
		if (GUILayout.Button("Open ShadowMap viewer"))
		{
			OpenShadowMapViewer();
		}
		if (GUILayout.Button("Close", GUILayout.ExpandWidth(expand: false)))
		{
			OnButtonClose();
		}
		GUILayout.EndHorizontal();
		GUI.DragWindow(new Rect(0f, 0f, m_MainWindowRect.width, 20f));
	}

	private void DrawAchievGroup(int first, int last)
	{
		CFGAchievementManager instance = CFGSingleton<CFGAchievementManager>.Instance;
		GUILayout.BeginVertical();
		for (int i = first; i <= last; i++)
		{
			int num = i;
			EAchievement eAchievement = (EAchievement)num;
			GUILayout.Label(eAchievement.ToString());
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		for (int j = first; j <= last; j++)
		{
			int num2 = j;
			EAchievement key = (EAchievement)num2;
			GUILayout.Label(instance.m_Achievements[key].m_Name);
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		for (int k = first; k <= last; k++)
		{
			int num3 = k;
			EAchievement key2 = (EAchievement)num3;
			GUILayout.Label(instance.m_Achievements[key2].m_Description);
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		for (int l = first; l <= last; l++)
		{
			int num4 = l;
			EAchievement eAchievement2 = (EAchievement)num4;
			if (instance.m_Achievements[eAchievement2].m_Achieved)
			{
				GUILayout.Label("unlocked");
			}
			else if (GUILayout.Button("unlock"))
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(eAchievement2);
			}
		}
		GUILayout.EndVertical();
		GUILayout.Space(50f);
	}

	private void DrawAchievements()
	{
		CFGAchievementManager instance = CFGSingleton<CFGAchievementManager>.Instance;
		GUILayout.BeginHorizontal();
		DrawAchievGroup(1, 18);
		DrawAchievGroup(19, 35);
		for (int i = 0; i < 2; i++)
		{
			int num = 18;
			if (i == 1)
			{
				num = 17;
			}
		}
		GUILayout.BeginVertical(GUILayout.Width(400f));
		GUILayout.Label("Current progress");
		GUILayout.Label("A19 active: " + CFGAchievmentTracker.EqualizationUsedInFirstTurn);
		GUILayout.Label("Used chars: " + CFGAchievmentTracker.UsedCharCnt);
		GUILayout.Label("Used items: " + CFGAchievmentTracker.UsedItemCnt);
		GUILayout.Label("Shots (total): " + CFGAchievmentTracker.TotalShots);
		GUILayout.Label("Shots (blind): " + CFGAchievmentTracker.BLINDShots);
		GUILayout.Label("Shots (rusty): " + CFGAchievmentTracker.RustyShots);
		GUILayout.Label("Shots (cth100): " + CFGAchievmentTracker.CTH100Shots);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(50f);
		if (GUILayout.Button("[DEV] reset all achievements on steam"))
		{
			instance.DEV_ResetAll();
		}
	}

	private void OpenShadowMapViewer()
	{
		if (m_ShadowMapVis == null)
		{
			string text = "Prefabs/ShadowMapVis";
			UnityEngine.Object @object = Resources.Load(text);
			if (@object == null)
			{
				Debug.LogError("Failed to load Shadow Map Vis prefab: " + text);
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(@object) as GameObject;
			if (gameObject == null)
			{
				Debug.LogError("Failed to instantiate Shadow Map Vis prefab!");
				return;
			}
			m_ShadowMapVis = gameObject.GetComponent<CFGCellShadowMapVis>();
			if (m_ShadowMapVis == null)
			{
				Debug.LogError("Failed to get shadow map vis object from: !" + gameObject);
				return;
			}
		}
		m_ShadowMapVis.Activate();
		OnButtonClose();
	}

	private void OnDraw_Keys()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("Keys");
		m_KEY_ScrollPos = GUILayout.BeginScrollView(m_KEY_ScrollPos, GUILayout.Width(220f), GUILayout.Height(450f));
		EActionCommand[] array = Enum.GetValues(typeof(EActionCommand)) as EActionCommand[];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0 && GUILayout.Button(array[i].ToString()))
			{
				m_KEY_Selected = array[i];
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Selected Key: " + m_KEY_Selected);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		if (m_KEY_Selected != 0)
		{
			CFGActionItem item = CFGInput.GetItem(m_KEY_Selected);
			if (item != null)
			{
				CheckKey("Key A: ", item.KeyA);
				CheckKey("Key B: ", item.KeyB);
			}
		}
		GUILayout.Space(120f);
		if (GUILayout.Button("Reset keys to hard-coded"))
		{
			CFGInput.Reset_ToHardCoded();
		}
		if (GUILayout.Button("Reset keys to defaults (from keys.tsv)"))
		{
			CFGInput.Reset_ToDefaults_GD();
		}
		GUILayout.Space(30f);
		GUILayout.Label("Please remember to save the key configuaration (there is no AUTO-save)!");
		GUILayout.Space(30f);
		if (GUILayout.Button("Save key configuration"))
		{
			CFGInput.SaveINI();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private bool CheckKey(string label, CFGKey kte)
	{
		bool result = false;
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(200f));
		if (GUILayout.Button(kte.ToText(), GUILayout.Width(200f)))
		{
			result = true;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		return result;
	}

	private void ShowLockStatus(string Label, ELockReason Status)
	{
		string text = Label + ": ";
		if (Status == ELockReason.NoLock)
		{
			text += "No Lock";
		}
		else
		{
			foreach (int value in Enum.GetValues(typeof(ELockReason)))
			{
				if (value != 0 && ((uint)Status & (uint)value) == (uint)value)
				{
					text = text + ((ELockReason)value).ToString() + " ";
				}
			}
		}
		GUILayout.Label(text);
	}

	private void OnDraw_Usable(CFGUsableObject uobj)
	{
		if (uobj == null)
		{
			return;
		}
		List<CFGCell> activatorCells = uobj.GetActivatorCells();
		CFGCell cell = CFGCellMap.GetCell(uobj.transform.position);
		if (cell == null)
		{
			GUILayout.Label("Invalid object position");
			return;
		}
		GUILayout.Label("Main Cell: " + cell);
		if (activatorCells == null || activatorCells.Count == 0)
		{
			GUILayout.Label("Activators: NONE");
			return;
		}
		GUILayout.Label("Activators: ");
		foreach (CFGCell item in activatorCells)
		{
			if (item != null)
			{
				GUILayout.Label(" " + item.ToString());
			}
		}
	}

	private void OnDraw_Location(CFGLocationObject lobj)
	{
		if (lobj == null)
		{
			return;
		}
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("State: " + lobj.State);
		string[] names = Enum.GetNames(typeof(ELocationState));
		if (names != null && names.Length > 0)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("New State: ");
			m_LOC_NewState = (ELocationState)GUILayout.SelectionGrid((int)m_LOC_NewState, names, 3);
			GUILayout.Label(" ");
			if (GUILayout.Button("Apply"))
			{
				lobj.SetState(m_LOC_NewState);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void OnDraw_Character(CFGCharacter character)
	{
		if (character == null)
		{
			return;
		}
		if (character.Owner == null)
		{
			GUILayout.Label("Owner: None");
		}
		else
		{
			GUILayout.Label("Owner: " + character.Owner.name);
		}
		if (character.CurrentCell == null)
		{
			GUILayout.Label("Tile: None");
		}
		else
		{
			character.CurrentCell.DecodePosition(out var PosX, out var PosZ, out var Floor);
			GUILayout.Label("Tile:  X = " + PosX + " Z = " + PosZ + " Floor = " + Floor);
		}
		GUILayout.Label("Rotation: " + character.GetBestCoverRotation());
		GUILayout.Label("CellObject: " + ((!(character.m_CurrentCellObject != null)) ? string.Empty : character.m_CurrentCellObject.name));
		GUILayout.Label("CellObject(Interior): " + ((!(character.m_CurrentCellObjectInside != null)) ? string.Empty : character.m_CurrentCellObjectInside.name));
		GUILayout.Label("AI team: " + character.AiTeam);
		GUILayout.Label("Action: " + character.CurrentAction);
		GUILayout.Label("HP: " + character.Hp + "/" + character.MaxHp + ((!character.IsAlive) ? " (dead)" : " (alive)"));
		GUILayout.Label("Luck: " + character.Luck + "/" + character.MaxLuck);
		GUILayout.Label(string.Concat("AI State: ", character.AIState, " Suspicion: (", character.Suspicion, " / ", character.CharacterData.SuspicionLimit, ") Subdued: (", character.CharacterData.SubduedCount, " / ", character.CharacterData.SubduedLimit, ")"));
		GUILayout.Label("Total Heat: " + character.CharacterData.TotalHeat);
		GUILayout.Label("Eavesdrop roll: " + character.EavesdropRoll);
		GUILayout.Label("Inventory:");
		string text = ((character.FirstWeapon == null || character.FirstWeapon.m_Definition == null) ? "NONE" : character.FirstWeapon.m_Definition.ItemID);
		string text2 = ((character.SecondWeapon == null || character.SecondWeapon.m_Definition == null) ? "NONE" : character.SecondWeapon.m_Definition.ItemID);
		if (character.CurrentWeapon == character.SecondWeapon)
		{
			text2 += " (*)";
		}
		else
		{
			text += " (*)";
		}
		GUILayout.Label("    Weapon 1: " + text);
		GUILayout.Label("    Weapon 2: " + text2);
		GUILayout.Label("Outline state: " + character.OutlineState);
		GUILayout.Label("Best detection type: " + character.BestDetectionType);
		string text3 = "Self Shadow Ratio: " + character.SelfShadowRatio + " In Shadow?: " + character.IsInShadow + " ShadowCloaked = " + character.IsShadowCloaked;
		GUILayout.Label(text3);
		GUILayout.Label("Abilities:");
		if (character.Abilities == null)
		{
			GUILayout.Label("    None!");
		}
		else
		{
			foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in character.Abilities)
			{
				GUILayout.Label("    " + ability.Key.ToString() + " Source: " + ability.Value.Source);
			}
		}
		GUILayout.Label("Buffs:");
		if (character.Buffs == null)
		{
			GUILayout.Label("    None");
			return;
		}
		foreach (CFGBuff buff in character.Buffs)
		{
			if (buff.m_Def != null)
			{
				GUILayout.Label("    " + buff.m_Def.BuffID);
			}
		}
	}

	private void AddItemAs(EItemSlot slot, CFGCharacterData chd)
	{
		if (string.IsNullOrEmpty(m_GiveItemID) || chd == null)
		{
			return;
		}
		CFGDef_Item itemDefinition = CFGStaticDataContainer.GetItemDefinition(m_GiveItemID);
		if (itemDefinition == null)
		{
			Debug.LogError("Invalid item: " + m_GiveItemID);
			return;
		}
		EItemSlot eItemSlot = slot;
		if ((eItemSlot == EItemSlot.Weapon1 || eItemSlot == EItemSlot.Weapon2) && itemDefinition.ItemType != CFGDef_Item.EItemType.Weapon)
		{
			Debug.LogError("Wrong item type!");
			return;
		}
		chd.EquipItem(slot, m_GiveItemID);
		eItemSlot = slot;
		if ((eItemSlot == EItemSlot.Weapon1 || eItemSlot == EItemSlot.Weapon2) && (bool)chd.CurrentModel)
		{
			chd.CurrentModel.EquipWeapon();
		}
	}

	private void OnDraw_Characters()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		int characterCount = CFGCharacterList.GetCharacterCount();
		Color backgroundColor = GUI.backgroundColor;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Chars");
		GUILayout.Space(20f);
		GUILayout.Label("Active team");
		int currentTeamID = CFGCharacterList.CurrentTeamID;
		currentTeamID = GUILayout.SelectionGrid(currentTeamID, m_TeamIDStr, 2);
		if (currentTeamID != CFGCharacterList.CurrentTeamID)
		{
			CFGCharacterList.SetActiveTeam(currentTeamID);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		m_CharsScrollPos = GUILayout.BeginScrollView(m_CharsScrollPos, GUILayout.Width(220f), GUILayout.Height(650f));
		for (int i = 0; i < characterCount; i++)
		{
			CFGCharacterData characterData = CFGCharacterList.GetCharacterData(i);
			string text = "#" + i;
			if (characterData == null)
			{
				GUI.enabled = false;
				text += " Null";
				GUILayout.Button(base.name);
				GUI.enabled = true;
				continue;
			}
			if (characterData.Definition != null)
			{
				text = text + " " + characterData.Definition.NameID;
			}
			if (characterData.IsStateSet(ECharacterStateFlag.TempTactical))
			{
				text += " (Temp)";
			}
			if (characterData.PositionInTeam > -1)
			{
				string text2 = text;
				text = text2 + "(Team " + characterData.TeamID + " @ #" + characterData.PositionInTeam.ToString() + ")";
			}
			if (characterData.IsDead)
			{
				text += " (Dead)";
			}
			if (i == m_SelectedChar)
			{
				GUI.backgroundColor = Color.white;
			}
			else
			{
				GUI.backgroundColor = backgroundColor;
			}
			if (GUILayout.Button(text))
			{
				m_SelectedChar = i;
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		if (m_SelectedChar >= 0)
		{
			GUILayout.Label("Char description #" + m_SelectedChar);
			CFGCharacterData characterData2 = CFGCharacterList.GetCharacterData(m_SelectedChar);
			if (characterData2 == null)
			{
				GUILayout.Label("Null Slot!");
			}
			else
			{
				string text3 = characterData2.Definition.NameID;
				if (characterData2.Invulnerable)
				{
					text3 += " (Invul)";
				}
				if (characterData2.IsStateSet(ECharacterStateFlag.TempTactical))
				{
					text3 += " Temporary";
				}
				if (characterData2.PositionInTeam > -1)
				{
					string text2 = text3;
					text3 = text2 + " In Team " + characterData2.TeamID + " at pos #" + characterData2.PositionInTeam;
				}
				if (characterData2.IsDead)
				{
					text3 += " KAPUT";
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("NameID: " + text3);
				if (characterData2.PositionInTeam > -1)
				{
					if (GUILayout.Button("Fire!"))
					{
						CFGCharacterList.RemoveFromTeam(characterData2.Definition.NameID, bRemoveEquipment: false);
					}
				}
				else if (GUILayout.Button("Hire!"))
				{
					CFGCharacterList.AssignToTeam(characterData2);
				}
				if (characterData2.PositionInTeam > 0 && characterData2.TeamID == CFGCharacterList.CurrentTeamID && GUILayout.Button("Set As Leader"))
				{
					CFGCharacterList.MoveCharToTeamTop(characterData2.Definition.NameID);
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				bool flag = characterData2.HasBuff("critical_buff");
				GUILayout.Label("Critical status: " + flag);
				if (GUILayout.Button("Change"))
				{
					if (flag)
					{
						characterData2.RemBuff("critical_buff");
					}
					else
					{
						characterData2.AddBuff("critical_buff", EBuffSource.Permanent);
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				m_GiveItemID = GUILayout.TextField(m_GiveItemID, GUILayout.Width(200f));
				if (GUILayout.Button("Add @Weapon1", GUILayout.Width(150f)))
				{
					AddItemAs(EItemSlot.Weapon1, characterData2);
				}
				if (GUILayout.Button("Add @Weapon2", GUILayout.Width(150f)))
				{
					AddItemAs(EItemSlot.Weapon2, characterData2);
				}
				if (GUILayout.Button("Add @Item1", GUILayout.Width(150f)))
				{
					AddItemAs(EItemSlot.Item1, characterData2);
				}
				if (GUILayout.Button("Add @Item2", GUILayout.Width(150f)))
				{
					AddItemAs(EItemSlot.Item2, characterData2);
				}
				if (GUILayout.Button("Add @Talisman", GUILayout.Width(150f)))
				{
					AddItemAs(EItemSlot.Talisman, characterData2);
				}
				GUILayout.EndHorizontal();
				GUILayout.Label("Aim: " + characterData2.Aim + " Buffed: " + characterData2.BuffedAim);
				GUILayout.Label("Defence: " + characterData2.Defence + " Buffed: " + characterData2.BuffedDefense);
				GUILayout.Label("HP: " + characterData2.Hp + " MaxHP: " + characterData2.MaxHp + " Buffed: " + characterData2.BuffedMaxHP);
				GUILayout.Label("Movement: " + characterData2.Movement + " Buffed: " + characterData2.BuffedMovement);
				GUILayout.Label("Sight: " + characterData2.Sight + " Buffed: " + characterData2.BuffedSight);
				GUILayout.Label("Luck: " + characterData2.Luck + " Max: " + characterData2.MaxLuck);
				GUILayout.Label("Decay Level: Base :" + characterData2.DecayLevel + " Total: " + characterData2.TotalDecayLevel);
				GUILayout.Label("Weapon 1: " + characterData2.Weapon1);
				GUILayout.Label("Weapon 2: " + characterData2.Weapon2);
				GUILayout.Label("Item 1: " + characterData2.Item1);
				GUILayout.Label("Item 2: " + characterData2.Item2);
				GUILayout.Label("Talisman: " + characterData2.Talisman);
				text3 = string.Empty;
				foreach (KeyValuePair<ETurnAction, CAbilityInfo> ability in characterData2.Abilities)
				{
					text3 = text3 + ability.Key.ToString() + ", ";
				}
				if (text3.Length < 1)
				{
					text3 = "None";
				}
				GUILayout.Label("Abilities: " + text3);
				HashSet<string> forbiddenBuffs = characterData2.ForbiddenBuffs;
				text3 = "Forbidden buffs: ";
				if (forbiddenBuffs != null && forbiddenBuffs.Count > 0)
				{
					foreach (string item in forbiddenBuffs)
					{
						text3 = text3 + item + ", ";
					}
				}
				else
				{
					text3 += "None";
				}
				GUILayout.Label(text3);
				GUILayout.Label("Buffs:");
				Dictionary<string, CFGBuff> buffs = characterData2.Buffs;
				if (buffs.Count == 0)
				{
					GUILayout.Label("None");
				}
				else
				{
					foreach (KeyValuePair<string, CFGBuff> item2 in buffs)
					{
						if (item2.Value != null)
						{
							GUILayout.Label(" " + item2.Key + " " + item2.Value.m_Duration);
						}
					}
				}
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void MakeSpawningWindow(int id)
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (!(component == null))
		{
			GUILayout.Label("Click Shift+LMB on tile to spawn:");
			CFGCell cellUnderCursor = component.CellUnderCursor;
			bool flag = cellUnderCursor?.CanStandOnThisTile(can_stand_now: true) ?? false;
			GUI.color = ((!flag) ? Color.red : Color.green);
			if (cellUnderCursor != null)
			{
				GUILayout.Label("Tile: " + cellUnderCursor.PositionX + ", " + cellUnderCursor.PositionZ + " (" + ((EFloorLevelType)cellUnderCursor.Floor).ToString() + ")");
			}
			else
			{
				GUILayout.Label("No tile under cursor");
			}
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel"))
			{
				m_IsInSpawningMode = false;
			}
			if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift) && flag)
			{
				CFGCharacterList.SpawnCharacter(m_SelectedCharacterId, m_SelectedOwner, cellUnderCursor.WorldPosition, Quaternion.identity, bSingleInstance: false);
				m_IsInSpawningMode = false;
			}
			GUI.DragWindow(new Rect(0f, 0f, m_SpawningWindowRect.width, 20f));
		}
	}

	private void OnButtonClose()
	{
		m_ErrorText = string.Empty;
		m_DialogId = string.Empty;
		Deactivate();
	}

	private CFGCell GetCell(string s_x, string s_z, string s_floor)
	{
		if (!int.TryParse(s_x, out var result) || !int.TryParse(s_z, out var result2) || !int.TryParse(s_floor, out var result3))
		{
			return null;
		}
		return CFGCellMap.GetCell(result, result2, result3);
	}

	private static void CellPositionField(ref int cellZ, ref int cellX, ref int cellFloor)
	{
		string empty = string.Empty;
		empty = GUILayout.TextField(cellZ.ToString(), 2, GUILayout.Width(30f));
		if (!int.TryParse(empty, out cellZ))
		{
			cellZ = 0;
		}
		empty = GUILayout.TextField(cellX.ToString(), 2, GUILayout.Width(30f));
		if (!int.TryParse(empty, out cellX))
		{
			cellX = 0;
		}
		empty = GUILayout.TextField(cellFloor.ToString(), 1, GUILayout.Width(20f));
		if (!int.TryParse(empty, out cellFloor))
		{
			cellFloor = 0;
		}
	}

	private void OnDrawLoS()
	{
		CFGCell cellUnderCursor = Camera.main.GetComponent<CFGSelectionManager>().CellUnderCursor;
		if (cellUnderCursor != null)
		{
			int PosZ = 0;
			int PosX = 0;
			int Floor = 0;
			cellUnderCursor.DecodePosition(out PosX, out PosZ, out Floor);
			GUILayout.Label("Tile: Axis Z = " + PosZ + ", Axis X = " + PosX + " ( Floor: " + Floor + ")");
		}
		else
		{
			GUILayout.Label("Tile: none");
		}
		GUILayout.Label("Press L_Shift+1 or R_Alt+1 to select as start tile");
		GUILayout.Label("Press L_Shift+2 or R_Alt+2 to select as end tile");
		if (cellUnderCursor != null)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1) && (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftShift)))
			{
				cellUnderCursor.DecodePosition(out startX, out startZ, out startFloor);
				m_LoSCalculated = false;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2) && (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftShift)))
			{
				cellUnderCursor.DecodePosition(out endX, out endZ, out endFloor);
				m_LoSCalculated = false;
			}
		}
		m_Draw_StartPoint = new Vector3((float)startX + 0.5f, (float)startFloor * 2.5f + 1.25f, (float)startZ + 0.5f);
		m_Draw_EndPoint = new Vector3((float)endX + 0.5f, (float)endFloor * 2.5f + 1.25f, (float)endZ + 0.5f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Start: (RED)");
		CellPositionField(ref startZ, ref startX, ref startFloor);
		CFGCell cell = CFGCellMap.GetCell(startZ, startX, startFloor);
		GUI.color = ((cell == null) ? Color.red : Color.green);
		GUILayout.Label((cell == null) ? "wrong tile coords" : "OK!");
		GUI.color = Color.white;
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("End: (BLUE)");
		CellPositionField(ref endZ, ref endX, ref endFloor);
		CFGCell cell2 = CFGCellMap.GetCell(endZ, endX, endFloor);
		GUI.color = ((cell2 == null) ? Color.red : Color.green);
		GUILayout.Label((cell2 == null) ? "wrong tile coords" : "OK!");
		GUI.color = Color.white;
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUI.enabled = cell != null && cell2 != null;
		bool flag = GUILayout.Button("Check LoS", GUILayout.Width(150f));
		GUI.enabled = true;
		if (flag)
		{
			los = CFGCellMap.GetLineOfSightAutoSideSteps(null, null, cell, cell2, 10000);
			m_LoSCalculated = true;
			if (los != 0)
			{
				m_LastIntersection = CFGCellMap.LastIntersection;
			}
			ECoverType targetCover = CFGCellMap.GetTargetCover(cell, cell2);
			m_LoSResult = string.Concat("Line of sight hit point: ", los, "\nTarget's cover: ", targetCover);
		}
		GUI.enabled = cell != null && cell2 != null;
		flag = GUILayout.Button("Find path", GUILayout.Width(150f));
		GUI.enabled = true;
		if (flag)
		{
			CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
			if (component != null && component.SelectedCharacter != null)
			{
				NavigationComponent component2 = component.SelectedCharacter.GetComponent<NavigationComponent>();
				if (component2 != null)
				{
					NavGoal_At navGoal_At = new NavGoal_At(cell2);
					LinkedList<CFGCell> path = new LinkedList<CFGCell>();
					if (component2.GeneratePath(cell, new NavGoalEvaluator[1] { navGoal_At }, out path))
					{
						m_LoSResult = "path found";
					}
					else
					{
						m_LoSResult = "path not found";
					}
				}
			}
		}
		GUI.enabled = cell != null;
		if (GUILayout.Button("Translate Selected char to StartPos", GUILayout.Width(300f)))
		{
			CFGSelectionManager component3 = Camera.main.GetComponent<CFGSelectionManager>();
			if (component3 != null && component3.SelectedCharacter != null)
			{
				component3.SelectedCharacter.Translate(cell.WorldPosition, Quaternion.identity);
			}
		}
		GUI.enabled = true;
		GUILayout.Label(m_LoSResult);
	}

	private void OnDrawAI()
	{
		CFGAiOwner aiOwner = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.AiOwner;
		if (aiOwner == null)
		{
			return;
		}
		aiOwner.m_AiDebugging = GUILayout.Toggle(aiOwner.m_AiDebugging, "AI debugging");
		CFGCheats.AllCharactersVisible = aiOwner.m_AiDebugging;
		if (!aiOwner.m_AiDebugging)
		{
			return;
		}
		if (CFGSingletonResourcePrefab<CFGTurnManager>.Instance.CurrentOwner != aiOwner)
		{
			GUILayout.Label("Player turn");
			return;
		}
		GUILayout.Label("AI turn");
		CFGCharacter currentCharacter = aiOwner.CurrentCharacter;
		if (currentCharacter == null)
		{
			GUILayout.Label("Current character: none");
			return;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Current character: ");
		if (GUILayout.Button(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(currentCharacter.NameId)))
		{
			CFGCamera component = Camera.main.GetComponent<CFGCamera>();
			if (component != null)
			{
				component.ChangeFocus(currentCharacter);
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Label("AP: " + currentCharacter.ActionPoints);
		GUILayout.Label("Team: " + currentCharacter.AiTeam);
		CFGAiPreset aiTeamPreset = aiOwner.GetAiTeamPreset(currentCharacter.AiTeam);
		if (aiTeamPreset == null)
		{
			GUILayout.Label("No AI preset for this team! Cannot debug.");
			if (GUILayout.Button("SKIP", GUILayout.Width(150f)))
			{
				aiOwner.m_DebugOnlySimulate = false;
				aiOwner.m_DebugHoldAiUpdate = false;
			}
			return;
		}
		bool flag = aiOwner.AiTeamIsRoaming(currentCharacter.AiTeam);
		Transform transform = ((!flag) ? aiOwner.GetAiTeamObjective(currentCharacter.AiTeam) : aiOwner.m_LastRoamingTarget);
		if (transform != null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("AI team objective: ");
			if (GUILayout.Button(transform.gameObject.name))
			{
				CFGCamera component2 = Camera.main.GetComponent<CFGCamera>();
				if (component2 != null)
				{
					component2.ChangeFocus(transform.position, component2.CurrentDistance, 0.5f);
				}
			}
			if (flag)
			{
				GUILayout.Label("(roaming)");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Label("    - Is near objective: " + ((!CFGAiOwner.IsCharacterNearObjective(currentCharacter, transform, aiTeamPreset.m_ObjectiveRadius, aiTeamPreset.m_ObjectiveInLoSRequired)) ? "NO" : "YES"));
		}
		else
		{
			GUILayout.Label("AI team objective: none");
		}
		GUILayout.Label("AI team preset: " + aiTeamPreset.name);
		GUILayout.Label("    - Movement mode: " + aiTeamPreset.m_MovementMode);
		GUILayout.Label("    - AP reserved for move: " + aiTeamPreset.m_ApReservedForMove);
		GUILayout.Label("    - Targeting mode: " + aiTeamPreset.m_TargetingMode);
		GUILayout.Label("Enemies in sight: " + currentCharacter.VisibleEnemies.Count + ((currentCharacter.VisibleEnemies.Count <= 0) ? " (not in fight)" : " (fight)"));
		if (flag && currentCharacter.ActionPoints == 2)
		{
			GUILayout.Label("AI has roaming enabled, cannot debug due to random chance.");
			if (GUILayout.Button("SKIP", GUILayout.Width(150f)))
			{
				aiOwner.m_DebugOnlySimulate = false;
				aiOwner.m_DebugHoldAiUpdate = false;
			}
			return;
		}
		if (currentCharacter.CurrentAction != ETurnAction.None)
		{
			GUILayout.Label("Doing action...");
			return;
		}
		if (GUILayout.Button("SIMULATE", GUILayout.Width(150f)))
		{
			aiOwner.m_DebugOnlySimulate = true;
			aiOwner.m_DebugHoldAiUpdate = false;
			aiOwner.m_DebugShowCellsEval = true;
		}
		if (aiOwner.m_DebugSimulationResult != string.Empty)
		{
			GUILayout.Label("Decision taken:");
			GUILayout.Label(aiOwner.m_DebugSimulationResult);
			if (GUILayout.Button((!aiOwner.m_DebugShowCellsEval) ? "Show cells evaluation" : "Hide cells evaluation", GUILayout.Width(150f)))
			{
				aiOwner.m_DebugShowCellsEval = !aiOwner.m_DebugShowCellsEval;
			}
		}
		if (GUILayout.Button("STEP", GUILayout.Width(150f)))
		{
			aiOwner.m_DebugOnlySimulate = false;
			aiOwner.m_DebugHoldAiUpdate = false;
		}
	}

	private void OnDrawGizmos()
	{
		if (m_CurrentPanel != EPanel.LoS)
		{
			return;
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(m_Draw_StartPoint, 0.3f);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(m_Draw_EndPoint, 0.3f);
		if (m_LoSCalculated)
		{
			if (los != 0)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(m_LastIntersection, 0.3f);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawLine(m_Draw_StartPoint, m_Draw_EndPoint);
		}
	}

	private void OnDrawJoy()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Active profile: ");
		int gamepadProfile = CFGOptions.Input.GamepadProfile;
		gamepadProfile--;
		int num = GUILayout.SelectionGrid(gamepadProfile, m_StrJoyProfiles, 4);
		if (num != gamepadProfile)
		{
			CFGOptions.Input.GamepadProfile = num + 1;
		}
		GUILayout.Space(50f);
		GUILayout.Label("Base DeadZone: " + CFGOptions.Input.GamepadDZ + " Current: " + CFGInput.GamepadDeadZone);
		GUILayout.EndHorizontal();
		string[] joystickNames = Input.GetJoystickNames();
		string text = string.Empty;
		for (int i = 0; i < joystickNames.Length; i++)
		{
			string text2 = text;
			text = text2 + "#" + i + " " + joystickNames[i] + ", ";
		}
		text = text + " Currently Set: " + CFGJoyManager.GamePadType;
		GUILayout.Label("Joysticks [" + joystickNames.Length + "]: " + text);
		string text3 = string.Empty;
		foreach (int value in Enum.GetValues(typeof(EJoyButton)))
		{
			if (value != 0)
			{
				float num2 = CFGJoyManager.ReadAsButton((EJoyButton)value);
				if (num2 != 0f)
				{
					string text2 = text3;
					text3 = text2 + ((EJoyButton)value).ToString() + " = " + num2 + "\n";
				}
			}
		}
		GUILayout.Label("Axes/Buttons: ");
		GUILayout.Label(text3);
		text3 = string.Empty;
		for (int j = 0; j < 20; j++)
		{
			int num3 = 350;
			num3 += j;
			KeyCode keyCode = (KeyCode)num3;
			if (Input.GetKey(keyCode))
			{
				text3 = text3 + keyCode.ToString() + ", ";
			}
		}
		GUILayout.Label("RAW Buttons: " + text3);
		text3 = string.Empty;
		for (int k = 1; k <= 20; k++)
		{
			string text4 = "J1_A" + k;
			float axisRaw = Input.GetAxisRaw(text4);
			string text2 = text3;
			text3 = text2 + " #" + text4 + " " + k + " = " + axisRaw + ", ";
		}
		GUILayout.Label("RAW Axes: " + text3);
	}

	private bool HasLastSelectedCharChanged()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component == null)
		{
			return false;
		}
		if (component.SelectedCharacter == m_LastSelectedChar)
		{
			return false;
		}
		m_LastSelectedChar = component.SelectedCharacter;
		return true;
	}

	private void OnDrawAbilities()
	{
		if (allabs == null)
		{
			ETurnAction[] array = Enum.GetValues(typeof(ETurnAction)) as ETurnAction[];
			List<ETurnAction> list = new List<ETurnAction>();
			if (array != null && array.Length > 0)
			{
				ETurnAction[] array2 = array;
				foreach (ETurnAction eTurnAction in array2)
				{
					if (eTurnAction.IsStandard())
					{
						list.Add(eTurnAction);
					}
				}
				if (list.Count > 0)
				{
					allabsta = list.ToArray();
					allabs = new string[allabsta.Length];
					for (int j = 0; j < allabsta.Length; j++)
					{
						allabs[j] = allabsta[j].ToString();
					}
				}
			}
		}
		HasLastSelectedCharChanged();
		GUILayout.BeginHorizontal();
		if (m_LastSelectedChar == null)
		{
			GUILayout.Label("Please select a character");
			GUILayout.EndHorizontal();
			return;
		}
		GUILayout.Label(string.Concat("Selected character ", m_LastSelectedChar, " has the following abilities"));
		GUILayout.EndHorizontal();
		Dictionary<ETurnAction, CAbilityInfo> abilities = m_LastSelectedChar.Abilities;
		foreach (KeyValuePair<ETurnAction, CAbilityInfo> item in abilities)
		{
			GUILayout.Label(string.Concat("Ability: ", item.Key, " Source: ", item.Value.Source));
		}
		if (allabs == null)
		{
			return;
		}
		m_LastSelAbility = GUILayout.SelectionGrid(m_LastSelAbility, allabs, 5);
		if (GUILayout.Button("Add"))
		{
			ETurnAction eTurnAction2 = allabsta[m_LastSelAbility];
			if (m_LastSelectedChar.HaveAbility(eTurnAction2))
			{
				Debug.Log("Ability is already present");
			}
			else
			{
				m_LastSelectedChar.CharacterData.AddAbilityWithCard(eTurnAction2, EAbilitySource.FlowCode);
			}
		}
	}

	private void OnDrawCards()
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < 4; num++)
		{
			if (CFGCharacterList.GetTeamCharacter(num) != null)
			{
				num2 |= 1 << num;
			}
		}
		foreach (KeyValuePair<string, CFGDef_Card> card2 in CFGStaticDataContainer.GetCards())
		{
			bool flag = CFGInventory.IsCardCollected(card2.Key);
			bool flag2 = CFGInventory.IsCardInUse(card2.Key);
			string text = "Card: " + card2.Key + " Collected? " + flag + " In Use?: " + flag2;
			GUILayout.BeginHorizontal();
			GUILayout.Label(text);
			if (!flag)
			{
				if (GUILayout.Button("Collect"))
				{
					CFGInventory.CollectCard(card2.Key);
				}
			}
			else if (!flag2)
			{
				for (num = 0; num < 4; num++)
				{
					if ((num2 & (1 << num)) != 0 && GUILayout.Button(num + ">>"))
					{
						CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(num);
						CFGInventory.MoveCardToCharacter(teamCharacter.Definition.NameID, card2.Key, -1);
					}
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		Rect screenRect = new Rect(500f, 100f, 600f, 600f);
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Collect all cards"))
		{
			foreach (KeyValuePair<string, CFGDef_Card> card3 in CFGStaticDataContainer.GetCards())
			{
				bool flag3 = CFGInventory.IsCardCollected(card3.Key);
				bool flag4 = CFGInventory.IsCardInUse(card3.Key);
				if (!flag3 && !flag4)
				{
					CFGInventory.CollectCard(card3.Key);
				}
			}
		}
		if (GUILayout.Button("Unlock all slots"))
		{
			for (int i = 0; i < 4; i++)
			{
				CFGCharacterData teamCharacter2 = CFGCharacterList.GetTeamCharacter(i);
				if (teamCharacter2 != null)
				{
					teamCharacter2.UnlockedCardSlots = 5;
				}
			}
		}
		if (GUILayout.Button("Remove all cards"))
		{
			CFGInventory.ResetCards();
		}
		GUILayout.EndHorizontal();
		for (num = 0; num < 4; num++)
		{
			if ((num2 & (1 << num)) == 0)
			{
				continue;
			}
			CFGCharacterData teamCharacter3 = CFGCharacterList.GetTeamCharacter(num);
			string text2 = "Character #" + num + " " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharacter3.Definition.NameID) + " Slots: " + teamCharacter3.UnlockedCardSlots;
			GUILayout.BeginHorizontal();
			GUILayout.Label(text2);
			if (teamCharacter3.UnlockedCardSlots < 5 && GUILayout.Button("Slot+"))
			{
				teamCharacter3.UnlockedCardSlots = 5;
			}
			if (GUILayout.Button("Rem 10 Luck"))
			{
				teamCharacter3.SetLuck(teamCharacter3.Luck - 10, bAllowSplash: false);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Cards: ");
			for (int j = 0; j < 5; j++)
			{
				CFGDef_Card card = teamCharacter3.GetCard(j);
				if (card != null && GUILayout.Button(card.CardID + "<<"))
				{
					CFGInventory.MoveCardFromCharacter(teamCharacter3.Definition.NameID, j);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			text2 = "     Handbonus: " + teamCharacter3.CardHandBonus;
			GUILayout.Label(text2);
		}
		GUILayout.EndArea();
	}

	private void OnDrawBuffs()
	{
		if (m_buffs == null)
		{
			Dictionary<string, CFGDef_Buff> buffDefs = CFGStaticDataContainer.GetBuffDefs();
			if (buffDefs == null || buffDefs.Count == 0)
			{
				return;
			}
			m_buffs = new string[buffDefs.Count];
			if (m_buffs == null || m_buffs.Length == 0)
			{
				return;
			}
			List<string> list = new List<string>(buffDefs.Keys);
			if (list == null)
			{
				return;
			}
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				m_buffs[i] = list[i];
			}
		}
		m_buffs_selection = GUILayout.SelectionGrid(m_buffs_selection, m_buffs, 6);
		GUILayout.BeginHorizontal();
		if (CFGSingleton<CFGGame>.Instance.IsInGame())
		{
			if (GUILayout.Button("Add to selected character"))
			{
				CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
				if ((bool)component && component.SelectedCharacter != null && component.SelectedCharacter.CharacterData != null)
				{
					component.SelectedCharacter.CharacterData.AddBuff(m_buffs[m_buffs_selection], EBuffSource.Ability);
				}
				else
				{
					Debug.LogWarning("Failed to add buff - no selection");
				}
			}
		}
		else if (CFGSingleton<CFGGame>.Instance.IsInStrategic())
		{
			for (int j = 0; j < 4; j++)
			{
				CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(j);
				if (teamCharacter != null)
				{
					string text = "Add to: " + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharacter.Definition.NameID);
					if (GUILayout.Button(text))
					{
						teamCharacter.AddBuff(m_buffs[m_buffs_selection], EBuffSource.Ability);
					}
				}
			}
		}
		if (GUILayout.Button("Heal team's buffs"))
		{
			foreach (CFGCharacterData teamCharacters in CFGCharacterList.GetTeamCharactersList())
			{
				teamCharacters?.HealBuffs();
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private void OnDrawMapVis()
	{
		bool flag = CMVis.Instance.enabled;
		if (GUILayout.Button((!flag) ? "Show" : "Hide"))
		{
			if (flag)
			{
				CMVis.Instance.HideVisualization();
			}
			else
			{
				CMVis.Instance.ShowVisualization();
			}
			mapVisDirty = true;
		}
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Select All"))
		{
			CMVis.Instance.showCovers = true;
			CMVis.Instance.showFloors = true;
			CMVis.Instance.showInteriors = true;
			CMVis.Instance.showImpassables = true;
			CMVis.Instance.showBlockSights = true;
			CMVis.Instance.showBlockBullets = true;
			CMVis.Instance.showSwitches = true;
			CMVis.Instance.showTriggers = true;
			mapVisDirty = true;
		}
		if (GUILayout.Button("Deselect All"))
		{
			CMVis.Instance.showCovers = false;
			CMVis.Instance.showFloors = false;
			CMVis.Instance.showInteriors = false;
			CMVis.Instance.showImpassables = false;
			CMVis.Instance.showBlockSights = false;
			CMVis.Instance.showBlockBullets = false;
			CMVis.Instance.showSwitches = false;
			CMVis.Instance.showTriggers = false;
			mapVisDirty = true;
		}
		GUILayout.EndHorizontal();
		int num = CMVis.Instance.startFloor;
		int num2 = CMVis.Instance.endFloor;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Start Floor");
		GUILayout.Label(CMVis.Instance.startFloor.ToString());
		int value = (int)GUILayout.HorizontalSlider(CMVis.Instance.startFloor, 0f, 3f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("End Floor");
		GUILayout.Label(CMVis.Instance.endFloor.ToString());
		int max = (int)GUILayout.HorizontalSlider(CMVis.Instance.endFloor, 0f, 3f);
		GUILayout.EndHorizontal();
		CMVis.Instance.startFloor = Mathf.Clamp(value, 0, max);
		CMVis.Instance.endFloor = max;
		if (num != CMVis.Instance.startFloor || num2 != CMVis.Instance.endFloor)
		{
			mapVisDirty = true;
		}
		Toggle("NIGHTMARE", ref CMVis.Instance.nightmare);
		GUILayout.Space(8f);
		Toggles();
		if (mapVisDirty)
		{
			CMVis.Instance.Recalculate();
		}
	}

	private void Toggle(string label, ref bool value)
	{
		bool flag = value;
		value = GUILayout.Toggle(value, label);
		if (flag != value)
		{
			mapVisDirty = true;
		}
	}

	private void Toggles()
	{
		Toggle("Covers", ref CMVis.Instance.showCovers);
		Toggle("Floors", ref CMVis.Instance.showFloors);
		Toggle("Interiors", ref CMVis.Instance.showInteriors);
		Toggle("Impassables", ref CMVis.Instance.showImpassables);
		Toggle("Block Sights", ref CMVis.Instance.showBlockSights);
		Toggle("Block Bulets", ref CMVis.Instance.showBlockBullets);
		Toggle("Switches", ref CMVis.Instance.showSwitches);
		Toggle("Triggers", ref CMVis.Instance.showTriggers);
	}

	private void MakePanel_Team()
	{
		int length = Enum.GetValues(typeof(ETeamPanel)).Length;
		if (m_TeamNames == null)
		{
			m_TeamNames = Enum.GetNames(typeof(ETeamPanel));
		}
		if (m_TeamNames != null && length != 0)
		{
			ETeamPanel eTeamPanel = (ETeamPanel)GUILayout.SelectionGrid((int)m_CurrentTeamPanel, m_TeamNames, length);
			if (m_CurrentTeamPanel != eTeamPanel)
			{
				m_CurrentTeamPanel = eTeamPanel;
			}
			GUILayout.Space(10f);
			switch (m_CurrentTeamPanel)
			{
			case ETeamPanel.Characters:
				OnDraw_Characters();
				break;
			case ETeamPanel.Ability:
				OnDrawAbilities();
				break;
			case ETeamPanel.Backpack:
				OnDrawBackpack();
				break;
			case ETeamPanel.Buffs:
				OnDrawBuffs();
				break;
			case ETeamPanel.Cards:
				OnDrawCards();
				break;
			case ETeamPanel.UsableItems:
				OnDrawUsableItems();
				break;
			case ETeamPanel.Shops:
				OnDrawShops();
				break;
			}
		}
	}

	private void GenerateShopList()
	{
		m_ShopList_Strings = null;
		m_ShopList_Slected = null;
		m_ShopList_SelectedID = 0;
		m_ShopList_ScrollBarPos = 0f;
		if (CFGEconomy.m_Shops == null || CFGEconomy.m_Shops.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		if (list == null)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGStore> shop in CFGEconomy.m_Shops)
		{
			if (shop.Value != null)
			{
				list.Add(shop.Key);
			}
		}
		list.Sort();
		m_ShopList_Strings = list.ToArray();
	}

	private void SelectShop(int ID)
	{
		if (ID < 0 || m_ShopList_Strings == null || ID >= m_ShopList_Strings.Length)
		{
			return;
		}
		m_ShopList_SelectedID = ID;
		CFGEconomy.m_Shops.TryGetValue(m_ShopList_Strings[ID], out m_ShopList_Slected);
		m_ShopGoodsAllItems = null;
		if (m_ShopList_Slected.m_Goods == null || m_ShopList_Slected.m_Goods.Count == 0)
		{
			return;
		}
		m_ShopGoodsAllItems = new List<CFGStoreGood>();
		if (m_ShopGoodsAllItems == null)
		{
			return;
		}
		foreach (KeyValuePair<string, CFGStoreGood> good in m_ShopList_Slected.m_Goods)
		{
			if (good.Value != null)
			{
				m_ShopGoodsAllItems.Add(good.Value);
			}
		}
	}

	private int Goods_Sort_ByName(CFGStoreGood ia, CFGStoreGood ib)
	{
		return string.Compare(ia.ItemID, ib.ItemID);
	}

	private int Goods_Sort_ByBuy(CFGStoreGood ia, CFGStoreGood ib)
	{
		if (ia.BuyPrice > ib.BuyPrice)
		{
			return -1;
		}
		if (ia.BuyPrice < ib.BuyPrice)
		{
			return 1;
		}
		return 0;
	}

	private int Goods_Sort_BySell(CFGStoreGood ia, CFGStoreGood ib)
	{
		if (ia.SellPrice > ib.SellPrice)
		{
			return -1;
		}
		if (ia.SellPrice < ib.SellPrice)
		{
			return 1;
		}
		return 0;
	}

	private int Goods_Sort_ByCount(CFGStoreGood ia, CFGStoreGood ib)
	{
		if (ia.ItemCount > ib.ItemCount)
		{
			return -1;
		}
		if (ia.ItemCount < ib.ItemCount)
		{
			return 1;
		}
		return 0;
	}

	private void OnDrawShops()
	{
		if (m_ShopList_Strings == null)
		{
			GenerateShopList();
			if (m_ShopList_Strings == null)
			{
				return;
			}
		}
		int num = Mathf.Min(m_ShopList_Strings.Length, 22);
		float num2 = m_ShopList_Strings.Length;
		float fSize = Mathf.Min(22f, num2);
		GUILayout.BeginArea(new Rect(80f, 90f, 1500f, 60f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("         Shops (" + m_ShopList_Strings.Length + ")", GUILayout.Width(140f));
		if (GUILayout.Button("Add All", GUILayout.Width(60f)))
		{
			CFGEconomy.RegisterAllShops();
			GenerateShopList();
		}
		if (GUILayout.Button("ResetList", GUILayout.Width(60f)))
		{
			GenerateShopList();
		}
		GUILayout.Space(20f);
		if (GUILayout.Button("BMod+"))
		{
			foreach (KeyValuePair<string, CFGStoreGood> good in m_ShopList_Slected.m_Goods)
			{
				good.Value.BuyModifier += 0.1f;
				good.Value.BaseBuyPrice = good.Value.ItemDef.DefBuyVal;
			}
		}
		if (GUILayout.Button("BMod-"))
		{
			foreach (KeyValuePair<string, CFGStoreGood> good2 in m_ShopList_Slected.m_Goods)
			{
				good2.Value.BuyModifier -= 0.1f;
				good2.Value.BaseBuyPrice = good2.Value.ItemDef.DefBuyVal;
			}
		}
		if (GUILayout.Button(" SMod+"))
		{
			foreach (KeyValuePair<string, CFGStoreGood> good3 in m_ShopList_Slected.m_Goods)
			{
				good3.Value.SellModifier += 0.1f;
			}
		}
		if (GUILayout.Button("SMod-"))
		{
			foreach (KeyValuePair<string, CFGStoreGood> good4 in m_ShopList_Slected.m_Goods)
			{
				good4.Value.SellModifier -= 0.1f;
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(320f);
		if (GUILayout.Button("Item name", GUILayout.Width(400f)) && m_ShopGoodsAllItems != null)
		{
			m_ShopGoodsAllItems.Sort(Goods_Sort_ByName);
		}
		if (GUILayout.Button("Buy", GUILayout.Width(100f)) && m_ShopGoodsAllItems != null)
		{
			m_ShopGoodsAllItems.Sort(Goods_Sort_ByBuy);
		}
		if (GUILayout.Button("Sell", GUILayout.Width(100f)) && m_ShopGoodsAllItems != null)
		{
			m_ShopGoodsAllItems.Sort(Goods_Sort_BySell);
		}
		if (GUILayout.Button("Count", GUILayout.Width(100f)) && m_ShopGoodsAllItems != null)
		{
			m_ShopGoodsAllItems.Sort(Goods_Sort_ByCount);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.BeginArea(new Rect(80f, 160f, 300f, 800f));
		for (int i = m_ShopList_FirstItem_Global; i < m_ShopList_FirstItem_Global + num; i++)
		{
			if (GUILayout.Button(m_ShopList_Strings[i]))
			{
				SelectShop(i);
			}
		}
		GUILayout.EndArea();
		HandleVScroller(ref m_ShopList_ScrollBarPos, fSize, num2, 20f, ref m_ShopList_FirstItem_Global);
		if (m_ShopList_Slected == null)
		{
			return;
		}
		fSize = 0f;
		num2 = 0f;
		Color color = GUI.color;
		GUILayout.BeginArea(new Rect(400f, 160f, 700f, 800f));
		if (m_ShopGoodsAllItems == null || m_ShopGoodsAllItems.Count == 0)
		{
			GUILayout.Space(100f);
			GUILayout.Label("        Shop has no ITEMS!");
		}
		else
		{
			num2 = m_ShopGoodsAllItems.Count;
			num = Mathf.Min((int)num2, 22);
			fSize = Mathf.Min(22f, num2);
			Color white = Color.white;
			Color color2 = new Color(0.85f, 0.85f, 0.85f);
			for (int j = m_ShopGoods_FirstItem_Global; j < m_ShopGoods_FirstItem_Global + num; j++)
			{
				if ((j & 1) == 1)
				{
					GUI.color = color2;
				}
				else
				{
					GUI.color = white;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Label("#" + j + " " + m_ShopGoodsAllItems[j].ItemID, GUILayout.Width(400f));
				GUILayout.Label(m_ShopGoodsAllItems[j].BuyPrice.ToString(), GUILayout.Width(100f));
				GUILayout.Label(m_ShopGoodsAllItems[j].SellPrice.ToString(), GUILayout.Width(100f));
				GUILayout.Label(m_ShopGoodsAllItems[j].ItemCount.ToString(), GUILayout.Width(100f));
				GUILayout.EndHorizontal();
			}
		}
		GUI.color = color;
		GUILayout.EndArea();
		HandleVScroller(ref m_ShopGoods_ScrollBarPos, fSize, num2, 1150f, ref m_ShopGoods_FirstItem_Global);
	}

	private void HandleVScroller(ref float Value, float fSize, float fCount, float fLeft, ref int FirstITEM)
	{
		if (!(fSize < 1f) && !(fCount < 1f))
		{
			GUILayout.BeginArea(new Rect(fLeft, 160f, fLeft + 20f, 800f));
			Value = GUILayout.VerticalScrollbar(Value, fSize, 0f, fCount, GUILayout.Height(540f));
			GUILayout.EndArea();
			FirstITEM = (int)Value;
		}
	}

	private void OnDrawUsableItems()
	{
		if (AllUsables == null)
		{
			Dictionary<string, CFGDef_UsableItem> usables = CFGStaticDataContainer.GetUsables();
			if (usables == null || usables.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			if (list == null)
			{
				return;
			}
			foreach (KeyValuePair<string, CFGDef_UsableItem> item in usables)
			{
				list.Add(item.Key);
			}
			list.Sort();
			if (list.Count == 0)
			{
				return;
			}
			AllUsables = new string[list.Count];
			if (AllUsables == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				AllUsables[i] = list[i];
			}
		}
		if (AllUsables == null)
		{
			return;
		}
		m_LastSelUsable = GUILayout.SelectionGrid(m_LastSelUsable, AllUsables, 5);
		for (int j = 0; j < 4; j++)
		{
			GUILayout.BeginHorizontal();
			CFGCharacterData teamCharacter = CFGCharacterList.GetTeamCharacter(j);
			if (teamCharacter == null)
			{
				GUILayout.Label("No character #" + j);
			}
			else
			{
				GUILayout.Label(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(teamCharacter.Definition.NameID));
				if (string.IsNullOrEmpty(teamCharacter.Item1))
				{
					if (GUILayout.Button("Add As Item1", GUILayout.Width(200f)))
					{
						teamCharacter.EquipItem(EItemSlot.Item1, AllUsables[m_LastSelUsable]);
					}
				}
				else
				{
					GUILayout.Label("Item1: " + teamCharacter.Item1, GUILayout.Width(200f));
				}
				if (string.IsNullOrEmpty(teamCharacter.Item2))
				{
					if (GUILayout.Button("Add As Item2", GUILayout.Width(200f)))
					{
						teamCharacter.EquipItem(EItemSlot.Item2, AllUsables[m_LastSelUsable]);
					}
				}
				else
				{
					GUILayout.Label("Item2: " + teamCharacter.Item2, GUILayout.Width(200f));
				}
				if (string.IsNullOrEmpty(teamCharacter.Talisman))
				{
					if (GUILayout.Button("Add As Talisman", GUILayout.Width(200f)))
					{
						teamCharacter.EquipItem(EItemSlot.Talisman, AllUsables[m_LastSelUsable]);
					}
				}
				else
				{
					GUILayout.Label("Talisman: " + teamCharacter.Talisman, GUILayout.Width(200f));
				}
			}
			GUILayout.EndHorizontal();
		}
	}

	private int SortAllItems_ByName(CFGDef_Item ia, CFGDef_Item ib)
	{
		return string.Compare(ia.ItemID, ib.ItemID);
	}

	private int SortAllItems_ByType(CFGDef_Item ia, CFGDef_Item ib)
	{
		string strA = ia.ItemType.ToString();
		string strB = ib.ItemType.ToString();
		return string.Compare(strA, strB);
	}

	private int SortAllItems_ByCount(CFGDef_Item ia, CFGDef_Item ib)
	{
		int num = CFGInventory.Item_GetCount(ib.ItemID);
		int value = CFGInventory.Item_GetCount(ia.ItemID);
		return num.CompareTo(value);
	}

	private void OnDrawBackpack()
	{
		if (m_AllItems == null || m_AllItems.Count == 0)
		{
			m_AllItems = new List<CFGDef_Item>();
			if (m_AllItems == null || CFGStaticDataContainer.ItemDefinitions == null)
			{
				return;
			}
			foreach (KeyValuePair<string, CFGDef_Item> itemDefinition in CFGStaticDataContainer.ItemDefinitions)
			{
				m_AllItems.Add(itemDefinition.Value);
			}
			m_AllItems.Sort(SortAllItems_ByName);
			m_Backpack_SortType = 0;
		}
		float num = m_AllItems.Count;
		if (num == 0f)
		{
			return;
		}
		int num2 = Mathf.Min(m_AllItems.Count, 22);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Item types in the backpack: " + CFGInventory.BackpackItems.Count);
		GUILayout.Space(200f);
		m_LootPack = GUILayout.TextField(m_LootPack, GUILayout.Width(150f));
		if (GUILayout.Button("Add Loot", GUILayout.Width(100f)))
		{
			CFGStaticDataContainer.ApplyLootNode(m_LootPack);
		}
		GUILayout.EndHorizontal();
		float width = 250f;
		float width2 = 150f;
		float width3 = 100f;
		GUILayout.BeginHorizontal();
		GUILayout.Space(70f);
		GUI.backgroundColor = ((m_Backpack_SortType != 0) ? Color.grey : Color.green);
		if (GUILayout.Button("NameID", GUILayout.Width(width)))
		{
			m_AllItems.Sort(SortAllItems_ByName);
			m_Backpack_SortType = 0;
		}
		GUILayout.Space(40f);
		GUI.backgroundColor = ((m_Backpack_SortType != 1) ? Color.grey : Color.green);
		if (GUILayout.Button("Type", GUILayout.Width(width2)))
		{
			m_AllItems.Sort(SortAllItems_ByType);
			m_Backpack_SortType = 1;
		}
		GUILayout.Space(40f);
		GUI.backgroundColor = ((m_Backpack_SortType != 2) ? Color.grey : Color.green);
		if (GUILayout.Button("Count", GUILayout.Width(width3)))
		{
			m_AllItems.Sort(SortAllItems_ByCount);
			m_Backpack_SortType = 2;
		}
		GUI.backgroundColor = Color.grey;
		GUILayout.EndHorizontal();
		Rect screenRect = new Rect(80f, 160f, 1100f, 800f);
		GUILayout.BeginArea(screenRect);
		for (int i = m_FirstItem_Global; i < m_FirstItem_Global + num2; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(m_AllItems[i].ItemID, GUILayout.Width(width));
			GUILayout.Space(40f);
			GUILayout.Label(m_AllItems[i].ItemType.ToString(), GUILayout.Width(width2));
			GUILayout.Space(40f);
			GUILayout.Label(CFGInventory.Item_GetCount(m_AllItems[i].ItemID).ToString(), GUILayout.Width(width3));
			int num3 = 0;
			if (GUILayout.Button("+1"))
			{
				num3 = 1;
			}
			if (GUILayout.Button("+10"))
			{
				num3 = 10;
			}
			if (GUILayout.Button("+100"))
			{
				num3 = 100;
			}
			if (GUILayout.Button("+1000"))
			{
				num3 = 1000;
			}
			if (GUILayout.Button("-1"))
			{
				num3 = -1;
			}
			if (GUILayout.Button("-10"))
			{
				num3 = -10;
			}
			if (GUILayout.Button("-100"))
			{
				num3 = -100;
			}
			if (GUILayout.Button("-1000"))
			{
				num3 = -1000;
			}
			if (num3 < 0)
			{
				CFGInventory.RemoveItem(m_AllItems[i].ItemID, -num3, SetAsNew: false);
			}
			if (num3 > 0)
			{
				CFGInventory.AddItem(m_AllItems[i].ItemID, num3, SetAsNew: true);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		float size = Mathf.Min(22f, num);
		screenRect = new Rect(20f, 160f, 40f, 800f);
		GUILayout.BeginArea(screenRect);
		m_ScrollBarPos = GUILayout.VerticalScrollbar(m_ScrollBarPos, size, 0f, num, GUILayout.Height(540f));
		GUILayout.EndArea();
		m_FirstItem_Global = (int)m_ScrollBarPos;
	}
}
