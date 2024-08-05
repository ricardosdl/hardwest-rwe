using System.Collections.Generic;
using UnityEngine;

public class CFGJoyMenuController
{
	public delegate void OnOnListChanged(int OldListID, int NewListID, bool bPrev);

	private List<CFGJoyMenuButtonList> m_Lists = new List<CFGJoyMenuButtonList>();

	private int m_CurrentList = -1;

	private EJoyButton m_PrevList = EJoyButton.LeftBumper;

	private EJoyButton m_NextList = EJoyButton.RightBumper;

	private EJoyButton m_PrevListAnalog;

	private EJoyButton m_NextListAnalog;

	private EJoyButton m_PrevListAnalog2;

	private EJoyButton m_NextListAnalog2;

	public OnOnListChanged m_CB_OnListChanged;

	private float m_NextReadTime;

	public int CurrentList => m_CurrentList;

	public CFGJoyMenuButtonList CurrentListObject
	{
		get
		{
			if (m_Lists == null)
			{
				return null;
			}
			if (m_CurrentList >= 0 && m_CurrentList < m_Lists.Count)
			{
				return m_Lists[m_CurrentList];
			}
			return null;
		}
	}

	public void Reset()
	{
		m_Lists.Clear();
	}

	public void SetListChangeButtons(EJoyButton Prev, EJoyButton Next, EJoyButton PrevAnalog, EJoyButton NextAnalog, EJoyButton PrevAnalog2, EJoyButton NextAnalog2)
	{
		m_PrevList = Prev;
		m_NextList = Next;
		m_NextListAnalog = NextAnalog;
		m_PrevListAnalog = PrevAnalog;
		m_PrevListAnalog2 = PrevAnalog2;
		m_NextListAnalog2 = NextAnalog2;
	}

	public CFGJoyMenuButtonList AddList(List<CFGButtonExtension> buttons, CFGJoyMenuButtonList.OnItemSelected CB_Selected, CFGJoyMenuButtonList.OnItemActivated CB_Activated, int collumns = 1, bool bCheckText = false)
	{
		if (buttons == null)
		{
			Debug.LogError("Failed to add list: no buttons!");
			return null;
		}
		CFGJoyMenuButtonList cFGJoyMenuButtonList = new CFGJoyMenuButtonList();
		cFGJoyMenuButtonList.m_Buttons = buttons;
		cFGJoyMenuButtonList.m_Collumns = collumns;
		cFGJoyMenuButtonList.m_CB_OnActivated = CB_Activated;
		cFGJoyMenuButtonList.m_CB_OnSelected = CB_Selected;
		cFGJoyMenuButtonList.m_CheckText = bCheckText;
		m_Lists.Add(cFGJoyMenuButtonList);
		return cFGJoyMenuButtonList;
	}

	public void UpdateInput()
	{
		if (CFGButtonExtension.IsWaitingForClick)
		{
			return;
		}
		int num = 0;
		float num2 = 0.2f;
		if (m_Lists.Count == 0)
		{
			return;
		}
		bool flag = m_Lists.Count == 1;
		CFGJoyMenuButtonList cFGJoyMenuButtonList = null;
		if (!flag)
		{
			if (Time.time > m_NextReadTime)
			{
				if (m_NextListAnalog != 0 && CFGJoyManager.ReadAsButton(m_NextListAnalog) > 0.5f)
				{
					num = 1;
				}
				if (m_NextListAnalog2 != 0 && CFGJoyManager.ReadAsButton(m_NextListAnalog2) > 0.5f)
				{
					num = 1;
				}
				if (m_PrevListAnalog != 0 && CFGJoyManager.ReadAsButton(m_PrevListAnalog) > 0.5f)
				{
					num = -1;
				}
				if (m_PrevListAnalog2 != 0 && CFGJoyManager.ReadAsButton(m_PrevListAnalog2) > 0.5f)
				{
					num = -1;
				}
				if (num != 0)
				{
					m_NextReadTime = Time.time + num2;
				}
			}
			if (m_NextList != 0 && CFGJoyManager.ReadAsButton(m_NextList) > 0f)
			{
				num = 1;
			}
			if (m_PrevList != 0 && CFGJoyManager.ReadAsButton(m_PrevList) > 0f)
			{
				num = -1;
			}
			if (num != 0)
			{
				ChangeList(num, bUseCallback: true);
			}
		}
		if (m_CurrentList < 0 && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			SelectList(0);
		}
		if (m_CurrentList >= 0 && m_CurrentList < m_Lists.Count)
		{
			cFGJoyMenuButtonList = m_Lists[m_CurrentList];
		}
		if (cFGJoyMenuButtonList != null)
		{
			bool bAnalog = false;
			if (Time.time > m_NextReadTime)
			{
				bAnalog = true;
			}
			cFGJoyMenuButtonList.ReadInput(ref bAnalog);
			if (bAnalog)
			{
				m_NextReadTime = Time.time + num2;
			}
		}
		switch (CFGInput.LastReadInputDevice)
		{
		case EInputMode.Gamepad:
			Deselect_NotCurrent(bOther: true);
			break;
		case EInputMode.KeyboardAndMouse:
			Deselect_Current(bOther: false);
			break;
		}
	}

	private int SelectionCount(CFGJoyMenuButtonList lst)
	{
		int num = 0;
		for (int num2 = lst.FindSelectedItem(0); num2 >= 0; num2 = lst.FindSelectedItem(num2 + 1))
		{
			num++;
		}
		return num;
	}

	private void Deselect_Current(bool bOther)
	{
		CFGJoyMenuButtonList currentListObject = CurrentListObject;
		if (currentListObject != null && currentListObject.m_CurrentItem >= 0 && currentListObject.m_Buttons != null && currentListObject.m_Buttons.Count != 0)
		{
			int num = SelectionCount(currentListObject);
			if (num != 1 && num != 0 && currentListObject.m_CurrentItem >= 0 && currentListObject.m_CurrentItem < currentListObject.m_Buttons.Count)
			{
				currentListObject.m_Buttons[currentListObject.m_CurrentItem].OnPointerExit(null);
			}
		}
	}

	private void Deselect_NotCurrent(bool bOther)
	{
		CFGJoyMenuButtonList currentListObject = CurrentListObject;
		if (currentListObject == null || currentListObject.m_CurrentItem < 0 || currentListObject.m_Buttons == null || currentListObject.m_Buttons.Count == 0)
		{
			return;
		}
		int num = SelectionCount(currentListObject);
		if (num == 1 || num == 0)
		{
			return;
		}
		for (int num2 = currentListObject.FindSelectedItem(0); num2 >= 0; num2 = currentListObject.FindSelectedItem(num2 + 1))
		{
			if (num2 != currentListObject.m_CurrentItem && currentListObject.m_Buttons[num2] != null)
			{
				currentListObject.m_Buttons[currentListObject.m_CurrentItem].OnPointerExit(null);
			}
		}
	}

	public void ChangeList(int Add, bool bUseCallback)
	{
		if (Add != 1 && Add != -1)
		{
			return;
		}
		int num = m_CurrentList;
		do
		{
			num += Add;
			if (num >= m_Lists.Count)
			{
				num = 0;
			}
			if (num < 0)
			{
				num = m_Lists.Count - 1;
			}
			if (num == m_CurrentList)
			{
				return;
			}
		}
		while (m_Lists[num] == null || (!m_Lists[num].m_bAllowEmpty && (m_Lists[num].m_Buttons == null || m_Lists[num].m_Buttons.Count <= 0)));
		if (num != m_CurrentList)
		{
			if (bUseCallback && m_CB_OnListChanged != null)
			{
				m_CB_OnListChanged(m_CurrentList, num, Add < 0);
			}
			SelectList(num);
		}
	}

	public void DeactivateList(int n)
	{
		if (n < 0 || n >= m_Lists.Count || m_Lists[n] == null)
		{
			return;
		}
		for (int i = 0; i < m_Lists[n].m_Buttons.Count; i++)
		{
			if (m_Lists[n].m_Buttons[i] != null)
			{
				m_Lists[n].m_Buttons[i].OnPointerExit(null);
			}
		}
		m_Lists[n].m_CurrentItem = -1;
	}

	public void SelectList(int n)
	{
		if (n < 0 || n >= m_Lists.Count)
		{
			n = -1;
		}
		if (m_CurrentList >= 0 && m_CurrentList < m_Lists.Count)
		{
			DeactivateList(m_CurrentList);
		}
		DeactivateList(n);
		m_CurrentList = n;
		if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			m_Lists[n].SelectFirstItem();
		}
	}

	public void ReclickCurrent()
	{
		if (m_CurrentList >= 0 && m_Lists[m_CurrentList].m_CurrentItem >= 0)
		{
			m_Lists[m_CurrentList].m_Buttons[m_Lists[m_CurrentList].m_CurrentItem].SimulateClick();
			m_Lists[m_CurrentList].m_Buttons[m_Lists[m_CurrentList].m_CurrentItem].OnPointerEnter(null);
		}
	}

	public void SelectFirstItem(bool bForce)
	{
		if (m_CurrentList < 0 || m_CurrentList >= m_Lists.Count || m_Lists[m_CurrentList] == null)
		{
			if (m_Lists.Count <= 0)
			{
				return;
			}
			m_CurrentList = 0;
		}
		if (m_Lists[m_CurrentList].m_Buttons != null && m_Lists[m_CurrentList].m_Buttons.Count != 0)
		{
			m_Lists[m_CurrentList].FindCurrentItem();
			if (bForce)
			{
				m_Lists[m_CurrentList].SelectFirstItem();
			}
			if (CFGInput.LastReadInputDevice == EInputMode.Gamepad)
			{
				m_Lists[m_CurrentList].SelectFirstItem();
			}
		}
	}
}
