using Azalea.Native.Windows;
using Azalea.Utils;

namespace Azalea.Platform.Windowing.Windows;
internal class WindowsDeviceContext : IPlatformDeviceContext
{
	public nint Handle { get; private init; }
	public ReadOnlyObservable<Vector2Int> ClientSize { get; }

	internal WindowsDeviceContext(nint handle, ReadOnlyObservable<Vector2Int> clientSize)
	{
		Handle = handle;
		ClientSize = clientSize;
	}

	public void SetDefaultPixelFormat()
	{
		var descriptor = new Win32.PIXELFORMATDESCRIPTOR();
		var pixelFormat = Win32.ChoosePixelFormat(Handle, in descriptor);

		Win32.SetPixelFormat(Handle, pixelFormat, in descriptor);
	}
}
