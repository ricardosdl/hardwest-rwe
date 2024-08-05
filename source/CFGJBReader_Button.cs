using UnityEngine;

public class CFGJBReader_Button : CFGJBReader
{
	private KeyCode Button;

	private bool Get_Key;

	private bool Get_Up;

	private bool Get_Down;

	public CFGJBReader_Button(KeyCode key)
	{
		Button = key;
	}

	public void Read()
	{
		Get_Key = Input.GetKey(Button);
		Get_Up = Input.GetKeyUp(Button);
		Get_Down = Input.GetKeyDown(Button);
	}

	public float GetValue(bool bCont, bool bUseUp)
	{
		if (bCont)
		{
			if (Get_Key)
			{
				return 1f;
			}
			return 0f;
		}
		if (bUseUp)
		{
			if (Get_Up)
			{
				return 1f;
			}
		}
		else if (Get_Down)
		{
			return 1f;
		}
		return 0f;
	}
}
