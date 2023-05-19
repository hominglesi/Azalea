using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform;

internal abstract class GameHost : IGameHost
{
    public virtual void Run(AzaleaGame game)
    {
        game.SetHost(this);
    }
}
