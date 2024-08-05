using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CFGScopedVariables
{
	public CFGVariableScope scope;

	public List<CFGVarDef> definitions = new List<CFGVarDef>();

	private Dictionary<string, CFGVar> _variables = new Dictionary<string, CFGVar>();

	private Dictionary<string, CFGVar> Variables
	{
		get
		{
			if (_variables == null)
			{
				ResetVariables();
			}
			return _variables;
		}
	}

	public CFGScopedVariables(CFGVariableScope scope)
	{
		if (scope == null)
		{
			Debug.LogError("Scope can not be null");
		}
		this.scope = scope;
		_variables = new Dictionary<string, CFGVar>();
	}

	public CFGVar GetVariable(string variableName)
	{
		CFGVar value = null;
		Variables.TryGetValue(CFGVarDef.GetID(variableName), out value);
		if (value == null)
		{
			Debug.LogError("Could not get variable: " + variableName + " from scope: " + scope.name);
		}
		return value;
	}

	public CFGVarDef GetDefiniton(string definitionName)
	{
		return definitions.FirstOrDefault((CFGVarDef x) => x.ID == CFGVarDef.GetID(definitionName));
	}

	public bool AddDefinition(CFGVarDef definition)
	{
		if (definition != null)
		{
			if (!definitions.Exists((CFGVarDef d) => d.ID == definition.ID))
			{
				definitions.Add(definition);
				if (Application.isPlaying && !Variables.ContainsKey(definition.ID))
				{
					CFGVar cFGVar = definition.InstantiateVariable();
					if (cFGVar == null)
					{
						return false;
					}
					Variables.Add(definition.ID, cFGVar);
				}
				return true;
			}
			Debug.LogError("Definition already exists");
		}
		else
		{
			Debug.LogError("Definition can not be null");
		}
		return false;
	}

	public bool RemoveDefinition(CFGVarDef definition)
	{
		bool result = definitions.Remove(definition);
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(definition);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(definition);
		}
		return result;
	}

	public void ResetVariables()
	{
		_variables = new Dictionary<string, CFGVar>();
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (CFGVarDef definition in definitions)
		{
			Variables.Add(definition.ID, definition.InstantiateVariable());
		}
	}

	public void Clear()
	{
		_variables.Clear();
		foreach (CFGVarDef definition in definitions)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(definition);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(definition);
			}
		}
		definitions.Clear();
	}

	public bool ContainsDefinition(string variableName)
	{
		return definitions.Exists((CFGVarDef d) => d.ID == CFGVarDef.GetID(variableName));
	}

	public bool ContainsVariable(string variableName)
	{
		return Variables.ContainsKey(CFGVarDef.GetID(variableName));
	}

	public bool OnSerialize(CFG_SG_Node nd)
	{
		if (nd == null)
		{
			return false;
		}
		if (_variables == null)
		{
			return true;
		}
		foreach (KeyValuePair<string, CFGVar> variable in _variables)
		{
			if (variable.Key != null && variable.Value != null)
			{
				CFG_SG_Node cFG_SG_Node = nd.AddSubNode("Item");
				if (cFG_SG_Node == null)
				{
					return false;
				}
				if (!variable.Value.OnSerialize(cFG_SG_Node))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool OnDeserialize(CFG_SG_Node nd)
	{
		if (nd == null)
		{
			Debug.LogWarning("NULL node!");
			return false;
		}
		if (_variables == null)
		{
			Debug.LogWarning("No vars to deserialize!");
			return true;
		}
		for (int i = 0; i < nd.SubNodeCount; i++)
		{
			CFG_SG_Node subNode = nd.GetSubNode(i);
			if (subNode == null || string.Compare(subNode.Name, "Item", ignoreCase: true) != 0)
			{
				continue;
			}
			string text = subNode.Attrib_Get<string>("ID", null);
			if (text == null)
			{
				Debug.LogWarning("Failed to find variable name: " + subNode.Name);
				continue;
			}
			CFGVar variable = GetVariable(text);
			if (variable == null)
			{
				Debug.LogWarning("Failed to find variable to deserialize: " + text);
			}
			else if (!variable.OnDeserialize(subNode))
			{
				Debug.LogWarning("Failed to deserialize variable: " + text);
				return false;
			}
		}
		return true;
	}
}
