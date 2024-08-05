namespace MP;

public class AudioStreamInfo
{
	public uint codecFourCC;

	public uint audioFormat;

	public int sampleCount;

	public int sampleSize;

	public int channels;

	public int sampleRate;

	public long lengthBytes;

	public float lengthSeconds => (float)sampleCount / (float)sampleRate;

	public AudioStreamInfo()
	{
	}

	public AudioStreamInfo(AudioStreamInfo ai)
	{
		codecFourCC = ai.codecFourCC;
		audioFormat = ai.audioFormat;
		sampleCount = ai.sampleCount;
		sampleSize = ai.sampleSize;
		channels = ai.channels;
		sampleRate = ai.sampleRate;
		lengthBytes = ai.lengthBytes;
	}
}
