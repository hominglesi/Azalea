using Azalea.Extentions;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Azalea.IO.Tiled;
public readonly struct Tilemap
{
	public string Orientation { get; init; }
	public string RenderOrder { get; init; }
	public int Width { get; init; }
	public int Height { get; init; }
	public Vector2 Size { get; init; }
	public int TileWidth { get; init; }
	public int TileHeight { get; init; }
	public Vector2 TileSize { get; init; }
	public Vector2 PixelSize { get; init; }
	public TilemapSource[] Tilesets { get; init; }
	public TileObject[] Objects { get; init; }
	public TilemapLayer[] Layers { get; init; }

	public Texture GetTextureById(int id)
	{
		foreach (var tileset in Tilesets)
			if (id >= tileset.FirstId && id < tileset.FirstId + tileset.Source.TileCount)
				return tileset.Source.Tiles[id - tileset.FirstId];

		throw new Exception($"Texture with id '{id}' was not found");
	}

	public static Tilemap Load(IResourceStore store, string path)
	{
		var xml = store.GetXML(path)!;
		var tilemapDirectory = Path.GetDirectoryName(path) ?? "";

		var tilemapNode = xml.DocumentElement!.SelectSingleNode("/map")!;
		var orientation = tilemapNode.GetAttribute("orientation");
		var renderOrder = tilemapNode.GetAttribute("renderorder");
		var width = tilemapNode.GetIntAttribute("width");
		var height = tilemapNode.GetIntAttribute("height");
		var tileWidth = tilemapNode.GetIntAttribute("tilewidth");
		var tileHeight = tilemapNode.GetIntAttribute("tileheight");

		var tilesetNodes = tilemapNode.GetNodes("tileset");
		var tilesets = new List<TilemapSource>();

		foreach (var tilesetNode in tilesetNodes)
		{
			var tilesetFirstId = tilesetNode.GetIntAttribute("firstgid");
			var tilesetSource = tilesetNode.GetAttribute("source");
			var tilesetPath = FileSystemUtils.CombinePaths(tilemapDirectory, tilesetSource);
			var tileset = new TilemapSource(tilesetFirstId, store.GetTileset(tilesetPath));
			tilesets.Add(tileset);
		}

		var layerNodes = tilemapNode.GetNodes("layer");
		var layers = new List<TilemapLayer>();

		foreach (var layerNode in layerNodes)
		{
			var layerId = layerNode.GetIntAttribute("id");
			var layerName = layerNode.GetAttribute("name");
			var dataNode = layerNode.GetNode("data");
			var stringData = dataNode.InnerText.Split(',');
			var data = new int[height, width];
			for (var i = 0; i < stringData.Length; i++)
			{
				var x = i % width;
				var y = i / width;
				data[y, x] = int.Parse(stringData[i]);
			}

			var layer = new TilemapLayer(layerId, layerName, data);
			layers.Add(layer);
		}

		var objectGroupNodes = tilemapNode.GetNodes("objectgroup");
		var objs = new List<TileObject>();

		foreach (var objectGroupNode in objectGroupNodes)
		{
			var objectNodes = objectGroupNode.GetNodes("object");
			foreach (var objectNode in objectNodes)
			{
				var obj = TileObject.Parse(objectNode);

				if (obj.Template != "")
				{
					var templatePath = FileSystemUtils.CombinePaths(tilemapDirectory, obj.Template);
					var template = store.GetTemplate(templatePath);
					obj = TileObject.ImplementTemplate(obj, template);
				}

				objs.Add(obj);
			}
		}

		return new Tilemap()
		{
			Orientation = orientation,
			RenderOrder = renderOrder,
			Width = width,
			Height = height,
			Size = new(width, height),
			TileWidth = tileWidth,
			TileHeight = tileHeight,
			TileSize = new(tileWidth, tileHeight),
			PixelSize = new(width * tileWidth, height * tileHeight),
			Tilesets = tilesets.ToArray(),
			Objects = objs.ToArray(),
			Layers = layers.ToArray()
		};
	}

	public readonly struct TilemapSource
	{
		public int FirstId { get; init; }
		public Tileset Source { get; init; }

		public TilemapSource(int firstId, Tileset source)
		{
			FirstId = firstId;
			Source = source;
		}
	}

	public readonly struct TilemapLayer
	{
		public int Id { get; init; }
		public string Name { get; init; }
		public int[,] Data { get; init; }

		public TilemapLayer(int id, string name, int[,] data)
		{
			Id = id;
			Name = name;
			Data = data;
		}
	}
}
