using System;
using System.Collections.Generic;
using UnityEngine;

public class CFGFlowAct_PeonAction : CFGFlowGameAction
{
	[CFGFlowProperty(displayName = "Risk", expectedType = typeof(CFGFlowVar_String))]
	public string risk;

	[CFGFlowProperty(displayName = "Req. Manpower", expectedType = typeof(CFGFlowVar_Int))]
	public int reqManpower;

	[CFGFlowProperty(displayName = "Sick/Dead chances", expectedType = typeof(CFGFlowVar_StringList))]
	public List<string> sickDeadChances;

	[CFGFlowProperty(displayName = "Success chances", expectedType = typeof(CFGFlowVar_IntList))]
	public List<int> successChances;

	[CFGFlowProperty(displayName = "Sick", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int sick;

	[CFGFlowProperty(displayName = "Dead", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int dead;

	[CFGFlowProperty(displayName = "Losess", expectedType = typeof(CFGFlowVar_Int), bWritable = true)]
	public int losses;

	[CFGFlowProperty(displayName = "Success", expectedType = typeof(CFGFlowVar_Bool), bWritable = true)]
	public bool success;

	private readonly string[] _riskStrings = new string[7] { "none", "s_low", "s_medium", "s_high", "i_low", "i_medium", "i_high" };

	public override void Activated()
	{
		sick = 0;
		dead = 0;
		losses = 0;
		success = false;
		if (string.Equals(risk, _riskStrings[0], StringComparison.OrdinalIgnoreCase))
		{
			success = true;
			return;
		}
		if (sickDeadChances == null || sickDeadChances.Count != _riskStrings.Length - 1 || successChances.Count != _riskStrings.Length - 1)
		{
			Debug.LogError("Invalid RiskValues");
			return;
		}
		int num = Array.FindIndex(_riskStrings, (string i) => string.Equals(risk, i, StringComparison.OrdinalIgnoreCase));
		if (num == -1)
		{
			Debug.LogError("Risk not found in RiskValues");
			return;
		}
		string text = sickDeadChances[num - 1];
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("Invalid RiskValue on index " + (num - 1));
			return;
		}
		if (!Calculate(text, reqManpower, out sick))
		{
			Debug.LogError("Could not calculate sick");
			return;
		}
		if (sick != 0 && !Calculate(text, sick, out dead))
		{
			Debug.LogError("Could not calculate dead");
			return;
		}
		sick -= dead;
		losses = sick + dead;
		int num2 = successChances[num - 1];
		success = num2 <= UnityEngine.Random.Range(0, 101);
	}

	private static bool Calculate(string range, int of, out int value)
	{
		value = 0;
		if (of <= 0)
		{
			return false;
		}
		if (string.IsNullOrEmpty(range))
		{
			return false;
		}
		string[] array = range.Split(';');
		if (array.Length != 2)
		{
			return false;
		}
		if (!int.TryParse(array[0], out var result))
		{
			return false;
		}
		if (!int.TryParse(array[1], out var result2))
		{
			return false;
		}
		value = (int)((double)of * ((double)UnityEngine.Random.Range(result, result2 + 1) / 100.0));
		return true;
	}

	public new static FlowActionInfo GetActionInfo()
	{
		FlowActionInfo flowActionInfo = new FlowActionInfo();
		flowActionInfo.Type = typeof(CFGFlowAct_PeonAction);
		flowActionInfo.DisplayName = "S6_PeonAction";
		flowActionInfo.CategoryName = "Custom";
		return flowActionInfo;
	}
}
