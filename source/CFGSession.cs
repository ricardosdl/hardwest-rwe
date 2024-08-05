#define USE_ERROR_REPORTING
using UnityEngine;

public abstract class CFGSession : MonoBehaviour
{
	protected string m_Current_Scene;

	protected CFGDef_Tactical m_Current_Tactical;

	protected CFGDef_Scenario m_Current_Scenario;

	protected CFGDef_Campaign m_Current_Campaign;

	public CFGDef_Tactical CurrentTactical => m_Current_Tactical;

	public abstract bool CreateSaveGame(CFG_SG_SaveGame.eSG_Mode _Mode, CFG_SG_SaveGame.eSG_Source _Source);

	public void GoToStrategic()
	{
		EGameState gameState = CFGSingleton<CFGGame>.Instance.GetGameState();
		if (gameState == EGameState.InGame || gameState == EGameState.UnloadingMission)
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadHUD();
			OnMissionUnloadingStart();
		}
		if (CFG_SG_Manager.HasStrategicRestorePoint)
		{
			Debug.Log("Loading strategic save...");
			if (CFG_SG_Manager.ReturnToRestorePoint_Strategic())
			{
				return;
			}
			Debug.LogError("Failed to restore strategic screen!");
		}
		if (m_Current_Campaign == null)
		{
			CFGError.ReportError("No campaign is selected. Cannot go to strategic screen", CFGError.ErrorCode.NotInitialized);
			return;
		}
		if (m_Current_Scenario == null)
		{
			CFGError.ReportError("No scenario is selected. Cannot go to strategic screen", CFGError.ErrorCode.NotInitialized);
			return;
		}
		if (string.IsNullOrEmpty(m_Current_Scenario.StrategicScene))
		{
			CFGError.ReportError("Current scenario (" + m_Current_Scenario.ScenarioID + ") has no strategic screen", CFGError.ErrorCode.NotInitialized);
			return;
		}
		m_Current_Scene = m_Current_Scenario.StrategicScene;
		Debug.Log("Loading Strategic Scene: " + m_Current_Scene);
		LoadLevel();
	}

	public void PrepareLevel_Strategic(string ScenarioName)
	{
		m_Current_Tactical = null;
		if (m_Current_Campaign == null)
		{
			CFGError.ReportError("No campaing is active. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
			return;
		}
		if (ScenarioName == null)
		{
			if (m_Current_Scenario == null)
			{
				CFGError.ReportError("No scenario is active. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
				return;
			}
			m_Current_Scene = m_Current_Scenario.StrategicScene;
			Debug.Log("Preparing Strategic Scene (Current Scenario: " + ScenarioName + ") Strategic Scene Name = " + m_Current_Scene);
			return;
		}
		m_Current_Scenario = m_Current_Campaign.GetScenarioByScenario(ScenarioName);
		if (m_Current_Scenario == null)
		{
			CFGError.ReportError("Failed to find scenario [" + ScenarioName + "]. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
			return;
		}
		m_Current_Scene = m_Current_Scenario.StrategicScene;
		Debug.Log("Preparing Strategic Scene: ScenarioID = " + ScenarioName + " Strategic Scene Name = " + m_Current_Scene);
	}

	public void PrepareLevel_AutoTactical(string SceneName)
	{
		m_Current_Tactical = null;
		m_Current_Scene = null;
		CFGDef_Tactical tacticalBySceneName = CFGStaticDataContainer.GetTacticalBySceneName(SceneName);
		if (tacticalBySceneName != null)
		{
			PrepareLevel_Tactical(tacticalBySceneName.TacticalID);
		}
	}

	public void PrepareLevel_Tactical(string TacticalID)
	{
		m_Current_Tactical = null;
		m_Current_Scene = null;
		if (m_Current_Campaign == null)
		{
			CFGError.ReportError("No campaing is active. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
			return;
		}
		if (m_Current_Scenario == null)
		{
			CFGError.ReportError("No scenario is active. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
			return;
		}
		m_Current_Tactical = CFGStaticDataContainer.GetTacticalByID(TacticalID);
		if (m_Current_Tactical == null)
		{
			CFGError.ReportError("Failed to find tactical scene [" + TacticalID + "]. Cannot determine strategic level name", CFGError.ErrorCode.Fail);
			return;
		}
		SetUpTacticalFlowCode();
		m_Current_Scene = m_Current_Tactical.SceneName;
		Debug.Log("Prepared scene: " + m_Current_Scene + " for TacticalID = " + TacticalID + "FlowCode = " + m_Current_Tactical.FlowCode);
	}

	public void SetUpTacticalFlowCode()
	{
		if (m_Current_Tactical != null)
		{
			CFGVar variable = CFGVariableContainer.Instance.GetVariable("TacticalFlowCode", "profile");
			if (variable != null)
			{
				variable.Value = m_Current_Tactical.FlowCode;
			}
		}
	}

	public abstract bool LoadLevel();

	public virtual void OnMissionLoaded()
	{
		CFGVariableContainer.Instance.LoadLocals();
		if (CFGSingletonResourcePrefab<CFGDialogSystem>.Instance != null)
		{
			Debug.Log("DialogSystem loaded");
		}
	}

	public virtual void OnMissionUnloadingStart()
	{
	}

	public abstract void MissionComplete();

	public abstract void MissionFail(string reason_id, params string[] args);

	public abstract void ScenarioComplete();

	public abstract void CampaignComplete(int video_idx);

	public int GetLoadingScreenID()
	{
		if (m_Current_Scenario != null)
		{
			return m_Current_Scenario.Index + 1;
		}
		return 0;
	}

	public string GetCurrentTextID()
	{
		if (m_Current_Tactical != null)
		{
			return m_Current_Tactical.TacticalID;
		}
		if (m_Current_Scenario != null)
		{
			return m_Current_Scenario.ScenarioID;
		}
		if (m_Current_Campaign != null)
		{
			return m_Current_Campaign.CampaignID;
		}
		return string.Empty;
	}

	public string GetStrategicMapName()
	{
		if (m_Current_Scenario == null)
		{
			return null;
		}
		return m_Current_Scenario.StrategicScene;
	}

	public int GetCurrentCampaignID()
	{
		if (m_Current_Campaign == null)
		{
			return -1;
		}
		return m_Current_Campaign.Index;
	}
}
