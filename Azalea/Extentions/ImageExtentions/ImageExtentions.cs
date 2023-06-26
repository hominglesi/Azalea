using SixLabors.ImageSharp.Advanced;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Extentions.ImageExtentions;

public static class ImageExtentions
{
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
