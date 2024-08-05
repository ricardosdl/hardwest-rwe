using System;
using System.Diagnostics;
using UnityEngine;

namespace MP.Decoder;

public abstract class VideoDecoderUnity : VideoDecoder
{
	protected Texture2D framebuffer;

	protected VideoStreamInfo streamInfo;

	protected Demux demux;

	private float _lastFrameDecodeTime;

	private int _lastFrameSizeBytes;

	private float _totalDecodeTime;

	private long _totalSizeBytes;

	private Stopwatch watch;

	private int lastFbWidth = -1;

	private int lastFbHeight = -1;

	public override int Position
	{
		get
		{
			return demux.VideoPosition;
		}
		set
		{
			demux.VideoPosition = value;
		}
	}

	public override float lastFrameDecodeTime => _lastFrameDecodeTime;

	public override int lastFrameSizeBytes => _lastFrameSizeBytes;

	public override float totalDecodeTime => _totalDecodeTime;

	public override long totalSizeBytes => _totalSizeBytes;

	public VideoDecoderUnity(VideoStreamInfo streamInfo = null)
	{
		this.streamInfo = streamInfo;
	}

	public override void Init(out Texture2D framebuffer, Demux demux, LoadOptions loadOptions = null)
	{
		if (demux == null)
		{
			throw new ArgumentException("Missing Demux to get video frames from");
		}
		this.framebuffer = new Texture2D(4, 4, TextureFormat.RGB24, mipmap: false);
		framebuffer = this.framebuffer;
		this.demux = demux;
		_lastFrameDecodeTime = 0f;
		_totalDecodeTime = 0f;
		watch = new Stopwatch();
	}

	public override void Shutdown()
	{
		if (framebuffer != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(framebuffer);
			}
			else
			{
				UnityEngine.Object.Destroy(framebuffer);
			}
		}
	}

	public override void DecodeNext()
	{
		if (framebuffer == null)
		{
			return;
		}
		watch.Reset();
		watch.Start();
		byte[] targetBuf;
		int num = demux.ReadVideoFrame(out targetBuf);
		bool flag = framebuffer.LoadImage(targetBuf);
		if (flag && lastFbWidth > 0)
		{
			if (framebuffer.width != lastFbWidth || framebuffer.height != lastFbHeight)
			{
				flag = false;
			}
			lastFbWidth = framebuffer.width;
			lastFbHeight = framebuffer.height;
		}
		if (flag)
		{
			framebuffer.Apply(updateMipmaps: false);
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't decode frame " + (demux.VideoPosition - 1) + " from " + targetBuf.Length + " bytes");
		}
		watch.Stop();
		_lastFrameDecodeTime = (float)(0.0010000000474974513 * watch.Elapsed.TotalMilliseconds);
		_lastFrameSizeBytes = num;
		_totalDecodeTime += _lastFrameDecodeTime;
		_totalSizeBytes += _lastFrameSizeBytes;
	}
}
