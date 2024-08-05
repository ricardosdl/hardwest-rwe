using UnityEngine;

public class CFGFlowCond_IsInObjectList : CFGFlowCond_IsInList<Object>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsInObjectList);
		flowConditionInfo.DisplayName = "Is In ObjectList";
		flowConditionInfo.CategoryName = "Lists/Object";
		return flowConditionInfo;
	}
}
