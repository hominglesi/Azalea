using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct HidPButtonCaps
{
	[FieldOffset(0)]
	public readonly ushort UsagePage;

	[FieldOffset(2)]
	public readonly byte ReportID;

	[FieldOffset(3), MarshalAs(UnmanagedType.U1)]
	public readonly bool IsAlias;

	[FieldOffset(4)]
	public readonly ushort BitField;

	[FieldOffset(6)]
	public readonly ushort LinkCollection;

	[FieldOffset(8)]
	public readonly ushort LinkUsage;

	[FieldOffset(10)]
	public readonly ushort LinkUsagePage;

	[FieldOffset(12), MarshalAs(UnmanagedType.U1)]
	public readonly bool IsRange;

	[FieldOffset(13), MarshalAs(UnmanagedType.U1)]
	public readonly bool IsStringRange;

	[FieldOffset(14), MarshalAs(UnmanagedType.U1)]
	public readonly bool IsDesignatorRange;

	[FieldOffset(15), MarshalAs(UnmanagedType.U1)]
	public readonly bool IsAbsolute;

	[FieldOffset(16)]
	public readonly ushort ReportCount;

	[FieldOffset(56)]
	public readonly HidPCapsRange Range;

	[FieldOffset(56)]
	public readonly HidPCapsNotRange NotRange;
}
