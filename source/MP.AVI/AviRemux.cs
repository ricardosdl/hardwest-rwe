using System;
using System.IO;
using UnityEngine;

namespace MP.AVI;

public class AviRemux : Remux
{
	private struct ByteOffsets
	{
		public long indexBase;

		public int avih;

		public int videoStrh;

		public int videoIndx;

		public int audioStrh;

		public int audioIndx;

		public int dmlh;
	}

	private int maxSuperindexEntries;

	private int maxRiffElementSize;

	private RiffWriter writer;

	private AviStreamIndex videoIndex;

	private int videoSuperIndexEntryCount;

	private AviStreamIndex audioIndex;

	private int audioSuperIndexEntryCount;

	private bool usingMultipleRiffs;

	private int totalFramesOld;

	private int totalFrames;

	private int totalSamples;

	private ByteOffsets offsets;

	private bool hasAudioStream => base.audioStreamInfo != null;

	public AviRemux(int maxSuperindexEntries = 32, int maxRiffElementSize = 2000000000)
	{
		this.maxSuperindexEntries = maxSuperindexEntries;
		this.maxRiffElementSize = maxRiffElementSize;
	}

	public override void Init(Stream dstStream, VideoStreamInfo videoStreamInfo, AudioStreamInfo audioStreamInfo)
	{
		if (dstStream == null || videoStreamInfo == null)
		{
			throw new ArgumentException("At least destination stream and video stream info is needed");
		}
		base.Init(dstStream, videoStreamInfo, audioStreamInfo);
		usingMultipleRiffs = false;
		totalFramesOld = 0;
		totalFrames = 0;
		totalSamples = 0;
		writer = new RiffWriter(dstStream);
		writer.BeginRiff(541677121u);
		writer.BeginList(1819436136u);
		offsets.avih = WriteMainHeader(writer, videoStreamInfo, hasAudioStream);
		writer.BeginList(1819440243u);
		offsets.videoStrh = WriteVideoStreamHeader(writer, videoStreamInfo);
		WriteVideoFormatHeader(writer, videoStreamInfo);
		offsets.videoIndx = WriteDummySuperIndex(writer, 1667510320u, maxSuperindexEntries);
		videoSuperIndexEntryCount = 0;
		writer.EndList();
		videoIndex = new AviStreamIndex();
		videoIndex.streamId = 1667510320u;
		if (hasAudioStream)
		{
			writer.BeginList(1819440243u);
			offsets.audioStrh = WriteAudioStreamHeader(writer, audioStreamInfo);
			WriteAudioFormatHeader(writer, audioStreamInfo);
			offsets.audioIndx = WriteDummySuperIndex(writer, 1651978544u, maxSuperindexEntries);
			audioSuperIndexEntryCount = 0;
			writer.EndList();
			audioIndex = new AviStreamIndex();
			audioIndex.streamId = 1651978544u;
		}
		writer.BeginList(1819108463u);
		offsets.dmlh = WriteDmlhHeader(writer, videoStreamInfo.frameCount);
		writer.EndList();
		writer.EndList();
		writer.BeginList(1769369453u);
		offsets.indexBase = writer.binaryWriter.Seek(0, SeekOrigin.Current);
	}

	public override void WriteNextVideoFrame(byte[] frameBytes, int size = -1)
	{
		if (writer.currentElementSize > maxRiffElementSize)
		{
			StartNewRiff();
		}
		if (size < 0)
		{
			size = frameBytes.Length;
		}
		AviStreamIndex.Entry entry = new AviStreamIndex.Entry();
		entry.chunkOffset = writer.binaryWriter.Seek(0, SeekOrigin.Current) + 8;
		entry.chunkLength = size;
		videoIndex.entries.Add(entry);
		writer.WriteChunk(1667510320u, frameBytes, size);
		totalFrames++;
		if (!usingMultipleRiffs)
		{
			totalFramesOld++;
		}
	}

	public override void WriteVideoFrame(int frameOffset, byte[] frameBytes, int size = -1)
	{
		throw new NotSupportedException("Only adding frames at the end is supported");
	}

	public bool WriteLookbackVideoFrame(int frame)
	{
		if (frame < 0)
		{
			frame = totalFrames - frame;
		}
		int num = frame - videoIndex.globalOffset;
		if (num < 0 || num >= videoIndex.entries.Count)
		{
			return false;
		}
		AviStreamIndex.Entry entry = videoIndex.entries[num];
		AviStreamIndex.Entry entry2 = new AviStreamIndex.Entry();
		entry2.chunkOffset = entry.chunkOffset;
		entry2.chunkLength = entry.chunkLength;
		videoIndex.entries.Add(entry2);
		totalFrames++;
		if (!usingMultipleRiffs)
		{
			totalFramesOld++;
		}
		return true;
	}

	public override void WriteNextAudioSamples(byte[] sampleBytes, int size = -1)
	{
		if (writer.currentElementSize > maxRiffElementSize)
		{
			StartNewRiff();
		}
		if (size < 0)
		{
			size = sampleBytes.Length;
		}
		AviStreamIndex.Entry entry = new AviStreamIndex.Entry();
		entry.chunkOffset = writer.binaryWriter.Seek(0, SeekOrigin.Current) + 8;
		entry.chunkLength = size;
		audioIndex.entries.Add(entry);
		writer.WriteChunk(1651978544u, sampleBytes);
		totalSamples += size / base.audioStreamInfo.sampleSize;
	}

	public override void WriteAudioSamples(int sampleOffset, byte[] frameBytes, int size = -1)
	{
		throw new NotSupportedException("Only adding samples at the end is supported");
	}

	public override void Shutdown()
	{
		BinaryWriter binaryWriter = writer.binaryWriter;
		long offset = binaryWriter.Seek(0, SeekOrigin.Current);
		binaryWriter.Seek(offsets.avih + 16, SeekOrigin.Begin);
		binaryWriter.Write(totalFramesOld);
		binaryWriter.Seek(offsets.videoStrh + 32, SeekOrigin.Begin);
		binaryWriter.Write(totalFrames);
		binaryWriter.Seek(offsets.dmlh, SeekOrigin.Begin);
		binaryWriter.Write(totalFrames);
		if (hasAudioStream)
		{
			binaryWriter.Seek(offsets.audioStrh + 32, SeekOrigin.Begin);
			binaryWriter.Write(totalSamples);
		}
		binaryWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
		if (videoIndex.entries.Count > 0)
		{
			WriteChunkIndex(writer, videoIndex, offsets.videoIndx, ref videoSuperIndexEntryCount, offsets.indexBase, maxSuperindexEntries);
		}
		if (hasAudioStream && audioIndex.entries.Count > 0)
		{
			WriteChunkIndex(writer, audioIndex, offsets.audioIndx, ref audioSuperIndexEntryCount, offsets.indexBase, maxSuperindexEntries);
		}
		writer.EndList();
		writer.EndRiff();
		writer.Close();
	}

	private void StartNewRiff()
	{
		if (videoIndex.entries.Count > 0)
		{
			WriteChunkIndex(writer, videoIndex, offsets.videoIndx, ref videoSuperIndexEntryCount, offsets.indexBase, maxSuperindexEntries);
		}
		if (hasAudioStream && audioIndex.entries.Count > 0)
		{
			WriteChunkIndex(writer, audioIndex, offsets.audioIndx, ref audioSuperIndexEntryCount, offsets.indexBase, maxSuperindexEntries);
		}
		writer.EndList();
		writer.EndRiff();
		writer.BeginRiff(1481201217u);
		writer.BeginList(1769369453u);
		offsets.indexBase = writer.binaryWriter.Seek(0, SeekOrigin.Current);
		usingMultipleRiffs = true;
	}

	private static int WriteMainHeader(RiffWriter rw, VideoStreamInfo vsi, bool hasAudioStream)
	{
		rw.BeginChunk(1751742049u);
		int result = (int)rw.binaryWriter.Seek(0, SeekOrigin.Current);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write(Mathf.RoundToInt(1000000f / vsi.framerate));
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		binaryWriter.Write(48);
		binaryWriter.Write(vsi.frameCount);
		binaryWriter.Write(0);
		binaryWriter.Write((!hasAudioStream) ? 1 : 2);
		binaryWriter.Write(0);
		binaryWriter.Write(vsi.width);
		binaryWriter.Write(vsi.height);
		binaryWriter.Write(0L);
		binaryWriter.Write(0L);
		rw.EndChunk();
		return result;
	}

	private static int WriteVideoStreamHeader(RiffWriter rw, VideoStreamInfo vsi)
	{
		rw.BeginChunk(1752331379u);
		int result = (int)rw.binaryWriter.Seek(0, SeekOrigin.Current);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write(1935960438u);
		binaryWriter.Write(vsi.codecFourCC);
		binaryWriter.Write(0);
		binaryWriter.Write((short)0);
		binaryWriter.Write((short)0);
		binaryWriter.Write(0);
		FindScaleAndRate(out var scale, out var rate, vsi.framerate);
		binaryWriter.Write(scale);
		binaryWriter.Write(rate);
		binaryWriter.Write(0);
		binaryWriter.Write(vsi.frameCount);
		binaryWriter.Write(0);
		binaryWriter.Write(-1);
		binaryWriter.Write(0);
		binaryWriter.Write((short)0);
		binaryWriter.Write((short)0);
		binaryWriter.Write((short)vsi.width);
		binaryWriter.Write((short)vsi.height);
		rw.EndChunk();
		return result;
	}

	private static int WriteAudioStreamHeader(RiffWriter rw, AudioStreamInfo asi)
	{
		rw.BeginChunk(1752331379u);
		int result = (int)rw.binaryWriter.Seek(0, SeekOrigin.Current);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write(1935963489u);
		binaryWriter.Write(asi.codecFourCC);
		binaryWriter.Write(0);
		binaryWriter.Write((short)0);
		binaryWriter.Write((short)0);
		binaryWriter.Write(0);
		binaryWriter.Write(1);
		binaryWriter.Write(asi.sampleRate);
		binaryWriter.Write(0);
		binaryWriter.Write(asi.sampleCount);
		binaryWriter.Write(0);
		binaryWriter.Write(-1);
		binaryWriter.Write(asi.sampleSize);
		binaryWriter.Write(0L);
		rw.EndChunk();
		return result;
	}

	private static void FindScaleAndRate(out int scale, out int rate, float framerate)
	{
		rate = Mathf.FloorToInt(framerate);
		scale = 1;
		while ((double)rate < 100000.0)
		{
			float num = (float)rate / (float)scale - framerate;
			if ((double)Mathf.Abs(num) < 1E-05)
			{
				break;
			}
			if (num > 0f)
			{
				scale++;
			}
			else
			{
				rate++;
			}
		}
	}

	private static void WriteVideoFormatHeader(RiffWriter rw, VideoStreamInfo vsi)
	{
		rw.BeginChunk(1718776947u);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write(40);
		binaryWriter.Write(vsi.width);
		binaryWriter.Write(vsi.height);
		binaryWriter.Write((short)1);
		binaryWriter.Write((short)vsi.bitsPerPixel);
		binaryWriter.Write(vsi.codecFourCC);
		binaryWriter.Write(vsi.width * vsi.height * vsi.bitsPerPixel / 8);
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		binaryWriter.Write(0);
		rw.EndChunk();
	}

	private static void WriteAudioFormatHeader(RiffWriter rw, AudioStreamInfo asi)
	{
		rw.BeginChunk(1718776947u);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write((ushort)asi.audioFormat);
		binaryWriter.Write((short)asi.channels);
		binaryWriter.Write(asi.sampleRate);
		binaryWriter.Write(asi.sampleRate * asi.sampleSize * asi.channels);
		binaryWriter.Write((short)asi.sampleSize);
		binaryWriter.Write((short)(8 * asi.sampleSize / asi.channels));
		binaryWriter.Write((short)0);
		rw.EndChunk();
	}

	private static int WriteDmlhHeader(RiffWriter rw, int totalFrames)
	{
		rw.BeginChunk(1751936356u);
		int result = (int)rw.binaryWriter.Seek(0, SeekOrigin.Current);
		rw.binaryWriter.Write(totalFrames);
		rw.EndChunk();
		return result;
	}

	private static int WriteDummySuperIndex(RiffWriter rw, uint streamId, int entriesToReserve)
	{
		rw.BeginChunk(2019847785u);
		int result = (int)rw.binaryWriter.Seek(0, SeekOrigin.Current);
		BinaryWriter binaryWriter = rw.binaryWriter;
		binaryWriter.Write((short)4);
		binaryWriter.Write((byte)0);
		binaryWriter.Write((byte)0);
		binaryWriter.Write(0);
		binaryWriter.Write(streamId);
		binaryWriter.Write(new byte[12 + entriesToReserve * 16]);
		rw.EndChunk();
		return result;
	}

	private static void WriteChunkIndex(RiffWriter rw, AviStreamIndex index, int superIndexChunkOffset, ref int superIndexEntryCount, long indexBaseOffset, int maxSuperindexEntries)
	{
		BinaryWriter binaryWriter = rw.binaryWriter;
		long num = binaryWriter.Seek(0, SeekOrigin.Current);
		superIndexEntryCount++;
		if (superIndexEntryCount > maxSuperindexEntries)
		{
			throw new MpException("Not enough space was reserved for superindex. Please increase maxSuperindexEntries");
		}
		binaryWriter.Seek(superIndexChunkOffset + 4, SeekOrigin.Begin);
		binaryWriter.Write(superIndexEntryCount);
		binaryWriter.Seek(superIndexChunkOffset + 24 + (superIndexEntryCount - 1) * 16, SeekOrigin.Begin);
		binaryWriter.Write(num);
		binaryWriter.Write(32 + 8 * index.entries.Count);
		binaryWriter.Write(index.entries.Count);
		binaryWriter.BaseStream.Seek(num, SeekOrigin.Begin);
		rw.BeginChunk((RiffParser.ToFourCC("ix__") & 0xFFFFu) | ((index.streamId << 16) & 0xFFFF0000u));
		binaryWriter.Write((short)2);
		binaryWriter.Write((byte)0);
		binaryWriter.Write((byte)1);
		binaryWriter.Write(index.entries.Count);
		binaryWriter.Write(index.streamId);
		binaryWriter.Write(indexBaseOffset);
		binaryWriter.Write(0);
		foreach (AviStreamIndex.Entry entry in index.entries)
		{
			long num2 = entry.chunkOffset - indexBaseOffset;
			if (num2 > int.MaxValue)
			{
				throw new MpException("Internal error. Can't write index, because chunk offset won't fit into 31 bits: " + num2);
			}
			binaryWriter.Write((uint)num2);
			binaryWriter.Write(entry.chunkLength);
		}
		rw.EndChunk();
		index.globalOffset += index.entries.Count;
		index.entries.Clear();
	}
}
