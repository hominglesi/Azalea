using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MonitorInfo
{
	private readonly uint _size;
	public readonly WinRectangle Monitor;
	public readonly WinRectangle WorkArea;
	public readonly uint Flags;

	public MonitorInfo()
	{
		_size = (uint)Marshal.SizeOf<MonitorInfo>();
		Monitor = new WinRectangle();
		WorkArea = new WinRectangle();
		Flags = 0;
	}
}
