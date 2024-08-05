namespace MP;

public class RiffParser
{
	public delegate bool ProcessRiffElement(RiffParser rp, uint fourCC, int length);

	public delegate bool ProcessListElement(RiffParser rp, uint fourCC, int length);

	public delegate void ProcessChunkElement(RiffParser rp, uint fourCC, int unpaddedLength, int paddedLength);

	public const uint RIFF4CC = 1179011410u;

	public const uint RIFX4CC = 1481001298u;

	public const uint LIST4CC = 1414744396u;

	private long nextElementOffset;

	public readonly AtomicBinaryReader reader;

	private uint streamRiff;

	public uint StreamRIFF => streamRiff;

	public long Position => nextElementOffset;

	public RiffParser(AtomicBinaryReader reader)
	{
		this.reader = reader;
		nextElementOffset = 0L;
		long offset = 0L;
		streamRiff = reader.ReadUInt32(ref offset);
		if (streamRiff != 1179011410 && streamRiff != 1481001298)
		{
			throw new RiffParserException("Error. Not a valid RIFF stream");
		}
	}

	public bool ReadNext(ProcessChunkElement chunkCallback, ProcessListElement listCallback = null, ProcessRiffElement riffCallback = null)
	{
		if (reader.BytesLeft(nextElementOffset) < 8)
		{
			return false;
		}
		uint num = reader.ReadUInt32(ref nextElementOffset);
		int num2 = reader.ReadInt32(ref nextElementOffset);
		if (reader.BytesLeft(nextElementOffset) < num2)
		{
			nextElementOffset = num2;
			throw new RiffParserException("Element size mismatch for element " + FromFourCC(num) + " need " + num2);
		}
		switch (num)
		{
		case 1179011410u:
		case 1481001298u:
			num = reader.ReadUInt32(ref nextElementOffset);
			if (reader.StreamLength < nextElementOffset + num2 - 4)
			{
				throw new RiffParserException("Error. Truncated stream");
			}
			if (riffCallback != null && !riffCallback(this, num, num2 - 4))
			{
				nextElementOffset += num2 - 4;
			}
			break;
		case 1414744396u:
			num = reader.ReadUInt32(ref nextElementOffset);
			if (listCallback != null && !listCallback(this, num, num2 - 4))
			{
				nextElementOffset += num2 - 4;
			}
			break;
		default:
		{
			int num3 = num2;
			if (((uint)num2 & (true ? 1u : 0u)) != 0)
			{
				num3++;
			}
			chunkCallback?.Invoke(this, num, num2, num3);
			nextElementOffset += num3;
			break;
		}
		}
		return true;
	}

	public void Rewind()
	{
		nextElementOffset = 0L;
	}

	public static string FromFourCC(uint fourCC)
	{
		return new string(new char[4]
		{
			(char)(fourCC & 0xFFu),
			(char)((fourCC >> 8) & 0xFFu),
			(char)((fourCC >> 16) & 0xFFu),
			(char)((fourCC >> 24) & 0xFFu)
		});
	}

	public static uint ToFourCC(string fourCC)
	{
		if (fourCC.Length != 4)
		{
			throw new RiffParserException("FourCC strings must be 4 characters long " + fourCC);
		}
		return ((uint)fourCC[3] << 24) | ((uint)fourCC[2] << 16) | ((uint)fourCC[1] << 8) | fourCC[0];
	}

	public static uint ToFourCC(char[] fourCC)
	{
		if (fourCC.Length != 4)
		{
			throw new RiffParserException("FourCC char arrays must be 4 characters long " + new string(fourCC));
		}
		return ((uint)fourCC[3] << 24) | ((uint)fourCC[2] << 16) | ((uint)fourCC[1] << 8) | fourCC[0];
	}

	public static uint ToFourCC(char c0, char c1, char c2, char c3)
	{
		return ((uint)c3 << 24) | ((uint)c2 << 16) | ((uint)c1 << 8) | c0;
	}
}
