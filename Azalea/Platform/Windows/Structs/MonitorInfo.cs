using Azalea.Native.Windows;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MonitorInfo
{
	private readonly uint _size;
	public readonly Win32.RECT Monitor;
	public readonly Win32.RECT WorkArea;
	public readonly uint Flags;

	public MonitorInfo()
	{
		_size = (uint)Marshal.SizeOf<MonitorInfo>();
		Monitor = new Win32.RECT();
		WorkArea = new Win32.RECT();
		Flags = 0;
	}
}
