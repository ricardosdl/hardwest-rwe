using System;

namespace BehaviorPattern;

public class ParamAttribute : BehaviorAttribute
{
	public string ParamName;

	public Type ParamType;

	public ParamAttribute(Type type, string name)
	{
		ParamName = name;
		ParamType = type;
	}
}
