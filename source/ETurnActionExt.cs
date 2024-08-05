public static class ETurnActionExt
{
	public static bool IsStdNonSpecial(this ETurnAction Action)
	{
		switch (Action)
		{
		case ETurnAction.Use_Item1:
		case ETurnAction.Use_Item2:
		case ETurnAction.Use_Talisman:
		case ETurnAction.Transfusion:
		case ETurnAction.Dodge:
		case ETurnAction.Smell:
		case ETurnAction.Equalization:
		case ETurnAction.Finder:
		case ETurnAction.Prayer:
		case ETurnAction.RewardedKill:
		case ETurnAction.Courage:
		case ETurnAction.ShadowKill:
		case ETurnAction.Shriek:
		case ETurnAction.Cannibal:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
		case ETurnAction.MultiShot:
		case ETurnAction.Demon:
			return true;
		default:
			return false;
		}
	}

	public static bool IsStandard(this ETurnAction Action)
	{
		switch (Action)
		{
		case ETurnAction.Ricochet:
		case ETurnAction.ShadowCloak:
		case ETurnAction.Disguise:
		case ETurnAction.Transfusion:
		case ETurnAction.Dodge:
		case ETurnAction.Smell:
		case ETurnAction.Equalization:
		case ETurnAction.Vengeance:
		case ETurnAction.Finder:
		case ETurnAction.Crippler:
		case ETurnAction.Hearing:
		case ETurnAction.Vampire:
		case ETurnAction.Prayer:
		case ETurnAction.Jinx:
		case ETurnAction.RewardedKill:
		case ETurnAction.Courage:
		case ETurnAction.ShadowKill:
		case ETurnAction.Shriek:
		case ETurnAction.Cannibal:
		case ETurnAction.Penetrate:
		case ETurnAction.Intimidate:
		case ETurnAction.ArteryShot:
		case ETurnAction.MultiShot:
		case ETurnAction.Demon:
			return true;
		default:
			return false;
		}
	}

	public static bool IsSilent(this ETurnAction Action, CFGCharacter User)
	{
		switch (Action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
			return false;
		default:
			if (!(User == null))
			{
				return User.GetAbility(Action)?.IsSilent() ?? true;
			}
			return CFGStaticDataContainer.GetAbilityDef(Action)?.IsSilent ?? true;
		}
	}

	public static bool CanCharacterBeGlued(this ETurnAction Action)
	{
		switch (Action)
		{
		case ETurnAction.None:
		case ETurnAction.Shoot:
		case ETurnAction.End:
		case ETurnAction.Reload:
		case ETurnAction.ChangeWeapon:
		case ETurnAction.OpenDoor:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
		case ETurnAction.Dodge:
		case ETurnAction.RewardedKill:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
			return true;
		default:
			return false;
		}
	}

	public static bool CanCharacterBeGluedinSetupStage(this ETurnAction Action)
	{
		switch (Action)
		{
		case ETurnAction.Shoot:
		case ETurnAction.Ricochet:
		case ETurnAction.Miss_Shoot:
		case ETurnAction.AltFire_Fanning:
		case ETurnAction.AltFire_ScopedShot:
		case ETurnAction.AltFire_ConeShot:
		case ETurnAction.Penetrate:
		case ETurnAction.ArteryShot:
			return true;
		default:
			return false;
		}
	}
}
