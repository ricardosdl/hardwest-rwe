using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGEnemyMark : Selectable
{
	public delegate void OnButtonClickedDelegate(int button_data);

	public delegate void OnButtonOverDelegate(int button_data);

	public delegate void OnButtonOutDelegate(int button_data);

	public delegate void OnButtonDragDelegate(int button_data);

	public Image m_HpMask;

	public CFGImageExtension m_HpBar;

	public CFGImageExtension m_Frame;

	public Text m_HpText;

	public CFGMaskedProgressBar m_HitChance;

	public string m_TooltipText = string.Empty;

	public OnButtonClickedDelegate m_ButtonClickedCallback;

	public OnButtonOverDelegate m_ButtonOverCallback;

	public OnButtonOutDelegate m_ButtonOutCallback;

	public bool m_IsSelected;

	public int m_Data;

	[SerializeField]
	protected List<GameObject> m_HoverObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_ClickedObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_HighlightObjs = new List<GameObject>();

	private CFGTooltip m_Tooltip;

	private int m_HP = -1;

	private int m_MaxHP = -1;

	public int MaxHP
	{
		get
		{
			return m_MaxHP;
		}
		set
		{
			m_MaxHP = value;
			OnHPChanged();
		}
	}

	public void SetVisibleToPlayerState(bool visible)
	{
		if (visible)
		{
			m_HpBar.IconNumber = 0;
		}
		else
		{
			m_HpBar.IconNumber = 1;
		}
	}

	public void Update()
	{
		m_Frame.IconNumber = (m_IsSelected ? 1 : 0);
		if ((bool)m_Tooltip)
		{
			if (Input.mousePosition.x > (float)(Screen.width - 100 * Screen.width / 1600))
			{
				m_Tooltip.transform.position = new Vector3(Input.mousePosition.x - (float)(100 * Screen.width / 1600), Input.mousePosition.y, 0f);
			}
			else
			{
				m_Tooltip.transform.position = Input.mousePosition;
			}
		}
		foreach (GameObject highlightObj in m_HighlightObjs)
		{
			highlightObj.SetActive(m_IsSelected);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if ((bool)m_Tooltip)
		{
			Object.Destroy(m_Tooltip.gameObject);
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (m_ButtonOverCallback != null)
		{
			m_ButtonOverCallback(m_Data);
		}
		if (m_TooltipText != string.Empty && (bool)CFGSingleton<CFGWindowMgr>.Instance)
		{
			m_Tooltip = CFGSingleton<CFGWindowMgr>.Instance.LoadTooltip();
			if (m_Tooltip != null)
			{
				m_Tooltip.TooltipText = m_TooltipText;
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			hoverObj.SetActive(value: true);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (m_ButtonOutCallback != null)
		{
			m_ButtonOutCallback(m_Data);
		}
		if ((bool)m_Tooltip)
		{
			Object.Destroy(m_Tooltip.gameObject);
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			hoverObj.SetActive(value: false);
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			clickedObj.SetActive(value: true);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (m_ButtonClickedCallback != null && !eventData.dragging)
		{
			m_ButtonClickedCallback(m_Data);
		}
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			clickedObj.SetActive(value: false);
		}
	}

	public void SetHP(int val, bool known)
	{
		if (CFGGame.IsCustomGameplayOptionActive(ECustomGameplayOption.UnknownEnemyHealth) && val == m_MaxHP)
		{
			m_HpText.text = "?";
		}
		else
		{
			m_HpText.text = ((!known) ? "?" : val.ToString());
		}
		m_HP = val;
		OnHPChanged();
	}

	private void OnHPChanged()
	{
		m_HpBar.fillAmount = (float)m_HP / (float)m_MaxHP;
	}
}
