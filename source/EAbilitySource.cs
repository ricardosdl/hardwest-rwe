using System;

[Flags]
public enum EAbilitySource
{
	Unknown = 0,
	CharDefinition = 1,
	FlowCode = 2,
	Card = 4,
	Item = 8,
	Any = 0xFFFF
}
