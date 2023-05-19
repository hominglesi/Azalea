using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform.XNA;

internal class XNAGameHost : GameHost
{
    private readonly GameWrapper _gameWrapper;

    public XNAGameHost()
    {
        _gameWrapper = new GameWrapper();
    }

    public override void Run(AzaleaGame game)
    {
        _gameWrapper.Run();
        base.Run(game);
    }
}
