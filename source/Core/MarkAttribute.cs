using System;

namespace Core;

public abstract class MarkAttribute : Attribute
{
	public string Name;

	public ReflectOptions Options;

	public Type TypeArg;

	public string StringArg;

	public bool BoolArg;

	public MarkAttribute(string name)
	{
		Name = name;
	}
}
