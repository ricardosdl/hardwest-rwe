using System.Collections.Generic;

namespace MP.AVI;

public class AviStreamIndex
{
	public class Entry
	{
		public long chunkOffset;

		public int chunkLength;

		public bool isKeyframe;
	}

	public enum Type
	{
		SUPERINDEX = 0,
		CHUNKS = 1,
		DATA = 0x80
	}

	public uint streamId;

	public List<Entry> entries = new List<Entry>();

	public int globalOffset;
}
