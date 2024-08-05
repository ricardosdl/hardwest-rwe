namespace MP.AVI;

public class AVIStreamHeader
{
	public const uint AVISF_DISABLED = 1u;

	public const uint AVISF_VIDEO_PALCHANGES = 65536u;

	public uint fccType;

	public uint fccHandler;

	public uint dwFlags;

	public ushort wPriority;

	public ushort wLanguage;

	public uint dwInitialFrames;

	public uint dwScale;

	public uint dwRate;

	public uint dwStart;

	public uint dwLength;

	public uint dwSuggestedBufferSize;

	public uint dwQuality;

	public uint dwSampleSize;

	public short rcFrameLeft;

	public short rcFrameTop;

	public short rcFrameRight;

	public short rcFrameBottom;
}
