using System;

[Flags]
public enum eTargetableType
{
	None = 0,
	Self = 1,
	Enemy = 2,
	Friendly = 4,
	Other = 8,
	Cell = 0x10
}
