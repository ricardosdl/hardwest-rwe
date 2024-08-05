using System;

[Flags]
public enum ECustomGameplayOption
{
	FullDeck = 1,
	NoReactionShot = 2,
	NoCoverReduction = 4,
	EnemyAbilities = 8,
	FastScars = 0x10,
	KillsReplenishLuck = 0x20,
	LuckRegeneration = 0x40,
	MoreWounds = 0x80,
	UnknownEnemyHealth = 0x100,
	NonchangableCards = 0x200
}
