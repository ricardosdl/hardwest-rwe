public class CFGFlowCond_IsEmptyIntList : CFGFlowCond_IsEmptyList<int>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsEmptyIntList);
		flowConditionInfo.DisplayName = "Is Empty IntList";
		flowConditionInfo.CategoryName = "Lists/Int";
		return flowConditionInfo;
	}
}
