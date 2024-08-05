using UnityEngine;
using UnityEngine.UI;

public class CFGCustomUIS1 : CFGPanel
{
	public Text m_WantedTxt;

	public Text m_RewardTxt;

	public Text m_WantedDescTxt;

	public CFGImageExtension m_GunIcon;

	public CFGImageExtension m_ElixirIcon;

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

	public void SetReward(int reward)
	{
		m_RewardTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("customui_01_reward", reward.ToString());
	}

	public void SetRewardDesc(int deaths, int damage)
	{
		m_WantedDescTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("customui_01_desc", deaths.ToString(), damage.ToString());
	}

	public void SetIcons(int ic_gun, int ic_elixir)
	{
		m_GunIcon.transform.parent.gameObject.SetActive(ic_gun != -1);
		m_ElixirIcon.transform.parent.gameObject.SetActive(ic_elixir != -1);
		if (ic_gun != -1)
		{
			m_GunIcon.IconNumber = ic_gun;
		}
		if (ic_elixir != -1)
		{
			m_ElixirIcon.IconNumber = ic_elixir;
		}
	}

	public override void SetLocalisation()
	{
		m_WantedTxt.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("customui_01_wanted");
	}
}
