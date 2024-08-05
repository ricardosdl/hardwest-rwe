using System;
using System.Collections.Generic;
using UnityEngine;

internal class CFGDevMenuWindow : CFGWindow
{
	private Rect m_MainWindowRect = new Rect((float)Screen.width * 0.5f, (float)Screen.height * 0.05f, (float)(Screen.width / 2) - (float)Screen.width * 0.05f, (float)Screen.height - (float)Screen.height * 0.1f);

	private string[] m_ScenarioNames = new string[9] { "Scenario 1", "Scenario 2", "Scenario 3", "Scenario 4", "Scenario 5", "Scenario 6", "Scenario 7", "Scenario 8", "Scenario 9" };

	private List<string[]> m_TacticalNames = new List<string[]>();

	private int m_CurrentScenario;

	private int m_CurrentTactical;

	public override EWindowID GetWindowID()
	{
		return EWindowID.DEV_MenuWindow;
	}

	protected override void OnActivate()
	{
	}

	protected override void OnDeactivate()
	{
	}

	protected override void OnUpdate()
	{
	}

	private void Start()
	{
		m_TacticalNames.Add(new string[5] { "1_0_Town", "0_demo", "1_2_Farm", "1_4_Mansion", "1_5_OilRig" });
		m_TacticalNames.Add(new string[4] { "2_1_Reverend_House", "2_2_Church", "2_4_Road", "2_5_Judge_House" });
		m_TacticalNames.Add(new string[4] { "3_1_Massacre", "3_2_GhostTown", "3_3_Journey", "3_4_Swamp" });
		m_TacticalNames.Add(new string[4] { "4_1_Gravediggers", "4_2_Brothel", "4_3_LookoutHill", "4_4_Purgatory" });
		m_TacticalNames.Add(new string[5] { "5_1_Lab", "5_2_Asylum", "5_3_Maze", "5_4_Crater", "5_5_Machine" });
		m_TacticalNames.Add(new string[5] { "6_1_FarmCamp", "6_2_LandingSite", "6_3_BanditsFort", "6_4_NightmareRuins", "6_5_NightmareEldoradoGates" });
		m_TacticalNames.Add(new string[5] { "7_1_AbandonedTown", "7_2_Homestead", "7_5_MexicoVillage", "7_5_MexicoVillageAlt", "7_6_HomesteadRuined" });
		m_TacticalNames.Add(new string[4] { "8_1_Gallows", "8_2_TrainCrash", "8_3_LakeHideout", "8_4_Sanitarium" });
		m_TacticalNames.Add(new string[7] { "9_1_Quarry", "9_2_Cemetery", "9_3_Ironlines", "9_4_GreenerPastures", "9_5_CemeteryEscape", "9_6_Harbor", "9_7_QuarryShowdown" });
	}

	protected override void DrawWindow()
	{
		m_MainWindowRect = GUILayout.Window((int)GetWindowID(), m_MainWindowRect, MakeMainWindow, "Developers panel");
	}

	private void MakeMainWindow(int id)
	{
		GUILayout.BeginHorizontal();
		m_CurrentScenario = GUILayout.SelectionGrid(m_CurrentScenario, m_ScenarioNames, 1);
		string[] array = new string[0];
		if (m_CurrentScenario < m_TacticalNames.Count)
		{
			array = m_TacticalNames[m_CurrentScenario];
		}
		m_CurrentTactical = Math.Min(m_CurrentTactical, array.Length - 1);
		m_CurrentTactical = GUILayout.SelectionGrid(m_CurrentTactical, array, 1);
		GUILayout.EndHorizontal();
		GUILayout.Space(20f);
		string text = m_TacticalNames[m_CurrentScenario][m_CurrentTactical];
		string text2 = "scen_0" + (m_CurrentScenario + 1);
		GUILayout.Label("Scene to load: " + text + " (" + text2 + ")");
		if (GUILayout.Button("Load"))
		{
			bool flag = false;
			CFGSessionSingle cFGSessionSingle = CFGSingleton<CFGGame>.Instance.CreateSessionSingle();
			if (!((m_CurrentScenario >= 8) ? (!cFGSessionSingle.SelectCampaign(EDLC.DLC1)) : (!cFGSessionSingle.SelectCampaign(1))))
			{
				Deactivate();
				cFGSessionSingle.StartScenario(text2);
				cFGSessionSingle.PrepareLevel_Tactical(text);
				cFGSessionSingle.ResetMissionStats();
				cFGSessionSingle.OnNewGame(EDifficulty.Normal);
				cFGSessionSingle.ReadCampaignVariableDefinitions();
				cFGSessionSingle.ReadScenarioVariableDefinitions();
				cFGSessionSingle.LoadLevel();
			}
			else
			{
				CFGSingleton<CFGGame>.Instance.DestroySession();
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close", GUILayout.ExpandWidth(expand: false)))
		{
			Deactivate();
		}
		GUILayout.EndHorizontal();
		GUI.DragWindow(new Rect(0f, 0f, m_MainWindowRect.width, 20f));
	}
}
