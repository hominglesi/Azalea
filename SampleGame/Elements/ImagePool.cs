using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SampleGame.Elements;

public class ImagePool
{
	private readonly string[] _images;
	private IResourceStore _store;
	public ImagePool(IResourceStore tilesStore)
	{
		_store = tilesStore;
		_images = tilesStore.GetAvalibleResources().ToArray();
	}

	public List<Texture> GenerateTiles(int count)
	{
		if (count % 2 != 0) throw new ArgumentException($"There must be an even number of tiles");
		if (count / 2 > _images.Length) throw new ArgumentException($"There are not enough images to generate enough tiles");

		count /= 2;

		var remainingImages = new List<string>(_images);
		var tiles = new List<Texture>();

		while (count > 0)
		{
			var tile = remainingImages.Random();
			var tileTexture = _store.GetTexture(tile);
			Debug.Assert(tileTexture != null);

			tileTexture.AssetName = tile;
			remainingImages.Remove(tile);
			tiles.Add(tileTexture);
			tiles.Add(tileTexture);

			count--;
		}

		return tiles;
	}
}
