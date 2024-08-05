namespace MP.Decoder;

public class VideoDecoderMJPEG : VideoDecoderUnity
{
	public const uint FOURCC_MJPG = 1196444237u;

	public const uint FOURCC_CJPG = 1196444227u;

	public const uint FOURCC_ffds = 1935959654u;

	public VideoDecoderMJPEG(VideoStreamInfo streamInfo = null)
		: base(streamInfo)
	{
	}
}
