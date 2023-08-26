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
}
