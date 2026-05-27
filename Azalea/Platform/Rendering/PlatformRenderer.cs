using Azalea.Graphics.Colors;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Windowing;
using Azalea.Threading;

public abstract class PlatformRenderer
{
	protected PlatformRenderer()
	{
		_thread = new RenderThread(this);
		_thread.Start();
	}

	protected abstract void Initialize();
	protected abstract void Update();

	public static PlatformRenderer AttachRenderer(PlatformWindow window)
	{
		var deviceContext = window.BorrowDeviceContext();

		// For now OpenGL is hardcoded
		var renderer = new GLRenderer(deviceContext);
		return renderer;
	}

	public abstract void Clear(Color color);


	#region Framebuffer

	public abstract Framebuffer CreateFramebuffer();

	protected Framebuffer? BoundFramebuffer { get; private set; } = null;
	protected abstract void BindFramebufferImplementation(Framebuffer? framebuffer);
	public void BindFramebuffer(Framebuffer? framebuffer)
	{
		if (framebuffer == BoundFramebuffer)
			return;

		BindFramebufferImplementation(framebuffer);
		BoundFramebuffer = framebuffer;
	}

	#endregion

	#region Texture2D

	public abstract Texture2D CreateTexture2D();

	protected Texture2D? BoundTexture2D { get; private set; } = null;
	public abstract void BindTexture2DImplementation(Texture2D texture2D);
	public void BindTexture2D(Texture2D texture2D)
	{
		if (texture2D == BoundTexture2D)
			return;

		BindTexture2DImplementation(texture2D);
		BoundTexture2D = texture2D;
	}

	#endregion

	#region Thread

	private readonly RenderThread _thread;

	class RenderThread(PlatformRenderer renderer) : GameThread(1)
	{
		public override string DisplayName => "Rendering Thread";

		private readonly PlatformRenderer _renderer = renderer;

		protected override void Initialize()
		{
			_renderer.Initialize();
		}

		protected override void Update()
		{
			_renderer.Update();
		}
	}

	#endregion
}
