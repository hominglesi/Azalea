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

    public XNAGameHost()
    {
        _gameWrapper = new GameWrapper();
        _gameWrapper.OnInitialize += onInitialized;
        _gameWrapper.OnDraw += onDraw;

        _renderer = new XNARenderer(_gameWrapper);
    }

    private void onInitialized()
    {
        ((IRenderer)_renderer).Initialize();
        Initialized?.Invoke();
    }

    private void onDraw(GameTime gameTime)
    {
        Renderer.Clear();
        Renderer.DrawQuad(new System.Numerics.Vector2(200, 300), new System.Numerics.Vector2(100, 150), Graphics.Color.Azalea);
        Renderer.DrawQuad(new System.Numerics.Vector2(500, 100), new System.Numerics.Vector2(200, 50), Graphics.Color.Azalea);
        Renderer.FlushCurrentBatch();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _gameWrapper.Run();
    }
}
