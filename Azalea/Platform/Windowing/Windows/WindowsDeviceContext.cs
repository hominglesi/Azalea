namespace Azalea.Platform.Windowing.Windows;
internal class WindowsDeviceContext : PlatformDeviceContext
{
	public nint Handle { get; private init; }

	internal WindowsDeviceContext(nint handle)
	{
		Handle = handle;
	}
}
