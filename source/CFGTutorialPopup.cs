using UnityEngine;
using UnityEngine.UI;

public class CFGTutorialPopup : CFGPanel
{
	public Text m_TitleText;

	public Text m_DescriptionText;

	public CFGButtonExtension m_ContinueBtn;

	public CFGImageExtension m_Image;

	public CFGButtonExtension m_ABtn;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	protected override void Start()
	{
		base.Start();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ABtn.gameObject.SetActive(flag);
			m_ContinueBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(Screen.width / 2, Screen.height / 2);
		m_ContinueBtn.m_ButtonClickedCallback = OnContinueButtonClick;
		CFGTimer.SetPaused_Gameplay(bPauseGameplay: true);
	}

	public override void Update()
	{
		base.Update();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ABtn.gameObject.SetActive(flag);
			m_ContinueBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f || CFGInput.IsActivated(EActionCommand.Confirm))
		{
			m_ABtn.SimulateClickGraphicAndSoundOnly();
			m_ContinueBtn.m_ButtonClickedCallback(0);
			CFGJoyManager.ClearJoyActions();
			Invoke("ContinueInvoke", 0.5f);
		}
	}

	public override void SetLocalisation()
	{
		m_ContinueBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_popup_continue");
		m_ABtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_popup_continue");
	}

	public void ContinueInvoke()
	{
		OnContinueButtonClick(0);
		CFGJoyManager.ClearJoyActions();
	}

	public void OnContinueButtonClick(int a)
	{
		if (!IsPanelWaitingForDestroy())
		{
			CFGTimer.SetPaused_Gameplay(bPauseGameplay: false);
			if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
			{
				CFGSingleton<CFGWindowMgr>.Instance.UnloadTutorialPopup();
			}
		}
	}
}
