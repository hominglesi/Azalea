using System.Numerics;
using System.Runtime.InteropServices;

namespace Azalea.Graphics.Rendering.Vertices;

[StructLayout(LayoutKind.Sequential)]
internal struct PositionColorVertex : IVertex
{
	public Vector2 Position;

	public Color Color;
}
