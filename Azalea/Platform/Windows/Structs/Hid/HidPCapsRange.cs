using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct HidPCapsRange
{
	public readonly ushort UsageMin;
	public readonly ushort UsageMax;
	public readonly ushort StringMin;
	public readonly ushort StringMax;
	public readonly ushort DesignatorMin;
	public readonly ushort DesignatorMax;
	public readonly ushort DataIndexMin;
	public readonly ushort DataIndexMax;
}
