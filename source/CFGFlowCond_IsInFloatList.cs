public class CFGFlowCond_IsInFloatList : CFGFlowCond_IsInList<float>
{
	public new static FlowConditionInfo GetConditionInfo()
	{
		FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
		flowConditionInfo.Type = typeof(CFGFlowCond_IsInFloatList);
		flowConditionInfo.DisplayName = "Is In FloatList";
		flowConditionInfo.CategoryName = "Lists/Float";
		return flowConditionInfo;
	}
}
