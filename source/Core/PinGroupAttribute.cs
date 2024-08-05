using System;

namespace Core;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public abstract class PinGroupAttribute : PinMarkAttribute
{
	public int Min;

	public PinGroupAttribute()
	{
	}
}
