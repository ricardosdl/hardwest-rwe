using System.Collections.Generic;
using System.IO;

namespace MP;

public class RiffWriter
{
	public const uint RIFF4CC = 1179011410u;

	public const uint LIST4CC = 1414744396u;

	private BinaryWriter writer;

	private Stack<long> stack;

	public BinaryWriter binaryWriter => writer;

	public long currentElementSize => writer.BaseStream.Position - stack.Peek();

	public RiffWriter(Stream stream)
	{
		writer = new BinaryWriter(stream);
		stack = new Stack<long>();
	}

	public RiffWriter(BinaryWriter writer)
	{
		this.writer = writer;
		stack = new Stack<long>();
	}

	public void BeginRiff(uint fourCC)
	{
		Begin(1179011410u, fourCC);
	}

	public void BeginList(uint fourCC)
	{
		Begin(1414744396u, fourCC);
	}

	public void BeginChunk(uint fourCC)
	{
		writer.Write(fourCC);
		stack.Push(writer.BaseStream.Position);
		writer.Write(0);
	}

	public void EndRiff()
	{
		End();
	}

	public void EndList()
	{
		End();
	}

	public void EndChunk()
	{
		End();
	}

	public void WriteChunk(uint fourCC, byte[] data, int size = -1)
	{
		if (size < 0)
		{
			size = data.Length;
		}
		writer.Write(fourCC);
		writer.Write(size);
		writer.Write(data, 0, size);
		if (size % 2 != 0)
		{
			writer.Write((byte)0);
		}
	}

	public void Close()
	{
		while (stack.Count > 0)
		{
			End();
		}
		writer.Close();
		writer.BaseStream.Close();
	}

	private void Begin(uint what, uint fourCC)
	{
		writer.Write(what);
		stack.Push(writer.BaseStream.Position);
		writer.Write(0);
		writer.Write(fourCC);
	}

	private void End()
	{
		long num = writer.BaseStream.Position - stack.Pop();
		if (num > int.MaxValue)
		{
			throw new RiffWriterException("RIFF or LIST element too large for writing (" + num + " bytes)");
		}
		int num2 = (int)num;
		int num3 = num2 % 2;
		if (num3 > 0)
		{
			writer.Write((byte)0);
		}
		writer.Seek(-num2 - num3, SeekOrigin.Current);
		writer.Write(num2 - 4);
		writer.Seek(num2 - 4 + num3, SeekOrigin.Current);
	}
}
