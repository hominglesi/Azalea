using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RawInputHeader
{
	public readonly uint Type;
	public readonly uint Size;
	public readonly IntPtr Device;
	public readonly IntPtr WParam;
}
