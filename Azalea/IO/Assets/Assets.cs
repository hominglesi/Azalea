using Azalea.Graphics.Rendering;
using System.Collections.Generic;

namespace Azalea.IO.Assets;

public static partial class Assets
{
    private static Dictionary<string, object> RESOURCE_STORE = new();

    internal static IRenderer? RENDERER;
}
