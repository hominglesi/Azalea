using System;

namespace Azalea.Utils;
public static class BitwiseUtils
{
	public static short GetLowOrderValue(IntPtr value) => (short)value;
	public static short GetHighOrderValue(IntPtr value) => (short)((uint)value >> 16);
	public static Vector2Int SplitValue(IntPtr value)
		=> new(GetLowOrderValue(value), GetHighOrderValue(value));
}
