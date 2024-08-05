using System;
using MP;
using MP.Net;
using UnityEngine;

public class MovieStreamer : MoviePlayerBase
{
	public string sourceUrl;

	public LoadOptions loadOptions = LoadOptions.Default;

	public string status;

	public long bytesReceived;

	private int lastVideoFrame = -1;

	public bool IsConnected => movie != null && movie.demux != null && ((Streamer)movie.demux).IsConnected;

	public bool Load(string srcUrl)
	{
		return Load(srcUrl, null);
	}

	public bool Load(string srcUrl, LoadOptions loadOptions)
	{
		sourceUrl = srcUrl;
		if (loadOptions == null)
		{
			loadOptions = this.loadOptions;
		}
		else
		{
			this.loadOptions = loadOptions;
		}
		try
		{
			Load(new MovieSource
			{
				url = srcUrl
			}, loadOptions);
			return true;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}

	[ContextMenu("Reconnect")]
	public bool ReConnect()
	{
		bool result = true;
		if (!string.IsNullOrEmpty(sourceUrl))
		{
			result = Load(sourceUrl, loadOptions);
		}
		return result;
	}

	private void Start()
	{
		ReConnect();
	}

	private void OnGUI()
	{
		if (IsConnected && movie.demux.hasVideo && drawToScreen && framebuffer != null && ((Streamer)movie.demux).VideoPosition > 0)
		{
			DrawFramebufferToScreen();
		}
	}

	private void Update()
	{
		if (movie != null && movie.demux != null && movie.demux is HttpMjpegStreamer)
		{
			status = ((HttpMjpegStreamer)movie.demux).Status;
			bytesReceived = ((HttpMjpegStreamer)movie.demux).BytesReceived;
		}
		HandlePlayStop();
		if (play)
		{
			HandleFrameDecode();
		}
	}

	protected void HandleFrameDecode()
	{
		if (IsConnected && movie.demux.hasVideo && movie.videoDecoder != null && movie.videoDecoder.Position != lastVideoFrame)
		{
			if (movie.videoDecoder.Position >= 0)
			{
				movie.videoDecoder.DecodeNext();
				movie.demux.videoStreamInfo.width = framebuffer.width;
				movie.demux.videoStreamInfo.height = framebuffer.height;
			}
			lastVideoFrame = movie.videoDecoder.Position;
		}
	}
}
