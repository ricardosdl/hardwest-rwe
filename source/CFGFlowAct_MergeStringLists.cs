public class CFGFlowAct_MergeStringLists : CFGFlowAct_MergeLists<string>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_MergeStringLists);
		flowActionInfo.DisplayName = "Merge String Lists";
		flowActionInfo.CategoryName = "Lists/String";
		return flowActionInfo;
	}
}
