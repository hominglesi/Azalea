using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct HidPCapsNotRange
{
	[FieldOffset(0)]
	public readonly ushort Usage;

	[FieldOffset(4)]
	public readonly ushort StringIndex;

	[FieldOffset(8)]
	public readonly ushort DesignatorIndex;

	[FieldOffset(12)]
	public readonly ushort DataIndex;
}
