using UnityEngine;

public class GUILayoutExt
{
	public static bool ListBox(bool open, string[] list, ref int selected, params GUILayoutOption[] options)
	{
		if (list.Length == 0)
		{
			return false;
		}
		string text = ((list.Length < selected) ? string.Empty : list[selected]);
		open = GUILayout.Toggle(open, text, GUI.skin.textField, options);
		if (open)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			lastRect = new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, lastRect.height * (float)list.Length);
			int num = GUI.SelectionGrid(lastRect, selected, list, 1);
			if (num != selected)
			{
				selected = num;
				open = false;
			}
		}
		return open;
	}
}
