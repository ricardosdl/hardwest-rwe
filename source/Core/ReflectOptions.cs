using System;

namespace Core;

[Flags]
public enum ReflectOptions
{
	EditInline = 1,
	EditNew = 2,
	Flags = 4
}
