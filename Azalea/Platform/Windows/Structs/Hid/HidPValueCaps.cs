using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct HidPValueCaps
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

	[FieldOffset(16), MarshalAs(UnmanagedType.U1)]
	public readonly bool HasNull;

	[FieldOffset(18)]
	public readonly ushort BitSize;

	[FieldOffset(20)]
	public readonly ushort ReportCount;

	[FieldOffset(32)]
	public readonly uint UnitsExp;

	[FieldOffset(36)]
	public readonly uint Units;

	[FieldOffset(40)]
	public readonly int LogicalMin;

	[FieldOffset(44)]
	public readonly int LogicalMax;

	[FieldOffset(48)]
	public readonly int PhysicalMin;

	[FieldOffset(52)]
	public readonly int PhysicalMax;

	[FieldOffset(56)]
	public readonly HidPCapsRange Range;

	[FieldOffset(56)]
	public readonly HidPCapsNotRange NotRange;
}


