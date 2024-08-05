using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFGFlowVar_ObjectList : CFGFlowVar_Typed<List<UnityEngine.Object>>
{
	[HideInInspector]
	public CFGType m_ListType;

	public new static bool SupportsType(Type varType)
	{
		if (varType.IsGenericType && varType.GetGenericTypeDefinition() == typeof(List<>) && (varType.GetProperty("Item").PropertyType.IsSubclassOf(typeof(UnityEngine.Object)) || varType.GetProperty("Item").PropertyType == typeof(UnityEngine.Object)))
		{
			return true;
		}
		return CFGFlowVar_Object.SupportsType(varType);
	}

	public override object GetVariableOfType(Type varType)
	{
		if (!varType.IsGenericList())
		{
			return null;
		}
		Type[] genericArguments = varType.GetGenericArguments();
		Type type = typeof(List<>).MakeGenericType(genericArguments[0]);
		IList list = (IList)Activator.CreateInstance(type);
		if (m_Value != null && list != null && genericArguments[0] != null)
		{
			if (genericArguments[0] == typeof(Component) || genericArguments[0].IsSubclassOf(typeof(Component)))
			{
				for (int i = 0; i < m_Value.Count; i++)
				{
					UnityEngine.Object @object = m_Value[i];
					if (@object is Component)
					{
						list.Add(@object);
					}
					else if (@object is GameObject)
					{
						GameObject gameObject = @object as GameObject;
						if (gameObject != null && genericArguments[0] != null)
						{
							list?.Add(gameObject.GetComponent(genericArguments[0]));
						}
					}
					else if (@object is UnityEngine.Object)
					{
						list.Add(null);
					}
					else if (@object == null)
					{
						list.Add(null);
					}
				}
				return list;
			}
			if (genericArguments[0] == typeof(GameObject))
			{
				for (int j = 0; j < m_Value.Count; j++)
				{
					UnityEngine.Object object2 = m_Value[j];
					if (object2 is Component)
					{
						list.Add((object2 as Component).gameObject);
					}
					else if (object2 is GameObject)
					{
						list.Add(object2);
					}
					else if (object2 is UnityEngine.Object)
					{
						list.Add(null);
					}
					else if (object2 == null)
					{
						list.Add(null);
					}
				}
				return list;
			}
			if (genericArguments[0] == typeof(UnityEngine.Object) || genericArguments[0].IsSubclassOf(typeof(UnityEngine.Object)))
			{
				for (int k = 0; k < m_Value.Count; k++)
				{
					UnityEngine.Object object3 = m_Value[k];
					if (object3 == null)
					{
						list.Add(null);
					}
					else
					{
						list.Add(object3);
					}
				}
				return list;
			}
		}
		return base.GetVariableOfType(varType);
	}

	public override void SetVariable(object varObj)
	{
		if (varObj == null)
		{
			return;
		}
		Type type = varObj.GetType();
		if (type == null || !type.IsGenericType)
		{
			LogWarning("trying to assign value that is not a list into a list flow container !");
		}
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		foreach (object item in (IEnumerable)varObj)
		{
			object obj = item;
			if (obj is Component)
			{
				obj = (obj as Component).gameObject;
			}
			list.Add(obj as UnityEngine.Object);
		}
		m_Value = list;
	}

	public override string GetTypeName()
	{
		return "ObjectList";
	}

	public override EFOSType GetFOS_Type()
	{
		return EFOSType.VarObjectList;
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
		cFG_SG_Node.Attrib_Set("Count", m_Value.Count);
		if (m_Value.Count == 0)
		{
			return true;
		}
		List<int> list = new List<int>();
		bool flag = true;
		for (int i = 0; i < m_Value.Count; i++)
		{
			UnityEngine.Object @object = m_Value[i];
			if (@object == null)
			{
				list.Add(0);
				continue;
			}
			GameObject gameObject = @object as GameObject;
			if (gameObject != null)
			{
				CFGSerializableObject component = gameObject.GetComponent<CFGSerializableObject>();
				if (component != null)
				{
					if (component.UniqueID == 0)
					{
						LogWarning("Serializable object has invalid uuid. Please regenerate uuids");
					}
					list.Add(component.UniqueID);
					continue;
				}
			}
			flag = false;
			break;
		}
		cFG_SG_Node.Attrib_Set("State", flag);
		if (!flag)
		{
			return true;
		}
		for (int j = 0; j < m_Value.Count; j++)
		{
			string attName = "Value" + j;
			cFG_SG_Node.Attrib_Set(attName, list[j]);
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
		if (!_FlowObject.Attrib_Get("State", DefVal: false))
		{
			LogError("List is not serializable");
			return true;
		}
		m_Value.Clear();
		for (int i = 0; i < num; i++)
		{
			string attName = "Value" + i;
			int _Value = 0;
			if (!_FlowObject.Attrib_Get(attName, ref _Value))
			{
				LogWarning("Failed to deserialize object list item #" + i + ": Missing )");
				m_Value.Add(null);
				continue;
			}
			CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(_Value, ESerializableType.NotSerializable);
			if (cFGSerializableObject == null)
			{
				LogWarning("Failed to deserialize object list item #" + i + ": Not Found on scene");
			}
			else
			{
				m_Value.Add(cFGSerializableObject.gameObject);
			}
		}
		return true;
	}
}
