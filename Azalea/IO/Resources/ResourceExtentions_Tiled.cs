using Azalea.Extentions;
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

	private static ResourceCache<Tilemap> _tilemapCache = new();
	public static Tilemap GetTilemap(this IResourceStore store, string path)
	{
		if (_tilemapCache.TryGetValue(store, path, out var cached))
			return cached;

		var tilemap = Tilemap.Load(store, path);
		_tilemapCache.AddValue(store, path, tilemap);

		return tilemap;
	}

	private static ResourceCache<TileObject> _templateCache = new();
	internal static TileObject GetTemplate(this IResourceStore store, string path)
	{
		if (_templateCache.TryGetValue(store, path, out var cached))
			return cached;

		var xml = store.GetXML(path);
		var objectNode = xml!.DocumentElement!.SelectSingleNode("/template")!.GetNode("object");

		var template = TileObject.Parse(objectNode);
		_templateCache.AddValue(store, path, template);

		return template;
	}
}
