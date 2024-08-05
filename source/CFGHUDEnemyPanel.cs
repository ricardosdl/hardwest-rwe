using System.Collections.Generic;
using UnityEngine;

public class CFGHUDEnemyPanel : CFGPanel
{
	public List<CFGEnemyMark> m_EnemyMarks = new List<CFGEnemyMark>();

	public CFGButtonExtension m_BackPad;

	public CFGButtonExtension m_StartPad;

	public GameObject m_EnemiesPanel;

	protected override void Start()
	{
		base.Start();
		foreach (CFGEnemyMark enemyMark in m_EnemyMarks)
		{
			enemyMark.m_ButtonClickedCallback = OnEnemyMarkClick;
			enemyMark.m_ButtonOverCallback = OnEnemyMarkMouseOver;
			enemyMark.m_ButtonOutCallback = OnEnemyMarkMouseOut;
		}
	}

	public override void SetLocalisation()
	{
		foreach (CFGEnemyMark enemyMark in m_EnemyMarks)
		{
			enemyMark.m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tac_enemyicon");
		}
		m_BackPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_characterscreen");
		m_StartPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_explorationscene_menu");
	}

	public override void Update()
	{
		base.Update();
		if (CFGJoyManager.ReadAsButton(EJoyButton.Back) > 0f && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized() && CFGSingleton<CFGWindowMgr>.Instance.m_TacticalCharacterDetails == null && CFGSingleton<CFGWindowMgr>.Instance.m_InGameMenu == null)
		{
			m_BackPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	public void OnEnemyMarkClick(int id)
	{
		CFGSelectionManager.Instance.OnEnemyClick(id);
	}

	public void OnEnemyMarkMouseOver(int id)
	{
		CFGSelectionManager.Instance.OnEnemyMarkHover(id, hover: true);
	}

	public void OnEnemyMarkMouseOut(int id)
	{
		CFGSelectionManager.Instance.OnEnemyMarkHover(id, hover: false);
	}

	public void SetEnemiesPanelVisibility(bool visible)
	{
		m_EnemiesPanel.SetActive(visible);
	}
}
