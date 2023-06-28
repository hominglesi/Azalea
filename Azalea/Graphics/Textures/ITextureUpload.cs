using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.Textures;

internal interface ITextureUpload : IDisposable
{
    ReadOnlySpan<Rgba32> Data { get; }

}
