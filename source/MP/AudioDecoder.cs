using System;
using MP.Decoder;
using UnityEngine;

namespace MP;

public abstract class AudioDecoder
{
	public abstract int Position { get; set; }

	public abstract float totalDecodeTime { get; }

	public static AudioDecoder CreateFor(AudioStreamInfo streamInfo)
	{
		if (streamInfo == null)
		{
			throw new ArgumentException("Can't choose AudioDecoder without streamInfo (with at least codecFourCC)");
		}
		uint codecFourCC = streamInfo.codecFourCC;
		if (codecFourCC == 1)
		{
			return new AudioDecoderPCM(streamInfo);
		}
		throw new MpException("No decoder for audio fourCC 0x" + streamInfo.codecFourCC.ToString("X") + " (" + RiffParser.FromFourCC(streamInfo.codecFourCC) + ")");
	}

	public abstract void Init(out AudioClip audioClip, Demux demux, LoadOptions loadOptions = null);

	public abstract void Shutdown();

	public abstract void DecodeNext(float[] data, int sampleCount);
}
