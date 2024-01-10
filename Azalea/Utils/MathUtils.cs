using System;

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
}
