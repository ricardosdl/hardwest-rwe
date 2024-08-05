using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MP.AVI;

public class AviDemux : Demux
{
	public const uint ID_AVI_ = 541677121u;

	public const uint ID_AVIX = 1481201217u;

	public const uint ID_hdrl = 1819436136u;

	public const uint ID_avih = 1751742049u;

	public const uint ID_strl = 1819440243u;

	public const uint ID_strh = 1752331379u;

	public const uint ID_strf = 1718776947u;

	public const uint ID_odml = 1819108463u;

	public const uint ID_dmlh = 1751936356u;

	public const uint ID_movi = 1769369453u;

	public const uint ID_00dc = 1667510320u;

	public const uint ID_00db = 1650733104u;

	public const uint ID_01wb = 1651978544u;

	public const uint ID_idx1 = 829973609u;

	public const uint ID_indx = 2019847785u;

	public const uint FCC_vids = 1935960438u;

	public const uint FCC_auds = 1935963489u;

	public AVIFile avi;

	private AtomicBinaryReader reader;

	private uint currentStrh4CC;

	private long idx1EntryOffset;

	private long idx1Offset;

	private int idx1Size;

	private byte[] rawVideoBuf;

	private byte[] rawAudioBuf;

	private long[] audioByteIndex;

	private int nextVideoFrame;

	private int nextAudioSample;

	public override int VideoPosition
	{
		get
		{
			return nextVideoFrame;
		}
		set
		{
			nextVideoFrame = value;
		}
	}

	public override int AudioPosition
	{
		get
		{
			return nextAudioSample;
		}
		set
		{
			nextAudioSample = value;
		}
	}

	public override void Init(Stream sourceStream, LoadOptions loadOptions = null)
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		reader = new AtomicBinaryReader(sourceStream);
		RiffParser riffParser = new RiffParser(reader);
		avi = new AVIFile();
		idx1EntryOffset = -1L;
		idx1Offset = -1L;
		currentStrh4CC = 0u;
		while (riffParser.ReadNext(ProcessAviChunk, ProcessAviList, ProcessAviRiff))
		{
		}
		if (avi.strhVideo != null)
		{
			base.videoStreamInfo = new VideoStreamInfo();
			base.videoStreamInfo.codecFourCC = avi.strhVideo.fccHandler;
			base.videoStreamInfo.bitsPerPixel = avi.strfVideo.biBitCount;
			base.videoStreamInfo.frameCount = (int)((avi.odml == null) ? avi.avih.dwTotalFrames : avi.odml.dwTotalFrames);
			base.videoStreamInfo.width = (int)avi.avih.dwWidth;
			base.videoStreamInfo.height = (int)avi.avih.dwHeight;
			base.videoStreamInfo.framerate = (float)avi.strhVideo.dwRate / (float)avi.strhVideo.dwScale;
		}
		else
		{
			base.videoStreamInfo = null;
		}
		if (avi.strhAudio != null)
		{
			base.audioStreamInfo = new AudioStreamInfo();
			base.audioStreamInfo.codecFourCC = avi.strhAudio.fccHandler;
			base.audioStreamInfo.audioFormat = avi.strfAudio.wFormatTag;
			base.audioStreamInfo.sampleCount = (int)avi.strhAudio.dwLength;
			base.audioStreamInfo.sampleSize = (int)avi.strhAudio.dwSampleSize;
			base.audioStreamInfo.channels = avi.strfAudio.nChannels;
			base.audioStreamInfo.sampleRate = (int)avi.strfAudio.nSamplesPerSec;
		}
		else
		{
			base.audioStreamInfo = null;
		}
		if (base.hasVideo)
		{
			if (avi.videoIndex == null)
			{
				avi.videoIndex = ParseOldIndex(idx1Offset, riffParser.reader, idx1Size, 1667510320u, idx1EntryOffset);
			}
			if (avi.videoIndex == null)
			{
				throw new MpException("No video index found (required for playback and seeking)");
			}
			PrepareVideoStream();
		}
		if (base.hasAudio)
		{
			if (avi.audioIndex == null)
			{
				avi.audioIndex = ParseOldIndex(idx1Offset, riffParser.reader, idx1Size, 1651978544u, idx1EntryOffset);
			}
			if (avi.audioIndex == null)
			{
				throw new MpException("No audio index found (required for playback and seeking)");
			}
			PrepareAudioStream();
		}
		if (base.videoStreamInfo != null && avi.videoIndex != null && base.videoStreamInfo.frameCount > avi.videoIndex.entries.Count)
		{
			base.videoStreamInfo.frameCount = avi.videoIndex.entries.Count;
		}
		stopwatch.Stop();
		nextVideoFrame = 0;
		nextAudioSample = 0;
	}

	public override void Shutdown(bool force = false)
	{
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		targetBuf = rawVideoBuf;
		if (nextVideoFrame < 0 || nextVideoFrame >= base.videoStreamInfo.frameCount)
		{
			return 0;
		}
		AviStreamIndex.Entry entry = avi.videoIndex.entries[nextVideoFrame++];
		long offset = entry.chunkOffset;
		return reader.Read(ref offset, rawVideoBuf, 0, entry.chunkLength);
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		int sampleSize = base.audioStreamInfo.sampleSize;
		int num = sampleCount * sampleSize;
		if (rawAudioBuf == null || rawAudioBuf.Length < num)
		{
			rawAudioBuf = new byte[num];
		}
		long num2 = nextAudioSample * sampleSize;
		nextAudioSample += sampleCount;
		if (rawAudioBuf.Length < num)
		{
			throw new ArgumentException("array.Length < count");
		}
		int num3 = Array.BinarySearch(audioByteIndex, num2);
		if (num3 == -1)
		{
			throw new MpException("audioByteIndex is corrupted");
		}
		if (num3 < 0)
		{
			num3 = -num3 - 2;
		}
		int num4 = 0;
		long num5 = num2;
		int num6 = num;
		int num7 = 0;
		int num9;
		int num10;
		do
		{
			AviStreamIndex.Entry entry = avi.audioIndex.entries[num3];
			int num8 = (int)(num5 - audioByteIndex[num3]);
			num9 = entry.chunkLength - num8;
			if (num9 > num6)
			{
				num9 = num6;
			}
			long offset = entry.chunkOffset + num8;
			num10 = reader.Read(ref offset, rawAudioBuf, num4, num9);
			num7 += num10;
			num6 -= num10;
			num8 += num10;
			num4 += num10;
			num5 += num10;
			if (num8 >= entry.chunkLength)
			{
				num3++;
			}
		}
		while (num6 > 0 && num10 == num9 && num3 < audioByteIndex.Length);
		targetBuf = rawAudioBuf;
		return num7 / sampleSize;
	}

	private static AVIMainHeader ParseMainHeader(AtomicBinaryReader br, long p)
	{
		AVIMainHeader aVIMainHeader = new AVIMainHeader();
		aVIMainHeader.dwMicroSecPerFrame = br.ReadUInt32(ref p);
		aVIMainHeader.dwMaxBytesPerSec = br.ReadUInt32(ref p);
		aVIMainHeader.dwPaddingGranularity = br.ReadUInt32(ref p);
		aVIMainHeader.dwFlags = br.ReadUInt32(ref p);
		aVIMainHeader.dwTotalFrames = br.ReadUInt32(ref p);
		aVIMainHeader.dwInitialFrames = br.ReadUInt32(ref p);
		aVIMainHeader.dwStreams = br.ReadUInt32(ref p);
		aVIMainHeader.dwSuggestedBufferSize = br.ReadUInt32(ref p);
		aVIMainHeader.dwWidth = br.ReadUInt32(ref p);
		aVIMainHeader.dwHeight = br.ReadUInt32(ref p);
		aVIMainHeader.dwReserved0 = br.ReadUInt32(ref p);
		aVIMainHeader.dwReserved1 = br.ReadUInt32(ref p);
		aVIMainHeader.dwReserved2 = br.ReadUInt32(ref p);
		aVIMainHeader.dwReserved3 = br.ReadUInt32(ref p);
		return aVIMainHeader;
	}

	private static AVIStreamHeader ParseStreamHeader(AtomicBinaryReader br, long p)
	{
		AVIStreamHeader aVIStreamHeader = new AVIStreamHeader();
		aVIStreamHeader.fccType = br.ReadUInt32(ref p);
		aVIStreamHeader.fccHandler = br.ReadUInt32(ref p);
		aVIStreamHeader.dwFlags = br.ReadUInt32(ref p);
		aVIStreamHeader.wPriority = br.ReadUInt16(ref p);
		aVIStreamHeader.wLanguage = br.ReadUInt16(ref p);
		aVIStreamHeader.dwInitialFrames = br.ReadUInt32(ref p);
		aVIStreamHeader.dwScale = br.ReadUInt32(ref p);
		aVIStreamHeader.dwRate = br.ReadUInt32(ref p);
		aVIStreamHeader.dwStart = br.ReadUInt32(ref p);
		aVIStreamHeader.dwLength = br.ReadUInt32(ref p);
		aVIStreamHeader.dwSuggestedBufferSize = br.ReadUInt32(ref p);
		aVIStreamHeader.dwQuality = br.ReadUInt32(ref p);
		aVIStreamHeader.dwSampleSize = br.ReadUInt32(ref p);
		aVIStreamHeader.rcFrameLeft = br.ReadInt16(ref p);
		aVIStreamHeader.rcFrameTop = br.ReadInt16(ref p);
		aVIStreamHeader.rcFrameRight = br.ReadInt16(ref p);
		aVIStreamHeader.rcFrameBottom = br.ReadInt16(ref p);
		return aVIStreamHeader;
	}

	private static BitmapInfoHeader ParseVideoFormatHeader(AtomicBinaryReader br, long p)
	{
		BitmapInfoHeader bitmapInfoHeader = new BitmapInfoHeader();
		bitmapInfoHeader.biSize = br.ReadUInt32(ref p);
		bitmapInfoHeader.biWidth = br.ReadInt32(ref p);
		bitmapInfoHeader.biHeight = br.ReadInt32(ref p);
		bitmapInfoHeader.biPlanes = br.ReadUInt16(ref p);
		bitmapInfoHeader.biBitCount = br.ReadUInt16(ref p);
		bitmapInfoHeader.biCompression = br.ReadUInt32(ref p);
		bitmapInfoHeader.biSizeImage = br.ReadUInt32(ref p);
		bitmapInfoHeader.biXPelsPerMeter = br.ReadInt32(ref p);
		bitmapInfoHeader.biYPelsPerMeter = br.ReadInt32(ref p);
		bitmapInfoHeader.biClrUsed = br.ReadUInt32(ref p);
		bitmapInfoHeader.biClrImportant = br.ReadUInt32(ref p);
		return bitmapInfoHeader;
	}

	private static WaveFormatEx ParseAudioFormatHeader(AtomicBinaryReader br, long p)
	{
		WaveFormatEx waveFormatEx = new WaveFormatEx();
		waveFormatEx.wFormatTag = br.ReadUInt16(ref p);
		waveFormatEx.nChannels = br.ReadUInt16(ref p);
		waveFormatEx.nSamplesPerSec = br.ReadUInt32(ref p);
		waveFormatEx.nAvgBytesPerSec = br.ReadUInt32(ref p);
		waveFormatEx.nBlockAlign = br.ReadUInt16(ref p);
		waveFormatEx.wBitsPerSample = br.ReadUInt16(ref p);
		waveFormatEx.cbSize = br.ReadUInt16(ref p);
		return waveFormatEx;
	}

	private static ODMLHeader ParseOdmlHeader(AtomicBinaryReader br, long p)
	{
		ODMLHeader oDMLHeader = new ODMLHeader();
		oDMLHeader.dwTotalFrames = br.ReadUInt32(ref p);
		return oDMLHeader;
	}

	private static AviStreamIndex ParseOldIndex(long idx1Offset, AtomicBinaryReader abr, int size, uint streamId, long idx1EntryOffset)
	{
		int num = size / 16;
		AviStreamIndex aviStreamIndex = new AviStreamIndex();
		aviStreamIndex.streamId = streamId;
		aviStreamIndex.entries.Capacity = num;
		long offset = idx1Offset;
		uint[] array = new uint[num * 4];
		abr.Read(ref offset, array, 0, num * 4);
		for (int i = 0; i < num; i++)
		{
			uint num2 = array[i * 4];
			if (num2 == streamId || (num2 == 1650733104 && streamId == 1667510320))
			{
				AviStreamIndex.Entry entry = new AviStreamIndex.Entry();
				entry.isKeyframe = (array[i * 4 + 1] & 0x10) != 0;
				entry.chunkOffset = idx1EntryOffset + array[i * 4 + 2];
				entry.chunkLength = (int)array[i * 4 + 3];
				aviStreamIndex.entries.Add(entry);
			}
		}
		return aviStreamIndex;
	}

	private static AviStreamIndex ParseOdmlIndex(AtomicBinaryReader reader, long p, out uint streamId)
	{
		ushort num = reader.ReadUInt16(ref p);
		byte b = reader.ReadByte(ref p);
		byte b2 = reader.ReadByte(ref p);
		uint num2 = reader.ReadUInt32(ref p);
		streamId = reader.ReadUInt32(ref p);
		AviStreamIndex index = new AviStreamIndex();
		index.streamId = streamId;
		switch (b2)
		{
		case 0:
		{
			p += 12;
			if (b != 0 || num != 4)
			{
			}
			for (uint num3 = 0u; num3 < num2; num3++)
			{
				long num4 = reader.ReadInt64(ref p);
				int num5 = reader.ReadInt32(ref p);
				reader.ReadInt32(ref p);
				if (num4 != 0L)
				{
					long num6 = p;
					p = num4;
					index.entries.Capacity += num5 / 8;
					ParseChunkIndex(reader, p, ref index);
					p = num6;
				}
			}
			break;
		}
		case 1:
			ParseChunkIndex(reader, p - 20, ref index);
			break;
		default:
			throw new MpException("Unsupported index type " + b2 + " encountered for stream " + RiffParser.FromFourCC(streamId));
		}
		index.entries.TrimExcess();
		return index;
	}

	private static void ParseChunkIndex(AtomicBinaryReader reader, long p, ref AviStreamIndex index)
	{
		uint num = reader.ReadUInt32(ref p);
		uint num2 = (num & 0xFFFFu) | 0x20200000u;
		if (num2 != RiffParser.ToFourCC("ix  ") && num != RiffParser.ToFourCC("indx"))
		{
			throw new MpException("Unexpected chunk id for index " + RiffParser.FromFourCC(num) + " for stream " + RiffParser.FromFourCC(index.streamId));
		}
		uint num3 = reader.ReadUInt32(ref p);
		ushort num4 = reader.ReadUInt16(ref p);
		byte b = reader.ReadByte(ref p);
		byte b2 = reader.ReadByte(ref p);
		uint num5 = reader.ReadUInt32(ref p);
		uint num6 = reader.ReadUInt32(ref p);
		if (b2 != 1 || b != 0 || num6 != index.streamId || num4 != 2 || num3 < 4 * num4 * num5 + 24)
		{
			throw new MpException("Broken or unsupported index for stream " + RiffParser.FromFourCC(num6) + ". " + num6 + "!=" + index.streamId + ", wLongsPerEntry=" + num4 + ", bIndexType=" + b2 + ", bSubIndexType=" + b);
		}
		long num7 = reader.ReadInt64(ref p);
		p += 4;
		uint[] array = new uint[num5 * 2];
		reader.Read(ref p, array, 0, (int)(num5 * 2));
		for (int i = 0; i < num5; i++)
		{
			AviStreamIndex.Entry entry = new AviStreamIndex.Entry();
			entry.chunkOffset = num7 + array[2 * i];
			uint num8 = array[2 * i + 1];
			entry.chunkLength = (int)(num8 & 0x7FFFFFFF);
			if ((num8 & 0x80000000u) == 0)
			{
				entry.isKeyframe = true;
			}
			index.entries.Add(entry);
		}
	}

	private bool ProcessAviRiff(RiffParser rp, uint fourCC, int length)
	{
		if (fourCC != 541677121 && fourCC != 1481201217)
		{
			throw new MpException("Not an AVI");
		}
		return true;
	}

	private bool ProcessAviList(RiffParser rp, uint fourCC, int length)
	{
		if (fourCC == 1769369453 && idx1EntryOffset < 0)
		{
			idx1EntryOffset = rp.Position + 4;
		}
		return fourCC == 1819436136 || fourCC == 1819440243 || fourCC == 1819108463;
	}

	private void ProcessAviChunk(RiffParser rp, uint fourCC, int unpaddedLength, int paddedLength)
	{
		switch (fourCC)
		{
		case 1751742049u:
			avi.avih = ParseMainHeader(rp.reader, rp.Position);
			break;
		case 1752331379u:
		{
			AVIStreamHeader aVIStreamHeader = ParseStreamHeader(rp.reader, rp.Position);
			currentStrh4CC = aVIStreamHeader.fccType;
			if (currentStrh4CC == 1935960438)
			{
				avi.strhVideo = aVIStreamHeader;
			}
			else if (currentStrh4CC == 1935963489)
			{
				avi.strhAudio = aVIStreamHeader;
			}
			break;
		}
		case 1718776947u:
			if (currentStrh4CC == 1935960438)
			{
				avi.strfVideo = ParseVideoFormatHeader(rp.reader, rp.Position);
			}
			else if (currentStrh4CC == 1935963489)
			{
				avi.strfAudio = ParseAudioFormatHeader(rp.reader, rp.Position);
			}
			break;
		case 829973609u:
			idx1Offset = rp.Position;
			idx1Size = unpaddedLength;
			break;
		case 1751936356u:
			avi.odml = ParseOdmlHeader(rp.reader, rp.Position);
			break;
		case 2019847785u:
		{
			uint streamId;
			AviStreamIndex aviStreamIndex = ParseOdmlIndex(rp.reader, rp.Position, out streamId);
			switch (streamId)
			{
			case 1650733104u:
			case 1667510320u:
				avi.videoIndex = aviStreamIndex;
				break;
			case 1651978544u:
				avi.audioIndex = aviStreamIndex;
				break;
			}
			break;
		}
		}
	}

	private void PrepareAudioStream()
	{
		long num = 0L;
		int num2 = 0;
		List<AviStreamIndex.Entry> entries = avi.audioIndex.entries;
		audioByteIndex = new long[entries.Count];
		for (int i = 0; i < entries.Count; i++)
		{
			AviStreamIndex.Entry entry = entries[i];
			audioByteIndex[i] = num;
			num += entry.chunkLength;
			if (entry.chunkLength > num2)
			{
				num2 = entry.chunkLength;
			}
		}
		rawAudioBuf = new byte[num2];
		base.audioStreamInfo.lengthBytes = num;
	}

	private void PrepareVideoStream()
	{
		long num = 0L;
		int num2 = 0;
		List<AviStreamIndex.Entry> entries = avi.videoIndex.entries;
		for (int i = 0; i < entries.Count; i++)
		{
			AviStreamIndex.Entry entry = entries[i];
			num += entry.chunkLength;
			if (entry.chunkLength > num2)
			{
				num2 = entry.chunkLength;
			}
		}
		rawVideoBuf = new byte[num2];
		base.videoStreamInfo.lengthBytes = num;
	}
}
