public class CFGFlowAct_ModifyIntList2 : CFGFlowAct_ModifyList2<int>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ModifyIntList2);
		flowActionInfo.DisplayName = "Modify IntList";
		flowActionInfo.CategoryName = "Lists/Int";
		return flowActionInfo;
	}
}
