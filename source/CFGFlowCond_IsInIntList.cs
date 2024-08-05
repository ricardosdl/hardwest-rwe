public class CFGFlowCond_IsInIntList : CFGFlowCond_IsInList<int>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsInIntList);
		flowConditionInfo.DisplayName = "Is In IntList";
		flowConditionInfo.CategoryName = "Lists/Int";
		return flowConditionInfo;
	}
}
