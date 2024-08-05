using System;
using System.Collections.Generic;
using System.Reflection;

public class CFGFlowGameCondition : CFGFlowConditionBase
{
	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		base.CreateAutoConnectors();
	}

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		CreateAutoConnectors();
	}

	public new static List<FlowConditionInfo> GetConditionList()
	{
		List<FlowConditionInfo> list = new List<FlowConditionInfo>();
		foreach (Type item2 in CFGClassUtil.AllSubClasses(typeof(CFGFlowGameCondition)))
		{
			if (!item2.IsAbstract && !item2.IsObsolete())
			{
				MethodInfo method = item2.GetMethod("GetConditionInfo", BindingFlags.Static | BindingFlags.Public);
				if (method != null)
				{
					FlowConditionInfo item = (FlowConditionInfo)method.Invoke(null, null);
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static FlowConditionInfo GetConditionInfo()
	{
		return new FlowConditionInfo();
	}
}
