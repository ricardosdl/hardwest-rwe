public class CFGVarDef_String : CFGVarDef_Typed<string>
{
	public override object Value
	{
		get
		{
			if (value == null)
			{
				value = string.Empty;
			}
			return value;
		}
		set
		{
			base.value = (string)value;
		}
	}

	public override string GetVariableTypeName()
	{
		return "string";
	}
}
