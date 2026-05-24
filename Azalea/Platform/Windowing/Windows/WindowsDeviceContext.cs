using Azalea.Native.Windows;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsDeviceContext : PlatformDeviceContext
{
	public nint Handle { get; private init; }

	internal WindowsDeviceContext(nint handle)
	{
		Handle = handle;
	}

	public void SetDefaultPixelFormat()
	{
		var descriptor = new Win32.PIXELFORMATDESCRIPTOR();
		var pixelFormat = Win32.ChoosePixelFormat(Handle, in descriptor);

		Win32.SetPixelFormat(Handle, pixelFormat, in descriptor);
	}
}
