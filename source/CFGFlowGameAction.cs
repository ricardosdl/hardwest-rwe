using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class CFGFlowGameAction : CFGFlowActionBase
{
	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		CreateAutoConnectors();
	}

	public new static List<FlowActionInfo> GetActionList()
	{
		List<FlowActionInfo> list = new List<FlowActionInfo>();
		foreach (Type item2 in CFGClassUtil.AllSubClasses(typeof(CFGFlowGameAction)))
		{
			if (!item2.IsAbstract && !item2.IsObsolete())
			{
				MethodInfo method = item2.GetMethod("GetActionInfo", BindingFlags.Static | BindingFlags.Public);
				if (method != null)
				{
					FlowActionInfo item = (FlowActionInfo)method.Invoke(null, null);
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static FlowActionInfo GetActionInfo()
	{
		return new FlowActionInfo();
	}

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		base.CreateAutoConnectors();
	}
}
