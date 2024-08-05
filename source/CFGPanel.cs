using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CFGPanel : UIBehaviour
{
	protected enum EFadeState
	{
		Unknown,
		Hidden,
		FadeIn,
		Visible,
		FadeOut
	}

	public delegate void OnPanelUnloadedDelegate();

	private const float FADING_SPEED = 4f;

	[SerializeField]
	private AudioClip m_OnOpenSound;

	[SerializeField]
	private AudioClip m_OnCloseSound;

	protected CanvasGroup m_CanvasGroup;

	protected float m_FadeAlpha;

	private bool m_PanelShouldBeDestroyed;

	private OnPanelUnloadedDelegate m_OnPanelUnloadedCallback;

	private EFadeState m_FadeState;

	public void SetPanelVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public void Unload(OnPanelUnloadedDelegate callback = null)
	{
		if (m_OnCloseSound != null)
		{
			CFGAudioManager.Instance.PlaySound2D(m_OnCloseSound, CFGAudioManager.Instance.m_MixInterface);
		}
		FadeOut();
		CanvasGroup component = GetComponent<CanvasGroup>();
		if (component != null)
		{
			component.interactable = false;
		}
		m_PanelShouldBeDestroyed = true;
		m_OnPanelUnloadedCallback = callback;
	}

	public virtual void SetLocalisation()
	{
	}

	protected override void Awake()
	{
		Canvas component = GetComponent<Canvas>();
		if ((bool)component)
		{
			component.overrideSorting = true;
		}
		m_CanvasGroup = GetComponent<CanvasGroup>();
		if (m_CanvasGroup != null)
		{
			if (m_CanvasGroup.alpha == 1f)
			{
				m_FadeState = EFadeState.Visible;
				m_FadeAlpha = 1f;
			}
			else if (m_CanvasGroup.alpha == 0f)
			{
				m_FadeState = EFadeState.Hidden;
				m_FadeAlpha = 0f;
			}
		}
	}

	protected override void Start()
	{
		SetLocalisation();
		FadeIn();
		if (m_OnOpenSound != null)
		{
			CFGAudioManager.Instance.PlaySound2D(m_OnOpenSound, CFGAudioManager.Instance.m_MixInterface);
		}
		Canvas component = GetComponent<Canvas>();
		if ((bool)component)
		{
			component.overrideSorting = true;
		}
		Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			int num = 1;
			num = ((!text.resizeTextForBestFit) ? text.fontSize : text.resizeTextMaxSize);
			if (Screen.height > 900 || Screen.width > 1600)
			{
				text.resizeTextForBestFit = false;
			}
			if (text as CFGTextExtension == null)
			{
				text.fontSize = Mathf.FloorToInt(Mathf.Min(num * Screen.width / 1600, num * Screen.height / 900));
			}
		}
	}

	public virtual void Update()
	{
		UpdateFading();
		if (m_PanelShouldBeDestroyed && m_FadeState == EFadeState.Hidden)
		{
			Object.Destroy(base.gameObject);
			if (m_OnPanelUnloadedCallback != null)
			{
				m_OnPanelUnloadedCallback();
			}
		}
	}

	protected void FadeIn()
	{
		if (m_FadeState == EFadeState.Unknown)
		{
			Debug.LogError("ERROR! CFGPanel.FadeIn() - m_FadeState == EFadeState.Unknown, " + base.gameObject.name);
		}
		else if (m_FadeState == EFadeState.Hidden || m_FadeState == EFadeState.FadeOut)
		{
			m_FadeState = EFadeState.FadeIn;
		}
	}

	protected void FadeOut()
	{
		if (m_FadeState == EFadeState.Unknown)
		{
			Debug.LogError("ERROR! CFGPanel.FadeOut() - m_FadeState == EFadeState.Unknown, " + base.gameObject.name);
		}
		else if (m_FadeState == EFadeState.Visible || m_FadeState == EFadeState.FadeIn)
		{
			m_FadeState = EFadeState.FadeOut;
		}
	}

	protected EFadeState GetFadeState()
	{
		return m_FadeState;
	}

	protected bool IsPanelWaitingForDestroy()
	{
		return m_PanelShouldBeDestroyed;
	}

	protected void CheckField(MonoBehaviour field, string field_name)
	{
		if (field == null)
		{
			Debug.LogWarning("<color=purple>Obiekt " + base.name + " ma puste pole " + field_name + "</color>");
		}
	}

	protected void CheckField(GameObject field, string field_name)
	{
		if (field == null)
		{
			Debug.LogWarning("<color=purple>Obiekt " + base.name + " ma puste pole " + field_name + "</color>");
		}
	}

	private void UpdateFading()
	{
		if (m_FadeState == EFadeState.FadeIn)
		{
			m_FadeAlpha += Time.deltaTime * 4f;
			if (m_FadeAlpha >= 1f)
			{
				m_FadeState = EFadeState.Visible;
				m_FadeAlpha = 1f;
			}
			UpdatePanelAlpha();
		}
		else if (m_FadeState == EFadeState.FadeOut)
		{
			m_FadeAlpha -= Time.deltaTime * 4f;
			if (m_FadeAlpha <= 0f)
			{
				m_FadeState = EFadeState.Hidden;
				m_FadeAlpha = 0f;
			}
			UpdatePanelAlpha();
		}
	}

	private void UpdatePanelAlpha()
	{
		if (m_CanvasGroup != null)
		{
			m_CanvasGroup.alpha = m_FadeAlpha;
		}
	}
}
