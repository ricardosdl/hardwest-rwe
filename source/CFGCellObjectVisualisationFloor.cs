using UnityEngine;

public class CFGCellObjectVisualisationFloor : MonoBehaviour
{
	private int m_Floor = -1;

	public int Floor
	{
		get
		{
			return m_Floor;
		}
		set
		{
			m_Floor = Mathf.Clamp(value, 0, 8);
		}
	}
}
