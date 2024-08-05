using System.Collections.Generic;
using UnityEngine;

public class CFGInteractiveObjVis : MonoBehaviour
{
	private enum EVisState
	{
		Normal,
		HighlightInRange,
		HighlightOutOfRange
	}

	private CFGGameObject m_Object;

	private CFGCell m_Cell;

	private HashSet<Material> m_Materials = new HashSet<Material>();

	private bool m_IsVisible = true;

	private EVisState m_VisState;

	private void Start()
	{
		m_Object = GetComponent<CFGGameObject>();
		if (m_Object == null)
		{
			Debug.LogWarning("WARNING! CFGInteractiveObjVis script attached to object with no CFGGameObject script!", base.gameObject);
			return;
		}
		m_Cell = CFGCellMap.GetCharacterCell(m_Object.Transform.position);
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
			{
				Material material = componentsInChildren[i].materials[j];
				if (material != null && material.shader.name == "CFG/Custom/Usable")
				{
					m_Materials.Add(material);
				}
			}
		}
	}

	private void Update()
	{
		if (m_Object == null || m_Cell == null)
		{
			return;
		}
		CFGCamera component = Camera.main.GetComponent<CFGCamera>();
		bool flag = component != null && (int)component.CurrentFloorLevel >= m_Cell.Floor;
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
		CFGDoorObject cFGDoorObject = m_Object as CFGDoorObject;
		if (cFGDoorObject != null && cFGDoorObject.IsOpened)
		{
			return EVisState.Normal;
		}
		CFGUsableObject cFGUsableObject = m_Object as CFGUsableObject;
		if (cFGUsableObject != null && !cFGUsableObject.CanBeUsed())
		{
			return EVisState.Normal;
		}
		if (component.CellUnderCursor != null && CFGCellMap.Distance(m_Cell, component.CellUnderCursor) < 3)
		{
			if (cFGUsableObject != null)
			{
				HashSet<CFGCell> hashSet = CFGCellDistanceFinder.FindCellsInDistance(component.SelectedCharacter.CurrentCell, (component.SelectedCharacter.ActionPoints != 2) ? component.SelectedCharacter.HalfMovement : component.SelectedCharacter.DashingMovement);
				List<CFGCell> activatorCells = cFGUsableObject.GetActivatorCells();
				bool flag = false;
				foreach (CFGCell item in activatorCells)
				{
					if (hashSet.Contains(item))
					{
						flag = true;
						break;
					}
				}
				return flag ? EVisState.HighlightInRange : EVisState.HighlightOutOfRange;
			}
			if (cFGDoorObject != null)
			{
				if (cFGDoorObject.IsLocked)
				{
					return EVisState.HighlightOutOfRange;
				}
				HashSet<CFGCell> hashSet2 = CFGCellDistanceFinder.FindCellsInDistance(component.SelectedCharacter.CurrentCell, (component.SelectedCharacter.ActionPoints != 2) ? component.SelectedCharacter.HalfMovement : component.SelectedCharacter.DashingMovement);
				List<CFGCell> activatorCells2 = cFGDoorObject.GetActivatorCells();
				bool flag2 = false;
				foreach (CFGCell item2 in activatorCells2)
				{
					if (hashSet2.Contains(item2))
					{
						flag2 = true;
						break;
					}
				}
				return flag2 ? EVisState.HighlightInRange : EVisState.HighlightOutOfRange;
			}
			return component.SelectedCharacter.IsCellInMoveRange(m_Cell) ? EVisState.HighlightInRange : EVisState.HighlightOutOfRange;
		}
		return EVisState.Normal;
	}

	private void UpdateVisualization()
	{
		Vector4 vector = m_VisState switch
		{
			EVisState.HighlightInRange => new Vector4(1f, 1f, 1f, 0f), 
			EVisState.HighlightOutOfRange => new Vector4(1f, 1f, 0f, 0f), 
			_ => new Vector4(0f, 0f, 0f, 0f), 
		};
		foreach (Material material in m_Materials)
		{
			material.SetVector("_RicoFX", vector);
		}
	}
}
