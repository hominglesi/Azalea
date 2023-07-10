using Azalea.Graphics.Textures;
using Azalea.IO.Stores;
using System;

namespace Azalea.IO.Assets;

public static partial class Assets
{
    public static IResourceStore<byte[]> Resources => _resources ?? throw new Exception("Game has not been initialized yet");
    private static IResourceStore<byte[]>? _resources;

    public static ITextureStore Textures => _textures ?? throw new Exception("Game has not been initialized yet");
    private static ITextureStore? _textures;

    internal static void InitializeAssets(AzaleaGame game)
    {
        _resources = game.Resources;
        _textures = game.Textures;
    }
}
