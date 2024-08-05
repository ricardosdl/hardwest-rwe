using System.IO;
using MP.AVI;
using MP.RAW;

namespace MP;

public abstract class Demux
{
	public VideoStreamInfo videoStreamInfo { get; protected set; }

	public AudioStreamInfo audioStreamInfo { get; protected set; }

	public bool hasVideo => videoStreamInfo != null;

	public bool hasAudio => audioStreamInfo != null;

	public abstract int VideoPosition { get; set; }

	public abstract int AudioPosition { get; set; }

	public static Demux forSource(Stream sourceStream)
	{
		byte[] array = new byte[4];
		sourceStream.Seek(0L, SeekOrigin.Begin);
		if (sourceStream.Read(array, 0, 4) < 4)
		{
			throw new MpException("Stream too small");
		}
		if (array[0] == 82 && array[1] == 73 && array[2] == 70 && (array[3] == 70 || array[3] == 88))
		{
			return new AviDemux();
		}
		if (array[0] == byte.MaxValue && array[1] == 216)
		{
			sourceStream.Seek(-2L, SeekOrigin.End);
			sourceStream.Read(array, 0, 2);
			if (array[0] == byte.MaxValue && array[1] == 217)
			{
				return new RawMjpegDemux();
			}
		}
		throw new MpException("Can't detect suitable DEMUX for given stream");
	}

	public abstract void Init(Stream sourceStream, LoadOptions loadOptions = null);

	public abstract void Shutdown(bool force = false);

	public abstract int ReadVideoFrame(out byte[] targetBuf);

	public abstract int ReadAudioSamples(out byte[] targetBuf, int sampleCount);
}
