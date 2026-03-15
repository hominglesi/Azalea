using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Azalea.Sounds.OpenAL;
internal unsafe partial class ALDevice : Disposable
{
	public readonly IntPtr Handle;

	private ALDevice(IntPtr handle)
	{
		Handle = handle;
	}

	public void Reopen(string? newDeviceName, int[] attibutes)
	{
		_alcReopenDeviceSOFT ??= Marshal.GetDelegateForFunctionPointer<ReopenDeviceDelegate>(alcGetProcAddress(Handle, "alcReopenDeviceSOFT"));

		_alcReopenDeviceSOFT.Invoke(Handle, newDeviceName, attibutes);
	}

	public bool PollConnected()
	{
		int connected;
		alcGetIntegerv(Handle, DeviceParameter.Connected, 1, &connected);
		return connected != 0;
	}

	public int GetFrequency()
	{
		int frequency;
		alcGetIntegerv(Handle, DeviceParameter.Frequency, 1, &frequency);
		return frequency;
	}

	public bool GetHRTF()
	{
		int hrtf;
		alcGetIntegerv(Handle, DeviceParameter.HRTFSoft, 1, &hrtf);
		return hrtf != 0;
	}

	public string GetDeviceName()
	{
		var str = alcGetString(Handle, DeviceParameter.AllDevicesSpecifier);
		return Marshal.PtrToStringAnsi(str)!;
	}

	protected override void OnDispose()
	{
		alcCloseDevice(Handle);
	}

	public static ALDevice OpenDefaultDevice()
	{
		var handle = alcOpenDevice(null);
		return new ALDevice(handle);
	}

	public static string GetDefaultDeviceName()
	{
		var str = alcGetString(IntPtr.Zero, DeviceParameter.DefaultAllDevicesSpecifier);
		return Marshal.PtrToStringAnsi(str)!;
	}

	public static IEnumerable<string> EnumerateDevices()
	{
		var deviceList = alcGetString(IntPtr.Zero, DeviceParameter.AllDevicesSpecifier);

		string? deviceName;
		while (string.IsNullOrEmpty(deviceName = Marshal.PtrToStringAnsi(deviceList)) == false)
		{
			yield return deviceName;
			deviceList += deviceName.Length + 1;
		}
	}

	enum DeviceParameter : int
	{
		DefaultDeviceSpecifier = 0x1004,
		DeviceSpecifier = 0x1005,
		Frequency = 0x1007,
		DefaultAllDevicesSpecifier = 0x1012,
		AllDevicesSpecifier = 0x1013,
		HRTFSoft = 0x1992,
		Connected = 0x313
	}

	[LibraryImport(ALC.LibraryPath)]
	[return: MarshalAs(UnmanagedType.U1)]
	private static partial bool alcCloseDevice(IntPtr device);

	[LibraryImport(ALC.LibraryPath)]
	private static partial void alcGetIntegerv(IntPtr device, DeviceParameter param, int size, int* value);

	[LibraryImport(ALC.LibraryPath, StringMarshalling = StringMarshalling.Utf8)]
	private static partial IntPtr alcGetProcAddress(IntPtr device, string functionName);

	[LibraryImport(ALC.LibraryPath)]
	private static partial IntPtr alcGetString(IntPtr device, DeviceParameter param);

	[LibraryImport(ALC.LibraryPath, StringMarshalling = StringMarshalling.Utf8)]
	private static partial IntPtr alcOpenDevice(string? deviceName);

	private delegate bool ReopenDeviceDelegate(IntPtr device, [MarshalAs(UnmanagedType.LPStr)] string? deviceName, int[] attributes);
	private ReopenDeviceDelegate? _alcReopenDeviceSOFT;
}
