using System;

[Flags]
public enum eTargetStates
{
	None = 0,
	InShadow = 1,
	Dead = 2,
	Wounded = 4,
	Unlooted = 8
}
