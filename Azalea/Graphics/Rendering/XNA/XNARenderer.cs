using Azalea.Platform.XNA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.Rendering.XNA;

internal class XNARenderer : Renderer
{
    private readonly GameWrapper _gameWrapper;

    public XNARenderer(GameWrapper gameWrapper)
    {
        _gameWrapper = gameWrapper;
    }

    protected override void ClearImplementation(Color color)
    {
        _gameWrapper.GraphicsDevice.Clear(color.ToXNAColor());
    }
}
