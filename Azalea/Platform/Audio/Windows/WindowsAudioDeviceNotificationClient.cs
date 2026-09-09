using Azalea.Native.Windows;
using System;
using System.Runtime.InteropServices;

namespace Azalea.Platform.Audio.Windows;
internal class WindowsAudioDeviceNotificationClient : Win32.IMMNotificationClient, IAudioDeviceNotificationClient
{
	private readonly Win32.IMMDeviceEnumerator _deviceEnumerator;

	public WindowsAudioDeviceNotificationClient()
	{
		_deviceEnumerator = (Win32.IMMDeviceEnumerator)new Win32.MMDeviceEnumerator();
		_deviceEnumerator.RegisterEndpointNotificationCallback(this);
	}

	public event Action? DefaultDeviceChanged;

	public void OnDefaultDeviceChanged([In] int dataFlow, [In] int role, [In, MarshalAs(UnmanagedType.LPWStr)] string deviceId)
	{
		// dataFlow 0 = Output Devices
		// role 1 = Multimedia

		if (dataFlow == 0 && role == 1)
			DefaultDeviceChanged?.Invoke();
	}

	public void OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, int newState) { }
	public void OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId) { }
	public void OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId) { }
	public void OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, Win32.PROPERTYKEY key) { }
}
