using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CFGFlowCustomAction : CFGFlowActionBase
{
	public delegate void ActionNoParam();

	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	public Delegate m_Delegate;

	public MethodInfo m_Method;

	public string m_MethodName;

	public object m_Return;

	public ParameterInfo[] m_Params = new ParameterInfo[0];

	public bool m_Latent;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (string.IsNullOrEmpty(m_MethodName))
		{
			return;
		}
		if (m_ParentSequence == null || m_ParentSequence.GameContext == null)
		{
			m_Deprecated = true;
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
		int num = m_Params.Length;
		if (m_Method.ReturnType != typeof(void) && !m_Latent)
		{
			num++;
		}
		if (num != m_Vars.Count)
		{
			m_Deprecated = true;
			return;
		}
		Type[] array;
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
		Type type3;
		if (m_Method.ReturnType != typeof(void))
		{
			Type type2 = null;
			switch (array.GetLength(0))
			{
			case 1:
				type2 = typeof(Func<>).MakeGenericType(array);
				break;
			case 2:
				type2 = typeof(Func<, >).MakeGenericType(array);
				break;
			case 3:
				type2 = typeof(Func<, , >).MakeGenericType(array);
				break;
			case 4:
				type2 = typeof(Func<, , , >).MakeGenericType(array);
				break;
			case 5:
				type2 = typeof(Func<, , , , >).MakeGenericType(array);
				break;
			case 6:
				type2 = typeof(Func<, , , , , >).MakeGenericType(array);
				break;
			default:
				LogWarning("Unsupported " + m_MethodName);
				break;
			}
			type3 = type2;
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
				LogWarning("Unsupported" + m_MethodName);
				break;
			}
			type3 = type4;
		}
		if (type3 != null)
		{
			m_Delegate = Delegate.CreateDelegate(type3, m_ParentSequence.GameContext, m_Method);
		}
	}

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		m_MethodName = info.Name;
		CreateAutoConnectors();
	}

	public override void Activated()
	{
		base.Activated();
		object[] functionParameters = GetFunctionParameters();
		for (int i = 0; i < functionParameters.Length; i++)
		{
			if (functionParameters[i] != null && functionParameters[i] is UnityEngine.Object)
			{
				UnityEngine.Object @object = (UnityEngine.Object)functionParameters[i];
				if (@object == null)
				{
					functionParameters[i] = null;
				}
			}
		}
		m_Return = m_Delegate.DynamicInvoke(functionParameters);
		foreach (CFGFlowConn_Var item in m_Vars.Where((CFGFlowConn_Var conn) => conn.m_ConnDir == ConnectorDirection.CD_Output))
		{
			item.m_Value = m_Return;
		}
	}

	public override bool UpdateFlow(float deltaTime)
	{
		if (!m_Latent || (bool)m_Return)
		{
			return true;
		}
		object[] functionParameters = GetFunctionParameters();
		m_Return = m_Method.Invoke(m_ParentSequence.GameContext, functionParameters);
		return false;
	}

	protected object[] GetFunctionParameters()
	{
		return (from v in m_Vars.Take(m_Params.Length)
			select v.m_Value).ToArray();
	}

	public static List<MethodInfo> GetSupportedActions(Type type)
	{
		MethodInfo[] methods = type.GetMethods();
		List<MethodInfo> list = new List<MethodInfo>();
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (methodInfo.GetCustomAttributes(typeof(CFGFlowCode), inherit: true) is CFGFlowCode[] source && source.Any())
			{
				list.Add(methodInfo);
			}
		}
		return list;
	}

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Exec, "In", null, null, string.Empty);
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		m_Method = CFGFlowSequence.GameType.GetMethod(m_MethodName);
		ParameterInfo[] parameters = m_Method.GetParameters();
		foreach (ParameterInfo parameterInfo in parameters)
		{
			Type parameterType = parameterInfo.ParameterType;
			Type flowVarType = CFGFlowVariable.GetFlowVarType(parameterType);
			CreateConnector(ConnectorDirection.CD_Input, ConnectorType.CT_Var, parameterInfo.Name, flowVarType, parameterType, string.Empty);
		}
		if (!(m_Method.GetCustomAttributes(typeof(CFGFlowCode), inherit: false).FirstOrDefault() is CFGFlowCode cFGFlowCode))
		{
			return;
		}
		if (cFGFlowCode.IsLatent)
		{
			m_Latent = true;
		}
		else if (m_Method.ReturnParameter != null)
		{
			Type parameterType2 = m_Method.ReturnParameter.ParameterType;
			if (parameterType2 != typeof(void))
			{
				CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, "Return", CFGFlowVariable.GetFlowVarType(parameterType2), parameterType2, string.Empty);
			}
		}
	}

	public new static List<FlowActionInfo> GetActionList()
	{
		Type gameType = CFGFlowSequence.GameType;
		MethodInfo[] methods = gameType.GetMethods();
		List<FlowActionInfo> list = new List<FlowActionInfo>();
		MethodInfo[] array = methods;
		foreach (MethodInfo methodInfo in array)
		{
			if (methodInfo.IsObsolete())
			{
				continue;
			}
			CFGFlowCode[] array2 = methodInfo.GetCustomAttributes(typeof(CFGFlowCode), inherit: true) as CFGFlowCode[];
			if (array2.GetLength(0) > 0 && array2[0].CodeType == FlowCodeType.CT_Action)
			{
				FlowActionInfo flowActionInfo = new FlowActionInfo();
				flowActionInfo.Type = typeof(CFGFlowCustomAction);
				if (array2[0].Title != null && array2[0].Title != string.Empty)
				{
					flowActionInfo.DisplayName = array2[0].Title;
				}
				else
				{
					flowActionInfo.DisplayName = methodInfo.Name;
				}
				flowActionInfo.Name = methodInfo.Name;
				flowActionInfo.CategoryName = array2[0].Category;
				list.Add(flowActionInfo);
			}
		}
		return list;
	}

	public override bool IsAllVariableConnectorsLinkedOrDefaults()
	{
		return true;
	}

	public override bool OnPostLoad()
	{
		base.OnPostLoad();
		Type type = m_ParentSequence.GameContext.GetType();
		m_Method = type.GetMethod(m_MethodName);
		if (m_Latent)
		{
			m_Return = false;
		}
		return true;
	}
}
