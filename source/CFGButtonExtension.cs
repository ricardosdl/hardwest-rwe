using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CFGButtonExtension : Button, IDragHandler, IEventSystemHandler, IDropHandler
{
	public delegate void OnButtonClickedDelegate(int button_data);

	public delegate void OnButtonOverDelegate(int button_data);

	public delegate void OnButtonOutDelegate(int button_data);

	public delegate void OnButtonDragDelegate(int button_data);

	public delegate void OnButtonDropDelegate(int button_data);

	public delegate void OnButtonStaysClickedDelegate(int button_data);

	public delegate void OnDoubleClickDelegate(int button_data);

	public delegate void OnRightClickDelegate(int button_data);

	public delegate void OnInactiveButtonOverDelegate(int button_data);

	public delegate void OnInactiveButtonOutDelegate(int button_data);

	public OnButtonClickedDelegate m_ButtonClickedCallback;

	public OnButtonOverDelegate m_ButtonOverCallback;

	public OnButtonOutDelegate m_ButtonOutCallback;

	public OnButtonDragDelegate m_ButtonDragCallback;

	public OnButtonDropDelegate m_ButtonRealeseOutsideCallback;

	public OnButtonDropDelegate m_ButtonDropCallback;

	public OnButtonStaysClickedDelegate m_ButtonStaysClickedCallback;

	public OnDoubleClickDelegate m_OnDoubleClickCallback;

	public OnRightClickDelegate m_OnRightClickCallback;

	public OnButtonOverDelegate m_InactiveButtonOverCallback;

	public OnButtonOutDelegate m_InactiveButtonOutCallback;

	[SerializeField]
	protected int m_IconNumber;

	[SerializeField]
	protected int m_SelectedIconOffset;

	public List<Sprite> m_SpriteList = new List<Sprite>();

	public Image m_Icon;

	public CFGImageExtension m_AdditionalIcon;

	[SerializeField]
	protected Image m_HoverImg;

	[SerializeField]
	protected Image m_ClickedImg;

	[SerializeField]
	protected Image m_HighlightImg;

	public List<GameObject> m_HoverObjs = new List<GameObject>();

	public List<GameObject> m_ClickedObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_HighlightObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_DisabledObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_NormalObjs = new List<GameObject>();

	[SerializeField]
	protected List<GameObject> m_UseMeObjs = new List<GameObject>();

	[SerializeField]
	protected Color m_HoverTextColor = default(Color);

	[SerializeField]
	protected Color m_ClickedTextColor = default(Color);

	[SerializeField]
	protected Color m_NormalTextColor = default(Color);

	[SerializeField]
	protected Color m_DisabledTextColor = default(Color);

	[SerializeField]
	protected int m_HoverFontSize;

	[SerializeField]
	protected int m_ClickedFontSize;

	[SerializeField]
	protected int m_NormalFontSize;

	protected bool m_IsSelected;

	[HideInInspector]
	public bool m_IsHighlighted;

	public Text m_Label;

	public Vector3 m_LabelHoverPosMod = default(Vector3);

	public Vector3 m_LabelClickedPosMod = default(Vector3);

	public Vector3 m_LabelDisabledPosMod = default(Vector3);

	public int m_Data;

	public int m_UserData1 = -1;

	public int m_UserData2 = -1;

	public string m_DataString = string.Empty;

	public string m_TooltipText = string.Empty;

	public bool m_NeedBigTooltup;

	public AudioClip m_OnClickSound;

	public AudioClip m_OnHoverSound;

	[HideInInspector]
	private bool m_bIsUnderCursor;

	private bool m_MyIsUnderCursor;

	public bool m_Draggable;

	public bool m_VisualsDisabled;

	protected GameObject m_DraggingObj;

	protected static CFGButtonExtension m_IsClicked;

	protected Vector3 m_LabelNormalPos = default(Vector3);

	private bool m_IsInUseMeState;

	private CFGTooltip m_Tooltip;

	public CFGStoreItem m_ShopItem;

	public Transform m_DragParent;

	public bool m_ShouldDragSelectItem;

	private bool m_IsCursorClickedOverGameObject;

	private float m_CursorClickedTickLastTime = -1f;

	public float m_CursorClickedTick = 0.5f;

	public bool m_IsCursorHoveredOverGameObject;

	private static bool m_bWaiting;

	private static bool m_Deselect;

	private bool m_Call = true;

	public bool IsUnderCursor => m_bIsUnderCursor;

	public int IconNumber
	{
		get
		{
			return m_IconNumber;
		}
		set
		{
			m_IconNumber = value;
			UpdateIconNumber();
		}
	}

	public bool IsSelected
	{
		get
		{
			return m_IsSelected;
		}
		set
		{
			m_IsSelected = value;
			UpdateIsSelected();
		}
	}

	public bool IsInUseMeState
	{
		get
		{
			return m_IsInUseMeState;
		}
		set
		{
			m_IsInUseMeState = value;
			UpdateIsInUseMeState();
		}
	}

	public bool IsDraggable
	{
		get
		{
			return m_Draggable;
		}
		set
		{
			if (value != m_Draggable)
			{
				m_Draggable = value;
			}
		}
	}

	public static bool IsWaitingForClick => m_bWaiting;

	public override Selectable FindSelectableOnDown()
	{
		return null;
	}

	public override Selectable FindSelectableOnLeft()
	{
		return null;
	}

	public override Selectable FindSelectableOnRight()
	{
		return null;
	}

	public override Selectable FindSelectableOnUp()
	{
		return null;
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)m_Label)
		{
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_Label.transform.position != Vector3.zero)
			{
				m_LabelNormalPos = m_Label.transform.position;
			}
		}
		ResetButton();
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.currentSelectedGameObject)
		{
			CFGButtonExtension component = CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.currentSelectedGameObject.GetComponent<CFGButtonExtension>();
			if (component == this)
			{
				component.OnPointerEnter(null);
			}
		}
	}

	protected void Update()
	{
		if ((bool)m_Tooltip)
		{
			if ((CFGInput.LastReadInputDevice == EInputMode.Gamepad || !m_IsCursorHoveredOverGameObject) && (bool)m_Tooltip)
			{
				Object.Destroy(m_Tooltip.gameObject);
			}
			if (m_TooltipText != m_Tooltip.m_TooltipText.text)
			{
				m_Tooltip.TooltipText = m_TooltipText;
			}
		}
		else if (m_IsCursorHoveredOverGameObject && m_TooltipText != string.Empty && CFGInput.LastReadInputDevice != EInputMode.Gamepad && CFGSingleton<CFGWindowMgr>.IsInstanceInitialized())
		{
			m_Tooltip = CFGSingleton<CFGWindowMgr>.Instance.LoadTooltip();
			if (m_Tooltip != null)
			{
				m_Tooltip.TooltipText = m_TooltipText;
			}
		}
		if (m_IsCursorHoveredOverGameObject && m_HoverObjs.Count > 0 && m_HoverObjs[0] != null && !m_HoverObjs[0].activeSelf)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			OnPointerEnter(pointerEventData);
		}
		if (!m_IsCursorHoveredOverGameObject && m_HoverObjs.Count > 0 && m_HoverObjs[0] != null && m_HoverObjs[0].activeSelf)
		{
			PointerEventData pointerEventData2 = new PointerEventData(EventSystem.current);
			pointerEventData2.position = Input.mousePosition;
			OnPointerExit(pointerEventData2);
		}
		if (m_VisualsDisabled)
		{
			foreach (GameObject hoverObj in m_HoverObjs)
			{
				if (hoverObj != null)
				{
					hoverObj.SetActive(value: false);
				}
			}
		}
		CFGImageExtension cFGImageExtension = m_Icon as CFGImageExtension;
		if ((bool)cFGImageExtension && cFGImageExtension.m_SpriteList != m_SpriteList)
		{
			cFGImageExtension.m_SpriteList = m_SpriteList;
			UpdateIconNumber();
		}
		UpdateIsInUseMeState();
		if (m_IsCursorClickedOverGameObject && m_CursorClickedTickLastTime + m_CursorClickedTick < Time.time)
		{
			if (m_ButtonStaysClickedCallback != null)
			{
				m_ButtonStaysClickedCallback(m_Data);
				m_CursorClickedTick -= m_CursorClickedTick / 10f;
			}
			m_CursorClickedTickLastTime = Time.time;
		}
		if (!m_IsCursorClickedOverGameObject)
		{
			m_CursorClickedTick = 0.1f;
		}
		if (m_VisualsDisabled && m_DisabledObjs.Count > 0)
		{
			bool flag = false;
			foreach (GameObject disabledObj in m_DisabledObjs)
			{
				if (!disabledObj.activeSelf)
				{
					flag = true;
				}
			}
			foreach (GameObject normalObj in m_NormalObjs)
			{
				if (normalObj.activeSelf)
				{
					flag = true;
				}
			}
			if (flag)
			{
				DisableVisuals();
			}
		}
		else
		{
			if (m_VisualsDisabled || m_DisabledObjs.Count <= 0)
			{
				return;
			}
			bool flag2 = false;
			foreach (GameObject disabledObj2 in m_DisabledObjs)
			{
				if (disabledObj2.activeSelf)
				{
					flag2 = true;
				}
			}
			foreach (GameObject normalObj2 in m_NormalObjs)
			{
				if (!normalObj2.activeSelf)
				{
					flag2 = true;
				}
			}
			foreach (GameObject hoverObj2 in m_HoverObjs)
			{
				if (hoverObj2.activeSelf)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				EnableVisuals();
			}
		}
	}

	public void LateUpdate()
	{
		if ((bool)EventSystem.current && !EventSystem.current.IsPointerOverGameObject() && m_IsCursorHoveredOverGameObject && CFGInput.LastReadInputDevice != EInputMode.Gamepad)
		{
			OnPointerExit(null);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject disabledObj in m_DisabledObjs)
		{
			if (disabledObj != null)
			{
				disabledObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if (normalObj != null)
			{
				normalObj.SetActive(value: true);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_NormalTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
		if ((bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.currentSelectedGameObject)
		{
			CFGButtonExtension component2 = CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.currentSelectedGameObject.GetComponent<CFGButtonExtension>();
			if (component2 == this)
			{
				component2.OnPointerEnter(null);
			}
		}
		m_bWaiting = false;
	}

	protected override void OnDisable()
	{
		m_IsCursorHoveredOverGameObject = false;
		m_IsCursorClickedOverGameObject = false;
		if ((bool)m_Tooltip)
		{
			Object.Destroy(m_Tooltip.gameObject);
		}
		base.OnDisable();
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if (normalObj != null)
			{
				normalObj.SetActive(value: false);
			}
		}
		foreach (GameObject disabledObj in m_DisabledObjs)
		{
			if (disabledObj != null)
			{
				disabledObj.SetActive(value: true);
			}
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_DisabledTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
		}
		m_bWaiting = false;
	}

	public void SetVisualsEnabled(bool enable)
	{
		if (m_VisualsDisabled != !enable)
		{
			if (enable)
			{
				EnableVisuals();
			}
			else
			{
				DisableVisuals();
			}
		}
	}

	public void DisableVisuals()
	{
		m_VisualsDisabled = true;
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if (normalObj != null)
			{
				normalObj.SetActive(value: false);
			}
		}
		foreach (GameObject disabledObj in m_DisabledObjs)
		{
			if (disabledObj != null)
			{
				disabledObj.SetActive(value: true);
			}
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_DisabledTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
		}
	}

	public void EnableVisuals()
	{
		m_VisualsDisabled = false;
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject disabledObj in m_DisabledObjs)
		{
			if (disabledObj != null)
			{
				disabledObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if (normalObj != null)
			{
				normalObj.SetActive(value: true);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_NormalTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
	}

	public void ResetButton()
	{
		UpdateIconNumber();
		UpdateIsSelected();
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if (normalObj != null)
			{
				normalObj.SetActive(value: true);
			}
		}
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		if ((bool)m_Label)
		{
			m_Label.color = ((!m_VisualsDisabled) ? m_NormalTextColor : m_DisabledTextColor);
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && (m_LabelHoverPosMod != Vector3.zero || m_LabelClickedPosMod != Vector3.zero))
			{
				m_Label.transform.position = m_LabelNormalPos;
			}
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
	}

	public void UpdateLabelTextColor()
	{
		if ((bool)m_Label)
		{
			m_Label.color = ((!m_VisualsDisabled) ? m_NormalTextColor : m_DisabledTextColor);
		}
	}

	protected void UpdateIconNumber()
	{
		if ((bool)m_Icon && m_IconNumber < m_SpriteList.Count)
		{
			m_Icon.sprite = m_SpriteList[m_IconNumber];
		}
	}

	protected void UpdateIsSelected()
	{
		if (m_IsSelected)
		{
			if (m_SelectedIconOffset > 0 && (bool)m_Icon && m_IconNumber + m_SelectedIconOffset < m_SpriteList.Count)
			{
				m_Icon.sprite = m_SpriteList[m_IconNumber + m_SelectedIconOffset];
			}
		}
		else if ((bool)m_Icon && m_IconNumber < m_SpriteList.Count)
		{
			m_Icon.sprite = m_SpriteList[m_IconNumber];
		}
		foreach (GameObject highlightObj in m_HighlightObjs)
		{
			if ((bool)highlightObj)
			{
				highlightObj.SetActive(m_IsSelected);
			}
		}
		ToggleHighlight(IsSelected);
	}

	protected void UpdateIsInUseMeState()
	{
		for (int i = 0; i < m_UseMeObjs.Count; i++)
		{
			m_UseMeObjs[i].SetActive(m_IsInUseMeState);
		}
	}

	public void ToggleHighlight(bool high)
	{
		if ((bool)m_HighlightImg)
		{
			m_HighlightImg.enabled = high;
		}
		foreach (GameObject highlightObj in m_HighlightObjs)
		{
			if ((bool)highlightObj)
			{
				highlightObj.SetActive(high);
			}
		}
		m_IsHighlighted = high;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (m_VisualsDisabled)
		{
			return;
		}
		if (eventData != null)
		{
			base.OnPointerDown(eventData);
		}
		if (!base.interactable || (m_IsClicked != null && m_IsClicked != this))
		{
			return;
		}
		m_IsClicked = this;
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: true);
			}
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = true;
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_ClickedTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && m_LabelClickedPosMod != Vector3.zero)
			{
				m_Label.transform.position = m_LabelNormalPos + m_LabelClickedPosMod;
			}
			if (m_ClickedFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_ClickedFontSize * component.rect.width / 1600f, (float)m_ClickedFontSize * component.rect.height / 900f));
			}
		}
		CFGAudioManager.Instance.PlaySound2D(m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
		m_IsCursorClickedOverGameObject = true;
	}

	public void SimulateClickGraphicAndSoundOnlyDisable()
	{
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: true);
			}
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = true;
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_ClickedTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && m_LabelClickedPosMod != Vector3.zero)
			{
				m_Label.transform.position = m_LabelNormalPos + m_LabelClickedPosMod;
			}
			if (m_ClickedFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_ClickedFontSize * component.rect.width / 1600f, (float)m_ClickedFontSize * component.rect.height / 900f));
			}
		}
		CFGAudioManager.Instance.PlaySound2D(m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
		Invoke("SimulateClickGraphicAndSoundOnlyDisableEnd", 0.2f);
	}

	public void SimulateClickGraphicAndSoundOnly(bool bPlaySnd = true)
	{
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: true);
			}
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = true;
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_ClickedTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && m_LabelClickedPosMod != Vector3.zero)
			{
				m_Label.transform.position = m_LabelNormalPos + m_LabelClickedPosMod;
			}
			if (m_ClickedFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_ClickedFontSize * component.rect.width / 1600f, (float)m_ClickedFontSize * component.rect.height / 900f));
			}
		}
		if (bPlaySnd)
		{
			CFGAudioManager.Instance.PlaySound2D(m_OnClickSound, CFGAudioManager.Instance.m_MixInterface);
		}
		Invoke("SimulateClickGraphicAndSoundOnlyEnd", 0.2f);
	}

	private void SimulateClickGraphicAndSoundOnlyEnd()
	{
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if ((bool)normalObj)
			{
				normalObj.SetActive(value: true);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_NormalTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && (m_LabelHoverPosMod != Vector3.zero || m_LabelClickedPosMod != Vector3.zero))
			{
				m_Label.transform.position = m_LabelNormalPos;
			}
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
	}

	private void SimulateClickGraphicAndSoundOnlyDisableEnd()
	{
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if ((bool)normalObj)
			{
				normalObj.SetActive(value: false);
			}
		}
		foreach (GameObject disabledObj in m_DisabledObjs)
		{
			if ((bool)disabledObj)
			{
				disabledObj.SetActive(value: true);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_DisabledTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && (m_LabelHoverPosMod != Vector3.zero || m_LabelClickedPosMod != Vector3.zero))
			{
				m_Label.transform.position = m_LabelNormalPos;
			}
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
	}

	public void SimulateClick(bool bDelayed = false, bool bDeselect = false, bool bCall = true)
	{
		if (!bDelayed)
		{
			OnPointerDown(null);
			OnPointerUp(null);
			if (bDeselect)
			{
				m_IsSelected = false;
				OnPointerExit(null);
			}
			return;
		}
		m_Deselect = bDeselect;
		m_Call = bCall;
		if (!m_bWaiting)
		{
			if (bDelayed)
			{
				m_bWaiting = true;
			}
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = base.transform.position;
			OnPointerEnter(null);
			OnPointerDown(pointerEventData);
			CFGJoyManager.ClearJoyActions(0.3f);
			Invoke("SimulateClickEnd", 0.2f);
		}
	}

	private void SimulateClickEnd()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = base.transform.position;
		OnPointerUp(pointerEventData);
		m_Call = true;
		m_bWaiting = false;
		if (m_Deselect)
		{
			pointerEventData.position = new Vector2(-1000f, -1000f);
			OnPointerExit(pointerEventData);
			Canvas.ForceUpdateCanvases();
			Canvas.ForceUpdateCanvases();
			m_IsSelected = false;
			m_IsHighlighted = false;
			m_bIsUnderCursor = false;
			m_IsInUseMeState = false;
			if ((bool)m_HighlightImg)
			{
				m_HighlightImg.enabled = false;
			}
			if ((bool)m_HoverImg)
			{
				m_HoverImg.enabled = false;
			}
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad && eventData != null)
		{
			return;
		}
		m_IsCursorHoveredOverGameObject = true;
		if (m_Tooltip == null && m_TooltipText != string.Empty && CFGInput.LastReadInputDevice != EInputMode.Gamepad && (bool)CFGSingleton<CFGWindowMgr>.Instance)
		{
			m_Tooltip = CFGSingleton<CFGWindowMgr>.Instance.LoadTooltip();
			if (m_Tooltip != null)
			{
				m_Tooltip.TooltipText = m_TooltipText;
			}
		}
		if (m_InactiveButtonOverCallback != null && !m_MyIsUnderCursor)
		{
			m_InactiveButtonOverCallback(m_Data);
			m_MyIsUnderCursor = true;
		}
		if (m_VisualsDisabled && !m_NeedBigTooltup)
		{
			return;
		}
		base.OnPointerEnter(eventData);
		if (!base.interactable || (m_IsClicked != null && m_IsClicked != this))
		{
			return;
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: true);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if ((bool)normalObj)
			{
				normalObj.SetActive(value: false);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = true;
		}
		if ((bool)m_Label)
		{
			if ((bool)m_IsClicked)
			{
				m_Label.color = m_ClickedTextColor;
				TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
				TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
				cachedTextGenerator.Invalidate();
				cachedTextGenerator.Populate(m_Label.text, generationSettings);
				if (m_LabelNormalPos != Vector3.zero && m_LabelClickedPosMod != Vector3.zero)
				{
					m_Label.transform.position = m_LabelNormalPos + m_LabelClickedPosMod;
				}
				if (m_ClickedFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
				{
					RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
					m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_ClickedFontSize * component.rect.width / 1600f, (float)m_ClickedFontSize * component.rect.height / 900f));
				}
			}
			else
			{
				m_Label.color = m_HoverTextColor;
				TextGenerator cachedTextGenerator2 = m_Label.cachedTextGenerator;
				TextGenerationSettings generationSettings2 = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
				cachedTextGenerator2.Invalidate();
				cachedTextGenerator2.Populate(m_Label.text, generationSettings2);
				if (m_LabelNormalPos != Vector3.zero && m_LabelHoverPosMod != Vector3.zero)
				{
					m_Label.transform.position = m_LabelNormalPos + m_LabelHoverPosMod;
				}
				if (m_HoverFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
				{
					RectTransform component2 = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
					m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_HoverFontSize * component2.rect.width / 1600f, (float)m_HoverFontSize * component2.rect.height / 900f));
				}
			}
		}
		if (!m_VisualsDisabled)
		{
			CFGAudioManager.Instance.PlaySound2D(m_OnHoverSound, CFGAudioManager.Instance.m_MixInterface);
		}
		m_bIsUnderCursor = true;
		if (m_ButtonOverCallback != null)
		{
			m_ButtonOverCallback(m_Data);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		m_IsCursorHoveredOverGameObject = false;
		m_IsCursorClickedOverGameObject = false;
		if ((bool)m_Tooltip)
		{
			Object.Destroy(m_Tooltip.gameObject);
		}
		if (m_InactiveButtonOutCallback != null && m_MyIsUnderCursor)
		{
			m_InactiveButtonOutCallback(m_Data);
			m_MyIsUnderCursor = false;
		}
		if (m_VisualsDisabled && !m_NeedBigTooltup)
		{
			return;
		}
		base.OnPointerExit(eventData);
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		foreach (GameObject hoverObj in m_HoverObjs)
		{
			if (hoverObj != null)
			{
				hoverObj.SetActive(value: false);
			}
		}
		foreach (GameObject normalObj in m_NormalObjs)
		{
			if ((bool)normalObj)
			{
				normalObj.SetActive(value: true);
			}
		}
		if ((bool)m_HoverImg)
		{
			m_HoverImg.enabled = false;
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		if ((bool)m_Label)
		{
			m_Label.color = m_NormalTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && (m_LabelHoverPosMod != Vector3.zero || m_LabelClickedPosMod != Vector3.zero))
			{
				m_Label.transform.position = m_LabelNormalPos;
			}
			if (m_NormalFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_NormalFontSize * component.rect.width / 1600f, (float)m_NormalFontSize * component.rect.height / 900f));
			}
		}
		m_bIsUnderCursor = false;
		if (m_ButtonOutCallback != null)
		{
			m_ButtonOutCallback(m_Data);
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickCount == 2 && eventData.button == PointerEventData.InputButton.Left && m_OnDoubleClickCallback != null)
		{
			m_OnDoubleClickCallback(m_Data);
		}
		if (eventData.button == PointerEventData.InputButton.Right && m_OnRightClickCallback != null)
		{
			m_OnRightClickCallback(m_Data);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (m_VisualsDisabled)
		{
			return;
		}
		if (eventData != null)
		{
			base.OnPointerUp(eventData);
		}
		if (!base.interactable)
		{
			return;
		}
		foreach (GameObject clickedObj in m_ClickedObjs)
		{
			if (clickedObj != null)
			{
				clickedObj.SetActive(value: false);
			}
		}
		if (!m_Draggable || (m_Draggable && eventData != null && !eventData.dragging))
		{
			foreach (GameObject hoverObj in m_HoverObjs)
			{
				if (hoverObj != null)
				{
					hoverObj.SetActive(value: true);
				}
			}
		}
		if ((bool)m_ClickedImg)
		{
			m_ClickedImg.enabled = false;
		}
		bool flag = false;
		if (eventData != null)
		{
			flag = eventData.dragging;
		}
		if ((bool)m_HoverImg && !flag)
		{
			m_HoverImg.enabled = true;
		}
		if ((bool)m_Label && !flag)
		{
			m_Label.color = m_HoverTextColor;
			TextGenerator cachedTextGenerator = m_Label.cachedTextGenerator;
			TextGenerationSettings generationSettings = m_Label.GetGenerationSettings(new Vector2(500f, 50f));
			cachedTextGenerator.Invalidate();
			cachedTextGenerator.Populate(m_Label.text, generationSettings);
			if (m_LabelNormalPos != Vector3.zero && (m_LabelHoverPosMod != Vector3.zero || m_LabelClickedPosMod != Vector3.zero))
			{
				m_Label.transform.position = m_LabelNormalPos + m_LabelHoverPosMod;
			}
			if (m_HoverFontSize != 0 && (bool)CFGSingleton<CFGWindowMgr>.Instance && (bool)CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas)
			{
				RectTransform component = CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.GetComponent<RectTransform>();
				m_Label.fontSize = Mathf.FloorToInt(Mathf.Min((float)m_HoverFontSize * component.rect.width / 1600f, (float)m_HoverFontSize * component.rect.height / 900f));
			}
		}
		if (eventData != null)
		{
			OnDeselect(eventData);
		}
		m_IsClicked = null;
		if (m_Call && m_ButtonClickedCallback != null && (!flag || m_ShouldDragSelectItem))
		{
			m_ButtonClickedCallback(m_Data);
		}
		m_IsCursorClickedOverGameObject = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!base.interactable || !m_Draggable)
		{
			return;
		}
		if (m_DraggingObj == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.gameObject.transform.parent;
			if ((bool)m_DragParent)
			{
				gameObject.transform.parent = m_DragParent;
			}
			gameObject.AddComponent<RectTransform>();
			gameObject.AddComponent<CanvasRenderer>();
			Image icon = gameObject.AddComponent<Image>();
			CFGButtonExtension cFGButtonExtension = gameObject.AddComponent<CFGButtonExtension>();
			cFGButtonExtension.m_ButtonDropCallback = m_ButtonRealeseOutsideCallback;
			cFGButtonExtension.m_SpriteList = m_SpriteList;
			cFGButtonExtension.m_Icon = icon;
			cFGButtonExtension.m_Icon.preserveAspect = true;
			cFGButtonExtension.IconNumber = IconNumber;
			gameObject.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0f);
			m_DraggingObj = gameObject;
			CFGAudioManager.Instance.PlaySound2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_UIDrag, CFGAudioManager.Instance.m_MixInterface);
		}
		else
		{
			m_DraggingObj.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0f);
		}
		if (m_ShouldDragSelectItem)
		{
			OnPointerUp(eventData);
		}
		if (m_ButtonDragCallback != null)
		{
			m_ButtonDragCallback(m_Data);
		}
		m_bWaiting = false;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (!base.interactable || !(eventData.pointerDrag != null))
		{
			return;
		}
		CFGButtonExtension component = eventData.pointerDrag.GetComponent<CFGButtonExtension>();
		if ((bool)component && (bool)component.m_DraggingObj)
		{
			if (base.gameObject == component.m_DraggingObj)
			{
				List<RaycastResult> list = new List<RaycastResult>();
				List<CFGButtonExtension> list2 = new List<CFGButtonExtension>();
				if ((bool)CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem)
				{
					CFGSingleton<CFGWindowMgr>.Instance.m_EventSystem.RaycastAll(eventData, list);
				}
				foreach (RaycastResult item in list)
				{
					if ((bool)item.gameObject && item.gameObject != component.m_DraggingObj && (bool)item.gameObject.GetComponent<CFGButtonExtension>())
					{
						list2.Add(item.gameObject.GetComponent<CFGButtonExtension>());
					}
				}
				foreach (CFGButtonExtension item2 in list2)
				{
					item2.OnDrop(eventData);
				}
				CFGAudioManager.Instance.PlaySound2D(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_UIDrop, CFGAudioManager.Instance.m_MixInterface);
			}
			if (component.m_DraggingObj.GetComponent<CFGButtonExtension>().m_ButtonDropCallback != null)
			{
				component.m_DraggingObj.GetComponent<CFGButtonExtension>().m_ButtonDropCallback(m_Data);
			}
			Object.Destroy(component.m_DraggingObj);
		}
		if (m_ButtonDropCallback != null)
		{
			m_ButtonDropCallback(m_Data);
		}
	}
}
