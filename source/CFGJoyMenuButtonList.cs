using System.Collections.Generic;
using UnityEngine;

public class CFGJoyMenuButtonList
{
	public enum EListNavigationMode
	{
		UpDown,
		LeftRight,
		Matrix
	}

	public delegate void OnItemSelected(int ItemID);

	public delegate void OnItemActivated(int ItemID, bool bPrimary);

	public List<CFGButtonExtension> m_Buttons;

	public int m_Collumns = 1;

	public int m_CurrentItem = -1;

	public OnItemSelected m_CB_OnSelected;

	public OnItemActivated m_CB_OnActivated;

	public bool m_CheckText;

	public EListNavigationMode m_NaviMode;

	public EJoyButton m_UP_Analog = EJoyButton.LA_Top;

	public EJoyButton m_Down_Analog = EJoyButton.LA_Bottom;

	public EJoyButton m_UP_Digital = EJoyButton.DPad_Top;

	public EJoyButton m_Down_Digital = EJoyButton.DPad_Bottom;

	public bool m_bCheckVisibleOnly;

	public bool m_bAllowEmpty;

	public void ChangeSelection(int Add)
	{
		if (m_Buttons == null || m_Buttons.Count == 0 || Add == 0)
		{
			return;
		}
		FindCurrentItem();
		int num = m_CurrentItem;
		bool flag = false;
		if (Add != 1 && Add != -1)
		{
			for (int num2 = (m_Buttons.Count + m_Collumns - 1) / m_Collumns; num2 > 0; num2--)
			{
				num += Add;
				if (num > m_Buttons.Count)
				{
					num -= m_Buttons.Count;
				}
				if (num < 0)
				{
					num = m_Buttons.Count + num;
				}
				int num3 = num / m_Collumns;
				int num4 = -1;
				int num5 = 10000;
				for (int i = 0; i < m_Collumns; i++)
				{
					int num6 = num3 * m_Collumns + i;
					if (num6 >= m_Buttons.Count)
					{
						num6 -= m_Buttons.Count;
					}
					if (!(m_Buttons[num6] == null) && !m_Buttons[num6].m_VisualsDisabled && m_Buttons[num6].enabled)
					{
						int num7 = Mathf.Abs(num - num6);
						if (num7 < num5)
						{
							num5 = num7;
							num4 = num6;
						}
					}
				}
				if (num4 > -1)
				{
					SelectItem(num4);
					break;
				}
			}
			return;
		}
		while (!flag)
		{
			num += Add;
			if (num < 0)
			{
				num = m_Buttons.Count - 1;
				if (m_CurrentItem == -1)
				{
					return;
				}
			}
			if (num >= m_Buttons.Count)
			{
				num = 0;
				if (m_CurrentItem == -1)
				{
					return;
				}
			}
			if (num == m_CurrentItem)
			{
				return;
			}
			if (!(m_Buttons[num] != null) || m_Buttons[num].m_VisualsDisabled || !m_Buttons[num].enabled || ((!m_bCheckVisibleOnly || !m_Buttons[num].isActiveAndEnabled) && m_bCheckVisibleOnly))
			{
				continue;
			}
			if (m_CheckText)
			{
				if (m_Buttons[num].m_Label.text != string.Empty)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		SelectItem(num);
	}

	private bool CanUseButton(int i)
	{
		if (i < 0 || i >= m_Buttons.Count)
		{
			return false;
		}
		if (m_Buttons[i] != null && m_Buttons[i].enabled && !m_Buttons[i].m_VisualsDisabled && ((m_bCheckVisibleOnly && m_Buttons[i].isActiveAndEnabled) || !m_bCheckVisibleOnly))
		{
			if (!m_CheckText)
			{
				return true;
			}
			if (m_Buttons[i].m_Label.text != string.Empty)
			{
				return true;
			}
		}
		return false;
	}

	public void FindCurrentItem()
	{
		int currentItem = m_CurrentItem;
		m_CurrentItem = FindSelectedItem(0);
	}

	public void SelectFirstItem()
	{
		if (m_Buttons == null || m_Buttons.Count == 0)
		{
			return;
		}
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			if (CanUseButton(i))
			{
				SelectItem(i);
				break;
			}
		}
	}

	public int FindSelectedItem(int First)
	{
		if (m_Buttons == null || m_Buttons.Count == 0 || First >= m_Buttons.Count)
		{
			return -1;
		}
		int i = First;
		if (i < 0)
		{
			i = 0;
		}
		for (; i < m_Buttons.Count; i++)
		{
			if (m_Buttons[i] != null && m_Buttons[i].IsUnderCursor && m_Buttons[i].enabled && !m_Buttons[i].m_VisualsDisabled && ((m_bCheckVisibleOnly && m_Buttons[i].isActiveAndEnabled) || !m_bCheckVisibleOnly))
			{
				if (!m_CheckText)
				{
					return i;
				}
				if (m_Buttons[i].m_Label.text != string.Empty)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public void SelectItem(int ItemID)
	{
		if (m_Buttons == null || m_Buttons.Count == 0)
		{
			return;
		}
		if (m_CurrentItem >= 0 && m_CurrentItem < m_Buttons.Count && m_Buttons[m_CurrentItem] != null && m_Buttons[m_CurrentItem].enabled)
		{
			m_Buttons[m_CurrentItem].OnPointerExit(null);
		}
		if (ItemID < 0 || ItemID >= m_Buttons.Count)
		{
			m_CurrentItem = -1;
		}
		else
		{
			m_CurrentItem = ItemID;
		}
		if (m_CurrentItem >= 0 && m_CurrentItem < m_Buttons.Count && m_Buttons[m_CurrentItem] != null && m_Buttons[m_CurrentItem].enabled)
		{
			m_Buttons[m_CurrentItem].OnPointerEnter(null);
			if (m_CB_OnSelected != null)
			{
				m_CB_OnSelected(m_CurrentItem);
			}
		}
		else
		{
			m_CurrentItem = -1;
		}
	}

	public void ReadInput(ref bool bAnalog)
	{
		if (m_Buttons == null || m_Buttons.Count == 0)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		if (bAnalog)
		{
			switch (m_NaviMode)
			{
			case EListNavigationMode.UpDown:
				if (CFGJoyManager.ReadAsButton(m_Down_Analog) > 0.25f)
				{
					num = 1;
					bAnalog = true;
				}
				if (CFGJoyManager.ReadAsButton(m_UP_Analog) > 0.25f)
				{
					num = -1;
					bAnalog = true;
				}
				break;
			case EListNavigationMode.LeftRight:
				if (CFGJoyManager.ReadAsButton(m_UP_Analog) > 0.3f)
				{
					num = 1;
					bAnalog = true;
				}
				if (CFGJoyManager.ReadAsButton(m_Down_Analog) > 0.3f)
				{
					num = -1;
					bAnalog = true;
				}
				break;
			case EListNavigationMode.Matrix:
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Bottom) > 0.5f)
				{
					num = m_Collumns;
				}
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Top) > 0.5f)
				{
					num = -m_Collumns;
				}
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Right) > 0.5f)
				{
					num = 1;
				}
				if (CFGJoyManager.ReadAsButton(EJoyButton.LA_Left) > 0.5f)
				{
					num = -1;
				}
				break;
			}
		}
		switch (m_NaviMode)
		{
		case EListNavigationMode.UpDown:
			if (CFGJoyManager.ReadAsButton(m_Down_Digital) > 0f)
			{
				num = 1;
			}
			if (CFGJoyManager.ReadAsButton(m_UP_Digital) > 0f)
			{
				num = -1;
			}
			break;
		case EListNavigationMode.LeftRight:
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right) > 0f)
			{
				num = 1;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left) > 0f)
			{
				num = -1;
			}
			break;
		case EListNavigationMode.Matrix:
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Right) > 0f)
			{
				num = 1;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Left) > 0f)
			{
				num = -1;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Bottom) > 0f)
			{
				num = m_Collumns;
			}
			if (CFGJoyManager.ReadAsButton(EJoyButton.DPad_Top) > 0f)
			{
				num = -m_Collumns;
			}
			break;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyA) > 0f)
		{
			flag = true;
		}
		if (CFGJoyManager.ReadAsButton(EJoyButton.KeyX) > 0f)
		{
			flag2 = true;
		}
		if (num != 0)
		{
			ChangeSelection(num);
		}
		if (m_CurrentItem >= 0 && m_CurrentItem < m_Buttons.Count && m_CB_OnActivated != null)
		{
			if (flag)
			{
				m_CB_OnActivated(m_CurrentItem, bPrimary: false);
			}
			if (flag2)
			{
				m_CB_OnActivated(m_CurrentItem, bPrimary: true);
			}
		}
		if (m_CurrentItem < 0 && CFGInput.LastReadInputDevice == EInputMode.Gamepad)
		{
			SelectFirstItem();
		}
	}
}
