using System;

public class CFGTableField : Attribute
{
	public string ColumnName = string.Empty;

	public object DefaultValue;

	public Type ImportType;

	public int ColumnID = -1;

	public Type ListObject;

	public int Width = 100;

	public bool Optional;
}
