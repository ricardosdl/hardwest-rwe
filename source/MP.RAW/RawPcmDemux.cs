using System;
using System.IO;

namespace MP.RAW;

public class RawPcmDemux : Demux
{
	private AtomicBinaryReader reader;

	private byte[] rawAudioBuf;

	private int nextAudioSample;

	public override int VideoPosition
	{
		get
		{
			throw new NotSupportedException("There's no hidden video in raw PCM audio");
		}
		set
		{
			throw new NotSupportedException("There's no hidden video in raw PCM audio");
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
		if (sourceStream == null || loadOptions == null || loadOptions.audioStreamInfo == null)
		{
			throw new ArgumentException("sourceStream and loadOptions.audioStreamInfo are required");
		}
		reader = new AtomicBinaryReader(sourceStream);
		base.audioStreamInfo = loadOptions.audioStreamInfo;
		base.audioStreamInfo.lengthBytes = reader.StreamLength;
		nextAudioSample = 0;
	}

	public override void Shutdown(bool force = false)
	{
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		throw new NotSupportedException("There's no hidden video in raw PCM audio");
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		if (nextAudioSample + sampleCount > base.audioStreamInfo.sampleCount)
		{
			sampleCount = base.audioStreamInfo.sampleCount - nextAudioSample;
		}
		int num = sampleCount * base.audioStreamInfo.sampleSize;
		if (rawAudioBuf == null || rawAudioBuf.Length < num)
		{
			rawAudioBuf = new byte[num];
		}
		targetBuf = rawAudioBuf;
		if (num <= 0)
		{
			return 0;
		}
		long offset = nextAudioSample * base.audioStreamInfo.sampleSize;
		nextAudioSample += sampleCount;
		return reader.Read(ref offset, rawAudioBuf, 0, num) / base.audioStreamInfo.sampleSize;
	}
}
