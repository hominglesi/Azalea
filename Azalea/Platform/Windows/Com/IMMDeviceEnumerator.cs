using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Windows.Com;

[ComImport]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceEnumerator
{
	[PreserveSig]
	int EnumAudioEndpoints(int dataFlow, DeviceState stateMask, [Out] out IMMDeviceCollection deviceCollection);

	[PreserveSig]
	int GetDefaultAudioEndpoint(int dataFlow, int role, [Out] out IMMDevice device);

	[PreserveSig]
	int GetDevice([In, MarshalAs(UnmanagedType.LPWStr)] string id, [Out] out IMMDevice device);

	[PreserveSig]
	int RegisterEndpointNotificationCallback(IMMNotificationClient notificationClient);

	[PreserveSig]
	int UnregisterEndpointNotificationCallback(IMMNotificationClient notificationClient);
}

[ComImport]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
internal class MMDeviceEnumeratorObject { }
