using System;
using UnityEngine;

public abstract class CFGVarDef_Typed<T> : CFGVarDef
{
	public T value = default(T);

	public override object Value
	{
		get
		{
			return value;
		}
		set
		{
			if (value is T)
			{
				this.value = (T)value;
				OnValueChanged();
				return;
			}
			Debug.LogError(string.Concat("Could not cast [", value, "] to ", typeof(T).Name, " for ", base.ID));
		}
	}

	public override Type ValueType => typeof(T);

	public override CFGVar InstantiateVariable()
	{
		return CFGVar.Create(value, this);
	}

	public override CFGVar InstantiateVariable(object customValue)
	{
		if (customValue is T)
		{
			return CFGVar.Create(customValue, this);
		}
		Debug.LogError(string.Concat("CFGVarDef::InstantiateVariable(object customValue) -> custom value does not match definition DefType: ", typeof(T), " CustomValueType: ", customValue.GetType()));
		return null;
	}
}
