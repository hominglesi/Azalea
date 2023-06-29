using Azalea.Graphics.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.IO.Assets;

public static partial class Assets
{ 
    public static Texture GetTexture(string path)
    {
        if (RESOURCE_STORE.ContainsKey(path) == false) loadTexture(path);

        return (Texture)RESOURCE_STORE[path];
    }

    private static void loadTexture(string path)
    {
        using var stream = File.OpenRead(path);

        Debug.Assert(RENDERER != null);
        var texture = Texture.FromStream(RENDERER, stream) 
            ?? throw new Exception("Could not load texture at " + path);

        RESOURCE_STORE.Add(path, texture);
    }
}

