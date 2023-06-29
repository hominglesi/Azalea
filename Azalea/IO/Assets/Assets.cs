using Azalea.Graphics.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.IO.Assets;

public static partial class Assets
{
    private static Dictionary<string, object> RESOURCE_STORE = new();

    internal static IRenderer? RENDERER;
}
