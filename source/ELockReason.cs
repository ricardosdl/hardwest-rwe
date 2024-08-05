using System;

[Flags]
public enum ELockReason
{
	NoLock = 0,
	FadeOut = 1,
	MakeAction = 2,
	NonPlayerTurn = 4,
	Wnd_BarterScreen = 0x8000,
	Wnd_CharacterScreen = 0x10000,
	Wnd_InGameMenu = 0x40000,
	Wnd_MissionEnd = 0x80000,
	Wnd_StrategicExplorator = 0x100000,
	Wnd_TutorialPopup = 0x200000
}
