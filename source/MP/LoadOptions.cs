using System;

namespace MP;

[Serializable]
public class LoadOptions
{
	public bool _3DSound;

	public bool preloadAudio;

	public bool preloadVideo;

	public bool skipVideo;

	public bool skipAudio;

	public AudioStreamInfo audioStreamInfo;

	public VideoStreamInfo videoStreamInfo;

	public float connectTimeout = 10f;

	public Demux demuxOverride;

	public static LoadOptions Default => new LoadOptions();
}
