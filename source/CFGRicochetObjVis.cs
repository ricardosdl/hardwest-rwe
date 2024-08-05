using System.Collections.Generic;
using UnityEngine;

public class CFGRicochetObjVis : MonoBehaviour
{
	private enum EVisState
	{
		Normal,
		InRange,
		Hover,
		Selected
	}

	private Animator m_Billboard;

	private CFGRicochetObject m_Object;

	private HashSet<Material> m_Materials = new HashSet<Material>();

	private bool m_IsVisible = true;

	private EVisState m_VisState;

	private void Start()
	{
		m_Billboard = Object.Instantiate(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_RicochetHelpers.m_Billboard.gameObject).GetComponent<Animator>();
		if ((bool)m_Billboard)
		{
			m_Billboard.transform.SetParent(base.transform);
			m_Billboard.transform.localPosition = new Vector3(0f, 1.11f, 0f);
			m_Billboard.gameObject.SetActive(value: false);
		}
		m_Object = GetComponent<CFGRicochetObject>();
		if (m_Object == null)
		{
			Debug.LogWarning("WARNING! CFGRicochetObjVis script attached to object with no CFGRicochetObject script!", base.gameObject);
			return;
		}
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
			{
				Material material = componentsInChildren[i].materials[j];
				if (material != null && material.shader.name == "CFG/Custom/Ricochet")
				{
					m_Materials.Add(material);
				}
			}
		}
	}

	private void Update()
	{
		if (m_Object == null)
		{
			return;
		}
		if (m_Object.Cell == null)
		{
			m_Object.UpdateCell();
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		bool flag = component != null && m_Object.Cell != null && (int)component.CurrentFloorLevel >= m_Object.Cell.Floor;
		if (m_IsVisible != flag)
		{
			m_IsVisible = flag;
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = m_IsVisible;
			}
		}
		if (m_IsVisible)
		{
			EVisState eVisState = CheckNewVisState();
			if (m_VisState != eVisState)
			{
				m_VisState = eVisState;
				UpdateVisualization();
			}
		}
	}

	private EVisState CheckNewVisState()
	{
		CFGSelectionManager component = Camera.main.GetComponent<CFGSelectionManager>();
		if (component == null || component.SelectedCharacter == null)
		{
			return EVisState.Normal;
		}
		if (component.RicochetObjects.Contains(m_Object))
		{
			return EVisState.Selected;
		}
		if (component.SelectedCharacter.HaveAbility(ETurnAction.Ricochet))
		{
			if (component.IsUsingRicochet)
			{
				if (component.AvailableRicochetObjects.Contains(m_Object))
				{
					return (!m_Object.IsUnderCursor) ? EVisState.InRange : EVisState.Hover;
				}
			}
			else if (component.SelectedCharacter.VisibleRicochetObjects.Contains(m_Object) || (component.CellUnderCursor != null && CFGCellMap.Distance(m_Object.Cell, component.CellUnderCursor) < 3))
			{
				return EVisState.InRange;
			}
		}
		return EVisState.Normal;
	}

	private void UpdateVisualization()
	{
		Vector4 vector;
		switch (m_VisState)
		{
		case EVisState.InRange:
			vector = new Vector4(1f, 1f, 1f, 0f);
			m_Billboard.gameObject.SetActive(value: true);
			m_Billboard.speed = 1f;
			break;
		case EVisState.Hover:
			vector = new Vector4(1f, 3.5f, 1f, 0f);
			m_Billboard.gameObject.SetActive(value: true);
			m_Billboard.speed = 2f;
			break;
		case EVisState.Selected:
			vector = new Vector4(2f, 0f, 1f, 0f);
			m_Billboard.gameObject.SetActive(value: true);
			m_Billboard.speed = 0.5f;
			break;
		default:
			vector = new Vector4(0f, 0f, 0f, 0f);
			m_Billboard.gameObject.SetActive(value: false);
			break;
		}
		foreach (Material material in m_Materials)
		{
			material.SetVector("_RicoFX", vector);
		}
	}
}
