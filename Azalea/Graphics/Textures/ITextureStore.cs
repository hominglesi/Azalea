using Azalea.IO.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azalea.Graphics.Textures;

public interface ITextureStore : IResourceStore<Texture>
{
    new Texture? Get(string name);
}

public static class ResourceStoreExtentions
{
    private static readonly string[] system_filename_ignore_list =
    {
        // Mac-specific
        "__MACOSX",
        ".DS_Store",
        // Windows-specific
        "Thumbs.db"
    };

    public static IEnumerable<string> ExcludeSystemFileNames(this IEnumerable<string> source) =>
        source.Where(entry => !system_filename_ignore_list.Any(ignoredName => entry.Contains(ignoredName, StringComparison.OrdinalIgnoreCase)));
}
