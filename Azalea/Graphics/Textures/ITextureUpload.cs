using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Azalea.Graphics.Textures;

internal interface ITextureUpload : IDisposable
{
    ReadOnlySpan<Rgba32> Data { get; }

}
