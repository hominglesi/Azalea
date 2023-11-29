using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.IO.Resources;
public static class ResourceStore_TextureData
{
	private static ResourceCache<TextureData> _textureDataCache = new();

	public static TextureData GetTextureData(this IResourceStore store, string path)
	{
		if (_textureDataCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path)
			?? throw new Exception("Texture could not be found.");

		var texture = new TextureData(stream);
		_textureDataCache.AddValue(store, path, texture);

		return texture;
	}

	private static IRenderer Renderer => AzaleaGame.Main.Host.Renderer;
}
