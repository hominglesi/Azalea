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

	private static readonly ResourceCache<ValuePromise<Image?>> _imagePromiseCache = new();

	public static ValuePromise<Image?> GetImagePromise(this IResourceStore store, string path)
	{
		if (_imageCache.TryGetValue(store, path, out var cached))
			return new ValuePromise<Image?>(cached);

		if (_imagePromiseCache.TryGetValue(store, path, out var cachedPromise))
			return cachedPromise;

		var promise = new Promise<Image?>();
		var result = new ValuePromise<Image?>(promise);
		_imagePromiseCache.AddValue(store, path, result);

		Scheduler.Run(() =>
		{
			var stream = store.GetStream(path);

			if (stream is null)
			{
				promise.Resolve(null);
				return;
			}

			var image = Image.FromStream(stream);

			_imageCache.AddValue(store, path, image);
			promise.Resolve(image);
		});

		return result;
	}
}
