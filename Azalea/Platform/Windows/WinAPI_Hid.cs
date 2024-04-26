using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal static unsafe partial class WinAPI
{
	private const string HIDPIPath = "hid.dll";

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetButtonCaps")]
	public static extern int HidP_GetButtonCaps(HidPReportType reportType, ref HidPButtonCaps buttonCaps,
		ref ushort buttonCapsLength, ref byte preparsedData);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetCaps")]
	public static extern int HidP_GetCaps(ref byte preparsedData, out HidPCaps capabilities);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetUsages")]
	public static extern int HidP_GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection,
		[Out] ushort[]? buttonList, ref uint usageLength, ref byte preparsedData, ref byte report, uint reportLength);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetUsageValue")]
	public static extern int HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection,
		ushort usage, out int usageValue, ref byte preparsedData, ref byte report, uint reportLength);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetValueCaps")]
	public static extern int HidP_GetValueCaps(HidPReportType reportType, [Out] HidPValueCaps[] valueCaps,
		ref ushort valueCapsLength, ref byte preparsedData);
}
