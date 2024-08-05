using System;

namespace Core;

public static class EnumUtils
{
	public static bool HasFlag(this Enum e, Enum test)
	{
		return (Convert.ToUInt64(e) & Convert.ToUInt64(test)) == Convert.ToUInt64(test);
	}

	public static int TranslateFlag(this Enum e, Enum test, Enum ret)
	{
		return e.HasFlag(test) ? Convert.ToInt32(ret) : 0;
	}
}
