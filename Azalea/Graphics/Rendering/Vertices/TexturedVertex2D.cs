using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.Rendering.Vertices;

[StructLayout(LayoutKind.Sequential)]
internal struct TexturedVertex2D : IVertex
{
    public Vector2 Position;

    public Color Color;

    public Vector2 TexturePosition;
}
