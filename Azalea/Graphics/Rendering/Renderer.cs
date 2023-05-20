using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
    public Color ClearColor { get; set; }

    public void Clear()
    {
        ClearImplementation(ClearColor);
    }

    protected abstract void ClearImplementation(Color color);
}
