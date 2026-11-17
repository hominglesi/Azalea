using System;

// Used extensively
#pragma warning disable IDE0130
namespace Azalea;
#pragma warning restore IDE0130

[Flags]
public enum Axes
{
	None = 0,

	X = 1,
	Y = 1 << 1,

	Both = X | Y
}
