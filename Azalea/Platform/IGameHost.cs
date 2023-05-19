using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform;

public interface IGameHost
{
    public void Run(AzaleaGame game);
}
