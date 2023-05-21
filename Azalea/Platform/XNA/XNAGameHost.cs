using Azalea.Graphics.Rendering;
using Azalea.Graphics.XNA;
using Azalea.Graphics.XNA.Batches;
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

    XNAVertexBatch tempBatch;
    private void onInitialized()
    {
        Initialized?.Invoke();
        tempBatch = new XNAVertexBatch(_renderer, _gameWrapper);
    }

    private void onDraw(GameTime gameTime)
    {
        Renderer.Clear();
        tempBatch.DrawRectangle(200, 300, 100, 150, Graphics.Color.Azalea);
        tempBatch.Draw();
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _gameWrapper.Run();
    }
}
