using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.IO;

namespace Azalea.IO.Tiled;
public readonly struct Tileset
{
	public string Name { get; init; }
	public int TileWidth { get; init; }
	public int TileHeight { get; init; }
	public int TileCount { get; init; }
	public Vector2Int TileSize { get; init; }
	public int Spacing { get; init; }
	public int Margin { get; init; }
	public Texture Source { get; init; }
	public Texture[] Tiles { get; init; }

	public static Tileset Load(IResourceStore store, string path)
	{
		var xml = store.GetXML(path)!;

		var tilesetNode = xml.DocumentElement!.SelectSingleNode("/tileset")!;
		var name = tilesetNode.GetAttribute("name");
		var tileCount = tilesetNode.GetIntAttribute("tilecount");
		var tileHeight = tilesetNode.GetIntAttribute("tileheight");
		var tileWidth = tilesetNode.GetIntAttribute("tilewidth");
		var tileSize = new Vector2Int(tileWidth, tileHeight);
		var spacing = 0;
		if (tilesetNode.ContainsAttribute("spacing"))
			spacing = tilesetNode.GetIntAttribute("spacing");

		var margin = 0;
		if (tilesetNode.ContainsAttribute("margin"))
			margin = tilesetNode.GetIntAttribute("margin");

		var imageNodes = tilesetNode.SelectNodes("image")!;
		var sources = new Image[imageNodes.Count];

		for (int i = 0; i < imageNodes.Count; i++)
		{
			var image = imageNodes[i]!;
			var imagePath = image.GetAttribute("source");
			var imageData = store.GetImage(PathUtils.CombinePaths(Path.GetDirectoryName(path)!, imagePath));

			sources[i] = imageData;
		}

		var tiles = new Texture[tileCount];
		var seekerStart = new Vector2Int(margin);
		var tileSeeker = seekerStart;

		int textureWidth = 10;
		int textureHeight = (int)MathF.Ceiling(tileCount / (float)textureWidth);
		var paddedSize = tileSize + new Vector2Int(2);
		var textureSize = new Vector2Int(textureWidth, textureHeight) * paddedSize;

		var textureSeeker = new Vector2Int(0, 0);

		var atlas = new Image(textureSize.X, textureSize.Y);

		// Add Tileset Texture data to our Texture

		for (int i = 0; i < tiles.Length; i++)
		{
			var source = sources[0];
			//Copy tile
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker, tileSize),
				new RectangleInt(textureSeeker + new Vector2Int(1, 1), tileSize));

			var horizontalSlice = new Vector2Int(tileSize.X, 1);
			var verticalSlice = new Vector2Int(1, tileSize.Y);
			var singlePixel = new Vector2Int(1, 1);

			//Copy tile top
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker, horizontalSlice),
				new RectangleInt(textureSeeker + new Vector2Int(1, 0), horizontalSlice));

			//Copy tile right
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker + new Vector2Int(tileSize.X - 1, 0), verticalSlice),
				new RectangleInt(textureSeeker + new Vector2Int(paddedSize.X - 1, 1), verticalSlice));

			//Copy tile bottom
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker + new Vector2Int(0, tileSize.Y - 1), horizontalSlice),
				new RectangleInt(textureSeeker + new Vector2Int(1, paddedSize.Y - 1), horizontalSlice));

			//Copy tile left
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker, verticalSlice),
				new RectangleInt(textureSeeker + new Vector2Int(0, 1), verticalSlice));

			//Copy tile top-left
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker, singlePixel),
				new RectangleInt(textureSeeker, singlePixel));

			//Copy tile top-right
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker + new Vector2Int(tileSize.X - 1, 0), singlePixel),
				new RectangleInt(textureSeeker + new Vector2Int(paddedSize.X - 1, 0), singlePixel));

			//Copy tile bottom-right
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker + tileSize - Vector2Int.One, singlePixel),
				new RectangleInt(textureSeeker + paddedSize - Vector2Int.One, singlePixel));

			//Copy tile bottom-left
			atlas.CopyFromSource(source,
				new RectangleInt(tileSeeker + new Vector2Int(0, tileSize.Y - 1), singlePixel),
				new RectangleInt(textureSeeker + new Vector2Int(0, paddedSize.Y - 1), singlePixel));

			tileSeeker.X += tileSize.X + spacing;
			if (tileSeeker.X + tileSize.X > source.Width)
			{
				tileSeeker.X = seekerStart.X;
				tileSeeker.Y += tileSize.Y + spacing;
			}

			textureSeeker.X += paddedSize.X;
			if (textureSeeker.X + paddedSize.X > textureSize.X)
			{
				textureSeeker.X = 0;
				textureSeeker.Y += paddedSize.Y;
			}
		}

		// Create our Texture
		var atlasTexture = Renderer.CreateTexture(atlas);

		// Create all texture regions

		textureSeeker = Vector2Int.Zero;
		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i] = new TextureRegion(atlasTexture, new RectangleInt(textureSeeker + Vector2Int.One, tileSize));

			textureSeeker.X += paddedSize.X;
			if (textureSeeker.X + paddedSize.X > textureSize.X)
			{
				textureSeeker.X = 0;
				textureSeeker.Y += paddedSize.Y;
			}
		}

		return new Tileset
		{
			Name = name,
			TileHeight = tileHeight,
			TileWidth = tileWidth,
			TileCount = tileCount,
			TileSize = tileSize,
			Spacing = spacing,
			Margin = margin,
			Source = atlasTexture,
			Tiles = tiles
		};
	}
}
