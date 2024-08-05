using UnityEngine;

public class CFGVarDef_Bool : CFGVarDef_Typed<bool>
{
	public override object Value
	{
		get
		{
			return value;
		}
		set
		{
			if (!bool.TryParse(value.ToString(), out base.value))
			{
				Debug.LogWarning($"Could not parse \"{value.ToString()}\" to bool - value will be set to FALSE");
			}
		}
	}

	public override string GetVariableTypeName()
	{
		return "bool";
	}
}
