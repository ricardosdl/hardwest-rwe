using UnityEngine;
using UnityEngine.UI;

public class CFGCustomUIS7 : CFGPanel
{
	public CFGImageExtension m_Mercury;

	public CFGImageExtension m_Drilling;

	public CFGImageExtension m_Stampmill;

	public CFGImageExtension m_Jet;

	public Text m_SpherePlacerTxt;

	public Text m_SphereDeeperTxt;

	public Text m_SphereHardrockTxt;

	public Text m_UsesTxt;

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
	}

	public override void SetLocalisation()
	{
		m_Mercury.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_prospect_mercury");
		m_Drilling.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_prospect_drilling");
		m_Stampmill.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_prospect_stampmill");
		m_Jet.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_prospect_jet");
		m_UsesTxt.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_usesleft");
		m_SpherePlacerTxt.gameObject.transform.parent.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_mining_placer");
		m_SphereDeeperTxt.gameObject.transform.parent.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_mining_deeper");
		m_SphereHardrockTxt.gameObject.transform.parent.gameObject.GetComponent<CFGButtonExtension>().m_TooltipText = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("s7_customgui_mining_hardrock");
	}
}
