using UnityEngine;
using UnityEngine.Audio;

public class CFGVolumeTester : MonoBehaviour
{
	private float m_Duration;

	private float m_Offset = 0.5f;

	private float m_StartTime;

	private float m_NextTime;

	private CFGSoundDef m_CurrentSoundDef;

	private AudioMixerGroup m_CurrentGroup;

	private AudioSource m_CurrentAudioSource;

	public void Play(CFGSoundDef _soundDef, AudioMixerGroup _MixerGroup)
	{
		if (_soundDef == null || _soundDef.m_AudioClips == null || _soundDef.m_AudioClips.Length == 0 || _MixerGroup == null)
		{
			return;
		}
		m_CurrentSoundDef = _soundDef;
		if (!(CFGTimer.MissionTime <= m_NextTime) || !(_MixerGroup == m_CurrentGroup))
		{
			if (m_CurrentAudioSource != null)
			{
				m_CurrentAudioSource.volume = 0f;
			}
			int num = Random.Range(0, _soundDef.m_AudioClips.Length);
			AudioClip audioClip = _soundDef.m_AudioClips[num];
			m_CurrentAudioSource = CFGAudioManager.Instance.PlaySound2D(audioClip, _MixerGroup);
			m_StartTime = CFGTimer.MissionTime;
			m_Duration = audioClip.length;
			m_NextTime = m_StartTime + m_Duration + m_Offset;
			m_CurrentGroup = _MixerGroup;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) || CFGJoyManager.ReadAsButton(EJoyButton.KeyA, bContinous: true) > 0f)
		{
			Play(m_CurrentSoundDef, m_CurrentGroup);
		}
		else
		{
			Reset();
		}
	}

	private void Reset()
	{
		m_CurrentSoundDef = null;
	}

	private void OnDestroy()
	{
		if (!(m_CurrentAudioSource == null))
		{
			m_CurrentAudioSource.volume = 0f;
			Reset();
		}
	}
}
