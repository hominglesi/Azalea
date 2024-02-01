using System;
using System.Numerics;

namespace Azalea.Numerics;

public struct Rectangle : IEquatable<Rectangle>
{
	public static Rectangle Empty { get; } = new Rectangle();
	public static Rectangle One { get; } = new Rectangle(0, 0, 1, 1);

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

	public Rectangle(Vector2 position, Vector2 size)
		: this(position.X, position.Y, size.X, size.Y) { }

	public readonly float Left => X;
	public readonly float Top => Y;
	public readonly float Right => X + Width;
	public readonly float Bottom => Y + Height;

	public readonly Vector2 TopLeft => new(Left, Top);
	public readonly Vector2 TopRight => new(Right, Top);
	public readonly Vector2 BottomLeft => new(Left, Bottom);
	public readonly Vector2 BottomRight => new(Right, Bottom);

	public Vector2 Position
	{
		readonly get => new(X, Y);
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	public Vector2 Size
	{
		readonly get => new(Width, Height);
		set
		{
			Width = value.X;
			Height = value.Y;
		}
	}

	public readonly Rectangle Offset(Vector2 pos) => Offset(pos.X, pos.Y);
	public readonly Rectangle Offset(float x, float y) => new Rectangle(X + x, Y + y, Width, Height);

	public readonly bool Contains(Vector2 point)
		=> X <= point.X && point.X < X + Width && Y <= point.Y && point.Y < Y + Height;

	public readonly bool Intersects(Rectangle other)
	{
		if (Left < other.Right && Right > other.Left && Top < other.Bottom)
			return Bottom > other.Top;

		return false;
	}

	public readonly bool Equals(Rectangle other)
		=> X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
	public override readonly bool Equals(object? obj)
		=> obj is not null && obj is Rectangle rObj && Equals(rObj);
	public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);
	public static bool operator !=(Rectangle left, Rectangle right) => !left.Equals(right);
	public override readonly int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
}
