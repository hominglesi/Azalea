using System;

namespace Azalea.Graphics;

public struct MarginPadding : IEquatable<MarginPadding>
{
    public float Top;
    public float Left;
    public float Bottom;
    public float Right;

    public readonly bool Equals(MarginPadding other) => Top == other.Top && Left == other.Left && Bottom == other.Bottom && Right == other.Right;
    public override readonly string ToString() => $@"({Top}, {Left}, {Bottom}, {Right})";
}
