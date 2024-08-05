public class CFGFlowAct_AccessStringList : CFGFlowAct_AccessList<string>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AccessStringList);
		flowActionInfo.DisplayName = "Access StringList";
		flowActionInfo.CategoryName = "Lists/String";
		return flowActionInfo;
	}
}
