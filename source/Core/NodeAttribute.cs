using System;

namespace Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class NodeAttribute : ObjectMarkAttribute
{
	public NodeAttribute(string name)
		: base(name)
	{
	}
}
