using System.Collections.Generic;
using UnityEngine;

public class CFGActionButtonsPanel : CFGPanel
{
	public List<CFGButtonExtension> m_ActionButtons = new List<CFGButtonExtension>();

	public GameObject m_BtnPanel;

	private EInputMode m_LastInput = EInputMode.KeyboardAndMouse;

	protected override void Start()
	{
		base.Start();
		foreach (CFGButtonExtension actionButton in m_ActionButtons)
		{
			actionButton.m_ButtonClickedCallback = OnActionButton;
		}
	}

	public override void SetLocalisation()
	{
		m_ActionButtons[0].m_Label.text = CFGInput.GetKeyCombo(EActionCommand.Camera_RotateLeft);
		m_ActionButtons[1].m_Label.text = CFGInput.GetKeyCombo(EActionCommand.Camera_RotateRight);
		m_ActionButtons[2].m_Label.text = CFGInput.GetKeyCombo(EActionCommand.EndTurn);
		m_ActionButtons[0].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_rotateleft", FormatShortcut(EActionCommand.Camera_RotateLeft));
		m_ActionButtons[1].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_rotateright", FormatShortcut(EActionCommand.Camera_RotateRight));
		m_ActionButtons[2].m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_endturn", FormatShortcut(EActionCommand.EndTurn));
	}

	public void OnActionButton(int id)
	{
		CFGSelectionManager.Instance.OnActionClick(id);
	}

	public static string FormatShortcut(EActionCommand command)
	{
		string keyCombo = CFGInput.GetKeyCombo(command);
		if (keyCombo != string.Empty)
		{
			return "[<color=#FFBE35>" + keyCombo + "</color>]";
		}
		return string.Empty;
	}

	public override void Update()
	{
		base.Update();
		m_BtnPanel.gameObject.SetActive(CFGInput.LastReadInputDevice != EInputMode.Gamepad);
	}
}
