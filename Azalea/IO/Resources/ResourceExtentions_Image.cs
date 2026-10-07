using Azalea.Graphics;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static readonly ResourceCache<Image?> _imageCache = new();

	public static Image? GetImage(this IResourceStore store, string path)
	{
		if (_imageCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path);

		if (stream is null)
		{
			_imageCache.AddValue(store, path, null);
			return null;
		}

		var image = Image.FromStream(stream);
		_imageCache.AddValue(store, path, image);

		return image;
	}
}
