using UnityEngine;

public class CFGJBReader_Axis : CFGJBReader
{
	private string AxisName;

	private bool bPositive;

	private float AxisValue;

	public CFGJBReader_Axis(string _AxisName, bool Positive)
	{
		AxisName = _AxisName;
		bPositive = Positive;
	}

	public void Read()
	{
		if (AxisName == null)
		{
			AxisValue = 0f;
			return;
		}
		float axis = Input.GetAxis(AxisName);
		if (!bPositive)
		{
			if (axis > -0.2f)
			{
				AxisValue = 0f;
			}
			else
			{
				AxisValue = 0f - axis;
			}
		}
		else if (axis <= 0f)
		{
			AxisValue = 0f;
		}
		else
		{
			AxisValue = axis;
		}
	}

	public float GetValue(bool bCont, bool bUseUp)
	{
		return AxisValue;
	}
}
