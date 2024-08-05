using System.Linq;

public class CFGFlowAct_SetReference : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "ScopeID", expectedType = typeof(CFGFlowVar_String))]
	public string scopeID;

	[CFGFlowProperty(displayName = "VariableID", expectedType = typeof(CFGFlowVar_String))]
	public string variableID;

	[CFGFlowProperty(displayName = "Reference", expectedType = typeof(CFGFlowVar_Ref), bWritable = true)]
	public object varRef;

	public override void Activated()
	{
		CFGFlowConn_Var varConnByFieldName = GetVarConnByFieldName("varRef");
		if (varConnByFieldName.IsLinkedToSingleVariable())
		{
			CFGFlowVar_Ref cFGFlowVar_Ref = varConnByFieldName.m_VarLinks[0] as CFGFlowVar_Ref;
			if (cFGFlowVar_Ref == null)
			{
				LogError("SetReference -> Connected variable is not reference");
				return;
			}
			if (CFGVariableContainer.Instance.ContainsVariable(variableID, scopeID))
			{
				cFGFlowVar_Ref.referencedVariableID = CFGVarDef.GetID(variableID);
				cFGFlowVar_Ref.referencedVariableScopeID = CFGVariableScope.GetID(scopeID);
				return;
			}
			cFGFlowVar_Ref.referencedVariableID = string.Empty;
			cFGFlowVar_Ref.referencedVariableScopeID = string.Empty;
			LogError("SetReference -> Given variableID [" + variableID + "] and scopeID [" + scopeID + "]do not match any existing variable");
		}
		else
		{
			LogError("SetReference -> Reference not connected");
		}
	}

	public override void PublishVariableValues()
	{
	}

	public override void PopulateVariableValues()
	{
		CFGFlowConn_Var varConnByFieldName = GetVarConnByFieldName("scopeID");
		if (varConnByFieldName != null && varConnByFieldName.IsLinkedToSingleVariable())
		{
			CFGFlowVariable cFGFlowVariable = varConnByFieldName.m_VarLinks.First() as CFGFlowVariable;
			scopeID = (string)cFGFlowVariable.GetVariableOfType(varConnByFieldName.m_ValueType.SystemType);
		}
		varConnByFieldName = GetVarConnByFieldName("variableID");
		if (varConnByFieldName != null && varConnByFieldName.IsLinkedToSingleVariable())
		{
			CFGFlowVariable cFGFlowVariable = varConnByFieldName.m_VarLinks.First() as CFGFlowVariable;
			variableID = (string)cFGFlowVariable.GetVariableOfType(varConnByFieldName.m_ValueType.SystemType);
		}
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_SetReference);
		flowActionInfo.DisplayName = "Set Reference";
		flowActionInfo.CategoryName = "Misc";
		return flowActionInfo;
	}
}
