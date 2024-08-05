using System;
using UnityEngine;

public class CFGFlowVar_Ref : CFGFlowVariable
{
	public string referencedVariableID = string.Empty;

	public string referencedVariableScopeID = string.Empty;

	public CFGVar Referenced => CFGVariableContainer.Instance.GetVariable(referencedVariableID, referencedVariableScopeID);

	public override object Value
	{
		get
		{
			if (Referenced != null)
			{
				return Referenced.Value;
			}
			return null;
		}
		set
		{
			Referenced.Value = value;
			if (string.Equals(referencedVariableScopeID, "profile", StringComparison.OrdinalIgnoreCase))
			{
				CFGVariableContainer.Instance.SaveValuesGlobal(null);
			}
		}
	}

	public CFGVarDef RefDef
	{
		get
		{
			if (IsEmptyReference())
			{
				return null;
			}
			return CFGVariableContainer.Instance.GetDefiniton(referencedVariableID, referencedVariableScopeID);
		}
	}

	public Type GetValueType()
	{
		if (Referenced != null)
		{
			if (Referenced.Value != null)
			{
				return Referenced.Value.GetType();
			}
			return Referenced.definition.ValueType;
		}
		return null;
	}

	public override void SetVariable(object value)
	{
		if (Referenced != null && !Referenced.definition.readOnly)
		{
			Referenced.Value = value;
			if (string.Equals(referencedVariableScopeID, "profile", StringComparison.OrdinalIgnoreCase))
			{
				CFGVariableContainer.Instance.SaveValuesGlobal(null);
			}
		}
	}

	public override object GetVariableOfType(Type varType)
	{
		if (varType == null)
		{
			return null;
		}
		if (Value == null)
		{
			return null;
		}
		if (Value.GetType() == varType)
		{
			return Value;
		}
		if (varType.IsClassOrSubclassOf(typeof(Component)))
		{
			return GetComponentFromValue(varType);
		}
		if (varType.IsClassOrSubclassOf(typeof(UnityEngine.Object)))
		{
			return Value;
		}
		return Value;
	}

	public Component GetComponentFromValue(Type componentType)
	{
		GameObject gameObject = null;
		if (Value is Component)
		{
			gameObject = (Value as Component).gameObject;
		}
		else if (Value is GameObject)
		{
			gameObject = Value as GameObject;
		}
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent(componentType);
	}

	public override string GetTypeName()
	{
		if (IsEmptyReference())
		{
			return "Ref: EMPTY";
		}
		if (CFGVariableContainer.Instance.ContainsDefinition(referencedVariableID, referencedVariableScopeID))
		{
			return "Ref: " + RefDef.GetVariableTypeName();
		}
		return "Ref: ERROR";
	}

	public override string GetValueName()
	{
		if (IsEmptyReference())
		{
			return string.Empty;
		}
		return ReferencedName() + "\n" + ReferencedValue();
	}

	public override bool IsOK()
	{
		if (IsEmptyReference())
		{
			return false;
		}
		if (!CFGVariableContainer.Instance.ContainsDefinition(referencedVariableID, referencedVariableScopeID))
		{
			return false;
		}
		if (!base.IsOK())
		{
			return false;
		}
		return true;
	}

	public string ReferencedName()
	{
		if (IsEmptyReference())
		{
			return string.Empty;
		}
		if (CFGVariableContainer.Instance.ContainsDefinition(referencedVariableID, referencedVariableScopeID))
		{
			CFGVarDef definiton = CFGVariableContainer.Instance.GetDefiniton(referencedVariableID, referencedVariableScopeID);
			return "Name: " + definiton.variableName + " [" + CFGVariableContainer.Instance.GetScope(referencedVariableScopeID).name + "]";
		}
		return "ID: " + referencedVariableID + "\nScopeID: " + referencedVariableScopeID;
	}

	public string ReferencedValue()
	{
		if (IsEmptyReference())
		{
			return string.Empty;
		}
		if (Application.isPlaying && CFGVariableContainer.Instance.ContainsVariable(referencedVariableID, referencedVariableScopeID))
		{
			return "Value: " + Referenced.ValueString;
		}
		if (CFGVariableContainer.Instance.ContainsDefinition(referencedVariableID, referencedVariableScopeID))
		{
			return "DefValue: " + RefDef.ValueString;
		}
		return string.Empty;
	}

	public bool IsEmptyReference()
	{
		if (string.IsNullOrEmpty(referencedVariableID) || string.IsNullOrEmpty(referencedVariableScopeID))
		{
			return true;
		}
		return false;
	}
}
