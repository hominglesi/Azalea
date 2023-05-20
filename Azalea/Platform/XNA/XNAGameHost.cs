using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.XNA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform.XNA;

internal class XNAGameHost : GameHost
{
    public override IRenderer Renderer => _renderer;
    private readonly XNARenderer _renderer;

    private readonly GameWrapper _gameWrapper;

    public XNAGameHost()
    {
        _gameWrapper = new GameWrapper();

        _renderer = new XNARenderer(_gameWrapper);
    }

    public override void Run(AzaleaGame game)
    {
        _gameWrapper.Run();
        base.Run(game);
    }
}
