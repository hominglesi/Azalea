using Azalea.Graphics.Primitives;
using System;

namespace Azalea.Numerics;

public struct RectangleInt : IEquatable<RectangleInt>
{
	public static RectangleInt Empty { get; } = new RectangleInt();

	public int X;
	public int Y;

	public int Width;
	public int Height;

	public RectangleInt(int x, int y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public RectangleInt(Vector2Int position, Vector2Int size)
		: this(position.X, position.Y, size.X, size.Y) { }

	public readonly int Left => X;
	public readonly int Top => Y;
	public readonly int Right => X + Width;
	public readonly int Bottom => Y + Height;

	public readonly Vector2Int TopLeft => new(Left, Top);
	public readonly Vector2Int TopRight => new(Right, Top);
	public readonly Vector2Int BottomLeft => new(Left, Bottom);
	public readonly Vector2Int BottomRight => new(Right, Bottom);

	public readonly Vector2Int Size => new(Width, Height);

	public readonly RectangleInt Offset(Vector2Int pos) => Offset(pos.X, pos.Y);
	public readonly RectangleInt Offset(int x, int y) => new(X + x, Y + y, Width, Height);

	public readonly bool Contains(Vector2Int point)
		=> X <= point.X && point.X < X + Width && Y <= point.Y && point.Y < Y + Height;

	public readonly bool Equals(RectangleInt other)
		=> X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
	public override readonly bool Equals(object? obj)
		=> obj is not null && obj is RectangleInt rObj && Equals(rObj);
	public static bool operator ==(RectangleInt left, RectangleInt right) => left.Equals(right);
	public static bool operator !=(RectangleInt left, RectangleInt right) => !left.Equals(right);

	public static explicit operator RectangleInt(Rectangle rect)
		=> new((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));

	public static explicit operator RectangleInt(Quad quad)
		=> new((int)Math.Round(quad.TopLeft.X), (int)Math.Round(quad.TopLeft.Y),
			(int)Math.Round(quad.BottomRight.X - quad.TopLeft.X), (int)Math.Round(quad.BottomRight.Y - quad.TopLeft.Y));
	public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
}
