using System;
using System.Numerics;

namespace Azalea.Utils;

public static class MathUtils
{
	public static float DegreesToRadians(float degress)
	{
		return degress * MathF.PI / 180;
	}

	public static float RadiansToDegrees(float radians)
	{
		return radians * (180 / MathF.PI);
	}

	public static bool IsPowerOfTwo(int value)
	{
		return (value & (value - 1)) == 0;
	}

	public static float Map(float value, float from1, float to1, float from2, float to2)
	{
		if (from1 == to1) return from2;
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public static int Ceiling(float value)
		=> (int)MathF.Ceiling(value);

	public static float GetAngleTowards(Vector2 startPositon, Vector2 endPositon)
	{
		var yDifference = endPositon.Y - startPositon.Y;
		var xDifference = endPositon.X - startPositon.X;
		return (float)Math.Atan2(yDifference, xDifference);
	}

	public static Vector2 GetDirectionTowards(Vector2 startPositon, Vector2 endPositon)
		=> GetDirectionFromAngle(GetAngleTowards(startPositon, endPositon));

	public static Vector2 GetDirectionFromAngle(float angle)
		=> new((float)Math.Cos(angle), (float)Math.Sin(angle));

	public static float GetAngleFromDirection(Vector2 direction)
		=> MathF.Atan2(direction.Y, direction.X);

	public static float DistanceBetween(Vector2 startPositon, Vector2 endPositon)
		=> MathF.Sqrt(MathF.Pow(endPositon.X - startPositon.X, 2) + MathF.Pow(endPositon.Y - startPositon.Y, 2));
}
