using System;

public class CFGFlowProperty : Attribute
{
	public Type expectedType;

	public string displayName;

	public bool bWritable;

	public byte maxLinks = 1;
}
