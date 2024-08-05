using System;
using System.Runtime.Serialization;

namespace MP;

public class RiffParserException : ApplicationException
{
	public RiffParserException()
	{
	}

	public RiffParserException(string msg)
		: base(msg)
	{
	}

	public RiffParserException(string msg, Exception inner)
		: base(msg, inner)
	{
	}

	public RiffParserException(SerializationInfo info, StreamingContext ctx)
		: base(info, ctx)
	{
	}
}
