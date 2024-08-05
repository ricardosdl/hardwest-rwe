using System;
using System.Runtime.Serialization;

namespace MP;

public class RiffWriterException : ApplicationException
{
	public RiffWriterException()
	{
	}

	public RiffWriterException(string msg)
		: base(msg)
	{
	}

	public RiffWriterException(string msg, Exception inner)
		: base(msg, inner)
	{
	}

	public RiffWriterException(SerializationInfo info, StreamingContext ctx)
		: base(info, ctx)
	{
	}
}
