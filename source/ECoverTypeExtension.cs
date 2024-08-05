using UnityEngine;

public static class ECoverTypeExtension
{
	public static ECoverType Max(ECoverType first, ECoverType second)
	{
		return (ECoverType)Mathf.Max((int)first, (int)second);
	}

	public static ECoverType Min(ECoverType first, ECoverType second)
	{
		return (ECoverType)Mathf.Min((int)first, (int)second);
	}

	public static ECoverType Next(this ECoverType cover)
	{
		return cover switch
		{
			ECoverType.NONE => ECoverType.HALF, 
			ECoverType.HALF => ECoverType.FULL, 
			_ => ECoverType.NONE, 
		};
	}

	public static int GetAimMod(this ECoverType cover)
	{
		return cover switch
		{
			ECoverType.FULL => -30, 
			ECoverType.HALF => -20, 
			_ => 0, 
		};
	}
}
