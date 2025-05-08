using System;

namespace Azalea.Utils;
public static class BitwiseUtils
{
	public static short GetLowOrderValue(IntPtr value) => (short)(long)value;
	public static short GetHighOrderValue(IntPtr value) => (short)((long)value >> 16);
	public static Vector2Int SplitValue(IntPtr value)
		=> new(GetLowOrderValue(value), GetHighOrderValue(value));

	public static bool GetSpecificBit(IntPtr value, int bitIndex) => ((int)value & (1 << bitIndex - 1)) != 0;
	public static bool GetSpecificBit(int value, int bitIndex) => (value & (1 << bitIndex - 1)) != 0;
}
