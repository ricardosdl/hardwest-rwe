using System;
using System.IO;
using MP.Local;
using MP.Net;

namespace MP;

public abstract class Streamer : Demux
{
	public abstract bool IsConnected { get; }

	public static Streamer forUrl(string url)
	{
		if (url.StartsWith("http"))
		{
			return new HttpMjpegStreamer();
		}
		if (url.StartsWith("webcam://"))
		{
			return new LocalWebcamStreamer();
		}
		throw new MpException("Can't detect suitable Streamer for given url: " + url);
	}

	public abstract void Connect(string url, LoadOptions loadOptions = null);

	public override void Init(Stream stream, LoadOptions loadOptions = null)
	{
		throw new MpException("Streamer requires you to call Connect() instead of Init()");
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		throw new NotSupportedException("Can't read arbitrary frame from a stream");
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		throw new NotSupportedException("Can't read arbitrary audio from a stream");
	}
}
