using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MP.RAW;

public class RawMjpegDemux : Demux
{
	private const int FILE_READ_BUFFER_SIZE = 8096;

	private AtomicBinaryReader reader;

	private List<long> frameStartIndex = new List<long>();

	private List<int> frameSize = new List<int>();

	private byte[] rawJpgBuffer;

	private int nextVideoFrame;

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
			throw new NotSupportedException("There's no hidden audio in raw MJPEG stream");
		}
		set
		{
			throw new NotSupportedException("There's no hidden audio in raw MJPEG stream");
		}
	}

	public override void Init(Stream sourceStream, LoadOptions loadOptions = null)
	{
		if (loadOptions != null && loadOptions.skipVideo)
		{
			return;
		}
		if (sourceStream == null)
		{
			throw new ArgumentException("sourceStream is required");
		}
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		reader = new AtomicBinaryReader(sourceStream);
		int num = 0;
		frameStartIndex.Clear();
		frameSize.Clear();
		long num2 = 0L;
		long num3 = -1L;
		bool flag = false;
		int num4 = -1;
		long num5 = 0L;
		byte[] array = new byte[8096];
		long offset = 0L;
		do
		{
			num4 = reader.Read(ref offset, array, 0, 8096);
			for (int i = 0; i < num4; i++)
			{
				byte b = array[i];
				if (b == byte.MaxValue)
				{
					flag = true;
				}
				else
				{
					if (!flag)
					{
						continue;
					}
					switch (b)
					{
					case 216:
						num3 = num5 + i - 1;
						break;
					case 217:
					{
						frameStartIndex.Add(num3);
						int num6 = (int)(num5 + i - num3 + 1);
						if (num6 > num)
						{
							num = num6;
						}
						frameSize.Add(num6);
						break;
					}
					}
					flag = false;
					num2++;
				}
			}
			num5 += num4;
		}
		while (num4 >= 8096);
		rawJpgBuffer = new byte[num];
		stopwatch.Stop();
		if (loadOptions != null && loadOptions.videoStreamInfo != null)
		{
			base.videoStreamInfo = loadOptions.videoStreamInfo;
		}
		else
		{
			base.videoStreamInfo = new VideoStreamInfo();
			base.videoStreamInfo.codecFourCC = 1196444237u;
		}
		base.videoStreamInfo.frameCount = frameSize.Count;
		base.videoStreamInfo.lengthBytes = reader.StreamLength;
	}

	public override void Shutdown(bool force = false)
	{
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		targetBuf = rawJpgBuffer;
		if (nextVideoFrame < 0 || nextVideoFrame >= base.videoStreamInfo.frameCount)
		{
			return 0;
		}
		nextVideoFrame++;
		long offset = frameStartIndex[nextVideoFrame - 1];
		return reader.Read(ref offset, rawJpgBuffer, 0, frameSize[nextVideoFrame - 1]);
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		throw new NotSupportedException("There's no hidden audio in raw MJPEG stream");
	}
}
