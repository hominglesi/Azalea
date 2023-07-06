using Azalea.Graphics.Rendering;
using Azalea.Graphics.XNA;

namespace Azalea.Platform.XNA;

internal class XNAGameHost : GameHost
{
    public override IWindow Window => throw new System.NotImplementedException();
    public override IRenderer Renderer => _renderer;
    private readonly XNARenderer _renderer;

    private readonly GameWrapper _gameWrapper;

    public XNAGameHost()
    {
        _gameWrapper = new GameWrapper();
        _gameWrapper.OnInitialize += CallInitialized;
        _gameWrapper.OnDraw += (_) => CallOnRender();
        _gameWrapper.OnUpdate += (_) => CallOnUpdate();

        _renderer = new XNARenderer(_gameWrapper);
    }

    public override void Run(AzaleaGame game)
    {
        base.Run(game);
        _gameWrapper.Run();
    }
}
