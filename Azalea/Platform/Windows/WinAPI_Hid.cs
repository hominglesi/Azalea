using Azalea.Platform.Windows.Enums.RawInput;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows;
internal static unsafe partial class WinAPI
{
	private const string HIDPIPath = "hid.dll";

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetButtonCaps")]
	public static extern HidStatus HidP_GetButtonCaps(HidPReportType reportType,
		[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] HidPButtonCaps[] buttonCaps,
		ref ushort buttonCapsLength, IntPtr preparsedData);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetCaps")]
	public static extern HidStatus HidP_GetCaps(IntPtr preparsedData, ref HidPCaps capabilities);

	[DllImport(HIDPIPath, EntryPoint = "HidD_GetManufacturerString")]
	public static extern bool HidD_GetManufacturerString(IntPtr deviceObject, IntPtr buffer, ulong bufferLength);

	[DllImport(HIDPIPath, EntryPoint = "HidD_GetProductString")]
	public static extern bool HidD_GetProductString(IntPtr deviceObject, IntPtr buffer, ulong bufferLength);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetUsagesEx")]
	public static extern HidStatus HidP_GetUsagesEx(HidPReportType reportType, ushort linkCollection,
		[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] HidUsageAndPage[] usageList, ref uint usageLength, IntPtr preparsedData,
		[Out][MarshalAs(UnmanagedType.LPArray)] byte[] report, uint reportLength);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetUsageValue")]
	public static extern HidStatus HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection,
		ushort usage, ref uint usageValue, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray)] byte[] report, uint reportLength);

	[DllImport(HIDPIPath, EntryPoint = "HidP_GetValueCaps")]
	public static extern HidStatus HidP_GetValueCaps(HidPReportType reportType,
		[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] HidPValueCaps[] valueCaps,
		ref ushort valueCapsLength, IntPtr preparsedData);
}
