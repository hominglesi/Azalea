using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Threading;
using System;
using System.Threading;

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

		var texture = Renderer.CreateTexture(data);
		texture.SetFiltering(filtering, filtering);
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

		Scheduler.Run(() =>
		{
			var image = store.GetImagePromise(path);

			while (image.IsResolved == false)
				Thread.Sleep(5);

			if (image.Value is null)
			{
				promise.Resolve(Assets.MissingTexture);
				return;
			}

			Scheduler.Schedule(() =>
			{
				var texture = Renderer.CreateTexture(image.Value);
				texture.SetFiltering(filtering, filtering);
				_textureCache.AddValue(store, path, texture);
				promise.Resolve(texture);
			});
		});

		return result;
	}

	public static PromisedTexture GetTextureAsync(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		var promise = GetTexturePromise(store, path, filtering);

		return new PromisedTexture(promise);
	}
}
