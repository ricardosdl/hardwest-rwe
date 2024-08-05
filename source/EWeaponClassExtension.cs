public static class EWeaponClassExtension
{
	public static int GetMinDist(this EWeaponClass weapon_class)
	{
		return weapon_class switch
		{
			EWeaponClass.CLOSE => 0, 
			EWeaponClass.SHORT => 6, 
			EWeaponClass.MEDIUM => 14, 
			EWeaponClass.LONG => 20, 
			_ => 0, 
		};
	}

	public static int GetMaxDist(this EWeaponClass weapon_class)
	{
		return weapon_class switch
		{
			EWeaponClass.CLOSE => 6, 
			EWeaponClass.SHORT => 14, 
			EWeaponClass.MEDIUM => 20, 
			EWeaponClass.LONG => 100, 
			_ => 0, 
		};
	}

	public static int GetPenaltyMod(this EWeaponClass weapon_class)
	{
		return weapon_class switch
		{
			EWeaponClass.CLOSE => -5, 
			EWeaponClass.SHORT => -2, 
			EWeaponClass.MEDIUM => -1, 
			EWeaponClass.LONG => -2, 
			_ => 0, 
		};
	}

	public static int GetDistMod(this EWeaponClass weapon_class, int distance)
	{
		if (distance < 0)
		{
			distance = 0;
		}
		int num = 15;
		int minDist = weapon_class.GetMinDist();
		int maxDist = weapon_class.GetMaxDist();
		int num2 = 0;
		if (distance < minDist)
		{
			num2 = minDist - distance;
		}
		else if (distance > maxDist)
		{
			num2 = distance - maxDist;
		}
		num -= num2 * 2;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public static string GetLocalizedName(this EWeaponClass weapon_class)
	{
		return weapon_class switch
		{
			EWeaponClass.CLOSE => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_range_close"), 
			EWeaponClass.SHORT => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_range_short"), 
			EWeaponClass.MEDIUM => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_range_medium"), 
			EWeaponClass.LONG => CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("weapon_range_long"), 
			_ => string.Empty, 
		};
	}
}
