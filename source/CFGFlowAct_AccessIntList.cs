public class CFGFlowAct_AccessIntList : CFGFlowAct_AccessList<int>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AccessIntList);
		flowActionInfo.DisplayName = "Access IntList";
		flowActionInfo.CategoryName = "Lists/Int";
		return flowActionInfo;
	}
}
