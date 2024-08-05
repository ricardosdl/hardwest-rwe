namespace MP.AVI;

public class AVIFile
{
	public AVIMainHeader avih;

	public AVIStreamHeader strhVideo;

	public BitmapInfoHeader strfVideo;

	public AVIStreamHeader strhAudio;

	public WaveFormatEx strfAudio;

	public ODMLHeader odml;

	public AviStreamIndex videoIndex;

	public AviStreamIndex audioIndex;
}
