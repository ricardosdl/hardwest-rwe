using System;
using UnityEngine;

[Obsolete]
public class CFGFlowVar_Named : CFGFlowVariable
{
	public string m_FindVarName = string.Empty;

	[HideInInspector]
	public CFGFlowVariable m_RefVar;

	public override string GetTypeName()
	{
		if (m_RefVar != null)
		{
			return "Named : " + m_RefVar.GetTypeName();
		}
		return "Named";
	}

	public new static bool SupportsType(Type varType)
	{
		return false;
	}

	public override object GetVariableOfType(Type varType)
	{
		if (m_RefVar != null)
		{
			return m_RefVar.GetVariableOfType(varType);
		}
		return base.GetVariableOfType(varType);
	}

	public override void SetVariable(object varObj)
	{
		if (m_RefVar != null)
		{
			m_RefVar.SetVariable(varObj);
		}
	}

	public override string GetValueName()
	{
		if (m_RefVar != null)
		{
			return m_RefVar.GetValueName();
		}
		return "None";
	}

	public override void OnReload()
	{
	}

	public bool RefMatch(Type type)
	{
		if (m_RefVar != null && (m_RefVar.GetType() == type || m_RefVar.GetType().IsSubclassOf(type)))
		{
			return true;
		}
		return false;
	}

	public override string GetDisplayName()
	{
		return "{" + m_FindVarName + "}";
	}
}
