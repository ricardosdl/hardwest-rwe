public class CFGFlowCond_IsEmptyStringList : CFGFlowCond_IsEmptyList<string>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsEmptyStringList);
		flowConditionInfo.DisplayName = "Is Empty StringList";
		flowConditionInfo.CategoryName = "Lists/String";
		return flowConditionInfo;
	}
}
