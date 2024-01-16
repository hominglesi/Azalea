using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct RawInputDevice
{
	public readonly ushort UsagePage;
	public readonly ushort Usage;
	public readonly uint Flags;
	public readonly IntPtr WindowTarget;

	public RawInputDevice(ushort usagePage, ushort usage, RawInputDeviceFlags flags, IntPtr windowTarget)
	{
		UsagePage = usagePage;
		Usage = usage;
		Flags = (uint)flags;
		WindowTarget = windowTarget;
	}
}
