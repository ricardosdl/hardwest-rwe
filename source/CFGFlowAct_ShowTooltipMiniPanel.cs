public class CFGFlowAct_ShowTooltipMiniPanel : CFGFlowGameAction
{
	private bool m_Finished;

	public override bool DefaultOutput => false;

	public override void CreateAutoConnectors()
	{
		base.CreateAutoConnectors();
		m_Outputs[0].m_ConnName = "ButtonYes";
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "ButtonNo", null, null, string.Empty);
	}

	public override void Activated()
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		m_Finished = false;
		if (instance != null && instance.m_TutorialMiniPopup == null)
		{
			instance.LoadTutorialMini();
		}
	}

	public override bool UpdateFlow(float deltaTime)
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance != null && instance.m_TutorialMiniPopup != null && instance.m_TutorialMiniPopup.m_YBtn.m_ButtonClickedCallback == null)
		{
			instance.m_TutorialMiniPopup.m_YBtn.m_ButtonClickedCallback = OnPanelButtonClick;
			instance.m_TutorialMiniPopup.m_NBtn.m_ButtonClickedCallback = OnPanelButtonClick;
		}
		return m_Finished;
	}

	public void OnPanelButtonClick(int button_id)
	{
		CFGWindowMgr instance = CFGSingleton<CFGWindowMgr>.Instance;
		if (instance != null)
		{
			instance.UnloadTutorialMini();
		}
		m_Outputs[button_id].m_HasImpulse = true;
		m_Finished = true;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ShowTooltipMiniPanel);
		flowActionInfo.DisplayName = "Show Tutorial Confirmation Popup";
		flowActionInfo.CategoryName = "GUI and HUD";
		return flowActionInfo;
	}
}
