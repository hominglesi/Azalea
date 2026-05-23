using Azalea.Platform.Windowing;
using Azalea.Platform.Windowing.Windows;
using System;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	internal GLRenderer(PlatformDeviceContext deviceContext)
		: base(deviceContext)
	{
		assureInitialized();

		if (deviceContext is WindowsDeviceContext winDC)
			Console.WriteLine("Created windows dc: " + winDC.Handle);
	}
}
