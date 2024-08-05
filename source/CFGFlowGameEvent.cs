using System;
using System.Collections.Generic;
using System.Reflection;

public class CFGFlowGameEvent : CFGFlowEventBase
{
	public override bool RegisterEvent(object inObject)
	{
		return true;
	}

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		CreateAutoConnectors();
	}

	public override bool IsOK()
	{
		return base.IsConnectedToFlow;
	}

	public new static List<FlowEventInfo> GetEventList()
	{
		List<FlowEventInfo> list = new List<FlowEventInfo>();
		foreach (Type item2 in CFGClassUtil.AllSubClasses(typeof(CFGFlowGameEvent)))
		{
			if (!item2.IsAbstract && !item2.IsObsolete())
			{
				MethodInfo method = item2.GetMethod("GetEventInfo", BindingFlags.Static | BindingFlags.Public);
				if (method != null)
				{
					FlowEventInfo item = (FlowEventInfo)method.Invoke(null, null);
					list.Add(item);
				}
			}
		}
		return list;
	}
}
