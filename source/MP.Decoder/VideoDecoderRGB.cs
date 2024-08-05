using System;
using System.Diagnostics;
using UnityEngine;

namespace MP.Decoder;

public class VideoDecoderRGB : VideoDecoder
{
	public const uint FOURCC_NULL = 0u;

	public const uint FOURCC_DIB_ = 541215044u;

	private Texture2D framebuffer;

	private Color32[] rgbBuffer;

	private Demux demux;

	private VideoStreamInfo info;

	private float _lastFrameDecodeTime;

	private int _lastFrameSizeBytes;

	private float _totalDecodeTime;

	private long _totalSizeBytes;

	private Stopwatch watch;

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

	public VideoDecoderRGB(VideoStreamInfo info = null)
	{
		this.info = info;
	}

	public override void Init(out Texture2D framebuffer, Demux demux, LoadOptions loadOptions = null)
	{
		if (demux == null)
		{
			throw new ArgumentException("Missing Demux to get video frames from");
		}
		if (info == null || info.width <= 0 || info.height <= 0 || info.bitsPerPixel <= 0)
		{
			throw new ArgumentException("Can't initialize stream decoder without proper VideoStreamInfo");
		}
		if (info.bitsPerPixel != 16 && info.bitsPerPixel != 24 && info.bitsPerPixel != 32)
		{
			throw new ArgumentException("Only RGB555, RGB24 and ARGB32 pixel formats are supported");
		}
		this.framebuffer = new Texture2D(info.width, info.height, (info.bitsPerPixel != 32) ? TextureFormat.RGB24 : TextureFormat.ARGB32, mipmap: false);
		framebuffer = this.framebuffer;
		rgbBuffer = new Color32[info.width * info.height];
		this.demux = demux;
		_lastFrameDecodeTime = 0f;
		_lastFrameSizeBytes = 0;
		_totalDecodeTime = 0f;
		_totalSizeBytes = 0L;
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
		int num2 = num / (info.bitsPerPixel / 8);
		if (num2 > rgbBuffer.Length)
		{
			throw new MpException("Too much data in frame " + (demux.VideoPosition - 1) + " to decode. Broken AVI?");
		}
		if (info.bitsPerPixel == 32)
		{
			for (int i = 0; i < num2; i++)
			{
				int num3 = i * 4;
				rgbBuffer[i].b = targetBuf[num3];
				rgbBuffer[i].g = targetBuf[num3 + 1];
				rgbBuffer[i].r = targetBuf[num3 + 2];
				rgbBuffer[i].a = targetBuf[num3 + 3];
			}
		}
		else if (info.bitsPerPixel == 24)
		{
			for (int j = 0; j < num2; j++)
			{
				int num4 = j * 3;
				rgbBuffer[j].b = targetBuf[num4];
				rgbBuffer[j].g = targetBuf[num4 + 1];
				rgbBuffer[j].r = targetBuf[num4 + 2];
			}
		}
		else if (info.bitsPerPixel == 16)
		{
			for (int k = 0; k < num2; k++)
			{
				int num5 = k * 2;
				int num6 = (targetBuf[num5 + 1] << 8) | targetBuf[num5];
				rgbBuffer[k].b = (byte)(num6 << 3);
				rgbBuffer[k].g = (byte)((uint)(num6 >> 2) & 0xF8u);
				rgbBuffer[k].r = (byte)((uint)(num6 >> 7) & 0xF8u);
			}
		}
		framebuffer.SetPixels32(rgbBuffer);
		framebuffer.Apply(updateMipmaps: false);
		watch.Stop();
		_lastFrameDecodeTime = (float)(0.0010000000474974513 * watch.Elapsed.TotalMilliseconds);
		_lastFrameSizeBytes = rgbBuffer.Length;
		_totalDecodeTime += _lastFrameDecodeTime;
		_totalSizeBytes += _lastFrameSizeBytes;
	}
}
