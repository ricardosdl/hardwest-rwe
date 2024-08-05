public class CFGFlowAct_MergeFloatLists : CFGFlowAct_MergeLists<float>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_MergeFloatLists);
		flowActionInfo.DisplayName = "Merge Float Lists";
		flowActionInfo.CategoryName = "Lists/Float";
		return flowActionInfo;
	}
}
