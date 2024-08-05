public class CFGFlowAct_AccessFloatList : CFGFlowAct_AccessList<float>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_AccessFloatList);
		flowActionInfo.DisplayName = "Access FloatList";
		flowActionInfo.CategoryName = "Lists/Float";
		return flowActionInfo;
	}
}
