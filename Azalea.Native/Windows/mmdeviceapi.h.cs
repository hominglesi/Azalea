using System.Runtime.InteropServices;

namespace Azalea.Native.Windows;
public static partial class Win32
{
	public const int DEVICE_STATE_ACTIVE = 0x00000001;
	public const int DEVICE_STATE_DISABLED = 0x00000002;
	public const int DEVICE_STATE_NOTPRESENT = 0x00000004;
	public const int DEVICE_STATE_UNPLUGGED = 0x00000008;
	public const int DEVICE_STATEMASK_ALL = 0x0000000F;

	[ComImport]
	[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMDevice
	{
		[PreserveSig]
		int Activate([In] GUID iid, [In] int dwClsCtx, [In] nint pActivationParams, [Out] out nint ppInterface);

		[PreserveSig]
		int GetId([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

		[PreserveSig]
		int GetState([Out] out int state);

		[PreserveSig]
		int OpenPropertyStore([In] int stgmAccess, [Out] out nint ppProperties);
	}

	[ComImport]
	[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMDeviceCollection
	{
		[PreserveSig]
		int GetCount([Out] out uint pcDevices);

		[PreserveSig]
		int Item([In] int nDevice, [Out] out IMMDevice ppDevice);
	}

	[ComImport]
	[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMDeviceEnumerator
	{
		[PreserveSig]
		int EnumAudioEndpoints([In] int dataFlow, [In] int dwStateMask, [Out] out IMMDeviceCollection ppDevices);

		[PreserveSig]
		int GetDefaultAudioEndpoint([In] int dataFlow, [In] int role, [Out] out IMMDevice ppEndpoint);

		[PreserveSig]
		int GetDevice([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrId, [Out] out IMMDevice ppDevice);

		[PreserveSig]
		int RegisterEndpointNotificationCallback([In] IMMNotificationClient pClient);

		[PreserveSig]
		int UnregisterEndpointNotificationCallback([In] IMMNotificationClient pClient);
	}

	[ComImport]
	[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMNotificationClient
	{
		void OnDefaultDeviceChanged([In] int flow, [In] int role, [In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);
		void OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
		void OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
		void OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, int dwNewState);
		void OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PROPERTYKEY key);
	}

	[ComImport]
	[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
	public class MMDeviceEnumerator { }
}
