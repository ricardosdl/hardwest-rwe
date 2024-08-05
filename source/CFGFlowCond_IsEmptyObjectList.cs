using UnityEngine;

public class CFGFlowCond_IsEmptyObjectList : CFGFlowCond_IsEmptyList<Object>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsEmptyObjectList);
		flowConditionInfo.DisplayName = "Is Empty ObjectList";
		flowConditionInfo.CategoryName = "Lists/Object";
		return flowConditionInfo;
	}
}
