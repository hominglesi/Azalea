using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Threading;
using System;
using System.Threading;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static readonly ResourceCache<Texture> _textureCache = new();

	public static Texture GetTexture(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
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

	private static readonly ResourceCache<Promise<Texture>> _texturePromiseCache = new();

	public static Promise<Texture> GetTexturePromise(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return new Promise<Texture>(cached);

		if (_texturePromiseCache.TryGetValue(store, path, out var cachedPromise))
			return cachedPromise;

		var promise = new Promise<Texture>(() =>
		{
			Texture result = Assets.MissingTexture;

			var imagePromise = store.GetImagePromise(path).Then(image =>
			{
				if (image is null)
					return;

				result = Renderer.CreateTexture(image);
				result.SetFiltering(filtering, filtering);
				_textureCache.AddValue(store, path, result);
			});

			while (imagePromise.IsResolved == false)
				Thread.Sleep(5);

			return result;
		});

		return promise;
	}
}
