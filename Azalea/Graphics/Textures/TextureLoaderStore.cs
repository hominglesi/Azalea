using Azalea.IO.Stores;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Graphics.Textures;

public class TextureLoaderStore : IResourceStore<TextureUpload>
{
    private readonly ResourceStore<byte[]> _store;

    public TextureLoaderStore(IResourceStore<byte[]> store)
    {
        _store = new ResourceStore<byte[]>(store);
    }

    public TextureUpload? Get(string name)
    {
        try
        {
            using var stream = _store.GetStream(name);

            if (stream is not null)
                return new TextureUpload(ImageFromStream<Rgba32>(stream));
        }
        catch { }

        return null;
    }

    public Stream? GetStream(string name) => _store.GetStream(name);

    protected virtual Image<TPixel> ImageFromStream<TPixel>(Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
        => TextureUpload.LoadFromStream<TPixel>(stream);

    public IEnumerable<string> GetAvalibleResources() => _store.GetAvalibleResources();
}
