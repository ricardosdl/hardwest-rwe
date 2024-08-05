public static class EActionResultExt
{
	public static bool CheckFlag(this EActionResult Status, EActionResult Flag)
	{
		if ((Status & Flag) == Flag)
		{
			return true;
		}
		return false;
	}
}
