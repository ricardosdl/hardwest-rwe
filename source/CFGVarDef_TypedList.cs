using System.Collections.Generic;

public abstract class CFGVarDef_TypedList<T> : CFGVarDef_Typed<List<T>>
{
	public override object Value
	{
		get
		{
			if (value == null)
			{
				value = new List<T>();
			}
			return value;
		}
		set
		{
			base.value = (List<T>)value;
		}
	}

	public override string ValueString
	{
		get
		{
			if (value != null)
			{
				return "[Count: " + value.Count + "]";
			}
			return "NULL";
		}
	}
}
