using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Inputs;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Windows;
using System.Numerics;

namespace Azalea.Graphics.Vulkan;
internal class VulkanRenderer : Renderer
{
	private readonly VulkanController _controller;

	public VulkanRenderer(IWindow window)
		: base(window)
	{
		if (window is not Win32Window win32Window)
			throw new System.Exception("Cant create surface from non win32 windows");

		_controller = new VulkanController(win32Window.Handle);

		window.OnClientResized += (_) => _controller.SetFramebufferResized();
	}

	internal override void BeginFrame()
	{
		base.BeginFrame();

		_controller.BeginFrame();

		_controller.PushQuad(new Vector2(50.0f, 50.0f), new Vector2(100.0f, 50.0f), new Vector2(100.0f, 100.0f), new Vector2(50.0f, 100.0f));
		_controller.PushQuad(new Vector2(700.0f, 500.0f), new Vector2(900.0f, 500.0f), new Vector2(900.0f, 600.0f), new Vector2(700.0f, 600.0f));
		var mousePosition = Input.MousePosition;
		_controller.PushQuad(
			mousePosition + new Vector2(-25, -25),
			mousePosition + new Vector2(25, -25),
			mousePosition + new Vector2(25, 25),
			mousePosition + new Vector2(-25, 25));
	}

	internal override void FinishFrame()
	{
		base.FinishFrame();

		var clientSize = Window.ClientSize;
		var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, clientSize.X, 0, clientSize.Y, 0.1f, 10);
		_controller.SetProjectionMatrix(projectionMatrix);
		_controller.FinishFrame();
	}

	// Not Implemented
	protected override void ClearImplementation(Color color) { }

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new VulkanTexture(this, width, height);

	// Not Implemented
	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new VulkanVertexBatch<TexturedVertex2D>();

	// Not Implemented
	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle) { }

	// Not Implemented
	protected override void SetScissorTestState(bool enabled) { }

	// Not Implemented
	protected override bool SetTextureImplementation(INativeTexture? texture, int unit) => true;

	// Not Implemented
	protected override void SetViewportImplementation(Vector2Int size) { }

	internal override void Dispose()
	{
		base.Dispose();

		_controller.Destroy();
	}
}
