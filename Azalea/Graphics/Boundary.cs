using System;
using System.Numerics;

namespace Azalea.Graphics;

public struct Boundary(float top = 0, float right = 0, float bottom = 0, float left = 0) : IEquatable<Boundary>
{
	public float Top = top;
	public float Left = left;
	public float Bottom = bottom;
	public float Right = right;

	public Boundary(float padding)
		: this(padding, padding, padding, padding) { }

	public readonly float Horizontal => Left + Right;
	public readonly float Vertical => Top + Bottom;
	public readonly Vector2 Total => new(Horizontal, Vertical);

	public static readonly Boundary Zero = new(0);

	public readonly Vector2 ClampWithin(Vector2 vector)
		=> new(Math.Clamp(vector.X, Left, Right), Math.Clamp(vector.Y, Top, Bottom));

	public readonly bool Equals(Boundary other) => Top == other.Top && Left == other.Left && Bottom == other.Bottom && Right == other.Right;
	public readonly override bool Equals(object? obj) => obj is Boundary b && Equals(b);
	public override readonly string ToString() => $@"({Top}, {Right}, {Bottom}, {Left})";
	public override readonly int GetHashCode() => HashCode.Combine(Top, Right, Bottom, Left);

	public static bool operator ==(Boundary left, Boundary right) => left.Equals(right);
	public static bool operator !=(Boundary left, Boundary right) => left.Equals(right) == false;

	public static implicit operator Boundary(float value) => new(value);

	public static Boundary operator +(Boundary left, Boundary right)
		=> new(left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom, left.Left + right.Left);
}
