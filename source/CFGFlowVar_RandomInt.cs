using System;
using UnityEngine;

public class CFGFlowVar_RandomInt : CFGFlowVar_Int
{
	public int m_Min;

	public int m_Max = 100;

	public override object GetVariableOfType(Type varType)
	{
		return UnityEngine.Random.Range(m_Min, m_Max);
	}

	public override void SetVariable(object varObj)
	{
	}

	public override string GetTypeName()
	{
		return "Int " + m_Min + ".." + m_Max;
	}
}
