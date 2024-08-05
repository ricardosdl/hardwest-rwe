namespace MP.Decoder;

public class VideoDecoderMPNG : VideoDecoderUnity
{
	public const uint FOURCC_MPNG = 1196314701u;

	public VideoDecoderMPNG(VideoStreamInfo streamInfo = null)
		: base(streamInfo)
	{
	}
}
