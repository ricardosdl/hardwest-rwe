using System.Collections.Generic;

public class CFGUniversalMethods : CFGSequencer
{
	[CFGFlowCode(Category = "Texts")]
	public string Add(string lhs, string rhs)
	{
		lhs = ((lhs != null) ? lhs : string.Empty);
		rhs = ((rhs != null) ? rhs : string.Empty);
		return lhs + rhs;
	}

	[CFGFlowCode(Category = "Texts")]
	public string ConcatenateStringList(List<string> strings)
	{
		string text = string.Empty;
		foreach (string @string in strings)
		{
			string text2 = ((@string != null) ? @string : string.Empty);
			text += text2;
		}
		return text;
	}

	[CFGFlowCode(Category = "Texts")]
	public string GetLocalizedText(string text_id)
	{
		return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id);
	}

	[CFGFlowCode(Category = "Texts")]
	public string GetLocalizedTextWithInts(string text_id, List<int> ints)
	{
		string[] array = new string[ints.Count];
		for (int i = 0; i < ints.Count; i++)
		{
			array[i] = ints[i].ToString();
		}
		return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id, array);
	}

	[CFGFlowCode(Category = "Texts")]
	public string GetLocalizedTextWithInt(string text_id, int integer)
	{
		return CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id, integer.ToString());
	}
}
