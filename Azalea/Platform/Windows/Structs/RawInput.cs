using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal struct RawInput
{
	public readonly RawInputHeader Header;
	public RawHID HID;
	//public readonly RawMouse Mouse;
}
