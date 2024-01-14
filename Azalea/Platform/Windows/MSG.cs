using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct MSG
{
	public IntPtr hwnd;
	public uint message;
	public UIntPtr wParam;
	public UIntPtr lParam;
	public uint time;
	public POINT pt;
}

public struct POINT
{
	public int x;
	public int Y;
}
