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

	public static Vector2Int Zero => new(0, 0);
	public static Vector2Int One => new(1, 1);

	public static bool operator ==(Vector2Int left, Vector2Int right) => left.Equals(right);
	public static bool operator !=(Vector2Int left, Vector2Int right) => !left.Equals(right);

	public static Vector2Int operator +(Vector2Int left, Vector2Int right) => new(left.X + right.X, left.Y + right.Y);
	public static Vector2Int operator -(Vector2Int left, Vector2Int right) => new(left.X - right.X, left.Y - right.Y);
	public static Vector2Int operator *(Vector2Int left, Vector2Int right) => new(left.X * right.X, left.Y * right.Y);
	public static Vector2Int operator *(Vector2Int left, int right) => new(left.X * right, left.Y * right);
	public static Vector2 operator *(Vector2Int left, float right) => new(left.X * right, left.Y * right);
	public static Vector2Int operator /(Vector2Int left, Vector2Int right) => new(left.X / right.X, left.Y / right.Y);
	public static Vector2Int operator /(Vector2Int left, int right) => new(left.X / right, left.Y / right);

	public static implicit operator Vector2(Vector2Int other) => new(other.X, other.Y);

	public static Vector2Int Parse(string value)
	{
		var args = value.Split(':');
		var x = int.Parse(args[0]);
		var y = int.Parse(args[1]);
		return new Vector2Int(x, y);
	}
}
