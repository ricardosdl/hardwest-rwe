using UnityEngine;

public class CFGCellObjectPart : CFGVeilingObject
{
	[SerializeField]
	private bool m_ShouldHide = true;

	[SerializeField]
	private int m_MinFloor = -1;

	protected override float CalcDestAlpha()
	{
		if (m_Parent == null || !m_ShouldHide)
		{
			return 1f;
		}
		if (m_Camera == null)
		{
			return 1f;
		}
		int currentFloorLevel = (int)m_Camera.CurrentFloorLevel;
		if (currentFloorLevel < m_Floor)
		{
			if (m_MinFloor > -1 && currentFloorLevel >= m_MinFloor)
			{
				return 1f;
			}
			return 0f;
		}
		return 1f;
	}
}
