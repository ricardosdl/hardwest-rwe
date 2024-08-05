using UnityEngine;
using UnityEngine.UI;

public class CFGLoadingScreen : CFGPanel
{
	public CFGImageExtension m_Bg;

	public Text m_Title;

	public CFGButtonExtension m_BtnPlay;

	public CFGMaskedProgressBar m_ProgressBar;

	public Text m_BtnLabel;

	public GameObject m_Spinner1;

	public GameObject m_Spinner2;

	public CFGButtonExtension m_AButtonPad;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	private bool bShowedmsg;

	private float m_SavedEnviroVolume;

	private float m_SavedSfxVolume;

	protected override void Start()
	{
		base.Start();
		m_SavedEnviroVolume = CFGAudioManager.Instance.EnviroVolume;
		m_SavedSfxVolume = CFGAudioManager.Instance.SFXVolume;
		CFGAudioManager.Instance.EnviroVolume = -80f;
		CFGAudioManager.Instance.SFXVolume = -80f;
		m_Spinner1.SetActive(value: true);
		m_Spinner2.SetActive(value: false);
		if (m_ProgressBar != null)
		{
			m_ProgressBar.SetProgress(0);
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (m_BtnPlay != null)
		{
			m_BtnPlay.m_ButtonClickedCallback = OnBtnPlay;
		}
		if (m_LastInput != CFGInput.LastReadInputDevice)
		{
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		CFGSession session = CFGSingleton<CFGGame>.Instance.GetSession();
		if (session != null)
		{
			if (m_Bg != null)
			{
				m_Bg.IconNumber = session.GetLoadingScreenID();
			}
			if (m_Title != null)
			{
				if (CFG_SG_Manager.LoadType == CFG_SG_Manager.eLoadType.ResetTactical)
				{
					string text_id = $"hint_loading_{Random.Range(1, 31):00}";
					m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(text_id);
				}
				else
				{
					m_Title.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(session.GetCurrentTextID() + "_name");
				}
			}
		}
		if (Screen.width != 1600 || Screen.height != 900)
		{
			float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			}
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
	}

	public override void SetLocalisation()
	{
		m_BtnLabel.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("loading_play");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("loading_play");
	}

	public override void Update()
	{
		base.Update();
		if (IsPanelWaitingForDestroy())
		{
			float t = 1f - m_FadeAlpha;
			CFGAudioManager.Instance.EnviroVolume = Mathf.Lerp(-80f, m_SavedEnviroVolume, t);
			CFGAudioManager.Instance.SFXVolume = Mathf.Lerp(-80f, m_SavedSfxVolume, t);
			return;
		}
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		if (sessionSingle == null)
		{
			return;
		}
		float loadingProgress = sessionSingle.GetLoadingProgress();
		if (loadingProgress > 1.1f)
		{
			if (!bShowedmsg)
			{
				bShowedmsg = true;
				if (m_BtnPlay != null)
				{
					Debug.Log("gamepad" + flag);
					m_BtnPlay.gameObject.SetActive(!flag);
					m_AButtonPad.transform.parent.gameObject.SetActive(flag);
				}
				m_Spinner1.SetActive(value: false);
				m_Spinner2.SetActive(value: true);
			}
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f || CFGJoyManager.ReadAsButton(EJoyButton.Start) > 0f)
			{
				m_BtnPlay.SimulateClick();
			}
		}
		if (m_LastInput != CFGInput.LastReadInputDevice && bShowedmsg)
		{
			m_LastInput = CFGInput.LastReadInputDevice;
			m_BtnPlay.gameObject.SetActive(!flag);
			m_AButtonPad.transform.parent.gameObject.SetActive(flag);
		}
		if (m_ProgressBar != null)
		{
			int progress = (int)(Mathf.Clamp01(loadingProgress) * 100f);
			m_ProgressBar.SetProgress(progress);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			m_AButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	public void OnBtnPlay(int a)
	{
		Input.ResetInputAxes();
		CFGJoyManager.ClearJoyActions();
		CFGSingleton<CFGWindowMgr>.Instance.UnloadLoadingScreen(OnFadeOutFinished);
		CFGSessionSingle sessionSingle = CFGSingleton<CFGGame>.Instance.SessionSingle;
		if (sessionSingle != null)
		{
			sessionSingle.StartLevel();
		}
	}

	private void OnFadeOutFinished()
	{
		CFGAudioManager.Instance.EnviroVolume = m_SavedEnviroVolume;
		CFGAudioManager.Instance.SFXVolume = m_SavedSfxVolume;
	}
}
