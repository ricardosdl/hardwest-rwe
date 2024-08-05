using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core;

namespace BehaviorPattern;

public static class BehaviorExtensions
{
	public static List<MethodInfo> GetCustomTasks(this IBehaviorAsset asset)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		Type customClass = asset.CustomClass;
		if (customClass != null)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo[] methods = customClass.GetMethods(bindingAttr);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if (ValidTask(methodInfo))
				{
					list.Add(methodInfo);
				}
			}
		}
		return list;
	}

	public static List<MethodInfo> GetCustomChecks(this IBehaviorAsset asset)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		Type customClass = asset.CustomClass;
		if (customClass != null)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo[] methods = customClass.GetMethods(bindingAttr);
			MethodInfo[] array = methods;
			foreach (MethodInfo methodInfo in array)
			{
				if (ValidCheck(methodInfo))
				{
					list.Add(methodInfo);
				}
			}
		}
		return list;
	}

	public static List<ParamAttribute> GetParams(this MethodInfo method)
	{
		List<ParamAttribute> list = new List<ParamAttribute>();
		object[] customAttributes = method.GetCustomAttributes(typeof(ParamAttribute), inherit: false);
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			list.Add(obj as ParamAttribute);
		}
		return list;
	}

	private static bool ValidTask(MethodInfo task)
	{
		TaskAttribute attribute = task.GetAttribute<TaskAttribute>(inherit: false);
		ParameterInfo returnParameter = task.ReturnParameter;
		ParameterInfo[] parameters = task.GetParameters();
		bool flag = returnParameter.ParameterType.IsA(typeof(IEnumerator<TaskResult>));
		bool flag2 = parameters.Count() > 0 && parameters[0].ParameterType.IsA(typeof(BehaviorComponent));
		return attribute != null && flag && flag2;
	}

	private static bool ValidCheck(MethodInfo task)
	{
		CheckAttribute attribute = task.GetAttribute<CheckAttribute>(inherit: false);
		ParameterInfo returnParameter = task.ReturnParameter;
		ParameterInfo[] parameters = task.GetParameters();
		bool flag = returnParameter.ParameterType.IsA(typeof(bool));
		bool flag2 = parameters.Count() > 0 && parameters[0].ParameterType.IsA(typeof(BehaviorComponent));
		return attribute != null && flag && flag2;
	}
}
