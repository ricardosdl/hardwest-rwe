using System.Collections.Generic;
using UnityEngine;

public class CFGGamepadStrategicPanel : CFGPanel
{
	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_LButtonPad;

	public CFGButtonExtension m_StartButtonPad;

	public CFGButtonExtension m_BackButtonPad;

	public List<GameObject> m_AllObjs = new List<GameObject>();

	protected override void Start()
	{
		base.Start();
		foreach (GameObject allObj in m_AllObjs)
		{
			allObj.SetActive(CFGInput.LastReadInputDevice == EInputMode.Gamepad);
		}
	}

	public override void SetLocalisation()
	{
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_goto");
		m_StartButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_menu");
		m_BackButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_characterscreen");
		m_LButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_move");
	}

	public override void Update()
	{
		base.Update();
		foreach (GameObject allObj in m_AllObjs)
		{
			allObj.SetActive(CFGInput.LastReadInputDevice == EInputMode.Gamepad);
		}
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null && !CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator.IsExplorationWindowVisible() && CFGSingleton<CFGWindowMgr>.Instance.m_CharacterScreen == null && CFGSingleton<CFGWindowMgr>.Instance.m_BarterScreen == null)
		{
			if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
			{
				m_AButtonPad.SimulateClickGraphicAndSoundOnly(bPlaySnd: false);
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.Start) > 0f)
			{
				m_StartButtonPad.SimulateClickGraphicAndSoundOnly();
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.Back) > 0f)
			{
				m_BackButtonPad.SimulateClickGraphicAndSoundOnly();
			}
		}
	}
}
