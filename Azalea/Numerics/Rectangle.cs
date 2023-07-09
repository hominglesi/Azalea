using System;
using System.Numerics;

namespace Azalea.Numerics;

public struct Rectangle : IEquatable<Rectangle>
{
    public static Rectangle Empty { get; } = new Rectangle();

    public float X;
    public float Y;

    public float Width;
    public float Height;

    public Rectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public readonly float Left => X;
    public readonly float Top => Y;
    public readonly float Right => X + Width;
    public readonly float Bottom => Y + Height;

    public readonly Vector2 TopLeft => new(Left, Top);
    public readonly Vector2 TopRight => new(Right, Top);
    public readonly Vector2 BottomLeft => new(Left, Bottom);
    public readonly Vector2 BottomRight => new(Right, Bottom);

    public readonly bool Equals(Rectangle other)
        => X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    public override readonly bool Equals(object? obj)
        => obj is not null && obj is Rectangle rObj && Equals(rObj);
    public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);
    public static bool operator !=(Rectangle left, Rectangle right) => !left.Equals(right);
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
}
