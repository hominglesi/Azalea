using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.IO.Resources;
public static partial class ResourceStoreExtentions
{
	private static ResourceCache<Texture> _textureCache = new();

	public static Texture GetTexture(this IResourceStore store, string path)
	{
		if (_textureCache.TryGetValue(store, path, out var cached))
			return cached;

		using var stream = store.GetStream(path);

		if (stream == null)
			return Assets.MissingTexture ?? throw new Exception("Texture could not be found.");

		var data = store.GetTextureData(path);
		var texture = createTexture(data);
		_textureCache.AddValue(store, path, texture);

		return texture;
	}

	public static Texture CreateTexture(this IResourceStore store, TextureData data)
		=> createTexture(data);

	private static Texture createTexture(TextureData data)
	{
		var texture = Renderer.CreateTexture(data.Width, data.Height);
		texture.SetData(data);

		return texture;
	}

	private static IRenderer Renderer => AzaleaGame.Main.Host.Renderer;
}
