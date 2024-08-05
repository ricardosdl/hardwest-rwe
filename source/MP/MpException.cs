using System;
using System.Runtime.Serialization;

namespace MP;

public class MpException : ApplicationException
{
	public MpException()
	{
	}

	public MpException(string msg)
		: base(msg)
	{
	}

	public MpException(string msg, Exception inner)
		: base(msg, inner)
	{
	}

	public MpException(SerializationInfo info, StreamingContext ctx)
		: base(info, ctx)
	{
	}
}
