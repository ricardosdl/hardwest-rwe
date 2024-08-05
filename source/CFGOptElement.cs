using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CFGOptElement : Selectable
{
	public delegate void OnSliderChangedDelegate(float val, int data);

	public delegate void OnPointerDownSliderDelegate(int data);

	public delegate void OnSelectDropdownDelegate(int val, int data);

	public delegate void OnToggleCheckBoxDelegate(bool val, int data);

	public Color m_NormalTextColor = default(Color);

	public Color m_DisabledTextColor = default(Color);

	public AudioClip m_OnHoverSound;

	public Text m_Text;

	public CFGButtonExtension m_CheckBox;

	public GameObject m_DisabledCheckBoxImage;

	public Slider m_Slider;

	public Text m_SliderText;

	public Image m_SliderDisabledImg;

	public Text m_Controls;

	public Text m_ControlsAlt;

	public CFGButtonExtension m_Bind;

	public CFGButtonExtension m_AltBind;

	public Text m_DropDownText;

	public CFGButtonExtension m_DropdownScrollButton;

	public GameObject m_DropdownSpawnListPosition;

	public GameObject m_EndFrame;

	public GameObject m_Highligth;

	public GameObject m_HighligthParent;

	public int m_Data;

	public GameObject m_ApplyFrame;

	public GameObject m_GrayTitleFrame;

	public CFGButtonExtension m_ApllyVideo;

	public Text m_TitleGray;

	public List<string> m_DropDownElements = new List<string>();

	public int m_CurrentSelectedDropdownElement;

	public Dictionary<int, Color> m_DropDownElementColors = new Dictionary<int, Color>();

	public CFGButtonExtension m_DropdownRight;

	public CFGButtonExtension m_DropdownLeft;

	public Text m_DropdownLR;

	public OnSliderChangedDelegate m_SliderChangedCallback;

	public OnPointerDownSliderDelegate m_SliderDownCallback;

	public OnSelectDropdownDelegate m_SelectDropdownCallback;

	public OnToggleCheckBoxDelegate m_ToggleCheckBoxCallback;

	public GameObject m_DropdownListPrefab;

	public GameObject m_BtnChoosePrefab;

	private GameObject m_DropdownList;

	private List<GameObject> m_OptBtns = new List<GameObject>();

	private int m_Type;

	private bool m_IsSelected;

	public bool CheckBox
	{
		get
		{
			return m_CheckBox.IsSelected;
		}
		set
		{
			m_CheckBox.IsSelected = value;
			m_DisabledCheckBoxImage.SetActive(!m_CheckBox.enabled && m_CheckBox.IsSelected);
		}
	}

	public float Slider
	{
		get
		{
			return m_Slider.value;
		}
		set
		{
			m_Slider.value = value;
		}
	}

	public float SliderMin
	{
		get
		{
			return m_Slider.minValue;
		}
		set
		{
			m_Slider.minValue = value;
		}
	}

	public float SliderMax
	{
		get
		{
			return m_Slider.maxValue;
		}
		set
		{
			m_Slider.maxValue = value;
		}
	}

	public string SliderText
	{
		get
		{
			return m_SliderText.text;
		}
		set
		{
			m_SliderText.text = value;
		}
	}

	public string Title
	{
		get
		{
			return m_Text.text;
		}
		set
		{
			m_Text.text = value;
		}
	}

	public string TitleGray
	{
		get
		{
			return m_TitleGray.text;
		}
		set
		{
			m_TitleGray.text = value;
		}
	}

	public string Controls
	{
		get
		{
			return m_Controls.text;
		}
		set
		{
			m_Controls.text = value;
		}
	}

	public string ControlsAlt
	{
		get
		{
			return m_ControlsAlt.text;
		}
		set
		{
			m_ControlsAlt.text = value;
		}
	}

	public bool ControlsSelected
	{
		get
		{
			return m_Bind.IsSelected;
		}
		set
		{
			m_Bind.IsSelected = value;
		}
	}

	public bool ControlsAltSelected
	{
		get
		{
			return m_AltBind.IsSelected;
		}
		set
		{
			m_AltBind.IsSelected = value;
		}
	}

	public CFGButtonExtension.OnButtonClickedDelegate BindClicked
	{
		set
		{
			m_Bind.m_ButtonClickedCallback = value;
		}
	}

	public CFGButtonExtension.OnButtonClickedDelegate AltBindClicked
	{
		set
		{
			m_AltBind.m_ButtonClickedCallback = value;
		}
	}

	public CFGButtonExtension.OnButtonClickedDelegate ApplyVideoBtn
	{
		set
		{
			m_ApllyVideo.m_ButtonClickedCallback = value;
		}
	}

	public bool IsSelected => m_IsSelected;

	public int GetControlType()
	{
		return m_Type;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (base.interactable)
		{
			m_Highligth.SetActive(value: true);
			CFGAudioManager.Instance.PlaySound2D(m_OnHoverSound, CFGAudioManager.Instance.m_MixInterface);
			m_IsSelected = true;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (base.interactable)
		{
			m_Highligth.SetActive(value: false);
			m_IsSelected = false;
		}
	}

	public void OnSliderDown(BaseEventData eventData)
	{
		if (m_SliderDownCallback != null)
		{
			m_SliderDownCallback(m_Data);
		}
	}

	public void SetType(int type)
	{
		m_Type = type;
		if (type == 4 || type == 7)
		{
			base.interactable = false;
		}
		m_CheckBox.m_Data = m_Data;
		m_Bind.m_Data = m_Data;
		m_AltBind.m_Data = m_Data;
		m_Text.gameObject.SetActive(type != 4 && type != 5 && type != 7);
		m_GrayTitleFrame.SetActive(type == 4);
		m_ApplyFrame.SetActive(type == 5);
		m_Slider.gameObject.SetActive(type == 1);
		m_SliderText.gameObject.SetActive(type == 1);
		m_SliderDisabledImg.gameObject.SetActive(value: false);
		if (type == 1)
		{
			m_Slider.onValueChanged.AddListener(OnSliderValueChange);
			EventTrigger eventTrigger = m_Slider.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = m_Slider.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback = new EventTrigger.TriggerEvent();
			UnityAction<BaseEventData> call = OnSliderDown;
			entry.callback.AddListener(call);
			eventTrigger.triggers.Add(entry);
		}
		else
		{
			m_Slider.onValueChanged.RemoveAllListeners();
			EventTrigger component = m_Slider.GetComponent<EventTrigger>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		m_Controls.gameObject.SetActive(type == 2);
		m_ControlsAlt.gameObject.SetActive(type == 2);
		m_Bind.gameObject.SetActive(type == 2);
		m_AltBind.gameObject.SetActive(type == 2);
		m_CheckBox.transform.parent.gameObject.SetActive(type == 0);
		m_DropdownRight.transform.parent.gameObject.SetActive(type == 6);
		m_DropDownText.transform.parent.gameObject.SetActive(type == 3);
		if (m_DropDownElements.Count > m_CurrentSelectedDropdownElement)
		{
			m_DropDownText.text = m_DropDownElements[m_CurrentSelectedDropdownElement];
			m_DropdownLR.text = m_DropDownElements[m_CurrentSelectedDropdownElement];
		}
		m_DropdownScrollButton.m_ButtonClickedCallback = OnDropdownScrollButtonClick;
		m_DropdownRight.m_ButtonClickedCallback = OnChooseOptionRight;
		m_DropdownLeft.m_ButtonClickedCallback = OnChooseOptionLeft;
		m_CheckBox.m_ButtonClickedCallback = OnCheckBoxVisuals;
		m_EndFrame.SetActive(type == 7);
	}

	public void ToggleEnabled(bool enabled)
	{
		m_Slider.enabled = enabled;
		if (m_Type == 1 && enabled)
		{
			m_Slider.onValueChanged.AddListener(OnSliderValueChange);
		}
		else
		{
			m_Slider.onValueChanged.RemoveAllListeners();
		}
		m_Bind.enabled = enabled;
		m_AltBind.enabled = enabled;
		m_CheckBox.enabled = enabled;
		m_DropdownScrollButton.enabled = enabled;
		m_DropdownRight.enabled = enabled;
		m_DropdownLeft.enabled = enabled;
		m_Text.color = ((!enabled) ? m_DisabledTextColor : m_NormalTextColor);
		m_DisabledCheckBoxImage.SetActive(!enabled && m_CheckBox.IsSelected);
		m_ApllyVideo.enabled = enabled;
	}

	protected void OnSliderValueChange(float val)
	{
		if (m_SliderChangedCallback != null)
		{
			m_SliderChangedCallback(val, m_Data);
		}
	}

	protected void OnCheckBoxVisuals(int a)
	{
		m_CheckBox.IsSelected = !m_CheckBox.IsSelected;
		if (m_ToggleCheckBoxCallback != null)
		{
			m_ToggleCheckBoxCallback(m_CheckBox.IsSelected, m_Data);
		}
	}

	public void OnDropdownScrollButtonClick(int a)
	{
		GameObject gameObject = Object.Instantiate(m_DropdownListPrefab);
		gameObject.name = "DropdownList";
		m_DropdownList = gameObject;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.SetParent(CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.transform, worldPositionStays: false);
		CFGButtonExtension componentInChildren = gameObject.GetComponentInChildren<CFGButtonExtension>();
		componentInChildren.m_ButtonClickedCallback = OnCloseDropdown;
		CFGImageExtension componentInChildren2 = gameObject.GetComponentInChildren<CFGImageExtension>();
		RectTransform component2 = componentInChildren2.gameObject.GetComponent<RectTransform>();
		float num = Mathf.Min((float)Screen.width / 1600f, (float)Screen.height / 900f);
		float num2 = 0.16f * num;
		if (Screen.width < 1600)
		{
			num2 = 0.28f * num;
		}
		else if (Screen.width > 1920)
		{
			num2 = 0.13f * num;
		}
		component2.anchorMin = new Vector2(0f, 0f);
		component2.anchorMax = new Vector2(1f, num2 * (float)m_DropDownElements.Count);
		component2.offsetMax = new Vector2(1f, 1f);
		component2.offsetMin = new Vector2(0f, 0f);
		float num3 = (float)Screen.width / 1600f;
		RectTransform component3 = gameObject.GetComponent<RectTransform>();
		if ((bool)component3)
		{
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num3 * component3.rect.width);
			component3.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num3 * component3.rect.height);
			Debug.Log(num3 * component3.rect.width);
		}
		componentInChildren2.transform.position = m_DropdownSpawnListPosition.transform.position;
		for (int i = 0; i < m_DropDownElements.Count; i++)
		{
			GameObject gameObject2 = Object.Instantiate(m_BtnChoosePrefab);
			gameObject2.name = "DropdownList";
			Text componentInChildren3 = gameObject2.GetComponentInChildren<Text>();
			componentInChildren3.text = m_DropDownElements[i];
			if (m_DropDownElementColors.TryGetValue(i, out var value))
			{
				componentInChildren3.color = value;
			}
			CFGButtonExtension componentInChildren4 = gameObject2.GetComponentInChildren<CFGButtonExtension>();
			componentInChildren4.m_ButtonClickedCallback = OnChooseOption;
			componentInChildren4.m_Data = i;
			componentInChildren4.IsSelected = i == m_CurrentSelectedDropdownElement;
			RectTransform component4 = gameObject2.GetComponent<RectTransform>();
			component4.SetParent(CFGSingleton<CFGWindowMgr>.Instance.m_UICanvas.transform, worldPositionStays: false);
			gameObject2.transform.position = new Vector3(componentInChildren2.transform.position.x, componentInChildren2.transform.position.y - (float)i * 23f * num - 12f * num, componentInChildren2.transform.position.z);
			m_OptBtns.Add(gameObject2);
		}
	}

	public void OnCloseDropdown(int a)
	{
		if (m_DropdownList != null)
		{
			Object.Destroy(m_DropdownList);
		}
		foreach (GameObject optBtn in m_OptBtns)
		{
			Object.Destroy(optBtn);
		}
		m_OptBtns.Clear();
	}

	public void OnChooseOption(int option)
	{
		m_CurrentSelectedDropdownElement = option;
		if (m_DropDownElements.Count > m_CurrentSelectedDropdownElement)
		{
			m_DropDownText.text = m_DropDownElements[m_CurrentSelectedDropdownElement];
		}
		if (m_SelectDropdownCallback != null)
		{
			m_SelectDropdownCallback(option, m_Data);
		}
		OnCloseDropdown(0);
	}

	public void OnChooseOptionLeft(int option)
	{
		if (m_CurrentSelectedDropdownElement != 0)
		{
			m_CurrentSelectedDropdownElement--;
			if (m_DropDownElements.Count > m_CurrentSelectedDropdownElement)
			{
				m_DropdownLR.text = m_DropDownElements[m_CurrentSelectedDropdownElement];
			}
			if (m_SelectDropdownCallback != null)
			{
				m_SelectDropdownCallback(m_CurrentSelectedDropdownElement, m_Data);
			}
		}
	}

	public void OnChooseOptionRight(int option)
	{
		if (m_CurrentSelectedDropdownElement != m_DropDownElements.Count - 1)
		{
			m_CurrentSelectedDropdownElement++;
			if (m_DropDownElements.Count > m_CurrentSelectedDropdownElement)
			{
				m_DropdownLR.text = m_DropDownElements[m_CurrentSelectedDropdownElement];
			}
			if (m_SelectDropdownCallback != null)
			{
				m_SelectDropdownCallback(m_CurrentSelectedDropdownElement, m_Data);
			}
		}
	}
}
