using System.IO;

namespace MP;

public abstract class Remux
{
	protected Stream dstStream;

	private VideoStreamInfo _videoStreamInfo;

	private AudioStreamInfo _audioStreamInfo;

	public VideoStreamInfo videoStreamInfo => _videoStreamInfo;

	public AudioStreamInfo audioStreamInfo => _audioStreamInfo;

	public virtual void Init(Stream dstStream, VideoStreamInfo videoStreamInfo, AudioStreamInfo audioStreamInfo)
	{
		this.dstStream = dstStream;
		_videoStreamInfo = videoStreamInfo;
		_audioStreamInfo = audioStreamInfo;
	}

	public abstract void Shutdown();

	public abstract void WriteNextVideoFrame(byte[] frameBytes, int size = -1);

	public abstract void WriteVideoFrame(int frameOffset, byte[] frameBytes, int size = -1);

	public abstract void WriteNextAudioSamples(byte[] sampleBytes, int size = -1);

	public abstract void WriteAudioSamples(int sampleOffset, byte[] sampleBytes, int size = -1);
}
