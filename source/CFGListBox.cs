using UnityEngine;

public class CFGListBox
{
	private string[] m_Elements;

	private bool m_IsOpen;

	private int m_SelectedElementIdx;

	public CFGListBox(string[] elements)
	{
		m_Elements = elements;
	}

	public void Draw(params GUILayoutOption[] options)
	{
		m_IsOpen = GUILayoutExt.ListBox(m_IsOpen, m_Elements, ref m_SelectedElementIdx, options);
	}

	public void SelectElement(int element_idx)
	{
		m_SelectedElementIdx = element_idx;
	}

	public int GetSelectedElementIdx()
	{
		return m_SelectedElementIdx;
	}
}
