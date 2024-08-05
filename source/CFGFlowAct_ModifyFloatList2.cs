public class CFGFlowAct_ModifyFloatList2 : CFGFlowAct_ModifyList2<float>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ModifyFloatList2);
		flowActionInfo.DisplayName = "Modify FloatList";
		flowActionInfo.CategoryName = "Lists/Float";
		return flowActionInfo;
	}
}
