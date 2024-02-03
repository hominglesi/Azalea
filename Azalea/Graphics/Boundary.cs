using System;
using System.Numerics;

namespace Azalea.Graphics;

public struct Boundary : IEquatable<Boundary>
{
	public float Top;
	public float Left;
	public float Bottom;
	public float Right;

	public Boundary(float top, float right, float bottom, float left)
	{
		Top = top;
		Right = right;
		Bottom = bottom;
		Left = left;
	}

	public Boundary(float padding)
		: this(padding, padding, padding, padding) { }

	public readonly float Horizontal => Left + Right;
	public readonly float Vertical => Top + Bottom;
	public readonly Vector2 Total => new(Horizontal, Vertical);

	public static readonly Boundary Zero = new(0);

	public readonly bool Equals(Boundary other) => Top == other.Top && Left == other.Left && Bottom == other.Bottom && Right == other.Right;
	public readonly override bool Equals(object? obj) => obj is Boundary b && Equals(b);
	public override readonly string ToString() => $@"({Top}, {Right}, {Bottom}, {Left})";
	public override readonly int GetHashCode() => HashCode.Combine(Top, Right, Bottom, Left);

	public static bool operator ==(Boundary left, Boundary right) => left.Equals(right);
	public static bool operator !=(Boundary left, Boundary right) => left.Equals(right) == false;

	public static Boundary operator +(Boundary left, Boundary right)
		=> new(left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom, left.Left + right.Left);
}
