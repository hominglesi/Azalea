using Azalea.IO.Tiled;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Tileset> _tilesetCache = new();
	public static Tileset GetTileset(this IResourceStore store, string path)
	{
		if (_tilesetCache.TryGetValue(store, path, out var cached))
			return cached;

		var tileset = Tileset.Load(store, path);
		_tilesetCache.AddValue(store, path, tileset);

		return tileset;
	}
}
