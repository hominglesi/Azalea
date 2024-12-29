using System;

namespace Azalea.Platform.Windows;

[Flags]
internal enum ClassStyles : uint
{
	VerticalReDraw = 0x0001,
	HorizontalReDraw = 0x0002,
	OwnDC = 0x20,
}
