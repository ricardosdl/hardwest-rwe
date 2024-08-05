public class CFGFlowCond_IsInStringList : CFGFlowCond_IsInList<string>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsInStringList);
		flowConditionInfo.DisplayName = "Is In StringList";
		flowConditionInfo.CategoryName = "Lists/String";
		return flowConditionInfo;
	}
}
