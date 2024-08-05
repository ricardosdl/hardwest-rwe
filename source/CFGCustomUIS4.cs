using UnityEngine;
using UnityEngine.UI;

public class CFGCustomUIS4 : CFGPanel
{
	public Text m_Name1;

	public Text m_Name2;

	public Text m_Name3;

	public Text m_Description1;

	public Text m_Description2;

	public Text m_Description3;

	public CFGImageExtension m_Icon1;

	public CFGImageExtension m_Icon2;

	public CFGImageExtension m_Icon3;

	public GameObject m_NoIcon1;

	public GameObject m_NoIcon2;

	public GameObject m_NoIcon3;

	public GameObject m_Frame1;

	public GameObject m_Frame2;

	protected override void Start()
	{
		base.Start();
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
			Debug.Log(num * component.rect.width);
		}
		base.transform.position = new Vector3(0f, 0f);
		m_Icon1.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_Icon2.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
		m_Icon3.m_SpriteList = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_UIPrefab.m_CharacterIcons;
	}
}
