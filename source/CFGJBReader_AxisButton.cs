using UnityEngine;

public class CFGJBReader_AxisButton : CFGJBReader
{
	private string AxisName;

	private bool bPositive;

	private float fDelay = 0.5f;

	private float fLast = -1000f;

	private float AxisValue;

	private float AxisCont;

	public CFGJBReader_AxisButton(string _AxisName, bool Positive, float Delay = 0.5f)
	{
		AxisName = _AxisName;
		bPositive = Positive;
		fDelay = Delay;
	}

	public void Read()
	{
		AxisCont = Input.GetAxis(AxisName);
		if (!bPositive)
		{
			if (AxisCont > 0f)
			{
				AxisCont = 0f;
			}
			else
			{
				AxisCont = 0f - AxisCont;
			}
		}
		else if (AxisCont <= 0f)
		{
			AxisCont = 0f;
		}
		if (AxisName == null || Time.time < fLast + fDelay)
		{
			AxisValue = 0f;
			return;
		}
		AxisValue = AxisCont;
		if (Mathf.Abs(AxisValue) < 0.34f)
		{
			AxisValue = 0f;
		}
		else
		{
			fLast = Time.time;
		}
	}

	public float GetValue(bool bCont, bool bUseUp)
	{
		if (bCont)
		{
			return AxisCont;
		}
		return AxisValue;
	}
}
