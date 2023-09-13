using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;

namespace Azalea.Extentions.ImageExtentions;

internal readonly ref struct ReadOnlyPixelSpan<TPixel>
	where TPixel : unmanaged, IPixel<TPixel>
{
	public readonly ReadOnlySpan<TPixel> Span;

	private readonly IMemoryOwner<TPixel>? _owner;

	internal ReadOnlyPixelSpan(Image<TPixel> image)
	{
		if (image.DangerousTryGetSinglePixelMemory(out var memory))
		{
			_owner = null;
			Span = memory.Span;
		}
		else
		{
			_owner = image.CreateContiguousMemory();
			Span = _owner.Memory.Span;
		}
	}

	public void Dispose()
	{
		_owner?.Dispose();
	}
}
