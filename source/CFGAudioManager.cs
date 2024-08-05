using UnityEngine;
using UnityEngine.Audio;

public class CFGAudioManager : MonoBehaviour
{
	private class BackgroundAudioSource
	{
		public enum EBackgroundState
		{
			FREE,
			NORMAL,
			FADING_OUT,
			FADING_IN
		}

		private AudioSource m_AudioSource;

		private EBackgroundState m_State;

		public void Play(AudioClip audio_clip)
		{
			m_AudioSource.clip = audio_clip;
			m_AudioSource.volume = 1f;
			m_AudioSource.Play();
			m_AudioSource.ignoreListenerVolume = true;
			m_State = EBackgroundState.NORMAL;
		}

		public void PlayWithFadeIn(AudioClip audio_clip)
		{
			m_AudioSource.clip = audio_clip;
			m_AudioSource.volume = 0f;
			m_AudioSource.Play();
			m_State = EBackgroundState.FADING_IN;
		}

		public void Stop()
		{
			m_AudioSource.volume = 0f;
			m_AudioSource.Stop();
			m_AudioSource.clip = null;
			m_State = EBackgroundState.FREE;
		}

		public void StopWithFadeOut()
		{
			FadeOut();
		}

		internal void Mute(bool bMute)
		{
			m_AudioSource.mute = bMute;
		}

		public bool IsPlaying()
		{
			return m_AudioSource != null && m_AudioSource.isPlaying;
		}

		public EBackgroundState GetState()
		{
			return m_State;
		}

		public bool IsFree()
		{
			return m_AudioSource != null && m_AudioSource.clip == null;
		}

		public bool ShouldSerialize()
		{
			if (m_State == EBackgroundState.NORMAL || m_State == EBackgroundState.FADING_IN)
			{
				return true;
			}
			return false;
		}

		public void Continue()
		{
			if (!(m_AudioSource == null) && !m_AudioSource.isPlaying)
			{
				m_AudioSource.Play();
			}
		}

		public string MusicName()
		{
			if (m_AudioSource != null)
			{
				return m_AudioSource.clip.name;
			}
			return null;
		}

		public void FadeOut()
		{
			switch (m_State)
			{
			case EBackgroundState.NORMAL:
			case EBackgroundState.FADING_IN:
				m_State = EBackgroundState.FADING_OUT;
				break;
			case EBackgroundState.FREE:
			case EBackgroundState.FADING_OUT:
				break;
			}
		}

		public void Init(AudioSource audio_source)
		{
			m_AudioSource = audio_source;
			m_AudioSource.priority = 0;
			m_AudioSource.loop = true;
			m_AudioSource.volume = 1f;
			m_AudioSource.outputAudioMixerGroup = Instance.m_MixMusic;
		}

		public void Update()
		{
			float num = 1f;
			switch (m_State)
			{
			case EBackgroundState.FADING_OUT:
			{
				float volume2 = m_AudioSource.volume;
				volume2 -= Time.deltaTime * 0.5f;
				if (volume2 > 0f)
				{
					m_AudioSource.volume = volume2;
					break;
				}
				m_AudioSource.volume = 0f;
				m_AudioSource.Stop();
				m_AudioSource.clip = null;
				m_State = EBackgroundState.FREE;
				break;
			}
			case EBackgroundState.FADING_IN:
			{
				float volume = m_AudioSource.volume;
				volume += Time.deltaTime * 0.5f;
				if (volume < num)
				{
					m_AudioSource.volume = volume;
					break;
				}
				m_AudioSource.volume = num;
				m_State = EBackgroundState.NORMAL;
				break;
			}
			default:
				m_AudioSource.volume = 1f;
				break;
			}
		}
	}

	public AudioMixer m_MainMixer;

	public AudioMixerGroup m_MixMaster;

	public AudioMixerGroup m_MixMusic;

	public AudioMixerGroup m_MixDialogs;

	public AudioMixerGroup m_MixEnviro;

	public AudioMixerGroup m_MixSFX;

	public AudioMixerGroup m_MixInterface;

	public AudioMixerGroup m_MixCinematic;

	private BackgroundAudioSource[] m_BackgroundAudioSources;

	private static CFGAudioManager _instance;

	public float MasterVolume
	{
		get
		{
			m_MainMixer.GetFloat("MasterVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("MasterVolume", value);
		}
	}

	public float MusicVolume
	{
		get
		{
			m_MainMixer.GetFloat("MusicVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("MusicVolume", value);
		}
	}

	public float DialogsVolume
	{
		get
		{
			m_MainMixer.GetFloat("DialogsVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("DialogsVolume", value);
		}
	}

	public float EnviroVolume
	{
		get
		{
			m_MainMixer.GetFloat("EnviroVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("EnviroVolume", value);
		}
	}

	public float SFXVolume
	{
		get
		{
			m_MainMixer.GetFloat("SFXVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("SFXVolume", value);
		}
	}

	public float InterfaceVolume
	{
		get
		{
			m_MainMixer.GetFloat("InterfaceVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("InterfaceVolume", value);
		}
	}

	public float CinematicVolume
	{
		get
		{
			m_MainMixer.GetFloat("CinematicVolume", out var value);
			return value;
		}
		set
		{
			m_MainMixer.SetFloat("CinematicVolume", value);
		}
	}

	private string BackGroudMusic
	{
		get
		{
			if (m_BackgroundAudioSources[0] != null && m_BackgroundAudioSources[0].ShouldSerialize())
			{
				return m_BackgroundAudioSources[0].MusicName();
			}
			if (m_BackgroundAudioSources[1] != null && m_BackgroundAudioSources[1].ShouldSerialize())
			{
				return m_BackgroundAudioSources[1].MusicName();
			}
			return string.Empty;
		}
	}

	public static CFGAudioManager Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = (CFGAudioManager)Object.FindObjectOfType(typeof(CFGAudioManager));
				if (!_instance)
				{
					Object @object = Resources.Load("Prefabs/AudioManager");
					if (!@object)
					{
						Debug.LogError("ERROR! Cannot load Assets/Resources/Prefabs/AudioManager.prefab");
						return null;
					}
					GameObject gameObject = (GameObject)Object.Instantiate(@object);
					_instance = gameObject.GetComponent<CFGAudioManager>();
				}
				_instance.Init();
			}
			return _instance;
		}
	}

	public static void PauseAllSounds(bool bPause)
	{
		AudioListener.pause = bPause;
	}

	public static bool GetMusicMuted()
	{
		return CFGOptions.Audio.MuteMusic;
	}

	public static bool GetSoundsMuted()
	{
		return CFGOptions.Audio.MuteSound;
	}

	public void DoMuteMusic(bool bMute)
	{
		m_BackgroundAudioSources[0].Mute(bMute);
		m_BackgroundAudioSources[1].Mute(bMute);
		CFGOptions.Audio.MuteMusic = bMute;
	}

	public static void MuteSounds(bool bMute)
	{
		CFGOptions.Audio.MuteSound = bMute;
		AudioListener.volume = ((!bMute) ? 1f : 0f);
	}

	public static float LinearToDb(float volume)
	{
		if (volume < 0.0001f)
		{
			return -80f;
		}
		return Mathf.Log10(volume) * 20f;
	}

	public static float DbToLinear(float volume)
	{
		return Mathf.Pow(10f, volume / 20f);
	}

	public void PlaySound3D(AudioClip audio_clip, Vector3 position, AudioMixerGroup mix_group)
	{
		if (audio_clip != null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SND3D_" + audio_clip.name;
			gameObject.transform.position = position;
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = mix_group;
			audioSource.spatialBlend = 1f;
			audioSource.clip = audio_clip;
			audioSource.Play();
			Object.Destroy(audioSource.gameObject, audio_clip.length);
		}
	}

	public AudioSource PlaySound2D(AudioClip audio_clip, AudioMixerGroup mix_group)
	{
		if (audio_clip != null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "SND2D_" + audio_clip.name;
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = mix_group;
			audioSource.spatialBlend = 0f;
			audioSource.clip = audio_clip;
			audioSource.Play();
			Object.Destroy(audioSource.gameObject, audio_clip.length);
			return audioSource;
		}
		return null;
	}

	public void PlayBackgroundMusic(AudioClip audio_clip)
	{
		if (audio_clip == null)
		{
			return;
		}
		if (m_BackgroundAudioSources[0].IsFree())
		{
			if (m_BackgroundAudioSources[1].IsFree())
			{
				m_BackgroundAudioSources[0].Play(audio_clip);
				return;
			}
			m_BackgroundAudioSources[0].PlayWithFadeIn(audio_clip);
			m_BackgroundAudioSources[1].StopWithFadeOut();
			return;
		}
		if (m_BackgroundAudioSources[1].IsFree())
		{
			m_BackgroundAudioSources[0].StopWithFadeOut();
			m_BackgroundAudioSources[1].PlayWithFadeIn(audio_clip);
			return;
		}
		Debug.LogWarning("ERROR! CFGAudioManager::PlayBackgroundMusic() - trying to change music while crossfading (new music: " + audio_clip.name + ", cross-fade between: " + m_BackgroundAudioSources[0].MusicName() + " and " + m_BackgroundAudioSources[1].MusicName());
		m_BackgroundAudioSources[0].PlayWithFadeIn(audio_clip);
		m_BackgroundAudioSources[1].Stop();
	}

	public void StopBackgroundMusic()
	{
		if (m_BackgroundAudioSources[0].IsPlaying())
		{
			m_BackgroundAudioSources[0].StopWithFadeOut();
		}
		if (m_BackgroundAudioSources[1].IsPlaying())
		{
			m_BackgroundAudioSources[1].StopWithFadeOut();
		}
	}

	public bool IsBackgroundMusicPlaying()
	{
		return m_BackgroundAudioSources[0].IsPlaying() || m_BackgroundAudioSources[1].IsPlaying();
	}

	public void StopAllMusic()
	{
		StopBackgroundMusic();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Init()
	{
		if (m_BackgroundAudioSources == null)
		{
			m_BackgroundAudioSources = new BackgroundAudioSource[2];
		}
		if (m_BackgroundAudioSources != null)
		{
			if (m_BackgroundAudioSources[0] == null)
			{
				m_BackgroundAudioSources[0] = new BackgroundAudioSource();
				m_BackgroundAudioSources[0].Init(base.gameObject.AddComponent<AudioSource>());
			}
			if (m_BackgroundAudioSources[1] == null)
			{
				m_BackgroundAudioSources[1] = new BackgroundAudioSource();
				m_BackgroundAudioSources[1].Init(base.gameObject.AddComponent<AudioSource>());
			}
		}
	}

	private void Update()
	{
		m_BackgroundAudioSources[0].Update();
		m_BackgroundAudioSources[1].Update();
		if (CFGInput.IsActivated(EActionCommand.SYS_ToggleMusic))
		{
			DoMuteMusic(!CFGOptions.Audio.MuteMusic);
		}
		if (CFGInput.IsActivated(EActionCommand.SYS_ToggleSound))
		{
			MuteSounds(!CFGOptions.Audio.MuteSound);
		}
	}

	private AudioClip FindAudioSourceByName(string Name, ref AudioClip[] alist)
	{
		if (Name == null || Name == string.Empty)
		{
			return null;
		}
		AudioClip[] array = alist;
		foreach (AudioClip audioClip in array)
		{
			if (audioClip != null && string.Compare(Name, audioClip.name, ignoreCase: true) == 0)
			{
				return audioClip;
			}
		}
		return null;
	}

	public void OnMissionEnd()
	{
		StopAllMusic();
	}

	public bool OnDeserialize(CFG_SG_Node node)
	{
		AudioClip[] alist = Resources.FindObjectsOfTypeAll(typeof(AudioClip)) as AudioClip[];
		StopAllMusic();
		string text = node.Attrib_Get("Main", string.Empty);
		string text2 = node.Attrib_Get("Back", string.Empty);
		AudioClip audioClip = FindAudioSourceByName(text2, ref alist);
		if (audioClip != null)
		{
			PlayBackgroundMusic(audioClip);
		}
		return true;
	}

	public void OnSerialize(CFG_SG_Node Node)
	{
		string empty = string.Empty;
		Node.Attrib_Set("Main", empty);
		Node.Attrib_Set("Back", BackGroudMusic);
	}

	public void OnPostSerialize()
	{
		m_BackgroundAudioSources[0].Continue();
		m_BackgroundAudioSources[1].Continue();
	}
}
