using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal struct IconInfo
{
	bool fIcon;
	uint xHotspot = 0;
	uint yHotspot = 0;
	IntPtr hbmMask;
	IntPtr hbmColor;

	public IconInfo(bool isIcon, IntPtr mask, IntPtr color)
	{
		fIcon = isIcon;
		hbmMask = mask;
		hbmColor = color;
	}
}
