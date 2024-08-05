namespace MP;

public class VideoStreamInfo
{
	public uint codecFourCC;

	public int bitsPerPixel;

	public int frameCount = 1;

	public int width = 1;

	public int height = 1;

	public float framerate = 30f;

	public long lengthBytes;

	public float lengthSeconds => (float)frameCount / framerate;

	public VideoStreamInfo()
	{
	}

	public VideoStreamInfo(VideoStreamInfo vi)
	{
		codecFourCC = vi.codecFourCC;
		bitsPerPixel = vi.bitsPerPixel;
		frameCount = vi.frameCount;
		width = vi.width;
		height = vi.height;
		framerate = vi.framerate;
		lengthBytes = vi.lengthBytes;
	}
}
