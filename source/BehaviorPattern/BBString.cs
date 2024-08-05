using System;

namespace BehaviorPattern;

[Serializable]
public class BBString : BBVariable<string>
{
	public BBString()
	{
		Value = string.Empty;
	}
}
