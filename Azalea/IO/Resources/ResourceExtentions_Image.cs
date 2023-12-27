using Azalea.Graphics;
using System;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Image> _imageCache = new();

	public static Image GetImage(this IResourceStore store, string path)
	{
		if (_imageCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Texture could not be found.");

		var image = Image.FromStream(stream);
		_imageCache.AddValue(store, path, image);

		return image;
	}
}
