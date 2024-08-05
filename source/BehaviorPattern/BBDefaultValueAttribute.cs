using System;

namespace BehaviorPattern;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class BBDefaultValueAttribute : Attribute
{
	public object DefaultValue;

	public BBDefaultValueAttribute(float value)
	{
		DefaultValue = value;
	}
}
