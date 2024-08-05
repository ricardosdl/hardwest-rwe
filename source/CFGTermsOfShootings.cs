using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGTermsOfShootings : CFGPanel
{
	public CFGButtonExtension m_AButtonPad;

	public CFGButtonExtension m_BButtonPad;

	public CFGButtonExtension m_LBButtonPad;

	public CFGButtonExtension m_BBButtonPad;

	public CFGButtonExtension m_LTButtonPad;

	public CFGButtonExtension m_RTButtonPad;

	public Image m_APImg;

	public Image m_APImg2;

	public Image m_LuckImg;

	public Text m_LuckTxt;

	public CFGTextExtension m_AbilityName;

	public CFGTextExtension m_Name1;

	public CFGTextExtension m_DamageVal;

	public CFGTextExtension m_DamageText;

	public CFGTextExtension m_Info;

	public CFGTextExtension m_EnemyInfo;

	public CFGTextExtension m_ChanceToHitText;

	public CFGTextExtension m_ChanceToHitValue;

	public CFGButtonExtension m_ConfirmButton;

	public CFGButtonExtension m_MoreInfoButton;

	public CFGButtonExtension m_RevertButton;

	public CFGButtonExtension m_NextTargetButton;

	public CFGButtonExtension m_PrevTargetButton;

	public Image m_EndTurnIcon;

	public GameObject m_MoreInfoWindow;

	public CFGButtonExtension m_CloseButton;

	public CFGTextExtension m_Name2;

	public List<CFGTextExtension> m_ParamNames = new List<CFGTextExtension>();

	public List<CFGTextExtension> m_ParamVals = new List<CFGTextExtension>();

	public CFGImageExtension m_BgCover;

	public Image m_HpMask;

	public CFGImageExtension m_HpBar;

	public CFGImageExtension m_Cover;

	public Text m_HpText;

	public Image m_Bg;

	public GameObject m_TextsList;

	private Vector3 m_TextsListPosition = default(Vector3);

	public int MaxHP { get; set; }

	public int AP
	{
		set
		{
			m_APImg.gameObject.SetActive(value > 0);
			m_APImg2.gameObject.SetActive(value > 1);
		}
	}

	public int Luck
	{
		set
		{
			m_LuckImg.gameObject.SetActive(value > 0);
			m_LuckTxt.gameObject.SetActive(value > 0);
			if (value > 0)
			{
				m_LuckTxt.text = value.ToString();
			}
		}
	}

	public int HP
	{
		set
		{
			m_HpText.text = value.ToString();
			m_HpBar.transform.SetParent(m_HpMask.transform.parent);
			m_HpBar.rectTransform.offsetMax = new Vector2(1f, 1f);
			m_HpBar.rectTransform.offsetMin = new Vector2(0f, 0f);
			m_HpBar.rectTransform.anchorMin = new Vector2(0f, 0f);
			m_HpBar.rectTransform.anchorMax = new Vector2(1f, 1f);
			m_HpMask.rectTransform.offsetMax = new Vector2(1f, 1f);
			m_HpMask.rectTransform.offsetMin = new Vector2(0f, 0f);
			m_HpMask.rectTransform.anchorMin = new Vector2(0f, 0.23f);
			m_HpMask.rectTransform.anchorMax = new Vector2(1f, 0.23f + (float)value / (float)MaxHP * 0.57f);
			m_HpBar.transform.SetParent(m_HpMask.transform);
		}
	}

	public int Cover
	{
		set
		{
			m_Cover.IconNumber = value - 1;
			m_BgCover.gameObject.SetActive(value > 0);
			m_Cover.gameObject.SetActive(value > 0);
		}
	}

	protected override void Start()
	{
		base.Start();
		m_MoreInfoButton.m_ButtonClickedCallback = OpenMoreInfo;
		m_CloseButton.m_ButtonClickedCallback = CloseMoreInfo;
		m_ConfirmButton.m_ButtonClickedCallback = CFGSelectionManager.Instance.OnConfirmationAttackClick;
		m_RevertButton.m_ButtonClickedCallback = CFGSelectionManager.Instance.OnRightClick;
		m_PrevTargetButton.m_ButtonClickedCallback = CFGSelectionManager.Instance.OnPrevTarget;
		m_NextTargetButton.m_ButtonClickedCallback = CFGSelectionManager.Instance.OnNextTarget;
		CloseMoreInfo(0);
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if ((bool)component)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num * component.rect.width);
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num * component.rect.height);
		}
		if (Screen.width < 1600 || Screen.height < 900)
		{
			base.transform.position = new Vector3(Screen.width / 2, (float)(Screen.height / 2) + 20f * (float)Screen.height / 900f);
		}
		else if (Screen.width > 1600 || Screen.height > 900)
		{
			base.transform.position = new Vector3(Screen.width / 2, (float)(Screen.height / 2) - 20f * (float)Screen.height / 900f);
		}
		m_TextsListPosition = m_TextsList.transform.position;
	}

	public override void Update()
	{
		base.Update();
		bool flag = CFGInput.LastReadInputDevice == EInputMode.Gamepad;
		m_CloseButton.gameObject.SetActive(!flag);
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyB) > 0f && m_BBButtonPad.enabled)
		{
			m_BBButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.RightBumper) > 0f)
		{
			m_RTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.LeftBumper) > 0f)
		{
			m_LTButtonPad.SimulateClickGraphicAndSoundOnly();
		}
	}

	public override void SetLocalisation()
	{
		m_ConfirmButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_confirm");
		m_RevertButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_revert");
		m_MoreInfoButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_moreinfo");
		m_CloseButton.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_close");
		m_BButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_close");
		m_LBButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_moreinfo");
		m_AButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tos_confirm");
		m_BBButtonPad.m_Label.text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("pad_ricochet_revert");
	}

	public void ClearParams()
	{
		foreach (CFGTextExtension paramName in m_ParamNames)
		{
			paramName.text = string.Empty;
		}
		foreach (CFGTextExtension paramVal in m_ParamVals)
		{
			paramVal.text = string.Empty;
		}
		m_Bg.rectTransform.offsetMax = new Vector2(1f, 1f);
		m_Bg.rectTransform.offsetMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMin = new Vector2(0f, 0f);
		m_Bg.rectTransform.anchorMax = new Vector2(1f, 1f);
		m_TextsList.transform.position = m_TextsListPosition;
	}

	public void SetMoreInfoWindowSize(int number_of_texts)
	{
		if (number_of_texts < 4)
		{
			m_Bg.rectTransform.offsetMax = new Vector2(1f, 1f);
			m_Bg.rectTransform.offsetMin = new Vector2(0f, 0f);
			m_Bg.rectTransform.anchorMin = new Vector2(0f, 0f);
			m_Bg.rectTransform.anchorMax = new Vector2(1f, 4f / (float)m_ParamNames.Count + 0.15f);
			m_TextsList.transform.Translate(0f, -150f * (float)Screen.height / 900f, 0f);
		}
		else if (m_ParamNames.Count > 0 && m_Bg != null)
		{
			m_Bg.rectTransform.offsetMax = new Vector2(1f, 1f);
			m_Bg.rectTransform.offsetMin = new Vector2(0f, 0f);
			m_Bg.rectTransform.anchorMin = new Vector2(0f, 0f);
			m_Bg.rectTransform.anchorMax = new Vector2(1f, (float)number_of_texts / (float)m_ParamNames.Count + 0.15f);
			m_TextsList.transform.Translate(0f, (float)(-(m_ParamNames.Count - number_of_texts)) * 15f * (float)Screen.height / 900f, 0f);
		}
	}

	public void SetParam(int idx, string param_name, string param_val)
	{
		if (idx < m_ParamNames.Count)
		{
			m_ParamNames[idx].text = param_name;
			int num = 0;
			try
			{
				num = int.Parse(param_val);
			}
			catch
			{
			}
			if (num > 0)
			{
				m_ParamVals[idx].text = "<color=#96BE46>" + param_val + "%</color>";
			}
			else if (num < 0)
			{
				m_ParamVals[idx].text = "<color=#EA4242>" + param_val + "%</color>";
			}
			else
			{
				m_ParamVals[idx].text = param_val;
			}
		}
	}

	public void CloseMoreInfo(int a)
	{
		m_MoreInfoWindow.SetActive(value: false);
	}

	public void OpenMoreInfo(int a)
	{
		m_MoreInfoWindow.SetActive(value: true);
	}

	public void OnConfirmationAttackClick(int a)
	{
		base.gameObject.SetActive(value: false);
	}
}
