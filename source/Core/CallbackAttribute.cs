using System;

namespace Core;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CallbackAttribute : MarkAttribute
{
	protected CallbackType m_CallbackType;

	public CallbackType CallType => m_CallbackType;

	public CallbackAttribute(CallbackType type)
		: base(null)
	{
		m_CallbackType = type;
	}
}
