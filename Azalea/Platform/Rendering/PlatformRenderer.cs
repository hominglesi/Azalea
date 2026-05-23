using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Windowing;

internal abstract class PlatformRenderer
{
	protected PlatformRenderer(PlatformDeviceContext deviceContext)
	{

	}

	public static PlatformRenderer AttachRenderer(PlatformWindow window)
	{
		var deviceContext = window.BorrowDeviceContext();

		// For now OpenGL is hardcoded
		var renderer = new GLRenderer(deviceContext);
		return renderer;
	}
}
