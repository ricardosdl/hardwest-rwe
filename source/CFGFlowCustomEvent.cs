using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

public class CFGFlowCustomEvent : CFGFlowEventBase
{
	public FieldInfo m_Event;

	[HideInInspector]
	public CFGType m_ContextType;

	public DynamicMethod m_CallMethod;

	[HideInInspector]
	public List<CFGSerializableObject> m_RegisteredSerializables = new List<CFGSerializableObject>();

	[HideInInspector]
	public List<MonoBehaviour> m_RegisteredMonos = new List<MonoBehaviour>();

	[HideInInspector]
	public List<object[]> m_ArgQueue = new List<object[]>();

	public int m_MaxActivationCount;

	public float m_ReActivationDelay = 0.1f;

	protected override void OnEnable()
	{
		base.OnEnable();
		if (m_DisplayName == null)
		{
			m_DisplayName = string.Empty;
		}
		if (m_EventName == null || !(m_EventName != string.Empty))
		{
			return;
		}
		if (m_ContextType == null)
		{
			LogWarning("Deprecating event... " + m_EventName);
			m_Deprecated = true;
			return;
		}
		Type systemType = m_ContextType.SystemType;
		FieldInfo field = systemType.GetField(m_EventName);
		if (field == null)
		{
			LogWarning("Deprecating event..." + m_EventName);
			m_Deprecated = true;
			return;
		}
		MethodInfo method = field.FieldType.GetMethod("Invoke");
		ParameterInfo[] parameters = method.GetParameters();
		int length = parameters.GetLength(0);
		if (length != m_Vars.Count)
		{
			LogWarning("Deprecating event..." + m_EventName);
			m_Deprecated = true;
		}
	}

	public static List<MemberInfo> GetSupportedEvents(Type type)
	{
		FieldInfo[] fields = type.GetFields();
		List<MemberInfo> list = new List<MemberInfo>();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			CFGFlowCode[] array2 = fieldInfo.GetCustomAttributes(typeof(CFGFlowCode), inherit: true) as CFGFlowCode[];
			if (array2.Length > 0)
			{
				MethodInfo method = fieldInfo.FieldType.GetMethod("Invoke");
				if (method != null)
				{
					list.Add(fieldInfo);
				}
			}
		}
		return list;
	}

	public override void InitFromInfo(FlowNodeInfo info)
	{
		base.InitFromInfo(info);
		FlowEventInfo flowEventInfo = info as FlowEventInfo;
		m_ContextType = new CFGType(flowEventInfo.EventContextType);
		m_EventName = flowEventInfo.Name;
		CreateAutoConnectors();
	}

	public override void CreateAutoConnectors()
	{
		CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Exec, "Out", null, null, string.Empty);
		MethodInfo method = m_ContextType.SystemType.GetField(m_EventName).FieldType.GetMethod("Invoke");
		ParameterInfo[] parameters = method.GetParameters();
		List<ParameterInfo> list = new List<ParameterInfo>(parameters);
		for (int i = 0; i < list.Count; i++)
		{
			Type flowVarType = CFGFlowVariable.GetFlowVarType(list[i].ParameterType);
			Type parameterType = list[i].ParameterType;
			CreateConnector(ConnectorDirection.CD_Output, ConnectorType.CT_Var, list[i].Name, flowVarType, parameterType, string.Empty);
		}
	}

	public new static List<FlowEventInfo> GetEventList()
	{
		List<FlowEventInfo> list = new List<FlowEventInfo>();
		foreach (Type item in CFGClassUtil.AllSubClasses(typeof(MonoBehaviour)))
		{
			list.AddRange(GetEventListForType(item));
		}
		return list;
	}

	public static List<FlowEventInfo> GetEventListForType(Type type)
	{
		List<FlowEventInfo> list = new List<FlowEventInfo>();
		FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.IsObsolete())
			{
				continue;
			}
			CFGFlowCode[] array2 = fieldInfo.GetCustomAttributes(typeof(CFGFlowCode), inherit: false) as CFGFlowCode[];
			if (array2.Length <= 0)
			{
				continue;
			}
			MethodInfo method = fieldInfo.FieldType.GetMethod("Invoke");
			if (method != null)
			{
				FlowEventInfo flowEventInfo = new FlowEventInfo();
				flowEventInfo.Type = typeof(CFGFlowCustomEvent);
				if (array2[0].Title != null && array2[0].Title != string.Empty)
				{
					flowEventInfo.DisplayName = array2[0].Title;
				}
				else
				{
					flowEventInfo.DisplayName = fieldInfo.Name;
				}
				flowEventInfo.Name = fieldInfo.Name;
				flowEventInfo.CategoryName = array2[0].Category;
				flowEventInfo.EventContextType = type;
				list.Add(flowEventInfo);
			}
		}
		return list;
	}

	public bool IsEventRegistered(object inObject)
	{
		if (inObject is CFGSerializableObject)
		{
			return m_RegisteredSerializables.FindIndex((CFGSerializableObject prop) => prop == inObject as CFGSerializableObject) != -1;
		}
		if (inObject is MonoBehaviour)
		{
			return m_RegisteredMonos.Contains(inObject as MonoBehaviour);
		}
		return false;
	}

	public override bool RegisterEvent(object inObject)
	{
		if (inObject == null)
		{
			LogWarning("NULL CONTEXT ! " + m_DisplayName + " guid = " + m_GUID.ToString());
			return false;
		}
		Type type = inObject.GetType();
		m_Event = type.GetField(m_EventName, BindingFlags.Instance | BindingFlags.Public);
		BindToEvent(inObject, m_Event);
		if (inObject.GetType() == typeof(CFGSerializableObject) || inObject.GetType().IsSubclassOf(typeof(CFGSerializableObject)))
		{
			CFGSerializableObject cFGSerializableObject = inObject as CFGSerializableObject;
			if (cFGSerializableObject != null)
			{
				m_RegisteredSerializables.Add(cFGSerializableObject);
			}
		}
		else if (inObject is MonoBehaviour)
		{
			Log(string.Concat("Event on MonoBehaviour: ", inObject, " It will not be serialized!"));
			m_RegisteredMonos.Add(inObject as MonoBehaviour);
		}
		else
		{
			LogError("Failed to register event!");
		}
		m_ParentSequence.m_RegisteredEvents.Add(this);
		return true;
	}

	public override void UnRegisterEvent(object inObject)
	{
		if (inObject == null)
		{
			LogWarning("NULL CONTEXT !");
			return;
		}
		Type type = inObject.GetType();
		m_Event = type.GetField(m_EventName, BindingFlags.Instance | BindingFlags.Public);
		Delegate value = m_CallMethod.CreateDelegate(m_Event.FieldType, this);
		Delegate value2 = Delegate.RemoveAll((Delegate)m_Event.GetValue(inObject), value);
		m_Event.SetValue(inObject, value2);
		if (inObject.GetType() == typeof(CFGSerializableObject) || inObject.GetType().IsSubclassOf(typeof(CFGSerializableObject)))
		{
			m_RegisteredSerializables.Remove(inObject as CFGSerializableObject);
		}
		else if (inObject is MonoBehaviour)
		{
			m_RegisteredMonos.Remove(inObject as MonoBehaviour);
		}
		if (m_ParentSequence != null)
		{
			m_ParentSequence.EventUnregistered(this);
		}
		m_ParentSequence.m_RegisteredEvents.Remove(this);
	}

	public void OnEvent(params object[] args)
	{
		m_ArgQueue.Add(args);
		m_ParentSequence.QueueFlowNode(this, bPushTop: true);
	}

	public override void Activated()
	{
		base.Activated();
		if (m_ArgQueue.Count <= 0)
		{
			LogError("no arguments ?");
			return;
		}
		object[] array = m_ArgQueue[0];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			m_Vars[i].m_Value = array[i];
		}
		m_ArgQueue.RemoveAt(0);
	}

	public void BindToEvent(object context, FieldInfo eventInfo)
	{
		if (eventInfo == null)
		{
			LogWarning("Can't bind to null EventInfo !");
			return;
		}
		MethodInfo method = eventInfo.FieldType.GetMethod("Invoke");
		if (method == null || method.ReturnType != typeof(void))
		{
			LogWarning("Method not found or incorrect method return type !");
			return;
		}
		MethodInfo method2 = GetType().GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.Public);
		ParameterInfo[] parameters = method.GetParameters();
		Type[] array = new Type[parameters.GetLength(0) + 1];
		array[0] = typeof(object);
		for (int i = 1; i < array.GetLength(0); i++)
		{
			array[i] = parameters[i - 1].ParameterType;
		}
		DynamicMethod dynamicMethod = new DynamicMethod("CallMethod", method.ReturnType, array);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		int num = array.Length - 1;
		LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(object[]));
		localBuilder.SetLocalSymInfo("arguments");
		iLGenerator.Emit(OpCodes.Ldarg_0);
		iLGenerator.Emit(OpCodes.Ldc_I4_S, num);
		iLGenerator.Emit(OpCodes.Newarr, typeof(object));
		iLGenerator.Emit(OpCodes.Stloc, localBuilder);
		for (int j = 0; j < num; j++)
		{
			if (!array[j + 1].IsPrimitive && !array[j + 1].IsEnum)
			{
				iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
				iLGenerator.Emit(OpCodes.Ldc_I4, j);
				iLGenerator.Emit(OpCodes.Ldarg, j + 1);
				iLGenerator.Emit(OpCodes.Stelem_Ref);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
				iLGenerator.Emit(OpCodes.Ldc_I4, j);
				iLGenerator.Emit(OpCodes.Ldarg, j + 1);
				iLGenerator.Emit(OpCodes.Box, array[j + 1]);
				iLGenerator.Emit(OpCodes.Stelem_Ref);
			}
		}
		iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
		iLGenerator.Emit(OpCodes.Callvirt, method2);
		iLGenerator.Emit(OpCodes.Ret);
		if (m_CallMethod == null)
		{
			m_CallMethod = dynamicMethod;
		}
		Delegate b = m_CallMethod.CreateDelegate(eventInfo.FieldType, this);
		Delegate value = Delegate.Combine((Delegate)eventInfo.GetValue(context), b);
		eventInfo.SetValue(context, value);
	}

	public override void PopulateVariableValues()
	{
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.Event;
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = BaseSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		if (!base.OnSerialize(cFG_SG_Node))
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("Count", m_RegisteredSerializables.Count);
		for (int i = 0; i < m_RegisteredSerializables.Count; i++)
		{
			if (!(m_RegisteredSerializables[i] == null) && m_RegisteredSerializables[i].IsUniqueID_OK)
			{
				string attName = "Value" + i;
				cFG_SG_Node.Attrib_Set(attName, m_RegisteredSerializables[i].UniqueID);
			}
		}
		return true;
	}

	public override bool OnDeSerialize(CFG_SG_Node _FlowObject)
	{
		if (!base.OnDeSerialize(_FlowObject))
		{
			return false;
		}
		int num = _FlowObject.Attrib_Get("Count", 0);
		if (num == 0)
		{
			return true;
		}
		m_RegisteredSerializables.Clear();
		int num2 = 0;
		List<CFGSerializableObject> list = new List<CFGSerializableObject>();
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			string attName = "Value" + i;
			int num3 = _FlowObject.Attrib_Get(attName, 0);
			if (num3 != 0)
			{
				CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(num3, ESerializableType.NotSerializable);
				if (cFGSerializableObject == null)
				{
					LogWarning("Failed to find serializable object: " + num3);
					continue;
				}
				RegisterEvent(cFGSerializableObject);
				num2++;
			}
		}
		if (num2 != num)
		{
			LogWarning("Read " + num2 + " event objects instead of " + num + " present at serialization");
		}
		return true;
	}

	public override bool OnPostLoad()
	{
		return base.OnPostLoad();
	}
}
