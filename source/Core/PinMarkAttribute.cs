using System;

namespace Core;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public abstract class PinMarkAttribute : MarkAttribute
{
	public string Ref;

	public bool HideRef;

	public PinMarkAttribute()
		: base(null)
	{
	}
}
