using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Enums.RawInput;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct RawInputDeviceInfo
{
	[FieldOffset(0)]
	public readonly uint Size;

	[FieldOffset(4)]
	public readonly uint Type;

	[FieldOffset(8)]
	public readonly RawInputKeyboardInfo Keyboard;

	[FieldOffset(8)]
	public readonly RawInputHidInfo Hid;
}
