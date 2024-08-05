public class CFGFlowAct_MergeIntLists : CFGFlowAct_MergeLists<int>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_MergeIntLists);
		flowActionInfo.DisplayName = "Merge Int Lists";
		flowActionInfo.CategoryName = "Lists/Int";
		return flowActionInfo;
	}
}
