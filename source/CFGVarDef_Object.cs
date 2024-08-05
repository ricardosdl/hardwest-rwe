using System;
using System.Linq;
using UnityEngine;

public class CFGVarDef_Object : CFGVarDef_Typed<UnityEngine.Object>
{
	[SerializeField]
	public CFGType objectType;

	public override Type ValueType
	{
		get
		{
			if (objectType == null)
			{
				SetValueType(value);
			}
			return objectType.SystemType;
		}
	}

	public override void OnValueChanged()
	{
		base.OnValueChanged();
		SetValueType(value);
	}

	public void SetValueType(object value)
	{
		if (value != null)
		{
			SetValueType(value.GetType());
		}
		else
		{
			objectType = new CFGType(base.ValueType);
		}
	}

	public void SetValueType(Type newType)
	{
		if (newType == typeof(GameObject))
		{
			CFGGameObject cFGGameObject = (value as GameObject).GetComponents<CFGGameObject>().FirstOrDefault();
			objectType = ((!(cFGGameObject != null)) ? new CFGType(newType) : new CFGType(cFGGameObject.GetType()));
		}
		else
		{
			objectType = new CFGType(newType);
		}
	}

	public override string GetVariableTypeName()
	{
		return "GameObject";
	}
}
