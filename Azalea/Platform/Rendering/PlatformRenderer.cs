using Azalea.Platform.Rendering.OpenGL;
using Azalea.Platform.Windowing;
using Azalea.Threading;

internal abstract class PlatformRenderer
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
