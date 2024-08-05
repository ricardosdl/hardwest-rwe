using System;

[Flags]
public enum ECharacterStateFlag
{
	None = 0,
	TempTactical = 1,
	CanBeHired = 2,
	ExistedAtTacticalStart = 4,
	InDemonForm = 0x400,
	IsCritical = 0x4000,
	IsDead = 0x8000,
	Imprisoned = 0x10000,
	ImmuneToGunpoint = 0x20000,
	Invulnerable = 0x40000,
	CorpseLooted = 0x80000,
	Injured = 0x100000,
	CanDoReactionShoot = 0x200000,
	TurnFinishedAndLocked = 0x400000,
	ImmuneToDecay = 0x800000,
	SensendByPlayer = 0x2000000,
	MarkedForAchiev_06 = 0x4000000
}
