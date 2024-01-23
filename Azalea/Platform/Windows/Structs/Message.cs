using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct Message
{
	public IntPtr Window;
	public uint Value;
	public UIntPtr WParam;
	public UIntPtr LParam;
	public uint Time;
	public Vector2Int Point;
}
