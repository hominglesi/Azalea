using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Windows;
using System.Numerics;

namespace Azalea.Graphics.Vulkan;
internal class VulkanRenderer : Renderer
{
	public readonly VulkanController Controller;

	public VulkanRenderer(IWindow window)
		: base(window)
	{
		if (window is not Win32Window win32Window)
			throw new System.Exception("Cant create surface from non win32 windows");

		Controller = new VulkanController(win32Window.Handle);

		window.OnClientResized += (_) => Controller.SetFramebufferResized();
	}

	internal override void BeginFrame()
	{
		base.BeginFrame();

		Controller.BeginFrame();

		/*
		Controller.PushQuad(new Vector2(50.0f, 50.0f), new Vector2(100.0f, 50.0f), new Vector2(100.0f, 100.0f), new Vector2(50.0f, 100.0f), 0);
		Controller.PushQuad(new Vector2(700.0f, 500.0f), new Vector2(900.0f, 500.0f), new Vector2(900.0f, 600.0f), new Vector2(700.0f, 600.0f), 0);
		var mousePosition = Input.MousePosition;
		Controller.PushQuad(
			mousePosition + new Vector2(-25, -25),
			mousePosition + new Vector2(25, -25),
			mousePosition + new Vector2(25, 25),
			mousePosition + new Vector2(-25, 25), 1);*/
	}

	internal override void FinishFrame()
	{
		base.FinishFrame();

		var clientSize = Window.ClientSize;
		var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, clientSize.X, 0, clientSize.Y, 0.1f, 10);
		Controller.SetProjectionMatrix(projectionMatrix);
		Controller.FinishFrame();
	}

	// Not Implemented
	protected override void ClearImplementation(Color color) { }

	protected override INativeTexture CreateNativeTexture(Image image)
		=> new VulkanTexture(this, image);

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

		Controller.Destroy();
	}
}
