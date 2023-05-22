using Azalea.Graphics.Rendering;
using Azalea.Graphics.XNA;
using Microsoft.Xna.Framework;

namespace Azalea.Platform.XNA;

internal class XNAGameHost : GameHost
{
    public override IRenderer Renderer => _renderer;
    private readonly XNARenderer _renderer;

    private readonly GameWrapper _gameWrapper;

    public override event Action? Initialized;
    public override event Action? OnRender;

    public XNAGameHost()
    {
        _gameWrapper = new GameWrapper();
        _gameWrapper.OnInitialize += onInitialized;
        _gameWrapper.OnDraw += onDraw;

        _renderer = new XNARenderer(_gameWrapper);
    }

    private void onInitialized()
    {
        Renderer.Initialize();
        Initialized?.Invoke();
    }

    private void onDraw(GameTime gameTime)
    {
        OnRender?.Invoke();
        Renderer.FlushCurrentBatch();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _gameWrapper.Run();
    }
}
