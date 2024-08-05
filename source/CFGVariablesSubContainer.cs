using UnityEngine;

public class CFGVariablesSubContainer : MonoBehaviour
{
	public CFGScopedVariables scoped;

	public void RemoveInvalidDefinitions()
	{
		scoped.definitions.RemoveAll((CFGVarDef item) => item == null);
	}

	public bool OnSerialize(CFG_SG_Node nd)
	{
		return true;
	}

	public bool OnDeserialize(CFG_SG_Node nd)
	{
		return true;
	}
}
