public class CFGFlowCond_IsEmptyFloatList : CFGFlowCond_IsEmptyList<float>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsEmptyFloatList);
		flowConditionInfo.DisplayName = "Is Empty FloatList";
		flowConditionInfo.CategoryName = "Lists/Float";
		return flowConditionInfo;
	}
}
