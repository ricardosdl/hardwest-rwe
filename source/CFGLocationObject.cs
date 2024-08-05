using System.Collections.Generic;
using CatlikeCoding.TextBox;
using UnityEngine;

public class CFGLocationObject : CFGGameObject
{
	private const float m_RevealingTime = 2.95f;

	[SerializeField]
	private ELocationState m_State;

	[Space(20f)]
	[SerializeField]
	private bool m_UseTextRotation;

	[SerializeField]
	private float m_TextRotation;

	private Texture2D m_TextureHover;

	private Texture2D m_TextureNormal;

	private Color m_TextureColorHover;

	private Color m_TextureColorNormal;

	private Color m_TextHoverColor;

	private Color m_TextNormalColor;

	private MeshRenderer m_Model;

	private CFGLocationFog m_Fog;

	private Renderer m_Background;

	private GameObject m_LocationText;

	private Vector3 m_LocationTextRotation = new Vector3(0f, 310f, 0f);

	private TextBox m_TextBox;

	private GameObject m_NewMarker;

	private bool m_ShouldNewMarkerBeDisplayed;

	private CFGCellObject m_CellObject;

	private Vector3 m_RevealStartPos;

	private Vector3 m_RevealEndPos;

	private float m_RevealingDelta;

	private bool m_IsRevealing;

	[HideInInspector]
	public static List<string> m_LocationsForPopup = new List<string>();

	private float m_LastLocationPopupTime = -1f;

	public ELocationState State => m_State;

	public override ESerializableType SerializableType => ESerializableType.Location;

	public override bool NeedsSaving => true;

	public void SetState(ELocationState state)
	{
		if (m_State != state)
		{
			OnStateChanged(m_State, state);
			m_State = state;
			ApplyState();
			UpdateHoverState();
		}
	}

	public override void OnCursorEnter()
	{
		UpdateHoverState();
	}

	public override void OnCursorLeave()
	{
		UpdateHoverState();
	}

	public void OnCharacterEnter()
	{
		DestroyNewMarker();
	}

	protected override void Start()
	{
		base.Start();
		CFGGameplaySettings instance = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance;
		m_TextureHover = instance.m_StrategicLocation.m_TextureHover;
		m_TextureNormal = instance.m_StrategicLocation.m_TextureNormal;
		m_TextureColorHover = instance.m_StrategicLocation.m_TextureColorHover;
		m_TextureColorNormal = instance.m_StrategicLocation.m_TextureColorNormal;
		m_TextHoverColor = instance.m_StrategicLocation.m_TextColorHover;
		m_TextNormalColor = instance.m_StrategicLocation.m_TextColorNormal;
		Transform transform = base.transform.Find("background");
		if ((bool)transform)
		{
			m_Background = transform.gameObject.GetComponent<Renderer>();
		}
		Transform transform2 = base.transform.Find("location_text");
		if ((bool)transform2)
		{
			m_LocationText = transform2.gameObject;
		}
		foreach (Transform item in base.transform)
		{
			if (item.name.StartsWith("str_"))
			{
				m_Model = item.GetComponent<MeshRenderer>();
				if (m_Model != null)
				{
					break;
				}
			}
		}
		if ((bool)m_LocationText)
		{
			Quaternion rotation = m_LocationText.transform.rotation;
			rotation.eulerAngles += m_LocationTextRotation;
			m_LocationText.transform.rotation = rotation;
			m_TextBox = m_LocationText.GetComponent<TextBox>();
			m_TextBox.SetText(CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(base.NameId).ToUpper());
			m_TextBox.RenderText();
			if (m_UseTextRotation)
			{
				Vector3 localEulerAngles = m_LocationText.transform.localEulerAngles;
				localEulerAngles.y = m_TextRotation;
				m_LocationText.transform.localEulerAngles = localEulerAngles;
			}
		}
		m_CellObject = GetComponent<CFGCellObject>();
		if (m_CellObject == null && base.transform.parent != null)
		{
			m_CellObject = base.transform.parent.GetComponent<CFGCellObject>();
		}
		ApplyState();
		InitState();
	}

	private void ApplyState()
	{
		if (m_CellObject != null)
		{
			switch (m_State)
			{
			case ELocationState.HIDDEN:
				CFGCellMap.ApplySingleObject(m_CellObject);
				break;
			case ELocationState.LOCKED:
				CFGCellMap.ApplySingleObject(m_CellObject, bAlternate: true);
				break;
			case ELocationState.OPEN:
				CFGCellMap.ApplySingleObject(m_CellObject);
				break;
			}
		}
		else
		{
			Debug.LogWarning("Strategic location: [" + base.name + "]: failed to find cellobject!");
		}
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsRevealing && m_Model != null)
		{
			m_RevealingDelta += Time.deltaTime;
			if (m_RevealingDelta >= 2.95f)
			{
				m_RevealingDelta = 2.95f;
				m_IsRevealing = false;
				if (m_Background != null)
				{
					m_Background.enabled = m_State == ELocationState.OPEN;
				}
				if (m_LocationText != null)
				{
					m_LocationText.SetActive(m_State == ELocationState.OPEN);
				}
				if (m_State == ELocationState.OPEN)
				{
					SpawnNewMarker();
				}
				else
				{
					DestroyNewMarker();
				}
			}
			m_Model.transform.position = CFGMath.EaseInOutQuad(m_RevealingDelta, m_RevealStartPos, m_RevealEndPos - m_RevealStartPos, 2.95f);
		}
		if (!(m_LastLocationPopupTime + 2f < Time.time))
		{
			return;
		}
		if (CFGSingleton<CFGGame>.Instance.IsInStrategic() && CFGSingleton<CFGWindowMgr>.Instance != null && m_LocationsForPopup.Count > 0)
		{
			string text = CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("popup_location_opened") + m_LocationsForPopup[0];
			for (int i = 1; i < m_LocationsForPopup.Count; i++)
			{
				text = text + ", " + m_LocationsForPopup[i];
			}
			CFGSingleton<CFGWindowMgr>.Instance.LoadStrategicEventPopup(text, 4, 0);
		}
		m_LocationsForPopup.Clear();
		m_LastLocationPopupTime = Time.time;
	}

	private void InitState()
	{
		switch (m_State)
		{
		case ELocationState.OPEN:
			if (m_Model != null)
			{
				m_Model.gameObject.SetActive(value: true);
			}
			if (m_Background != null)
			{
				m_Background.enabled = true;
			}
			if (m_LocationText != null)
			{
				m_LocationText.SetActive(value: true);
			}
			if (m_Fog != null)
			{
				m_Fog.enabled = true;
			}
			SpawnNewMarker();
			UpdateHoverState();
			break;
		case ELocationState.LOCKED:
			if (m_Model != null)
			{
				m_Model.gameObject.SetActive(value: true);
			}
			if (m_Background != null)
			{
				m_Background.enabled = false;
			}
			if (m_LocationText != null)
			{
				m_LocationText.SetActive(value: false);
			}
			if (m_Fog != null)
			{
				m_Fog.enabled = true;
			}
			break;
		case ELocationState.HIDDEN:
			if (m_Model != null)
			{
				m_Model.gameObject.SetActive(value: false);
			}
			if (m_Background != null)
			{
				m_Background.enabled = false;
			}
			if (m_LocationText != null)
			{
				m_LocationText.SetActive(value: false);
			}
			if (m_Fog == null)
			{
				CFGLocationFog locationFogPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_LocationFogPrefab;
				if (locationFogPrefab != null)
				{
					m_Fog = Object.Instantiate(locationFogPrefab, base.transform.position, Quaternion.identity) as CFGLocationFog;
					m_Fog.transform.parent = base.transform;
				}
			}
			break;
		}
	}

	private void OnStateChanged(ELocationState old_state, ELocationState new_state)
	{
		if (old_state == ELocationState.HIDDEN)
		{
			if (m_Model != null)
			{
				m_Model.gameObject.SetActive(value: true);
				m_RevealStartPos = (m_RevealEndPos = m_Model.transform.position);
				m_RevealStartPos.y -= 1.1f;
				m_Model.transform.position = m_RevealStartPos;
				m_RevealingDelta = 0f;
				m_IsRevealing = true;
			}
			if (m_Fog != null)
			{
				m_Fog.enabled = true;
			}
			GameObject locationRevealFxPrefab = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_LocationRevealFxPrefab;
			if (locationRevealFxPrefab != null)
			{
				GameObject gameObject = Object.Instantiate(locationRevealFxPrefab, base.transform.position, Quaternion.identity) as GameObject;
				gameObject.transform.parent = base.transform;
			}
			CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_LocationReveal, base.transform);
		}
		else
		{
			if (m_Background != null)
			{
				m_Background.enabled = new_state == ELocationState.OPEN;
			}
			if (m_LocationText != null)
			{
				m_LocationText.SetActive(new_state == ELocationState.OPEN);
			}
			if (new_state == ELocationState.OPEN)
			{
				SpawnNewMarker();
			}
			else
			{
				DestroyNewMarker();
			}
		}
	}

	private void UpdateHoverState()
	{
		if (m_State != 0)
		{
			return;
		}
		if (base.IsUnderCursor && !CFGSingleton<CFGWindowMgr>.Instance.m_StrategicExplorator.IsExplorationWindowVisible())
		{
			if (m_Background != null && m_Background.material != null)
			{
				m_Background.material.SetColor("_TintColor", m_TextureColorHover);
				m_Background.material.SetTexture("_MainTex", m_TextureHover);
			}
			if (m_TextBox != null)
			{
				m_TextBox.tint = m_TextHoverColor;
			}
		}
		else
		{
			if (m_Background != null && m_Background.material != null)
			{
				m_Background.material.SetColor("_TintColor", m_TextureColorNormal);
				m_Background.material.SetTexture("_MainTex", m_TextureNormal);
			}
			if (m_TextBox != null)
			{
				m_TextBox.tint = m_TextNormalColor;
			}
		}
		m_TextBox.RenderText();
	}

	private void SpawnNewMarker()
	{
		m_ShouldNewMarkerBeDisplayed = true;
		if (!(m_NewMarker != null))
		{
			GameObject locationNewMarker = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_LocationNewMarker;
			if (locationNewMarker != null)
			{
				m_NewMarker = Object.Instantiate(locationNewMarker, base.transform.position, Quaternion.identity) as GameObject;
				m_NewMarker.transform.parent = base.transform;
			}
		}
	}

	private void DestroyNewMarker()
	{
		m_ShouldNewMarkerBeDisplayed = false;
		if (!(m_NewMarker == null))
		{
			ParticleSystem[] componentsInChildren = m_NewMarker.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].loop = false;
			}
			m_NewMarker = null;
		}
	}

	private void DestroyNewMarkerImmediately()
	{
		m_ShouldNewMarkerBeDisplayed = false;
		if (!(m_NewMarker == null))
		{
			Object.Destroy(m_NewMarker);
			m_NewMarker = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 center = ((!(base.transform.parent != null)) ? base.transform.position : base.transform.parent.position);
		center.y += 0.5f;
		center.x += 0.5f;
		center.z += 0.5f;
		Gizmos.DrawCube(center, base.transform.transform.lossyScale);
	}

	public override bool OnSerialize(CFG_SG_Node Parent)
	{
		CFG_SG_Node cFG_SG_Node = OnBeginSerialization(Parent);
		if (cFG_SG_Node == null)
		{
			return false;
		}
		cFG_SG_Node.Attrib_Set("State", m_State);
		cFG_SG_Node.Attrib_Set("ShowNewMarker", m_ShouldNewMarkerBeDisplayed);
		return true;
	}

	public override bool OnDeserialize(CFG_SG_Node LocNode)
	{
		m_State = LocNode.Attrib_Get("State", ELocationState.OPEN);
		m_ShouldNewMarkerBeDisplayed = LocNode.Attrib_Get("ShowNewMarker", DefVal: false);
		bool shouldNewMarkerBeDisplayed = m_ShouldNewMarkerBeDisplayed;
		if (m_CellObject != null)
		{
			CFGCellMap.ApplySingleObject(m_CellObject, m_State != ELocationState.OPEN);
		}
		InitState();
		if (!shouldNewMarkerBeDisplayed)
		{
			DestroyNewMarkerImmediately();
		}
		return true;
	}
}
