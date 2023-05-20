using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.Rendering;

public interface IRenderer
{
    public Color ClearColor { get; set; }

    internal void Clear();
}
