using System;
using System.IO;
using MP;
using UnityEngine;

public class MoviePlayer : MoviePlayerBase
{
	public const string PACKAGE_VERSION = "v0.9";

	public TextAsset source;

	public AudioClip audioSource;

	public LoadOptions loadOptions = LoadOptions.Default;

	public float videoTime;

	public int videoFrame;

	public bool loop;

	protected float lastVideoTime;

	protected int lastVideoFrame;

	public event MovieEvent OnLoop;

	public bool Load(byte[] bytes)
	{
		return Load(new MemoryStream(bytes));
	}

	public bool Load(byte[] bytes, LoadOptions loadOptions)
	{
		return Load(new MemoryStream(bytes), loadOptions);
	}

	public bool Load(TextAsset textAsset)
	{
		source = textAsset;
		return Load(new MemoryStream(textAsset.bytes));
	}

	public bool Load(TextAsset textAsset, LoadOptions loadOptions)
	{
		source = textAsset;
		return Load(new MemoryStream(textAsset.bytes), loadOptions);
	}

	public bool Load(string path)
	{
		return Load(File.OpenRead(path));
	}

	public bool Load(string path, LoadOptions loadOptions)
	{
		return Load(File.OpenRead(path), loadOptions);
	}

	public bool Load(Stream srcStream)
	{
		return Load(srcStream, null);
	}

	public bool Load(Stream srcStream, LoadOptions loadOptions)
	{
		if (loadOptions == null)
		{
			loadOptions = this.loadOptions;
		}
		else
		{
			this.loadOptions = loadOptions;
		}
		bool flag = audioSource != null && !loadOptions.skipAudio;
		if (flag)
		{
			loadOptions.skipAudio = true;
		}
		bool result = false;
		try
		{
			if (flag)
			{
				audiobuffer = audioSource;
			}
			Load(new MovieSource
			{
				stream = srcStream
			}, loadOptions);
			if (!loadOptions.preloadVideo && movie.videoDecoder != null)
			{
				movie.videoDecoder.Decode(videoFrame);
			}
			UpdateRendererUVRect();
			result = true;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return result;
	}

	[ContextMenu("Reload")]
	public bool Reload()
	{
		bool result = true;
		if (source != null)
		{
			result = Load(source.bytes, loadOptions);
			lastVideoFrame = -1;
		}
		return result;
	}

	private void Start()
	{
		Reload();
	}

	private void OnGUI()
	{
		if (movie != null && movie.demux != null && movie.demux.videoStreamInfo != null && drawToScreen && framebuffer != null)
		{
			Rect value = movie.frameUV[videoFrame % movie.frameUV.Length];
			DrawFramebufferToScreen(value);
		}
	}

	private void Update()
	{
		HandlePlayStop();
		bool wasSeeked = HandlePlayheadMove();
		HandleFrameDecode(wasSeeked);
		if (play)
		{
			HandleAudioSync();
			HandleLoop();
		}
	}

	protected bool HandlePlayheadMove()
	{
		bool flag = videoFrame != lastVideoFrame;
		bool flag2 = videoTime != lastVideoTime;
		if (flag)
		{
			videoTime = (float)videoFrame / base.framerate;
		}
		else if (play)
		{
			videoTime += Time.deltaTime;
		}
		return flag || flag2;
	}

	protected void HandleFrameDecode(bool wasSeeked)
	{
		if (movie == null)
		{
			return;
		}
		videoFrame = Mathf.FloorToInt(videoTime * base.framerate);
		if (lastVideoFrame != videoFrame)
		{
			if (!loadOptions.preloadVideo && movie.videoDecoder != null)
			{
				movie.videoDecoder.Decode(videoFrame);
			}
			UpdateRendererUVRect();
			if (!wasSeeked && lastVideoFrame != videoFrame - 1)
			{
				int num = videoFrame - lastVideoFrame - 1;
				_framesDropped += num;
			}
		}
		lastVideoFrame = videoFrame;
		lastVideoTime = videoTime;
	}

	protected void HandleAudioSync()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (!(component == null) && component.enabled && !(component.clip == null) && videoTime <= component.clip.length && Mathf.Abs(videoTime - component.time) > (float)maxSyncErrorFrames / base.framerate)
		{
			component.Stop();
			component.time = videoTime;
			component.Play();
			_syncEvents++;
		}
	}

	protected void HandleLoop()
	{
		if (movie == null || movie.demux == null || movie.demux.videoStreamInfo == null || !(videoTime >= movie.demux.videoStreamInfo.lengthSeconds))
		{
			return;
		}
		if (loop)
		{
			videoTime = 0f;
			if (this.OnLoop != null)
			{
				this.OnLoop(this);
			}
			SendMessage("OnLoop", this, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			play = false;
		}
	}

	public void UpdateRendererUVRect()
	{
		Renderer component = GetComponent<Renderer>();
		if (movie != null && movie.frameUV != null && movie.frameUV.Length > 0)
		{
			Rect rect = movie.frameUV[videoFrame % movie.frameUV.Length];
			if (component != null && component.sharedMaterial != null)
			{
				component.sharedMaterial.SetTextureOffset(texturePropertyName, new Vector2(rect.x, rect.y));
				component.sharedMaterial.SetTextureScale(texturePropertyName, new Vector2(rect.width, rect.height));
			}
			if (material != null)
			{
				material.SetTextureOffset(texturePropertyName, new Vector2(rect.x, rect.y));
				material.SetTextureScale(texturePropertyName, new Vector2(rect.width, rect.height));
			}
		}
	}
}
