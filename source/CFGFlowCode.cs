using System;

[AttributeUsage(AttributeTargets.All, Inherited = true)]
public class CFGFlowCode : Attribute
{
	public string[] OutputNames;

	public FlowCodeType CodeType;

	public bool IsLatent;

	public string Title;

	public string Category;

	public bool HideSetter;

	public bool HideGetter;

	public CFGFlowCode()
	{
		OutputNames = new string[0];
		Category = string.Empty;
	}
}
