using System.IO;
using System.Text;

namespace MP;

public class AtomicBinaryReader
{
	private readonly object locker = new object();

	private BinaryReader reader;

	public long StreamLength => reader.BaseStream.Length;

	public AtomicBinaryReader(Stream stream)
		: this(stream, Encoding.UTF8)
	{
	}

	public AtomicBinaryReader(Stream stream, Encoding encoding)
	{
		reader = new BinaryReader(stream, encoding);
	}

	public void Close()
	{
		if (reader != null)
		{
			reader.Close();
		}
	}

	public long BytesLeft(long offset)
	{
		return reader.BaseStream.Length - offset;
	}

	public int Read(ref long offset, byte[] buffer, int index, int count)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int num = reader.Read(buffer, index, count);
			offset += num;
			return num;
		}
	}

	public int Read(ref long offset, uint[] buffer, int index, int count)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int i = 0;
			try
			{
				for (; i < count; i++)
				{
					buffer[index + i] = reader.ReadUInt32();
				}
			}
			catch (EndOfStreamException)
			{
			}
			return i;
		}
	}

	public byte ReadByte(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			byte result = reader.ReadByte();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public sbyte ReadSByte(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			sbyte result = reader.ReadSByte();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public short ReadInt16(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			short result = reader.ReadInt16();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public ushort ReadUInt16(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			ushort result = reader.ReadUInt16();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public int ReadInt32(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int result = reader.ReadInt32();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public uint ReadUInt32(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			uint result = reader.ReadUInt32();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public long ReadInt64(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			long result = reader.ReadInt64();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public ulong ReadUInt64(ref long offset)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			ulong result = reader.ReadUInt64();
			offset = reader.BaseStream.Position;
			return result;
		}
	}

	public byte[] ReadBytes(ref long offset, int count)
	{
		lock (locker)
		{
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			byte[] result = reader.ReadBytes(count);
			offset = reader.BaseStream.Position;
			return result;
		}
	}
}
