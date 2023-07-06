using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Tests.Rendering;

namespace Azalea.Tests.Platform;

internal class DummyGameHost : GameHost
{
    public override IWindow Window => throw new NotImplementedException();
    public override IRenderer Renderer => _renderer;
    private readonly DummyRenderer _renderer;

    public DummyGameHost()
    {
        _renderer = new DummyRenderer();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        CallInitialized();
        CallOnRender();
    }
}
