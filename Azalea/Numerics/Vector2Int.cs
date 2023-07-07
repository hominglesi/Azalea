using Silk.NET.Maths;
using System;
using System.Numerics;

namespace Azalea;

public struct Vector2Int : IEquatable<Vector2Int>
{
    public int X;
    public int Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2Int(int value)
        : this(value, value) { }

    public readonly bool Equals(Vector2Int other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override readonly bool Equals(object? obj)
        => obj is not null && obj is Vector2Int oInt && Equals(oInt);
    public override readonly int GetHashCode() => HashCode.Combine(X, Y);
    public override readonly string ToString() => $"{X}, {Y}";

    public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);
    public static bool operator !=(Vector2Int left, Vector2Int right) => !left.Equals(right);

    public static implicit operator Vector2Int(Vector2D<int> other) => new(other.X, other.Y);
    public static implicit operator Vector2D<int>(Vector2Int other) => new(other.X, other.Y);

    public static implicit operator Vector2(Vector2Int other) => new(other.X, other.Y);
}
