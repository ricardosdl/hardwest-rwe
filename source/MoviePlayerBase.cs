using System;
using MP;
using UnityEngine;

public abstract class MoviePlayerBase : MonoBehaviour
{
	public enum ScreenMode
	{
		Crop,
		Fill,
		Stretch,
		CustomRect
	}

	public delegate void MovieEvent(MoviePlayerBase caller);

	public Texture2D framebuffer;

	public AudioClip audiobuffer;

	public bool drawToScreen;

	[Obsolete("Use property named material instead, it works exactly like otherMaterial did. In later version otherMaterial will be removed")]
	public Material otherMaterial;

	public Material material;

	public string texturePropertyName = "_MainTex";

	public ScreenMode screenMode;

	public int screenGuiDepth;

	public Rect customScreenRect = new Rect(0f, 0f, 100f, 100f);

	public bool play;

	public Movie movie;

	public int maxSyncErrorFrames = 2;

	protected int _framesDropped;

	protected int _syncEvents;

	protected bool lastPlay;

	public int framesSkipped => _framesDropped;

	public int syncEvents => _syncEvents;

	public float framerate => (movie == null || movie.demux == null || movie.demux.videoStreamInfo == null) ? 30f : movie.demux.videoStreamInfo.framerate;

	public float lengthSeconds => (movie == null || movie.demux == null || movie.demux.videoStreamInfo == null) ? 0f : movie.demux.videoStreamInfo.lengthSeconds;

	public event MovieEvent OnPlay;

	public event MovieEvent OnStop;

	protected void Load(MovieSource source, LoadOptions loadOptions = null)
	{
		Texture2D targetFramebuffer;
		AudioClip targetAudioBuffer;
		Movie movie = MoviePlayerUtil.Load(source, out targetFramebuffer, out targetAudioBuffer, loadOptions);
		if (this.movie != null)
		{
			MoviePlayerUtil.Unload(this.movie);
		}
		this.movie = movie;
		_framesDropped = 0;
		_syncEvents = 0;
		framebuffer = targetFramebuffer;
		if (targetAudioBuffer != null)
		{
			audiobuffer = targetAudioBuffer;
		}
		Bind();
	}

	[ContextMenu("Unload (disconnect)")]
	public void Unload()
	{
		if (movie != null)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (component != null)
			{
				component.Stop();
			}
			MoviePlayerUtil.Unload(movie);
			movie = null;
		}
	}

	protected void Bind()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			component.sharedMaterial.SetTexture(texturePropertyName, framebuffer);
		}
		if (material != null)
		{
			material.SetTexture(texturePropertyName, framebuffer);
		}
		AudioSource audioSource = GetComponent<AudioSource>();
		if (audiobuffer != null)
		{
			if (audioSource == null)
			{
				audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			audioSource.clip = audiobuffer;
			audioSource.playOnAwake = false;
		}
		else if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.clip = null;
		}
	}

	protected void DrawFramebufferToScreen(Rect? sourceUV = null)
	{
		Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		if (screenMode == ScreenMode.CustomRect)
		{
			screenRect = customScreenRect;
		}
		else if (screenMode != ScreenMode.Stretch)
		{
			float num = (float)movie.demux.videoStreamInfo.width / (float)movie.demux.videoStreamInfo.height;
			float num2 = (float)Screen.width / (float)Screen.height;
			if (screenMode == ScreenMode.Crop)
			{
				if (num2 < num)
				{
					screenRect.height = Mathf.Round(screenRect.width / num);
				}
				else
				{
					screenRect.width = Mathf.Round(screenRect.height * num);
				}
			}
			else if (screenMode == ScreenMode.Fill)
			{
				if (num2 < num)
				{
					screenRect.width = Mathf.Round((float)Screen.height * num);
				}
				else
				{
					screenRect.height = Mathf.Round((float)Screen.width / num);
				}
			}
			screenRect.x = Mathf.Round(((float)Screen.width - screenRect.width) / 2f);
			screenRect.y = Mathf.Round(((float)Screen.height - screenRect.height) / 2f);
		}
		GUI.depth = screenGuiDepth;
		Event current = Event.current;
		if (current == null || current.type == EventType.Repaint)
		{
			if (!sourceUV.HasValue)
			{
				Graphics.DrawTexture(screenRect, framebuffer, material);
			}
			else
			{
				Graphics.DrawTexture(screenRect, framebuffer, sourceUV.Value, 0, 0, 0, 0, material);
			}
		}
	}

	protected void HandlePlayStop()
	{
		if (play == lastPlay)
		{
			return;
		}
		AudioSource component = GetComponent<AudioSource>();
		if (play)
		{
			if (this.OnPlay != null)
			{
				this.OnPlay(this);
			}
			SendMessage("OnPlay", this, SendMessageOptions.DontRequireReceiver);
			if (component != null)
			{
				component.Play();
				if (this is MoviePlayer)
				{
					component.time = ((MoviePlayer)this).videoTime;
				}
			}
		}
		else
		{
			if (this.OnStop != null)
			{
				this.OnStop(this);
			}
			SendMessage("OnStop", this, SendMessageOptions.DontRequireReceiver);
			if (component != null)
			{
				component.Stop();
			}
		}
		lastPlay = play;
	}
}
