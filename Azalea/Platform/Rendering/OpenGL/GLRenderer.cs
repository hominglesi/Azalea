using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Platform.Windowing;

namespace Azalea.Platform.Rendering.OpenGL;
internal partial class GLRenderer : PlatformRenderer
{
	private readonly PlatformDeviceContext _deviceContext;

	internal GLRenderer(PlatformDeviceContext deviceContext)
	{
		_deviceContext = deviceContext;
	}

	protected override void Initialize()
	{
		assureGLInitialized();

		var context = GLContext.Create(_deviceContext);
		context.MakeCurrent();

		GL.ClearColor(Palette.Aqua);
		GL.Clear(GLBufferBit.Color);
		context.SwapBuffers();
	}

	protected override void Update()
	{

	}
}
