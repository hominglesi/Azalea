using Azalea.Graphics.Rendering;
using Azalea.Platform;
using Azalea.Tests.Rendering;

namespace Azalea.Tests.Platform;

internal class DummyGameHost : GameHost
{
    public override IRenderer Renderer => _renderer;
    private readonly DummyRenderer _renderer;

    public override event Action? Initialized;
    public override event Action? OnRender;

    public DummyGameHost()
    {
        _renderer = new DummyRenderer();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        Initialized?.Invoke();
        OnRender?.Invoke();
    }
}
