using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CFGVar
{
	public CFGVarDef definition;

	public virtual object Value { get; set; }

	public string VariableName => definition.variableName;

	public virtual string ValueString
	{
		get
		{
			if (Value != null)
			{
				if (Value is IList)
				{
					return "[Count: " + (Value as IList).Count + "]";
				}
				return Value.ToString();
			}
			return "NULL";
		}
	}

	public abstract string GetVariableTypeName();

	public static CFGVar<T> Create<T>(T value, CFGVarDef definiton)
	{
		return new CFGVar<T>(value, definiton);
	}

	public bool OnSerialize(CFG_SG_Node nd)
	{
		nd.Attrib_Set("ID", definition.ID);
		return HandleSerialize(nd);
	}

	public abstract bool OnDeserialize(CFG_SG_Node nd);

	protected abstract bool HandleSerialize(CFG_SG_Node nd);
}
public class CFGVar<T> : CFGVar
{
	public T value;

	public override object Value
	{
		get
		{
			return value;
		}
		set
		{
			if (value == null)
			{
				Debug.LogWarning("CFGVar::SetValue " + definition.ID + ": value was null");
				return;
			}
			if (value is T)
			{
				this.value = (T)value;
				return;
			}
			Debug.LogError(string.Concat("InvalidCast from ", value.GetType(), " to ", typeof(T)));
		}
	}

	public CFGVar(T value, CFGVarDef definition)
	{
		base.definition = definition;
		if (typeof(T).IsValueType || typeof(T) == typeof(string) || typeof(T).IsClassOrSubclassOf(typeof(UnityEngine.Object)))
		{
			this.value = value;
			return;
		}
		this.value = (T)Activator.CreateInstance(typeof(T), value);
	}

	public override string GetVariableTypeName()
	{
		return value.GetType().PrettyName();
	}

	protected override bool HandleSerialize(CFG_SG_Node nd)
	{
		if (typeof(T) == typeof(UnityEngine.Object))
		{
			UnityEngine.Object @object = value as UnityEngine.Object;
			CFGSerializableObject cFGSerializableObject = null;
			int num = 0;
			if (@object != null)
			{
				cFGSerializableObject = @object as CFGSerializableObject;
				if (cFGSerializableObject == null)
				{
					GameObject gameObject = value as GameObject;
					if (gameObject != null)
					{
						cFGSerializableObject = gameObject.GetComponent<CFGSerializableObject>();
						if (cFGSerializableObject == null)
						{
							cFGSerializableObject = gameObject.GetComponentInChildren<CFGSerializableObject>();
						}
					}
				}
			}
			if (cFGSerializableObject != null)
			{
				num = cFGSerializableObject.UniqueID;
			}
			nd.Attrib_Set("Owner", num);
			return true;
		}
		if (typeof(T) == typeof(List<int>))
		{
			List<int> list = value as List<int>;
			string text = string.Empty;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					text += list[i];
					if (i < list.Count - 1)
					{
						text += ";";
					}
				}
			}
			nd.Attrib_Set("INTLST", text);
			return true;
		}
		if (typeof(T) == typeof(List<float>))
		{
			List<float> list2 = value as List<float>;
			string text2 = string.Empty;
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					text2 += list2[j];
					if (j < list2.Count - 1)
					{
						text2 += ";";
					}
				}
			}
			nd.Attrib_Set("FLTLST", text2);
			return true;
		}
		if (typeof(T) == typeof(List<string>))
		{
			List<string> list3 = value as List<string>;
			string text3 = string.Empty;
			if (list3 != null)
			{
				for (int k = 0; k < list3.Count; k++)
				{
					text3 += list3[k];
					if (k < list3.Count - 1)
					{
						text3 += "`";
					}
				}
			}
			nd.Attrib_Set("STRLST", text3);
			return true;
		}
		nd.Attrib_Set("Value", value);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node nd)
	{
		if (nd.AttribExists("Owner"))
		{
			int num = nd.Attrib_Get("Owner", 0);
			if (num != 0)
			{
				CFGSerializableObject cFGSerializableObject = CFGSingletonResourcePrefab<CFGObjectManager>.Instance.FindSerializableGO<CFGSerializableObject>(num, ESerializableType.NotSerializable);
				Value = cFGSerializableObject;
			}
			return true;
		}
		if (nd.AttribExists("INTLST"))
		{
			string text = nd.Attrib_Get("INTLST", string.Empty);
			List<int> list = new List<int>();
			if (list == null)
			{
				return false;
			}
			string[] array = text.Split(';');
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					int result = 0;
					int.TryParse(array[i], out result);
					list.Add(result);
				}
			}
			Value = list;
			return true;
		}
		if (nd.AttribExists("FLTLST"))
		{
			string text2 = nd.Attrib_Get("FLTLST", string.Empty);
			List<float> list2 = new List<float>();
			if (list2 == null)
			{
				return false;
			}
			string[] array2 = text2.Split(';');
			if (array2 != null && array2.Length > 0)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					float result2 = 0f;
					float.TryParse(array2[j], out result2);
					list2.Add(result2);
				}
			}
			Value = list2;
			return true;
		}
		if (nd.AttribExists("STRLST"))
		{
			string text3 = nd.Attrib_Get("STRLST", string.Empty);
			List<string> list3 = new List<string>();
			if (list3 == null)
			{
				return false;
			}
			string[] array3 = text3.Split('`');
			if (array3 != null && array3.Length > 0)
			{
				for (int k = 0; k < array3.Length; k++)
				{
					list3.Add(array3[k]);
				}
			}
			Value = list3;
			return true;
		}
		return nd.Attrib_Get("Value", ref value);
	}
}
