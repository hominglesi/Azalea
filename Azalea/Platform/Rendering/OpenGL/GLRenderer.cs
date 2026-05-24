using Azalea.Platform.Windowing;
using Azalea.Utils;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private readonly PlatformDeviceContext _deviceContext;
	private GLContext _context;

	internal GLRenderer(PlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;
	}

	protected override void Initialize()
	{
		assureGLInitialized();

		_context = GLContext.Create(_deviceContext);
		_context.MakeCurrent();
	}

	protected override void Update()
	{
		_context.ClearColor(Rng.Color());
		_context.Clear();
		_context.SwapBuffers();
	}
}
