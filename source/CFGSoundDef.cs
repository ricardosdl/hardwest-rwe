using UnityEngine;

public class CFGSoundDef : MonoBehaviour
{
	public AudioSource m_AudioSource;

	public AudioClip[] m_AudioClips;

	public bool m_RandomPitch;

	public float m_MinPitch = 1f;

	public float m_MaxPitch = 1f;

	public bool m_RandomVolume;

	public float m_MinVolume = 1f;

	public float m_MaxVolume = 1f;

	public static void Play(CFGSoundDef sound_def, Transform transform)
	{
		if (sound_def == null || sound_def.m_AudioSource == null || sound_def.m_AudioClips.Length == 0)
		{
			return;
		}
		int num = Random.Range(0, sound_def.m_AudioClips.Length);
		AudioClip audioClip = sound_def.m_AudioClips[num];
		if (audioClip != null)
		{
			AudioSource audioSource = Object.Instantiate(sound_def.m_AudioSource, transform.position, transform.rotation) as AudioSource;
			audioSource.transform.parent = transform;
			audioSource.clip = audioClip;
			if (sound_def.m_RandomPitch)
			{
				audioSource.pitch = Random.Range(sound_def.m_MinPitch, sound_def.m_MaxPitch);
			}
			if (sound_def.m_RandomVolume)
			{
				audioSource.volume = Random.Range(sound_def.m_MinVolume, sound_def.m_MaxVolume);
			}
			audioSource.Play();
			Object.Destroy(audioSource.gameObject, audioClip.length);
		}
	}

	public static void Play(CFGSoundDef sound_def, Vector3 position)
	{
		if (sound_def == null || sound_def.m_AudioSource == null || sound_def.m_AudioClips.Length == 0)
		{
			return;
		}
		int num = Random.Range(0, sound_def.m_AudioClips.Length);
		AudioClip audioClip = sound_def.m_AudioClips[num];
		if (audioClip != null)
		{
			AudioSource audioSource = Object.Instantiate(sound_def.m_AudioSource, position, Quaternion.identity) as AudioSource;
			audioSource.clip = audioClip;
			if (sound_def.m_RandomPitch)
			{
				audioSource.pitch = Random.Range(sound_def.m_MinPitch, sound_def.m_MaxPitch);
			}
			if (sound_def.m_RandomVolume)
			{
				audioSource.volume = Random.Range(sound_def.m_MinVolume, sound_def.m_MaxVolume);
			}
			audioSource.Play();
			Object.Destroy(audioSource.gameObject, audioClip.length);
		}
	}

	public static void Play2D(CFGSoundDef sound_def)
	{
		if (sound_def == null || sound_def.m_AudioSource == null || sound_def.m_AudioClips.Length == 0)
		{
			return;
		}
		int num = Random.Range(0, sound_def.m_AudioClips.Length);
		AudioClip audioClip = sound_def.m_AudioClips[num];
		if (audioClip != null)
		{
			AudioSource audioSource = Object.Instantiate(sound_def.m_AudioSource);
			audioSource.clip = audioClip;
			if (sound_def.m_RandomPitch)
			{
				audioSource.pitch = Random.Range(sound_def.m_MinPitch, sound_def.m_MaxPitch);
			}
			if (sound_def.m_RandomVolume)
			{
				audioSource.volume = Random.Range(sound_def.m_MinVolume, sound_def.m_MaxVolume);
			}
			audioSource.Play();
			Object.Destroy(audioSource.gameObject, audioClip.length);
		}
	}
}
