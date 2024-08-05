using System.Collections.Generic;

public class CFGFlowVar_IntList : CFGFlowVar_Typed<List<int>>
{
	public override string GetTypeName()
	{
		return "IntList";
	}
}
