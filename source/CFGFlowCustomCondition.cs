using System;
using System.Collections.Generic;
using System.Reflection;

public class CFGFlowCustomCondition : CFGFlowConditionBase
{
	public delegate void ActionNoParam();

	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	public Delegate m_Delegate;

	public MethodInfo m_Method;

	public string m_MethodName;

	public object m_Return;

	public ParameterInfo[] m_Params;

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		m_MethodName = info.Name;
		CreateAutoConnectors();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (string.IsNullOrEmpty(m_MethodName))
		{
			return;
		}
		Type type = m_ParentSequence.GameContext.GetType();
		m_Method = type.GetMethod(m_MethodName);
		if (m_Method == null)
		{
			m_Deprecated = true;
			return;
		}
		if (m_Method.IsObsolete())
		{
			m_Deprecated = true;
		}
		m_Params = m_Method.GetParameters();
		Type[] array = null;
		if (m_Method.ReturnType != typeof(void))
		{
			array = new Type[m_Params.GetLength(0) + 1];
			array[m_Params.GetLength(0)] = m_Method.ReturnType;
		}
		else
		{
			array = new Type[m_Params.GetLength(0)];
		}
		for (int i = 0; i < m_Params.GetLength(0); i++)
		{
			array[i] = m_Params[i].ParameterType;
		}
		Type type2 = null;
		if (m_Method.ReturnType != typeof(void))
		{
			Type type3 = null;
			switch (array.GetLength(0))
			{
			case 1:
				type3 = typeof(Func<>).MakeGenericType(array);
				break;
			case 2:
				type3 = typeof(Func<, >).MakeGenericType(array);
				break;
			case 3:
				type3 = typeof(Func<, , >).MakeGenericType(array);
				break;
			case 4:
				type3 = typeof(Func<, , , >).MakeGenericType(array);
				break;
			case 5:
				type3 = typeof(Func<, , , , >).MakeGenericType(array);
				break;
			case 6:
				type3 = typeof(Func<, , , , , >).MakeGenericType(array);
				break;
			default:
				LogWarning("Unsupported");
				break;
			}
			type2 = type3;
		}
		else
		{
			Type type4 = null;
			switch (array.GetLength(0))
			{
			case 0:
				type4 = typeof(ActionNoParam);
				break;
			case 1:
				type4 = typeof(Action<>).MakeGenericType(array);
				break;
			case 2:
				type4 = typeof(Action<, >).MakeGenericType(array);
				break;
			case 3:
				type4 = typeof(Action<, , >).MakeGenericType(array);
				break;
			case 4:
				type4 = typeof(Action<, , , >).MakeGenericType(array);
				break;
			case 5:
				type4 = typeof(Action<, , , , >).MakeGenericType(array);
				break;
			case 6:
				type4 = typeof(Action<, , , , , >).MakeGenericType(array);
				break;
			default:
				LogWarning("Unsupported");
				break;
			}
			type2 = type4;
		}
		m_Delegate = Delegate.CreateDelegate(type2, m_ParentSequence.GameContext, m_Method);
	}

	public override void Activated()
	{
		base.Activated();
		object[] functionParameters = GetFunctionParameters();
		m_Return = m_Delegate.DynamicInvoke(functionParameters);
	}

	public override void DeActivated()
	{
		int num = Convert.ToInt32(m_Return);
		if (num < m_Outputs.Count)
		{
			m_Outputs[num].m_HasImpulse = true;
		}
		base.DeActivated();
	}

	public override List<CFGFlowNode> GetImpulsedFlow()
	{
		List<CFGFlowNode> list = new List<CFGFlowNode>();
		int num = Convert.ToInt32(m_Return);
		if (num >= 0 && num < m_Outputs.Count)
		{
			m_Outputs[num].m_ActivateCount++;
			foreach (CFGFlowConn_Exec link in m_Outputs[num].m_Links)
			{
				if (link.m_HasImpulse)
				{
					link.m_ActivateCount++;
					list.Add(link.m_OwningNode);
				}
			}
		}
		return list;
	}

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		m_Method = CFGFlowSequence.GameType.GetMethod(m_MethodName);
		CFGFlowCode[] array = m_Method.GetCustomAttributes(typeof(CFGFlowCode), inherit: false) as CFGFlowCode[];
		ParameterInfo[] parameters = m_Method.GetParameters();
		List<ParameterInfo> list = new List<ParameterInfo>(parameters);
		for (int i = 0; i < list.Count; i++)
		{
			Type flowVarType = CFGFlowVariable.GetFlowVarType(list[i].ParameterType);
			Type parameterType = list[i].ParameterType;
			CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, list[i].Name, flowVarType, parameterType, string.Empty);
		}
		Type functionReturnType = GetFunctionReturnType();
		if ((functionReturnType != typeof(void) && (functionReturnType.IsEnum || functionReturnType == typeof(int) || functionReturnType == typeof(byte))) || functionReturnType == typeof(bool))
		{
			for (int j = 0; j < array[0].OutputNames.GetLength(0); j++)
			{
				CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, array[0].OutputNames[j], null, null, string.Empty);
			}
		}
	}

	protected object[] GetFunctionParameters()
	{
		List<ParameterInfo> list = new List<ParameterInfo>(m_Params);
		Type[] array = new Type[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i].ParameterType;
		}
		object[] array2 = new object[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			array2[j] = m_Vars[j].m_Value;
		}
		return array2;
	}

	protected Type GetFunctionReturnType()
	{
		ParameterInfo returnParameter = m_Method.ReturnParameter;
		return returnParameter.ParameterType;
	}

	public new static List<FlowConditionInfo> GetConditionList()
	{
		Type gameType = CFGFlowSequence.GameType;
		MethodInfo[] methods = gameType.GetMethods();
		List<FlowConditionInfo> list = new List<FlowConditionInfo>();
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			CFGFlowCode[] array2 = methodInfo.GetCustomAttributes(typeof(CFGFlowCode), inherit: true) as CFGFlowCode[];
			if (array2.GetLength(0) > 0 && array2[0].CodeType == FlowCodeType.CT_Condition)
			{
				FlowConditionInfo flowConditionInfo = new FlowConditionInfo();
				flowConditionInfo.Type = typeof(CFGFlowCustomCondition);
				if (array2[0].Title != null && array2[0].Title != string.Empty)
				{
					flowConditionInfo.DisplayName = array2[0].Title;
				}
				else
				{
					flowConditionInfo.DisplayName = methodInfo.Name;
				}
				flowConditionInfo.CategoryName = array2[0].Category;
				flowConditionInfo.Name = methodInfo.Name;
				list.Add(flowConditionInfo);
			}
		}
		return list;
	}

	public override bool OnPostLoad()
	{
		base.OnPostLoad();
		Type type = m_ParentSequence.GameContext.GetType();
		m_Method = type.GetMethod(m_MethodName);
		return true;
	}
}
