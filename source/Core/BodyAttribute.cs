using System;
using System.Reflection;

namespace Core;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class BodyAttribute : MarkAttribute
{
	public string[] Tags = new string[0];

	public bool EditInline => Options.HasFlag(ReflectOptions.EditInline);

	public bool EditNew => Options.HasFlag(ReflectOptions.EditNew);

	public BodyAttribute()
		: base(null)
	{
	}

	public BodyAttribute(string name)
		: base(name)
	{
	}

	public MethodInfo GetMethodFromString(Type target)
	{
		if (StringArg != null)
		{
			return target.GetMethod(StringArg);
		}
		return null;
	}
}
