using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGEventStrategicPopup : UIBehaviour
{
	public CFGImageExtension m_CharacterImg;

	public CFGImageExtension m_ItemImg;

	public CFGImageExtension m_BuffImg;

	public CFGImageExtension m_IconExploratorImg;

	public CFGImageExtension m_IconCashPlace;

	public CFGImageExtension m_CardsIcon;

	public Text m_EventText;

	public int m_Number;

	protected override void Start()
	{
		base.Start();
		m_ItemImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_ItemsIcons;
		m_CardsIcon.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CardsIcons;
		m_CharacterImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_BuffImg.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_BuffsIcons;
	}

	public void OnEndAnim()
	{
		CFGSingleton<CFGWindowMgr>.Instance.UnloadStrategicEventPopup(this);
	}
}
