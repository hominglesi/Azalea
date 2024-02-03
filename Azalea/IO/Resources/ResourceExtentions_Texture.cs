using Azalea.Graphics.Textures;
using System;

namespace Azalea.IO.Resources;

public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Texture> _textureCache = new();

	public static Texture GetTexture(this IResourceStore store, string path, TextureFiltering filtering = TextureFiltering.Nearest)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path);

		if (stream == null)
			return Assets.MissingTexture ?? throw new Exception("Texture could not be found.");

		var data = store.GetImage(path);
		var texture = AzaleaGame.Main.Host.Renderer.CreateTexture(data);
		texture.SetFiltering(filtering, filtering);
		_textureCache.AddValue(store, path, texture);

		return texture;
	}
}
