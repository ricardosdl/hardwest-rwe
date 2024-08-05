using UnityEngine;

public class CFGFlowAct_AccessObjectList : CFGFlowAct_AccessList<Object>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AccessObjectList);
		flowActionInfo.DisplayName = "Access ObjectList";
		flowActionInfo.CategoryName = "Lists/Object";
		return flowActionInfo;
	}
}
