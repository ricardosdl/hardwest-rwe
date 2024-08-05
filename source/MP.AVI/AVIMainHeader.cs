namespace MP.AVI;

public class AVIMainHeader
{
	public const uint AVIF_COPYRIGHTED = 131072u;

	public const uint AVIF_HASINDEX = 16u;

	public const uint AVIF_ISINTERLEAVED = 256u;

	public const uint AVIF_MUSTUSEINDEX = 32u;

	public const uint AVIF_TRUSTCKTYPE = 2048u;

	public const uint AVIF_WASCAPTUREFILE = 65536u;

	public uint dwMicroSecPerFrame;

	public uint dwMaxBytesPerSec;

	public uint dwPaddingGranularity;

	public uint dwFlags;

	public uint dwTotalFrames;

	public uint dwInitialFrames;

	public uint dwStreams;

	public uint dwSuggestedBufferSize;

	public uint dwWidth;

	public uint dwHeight;

	public uint dwReserved0;

	public uint dwReserved1;

	public uint dwReserved2;

	public uint dwReserved3;
}
