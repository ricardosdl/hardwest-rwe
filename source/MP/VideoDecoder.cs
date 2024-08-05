using System;
using MP.Decoder;
using UnityEngine;

namespace MP;

public abstract class VideoDecoder
{
	public abstract int Position { get; set; }

	public abstract float lastFrameDecodeTime { get; }

	public abstract int lastFrameSizeBytes { get; }

	public abstract float totalDecodeTime { get; }

	public abstract long totalSizeBytes { get; }

	public static VideoDecoder CreateFor(VideoStreamInfo streamInfo)
	{
		if (streamInfo == null)
		{
			throw new ArgumentException("Can't choose VideoDecoder without streamInfo (with at least codecFourCC)");
		}
		switch (streamInfo.codecFourCC)
		{
		case 1196444227u:
		case 1196444237u:
		case 1935959654u:
			return new VideoDecoderMJPEG(streamInfo);
		case 1196314701u:
			return new VideoDecoderMPNG(streamInfo);
		case 0u:
		case 541215044u:
			return new VideoDecoderRGB(streamInfo);
		default:
			throw new MpException("No decoder for video fourCC 0x" + streamInfo.codecFourCC.ToString("X") + " (" + RiffParser.FromFourCC(streamInfo.codecFourCC) + ")");
		}
	}

	public abstract void Init(out Texture2D framebuffer, Demux demux, LoadOptions loadOptions = null);

	public abstract void Shutdown();

	public abstract void DecodeNext();

	public void Decode(int frame)
	{
		Position = frame;
		DecodeNext();
	}
}
