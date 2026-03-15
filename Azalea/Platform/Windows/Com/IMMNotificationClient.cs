using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Com;

[ComImport]
[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMNotificationClient
{
	void OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, int newState);

	void OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

	void OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

	void OnDefaultDeviceChanged([In] int dataFlow, [In] int role, [In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

	void OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey key);
}

[StructLayout(LayoutKind.Sequential)]
public struct PropertyKey()
{
	public Guid ID;
	public int PropertyID;
}
