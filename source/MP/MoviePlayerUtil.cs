using System;
using System.IO;
using UnityEngine;

namespace MP;

public class MoviePlayerUtil
{
	private const int MAX_DESKTOP_ATLAS_WH = 8192;

	private const int MAX_MOBILE_ATLAS_WH = 2048;

	public static Movie Load(Stream srcStream, out Texture2D targetFramebuffer, LoadOptions loadOptions = null)
	{
		MovieSource movieSource = new MovieSource();
		movieSource.stream = srcStream;
		AudioClip targetAudioBuffer;
		return Load(movieSource, out targetFramebuffer, out targetAudioBuffer, loadOptions);
	}

	public static Movie Load(Stream srcStream, out Texture2D targetFramebuffer, out AudioClip targetAudioBuffer, LoadOptions loadOptions = null)
	{
		MovieSource movieSource = new MovieSource();
		movieSource.stream = srcStream;
		return Load(movieSource, out targetFramebuffer, out targetAudioBuffer, loadOptions);
	}

	public static Movie Load(string srcUrl, out Texture2D targetFramebuffer, LoadOptions loadOptions = null)
	{
		MovieSource movieSource = new MovieSource();
		movieSource.url = srcUrl;
		AudioClip targetAudioBuffer;
		return Load(movieSource, out targetFramebuffer, out targetAudioBuffer, loadOptions);
	}

	public static Movie Load(string srcUrl, out Texture2D targetFramebuffer, out AudioClip targetAudioBuffer, LoadOptions loadOptions = null)
	{
		MovieSource movieSource = new MovieSource();
		movieSource.url = srcUrl;
		return Load(movieSource, out targetFramebuffer, out targetAudioBuffer, loadOptions);
	}

	public static Movie Load(MovieSource source, out Texture2D targetFramebuffer, out AudioClip targetAudioBuffer, LoadOptions loadOptions = null)
	{
		if (loadOptions == null)
		{
			loadOptions = LoadOptions.Default;
		}
		if (source.stream == null && source.url == null)
		{
			throw new MpException("Either source.stream or source.url must be provided");
		}
		targetFramebuffer = null;
		targetAudioBuffer = null;
		Movie movie = new Movie();
		movie.sourceStream = source.stream;
		if (source.url != null)
		{
			movie.demux = ((loadOptions.demuxOverride == null) ? Streamer.forUrl(source.url) : loadOptions.demuxOverride);
			((Streamer)movie.demux).Connect(source.url, loadOptions);
		}
		else
		{
			movie.demux = ((loadOptions.demuxOverride == null) ? Demux.forSource(source.stream) : loadOptions.demuxOverride);
			movie.demux.Init(source.stream, loadOptions);
		}
		if (movie.demux.hasVideo && !loadOptions.skipVideo)
		{
			VideoStreamInfo videoStreamInfo = movie.demux.videoStreamInfo;
			movie.videoDecoder = VideoDecoder.CreateFor(videoStreamInfo);
			movie.videoDecoder.Init(out targetFramebuffer, movie.demux, loadOptions);
			if (loadOptions.preloadVideo)
			{
				movie.frameUV = UnpackFramesToAtlas(movie.videoDecoder, ref targetFramebuffer, videoStreamInfo.frameCount);
			}
			else
			{
				movie.frameUV = new Rect[1]
				{
					new Rect(0f, 0f, 1f, 1f)
				};
			}
		}
		if (movie.demux.hasAudio && !loadOptions.skipAudio)
		{
			movie.audioDecoder = AudioDecoder.CreateFor(movie.demux.audioStreamInfo);
			movie.audioDecoder.Init(out targetAudioBuffer, movie.demux, loadOptions);
		}
		return movie;
	}

	public static void Unload(Movie movie)
	{
		if (movie != null)
		{
			if (movie.sourceStream != null)
			{
				movie.sourceStream.Dispose();
				movie.sourceStream = null;
			}
			if (movie.videoDecoder != null)
			{
				movie.videoDecoder.Shutdown();
				movie.videoDecoder = null;
			}
			if (movie.audioDecoder != null)
			{
				movie.audioDecoder.Shutdown();
				movie.audioDecoder = null;
			}
			if (movie.demux != null)
			{
				movie.demux.Shutdown();
				movie.demux = null;
			}
		}
	}

	private static Rect[] UnpackFramesToAtlas(VideoDecoder videoDecoder, ref Texture2D framebuffer, int frameCount)
	{
		if (frameCount < 1)
		{
			throw new MpException("Expecting at least 1 video frame");
		}
		int num = 8192;
		videoDecoder.Position = 0;
		videoDecoder.DecodeNext();
		int width = framebuffer.width;
		int height = framebuffer.height;
		int num2 = num / width;
		int num3 = num / height;
		if (frameCount > num2 * num3)
		{
			throw new MpException(frameCount + " " + width + "x" + height + " video frames can't fit into " + num + "x" + num + " atlas texture. Consider lowering frame count or resolution, or disable video preloading");
		}
		Texture2D[] array = new Texture2D[frameCount];
		array[0] = CloneTexture(framebuffer);
		for (int i = 1; i < frameCount; i++)
		{
			videoDecoder.DecodeNext();
			array[i] = CloneTexture(framebuffer);
		}
		Rect[] result = framebuffer.PackTextures(array, 0, num);
		for (int j = 0; j < frameCount; j++)
		{
			UnityEngine.Object.Destroy(array[j]);
		}
		return result;
	}

	private static Texture2D CloneTexture(Texture2D srcTex)
	{
		Texture2D texture2D = new Texture2D(srcTex.width, srcTex.height, srcTex.format, srcTex.mipmapCount > 1);
		texture2D.SetPixels32(srcTex.GetPixels32());
		return texture2D;
	}

	public static byte[] ExtractRawAudio(Stream sourceStream)
	{
		Demux demux;
		return ExtractRawAudio(sourceStream, out demux);
	}

	public static byte[] ExtractRawAudio(Stream sourceStream, out Demux demux)
	{
		demux = Demux.forSource(sourceStream);
		demux.Init(sourceStream);
		if (!demux.hasAudio)
		{
			return null;
		}
		byte[] targetBuf = new byte[demux.audioStreamInfo.lengthBytes];
		demux.ReadAudioSamples(out targetBuf, demux.audioStreamInfo.sampleCount);
		return targetBuf;
	}

	public static byte[] ExtractRawVideo(Stream sourceStream)
	{
		Demux demux;
		return ExtractRawVideo(sourceStream, out demux);
	}

	public static byte[] ExtractRawVideo(Stream sourceStream, out Demux demux)
	{
		demux = Demux.forSource(sourceStream);
		demux.Init(sourceStream);
		if (!demux.hasVideo)
		{
			return null;
		}
		byte[] array = new byte[demux.videoStreamInfo.lengthBytes];
		int num = 0;
		int num2 = 0;
		do
		{
			num2 = demux.ReadVideoFrame(out var targetBuf);
			Array.Copy(targetBuf, 0, array, num, num2);
			num += num2;
		}
		while (num2 > 0);
		return array;
	}
}
