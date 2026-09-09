using System.Runtime.InteropServices;

namespace Azalea.Native.OpenAL;
public static partial class ALC
{
	private const string SoftOalPath = "soft_oal.dll";

	public const int FALSE = 0;
	public const int FREQUENCY = 0x1007;
	public const int CONNECTED = 0x313;
	public const int MONO_SOURCES = 0x1010;
	public const int STEREO_SOURCES = 0x1011;
	public const int EXT_CAPTURE = 1;
	public const int CAPTURE_DEVICE_SPECIFIER = 0x310;
	public const int CAPTURE_DEFAULT_DEVICE_SPECIFIER = 0x311;
	public const int CAPTURE_SAMPLES = 0x312;
	public const int ENUMERATE_ALL_EXT = 1;
	public const int DEFAULT_ALL_DEVICES_SPECIFIER = 0x1012;
	public const int ALL_DEVICES_SPECIFIER = 0x1013;
	public const int HRTF_SOFT = 0x1992;

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcCloseDevice">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcCloseDevice")]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool CloseDevice(nint device);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcCreateContext">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcCreateContext")]
	public static partial nint CreateContext(nint device, ref int attrlist);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcGetIntegerv">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcGetIntegerv")]
	public static partial void GetIntegerv(nint device, int param, int size, ref int value);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcGetProcAddress">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcGetProcAddress", StringMarshalling = StringMarshalling.Utf8)]
	public static partial nint GetProcAddress(nint device, string funcName);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcGetString">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcGetString")]
	public static partial nint GetString(nint device, int param);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcMakeContextCurrent">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcMakeContextCurrent")]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool MakeContextCurrent(nint context);

	/// <summary><see href="https://github.com/kcat/openal-soft/wiki/Programmer's-Guide#alcOpenDevice">Official Documentation</see></summary>
	[LibraryImport(SoftOalPath, EntryPoint = "alcOpenDevice", StringMarshalling = StringMarshalling.Utf8)]
	public static partial nint OpenDevice(string? deviceName);

	private delegate bool ReopenDeviceSOFTDelegate(nint device, [MarshalAs(UnmanagedType.LPStr)] string? deviceName, ref int attribs);
	private static ReopenDeviceSOFTDelegate? __ReopenDeviceSOFTDelegate;
	/// <summary><see href="https://registry.khronos.org/OpenGL/extensions/EXT/WGL_EXT_swap_control.txt">Official Documentation</see></summary>
	public static bool ReopenDeviceSOFT(nint device, string? deviceName, ref int attribs) => __ReopenDeviceSOFTDelegate!(device, deviceName, ref attribs);

	public static bool DynamicFunctionsLoaded { get; private set; } = false;
	/// <summary> A valid OpenGL context must be current before calling this method </summary>
	public static void LoadDynamicFunctions(Func<nint, string, nint> getProcAddressMethod, nint device)
	{
		__ReopenDeviceSOFTDelegate = Marshal.GetDelegateForFunctionPointer<ReopenDeviceSOFTDelegate>(getProcAddressMethod(device, "alcReopenDeviceSOFT"));
		DynamicFunctionsLoaded = true;
	}
}
