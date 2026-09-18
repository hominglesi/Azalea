using Azalea.Native.Windows;
using Azalea.Utils;
using System;

namespace Azalea.Platform.Windowing.Windows;
internal partial class WindowsDeviceContext : IPlatformDeviceContext
{
	public nint Handle { get; private init; }
	public Vector2Int ClientSize { get; private set; }
	public event Action<Vector2Int> OnClientSizeChanged;

	internal WindowsDeviceContext(nint handle, Vector2Int clientSize)
	{
		Handle = handle;
		ClientSize = clientSize;
	}

	public void UpdateClientSize(Vector2Int clientSize)
	{
		ClientSize = clientSize;
		OnClientSizeChanged.Invoke(clientSize);
	}

	public void SetDefaultPixelFormat()
	{
		var descriptor = new Win32.PIXELFORMATDESCRIPTOR();
		var pixelFormat = Win32.ChoosePixelFormat(Handle, in descriptor);

		Win32.SetPixelFormat(Handle, pixelFormat, in descriptor);
	}
}
