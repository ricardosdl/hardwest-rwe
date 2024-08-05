public class CFGFlowAct_ModifyStringList2 : CFGFlowAct_ModifyList2<string>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ModifyStringList2);
		flowActionInfo.DisplayName = "Modify StringList";
		flowActionInfo.CategoryName = "Lists/String";
		return flowActionInfo;
	}
}
