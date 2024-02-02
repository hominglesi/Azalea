using Azalea.Numerics;
using System;
using System.Numerics;

namespace Azalea.Extentions;

public static class Vector2Extentions
{
	public static Vector2 Parse(string value)
	{
		var args = value.Split(':');
		var x = float.Parse(args[0]);
		var y = float.Parse(args[1]);
		return new Vector2(x, y);
	}

	public static Vector2 ComponentMax(Vector2 a, Vector2 b)
	{
		a.X = a.X > b.X ? a.X : b.X;
		a.Y = a.Y > b.Y ? a.Y : b.Y;
		return a;
	}

	public static Vector2 Transform(Vector2 pos, Matrix3 mat)
	{
		Transform(ref pos, ref mat, out Vector2 result);
		return result;
	}

	public static void Transform(ref Vector2 pos, ref Matrix3 mat, out Vector2 result)
	{
		result.X = mat.Row0.X * pos.X + mat.Row1.X * pos.Y + mat.Row2.X;
		result.Y = mat.Row0.Y * pos.X + mat.Row1.Y * pos.Y + mat.Row2.Y;
	}

	public static float Distance(Vector2 vec1, Vector2 vec2)
	{
		Distance(ref vec1, ref vec2, out float result);
		return result;
	}

	public static void Distance(ref Vector2 vec1, ref Vector2 vec2, out float result)
	{
		result = MathF.Sqrt((vec2.X - vec1.X) * (vec2.X - vec1.X) + (vec2.Y - vec1.Y) * (vec2.Y - vec1.Y));
	}

	public static float PerpDot(Vector2 left, Vector2 right)
		=> left.X * right.Y - left.Y * right.X;

	public static float Cross(Vector2 left, Vector2 right)
		=> left.X * right.Y - left.Y * right.X;

	public static Vector2 Cross(Vector2 vector, float s)
		=> new(s * vector.Y, -s * vector.X);

	public static Vector2 Cross(float s, Vector2 vector)
		=> new(-s * vector.Y, s * vector.X);

	public static Vector2 Rotate(this Vector2 vector, float angle, bool isDegrees = true)
	{
		if (isDegrees)
			angle = (angle / 180) * MathF.PI;

		return new Vector2(MathF.Cos(angle) * vector.X - MathF.Sin(angle) * vector.Y,
						   MathF.Sin(angle) * vector.X + MathF.Cos(angle) * vector.Y);
	}
}
