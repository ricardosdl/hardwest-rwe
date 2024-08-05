using UnityEngine;
using UnityEngine.UI;

public class CFGTutorialPanel : CFGPanel
{
	public Text m_Text;

	public CFGButtonExtension m_ButtonOk;

	public Image m_Bg;

	public GameObject m_BottomLine;

	public Image m_Mask;

	public bool m_IsWithButton = true;

	public CanvasGroup m_CG;

	public CFGButtonExtension m_ABtn;

	private Vector3 m_BottomLinePosition = default(Vector3);

	protected override void Start()
	{
		base.Start();
		m_CG.alpha = 0f;
		if ((bool)m_BottomLine)
		{
			m_BottomLinePosition = m_BottomLine.transform.position;
		}
		m_ButtonOk.m_ButtonClickedCallback = OnOkButtonClick;
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_ABtn.gameObject.SetActive(m_IsWithButton && flag);
		m_ButtonOk.gameObject.SetActive(m_IsWithButton && !flag);
	}

	public override void Update()
	{
		base.Update();
		int num = 0;
		if (m_Text != null)
		{
			num = m_Text.cachedTextGenerator.lineCount;
		}
		m_Bg.transform.SetParent(m_Mask.transform.parent);
		m_Bg.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Bg.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Mask.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Mask.rectTransform.anchorMin = new Vector2(0f, ((!m_IsWithButton) ? 0.8f : 0.74f) - (float)num * 0.06f);
		m_Mask.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_Bg.transform.SetParent(m_Mask.transform);
		m_BottomLine.transform.position = new Vector3(m_BottomLinePosition.x, m_Mask.transform.position.y, m_BottomLinePosition.y);
		m_CG.alpha = 1f;
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_ABtn.gameObject.SetActive(m_IsWithButton && flag);
		m_ButtonOk.gameObject.SetActive(m_IsWithButton && !flag);
		if ((CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f || CFGInput.IsActivated(EActionCommand.Confirm)) && m_IsWithButton)
		{
			OnOkButtonClick(0);
			m_ABtn.SimulateClickGraphicAndSoundOnly();
		}
	}

	public override void SetLocalisation()
	{
		m_ABtn.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tutorial_panel_ok");
	}

	public void OnOkButtonClick(int a)
	{
		if (CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			CFGSingleton<CFGWindowMgr>.Instance.UnloadTutorialPanel();
		}
	}
}
