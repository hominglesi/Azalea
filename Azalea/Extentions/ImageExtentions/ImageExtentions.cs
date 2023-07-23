using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System.Buffers;

namespace Azalea.Extentions.ImageExtentions;

public static class ImageExtentions
{
    internal static ReadOnlyPixelSpan<TPixel> CreateReadOnlyPixelSpan<TPixel>(this Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
        => new(image);

    internal static ReadOnlyPixelMemory<TPixel> CreateReadOnlyPixelMemory<TPixel>(this Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
        => new(image);

    internal static IMemoryOwner<TPixel> CreateContiguousMemory<TPixel>(this Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var allocationOwner = SixLabors.ImageSharp.Configuration.Default.MemoryAllocator.Allocate<TPixel>(image.Width * image.Height);
        var allocatedMemory = allocationOwner.Memory;

        for (int r = 0; r < image.Height; r++)
            image.DangerousGetPixelRowMemory(r).CopyTo(allocatedMemory[(r * image.Width)..]);

        return allocationOwner;
    }
}
