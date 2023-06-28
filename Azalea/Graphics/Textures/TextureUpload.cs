using Azalea.Extentions.ImageExtentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.Textures;

internal class TextureUpload : ITextureUpload
{
    public ReadOnlySpan<Rgba32> Data => _pixelMemory.Span;

    public int Width => _image?.Width ?? 0;
    public int Height => _image?.Height ?? 0;

    private readonly Image<Rgba32> _image;
    private ReadOnlyPixelMemory<Rgba32> _pixelMemory;

    public TextureUpload(Image<Rgba32> image)
    {
        _image = image;
        _pixelMemory = _image.CreateReadOnlyPixelMemory();
    }

    public TextureUpload(Stream stream)
        : this(LoadFromStream<Rgba32>(stream))
    {

    }

    internal static Image<TPixel> LoadFromStream<TPixel>(Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return Image.Load<TPixel>(stream);
    }

    private bool disposed;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (disposed) return;

        _image?.Dispose();
        _pixelMemory.Dispose();

        disposed = true;
    }
}
