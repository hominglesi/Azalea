using System;

namespace Azalea.Graphics;

public struct MarginPadding : IEquatable<MarginPadding>
{
    public float Top;
    public float Left;
    public float Bottom;
    public float Right;

    public MarginPadding(float top, float right, float bottom, float left)
    {
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public readonly float Horizontal => Left + Right;
    public readonly float Vertical => Top + Bottom;

    public readonly bool Equals(MarginPadding other) => Top == other.Top && Left == other.Left && Bottom == other.Bottom && Right == other.Right;
    public override readonly string ToString() => $@"({Top}, {Left}, {Bottom}, {Right})";
}
