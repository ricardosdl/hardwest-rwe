using UnityEngine;

public class CFGFlowAct_MergeObjectLists : CFGFlowAct_MergeLists<Object>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_MergeObjectLists);
		flowActionInfo.DisplayName = "Merge Object Lists";
		flowActionInfo.CategoryName = "Lists/Object";
		return flowActionInfo;
	}
}
