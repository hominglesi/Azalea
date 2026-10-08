using Azalea.Graphics;
using Azalea.Graphics.Textures;
using Azalea.Platform;
using Azalea.Platform.Loading;
using System;
using System.Threading.Tasks;

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

		var newTexture = GameHost.Instance.Loader.CreateTexture(data.Width, data.Height, data.Data, false);
		var texture = new Texture(newTexture);
		_textureCache.AddValue(store, path, texture);

		return texture;
	}

	public static Texture GetTextureAsync(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return cached;

		var nativeTexture = GameHost.Instance.Loader.CreateEmptyTexture();
		var texture = new Texture(nativeTexture);
		_textureCache.AddValue(store, path, texture);

		Task.Run(async () =>
		{
			var stream = store.GetStream(path);

			if (stream is null)
				return Task.CompletedTask;

			var image = Image.FromStream(stream);

			if (stream is null)
				return Task.CompletedTask;

			GameHost.Instance.Loader.CreateTexture(image.Width, image.Height, image.Data, false, nativeTexture);

			return Task.CompletedTask;
		});

		return texture;
	}
}
