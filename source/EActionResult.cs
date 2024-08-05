using System;

[Flags]
public enum EActionResult
{
	Success = 0,
	Failed = 1,
	NotEnoughAP = 2,
	NotEnoughLuck = 4,
	OnCooldown = 8,
	InvalidTarget = 0x10,
	TargetNotVisible = 0x20,
	TargetDead = 0x40,
	OutOfRange = 0x80,
	NotInCone = 0x100,
	NoWeapon = 0x400,
	NoAbility = 0x800,
	NoAmmo = 0x1000,
	NotInShadow = 0x8000,
	NoTargetInRange = 0x10000,
	NoLineOfFire = 0x100000,
	NoLineOfSight = 0x200000,
	NotInSetupStage = 0x2000000,
	Dead = 0x20000000,
	Busy = 0x40000000,
	Imprisoned = int.MinValue
}
