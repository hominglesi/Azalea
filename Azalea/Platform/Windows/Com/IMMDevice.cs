using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Com;

[ComImport]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
	[PreserveSig]
	int Activate(Guid iid, int clsctx, IntPtr activationParams, [Out] out IntPtr pinterface);

	[PreserveSig]
	int OpenPropertyStore(int access, [Out] out IntPtr propertystore);

	[PreserveSig]
	int GetId([Out, MarshalAs(UnmanagedType.LPWStr)] out string deviceId);

	[PreserveSig]
	int GetState([Out] out DeviceState state);
}

public enum DeviceState
{
	Active = 0x00000001,
	Disabled = 0x00000002,
	NotPresent = 0x00000004,
	UnPlugged = 0x00000008,
	All = 0x0000000F
}
