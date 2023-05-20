using Azalea.Graphics.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Platform;

public interface IGameHost
{
    public IRenderer Renderer { get; }

    public void Run(AzaleaGame game);
}
