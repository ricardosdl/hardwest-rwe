using UnityEngine;

public class CFGFlowAct_ModifyObjectList2 : CFGFlowAct_ModifyList2<Object>
{
	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_ModifyObjectList2);
		flowActionInfo.DisplayName = "Modify ObjectList";
		flowActionInfo.CategoryName = "Lists/Object";
		return flowActionInfo;
	}

	protected override bool IsValidToAdd()
	{
		return (bool)m_Object && !m_List.Contains(m_Object);
	}
}
