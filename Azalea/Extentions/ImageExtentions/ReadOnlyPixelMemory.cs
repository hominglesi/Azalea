using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;
using System.Diagnostics;

namespace Azalea.Extentions.ImageExtentions;

internal struct ReadOnlyPixelMemory<TPixel> : IDisposable
    where TPixel : unmanaged, IPixel<TPixel>
{
    private Image<TPixel>? _image;
    private Memory<TPixel>? _memory;
    private IMemoryOwner<TPixel>? _owner;

    internal ReadOnlyPixelMemory(Image<TPixel> image)
    {
        _image = image;
        if (_image.DangerousTryGetSinglePixelMemory(out _))
        {
            _owner = null;
            _memory = null;
        }
        else
        {
            _owner = _image.CreateContiguousMemory();
            _memory = _owner.Memory;
        }
    }

    public ReadOnlySpan<TPixel> Span
    {
        get
        {
            if (_image == null)
                return Span<TPixel>.Empty;

            if (_image.DangerousTryGetSinglePixelMemory(out var pixelMemory))
                return pixelMemory.Span;

            Debug.Assert(_memory != null);
            return _memory.Value.Span;
        }
    }

    public void Dispose()
    {
        _owner?.Dispose();
        _image = null;
        _memory = null;
        _owner = null;
    }
}
