using System;

namespace BehaviorPattern;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public abstract class BehaviorAttribute : Attribute
{
	public BehaviorAttribute()
	{
	}

	public BehaviorAttribute(string name)
	{
	}
}
