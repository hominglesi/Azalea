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
		int Activate(GUID iid, int dwClsCtx, nint pActivationParams, out nint ppInterface);

		[PreserveSig]
		int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

		[PreserveSig]
		int GetState(out int state);

		[PreserveSig]
		int OpenPropertyStore([In] int stgmAccess, [Out] out nint ppProperties);
	}

	[ComImport]
	[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMDeviceCollection
	{
		[PreserveSig]
		int GetCount(out uint pcDevices);

		[PreserveSig]
		int Item([In] int nDevice, out IMMDevice ppDevice);
	}

	[ComImport]
	[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMDeviceEnumerator
	{
		[PreserveSig]
		int EnumAudioEndpoints(int dataFlow, int dwStateMask, out IMMDeviceCollection ppDevices);

		[PreserveSig]
		int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppEndpoint);

		[PreserveSig]
		int GetDevice([MarshalAs(UnmanagedType.LPWStr)] string pwstrId, out IMMDevice ppDevice);

		[PreserveSig]
		int RegisterEndpointNotificationCallback(IMMNotificationClient pClient);

		[PreserveSig]
		int UnregisterEndpointNotificationCallback(IMMNotificationClient pClient);
	}

	[ComImport]
	[Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IMMNotificationClient
	{
		void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, int dwNewState);
		void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
		void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
		void OnDefaultDeviceChanged(int flow, int role, [MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);
		void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PROPERTYKEY key);
	}

	[ComImport]
	[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
	public class MMDeviceEnumeratorComObject { }
}
