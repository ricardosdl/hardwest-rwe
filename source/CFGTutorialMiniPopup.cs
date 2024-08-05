using UnityEngine.UI;

public class CFGTutorialMiniPopup : CFGPanel
{
	public Text m_Header;

	public Text m_Text;

	public CFGButtonExtension m_YBtn;

	public CFGButtonExtension m_NBtn;

	public CFGButtonExtension m_ABtn;

	public CFGButtonExtension m_BBtn;

	private EInputMode m_LastInput = EInputMode.Gamepad;

	protected override void Start()
	{
		base.Start();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ABtn.gameObject.SetActive(flag);
			m_BBtn.gameObject.SetActive(flag);
			m_YBtn.gameObject.SetActive(!flag);
			m_NBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
	}

	public override void SetLocalisation()
	{
		m_Header.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_h");
		m_Text.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_desc");
		m_YBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_yes");
		m_NBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_no");
		m_ABtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_yes");
		m_BBtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_init_no");
	}

	public override void Update()
	{
		base.Update();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		if (CFGInput.LastReadInputDevice != m_LastInput)
		{
			m_ABtn.gameObject.SetActive(flag);
			m_BBtn.gameObject.SetActive(flag);
			m_YBtn.gameObject.SetActive(!flag);
			m_NBtn.gameObject.SetActive(!flag);
			m_LastInput = CFGInput.LastReadInputDevice;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			m_ABtn.SimulateClickGraphicAndSoundOnly();
			m_YBtn.m_ButtonClickedCallback(0);
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f)
		{
			m_BBtn.SimulateClickGraphicAndSoundOnly();
			m_NBtn.m_ButtonClickedCallback(1);
		}
	}
}
