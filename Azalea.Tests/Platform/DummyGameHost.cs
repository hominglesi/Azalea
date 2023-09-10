using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Tests.Rendering;

namespace Azalea.Tests.Platform;

internal class DummyGameHost : GameHost
{
	public override IWindow Window => _window;
	private readonly IWindow _window;
	public override IRenderer Renderer => _renderer;
	private readonly DummyRenderer _renderer;

	public DummyGameHost()
	{
		_renderer = new DummyRenderer();
		_window = new DummyWindow();
	}

	public override void Run(AzaleaGame game)
	{
		base.Run(game);
		CallInitialized();
		CallOnRender();
	}
}
