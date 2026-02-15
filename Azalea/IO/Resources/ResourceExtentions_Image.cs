using Azalea.Graphics;
using Azalea.Threading;

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

	private static readonly ResourceCache<Promise<Image?>> _imagePromiseCache = new();

	public static Promise<Image?> GetImagePromise(this IResourceStore store, string path)
	{
		if (_imageCache.TryGetValue(store, path, out var cached))
			return new Promise<Image?>(cached);

		if (_imagePromiseCache.TryGetValue(store, path, out var cachedPromise))
			return cachedPromise;

		var promise = new Promise<Image?>(() =>
		{
			var stream = store.GetStream(path);

			if (stream is null)
				return null;

			return Image.FromStream(stream);
		}).Then(image =>
		{
			_imageCache.AddValue(store, path, image);
		});

		return promise;
	}
}
