using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Platform;
using Azalea.Platform.Loading;
using Azalea.Platform.Loading.OpenGL;
using Azalea.Platform.Rendering.OpenGL;
using Azalea.Threading;
using System;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static readonly ResourceCache<ITexture> _textureCache = new();

	public static ITexture GetTexture(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return cached;

		var data = store.GetImage(path);
		if (data is null)
			return Assets.MissingTexture ?? throw new Exception("Texture could not be found.");

		var newTexture = GameHost.Instance.Loader.CreateTexture(data.Width, data.Height, data.Data, false);
		var texture = new Texture(newTexture);
		_textureCache.AddValue(store, path, texture);

		return texture;
	}

	private static readonly ResourceCache<ValuePromise<ITexture>> _texturePromiseCache = new();

	public static ValuePromise<ITexture> GetTexturePromise(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return new ValuePromise<ITexture>(cached);

		if (_texturePromiseCache.TryGetValue(store, path, out var cachedPromise))
			return cachedPromise;

		var promise = new Promise<ITexture>();
		var result = new ValuePromise<ITexture>(promise);

		_texturePromiseCache.AddValue(store, path, result);

		Scheduler.Run(async () =>
		{
			var image = await store.GetImagePromise(path);

			if (image is null)
			{
				promise.Resolve(Assets.MissingTexture);
				return;
			}

			var newTexture = GameHost.Instance.Loader.CreateTexture(image.Width, image.Height, image.Data, false);
			var texture = new Texture(newTexture);
			_textureCache.AddValue(store, path, texture);
			promise.Resolve(texture);
		});

		return result;
	}

	public static PromisedTexture GetTextureAsync(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		var promise = GetTexturePromise(store, path, filtering);

		return new PromisedTexture(promise);
	}
}
