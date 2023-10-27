using Azalea.IO.Stores;
using StbImageSharp;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Graphics.Textures;

public class TextureLoaderStore : IResourceStore<TextureData>
{
	private readonly ResourceStore<byte[]> _store;

	public TextureLoaderStore(IResourceStore<byte[]> store)
	{
		_store = new ResourceStore<byte[]>(store);
	}

	public TextureData? Get(string name)
	{
		try
		{
			using var stream = _store.GetStream(name);

			if (stream is not null)
				return new TextureData(ImageFromStream(stream));
		}
		catch { }

		return null;
	}

	public Stream? GetStream(string name) => _store.GetStream(name);

	protected virtual ImageResult ImageFromStream(Stream stream)
		=> TextureData.LoadFromStream(stream);

	public IEnumerable<string> GetAvalibleResources() => _store.GetAvalibleResources();
}
