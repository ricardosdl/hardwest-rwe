using System;
using UnityEngine;

namespace MP.Local;

public class LocalWebcamStreamer : Streamer
{
	public const string URL_PREFIX = "webcam://";

	private int receivedFrameCount;

	private WebCamTexture webcam;

	private Color32[] colorBuffer;

	private byte[] rawBuffer;

	public override bool IsConnected => webcam != null && webcam.isPlaying;

	public override int VideoPosition
	{
		get
		{
			return receivedFrameCount;
		}
		set
		{
			throw new NotSupportedException("Can't seek a live stream");
		}
	}

	public override int AudioPosition
	{
		get
		{
			throw new NotSupportedException("There's no webcam audio support for now");
		}
		set
		{
			throw new NotSupportedException("Can't seek a live stream");
		}
	}

	public override void Connect(string url, LoadOptions loadOptions = null)
	{
		if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
		{
			throw new MpException("Not authorized to use webcam. Use Application.RequestUserAuthorization before calling this");
		}
		if (loadOptions != null && loadOptions.videoStreamInfo != null)
		{
			base.videoStreamInfo = loadOptions.videoStreamInfo;
			webcam = new WebCamTexture(url.Substring(9), loadOptions.videoStreamInfo.width, loadOptions.videoStreamInfo.height, (int)loadOptions.videoStreamInfo.framerate);
		}
		else
		{
			webcam = new WebCamTexture(url.Substring(9));
		}
		webcam.Play();
		base.videoStreamInfo = new VideoStreamInfo();
		base.videoStreamInfo.codecFourCC = 0u;
		base.videoStreamInfo.width = webcam.width;
		base.videoStreamInfo.height = webcam.height;
		base.videoStreamInfo.bitsPerPixel = 24;
		base.videoStreamInfo.framerate = webcam.requestedFPS;
		colorBuffer = new Color32[webcam.width * webcam.height];
		rawBuffer = new byte[colorBuffer.Length * 3];
	}

	public override void Shutdown(bool force = false)
	{
		webcam.Stop();
		webcam = null;
		colorBuffer = null;
		rawBuffer = null;
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		if (webcam.didUpdateThisFrame)
		{
			webcam.GetPixels32(colorBuffer);
			int num = colorBuffer.Length;
			for (int i = 0; i < num; i++)
			{
				Color32 color = colorBuffer[i];
				rawBuffer[i * 3] = color.b;
				rawBuffer[i * 3 + 1] = color.g;
				rawBuffer[i * 3 + 2] = color.r;
			}
			receivedFrameCount++;
		}
		targetBuf = rawBuffer;
		return rawBuffer.Length;
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		throw new NotSupportedException("There's no webcam audio support for now");
	}
}
