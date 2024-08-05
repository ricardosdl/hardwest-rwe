using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CFGVarDef : ScriptableObject
{
	public string variableName;

	public bool readOnly;

	public bool dontSave;

	public string ID => GetID(variableName);

	public abstract object Value { get; set; }

	public abstract Type ValueType { get; }

	public virtual string ValueString => (Value == null) ? string.Empty : Value.ToString();

	public virtual void OnValueChanged()
	{
	}

	public abstract CFGVar InstantiateVariable();

	public abstract CFGVar InstantiateVariable(object customValue);

	public void Init(string name, object value = null)
	{
		variableName = name;
		if (value != null)
		{
			Value = value;
		}
	}

	public virtual string GetVariableTypeName()
	{
		return string.Empty;
	}

	private static CFGVarDef CreateVar(Type type, string name, object value = null)
	{
		CFGVarDef cFGVarDef = (CFGVarDef)ScriptableObject.CreateInstance(type);
		if (cFGVarDef == null)
		{
			throw new ArgumentException("Could not create definition for given type");
		}
		cFGVarDef.Init(name, value);
		return cFGVarDef;
	}

	public static CFGVarDef Create(string variableName, string typeName, object value = null)
	{
		Type variableTypeForString = GetVariableTypeForString(typeName);
		return CreateVar(variableTypeForString, variableName, value);
	}

	public static CFGVarDef Create(Type type, string name)
	{
		return CreateVar(type, name);
	}

	public static CFGVarDef Create(string name, Type type, object value = null)
	{
		return CreateVar(type, name, value);
	}

	public static Dictionary<string, Type> GetVariableTypes()
	{
		Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
		foreach (Type item in CFGClassUtil.AllSubClasses(typeof(CFGVarDef)))
		{
			if (item.IsObsolete() || item.IsAbstract || item.IsGenericType)
			{
				continue;
			}
			string text = string.Empty;
			Type baseType = item.BaseType;
			if (baseType.IsGenericType)
			{
				Type[] genericArguments = baseType.GetGenericArguments();
				if (genericArguments.Count() != 1)
				{
					continue;
				}
				Type t = genericArguments[0];
				if (baseType.GetGenericTypeDefinition() == typeof(CFGVarDef_Typed<>))
				{
					text = t.PrettyName();
				}
				if (baseType.GetGenericTypeDefinition() == typeof(CFGVarDef_TypedList<>))
				{
					text = t.PrettyName() + "List";
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(text, item);
			}
		}
		return dictionary;
	}

	public static Type GetVariableTypeForString(string typeString)
	{
		return typeString switch
		{
			"int" => typeof(CFGVarDef_Int), 
			"float" => typeof(CFGVarDef_Float), 
			"string" => typeof(CFGVarDef_String), 
			"bool" => typeof(CFGVarDef_Bool), 
			_ => throw new NotImplementedException("Not implemented value Type " + typeString + "."), 
		};
	}

	public static string GetID(string variableName)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			return string.Empty;
		}
		return variableName.ToLower();
	}
}
