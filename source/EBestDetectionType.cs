using System;

[Flags]
public enum EBestDetectionType
{
	NotDetected = 0,
	ShadowSpotted = 1,
	Heard = 2,
	Smelled = 4,
	Sensed = 8,
	Visible = 0x10,
	StartVal = 0x20
}
