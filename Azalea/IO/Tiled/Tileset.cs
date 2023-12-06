using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using System;
using System.IO;

namespace Azalea.IO.Tiled;
public struct Tileset
{
	public string Name { get; init; }
	public int TileWidth { get; init; }
	public int TileHeight { get; init; }
	public int TileCount { get; init; }
	public Vector2Int TileSize { get; init; }
	public Texture[] Sources { get; init; }
	public Texture[] Tiles { get; init; }

	public static Tileset Load(IResourceStore store, string path)
	{
		var xml = store.GetXML(path)!;

		var tilesetNode = xml.DocumentElement!.SelectSingleNode("/tileset")!;
		var name = tilesetNode.Attributes!["name"]!.Value;
		var tileHeight = Convert.ToInt32(tilesetNode.Attributes!["tileheight"]!.Value);
		var tileWidth = Convert.ToInt32(tilesetNode.Attributes!["tilewidth"]!.Value);
		var tileSize = new Vector2Int(tileWidth, tileHeight);
		var tileCount = Convert.ToInt32(tilesetNode.Attributes!["tilecount"]!.Value);

		var imageNodes = tilesetNode.SelectNodes("image")!;
		var sources = new Texture[imageNodes.Count];

		for (int i = 0; i < imageNodes.Count; i++)
		{
			var image = imageNodes[i]!;
			var imagePath = image.Attributes!["source"]!.Value;
			sources[i] = store.GetTexture(Path.Join(Path.GetDirectoryName(path), imagePath));
		}

		var tiles = new Texture[tileCount];
		var tileSeeker = Vector2Int.Zero;

		for (int i = 0; i < tiles.Length; i++)
		{
			var source = sources[0];
			tiles[i] = new TextureRegion(source, new RectangleInt(tileSeeker, tileSize));

			tileSeeker.X += tileSize.X;
			if (tileSeeker.X + tileSize.X > source.Width)
			{
				tileSeeker.X = 0;
				tileSeeker.Y += tileSize.Y;
			}
		}

		var output = new Tileset
		{
			Name = name,
			TileHeight = tileHeight,
			TileWidth = tileWidth,
			TileCount = tileCount,
			TileSize = tileSize,
			Sources = sources,
			Tiles = tiles
		};

		return output;
	}
}
