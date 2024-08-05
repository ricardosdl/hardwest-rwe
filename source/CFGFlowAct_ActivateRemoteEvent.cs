using System.Collections.Generic;

public class CFGFlowAct_ActivateRemoteEvent : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "RemoteEvent", expectedType = typeof(CFGFlowVar_String))]
	public string m_RemoteEvent = string.Empty;

	public override void Activated()
	{
		List<CFGFlowEvent_RemoteEvent> nodesOfType = CFGSequencerBase.GetMainSequence().GetNodesOfType<CFGFlowEvent_RemoteEvent>();
		foreach (CFGFlowEvent_RemoteEvent item in nodesOfType)
		{
			if (item != null && item.m_RemoteName == m_RemoteEvent)
			{
				item.m_ParentSequence.QueueFlowNode(item, bPushTop: true, bAllowSame: true);
			}
		}
	}

	public override string GetDisplayName()
	{
		return base.GetDisplayName() + ((!(m_RemoteEvent == string.Empty)) ? (": " + m_RemoteEvent) : string.Empty);
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ActivateRemoteEvent);
		flowActionInfo.DisplayName = "Activate Remote Event";
		flowActionInfo.CategoryName = "Events";
		return flowActionInfo;
	}
}
