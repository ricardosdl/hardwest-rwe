public static class EFloorLevelTypeExtension
{
	public static EFloorLevelType GetLevel(this EFloorLevelType level, EFloorLevelDirection dir)
	{
		if (dir == EFloorLevelDirection.UP)
		{
			if (level == EFloorLevelType.ZERO)
			{
				return EFloorLevelType.FIRST;
			}
			return EFloorLevelType.SECOND;
		}
		if (level == EFloorLevelType.SECOND)
		{
			return EFloorLevelType.FIRST;
		}
		return EFloorLevelType.ZERO;
	}

	public static int GetFloorAimMod(this EFloorLevelType level_from, EFloorLevelType level_to)
	{
		return (level_from > level_to) ? 10 : 0;
	}
}
