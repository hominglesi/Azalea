﻿using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RawInput
{
	public readonly RawInputHeader Header;
	public readonly RawMouse Mouse;
}
