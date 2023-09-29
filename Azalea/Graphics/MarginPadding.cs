using System;

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

	public readonly bool Equals(Boundary other) => Top == other.Top && Left == other.Left && Bottom == other.Bottom && Right == other.Right;
	public override readonly string ToString() => $@"({Top}, {Left}, {Bottom}, {Right})";
}
